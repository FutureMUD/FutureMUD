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
			return "Lifecycle weapons require an unscripted prototype without ordinary morphing.";
		if (world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("GameItem")))
			return "Lifecycle weapons require an adapter for configured GameItem default hooks before payment.";
		if (!prototype.Components.Any() || prototype.Components.Any(x => x.GetType() != typeof(HoldableGameItemComponentProto) &&
			x.GetType() != typeof(MeleeWeaponGameItemComponentProto) && x.GetType() != typeof(SalvageableGameItemComponentProto)) ||
			!prototype.IsItemType<HoldableGameItemComponentProto>())
			return "Lifecycle creation currently supports one plain holdable weapon; other component graphs need an adapter.";
		if (prototype.GetItemType<MeleeWeaponGameItemComponentProto>() is { } weapon &&
			(weapon.WeaponType is null || weapon.CanWieldProg is not null || weapon.WhyCannotWieldProg is not null))
			return "Lifecycle weapons require a configured native weapon type without unadapted wield scripts.";
		return null;
	}
}
