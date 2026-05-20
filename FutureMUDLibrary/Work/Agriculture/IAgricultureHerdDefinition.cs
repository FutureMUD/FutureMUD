using MudSharp.Framework;
using MudSharp.NPC.Templates;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureHerdDefinition : IFrameworkItem, IHaveFuturemud
{
	string Description { get; }
	double AnimalUnits { get; }
	double DailyGraze { get; }
	int MaximumCondition { get; }
	INPCTemplate NpcTemplate { get; }
	bool CanMaterialise { get; }
}
