using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Strategies;

public class SwooperStrategy : RangeBaseStrategy
{
    protected SwooperStrategy()
    {
    }

    public static SwooperStrategy Instance { get; } = new();
    public override CombatStrategyMode Mode => CombatStrategyMode.Swooper;

	private ICombatMove AttemptSwoopSkirmish(ICharacter defender, ICharacter assailant)
	{
		if (defender.PositionState != PositionFlying.Instance ||
			defender.MeleeRange ||
			!defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)) ||
			defender.EffectsOfType<IRageEffect>().Any(x => x.IsSuperRaging))
		{
			return null;
		}

		List<IRangedWeapon> rangedWeapons = GetReadyRangedWeapons(defender).ToList();
		return rangedWeapons.Any()
			? new SkirmishAndFire(defender, assailant, rangedWeapons.First())
			: new SkirmishResponseMove { Assailant = defender };
	}

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		return AttemptSwoopSkirmish(defender, move.Assailant) ?? base.ResponseToChargeToMelee(move, defender, assailant);
	}

	protected override ICombatMove ResponseToMoveToMelee(MoveToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		return AttemptSwoopSkirmish(defender, move.Assailant) ?? base.ResponseToMoveToMelee(move, defender, assailant);
	}

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		ICombatMove move = base.HandleCombatMovement(combatant);
		if (move != null)
		{
			return move;
		}

		if (combatant is not ICharacter ch)
		{
			return null;
		}

		switch (ch.CombatSettings.MovementManagement)
		{
			case AutomaticMovementSettings.FullyAutomatic:
			case AutomaticMovementSettings.KeepRange:
				break;
			case AutomaticMovementSettings.FullyManual:
			case AutomaticMovementSettings.SeekCoverOnly:
				return null;
		}

		if (ch.CombatTarget is null)
		{
			return null;
		}

		if (ch.Location == ch.CombatTarget.Location &&
			ch.RoomLayer != ch.CombatTarget.RoomLayer)
		{
			return HandleChangeLayer(ch);
		}

		if (ch.PositionState != PositionFlying.Instance && ch.CanFly().Truth)
		{
			return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.Fly);
		}

		return null;
	}

    protected override ICombatMove HandleGeneralAttacks(ICharacter combatant)
    {
        if (combatant.CombatTarget is ICharacter target &&
            combatant.PositionState == PositionFlying.Instance)
        {
            INaturalAttack breathAttack = combatant.Race
                                       .UsableNaturalWeaponAttacks(combatant, target, false,
                                           BuiltInCombatMoveType.BreathWeaponAttack)
                                       .Where(x => x.Attack.Weighting * ManualCombatCommandResolver.AiWeightMultiplier(combatant, x.Attack) > 0.0)
                                       .Where(x => combatant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(combatant, x.Attack)))
                                       .GetWeightedRandom(x => x.Attack.Weighting * ManualCombatCommandResolver.AiWeightMultiplier(combatant, x.Attack));
            if (breathAttack is not null && (combatant.Location != target.Location || combatant.RoomLayer != target.RoomLayer))
            {
                return new BreathSwoopAttackMove(combatant, breathAttack, target);
            }
        }

        return base.HandleGeneralAttacks(combatant);
    }
}
