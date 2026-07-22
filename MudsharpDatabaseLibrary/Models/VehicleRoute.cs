#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleRoute
{
	public VehicleRoute()
	{
		Stops = new HashSet<VehicleRouteStop>();
		Legs = new HashSet<VehicleRouteLeg>();
		TopologyPins = new HashSet<VehicleRouteTopologyPin>();
		Services = new HashSet<VehicleService>();
		Journeys = new HashSet<VehicleJourney>();
	}

	public long Id { get; set; }
	public int RevisionNumber { get; set; }
	public long EditableItemId { get; set; }
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;

	public virtual EditableItem EditableItem { get; set; } = null!;
	public virtual ICollection<VehicleRouteStop> Stops { get; set; }
	public virtual ICollection<VehicleRouteLeg> Legs { get; set; }
	public virtual ICollection<VehicleRouteTopologyPin> TopologyPins { get; set; }
	public virtual ICollection<VehicleService> Services { get; set; }
	public virtual ICollection<VehicleJourney> Journeys { get; set; }
}
