using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Schedules (timetable slots) CRUD.
    /// GET /api/schedules supports ?classId and ?teacherId filters — required by
    /// the frontend's scheduleService.js and teacher dependency checks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
    public class SchedulesController : ControllerBase
    {
        private readonly ITimetableService _timetableService;

        public SchedulesController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        // GET /api/schedules?classId=X&teacherId=Y
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int? classId   = null,
            [FromQuery] int? teacherId = null)
        {
            var all = await _timetableService.GetAllAsync();

            // Apply optional filters
            if (classId.HasValue)
                all = all.Where(t => t.ClassId == classId.Value);

            if (teacherId.HasValue)
                all = all.Where(t => t.TeacherId == teacherId.Value);

            return Ok(all);
        }

        [HttpGet("{id:int}", Name = "GetScheduleById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _timetableService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TimeTable entity)
        {
            var created = await _timetableService.AddLesson(entity);
            return CreatedAtRoute("GetScheduleById", new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] TimeTable entity)
        {
            if (id != entity.Id) return BadRequest(new { message = "ID mismatch." });
            await _timetableService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _timetableService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Schedule slot deleted." });
        }
    }
}
