#nullable enable
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	private sealed class AutopayPortfolio
	{
		internal HashSet<long> Loans { get; } = new();
		internal bool Listening { get; set; }
	}

	private static readonly ConditionalWeakTable<IFuturemud, AutopayPortfolio> AutopayPortfolios = new();

	private static void TrackAutopay(IFuturemud world, LendingContract loan)
	{
		var portfolio = AutopayPortfolios.GetValue(world, _ => new AutopayPortfolio());
		if (Terms(loan).AutopayAccountId.HasValue && !Terms(loan).BorrowerDiedOutsideProbate && loan.Status == "Active" && !loan.EstateId.HasValue)
			portfolio.Loans.Add(loan.Id);
		else portfolio.Loans.Remove(loan.Id);
		if (portfolio.Listening || portfolio.Loans.Count == 0 || world.HeartbeatManager == null) return;
		portfolio.Listening = true;
		world.HeartbeatManager.FuzzyMinuteHeartbeat += () => ProcessAutopay(world);
	}

	internal static string ConfigureAutopay(ICharacter actor, long id, string input) => Settle(actor, () =>
	{
		var loan = FMDB.Context.LendingContracts.Find(id);
		if (loan?.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(actor) || loan.Status != "Active" || loan.EstateId.HasValue || Terms(loan).BorrowerDiedOutsideProbate)
			return "You have no such active personal loan.";
		var terms = Terms(loan);
		if (string.IsNullOrWhiteSpace(input)) return AutopayDescription(actor.Gameworld, loan, terms);
		if (input.EqualTo("off"))
		{
			terms.AutopayAccountId = null;
			SaveAutopayConfiguration(actor.Gameworld, loan, terms);
			AfterCommit(() => TrackAutopay(actor.Gameworld, loan));
			return "Automatic payments disabled. Any outstanding instalments still need payment.";
		}
		var found = Banking.Bank.FindBankAccount(input, Lender(actor.Gameworld, loan) as IBank, actor);
		if (found.Account is not { } account) return found.Error;
		if (account.AccountStatus != BankAccountStatus.Active || account.Currency.Id != loan.CurrencyId || !account.IsAuthorisedAccountUser(actor))
			return "Choose an active account in the loan currency that you can withdraw from.";
		terms.AutopayAccountId = account.Id;
		terms.AutopayLastAttempt = null;
		SaveAutopayConfiguration(actor.Gameworld, loan, terms);
		AfterCommit(() => TrackAutopay(actor.Gameworld, loan));
		return $"Automatic payments enabled from {account.AccountReference}. Amounts due and bank charges will be withdrawn, including while you are offline. Failed payments retry each game day. Use LOAN AUTOPAY {id} RETRY to try sooner.";
	}, guard: true);

	private static void SaveAutopayConfiguration(IFuturemud world, LendingContract loan, LoanTerms terms)
	{
		Store(loan, terms);
		Post(loan, "Autopay", 0, 0, 0, terms.AutopayAccountId is { } id ? $"Payment account #{id}" : "Disabled",
			world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime, loan.BorrowerId);
	}

	internal static string RetryAutopay(ICharacter actor, long id)
	{
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.AsNoTracking().FirstOrDefault(x => x.Id == id);
			if (loan?.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(actor)) return "You have no such loan.";
		}
		return AttemptAutopay(actor.Gameworld, id, true);
	}

	private static string AutopayDescription(IFuturemud world, LendingContract loan, LoanTerms terms)
	{
		var source = terms.AutopayAccountId is { } id ? world.BankAccounts.Get(id)?.AccountReference ?? $"missing account #{id}" : "off";
		return $"Autopay: {source}." + (terms.AutopayFailure == null ? "" : $" Failed automatic payment: {terms.AutopayFailure} Use LOAN AUTOPAY {loan.Id} RETRY or LOAN PAY {loan.Id}.");
	}

	internal static void ProcessAutopay(IFuturemud world)
	{
		if (!AutopayPortfolios.TryGetValue(world, out var portfolio) || portfolio.Loans.Count == 0) return;
		using var handle = new FMDB();
		var ids = portfolio.Loans.ToList();
		var loans = FMDB.Context.LendingContracts.AsNoTracking().Where(x => ids.Contains(x.Id)).ToList();
		foreach (var loan in loans)
		{
			if (loan.Status != "Active" || loan.EstateId.HasValue) { portfolio.Loans.Remove(loan.Id); continue; }
			var terms = Terms(loan);
			var now = world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime;
			Accrue(loan, terms, now);
			var due = DueNow(terms, now);
			if (due > 0 && terms.AutopayLastAttempt != AutopayAttemptKey(terms, now)) AttemptAutopay(world, loan.Id);
		}
	}

	internal static string AttemptAutopay(IFuturemud world, long id, bool retry = false)
	{
		if (_settling) return "A lending transaction is already in progress.";
		_settling = true;
		try
		{
			var result = GuardedSettlement(world, () =>
			{
				var loan = FMDB.Context.LendingContracts.Find(id);
				if (loan == null) return "No such loan.";
				var terms = Terms(loan);
				if (loan.Status != "Active" || loan.EstateId.HasValue || terms.BorrowerDiedOutsideProbate || !terms.AutopayAccountId.HasValue)
				{
					TrackAutopay(world, loan);
					return "Automatic payments are unavailable for this loan.";
				}
				var now = world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime;
				Accrue(loan, terms, now);
				var due = DueNow(terms, now);
				if (due <= 0)
				{
					terms.AutopayFailure = null;
					Store(loan, terms);
					return "No loan payment is due.";
				}
				var attempt = AutopayAttemptKey(terms, now);
				if (!retry && terms.AutopayLastAttempt == attempt) return "Automatic payment already attempted today.";
				terms.AutopayLastAttempt = attempt;
				var actor = world.TryGetCharacter(loan.BorrowerId, true);
				var account = world.BankAccounts.Get(terms.AutopayAccountId.Value);
				var lender = Lender(world, loan);
				var failure = AutopayProblem(actor, loan, lender, account, due);
				if (failure == null && terms.Kind == "Pawn" && terms.CustodyStatus == "Held" && world.TryGetItem(terms.AssetId, true)?.Owner != lender)
					failure = "The lender cannot locate the pledged item. Contact a manager.";
				if (failure != null)
				{
					var notify = terms.AutopayFailure != failure;
					terms.AutopayFailure = failure;
					Store(loan, terms);
					if (notify) AfterCommit(() => NotifyAutopay(world, loan, terms));
					return failure;
				}
				CompletePayment(world, loan, terms, lender!, account, due, now, loan.BorrowerId, Guid.NewGuid().ToString("N"));
				AfterCommit(() => TrackAutopay(world, loan));
				AfterCommit(() => world.Actors.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == loan.BorrowerId)?.OutputHandler.Send($"Automatic payment of {Money(account!.Currency, due)} received for loan #{id}."));
				return "Automatic payment received.";
			});
			if (result.StartsWith("The settlement could not be completed.", StringComparison.Ordinal))
			{
				// The failed financial transaction was rolled back. Record its notice separately.
				GuardedSettlement(world, () =>
				{
					var loan = FMDB.Context.LendingContracts.Find(id)!;
					var terms = Terms(loan);
					var now = world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime;
					Accrue(loan, terms, now);
					var due = DueNow(terms, now);
					if (due <= 0) return "No loan payment is due.";
					terms.AutopayLastAttempt = AutopayAttemptKey(terms, now);
					terms.AutopayFailure = "The payment could not be saved. Retry the automatic payment.";
					Store(loan, terms);
					AfterCommit(() => NotifyAutopay(world, loan, terms));
					return "Failure recorded.";
				});
			}
			return result;
		}
		finally { _settling = false; }
	}

	private static string AutopayAttemptKey(LoanTerms terms, MudDateTime now) =>
		$"{now.Date.GetDateString()}|{terms.Bills[^1].Due}";

	private static string? AutopayProblem(ICharacter? actor, LendingContract loan, IFrameworkItem? lender, IBankAccount? account, decimal due)
	{
		if (actor == null || lender == null) return "The borrower or lender is unavailable.";
		if (account == null || account.AccountStatus != BankAccountStatus.Active) return "The payment account is unavailable or inactive.";
		if (account.Currency.Id != loan.CurrencyId || account.Currency != ZoneOf(lender).Currency) return "The payment currency has changed.";
		if (!account.IsAuthorisedAccountUser(actor)) return "You no longer have withdrawal authority for the account.";
		var check = account.CanWithdraw(due, account.Bank == lender);
		if (!check.Truth) return check.Error;
		var fee = account.BankAccountType.QuoteTransactionWithdrawalFees(account, due);
		if (due + fee > account.MaximumWithdrawal()) return "Insufficient funds for the amount due and bank charges.";
		if (account.Bank != lender && account.Bank.CurrencyReserves[account.Currency] < due) return "The paying bank has insufficient reserves.";
		return null;
	}

	private static void NotifyAutopay(IFuturemud world, LendingContract loan, LoanTerms terms) =>
		world.Actors.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == loan.BorrowerId)?.OutputHandler.Send(AutopayDescription(world, loan, terms));
}
