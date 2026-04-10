using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class TeacherSemester
    {
        public int Id { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        [ForeignKey("Semester")]
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
    }
}
