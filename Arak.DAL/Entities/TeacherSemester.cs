using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class TeacherSemester
    {
        public int Id { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }
        [JsonIgnore]
        public Semester? Semester { get; set; }
    }
}
