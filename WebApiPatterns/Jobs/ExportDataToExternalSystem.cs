using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class ExportDataToExternalSystem : IJobHandler<ExportDataCommand>
    {
        public async Task ExecuteJob(ExportDataCommand command)
        {
            // Если один пользователь уже запустил эту задачу, то не запускать еще раз?
            // Добавить возможность отменить задачу
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine("Задача готова!");
        }
    }
}
