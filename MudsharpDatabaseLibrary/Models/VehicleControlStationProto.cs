namespace MudSharp.Models;

public class VehicleControlStationProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long VehicleOccupantSlotProtoId { get; set; }
	public string Name { get; set; }
	public bool IsPrimary { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleOccupantSlotProto VehicleOccupantSlotProto { get; set; }
}
