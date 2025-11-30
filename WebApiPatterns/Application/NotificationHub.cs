using Microsoft.AspNetCore.SignalR;

namespace WebApiPatterns.Application
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Conntection accepted  " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Conntection aborted  " + Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

    }
}
