using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class GradeLevel
	{
		public int Id { get; set; }
		public string Name { get; set; } // Grade 10, Grade 11

		public ICollection<Class> Classes { get; set; } = new List<Class>();
	}

}
