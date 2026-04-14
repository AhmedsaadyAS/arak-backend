namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Flattened Teacher DTO — merges Teacher entity + ApplicationUser profile fields.
    /// The raw Teacher entity has no name/email; they live on the linked ApplicationUser.
    /// </summary>
    public class TeacherDto
    {
        public int     Id             { get; init; }         // TeacherId
        public string  TeacherId      { get; init; } = string.Empty; // Display code e.g. "TCH-001"
        public string  Name           { get; init; } = string.Empty;
        public string  Email          { get; init; } = string.Empty;
        public string? PhoneNumber    { get; init; }
        public string? Address        { get; init; }
        public string? Subject        { get; init; }
        public string? Department     { get; init; }
        public int?    Experience     { get; init; }
        public string? City           { get; init; }
        public int?    SubjectId      { get; init; }
        /// <summary>
        /// IDs of classes assigned to this teacher via TeacherClass junction table.
        /// The frontend scheduleService.js reads and patches this field.
        /// </summary>
        public int[]   AssignedClasses { get; init; } = [];
    }

    /// <summary>Request body for PATCH /api/teachers/{id}</summary>
    public class PatchTeacherDto
    {
        public string?  Name            { get; init; }
        public string?  PhoneNumber     { get; init; }
        public string?  Address         { get; init; }
        public int?     SubjectId       { get; init; }
        /// <summary>
        /// When provided, completely replaces the teacher's class assignments
        /// in the TeacherClass junction table.
        /// </summary>
        public int[]?   AssignedClasses { get; init; }
    }
}
