#nullable enable
using System.Text.Json;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy.Lending;

internal sealed partial class LoanTerms
{
	public long? AutopayAccountId { get; set; }
	public string? AutopayLastAttempt { get; set; }
	public string? AutopayFailure { get; set; }
	public string Kind { get; set; } = "Unsecured";
	public decimal Rate { get; set; }
	public string Interval { get; set; } = "every 1 month";
	public int Periods { get; set; } = 12;
	public int GraceDays { get; set; } = 7;
	public decimal Instalment { get; set; }
	public string Minimum { get; set; } = "Interest";
	public bool Recourse { get; set; } = true;
	public long AuctionHouseId { get; set; }
	public long AssetId { get; set; }
	public long? SaleOrderId { get; set; }
	public long? ExistingLeaseId { get; set; }
	public decimal Appraisal { get; set; }
	public decimal Price { get; set; }
	public decimal DownPayment { get; set; }
	public List<LoanSellerShare> SellerShares { get; set; } = new();
	public string CustodyStatus { get; set; } = "None";
	public bool RetainedItemDisposed { get; set; }
	public long? LoanEstateClaimId { get; set; }
	public bool InterestPausedForRecovery { get; set; }
	public bool BorrowerDiedOutsideProbate { get; set; }
	public long? SuccessorId { get; set; }
	public decimal Advance { get; set; }
	public decimal Quantum { get; set; } = 1.0M;
	public bool UseShopAccount { get; set; }
	public decimal LiquidityFloor { get; set; }
	public DateTime OfferExpiryUtc { get; set; }
	public long? ApprovedBy { get; set; }
	public string PeriodStart { get; set; } = "";
	public string LastAccrual { get; set; } = "";
	public string NextDue { get; set; } = "";
	public int PeriodsBilled { get; set; }
	public decimal FractionalInterest { get; set; }
	public decimal RolledInterest { get; set; }
	public List<LoanInstalment> Bills { get; set; } = new();
	public LoanAmendment? Amendment { get; set; }
	public string? PauseUntil { get; set; }
	public bool ChargeDuringPause { get; set; }
}

internal sealed record LoanAmendment(string Kind, decimal Instalment, int Periods, bool ChargeInterest,
	long ManagerId, string Reason, DateTime ExpiresUtc);

internal sealed record LoanSellerShare(long OwnerId, string OwnerType, decimal Share);

internal sealed class LoanInstalment
{
	public string Due { get; set; } = "";
	public decimal Principal { get; set; }
	public decimal Interest { get; set; }
}

internal static partial class LoanService
{
	internal static LoanTerms Terms(LendingContract contract) =>
		JsonSerializer.Deserialize<LoanTerms>(contract.Definition) ?? throw new InvalidOperationException("Loan terms are missing.");

	internal static void Store(LendingContract contract, LoanTerms terms) =>
		contract.Definition = JsonSerializer.Serialize(terms);

	internal static void StartSchedule(LoanTerms terms, MudDateTime start)
	{
		terms.Bills.Clear();
		terms.AutopayLastAttempt = null;
		terms.AutopayFailure = null;
		terms.PeriodStart = start.GetDateTimeString();
		terms.LastAccrual = terms.PeriodStart;
		terms.NextDue = RecurringInterval.Parse(terms.Interval).GetNextDateTimeAfter(start).GetDateTimeString();
		terms.PeriodsBilled = 0;
	}

	internal static decimal AccrueSlice(decimal principal, decimal rate, decimal fraction, LoanTerms terms)
	{
		var exact = terms.FractionalInterest + principal * rate * fraction;
		var billed = decimal.Floor(exact / terms.Quantum) * terms.Quantum;
		terms.FractionalInterest = exact - billed;
		return billed;
	}

	internal static void Accrue(LendingContract contract, LoanTerms terms, MudDateTime at)
	{
		if (contract.Status != "Active" || contract.EstateId.HasValue || terms.InterestPausedForRecovery || terms.LastAccrual.Length == 0) return;
		var last = new MudDateTime(terms.LastAccrual, at.Gameworld);
		if (at <= last) return;
		var start = new MudDateTime(terms.PeriodStart, at.Gameworld);
		var next = new MudDateTime(terms.NextDue, at.Gameworld);
		var interval = RecurringInterval.Parse(terms.Interval);
		var billedPrincipal = terms.Bills.Sum(x => x.Principal);
		var billedInterest = terms.Bills.Sum(x => x.Interest);
		if (terms.PauseUntil is not null)
		{
			var resume = new MudDateTime(terms.PauseUntil, at.Gameworld);
			var through = resume < at ? resume : at;
			if (terms.ChargeDuringPause && last < through)
			{
				var period = MudInstant.FromMudDateTime(next).Ticks - MudInstant.FromMudDateTime(start).Ticks;
				var elapsed = MudInstant.FromMudDateTime(through).Ticks - MudInstant.FromMudDateTime(last).Ticks;
				contract.Interest += AccrueSlice(contract.Principal, terms.Rate, (decimal)elapsed / period, terms);
			}
			last = through;
			terms.LastAccrual = last.GetDateTimeString();
			if (through >= resume) terms.PauseUntil = null;
		}
		while (last < at)
		{
			var matured = terms.PeriodsBilled >= terms.Periods;
			var end = !matured && next < at ? next : at;
			var denominator = MudInstant.FromMudDateTime(next).Ticks - MudInstant.FromMudDateTime(start).Ticks;
			if (denominator <= 0) throw new InvalidOperationException("Loan interval must advance its calendar.");
			var elapsed = MudInstant.FromMudDateTime(end).Ticks - MudInstant.FromMudDateTime(last).Ticks;
			var interest = AccrueSlice(contract.Principal, terms.Rate, (decimal)elapsed / denominator, terms);
			contract.Interest += interest;
			if (matured)
			{
				// After maturity the remaining balance is callable. Use the last period's length.
				if (interest > 0)
				{
					var bill = terms.Bills.LastOrDefault();
					if (bill is null) terms.Bills.Add(new LoanInstalment { Due = terms.NextDue, Interest = interest });
					else bill.Interest += interest;
				}
			}
			else if (end >= next)
			{
				terms.PeriodsBilled++;
				var final = terms.PeriodsBilled == terms.Periods;
				var unbilledPrincipal = Math.Max(0M, contract.Principal - billedPrincipal);
				var principalDue = final ? unbilledPrincipal : terms.Minimum == "Principal" ? Math.Min(terms.Instalment, unbilledPrincipal) : 0M;
				var interestDue = final || terms.Minimum != "None" ? contract.Interest - billedInterest : 0M;
				if (principalDue + interestDue > 0)
					terms.Bills.Add(new LoanInstalment { Due = terms.NextDue, Principal = principalDue, Interest = interestDue });
				billedPrincipal += principalDue;
				billedInterest += interestDue;
				if (!final)
				{
					start = next;
					next = interval.GetNextDateTimeAfter(start);
					terms.PeriodStart = start.GetDateTimeString();
					terms.NextDue = next.GetDateTimeString();
				}
			}
			last = end;
		}
		terms.LastAccrual = at.GetDateTimeString();
		Store(contract, terms);
	}

	internal static (decimal Principal, decimal Interest) ApplyPayment(LendingContract contract, LoanTerms terms, decimal amount)
	{
		if (amount <= 0 || amount > contract.Principal + contract.Interest) throw new ArgumentOutOfRangeException(nameof(amount));
		var remaining = amount;
		decimal interest = 0, principal = 0;
		foreach (var bill in terms.Bills)
		{
			var paid = Math.Min(remaining, bill.Interest);
			bill.Interest -= paid;
			interest += paid;
			remaining -= paid;
		}
		foreach (var bill in terms.Bills)
		{
			var paid = Math.Min(remaining, bill.Principal);
			bill.Principal -= paid;
			principal += paid;
			remaining -= paid;
		}
		var unbilledInterest = Math.Min(remaining, contract.Interest - interest);
		interest += unbilledInterest;
		principal += remaining - unbilledInterest;
		contract.Interest -= interest;
		contract.Principal -= principal;
		// Capitalised interest remains interest for tax when collected.
		var paidRolledInterest = Math.Min(terms.RolledInterest, principal);
		terms.RolledInterest -= paidRolledInterest;
		terms.Bills.RemoveAll(x => x.Principal + x.Interest == 0);
		Store(contract, terms);
		return (principal - paidRolledInterest, interest + paidRolledInterest);
	}
}
