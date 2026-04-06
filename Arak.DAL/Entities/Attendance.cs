using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Attendance
	{
		public int Id { get; set; }

		public int EnrollmentId { get; set; }
		public Enrollment Enrollment { get; set; }

		public int TeacherSubjectId { get; set; }

		public DateTime Date { get; set; }
		public bool IsPresent { get; set; }
	}
}
