using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Arak.DAL.Database;

namespace Arak.BLL.Service.Implementation
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<AttendanceRecord> _attendanceRepository;
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly AppDbContext _context;

        public AttendanceService(
            IGenericRepository<AttendanceRecord> attendanceRepository,
            IGenericRepository<Student> studentRepository,
            AppDbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
            _context = context;
        }

        public Task<AttendanceDto> MarkAttendanceAsync(MarkAttendanceDto dto) => throw new NotImplementedException();
        public async Task<int> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto, int teacherId)
        {
            // Direct filtered query to avoid loading all records
            var todayRecords = await _context.AttendanceRecords
                .Where(a => a.Date == dto.Date && a.ClassId == dto.ClassId)
                .ToDictionaryAsync(a => a.StudentId);

            int savedCount = 0;

            foreach (var record in dto.Records)
            {
                if (todayRecords.TryGetValue(record.StudentId, out var existing))
                {
                    // UPDATE existing logic
                    existing.Status = record.Status;
                    existing.TimeIn = record.TimeIn;
                    existing.Session = dto.Session; 
                    existing.TeacherId = teacherId;
                    existing.UpdatedAt = DateTime.UtcNow;

                    await _attendanceRepository.UpdateAsync(existing);
                }
                else
                {
                    // CREATE new logic
                    var newAttendance = new AttendanceRecord
                    {
                        StudentId = record.StudentId,
                        ClassId = dto.ClassId,
                        TeacherId = teacherId,
                        Date = dto.Date,
                        Session = dto.Session,
                        Status = record.Status,
                        TimeIn = record.TimeIn,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _attendanceRepository.CreateAsync(newAttendance);
                }
                savedCount++;
            }

            await _attendanceRepository.SaveChangesAsync();
            return savedCount;
        }
        public async Task<ClassAttendanceResponseDto> GetClassAttendanceAsync(int classId, DateOnly date)
        {
            // 1. Get all students actively enrolled in this class
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            // 2. Get existing attendance records for the scope
            var recordsCache = await _context.AttendanceRecords
                .Where(a => a.ClassId == classId && a.Date == date)
                .ToDictionaryAsync(a => a.StudentId);

            // 3. Map (Left Join)
            var result = new ClassAttendanceResponseDto
            {
                ClassId = classId,
                Date = date,
                Students = students.Select(s =>
                {
                    var hasRecord = recordsCache.TryGetValue(s.Id, out var record);
                    return new StudentAttendanceItemDto
                    {
                        StudentId = s.Id,
                        StudentName = s.Name,
                        Status = hasRecord ? record!.Status : "NotRecorded",
                        TimeIn = hasRecord ? record!.TimeIn : null,
                        TimeOut = hasRecord ? record!.TimeOut : null
                    };
                }).ToList()
            };

            return result;
        }
        public async Task<AttendanceSummaryDto> GetClassSummaryAsync(int classId, DateOnly date)
        {
            var totalStudents = await _context.Students.CountAsync(s => s.ClassId == classId);
            var records = await _context.AttendanceRecords
                .Where(a => a.ClassId == classId && a.Date == date)
                .ToListAsync();

            var present = records.Count(r => r.Status == "Present");
            var absent = records.Count(r => r.Status == "Absent");
            var late = records.Count(r => r.Status == "Late");
            var notRecorded = totalStudents - records.Count;

            return new AttendanceSummaryDto
            {
                TotalStudents = totalStudents,
                PresentCount = present,
                PresentRate = totalStudents > 0 ? (present / (double)totalStudents) * 100 : 0,
                AbsentCount = absent,
                AbsentRate = totalStudents > 0 ? (absent / (double)totalStudents) * 100 : 0,
                LateCount = late,
                LateRate = totalStudents > 0 ? (late / (double)totalStudents) * 100 : 0,
                NotRecordedCount = notRecorded
            };
        }

        public async Task<StudentAttendanceDetailDto> GetStudentAttendanceDetailsAsync(int studentId, int month, int year, string userId, string role)
        {
            // Authorization Check for Parents
            if (role == "Parent")
            {
                // We assume ApplicationUserId is a shadow property or accessible via EF.Property
                var parent = await _context.Parents.Include(p => p.Students)
                    .FirstOrDefaultAsync(p => EF.Property<string>(p, "ApplicationUserId") == userId);
                
                if (parent == null || !parent.Students.Any(s => s.Id == studentId))
                    throw new UnauthorizedAccessException("You can only view attendance for your linked children.");
            }

            var student = await _context.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) throw new KeyNotFoundException("Student not found.");

            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var records = await _context.AttendanceRecords
                .Where(a => a.StudentId == studentId && a.Date >= startDate && a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var todayRecord = records.FirstOrDefault(r => r.Date == today);

            // Stats calculations
            int totalDays = records.Count;
            int present = records.Count(r => r.Status == "Present" || r.Status == "Late");
            int late = records.Count(r => r.Status == "Late");
            int absent = records.Count(r => r.Status == "Absent");

            return new StudentAttendanceDetailDto
            {
                StudentName = student.Name,
                Grade = student.Grade,
                ClassName = student.Class?.Name ?? "N/A",
                TodayStatus = todayRecord?.Status ?? "NotRecorded",
                TodayTimeIn = todayRecord?.TimeIn,
                TodayTimeOut = todayRecord?.TimeOut,
                AttendanceRate = totalDays > 0 ? (present / (double)totalDays) * 100 : 0,
                LateArrivals = late,
                Absences = absent,
                Records = records.Select(r => new AttendanceRecordItemDto
                {
                    Date = r.Date,
                    Status = r.Status,
                    TimeIn = r.TimeIn,
                    TimeOut = r.TimeOut
                }).ToList()
            };
        }

        public async Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto)
        {
            var record = await _attendanceRepository.GetByIdAsync(id);
            if (record == null) throw new KeyNotFoundException("Attendance record not found.");

            record.Status = dto.Status;
            record.TimeIn = dto.TimeIn;
            record.TimeOut = dto.TimeOut;
            record.UpdatedAt = DateTime.UtcNow;

            await _attendanceRepository.UpdateAsync(record);
            await _attendanceRepository.SaveChangesAsync();

            return new AttendanceDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                Date = record.Date,
                Status = record.Status,
                TimeIn = record.TimeIn.HasValue ? record.TimeIn.Value.ToTimeSpan() : null,
                TimeOut = record.TimeOut.HasValue ? record.TimeOut.Value.ToTimeSpan() : null
            };
        }
        public async Task<int> BulkUpdateTimeoutAsync(BulkTimeoutDto dto, int teacherId)
        {
            // Direct filtered query 
            var todayRecords = await _context.AttendanceRecords
                .Where(a => a.Date == dto.Date && a.ClassId == dto.ClassId)
                .ToDictionaryAsync(a => a.StudentId);

            int updatedCount = 0;

            foreach (var record in dto.Records)
            {
                // Skip silently if the record doesn't exist 
                if (todayRecords.TryGetValue(record.StudentId, out var existing))
                {
                    existing.TimeOut = record.TimeOut;
                    existing.Session = "Afternoon";
                    existing.TeacherId = teacherId; // Update trailing record of who signed them out
                    existing.UpdatedAt = DateTime.UtcNow;

                    await _attendanceRepository.UpdateAsync(existing);
                    updatedCount++;
                }
            }

            await _attendanceRepository.SaveChangesAsync();
            return updatedCount;
        }
    }
}
