using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleDamageZone
{
	public VehicleDamageZone()
	{
		Wounds = new HashSet<Wound>();
	}

	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleDamageZoneProtoId { get; set; }
	public string Name { get; set; }
	public double CurrentDamage { get; set; }
	public int Status { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleDamageZoneProto VehicleDamageZoneProto { get; set; }
	public virtual ICollection<Wound> Wounds { get; set; }
}
