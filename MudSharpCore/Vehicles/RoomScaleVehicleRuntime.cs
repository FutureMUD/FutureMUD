#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

/// <summary>
/// A stable, persisted compartment instance. Its hosted cell belongs to the
/// vehicle rather than to the vehicle's current exterior room.
/// </summary>
public sealed class VehicleCompartment : FrameworkItem, IVehicleCompartment
{
	private readonly List<IVehicleCompartmentLink> _links = [];
	private long? _interiorCellId;

	public VehicleCompartment(IVehicle vehicle, DB.VehicleCompartment dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_interiorCellId = dbitem.InteriorCellId;
		Prototype = vehicle.Prototype.Compartments
			.FirstOrDefault(x => x.Id == dbitem.VehicleCompartmentProtoId)!;
	}

	public override string FrameworkItemType => "VehicleCompartment";
	public IVehicle Vehicle { get; }
	public IVehicleCompartmentPrototype Prototype { get; }
	public long? InteriorCellId => _interiorCellId;
	public ICell? InteriorCell => _interiorCellId is null
		? null
		: Vehicle.Gameworld.Cells.Get(_interiorCellId.Value);
	public IEnumerable<IVehicleCompartmentLink> Links => _links;

	internal void AddLink(IVehicleCompartmentLink link)
	{
		if (_links.All(x => x.Id != link.Id))
		{
			_links.Add(link);
		}
	}

	internal void ClearLinks()
	{
		_links.Clear();
	}

	internal void SetInteriorCell(ICell cell)
	{
		_interiorCellId = cell?.Id;
	}
}

/// <summary>
/// A live internal passage built from a revisioned compartment-link blueprint.
/// The exit itself is intentionally transient; the two hosted cell IDs are the
/// durable identity on either side of it.
/// </summary>
public sealed class VehicleCompartmentLink : FrameworkItem, IVehicleCompartmentLink
{
	private IExit? _exit;

	public VehicleCompartmentLink(IVehicle vehicle, IVehicleCompartmentLinkPrototype prototype,
		VehicleCompartment source, VehicleCompartment destination)
	{
		Vehicle = vehicle;
		Prototype = prototype;
		SourceCompartment = source;
		DestinationCompartment = destination;
		_id = prototype.Id;
		_name = $"{source.Name} to {destination.Name}";
	}

	public override string FrameworkItemType => "VehicleCompartmentLink";
	public IVehicle Vehicle { get; }
	public IVehicleCompartmentLinkPrototype Prototype { get; }
	public IVehicleCompartment SourceCompartment { get; }
	public IVehicleCompartment DestinationCompartment { get; }
	public long? ExitId => _exit?.Id;
	public IExit? Exit => _exit;

	internal bool Rebuild()
	{
		Remove();
		if (SourceCompartment.InteriorCell is not { } source ||
		    DestinationCompartment.InteriorCell is not { } destination)
		{
			return false;
		}

		_exit = new TransientExit(
			Vehicle.Gameworld,
			source,
			destination,
			"enter",
			Prototype.OutboundDirection,
			Prototype.InboundDirection,
			Prototype.OutboundDescription,
			Prototype.InboundDescription,
			"towards",
			"from",
			1.0);
		Vehicle.Gameworld.ExitManager.RegisterTransientExit(_exit);
		return true;
	}

	internal void Remove()
	{
		if (_exit is null)
		{
			return;
		}

		Vehicle.Gameworld.ExitManager.UnregisterTransientExit(_exit);
		_exit = null;
	}
}

public sealed class VehicleDocking : FrameworkItem, IVehicleDocking
{
	private IExit? _transientExit;
	private bool _registered;
	private long? _stopId;
	private VehicleDockingState _state;

	public VehicleDocking(IVehicle vehicle, DB.VehicleDocking dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = $"Vehicle Docking #{dbitem.Id:N0}";
		AccessPoint = vehicle.AccessPoints.FirstOrDefault(x => x.Id == dbitem.VehicleAccessPointId)!;
		Compartment = vehicle.Compartments.FirstOrDefault(x => x.Id == dbitem.VehicleCompartmentId)!;
		ExteriorCell = vehicle.Gameworld.Cells.Get(dbitem.ExteriorCellId)!;
		ExteriorLayer = (RoomLayer)dbitem.ExteriorRoomLayer;
		_stopId = dbitem.VehicleRouteStopId;
		_state = (VehicleDockingState)dbitem.State;
	}

	public override string FrameworkItemType => "VehicleDocking";
	public IVehicle Vehicle { get; }
	public IVehicleAccessPoint AccessPoint { get; }
	public IVehicleCompartment Compartment { get; }
	public ICell ExteriorCell { get; private set; }
	public RoomLayer ExteriorLayer { get; private set; }
	public IVehicleRouteStop? Stop => _stopId is null
		? null
		: Vehicle.Gameworld.VehicleRoutes
			.SelectMany(x => x.Stops)
			.FirstOrDefault(x => x.Id == _stopId.Value);
	public VehicleDockingState State => _state;
	public IExit TransientExit => _transientExit!;
	internal long? StopId => _stopId;
	internal bool IsRegistered => _registered;

	internal void Rebind(ICell exteriorCell, RoomLayer exteriorLayer, bool boardingOpen,
		IVehicleRouteStop? stop = null)
	{
		Suspend(false);
		ExteriorCell = exteriorCell;
		ExteriorLayer = exteriorLayer;
		_stopId = stop?.Id;
		_state = boardingOpen ? VehicleDockingState.BoardingOpen : VehicleDockingState.DockedClosed;
		Persist();
		BuildAndRegisterIfOpen();
	}

	internal void SetBoardingOpen(bool open)
	{
		Suspend(false);
		_state = open ? VehicleDockingState.BoardingOpen : VehicleDockingState.DockedClosed;
		Persist();
		BuildAndRegisterIfOpen();
	}

	internal void Suspend(bool persistClosed = true)
	{
		if (_registered && _transientExit is not null)
		{
			Vehicle.Gameworld.ExitManager.UnregisterTransientExit(_transientExit);
		}

		_registered = false;
		_transientExit = null;
		if (persistClosed && _state != VehicleDockingState.DockedClosed)
		{
			_state = VehicleDockingState.DockedClosed;
			Persist();
		}
	}

	internal void BuildAndRegisterIfOpen()
	{
		if (_state != VehicleDockingState.BoardingOpen ||
		    Compartment.InteriorCell is not { } interior ||
		    ExteriorCell is null ||
		    AccessPoint.IsDisabled || AccessPoint.IsLocked || !AccessPoint.IsOpen)
		{
			return;
		}

		var exteriorTarget = Vehicle.ExteriorItem?.Name ?? Vehicle.Name;
		var interiorTarget = AccessPoint.Prototype.Description.IfNullOrWhiteSpace(AccessPoint.Name);
		_transientExit = new TransientExit(
			Vehicle.Gameworld,
			ExteriorCell,
			interior,
			"enter",
			AccessPoint.Name.ToLowerInvariant(),
			"outside",
			interiorTarget,
			exteriorTarget,
			"through",
			"towards",
			1.0);
		Vehicle.Gameworld.ExitManager.RegisterTransientExit(_transientExit);
		_registered = true;
	}

	private void Persist()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleDockings.Find(Id);
			if (dbitem is null)
			{
				return;
			}

			dbitem.ExteriorCellId = ExteriorCell.Id;
			dbitem.ExteriorRoomLayer = (int)ExteriorLayer;
			dbitem.VehicleRouteStopId = _stopId;
			dbitem.State = (int)_state;
			FMDB.Context.SaveChanges();
		}
	}
}

/// <summary>
/// Creates hosted cells only when explicitly requested by vehicle creation or
/// recovery. Normal load never invents a replacement for a missing cell ID.
/// </summary>
public static class RoomScaleVehicleInteriorService
{
	internal enum RecoveryAction
	{
		None,
		Relinked,
		Created
	}

	internal static bool TryRecoverInterior(Vehicle vehicle, VehicleCompartment compartment,
		out RecoveryAction action, out string reason)
	{
		if (compartment.InteriorCell is not null)
		{
			action = RecoveryAction.None;
			reason = string.Empty;
			return true;
		}

		using (new FMDB())
		{
			if (!TryRelinkPersistedInterior(vehicle, compartment, FMDB.Context, out var relinked, out reason))
			{
				action = RecoveryAction.None;
				return false;
			}

			if (relinked)
			{
				action = RecoveryAction.Relinked;
				return true;
			}
		}

		if (!TryCreateInterior(vehicle, compartment, out reason))
		{
			action = RecoveryAction.None;
			return false;
		}

		action = RecoveryAction.Created;
		return true;
	}

	internal static bool TryRelinkPersistedInterior(Vehicle vehicle, VehicleCompartment compartment,
		FuturemudDatabaseContext context, out bool relinked, out string reason)
	{
		relinked = false;
		var candidateIds = context.Cells
			.Where(x => x.HostedVehicleId == vehicle.Id &&
			            x.HostedVehicleCompartmentId == compartment.Id)
			.Select(x => x.Id)
			.Take(2)
			.ToList();

		if (candidateIds.Count == 0)
		{
			reason = string.Empty;
			return true;
		}

		if (candidateIds.Count > 1)
		{
			reason = $"More than one persisted hosted cell claims vehicle #{vehicle.Id:N0} compartment " +
			         $"#{compartment.Id:N0}. Resolve the duplicate ownership records before retrying recovery.";
			return false;
		}

		var candidateId = candidateIds[0];
		var cell = vehicle.Gameworld.Cells.Get(candidateId);
		if (cell is null)
		{
			reason = $"Persisted hosted cell #{candidateId:N0} already belongs to {compartment.Name}, but it is not " +
			         "loaded. Recovery refused to create a duplicate; restore or reload that cell first.";
			return false;
		}

		var claimedBy = context.VehicleCompartments
			.Where(x => x.Id != compartment.Id && x.InteriorCellId == candidateId)
			.Select(x => x.Id)
			.FirstOrDefault();
		if (claimedBy != 0)
		{
			reason = $"Persisted hosted cell #{candidateId:N0} is already linked to vehicle compartment " +
			         $"#{claimedBy:N0}. Recovery refused to steal that cell.";
			return false;
		}

		var dbcompartment = context.VehicleCompartments.Find(compartment.Id);
		if (dbcompartment is null)
		{
			reason = $"Vehicle compartment #{compartment.Id:N0} disappeared while its hosted cell was being recovered.";
			return false;
		}

		var previousInteriorCellId = dbcompartment.InteriorCellId;
		dbcompartment.InteriorCellId = candidateId;
		try
		{
			context.SaveChanges();
		}
		catch (DbUpdateException)
		{
			dbcompartment.InteriorCellId = previousInteriorCellId;
			context.Entry(dbcompartment).Property(x => x.InteriorCellId).IsModified = false;
			reason = $"Persisted hosted cell #{candidateId:N0} could not be relinked because its ownership " +
			         "changed or conflicts with another record. Recovery did not create a replacement.";
			return false;
		}

		compartment.SetInteriorCell(cell);
		relinked = true;
		reason = string.Empty;
		return true;
	}

	public static bool TryCreateInterior(Vehicle vehicle, VehicleCompartment compartment, out string reason)
	{
		if (vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			reason = "Only room-scale vehicles have hosted interior cells.";
			return false;
		}

		if (compartment.InteriorCell is not null)
		{
			reason = string.Empty;
			return true;
		}

		if (vehicle.Location is null)
		{
			reason = "The vehicle has no exterior cell from which to source its hosted interior.";
			return false;
		}

		if (compartment.Prototype.InteriorTerrain is null)
		{
			reason = $"The {compartment.Prototype.Name} compartment has no interior terrain.";
			return false;
		}

		var room = new Room(
			vehicle.Location.Zone,
			vehicle.Location.CurrentOverlay.Package,
			vehicle.Location,
			false);
		room.SetName($"{vehicle.Name} - {compartment.Name}");
		var cell = (Cell)room.Cells.Single();
		var overlay = (IEditableCellOverlay)cell.CurrentOverlay;
		overlay.CellName = $"{vehicle.Name} - {compartment.Name}";
		overlay.CellDescription = compartment.Prototype.Description;
		overlay.Terrain = compartment.Prototype.InteriorTerrain;
		overlay.OutdoorsType = compartment.Prototype.InteriorOutdoorsType;
		overlay.AmbientLightFactor = AmbientLightFactor(compartment.Prototype.InteriorOutdoorsType);
		cell.SetHostedVehicle(vehicle.Id, compartment.Id);
		vehicle.Gameworld.SaveManager.Flush();

		using (new FMDB())
		{
			var dbcell = FMDB.Context.Cells.Find(cell.Id);
			var dbcompartment = FMDB.Context.VehicleCompartments.Find(compartment.Id);
			if (dbcell is null || dbcompartment is null)
			{
				reason = "The hosted cell or vehicle compartment disappeared while it was being persisted.";
				return false;
			}

			dbcell.HostedVehicleId = vehicle.Id;
			dbcell.HostedVehicleCompartmentId = compartment.Id;
			dbcompartment.InteriorCellId = cell.Id;
			FMDB.Context.SaveChanges();
		}

		compartment.SetInteriorCell(cell);
		reason = string.Empty;
		return true;
	}

	public static void RepairHostMetadata(Vehicle vehicle, VehicleCompartment compartment)
	{
		if (compartment.InteriorCell is not Cell cell)
		{
			return;
		}

		cell.SetHostedVehicle(vehicle.Id, compartment.Id);
		using (new FMDB())
		{
			var dbcell = FMDB.Context.Cells.Find(cell.Id);
			if (dbcell is not null)
			{
				dbcell.HostedVehicleId = vehicle.Id;
				dbcell.HostedVehicleCompartmentId = compartment.Id;
				FMDB.Context.SaveChanges();
			}
		}
	}

	private static double AmbientLightFactor(CellOutdoorsType outdoorsType)
	{
		return outdoorsType switch
		{
			CellOutdoorsType.Outdoors => 1.0,
			CellOutdoorsType.Indoors => 0.25,
			CellOutdoorsType.IndoorsClimateExposed => 0.9,
			CellOutdoorsType.IndoorsWithWindows => 0.35,
			CellOutdoorsType.IndoorsNoLight => 0.0,
			_ => 0.25
		};
	}
}

public sealed class VehicleDockingService : IVehicleDockingService
{
	public IReadOnlyCollection<IVehicleDocking> ActiveDockings(IVehicle vehicle)
	{
		return vehicle.Dockings
			.Where(x => x.State == VehicleDockingState.BoardingOpen)
			.OfType<VehicleDocking>()
			.Where(x => x.IsRegistered)
			.Cast<IVehicleDocking>()
			.ToList();
	}

	public bool CanDock(IVehicle vehicle, IVehicleAccessPoint accessPoint, ICell exteriorCell,
		RoomLayer exteriorLayer, IVehicleRouteStop? stop, out string reason)
	{
		if (vehicle is not Vehicle concrete || vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			reason = "Only live room-scale vehicles can create hosted-cell dockings.";
			return false;
		}

		if (accessPoint is null || vehicle.AccessPoints.All(x => x.Id != accessPoint.Id))
		{
			reason = "That access point does not belong to this vehicle.";
			return false;
		}

		var compartment = concrete.CompartmentFor(accessPoint.Prototype.Compartment);
		if (compartment?.InteriorCell is null)
		{
			reason = "That access point does not lead to a live hosted interior.";
			return false;
		}

		if (exteriorCell is null)
		{
			reason = "A docking requires an exterior cell.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public IVehicleDocking Dock(IVehicle vehicle, IVehicleAccessPoint accessPoint, ICell exteriorCell,
		RoomLayer exteriorLayer, IVehicleRouteStop? stop = null)
	{
		if (!CanDock(vehicle, accessPoint, exteriorCell, exteriorLayer, stop, out var reason))
		{
			throw new InvalidOperationException(reason);
		}

		var concrete = (Vehicle)vehicle;
		var compartment = concrete.CompartmentFor(accessPoint.Prototype.Compartment)!;
		var existing = concrete.DockingsInternal.FirstOrDefault(x => x.AccessPoint.Id == accessPoint.Id);
		if (existing is not null)
		{
			existing.Rebind(exteriorCell, exteriorLayer,
				accessPoint.IsOpen && !accessPoint.IsDisabled && !accessPoint.IsLocked, stop);
			return existing;
		}

		DB.VehicleDocking dbitem;
		using (new FMDB())
		{
			dbitem = new DB.VehicleDocking
			{
				VehicleId = vehicle.Id,
				VehicleAccessPointId = accessPoint.Id,
				VehicleCompartmentId = compartment.Id,
				ExteriorCellId = exteriorCell.Id,
				ExteriorRoomLayer = (int)exteriorLayer,
				VehicleRouteStopId = stop?.Id,
				State = (int)(accessPoint.IsOpen && !accessPoint.IsDisabled && !accessPoint.IsLocked
					? VehicleDockingState.BoardingOpen
					: VehicleDockingState.DockedClosed)
			};
			FMDB.Context.VehicleDockings.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var docking = new VehicleDocking(vehicle, dbitem);
		concrete.AddDocking(docking);
		docking.BuildAndRegisterIfOpen();
		return docking;
	}

	public void SetBoardingOpen(IVehicleDocking docking, bool open)
	{
		if (docking is VehicleDocking concrete)
		{
			concrete.SetBoardingOpen(open);
		}
	}

	public void Undock(IVehicleDocking docking)
	{
		if (docking is not VehicleDocking concrete || docking.Vehicle is not Vehicle vehicle)
		{
			return;
		}

		concrete.Suspend();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleDockings.Find(docking.Id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleDockings.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		vehicle.RemoveDocking(docking.Id);
	}

	public void RebuildDockings(IVehicle vehicle)
	{
		if (vehicle is not Vehicle concrete || vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			return;
		}

		if (vehicle.Location is null || vehicle.ExteriorItem is null ||
		    vehicle.ExteriorItem.Deleted || vehicle.ExteriorItem.Destroyed)
		{
			concrete.SuspendDockings();
			return;
		}

		if (vehicle.Location.RouteDefinition is not null)
		{
			foreach (var docking in concrete.DockingsInternal)
			{
				if (!IsAtBoundRouteStop(vehicle, docking))
				{
					docking.Suspend();
					continue;
				}

				docking.Rebind(docking.ExteriorCell, docking.ExteriorLayer,
					ShouldReopenRouteDocking(
						docking.State,
						docking.AccessPoint.IsOpen,
						docking.AccessPoint.IsDisabled,
						docking.AccessPoint.IsLocked),
					docking.Stop);
			}

			return;
		}

		var validAccessIds = new HashSet<long>();
		foreach (var accessPoint in vehicle.AccessPoints)
		{
			var compartment = concrete.CompartmentFor(accessPoint.Prototype.Compartment);
			if (compartment?.InteriorCell is null)
			{
				continue;
			}

			validAccessIds.Add(accessPoint.Id);
			var existing = concrete.DockingsInternal.FirstOrDefault(x => x.AccessPoint.Id == accessPoint.Id);
			if (existing is null)
			{
				Dock(vehicle, accessPoint, vehicle.Location, vehicle.RoomLayer);
				continue;
			}

			existing.Rebind(vehicle.Location, vehicle.RoomLayer,
				accessPoint.IsOpen && !accessPoint.IsDisabled && !accessPoint.IsLocked);
		}

		foreach (var stale in concrete.DockingsInternal
			         .Where(x => !validAccessIds.Contains(x.AccessPoint.Id))
			         .ToList())
		{
			Undock(stale);
		}
	}

	internal static bool ShouldReopenRouteDocking(
		VehicleDockingState persistedState,
		bool accessPointOpen,
		bool accessPointDisabled,
		bool accessPointLocked)
	{
		return persistedState == VehicleDockingState.BoardingOpen &&
		       accessPointOpen &&
		       !accessPointDisabled &&
		       !accessPointLocked;
	}

	internal static bool IsAtBoundRouteStop(IVehicle vehicle, VehicleDocking docking)
	{
		if (docking.Stop is not { } stop || vehicle.Location?.Id != stop.Location.Cell.Id ||
		    vehicle.RoomLayer != stop.Location.Layer)
		{
			return false;
		}

		var binding = stop.PlatformBindings.FirstOrDefault(x =>
			x.AccessPoint.Id == docking.AccessPoint.Prototype.Id &&
			x.PlatformCell.Id == docking.ExteriorCell.Id);
		if (binding is null)
		{
			return false;
		}

		if (stop.Location.RoutePositionMetres is not { } stopPosition)
		{
			return true;
		}

		return vehicle.RoutePositionMetres is { } vehiclePosition &&
		       Math.Abs(vehiclePosition - stopPosition) <= binding.DockingToleranceMetres;
	}
}
