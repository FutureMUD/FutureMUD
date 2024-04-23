#nullable enable
using MudSharp.FutureProg;

namespace MudSharp.Economy;

public record MarketStressPoint
{
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required decimal StressThreshold { get; init; }
	public required IFutureProg? ExecuteOnStart { get; init; }
	public required IFutureProg? ExecuteOnEnd { get; init; }
}