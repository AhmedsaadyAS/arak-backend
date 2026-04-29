using Arak.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    /// <summary>
    /// Orchestrates notifications: saves to inbox, pushes to SignalR, and fans out to FCM devices.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Main dispatch method.
        /// 1. Persists to Notification table.
        /// 2. Pushes to connected SignalR clients.
        /// 3. Pushes to all registered FCM devices for the user.
        /// </summary>
        Task SendAsync(string recipientUserId, string title, string body, string type, int? referenceId = null);

        // Inbox management
        Task<IEnumerable<DtoNotification>> GetInboxAsync(string userId, int page, int pageSize);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> MarkAsReadAsync(int id, string userId);
        Task MarkAllAsReadAsync(string userId);
    }
}
