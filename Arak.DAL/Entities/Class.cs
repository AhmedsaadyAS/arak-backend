using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Class
	{
		public int Id { get; set; }
		public string Name { get; set; } // A, B

		public int GradeLevelId { get; set; }
		public GradeLevel GradeLevel { get; set; }

		public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
	}
}
