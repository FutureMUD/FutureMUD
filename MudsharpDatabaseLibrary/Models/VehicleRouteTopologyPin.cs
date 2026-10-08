#nullable enable

namespace MudSharp.Models;

public class VehicleRouteTopologyPin
{
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public long RouteRoomId { get; set; }
	public long TopologyVersion { get; set; }

	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual RouteRoom RouteRoom { get; set; } = null!;
}
