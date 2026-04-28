using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class TaskSubmission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Assignment")]
        public int TaskId { get; set; }

        [JsonIgnore]
        public Assignment? Assignment { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        /// <summary>
        /// true = student submitted / done, false = pending
        /// </summary>
        public bool IsDone { get; set; } = false;

        public DateTime? SubmittedAt { get; set; }
    }
}
