namespace MudSharp.Models;

public class VehicleCargoSpace
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleCargoSpaceProtoId { get; set; }
	public string Name { get; set; }
	public long? ProjectionItemId { get; set; }
	public bool IsDisabled { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleCargoSpaceProto VehicleCargoSpaceProto { get; set; }
	public virtual GameItem ProjectionItem { get; set; }
}
