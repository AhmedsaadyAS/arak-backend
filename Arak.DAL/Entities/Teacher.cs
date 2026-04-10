using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class Teacher
    {
		public int TeacherId { get; set; }
		[ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        public TimeTable TimeTable { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<TeacherClass> Classes { get; set; } = new List<TeacherClass>();
        public ICollection<TeacherSemester> Semesters { get; set; } = new List<TeacherSemester>();
		public ApplicationUser ApplicationUser { get; set; }

	}
}
