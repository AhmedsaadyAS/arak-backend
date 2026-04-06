using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class AcademicYear
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public bool IsActive { get; set; }

		public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
	}
}
