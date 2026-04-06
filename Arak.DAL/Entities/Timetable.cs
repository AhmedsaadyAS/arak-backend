using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class Timetable
	{
		public int Id { get; set; }

		public int TeacherSubjectId { get; set; }
		public TeacherSubject TeacherSubject { get; set; }

		public DayOfWeek DayOfWeek { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
	}
}
