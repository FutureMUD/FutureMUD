using MudSharp.Framework;
using MudSharp.NPC.Templates;
using MudSharp.FutureProg;
using System.Collections.Generic;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureHerdDefinition : IFrameworkItem, IHaveFuturemud, IProgVariable
{
	string Description { get; }
	double AnimalUnits { get; }
	double DailyGraze { get; }
	int MaximumCondition { get; }
	IReadOnlyCollection<AgricultureCommodityYield> SecondaryOutputs { get; }
	INPCTemplate NpcTemplate { get; }
	bool CanMaterialise { get; }
}
