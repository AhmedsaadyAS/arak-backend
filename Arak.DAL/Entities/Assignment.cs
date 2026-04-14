using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DeadLine { get; set; }
        public string State { get; set; } = "Pending";


        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }

        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }
        [JsonIgnore]
        public Semester? Semester { get; set; }


    }
}
