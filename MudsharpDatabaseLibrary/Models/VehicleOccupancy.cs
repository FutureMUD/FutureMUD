namespace MudSharp.Models;

public class VehicleOccupancy
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long CharacterId { get; set; }
	public long VehicleOccupantSlotProtoId { get; set; }
	public bool IsController { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual Character Character { get; set; }
	public virtual VehicleOccupantSlotProto VehicleOccupantSlotProto { get; set; }
}
