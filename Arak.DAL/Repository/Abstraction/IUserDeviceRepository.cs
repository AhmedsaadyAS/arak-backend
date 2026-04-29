using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Abstraction
{
    /// <summary>
    /// Repository contract for managing user FCM device tokens.
    /// Supports multi-device push: one user can have multiple rows.
    /// </summary>
    public interface IUserDeviceRepository : IGenericRepository<UserDevice>
    {
        /// <summary>
        /// Inserts a new device token or updates LastSeenAt if the token already exists.
        /// </summary>
        Task UpsertAsync(string userId, string fcmToken, string? deviceName);

        /// <summary>
        /// Removes a specific FCM token (e.g. on logout or token refresh).
        /// </summary>
        Task DeleteByTokenAsync(string userId, string fcmToken);

        /// <summary>
        /// Returns all active FCM tokens for a user (across all their devices).
        /// </summary>
        Task<IEnumerable<string>> GetTokensForUserAsync(string userId);
    }
}
