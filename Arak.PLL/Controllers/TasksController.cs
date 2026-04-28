using Arak.BLL.Service.Abstraction;
using Arak.DAL.Database;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Tasks (homework assignments) CRUD.
    /// GET /api/tasks supports ?teacherId and ?classId filters — required by
    /// the frontend's TaskMonitor and teacher dependency checks before deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin,Teacher,Parent")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly AppDbContext _db;

        public TasksController(ITaskService taskService, AppDbContext db)
        {
            _taskService = taskService;
            _db = db;
        }

        // GET /api/tasks?teacherId=X&classId=Y&studentId=Z&status=Pending
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int?    teacherId = null,
            [FromQuery] int?    classId   = null,
            [FromQuery] int?    studentId = null,
            [FromQuery] string? status    = null)
        {
            var all = await _taskService.GetAllAsync();

            if (teacherId.HasValue)
                all = all.Where(t => t.TeacherId == teacherId.Value);

            if (classId.HasValue)
                all = all.Where(t => t.ClassId == classId.Value);

            // studentId filter: resolve student's classId, then filter tasks by that class
            if (studentId.HasValue)
            {
                var student = await _db.Students.FindAsync(studentId.Value);
                if (student != null && student.ClassId.HasValue)
                    all = all.Where(t => t.ClassId == student.ClassId.Value);
                else
                    all = Enumerable.Empty<Arak.DAL.Entities.Assignment>();
            }

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
            if (entity == null)
                return BadRequest(new { message = "Request body is required." });

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.Equals(role, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (string.IsNullOrWhiteSpace(appUserId))
                    return Unauthorized(new { message = "Invalid token claims." });

                var teacher = await _db.Teachers
                    .FirstOrDefaultAsync(t => t.ApplicationUser != null
                                           && t.ApplicationUser.Id == appUserId);

                if (teacher == null)
                    return Unauthorized(new { message = "Teacher profile not found." });

                entity.TeacherId = teacher.TeacherId;
            }

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

        // ── GET /api/tasks/{taskId}/status ────────────────────────────────────
        // Returns all students in the task's class with their submission status.
        // If no TaskSubmission rows exist yet, auto-generates them as "Pending".
        [HttpGet("{taskId:int}/status")]
        public async Task<IActionResult> GetTaskStatusAsync(int taskId)
        {
            var task = await _db.Assignments.FindAsync(taskId);
            if (task == null)
                return NotFound(new { message = $"Task {taskId} not found." });

            if (!task.ClassId.HasValue)
                return BadRequest(new { message = "Task has no class assigned." });

            // Get all students in the class
            var students = await _db.Students
                .Where(s => s.ClassId == task.ClassId.Value)
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Get existing submission records
            var submissions = await _db.TaskSubmissions
                .Where(ts => ts.TaskId == taskId)
                .ToListAsync();

            // Build response — merge students with their submission status
            var result = students.Select(s =>
            {
                var sub = submissions.FirstOrDefault(x => x.StudentId == s.Id);
                return new
                {
                    taskId      = taskId.ToString(),
                    studentId   = s.Id.ToString(),
                    studentName = s.Name,
                    isDone      = sub?.IsDone ?? false,
                };
            }).ToList();

            return Ok(result);
        }

        // ── PUT /api/tasks/{taskId}/status ────────────────────────────────────
        // Bulk upsert submission statuses for a task.
        // Request body: [ { "studentId": "5", "isDone": true }, ... ]
        [HttpPut("{taskId:int}/status")]
        public async Task<IActionResult> UpdateTaskStatusAsync(
            int taskId,
            [FromBody] List<TaskStatusUpdateDto> updates)
        {
            var task = await _db.Assignments.FindAsync(taskId);
            if (task == null)
                return NotFound(new { message = $"Task {taskId} not found." });

            foreach (var update in updates)
            {
                if (!int.TryParse(update.StudentId, out var studentId))
                    continue;

                var existing = await _db.TaskSubmissions
                    .FirstOrDefaultAsync(ts => ts.TaskId == taskId && ts.StudentId == studentId);

                if (existing != null)
                {
                    existing.IsDone      = update.IsDone;
                    existing.SubmittedAt = update.IsDone ? DateTime.UtcNow : null;
                }
                else
                {
                    _db.TaskSubmissions.Add(new TaskSubmission
                    {
                        TaskId      = taskId,
                        StudentId   = studentId,
                        IsDone      = update.IsDone,
                        SubmittedAt = update.IsDone ? DateTime.UtcNow : null,
                    });
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Task statuses updated." });
        }
    }

    public class TaskStatusUpdateDto
    {
        public string StudentId { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }
}
