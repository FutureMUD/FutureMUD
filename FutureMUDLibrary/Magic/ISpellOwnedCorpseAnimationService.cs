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
	bool TryRetire(long instanceId, SpellRetirementReason reason, out string diagnostic);
	bool IsBorrowedCorpse(long corpseId);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
