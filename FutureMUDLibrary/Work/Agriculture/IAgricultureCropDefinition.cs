using System.Collections.Generic;
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
	bool IsPerennial { get; }
	int HarvestCycleDays { get; }
	IReadOnlyCollection<AgricultureScoreRange> ScoreRanges { get; }
	IReadOnlyCollection<AgricultureCommodityYield> YieldOutputs { get; }
	IReadOnlyCollection<AgricultureCommodityYield> SeedRequirements { get; }
}
