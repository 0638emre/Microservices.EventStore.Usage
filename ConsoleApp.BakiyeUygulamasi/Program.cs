

using System.Text.Json;
using ConsoleApp.BakiyeUygulamasi.Events;
using ConsoleApp.BakiyeUygulamasi.Models;
using ConsoleApp.BakiyeUygulamasi.Services;

EventStoreService eventStoreService = new EventStoreService();

#region Event oluşturma (sırasıyla)
AccountCreatedEvent accountCreatedEvent = new()
{
    AccountId = "12345",
    CustomerId = "98765",
    StartBalance = 0,
    Date = DateTime.UtcNow.Date,
};
MoneyDepositedEvent moneyDepositedEvent1 = new()
{
    AccountId = "12345",
    Amount = 1000,
    Date = DateTime.UtcNow.Date,
};
MoneyDepositedEvent moneyDepositedEvent2 = new()
{
    AccountId = "12345",
    Amount = 500,
    Date = DateTime.UtcNow.Date,
};
MoneyWithdrawnEvent moneyWithdrawnEvent = new()
{
    AccountId = "12345",
    Amount = 500,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent3 = new()
{
    AccountId = "12345",
    Amount = 50,
    Date = DateTime.UtcNow.Date,
};
MoneyTransferedEvent moneyTransferedEvent1 = new()
{
    AccountId = "12345",
    Amount = 250,
    Date = DateTime.UtcNow.Date,
};
MoneyTransferedEvent moneyTransferedEvent2 = new()
{
    AccountId = "12345",
    Amount = 150,
    Date = DateTime.UtcNow.Date,
};
MoneyDepositedEvent moneyDepositedEvent4 = new()
{
    AccountId = "12345",
    Amount = 2000,
    Date = DateTime.UtcNow.Date,
};

// await eventStoreService.AppendToStreamAsync(
//     streamName: $"customer-{accountCreatedEvent.CustomerId}-stream",
//     eventData: new[]
//     {
//         eventStoreService.GenerateEventData(accountCreatedEvent),
//         eventStoreService.GenerateEventData(moneyDepositedEvent1),
//         eventStoreService.GenerateEventData(moneyDepositedEvent2),
//         eventStoreService.GenerateEventData(moneyWithdrawnEvent),
//         eventStoreService.GenerateEventData(moneyDepositedEvent3),
//         eventStoreService.GenerateEventData(moneyTransferedEvent1),
//         eventStoreService.GenerateEventData(moneyTransferedEvent2),
//         eventStoreService.GenerateEventData(moneyDepositedEvent4)
//     });


#endregion

BalanceInfo balanceInfo = new();
await eventStoreService.SubscribeToStreamAsync(
    streamName: $"customer-{accountCreatedEvent.CustomerId}-stream",
    async (ss, re, ct) =>
    {
        string eventType = re.Event.EventType;
        // object @event =  JsonSerializer.Deserialize(re.Event.Data.ToArray(), Type.GetType(eventType));
        object @event = JsonSerializer.Deserialize(re.Event.Data.ToArray(), eventStoreService.GetEventType(eventType));
        switch (@event)
        {
            case AccountCreatedEvent e:
                balanceInfo.AccountId = e.AccountId;
                balanceInfo.Balance = e.StartBalance;
                break;
            case MoneyDepositedEvent e:
                balanceInfo.Balance += e.Amount;
                break;
            case MoneyWithdrawnEvent e:
                balanceInfo.Balance -= e.Amount;
                break;
            case MoneyTransferedEvent e:
                balanceInfo.Balance -= e.Amount;
                break;
        }

        await Console.Out.WriteLineAsync("****Balance****");
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(balanceInfo));
        await Console.Out.WriteLineAsync("****Balance****");
        await Console.Out.WriteLineAsync("****Balance****");
        await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync();
    }
);

Console.ReadLine();

Console.WriteLine("Hello, World!");