using Microsoft.AspNetCore.SignalR;
using WebApiPatterns.Application;
using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Notificators
{
    public class WebApplicationSender(IHubContext<NotificationHub> _hubContext,
                                      ILogger<WebApplicationSender> logger) : NotificatorBase(logger)
    {
        public override bool CanHandle(int notificationType) => true;

        protected override async Task<NotificationResponse> SendAsync(NotificationRequest notification)
        {
            await _hubContext.Clients.All.SendAsync(notification.Notification);

            return new NotificationResponse(notification.Notification, "WebApplicationSender");
        }
    }
}
