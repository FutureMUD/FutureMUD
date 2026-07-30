namespace MudSharp.Models;

public class VehicleRiderStaminaModifierProto
{
	public long Id { get; set; }
	public long VehiclePropulsionProfileProtoId { get; set; }
	public long? TerrainId { get; set; }
	public long? TerrainTagId { get; set; }
	public double Multiplier { get; set; } = 1.0;

	public virtual VehiclePropulsionProfileProto VehiclePropulsionProfileProto { get; set; }
	public virtual Terrain Terrain { get; set; }
	public virtual Tag TerrainTag { get; set; }
}
