using Arak.BLL.Service.Abstraction;
using Arak.DAL.Database;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly AppDbContext _context;

        public EventsController(IEventService eventService, AppDbContext context)
        {
            _eventService = eventService;
            _context = context;
        }

        // GET /api/events?startDate=2025-01-01&endDate=2025-12-31
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate   = null)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(startDate) && DateOnly.TryParse(startDate, out var sd))
                query = query.Where(e => e.Date >= sd);

            if (!string.IsNullOrWhiteSpace(endDate) && DateOnly.TryParse(endDate, out var ed))
                query = query.Where(e => e.Date <= ed);

            var results = await query.ToListAsync();
            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetEventById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _eventService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ArakEvent entity)
        {
            await _eventService.CreateAsync(entity);
            return CreatedAtRoute("GetEventById", new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, ArakEvent entity)
        {
            if (id != entity.Id) return BadRequest("ID Mismatch");
            await _eventService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _eventService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Event deleted." });
        }
    }
}
