using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Threading.Channels;
using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Application
{
    public class CriticalEventHandler
    {
        ILogger<CriticalEventHandler> _logger;
        public CriticalEventHandler(ILogger<CriticalEventHandler> logger, Channel<Accident> processedVents)
        {
            _logger = logger;
            _processedEvents = processedVents;

            typesHandlers[CriticalEventType.type1] = CreateIncidentOne;
            typesHandlers[CriticalEventType.type2] = CreateIncidentTwo;
            typesHandlers[CriticalEventType.type3] = CreateIncidentThree;
        }

        public readonly Channel<Accident> _processedEvents; // Обработанные события пишутся и сюда в очередь и фоновая задача добавляет в базу

        private readonly object _lockObject = new();


        private static readonly ConcurrentDictionary<CriticalEventType, Action<CriticalEvent>> typesHandlers = new();

        public void ProcessCriticalEvent(CriticalEventRequest request)
        {
            CriticalEvent criticalEvent = new(request.id, request.Description, request.Type, DateTime.Now);
            _logger.LogInformation("В обработку получено критическое событие, id = " + criticalEvent.id.ToString() + " типа " + criticalEvent.Type);

            Action<CriticalEvent> handler = typesHandlers[criticalEvent.Type];

            handler(criticalEvent);
        }

        public void CreateIncidentOne(CriticalEvent criticalEvent)
        {
            var accident = new Accident(Guid.NewGuid(), AccidentType.Type1, criticalEvent);
            _logger.LogInformation("Создан инцидент типа 1 на основе события с id " + criticalEvent.id.ToString());

        }
        public void CreateIncidentTwo(CriticalEvent criticalEvent)
        {

            var sourceEventDate = DateTime.Now;
            int secondsToWait = 20;

            async void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("20 секунд еще не прошло, создаем!!");

                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type2, criticalEvent, ce);

                    await _processedEvents.Writer.WriteAsync(accident);

                    _logger.LogInformation("Создан инцидент типа 2 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }

                RemoveTypeHandler(CriticalEventType.type1, LocalHandler);
                AddTypeHandler(CriticalEventType.type1, CreateIncidentOne);
            }

            AddTypeHandler(CriticalEventType.type1, LocalHandler);
            RemoveTypeHandler(CriticalEventType.type1, CreateIncidentOne);
            


        }

        public void CreateIncidentThree(CriticalEvent criticalEvent)
        {

            var sourceEventDate = DateTime.Now;

            int secondsToWait = 30;

            async void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("30 секунд еще не прошло, создаем!!");

                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type3, criticalEvent, ce);

                    await _processedEvents.Writer.WriteAsync(accident);

                    _logger.LogInformation("Создан инцидент типа 3 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }

                RemoveTypeHandler(CriticalEventType.type2, LocalHandler);
                AddTypeHandler(CriticalEventType.type2, CreateIncidentTwo);
            }

            AddTypeHandler(CriticalEventType.type2, LocalHandler);
            RemoveTypeHandler(CriticalEventType.type2, CreateIncidentTwo);
        }


        private void AddTypeHandler(CriticalEventType type, Action<CriticalEvent> handler)
        {
            lock (_lockObject)
            {
                typesHandlers[type] += handler; // += с делегатами непотокобезопасен?
            }

        }
        private void RemoveTypeHandler(CriticalEventType type, Action<CriticalEvent> handler)
        {
            
                if (handler is not null &&
                    typesHandlers.TryGetValue(type, out var existingHandlers) &&
                    existingHandlers is not null &&
                    existingHandlers.GetInvocationList().Any(d => d.Equals(handler)))
                {
                    var updatedHandlers = existingHandlers - handler;
                    typesHandlers[type] = updatedHandlers!;
                }
            

        }
       

    }
}