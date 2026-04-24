using Arak.BLL.DTOs.Auth;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Arak.PLL.Controllers
{
    /// <summary>
    /// Authentication controller — the ONLY controller without [Authorize].
    /// POST /api/auth/login is the single public endpoint per BACKEND.md §7.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }

        /// <summary>
        /// POST /api/auth/login
        ///
        /// Request body: { "email": "admin@arak.com", "password": "Admin@123" }
        ///
        /// Success 200:
        /// {
        ///   "token": "eyJ...",
        ///   "expiration": "2026-04-12T21:00:00Z",
        ///   "user": {
        ///     "id": 1, "name": "...", "email": "...", "role": "Admin",
        ///     "avatar": "AD", "roleId": 2
        ///   }
        /// }
        ///
        /// Failure 401: { "message": "Invalid email or password" }
        /// </summary>
        [HttpPost("login")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (result is null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Resolve roleId from role name using RoleManager
            var roleId = await ResolveRoleIdAsync(result.Role);

            // Shape the response to match exactly what BACKEND.md §Auth specifies
            return Ok(new
            {
                token = result.Token,
                expiration = result.Expiration,
                user = new
                {
                    id     = result.UserId,
                    name   = result.Name,
                    email  = result.Email,
                    role   = result.Role,
                    avatar = result.Avatar,
                    roleId = roleId
                }
            });
        }

        /// <summary>
        /// Resolve a role name string to its integer ID from the database.
        /// Falls back to 0 if the role is not found.
        /// </summary>
        private async Task<int> ResolveRoleIdAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null && int.TryParse(role.Id, out var id))
                return id;

            // Fallback mapping — keep in sync with DbInitializer role creation order
            return roleName switch
            {
                "Super Admin"    => 1,
                "Admin"          => 2,
                "Academic Admin" => 3,
                "Teacher"        => 4,
                "Fees Admin"     => 5,
                "Users Admin"    => 6,
                "Parent"         => 7,
                _                => 0
            };
        }

        /// <summary>
        /// POST /api/auth/logout
        /// Since we use stateless JWT, logout is handled client-side (clear token).
        /// This endpoint exists to satisfy the API contract and allow future
        /// server-side token revocation (refresh token blacklist).
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Stateless JWT — client clears localStorage / sessionStorage
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// GET /api/auth/me
        /// Used by the frontend on app initialization to verify the token is valid
        /// and to fetch the current user's profile and roles.
        /// </summary>
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            var email  = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var name   = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var role   = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userId == null) return Unauthorized(new { message = "Invalid token claims" });

            // Parse userId to int for consistency with login response
            int.TryParse(userId, out var parsedUserId);

            // Resolve roleId from role name
            var roleId = role != null ? await ResolveRoleIdAsync(role) : 0;

            return Ok(new
            {
                user = new
                {
                    id     = parsedUserId,
                    name   = name ?? string.Empty,
                    email  = email ?? string.Empty,
                    role   = role ?? string.Empty,
                    roleId = roleId,
                    // Re-calculate avatar
                    avatar = string.IsNullOrWhiteSpace(name) ? "U" : (name.Length >= 2 ? name[..2].ToUpper() : name.ToUpper())
                }
            });
        }
    }
}
