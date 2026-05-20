namespace MudSharp.Models;

public class VehicleAccessPointLock
{
	public long Id { get; set; }
	public long VehicleAccessPointId { get; set; }
	public long LockItemId { get; set; }

	public virtual VehicleAccessPoint VehicleAccessPoint { get; set; }
	public virtual GameItem LockItem { get; set; }
}
