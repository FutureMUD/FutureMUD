#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
namespace MudSharp.Magic;

public static class ArmageddonHoveringLightStock
{
	public const string Key=ArmageddonReviewedUtilityContent.HoveringLightKey;
	public const string Name=ArmageddonReviewedUtilityContent.HoveringLightName;
	public const string LifetimeSeconds=ArmageddonReviewedUtilityContent.HoveringLightLifetimeSeconds;
	public const double MinimumEnergy=ArmageddonReviewedUtilityContent.HoveringLightMinimumEnergy;
	public static MagicSpell Create(IFuturemud world,IMagicSchool school,ITraitDefinition trait,IMagicResource resource,IGameItemProto light)
	{
		if(world.SpellOwnedItems is null || NativeItemCreationEligibility.Error(light,world) is {} ||
			light.GetItemType<WearableGameItemComponentProto>() is null || light.GetItemType<ProgLightGameItemComponentProto>() is null)
			throw new InvalidOperationException("Select an approved plain native wearable prog-light prototype and enable spell-owned item lifecycle services.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedUtilityContent.HoveringLight(light.Id));
	}
	internal static XElement Definition(long resource,long cost,long filter,long prototype)=> ArmageddonReviewedUtilityContent.HoveringLight(prototype).BuildDefinition(resource, cost, filter);
}
