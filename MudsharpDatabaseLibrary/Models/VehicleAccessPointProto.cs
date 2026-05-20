namespace MudSharp.Models;

public class VehicleAccessPointProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long? VehicleCompartmentProtoId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int AccessPointType { get; set; }
	public long? ProjectionItemProtoId { get; set; }
	public int? ProjectionItemProtoRevision { get; set; }
	public bool StartsOpen { get; set; }
	public bool MustBeClosedForMovement { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
	public virtual GameItemProto ProjectionItemProto { get; set; }
}
