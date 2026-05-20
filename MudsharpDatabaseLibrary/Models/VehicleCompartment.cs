namespace MudSharp.Models;

public class VehicleCompartment
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleCompartmentProtoId { get; set; }
	public string Name { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
}
