using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class StudentAttendance
    {
        public int Id { get; set; }

        [ForeignKey("Attendance")]
        public int AttendanceId { get; set; }
        [JsonIgnore]
        public Attendance? Attendance { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [JsonIgnore]
        public Student? Student { get; set; }
    }
}
