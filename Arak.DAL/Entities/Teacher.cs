using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Teacher
	{
		public int Id { get; set; }

		public string UserId { get; set; }   // ⭐ FK
		public ApplicationUser User { get; set; }

		public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
	}
}
