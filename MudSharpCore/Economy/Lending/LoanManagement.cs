#nullable enable
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using AcceptEffect = MudSharp.Effects.Concrete.Accept;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	internal static string Manage(ICharacter actor, IFrameworkItem lender, string action, long id, StringStack args)
	{
		if (!LocalLenders(actor).Contains(lender) || !IsManager(actor, lender)) return "You do not manage this lender here.";
		if (id == 0 || action == "list")
		{
			using (new FMDB())
			{
				var rows = FMDB.Context.LendingContracts.AsNoTracking().Where(x => x.LenderType == lender.FrameworkItemType &&
					x.LenderId == lender.Id && (x.Status == "Active" || x.Status == "Offered" || x.Status == "Requested" ||
						x.Status == "Recovery" || x.Status == "Paid" || x.Surplus > 0))
					.OrderByDescending(x => x.Id).Take(100).ToList();
				var zone = ZoneOf(lender);
				return rows.Count == 0 ? "The loan book is empty." : string.Join("\n", rows.Select(x =>
				{
					var terms = Terms(x);
					var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
					Accrue(x, terms, now);
					return $"#{x.Id} {x.Status} — borrower #{x.BorrowerId}, principal {Money(zone.Currency, x.Principal)}, due {Money(zone.Currency, DueNow(terms, now))}";
				}));
			}
		}
		if (action is "enforce" or "writeoff" or "relist")
		{
			var reason = args.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(reason)) return "Supply an audit reason.";
			actor.AddEffect(new AcceptEffect(actor, new GenericProposal(
				_ => actor.OutputHandler.Send(ManageConfirmed(actor, lender, action, id, new StringStack(reason))),
				_ => actor.OutputHandler.Send("Action cancelled."), () => { },
				$"{action} on loan #{id}", "loan", action)), TimeSpan.FromSeconds(60));
			return $"Confirm {action} on loan #{id} with ACCEPT {action.ToUpperInvariant()}.";
		}
		return ManageConfirmed(actor, lender, action, id, args);
	}

	private static string ManageConfirmed(ICharacter actor, IFrameworkItem lender, string action, long id, StringStack args)
		=> Settle(actor, () =>
	{
		if (!LocalLenders(actor).Contains(lender) || !IsManager(actor, lender)) return "Your authority or location changed.";
		var loan = FMDB.Context.LendingContracts.Find(id);
		if (loan is null || loan.LenderType != lender.FrameworkItemType || loan.LenderId != lender.Id)
			return "No such loan at this establishment.";
		var manager = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (loan.BorrowerId == manager) return "Another manager must handle your loan.";
		var zone = ZoneOf(lender);
		var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var terms = Terms(loan);
		if (action == "successor" && actor.IsAdministrator() && terms.BorrowerDiedOutsideProbate && loan.EstateId == null)
		{
			if (!long.TryParse(args.PopSpeech(), out var target) || actor.Gameworld.TryGetCharacter(target, true) is not { } recipient ||
			    args.IsFinished) return "Supply the successor's character ID and the reason for this succession decision.";
			terms.SuccessorId = CharacterInstanceIdentityComparer.IdentityId(recipient);
			Store(loan, terms);
			Post(loan, "Successor", 0, 0, 0, $"Collection assigned to #{terms.SuccessorId}. {args.SafeRemainingArgument}", now, manager);
			return $"The successor can use LOAN COLLECT {id} at this lender.";
		}
		if (action is "approve" or "offer" or "deny")
		{
			if (loan.Status != "Requested" || terms.OfferExpiryUtc <= DateTime.UtcNow) return "This application is no longer awaiting approval.";
			if (action != "deny" && (!actor.Gameworld.GetStaticBool("LoansEnabled") || FMDB.Context.LendingProducts.Find(loan.ProductId)?.Enabled != true))
				return "This product is unavailable.";
			if (action != "deny")
			{
				var borrower = actor.Gameworld.TryGetCharacter(loan.BorrowerId, true);
				var current = FMDB.Context.LendingProducts.Find(loan.ProductId)!;
				if (borrower is null || ProductTerms(current).Kind != terms.Kind) return "The borrower or original product is unavailable.";
				if (!Underwrite(borrower, lender, current, terms.Advance, terms.Appraisal, Exposure(lender, loan.BorrowerId), out var error))
					return error;
				if (!LocalLenders(actor).Contains(lender) || !IsManager(actor, lender)) return "Your lending authority changed.";
			}
			loan.Status = action == "deny" ? "Declined" : "Offered";
			terms.ApprovedBy = manager;
			terms.OfferExpiryUtc = DateTime.UtcNow.AddDays(7);
			Store(loan, terms);
			Post(loan, loan.Status, 0, 0, terms.Advance, args.SafeRemainingArgument, now, manager);
			return $"Loan #{id}: {loan.Status}.";
		}
		if (loan.EstateId.HasValue) return "This loan is being handled through probate.";
		if (loan.Status == "Recovery" && action == "relist") return Enforce(actor, loan, terms);
		if (loan.Status != "Active") return "This loan is not active.";
		Accrue(loan, terms, now);
		switch (action)
		{
			case "enforce":
				if (terms.PauseUntil is not null || terms.Bills.FirstOrDefault() is not { } bill ||
				    now <= new MudDateTime(bill.Due, actor.Gameworld) + MudTimeSpan.FromDays(terms.GraceDays))
					return "This loan is current, paused or still within its grace period.";
				if (loan.SecurityKey == null) return "There is no security to seize. Offer a plan or write off the outstanding balance.";
				return Enforce(actor, loan, terms);
			case "writeoff":
				Post(loan, "WriteOff", loan.Principal - terms.RolledInterest, loan.Interest + terms.RolledInterest,
					loan.Principal + loan.Interest, args.SafeRemainingArgument, now, manager);
				loan.Principal = loan.Interest = terms.RolledInterest = 0;
				terms.Bills.Clear();
				terms.Amendment = null;
				loan.Status = terms.CustodyStatus == "Held" ? "Paid" : "Closed";
				Release(actor.Gameworld, loan, terms);
				Store(loan, terms);
				return $"Loan #{id} written off and its security released.";
			case "pause":
			case "plan":
			case "refinance":
				if (terms.PauseUntil is not null) return "Wait for the existing pause to end before changing the schedule.";
				if (!int.TryParse(args.PopSpeech(), out var periods) || periods is < 1 or > 360)
					return "Supply a duration from 1 to 360 days (pause) or periods (plan/refinance).";
				var parameter = args.PopSpeech();
				decimal instalment = 0;
				if (action == "pause" && !parameter.EqualTo("interest") && !parameter.EqualTo("free"))
					return "Specify INTEREST or FREE for the pause.";
				if (action != "pause" && parameter != "0" &&
				    (!zone.Currency.TryGetBaseCurrency(parameter, out instalment) || instalment <= 0 || instalment % terms.Quantum != 0))
					return "Supply a quoted principal instalment in this currency, or 0 for interest only.";
				var reason = args.SafeRemainingArgument;
				if (string.IsNullOrWhiteSpace(reason)) return "Supply a reason for the borrower.";
				terms.Amendment = new LoanAmendment(action, instalment, periods, parameter.EqualTo("interest"), manager, reason, DateTime.UtcNow.AddDays(7));
				Store(loan, terms);
				Post(loan, "Proposal", 0, 0, 0, $"{action}: {reason}", now, manager);
				return $"Offered {AmendmentDescription(terms.Amendment, zone.Currency)}. The borrower can use LOAN ACCEPT {id}.";
			default:
				return "Use APPROVE, DENY, PLAN, PAUSE, REFINANCE, ENFORCE, RELIST or WRITEOFF.";
		}
	}, guard: true);

	private static string AmendmentDescription(LoanAmendment a, ICurrency currency) => a.Kind == "pause"
		? $"{a.Periods} days with {(a.ChargeInterest ? "continuing" : "no")} interest and all dues deferred"
		: $"{a.Periods} periods of interest plus {Money(currency, a.Instalment)} principal per period" +
		  (a.Kind == "refinance" ? ", rolling outstanding interest into principal" : "");

	private static string AcceptAmendment(ICharacter actor, LendingContract loan, LoanTerms terms, IFrameworkItem lender, MudDateTime now)
	{
		var proposal = terms.Amendment!;
		if (!LocalLenders(actor).Contains(lender) || loan.EstateId.HasValue) return "Visit the lender to change an active loan.";
		if (proposal.ExpiresUtc <= DateTime.UtcNow) return "This amendment has expired.";
		var manager = actor.Gameworld.TryGetCharacter(proposal.ManagerId, true);
		if (manager is null || !IsManager(manager, lender))
			return "The issuing manager no longer represents the lender. Ask for a new proposal.";
		Accrue(loan, terms, now);
		var currency = ZoneOf(lender).Currency;
		var actorId = CharacterInstanceIdentityComparer.IdentityId(actor);
		var destination = loan;
		if (proposal.Kind == "pause")
		{
			var shift = MudTimeSpan.FromDays(proposal.Periods);
			var resume = now + shift;
			terms.PauseUntil = resume.GetDateTimeString();
			terms.ChargeDuringPause = proposal.ChargeInterest;
			terms.PeriodStart = (new MudDateTime(terms.PeriodStart, actor.Gameworld) + shift).GetDateTimeString();
			terms.NextDue = (new MudDateTime(terms.NextDue, actor.Gameworld) + shift).GetDateTimeString();
			foreach (var bill in terms.Bills) bill.Due = terms.PauseUntil;
		}
		else
		{
			if (proposal.Kind == "refinance")
			{
				var amount = loan.Principal + loan.Interest;
				var product = FMDB.Context.LendingProducts.Find(loan.ProductId);
				var existingExposure = Exposure(lender, actorId) - loan.Principal;
				if (product is null) return "The original lending product is unavailable.";
				if (loan.SecurityKey is null && terms.CustodyStatus is "Retained" or "Auctioned")
				{
					// The asset has already settled part of this debt. Underwrite the deficiency as unsecured.
					var current = ProductDefinition(product);
					current.Terms.Kind = "Unsecured";
					product = new LendingProduct { Definition = System.Text.Json.JsonSerializer.Serialize(current), Enabled = product.Enabled };
					terms.Kind = "Unsecured";
					terms.Appraisal = 0;
				}
				else if (ProductTerms(product).Kind != terms.Kind) return "The original lending product has changed security type.";
				var appraisal = terms.Kind == "Mortgage" ? Math.Min(terms.Price, terms.Appraisal) : terms.Appraisal;
				if (!Underwrite(actor, lender, product, amount, appraisal, existingExposure, out var error, servicing: true)) return error;
				if (!LocalLenders(actor).Contains(lender)) return "Return to the lender to accept.";
				destination = new LendingContract
				{
					BorrowerId = actorId, LenderType = loan.LenderType, LenderId = loan.LenderId, ProductId = loan.ProductId,
					CurrencyId = loan.CurrencyId, EconomicZoneId = loan.EconomicZoneId, Status = "Active",
					Principal = amount, SecurityKey = loan.SecurityKey, Definition = loan.Definition
				};
				terms.RolledInterest += loan.Interest;
				terms.Advance = amount;
				loan.SecurityKey = null;
				loan.Principal = loan.Interest = 0;
				loan.Status = "Closed";
				FMDB.Context.SaveChanges();
				FMDB.Context.LendingContracts.Add(destination);
				FMDB.Context.SaveChanges();
				Post(loan, "Refinanced", 0, 0, amount, $"Replaced by #{destination.Id}", now, actorId);
			}
			terms.Periods = proposal.Periods;
			terms.InterestPausedForRecovery = false;
			terms.Instalment = proposal.Instalment;
			terms.Minimum = terms.Instalment == 0 ? "Interest" : "Principal";
			StartSchedule(terms, now);
		}
		terms.Amendment = null;
		Store(destination, terms);
		AfterCommit(() => TrackAutopay(actor.Gameworld, destination));
		Post(destination, proposal.Kind, 0, 0, 0, $"Agreed by borrower. Manager #{proposal.ManagerId}. {proposal.Reason}. {AmendmentDescription(proposal, currency)}. Prior loan #{loan.Id}.", now, actorId);
		return $"Agreed. {Summary(actor, destination, terms)}";
	}
}
