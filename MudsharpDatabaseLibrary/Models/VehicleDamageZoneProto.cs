using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleDamageZoneProto
{
	public VehicleDamageZoneProto()
	{
		Effects = new HashSet<VehicleDamageZoneEffectProto>();
	}

	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public double MaximumDamage { get; set; }
	public double HitWeight { get; set; }
	public double DisabledThreshold { get; set; }
	public double DestroyedThreshold { get; set; }
	public bool DisablesMovement { get; set; }
	public int DisplayOrder { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual ICollection<VehicleDamageZoneEffectProto> Effects { get; set; }
}
