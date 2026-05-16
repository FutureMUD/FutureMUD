namespace MudSharp.Models;

public class VehicleInstallationPointProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long? RequiredAccessPointProtoId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string MountType { get; set; }
	public string RequiredRole { get; set; }
	public bool RequiredForMovement { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleAccessPointProto RequiredAccessPointProto { get; set; }
}
