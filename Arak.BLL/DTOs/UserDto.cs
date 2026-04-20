namespace Arak.BLL.DTOs
{
    /// <summary>
    /// ApplicationUser response DTO for the /api/users endpoint (User Management page).
    /// </summary>
    public class UserDto
    {
        public string Id          { get; init; } = string.Empty;
        public string Name        { get; init; } = string.Empty;
        public string Email       { get; init; } = string.Empty;
        public string? PhoneNumber{ get; init; }
        public string? Role       { get; init; }
        public string? Address    { get; init; }
        public bool   IsActive    { get; init; } = true;
    }

    public class CreateUserDto
    {
        public string  Name     { get; init; } = string.Empty;
        public string  Email    { get; init; } = string.Empty;
        public string  Password { get; init; } = string.Empty;
        public string  Role     { get; init; } = string.Empty;
        public string? Address  { get; init; }
        public string? PhoneNumber { get; init; }
    }

    public class UpdateUserDto
    {
        public string? Name        { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address     { get; init; }
        public string? Role        { get; init; }
        public string? Password    { get; init; }
    }
}
