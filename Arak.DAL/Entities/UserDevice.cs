using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    /// <summary>
    /// Stores a single FCM device registration token per physical device.
    /// One user can have many rows — one per device (phone, tablet, etc.).
    /// Upserted on every app login; deleted on logout.
    /// </summary>
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        /// <summary>FCM registration token (unique per device installation).</summary>
        [Required]
        [MaxLength(512)]
        public string FcmToken { get; set; } = string.Empty;

        /// <summary>Human-readable device label sent by the app (e.g. "Galaxy S24"). Optional.</summary>
        [MaxLength(128)]
        public string? DeviceName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Updated on every successful upsert so stale tokens can be identified.</summary>
        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    }
}
