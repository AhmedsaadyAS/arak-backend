using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARAK.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// GET /api/students
        /// Supports server-side pagination and filtering.
        /// Query params: _page (default 1), _per_page (default 20), q (search), grade, status, classId
        /// Returns: { data: StudentDto[], total: int, items: int, page: int, pageSize: int }
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStudents(
            [FromQuery] int    _page     = 1,
            [FromQuery] int    _per_page = 20,
            [FromQuery] string? q        = null,
            [FromQuery] string? grade    = null,
            [FromQuery] string? status   = null,
            [FromQuery] int?    classId  = null)
        {
            var (students, total) = await _studentService.GetPagedAsync(
                _page, _per_page, q, grade, status, classId);

            // Map to DTO (resolves studentCode → studentId naming)
            var dtos = students.Select(MapToDto);

            return Ok(new PagedResult<StudentDto>
            {
                Data     = dtos,
                Total    = total,
                Page     = _page,
                PageSize = _per_page
            });
        }

        [HttpGet("{id:int}", Name = "GetStudentById")]
        public async Task<IActionResult> GetStudentByIdAsync(int id)
        {
            var student = await _studentService.GetStudentsByIdAsync(id);
            if (student == null)
                return NotFound(new { message = $"Student with id {id} not found." });

            return Ok(MapToDto(student));
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetStudentsByStatus(string status)
        {
            var students = await _studentService.GetByStatusAsync(status);
            return Ok(students.Select(MapToDto));
        }

        [HttpGet("SearchStudentsByName/{name}")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var students = await _studentService.GetByNameAsync(name);
            return Ok(students.Select(MapToDto));
        }

        [HttpGet("SearchStudentsByClassId/{classId}")]
        public async Task<IActionResult> GetStudentsByClassId(int classId)
        {
            var students = await _studentService.GetStudentByClassId(classId);
            return Ok(students.Select(MapToDto));
        }

        [HttpGet("SearchStudentsByParentId/{parentId}")]
        public async Task<IActionResult> GetStudentsByParentId(int parentId)
        {
            var students = await _studentService.GetStudentByParentId(parentId);
            return Ok(students.Select(MapToDto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
        {
            var student = new Student
            {
                StudentCode  = dto.StudentId,
                Name         = dto.Name,
                UserName     = dto.UserName,
                Age          = dto.Age,
                Email        = dto.Email,
                DateOfBirth  = string.IsNullOrWhiteSpace(dto.DateOfBirth) ? DateTime.MinValue : DateTime.Parse(dto.DateOfBirth),
                PlaceOfBirth = dto.PlaceOfBirth,
                Address      = dto.Address,
                City         = dto.City,
                PhoneNumber  = dto.PhoneNumber,
                Grade        = dto.Grade,
                Status       = dto.Status,
                Image        = dto.Image,
                ClassId      = dto.ClassId,
                ParentId     = dto.ParentId
            };

            var created = await _studentService.CreateAsync(student);
            return CreatedAtRoute("GetStudentById", new { id = created.Id }, MapToDto(created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest(new { message = "URL id does not match body id." });

            var updated = await _studentService.UpdateAsync(student);
            return Ok(MapToDto(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _studentService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Student with id {id} not found." });

            return Ok(new { message = "Student deleted successfully." });
        }

        // ── Private helpers ───────────────────────────────────────────────

        private static StudentDto MapToDto(Student s) => new()
        {
            Id          = s.Id,
            StudentId   = s.StudentCode,          // rename for frontend contract
            Name        = s.Name,
            UserName    = s.UserName,
            Age         = s.Age,
            Email       = s.Email,
            DateOfBirth = s.DateOfBirth.ToString("yyyy-MM-dd"),
            PlaceOfBirth= s.PlaceOfBirth,
            Address     = s.Address,
            City        = s.City,
            PhoneNumber = s.PhoneNumber,
            Grade       = s.Grade,
            Status      = s.Status,
            Image       = s.Image,
            ClassId     = s.ClassId,
            ParentId    = s.ParentId,
            // Navigation properties are not eagerly loaded from generic repo —
            // ClassName / ParentName will be null unless we load them. 
            // A future enhancement can use Include() on dedicated queries.
            ClassName   = null,
            ParentName  = null,
        };
    }
}
