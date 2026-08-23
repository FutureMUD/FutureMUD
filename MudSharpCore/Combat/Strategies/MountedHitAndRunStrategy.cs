#nullable enable

using MudSharp.Combat.Moves;

namespace MudSharp.Combat.Strategies;

public sealed class MountedHitAndRunStrategy : MountedChargeStrategy
{
	private MountedHitAndRunStrategy()
	{
	}

	public new static MountedHitAndRunStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.MountedHitAndRun;

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
}
