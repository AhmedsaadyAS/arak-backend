using Arak.BLL.DTOs;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// User Management — wraps ASP.NET Core Identity UserManager.
    /// Serves the /api/users endpoints consumed by UserManagement.jsx.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Super Admin,Admin,Users Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole>   _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole>   roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET /api/users?email=X&role=Y
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string? email = null,
            [FromQuery] string? role = null)
        {
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(email))
            {
                // Exact match for login lookup — not partial
                query = query.Where(u => u.Email == email);
            }
            if (!string.IsNullOrWhiteSpace(role))
            {
                // Filter by role — load all users then filter by role membership
                var users = await query.ToListAsync();
                var filtered = new List<ApplicationUser>();
                foreach (var u in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(u);
                    if (userRoles.Contains(role))
                        filtered.Add(u);
                }
                var dtos = new List<UserDto>();
                foreach (var u in filtered)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    dtos.Add(MapToDto(u, roles.FirstOrDefault()));
                }
                return Ok(dtos);
            }

            var allUsers = await query.ToListAsync();
            var allDtos = new List<UserDto>();
            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                allDtos.Add(MapToDto(u, roles.FirstOrDefault()));
            }
            return Ok(allDtos);
        }

        // GET /api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found." });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(MapToDto(user, roles.FirstOrDefault()));
        }

        // POST /api/users  — create a new user account
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName       = dto.Email,
                Email          = dto.Email,
                Name           = dto.Name,
                Address        = dto.Address,
                PhoneNumber    = dto.PhoneNumber,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            if (!string.IsNullOrWhiteSpace(dto.Role) && await _roleManager.RoleExistsAsync(dto.Role))
                await _userManager.AddToRoleAsync(user, dto.Role);

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(MapToDto(user, roles.FirstOrDefault()));
        }

        // PATCH /api/users/{id}  — update profile (name, phone, address, role)
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found." });

            if (dto.Name        is not null) user.Name        = dto.Name;
            if (dto.PhoneNumber is not null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.Address     is not null) user.Address     = dto.Address;

            await _userManager.UpdateAsync(user);

            // Update role if provided
            if (!string.IsNullOrWhiteSpace(dto.Role) && await _roleManager.RoleExistsAsync(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(MapToDto(user, roles.FirstOrDefault()));
        }

        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found." });

            await _userManager.DeleteAsync(user);
            return Ok(new { message = "User deleted." });
        }

        // ── DTO mapping ───────────────────────────────────────────────────

        private static UserDto MapToDto(ApplicationUser u, string? role) => new()
        {
            Id          = u.Id,
            Name        = u.Name        ?? string.Empty,
            Email       = u.Email       ?? string.Empty,
            PhoneNumber = u.PhoneNumber,
            Address     = u.Address,
            Role        = role,
            IsActive    = !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow,
        };
    }
}
