namespace Arak.BLL.DTOs.Auth
{
    /// <summary>
    /// The response body for a successful POST /api/auth/login.
    /// Must match the shape documented in BACKEND.md §Authentication (login response).
    ///
    /// The frontend reads:
    ///   response.data.token  → stored in localStorage / sessionStorage
    ///   response.data.user   → stored in context
    ///
    /// The "user" sub-object is included as flat properties here and
    /// assembled into the { user: {...} } shape by the controller.
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        // ── User object fields (BACKEND.md §1 Critical Notice) ────────────────
        // role must be the string NAME (e.g. "Super Admin"), NOT an ID.
        // The frontend does:  if (user.role === 'Super Admin') return true;

        public int    UserId   { get; set; }
        public string Name     { get; set; } = string.Empty;
        public string Email    { get; set; } = string.Empty;

        /// <summary>
        /// Role NAME string — must match exactly one of:
        /// "Super Admin" | "Admin" | "Academic Admin" | "Teacher" | "Fees Admin" | "Users Admin" | "Parent"
        /// </summary>
        public string Role     { get; set; } = string.Empty;

        /// <summary>
        /// Avatar initials — derived from Name (e.g. "Ahmed Saady" → "AS")
        /// </summary>
        public string Avatar   { get; set; } = string.Empty;
    }
}
