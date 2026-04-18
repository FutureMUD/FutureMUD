#nullable enable
namespace MudSharp.Economy;

public record MarketImpact
{
    public required IMarketCategory MarketCategory { get; init; }
    public required double SupplyImpact { get; init; }
    public required double DemandImpact { get; init; }
    public required double FlatPriceImpact { get; init; }
}
