using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
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
	}
}
