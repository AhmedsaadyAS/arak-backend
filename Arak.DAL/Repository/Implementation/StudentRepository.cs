using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
//using Arak.BLL.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Implementation
{
	public class StudentRepository : GenericRepository<Student>, IStudentRepository
	{
		public StudentRepository(AppDbContext context) : base(context) { }
		public async Task<ICollection<Student>> GetByStatusAsync(string status)
		{
			var students = await _context.Students.Where(x => x.Status == status).ToListAsync();
			return students;
		}

		public async Task<ICollection<Student>> GetByNameAsync(string name)
		{
            var students = await _context.Students.Where(x => x.Name == name).ToListAsync();
			return students;	
        }

		public async Task<ICollection<Student>> GetByEmailAsync(string email)
		{
			var students = await _context.Students.Where(x => x.Email == email).ToListAsync();
			return students;
		}

		public async Task<ICollection<Student>> GetStudentByClassId(int classId)
		{
			var students = await _context.Students.Where(x=>x.ClassId == classId).ToListAsync();
			return students;
		}

        public async Task<ICollection<Student>> GetStudentByParentId(int parentId)
        {
            var students = await _context.Students.Where(x => x.ParentId == parentId).ToListAsync();
            return students;
        }

        public async Task<(IEnumerable<Student> Students, int Total)> GetPagedAsync(
            int   page,
            int   pageSize,
            string? search,
            string? grade,
            string? status,
            int?    classId)
        {
            var query = _context.Students.AsQueryable();

            // --- Filtering ---
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search;
                // SQL Server default collation is case-insensitive, no .ToLower() needed
                query = query.Where(x =>
                    x.Name.Contains(s)        ||
                    x.Email.Contains(s)       ||
                    x.StudentCode.Contains(s) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(grade) && grade != "All")
                query = query.Where(x => x.Grade == grade);

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(x => x.Status == status);

            if (classId.HasValue)
                query = query.Where(x => x.ClassId == classId.Value);

            // --- Total before paging ---
            var total = await query.CountAsync();

            // --- Paging ---
            var students = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students, total);
        }
    }
}
