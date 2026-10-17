#nullable enable

using System;

namespace MudSharp.Magic;

/// <summary>A pulse journal owns neither its existing room nor its existing occupants.</summary>
public interface ISpellRoomHazardJournal
{
	SpellOwnedLifecycle Create(SpellLifecycleOrigin origin);
	SpellOwnedLifecycle? Find(Guid id);
	SpellOwnedLifecycle ClaimPulse(Guid id, long expectedVersion, DateTime nowUtc);
	void Retire(Guid id, SpellRetirementReason reason, DateTime nowUtc);
	void Hold(Guid id, DateTime nowUtc, string diagnostic);
	int Reconcile(DateTime nowUtc, int limit = 100);
}
