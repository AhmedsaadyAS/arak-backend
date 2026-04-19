using Arak.BLL.DTOs;
using Arak.DAL.Database;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ARAK.PLL.Controllers
{
    /// <summary>
    /// Class (section) management.
    /// GET /api/classes supports filtering by stage and teacherId.
    /// PATCH /api/classes/{id} toggles gradesLocked and other fields.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
    public class ClassesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClassesController(AppDbContext db)
        {
            _db = db;
        }

        // ── GET /api/classes ─────────────────────────────────────────────────
        // Supports: ?stage=primary  |  ?teacherId=3
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string? stage     = null,
            [FromQuery] int?    teacherId = null)
        {
            var query = _db.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(stage) && stage != "all")
                query = query.Where(c => c.Stage == stage.ToLower());

            // teacherId filter — find classes where this teacher is assigned
            if (teacherId.HasValue)
                query = query.Where(c => c.Teachers.Any(tc => tc.TeacherId == teacherId.Value));

            var classes = await query.ToListAsync();
            return Ok(classes.Select(MapToDto));
        }

        // ── GET /api/classes/{id} ────────────────────────────────────────────
        [HttpGet("{id:int}", Name = "GetClassById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var cls = await _db.Classes
                .Include(c => c.Students)
                .Include(c => c.Teachers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cls == null)
                return NotFound(new { message = $"Class {id} not found." });

            return Ok(MapToDto(cls));
        }

        // ── POST /api/classes ────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateClassDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cls = new Class
            {
                Name        = dto.Name,
                Grade       = dto.Grade,
                Stage       = dto.Stage?.ToLower(),
                Description = dto.Description,
                GradesLocked = false,
                MaxStudents = dto.MaxStudents ?? 30,
            };

            _db.Classes.Add(cls);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetClassById", new { id = cls.Id }, MapToDto(cls));
        }

        // ── PUT /api/classes/{id} ────────────────────────────────────────────
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateClassDto dto)
        {
            var cls = await _db.Classes.FindAsync(id);
            if (cls == null) return NotFound(new { message = $"Class {id} not found." });

            cls.Name        = dto.Name;
            cls.Grade       = dto.Grade;
            cls.Stage       = dto.Stage?.ToLower();
            cls.Description = dto.Description;
            if (dto.MaxStudents.HasValue)
                cls.MaxStudents = dto.MaxStudents.Value;

            await _db.SaveChangesAsync();
            return Ok(MapToDto(cls));
        }

        // ── PATCH /api/classes/{id} ──────────────────────────────────────────
        // Used for: toggling gradesLocked, partial updates
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] PatchClassDto dto)
        {
            var cls = await _db.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cls == null) return NotFound(new { message = $"Class {id} not found." });

            if (dto.Name        is not null) cls.Name        = dto.Name;
            if (dto.Grade       is not null) cls.Grade       = dto.Grade;
            if (dto.Stage       is not null) cls.Stage       = dto.Stage.ToLower();
            if (dto.Description is not null) cls.Description = dto.Description;
            if (dto.GradesLocked.HasValue)   cls.GradesLocked = dto.GradesLocked.Value;
            if (dto.MaxStudents.HasValue)    cls.MaxStudents  = dto.MaxStudents.Value;

            await _db.SaveChangesAsync();
            return Ok(MapToDto(cls));
        }

        // ── DELETE /api/classes/{id} ─────────────────────────────────────────
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var cls = await _db.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cls == null) return NotFound(new { message = $"Class {id} not found." });

            // Guard: prevent deletion if students are enrolled or teachers are assigned
            if (cls.Students.Any() || cls.Teachers.Any())
                return Conflict(new
                {
                    message = $"Cannot delete class '{cls.Name}' — it has enrolled students or assigned teachers."
                });

            _db.Classes.Remove(cls);
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Class '{cls.Name}' deleted." });
        }

        // ── Mapping helper ───────────────────────────────────────────────────

        private static ClassDto MapToDto(Class c) => new()
        {
            Id           = c.Id,
            Name         = c.Name,
            Grade        = c.Grade,
            Stage        = c.Stage,
            GradesLocked = c.GradesLocked,
            Description  = c.Description,
            MaxStudents  = c.MaxStudents,
            StudentCount = c.Students?.Count ?? 0,
        };
    }
}
