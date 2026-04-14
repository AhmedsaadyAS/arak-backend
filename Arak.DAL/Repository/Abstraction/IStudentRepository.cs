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
		Task<ICollection<Student>> GetByStatusAsync(string status);

        Task<ICollection<Student>> GetByNameAsync(string name);

        Task<ICollection<Student>> GetByEmailAsync(string email);

        Task<ICollection<Student>> GetStudentByClassId(int classId);

        Task<ICollection<Student>> GetStudentByParentId(int parentId);

        /// <summary>
        /// Server-side paged + filtered query.
        /// search  → searches Name, Email, StudentCode (case-insensitive contains)
        /// grade   → exact match on Student.Grade (the denormalised string column)
        /// status  → exact match on Student.Status ("Active" / "Inactive")
        /// classId → exact match on Student.ClassId
        /// Returns total count before paging so the frontend can render pagination controls.
        /// </summary>
        Task<(IEnumerable<Student> Students, int Total)> GetPagedAsync(
            int   page,
            int   pageSize,
            string? search,
            string? grade,
            string? status,
            int?    classId);
    }
}
