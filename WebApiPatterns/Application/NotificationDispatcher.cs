using WebApiPatterns.Application.Dtos;
using WebApiPatterns.Exceptions;
using WebApiPatterns.Interfaces;
using WebApiPatterns.Validators;

namespace WebApiPatterns.Application
{
    public class NotificationDispatcher(IEnumerable<INotificator> notificationStategies,
                                        NotificationValidator validator,
                                        ILogger<NotificationDispatcher> logger)
    {
        public async Task<IEnumerable<NotificationResponse>> SendAsync(NotificationRequest notification)
        {
            try
            {
                validator.ThrowIfInvalid(notification);

                var notificators = notificationStategies.Where(t => t.CanHandle(notification.Type));

                var tasks = notificators.Select(t => t.SendNotificationAsync(notification));

                var replies = await Task.WhenAll(tasks);

                return [.. replies];
            }
            catch(FailedToValidateNotification ex) 
            {
                logger.LogError("Failed to send notification {@exceptionInfo}", new { notificationType = notification.Type, exceptionInfo = ex.ToString()});

                throw;
            }
        }
    }
}
