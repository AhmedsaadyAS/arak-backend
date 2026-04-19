using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<Attendance> _attendanceRepository;
        private readonly IGenericRepository<Student> _studentRepository;

        public AttendanceService(
            IGenericRepository<Attendance> attendanceRepository,
            IGenericRepository<Student> studentRepository)
        {
            _attendanceRepository = attendanceRepository;
            _studentRepository = studentRepository;
        }

        public async Task<AttendanceDto> MarkAttendanceAsync(MarkAttendanceDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null) throw new KeyNotFoundException($"Student with ID {dto.StudentId} not found");

            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                Date = dto.Date,
                TimeIn = dto.TimeIn,
                TimeOut = dto.TimeOut,
                Status = dto.Status,
                Notes = dto.Notes
            };

            var created = await _attendanceRepository.CreateAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();

            return MapToDto(created, student.Name);
        }

        public async Task<IEnumerable<AttendanceDto>> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto)
        {
            var results = new List<AttendanceDto>();
            var allStudents = await _studentRepository.GetAllAsync();
            var studentIds = dto.Records.Select(r => r.StudentId).ToList();
            var studentDict = allStudents
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s.Name);

            foreach (var record in dto.Records)
            {
                var attendance = new Attendance
                {
                    StudentId = record.StudentId,
                    Date = dto.Date,
                    TimeIn = record.TimeIn,
                    TimeOut = record.TimeOut,
                    Status = record.Status,
                    Notes = record.Notes
                };

                var created = await _attendanceRepository.CreateAsync(attendance);
                results.Add(MapToDto(created, studentDict.GetValueOrDefault(record.StudentId, "Unknown")));
            }

            await _attendanceRepository.SaveChangesAsync();
            return results;
        }

        public async Task<PagedResult<AttendanceDto>> GetClassAttendanceByDateAsync(int classId, DateOnly date, int page, int pageSize)
        {
            // 1. Get all students in the class
            var allStudents = await _studentRepository.GetAllAsync();
            var classStudents = allStudents.Where(s => s.ClassId == classId).ToList();

            // 2. Get all attendance records for these students on this date
            var studentIds = classStudents.Select(s => s.Id).ToList();
            var allAttendance = await _attendanceRepository.GetAllAsync();
            var dateAttendance = allAttendance
                .Where(a => a.Date == date && studentIds.Contains(a.StudentId))
                .ToDictionary(a => a.StudentId);

            // 3. Map to DTOs, including "NotMarked" for students without records
            var attendanceDtos = classStudents.Select(s => 
            {
                if (dateAttendance.TryGetValue(s.Id, out var att))
                {
                    return MapToDto(att, s.Name);
                }
                
                return new AttendanceDto
                {
                    StudentId = s.Id,
                    StudentName = s.Name,
                    Date = date,
                    Status = "NotMarked"
                };
            }).ToList();

            // 4. Paginate
            var total = attendanceDtos.Count;
            var pagedData = attendanceDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<AttendanceDto>
            {
                Data = pagedData,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<AttendanceDto>> GetStudentAttendanceByMonthAsync(int studentId, int month, int year)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return Enumerable.Empty<AttendanceDto>();

            var allAttendance = await _attendanceRepository.GetAllAsync();
            return allAttendance
                .Where(a => a.StudentId == studentId && a.Date.Month == month && a.Date.Year == year)
                .OrderBy(a => a.Date)
                .Select(a => MapToDto(a, student.Name))
                .ToList();
        }

        public async Task<StudentAttendanceStatsDto> GetStudentStatsAsync(int studentId)
        {
            var allAttendance = await _attendanceRepository.GetAllAsync();
            var studentAttendance = allAttendance.Where(a => a.StudentId == studentId).ToList();

            if (!studentAttendance.Any())
            {
                return new StudentAttendanceStatsDto { TotalDays = 0 };
            }

            int presentCount = studentAttendance.Count(a => a.Status == AttendanceStatus.Present);
            int lateCount = studentAttendance.Count(a => a.Status == AttendanceStatus.Late);
            int absenceCount = studentAttendance.Count(a => a.Status == AttendanceStatus.Absent);
            int totalDays = studentAttendance.Count;

            double rate = (double)(presentCount + lateCount) / totalDays * 100;

            return new StudentAttendanceStatsDto
            {
                AttendanceRate = Math.Round(rate, 1),
                PresentCount = presentCount,
                LateCount = lateCount,
                AbsenceCount = absenceCount,
                TotalDays = totalDays
            };
        }

        public async Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null) throw new KeyNotFoundException($"Attendance record with ID {id} not found");

            var student = await _studentRepository.GetByIdAsync(attendance.StudentId);

            attendance.TimeIn = dto.TimeIn;
            attendance.TimeOut = dto.TimeOut;
            attendance.Status = dto.Status;
            attendance.Notes = dto.Notes;

            await _attendanceRepository.UpdateAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();

            return MapToDto(attendance, student?.Name ?? "Unknown");
        }

        public async Task<int> BulkUpdateTimeOutAsync(BulkUpdateTimeOutDto dto)
        {
            // 1. Get all students in the class
            var allStudents = await _studentRepository.GetAllAsync();
            var classStudentIds = allStudents
                .Where(s => s.ClassId == dto.ClassId)
                .Select(s => s.Id)
                .ToList();

            // 2. Get existing attendance records for these students on this date
            var allAttendance = await _attendanceRepository.GetAllAsync();
            var existingRecords = allAttendance
                .Where(a => a.Date == dto.Date && classStudentIds.Contains(a.StudentId))
                .ToDictionary(a => a.StudentId);

            int updatedCount = 0;

            // 3. Update TimeOut for matching records
            foreach (var record in dto.Records)
            {
                if (existingRecords.TryGetValue(record.StudentId, out var attendance))
                {
                    attendance.TimeOut = record.TimeOut;
                    await _attendanceRepository.UpdateAsync(attendance);
                    updatedCount++;
                }
            }

            // 4. Save all changes
            if (updatedCount > 0)
            {
                await _attendanceRepository.SaveChangesAsync();
            }

            return updatedCount;
        }

        private static AttendanceDto MapToDto(Attendance entity, string studentName)
        {
            return new AttendanceDto
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = studentName,
                Date = entity.Date,
                TimeIn = entity.TimeIn,
                TimeOut = entity.TimeOut,
                Status = entity.Status.ToString(),
                Notes = entity.Notes
            };
        }
    }
}
