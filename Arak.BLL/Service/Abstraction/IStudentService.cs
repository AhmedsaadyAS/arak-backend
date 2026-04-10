using Arak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
	public interface IStudentService
	{
		Task<IEnumerable<Student>> GetAllStudentsAsync();
		Task<Student> GetStudentByIdAsync(int id);
	}
}
