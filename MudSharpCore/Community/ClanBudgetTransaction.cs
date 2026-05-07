using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

#nullable enable

namespace MudSharp.Community;

public class ClanBudgetTransaction : IClanBudgetTransaction
{
	private readonly long _actorId;
	private readonly long? _bankAccountId;
	private ICharacter? _actor;
	private IBankAccount? _bankAccount;
	private IFuturemud Gameworld => ((ClanBudget)Budget).Gameworld;

	public ClanBudgetTransaction(MudSharp.Models.ClanBudgetTransaction transaction, IClanBudget budget)
	{
		Budget = budget;
		_id = transaction.Id;
		_actorId = transaction.ActorId;
		_bankAccountId = transaction.BankAccountId;
		Currency = Gameworld.Currencies.Get(transaction.CurrencyId)!;
		Amount = transaction.Amount;
		TransactionTime = MudDateTime.FromStoredStringOrFallback(transaction.TransactionTime, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanBudgetTransaction", transaction.Id, budget.Name,
			"TransactionTime");
		PeriodStart = MudDateTime.FromStoredStringOrFallback(transaction.PeriodStart, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanBudgetTransaction", transaction.Id, budget.Name,
			"PeriodStart");
		PeriodEnd = MudDateTime.FromStoredStringOrFallback(transaction.PeriodEnd, Gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanBudgetTransaction", transaction.Id, budget.Name,
			"PeriodEnd");
		BankBalanceAfter = transaction.BankBalanceAfter;
		Reason = transaction.Reason;
	}

	public ClanBudgetTransaction(IClanBudget budget, ICharacter actor, decimal amount, string reason)
	{
		Budget = budget;
		_actor = actor;
		_actorId = actor.Id;
		_bankAccount = budget.BankAccount;
		_bankAccountId = budget.BankAccount?.Id;
		Currency = budget.Currency;
		Amount = amount;
		TransactionTime = budget.Clan.Calendar.CurrentDateTime;
		PeriodStart = budget.CurrentPeriodStart;
		PeriodEnd = budget.CurrentPeriodEnd;
		BankBalanceAfter = budget.BankAccount?.CurrentBalance ?? VirtualCashLedger.Balance(budget.Clan, budget.Currency);
		Reason = reason;

		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.ClanBudgetTransaction
			{
				ClanBudgetId = budget.Id,
				ActorId = actor.Id,
				BankAccountId = budget.BankAccount?.Id,
				CurrencyId = budget.Currency.Id,
				Amount = amount,
				TransactionTime = TransactionTime.GetDateTimeString(),
				PeriodStart = PeriodStart.GetDateTimeString(),
				PeriodEnd = PeriodEnd.GetDateTimeString(),
				BankBalanceAfter = BankBalanceAfter,
				Reason = reason
			};
			FMDB.Context.ClanBudgetTransactions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private readonly long _id;
	public long Id => _id;
	public string Name => $"Budget Drawdown #{Id:N0}";
	public string FrameworkItemType => "ClanBudgetTransaction";
	public IClanBudget Budget { get; }
	public ICharacter Actor => _actor ??= Gameworld.TryGetCharacter(_actorId, true)!;
	public IBankAccount? BankAccount => _bankAccount ??= _bankAccountId.HasValue ? Gameworld.BankAccounts.Get(_bankAccountId.Value) : null;
	public ICurrency Currency { get; }
	public decimal Amount { get; }
	public MudDateTime TransactionTime { get; }
	public MudDateTime PeriodStart { get; }
	public MudDateTime PeriodEnd { get; }
	public decimal BankBalanceAfter { get; }
	public string Reason { get; }
}
