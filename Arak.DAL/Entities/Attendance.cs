using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public enum AttendanceStatus
    {
        Present = 0,
        Absent = 1,
        Late = 2
    }

    public class Attendance
    {
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan? TimeIn { get; set; }

        public TimeSpan? TimeOut { get; set; }

        public AttendanceStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}
