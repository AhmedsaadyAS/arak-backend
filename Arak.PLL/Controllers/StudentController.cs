using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Http;
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
			if (students == null)
			{
				return NotFound($"The Status {status} is invalid");
			}
			return Ok(students);
		}

		[HttpGet("SearchStudentsByName/{name}")]
		public async Task<IActionResult> GetByNameAsync(string name)
		{
			var students = await _studentService.GetByNameAsync(name);
			if (students == null) {	
				return NotFound($"The Status {name} is invalid");
            }
            return Ok(students);
        }
	}
}
