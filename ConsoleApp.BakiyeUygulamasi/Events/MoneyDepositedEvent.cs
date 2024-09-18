namespace ConsoleApp.BakiyeUygulamasi.Events;

public class MoneyDepositedEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}