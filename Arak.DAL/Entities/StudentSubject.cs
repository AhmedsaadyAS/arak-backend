using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class StudentSubject
    {
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [JsonIgnore]
        public Student? Student { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        [JsonIgnore]
        public Subject? Subject { get; set; }
    }
}
