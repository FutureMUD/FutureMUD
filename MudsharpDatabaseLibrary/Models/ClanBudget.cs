using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ClanBudget
{
	public ClanBudget()
	{
		ClanBudgetTransactions = new HashSet<ClanBudgetTransaction>();
	}

	public long Id { get; set; }
	public long ClanId { get; set; }
	public long AppointmentId { get; set; }
	public long? BankAccountId { get; set; }
	public long CurrencyId { get; set; }
	public string Name { get; set; }
	public decimal AmountPerPeriod { get; set; }
	public int PeriodIntervalType { get; set; }
	public int PeriodIntervalModifier { get; set; }
	public int PeriodIntervalOther { get; set; }
	public int PeriodIntervalOtherSecondary { get; set; }
	public int PeriodIntervalFallback { get; set; }
	public string CurrentPeriodStart { get; set; }
	public string CurrentPeriodEnd { get; set; }
	public decimal CurrentPeriodDrawdown { get; set; }
	public bool IsActive { get; set; }

	public virtual Clan Clan { get; set; }
	public virtual Appointment Appointment { get; set; }
	public virtual BankAccount BankAccount { get; set; }
	public virtual Currency Currency { get; set; }
	public virtual ICollection<ClanBudgetTransaction> ClanBudgetTransactions { get; set; }
}
