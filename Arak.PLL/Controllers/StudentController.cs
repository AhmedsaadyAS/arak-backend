using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ARAK.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
		private readonly IStudentService _studentService;
		public StudentsController(IStudentService studentService)
		{
			_studentService = studentService;
		}

		[HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var allStudents = await _studentService.GetAllStudentsAsync();
			return Ok(allStudents);
        }

		[HttpGet("{status}")]
		public async Task<IActionResult> GetStudentsByStatus(bool status)
		{
			var students = await _studentService.GetByStatusAsync(status);
			if (students.Any() == false)
			{
				return NotFound($"The Status {status} is invalid!");
			}
			return Ok(students);
		}

		[HttpGet("SearchStudentsByName/{name}")]
		public async Task<IActionResult> GetByNameAsync(string name)
		{
			var students = await _studentService.GetByNameAsync(name);
			if (students.Any() == false) {	
				return NotFound($"The Status {name} is invalid!");
            }
            return Ok(students);
        }

		[HttpGet("SearchStudentsByEmail/{email}")]
		public async Task<IActionResult> GetByEmailAsync(string email)
		{
			var students = await _studentService.GetByEmailAsync(email);
			if (students.Any() == false)
			{
				return BadRequest($"The Email {email} is invalid!");
			}
			return Ok(students);
		}

        [HttpGet("SearchStudentsByClassId/{classId}")]
        public async Task<IActionResult> GetStudentByClassId(int classId)
        {
            var students = await _studentService.GetStudentByClassId(classId);
            if (students.Any() == false)
            {
                return NotFound($"The ClassId {classId} is invalid!");
            }
            return Ok(students);
        }

        [HttpPost]
		public async Task<IActionResult> CreateStudent(Student student)
		{
			var Std = await _studentService.CreateAsync(student);
			return Ok(student);
		}
	}
}
