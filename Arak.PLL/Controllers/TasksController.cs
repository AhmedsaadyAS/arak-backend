using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Tasks (homework assignments) CRUD.
    /// GET /api/tasks supports ?teacherId and ?classId filters — required by
    /// the frontend's TaskMonitor and teacher dependency checks before deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin,Teacher")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET /api/tasks?teacherId=X&classId=Y&status=Pending
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int?    teacherId = null,
            [FromQuery] int?    classId   = null,
            [FromQuery] string? status    = null)
        {
            var all = await _taskService.GetAllAsync();

            if (teacherId.HasValue)
                all = all.Where(t => t.TeacherId == teacherId.Value);

            if (classId.HasValue)
                all = all.Where(t => t.ClassId == classId.Value);

            // Assignment entity uses "State" field (not "Status")
            if (!string.IsNullOrWhiteSpace(status))
                all = all.Where(t => t.State != null &&
                                     t.State.Equals(status, StringComparison.OrdinalIgnoreCase));

            return Ok(all);
        }

        [HttpGet("{id:int}", Name = "GetTaskById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _taskService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Assignment entity)
        {
            await _taskService.CreateAsync(entity);
            return CreatedAtRoute("GetTaskById", new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Assignment entity)
        {
            if (id != entity.Id) return BadRequest(new { message = "ID mismatch." });
            await _taskService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _taskService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Task deleted." });
        }
    }
}
