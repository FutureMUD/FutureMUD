using System;

namespace MudSharp.Models;

public class VehicleTowLink
{
	public long Id { get; set; }
	public long SourceVehicleId { get; set; }
	public long TargetVehicleId { get; set; }
	public long SourceTowPointProtoId { get; set; }
	public long TargetTowPointProtoId { get; set; }
	public long? HitchItemId { get; set; }
	public bool IsDisabled { get; set; }
	public DateTime CreatedDateTime { get; set; }

	public virtual Vehicle SourceVehicle { get; set; }
	public virtual Vehicle TargetVehicle { get; set; }
	public virtual VehicleTowPointProto SourceTowPointProto { get; set; }
	public virtual VehicleTowPointProto TargetTowPointProto { get; set; }
	public virtual GameItem HitchItem { get; set; }
}
