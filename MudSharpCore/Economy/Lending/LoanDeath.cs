#nullable enable
using MudSharp.Database;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	internal static void RecordDeath(ICharacter character, bool eligibleForProbate)
	{
		if (!HasPortfolio(character.Gameworld)) return;
		var borrower = CharacterInstanceIdentityComparer.IdentityId(character);
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts.Where(x => x.BorrowerId == borrower && x.EstateId == null &&
				(x.Status == "Requested" || x.Status == "Offered" || x.Status == "Active" || x.Status == "Recovery" ||
				 x.Status == "Paid" || x.Surplus > 0)).ToList();
			foreach (var loan in loans)
			{
				if (loan.Status is "Requested" or "Offered")
				{
					loan.Status = "Withdrawn";
					continue;
				}
				var zone = character.Gameworld.EconomicZones.Get(loan.EconomicZoneId);
				if (eligibleForProbate && zone.EstatesEnabled) continue;
				var terms = Terms(loan);
				if (terms.BorrowerDiedOutsideProbate) continue;
				var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
				Accrue(loan, terms, now);
				terms.BorrowerDiedOutsideProbate = true;
				terms.Amendment = null;
				terms.PauseUntil = null;
				terms.InterestPausedForRecovery = true;
				if (loan.Status == "Active" && loan.SecurityKey == null)
				{
					Post(loan, "WriteOff", loan.Principal - terms.RolledInterest, loan.Interest + terms.RolledInterest,
						loan.Principal + loan.Interest, "Borrower died without an estate", now);
					loan.Principal = loan.Interest = 0;
					loan.Status = "Closed";
				}
				else if (loan.Status == "Active" && terms.Bills.Count == 0)
				{
					var maturity = new MudDateTime(terms.NextDue, character.Gameworld);
					var interval = RecurringInterval.Parse(terms.Interval);
					for (var i = terms.PeriodsBilled + 1; i < terms.Periods; i++)
						maturity = interval.GetNextDateTimeAfter(maturity);
					terms.Bills.Add(new LoanInstalment { Due = maturity.GetDateTimeString(), Principal = loan.Principal, Interest = loan.Interest });
				}
				Post(loan, "Death", 0, 0, 0, "Interest frozen at death outside probate", now);
				Store(loan, terms);
			}
			FMDB.Context.SaveChanges();
		}
	}
}
