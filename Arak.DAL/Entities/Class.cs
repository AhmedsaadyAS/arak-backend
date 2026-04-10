using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }
        public TimeTable TimeTable { get; set; }

        [JsonIgnore]
        public ICollection<Student> Students { get; set; } = new List<Student>();

        [JsonIgnore]
        public ICollection<Assignment> Tasks { get; set; } = new List<Assignment>();

        [JsonIgnore]
        public ICollection<TeacherClass> Teachers { get; set; } = new List<TeacherClass>();
    }
}
