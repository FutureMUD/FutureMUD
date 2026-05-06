using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp.Community;

public interface IClanBudgetTransaction : IFrameworkItem
{
	IClanBudget Budget { get; }
	ICharacter Actor { get; }
	IBankAccount BankAccount { get; }
	ICurrency Currency { get; }
	decimal Amount { get; }
	MudDateTime TransactionTime { get; }
	MudDateTime PeriodStart { get; }
	MudDateTime PeriodEnd { get; }
	decimal BankBalanceAfter { get; }
	string Reason { get; }
}
