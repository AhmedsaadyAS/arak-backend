using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    /// <summary>
    /// Represents a student fee record (tuition, activity fees, etc.).
    /// </summary>
    public class Fee
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        /// <summary>
        /// One of: "Paid", "Unpaid", "Overdue"
        /// </summary>
        public string Status { get; set; } = "Unpaid";

        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public string? Notes { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [JsonIgnore]
        public Student Student { get; set; } = null!;
    }
}
