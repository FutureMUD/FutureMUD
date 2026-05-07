using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Community;

public interface IClanBudget : IFrameworkItem, ISaveable
{
	IClan Clan { get; }
	IAppointment Appointment { get; set; }
	IBankAccount? BankAccount { get; set; }
	ICurrency Currency { get; }
	decimal AmountPerPeriod { get; set; }
	decimal CurrentPeriodDrawdown { get; }
	decimal RemainingBudget { get; }
	RecurringInterval PeriodInterval { get; set; }
	MudDateTime CurrentPeriodStart { get; }
	MudDateTime CurrentPeriodEnd { get; }
	bool IsActive { get; set; }
	IEnumerable<IClanBudgetTransaction> Transactions { get; }
	void RollToCurrentPeriod();
	void AddDrawdown(IClanBudgetTransaction transaction);
	void Delete();
}
