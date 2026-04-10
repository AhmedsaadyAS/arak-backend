using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class TeacherClass
    {
        public int Id { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class Class { get; set; }
    }
}
