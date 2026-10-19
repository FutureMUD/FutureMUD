#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Planes;
using MudSharp.Vehicles;

namespace MudSharp.Construction;

/// <summary>Pure placement, coverage and anatomy queries for categorical ground media.</summary>
public static class TerrainImmersionHelper
{
	public const double WadingTimeMultiplier = 1.5;
	public const double WadingStaminaMultiplier = 1.5;

	public static TerrainImmersionProfile? ProfileFor(IRoom? room, RoomLayer layer) =>
		layer == RoomLayer.GroundLevel ? room?.Terrain(null)?.Immersion : null;

	public static SizeCategory SizeForPosture(IBody body, IPositionState posture) =>
		posture == PositionSwimming.Instance ? body.SizeProne : posture.Upright ? body.SizeStanding :
		posture.CompareTo(PositionProne.Instance) == PositionHeightComparison.Higher ? body.SizeSitting : body.SizeProne;

	public static bool IsSupportedByVehicle(ICharacter character, out IVehicle? vehicle)
	{
		vehicle = character.Gameworld?.Vehicles?.FirstOrDefault(x =>
			x.IsOccupant(character) && !x.Destroyed && x.ExteriorItem is { Deleted: false, Destroyed: false } &&
			x.ExteriorItem.Location == x.Location && x.ExteriorItem.RoomLayer == x.RoomLayer &&
			x.Location == character.Location && x.RoomLayer == character.RoomLayer &&
			x.CanTraverseEnvironment(x.Location, x.RoomLayer, out _));
		return vehicle is not null;
	}

	private static bool AboveMedium(IBody body, IPositionState posture, IRoom room, RoomLayer layer) =>
		posture == PositionFlying.Instance || body.Actor.SuspendsPhysicalContact() ||
		body.Actor.RidingMount is { } mount && mount.Location == room && mount.RoomLayer == layer ||
		body.Location == room && body.RoomLayer == layer && IsSupportedByVehicle(body.Actor, out var vehicle) &&
		vehicle?.MovementProfile?.ExposesOccupantsToWater != true;

	public static ImmersionContact ContactFor(IBody body, IPositionState? posture = null,
		IRoom? room = null, RoomLayer? layer = null, bool atBottom = false)
	{
		room ??= body.Location;
		var actualLayer = layer ?? body.RoomLayer;
		var profile = ProfileFor(room, actualLayer);
		posture ??= body.PositionState;
		if (profile is null || room is null || AboveMedium(body, posture, room, actualLayer)) return ImmersionContact.None;
		if (!atBottom && profile.Medium == TerrainMedium.Liquid &&
		    (posture == PositionSwimming.Instance && !body.Actor.State.IsUnconscious() ||
		     IsSupportedByVehicle(body.Actor, out var exposedVehicle) && exposedVehicle?.MovementProfile?.ExposesOccupantsToWater == true))
			return new(ImmersionExtent.Partial, Orientation.Centre, 1);
		return profile.ContactFor(SizeForPosture(body, posture));
	}

	public static bool RequiresSwimming(ICharacter character, IRoom? room = null, RoomLayer? layer = null,
		IPositionState? posture = null)
	{
		room ??= character.Location;
		var actualLayer = layer ?? character.RoomLayer;
		if (ProfileFor(room, actualLayer) is not { Medium: TerrainMedium.Liquid } profile) return false;
		posture ??= character.PositionState;
		if (room is null || AboveMedium(character.Body, posture, room, actualLayer) ||
		    character.Location == room && IsSupportedByVehicle(character, out _)) return false;
		// Swimming describes flotation, not the standing body's ability to use the bottom.
		var size = posture == PositionSwimming.Instance ? character.Body.SizeStanding : SizeForPosture(character.Body, posture);
		return profile.ContactFor(size).Extent == ImmersionExtent.Full;
	}

	public static bool IsWading(ICharacter character, IPositionState? posture = null,
		IRoom? room = null, RoomLayer? layer = null)
	{
		if (ProfileFor(room ?? character.Location, layer ?? character.RoomLayer) is null) return false;
		posture ??= character.PositionState;
		if (posture == PositionSwimming.Instance || posture == PositionFlying.Instance) return false;
		var contact = ContactFor(character.Body, posture, room, layer);
		return contact.IsWading || contact.Extent == ImmersionExtent.Full &&
			ProfileFor(room ?? character.Location, layer ?? character.RoomLayer)?.Medium == TerrainMedium.Silt;
	}

	public static ImmersionContact ItemContactFor(IGameItem item)
	{
		var profile = ProfileFor(item.Location, item.RoomLayer);
		if (profile is null || item.ContainedIn is not null || item.InInventoryOf is not null ||
		    item.SuspendsPhysicalContact() || item.PositionState == PositionFlying.Instance ||
		    item.Gameworld?.Vehicles?.Any(x => x.ExteriorItem == item && !x.Destroyed &&
			    x.CanTraverseEnvironment(x.Location, x.RoomLayer, out _) &&
			    x.MovementProfile?.MovementEnvironment != VehicleMovementEnvironment.Unrestricted) == true)
			return ImmersionContact.None;
		if (profile.Medium == TerrainMedium.Liquid && item.Location?.Terrain(null).WaterFluid is ILiquid liquid &&
		    item.Buoyancy(liquid.Density) > 0)
			return new(ImmersionExtent.Partial, Orientation.Lowest, int.MaxValue);
		return profile.ContactFor(item.Size);
	}

	public static bool IsObscuredByTerrain(IGameItem item) =>
		ProfileFor(item.Location, item.RoomLayer)?.ObscuresItems == true &&
		ItemContactFor(item).Extent == ImmersionExtent.Full;

	public static ILiquid? LiquidFor(IBody body) =>
		ProfileFor(body.Location, body.RoomLayer) is { Medium: TerrainMedium.Silt } ? null :
		body.Location?.Terrain(null)?.WaterFluid as ILiquid;

	public static ILiquid? LiquidFor(IGameItem item) =>
		ProfileFor(item.Location, item.RoomLayer) is { Medium: TerrainMedium.Silt } ? null :
		item.Location?.Terrain(null)?.WaterFluid as ILiquid;

	public static IExternalBodypart[] ImmersedParts(IBody body, IPerceivable? anchor = null)
	{
		var room = anchor?.Location ?? body.Location;
		var layer = anchor?.RoomLayer ?? body.RoomLayer;
		var profile = ProfileFor(room, layer);
		if (profile is null) return Array.Empty<IExternalBodypart>();
		var posture = anchor is IGameItem ? PositionProne.Instance : body.PositionState;
		if (anchor is IGameItem anchoredItem && ItemContactFor(anchoredItem).Extent == ImmersionExtent.None)
			return Array.Empty<IExternalBodypart>();
		var floatingRemains = anchor is IGameItem remains && profile.Medium == TerrainMedium.Liquid &&
			LiquidFor(remains) is { } liquid && remains.Buoyancy(liquid.Density) > 0;
		var contact = floatingRemains ? new ImmersionContact(ImmersionExtent.Partial, Orientation.Centre, 1) :
			anchor is IGameItem ? profile.ContactFor(SizeForPosture(body, posture)) : ContactFor(body);
		if (floatingRemains) posture = PositionStanding.Instance; // Coarse surface contact, not full bed coverage.
		var atSurface = anchor is null && profile.Medium == TerrainMedium.Liquid &&
			(posture == PositionSwimming.Instance && !body.Actor.State.IsUnconscious() ||
			 IsSupportedByVehicle(body.Actor, out var vehicle) && vehicle?.MovementProfile?.ExposesOccupantsToWater == true);
		return body.Bodyparts.OfType<IExternalBodypart>()
			.Where(part => !(atSurface && part.BodypartType is BodypartTypeEnum.Mouth or BodypartTypeEnum.Blowhole) &&
				contact.Covers(EffectiveOrientation(body, part, posture))).ToArray();
	}

	public static bool IsPartImmersed(IBody body, IBodypart? part)
	{
		var profile = ProfileFor(body.Location, body.RoomLayer);
		if (profile is null) return body.Location?.IsUnderwaterLayer(body.RoomLayer) == true;
		if (profile.Medium != TerrainMedium.Liquid) return false;
		if (part is null) return ContactFor(body).Extent == ImmersionExtent.Full;
		var immersed = ImmersedParts(body);
		if (part is IExternalBodypart external) return immersed.Contains(external);
		var visited = new HashSet<IBodypart>();
		for (var current = part; current is not null && visited.Add(current); current = current.UpstreamConnection)
			if (immersed.Contains(current)) return true;
		return part is not IExternalBodypart && (body.Bodyparts.OfType<IExternalBodypart>()
			.Any(x => x.Organs?.Any(organ => ReferenceEquals(organ, part)) == true && immersed.Contains(x)) ||
			ContactFor(body).Extent == ImmersionExtent.Full);
	}

	public static IFluid? RespiratoryFluid(IBody body, IBodypart? part = null) =>
		IsPartImmersed(body, part)
			? (ProfileFor(body.Location, body.RoomLayer) is null
				? body.Location?.Terrain(body.Actor)?.WaterFluid
				: LiquidFor(body)) ?? body.Location?.Atmosphere
			: body.Location?.Atmosphere;

	private static Orientation EffectiveOrientation(IBody body, IBodypart part, IPositionState posture)
	{
		var orientation = part.Orientation;
		if (orientation is Orientation.Appendage or Orientation.Irrelevant)
		{
			if (body.Prototype.Limbs.Any(x => x.LimbType == LimbType.Arm && x.Parts.Contains(part)))
				orientation = Orientation.Centre;
			else
			{
				var visited = new HashSet<IBodypart>();
				for (var current = part.UpstreamConnection; current is not null && visited.Add(current);
				     current = current.UpstreamConnection)
					if (current.Orientation is >= Orientation.Lowest and <= Orientation.Highest)
					{
						orientation = current.Orientation;
						break;
					}
				if (orientation is Orientation.Appendage or Orientation.Irrelevant) orientation = Orientation.Centre;
			}
		}
		if (posture is PositionProne or PositionProstrate or PositionSprawled or PositionLyingDown)
			return part.BodypartType is BodypartTypeEnum.Mouth or BodypartTypeEnum.Blowhole &&
			       posture is PositionProne or PositionProstrate ? Orientation.Lowest : Orientation.Low;
		if (posture is PositionSitting or PositionKneeling &&
		    body.Prototype.Limbs.Any(x => x.LimbType == LimbType.Leg && x.Parts.Contains(part)))
			return Orientation.Lowest;
		return orientation;
	}
}
