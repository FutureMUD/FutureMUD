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
	private static void ValidateVehicleSeedSpec(VehicleSeedSpec spec)
	{
		if (!Regex.IsMatch(spec.StableReference, "^vehicle_(antiquity|medieval|renaissance|earlymodern|revolution|modern|atomic|computer)_[a-z0-9_]+$"))
		{
			throw VehicleValidation(spec, $"stable reference '{spec.StableReference}' must be lowercase underscore notation and begin with its era token");
		}
		if (!VehicleEraTags.ContainsKey(spec.EraKey) ||
		    !spec.StableReference.StartsWith($"vehicle_{spec.EraKey}_", StringComparison.OrdinalIgnoreCase))
		{
			throw VehicleValidation(spec, $"era key '{spec.EraKey}' does not agree with the stable reference");
		}
		if (!spec.Domain.Equals(VehicleDomainTerrestrial, StringComparison.OrdinalIgnoreCase) &&
		    !spec.Domain.Equals(VehicleDomainAquatic, StringComparison.OrdinalIgnoreCase))
		{
			throw VehicleValidation(spec, $"domain must be {VehicleDomainTerrestrial} or {VehicleDomainAquatic}");
		}
		RequireText(spec, spec.Name, "name");
		RequireText(spec, spec.Description, "description");
		RequireText(spec, spec.Archetype, "archetype");
		ValidateVehicleItem(spec, spec.ExteriorItem, "exterior item", projection: false);
		ValidateUniqueKeys(spec, spec.Compartments.Select(x => x.Key), "compartment");
		ValidateUniqueKeys(spec, spec.CompartmentLinks.Select(x => x.Key), "compartment link");
		ValidateUniqueKeys(spec, spec.OccupantSlots.Select(x => x.Key), "occupant slot");
		ValidateUniqueKeys(spec, spec.ControlStations.Select(x => x.Key), "control station");
		ValidateUniqueKeys(spec, spec.MovementProfiles.Select(x => x.Key), "movement profile");
		ValidateUniqueKeys(spec, spec.AccessPoints.Select(x => x.Key), "access point");
		ValidateUniqueKeys(spec, spec.CargoSpaces.Select(x => x.Key), "cargo space");
		ValidateUniqueKeys(spec, spec.InstallationPoints.Select(x => x.Key), "installation point");
		ValidateUniqueKeys(spec, spec.TowPoints.Select(x => x.Key), "tow point");
		ValidateUniqueKeys(spec, spec.DamageZones.Select(x => x.Key), "damage zone");

		if (spec.Compartments.Count == 0)
		{
			throw VehicleValidation(spec, "at least one compartment is required");
		}
		if (spec.Scale == VehicleScale.RoomScale && spec.Compartments.Any(x => x.InteriorTerrainId is null))
		{
			throw VehicleValidation(spec, "every RoomScale compartment requires an interior terrain id");
		}
		ValidateUniqueIntegers(spec, spec.Compartments.Select(x => x.DisplayOrder), "compartment display order");
		foreach (var compartment in spec.Compartments)
		{
			RequireText(spec, compartment.Name, $"compartment {compartment.Key} name");
			RequireText(spec, compartment.Description, $"compartment {compartment.Key} description");
			if (compartment.InteriorTerrainId is <= 0)
			{
				throw VehicleValidation(spec, $"compartment {compartment.Key} has an invalid interior terrain id");
			}
		}

		var compartmentKeys = spec.Compartments.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var link in spec.CompartmentLinks)
		{
			RequireReference(spec, compartmentKeys, link.SourceCompartmentKey, $"link {link.Key} source compartment");
			RequireReference(spec, compartmentKeys, link.DestinationCompartmentKey, $"link {link.Key} destination compartment");
			if (link.SourceCompartmentKey.Equals(link.DestinationCompartmentKey, StringComparison.OrdinalIgnoreCase))
			{
				throw VehicleValidation(spec, $"link {link.Key} cannot connect a compartment to itself");
			}
			RequireText(spec, link.OutboundDirection, $"link {link.Key} outbound direction");
			RequireText(spec, link.InboundDirection, $"link {link.Key} inbound direction");
			RequireText(spec, link.OutboundDescription, $"link {link.Key} outbound description");
			RequireText(spec, link.InboundDescription, $"link {link.Key} inbound description");
		}

		if (!spec.OccupantSlots.Any(x => x.SlotType == VehicleOccupantSlotType.Driver))
		{
			throw VehicleValidation(spec, "at least one driver slot is required");
		}
		foreach (var slot in spec.OccupantSlots)
		{
			RequireReference(spec, compartmentKeys, slot.CompartmentKey, $"slot {slot.Key} compartment");
			RequireText(spec, slot.Name, $"slot {slot.Key} name");
			if (slot.Capacity <= 0)
			{
				throw VehicleValidation(spec, $"slot {slot.Key} capacity must be positive");
			}
			if (slot.SlotType == VehicleOccupantSlotType.Driver && slot.Capacity != 1)
			{
				throw VehicleValidation(spec, $"driver slot {slot.Key} must have capacity one");
			}
		}

		var slotKeys = spec.OccupantSlots.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (spec.ControlStations.Count(x => x.IsPrimary) != 1)
		{
			throw VehicleValidation(spec, "exactly one primary control station is required");
		}
		foreach (var station in spec.ControlStations)
		{
			RequireReference(spec, slotKeys, station.OccupantSlotKey, $"control station {station.Key} occupant slot");
			RequireText(spec, station.Name, $"control station {station.Key} name");
			var slot = spec.OccupantSlots.First(x => x.Key.Equals(station.OccupantSlotKey, StringComparison.OrdinalIgnoreCase));
			if (station.IsPrimary && slot.SlotType != VehicleOccupantSlotType.Driver)
			{
				throw VehicleValidation(spec, $"primary control station {station.Key} must belong to a driver slot");
			}
		}

		if (spec.MovementProfiles.Count == 0 || spec.MovementProfiles.Count(x => x.IsDefault) != 1)
		{
			throw VehicleValidation(spec, "at least one movement profile and exactly one default movement profile are required");
		}
		var installationKeys = spec.InstallationPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var movement in spec.MovementProfiles)
		{
			ValidateMovementProfile(spec, movement, installationKeys);
		}

		ValidateUniqueIntegers(spec, spec.AccessPoints.Select(x => x.DisplayOrder), "access-point display order");
		foreach (var access in spec.AccessPoints)
		{
			if (!string.IsNullOrWhiteSpace(access.CompartmentKey))
			{
				RequireReference(spec, compartmentKeys, access.CompartmentKey, $"access point {access.Key} compartment");
			}
			RequireText(spec, access.Name, $"access point {access.Key} name");
			RequireText(spec, access.Description, $"access point {access.Key} description");
			ValidateVehicleItem(spec, access.ProjectionItem, $"access point {access.Key} projection", projection: true);
		}

		var accessKeys = spec.AccessPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		ValidateUniqueIntegers(spec, spec.CargoSpaces.Select(x => x.DisplayOrder), "cargo-space display order");
		foreach (var cargo in spec.CargoSpaces)
		{
			if (!string.IsNullOrWhiteSpace(cargo.CompartmentKey))
			{
				RequireReference(spec, compartmentKeys, cargo.CompartmentKey, $"cargo space {cargo.Key} compartment");
			}
			if (!string.IsNullOrWhiteSpace(cargo.RequiredAccessPointKey))
			{
				RequireReference(spec, accessKeys, cargo.RequiredAccessPointKey, $"cargo space {cargo.Key} required access point");
			}
			RequireText(spec, cargo.Name, $"cargo space {cargo.Key} name");
			RequireText(spec, cargo.Description, $"cargo space {cargo.Key} description");
			RequireText(spec, cargo.ContainerComponent, $"cargo space {cargo.Key} container component");
			ValidateVehicleItem(spec, cargo.ProjectionItem, $"cargo space {cargo.Key} projection", projection: true);
		}
		if (spec.ProvidesCargoService && spec.CargoSpaces.Count == 0)
		{
			throw VehicleValidation(spec, "a vehicle marked as providing cargo service must define a cargo space");
		}
		if (spec.ProvidesPassengerService && !spec.OccupantSlots.Any(x => x.SlotType == VehicleOccupantSlotType.Passenger || x.SlotType == VehicleOccupantSlotType.Crew))
		{
			throw VehicleValidation(spec, "a vehicle marked as providing passenger service must define passenger or crew capacity");
		}

		ValidateUniqueIntegers(spec, spec.InstallationPoints.Select(x => x.DisplayOrder), "installation-point display order");
		foreach (var point in spec.InstallationPoints)
		{
			if (!string.IsNullOrWhiteSpace(point.RequiredAccessPointKey))
			{
				RequireReference(spec, accessKeys, point.RequiredAccessPointKey, $"installation point {point.Key} required access point");
			}
			RequireText(spec, point.Name, $"installation point {point.Key} name");
			RequireText(spec, point.Description, $"installation point {point.Key} description");
			RequireText(spec, point.MountType, $"installation point {point.Key} mount type");
			if (point.RequiredForMovement && string.IsNullOrWhiteSpace(point.RequiredRole))
			{
				throw VehicleValidation(spec, $"movement-required installation point {point.Key} must declare a required role");
			}
		}

		ValidateUniqueIntegers(spec, spec.TowPoints.Select(x => x.DisplayOrder), "tow-point display order");
		foreach (var point in spec.TowPoints)
		{
			if (!string.IsNullOrWhiteSpace(point.RequiredAccessPointKey))
			{
				RequireReference(spec, accessKeys, point.RequiredAccessPointKey, $"tow point {point.Key} required access point");
			}
			RequireText(spec, point.Name, $"tow point {point.Key} name");
			RequireText(spec, point.Description, $"tow point {point.Key} description");
			RequireText(spec, point.TowType, $"tow point {point.Key} tow type");
			if (!point.CanTow && !point.CanBeTowed)
			{
				throw VehicleValidation(spec, $"tow point {point.Key} must either tow or be towable");
			}
			RequirePositiveFinite(spec, point.MaximumTowedWeight, $"tow point {point.Key} maximum towed weight");
			RequirePositiveFinite(spec, point.CharacterPullMultiplier, $"tow point {point.Key} character pull multiplier");
			ValidateTowStress(spec, point);
		}

		ValidateUniqueIntegers(spec, spec.DamageZones.Select(x => x.DisplayOrder), "damage-zone display order");
		if (spec.DamageZones.Count == 0)
		{
			throw VehicleValidation(spec, "at least one damage zone is required");
		}
		var movementKeys = spec.MovementProfiles.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var cargoKeys = spec.CargoSpaces.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var towKeys = spec.TowPoints.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var zone in spec.DamageZones)
		{
			RequireText(spec, zone.Name, $"damage zone {zone.Key} name");
			RequireText(spec, zone.Description, $"damage zone {zone.Key} description");
			RequirePositiveFinite(spec, zone.MaximumDamage, $"damage zone {zone.Key} maximum damage");
			RequirePositiveFinite(spec, zone.HitWeight, $"damage zone {zone.Key} hit weight");
			if (!double.IsFinite(zone.DisabledThreshold) || !double.IsFinite(zone.DestroyedThreshold) ||
			    zone.DisabledThreshold <= 0.0 || zone.DisabledThreshold >= zone.DestroyedThreshold || zone.DestroyedThreshold > 1.0)
			{
				throw VehicleValidation(spec, $"damage zone {zone.Key} thresholds must satisfy 0 < disabled < destroyed <= 1");
			}
			var effectKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var effect in zone.Effects)
			{
				ValidateDamageEffectReference(spec, zone, effect, movementKeys, accessKeys, cargoKeys, installationKeys, towKeys);
				var identity = $"{effect.TargetType}:{effect.TargetKey ?? "<vehicle>"}";
				if (!effectKeys.Add(identity))
				{
					throw VehicleValidation(spec, $"damage zone {zone.Key} repeats effect target {identity}");
				}
			}
		}
	}

}
