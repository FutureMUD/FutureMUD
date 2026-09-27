#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction;

namespace MudSharp.Magic.Environment;

public enum LandRejuvenationStatus
{
	Active,
	Pending,
	Completed,
	Cancelled,
	Faulted
}

/// <summary>Authoritative persisted checkpoint. No process-local clock values are stored.</summary>
public sealed record LandRejuvenationProgress
{
	public int Version { get; init; } = 1;
	public Guid Id { get; init; }
	public Guid ParentId { get; init; }
	public long CellId { get; init; }
	public long SpellId { get; init; }
	public long CasterId { get; init; }
	public long ActingInstanceId { get; init; }
	public long[] PlaneIds { get; init; } = [];
	public long ProfileId { get; init; }
	public int Layer { get; init; }
	public bool RequiresPresence { get; init; }
	public long? ContinuationProgId { get; init; }
	public double Rate { get; init; }
	public double InitialBudget { get; init; }
	public double RemainingBudget { get; init; }
	public double TotalRepaired { get; init; }
	public double RemainingSeconds { get; init; }
	public double EarnedWork { get; init; }
	public long Revision { get; init; }
	public long Sequence { get; init; }
	public long AcknowledgedSequence { get; init; }
	public LandRejuvenationStatus Status { get; init; }
	public bool CancellationRequested { get; init; }
	public EnvironmentalMagicOperationRequest? PendingRequest { get; init; }
	public Guid? LastOperationId { get; init; }
	public string Diagnostic { get; init; } = string.Empty;
	public bool IsTerminal => Status is LandRejuvenationStatus.Completed or LandRejuvenationStatus.Cancelled or LandRejuvenationStatus.Faulted;
}

public sealed record LandRejuvenationPolicy(long? ProfileId, double? Ceiling, string? Error)
{
	public bool IsValid => ProfileId.HasValue && Error is null && Ceiling != 0.0;
}

/// <summary>Lifecycle callback owned by an attached spell child; queries never advance repair.</summary>
public interface ILandRejuvenationEffect
{
	Guid TreatmentId { get; }
	Guid ParentIdentity { get; }
	ICell TreatmentCell { get; }
	bool IsAttached { get; }
	bool CheckMaintenance(out string? error);
	void ActivateTreatment();
	void CheckpointTreatment();
	void ExpireTreatment();
	void TreatmentEnded(string diagnostic);
}
