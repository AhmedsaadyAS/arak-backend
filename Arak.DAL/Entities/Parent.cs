using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Parent : ApplicationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        [JsonIgnore]
        public ICollection<Student> Students { get; set; } = new List<Student>();

    }
}
