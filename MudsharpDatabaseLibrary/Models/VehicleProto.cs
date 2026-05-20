using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleProto
{
	public VehicleProto()
	{
		Compartments = new HashSet<VehicleCompartmentProto>();
		OccupantSlots = new HashSet<VehicleOccupantSlotProto>();
		ControlStations = new HashSet<VehicleControlStationProto>();
		MovementProfiles = new HashSet<VehicleMovementProfileProto>();
		AccessPoints = new HashSet<VehicleAccessPointProto>();
		CargoSpaces = new HashSet<VehicleCargoSpaceProto>();
		InstallationPoints = new HashSet<VehicleInstallationPointProto>();
		TowPoints = new HashSet<VehicleTowPointProto>();
		DamageZones = new HashSet<VehicleDamageZoneProto>();
		Vehicles = new HashSet<Vehicle>();
	}

	public long Id { get; set; }
	public int RevisionNumber { get; set; }
	public long EditableItemId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int VehicleScale { get; set; }
	public long? ExteriorItemProtoId { get; set; }
	public int? ExteriorItemProtoRevision { get; set; }

	public virtual EditableItem EditableItem { get; set; }
	public virtual GameItemProto ExteriorItemProto { get; set; }
	public virtual ICollection<VehicleCompartmentProto> Compartments { get; set; }
	public virtual ICollection<VehicleOccupantSlotProto> OccupantSlots { get; set; }
	public virtual ICollection<VehicleControlStationProto> ControlStations { get; set; }
	public virtual ICollection<VehicleMovementProfileProto> MovementProfiles { get; set; }
	public virtual ICollection<VehicleAccessPointProto> AccessPoints { get; set; }
	public virtual ICollection<VehicleCargoSpaceProto> CargoSpaces { get; set; }
	public virtual ICollection<VehicleInstallationPointProto> InstallationPoints { get; set; }
	public virtual ICollection<VehicleTowPointProto> TowPoints { get; set; }
	public virtual ICollection<VehicleDamageZoneProto> DamageZones { get; set; }
	public virtual ICollection<Vehicle> Vehicles { get; set; }
}
