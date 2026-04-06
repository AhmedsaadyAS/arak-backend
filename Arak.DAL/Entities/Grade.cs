using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Grade
	{
		public int Id { get; set; }

		public int EnrollmentId { get; set; }
		public Enrollment Enrollment { get; set; }

		public int TeacherSubjectId { get; set; }

		public string ExamType { get; set; }
		public double Score { get; set; }
	}
}
