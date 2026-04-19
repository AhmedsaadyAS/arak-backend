using Arak.BLL.DTOs;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ARAK.PLL.Controllers
{
    /// <summary>
    /// Parent CRUD — merges Parent entity with ApplicationUser profile data.
    /// The Parent entity only has FKs; name/email/phone live on ApplicationUser.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Arak.DAL.Database.AppDbContext _db;

        public ParentsController(
            UserManager<ApplicationUser> userManager,
            Arak.DAL.Database.AppDbContext db)
        {
            _userManager = userManager;
            _db          = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var parents = await _db.Parents
                .Include(p => p.ApplicationUser)
                .Include(p => p.Students)
                .ToListAsync();

            return Ok(parents.Select(MapToDto));
        }

        [HttpGet("{id:int}", Name = "GetParentById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var parent = await _db.Parents
                .Include(p => p.ApplicationUser)
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.ParentId == id);

            if (parent == null)
                return NotFound(new { message = $"Parent {id} not found." });

            return Ok(MapToDto(parent));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateParentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { errors = new[] { "Email is required." } });

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            ApplicationUser user;

            if (existingUser != null)
            {
                // User exists — check if parent already linked
                var existingParent = await _db.Parents
                    .FirstOrDefaultAsync(p => p.ApplicationUser != null && p.ApplicationUser.Id == existingUser.Id);
                if (existingParent != null)
                    return BadRequest(new { errors = new[] { "A parent with this email already exists." } });

                user = existingUser;
            }
            else
            {
                user = new ApplicationUser
                {
                    UserName       = dto.Email,
                    Email          = dto.Email,
                    Name           = dto.Name,
                    PhoneNumber    = dto.Phone,
                    Address        = dto.Address,
                    EmailConfirmed = true
                };

                var defaultPassword = Environment.GetEnvironmentVariable("ARAK_DEFAULT_PASSWORD") ?? "Parent@123";
                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            // Ensure user has Parent role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Parent"))
                await _userManager.AddToRoleAsync(user, "Parent");

            var parent = new Parent
            {
                ApplicationUser = user
            };

            _db.Parents.Add(parent);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetParentById", new { id = parent.ParentId }, MapToDto(parent));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateParentDto dto)
        {
            var parent = await _db.Parents
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.ParentId == id);

            if (parent == null) return NotFound(new { message = $"Parent {id} not found." });

            if (parent.ApplicationUser != null)
            {
                parent.ApplicationUser.Name        = dto.Name;
                parent.ApplicationUser.Email       = dto.Email;
                parent.ApplicationUser.UserName    = dto.Email;
                parent.ApplicationUser.PhoneNumber = dto.Phone;
                parent.ApplicationUser.Address     = dto.Address;
                await _userManager.UpdateAsync(parent.ApplicationUser);
            }

            await _db.SaveChangesAsync();
            return Ok(MapToDto(parent));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var parent = await _db.Parents
                .Include(p => p.ApplicationUser)
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.ParentId == id);

            if (parent == null) return NotFound(new { message = $"Parent {id} not found." });

            try
            {
                // 1. Unlink dependent students (set ParentId = null) instead of deleting
                if (parent.Students != null)
                {
                    foreach (var student in parent.Students)
                        student.ParentId = null;
                    await _db.SaveChangesAsync();
                }

                // 2. Remove the Parent domain row
                _db.Parents.Remove(parent);
                await _db.SaveChangesAsync();

                // 3. Remove the Parent role, and only delete Identity user if no other roles remain
                if (parent.ApplicationUser != null)
                {
                    if (await _userManager.IsInRoleAsync(parent.ApplicationUser, "Parent"))
                    {
                        var uid = parent.ApplicationUser.Id;
                        await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserRoles WHERE UserId = '{uid}' AND RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Parent')");
                    }

                    var remainingRoles = await _userManager.GetRolesAsync(parent.ApplicationUser);
                    if (!remainingRoles.Any())
                    {
                        var uid = parent.ApplicationUser.Id;
                        await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserRoles WHERE UserId = '{uid}'");
                        await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserClaims WHERE UserId = '{uid}'");
                        await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserLogins WHERE UserId = '{uid}'");
                        await _db.Database.ExecuteSqlRawAsync($"DELETE FROM AspNetUserTokens WHERE UserId = '{uid}'");

                        await _userManager.DeleteAsync(parent.ApplicationUser);
                    }
                }

                return Ok(new { message = "Parent deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the parent.", error = ex.Message });
            }
        }

        // ── DTO mapping ───────────────────────────────────────────────────

        private static ParentDto MapToDto(Parent p)
        {
            var user = p.ApplicationUser;
            return new ParentDto
            {
                Id           = p.ParentId,
                Name         = user?.Name        ?? string.Empty,
                Email        = user?.Email       ?? string.Empty,
                PhoneNumber  = user?.PhoneNumber ?? null,
                Address      = user?.Address     ?? null,
                StudentIds   = p.Students?.Select(s => s.Id) ?? [],
                StudentCount = p.Students?.Count ?? 0,
            };
        }
    }

    public class CreateParentDto
    {
        public string  Name        { get; init; } = string.Empty;
        public string  Email       { get; init; } = string.Empty;
        public string? Phone       { get; init; }
        public string? Address     { get; init; }
    }
}
