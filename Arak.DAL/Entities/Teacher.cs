using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Teacher
    {
		public int TeacherId { get; set; }
		[ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        [JsonIgnore]
        public Subject? Subject { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        [JsonIgnore]
        public TimeTable? TimeTable { get; set; }

        [JsonIgnore]
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        [JsonIgnore]
        public ICollection<TeacherClass> Classes { get; set; } = new List<TeacherClass>();
        [JsonIgnore]
        public ICollection<TeacherSemester> Semesters { get; set; } = new List<TeacherSemester>();
        [JsonIgnore]
        public ApplicationUser? ApplicationUser { get; set; }
	}
}
