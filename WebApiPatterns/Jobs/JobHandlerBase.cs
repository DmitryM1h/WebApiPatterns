using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebApiPatterns.Application;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs
{
    public abstract class JobHandlerBase<ICommand> where ICommand : CommandBase
    {
        private string Initiator { get; set; } = null!;
        protected int ProgressPercent { get; set; }

        private readonly IHubContext<NotificationHub> HubContext;

        private static ConcurrentDictionary<string, CancellationTokenSource> activeTasks = new();

        ILogger<JobHandlerBase<ICommand>> _logger;

        protected JobHandlerBase(IServiceProvider serviceProvider, string initiator)
        {
            Initiator = initiator;
            ProgressPercent = 0;
            HubContext = serviceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            _logger = serviceProvider.GetRequiredService<ILogger<JobHandlerBase<ICommand>>>();

            var src = new CancellationTokenSource();

            src.Token.Register(async () => await NotifyCancel());

            activeTasks[initiator] = src;

        }


        public async Task ExecuteJob(ICommand command)
        {
            try
            {
                await foreach (var _ in ExecuteJobAsync(command))
                {
                    await NotifyProgress();

                    await Task.Delay(20);

                    ThrowIfTaskCancelled();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Application job failed {details}", ex.ToString());
                await NotifyError();
            }

            activeTasks[Initiator].Dispose();
        }

        protected abstract IAsyncEnumerable<int> ExecuteJobAsync(ICommand command);

        protected async Task NotifyProgress()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", new { Initiator, ProgressPercent });
        }
        protected async Task NotifyCancel()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskCancelled", $"Задача отменена пользователем {Initiator}");
        }
        protected async Task NotifyError()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskReceiveError", $"Задача завершена с ошибкой. Инциатор: {Initiator}");
        }

        public static void CancelTask(string initiator)
        {
            activeTasks[initiator].Cancel();

            activeTasks[initiator].Dispose();
        }

        protected void ThrowIfTaskCancelled()
        {
            activeTasks[Initiator].Token.ThrowIfCancellationRequested();
        }
    }
}
