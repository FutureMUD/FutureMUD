using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp.Combat.Strategies;

public class DropperStrategy : StandardMeleeStrategy
{
	protected DropperStrategy()
	{
	}

	public new static DropperStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.Dropper;

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target || !CanDropTarget(ch, target))
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		var move = base.HandleCombatMovement(combatant);
		if (move is not null)
		{
			return move;
		}

		return TryCarryHigher(ch, target);
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target || !CanDropTarget(ch, target))
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		if (!CombatForcedMovementUtilities.HasControlledGrapple(ch, target))
		{
			return GrappleForControlStrategy.Instance.ChooseMove(combatant);
		}

		var higher = TryCarryHigher(ch, target);
		if (higher is not null)
		{
			return higher;
		}

		if (!ch.Location.Terrain(ch).TerrainLayers.Any(x => x.IsHigherThan(ch.RoomLayer)))
		{
			return new DropGrappledTargetMove(ch, target);
		}

		return base.HandleAttacks(combatant);
	}

	private static bool CanDropTarget(ICharacter ch, ICharacter target)
	{
		return ch.CanFly().Truth &&
		       ch.MaximumDragWeight >= target.Weight &&
		       target.Location == ch.Location;
	}

	private static ICombatMove TryCarryHigher(ICharacter ch, ICharacter target)
	{
		if (!CombatForcedMovementUtilities.HasControlledGrapple(ch, target))
		{
			return null;
		}

		var higherLayers = ch.Location.Terrain(ch).TerrainLayers
		                     .Where(x => x.IsHigherThan(ch.RoomLayer))
		                     .Where(ch.CouldTransitionToLayer)
		                     .OrderBy(x => x.LayerHeight())
		                     .ToList();
		if (!higherLayers.Any())
		{
			return null;
		}

		var desired = higherLayers.First();
		var choice = CombatForcedMovementUtilities.FindBestForcedMovementAttack(ch, target, ForcedMovementVerbs.Pull,
			ForcedMovementTypes.Layer);
		return choice is null
			? null
			: new ForcedMovementMove(ch, target, choice.Attack, ForcedMovementVerbs.Pull, desired)
			{
				Weapon = choice.Weapon,
				NaturalAttack = choice.NaturalAttack
			};
	}
}
