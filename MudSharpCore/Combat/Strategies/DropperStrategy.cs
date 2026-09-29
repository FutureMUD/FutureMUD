using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Body.Position.PositionStates;

namespace MudSharp.Combat.Strategies;

public class DropperStrategy : StandardMeleeStrategy
{
	protected DropperStrategy()
	{
	}

	public new static DropperStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.Dropper;

	protected override ICombatMove HandleClinchBreaking(ICharacter ch, bool canMove)
	{
		return ch.CombatTarget is ICharacter target && CanDropTarget(ch, target)
			? null
			: base.HandleClinchBreaking(ch, canMove);
	}

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant.CombatTarget is { } distant &&
		    (!combatant.ColocatedWith(distant) || combatant.RoomLayer != distant.RoomLayer))
			return AttemptApproach(combatant);
		if (combatant is ICharacter carrier && carrier.CombatTarget is ICharacter prey &&
		    CombatForcedMovementUtilities.HasControlledGrapple(carrier, prey))
		{
			return CanDropTarget(carrier, prey)
				? TryCarryHigher(carrier, prey) ?? new DropGrappledTargetMove(carrier, prey)
				: new DropGrappledTargetMove(carrier, prey);
		}
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target || !CanDropTarget(ch, target))
		{
			return !combatant.MeleeRange ? AttemptApproach(combatant) : StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}
		if (!combatant.MeleeRange) return AttemptApproach(combatant);

		var move = base.HandleCombatMovement(combatant);
		if (move is not null)
		{
			return move;
		}

		return TryCarryHigher(ch, target);
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		if (!combatant.MeleeRange || combatant.CombatTarget is not { } nearby ||
		    !combatant.ColocatedWith(nearby) || combatant.RoomLayer != nearby.RoomLayer) return null;
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target || !CanDropTarget(ch, target))
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		if (!CombatForcedMovementUtilities.HasControlledGrapple(ch, target))
		{
			return GrappleForControlStrategy.Instance.AttemptGrappleForControlOnly(ch);
		}

		var higher = TryCarryHigher(ch, target);
		if (higher is not null)
		{
			return higher;
		}

		return new DropGrappledTargetMove(ch, target);
	}

	private static bool CanDropTarget(ICharacter ch, ICharacter target)
	{
		return CombatForcedMovementUtilities.CanCarryFlying(ch, target);
	}

	private static ICombatMove AttemptApproach(IPerceiver combatant)
	{
		if (combatant is ICharacter ch && ch.CombatTarget is { } target &&
		    ch.CombatSettings.MovementManagement.In(AutomaticMovementSettings.FullyAutomatic, AutomaticMovementSettings.KeepRange) &&
		    ch.CombatSettings.AutomaticallyMoveTowardsTarget && ch.Movement is null &&
		    ch.SharesLongitudinalVicinityWith(target) && ch.RoomLayer != target.RoomLayer &&
		    ch.PositionState == PositionFlying.Instance)
		{
			// Keep flying on the way to ground prey; landing in an intervening tree layer would drop the hunter.
			if (ch.RoomLayer.IsHigherThan(target.RoomLayer) && ((IFly)ch).CanDive().Truth)
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyDown);
			if (ch.RoomLayer.IsLowerThan(target.RoomLayer) && ((IFly)ch).CanAscend().Truth)
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyUp);
			return null;
		}
		return FullAdvanceStrategy.Instance.AttemptAdvance(combatant);
	}

	private static ICombatMove TryCarryHigher(ICharacter ch, ICharacter target)
	{
		if (!CombatForcedMovementUtilities.HasControlledGrapple(ch, target))
		{
			return null;
		}

		var higherLayers = ch.Location.Terrain(ch).TerrainLayers
		                     .Where(x => x.IsHigherThan(ch.RoomLayer))
		                     .Where(x => !x.IsUnderwater())
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
				RequiresFlight = true,
				NaturalAttack = choice.NaturalAttack
			};
	}
}
