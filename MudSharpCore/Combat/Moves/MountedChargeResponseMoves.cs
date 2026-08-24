#nullable enable

namespace MudSharp.Combat.Moves;

public sealed class EvadeMountedChargeMove : CombatMoveBase
{
	public override string Description => "evading a mounted charge";
	public override double StaminaCost => SkirmishResponseMove.MoveStaminaCost(Assailant);
	public override double BaseDelay => 0.0;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		return new CombatMoveResult { MoveWasSuccessful = true };
	}
}

public sealed class CounterMountedChargeMove : CombatMoveBase
{
	public override string Description => "counter-charging a mounted attacker";
	public override double StaminaCost => ChargeToMeleeMove.MoveStaminaCost(Assailant);
	public override double BaseDelay => 0.0;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		return new CombatMoveResult { MoveWasSuccessful = true };
	}
}
