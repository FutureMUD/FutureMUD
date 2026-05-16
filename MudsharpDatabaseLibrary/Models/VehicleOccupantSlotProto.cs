namespace MudSharp.Models;

public class VehicleOccupantSlotProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public long VehicleCompartmentProtoId { get; set; }
	public string Name { get; set; }
	public int SlotType { get; set; }
	public int Capacity { get; set; }
	public bool RequiredForMovement { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual VehicleCompartmentProto VehicleCompartmentProto { get; set; }
}
