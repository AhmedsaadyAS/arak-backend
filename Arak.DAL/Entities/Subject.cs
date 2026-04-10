using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? BookUrl { get; set; }

        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }
        public Semester Semester { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        public TimeTable TimeTable { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<StudentSubject> Students { get; set; } = new List<StudentSubject>();
    }
}
