using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class Vehicle
{
	public Vehicle()
	{
		Compartments = new HashSet<VehicleCompartment>();
		Occupancies = new HashSet<VehicleOccupancy>();
		AccessStates = new HashSet<VehicleAccessState>();
		AccessPoints = new HashSet<VehicleAccessPoint>();
		CargoSpaces = new HashSet<VehicleCargoSpace>();
		Installations = new HashSet<VehicleInstallation>();
		SourceTowLinks = new HashSet<VehicleTowLink>();
		TargetTowLinks = new HashSet<VehicleTowLink>();
		DamageZones = new HashSet<VehicleDamageZone>();
	}

	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public long? ExteriorItemId { get; set; }
	public int LocationType { get; set; }
	public long? CurrentCellId { get; set; }
	public int CurrentRoomLayer { get; set; }
	public int MovementStatus { get; set; }
	public long? CurrentExitId { get; set; }
	public long? DestinationCellId { get; set; }
	public long? MovementProfileProtoId { get; set; }
	public DateTime CreatedDateTime { get; set; }
	public DateTime? LastMovementDateTime { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual GameItem ExteriorItem { get; set; }
	public virtual Cell CurrentCell { get; set; }
	public virtual Cell DestinationCell { get; set; }
	public virtual Exit CurrentExit { get; set; }
	public virtual VehicleMovementProfileProto MovementProfileProto { get; set; }
	public virtual ICollection<VehicleCompartment> Compartments { get; set; }
	public virtual ICollection<VehicleOccupancy> Occupancies { get; set; }
	public virtual ICollection<VehicleAccessState> AccessStates { get; set; }
	public virtual ICollection<VehicleAccessPoint> AccessPoints { get; set; }
	public virtual ICollection<VehicleCargoSpace> CargoSpaces { get; set; }
	public virtual ICollection<VehicleInstallation> Installations { get; set; }
	public virtual ICollection<VehicleTowLink> SourceTowLinks { get; set; }
	public virtual ICollection<VehicleTowLink> TargetTowLinks { get; set; }
	public virtual ICollection<VehicleDamageZone> DamageZones { get; set; }
}
