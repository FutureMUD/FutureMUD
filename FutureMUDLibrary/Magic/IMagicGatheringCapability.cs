#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>
/// The only gathering source kinds currently implemented by the engine. Land gathering is deliberately
/// not represented as an executable kind until its separate gameplay contract exists.
/// </summary>
public enum MagicGatheringMethodKind
{
	Self,
	Gentle
}

/// <summary>
/// Capability-owned, durable configuration for one explicitly chosen gathering route. The key is identity;
/// alias, name and order are presentation only.
/// </summary>
public sealed record MagicGatheringMethodDefinition(
	Guid Key,
	string Alias,
	string Name,
	MagicGatheringMethodKind Kind,
	long DestinationResourceId,
	long? SourceResourceId,
	double MinimumAmount,
	double MaximumAmount,
	double DurationSeconds,
	double SourceUnitsPerDestinationUnit = 1.0,
	double StaminaCost = 0.0,
	double MinimumStamina = 0.0,
	double DamageCost = 0.0,
	double PainCost = 0.0,
	double StunCost = 0.0,
	WoundSeverity MaximumHealthSeverity = WoundSeverity.None,
	long PermissionProgId = 0,
	long DurationProgId = 0,
	long StaminaCostProgId = 0,
	long DamageCostProgId = 0,
	long PainCostProgId = 0,
	long StunCostProgId = 0,
	long OnGatheredProgId = 0,
	int StructuralVersion = 1);

/// <summary>
/// Optional capability extension. A capability with no methods grants no gathering behaviour.
/// </summary>
public interface IMagicGatheringCapability : IMagicCapability
{
	IReadOnlyList<MagicGatheringMethodDefinition> GatheringMethods { get; }
	IReadOnlyList<string> GatheringConfigurationErrors();
}

public sealed record MagicGatheringQuote(
	Guid MethodKey,
	int MethodVersion,
	MagicGatheringMethodKind Kind,
	long DestinationResourceId,
	long? SourceResourceId,
	long? SourceCellId,
	long? SourceProfileId,
	long? SourceProfileRevision,
	double RequestedAmount,
	double SourceDebit,
	double DurationSeconds,
	double StaminaCost,
	double MinimumStamina,
	double DamageCost,
	double PainCost,
	double StunCost,
	long? HealthTargetBodypartId = null,
	bool HealthCostUsesExistingWound = false);

public sealed record MagicGatheringMethodView(
	Guid Key,
	string Alias,
	string Name,
	MagicGatheringMethodKind Kind,
	double MinimumAmount,
	double MaximumAmount,
	string? UnavailableReason = null);

public sealed record MagicGatheringResult(
	bool Success,
	string Message,
	Guid? OperationId = null,
	MagicGatheringQuote? Quote = null);

public sealed record MagicGatheringOperationSummary(
	Guid Id,
	long OwnerId,
	long ActorId,
	long BodyId,
	long CapabilityId,
	Guid MethodKey,
	int MethodVersion,
	string Kind,
	string Status,
	double RequestedAmount,
	double SourceDebit,
	double StaminaCost,
	double DamageCost,
	double PainCost,
	double StunCost,
	bool SourceDebited,
	bool BodilyCostApplied,
	bool DestinationCredited,
	bool AccountingPersisted,
	bool NotificationCompleted,
	DateTime CreatedUtc,
	DateTime UpdatedUtc,
	string Diagnostic);

/// <summary>
/// World-local gathering gateway. Commands and FutureProg adapters use this same guarded service so that
/// scripts cannot bypass quote, elapsed-time, receipt or exact-debit rules.
/// </summary>
public interface IMagicGatheringService
{
	IReadOnlyList<MagicGatheringMethodView> Methods(ICharacter actor, IMagicGatheringCapability capability);
	MagicGatheringResult Preview(ICharacter actor, IMagicGatheringCapability capability, string method, double amount);
	MagicGatheringResult Begin(ICharacter actor, IMagicGatheringCapability capability, string method, double amount);
	MagicGatheringResult Complete(ICharacter actor, Guid operationId);
	MagicGatheringResult Cancel(ICharacter actor, Guid? operationId = null);
	MagicGatheringOperationSummary? Operation(Guid operationId);
	IReadOnlyList<MagicGatheringOperationSummary> UnresolvedOperations(long? ownerId = null);
	MagicGatheringResult Acknowledge(Guid operationId);
}
