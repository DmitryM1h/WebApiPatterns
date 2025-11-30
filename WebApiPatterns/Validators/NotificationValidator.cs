using WebApiPatterns.Application.Dtos;
using WebApiPatterns.Exceptions;

namespace WebApiPatterns.Validators
{
    public class NotificationValidator
    {
        private readonly List<int> AcceptableTypes = [1, 2];
        public void ThrowIfInvalid(NotificationRequest notificationDto)
        {
            if (!AcceptableTypes.Contains(notificationDto.Type))
                throw new FailedToValidateNotification("Invalid notification type");
        }
    }
}
