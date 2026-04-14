using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
    public class TimetableController : ControllerBase
    {
        private readonly ITimetableService _timetableService;
        public TimetableController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        [HttpGet("{id:int}", Name = "GetTimetableById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _timetableService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Schedule slot {id} not found." });
            return Ok(entity);
        }

        [HttpGet("GetTimetablesByClassId/{classId}")]
        public async Task<IActionResult> GetTimetableByClassId(int classId)
        {
            var timetables = await _timetableService.GetTimetableByClassId(classId);
            if (!timetables.Any())
            {
                return NotFound(new { message = $"No timetable found for class {classId}." });
            }

            return Ok(timetables);
        }

        [HttpGet("GetTimetablesByTeacherId/{teacherId}")]
        public async Task<IActionResult> GetTimetableByTeacherId(int teacherId)
        {
            var timetables = await _timetableService.GetTimetableByTeacherId(teacherId);
            if (timetables.Any() == false)
            {
                return NotFound(new { message = $"No timetable found for teacher {teacherId}." });
            }

            return Ok(timetables);
        }

        [HttpPost]
        public async Task<IActionResult> AddLesson([FromBody] TimeTable timeTable)
        {
            var created = await _timetableService.AddLesson(timeTable);
            return CreatedAtRoute("GetTimetableById", new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, TimeTable timeTable)
        {
            if (id != timeTable.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }
            var updated = await _timetableService.UpdateAsync(timeTable);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _timetableService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Schedule slot with ID {id} not found." });
            }

            return Ok(new { message = "Lesson deleted." });
        }
    }
}
