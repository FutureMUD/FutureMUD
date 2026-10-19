#nullable enable

using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Vehicles;

public static class VehicleEnvironmentRules
{
	public static bool IsSurfaceWater(this IRoom? location, RoomLayer layer)
	{
		return location is not null &&
		       layer == RoomLayer.GroundLevel &&
		       (location.IsSwimmingLayer(RoomLayer.GroundLevel) ||
		        location.Terrain(null)?.Immersion?.Medium == TerrainMedium.Liquid);
	}

	public static bool IsSiltSurface(this IRoom? location, RoomLayer layer)
	{
		return location is not null && layer == RoomLayer.GroundLevel &&
		       location.Terrain(null)?.Immersion?.Medium == TerrainMedium.Silt;
	}

	public static bool IsSiltVehicle(this IVehicle? vehicle)
	{
		return vehicle?.MovementProfile?.MovementEnvironment == VehicleMovementEnvironment.Silt;
	}

	public static bool IsSurfaceWaterVehicle(this IVehicle? vehicle)
	{
		return vehicle?.MovementProfile?.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater;
	}

	public static bool IsSupportedBySurfaceWaterVehicle(this ICharacter? character, out IVehicle? vehicle)
	{
		vehicle = character?.Gameworld?.Vehicles
			.FirstOrDefault(x => x.IsOccupant(character));
		return vehicle is not null &&
		       IsIntact(vehicle) &&
		       vehicle.IsSurfaceWaterVehicle() &&
		       vehicle.Location.IsSurfaceWater(vehicle.RoomLayer) &&
		       character!.Location == vehicle.Location &&
		       character.RoomLayer == vehicle.RoomLayer;
	}

	public static bool IsProtectedFromSurfaceWater(this ICharacter? character, out IVehicle? vehicle)
	{
		return character.IsSupportedBySurfaceWaterVehicle(out vehicle) &&
		       vehicle?.MovementProfile?.ExposesOccupantsToWater != true;
	}

	public static bool KeepsExteriorAfloat(this IVehicle? vehicle)
	{
		return vehicle is not null &&
		       IsIntact(vehicle) &&
		       vehicle.IsSurfaceWaterVehicle() &&
		       vehicle.Location.IsSurfaceWater(vehicle.RoomLayer) &&
		       vehicle.ExteriorItem?.Location == vehicle.Location &&
		       vehicle.ExteriorItem.RoomLayer == vehicle.RoomLayer;
	}

	public static bool CanTraverseEnvironment(this IVehicle? vehicle, IRoom? destination, RoomLayer destinationLayer,
		out string reason)
	{
		if (vehicle.IsSiltVehicle())
		{
			if (!vehicle!.Location.IsSiltSurface(vehicle.RoomLayer))
			{
				reason = $"{vehicle.Name} is not at the surface of a silt location.";
				return false;
			}

			if (!destination.IsSiltSurface(destinationLayer))
			{
				reason = $"{vehicle.Name} can only move to another silt-surface location.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		if (!vehicle.IsSurfaceWaterVehicle())
		{
			reason = string.Empty;
			return true;
		}

		if (!vehicle!.Location.IsSurfaceWater(vehicle.RoomLayer))
		{
			reason = $"{vehicle.Name} is not at the surface of a water location.";
			return false;
		}

		if (!destination.IsSurfaceWater(destinationLayer))
		{
			reason = $"{vehicle.Name} can only move to another surface-water location.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool IsIntact(IVehicle vehicle)
	{
		return !vehicle.Destroyed &&
		       vehicle.ExteriorItem is { Deleted: false, Destroyed: false };
	}
}
