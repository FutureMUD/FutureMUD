#nullable enable
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy;

public interface IBankAccountTransaction : ILateInitialisingItem
{
	IBankAccount Account { get; }
	BankTransactionType TransactionType { get; }
	decimal Amount { get; }
	decimal AccountBalanceAfter { get; }
	MudDateTime TransactionTime { get; }
	string TransactionDescription { get; }
}