#nullable enable

namespace MudSharp.Economy;

public record MarketCategoryComponent
{
	public required IMarketCategory MarketCategory { get; init; }
	public required decimal Weight { get; init; }
}
