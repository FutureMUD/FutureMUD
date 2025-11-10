#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Arenas;

/// <summary>
/// Provides scoring callbacks for arena combat events.
/// </summary>
public interface IArenaScoringStrategy {
	void OnDamage(IArenaEvent arenaEvent, ICharacter source, ICharacter target, double amount);
	void OnKill(IArenaEvent arenaEvent, ICharacter source, ICharacter target);
	void OnSurrender(IArenaEvent arenaEvent, ICharacter source);
	IReadOnlyDictionary<int, double> GetScores(IArenaEvent arenaEvent);
}

/// <summary>
/// Determines elimination and mercy stop conditions.
/// </summary>
public interface IArenaEliminationStrategy {
	bool IsEliminated(IArenaEvent arenaEvent, ICharacter participant);
	bool MercyStopAllowed(IArenaEvent arenaEvent);
}
