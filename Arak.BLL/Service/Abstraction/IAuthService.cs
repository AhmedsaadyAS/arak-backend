using Arak.BLL.DTOs.Auth;

namespace Arak.BLL.Service.Abstraction
{
    public interface IAuthService
    {
        /// <summary>
        /// Validates credentials and returns a JWT response DTO,
        /// or null if the email/password combination is incorrect.
        /// </summary>
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
