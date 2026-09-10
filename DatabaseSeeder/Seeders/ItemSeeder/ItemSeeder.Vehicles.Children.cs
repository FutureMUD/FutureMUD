#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private readonly Dictionary<Type, long> _nextVehicleChildIds = [];

	private VehicleProto? FindVehiclePrototypeByExterior(GameItemProto exterior)
	{
		return _context!.VehicleProtos.Local.FirstOrDefault(x =>
				x.ExteriorItemProtoId == exterior.Id && x.ExteriorItemProtoRevision == exterior.RevisionNumber) ??
		       _context.VehicleProtos.FirstOrDefault(x =>
			       x.ExteriorItemProtoId == exterior.Id && x.ExteriorItemProtoRevision == exterior.RevisionNumber);
	}

	private long NextVehicleChildId<T>(DbSet<T> set, Func<T, long> selector) where T : class
	{
		if (!_nextVehicleChildIds.TryGetValue(typeof(T), out var nextId))
		{
			var databaseMaximum = set
				.AsNoTracking()
				.AsEnumerable()
				.Select(selector)
				.DefaultIfEmpty(0L)
				.Max();
			var localMaximum = set.Local
				.Select(selector)
				.DefaultIfEmpty(0L)
				.Max();
			nextId = Math.Max(databaseMaximum, localMaximum) + 1L;
		}

		_nextVehicleChildIds[typeof(T)] = nextId + 1L;
		return nextId;
	}

	private VehicleCompartmentProto UpsertCompartment(
		VehicleProto vehicle,
		string vehicleReference,
		VehicleCompartmentSeedSpec spec)
	{
		var row = _context!.VehicleCompartmentProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleCompartmentProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleCompartmentProto
			{
				Id = NextVehicleChildId(_context.VehicleCompartmentProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleCompartmentProtos.Add(row);
		}

		row.Name = spec.Name;
		row.Description = spec.Description;
		row.DisplayOrder = spec.DisplayOrder;
		row.InteriorTerrainId = spec.InteriorTerrainId;
		row.InteriorOutdoorsType = spec.InteriorOutdoorsType;
		return row;
	}

	private VehicleCompartmentLinkProto UpsertCompartmentLink(
		VehicleProto vehicle,
		VehicleCompartmentLinkSeedSpec spec,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var source = RequireKey(compartments, spec.SourceCompartmentKey, vehicle.Name, "compartment");
		var destination = RequireKey(compartments, spec.DestinationCompartmentKey, vehicle.Name, "compartment");
		var row = _context!.VehicleCompartmentLinkProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.SourceVehicleCompartmentProtoId == source.Id && x.DestinationVehicleCompartmentProtoId == destination.Id &&
				x.OutboundDirection.Equals(spec.OutboundDirection, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleCompartmentLinkProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			                      x.SourceVehicleCompartmentProtoId == source.Id &&
			                      x.DestinationVehicleCompartmentProtoId == destination.Id)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.OutboundDirection.Equals(spec.OutboundDirection, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleCompartmentLinkProto
			{
				Id = NextVehicleChildId(_context.VehicleCompartmentLinkProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleCompartmentLinkProtos.Add(row);
		}

		row.SourceVehicleCompartmentProto = source;
		row.SourceVehicleCompartmentProtoId = source.Id;
		row.DestinationVehicleCompartmentProto = destination;
		row.DestinationVehicleCompartmentProtoId = destination.Id;
		row.OutboundDirection = spec.OutboundDirection;
		row.InboundDirection = spec.InboundDirection;
		row.OutboundDescription = spec.OutboundDescription;
		row.InboundDescription = spec.InboundDescription;
		return row;
	}

	private VehicleOccupantSlotProto UpsertOccupantSlot(
		VehicleProto vehicle,
		VehicleOccupantSlotSeedSpec spec,
		IReadOnlyDictionary<string, VehicleCompartmentProto> compartments)
	{
		var compartment = RequireKey(compartments, spec.CompartmentKey, vehicle.Name, "compartment");
		var row = _context!.VehicleOccupantSlotProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.VehicleCompartmentProtoId == compartment.Id &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleOccupantSlotProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			                      x.VehicleCompartmentProtoId == compartment.Id)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleOccupantSlotProto
			{
				Id = NextVehicleChildId(_context.VehicleOccupantSlotProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleOccupantSlotProtos.Add(row);
		}

		row.VehicleCompartmentProto = compartment;
		row.VehicleCompartmentProtoId = compartment.Id;
		row.Name = spec.Name;
		row.SlotType = (int)spec.SlotType;
		row.Capacity = spec.Capacity;
		row.RequiredForMovement = spec.RequiredForMovement;
		row.ContributesToPropulsion = spec.ContributesToPropulsion;
		row.BoatStabilityDifficulty = (int)spec.BoatStabilityDifficulty;
		return row;
	}

	private VehicleControlStationProto UpsertControlStation(
		VehicleProto vehicle,
		VehicleControlStationSeedSpec spec,
		IReadOnlyDictionary<string, VehicleOccupantSlotProto> slots)
	{
		var slot = RequireKey(slots, spec.OccupantSlotKey, vehicle.Name, "occupant slot");
		var row = _context!.VehicleControlStationProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.VehicleOccupantSlotProtoId == slot.Id &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleControlStationProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
			                      x.VehicleOccupantSlotProtoId == slot.Id)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleControlStationProto
			{
				Id = NextVehicleChildId(_context.VehicleControlStationProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleControlStationProtos.Add(row);
		}

		row.VehicleOccupantSlotProto = slot;
		row.VehicleOccupantSlotProtoId = slot.Id;
		row.Name = spec.Name;
		row.IsPrimary = spec.IsPrimary;
		return row;
	}

	private VehicleMovementProfileProto UpsertMovementProfile(
		VehicleProto vehicle,
		VehicleMovementProfileSeedSpec spec)
	{
		var row = _context!.VehicleMovementProfileProtos.Local.FirstOrDefault(x =>
				x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber &&
				x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)) ??
		          _context.VehicleMovementProfileProtos
			          .Where(x => x.VehicleProtoId == vehicle.Id && x.VehicleProtoRevision == vehicle.RevisionNumber)
			          .AsEnumerable().FirstOrDefault(x =>
			          x.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
		if (row is null)
		{
			row = new VehicleMovementProfileProto
			{
				Id = NextVehicleChildId(_context.VehicleMovementProfileProtos, x => x.Id),
				VehicleProto = vehicle,
				VehicleProtoId = vehicle.Id,
				VehicleProtoRevision = vehicle.RevisionNumber
			};
			_context.VehicleMovementProfileProtos.Add(row);
		}

		row.Name = spec.Name;
		row.MovementType = (int)spec.MovementType;
		row.MovementEnvironment = (int)spec.Environment;
		row.ExposesOccupantsToWater = spec.ExposesOccupantsToWater;
		row.IsDefault = spec.IsDefault;
		row.RequiredPowerSpikeInWatts = spec.RequiredPowerSpikeInWatts;
		row.MinimumEnginePowerInWatts = spec.MinimumEnginePowerInWatts;
		row.FuelLiquidId = string.IsNullOrWhiteSpace(spec.FuelLiquid)
			? null
			: _liquids.TryGetValue(spec.FuelLiquid, out var liquid)
				? liquid.Id
				: throw new InvalidOperationException($"Vehicle movement profile {vehicle.Name}/{spec.Name} uses unknown liquid {spec.FuelLiquid}.");
		row.FuelVolumePerMove = spec.FuelVolumePerMove;
		row.RequiredInstalledRole = spec.RequiredInstalledRole;
		row.RequiresTowLinksClosed = spec.RequiresTowLinksClosed;
		row.RequiresAccessPointsClosed = spec.RequiresAccessPointsClosed;
		row.RouteSpeedMetresPerSecond = spec.RouteSpeedMetresPerSecond;
		row.RoutePropulsionMode = (int)spec.RoutePropulsionMode;
		row.RouteFuelVolumePerMetre = spec.RouteFuelVolumePerMetre;
		row.RoutePowerDrawWatts = spec.RoutePowerDrawWatts;
		row.AutomaticOperationCapable = spec.AutomaticOperationCapable;
		return row;
	}

	private void UpsertPropulsionProfile(
		VehicleMovementProfileProto movement,
		VehiclePropulsionSeedSpec spec)
	{
		var row = _context!.VehiclePropulsionProfileProtos.Local.FirstOrDefault(x =>
				x.VehicleMovementProfileProtoId == movement.Id && x.PropulsionType == (int)spec.PropulsionType) ??
		          _context.VehiclePropulsionProfileProtos.FirstOrDefault(x =>
			          x.VehicleMovementProfileProtoId == movement.Id && x.PropulsionType == (int)spec.PropulsionType);
		if (row is null)
		{
			row = new VehiclePropulsionProfileProto
			{
				Id = NextVehicleChildId(_context.VehiclePropulsionProfileProtos, x => x.Id),
				VehicleMovementProfileProto = movement,
				VehicleMovementProfileProtoId = movement.Id
			};
			_context.VehiclePropulsionProfileProtos.Add(row);
		}

		var trait = spec.TraitCandidates
			.Select(candidate => _traits.TryGetValue(candidate, out var definition) ? definition : null)
			.FirstOrDefault(x => x is not null);
		if ((spec.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed) && trait is null)
		{
			throw new InvalidOperationException(
				$"Vehicle propulsion profile {movement.Name}/{spec.PropulsionType} requires one of these traits: {string.Join(", ", spec.TraitCandidates)}.");
		}

		row.PropulsionType = (int)spec.PropulsionType;
		row.IsDefault = spec.IsDefault;
		row.BaseMoveTimeMilliseconds = spec.BaseMoveTimeMilliseconds;
		row.PropulsionTraitDefinition = trait;
		row.PropulsionTraitDefinitionId = trait?.Id;
		row.CheckDifficulty = (int)spec.CheckDifficulty;
		row.SpeedMultiplierExpression = spec.SpeedMultiplierExpression;
		row.StaminaCostExpression = spec.StaminaCostExpression;
		row.RiderStaminaMultiplier = spec.RiderStaminaMultiplier;
	}

}
