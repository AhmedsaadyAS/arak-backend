using System;

namespace Arak.BLL.DTO
{
    public class DtoConversation
    {
        public string ParticipantId { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public DtoConversationParticipant Participant { get; set; }
    }
}
