using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Notificators
{
    public class EmailSender(ILogger<EmailSender> logger) : NotificatorBase(logger)
    {
        public override bool CanHandle(int notificationType) => true;

        protected override Task<NotificationResponse> SendAsync(NotificationRequest notification)
        {
            var response = new NotificationResponse(notification.Notification, "EmailSender");

            return Task.FromResult(response);
        }

    }
}
