namespace Arak.DAL.Entities
{
    /// <summary>
    /// Represents a school event (holiday, exam, meeting, sports, cultural).
    /// Named ArakEvent to avoid conflict with the C# system keyword "Event".
    /// </summary>
    public class ArakEvent
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// One of: "Holiday", "Exam", "Meeting", "Sports", "Cultural"
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
