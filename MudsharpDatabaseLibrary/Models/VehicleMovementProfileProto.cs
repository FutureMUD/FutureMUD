using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleMovementProfileProto
{
	public VehicleMovementProfileProto()
	{
		PropulsionProfiles = new HashSet<VehiclePropulsionProfileProto>();
	}
	public long Id { get; set; }
	public long VehicleProtoId { get; set; }
	public int VehicleProtoRevision { get; set; }
	public string Name { get; set; }
	public int MovementType { get; set; }
	public int MovementEnvironment { get; set; }
	public bool ExposesOccupantsToWater { get; set; }
	public bool IsDefault { get; set; }
	public double RequiredPowerSpikeInWatts { get; set; }
	public long? FuelLiquidId { get; set; }
	public double FuelVolumePerMove { get; set; }
	public string RequiredInstalledRole { get; set; }
	public bool RequiresTowLinksClosed { get; set; }
	public bool RequiresAccessPointsClosed { get; set; }
	public double RouteSpeedMetresPerSecond { get; set; }
	public int RoutePropulsionMode { get; set; }
	public double RouteFuelVolumePerMetre { get; set; }
	public double RoutePowerDrawWatts { get; set; }
	public bool AutomaticOperationCapable { get; set; }

	public virtual VehicleProto VehicleProto { get; set; }
	public virtual ICollection<VehiclePropulsionProfileProto> PropulsionProfiles { get; set; }
}
