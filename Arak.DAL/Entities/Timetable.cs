using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class TimeTable
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }

        [ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        [JsonIgnore]
        public Subject? Subject { get; set; }

        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }
        [JsonIgnore]
        public Semester? Semester { get; set; }
    }
}
