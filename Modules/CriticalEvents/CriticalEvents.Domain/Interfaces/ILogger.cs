using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Domain.Interfaces
{
    public interface ILogger
    {
        public void LogInformation(string message, object? @params = null);
        public void LogError(string message, object? @params = null);

    }
}
