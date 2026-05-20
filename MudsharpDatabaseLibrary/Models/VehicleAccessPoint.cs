using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleAccessPoint
{
	public VehicleAccessPoint()
	{
		Locks = new HashSet<VehicleAccessPointLock>();
	}

	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleAccessPointProtoId { get; set; }
	public string Name { get; set; }
	public long? ProjectionItemId { get; set; }
	public bool IsOpen { get; set; }
	public bool IsDisabled { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleAccessPointProto VehicleAccessPointProto { get; set; }
	public virtual GameItem ProjectionItem { get; set; }
	public virtual ICollection<VehicleAccessPointLock> Locks { get; set; }
}
