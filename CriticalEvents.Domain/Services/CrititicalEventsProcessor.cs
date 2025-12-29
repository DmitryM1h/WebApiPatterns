using CriticalEvents.Domain.Entities;
using CriticalEvents.Domain.Enums;
using CriticalEvents.Domain.Interfaces;
using System.Collections.Concurrent;


namespace CriticalEvents.Domain.Services;

public class CrititicalEventsProcessor(IAccidentStorageFactory storageFactory)
{

    private readonly ConcurrentDictionary<CriticalEventType, Action<CriticalEvent>> typesHandlers = new();


    public async Task ReceiveCriticalEvent(CriticalEvent criticalEvent)
    {
        //_logger.LogInformation("В обработку получено критическое событие: {event}", new {id = criticalEvent.id.ToString(), type = criticalEvent.Type});

        typesHandlers.TryGetValue(criticalEvent.Type, out var handler);

        IAccidentStorage storage = await storageFactory.CreateAccidentStorage();

        _ = Task.Run(() =>
        {
            switch (criticalEvent.Type)
            {
                case CriticalEventType.type1:
                    if (handler is not null) handler(criticalEvent);
                    else CreateIncidentOne(criticalEvent, storage);
                    break;

                case CriticalEventType.type2:
                    if (handler is not null) handler(criticalEvent);
                    else CreateIncidentTwo(criticalEvent, storage);
                    break;

                case CriticalEventType.type3:
                    CreateIncidentThree(criticalEvent, storage);
                    break;
            }
        });
    }

    private async void CreateIncidentOne(CriticalEvent criticalEvent, IAccidentStorage storage)
    {
        var accident = new Accident(Guid.NewGuid(), AccidentType.Type1, criticalEvent);
        await storage.StoreEvent(accident);
        //_logger.LogInformation("Создан инцидент типа 1 на основе события: {event} ", new {id = criticalEvent.id.ToString()});

    }

    // async Task?
    private async void CreateIncidentTwo(CriticalEvent criticalEvent, IAccidentStorage storage)
    {
        var sourceEventDate = DateTime.Now;
        int secondsToWait = 20;

        using CancellationTokenSource src = new();
        var token = src.Token;

        AddTypeHandler(CriticalEventType.type1, LocalHandler);

        await WaitToCreateDefaultIncidentAsync(criticalEvent, storage, secondsToWait, token);

        RemoveTypeHandler(CriticalEventType.type1, LocalHandler);

        async void LocalHandler(CriticalEvent ce)
        {
            if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
            {
                //_logger.LogInformation("20 секунд еще не прошло, создаем!!");

                src.Cancel();

                var accident = new Accident(Guid.NewGuid(), AccidentType.Type2, criticalEvent, ce);

                await storage.StoreEvent(accident);

                //_logger.LogInformation("Создан инцидент типа 2 на основе событий {events}" , new { firstEvent = accident.CriticalEventFirst.id.ToString(), secondEvent = accident.CriticalEventSecond!.id.ToString()});
            }
        }
    }

    private async void CreateIncidentThree(CriticalEvent criticalEvent, IAccidentStorage storage)
    {
        var sourceEventDate = DateTime.Now;

        int secondsToWait = 30;

        using CancellationTokenSource src = new();
        var token = src.Token;

        AddTypeHandler(CriticalEventType.type2, LocalHandler);

        await WaitToCreateDefaultIncidentAsync(criticalEvent, storage, secondsToWait, token);

        RemoveTypeHandler(CriticalEventType.type2, LocalHandler);

        async void LocalHandler(CriticalEvent ce)
        {
            if (DateTime.Now - sourceEventDate < TimeSpan.FromSeconds(secondsToWait))
            {
                //_logger.LogInformation("30 секунд еще не прошло, создаем!!");

                src.Cancel();

                var accident = new Accident(Guid.NewGuid(), AccidentType.Type3, criticalEvent, ce);

                await storage.StoreEvent(accident);

                //_logger.LogInformation("Создан инцидент типа 3 на основе событий {events}", new { firstEvent = accident.CriticalEventFirst.id.ToString(), secondEvent = accident.CriticalEventSecond!.id.ToString() });
            }

        }
    }


    private async Task WaitToCreateDefaultIncidentAsync(CriticalEvent criticalEvent, IAccidentStorage storage, int secondsToWait, CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(secondsToWait), token);

            //_logger.LogInformation("Не дождались следующего события, создаем дефолтный инцидент");

            var accidentType = CriticalEventType.type3 == criticalEvent.Type ? AccidentType.Type2 : AccidentType.Type1;

            var accident = new Accident(Guid.NewGuid(), accidentType, criticalEvent);

            await storage.StoreEvent(accident);

            //_logger.LogInformation("Создан инцидент на основе событий {accident}", new {accidentType, firstEvent = accident.CriticalEventFirst.id.ToString()});
        }
        catch (OperationCanceledException)
        {
            //_logger.LogInformation("Событие пришло, дефолтный инцидент не создаётся");
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

