#nullable enable
using MudSharp.Community;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	internal static long? EstateLoanId(IEstate estate)
	{
		using var handle = new FMDB();
		var borrower = CharacterInstanceIdentityComparer.IdentityId(estate.Character);
		return FMDB.Context.LendingContracts.Where(x => x.BorrowerId == borrower && x.EconomicZoneId == estate.EconomicZone.Id)
			.OrderBy(x => x.Id).Select(x => (long?)x.Id).FirstOrDefault();
	}

	internal static T EstateTransaction<T>(IEstate estate, Func<T> action)
	{
		if (GuardingSettlement) return action();
		var id = EstateLoanId(estate);
		if (!id.HasValue) return action();
		try
		{
			return InTransaction(estate.Gameworld, () =>
			{
				RecordSettlement(estate.Gameworld, id.Value, "Estate servicing");
				return action();
			});
		}
		catch (Exception failure) when (IsDatabaseFailure(failure))
		{
			MudSharp.Server.ExceptionReporter.WriteFallback(null,
				MudSharp.Server.ExceptionReporter.Format(failure, "Estate settlement deferred after rollback", false));
			return default!;
		}
	}

	internal static void RecordSettlement(IFuturemud world, long id, string detail)
	{
		var loan = FMDB.Context.LendingContracts.Find(id);
		if (loan == null) throw new InvalidOperationException("The settlement loan no longer exists.");
		Post(loan, "Settlement", 0, 0, 0, detail,
			world.EconomicZones.Get(loan.EconomicZoneId).FinancialPeriodReferenceCalendar.CurrentDateTime);
	}
}
