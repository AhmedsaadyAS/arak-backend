using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Class
    {
        public int Id { get; set; }

        /// <summary>Full class name, e.g. "Grade 4-A"</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Grade group, e.g. "Grade 4". Used for grouping in Gradebook.</summary>
        public string? Grade { get; set; }

        /// <summary>
        /// School stage: "primary" | "preparatory" | "secondary"
        /// Required by Gradebook Monitor and grade schema selection.
        /// </summary>
        public string? Stage { get; set; }

        /// <summary>
        /// When true, no new evaluations may be posted for students in this class.
        /// Toggled by Super Admin / Academic Admin via PATCH /classes/{id}.
        /// </summary>
        public bool GradesLocked { get; set; } = false;

        public string? Description { get; set; }

        /// <summary>
        /// Maximum number of students allowed in this class.
        /// Default is 30. Used for capacity validation when adding students.
        /// </summary>
        public int MaxStudents { get; set; } = 30;

        [ForeignKey("TimeTable")]
        public int? TimeTableId { get; set; }

        [JsonIgnore]
        public TimeTable? TimeTable { get; set; }

        [JsonIgnore]
        public ICollection<Student> Students { get; set; } = new List<Student>();

        [JsonIgnore]
        public ICollection<Assignment> Tasks { get; set; } = new List<Assignment>();

        [JsonIgnore]
        public ICollection<TeacherClass> Teachers { get; set; } = new List<TeacherClass>();

        [JsonIgnore]
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
