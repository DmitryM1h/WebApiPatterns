using WebApiPatterns.Application.Dtos;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Notificators
{
    public abstract class NotificatorBase(ILogger logger) : INotificator
    {
        public async Task<NotificationResponse> SendNotificationAsync(NotificationRequest notification)
        {
            try
            {
                return await SendAsync(notification);
            }
            catch (Exception ex)
            {
                logger.LogError("Ошибка отправки {@notification}", new { notification.UserId, notification.Notification, ex.Message, ex.StackTrace, sender = GetType() });
            }
            return new NotificationResponse(notification.Notification, $"Not received by {GetType()}");
        }

        protected abstract Task<NotificationResponse> SendAsync(NotificationRequest notification);
        public abstract bool CanHandle(int notificationType);
    }
}
