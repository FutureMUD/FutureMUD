#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
namespace MudSharp.Magic;

public static class ArmageddonSustainMealStock
{
	public const string Key = ArmageddonReviewedProvisionContent.SustainMealKey;
	public const string Name = ArmageddonReviewedProvisionContent.SustainMealName;
	public const string LifetimeSeconds = ArmageddonReviewedProvisionContent.SustainMealLifetimeSeconds;
	public const double MinimumEnergy = ArmageddonReviewedProvisionContent.MinimumEnergy;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<IGameItemProto> defaultPool)
	{
		if (world.SpellOwnedItems is null || defaultPool.Count is < 1 or > 32 || defaultPool.Select(x => x.Id).Distinct().Count() != defaultPool.Count ||
			defaultPool.Any(food => NativeItemCreationEligibility.Error(food, world) is not null || food.GetItemType<FoodGameItemComponentProto>() is null))
			throw new InvalidOperationException("Select an approved plain native food prototype and enable spell-owned item lifecycle services.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedProvisionContent.SustainMeal(defaultPool.Select(x => x.Id).ToArray()));
	}
	internal static XElement Definition(long resource, long cost, params long[] prototypes) =>
		ArmageddonReviewedProvisionContent.SustainMeal(prototypes).BuildDefinition(resource, cost, 0);
}
