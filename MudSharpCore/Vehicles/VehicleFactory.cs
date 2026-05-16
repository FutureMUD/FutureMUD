using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.GameItems.Interfaces;
using System;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public static class VehicleFactory
{
	public static IVehicle CreateVehicle(IVehiclePrototype prototype, ICell location, RoomLayer roomLayer, ICharacter loader = null)
	{
		if (!prototype.CanCreateVehicle(out var reason))
		{
			throw new InvalidOperationException(reason);
		}

		var exterior = prototype.ExteriorItemPrototype.CreateNew(loader);
		prototype.Gameworld.Add(exterior);
		exterior.RoomLayer = roomLayer;
		location.Insert(exterior, true);
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
				LocationType = (int)VehicleLocationType.Cell,
				CurrentCellId = location.Id,
				CurrentRoomLayer = (int)roomLayer,
				MovementStatus = (int)VehicleMovementStatus.Stationary,
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
			             .Include(x => x.Occupancies)
			             .Include(x => x.AccessStates)
			             .Include(x => x.AccessPoints).ThenInclude(x => x.Locks)
			             .Include(x => x.CargoSpaces)
			             .Include(x => x.Installations)
			             .Include(x => x.SourceTowLinks)
			             .Include(x => x.TargetTowLinks)
			             .Include(x => x.DamageZones).ThenInclude(x => x.Wounds)
			             .AsNoTracking()
			             .First(x => x.Id == dbitem.Id);
		}

		var vehicle = new Vehicle(dbitem, prototype.Gameworld);
		prototype.Gameworld.Add(vehicle);
		vehicle.LinkExteriorItem(exterior);
		exterior.GetItemType<IVehicleExterior>()?.LinkVehicle(vehicle);
		CreateProjectionItems(vehicle, loader);
		exterior.HandleEvent(EventType.ItemFinishedLoading, exterior);
		exterior.Login();
		prototype.Gameworld.SaveManager.Flush();
		return vehicle;
	}

	private static void CreateProjectionItems(IVehicle vehicle, ICharacter loader)
	{
		foreach (var access in vehicle.AccessPoints.Where(x => x.ProjectionItem is null && x.Prototype.ProjectionItemPrototype is not null))
		{
			var item = access.Prototype.ProjectionItemPrototype.CreateNew(loader);
			vehicle.Gameworld.Add(item);
			vehicle.Gameworld.SaveManager.Flush();
			item.GetItemType<IVehicleAccessPointItem>()?.LinkAccessPoint(access);
			access.LinkProjectionItem(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
		}

		foreach (var cargo in vehicle.CargoSpaces.Where(x => x.ProjectionItem is null && x.Prototype.ProjectionItemPrototype is not null))
		{
			var item = cargo.Prototype.ProjectionItemPrototype.CreateNew(loader);
			vehicle.Gameworld.Add(item);
			vehicle.Gameworld.SaveManager.Flush();
			item.GetItemType<IVehicleCargoSpaceItem>()?.LinkCargoSpace(cargo);
			cargo.LinkProjectionItem(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
		}
	}
}
