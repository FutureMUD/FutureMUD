#nullable enable
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using AcceptEffect = MudSharp.Effects.Concrete.Accept;
using BankRuntime = MudSharp.Economy.Banking.Bank;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	private static decimal DueNow(LoanTerms terms, MudDateTime now) => terms.Bills
		.Where(x => new MudDateTime(x.Due, now.Gameworld) <= now)
		.Sum(x => x.Principal + x.Interest);

	internal static void NotifyBorrower(ICharacter actor)
	{
		if (!HasPortfolio(actor.Gameworld)) return;
		var borrower = CharacterInstanceIdentityComparer.IdentityId(actor);
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts.AsNoTracking().Where(x => x.BorrowerId == borrower &&
				(x.Status == "Offered" || x.Status == "Active" || x.Status == "Recovery" ||
				 x.Status == "Paid" || x.Surplus > 0)).ToList();
			foreach (var loan in loans)
			{
				var terms = Terms(loan);
				var zone = actor.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
				var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
				Accrue(loan, terms, now);
				var due = DueNow(terms, now);
				if (due > 0 && terms.AutopayFailure != null) actor.OutputHandler.Send(AutopayDescription(world: actor.Gameworld, loan, terms));
				var notice = loan.Status switch
				{
					_ when loan.Surplus > 0 || loan.Status == "Paid" => $"Collection available. Use LOAN COLLECT {loan.Id}.",
					"Offered" when terms.OfferExpiryUtc > DateTime.UtcNow => $"Offer ready. Use LOAN {loan.Id} to review.",
					"Active" when terms.Amendment?.ExpiresUtc > DateTime.UtcNow => $"Revised terms await your decision. Use LOAN {loan.Id}.",
					"Active" when due > 0 => $"{Money(zone.Currency, due)} due. Cure by {new MudDateTime(terms.Bills[0].Due, actor.Gameworld) + MudTimeSpan.FromDays(terms.GraceDays)}.",
					"Recovery" => $"Collateral in recovery. Use LOAN {loan.Id} for details.",
					_ => null
				};
				if (notice != null) actor.OutputHandler.Send($"Loan #{loan.Id} at {Lender(actor.Gameworld, loan)?.Name}: {notice}");
			}
		}
	}
	private static string Summary(ICharacter actor, LendingContract loan, LoanTerms terms)
	{
		var world = actor.Gameworld;
		var currency = world.Currencies.Get(loan.CurrencyId);
		string Amount(decimal d) => Money(currency, d);
		string Date(string s) => new MudDateTime(s, world).ToString();
		var sb = new StringBuilder();
		sb.AppendLine($"Loan #{loan.Id.ToString("N0", actor)} at {Lender(world, loan)?.Name.ColourName()} — {loan.Status.ColourName()}");
		var asset = loan.Status == "Active" && loan.SecurityKey is null && terms.CustodyStatus is "Auctioned" or "Retained"
			? "unsecured balance after collateral recovery" : terms.Kind switch
		{
			"Pawn" => $"pledged item #{terms.AssetId}",
			"Property" or "Mortgage" => world.Properties.Get(terms.AssetId)?.Name ?? $"property #{terms.AssetId}",
			_ => "unsecured"
		};
		sb.AppendLine(AutopayDescription(world, loan, terms));
		sb.AppendLine($"{asset}. Advance {Amount(terms.Advance)}. Simple interest {terms.Rate.ToString("P2", actor)} {terms.Interval}.");
		var minimum = terms.Minimum switch
		{
			"Principal" => $"interest plus {Amount(terms.Instalment)} principal",
			"Interest" => "interest only",
			_ => "no interim payment"
		};
		sb.AppendLine($"Minimum: {minimum}. Final balance due after {terms.Periods.ToString("N0", actor)} periods. Grace: {terms.GraceDays.ToString("N0", actor)} days.");
		if (terms.Kind == "Mortgage") sb.AppendLine($"Purchase price {Amount(terms.Price)}, your contribution {Amount(terms.DownPayment)}.");
		if (terms.Kind != "Unsecured")
			sb.AppendLine($"Collateral appraisal {Amount(terms.Appraisal)}. On default: {(terms.AuctionHouseId > 0 ? "auction" : "retention in settlement")}. " +
			              (terms.Recourse ? "You owe any shortfall." : "A shortfall is forgiven.") + " Any surplus is yours.");
		if (loan.Status is "Offered" or "Requested")
		{
			sb.AppendLine($"Offer expires {terms.OfferExpiryUtc:u}. " + (loan.Status == "Offered" ? $"Use LOAN ACCEPT {loan.Id}." : "Awaiting staff approval."));
		}
		else
		{
			sb.AppendLine($"Principal {Amount(loan.Principal)}, interest {Amount(loan.Interest)}, payoff {Amount(loan.Principal + loan.Interest)}. Surplus {Amount(loan.Surplus)}.");
			if (loan.Status == "Active")
			{
				var now = world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime;
				sb.AppendLine($"Due now {Amount(DueNow(terms, now))}. {(terms.PeriodsBilled >= terms.Periods ? "Maturity" : "Next payment date")} {Date(terms.NextDue)}.");
				if (terms.Bills.FirstOrDefault() is { } oldest)
					sb.AppendLine($"Oldest payment due {Date(oldest.Due)}. Cure by {new MudDateTime(oldest.Due, world) + MudTimeSpan.FromDays(terms.GraceDays)}.");
				if (terms.PauseUntil is not null) sb.AppendLine($"Paused until {Date(terms.PauseUntil)}.");
				if (terms.Amendment is { } a)
					sb.AppendLine($"Proposed {a.Kind}: {AmendmentDescription(a, currency)}. Reason: {a.Reason}. Use LOAN ACCEPT {loan.Id} or LOAN DECLINE {loan.Id}.");
			}
		}
		return sb.ToString().TrimEnd();
	}

	internal static string List(ICharacter actor)
	{
		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts.AsNoTracking().Where(x => x.BorrowerId == id)
				.OrderByDescending(x => x.Id).Take(100).ToList();
			if (loans.Count == 0) return "You have no loans. At a lender, use LOAN OFFERS.";
			return string.Join("\n", loans.Select(x => $"#{x.Id.ToString("N0", actor)} {Lender(actor.Gameworld, x)?.Name} — {x.Status}")) +
			       "\nUse LOAN <number> for the current balance, terms and next action.";
		}
	}

	internal static string Show(ICharacter actor, long id, bool history)
	{
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.AsNoTracking().FirstOrDefault(x => x.Id == id);
			if (loan is null) return "No such accessible loan.";
			var lender = Lender(actor.Gameworld, loan);
			if (loan.BorrowerId != CharacterInstanceIdentityComparer.IdentityId(actor) &&
			    (lender is null || !LocalLenders(actor).Contains(lender) || !IsManager(actor, lender))) return "No such accessible loan.";
			var terms = Terms(loan);
			Accrue(loan, terms, actor.Gameworld.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime);
			var summary = Summary(actor, loan, terms);
			if (!history) return summary;
			var rows = FMDB.Context.LendingPostings.AsNoTracking().Where(x => x.ContractId == id).OrderByDescending(x => x.Id).Take(100).ToList();
			var currency = actor.Gameworld.Currencies.Get(loan.CurrencyId);
			return summary + "\n" + string.Join("\n", rows.Select(x => $"{new MudDateTime(x.MudDate, actor.Gameworld)} — {x.Kind} {Money(currency, x.Amount)}. {x.Detail}"));
		}
	}

	internal static string Pay(ICharacter actor, long id, string amount, string? account) =>
		Payment(actor, id, amount, account, null, null, Guid.NewGuid().ToString("N"));

	private static string Payment(ICharacter actor, long id, string input, string? accountReference, decimal? authorised, decimal? authorisedFee, string operationKey)
		=> Settle(actor, () =>
	{
		var loan = FMDB.Context.LendingContracts.Find(id);
		var actorId = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (authorised.HasValue && loan?.BorrowerId == actorId &&
		    FMDB.Context.LendingPostings.Any(x => x.ContractId == id && x.OperationKey == operationKey))
			return $"That payment on loan #{id} was already received.";
		if (loan?.BorrowerId != actorId || loan.Status != "Active" || loan.EstateId.HasValue) return "You have no such repayable loan.";
		var lender = Lender(actor.Gameworld, loan);
		if (lender is null || !LocalLenders(actor).Contains(lender)) return "Visit the issuing lender to pay.";
		var zone = ZoneOf(lender);
		var currency = actor.Gameworld.Currencies.Get(loan.CurrencyId);
		if (currency != zone.Currency) return "The lender's currency changed. Contact a manager.";
		var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var terms = Terms(loan);
		if (terms.Kind == "Pawn" && terms.CustodyStatus == "Held" &&
		    actor.Gameworld.TryGetItem(terms.AssetId, true)?.Owner != lender)
			return "The lender cannot locate your pledged item. Ask a manager to resolve its custody before paying.";
		Accrue(loan, terms, now);
		var payoff = loan.Principal + loan.Interest;
		decimal amount;
		if (string.IsNullOrWhiteSpace(input)) amount = DueNow(terms, now);
		else if (input.EqualTo("all")) amount = payoff;
		else if (!currency.TryGetBaseCurrency(input, out amount)) return "Specify a currency amount or ALL.";
		if (amount <= 0) return $"No positive payment selected. Pay off this loan with LOAN PAY {id} ALL.";
		if (amount > payoff) return $"The payoff is only {Money(currency, payoff)}.";
		if (authorised.HasValue && authorised.Value != amount) return "The quoted amount changed. Request a fresh payment quote.";
		IBankAccount? account = null;
		decimal fee = 0M;
		if (!string.IsNullOrWhiteSpace(accountReference))
		{
			var found = BankRuntime.FindBankAccount(accountReference, lender as IBank, actor);
			account = found.Account;
			if (account is null) return found.Error;
			if (account.Currency != currency || !account.IsAuthorisedAccountUser(actor)) return "Choose a same-currency account you can withdraw from.";
			var check = account.CanWithdraw(amount, account.Bank == lender);
			if (!check.Truth) return check.Error;
			fee = account.BankAccountType.QuoteTransactionWithdrawalFees(account, amount);
			if (amount + fee > account.MaximumWithdrawal()) return "Your available balance cannot cover the payment and bank charges.";
			if (account.Bank != lender && account.Bank.CurrencyReserves[currency] < amount) return "The paying bank has insufficient reserves.";
		}
		else if (!CanTender(actor, currency, amount)) return "You need accessible coins and exact change, or use FROM <account>.";
		if (authorised is null)
		{
			var source = account?.AccountReference;
			actor.AddEffect(new AcceptEffect(actor, new GenericProposal(
				_ => actor.OutputHandler.Send(Payment(actor, id, input, source, amount, fee, operationKey)),
				_ => actor.OutputHandler.Send("Payment cancelled."), () => { },
				$"paying loan #{id}", "loan", "payment")), TimeSpan.FromSeconds(60));
			return $"Pay {Money(currency, amount)} from {source ?? "cash"} towards loan #{id}, leaving {Money(currency, payoff - amount)}? " +
			       (fee == 0 ? "" : $"Account charges: {Money(currency, fee)}. ") + "Type ACCEPT PAYMENT.";
		}
		if (authorisedFee != fee) return "The account charge changed. Request a fresh payment quote.";
		if (account is null) TakeSettlementCash(actor, currency, amount);
		CompletePayment(actor.Gameworld, loan, terms, lender, account, amount, now, actorId, operationKey);
		Store(loan, terms);
		return $"{Money(currency, amount)} paid. {Summary(actor, loan, terms)}" +
		       (loan.Status == "Paid" && terms.Kind == "Pawn" ? $"\nUse LOAN COLLECT {id} for your item." : "");
	}, guard: authorised.HasValue);

	private static void CompletePayment(IFuturemud world, LendingContract loan, LoanTerms terms, IFrameworkItem lender,
		IBankAccount? account, decimal amount, MudDateTime now, long actorId, string operationKey)
	{
		var id = loan.Id;
		var currency = world.Currencies.Get(loan.CurrencyId);
		if (account != null)
		{
			account.WithdrawFromTransaction(amount, $"Loan #{id} repayment");
			AdjustReserves(account.Bank, currency, -amount);
		}
		Credit(lender, amount, $"Loan #{id} repayment");
		var paid = ApplyPayment(loan, terms, amount);
		Post(loan, "Payment", paid.Principal, paid.Interest, amount, account?.AccountReference ?? "Cash", now, actorId, operationKey);
		if (loan.Principal + loan.Interest == 0)
		{
			loan.Status = terms.CustodyStatus == "Held" ? "Paid" : "Closed";
			Release(world, loan, terms);
		}
		if (DueNow(terms, now) == 0) terms.AutopayFailure = null;
		Store(loan, terms);
	}

	internal static bool CanTender(ICharacter actor, ICurrency currency, decimal amount)
	{
		if (amount <= 0 || new OtherCashPayment(currency, actor).AccessibleMoneyForPayment() < amount) return false;
		var piles = actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true);
		var selected = currency.FindCurrency(piles, amount);
		if (selected.Keys.Any(x => SpellOwnedItemValuePolicy.ContainsTemporaryValue(x.Parent))) return false;
		var tender = selected.Values.Sum(coins => coins.Sum(x => x.Key.Value * x.Value));
		if (tender < amount) return false;
		currency.FindCoinsForAmount(tender - amount, out var exact);
		return tender == amount || exact;
	}

	internal static string CollectLoan(ICharacter actor, long id, string? account = null) => Settle(actor, () =>
	{
		var loan = FMDB.Context.LendingContracts.Find(id);
		if (loan is null || loan.EstateId.HasValue) return "There is nothing for you to collect.";
		var terms = Terms(loan);
		if ((terms.SuccessorId ?? loan.BorrowerId) != CharacterInstanceIdentityComparer.IdentityId(actor)) return "There is nothing for you to collect.";
		var lender = Lender(actor.Gameworld, loan);
		if (lender is null || !LocalLenders(actor).Contains(lender)) return "Collect at the issuing lender.";
		IBankAccount? destination = null;
		if (!string.IsNullOrWhiteSpace(account))
		{
			if (loan.Surplus <= 0) return "There is no surplus to deposit.";
			var found = BankRuntime.FindBankAccount(account, lender as IBank, actor);
			destination = found.Account;
			if (destination is null) return found.Error;
			if (!destination.IsAccountOwner(actor) || destination.Currency.Id != loan.CurrencyId)
				return "Choose your own active account in the loan currency.";
		}
		if (loan.Surplus > 0 || terms.CustodyStatus == "Ready") RecordSettlement(actor.Gameworld, loan.Id, "Pledge or surplus collection");
		var result = Collect(actor, loan, terms, destination);
		Store(loan, terms);
		return result;
	}, guard: true);
}
