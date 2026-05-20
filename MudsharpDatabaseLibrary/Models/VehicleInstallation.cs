namespace MudSharp.Models;

public class VehicleInstallation
{
	public long Id { get; set; }
	public long VehicleId { get; set; }
	public long VehicleInstallationPointProtoId { get; set; }
	public long? InstalledItemId { get; set; }
	public bool IsDisabled { get; set; }

	public virtual Vehicle Vehicle { get; set; }
	public virtual VehicleInstallationPointProto VehicleInstallationPointProto { get; set; }
	public virtual GameItem InstalledItem { get; set; }
}
