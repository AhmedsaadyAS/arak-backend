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
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;

        public EventsController(IEventService eventService, INotificationService notificationService, AppDbContext context)
        {
            _eventService = eventService;
            _notificationService = notificationService;
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

            // ── Announcement Fanout ──
            if (entity.ClassId.HasValue)
            {
                var parentUserIds = await _context.Students
                    .Include(s => s.Parent)
                    .ThenInclude(p => p.ApplicationUser)
                    .Where(s => s.ClassId == entity.ClassId.Value && s.Parent != null && s.Parent.ApplicationUser != null)
                    .Select(s => s.Parent!.ApplicationUser!.Id)
                    .Distinct()
                    .ToListAsync();

                foreach (var pId in parentUserIds)
                {
                    _ = _notificationService.SendAsync(
                        recipientUserId: pId,
                        title: "New School Event",
                        body: $"An event '{entity.Title}' has been scheduled for {entity.Date:MMM dd}.",
                        type: "Announcement",
                        referenceId: entity.Id);
                }
            }

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
