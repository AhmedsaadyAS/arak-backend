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
		Task<ICollection<Student>> GetByNameAsync(string name);
		Task<ICollection<Student>> GetByEmailAsync(string email);
		Task<ICollection<Student>> GetStudentByClassId(int classId);
		Task<ICollection<Student>> GetByStatusAsync(bool status);
		Task<Student> GetStudentByIdAsync(int id);


		Task<Student> CreateAsync(Student student);

		Task<Student> UpdateAsync(Student student);

		Task<bool> DeleteAsync(int Id);
    }
}
