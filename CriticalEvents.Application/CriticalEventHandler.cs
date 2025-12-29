using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Interfaces;
using CriticalEvents.Domain.Services;
using CriticalEvents.Domain.Services.Requests;


namespace CriticalEvents.Application
{
    public class CriticalEventHandler(CrititicalEventsProcessor _eventProcessor, IAccidentStorage storage)
    {
        public async Task Handle(CriticalEventRequest processEventRequest)
        { 
            var criticalEvent = CriticalEvent.Create(processEventRequest);

            await _eventProcessor.ReceiveCriticalEvent(criticalEvent);

        }
        
    }
}