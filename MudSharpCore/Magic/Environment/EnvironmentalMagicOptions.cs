#nullable enable

using System.Globalization;

namespace MudSharp.Magic.Environment;

public sealed record EnvironmentalMagicOptions
{
	public double ActiveCadenceSeconds { get; init; } = 60.0;
	public double ReconciliationSeconds { get; init; } = 3600.0;
	public int MaximumCellVisits { get; init; } = 1024;
	public int MaximumOutputWork { get; init; } = 8192;
	public double SoftBudgetMilliseconds { get; init; } = 5.0;
	public double SlowProgMilliseconds { get; init; } = 5.0;

	public EnvironmentalMagicOptions WithSetting(string name, string value)
	{
		var candidate = name.ToLowerInvariant() switch
		{
			"environmentalmagicactiveseconds" => this with { ActiveCadenceSeconds = double.Parse(value, CultureInfo.InvariantCulture) },
			"environmentalmagicauditseconds" => this with { ReconciliationSeconds = double.Parse(value, CultureInfo.InvariantCulture) },
			"environmentalmagicmaximumcellvisits" => this with { MaximumCellVisits = int.Parse(value, CultureInfo.InvariantCulture) },
			"environmentalmagicmaximumoutputwork" => this with { MaximumOutputWork = int.Parse(value, CultureInfo.InvariantCulture) },
			"environmentalmagicbudgetmilliseconds" => this with { SoftBudgetMilliseconds = double.Parse(value, CultureInfo.InvariantCulture) },
			_ => this
		};
		candidate.Validate();
		return candidate;
	}

	public void Validate()
	{
		if (!double.IsFinite(ActiveCadenceSeconds) || ActiveCadenceSeconds < 1.0 ||
			!double.IsFinite(ReconciliationSeconds) || ReconciliationSeconds < ActiveCadenceSeconds ||
			MaximumCellVisits < 1 || MaximumOutputWork < 8 ||
			!double.IsFinite(SoftBudgetMilliseconds) || SoftBudgetMilliseconds <= 0 ||
			!double.IsFinite(SlowProgMilliseconds) || SlowProgMilliseconds <= 0)
			throw new ArgumentException("Environmental magic budgets must be finite and positive; audit cadence must be at least the active cadence.");
	}

	public static EnvironmentalMagicOptions FromGameworld(IFuturemud world) => new()
	{
		ActiveCadenceSeconds = world.GetStaticDouble("EnvironmentalMagicActiveSeconds"),
		ReconciliationSeconds = world.GetStaticDouble("EnvironmentalMagicAuditSeconds"),
		MaximumCellVisits = world.GetStaticInt("EnvironmentalMagicMaximumCellVisits"),
		MaximumOutputWork = world.GetStaticInt("EnvironmentalMagicMaximumOutputWork"),
		SoftBudgetMilliseconds = world.GetStaticDouble("EnvironmentalMagicBudgetMilliseconds")
	};
}

public sealed record EnvironmentalMagicDiagnostics(int Configured, int ActiveProduction, int ActiveMaintenance,
	int Dormant, int Dirty, int Faulted, int ProductionQueue, int AuditQueue, int DiscoveryRemaining,
	int LastCellVisits, int LastEvaluations, int LastInputProgExecutions, int LastWrites,
	double LastPumpMilliseconds, double MaximumPumpMilliseconds, double OldestReadySeconds,
	double OldestAuditSeconds, long BudgetLimitedPumps, long TotalEvaluations, long TotalWrites,
	long TotalInputProgExecutions, long TotalFaults, string? RepresentativeError, string? SlowProg,
	long TotalSlowInputProgs = 0);
