using WebApiPatterns.Application.Dtos;
using WebApiPatterns.Exceptions;
using WebApiPatterns.Interfaces;
using WebApiPatterns.Validators;

namespace WebApiPatterns.Application
{
    public class NotificationDispatcher(IEnumerable<INotificator> _notificators,
                                        NotificationValidator _validator,
                                        ILogger<NotificationDispatcher> _logger)
    {
        public async Task<IEnumerable<NotificationResponse>> SendAsync(NotificationRequest notification)
        {
            try
            {
                _validator.ThrowIfInvalid(notification);

                var notificators = _notificators.Where(t => t.CanHandle(notification.Type));

                //Излишний параллелизм
                var tasks = notificators.Select(t => t.SendNotificationAsync(notification));

                var replies = await Task.WhenAll(tasks);

                return [.. replies];
            }
            catch(FailedToValidateNotification ex) 
            {
                _logger.LogError("Failed to send notification {@exceptionInfo}", new { notificationType = notification.Type, exceptionInfo = ex.ToString()});

                throw;
            }
        }
    }
}
