using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Notificators
{
    public class TelegramSender(ILogger<TelegramSender> logger) : NotificatorBase(logger)
    {
        public override bool CanHandle(int notificationType) => true;

        protected override Task<NotificationResponse> SendAsync(NotificationRequest notification)
        {
            var response = new NotificationResponse(notification.Notification, "TelegramSender");
            return Task.FromResult(response);

        }
    }

}
