namespace MudSharp.Models;

public class VehicleDamageZoneEffectProto
{
	public long Id { get; set; }
	public long VehicleDamageZoneProtoId { get; set; }
	public int TargetType { get; set; }
	public long? TargetProtoId { get; set; }
	public int MinimumStatus { get; set; }

	public virtual VehicleDamageZoneProto VehicleDamageZoneProto { get; set; }
}
