using System.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;

namespace MudSharp.Combat.Strategies;

public class SwooperStrategy : RangeBaseStrategy
{
	protected SwooperStrategy()
	{
	}

	public static SwooperStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.Swooper;

	protected override ICombatMove HandleGeneralAttacks(ICharacter combatant)
	{
		if (combatant.CombatTarget is ICharacter target &&
		    combatant.PositionState == PositionFlying.Instance)
		{
			var breathAttack = combatant.Race
			                           .UsableNaturalWeaponAttacks(combatant, target, false,
				                           BuiltInCombatMoveType.BreathWeaponAttack)
			                           .Where(x => combatant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(combatant, x.Attack)))
			                           .GetWeightedRandom(x => x.Attack.Weighting);
			if (breathAttack is not null && (combatant.Location != target.Location || combatant.RoomLayer != target.RoomLayer))
			{
				return new BreathSwoopAttackMove(combatant, breathAttack, target);
			}
		}

		return base.HandleGeneralAttacks(combatant);
	}
}
