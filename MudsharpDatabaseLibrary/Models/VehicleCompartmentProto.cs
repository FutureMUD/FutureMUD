namespace MudSharp.Models;

public class VehicleCompartmentProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
}
