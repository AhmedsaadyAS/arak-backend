using Arak.BLL.Service.Abstraction;
using Arak.DAL.Database;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Evaluations (grades) CRUD.
    /// GET /api/evaluations supports ?classId, ?subjectId, ?studentId, ?assessmentType filters.
    /// These are required by the Gradebook Monitor and the Control Sheet upsert workflow.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin,Teacher,Parent")]
    public class EvaluationsController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        private readonly AppDbContext _context;

        public EvaluationsController(IEvaluationService evaluationService, AppDbContext context)
        {
            _evaluationService = evaluationService;
            _context = context;
        }

        // GET /api/evaluations?classId=X&subjectId=Y&studentId=Z&assessmentType=Month1
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int?    classId        = null,
            [FromQuery] int?    subjectId      = null,
            [FromQuery] int?    studentId      = null,
            [FromQuery] string? assessmentType = null)
        {
            // Use IQueryable for database-level filtering — not in-memory
            var query = _context.Evaluations.AsQueryable();

            if (classId.HasValue)
                query = query.Where(e => e.ClassId == classId.Value);

            if (subjectId.HasValue)
                query = query.Where(e => e.SubjectId == subjectId.Value);

            if (studentId.HasValue)
                query = query.Where(e => e.StudentId == studentId.Value);

            if (!string.IsNullOrWhiteSpace(assessmentType))
                query = query.Where(e => e.AssessmentType == assessmentType);

            var results = await query.ToListAsync();
            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetEvaluationById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _evaluationService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Evaluation entity)
        {
            await _evaluationService.CreateAsync(entity);
            return CreatedAtRoute("GetEvaluationById", new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Evaluation entity)
        {
            if (id != entity.Id) return BadRequest(new { message = "ID mismatch." });
            await _evaluationService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _evaluationService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Evaluation deleted." });
        }
    }
}
