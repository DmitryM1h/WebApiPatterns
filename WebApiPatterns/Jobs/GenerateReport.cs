using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class GenerateReport : IJobHandler<GenerateReportCommand>
    {
        public async Task ExecuteJob(GenerateReportCommand command)
        {
            // Задача может занимать часы времени. т.е. что то вычислительное и долгое
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

    }
}
