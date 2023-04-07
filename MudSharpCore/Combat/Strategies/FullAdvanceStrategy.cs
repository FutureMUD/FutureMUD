using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Strategies;

public class FullAdvanceStrategy : RangeBaseStrategy
{
	protected FullAdvanceStrategy()
	{
	}

	public static FullAdvanceStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.FullAdvance;

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
			    (ch.PositionState.TransitionOnMovement ?? ch.PositionState) != position)
			{
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

				if (ch.CanSpendStamina(ChargeToMeleeMove.MoveStaminaCost(ch)))
				{
					return new ChargeToMeleeMove { Assailant = ch };
				}

				return ch.CanSpendStamina(MoveToMeleeMove.MoveStaminaCost(ch))
					? (ICombatMove)new MoveToMeleeMove { Assailant = ch }
					: new TooExhaustedMove { Assailant = ch };
			}
		}

		return null;
	}

	protected override IEnumerable<IRangedWeapon> GetNotReadyButLoadableWeapons(ICharacter shooter)
	{
		return Enumerable.Empty<IRangedWeapon>();
	}

	protected override IEnumerable<IRangedWeapon> GetReadyRangedWeapons(ICharacter shooter)
	{
		return Enumerable.Empty<IRangedWeapon>();
	}

	protected override ICombatMove AttemptGetRangedWeapon(ICharacter ch)
	{
		return null;
	}

	#region Overrides of RangeBaseStrategy

	protected override ICombatMove HandleGeneralAttacks(ICharacter combatant)
	{
		return null;
	}

	#endregion
}