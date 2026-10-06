#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.NPC.AI;

namespace MudSharp.Magic;

/// <summary>Owns a newly created secondary instance; its real corpse, body and identity remain borrowed.</summary>
public interface ISpellOwnedCorpseAnimationService
{
	string? AdmissionError(IGameItem corpse);
	ICharacter Create(IGameItem corpse, ICharacter caster, IReadOnlyCollection<IArtificialIntelligence> ais,
		SpellLifecycleOrigin origin);
	bool OwnsInstance(long instanceId);
	/// <summary>Checks an explicit, unexpired creator command grant on an active animation.</summary>
	bool CanCommand(long instanceId, long commanderIdentityId);
	/// <summary>Returns immutable provenance for the currently valid grant. Queued orders must retain
	/// this origin and compare it with the live grant, rather than accepting a replacement grant.</summary>
	SpellLifecycleOrigin? CommandGrant(long instanceId, long commanderIdentityId) => null;
	bool TryRetire(long instanceId, SpellRetirementReason reason, out string diagnostic);
	bool IsBorrowedCorpse(long corpseId);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
