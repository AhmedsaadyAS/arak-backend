using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class Semester
    {
        public int Id { get; set; }
        public SemesterName Name { get; set; }
        public string AcademicYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        public TimeTable TimeTable { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<TeacherSemester> Teachers { get; set; } = new List<TeacherSemester>();

        public enum SemesterName
        {
            First = 1,
            Second = 2,
            Summer = 3
        }
    }
}
