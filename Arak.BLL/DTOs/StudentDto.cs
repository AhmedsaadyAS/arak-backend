namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Flattened Student response DTO — maps StudentCode → studentId for frontend contract.
    /// All navigation properties are resolved and embedded; no circular references.
    /// </summary>
    public class StudentDto
    {
        public int    Id          { get; init; }
        /// <summary> Maps to Student.StudentCode — the human-readable display ID (e.g. STU-4A-01). </summary>
        public string StudentId   { get; init; } = string.Empty;
        public string Name        { get; init; } = string.Empty;
        public string UserName    { get; init; } = string.Empty;
        public int    Age         { get; init; }
        public string Email       { get; init; } = string.Empty;
        public string DateOfBirth { get; init; } = string.Empty;
        public string PlaceOfBirth{ get; init; } = string.Empty;
        public string Address     { get; init; } = string.Empty;
        public string City        { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Grade       { get; init; } = string.Empty;
        public string Status      { get; init; } = "Active";
        public string? Image      { get; init; }

        // Resolved FK fields
        public int?   ClassId     { get; init; }
        public string? ClassName  { get; init; }
        public int?   ParentId    { get; init; }
        public string? ParentName { get; init; }
    }

    /// <summary>
    /// DTO for creating a new student without exposing internal entity structure.
    /// </summary>
    public class CreateStudentDto
    {
        public string StudentId    { get; set; } = string.Empty;
        public string Name         { get; set; } = string.Empty;
        public string UserName     { get; set; } = string.Empty;
        public string Password     { get; set; } = string.Empty; // Required for identity creation
        public int    Age          { get; set; }
        public string Email        { get; set; } = string.Empty;
        public string DateOfBirth  { get; set; } = string.Empty;
        public string PlaceOfBirth { get; set; } = string.Empty;
        public string Address      { get; set; } = string.Empty;
        public string City         { get; set; } = string.Empty;
        public string PhoneNumber  { get; set; } = string.Empty;
        public string Grade        { get; set; } = string.Empty;
        public string Status       { get; set; } = "Active";
        public string? Image       { get; set; }
        
        public int?   ClassId      { get; set; }
        public int?   ParentId     { get; set; }
    }
}
