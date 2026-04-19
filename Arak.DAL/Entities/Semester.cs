using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Semester
    {
        public int Id { get; set; }
        public SemesterName Name { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        [JsonIgnore]
        public TimeTable? TimeTable { get; set; }

        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        [JsonIgnore]
        public ICollection<AttendanceRecord> Attendances { get; set; } = new List<AttendanceRecord>();
        [JsonIgnore]
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        [JsonIgnore]
        public ICollection<TeacherSemester> Teachers { get; set; } = new List<TeacherSemester>();
    }

    /// <summary>
    /// Semester names — moved out of the Semester class to avoid EF Core modelling issues.
    /// </summary>
    public enum SemesterName
    {
        First = 1,
        Second = 2,
        Summer = 3
    }
}
