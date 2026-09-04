#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy.Currency;
using MudSharp.Framework;

namespace MudSharp.Economy;

[Flags]
public enum EconomicVolumeClassification
{
	None = 0,
	Exchange = 1 << 0,
	GeneralTransfer = 1 << 1,
	InternalMovement = 1 << 2,
	Source = 1 << 3,
	Sink = 1 << 4,
	Refund = 1 << 5
}

public enum EconomicActivityType
{
	Other = 0,
	RetailSale = 1,
	RetailPurchase = 2,
	Service = 3,
	Property = 4,
	Rent = 5,
	Auction = 6,
	Wage = 7,
	ProjectPayment = 8,
	ClanPayment = 9,
	Tax = 10,
	LegalPayment = 11,
	BankTransfer = 12,
	CashGift = 13,
	Refund = 14
}

public enum EconomyHoldingMetric
{
	PhysicalCash = 0,
	BankDeposits = 1,
	BankDebt = 2,
	VirtualBalance = 3,
	BankReserves = 4,
	PropertyEquity = 5,
	ExchangeVolume = 100,
	GrossMovement = 101,
	PcControlledWealth = 102,
	ReserveCoverage = 103,
	BroadMoneySupply = 104
}

public enum EconomicControlBucket
{
	DirectPc = 0,
	SharedPcControlled = 1,
	Npc = 2,
	Institutional = 3,
	Staff = 4,
	Unclaimed = 5,
	Ambiguous = 6
}

public enum EconomySnapshotReason
{
	Baseline = 0,
	Periodic = 1,
	FinancialPeriodRollover = 2,
	Manual = 3
}

public enum EconomyQueryWindowKind
{
	RealDay = 0,
	RealWeek = 1,
	RealMonth = 2,
	MudDay = 3,
	MudWeek = 4,
	MudMonth = 5,
	FinancialPeriod = 6
}

public sealed record EconomicActivityEvent(
	EconomicActivityType ActivityType,
	EconomicVolumeClassification Classification,
	long CurrencyId,
	decimal Amount,
	long? EconomicZoneId = null,
	long? SourceId = null,
	string? SourceType = null,
	long? DestinationId = null,
	string? DestinationType = null,
	long? ReferenceId = null,
	string? ReferenceType = null,
	string? ReferenceText = null);

public sealed record EconomyHolding(
	long? EconomicZoneId,
	long CurrencyId,
	EconomyHoldingMetric Metric,
	EconomicControlBucket ControlBucket,
	decimal Amount,
	decimal GlobalBaseValue,
	long? ControllerId = null,
	string? ControllerType = null,
	string? Description = null);

public sealed record EconomyVolumeResult(
	DateTime CoverageStartUtc,
	DateTime WindowStartUtc,
	DateTime WindowEndUtc,
	decimal ExchangeGlobalBaseValue,
	decimal MovementGlobalBaseValue,
	IReadOnlyDictionary<EconomicActivityType, decimal> ByActivity,
	IReadOnlyDictionary<EconomicControlBucket, decimal> ByPcInvolvement,
	long EventCount);

public sealed record EconomySnapshotPoint(
	long SnapshotId,
	DateTime RealDateTimeUtc,
	long? EconomicZoneId,
	long? FinancialPeriodId,
	EconomySnapshotReason Reason,
	decimal GlobalBaseValue);

public sealed record EconomyRisk(
	string Code,
	string Description,
	decimal? GlobalBaseValue = null,
	long? EconomicZoneId = null,
	long? CurrencyId = null);

public static class EconomyAnalyticsMath
{
	public static decimal Gini(IEnumerable<decimal> values)
	{
		var sorted = values.Where(x => x >= 0.0M).OrderBy(x => x).ToList();
		if (sorted.Count == 0 || sorted.Sum() == 0.0M)
		{
			return 0.0M;
		}

		var weighted = sorted.Select((value, index) => (index + 1) * value).Sum();
		return (2.0M * weighted) / (sorted.Count * sorted.Sum()) -
		       (sorted.Count + 1.0M) / sorted.Count;
	}

	public static DateTime NextPeriodicDue(DateTime lastSuccessfulPeriodicUtc, TimeSpan interval)
	{
		return lastSuccessfulPeriodicUtc.Add(interval);
	}

	public static bool IsValidSnapshotInterval(TimeSpan interval)
	{
		return interval >= TimeSpan.FromHours(1.0);
	}

	public static decimal ConvertGlobalBaseValue(decimal globalBaseValue,
		decimal baseCurrencyToGlobalBaseCurrencyConversion)
	{
		if (baseCurrencyToGlobalBaseCurrencyConversion <= 0.0M)
		{
			throw new ArgumentOutOfRangeException(nameof(baseCurrencyToGlobalBaseCurrencyConversion),
				"A display currency must have a positive global-base conversion factor.");
		}

		return globalBaseValue / baseCurrencyToGlobalBaseCurrencyConversion;
	}
}

public interface IEconomyAnalyticsService
{
	bool SnapshotsEnabled { get; }
	TimeSpan SnapshotInterval { get; }
	bool RolloverSnapshotsEnabled { get; }
	ICurrency GlobalDisplayCurrency { get; }
	DateTime? LastSnapshotUtc { get; }
	DateTime? NextPeriodicSnapshotUtc { get; }
	DateTime? ActivityCoverageStartUtc { get; }

	void Initialise();
	void RecordActivity(EconomicActivityEvent activity);
	EconomicControlBucket ResolveControl(string? frameworkItemType, long? frameworkItemId);
	IReadOnlyList<EconomyHolding> GetCurrentHoldings(long? economicZoneId = null, long? currencyId = null);
	EconomyVolumeResult GetVolume(EconomyQueryWindowKind window, long? economicZoneId = null,
		long? currencyId = null, long? financialPeriodId = null);
	IReadOnlyList<EconomySnapshotPoint> GetTrends(EconomyHoldingMetric? metric,
		EconomicVolumeClassification? volumeClassification, long? economicZoneId = null,
		long? currencyId = null, int count = 30);
	IReadOnlyList<EconomyRisk> GetRisks(long? economicZoneId = null);
	IReadOnlyList<EconomyRisk> GetRisks(IReadOnlyList<EconomyHolding> currentHoldings,
		long? economicZoneId = null);
	long? TakeSnapshot(EconomySnapshotReason reason, long? economicZoneId = null,
		long? financialPeriodId = null);
	void NotifyFinancialPeriodClosed(long economicZoneId, long financialPeriodId);
	void SetSnapshotsEnabled(bool enabled);
	bool TrySetSnapshotInterval(TimeSpan interval, out string error);
	void SetRolloverSnapshotsEnabled(bool enabled);
	bool TrySetGlobalDisplayCurrency(ICurrency currency, out string error);
}
