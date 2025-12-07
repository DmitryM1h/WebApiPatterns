using System.Collections.Concurrent;
using System.Threading.Channels;
using WebApiPatterns.Application.Dtos;

namespace WebApiPatterns.Application
{
    public class CriticalEventHandler
    {
        private readonly ILogger<CriticalEventHandler> _logger;
        public CriticalEventHandler(ILogger<CriticalEventHandler> logger, Channel<Accident> processedVents)
        {
            _logger = logger;
            _processedEvents = processedVents;

        }

        private readonly Channel<Accident> _processedEvents;

        private readonly ConcurrentDictionary<CriticalEventType, Action<CriticalEvent>> typesHandlers = new();

        public void ProcessCriticalEvent(CriticalEventRequest request)
        {
            CriticalEvent criticalEvent = new(request.id, request.Description, request.Type, DateTime.Now);
            _logger.LogInformation("В обработку получено критическое событие, id = " + criticalEvent.id.ToString() + " типа " + criticalEvent.Type);

            typesHandlers.TryGetValue(criticalEvent.Type, out var handler);

            switch(criticalEvent.Type)
            {
                case CriticalEventType.type1:
                    if (handler is not null) handler(criticalEvent);
                    else CreateIncidentOne(criticalEvent);
                    break;

                case CriticalEventType.type2:
                    if (handler is not null) handler(criticalEvent);
                    else CreateIncidentTwo(criticalEvent);
                    break;

                case CriticalEventType.type3:
                    CreateIncidentThree(criticalEvent);
                    break;
            }

        }

        private void CreateIncidentOne(CriticalEvent criticalEvent)
        {
            var accident = new Accident(Guid.NewGuid(), AccidentType.Type1, criticalEvent);
            _logger.LogInformation("Создан инцидент типа 1 на основе события с id " + criticalEvent.id.ToString());

        }

        // async Task?
        private async void CreateIncidentTwo(CriticalEvent criticalEvent)
        {
            var sourceEventDate = DateTime.Now;
            int secondsToWait = 20;

            CancellationTokenSource src = new CancellationTokenSource();
            var token = src.Token;
         
            AddTypeHandler(CriticalEventType.type1, LocalHandler);

            await WaitToCreateDefaultIncidentAsync(criticalEvent, secondsToWait, token);

            RemoveTypeHandler(CriticalEventType.type1, LocalHandler);


            async void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("20 секунд еще не прошло, создаем!!");

                    src.Cancel();

                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type2, criticalEvent, ce);

                    await _processedEvents.Writer.WriteAsync(accident);

                    _logger.LogInformation("Создан инцидент типа 2 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }
            }
        }

        private async void CreateIncidentThree(CriticalEvent criticalEvent)
        {
            var sourceEventDate = DateTime.Now;

            int secondsToWait = 30;

            CancellationTokenSource src = new CancellationTokenSource();
            var token = src.Token;

            AddTypeHandler(CriticalEventType.type2, LocalHandler);

            await WaitToCreateDefaultIncidentAsync(criticalEvent, secondsToWait, token);

            RemoveTypeHandler(CriticalEventType.type2, LocalHandler);


            async void LocalHandler(CriticalEvent ce)
            {
                if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
                {
                    _logger.LogInformation("30 секунд еще не прошло, создаем!!");

                    src.Cancel();

                    var accident = new Accident(Guid.NewGuid(), AccidentType.Type3, criticalEvent, ce);

                    await _processedEvents.Writer.WriteAsync(accident);

                    _logger.LogInformation("Создан инцидент типа 3 на основе событий с id = " + accident.CriticalEventFirst.id.ToString() + " " + accident.CriticalEventSecond!.id.ToString());
                }

            }
        }


        private async Task WaitToCreateDefaultIncidentAsync(CriticalEvent criticalEvent, int secondsToWait, CancellationToken token)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(secondsToWait), token);

                _logger.LogInformation("Не дождались следующего события, создаем дефолтный инцидент");

                var accidentType = CriticalEventType.type3 == criticalEvent.Type ? AccidentType.Type2 : AccidentType.Type1;

                var accident = new Accident(Guid.NewGuid(), accidentType, criticalEvent);

                await _processedEvents.Writer.WriteAsync(accident);

                _logger.LogInformation($"Создан инцидент типа {(int)criticalEvent.Type + 1}  на основе события с id = " + criticalEvent.id.ToString());
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Событие пришло, дефолтный инцидент не создаётся");
            }

        }


        private void AddTypeHandler(CriticalEventType type, Action<CriticalEvent> handler)
        {
            typesHandlers.TryGetValue(type, out var existingHandlers);

            if (existingHandlers is not null)
            {
                existingHandlers += handler;
                typesHandlers[type] = existingHandlers;
            }
            else
            {
                typesHandlers[type] = handler;
            }
        }
        private void RemoveTypeHandler(CriticalEventType type, Action<CriticalEvent> handler)
        {
            typesHandlers.TryGetValue(type, out var existingHandlers);

            var updatedHandlers = existingHandlers - handler;

            typesHandlers[type] = updatedHandlers!;
        }
       
    }
}