using MudSharp.Framework;

namespace MudSharp.Combat.Strategies;

public class FullDefenseStrategy : StandardMeleeStrategy
{
	protected FullDefenseStrategy()
	{
	}

	public new static FullDefenseStrategy Instance { get; } = new();
	public override CombatStrategyMode Mode => CombatStrategyMode.FullDefense;

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		return null; // TODO - auxillaries
	}
}