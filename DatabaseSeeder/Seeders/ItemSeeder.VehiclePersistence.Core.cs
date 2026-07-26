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
	private void SeedVehiclePrototype(VehiclePrototypeSeedSpec spec)
	{
		var vehicle = EnsureVehiclePrototype(spec);
		_context!.SaveChanges();

		var exteriorComponent = EnsureVehicleComponentPrototype(
			$"Vehicle Exterior - {spec.StableReference}",
			VehicleExteriorComponentType,
			$"Vehicle exterior projection for {spec.Name}",
			new XElement("Definition", new XElement("VehiclePrototypeId", vehicle.Id)).ToString());
		var exterior = EnsureVehicleProjectionItem(spec.ExteriorItem, exteriorComponent);
		vehicle.ExteriorItemProtoId = exterior.Id;
		vehicle.ExteriorItemProtoRevision = exterior.RevisionNumber;
		_context.SaveChanges();

		var compartments = UpsertVehicleCompartments(vehicle, spec.Compartments);
		UpsertVehicleCompartmentLinks(vehicle, spec.CompartmentLinks, compartments);
		var slots = UpsertVehicleOccupantSlots(vehicle, spec.OccupantSlots, compartments);
		UpsertVehicleControlStations(vehicle, spec.ControlStations, slots);
		var movements = UpsertVehicleMovementProfiles(vehicle, spec.MovementProfiles);
		var accesses = UpsertVehicleAccessPoints(vehicle, spec.AccessPoints, compartments);
		var cargos = UpsertVehicleCargoSpaces(vehicle, spec.CargoSpaces, compartments, accesses);
		var installations = UpsertVehicleInstallationPoints(vehicle, spec.InstallationPoints, accesses);
		var towPoints = UpsertVehicleTowPoints(vehicle, spec.TowPoints, accesses);
		UpsertVehicleDamageZones(vehicle, spec.DamageZones, movements, accesses, cargos, installations, towPoints);
		_context.SaveChanges();
	}

	private VehicleProto EnsureVehiclePrototype(VehiclePrototypeSeedSpec spec)
	{
		var marker = VehicleSeederMarker(spec.StableReference);
		var all = _context!.VehicleProtos
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.ToList();
		var owned = all
			.Where(x => ContainsVehicleSeederMarker(x.Description, marker) ||
			            ContainsVehicleSeederMarker(x.EditableItem?.BuilderComment, marker))
			.ToArray();
		var ownedIds = owned.Select(x => x.Id).Distinct().ToArray();
		if (ownedIds.Length > 1)
		{
			throw new InvalidOperationException(
				$"Multiple vehicle prototype IDs claim stock stable reference {spec.StableReference}.");
		}

		var currentOwned = owned
			.Where(x => x.EditableItem?.RevisionStatus == 4)
			.OrderByDescending(x => x.RevisionNumber)
			.FirstOrDefault() ?? owned.OrderByDescending(x => x.RevisionNumber).FirstOrDefault();
		var nameCollision = all.FirstOrDefault(x =>
			x.EditableItem?.RevisionStatus == 4 &&
			x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase) &&
			(currentOwned is null || x.Id != currentOwned.Id));
		if (nameCollision is not null)
		{
			throw new InvalidOperationException(
				$"Cannot seed stock vehicle {spec.StableReference} because a builder-authored current prototype already uses the name {spec.Name}.");
		}

		if (currentOwned is null)
		{
			currentOwned = new VehicleProto
			{
				Id = NextVehiclePrototypeId(),
				RevisionNumber = 0,
				EditableItem = NewCurrentVehicleEditableItem(marker),
				Name = spec.Name,
				Description = WithVehicleSeederMarker(spec.Description, marker),
				VehicleScale = (int)spec.Scale
			};
			_context.VehicleProtos.Add(currentOwned);
		}
		else
		{
			currentOwned.Name = spec.Name;
			currentOwned.Description = WithVehicleSeederMarker(spec.Description, marker);
			currentOwned.VehicleScale = (int)spec.Scale;
			currentOwned.EditableItem.BuilderComment = marker;
			currentOwned.EditableItem.BuilderDate = _now;
			currentOwned.EditableItem.RevisionStatus = 4;
			currentOwned.EditableItem.ReviewerAccountId = _dbAccount.Id;
			currentOwned.EditableItem.ReviewerComment = "Reconciled by the item seeder vehicle catalogue.";
			currentOwned.EditableItem.ReviewerDate = _now;
		}

		return currentOwned;
	}

	private Dictionary<string, VehicleCompartmentProto> UpsertVehicleCompartments(
		VehicleProto vehicle,
		IReadOnlyList<VehicleCompartmentSeedSpec> specs)
	{
		var existing = _context!.VehicleCompartmentProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleCompartmentProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "compartment");
			if (row is null)
			{
				row = new VehicleCompartmentProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleCompartmentProtos.Add(row);
				existing.Add(row);
			}

			row.Name = spec.Name;
			row.Description = spec.Description;
			row.DisplayOrder = spec.DisplayOrder;
			row.InteriorTerrainId = ResolveVehicleInteriorTerrainId(spec.InteriorTerrainName, vehicle, spec);
			row.InteriorOutdoorsType = spec.InteriorOutdoorsType;
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private void UpsertVehicleCompartmentLinks(
		VehicleProto vehicle,
		IReadOnlyList<VehicleCompartmentLinkSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var existing = _context!.VehicleCompartmentLinkProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		foreach (var spec in specs)
		{
			var source = compartments[spec.SourceCompartmentKey];
			var destination = compartments[spec.DestinationCompartmentKey];
			var matches = existing.Where(x =>
				x.SourceVehicleCompartmentProtoId == source.Id &&
				x.DestinationVehicleCompartmentProtoId == destination.Id &&
				x.OutboundDirection.Equals(spec.OutboundDirection, StringComparison.OrdinalIgnoreCase)).ToArray();
			if (matches.Length > 1)
			{
				throw new InvalidOperationException($"Vehicle {vehicle.Name} has duplicate compartment links for {spec.OutboundDirection}.");
			}

			var row = matches.SingleOrDefault();
			if (row is null)
			{
				row = new VehicleCompartmentLinkProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber,
					SourceVehicleCompartmentProtoId = source.Id,
					DestinationVehicleCompartmentProtoId = destination.Id
				};
				_context.VehicleCompartmentLinkProtos.Add(row);
				existing.Add(row);
			}

			row.OutboundDirection = spec.OutboundDirection;
			row.InboundDirection = spec.InboundDirection;
			row.OutboundDescription = spec.OutboundDescription;
			row.InboundDescription = spec.InboundDescription;
		}

		_context.SaveChanges();
	}

	private Dictionary<string, VehicleOccupantSlotProto> UpsertVehicleOccupantSlots(
		VehicleProto vehicle,
		IReadOnlyList<VehicleOccupantSlotSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var existing = _context!.VehicleOccupantSlotProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		var result = new Dictionary<string, VehicleOccupantSlotProto>(StringComparer.OrdinalIgnoreCase);
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "occupant slot");
			if (row is null)
			{
				row = new VehicleOccupantSlotProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleOccupantSlotProtos.Add(row);
				existing.Add(row);
			}

			row.VehicleCompartmentProtoId = compartments[spec.CompartmentKey].Id;
			row.Name = spec.Name;
			row.SlotType = (int)spec.SlotType;
			row.Capacity = spec.Capacity;
			row.RequiredForMovement = spec.RequiredForMovement;
			row.ContributesToPropulsion = spec.ContributesToPropulsion;
			row.SameLevelRangedCoverId = null;
			row.AboveRangedCoverId = null;
			row.BelowRangedCoverId = null;
			row.BoatStabilityDifficulty = (int)spec.BoatStabilityDifficulty;
			result.Add(spec.Key, row);
		}

		_context.SaveChanges();
		return result;
	}

	private void UpsertVehicleControlStations(
		VehicleProto vehicle,
		IReadOnlyList<VehicleControlStationSeedSpec> specs,
		IReadOnlyDictionary<string, VehicleOccupantSlotProto> slots)
	{
		var existing = _context!.VehicleControlStationProtos
			.Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			.AsEnumerable()
			.ToList();
		foreach (var spec in specs)
		{
			var row = SingleVehicleChild(existing, x => x.Name, spec.Name, vehicle, "control station");
			if (row is null)
			{
				row = new VehicleControlStationProto
				{
					VehicleProtoId = vehicle.Id,
					VehicleProtoRevision = vehicle.RevisionNumber
				};
				_context.VehicleControlStationProtos.Add(row);
				existing.Add(row);
			}

			row.VehicleOccupantSlotProtoId = slots[spec.SlotKey].Id;
			row.Name = spec.Name;
			row.IsPrimary = spec.IsPrimary;
		}

		_context.SaveChanges();
	}

}
