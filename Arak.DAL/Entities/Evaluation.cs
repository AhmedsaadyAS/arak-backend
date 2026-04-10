using System.ComponentModel.DataAnnotations.Schema;

namespace Arak.DAL.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }

        /*[ForeignKey("student")]
        public int StudentId { get; set; }
        public Student student { get; set; }

        public int Mark { get; set; }

        [ForeignKey("subject")]
        public int SubjectId { get; set; }
        public Subject subject { get; set; }*/
    }
}
