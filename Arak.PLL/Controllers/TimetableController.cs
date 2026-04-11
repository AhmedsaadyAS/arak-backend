using Arak.BLL.Service.Abstraction;
using Arak.BLL.Service.Implementation;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ITimetableService _timetableService;
        public TimetableController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        [HttpGet("GetTimetablesByClassId/{classId}")]
        public async Task<IActionResult> GetTimetableByClassId(int classId)
        {
            var timetables = await _timetableService.GetTimetableByClassId(classId);
            if (timetables.Any() == false)
            {
                return NotFound($"The ClassId {classId} is invalid!");
            }

            return Ok(timetables);
        }

        [HttpGet("GetTimetablesByTeacherId/{teacherId}")]
        public async Task<IActionResult> GetTimetableByTeacherId(int teacherId)
        {
            var timetables = await _timetableService.GetTimetableByTeacherId(teacherId);
            if (timetables.Any() == false)
            {
                return NotFound($"The TeacherId {teacherId} is invalid!");
            }

            return Ok(timetables);
        }

        [HttpGet("GetTimetableInStudent/{StudentClassId}")]
        public async Task<IActionResult> GetTimetableInStudent(int TimeClassId, int StudentClassId)
        {
            var timetable = await _timetableService.GetTimetableByClassId(TimeClassId);
            if (timetable.Any() == true && TimeClassId == StudentClassId)
            {
                return Ok(timetable);
            }

            return NotFound("There is a problem!");
        }

        [HttpPost]
        public async Task<IActionResult> AddLesson(TimeTable timeTable)
        {
            await _timetableService.AddLesson(timeTable);
            return Ok(timeTable);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int Id, TimeTable timeTable)
        {
            if (Id != timeTable.Id)
            {
                return NotFound($"The Id {Id} is invalid!");
            }
            var Std = await _timetableService.UpdateAsync(timeTable);
            return Ok(timeTable);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int Id)
        {
            var result = await _timetableService.DeleteAsync(Id);
            if (!result)
            {
                return NotFound($"The Id {Id} is invalid!");
            }

            return Ok("Lesson had been deleted successfully!");
        }
    }
}
