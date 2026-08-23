#nullable enable

using MudSharp.Combat.Moves;

namespace MudSharp.Combat.Strategies;

public sealed class MountedSkirmishStrategy : SkirmishStrategy
{
	private MountedSkirmishStrategy()
	{
	}

	public new static MountedSkirmishStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.MountedSkirmish;

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant is ICharacter character && character.MeleeRange &&
		    MountedCombatService.Instance.ResolveContext(character) is not null &&
		    character.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(character)))
		{
			return new MountedDisengageMove { Assailant = character };
		}

		return base.HandleCombatMovement(combatant);
	}

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (move.IsMountedCharge && MountedCombatService.Instance.ResolveContext(defender) is not null &&
		    defender.CanSpendStamina(SkirmishResponseMove.MoveStaminaCost(defender)))
		{
			return new EvadeMountedChargeMove { Assailant = defender, PrimaryTarget = assailant };
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
	}
}
