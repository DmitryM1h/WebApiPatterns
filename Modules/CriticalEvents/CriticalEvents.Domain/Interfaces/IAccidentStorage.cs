using CriticalEvents.Domain.Entities;



namespace CriticalEvents.Domain.Interfaces
{
    public interface IAccidentStorage
    {
        public Task StoreEvent(Accident @event);

    }
}
