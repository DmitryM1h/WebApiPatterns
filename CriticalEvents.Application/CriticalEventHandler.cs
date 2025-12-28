using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Interfaces;
using CriticalEvents.Domain.Services;
using CriticalEvents.Domain.Services.Requests;


namespace CriticalEvents.Application
{
    public class CriticalEventHandler(CrititicalEventsProcessor _eventProcessor, IAccidentStorage storage)
    {
        public void Handle(CriticalEventRequest processEventRequest)
        { 
            var criticalEvent = CriticalEvent.Create(processEventRequest);

            _eventProcessor.ProcessCriticalEvent(criticalEvent, storage);

        }
        
    }
}