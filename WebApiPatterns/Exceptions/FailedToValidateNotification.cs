namespace WebApiPatterns.Exceptions
{
    public class FailedToValidateNotification : Exception
    {
        public FailedToValidateNotification(string msg) : base(msg)
        { }
    }
}
