using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Arak.DAL.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
	public class StudentService : IStudentService
	{
		private readonly IStudentRepository _studentRepository;
		public StudentService(IStudentRepository studentRepository)
		{
			_studentRepository = studentRepository;
		}

		public async Task<IEnumerable<Student>> GetAllStudentsAsync()
		{
			return await _studentRepository.GetAllAsync();
		}

		public async Task<Student> GetStudentsByIdAsync(int id)
		{

			return await _studentRepository.GetByIdAsync(id);
		}

		public async Task<ICollection<Student>> GetByStatusAsync(string status)
		{
			return await _studentRepository.GetByStatusAsync(status);
		}

		public async Task<ICollection<Student>> GetByNameAsync(string name)
		{
			return await _studentRepository.GetByNameAsync(name);
		}

		public async Task<Student> CreateAsync(Student student)
		{
			var created = await _studentRepository.CreateAsync(student);
			await _studentRepository.SaveChangesAsync();
			return created;
		}

		public async Task<ICollection<Student>> GetByEmailAsync(string email)
		{
			return await _studentRepository.GetByEmailAsync(email);
		}

		public async Task<ICollection<Student>> GetStudentByClassId(int classId)
		{
			return await _studentRepository.GetStudentByClassId(classId);
		}

		public async Task<ICollection<Student>> GetStudentByParentId(int parentId)
		{
			return await _studentRepository.GetStudentByParentId(parentId);
		}

		public async Task<(IEnumerable<Student> Students, int Total)> GetPagedAsync(
            int page, int pageSize, string? search, string? grade, string? status, int? classId)
        {
            return await _studentRepository.GetPagedAsync(page, pageSize, search, grade, status, classId);
        }

        public async Task<Student> UpdateAsync(Student student)
		{
			var updated = await _studentRepository.UpdateAsync(student);
			await _studentRepository.SaveChangesAsync();
			return updated;
		}

		public async Task<bool> DeleteAsync(int Id)
		{
			var result = await _studentRepository.DeleteAsync(Id);
			if (result) 
            {
                try 
                {
                    await _studentRepository.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    throw new InvalidOperationException("Cannot delete this entity as it is currently referenced by other active records. Please re-assign or remove them first.");
                }
            }
			return result;
		}
    }
}
