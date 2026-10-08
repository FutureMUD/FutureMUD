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
	public long? InteriorRoomId { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
	public virtual Room InteriorRoom { get; set; }
	public virtual ICollection<VehicleDocking> Dockings { get; set; }
}
