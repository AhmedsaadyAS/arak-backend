using Arak.DAL.Database;
using Arak.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Implementation
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly AppDbContext _context;

		public GenericRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<T>> GetAllAsync()
			=> await _context.Set<T>().ToListAsync();

		public async Task<T> GetByIdAsync(int id)
			=> await _context.Set<T>().FindAsync(id);

		public async Task CreateAsync(T entity)
			=> await _context.Set<T>().AddAsync(entity);

		public void Update(T entity)
			=> _context.Set<T>().Update(entity);

		public void Delete(T entity)
			=> _context.Set<T>().Remove(entity);
	}
}
