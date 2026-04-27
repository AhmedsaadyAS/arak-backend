using System;

namespace Arak.BLL.DTO
{
    public class DtoMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead => ReadAt.HasValue;
    }
}
