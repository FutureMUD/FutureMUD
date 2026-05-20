namespace MudSharp.Models;

public class VehicleMovementProfileProto
{
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public int MovementType { get; set; }
	public bool IsDefault { get; set; }
	public double RequiredPowerSpikeInWatts { get; set; }
	public long? FuelLiquidId { get; set; }
	public double FuelVolumePerMove { get; set; }
	public string RequiredInstalledRole { get; set; }
	public bool RequiresTowLinksClosed { get; set; }
	public bool RequiresAccessPointsClosed { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
}
