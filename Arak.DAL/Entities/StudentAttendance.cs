using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class StudentAttendance
    {
        public int Id { get; set; }

        [ForeignKey("Attendance")]
        public int AttendanceId { get; set; }
        public Attendance Attendance { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
