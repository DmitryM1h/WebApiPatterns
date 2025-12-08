namespace WebApiPatterns.Interfaces
{
    public interface IJobHandler<ICommand> where ICommand : CommandBase
    {
       public Task ExecuteJob(ICommand command);
    }
}
