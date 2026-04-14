using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Placeholder CRUD controller for Attendance. 
    /// Primarily built to ensure the React frontend's pre-deletion dependency guard works.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        // Currently returning empty arrays as we don't have an Attendance Service implemented yet.
        // This satisfies the `api.js` check `api.getAttendance({ studentId: id })`

        [HttpGet]
        public IActionResult GetAttendance([FromQuery] int? studentId)
        {
            return Ok(new List<object>()); // Empty list means no dependency blocks delete
        }
    }
}
