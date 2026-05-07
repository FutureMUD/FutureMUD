namespace MudSharp.Models;

public partial class ClanBudgetTransaction
{
	public long Id { get; set; }
	public long ClanBudgetId { get; set; }
	public long ActorId { get; set; }
	public long? BankAccountId { get; set; }
	public long CurrencyId { get; set; }
	public decimal Amount { get; set; }
	public string TransactionTime { get; set; }
	public string PeriodStart { get; set; }
	public string PeriodEnd { get; set; }
	public decimal BankBalanceAfter { get; set; }
	public string Reason { get; set; }

	public virtual ClanBudget ClanBudget { get; set; }
	public virtual Character Actor { get; set; }
	public virtual BankAccount BankAccount { get; set; }
	public virtual Currency Currency { get; set; }
}
