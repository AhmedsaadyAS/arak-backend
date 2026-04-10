using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class Assignment
    {
        [Key] 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime DeadLine { get; set; }
        public string State { get; set; }


        [ForeignKey("Teacher")]
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class Class { get; set; }

        [ForeignKey("Semester")]
        public int? SemesterId { get; set; }
        public Semester Semester { get; set; }


    }
}
