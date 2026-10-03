#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Magic;

public enum SpellLifecycleMode { Permanent, TemporaryCleanup, DeathOnExpiry }
public enum SpellLifecycleState { Active, Retiring, RemainsPending, Completed }
public enum SpellRetirementReason { Expiry, Dispel, Dismissal, CapabilityLoss, Logout, EarlyDeath, EarlyItemRemoval }
public enum SpellOwnedEntityKind { GameItem, AutonomousCharacter, CharacterInstance, Body, Cell, Exit }
public enum SpellOwnedEntityRole { CreatedEntity, GeneratedPossession }

/// <summary>An exact created row, never ownership inferred from inventory or reachability.</summary>
public sealed record SpellOwnedEntity(SpellOwnedEntityKind Kind, long Id,
	SpellOwnedEntityRole Role = SpellOwnedEntityRole.CreatedEntity);

/// <summary>Immutable creation provenance. Control and current life state are separate policies.</summary>
public sealed record SpellLifecycleOrigin(Guid Id, long SpellId, int Grade, long CreatorId,
	string Family, SpellLifecycleMode Mode, DateTime CreatedUtc, DateTime? DeadlineUtc, string Provenance = "")
{
	public void Validate()
	{
		if (Id == Guid.Empty || SpellId <= 0 || CreatorId <= 0 || Grade is < 1 or > 7 ||
		    string.IsNullOrWhiteSpace(Family) || Family.Length > 128 || Provenance is null || Provenance.Length > 2048 ||
		    !Enum.IsDefined(Mode) || CreatedUtc.Kind != DateTimeKind.Utc ||
		    DeadlineUtc is { } deadline && (deadline.Kind != DateTimeKind.Utc || deadline <= CreatedUtc) ||
		    (Mode == SpellLifecycleMode.Permanent) != (DeadlineUtc is null))
		{
			throw new ArgumentException("Invalid spell lifecycle provenance, mode or absolute UTC deadline.");
		}
	}
}

public sealed record SpellOwnedLifecycle(SpellLifecycleOrigin Origin, IReadOnlyList<SpellOwnedEntity> Entities,
	SpellLifecycleState State, SpellRetirementReason? Reason, DateTime? DeathObservedUtc,
	long? RemainsItemId, DateTime UpdatedUtc, long Version, string Diagnostic)
{
	public DateTime? RemainsRemovalRequestedUtc { get; init; }
	public DateTime? RemainsNotificationAttemptedUtc { get; init; }
	public DateTime? RemainsNotificationCompletedUtc { get; init; }
	public bool MayRemoveOwnedEntities => Origin.Mode != SpellLifecycleMode.Permanent;
	public bool RequiresNativeDeath => State == SpellLifecycleState.Retiring &&
		Origin.Mode == SpellLifecycleMode.DeathOnExpiry && DeathObservedUtc is null;
}

/// <summary>
/// Durable ownership and retirement intent. Entity adapters must prove conservation and actual native
/// death/remains state before acting; reading a pending row is not permission to replay death or deletion.
/// </summary>
public interface ISpellOwnedLifecycleStore
{
	SpellOwnedLifecycle? Find(Guid id);
	IReadOnlyList<SpellOwnedLifecycle> Pending(DateTime nowUtc, int limit = 100);
	SpellOwnedLifecycle BeginRetirement(Guid id, long expectedVersion, SpellRetirementReason reason, DateTime nowUtc);
	SpellOwnedLifecycle ObserveDeath(Guid id, long expectedVersion, long? remainsItemId, DateTime nowUtc);
	SpellOwnedLifecycle RequestRemainsRemoval(Guid id, long expectedVersion, long remainsItemId, DateTime nowUtc);
	SpellOwnedLifecycle AttemptRemainsNotification(Guid id, long expectedVersion, DateTime nowUtc);
	SpellOwnedLifecycle CompleteRemainsNotification(Guid id, long expectedVersion, DateTime nowUtc);
	SpellOwnedLifecycle Hold(Guid id, long expectedVersion, string diagnostic, DateTime nowUtc);
	SpellOwnedLifecycle Complete(Guid id, long expectedVersion, DateTime nowUtc);
}

public static class SpellLifecycleTransitions
{
	public static void ValidateUtc(DateTime nowUtc, SpellOwnedLifecycle lifecycle)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || nowUtc < lifecycle.UpdatedUtc)
		{
			throw new ArgumentException("Lifecycle transitions require nondecreasing absolute UTC time.");
		}
	}

	public static SpellLifecycleState BeginRetirement(SpellOwnedLifecycle lifecycle, SpellRetirementReason reason,
		DateTime nowUtc)
	{
		ValidateUtc(nowUtc, lifecycle);
		if (!Enum.IsDefined(reason) || reason == SpellRetirementReason.EarlyDeath)
		{
			throw new ArgumentException("Early death must be correlated through ObserveDeath after native death persists.");
		}
		if (!lifecycle.MayRemoveOwnedEntities)
		{
			throw new InvalidOperationException("Permanent creations are ordinary durable entities and cannot be expired.");
		}
		if (reason == SpellRetirementReason.EarlyItemRemoval &&
			(lifecycle.Entities.Count != 1 || lifecycle.Entities[0].Kind != SpellOwnedEntityKind.GameItem))
			throw new ArgumentException("Early item removal requires one exact generated item, never actor death correlation.");
		if (reason == SpellRetirementReason.Expiry && nowUtc < lifecycle.Origin.DeadlineUtc)
		{
			throw new InvalidOperationException("The absolute lifetime deadline has not passed.");
		}
		return lifecycle.State == SpellLifecycleState.Active ? SpellLifecycleState.Retiring : lifecycle.State;
	}
}
