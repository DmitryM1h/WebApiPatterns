using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Interfaces
{
    public interface INotificator
    {
        public bool CanHandle(int notificationType);
        public Task<NotificationResponse> SendNotificationAsync(NotificationRequest notification);

    }
}
