using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.GameItems.Prototypes;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public sealed class VehicleFactoryPartialCreationException : InvalidOperationException
{
	public VehicleFactoryPartialCreationException(string message) : base(message)
	{
	}
}

public static class VehicleFactory
{
	public static IVehicle CreateVehicle(IVehiclePrototype prototype, ICell location, RoomLayer roomLayer, ICharacter loader = null)
	{
		if (!prototype.CanCreateVehicle(out var reason))
		{
			throw new InvalidOperationException(reason);
		}

		var exterior = prototype.ExteriorItemPrototype.CreateNew(loader);
		var movementProfile = prototype.MovementProfiles
			.Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
			.OrderByDescending(x => x.IsDefault)
			.FirstOrDefault() ?? prototype.MovementProfiles
			.Where(x => x.MovementType == VehicleMovementProfileType.Route)
			.OrderByDescending(x => x.IsDefault)
			.FirstOrDefault();
		var propulsionProfile = movementProfile?.PropulsionProfiles
			.OrderByDescending(x => x.IsDefault)
			.FirstOrDefault();
		prototype.Gameworld.Add(exterior);
		exterior.RoomLayer = roomLayer;
		if (loader is not null && ReferenceEquals(loader.Location, location))
		{
			exterior.InsertAtSource(loader, true);
		}
		else
		{
			location.Insert(exterior, true);
		}
		prototype.Gameworld.SaveManager.Flush();

		DB.Vehicle dbitem;
		using (new FMDB())
		{
			dbitem = new DB.Vehicle
			{
				VehicleProtoId = prototype.Id,
				VehicleProtoRevision = prototype.RevisionNumber,
				Name = prototype.Name,
				ExteriorItemId = exterior.Id,
				LocationType = (int)(location.RouteDefinition is null
					? VehicleLocationType.Cell
					: VehicleLocationType.Route),
				CurrentCellId = location.Id,
				CurrentRoomLayer = (int)exterior.RoomLayer,
				CurrentRoutePosition = exterior.RoutePositionMetres is null
					? null
					: (decimal)exterior.RoutePositionMetres.Value,
				MovementStatus = (int)VehicleMovementStatus.Stationary,
				MovementProfileProtoId = movementProfile?.Id,
				ActivePropulsionProfileProtoId = propulsionProfile?.Id,
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.Vehicles.Add(dbitem);
			FMDB.Context.SaveChanges();

			foreach (var compartment in prototype.Compartments)
			{
				FMDB.Context.VehicleCompartments.Add(new DB.VehicleCompartment
				{
					VehicleId = dbitem.Id,
					VehicleCompartmentProtoId = compartment.Id,
					Name = compartment.Name
				});
			}

			foreach (var access in prototype.AccessPoints)
			{
				FMDB.Context.VehicleAccessPoints.Add(new DB.VehicleAccessPoint
				{
					VehicleId = dbitem.Id,
					VehicleAccessPointProtoId = access.Id,
					Name = access.Name,
					IsOpen = access.StartsOpen,
					IsDisabled = false
				});
			}

			foreach (var cargo in prototype.CargoSpaces)
			{
				FMDB.Context.VehicleCargoSpaces.Add(new DB.VehicleCargoSpace
				{
					VehicleId = dbitem.Id,
					VehicleCargoSpaceProtoId = cargo.Id,
					Name = cargo.Name,
					IsDisabled = false
				});
			}

			foreach (var install in prototype.InstallationPoints)
			{
				FMDB.Context.VehicleInstallations.Add(new DB.VehicleInstallation
				{
					VehicleId = dbitem.Id,
					VehicleInstallationPointProtoId = install.Id,
					IsDisabled = false
				});
			}

			foreach (var zone in prototype.DamageZones)
			{
				FMDB.Context.VehicleDamageZones.Add(new DB.VehicleDamageZone
				{
					VehicleId = dbitem.Id,
					VehicleDamageZoneProtoId = zone.Id,
					Name = zone.Name,
					CurrentDamage = 0.0,
					Status = (int)VehicleSystemStatus.Functional
				});
			}

			FMDB.Context.SaveChanges();
			dbitem = FMDB.Context.Vehicles
			             .Include(x => x.Compartments)
			             .Include(x => x.Occupancies)
			             .Include(x => x.AccessStates)
			             .Include(x => x.AccessPoints).ThenInclude(x => x.Locks)
			             .Include(x => x.CargoSpaces)
			             .Include(x => x.Installations)
			             .Include(x => x.SourceTowLinks)
			             .Include(x => x.TargetTowLinks)
			             .Include(x => x.DamageZones).ThenInclude(x => x.Wounds)
			             .Include(x => x.Dockings)
			             .AsNoTracking()
			             .First(x => x.Id == dbitem.Id);
		}

		var vehicle = new Vehicle(dbitem, prototype.Gameworld);
		prototype.Gameworld.Add(vehicle);
		vehicle.LinkExteriorItem(exterior);
		exterior.GetItemType<IVehicleExterior>()?.LinkVehicle(vehicle);
		vehicle.SynchroniseExteriorItemToLocation();
		// The canonical vehicle and exterior are already durable at this point. Persist their
		// mutual link before creating hosted cells so an interrupted RoomScale factory run
		// always leaves an inspectable, recoverable projection.
		prototype.Gameworld.SaveManager.Flush();
		if (vehicle is Vehicle concrete && !concrete.EnsureRoomScaleInteriors(out var interiorReason))
		{
			throw new VehicleFactoryPartialCreationException(
				$"Vehicle #{vehicle.Id:N0} was persisted, but its hosted interiors were not fully created: {interiorReason} " +
				$"The partial vehicle was retained. Use vehicle audit {vehicle.Id:N0} interior, then vehicle recover {vehicle.Id:N0} interior fix.");
		}
		var projectionFailures = CreateProjectionItems(vehicle, loader);
		if (projectionFailures.Any())
		{
			throw new VehicleFactoryPartialCreationException(
				$"Vehicle #{vehicle.Id:N0} was persisted, but one or more authored projection items were not fully created: " +
				$"{projectionFailures.ListToString()} The partial vehicle was retained. Use vehicle recover " +
				$"{vehicle.Id:N0} projection fix.");
		}

		if (vehicle is Vehicle roomScaleVehicle)
		{
			roomScaleVehicle.RebuildDockings();
		}
		exterior.HandleEvent(EventType.ItemFinishedLoading, exterior);
		exterior.Login();
		prototype.Gameworld.SaveManager.Flush();
		return vehicle;
	}

	internal static IReadOnlyList<string> CreateProjectionItems(IVehicle vehicle, ICharacter loader)
	{
		var failures = new List<string>();
		foreach (var access in vehicle.AccessPoints)
		{
			if (!TryCreateAccessProjectionItem(access, loader, out _, out var reason))
			{
				failures.Add($"access point {access.Name}: {reason}");
			}
		}

		foreach (var cargo in vehicle.CargoSpaces)
		{
			if (!TryCreateCargoProjectionItem(cargo, loader, out _, out var reason))
			{
				failures.Add($"cargo space {cargo.Name}: {reason}");
			}
		}

		return failures;
	}

	internal static bool TryCreateAccessProjectionItem(IVehicleAccessPoint access, ICharacter loader,
		out bool created, out string reason)
	{
		created = false;
		try
		{
			if (access.ProjectionItem is { Deleted: false, Destroyed: false })
			{
				reason = string.Empty;
				return true;
			}

			var prototype = access.Prototype.ProjectionItemPrototype;
			if (prototype is null)
			{
				reason = "the authored access-point projection item prototype is missing";
				return false;
			}
			if (!prototype.IsItemType<IVehicleAccessPointItemPrototype>())
			{
				reason = "the authored item prototype has no vehicle access-point component";
				return false;
			}

			var item = prototype.CreateNew(loader);
			var component = item?.GetItemType<IVehicleAccessPointItem>();
			if (item is null || component is null)
			{
				reason = "the authored item prototype did not create an item with a vehicle access-point component";
				return false;
			}

			component.LinkAccessPoint(access);
			access.Vehicle.Gameworld.Add(item);
			access.Vehicle.Gameworld.SaveManager.Flush();
			access.LinkProjectionItem(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
			access.Vehicle.Gameworld.SaveManager.Flush();
			created = true;
			reason = string.Empty;
			return true;
		}
		catch (Exception ex)
		{
			reason = $"projection creation failed safely: {ex.Message}";
			return false;
		}
	}

	internal static bool TryCreateCargoProjectionItem(IVehicleCargoSpace cargo, ICharacter loader,
		out bool created, out string reason)
	{
		created = false;
		try
		{
			if (cargo.ProjectionItem is { Deleted: false, Destroyed: false })
			{
				reason = string.Empty;
				return true;
			}

			var prototype = cargo.Prototype.ProjectionItemPrototype;
			if (prototype is null)
			{
				reason = "the authored cargo projection item prototype is missing";
				return false;
			}
			if (!prototype.IsItemType<IVehicleCargoSpaceItemPrototype>() ||
			    !prototype.IsItemType<IContainerPrototype>())
			{
				reason = "the authored item prototype must have vehicle cargo-space and container components";
				return false;
			}

			var item = prototype.CreateNew(loader);
			var component = item?.GetItemType<IVehicleCargoSpaceItem>();
			if (item is null || component is null)
			{
				reason = "the authored item prototype did not create an item with a vehicle cargo-space component";
				return false;
			}

			component.LinkCargoSpace(cargo);
			cargo.Vehicle.Gameworld.Add(item);
			cargo.Vehicle.Gameworld.SaveManager.Flush();
			cargo.LinkProjectionItem(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
			cargo.Vehicle.Gameworld.SaveManager.Flush();
			created = true;
			reason = string.Empty;
			return true;
		}
		catch (Exception ex)
		{
			reason = $"projection creation failed safely: {ex.Message}";
			return false;
		}
	}
}
