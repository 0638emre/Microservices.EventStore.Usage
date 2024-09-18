using System.Collections;
using System.Text.Json;
using ConsoleApp.BakiyeUygulamasi.Events;
using EventStore.Client;

namespace ConsoleApp.BakiyeUygulamasi.Services;

public class EventStoreService
{
    private EventStoreClientSettings GetEventStoreClientSettings(
        string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false") =>
        EventStoreClientSettings.Create(connectionString);
    
    private EventStoreClient Client { get => new EventStoreClient(GetEventStoreClientSettings());  }

    public async Task AppendToStreamAsync(string streamName,
        IEnumerable <EventData> eventData) => await Client.AppendToStreamAsync(
        streamName : streamName,
        eventData : eventData,
        expectedState: StreamState.Any
    );

    public EventData GenerateEventData(object @event)
        => new(
            eventId: Uuid.NewUuid(),
            type: @event.GetType().Name,
            data: JsonSerializer.SerializeToUtf8Bytes(@event)
            );

    public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared) => 
        await Client.SubscribeToStreamAsync(
        streamName: streamName,
        start: FromStream.Start, 
        eventAppeared: eventAppeared,
        subscriptionDropped: (x,y,z) => Console.WriteLine("Bağlantı Koptu !")
    );

    
    public Type GetEventType(string eventType)
    {
        return eventType switch
        {
            nameof(AccountCreatedEvent) => typeof(AccountCreatedEvent),
            nameof(MoneyDepositedEvent) => typeof(MoneyDepositedEvent),
            nameof(MoneyWithdrawnEvent) => typeof(MoneyWithdrawnEvent),
            nameof(MoneyTransferedEvent) => typeof(MoneyTransferedEvent),
            _ => null
        };
    }
}