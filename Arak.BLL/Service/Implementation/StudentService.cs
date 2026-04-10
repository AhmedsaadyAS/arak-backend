using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Arak.DAL.Repository.Implementation;
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
	}
}
