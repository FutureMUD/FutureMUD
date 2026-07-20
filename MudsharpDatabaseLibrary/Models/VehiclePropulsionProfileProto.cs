namespace MudSharp.Models;

public class VehiclePropulsionProfileProto
{
	public long Id { get; set; }
	public long VehicleMovementProfileProtoId { get; set; }
	public int PropulsionType { get; set; }
	public bool IsDefault { get; set; }
	public double BaseMoveTimeMilliseconds { get; set; } = 10000.0;
	public long? PropulsionTraitDefinitionId { get; set; }
	public int CheckDifficulty { get; set; }
	public string SpeedMultiplierExpression { get; set; }
	public string StaminaCostExpression { get; set; }

	public virtual VehicleMovementProfileProto VehicleMovementProfileProto { get; set; }
	public virtual TraitDefinition PropulsionTraitDefinition { get; set; }
}
