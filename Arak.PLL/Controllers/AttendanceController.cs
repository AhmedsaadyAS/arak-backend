using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Arak.DAL.Database;
using Arak.DAL.Entities;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly AppDbContext _context;

        public AttendanceController(IAttendanceService attendanceService, AppDbContext context)
        {
            _attendanceService = attendanceService;
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
        {
            try
            {
                // 1. Get Logged-in User ID string
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("User claim not found.");

                // 2. Fetch linked Teacher Profile (if exists)
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => EF.Property<string>(t, "ApplicationUserId") == userId);

                int? teacherId = teacher?.TeacherId;

                // 3. Mark Attendance
                var result = await _attendanceService.MarkAttendanceAsync(dto, teacherId);
                return CreatedAtAction(nameof(MarkAttendance), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Super Admin,Admin,Teacher")]
        public async Task<IActionResult> BulkMarkAttendance([FromBody] BulkMarkAttendanceDto dto)
        {
            try
            {
                // 1. Get Logged-in User ID string
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("User claim not found.");

                // 2. Fetch linked Teacher Profile (if exists)
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => EF.Property<string>(t, "ApplicationUserId") == userId);

                int? teacherId = teacher?.TeacherId;

                // 3. Upsert Records
                var count = await _attendanceService.BulkMarkAttendanceAsync(dto, teacherId);
                
                return Ok(new { message = "Attendance saved successfully.", updatedCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("class/{classId:int}")]
        [Authorize(Roles = "Super Admin,Admin,Teacher")]
        public async Task<IActionResult> GetClassAttendanceByDate(
            int classId, 
            [FromQuery] DateOnly? date = null)
        {
            try
            {
                // Default to today if date isn't expressly passed
                var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
                
                var result = await _attendanceService.GetClassAttendanceAsync(classId, targetDate);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("summary/{classId:int}")]
        [Authorize(Roles = "Super Admin,Admin,Teacher")]
        public async Task<IActionResult> GetClassSummary(int classId, [FromQuery] DateOnly? date = null)
        {
            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var result = await _attendanceService.GetClassSummaryAsync(classId, targetDate);
            return Ok(result);
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Super Admin,Admin,Teacher,Parent")]
        public async Task<IActionResult> GetStudentAttendanceDetails(
            int studentId, 
            [FromQuery] int? month = null, 
            [FromQuery] int? year = null)
        {
            try
            {
                var targetMonth = month ?? DateTime.Today.Month;
                var targetYear = year ?? DateTime.Today.Year;
                
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                    return Unauthorized();

                var result = await _attendanceService.GetStudentAttendanceDetailsAsync(studentId, targetMonth, targetYear, userId, role);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] UpdateAttendanceDto dto)
        {
            try
            {
                var result = await _attendanceService.UpdateAttendanceAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("bulk-timeout")]
        [Authorize(Roles = "Super Admin,Admin,Teacher")]
        public async Task<IActionResult> BulkUpdateTimeout([FromBody] BulkTimeoutDto dto)
        {
            try
            {
                // 1. Get Logged-in User ID string
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("User claim not found.");

                // 2. Fetch linked Teacher Profile (if exists)
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => EF.Property<string>(t, "ApplicationUserId") == userId);

                int? teacherId = teacher?.TeacherId;

                // 3. Process the Bulk Timeout
                var count = await _attendanceService.BulkUpdateTimeoutAsync(dto, teacherId);
                
                return Ok(new { message = "TimeOut records updated successfully.", updatedCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("seed")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> SeedAttendance()
        {
            try
            {
                var classes = await _context.Classes.Take(2).ToListAsync();
                if (!classes.Any()) return BadRequest("No classes found to seed attendance for.");

                var random = new Random();
                int recordsAdded = 0;

                var teacher = await _context.Teachers.FirstOrDefaultAsync();
                if (teacher == null) return BadRequest("No teachers found to assign to attendance records.");

                foreach (var cls in classes)
                {
                    var students = await _context.Students.Where(s => s.ClassId == cls.Id).ToListAsync();
                    
                    for (int i = 0; i < 10; i++)
                    {
                        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-i));
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                        foreach (var student in students)
                        {
                            if (await _context.AttendanceRecords.AnyAsync(a => a.StudentId == student.Id && a.Date == date))
                                continue;

                            int chance = random.Next(1, 101);
                            string status = "Present";
                            TimeOnly? timeIn = new TimeOnly(8, random.Next(0, 15));
                            TimeOnly? timeOut = new TimeOnly(14, random.Next(0, 30));

                            if (chance > 85) { status = "Absent"; timeIn = null; timeOut = null; }
                            else if (chance > 70) { status = "Late"; timeIn = new TimeOnly(8, 30 + random.Next(1, 30)); }

                            _context.AttendanceRecords.Add(new AttendanceRecord
                            {
                                StudentId = student.Id,
                                ClassId = cls.Id,
                                Date = date,
                                Status = status,
                                TimeIn = timeIn,
                                TimeOut = timeOut,
                                Session = status == "Absent" ? "Morning" : "Afternoon",
                                TeacherId = teacher.TeacherId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                            recordsAdded++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Seeded {recordsAdded} attendance records across 2 classes for the last 10 days.", count = recordsAdded });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
