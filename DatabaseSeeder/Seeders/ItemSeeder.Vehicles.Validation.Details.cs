#nullable enable

using ExpressionEngine;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static void ValidateMovementProfile(
		VehicleSeedSpec owner,
		VehicleMovementProfileSeedSpec movement,
		IReadOnlySet<string> installationKeys)
	{
		RequireText(owner, movement.Name, $"movement profile {movement.Key} name");
		RequireNonNegativeFinite(owner, movement.RequiredPowerSpikeInWatts, $"movement profile {movement.Key} required power spike");
		RequireNonNegativeFinite(owner, movement.FuelVolumePerMove, $"movement profile {movement.Key} fuel volume per move");
		RequireNonNegativeFinite(owner, movement.RouteSpeedMetresPerSecond, $"movement profile {movement.Key} route speed");
		RequireNonNegativeFinite(owner, movement.RouteFuelVolumePerMetre, $"movement profile {movement.Key} route fuel use");
		RequireNonNegativeFinite(owner, movement.RoutePowerDrawWatts, $"movement profile {movement.Key} route power draw");
		if (movement.FuelVolumePerMove > 0.0 && string.IsNullOrWhiteSpace(movement.FuelLiquid))
		{
			throw VehicleValidation(owner, $"movement profile {movement.Key} consumes fuel per move but has no fuel liquid");
		}
		if (movement.MovementType == VehicleMovementProfileType.Route)
		{
			RequirePositiveFinite(owner, movement.RouteSpeedMetresPerSecond, $"route profile {movement.Key} speed");
			if (movement.AutomaticOperationCapable && movement.RoutePropulsionMode != RouteVehiclePropulsionMode.Powered)
			{
				throw VehicleValidation(owner, $"route profile {movement.Key} cannot be automatic while externally pulled");
			}
			if (movement.RoutePropulsionMode == RouteVehiclePropulsionMode.Powered &&
			    movement.RouteFuelVolumePerMetre <= 0.0 && movement.RoutePowerDrawWatts <= 0.0)
			{
				throw VehicleValidation(owner, $"powered route profile {movement.Key} must consume fuel or electrical power");
			}
		}
		else if (movement.RouteSpeedMetresPerSecond != 0.0 || movement.RouteFuelVolumePerMetre != 0.0 ||
		         movement.RoutePowerDrawWatts != 0.0 || movement.AutomaticOperationCapable)
		{
			throw VehicleValidation(owner, $"non-route profile {movement.Key} cannot use route-only values");
		}

		if (movement.Environment == VehicleMovementEnvironment.SurfaceWater)
		{
			if (movement.PropulsionProfiles.Count == 0 || movement.PropulsionProfiles.Count(x => x.IsDefault) != 1)
			{
				throw VehicleValidation(owner, $"surface-water profile {movement.Key} requires propulsion profiles and exactly one default");
			}
			var duplicate = movement.PropulsionProfiles.GroupBy(x => x.PropulsionType).FirstOrDefault(x => x.Count() > 1);
			if (duplicate is not null)
			{
				throw VehicleValidation(owner, $"surface-water profile {movement.Key} repeats propulsion type {duplicate.Key}");
			}
			foreach (var propulsion in movement.PropulsionProfiles)
			{
				ValidatePropulsionProfile(owner, movement, propulsion);
			}
			if (movement.PropulsionProfiles.Any(x => x.PropulsionType == VehiclePropulsionType.Rowed) &&
			    !owner.OccupantSlots.Any(x => x.ContributesToPropulsion))
			{
				throw VehicleValidation(owner, $"rowed profile {movement.Key} requires at least one propulsion-contributing occupant slot");
			}
			if (movement.PropulsionProfiles.Any(x => x.PropulsionType == VehiclePropulsionType.OutboardMotor) &&
			    !owner.InstallationPoints.Any(x => x.MountType.Equals(VehicleOutboardMountType, StringComparison.OrdinalIgnoreCase) &&
			                                       x.RequiredRole.Equals(VehiclePropulsionRole, StringComparison.OrdinalIgnoreCase)))
			{
				throw VehicleValidation(owner, $"outboard profile {movement.Key} requires an outboard_motor installation point with the propulsion role");
			}
		}
		else if (movement.PropulsionProfiles.Count > 0)
		{
			throw VehicleValidation(owner, $"terrestrial profile {movement.Key} must use movement resources/towing rather than surface-water propulsion profiles");
		}

		if (!string.IsNullOrWhiteSpace(movement.RequiredInstalledRole) &&
		    !owner.InstallationPoints.Any(x => x.RequiredRole.Equals(movement.RequiredInstalledRole, StringComparison.OrdinalIgnoreCase)))
		{
			throw VehicleValidation(owner, $"movement profile {movement.Key} requires role '{movement.RequiredInstalledRole}' but no installation point supplies it");
		}
	}

	private static void ValidatePropulsionProfile(
		VehicleSeedSpec owner,
		VehicleMovementProfileSeedSpec movement,
		VehiclePropulsionSeedSpec propulsion)
	{
		if (propulsion.PropulsionType == VehiclePropulsionType.None)
		{
			throw VehicleValidation(owner, $"movement profile {movement.Key} cannot seed propulsion type None");
		}
		RequirePositiveFinite(owner, propulsion.BaseMoveTimeMilliseconds,
			$"movement profile {movement.Key}/{propulsion.PropulsionType} base move time");
		if (propulsion.BaseMoveTimeMilliseconds < 250.0 || propulsion.BaseMoveTimeMilliseconds > 300000.0)
		{
			throw VehicleValidation(owner,
				$"movement profile {movement.Key}/{propulsion.PropulsionType} base move time should be between 250 and 300000 milliseconds");
		}
		if ((propulsion.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed) &&
		    propulsion.TraitCandidates.Count == 0)
		{
			throw VehicleValidation(owner, $"movement profile {movement.Key}/{propulsion.PropulsionType} needs trait candidates");
		}
		RequireText(owner, propulsion.SpeedMultiplierExpression,
			$"movement profile {movement.Key}/{propulsion.PropulsionType} speed expression");
		RequireText(owner, propulsion.StaminaCostExpression,
			$"movement profile {movement.Key}/{propulsion.PropulsionType} stamina expression");
		try
		{
			foreach (var outcome in Enumerable.Range(-3, 7))
			{
				var speed = new Expression(propulsion.SpeedMultiplierExpression).EvaluateDoubleWith(
					("outcome", (double)outcome), ("wind", 4.0), ("output", 1.0), ("swimcost", 1.0));
				var stamina = new Expression(propulsion.StaminaCostExpression).EvaluateDoubleWith(
					("outcome", (double)outcome), ("wind", 4.0), ("output", 1.0), ("swimcost", 1.0));
				if (!double.IsFinite(speed) || speed <= 0.0 || !double.IsFinite(stamina) || stamina < 0.0)
				{
					throw VehicleValidation(owner,
						$"movement profile {movement.Key}/{propulsion.PropulsionType} expressions produce invalid values");
				}
			}
		}
		catch (InvalidOperationException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw VehicleValidation(owner,
				$"movement profile {movement.Key}/{propulsion.PropulsionType} contains an invalid expression: {ex.Message}");
		}
	}

	private static void ValidateVehicleItem(VehicleSeedSpec owner, VehicleItemSeedSpec item, string label, bool projection)
	{
		RequireText(owner, item.Noun, $"{label} noun");
		RequireText(owner, item.ShortDescription, $"{label} short description");
		RequireText(owner, item.FullDescription, $"{label} full description");
		RequireText(owner, item.Material, $"{label} material");
		RequireText(owner, item.DestroyableComponent, $"{label} destroyable component");
		RequirePositiveFinite(owner, item.WeightInGrams, $"{label} weight");
		if (item.Cost < 0.0m)
		{
			throw VehicleValidation(owner, $"{label} cost cannot be negative");
		}
		if (projection && (item.Portable || item.Skinnable || !item.HiddenFromPlayers))
		{
			throw VehicleValidation(owner, $"{label} must be hidden, non-portable and non-skinnable");
		}
	}

	private static void ValidateDamageEffectReference(
		VehicleSeedSpec owner,
		VehicleDamageZoneSeedSpec zone,
		VehicleDamageEffectSeedSpec effect,
		IReadOnlySet<string> movementKeys,
		IReadOnlySet<string> accessKeys,
		IReadOnlySet<string> cargoKeys,
		IReadOnlySet<string> installationKeys,
		IReadOnlySet<string> towKeys)
	{
		if (effect.TargetType == VehicleDamageEffectTargetType.WholeVehicleMovement)
		{
			if (!string.IsNullOrWhiteSpace(effect.TargetKey))
			{
				throw VehicleValidation(owner, $"damage zone {zone.Key} whole-vehicle effect must not specify a target key");
			}
			return;
		}
		if (string.IsNullOrWhiteSpace(effect.TargetKey))
		{
			throw VehicleValidation(owner, $"damage zone {zone.Key} effect {effect.TargetType} requires a target key");
		}

		var keys = effect.TargetType switch
		{
			VehicleDamageEffectTargetType.MovementProfile => movementKeys,
			VehicleDamageEffectTargetType.AccessPoint => accessKeys,
			VehicleDamageEffectTargetType.CargoSpace => cargoKeys,
			VehicleDamageEffectTargetType.InstallationPoint => installationKeys,
			VehicleDamageEffectTargetType.TowPoint => towKeys,
			_ => throw VehicleValidation(owner, $"damage zone {zone.Key} uses unsupported effect target {effect.TargetType}")
		};
		RequireReference(owner, keys, effect.TargetKey, $"damage zone {zone.Key} effect target");
	}

	private static void ValidateTowStress(VehicleSeedSpec owner, VehicleTowPointSeedSpec point)
	{
		var values = new[]
		{
			point.TowStressWarningRatio,
			point.TowStressFailureStartRatio,
			point.TowStressMaximumFailureChance,
			point.TowStressDamageMultiplier
		};
		if (values.All(x => x is null))
		{
			return;
		}
		if (values.Any(x => x is null))
		{
			throw VehicleValidation(owner, $"tow point {point.Key} must specify either all or none of the tow-stress values");
		}
		if (!double.IsFinite(point.TowStressWarningRatio!.Value) || point.TowStressWarningRatio.Value <= 0.0 ||
		    !double.IsFinite(point.TowStressFailureStartRatio!.Value) || point.TowStressFailureStartRatio.Value < point.TowStressWarningRatio.Value ||
		    !double.IsFinite(point.TowStressMaximumFailureChance!.Value) || point.TowStressMaximumFailureChance.Value is < 0.0 or > 1.0 ||
		    !double.IsFinite(point.TowStressDamageMultiplier!.Value) || point.TowStressDamageMultiplier.Value <= 0.0)
		{
			throw VehicleValidation(owner, $"tow point {point.Key} has invalid tow-stress values");
		}
	}

	private static void ValidateUniqueKeys(VehicleSeedSpec owner, IEnumerable<string> keys, string label)
	{
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var key in keys)
		{
			if (string.IsNullOrWhiteSpace(key) || !Regex.IsMatch(key, "^[a-z0-9_]+$") || !seen.Add(key))
			{
				throw VehicleValidation(owner, $"{label} keys must be unique lowercase underscore identifiers; invalid key '{key}'");
			}
		}
	}

	private static void ValidateUniqueIntegers(VehicleSeedSpec owner, IEnumerable<int> values, string label)
	{
		var array = values.ToArray();
		if (array.Length != array.Distinct().Count())
		{
			throw VehicleValidation(owner, $"{label} values must be unique within the vehicle");
		}
	}

	private static void RequireReference(VehicleSeedSpec owner, IReadOnlySet<string> keys, string key, string label)
	{
		if (!keys.Contains(key))
		{
			throw VehicleValidation(owner, $"{label} references missing key '{key}'");
		}
	}

	private static void RequireText(VehicleSeedSpec owner, string? value, string label)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw VehicleValidation(owner, $"{label} is required");
		}
	}

	private static void RequirePositiveFinite(VehicleSeedSpec owner, double value, string label)
	{
		if (!double.IsFinite(value) || value <= 0.0)
		{
			throw VehicleValidation(owner, $"{label} must be positive and finite");
		}
	}

	private static void RequireNonNegativeFinite(VehicleSeedSpec owner, double value, string label)
	{
		if (!double.IsFinite(value) || value < 0.0)
		{
			throw VehicleValidation(owner, $"{label} must be non-negative and finite");
		}
	}

	private static InvalidOperationException VehicleValidation(VehicleSeedSpec owner, string message)
	{
		return new InvalidOperationException($"Vehicle seed '{owner.StableReference}' is invalid: {message}.");
	}
}
