#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehicleItemSeedSpec Exterior(
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		ItemQuality quality,
		double weight,
		decimal cost,
		string material,
		string destroyable,
		bool portable)
	{
		return new VehicleItemSeedSpec(noun, shortDescription, fullDescription, size, quality, weight, cost, material,
			destroyable, portable);
	}

	private static VehicleItemSeedSpec Projection(
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		string material,
		string destroyable)
	{
		return new VehicleItemSeedSpec(noun, shortDescription, fullDescription, size, ItemQuality.Standard, 1.0, 0.0m,
			material, destroyable, false, false, true);
	}

	private static VehicleMovementProfileSeedSpec LandMovement(
		string key,
		string name,
		string? fuelLiquid,
		double fuelPerMove,
		double requiredPower,
		string requiredRole,
		bool requiresAccessClosed)
	{
		return new VehicleMovementProfileSeedSpec(key, name, VehicleMovementProfileType.CellExit,
			VehicleMovementEnvironment.Unrestricted, false, true, requiredPower, fuelLiquid, fuelPerMove, requiredRole,
			true, requiresAccessClosed, 0.0, RouteVehiclePropulsionMode.Powered, 0.0, 0.0, false, []);
	}

	private static VehicleMovementProfileSeedSpec RouteMovement(
		string key,
		string name,
		RouteVehiclePropulsionMode mode,
		double speedMetresPerSecond,
		string? fuelLiquid,
		double fuelPerMetre,
		double powerDrawWatts,
		bool automatic)
	{
		return new VehicleMovementProfileSeedSpec(key, name, VehicleMovementProfileType.Route,
			VehicleMovementEnvironment.Unrestricted, false, true, 0.0, fuelLiquid, 0.0,
			mode == RouteVehiclePropulsionMode.Powered ? VehiclePropulsionRole : string.Empty,
			true, true, speedMetresPerSecond, mode, fuelPerMetre, powerDrawWatts, automatic, []);
	}

	private static VehicleMovementProfileSeedSpec WaterMovement(
		string key,
		string name,
		bool requiresAccessClosed,
		IReadOnlyCollection<VehiclePropulsionSeedSpec> propulsion)
	{
		return new VehicleMovementProfileSeedSpec(key, name, VehicleMovementProfileType.CellExit,
			VehicleMovementEnvironment.SurfaceWater, true, true, 0.0, null, 0.0, string.Empty,
			true, requiresAccessClosed, 0.0, RouteVehiclePropulsionMode.Powered, 0.0, 0.0, false, propulsion);
	}

	private static VehiclePropulsionSeedSpec SelfPoweredProfile(bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(VehiclePropulsionType.SelfPowered, isDefault, 8500.0,
			["Swimming", "Rowing", "Athletics"], Difficulty.Normal,
			"1.0 + (0.15 * outcome)", "max(0.5, swimcost * (1.0 - (0.05 * outcome)))");
	}

	private static VehiclePropulsionSeedSpec RowedProfile(bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(VehiclePropulsionType.Rowed, isDefault, 9000.0,
			["Rowing", "Swimming", "Athletics"], Difficulty.Normal,
			"1.0 + (0.15 * outcome)", "max(0.5, swimcost * (1.0 - (0.05 * outcome)))");
	}

	private static VehiclePropulsionSeedSpec SailProfile(bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(VehiclePropulsionType.Sail, isDefault, 7800.0, [], Difficulty.Normal,
			"0.4 + (0.2 * wind)", "0.0");
	}

	private static VehiclePropulsionSeedSpec OutboardProfile(bool isDefault)
	{
		return new VehiclePropulsionSeedSpec(VehiclePropulsionType.OutboardMotor, isDefault, 5200.0, [], Difficulty.Easy,
			"max(0.25, output)", "0.0");
	}

	private static IReadOnlyCollection<VehicleDamageZoneSeedSpec> LandDamageZones(string movementKey, double massHint)
	{
		return
		[
			new VehicleDamageZoneSeedSpec("frame", "frame and body", "Damage to the principal frame can disable or destroy the whole vehicle.",
				Math.Clamp(massHint / 10000.0, 120.0, 1200.0), 3.0, 0.65, 1.0, true, 0,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.WholeVehicleMovement, null, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("running_gear", "wheels and running gear", "Damage to the wheels, axle or running gear prevents controlled travel.",
				Math.Clamp(massHint / 25000.0, 80.0, 500.0), 2.0, 0.55, 1.0, false, 1,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.MovementProfile, movementKey, VehicleSystemStatus.Disabled)])
		];
	}

	private static IReadOnlyCollection<VehicleDamageZoneSeedSpec> PoweredLandDamageZones(string movementKey, string installationKey)
	{
		return
		[
			new VehicleDamageZoneSeedSpec("body", "body and chassis", "Serious structural damage compromises the vehicle as a whole.",
				500.0, 3.0, 0.7, 1.0, true, 0,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.WholeVehicleMovement, null, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("running_gear", "wheels and running gear", "Damage to wheels, suspension or steering prevents controlled road movement.",
				300.0, 2.0, 0.55, 1.0, false, 1,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.MovementProfile, movementKey, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("drive", "drive module and mount", "Damage around the drive bay can disable the installed propulsion module.",
				220.0, 1.5, 0.55, 1.0, false, 2,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.InstallationPoint, installationKey, VehicleSystemStatus.Disabled)])
		];
	}

	private static IReadOnlyCollection<VehicleDamageZoneSeedSpec> WaterDamageZones(string movementKey, string? installationKey)
	{
		IReadOnlyCollection<VehicleDamageEffectSeedSpec> installationEffects = string.IsNullOrWhiteSpace(installationKey)
			? Array.Empty<VehicleDamageEffectSeedSpec>()
			: [new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.InstallationPoint, installationKey, VehicleSystemStatus.Disabled)];
		return
		[
			new VehicleDamageZoneSeedSpec("hull", "hull", "Hull damage threatens buoyancy and eventually prevents all movement.",
				420.0, 4.0, 0.7, 1.0, true, 0,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.WholeVehicleMovement, null, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("propulsion", "propulsion fittings", "Damage to propulsion fittings prevents the selected movement system from working.",
				180.0, 1.5, 0.55, 1.0, false, 1,
				installationEffects.Count > 0
					? installationEffects
					: [new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.MovementProfile, movementKey, VehicleSystemStatus.Disabled)])
		];
	}

	private static IReadOnlyCollection<VehicleDamageZoneSeedSpec> SailDamageZones(
		string movementKey,
		string accessKey,
		string? cargoKey)
	{
		var holdEffects = new List<VehicleDamageEffectSeedSpec>
		{
			new(VehicleDamageEffectTargetType.AccessPoint, accessKey, VehicleSystemStatus.Disabled)
		};
		if (!string.IsNullOrWhiteSpace(cargoKey))
		{
			holdEffects.Add(new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, cargoKey, VehicleSystemStatus.Disabled));
		}
		return
		[
			new VehicleDamageZoneSeedSpec("hull", "hull", "Serious hull damage compromises buoyancy and the vessel as a whole.",
				900.0, 4.0, 0.7, 1.0, true, 0,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.WholeVehicleMovement, null, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("rig", "mast, sails and rigging", "Damage to the sailing rig prevents effective propulsion.",
				420.0, 2.0, 0.55, 1.0, false, 1,
				[new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.MovementProfile, movementKey, VehicleSystemStatus.Disabled)]),
			new VehicleDamageZoneSeedSpec("hold", "hold and hatch", "Damage around the hold can jam its hatch or make cargo inaccessible.",
				300.0, 1.0, 0.6, 1.0, false, 2, holdEffects)
		];
	}
}
