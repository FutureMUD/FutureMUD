#nullable enable

using MudSharp.Combat.Moves;

namespace MudSharp.Combat.Strategies;

public class MountedChargeStrategy : FullAdvanceStrategy
{
	protected MountedChargeStrategy()
	{
	}

	public new static MountedChargeStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.MountedCharge;

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant is ICharacter character &&
		    MountedCombatService.Instance.ResolveContext(character) is not null &&
		    character.CombatTarget is { } target && !character.MeleeRange && character.Location == target.Location &&
		    character.RoomLayer == target.RoomLayer && character.Movement is null &&
		    character.CombatSettings.MovementManagement.In(AutomaticMovementSettings.FullyAutomatic,
			    AutomaticMovementSettings.KeepRange))
		{
			return character.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(character))
				? new ChargeToMeleeMove { Assailant = character }
				: new TooExhaustedMove { Assailant = character };
		}

		return base.HandleCombatMovement(combatant);
	}

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (move.IsMountedCharge && MountedCombatService.Instance.ResolveContext(defender) is not null &&
		    defender.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(defender)))
		{
			return new CounterMountedChargeMove { Assailant = defender, PrimaryTarget = assailant };
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
	}
}
