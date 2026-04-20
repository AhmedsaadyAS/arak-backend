using Arak.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Response DTO for Attendance records.
    /// </summary>
    public class AttendanceDto
    {
        public int Id { get; init; }
        public int StudentId { get; init; }
        public string StudentName { get; init; } = string.Empty;
        public DateOnly Date { get; init; }
        public TimeOnly? TimeIn { get; init; }
        public TimeOnly? TimeOut { get; init; }
        public string Status { get; init; } = string.Empty;
        public int ClassId { get; init; }
        public string? Session { get; init; }
        public string? Notes { get; init; }
    }

    /// <summary>
    /// Request DTO for marking a single attendance record.
    /// </summary>
    public class MarkAttendanceDto
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int ClassId { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }
        
        public string Session { get; set; } = "Morning";
        
        public TimeOnly? TimeIn { get; set; }
        
        public TimeOnly? TimeOut { get; set; }
        
        [Required]
        public string Status { get; set; }
        
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request DTO for bulk marking morning attendance.
    /// </summary>
    public class BulkMarkAttendanceDto
    {
        [Required]
        public int ClassId { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }
        
        public string Session { get; set; } = "Morning";
        
        public List<MorningRecordDto> Records { get; set; } = new();
    }

    public class MorningRecordDto
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public string Status { get; set; } = "Present";
        
        public TimeOnly? TimeIn { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an existing attendance record.
    /// </summary>
    public class UpdateAttendanceDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
        
        public TimeOnly? TimeIn { get; set; }
        
        public TimeOnly? TimeOut { get; set; }
        
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Response DTO for student attendance statistics.
    /// </summary>
    public class StudentAttendanceStatsDto
    {
        public double AttendanceRate { get; init; }
        public int LateCount { get; init; }
        public int AbsenceCount { get; init; }
        public int PresentCount { get; init; }
        public int TotalDays { get; init; }
    }

    /// <summary>
    /// Request DTO for bulk updating TimeOut (Afternoon session).
    /// </summary>
    public class BulkTimeoutDto
    {
        [Required]
        public int ClassId { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }
        
        public List<TimeoutRecordDto> Records { get; set; } = new();
    }

    public class TimeoutRecordDto
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public TimeOnly TimeOut { get; set; }
    }
    
    /// <summary>
    /// Response DTO containing all students in a class with matching records or fallbacks.
    /// </summary>
    public class ClassAttendanceResponseDto
    {
        public int ClassId { get; set; }
        public DateOnly Date { get; set; }
        public List<StudentAttendanceItemDto> Students { get; set; } = new();
    }

    public class StudentAttendanceItemDto
    {
        public int AttendanceRecordId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Status { get; set; } = "NotRecorded";
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
    }

    public class AttendanceSummaryDto
    {
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public double PresentRate { get; set; }
        public int AbsentCount { get; set; }
        public double AbsentRate { get; set; }
        public int LateCount { get; set; }
        public double LateRate { get; set; }
        public int NotRecordedCount { get; set; }
    }

    public class StudentAttendanceDetailDto
    {
        public string StudentName { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string TodayStatus { get; set; } = "NotRecorded";
        public TimeOnly? TodayTimeIn { get; set; }
        public TimeOnly? TodayTimeOut { get; set; }
        public double AttendanceRate { get; set; }
        public int LateArrivals { get; set; }
        public int Absences { get; set; }
        public List<AttendanceRecordItemDto> Records { get; set; } = new();
    }

    public class AttendanceRecordItemDto
    {
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
    }
}
