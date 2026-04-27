using Arak.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IMessageService
    {
        Task<DtoMessage> SendMessageAsync(DtoSendMessage dto, string senderId, string receiverId);
        Task<IEnumerable<DtoMessage>> GetConversationHistoryAsync(string currentUserId, string otherUserId, int page = 1, int pageSize = 50);
        Task<IEnumerable<DtoConversation>> GetUserConversationsAsync(string currentUserId);
        Task<bool> MarkAsReadAsync(int messageId, string currentUserId);
        Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId);
    }
}
