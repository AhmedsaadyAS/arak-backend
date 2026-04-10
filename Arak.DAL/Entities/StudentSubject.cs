using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class StudentSubject
    {
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
