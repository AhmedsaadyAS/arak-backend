using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Attendance
    {

        public int Id { get; set; }
        public AttendanceStatus Status { get; set; }

        //هنا محتاجينه ياخد التاريخ من الفرونت
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;      //هنا المفروض نعدلها و نخليها DateOnly بحيث انها ترجع اليوم فقط
        public TimeOnly ArrivalTime { get; set; } 
        public TimeOnly DepartureTime { get; set; }

        [ForeignKey("semester")]
        public int? SemesterId { get; set; }
        public Semester Semester { get; set; }

        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<StudentAttendance> Students { get; set; } = new List<StudentAttendance>();

        public enum AttendanceStatus
        {
            Present = 1,
            Absent = 2,
            Late = 3
        }
    }
}
