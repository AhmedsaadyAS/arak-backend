using System.ComponentModel.DataAnnotations;

namespace Arak.BLL.DTOs.Auth
{
    /// <summary>
    /// The request body for POST /api/auth/login.
    /// Must match the shape documented in BACKEND.md §Authentication.
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
