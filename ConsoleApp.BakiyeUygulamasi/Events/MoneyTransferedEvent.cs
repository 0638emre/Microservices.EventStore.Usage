namespace ConsoleApp.BakiyeUygulamasi.Events;

public class MoneyTransferedEvent
{
    public string AccountId { get; set; }
    public string TargetAccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}