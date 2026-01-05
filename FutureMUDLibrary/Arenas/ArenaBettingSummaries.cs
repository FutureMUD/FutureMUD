#nullable enable

using System;

namespace MudSharp.Arenas;

public sealed record ArenaBetSummary(
	long BetId,
	long ArenaEventId,
	long ArenaId,
	string ArenaName,
	string EventTypeName,
	DateTime ScheduledAt,
	int? SideIndex,
	decimal Stake,
	decimal? FixedDecimalOdds,
	BettingModel BettingModel,
	DateTime PlacedAt,
	bool IsCancelled,
	DateTime? CancelledAt,
	ArenaEventState EventState,
	decimal CollectedPayout,
	decimal OutstandingPayout,
	decimal BlockedPayout);

public sealed record ArenaBetPayoutSummary(
	long ArenaEventId,
	long ArenaId,
	string ArenaName,
	string EventTypeName,
	decimal Amount,
	bool IsBlocked,
	DateTime CreatedAt);

public sealed record ArenaBetCollectionSummary(
	int CollectedCount,
	int FailedCount,
	int BlockedCount,
	decimal CollectedAmount,
	decimal FailedAmount,
	decimal BlockedAmount);
