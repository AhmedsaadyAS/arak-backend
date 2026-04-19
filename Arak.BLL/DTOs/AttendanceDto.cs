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
        public TimeSpan? TimeIn { get; init; }
        public TimeSpan? TimeOut { get; init; }
        public string Status { get; init; } = string.Empty;
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
        public DateOnly Date { get; set; }
        
        public TimeSpan? TimeIn { get; set; }
        
        public TimeSpan? TimeOut { get; set; }
        
        [Required]
        public AttendanceStatus Status { get; set; }
        
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request DTO for bulk marking attendance for a class.
    /// </summary>
    public class BulkMarkAttendanceDto
    {
        [Required]
        public int ClassId { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }
        
        public List<BulkAttendanceRecord> Records { get; set; } = new();
    }

    public class BulkAttendanceRecord
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public AttendanceStatus Status { get; set; }
        
        public TimeSpan? TimeIn { get; set; }
        
        public TimeSpan? TimeOut { get; set; }
        
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an existing attendance record.
    /// </summary>
    public class UpdateAttendanceDto
    {
        public TimeSpan? TimeIn { get; set; }
        
        public TimeSpan? TimeOut { get; set; }
        
        [Required]
        public AttendanceStatus Status { get; set; }
        
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
    /// Request DTO for bulk updating TimeOut for a class.
    /// </summary>
    public class BulkUpdateTimeOutDto
    {
        [Required]
        public int ClassId { get; set; }
        
        [Required]
        public DateOnly Date { get; set; }
        
        public List<TimeOutRecord> Records { get; set; } = new();
    }

    public class TimeOutRecord
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public TimeSpan TimeOut { get; set; }
    }
}
