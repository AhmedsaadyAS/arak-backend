using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Arak.DAL.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [ForeignKey("SenderId")]
        [JsonIgnore]
        public ApplicationUser? Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        [JsonIgnore]
        public ApplicationUser? Receiver { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
