#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private IReadOnlyList<string> ValidateVehiclePrototypeSeedSpecs(IReadOnlyList<VehiclePrototypeSeedSpec> specs)
	{
		var issues = new List<string>();
		var duplicateStableReferences = specs
			.SelectMany(AllVehicleStableReferences)
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key);
		issues.AddRange(duplicateStableReferences.Select(x => $"Duplicate vehicle or projection stable reference {x}"));

		foreach (var spec in specs)
		{
			ValidateVehicleProjectionItem(spec.StableReference, "exterior", spec.ExteriorItem, issues);
			foreach (var access in spec.AccessPoints)
			{
				ValidateVehicleProjectionItem(spec.StableReference, $"access {access.Key}", access.ProjectionItem, issues);
			}
			foreach (var cargo in spec.CargoSpaces)
			{
				ValidateVehicleProjectionItem(spec.StableReference, $"cargo {cargo.Key}", cargo.ProjectionItem, issues);
			}

			if (!StableVehicleReferenceRegex.IsMatch(spec.StableReference))
			{
				issues.Add($"{spec.StableReference}: vehicle stable reference must be lowercase snake case");
			}

			if (!StableVehicleReferenceRegex.IsMatch(spec.ExteriorItem.StableReference))
			{
				issues.Add($"{spec.StableReference}: exterior stable reference is not lowercase snake case");
			}

			ValidateUniqueVehicleKeys(spec.StableReference, "compartment", spec.Compartments.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "slot", spec.OccupantSlots.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "movement", spec.MovementProfiles.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "access", spec.AccessPoints.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "cargo", spec.CargoSpaces.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "installation", spec.InstallationPoints.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "tow", spec.TowPoints.Select(x => x.Key), issues);
			ValidateUniqueVehicleKeys(spec.StableReference, "damage", spec.DamageZones.Select(x => x.Key), issues);

			var compartmentKeys = spec.Compartments.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var slotKeys = spec.OccupantSlots.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var movementKeys = spec.MovementProfiles.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var accessKeys = spec.AccessPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var cargoKeys = spec.CargoSpaces.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var installationKeys = spec.InstallationPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var towKeys = spec.TowPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

			if (spec.Compartments.Count == 0)
			{
				issues.Add($"{spec.StableReference}: at least one compartment is required");
			}

			if (spec.Scale == VehicleScale.RoomScale && spec.Compartments.Any(x => string.IsNullOrWhiteSpace(x.InteriorTerrainName)))
			{
				issues.Add($"{spec.StableReference}: every RoomScale compartment requires an interior terrain name");
			}

			foreach (var slot in spec.OccupantSlots)
			{
				if (!compartmentKeys.Contains(slot.CompartmentKey))
				{
					issues.Add($"{spec.StableReference}: slot {slot.Key} references missing compartment {slot.CompartmentKey}");
				}
				if (slot.Capacity <= 0)
				{
					issues.Add($"{spec.StableReference}: slot {slot.Key} must have positive capacity");
				}
			}

			var drivers = spec.OccupantSlots.Where(x => x.SlotType == VehicleOccupantSlotType.Driver).ToArray();
			if (drivers.Length == 0)
			{
				issues.Add($"{spec.StableReference}: at least one driver slot is required");
			}

			if (spec.ControlStations.Count(x => x.IsPrimary) != 1)
			{
				issues.Add($"{spec.StableReference}: exactly one primary control station is required");
			}
			foreach (var station in spec.ControlStations)
			{
				if (!slotKeys.Contains(station.SlotKey))
				{
					issues.Add($"{spec.StableReference}: control station {station.Name} references missing slot {station.SlotKey}");
				}
				else if (spec.OccupantSlots.Single(x => x.Key.Equals(station.SlotKey, StringComparison.OrdinalIgnoreCase)).SlotType != VehicleOccupantSlotType.Driver)
				{
					issues.Add($"{spec.StableReference}: control station {station.Name} must belong to a driver slot");
				}
			}

			if (spec.MovementProfiles.Count == 0 ||
			    spec.MovementProfiles.All(x => x.MovementType is not (VehicleMovementProfileType.CellExit or VehicleMovementProfileType.Route)))
			{
				issues.Add($"{spec.StableReference}: at least one CellExit or Route movement profile is required");
			}
			if (spec.MovementProfiles.Count(x => x.IsDefault) != 1)
			{
				issues.Add($"{spec.StableReference}: exactly one default movement profile is required");
			}

			foreach (var movement in spec.MovementProfiles)
			{
				if (movement.RequiredPowerSpikeInWatts < 0.0 || movement.FuelVolumePerMove < 0.0 ||
				    movement.RouteFuelVolumePerMetre < 0.0 || movement.RoutePowerDrawWatts < 0.0)
				{
					issues.Add($"{spec.StableReference}: movement {movement.Key} has a negative resource requirement");
				}
				if (movement.FuelLiquidName is null && movement.FuelVolumePerMove != 0.0)
				{
					issues.Add($"{spec.StableReference}: movement {movement.Key} has fuel consumption without a fuel liquid");
				}
				if (movement.FuelLiquidName is not null && !_liquids.ContainsKey(movement.FuelLiquidName))
				{
					issues.Add($"{spec.StableReference}: movement {movement.Key} references missing liquid {movement.FuelLiquidName}");
				}
				foreach (var propulsion in movement.PropulsionProfiles)
				{
					if (!double.IsFinite(propulsion.BaseMoveTimeMilliseconds) || propulsion.BaseMoveTimeMilliseconds <= 0.0)
					{
						issues.Add($"{spec.StableReference}: {propulsion.PropulsionType} propulsion requires a positive finite base move time");
					}
					if (string.IsNullOrWhiteSpace(propulsion.SpeedMultiplierExpression) ||
					    string.IsNullOrWhiteSpace(propulsion.StaminaCostExpression))
					{
						issues.Add($"{spec.StableReference}: {propulsion.PropulsionType} propulsion requires speed and stamina expressions");
					}
				}
				if (movement.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater)
				{
					if (movement.MovementType != VehicleMovementProfileType.CellExit)
					{
						issues.Add($"{spec.StableReference}: surface-water movement must use CellExit movement");
					}
					if (movement.PropulsionProfiles.Count == 0)
					{
						issues.Add($"{spec.StableReference}: surface-water movement {movement.Key} requires propulsion profiles");
					}
					if (movement.PropulsionProfiles.Count(x => x.IsDefault) != 1)
					{
						issues.Add($"{spec.StableReference}: surface-water movement {movement.Key} requires exactly one default propulsion mode");
					}
					var types = movement.PropulsionProfiles.Select(x => x.PropulsionType).ToArray();
					if (types.Contains(VehiclePropulsionType.None) && types.Length > 1)
					{
						issues.Add($"{spec.StableReference}: none propulsion cannot be combined with another mode");
					}
				}
			}

			foreach (var access in spec.AccessPoints)
			{
				if (access.CompartmentKey is not null && !compartmentKeys.Contains(access.CompartmentKey))
				{
					issues.Add($"{spec.StableReference}: access {access.Key} references missing compartment {access.CompartmentKey}");
				}
				if (!StableVehicleReferenceRegex.IsMatch(access.ProjectionItem.StableReference))
				{
					issues.Add($"{spec.StableReference}: access projection {access.ProjectionItem.StableReference} is not lowercase snake case");
				}
				if (access.ProjectionItem.Components.Any(x =>
				        _components.TryGetValue(x, out var component) &&
				        component.Type is "Door" or "LockingDoor" or "ElectronicDoor"))
				{
					issues.Add($"{spec.StableReference}: access projection {access.ProjectionItem.StableReference} must not own ordinary door state");
				}
			}

			foreach (var cargo in spec.CargoSpaces)
			{
				if (cargo.CompartmentKey is not null && !compartmentKeys.Contains(cargo.CompartmentKey))
				{
					issues.Add($"{spec.StableReference}: cargo {cargo.Key} references missing compartment {cargo.CompartmentKey}");
				}
				if (cargo.RequiredAccessPointKey is not null && !accessKeys.Contains(cargo.RequiredAccessPointKey))
				{
					issues.Add($"{spec.StableReference}: cargo {cargo.Key} references missing access {cargo.RequiredAccessPointKey}");
				}
				var containerCount = cargo.ProjectionItem.Components.Count(IsOrdinaryVehicleCargoContainerComponent);
				if (containerCount != 1)
				{
					issues.Add($"{spec.StableReference}: cargo projection {cargo.ProjectionItem.StableReference} requires exactly one ordinary container component");
				}
			}

			foreach (var installation in spec.InstallationPoints)
			{
				if (installation.RequiredAccessPointKey is not null && !accessKeys.Contains(installation.RequiredAccessPointKey))
				{
					issues.Add($"{spec.StableReference}: installation {installation.Key} references missing access {installation.RequiredAccessPointKey}");
				}
				if (string.IsNullOrWhiteSpace(installation.MountType))
				{
					issues.Add($"{spec.StableReference}: installation {installation.Key} requires a mount type");
				}
			}

			foreach (var tow in spec.TowPoints)
			{
				if (tow.RequiredAccessPointKey is not null && !accessKeys.Contains(tow.RequiredAccessPointKey))
				{
					issues.Add($"{spec.StableReference}: tow point {tow.Key} references missing access {tow.RequiredAccessPointKey}");
				}
				if (!tow.CanTow && !tow.CanBeTowed)
				{
					issues.Add($"{spec.StableReference}: tow point {tow.Key} must tow, be towed, or both");
				}
				if (tow.MaximumTowedWeight < 0.0 || tow.CharacterPullMultiplier <= 0.0)
				{
					issues.Add($"{spec.StableReference}: tow point {tow.Key} has invalid weight or pull multiplier");
				}
				foreach (var ratio in new[] { tow.TowStressWarningRatio, tow.TowStressFailureStartRatio, tow.TowStressMaximumFailureChance })
				{
					if (ratio is < 0.0 or > 1.0)
					{
						issues.Add($"{spec.StableReference}: tow point {tow.Key} has a stress ratio outside 0.0-1.0");
					}
				}
				if (tow.TowStressDamageMultiplier is < 0.0)
				{
					issues.Add($"{spec.StableReference}: tow point {tow.Key} has a negative stress damage multiplier");
				}
			}

			foreach (var zone in spec.DamageZones)
			{
				if (zone.MaximumDamage <= 0.0 || zone.HitWeight <= 0.0 || zone.DisabledThreshold <= 0.0 ||
				    zone.DestroyedThreshold < zone.DisabledThreshold)
				{
					issues.Add($"{spec.StableReference}: damage zone {zone.Key} has invalid thresholds");
				}
				foreach (var effect in zone.Effects)
				{
					var valid = effect.TargetType switch
					{
						VehicleDamageEffectTargetType.WholeVehicleMovement => effect.TargetKey is null,
						VehicleDamageEffectTargetType.MovementProfile => effect.TargetKey is null || movementKeys.Contains(effect.TargetKey),
						VehicleDamageEffectTargetType.AccessPoint => effect.TargetKey is null || accessKeys.Contains(effect.TargetKey),
						VehicleDamageEffectTargetType.CargoSpace => effect.TargetKey is null || cargoKeys.Contains(effect.TargetKey),
						VehicleDamageEffectTargetType.InstallationPoint => effect.TargetKey is null || installationKeys.Contains(effect.TargetKey),
						VehicleDamageEffectTargetType.TowPoint => effect.TargetKey is null || towKeys.Contains(effect.TargetKey),
						_ => false
					};
					if (!valid)
					{
						issues.Add($"{spec.StableReference}: damage zone {zone.Key} has an invalid {effect.TargetType} target {effect.TargetKey ?? "all"}");
					}
				}
			}
		}

		return issues.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
	}

	private void ValidateVehicleProjectionItem(
		string vehicleReference,
		string role,
		VehicleProjectionItemSeedSpec item,
		ICollection<string> issues)
	{
		if (!StableVehicleReferenceRegex.IsMatch(item.StableReference))
		{
			issues.Add($"{vehicleReference}: {role} item stable reference {item.StableReference} is not lowercase snake case");
		}
		if (string.IsNullOrWhiteSpace(item.Noun) || string.IsNullOrWhiteSpace(item.ShortDescription) ||
		    string.IsNullOrWhiteSpace(item.FullDescription))
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} has incomplete player-facing text");
		}
		if (!double.IsFinite(item.WeightInGrams) || item.WeightInGrams <= 0.0 || item.Cost < 0m)
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} has invalid weight or cost");
		}
		if (!_materials.ContainsKey(item.Material))
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} references missing material {item.Material}");
		}
		foreach (var tag in item.Tags.Where(x => !_tagsByFullPath.ContainsKey(x)))
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} references missing tag {tag}");
		}
		foreach (var component in item.Components.Where(x => !_components.ContainsKey(x)))
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} references missing component {component}");
		}
		if (item.Components.Contains("Holdable", StringComparer.OrdinalIgnoreCase))
		{
			issues.Add($"{vehicleReference}: {role} projection item {item.StableReference} must omit Holdable");
		}
		var destroyableCount = item.Components.Count(x => x.StartsWith("Destroyable_", StringComparison.Ordinal));
		if (destroyableCount != 1)
		{
			issues.Add($"{vehicleReference}: {role} item {item.StableReference} requires exactly one destroyable component");
		}
	}

	private static IEnumerable<string> AllVehicleStableReferences(VehiclePrototypeSeedSpec spec)
	{
		yield return spec.StableReference;
		yield return spec.ExteriorItem.StableReference;
		foreach (var item in spec.AccessPoints.Select(x => x.ProjectionItem.StableReference))
		{
			yield return item;
		}
		foreach (var item in spec.CargoSpaces.Select(x => x.ProjectionItem.StableReference))
		{
			yield return item;
		}
	}

	private static void ValidateUniqueVehicleKeys(string stableReference, string kind, IEnumerable<string> keys, ICollection<string> issues)
	{
		foreach (var duplicate in keys.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
		{
			issues.Add($"{stableReference}: duplicate {kind} key {duplicate.Key}");
		}
	}

	private static bool IsOrdinaryVehicleCargoContainerComponent(string componentName)
	{
		return componentName.StartsWith("Container_", StringComparison.Ordinal) ||
		       componentName.StartsWith("LockingContainer_", StringComparison.Ordinal);
	}

}
