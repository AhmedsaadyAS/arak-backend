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

		public async Task<Student> GetStudentByIdAsync(int id)
		{
			return await _studentRepository.GetByIdAsync(id);
		}

		public async Task<ICollection<Student>> GetByStatusAsync(bool status)
		{
			return await _studentRepository.GetByStatusAsync(status);
		}

		public async Task<ICollection<Student>> GetByNameAsync(string name)
		{
			return await _studentRepository.GetByNameAsync(name);
		}

		public async Task<Student> CreateAsync(Student student) 
		{
			return await _studentRepository.CreateAsync(student);
		}

		public async Task<ICollection<Student>> GetByEmailAsync(string email)
		{
			return await _studentRepository.GetByEmailAsync(email);
		}

		public async Task<ICollection<Student>> GetStudentByClassId(int classId)
		{
			return await _studentRepository.GetStudentByClassId(classId);
		}

		public async Task<Student> UpdateAsync(Student student)
		{
			return await _studentRepository.UpdateAsync(student);
		}

		public async Task<bool> DeleteAsync(int Id)
		{

			var result = await _studentRepository.DeleteAsync(Id);
			return result;
		}
    }
}
