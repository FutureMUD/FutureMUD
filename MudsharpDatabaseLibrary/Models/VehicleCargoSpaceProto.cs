namespace MudSharp.Models;

public class VehicleCargoSpaceProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long? VehicleCompartmentProtoId { get; set; }
	public long? RequiredAccessPointProtoId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public long? ProjectionItemProtoId { get; set; }
	public int? ProjectionItemProtoRevision { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
	public virtual VehicleAccessPointProto RequiredAccessPointProto { get; set; }
	public virtual GameItemProto ProjectionItemProto { get; set; }
}
