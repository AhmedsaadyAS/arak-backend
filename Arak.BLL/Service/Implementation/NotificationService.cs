using Arak.BLL.DTO;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserDeviceRepository _deviceRepo;
        private readonly IFcmService _fcmService;
        // The HubContext type will be Arak.PLL.Hubs.NotificationHub, but BLL can't reference PLL.
        // For clean architecture, we use a generic dynamic hub context or inject it via an interface.
        // However, since BLL needs to push, the standard pattern is either a delegate or an interface defined in BLL and implemented in PLL.
        // To keep it simple and avoid circular deps, we can use IHubContext<dynamic> if registered that way,
        // or define an INotificationHubContext in BLL.
        // We'll define INotificationHubDispatcher here to keep it clean.
        private readonly INotificationHubDispatcher _hubDispatcher;

        public NotificationService(
            INotificationRepository notificationRepo,
            IUserDeviceRepository deviceRepo,
            IFcmService fcmService,
            INotificationHubDispatcher hubDispatcher)
        {
            _notificationRepo = notificationRepo;
            _deviceRepo = deviceRepo;
            _fcmService = fcmService;
            _hubDispatcher = hubDispatcher;
        }

        public async Task SendAsync(string recipientUserId, string title, string body, string type, int? referenceId = null)
        {
            // 1. Persist to DB Inbox
            var notification = new Notification
            {
                UserId = recipientUserId,
                Title = title,
                Body = body,
                Type = type,
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationRepo.CreateAsync(notification);
            await _notificationRepo.SaveChangesAsync();

            var dto = MapToDto(notification);

            // 2. Push real-time to SignalR (if user is connected)
            await _hubDispatcher.SendToUserAsync(recipientUserId, dto);

            // 3. Push background/offline to FCM devices
            var tokens = await _deviceRepo.GetTokensForUserAsync(recipientUserId);
            var fcmTasks = tokens.Select(token => 
                _fcmService.SendAsync(token, title, body, new Dictionary<string, string>
                {
                    { "type", type },
                    { "referenceId", referenceId?.ToString() ?? "" },
                    { "notificationId", notification.Id.ToString() }
                })
            );

            await Task.WhenAll(fcmTasks);
        }

        public async Task<IEnumerable<DtoNotification>> GetInboxAsync(string userId, int page, int pageSize)
        {
            var entities = await _notificationRepo.GetUserNotificationsAsync(userId, page, pageSize);
            return entities.Select(MapToDto);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _notificationRepo.GetUnreadCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(int id, string userId)
        {
            return await _notificationRepo.MarkAsReadAsync(id, userId);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _notificationRepo.MarkAllAsReadAsync(userId);
        }

        private static DtoNotification MapToDto(Notification n) => new DtoNotification
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            Type = n.Type,
            ReferenceId = n.ReferenceId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        };
    }

    /// <summary>
    /// Abstracts SignalR away from the BLL.
    /// Implemented in PLL, injected into BLL.
    /// </summary>
    public interface INotificationHubDispatcher
    {
        Task SendToUserAsync(string userId, DtoNotification notification);
    }
}
