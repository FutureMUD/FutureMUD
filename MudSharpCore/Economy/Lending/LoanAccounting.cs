#nullable enable
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Economy.Tax;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	private static readonly ConditionalWeakTable<IFuturemud, object> Portfolios = new();
	private static readonly ConditionalWeakTable<IFuturemud, Dictionary<long, RetainedPawn>> RetainedItems = new();
	internal sealed record RetainedPawn(long ContractId, long ItemId, long LenderId, string LenderType,
		long ZoneId, long CurrencyId, decimal Value, decimal StockCost);

	private static void RegisterRetainedPawn(IFuturemud world, LendingContract loan, LoanTerms terms, decimal interest)
	{
		RetainedItems.GetOrCreateValue(world)[terms.AssetId] = new RetainedPawn(loan.Id, terms.AssetId, loan.LenderId,
			loan.LenderType, loan.EconomicZoneId, loan.CurrencyId, terms.Appraisal, terms.Appraisal - interest);
	}

	internal static void EndRetainedPawnOwnership(IGameItem item)
	{
		if (item.Gameworld is not { } world || !RetainedItems.TryGetValue(world, out var items) ||
		    !items.TryGetValue(item.Id, out var pawn)) return;
		using (new FMDB())
		{
			var loan = FMDB.Context.LendingContracts.Find(pawn.ContractId);
			if (loan != null)
			{
				var terms = Terms(loan);
				terms.RetainedItemDisposed = true;
				Store(loan, terms);
				FMDB.Context.SaveChanges();
			}
			items.Remove(item.Id);
		}
	}

	internal static bool IsRetainedPawn(IGameItem item) =>
		item.Gameworld != null && RetainedItems.TryGetValue(item.Gameworld, out var items) &&
		item.OwnershipReference?.FrameworkItemType is "Bank" or "Shop" && items.ContainsKey(item.Id);

	internal static IReadOnlyList<RetainedPawn> RetainedPawns(IFuturemud world, long? zoneId, long? currencyId,
		IFrameworkItem? lender = null)
	{
		if (!RetainedItems.TryGetValue(world, out var items)) return Array.Empty<RetainedPawn>();
		var candidates = items.Values.Where(x => (!zoneId.HasValue || x.ZoneId == zoneId) &&
			(!currencyId.HasValue || x.CurrencyId == currencyId) &&
			(lender == null || lender.FrameworkItemEquals(x.LenderId, x.LenderType))).ToList();
		if (candidates.Count == 0) return candidates;
		// An item outside the loaded world may have been destroyed. Never recognise a missing asset.
		var ids = candidates.Select(x => x.ItemId).ToList();
		using (new FMDB())
		{
			var keys = ids.Select(id => $"Item:{id}").ToList();
			var pledged = FMDB.Context.LendingContracts.Where(x => x.SecurityKey != null && keys.Contains(x.SecurityKey))
				.Select(x => x.SecurityKey).ToHashSet();
			var owners = FMDB.Context.GameItems.AsNoTracking().Where(x => ids.Contains(x.Id))
				.Select(x => new { x.Id, x.OwnerId, x.OwnerType }).ToDictionary(x => x.Id);
			return candidates.Where(asset =>
			{
				if (pledged.Contains($"Item:{asset.ItemId}")) return false;
				var live = world.Items.Get(asset.ItemId);
				if (live != null) return !live.Deleted && live.Owner.FrameworkItemEquals(asset.LenderId, asset.LenderType);
				return owners.TryGetValue(asset.ItemId, out var stored) && stored.OwnerId == asset.LenderId &&
				       stored.OwnerType == asset.LenderType;
			}).ToList();
		}
	}

	internal static decimal? RetainedStockCost(IShop shop, IGameItem item) =>
		shop.Gameworld != null && RetainedItems.TryGetValue(shop.Gameworld, out var items) &&
		items.TryGetValue(item.Id, out var asset) && shop.FrameworkItemEquals(asset.LenderId, asset.LenderType)
			? asset.StockCost : null;
	internal static bool HasPortfolio(IFuturemud? world) => world != null && Portfolios.TryGetValue(world, out _);
	internal static bool NeedsServicing(IEconomicZone zone, IFrameworkItem? lender = null)
	{
		if (!HasPortfolio(zone.Gameworld)) return false;
		using (new FMDB())
		{
			var loans = FMDB.Context.LendingContracts.Where(x => x.EconomicZoneId == zone.Id &&
				(x.Status == "Active" || x.Status == "Recovery" || x.Surplus > 0 || x.SecurityKey != null || x.Status == "Paid"));
			if (lender != null) loans = loans.Where(x => x.LenderType == lender.FrameworkItemType && x.LenderId == lender.Id);
			return loans.Any();
		}
	}
	internal static void LoadPortfolio(IFuturemud world)
	{
		using (new FMDB())
		{
			if (!FMDB.Context.LendingContracts.Any()) return;
			Portfolios.GetOrCreateValue(world);
			foreach (var loan in FMDB.Context.LendingContracts.AsNoTracking().Where(x => x.Status == "Active" && x.EstateId == null)) TrackAutopay(world, loan);
			var retained = from post in FMDB.Context.LendingPostings
			               join loan in FMDB.Context.LendingContracts on post.ContractId equals loan.Id
			               where post.Kind == "CollateralRecovery"
			               orderby post.Id
			               select new { Loan = loan, post.Interest };
			foreach (var receipt in retained.AsNoTracking())
			{
				var terms = Terms(receipt.Loan);
				if (terms.Kind == "Pawn" && terms.CustodyStatus == "Retained" && !terms.RetainedItemDisposed)
					RegisterRetainedPawn(world, receipt.Loan, terms, receipt.Interest);
			}
		}
	}

	internal static void DailyUpdate(IEconomicZone zone)
	{
		if (!HasPortfolio(zone.Gameworld)) return;
		using (new FMDB())
		{
			var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			long last = 0;
			while (true)
			{
				var loans = FMDB.Context.LendingContracts.Where(x => x.EconomicZoneId == zone.Id && x.Id > last &&
						x.Status == "Active" && x.EstateId == null).OrderBy(x => x.Id).Take(100).ToList();
				if (loans.Count == 0) break;
				foreach (var loan in loans) Accrue(loan, Terms(loan), now);
				FMDB.Context.SaveChanges();
				last = loans[^1].Id;
			}
		}
	}

	internal static IReadOnlyList<LendingContract> Balances(IFuturemud world, long? zoneId, long? currencyId)
	{
		if (!HasPortfolio(world)) return Array.Empty<LendingContract>();
		using (new FMDB())
		{
			var query = FMDB.Context.LendingContracts.AsNoTracking().Where(x => x.Principal != 0 || x.Interest != 0 || x.Surplus != 0);
			if (zoneId.HasValue) query = query.Where(x => x.EconomicZoneId == zoneId);
			if (currencyId.HasValue) query = query.Where(x => x.CurrencyId == currencyId);
			var loans = query.ToList();
			foreach (var loan in loans)
			{
				var zone = world.EconomicZones.Get(loan.EconomicZoneId);
				if (zone != null) Accrue(loan, Terms(loan), zone.FinancialPeriodReferenceCalendar.CurrentDateTime);
			}
			return loans;
		}
	}

	internal static Dictionary<long, decimal> ShopProfit(IEconomicZone zone, DateTime start, DateTime end, bool deductLosses)
	{
		if (!HasPortfolio(zone.Gameworld)) return new Dictionary<long, decimal>();
		using (new FMDB())
		{
			return (from post in FMDB.Context.LendingPostings
					join loan in FMDB.Context.LendingContracts on post.ContractId equals loan.Id
					where loan.LenderType == "Shop" && loan.EconomicZoneId == zone.Id && post.RealDate >= start && post.RealDate < end &&
					      (post.Kind == "Payment" || post.Kind == "RecoveryPayment" || post.Kind == "EstatePayment" || post.Kind == "WriteOff")
					group post by loan.LenderId into book
					select new { Id = book.Key, Profit = book.Sum(x => x.Kind == "WriteOff" ? (deductLosses ? -x.Principal : 0M) : x.Interest) })
				.ToDictionary(x => x.Id, x => x.Profit);
		}
	}

	internal static decimal BankTaxWithheld(IEconomicZone zone, DateTime start, DateTime end)
	{
		if (!HasPortfolio(zone.Gameworld)) return 0M;
		using (new FMDB())
		{
			return (from post in FMDB.Context.LendingPostings
			        join loan in FMDB.Context.LendingContracts on post.ContractId equals loan.Id
			        where loan.EconomicZoneId == zone.Id && post.Kind == "Tax" && post.RealDate >= start && post.RealDate < end
			        select (decimal?)post.Amount).Sum() ?? 0;
		}
	}

	private static void RecordMovement(LendingContract loan, string kind, decimal interest, decimal amount, MudDateTime now)
	{
		if (kind is not ("Advance" or "Payment" or "EstatePayment" or "RecoveryPayment")) return;
		var world = now.Gameworld;
		var outgoing = kind == "Advance";
		world.EconomyAnalytics?.RecordActivity(new EconomicActivityEvent(EconomicActivityType.Loan,
			EconomicVolumeClassification.GeneralTransfer, loan.CurrencyId, amount, loan.EconomicZoneId,
			outgoing ? loan.LenderId : loan.BorrowerId, outgoing ? loan.LenderType : "Character",
			outgoing ? loan.BorrowerId : loan.LenderId, outgoing ? "Character" : loan.LenderType,
			loan.Id, "Loan", kind));
		if (outgoing || interest <= 0 || Lender(world, loan) is not IBank bank) return;
		var zone = bank.EconomicZone;
		var rate = zone.ProfitTaxes.OfType<BankInterestTax>().Sum(x => x.Rate);
		var tax = interest * Math.Min(1M, rate);
		if (tax <= 0) return;
		Credit(bank, -tax, $"Lending tax on loan #{loan.Id}");
		var revenueBeforeTax = zone.TotalRevenueHeld;
		if (GuardingSettlement) OnRollback(() => zone.TotalRevenueHeld = revenueBeforeTax);
		zone.TotalRevenueHeld += tax;
		zone.Changed = true;
		VirtualCashLedger.Credit(zone, zone.Currency, tax, null, bank, "LendingTax", $"Interest on loan #{loan.Id}", now);
		Post(loan, "Tax", 0, 0, tax, "Tax on realised bank interest", now);
	}
}
