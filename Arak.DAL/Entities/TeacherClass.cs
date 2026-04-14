using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class TeacherClass
    {
        public int Id { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }
    }
}
