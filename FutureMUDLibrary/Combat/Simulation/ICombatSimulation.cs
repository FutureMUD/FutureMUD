using System;
using System.Collections.Generic;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.NPC.Templates;

#nullable enable

namespace MudSharp.Combat.Simulation;

public enum CombatSimulationSourceType
{
	Character,
	NpcTemplate
}

public enum CombatSimulationRunStatus
{
	Completed,
	Stalemate,
	VirtualTimeLimit,
	EventLimit,
	WallClockLimit,
	ValidationFailed,
	Error
}

public enum CombatSimulationOutcome
{
	SurvivingWinner,
	Dead,
	Incapacitated,
	FullGrappleControl,
	Surrendered,
	Fled,
	Withdrew,
	Stalemate,
	Unknown
}

public sealed record CombatSimulationParticipantRequest(
	int Slot,
	string Team,
	CombatSimulationSourceType SourceType,
	ICharacter? Character,
	INPCTemplate? NpcTemplate,
	int Ordinal = 1,
	ICell? StartingCell = null,
	RoomLayer StartingLayer = RoomLayer.GroundLevel,
	IPositionState? StartingPosition = null,
	bool StartsInMelee = true)
{
	public string SourceDescription => SourceType switch
	{
		CombatSimulationSourceType.Character =>
			$"character #{Character?.Id:N0} ({Character?.PersonalName.GetName(NameStyle.SimpleFull) ?? "unknown"})",
		_ => $"NPC template #{NpcTemplate?.Id:N0} ({NpcTemplate?.Name ?? "unknown"})"
	};
}

public sealed record CombatSimulationRequest(
	Guid RunId,
	ICharacter RequestedBy,
	ICell Scene,
	IReadOnlyList<CombatSimulationParticipantRequest> Participants,
	int Seed,
	TimeSpan MaximumVirtualTime,
	int MaximumEvents,
	int MaximumTranscriptEntries,
	TimeSpan MaximumWallClockTime,
	bool Force,
	IReadOnlyList<ICell>? Cells = null);

public sealed record CombatSimulationBatchRequest(
	Guid BatchId,
	ICharacter RequestedBy,
	ICell Scene,
	IReadOnlyList<CombatSimulationParticipantRequest> Participants,
	int FirstSeed,
	int SeedIncrement,
	int RunCount,
	TimeSpan MaximumVirtualTime,
	int MaximumEvents,
	TimeSpan MaximumWallClockTime,
	TimeSpan MaximumBatchWallClockTime,
	bool Force,
	IReadOnlyList<ICell>? Cells = null);

public sealed record CombatSimulationValidationMessage(bool IsError, string Message);

public sealed record CombatSimulationParticipantResult(
	int Slot,
	string Team,
	string Name,
	CombatSimulationOutcome Outcome,
	CharacterState FinalState,
	double BloodRatio,
	double StaminaRatio,
	int WoundCount,
	bool UnderFullGrappleControl);

/// <summary>
/// Component digests for diagnosing which part of two otherwise-identical simulation runs first diverged.
/// </summary>
public sealed record CombatSimulationExecutionTraceCheckpoint(
	int EventCount,
	int RandomOperations,
	string RandomFingerprint,
	int SchedulerTicks,
	string SchedulerFingerprint,
	int TranscriptEntries,
	string TranscriptFingerprint);

/// <summary>
/// A bounded, tail-only record of seeded random operations. It is retained so an operator can identify the
/// first random call that differs after a checkpoint reports a replay mismatch.
/// </summary>
public sealed record CombatSimulationRandomTraceEntry(int OperationIndex, string Description);

/// <summary>
/// A bounded record of the stable runtime state captured immediately after a combatant is materialised.
/// It is retained for repeated-seed diagnostics so that a replay can distinguish materialisation drift
/// from a later combat-ordering difference.
/// </summary>
public sealed record CombatSimulationMaterialisationTraceEntry(int OperationIndex, string Description);

/// <summary>
/// A bounded diagnostic record of deterministic simulation state transitions. It is populated only for
/// repeated-seed batches so an operator can identify a branch that diverges before it consumes a different
/// random value.
/// </summary>
public sealed record CombatSimulationStateTraceEntry(int OperationIndex, string Description);

public sealed record CombatSimulationExecutionTraceSummary(
	int MaterialisationOperations,
	string MaterialisationFingerprint,
	IReadOnlyList<CombatSimulationMaterialisationTraceEntry> MaterialisationEntries,
	int RandomOperations,
	string RandomFingerprint,
	int SchedulerTicks,
	string SchedulerFingerprint,
	int TranscriptEntries,
	string TranscriptFingerprint,
	string TerminalFingerprint,
	IReadOnlyList<CombatSimulationRandomTraceEntry> RecentRandomOperations,
	IReadOnlyList<CombatSimulationStateTraceEntry> RecentStateOperations,
	IReadOnlyList<CombatSimulationExecutionTraceCheckpoint> Checkpoints,
	bool CheckpointsTruncated);

public sealed record CombatSimulationResult(
	Guid RunId,
	CombatSimulationRunStatus Status,
	string? WinningTeam,
	int Seed,
	TimeSpan VirtualDuration,
	TimeSpan WallClockDuration,
	int EventCount,
	IReadOnlyList<CombatSimulationParticipantResult> Participants,
	IReadOnlyList<CombatSimulationValidationMessage> Validation,
	IReadOnlyList<string> Transcript,
	bool TranscriptTruncated,
	string ExecutionFingerprint,
	CombatSimulationExecutionTraceSummary ExecutionTrace,
	string? ErrorMessage = null);

public sealed record CombatSimulationBatchTeamResult(
	string Team,
	int Wins,
	double WinRate);

public sealed record CombatSimulationBatchStatusResult(
	CombatSimulationRunStatus Status,
	int Count);

public sealed record CombatSimulationBatchOutcomeResult(
	CombatSimulationOutcome Outcome,
	int Count);

public sealed record CombatSimulationBatchResult(
	Guid BatchId,
	int FirstSeed,
	int SeedIncrement,
	int RequestedRunCount,
	IReadOnlyList<CombatSimulationResult> Runs,
	IReadOnlyList<CombatSimulationBatchTeamResult> Teams,
	IReadOnlyList<CombatSimulationBatchStatusResult> Statuses,
	IReadOnlyList<CombatSimulationBatchOutcomeResult> Outcomes,
	TimeSpan TotalVirtualDuration,
	TimeSpan AverageVirtualDuration,
	TimeSpan FastestVirtualDuration,
	TimeSpan SlowestVirtualDuration,
	TimeSpan TotalWallClockDuration,
	TimeSpan AverageWallClockDuration,
	TimeSpan BatchWallClockDuration,
	IReadOnlyList<CombatSimulationValidationMessage> Validation,
	string? ErrorMessage = null);

public interface ICombatSimulationService
{
	IReadOnlyList<CombatSimulationValidationMessage> Validate(CombatSimulationRequest request);
	IReadOnlyList<CombatSimulationValidationMessage> ValidateBatch(CombatSimulationBatchRequest request);
	CombatSimulationResult Run(CombatSimulationRequest request);
	CombatSimulationBatchResult RunBatch(CombatSimulationBatchRequest request);
}

/// <summary>
/// Removes a transient simulation actor from registries without placing it in the ordinary
/// offline-character cache.
/// </summary>
public interface ICombatSimulationRuntimeRegistry
{
	void ForgetCombatSimulationActor(ICharacter actor);
}

/// <summary>
/// Optional combat contract used by team-aware combat implementations to supply a replacement target.
/// </summary>
public interface ICombatTargetingPolicy
{
	IPerceiver? AcquireTargetFor(IPerceiver combatant);
}
