using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class TeacherSubject
	{
		public int Id { get; set; }

		public int TeacherId { get; set; }
		public Teacher Teacher { get; set; }

		public int SubjectId { get; set; }
		public Subject Subject { get; set; }

		public int ClassId { get; set; }
		public Class Class { get; set; }

		public int AcademicYearId { get; set; }
		public int SemesterId { get; set; }

		public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
	}
}
