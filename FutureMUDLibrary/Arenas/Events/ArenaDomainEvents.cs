#nullable enable

using System.Collections.Generic;

namespace MudSharp.Arenas.Events;

public record ArenaEventStateChanged(long EventId, ArenaEventState OldState, ArenaEventState NewState);

public record ArenaScoreEvent(long EventId, long SourceId, long? TargetId, string Action, double Value);

public record ArenaPayoutEvent(long EventId, ArenaOutcome Outcome, IReadOnlyCollection<int> WinningSides);

public record ArenaParticipantEliminated(long EventId, long CharacterId, EliminationReason Reason);
