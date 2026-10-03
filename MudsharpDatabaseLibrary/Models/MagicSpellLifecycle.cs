#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

/// <summary>Lightweight durable provenance; entity IDs deliberately have no destructive entity foreign keys.</summary>
public class MagicSpellLifecycle
{
	public Guid Id { get; set; }
	public long SpellId { get; set; }
	public int Grade { get; set; }
	public long CreatorId { get; set; }
	public string Family { get; set; } = "";
	public int Mode { get; set; }
	public DateTime CreatedUtc { get; set; }
	public DateTime? DeadlineUtc { get; set; }
	public string Provenance { get; set; } = "";
	public int State { get; set; }
	public int? Reason { get; set; }
	public DateTime? DeathObservedUtc { get; set; }
	public long? RemainsItemId { get; set; }
	public DateTime? RemainsRemovalRequestedUtc { get; set; }
	public DateTime? RemainsNotificationAttemptedUtc { get; set; }
	public DateTime? RemainsNotificationCompletedUtc { get; set; }
	public DateTime UpdatedUtc { get; set; }
	public long Version { get; set; }
	public string Diagnostic { get; set; } = "";
	public virtual ICollection<MagicSpellOwnedEntity> Entities { get; set; } = new List<MagicSpellOwnedEntity>();
}

public class MagicSpellOwnedEntity
{
	public int Kind { get; set; }
	public long EntityId { get; set; }
	public int Role { get; set; }
	public Guid LifecycleId { get; set; }
	public virtual MagicSpellLifecycle Lifecycle { get; set; } = null!;
}
