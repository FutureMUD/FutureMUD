#nullable enable

namespace MudSharp.Models;

public class VehicleRoutePlatformBinding
{
	public long Id { get; set; }
	public long VehicleRouteStopId { get; set; }
	public long PlatformRoomId { get; set; }
	public long VehicleAccessPointProtoId { get; set; }
	public decimal DockingToleranceMetres { get; set; }

	public virtual VehicleRouteStop VehicleRouteStop { get; set; } = null!;
	public virtual Room PlatformRoom { get; set; } = null!;
	public virtual VehicleAccessPointProto VehicleAccessPointProto { get; set; } = null!;
}
