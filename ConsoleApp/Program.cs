using System.Text.Json;
using EventStore.Client;

//Bu console uygulaması event store üzerindeki işlemleri dotnet tarafında nasıl yürütüldüğünü öğrenmek amacıyla hedef edilmiştir.
//docker komutu (event store db) :  docker run -d --name esdb-node -it -p 2113:2113 eventstore/eventstore:24.6.0-alpha-arm64v8 --insecure --run-projections=All --enable-atom-pub-over-http


var connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";

var setting = EventStoreClientSettings.Create(connectionString);
var client = new EventStoreClient(setting);


OrderPlacedEvent orderPlacedEvent = new()
{
    OrderId = 1,
    TotalAmount = 100
};

#region EventStore a event gönderimi
//burada event store db ye bir eventi atmayı örneklendiriyoruz.
// var count = 0;
// while (count < 101)
// {
//     EventData eventData = new(
//         eventId: Uuid.NewUuid(),
//         type: orderPlacedEvent.GetType().Name,
//         data: JsonSerializer.SerializeToUtf8Bytes(orderPlacedEvent)
//     );
//
//     await client.AppendToStreamAsync(
//         streamName: "order-stream",
//         expectedState: StreamState.Any,
//         eventData: new[] { eventData }
//     );
//     
//     count++; 
// }


#endregion

#region EventStora dan event okuma

//burada event store db de oluşmuş eventleri okumayı örneklendiriyoruz.
var events = client.ReadStreamAsync(
    streamName: "order-stream",
    direction: Direction.Forwards, //baştan sona mı sonadn başam
    revision: StreamPosition.Start //kaçıncı eventten başlayacağını belirtir
);

var datas = await events.ToListAsync();


#endregion

#region EventStore a subscribe olmak

await client.SubscribeToStreamAsync(
    streamName: "order-stream",
    start: FromStream.Start,
    eventAppeared: async (streamSubscription, resolvedEvent, cancellationToken) =>
    {
        OrderPlacedEvent @event =  JsonSerializer.Deserialize<OrderPlacedEvent>(resolvedEvent.Event.Data.ToArray());
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(@event));
    },
    subscriptionDropped: (streamSubscription, subscriptionDroppedReason, exception) =>
        Console.WriteLine("Bağlantı koptu.")
);

Console.ReadLine();

#endregion



Console.WriteLine();

class OrderPlacedEvent
{
    public int OrderId { get; set; }
    public int TotalAmount { get; set; }
}

// Console.WriteLine("emre");