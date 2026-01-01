using CriticalEvents.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Domain.Services.Requests
{
    public record CriticalEventRequest(string Description, CriticalEventType Type);

}
