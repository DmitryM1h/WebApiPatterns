using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Notificators
{
    public class SmsSenderSecond(ILogger<SmsSenderSecond> logger) : NotificatorBase(logger)
    {
        public override bool CanHandle(int notificationType) => notificationType == 2;

        protected override Task<NotificationResponse> SendAsync(NotificationRequest notification)
        {
            var response = new NotificationResponse(notification.Notification, "SmsSenderSecond");

            return Task.FromResult(response);
        }
    }
}
