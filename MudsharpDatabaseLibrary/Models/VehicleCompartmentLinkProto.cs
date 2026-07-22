#nullable enable

namespace MudSharp.Models;

public class VehicleCompartmentLinkProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long SourceVehicleCompartmentProtoId { get; set; }
	public long DestinationVehicleCompartmentProtoId { get; set; }
	public string OutboundDirection { get; set; } = null!;
	public string InboundDirection { get; set; } = null!;
	public string OutboundDescription { get; set; } = null!;
	public string InboundDescription { get; set; } = null!;

	public virtual VehicleProto VehicleProto { get; set; } = null!;
	public virtual VehicleCompartmentProto SourceVehicleCompartmentProto { get; set; } = null!;
	public virtual VehicleCompartmentProto DestinationVehicleCompartmentProto { get; set; } = null!;
}
