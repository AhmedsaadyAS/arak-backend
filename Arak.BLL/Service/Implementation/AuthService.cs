using Arak.BLL.DTOs.Auth;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Arak.BLL.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Attempts to authenticate the user.
        /// Returns null if credentials are invalid so the controller can return 401.
        /// </summary>
        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            // 1. Find user by email (case-insensitive via UserManager default normalizer)
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return null;

            // 2. Verify password using ASP.NET Core Identity (bcrypt under the hood)
            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return null;

            // 3. Get the user's roles (returns list — we take the first/highest)
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                // Security: A user with no roles should not gain admin privileges by default.
                // Return null to trigger a 401 — admin must assign a role first.
                return null;
            }
            var primaryRole = roles.First();

            // 4. Build JWT claims
            //    BACKEND.md §1 Critical Notice: role claim must be the string NAME
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name,               user.Name ?? user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role,               primaryRole),
            };

            // 5. Read JWT settings from appsettings.json
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            {
                // JWT signing key must be at least 256 bits (32 bytes) for HMAC-SHA256
                throw new InvalidOperationException("Jwt:Key configuration is missing or too short (minimum 32 characters).");
            }
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var h) ? h : 24;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(expirationHours);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 6. Build avatar initials from the Name field (e.g. "Ahmed Saady" → "AS")
            var avatar = BuildAvatar(user.Name ?? user.UserName ?? "U");

            // 7. Return the full response DTO that matches BACKEND.md login response shape
            return new AuthResponseDto
            {
                Token = tokenString,
                Expiration = expiration,
                // These flat fields are wrapped into a "user" object by the controller
                UserId = int.TryParse(user.Id, out var uid) ? uid : 0,
                Name = user.Name ?? user.UserName ?? string.Empty,
                Email = user.Email!,
                Role = primaryRole,   // String name — critical for frontend RBAC
                Avatar = avatar,
            };
        }

        // ── Private helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Builds initials from a display name.
        /// "Maria Historica" → "MH" | "Admin" → "AD"
        /// </summary>
        private static string BuildAvatar(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : name.Length >= 2 ? name[..2].ToUpper() : name.ToUpper();
        }
    }
}
