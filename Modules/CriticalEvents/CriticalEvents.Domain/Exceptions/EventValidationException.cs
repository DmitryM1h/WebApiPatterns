using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Domain.Exceptions
{
    public class EventValidationException : Exception
    {
        public EventValidationException() : base("Invalid event") { }
    }
}
