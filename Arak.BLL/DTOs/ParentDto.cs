namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Flattened Parent DTO — merges Parent entity + ApplicationUser profile fields.
    /// Includes StudentIds array (not just count) so the frontend can link parent ↔ students.
    /// </summary>
    public class ParentDto
    {
        public int     Id           { get; init; }
        public string  Name         { get; init; } = string.Empty;
        public string  Email        { get; init; } = string.Empty;
        public string? PhoneNumber  { get; init; }
        public string? Address      { get; init; }
        /// <summary>
        /// IDs of all students linked to this parent.
        /// Frontend reads `parent.studentIds` for parent-student linking.
        /// </summary>
        public IEnumerable<int> StudentIds   { get; init; } = [];
        public int              StudentCount { get; init; }   // convenience count
    }
}
