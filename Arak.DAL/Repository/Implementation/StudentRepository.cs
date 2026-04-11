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
		public async Task<ICollection<Student>> GetByStatusAsync(bool status)
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

    }
}
