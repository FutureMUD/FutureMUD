#nullable enable

namespace MudSharp.Models;

public class VehicleRouteTopologyPin
{
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public long RouteCellId { get; set; }
	public long TopologyVersion { get; set; }

	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual RouteCell RouteCell { get; set; } = null!;
}
