#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.NPC.Templates;

namespace MudSharp.Magic;

/// <summary>Native NPC creation and persisted death correlation. This does not authorize destruction.</summary>
public interface ISpellOwnedNpcService
{
	ICharacter Create(INPCTemplate template, SpatialLocation location, SpellLifecycleOrigin origin);
	void ObserveNativeDeath(ICharacter character, IGameItem? remains);
	int ReconcilePersistedDeaths(DateTime nowUtc, int limit = 100);
	bool SuppressNativeRemains(ICharacter character);
	/// <summary>Read exact NPC/body claims and durable retirement intent without loading other actors.</summary>
	bool HasPendingRetirement(ICharacter character, long creatorId);
	bool TryPrepareRemainsRemoval(IGameItem remains, out string diagnostic, bool morphing = false);
	bool TryNotifyRemainsDeletion(IGameItem remains, Action notify);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
