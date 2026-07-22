#nullable enable

namespace MudSharp.Models;

public class VehicleRoutePlatformBinding
{
	public long Id { get; set; }
	public long VehicleRouteStopId { get; set; }
	public long PlatformCellId { get; set; }
	public long VehicleAccessPointProtoId { get; set; }
	public decimal DockingToleranceMetres { get; set; }

	public virtual VehicleRouteStop VehicleRouteStop { get; set; } = null!;
	public virtual Cell PlatformCell { get; set; } = null!;
	public virtual VehicleAccessPointProto VehicleAccessPointProto { get; set; } = null!;
}
