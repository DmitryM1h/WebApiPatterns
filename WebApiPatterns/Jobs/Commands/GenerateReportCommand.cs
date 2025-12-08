using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs.Commands
{
    public class GenerateReportCommand : CommandBase
    {
        string description { get; init; } = null!;

        public GenerateReportCommand(string description)
        {
            this.description = description;
        }
    }
}
