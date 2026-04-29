using Arak.BLL.DTO;
using Arak.BLL.Service.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Arak.PLL.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Context.UserIdentifier is automatically populated from the NameIdentifier claim
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }

    /// <summary>
    /// Implements the BLL's interface so the BLL can trigger SignalR without referencing Microsoft.AspNetCore.SignalR directly
    /// or causing a circular dependency to PLL.
    /// </summary>
    public class NotificationHubDispatcher : INotificationHubDispatcher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubDispatcher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, DtoNotification notification)
        {
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
        }
    }
}
