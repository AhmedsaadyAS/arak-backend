using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class FcmService : Abstraction.IFcmService
    {
        private readonly ILogger<FcmService> _logger;

        public FcmService(ILogger<FcmService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null)
        {
            if (string.IsNullOrWhiteSpace(fcmToken)) return;

            var message = new Message
            {
                Token = fcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            try
            {
                // FirebaseApp is initialized once in Program.cs
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (FirebaseMessagingException ex)
            {
                // Silently swallow errors like invalid/expired tokens so the app doesn't crash
                _logger.LogWarning(ex, "FCM Push failed for token {Token}: {Message}", fcmToken, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending FCM to {Token}", fcmToken);
            }
        }
    }
}
