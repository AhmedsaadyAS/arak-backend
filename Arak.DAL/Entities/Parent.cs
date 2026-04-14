using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Parent
    {
		public int ParentId { get; set; }
		[JsonIgnore]
        public ICollection<Student> Students { get; set; } = new List<Student>();
		[JsonIgnore]
        public ApplicationUser? ApplicationUser { get; set; }
	}
}
