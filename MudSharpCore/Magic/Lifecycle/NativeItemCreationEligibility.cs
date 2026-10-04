#nullable enable

using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.Lifecycle;

internal static class NativeItemCreationEligibility
{
	internal static string? Error(IGameItemProto? prototype, IFuturemud world)
	{
		if (prototype is not GameItemProto native || !ReferenceEquals(prototype.Gameworld, world) ||
			prototype.Status != RevisionStatus.Current || prototype.PreventManualLoad)
			return "Lifecycle item creation requires an approved loadable native prototype in this world.";
		if (native.OnLoadProgs.Any() || prototype.Morphs)
			return "Lifecycle items require an unscripted prototype without ordinary morphing.";
		if (world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("GameItem")))
			return "Lifecycle items require an adapter for configured GameItem default hooks before payment.";
		var types = prototype.Components.Select(x => x.GetType()).ToArray();
		var weaponGraph = types.Length > 0 && types.All(x => x == typeof(HoldableGameItemComponentProto) ||
			x == typeof(MeleeWeaponGameItemComponentProto) || x == typeof(SalvageableGameItemComponentProto));
		var foodGraph = types.Length == 2 && types.Contains(typeof(HoldableGameItemComponentProto)) && types.Contains(typeof(FoodGameItemComponentProto));
		var lightGraph = types.Length == 3 && types.Contains(typeof(HoldableGameItemComponentProto)) &&
			types.Contains(typeof(WearableGameItemComponentProto)) && types.Contains(typeof(ProgLightGameItemComponentProto));
		if (!prototype.IsItemType<HoldableGameItemComponentProto>() || !(weaponGraph || foodGraph || lightGraph))
			return "Lifecycle creation supports plain holdable weapons, food or wearable lights; other component graphs need an adapter.";
		if (prototype.GetItemType<FoodGameItemComponentProto>() is { } food &&
			(food.OnEatProg is not null || !double.IsFinite(food.Bites) || food.Bites <= 0 || food.Decorator is null ||
			 new[] { food.SatiationPoints, food.ThirstPoints, food.WaterLitres, food.AlcoholLitres }.Any(x => !double.IsFinite(x) || x < 0)))
			return "Lifecycle food requires finite positive bites, nonnegative nutrition, a decorator and no eating script.";
		if (prototype.GetItemType<WearableGameItemComponentProto>() is { } wearable &&
			(wearable.DefaultProfile is null || wearable.Profiles.Any(x => x is not MudSharp.GameItems.Inventory.WearProfile) || wearable.WearableProg is not null || wearable.WhyCannotWearProg is not null))
			return "Lifecycle lights require a configured native wear profile without wear scripts.";
		if (prototype.GetItemType<ProgLightGameItemComponentProto>() is { } light &&
			(!double.IsFinite(light.IlluminationProvided) || light.IlluminationProvided <= 0))
			return "Lifecycle lights require finite positive illumination.";
		if (prototype.GetItemType<MeleeWeaponGameItemComponentProto>() is { } weapon &&
			(weapon.WeaponType is null || weapon.CanWieldProg is not null || weapon.WhyCannotWieldProg is not null))
			return "Lifecycle weapons require a configured native weapon type without unadapted wield scripts.";
		return null;
	}
}
