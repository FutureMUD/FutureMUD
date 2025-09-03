using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;

namespace MudSharp.Combat.Strategies;

public class FireAndAdvanceStrategy : RangeBaseStrategy
{
	protected FireAndAdvanceStrategy()
	{
	}

	public static FireAndAdvanceStrategy Instance { get; } = new();

	#region Overrides of StandardRangedStrategy

	public override CombatStrategyMode Mode => CombatStrategyMode.FireAndAdvance;

	protected override ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var rangedWeapons = GetReadyRangedWeapons(defender).ToList();
		if (!defender.IsEngagedInMelee && rangedWeapons.Any())
		{
			return new StandAndFireMove(defender, assailant as ICharacter, rangedWeapons.First());
		}

		return base.ResponseToChargeToMelee(move, defender, assailant);
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

			var rangedWeapons = GetReadyRangedWeapons(ch).ToList();
			if (ch.Aim != null && rangedWeapons.Contains(ch.Aim.Weapon) && Dice.Roll(1, 6) <= 5)
			{
				return null;
			}

			if (ch.Aim == null)
			{
				var shootpath = ch.PathBetween(ch.CombatTarget, 10, false, false, true).ToList();
				if ((shootpath.Any() ||
				     (ch.Location == ch.CombatTarget.Location && ch.RoomLayer != ch.CombatTarget.RoomLayer)) &&
				    Dice.Roll(1, 2) == 1)
				{
					var weapon = rangedWeapons
					             .Where(x => x.WeaponType.DefaultRangeInRooms >= shootpath.Count)
					             .OrderByDescending(x => ch.GetTrait(x.Trait)?.Value)
					             .FirstOrDefault();
					if (weapon != null)
					{
						return new AimRangedWeaponMove(ch, ch.CombatTarget, weapon);
					}
				}
			}

			var position = ch.MostUprightMobilePosition();
			if (!ch.PositionState.In(PositionFlying.Instance, PositionSwimming.Instance) && position != null &&
			    (ch.PositionState.TransitionOnMovement ?? ch.PositionState) != position)
			{
				return new ChangePositionMove { Assailant = ch, DesiredState = position };
			}

			if (ch.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers))
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

				if (rangedWeapons.Any() && ch.CanSpendStamina(FireAndAdvanceToMeleeMove.MoveStaminaCost(ch)))
				{
					move = new FireAndAdvanceToMeleeMove(ch, ch.CombatTarget as ICharacter, rangedWeapons.First());
					if (ch.CanSpendStamina(move.StaminaCost))
					{
						return move;
					}
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

	protected override ICombatMove CheckWeaponryLoadout(ICharacter ch)
	{
		if (ch.CombatSettings.WeaponUsePercentage > 0.0 && ch.Race.CombatSettings.CanUseWeapons &&
		    !ch.Body.WieldedItems.Any(x =>
			    x.GetItemType<IRangedWeapon>() is IRangedWeapon rw &&
			    ch.CombatSettings.ClassificationsAllowed.Contains(rw.Classification) && (rw.ReadyToFire ||
				    (!rw.IsLoaded && rw.CanLoad(ch, true)) || (!rw.IsReadied && rw.CanReady(ch)))))
		{
			return AttemptGetRangedWeapon(ch);
		}

		return base.CheckWeaponryLoadout(ch);
	}

	#endregion
}