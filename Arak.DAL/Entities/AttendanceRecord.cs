using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class AttendanceRecord
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

        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

        public DateOnly Date { get; set; }
        
        public string Session { get; set; } = "Morning"; 
        
        public string Status { get; set; } = "NotRecorded";

        public TimeOnly? TimeIn { get; set; }
        
        public TimeOnly? TimeOut { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
