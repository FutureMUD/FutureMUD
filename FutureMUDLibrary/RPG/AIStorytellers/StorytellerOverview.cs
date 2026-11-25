#nullable enable

namespace MudSharp.RPG.AIStorytellers;

/// <summary>Current game-loop workload and cumulative provider counters since startup.</summary>
public sealed record StorytellerOverview(int QueuedBatches, string? ActiveRole, long Requests,
	long InputTokens, long OutputTokens, long Failures);
