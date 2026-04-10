using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class TimeTable
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class Class { get; set; }

        [ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }
        public Semester Semester { get; set; }
    }
}
