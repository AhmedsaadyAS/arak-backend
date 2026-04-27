using Arak.DAL.Entities;

namespace Arak.DAL.Models
{
    public class ConversationResult
    {
        public Message LatestMessage { get; set; } = null!;
        public int UnreadCount { get; set; }
    }
}
