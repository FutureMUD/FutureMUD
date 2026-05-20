using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public interface IAgricultureWoodlandDefinition : IFrameworkItem, IHaveFuturemud
{
	string Description { get; }
	string WoodlandType { get; }
	int EstablishmentDays { get; }
	int HarvestCycleDays { get; }
}
