namespace WebApiPatterns.Application.Dtos
{
    public record NotificationRequest(string UserId, string Notification, int Type);

}
