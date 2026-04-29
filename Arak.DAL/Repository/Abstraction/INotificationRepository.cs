using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Abstraction
{
    /// <summary>
    /// Repository contract for the notification inbox.
    /// </summary>
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// Returns a paginated list of notifications for a user, newest first.
        /// </summary>
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int page, int pageSize);

        /// <summary>
        /// Returns the count of unread notifications for a user.
        /// </summary>
        Task<int> GetUnreadCountAsync(string userId);

        /// <summary>
        /// Marks a single notification as read. Returns false if not found or not owned by user.
        /// </summary>
        Task<bool> MarkAsReadAsync(int notificationId, string userId);

        /// <summary>
        /// Marks all notifications for a user as read.
        /// </summary>
        Task MarkAllAsReadAsync(string userId);
    }
}
