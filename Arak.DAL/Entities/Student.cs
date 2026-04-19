using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Student
    {
        public int Id { get; set; }

        /// <summary>
        /// Human-readable display code. E.g. "STU-4A-01"
        /// Maps to "studentId" in the API contract (BACKEND.md).
        /// </summary>
        public string StudentCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Grade name string. E.g. "Grade 4-A".
        /// Denormalized for quick filtering — the ClassId FK is the authoritative source.
        /// </summary>
        public string Grade { get; set; } = string.Empty;

        /// <summary>
        /// "Active" or "Inactive" — matches the API contract string format.
        /// </summary>
        public string Status { get; set; } = "Active";

        public string? Image { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        [JsonIgnore]
        public Parent? Parent { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }

        [JsonIgnore]
        public Class? Class { get; set; }

        [JsonIgnore]
        public ICollection<StudentSubject> Subjects { get; set; } = new List<StudentSubject>();

        [JsonIgnore]
        public ICollection<Fee> Fees { get; set; } = new List<Fee>();

        [JsonIgnore]
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
