#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleRouteStop
{
	public VehicleRouteStop()
	{
		PlatformBindings = new HashSet<VehicleRoutePlatformBinding>();
		OriginLegs = new HashSet<VehicleRouteLeg>();
		DestinationLegs = new HashSet<VehicleRouteLeg>();
		CurrentJourneys = new HashSet<VehicleJourney>();
		NextJourneys = new HashSet<VehicleJourney>();
		Dockings = new HashSet<VehicleDocking>();
	}

	public long Id { get; set; }
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public string Name { get; set; } = null!;
	public int Sequence { get; set; }
	public long CellId { get; set; }
	public int RoomLayer { get; set; }
	public decimal? RoutePositionMetres { get; set; }
	public long DwellDurationMilliseconds { get; set; }

	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual Cell Cell { get; set; } = null!;
	public virtual ICollection<VehicleRoutePlatformBinding> PlatformBindings { get; set; }
	public virtual ICollection<VehicleRouteLeg> OriginLegs { get; set; }
	public virtual ICollection<VehicleRouteLeg> DestinationLegs { get; set; }
	public virtual ICollection<VehicleJourney> CurrentJourneys { get; set; }
	public virtual ICollection<VehicleJourney> NextJourneys { get; set; }
	public virtual ICollection<VehicleDocking> Dockings { get; set; }
}
