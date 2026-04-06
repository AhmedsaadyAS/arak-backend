using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Student
	{
		public int Id { get; set; }

		public string UserId { get; set; }   // ⭐ FK to Identity
		public ApplicationUser User { get; set; }

		public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
	}
}
