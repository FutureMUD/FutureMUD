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
	private Dictionary<string, VehicleMovementProfileProto> UpsertVehicleMovementProfiles(
		VehicleProto vehicle,
		IReadOnlyList<VehicleMovementProfileSeedSpec> specs)
	{
		var existing = _context!.VehicleMovementProfileProtos
			.Include(x => x.PropulsionProfiles)
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleMovementProfileProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "movement profile");
			if (row is null)
			{
				row = new VehicleMovementProfileProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber,
					PropulsionProfiles = new HashSet<VehiclePropulsionProfileProto>()
				};
				_context.VehicleMovementProfileProtos.Add(row);
				existing.Add(row);
			}

			row.Name = spec.Name;
			row.MovementType = (int)spec.MovementType;
			row.MovementEnvironment = (int)spec.MovementEnvironment;
			row.ExposesOccupantsToWater = spec.ExposesOccupantsToWater;
			row.IsDefault = spec.IsDefault;
			row.RequiredPowerSpikeInWatts = spec.RequiredPowerSpikeInWatts;
			row.FuelLiquidId = ResolveVehicleFuelLiquidId(spec.FuelLiquidName, vehicle, spec);
			row.FuelVolumePerMove = row.FuelLiquidId is null ? 0.0 : spec.FuelVolumePerMove;
			row.RequiredInstalledRole = spec.RequiredInstalledRole;
			row.RequiresTowLinksClosed = spec.RequiresTowLinksClosed;
			row.RequiresAccessPointsClosed = spec.RequiresAccessPointsClosed;
			row.RouteSpeedMetresPerSecond = spec.RouteSpeedMetresPerSecond;
			row.RoutePropulsionMode = (int)spec.RoutePropulsionMode;
			row.RouteFuelVolumePerMetre = spec.RouteFuelVolumePerMetre;
			row.RoutePowerDrawWatts = spec.RoutePowerDrawWatts;
			row.AutomaticOperationCapable = spec.AutomaticOperationCapable;
			_context.SaveChanges();

			UpsertVehiclePropulsionProfiles(vehicle, row, spec.PropulsionProfiles);
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private void UpsertVehiclePropulsionProfiles(
		VehicleProto vehicle,
		VehicleMovementProfileProto movement,
		IReadOnlyList<VehiclePropulsionSeedSpec> specs)
	{
		var existing = _context!.VehiclePropulsionProfileProtos
			.Where(x => x.VehicleMovementProfileProtoId == movement.Id)
			.AsEnumerable()
			.ToList();
		foreach (var spec in specs)
		{
			var matches = existing.Where(x => x.PropulsionType == (int)spec.PropulsionType).ToArray();
			if (matches.Length > 1)
			{
				throw new InvalidOperationException(
					$"Vehicle {vehicle.Name} movement profile {movement.Name} has duplicate {spec.PropulsionType} propulsion rows.");
			}

			var row = matches.SingleOrDefault();
			if (row is null)
			{
				row = new VehiclePropulsionProfileProto
				{
					VehicleMovementProfileProtoId = movement.Id,
					PropulsionType = (int)spec.PropulsionType
				};
				_context.VehiclePropulsionProfileProtos.Add(row);
				existing.Add(row);
			}

			row.IsDefault = spec.IsDefault;
			row.BaseMoveTimeMilliseconds = spec.BaseMoveTimeMilliseconds;
			row.PropulsionTraitDefinitionId = ResolveVehiclePropulsionTraitId(spec.PropulsionTraitName, vehicle, movement, spec);
			row.CheckDifficulty = (int)spec.CheckDifficulty;
			row.SpeedMultiplierExpression = spec.SpeedMultiplierExpression;
			row.StaminaCostExpression = spec.StaminaCostExpression;
		}

		_context.SaveChanges();
	}

	private Dictionary<string, VehicleAccessPointProto> UpsertVehicleAccessPoints(
		VehicleProto vehicle,
		IReadOnlyList<VehicleAccessPointSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var existing = _context!.VehicleAccessPointProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleAccessPointProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "access point");
			if (row is null)
			{
				row = new VehicleAccessPointProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleAccessPointProtos.Add(row);
				existing.Add(row);
			}

			row.VehicleCompartmentProtoId = spec.CompartmentKey is null ? null : compartments[spec.CompartmentKey].Id;
			row.Name = spec.Name;
			row.Description = spec.Description;
			row.AccessPointType = (int)spec.AccessPointType;
			row.StartsOpen = spec.StartsOpen;
			row.MustBeClosedForMovement = spec.MustBeClosedForMovement;
			row.DisplayOrder = spec.DisplayOrder;
			_context.SaveChanges();

			var component = EnsureVehicleComponentPrototype(
				$"Vehicle Access - {spec.ProjectionItem.StableReference}",
				VehicleAccessComponentType,
				$"Vehicle access point projection for {vehicle.Name}: {spec.Name}",
				new XElement("Definition",
					new XElement("VehiclePrototypeId", vehicle.Id),
					new XElement("AccessPointPrototypeId", row.Id)).ToString());
			var item = EnsureVehicleProjectionItem(spec.ProjectionItem, component);
			row.ProjectionItemProtoId = item.Id;
			row.ProjectionItemProtoRevision = item.RevisionNumber;
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private Dictionary<string, VehicleCargoSpaceProto> UpsertVehicleCargoSpaces(
		VehicleProto vehicle,
		IReadOnlyList<VehicleCargoSpaceSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments,
		IReadOnlyDictionary<string, VehicleAccessPointProto> accesses)
	{
		var existing = _context!.VehicleCargoSpaceProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleCargoSpaceProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "cargo space");
			if (row is null)
			{
				row = new VehicleCargoSpaceProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleCargoSpaceProtos.Add(row);
				existing.Add(row);
			}

			row.VehicleCompartmentProtoId = spec.CompartmentKey is null ? null : compartments[spec.CompartmentKey].Id;
			row.RequiredAccessPointProtoId = spec.RequiredAccessPointKey is null ? null : accesses[spec.RequiredAccessPointKey].Id;
			row.Name = spec.Name;
			row.Description = spec.Description;
			row.DisplayOrder = spec.DisplayOrder;
			_context.SaveChanges();

			var component = EnsureVehicleComponentPrototype(
				$"Vehicle Cargo - {spec.ProjectionItem.StableReference}",
				VehicleCargoComponentType,
				$"Vehicle cargo projection for {vehicle.Name}: {spec.Name}",
				new XElement("Definition",
					new XElement("VehiclePrototypeId", vehicle.Id),
					new XElement("CargoSpacePrototypeId", row.Id)).ToString());
			var item = EnsureVehicleProjectionItem(spec.ProjectionItem, component);
			row.ProjectionItemProtoId = item.Id;
			row.ProjectionItemProtoRevision = item.RevisionNumber;
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

}
