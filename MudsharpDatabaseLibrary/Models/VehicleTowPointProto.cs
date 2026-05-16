namespace MudSharp.Models;

public class VehicleTowPointProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long? RequiredAccessPointProtoId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string TowType { get; set; }
	public bool CanTow { get; set; }
	public bool CanBeTowed { get; set; }
	public double MaximumTowedWeight { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleAccessPointProto RequiredAccessPointProto { get; set; }
}
