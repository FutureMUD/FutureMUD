using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.Combat.Moves;

namespace MudSharp.Combat.Strategies;

public class CoverAndAdvanceStrategy : CoverSeekingRangedStrategy
{
	protected CoverAndAdvanceStrategy()
	{
	}

	public static CoverAndAdvanceStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.CoverAndAdvance;

	protected override bool RequireMovingCover => true;

	protected override bool ShouldCharacterStand(ICharacter ch)
	{
		return false; // Standing is managed in HandleCombatMovement
	}

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		ICombatMove move = null;
		if ((move = base.HandleCombatMovement(combatant)) != null)
		{
			return move;
		}

		if (!combatant.CombatSettings.MovementManagement.In(AutomaticMovementSettings.FullyAutomatic,
			    AutomaticMovementSettings.KeepRange))
		{
			return null;
		}

		if (combatant.CombatTarget == null || combatant.MeleeRange)
		{
			return null;
		}

		if (combatant is ICharacter ch)
		{
			if (ch.Movement != null)
			{
				return null;
			}

			var position = ch.MostUprightMobilePosition();
			if (!ch.PositionState.In(PositionFlying.Instance, PositionSwimming.Instance) && position != null &&
			    ch.PositionState.TransitionOnMovement != position)
			{
				if (ch.Cover?.Cover.HighestPositionState.CompareTo(position).In(PositionHeightComparison.Lower,
					    PositionHeightComparison.Undefined) ?? false)
				{
					if (ch.Cover.Cover.HighestPositionState.CompareTo(PositionProstrate.Instance)
					      .In(PositionHeightComparison.Higher, PositionHeightComparison.Equivalent) &&
					    ch.CouldMove(true, PositionProstrate.Instance).Success)
					{
						return new ChangePositionMove { Assailant = ch, DesiredState = PositionProstrate.Instance };
					}

					if (ch.Cover.Cover.HighestPositionState.CompareTo(PositionProne.Instance)
					      .In(PositionHeightComparison.Higher, PositionHeightComparison.Equivalent) &&
					    ch.CouldMove(true, PositionProne.Instance).Success)
					{
						return new ChangePositionMove { Assailant = ch, DesiredState = PositionProne.Instance };
					}
				}

				return new ChangePositionMove { Assailant = ch, DesiredState = position };
			}

			if (ch.CanMove())
			{
				if (ch.CombatTarget.Location != ch.Location)
				{
					if (ch.CombatSettings.AutomaticallyMoveTowardsTarget)
					{
						var path = ch.PathBetween(ch.CombatTarget, 10, GetPathFunction(ch)).ToList();
						if (path.Any() && ch.CanMove(path.First()))
						{
							return new CombatMoveRoom { Assailant = ch, Direction = path.First() };
						}
					}

					return null;
				}

				if (ch.CombatTarget.RoomLayer != ch.RoomLayer)
				{
					if (ch.CombatSettings.AutomaticallyMoveTowardsTarget)
					{
						return HandleChangeLayer(ch);
					}

					return null;
				}

				// Cover and Advance does not charge because it makes them leave cover

				return ch.CanSpendStamina(MoveToMeleeMove.MoveStaminaCost(ch))
					? (ICombatMove)new MoveToMeleeMove { Assailant = ch }
					: new TooExhaustedMove { Assailant = ch };
			}
		}

		return null;
	}
}