using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Strategies;

public class DrownerStrategy : StandardMeleeStrategy
{
	protected DrownerStrategy()
	{
	}

	public new static DrownerStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.Drowner;

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target)
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		if (TargetIsInnatelyWaterSafe(target))
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		var move = base.HandleCombatMovement(combatant);
		if (move is not null)
		{
			return move;
		}

		return TryMoveTargetIntoWater(ch, target);
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target)
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		if (TargetIsInnatelyWaterSafe(target))
		{
			return StandardMeleeStrategy.Instance.ChooseMove(combatant);
		}

		if (target.Location.IsUnderwaterLayer(target.RoomLayer))
		{
			return GrappleForControlStrategy.Instance.ChooseMove(combatant);
		}

		return TryMoveTargetIntoWater(ch, target) ?? base.HandleAttacks(combatant);
	}

	private static bool TargetIsInnatelyWaterSafe(ICharacter target)
	{
		if (!target.Race.NeedsToBreathe)
		{
			return true;
		}

		return LocalWaterFluids(target).Any(x => target.Race.CanBreatheFluid(x).Truth);
	}

	private static IEnumerable<IFluid> LocalWaterFluids(ICharacter target)
	{
		var current = target.Location?.Terrain(target)?.WaterFluid;
		if (current is not null)
		{
			yield return current;
		}

		if (target.Location is null)
		{
			yield break;
		}

		foreach (var fluid in target.Location
		                            .ExitsFor(target, true)
		                            .Select(x => x.Destination.Terrain(target)?.WaterFluid)
		                            .Where(x => x is not null)
		                            .Distinct())
		{
			yield return fluid;
		}
	}

	private static ICombatMove TryMoveTargetIntoWater(ICharacter ch, ICharacter target)
	{
		var underwaterLayer = target.Location.Terrain(target).TerrainLayers
		                            .Where(x => x.IsUnderwater())
		                            .OrderByDescending(x => x.LayerHeight())
		                            .FirstOrDefault();
		if (underwaterLayer != default && underwaterLayer != target.RoomLayer)
		{
			var move = TryForcedLayerMove(ch, target, underwaterLayer, ForcedMovementVerbs.Pull) ??
			           TryForcedLayerMove(ch, target, underwaterLayer, ForcedMovementVerbs.Shove);
			if (move is not null)
			{
				return move;
			}
		}

		var waterExit = target.Location
		                      .ExitsFor(target, true)
		                      .Where(x => x.Destination.Terrain(target).TerrainLayers.Any(y => y.IsUnderwater()))
		                      .Where(x => x.MovementTransition(target).TransitionType != CellMovementTransition.NoViableTransition)
		                      .GetRandomElement();
		if (waterExit is null)
		{
			return null;
		}

		return TryForcedExitMove(ch, target, waterExit, ForcedMovementVerbs.Pull) ??
		       TryForcedExitMove(ch, target, waterExit, ForcedMovementVerbs.Shove);
	}

	private static ICombatMove TryForcedLayerMove(ICharacter ch, ICharacter target, RoomLayer layer, ForcedMovementVerbs verb)
	{
		var choice = CombatForcedMovementUtilities.FindBestForcedMovementAttack(ch, target, verb, ForcedMovementTypes.Layer);
		return choice is null
			? null
			: new ForcedMovementMove(ch, target, choice.Attack, verb, layer)
			{
				Weapon = choice.Weapon,
				NaturalAttack = choice.NaturalAttack
			};
	}

	private static ICombatMove TryForcedExitMove(ICharacter ch, ICharacter target, ICellExit exit, ForcedMovementVerbs verb)
	{
		var choice = CombatForcedMovementUtilities.FindBestForcedMovementAttack(ch, target, verb, ForcedMovementTypes.Exit);
		return choice is null
			? null
			: new ForcedMovementMove(ch, target, choice.Attack, verb, exit)
			{
				Weapon = choice.Weapon,
				NaturalAttack = choice.NaturalAttack
			};
	}
}
