using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin,Users Admin")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
        {
            try
            {
                var result = await _attendanceService.MarkAttendanceAsync(dto);
                return CreatedAtAction(nameof(MarkAttendance), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkMarkAttendance([FromBody] BulkMarkAttendanceDto dto)
        {
            try
            {
                var result = await _attendanceService.BulkMarkAttendanceAsync(dto);
                return CreatedAtAction(nameof(BulkMarkAttendance), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("class/{classId:int}")]
        public async Task<IActionResult> GetClassAttendanceByDate(
            int classId, 
            [FromQuery] DateOnly? date = null, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 30)
        {
            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var result = await _attendanceService.GetClassAttendanceByDateAsync(classId, targetDate, page, pageSize);
            return Ok(result);
        }

        [HttpGet("student/{studentId:int}")]
        public async Task<IActionResult> GetStudentAttendanceByMonth(
            int studentId, 
            [FromQuery] int? month = null, 
            [FromQuery] int? year = null)
        {
            var targetMonth = month ?? DateTime.Today.Month;
            var targetYear = year ?? DateTime.Today.Year;
            
            var result = await _attendanceService.GetStudentAttendanceByMonthAsync(studentId, targetMonth, targetYear);
            return Ok(result);
        }

        [HttpGet("student/{studentId:int}/stats")]
        public async Task<IActionResult> GetStudentStats(int studentId)
        {
            var result = await _attendanceService.GetStudentStatsAsync(studentId);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] UpdateAttendanceDto dto)
        {
            try
            {
                var result = await _attendanceService.UpdateAttendanceAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("bulk-timeout")]
        public async Task<IActionResult> BulkUpdateTimeOut([FromBody] BulkUpdateTimeOutDto dto)
        {
            var count = await _attendanceService.BulkUpdateTimeOutAsync(dto);
            return Ok(new { updated = count });
        }
    }
}
