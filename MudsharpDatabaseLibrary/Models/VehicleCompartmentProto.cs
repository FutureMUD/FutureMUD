using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleCompartmentProto
{
	public VehicleCompartmentProto()
	{
		SourceLinks = new HashSet<VehicleCompartmentLinkProto>();
		DestinationLinks = new HashSet<VehicleCompartmentLinkProto>();
	}

	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int DisplayOrder { get; set; }
	public long? InteriorTerrainId { get; set; }
	public int InteriorOutdoorsType { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual Terrain InteriorTerrain { get; set; }
	public virtual ICollection<VehicleCompartmentLinkProto> SourceLinks { get; set; }
	public virtual ICollection<VehicleCompartmentLinkProto> DestinationLinks { get; set; }
}
