using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Effects.Interfaces;
using MudSharp.Combat.Moves;
using MudSharp.Character;
using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Strategies;

public class FleeStrategy : StandardMeleeStrategy
{
	protected FleeStrategy()
	{
	}

	public new static FleeStrategy Instance { get; } = new();

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		ICombatMove move;
		if ((move = base.HandleCombatMovement(combatant)) != null)
		{
			return move;
		}

		if (combatant is not ICharacter ch || combatant.EffectsOfType<IRageEffect>().Any())
		{
			return null;
		}

		if (ch.CombinedEffectsOfType<IBeingGrappled>().Any() ||
		    ch.CombinedEffectsOfType<Dragging.DragTarget>().Any())
		{
			return null;
		}

		return new FleeMove { Assailant = ch };
	}

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)) && !defender.MeleeRange &&
		    defender.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging))
		{
			return new SkirmishResponseMove { Assailant = defender };
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
	}

	protected override ICombatMove ResponseToMoveToMelee(MoveToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)) && !defender.MeleeRange &&
		    defender.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging))
		{
			return new SkirmishResponseMove { Assailant = defender };
		}

		return base.ResponseToMoveToMelee(move, defender, assailant);
	}
}