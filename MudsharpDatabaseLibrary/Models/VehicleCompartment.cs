using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleCompartment
{
	public VehicleCompartment()
	{
		Dockings = new HashSet<VehicleDocking>();
	}

	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleCompartmentProtoId { get; set; }
	public string Name { get; set; }
	public long? InteriorCellId { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
	public virtual Cell InteriorCell { get; set; }
	public virtual ICollection<VehicleDocking> Dockings { get; set; }
}
