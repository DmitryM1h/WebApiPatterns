using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Interfaces;
using System.Threading.Channels;

namespace CriticalEvents.Persistence
{
    public class AccidentStorage : IAccidentStorage
    {
        private Channel <Accident> _eventsStorage = Channel.CreateUnbounded<Accident>();

        public async Task StoreEvent(Accident accident)
        {
            await _eventsStorage.Writer.WriteAsync(accident);
        }
    }
}
