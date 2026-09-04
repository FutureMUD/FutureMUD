using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureWoodlandDefinition : IFrameworkItem, IHaveFuturemud, IProgVariable
{
	string Description { get; }
	string WoodlandType { get; }
	int EstablishmentDays { get; }
	int HarvestCycleDays { get; }
	IReadOnlyCollection<AgricultureCommodityYield> YieldOutputs { get; }
}
