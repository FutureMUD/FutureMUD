using System.Collections.Generic;

namespace MudSharp.Models;

public class VehiclePropulsionProfileProto
{
	public VehiclePropulsionProfileProto()
	{
		RiderStaminaModifiers = new HashSet<VehicleRiderStaminaModifierProto>();
	}

	public long Id { get; set; }
	public long VehicleMovementProfileProtoId { get; set; }
	public int PropulsionType { get; set; }
	public bool IsDefault { get; set; }
	public double BaseMoveTimeMilliseconds { get; set; } = 10000.0;
	public long? PropulsionTraitDefinitionId { get; set; }
	public int CheckDifficulty { get; set; }
	public string SpeedMultiplierExpression { get; set; }
	public string StaminaCostExpression { get; set; }
	public double RiderStaminaMultiplier { get; set; } = 1.0;

	public virtual VehicleMovementProfileProto VehicleMovementProfileProto { get; set; }
	public virtual TraitDefinition PropulsionTraitDefinition { get; set; }
	public virtual ICollection<VehicleRiderStaminaModifierProto> RiderStaminaModifiers { get; set; }
}
