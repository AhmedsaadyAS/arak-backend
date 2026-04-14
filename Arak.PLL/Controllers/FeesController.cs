using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Fees Admin")]
    public class FeesController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeesController(IFeeService feeService)
        {
            _feeService = feeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() => Ok(await _feeService.GetAllAsync());

        [HttpGet("{id:int}", Name = "GetFeeById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entity = await _feeService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Fee entity)
        {
            await _feeService.CreateAsync(entity);
            return CreatedAtRoute("GetFeeById", new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, Fee entity)
        {
            if (id != entity.Id) return BadRequest("ID Mismatch");
            await _feeService.UpdateAsync(entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _feeService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Fee deleted." });
        }
    }
}
