using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Enrollment
	{
		public int Id { get; set; }

		public int StudentId { get; set; }
		public Student Student { get; set; }

		public int ClassId { get; set; }
		public Class Class { get; set; }

		public int AcademicYearId { get; set; }
		public AcademicYear AcademicYear { get; set; }

		public int SemesterId { get; set; }
		public Semester Semester { get; set; }

		public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
		public ICollection<Grade> Grades { get; set; } = new List<Grade>();
		public ICollection<Fee> Fees { get; set; } = new List<Fee>();
	}
}
