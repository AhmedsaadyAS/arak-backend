using Arak.BLL.DTOs.Auth;
using Arak.BLL.Service.Abstraction;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (result is null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Shape the response to match exactly what BACKEND.md §Auth specifies
            // and what the frontend AuthContext.jsx reads.
            return Ok(new
            {
                token = result.Token,
                expiration = result.Expiration,
                // The frontend reads response.data.user.* — wrap in a "user" object
                user = new
                {
                    id     = result.UserId,
                    name   = result.Name,
                    email  = result.Email,
                    role   = result.Role,      // String name — critical! (BACKEND.md §1)
                    avatar = result.Avatar,
                    roleId = 0                  // Placeholder; expand when RoleId is available
                }
            });
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
    }
}
