#nullable enable

namespace MudSharp.Models;

public class VehicleDocking
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleAccessPointId { get; set; }
	public long VehicleCompartmentId { get; set; }
	public long ExteriorCellId { get; set; }
	public int ExteriorRoomLayer { get; set; }
	public long? VehicleRouteStopId { get; set; }
	public int State { get; set; }

	public virtual Vehicle Vehicle { get; set; } = null!;
	public virtual VehicleAccessPoint VehicleAccessPoint { get; set; } = null!;
	public virtual VehicleCompartment VehicleCompartment { get; set; } = null!;
	public virtual Cell ExteriorCell { get; set; } = null!;
	public virtual VehicleRouteStop? VehicleRouteStop { get; set; }
}
