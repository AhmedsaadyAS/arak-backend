using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    /// <summary>
    /// A persisted notification entry in a user's inbox.
    /// Created by NotificationService whenever a domain event triggers a notification.
    /// </summary>
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        /// <summary>The user who should receive this notification.</summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1024)]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Discriminator: "Message" | "NewTask" | "Attendance" | "Announcement" | "Alert"
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Optional reference to the triggering entity (e.g. MessageId, AssignmentId, StudentId).
        /// </summary>
        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
