using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Notificators
{
    public class SmsSenderFirst(ILogger<SmsSenderFirst> logger) : NotificatorBase(logger)
    {
        public override bool CanHandle(int notificationType) => notificationType == 1;

        protected override Task<NotificationResponse> SendAsync(NotificationRequest notification)
        {
            var response = new NotificationResponse(notification.Notification, "SmsSenderFirst");

            return Task.FromResult(response);
        }
    }
}
