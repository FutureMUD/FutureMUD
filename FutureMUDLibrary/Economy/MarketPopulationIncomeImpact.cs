#nullable enable
namespace MudSharp.Economy;

public record MarketPopulationIncomeImpact
{
    public required IMarketPopulation MarketPopulation { get; init; }
    public required decimal AdditiveIncomeImpact { get; init; }
    public required decimal MultiplicativeIncomeImpact { get; init; }
}
