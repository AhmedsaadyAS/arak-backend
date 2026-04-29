using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    /// <summary>
    /// Contract for sending push notifications via Firebase Cloud Messaging.
    /// </summary>
    public interface IFcmService
    {
        /// <summary>
        /// Sends a push notification to a specific device token.
        /// Silently swallows errors (e.g., invalid token) to prevent crashing the caller.
        /// </summary>
        Task SendAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
    }
}
