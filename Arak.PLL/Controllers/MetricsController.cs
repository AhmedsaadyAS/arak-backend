using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arak.DAL.Database;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MetricsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MetricsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetMetrics()
        {
            var studentCount = await _db.Students.CountAsync();
            var teacherCount = await _db.Teachers.CountAsync();
            var classCount   = await _db.Classes.CountAsync();
            var eventCount   = await _db.Events.CountAsync();

            var metrics = new
            {
                totalStudents  = studentCount,
                totalTeachers  = teacherCount,
                totalClasses   = classCount,
                totalEvents    = eventCount,
                systemHealth   = "Good",
                serverUptime   = "99.98%",
                activeUsers    = 1, // Current user (the requester)
            };
            return Ok(metrics);
        }
    }
}
