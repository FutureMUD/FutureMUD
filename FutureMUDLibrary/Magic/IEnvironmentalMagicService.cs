#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Environment;

public enum EnvironmentalMagicBindingMode
{
	Inherit,
	Explicit,
	Disabled
}

[Flags]
public enum EnvironmentalMagicDirtyReason
{
	None = 0,
	Balance = 1,
	Damage = 2,
	Forage = 4,
	Agriculture = 8,
	Binding = 16,
	Definition = 32,
	Policy = 64
}

public enum EnvironmentalResourceMutation
{
	Add,
	Set,
	Debit
}

/// <summary>Persistent ecological state. Resource balances remain on IHaveMagicResource.</summary>
public sealed record EnvironmentalMagicState
{
	public static EnvironmentalMagicState Empty { get; } = new();
	public int SchemaVersion { get; init; } = 1;
	public long Revision { get; init; }
	public double ScarDamage { get; init; }
	public DateTimeOffset? LastDefileUtc { get; init; }
	public double RecentPressure { get; init; }
	public DateTimeOffset? PressureReferenceUtc { get; init; }
	public double PressureHalfLifeSeconds { get; init; } = 3600.0;
	public long? PressureProfileId { get; init; }
	public double PressureDecayAnchor { get; init; }
}

public sealed record EnvironmentalResourceSnapshot(long ResourceId, string Name, double Balance,
	bool IsValid, double Maximum, double Rate, string? Error);

/// <summary>One exact recorded ambient payment in a bounded Land source group.</summary>
public sealed record EnvironmentalLandAmbientDebit(IMagicResource Resource, double Amount);

public sealed record EnvironmentalMagicStateSnapshot(EnvironmentalMagicState State, double Pressure);

/// <summary>A pure Land authorisation view that does not evaluate unrelated mana output formulas.</summary>
public sealed record EnvironmentalOrganicProfileSnapshot(long? ProfileId, long Revision,
	bool HasOrganicConfiguration, bool HasPendingOperation, IReadOnlyList<string> Errors);

public sealed record EnvironmentalMagicSnapshot(long RoomId, EnvironmentalMagicBindingMode BindingMode,
	long? ProfileId, string? ProfileName, EnvironmentalMagicState State, double Pressure,
	IReadOnlyDictionary<string, double> Inputs, IReadOnlyList<EnvironmentalResourceSnapshot> Outputs,
	IReadOnlyList<string> Errors)
{
	public bool IsValid => ProfileId.HasValue && Errors.Count == 0;
}

public sealed record EnvironmentalMagicOperationRequest(Guid OperationId, long? ActorId, string Attribution,
	double Damage = 0.0, double Pressure = 0.0, double Repair = 0.0);

public sealed record EnvironmentalMagicOperationResult(Guid OperationId, bool Success, bool Replayed,
	double AppliedDamage, double AppliedPressure, double AppliedRepair, string? Error);

/// <summary>
/// World-local simulation-thread service. Inspection never advances production or changes scheduling.
/// Managed mutations validate fresh inputs; Debit requires the full amount and never partially consumes stock.
/// </summary>
public interface IEnvironmentalMagicService : IDisposable
{
	DateTimeOffset UtcNow { get; }
	EnvironmentalMagicStateSnapshot InspectState(IRoom room);
	LandRejuvenationPolicy InspectRepairPolicy(IRoom room);
	IReadOnlyList<LandRejuvenationProgress> InspectTreatments(IRoom room);
	LandRejuvenationProgress? InspectTreatment(IRoom room, Guid treatmentId);
	bool CanInstallTreatment(IRoom room, out string? error);
	bool ActivateTreatment(ILandRejuvenationEffect effect, LandRejuvenationProgress? initial, out string? error);
	void RegisterLoadedTreatment(ILandRejuvenationEffect effect);
	/// <summary>Persists earned online work and lifetime without applying repair, for parent serialization.</summary>
	void CheckpointTreatment(IRoom room, Guid treatmentId);
	void CancelTreatment(IRoom room, Guid treatmentId, string reason);
	void ExpireTreatment(IRoom room, Guid treatmentId);
	bool ConfirmTreatment(IRoom room, Guid treatmentId, out string? error);
	bool EvaluateRepairPolicy(IRoom room, MudSharp.Character.ICharacter caster, MudSharp.FutureProg.IFutureProg prog, out string? error);
	void ScarStateChanged(IRoom room);
	EnvironmentalOrganicProfileSnapshot InspectOrganicProfile(IRoom room);
	EnvironmentalMagicSnapshot Inspect(IRoom room);
	/// <summary>
	/// Returns whether the pair is managed. A managed invalid result has IsValid false; it is not an
	/// unbound fallback or a zero cap. Available recorded stock is min(Balance, Maximum) when valid.
	/// </summary>
	bool TryInspectResource(IRoom room, IMagicResource resource, out EnvironmentalResourceSnapshot result);
	/// <summary>Purely evaluates one Land-funded output without evaluating unrelated output formulas.</summary>
	bool TryInspectLandResource(IRoom room, IMagicResource resource, out EnvironmentalResourceSnapshot result);
	/// <summary>
	/// Resolves a managed output from one already-taken pure inspection snapshot. This avoids re-evaluating
	/// profile formulae simply to identify a displayed or quoted output; callers must still reject an invalid
	/// snapshot/output and take a fresh snapshot for a later commitment.
	/// </summary>
	bool TryInspectResource(EnvironmentalMagicSnapshot snapshot, IMagicResource resource,
		out EnvironmentalResourceSnapshot result)
	{
		foreach (EnvironmentalResourceSnapshot output in snapshot.Outputs)
		{
			if (output.ResourceId == resource.Id)
			{
				result = output;
				return snapshot.ProfileId.HasValue;
			}
		}

		result = null!;
		return false;
	}
	/// <summary>
	/// Returns whether this service owns the pair; success separately reports whether the requested
	/// mutation succeeded. Validates one current input snapshot and accepts the previous online segment.
	/// A failed exact debit never partially consumes stock; valid downward cap reconciliation can still occur.
	/// </summary>
	bool TryMutateResource(IRoom room, IMagicResource resource, EnvironmentalResourceMutation mutation,
		double amount, out bool success);
	/// <summary>Debits only the complete requested recorded amount after current validation; never spends unrecorded accrual.</summary>
	bool TryDebit(IRoom room, IMagicResource resource, double amount, out string? error);
	/// <summary>Purely resolves all declarations on the cell's effective environmental profile.</summary>
	IReadOnlyList<NativeOrganicSourceSnapshot> InspectOrganicSources(IRoom room);
	/// <summary>Purely resolves one canonical selector, including explicit unauthorised/error states.</summary>
	NativeOrganicSourceSnapshot InspectOrganicSource(IRoom room, string selector);
	/// <summary>Creates a short-lived exact native-owner plan without reserving or mutating stock.</summary>
	bool TryPlanOrganicDebit(IRoom room, string selector, double amount, out NativeOrganicDebitPlan plan,
		out string? error);
	/// <summary>Revalidates and applies one owner mutation. It grants no magic resource and creates no receipt.</summary>
	bool TryApplyOrganicDebit(IRoom room, NativeOrganicDebitPlan plan, out NativeOrganicSourceSnapshot result,
		out string? error);
	/// <summary>Validates a bounded group against one precommit state, then applies its own sequential owner changes.</summary>
	bool TryApplyOrganicDebitBatch(IRoom room, IReadOnlyList<NativeOrganicDebitPlan> plans,
		out IReadOnlyList<NativeOrganicDebitPlan> applied, out string? error);
	/// <summary>Validates all Land funding against the current environment before paying ambient then native owners.</summary>
	bool TryApplyLandDebitGroup(IRoom room, IReadOnlyList<EnvironmentalLandAmbientDebit> ambient,
		IReadOnlyList<NativeOrganicDebitPlan> native, out IReadOnlyList<long> appliedAmbient,
		out IReadOnlyList<NativeOrganicDebitPlan> appliedNative, out string? error);
	/// <summary>Evaluates only the requested native suppression channel and its declared dependencies.</summary>
	NativeOrganicPenaltyEvaluation EvaluateOrganicPenalty(IRoom room, NativeOrganicPenaltyChannel channel,
		NativeOrganicPenaltyContext context);
	/// <summary>Clears malformed field accounting only; never adds stock, removes scars or grants mana.</summary>
	bool RepairNativeOrganicAccounting(IRoom room, NativeOrganicSourceKind? kind, out string result);
	/// <summary>Atomically persists quantified ecological changes and a caller-supplied unique receipt; retries reuse that exact request.</summary>
	EnvironmentalMagicOperationResult ApplyOperation(IRoom room, EnvironmentalMagicOperationRequest request);
	void SetBinding(IRoom room, EnvironmentalMagicBindingMode mode, long? profileId);
	void Register(IRoom room);
	void Unregister(IRoom room);
	void RoomTerrainChanged(IRoom room);
	void TerrainDefaultChanged(ITerrain terrain);
	/// <summary>Coalesces a source change for bounded later evaluation; never grants production or adds a per-cell callback.</summary>
	void MarkDirty(IRoom room, EnvironmentalMagicDirtyReason reason);
	void BeforeProfileChange(IEnvironmentalMagicProfile profile);
	void ProfileChanged(IEnvironmentalMagicProfile profile);
	void SourceDefinitionChanged();
	void FieldChanged(IAgricultureField field, bool removed = false);
	IAgricultureField? FieldFor(IRoom room);
	/// <summary>Indexed apiary candidates for native pollination; null only for services without this index.</summary>
	IEnumerable<IAgricultureField>? PollinationCandidates();
	void RefreshPollinationCandidate(IAgricultureField field);
	void Initialise();
	void Pump();
	string DescribeDiagnostics();
}
