using MudSharp.Effects.Concrete;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Vehicles;

public static class HitchGearRules
{
	public static bool TowPointRequiresHitchItem(IVehicleTowPointPrototype? towPoint)
	{
		return towPoint is not null && !towPoint.TowType.EqualToAny("hand", "manual", "direct", "none", "pull");
	}

	public static bool TowPointsRequireHitchItem(IVehicleTowPointPrototype? sourceTowPoint,
		IVehicleTowPointPrototype? targetTowPoint)
	{
		return TowPointRequiresHitchItem(sourceTowPoint) || TowPointRequiresHitchItem(targetTowPoint);
	}

	public static bool IsUsableHitchGear(IGameItem? item)
	{
		return item?.GetItemType<IHitchGear>() is not null || item?.GetItemType<IDragAid>() is not null;
	}

	public static IDragAid? DragAidFor(IGameItem? item)
	{
		return item?.GetItemType<IHitchGear>() ?? item?.GetItemType<IDragAid>();
	}

	public static bool IsReserved(IGameItem item)
	{
		return item.AffectedBy<HitchGearInUse>();
	}

	public static bool GearCompatible(IGameItem? item, double towedWeight, out string reason,
		params IVehicleTowPointPrototype?[] towPoints)
	{
		return GearCompatible(item, towedWeight, out reason, true, towPoints);
	}

	public static bool GearCompatible(IGameItem? item, double towedWeight, out string reason, bool checkReserved,
		params IVehicleTowPointPrototype?[] towPoints)
	{
		if (item is null)
		{
			reason = "A hitch item is required for those tow points.";
			return false;
		}

		if (!IsUsableHitchGear(item))
		{
			reason = $"{item.HowSeen(null, true)} is not usable hitch gear.";
			return false;
		}

		if (checkReserved && IsReserved(item))
		{
			reason = $"{item.HowSeen(null, true)} is already being used as hitch gear.";
			return false;
		}

		var hitchGear = item.GetItemType<IHitchGear>();
		if (hitchGear is null)
		{
			reason = string.Empty;
			return true;
		}

		if (hitchGear.MaximumTowedWeight > 0.0 && towedWeight > hitchGear.MaximumTowedWeight)
		{
			reason = $"{item.HowSeen(null, true)} can only support {hitchGear.MaximumTowedWeight:N2} towed weight, but the target weighs {towedWeight:N2}.";
			return false;
		}

		foreach (var towPoint in towPoints.Where(TowPointRequiresHitchItem))
		{
			var required = RequiredRoles(towPoint!.TowType);
			if (required != HitchGearRole.None && !hitchGear.Roles.HasAny(required))
			{
				reason = $"{item.HowSeen(null, true)} is not compatible with a {towPoint.TowType} tow point.";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public static void Reserve(IGameItem? item, long? vehicleTowLinkId = null, long? vehicleHitchLinkId = null,
		long? sourceCharacterId = null, long? targetId = null)
	{
		if (item is null)
		{
			return;
		}

		item.AddEffect(new HitchGearInUse(item, vehicleTowLinkId, vehicleHitchLinkId, sourceCharacterId, targetId));
	}

	public static void Release(IGameItem? item, long? vehicleTowLinkId = null, long? vehicleHitchLinkId = null,
		long? sourceCharacterId = null, long? targetId = null)
	{
		item?.RemoveAllEffects<HitchGearInUse>(
			x => x.Matches(vehicleTowLinkId, vehicleHitchLinkId, sourceCharacterId, targetId),
			fireRemovalAction: true);
	}

	private static HitchGearRole RequiredRoles(string towType)
	{
		return towType.ToLowerInvariant() switch
		{
			"towbar" or "tow bar" or "bar" or "drawbar" or "draw bar" => HitchGearRole.TowBar,
			"yoke" => HitchGearRole.Yoke,
			"harness" => HitchGearRole.Harness | HitchGearRole.Traces,
			"rope" => HitchGearRole.Rope | HitchGearRole.LeadRope,
			"lead" or "leadrope" or "lead rope" => HitchGearRole.LeadRope | HitchGearRole.Rope,
			"chain" => HitchGearRole.Chain,
			"hitch" => HitchGearRole.TowBar | HitchGearRole.Yoke | HitchGearRole.Harness |
			           HitchGearRole.Rope | HitchGearRole.Chain | HitchGearRole.Traces,
			_ => HitchGearRole.None
		};
	}

	private static bool HasAny(this HitchGearRole roles, HitchGearRole required)
	{
		return (roles & required) != HitchGearRole.None;
	}
}
