using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureCropDefinition : IFrameworkItem, IHaveFuturemud
{
	string Description { get; }
	string Category { get; }
	int BaseGrowthDays { get; }
	int HarvestWindowDays { get; }
	int MinimumMoisture { get; }
	int MaximumMoisture { get; }
	int MinimumTemperature { get; }
	int MaximumTemperature { get; }
}
