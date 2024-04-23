#nullable enable
namespace MudSharp.Economy;

public record MarketPopulationNeed
{
	public required IMarketCategory MarketCategory { get; init; }
	public required decimal BaseExpenditure { get; init; }
}