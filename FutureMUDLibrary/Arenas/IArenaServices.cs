#nullable enable

using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.Arenas;

/// <summary>
/// Handles scheduling and recovery of arena events.
/// </summary>
public interface IArenaScheduler {
	void Schedule(IArenaEvent arenaEvent);
	void Cancel(IArenaEvent arenaEvent);
	void RecoverAfterReboot();
}

/// <summary>
/// Provides state transition orchestration and crash safety.
/// </summary>
public interface IArenaLifecycleService {
	void Transition(IArenaEvent arenaEvent, ArenaEventState targetState);
	void RebootRecovery();
}

/// <summary>
/// Manages remote observation mirrors for arena fights.
/// </summary>
public interface IArenaObservationService {
	(bool Truth, string Reason) CanObserve(ICharacter observer, IArenaEvent arenaEvent);
	void StartObserving(ICharacter observer, IArenaEvent arenaEvent, ICell observationCell);
	void StopObserving(ICharacter observer, IArenaEvent arenaEvent);
}

/// <summary>
/// Handles betting lifecycle including settlement and refunds.
/// </summary>
public interface IArenaBettingService {
	(bool Truth, string Reason) CanBet(ICharacter actor, IArenaEvent arenaEvent);
	void PlaceBet(ICharacter actor, IArenaEvent arenaEvent, int? sideIndex, decimal stake);
	void CancelBet(ICharacter actor, IArenaEvent arenaEvent);
	void Settle(IArenaEvent arenaEvent, ArenaOutcome outcome, IEnumerable<int> winningSides);
	void RefundAll(IArenaEvent arenaEvent, string reason);
	(decimal? FixedOdds, (decimal Pool, decimal TakeRate)? PariMutuel) GetQuote(IArenaEvent arenaEvent, int? sideIndex);
}

/// <summary>
/// Updates and queries combat arena ratings.
/// </summary>
public interface IArenaRatingsService {
	decimal GetRating(ICharacter character, ICombatantClass combatantClass);
	void UpdateRatings(IArenaEvent arenaEvent, IReadOnlyDictionary<ICharacter, decimal> deltas);
	void ApplyDefaultElo(IArenaEvent arenaEvent);
}

/// <summary>
/// Arena financial guardrails (solvency, tax, deferred payouts).
/// </summary>
public interface IArenaFinanceService {
	(bool Truth, string Reason) IsSolvent(ICombatArena arena, decimal required);
	void WithholdTax(ICombatArena arena, decimal amount, string reference);
	void PostProfitLoss(ICombatArena arena, string reference);
	void BlockPayout(ICombatArena arena, IArenaEvent arenaEvent, IEnumerable<(ICharacter Winner, decimal Amount)> payouts);
	void UnblockPayouts(ICombatArena arena);
}

/// <summary>
/// Provides NPC auto-fill, outfitting, and restoration flows.
/// </summary>
public interface IArenaNpcService {
        IEnumerable<ICharacter> AutoFill(IArenaEvent arenaEvent, int sideIndex, int slotsNeeded);
        void PrepareNpc(ICharacter npc, IArenaEvent arenaEvent, int sideIndex, ICombatantClass combatantClass);
        void ReturnNpc(ICharacter npc, IArenaEvent arenaEvent, bool resurrect);
}

/// <summary>
/// Applies participation locks that keep players in the arena until the event concludes.
/// </summary>
public interface IArenaParticipationService {
        void EnsureParticipation(ICharacter participant, IArenaEvent arenaEvent);
        void EnsureParticipation(IArenaEvent arenaEvent);
        bool HasParticipation(ICharacter participant, IArenaEvent arenaEvent);
        void ClearParticipation(ICharacter participant, IArenaEvent arenaEvent);
        void ClearParticipation(IArenaEvent arenaEvent);
}

/// <summary>
/// Centralises arena-related output helpers for commands and builders.
/// </summary>
public interface IArenaCommandService {
        void ShowArena(ICharacter actor, ICombatArena arena);
	void ShowEvent(ICharacter actor, IArenaEvent arenaEvent);
	void ShowEventType(ICharacter actor, IArenaEventType eventType);
}
