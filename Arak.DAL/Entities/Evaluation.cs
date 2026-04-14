using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    /// <summary>
    /// Represents a student grade/evaluation record.
    /// Used by Gradebook Monitor and Control Sheet upload.
    /// Assessment types vary by stage:
    ///   Primary: Month1, Month2, Month3, Final
    ///   Preparatory: Month1, Month2, Final, Oral, Practical
    ///   Secondary: Month1, Month2, Final, Oral, Practical, Research
    /// </summary>
    public class Evaluation
    {
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }

        [JsonIgnore]
        public Class? Class { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }

        [JsonIgnore]
        public Subject? Subject { get; set; }

        /// <summary>
        /// The assessment/exam type. E.g. "Month1", "Month2", "Final", "Oral", "Practical", "Research"
        /// </summary>
        public string AssessmentType { get; set; } = string.Empty;

        /// <summary>
        /// Alias for AssessmentType — kept for backward compatibility with the ERD naming.
        /// </summary>
        public string ExamType
        {
            get => AssessmentType;
            set => AssessmentType = value;
        }

        public float Marks { get; set; }

        public float MaxMarks { get; set; } = 100f;

        public string? Feedback { get; set; }

        public DateOnly Date { get; set; }

        /// <summary>
        /// True when the student was absent (marked with 'غ' or 'A' in the Excel upload).
        /// </summary>
        public bool IsAbsent { get; set; } = false;

        // TermId maps to Semester (the existing entity in this solution)
        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }

        [JsonIgnore]
        public Semester? Semester { get; set; }
    }
}
