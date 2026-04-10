using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; } = true;
        public string? Image { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        [JsonIgnore]
        public Parent? Parent { get; set; }


        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }



        [JsonIgnore]
        public ICollection<StudentAttendance> Attendances { get; set; } = new List<StudentAttendance>();

        [JsonIgnore]
        public ICollection<StudentSubject> Subjects { get; set; } = new List<StudentSubject>();

    }
}
