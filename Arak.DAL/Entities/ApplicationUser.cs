using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; }

		// Navigation
		public Student Student { get; set; }
		public Teacher Teacher { get; set; }
	}
}
