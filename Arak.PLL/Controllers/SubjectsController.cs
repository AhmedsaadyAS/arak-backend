using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Academic Admin")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() => Ok(await _subjectService.GetAllAsync());

        [HttpGet("{id:int}", Name = "GetSubjectById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _subjectService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Subject entity)
        {
            await _subjectService.CreateAsync(entity);
            return CreatedAtRoute("GetSubjectById", new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, Subject entity)
        {
            if (id != entity.Id) return BadRequest("ID Mismatch");
            await _subjectService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _subjectService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Subject deleted." });
        }
    }
}
