namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Flattened Class DTO for the /api/classes endpoint.
    /// Includes computed StudentCount and the GradesLocked flag.
    /// </summary>
    public class ClassDto
    {
        public int    Id           { get; init; }
        public string Name         { get; init; } = string.Empty;
        public string? Grade       { get; init; }   // e.g. "Grade 4"
        public string? Stage       { get; init; }   // "primary" | "preparatory" | "secondary"
        public bool   GradesLocked { get; init; }
        public string? Description { get; init; }
        public int    MaxStudents  { get; init; }   // Maximum student capacity
        public int    StudentCount { get; init; }   // computed from Students.Count
    }

    /// <summary>Request body for POST /classes and PUT /classes/{id}</summary>
    public class CreateClassDto
    {
        public string  Name        { get; init; } = string.Empty;
        public string? Grade       { get; init; }
        public string? Stage       { get; init; }
        public string? Description { get; init; }
        public int?    MaxStudents { get; init; }
    }

    /// <summary>Request body for PATCH /classes/{id}</summary>
    public class PatchClassDto
    {
        public string? Name         { get; init; }
        public string? Grade        { get; init; }
        public string? Stage        { get; init; }
        public string? Description  { get; init; }
        public bool?   GradesLocked { get; init; }
        public int?    MaxStudents  { get; init; }
    }
}
