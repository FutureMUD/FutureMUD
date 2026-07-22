#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleRouteLeg
{
	public VehicleRouteLeg()
	{
		Steps = new HashSet<VehicleRouteStep>();
	}

	public long Id { get; set; }
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public int Sequence { get; set; }
	public long OriginStopId { get; set; }
	public long DestinationStopId { get; set; }
	public decimal RouteDistanceMetres { get; set; }
	public decimal RoomEquivalentCost { get; set; }

	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual VehicleRouteStop OriginStop { get; set; } = null!;
	public virtual VehicleRouteStop DestinationStop { get; set; } = null!;
	public virtual ICollection<VehicleRouteStep> Steps { get; set; }
}
