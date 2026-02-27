#nullable enable

namespace MudSharp.Arenas;

/// <summary>
/// Represents the lifecycle states an arena event can progress through.
/// </summary>
public enum ArenaEventState {
	Draft,
	Scheduled,
	RegistrationOpen,
	Preparing,
	Staged,
	Live,
	Resolving,
	Cleanup,
	Completed,
	Aborted
}

/// <summary>
/// Betting model options available to arena event types.
/// </summary>
public enum BettingModel {
	FixedOdds,
	PariMutuel
}

/// <summary>
/// Defines which combat state ends a bout.
/// </summary>
public enum ArenaEliminationMode {
	NoElimination,
	PointsElimination,
	KnockDown,
	Knockout,
	Death
}

/// <summary>
/// Reasons why a participant might be eliminated from an event.
/// </summary>
public enum EliminationReason {
	Knockout,
	Death,
	Surrender,
	Time,
	Disqualification,
	KnockDown
}

/// <summary>
/// High level outcome states for a completed event.
/// </summary>
public enum ArenaOutcome {
	Win,
	Draw,
	Aborted
}

/// <summary>
/// Governs how a side accepts registrations.
/// </summary>
public enum ArenaSidePolicy {
	Open,
	ManagersOnly,
	ReservedOnly,
	Closed
}

/// <summary>
/// Fee categories supported by arenas.
/// </summary>
public enum ArenaFeeType {
	Appearance,
	Victory
}

/// <summary>
/// Distinguishes payout liabilities by source.
/// </summary>
public enum ArenaPayoutType {
	Bet = 0,
	Appearance = 1
}

/// <summary>
/// Selects which Elo variant to use when settling arena ratings.
/// </summary>
public enum ArenaEloStyle {
	TeamAverage = 0,
	PairwiseIndividual = 1,
	PairwiseSide = 2
}
