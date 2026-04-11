using Arak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Abstraction
{
	public interface IStudentRepository : IGenericRepository<Student>
	{
		Task<ICollection<Student>> GetByStatusAsync(bool status);

        Task<ICollection<Student>> GetByNameAsync(string name);

        Task<ICollection<Student>> GetByEmailAsync(string email);

        Task<ICollection<Student>> GetStudentByClassId(int classId);

        Task<ICollection<Student>> GetStudentByParentId(int parentId);
    }
}
