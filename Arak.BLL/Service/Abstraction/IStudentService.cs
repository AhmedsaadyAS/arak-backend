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
		Task<ICollection<Student>> GetStudentByParentId(int parentId);
        Task<ICollection<Student>> GetByStatusAsync(string status);
		Task<Student> GetStudentsByIdAsync(int id);
        Task<(IEnumerable<Student> Students, int Total)> GetPagedAsync(
            int page, int pageSize, string? search, string? grade, string? status, int? classId);

        Task<Student> CreateAsync(Student student);

		Task<Student> UpdateAsync(Student student);

		Task<bool> DeleteAsync(int Id);
    }
}
