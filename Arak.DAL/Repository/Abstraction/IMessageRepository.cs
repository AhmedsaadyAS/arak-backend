using Arak.DAL.Entities;
using Arak.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Abstraction
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2, int page, int pageSize);
        Task<IEnumerable<ConversationResult>> GetUserConversationsAsync(string userId);
        Task<bool> UserExistsAsync(string userId);
        Task<Message?> GetByIdWithUsersAsync(int id);
        Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId);
    }
}
