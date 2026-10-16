#nullable enable

using System;
using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

/// <summary>Immutable creation authority; legal title and current custody do not change it.</summary>
public sealed record SpellOwnedItemOrigin(Guid LifecycleId, SpellLifecycleMode Mode, DateTime? DeadlineUtc, long CreatorId = 0, long SpellId = 0, int Grade = 0)
{
	public bool IsTemporary => Mode == SpellLifecycleMode.TemporaryCleanup;
	public void Deconstruct(out Guid lifecycleId, out SpellLifecycleMode mode, out DateTime? deadlineUtc)
	{
		lifecycleId = LifecycleId;
		mode = Mode;
		deadlineUtc = DeadlineUtc;
	}
}

public interface ISpellOwnedItemService
{
	IGameItem Create(IGameItemProto prototype, ICharacter caster, ItemQuality quality, SpellLifecycleOrigin origin);
	SpellOwnedItemOrigin? FindOrigin(long itemId);
	bool IsActivationPending(long itemId);
	/// <summary>Checks the current persisted claim, active state and deadline without loading an item.</summary>
	bool HasActiveOrigin(IGameItem item, SpellOwnedItemOrigin expected) => false;
	/// <summary>Commits a supported component's one-use latch before it produces consequences.</summary>
	bool TryCommitComponentConsumption(IGameItemComponent component, SpellOwnedItemOrigin expected) => false;
	bool TryPrepareRemoval(IGameItem item, out string diagnostic);
	void ObserveRemoval(IGameItem item);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
