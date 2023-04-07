using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;

namespace MudSharp.Combat.Strategies;

public class WardStrategy : StandardMeleeStrategy
{
	protected WardStrategy()
	{
	}

	public new static WardStrategy Instance { get; } = new();

	#region Overrides of StandardMeleeStrategy

	public override CombatStrategyMode Mode => CombatStrategyMode.Ward;

	protected override ICombatMove ResponseToStartClinch(StartClinchMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (!defender.CanSpendStamina(WardDefenseMove.MoveStaminaCost(defender)))
		{
			return base.ResponseToStartClinch(move, defender, assailant);
		}

		var wardWeapon =
			defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
			        .Where(
				        x =>
					        x.WeaponType
					         .UsableAttacks(defender, x.Parent, assailant, x.HandednessForWeapon(defender), false,
						         BuiltInCombatMoveType.WardFreeAttack)
					         .Any(y => defender.CanSpendStamina(y.StaminaCost)))
			        .FirstMax(x => x.WeaponType.Reach);
		if (wardWeapon != null ||
		    (defender.CombatSettings.FallbackToUnarmedIfNoWeapon &&
		     defender.Race.UsableNaturalWeaponAttacks(defender, assailant, false,
			             BuiltInCombatMoveType.WardFreeUnarmedAttack)
		             .Any(x => defender.CanSpendStamina(x.Attack.StaminaCost))))
		{
			return new WardDefenseMove
			{
				Assailant = defender,
				WardWeapon = wardWeapon
			};
		}

		return base.ResponseToStartClinch(move, defender, assailant);
	}

	protected override ICombatMove ResponseToMagicPowerAttackMove(MagicPowerAttackMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (!defender.EffectsOfType<IWardBeatenEffect>().Any() &&
		    defender.CanSpendStamina(WardDefenseMove.MoveStaminaCost(defender)) &&
		    assailant.PositionState.Upright
		   )
		{
			var wardWeapon =
				defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
				        .Where(
					        x =>
						        x.WeaponType.Attacks.Any(
							        y =>
								        y.MoveType == BuiltInCombatMoveType.WardFreeAttack &&
								        ((bool?)y.UsabilityProg?.Execute(defender, x.Parent) ?? true)))
				        .FirstMax(x => x.WeaponType.Reach);
			if (wardWeapon != null ||
			    (defender.CombatSettings.FallbackToUnarmedIfNoWeapon &&
			     defender.Race.NaturalWeaponAttacks.Any(
				     x =>
					     x.Attack.MoveType == BuiltInCombatMoveType.WardFreeUnarmedAttack &&
					     ((bool?)x.Attack.UsabilityProg?.Execute(defender) ?? true))))
			{
				return new WardDefenseMove
				{
					Assailant = defender,
					WardWeapon = wardWeapon
				};
			}
		}

		return base.ResponseToMagicPowerAttackMove(move, defender, assailant);
	}

	public override ICombatMove ResponseToWeaponAttack(IWeaponAttackMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (!defender.EffectsOfType<IWardBeatenEffect>().Any() &&
		    defender.CanSpendStamina(WardDefenseMove.MoveStaminaCost(defender)) &&
		    assailant.PositionState.Upright
		   )
		{
			var wardWeapon =
				defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
				        .Where(
					        x =>
						        x.WeaponType.Attacks.Any(
							        y =>
								        y.MoveType == BuiltInCombatMoveType.WardFreeAttack &&
								        ((bool?)y.UsabilityProg?.Execute(defender, x.Parent) ?? true)))
				        .FirstMax(x => x.WeaponType.Reach);
			if (wardWeapon != null ||
			    (defender.CombatSettings.FallbackToUnarmedIfNoWeapon &&
			     defender.Race.NaturalWeaponAttacks.Any(
				     x =>
					     x.Attack.MoveType == BuiltInCombatMoveType.WardFreeUnarmedAttack &&
					     ((bool?)x.Attack.UsabilityProg?.Execute(defender) ?? true))))
			{
				return new WardDefenseMove
				{
					Assailant = defender,
					WardWeapon = wardWeapon
				};
			}
		}

		return base.ResponseToWeaponAttack(move, defender, assailant);
	}

	protected override Func<IGameItem, double> MeleeWeaponFitnessFunction(ICharacter ch)
	{
		double InternalFunction(IGameItem item)
		{
			return item?.GetItemType<IMeleeWeapon>()?.WeaponType.UsableAttacks(ch, item, ch.CombatTarget,
				AttackHandednessOptions.Any, false, BuiltInCombatMoveType.WardFreeAttack).Any() == true
				? 100
				: 1 * base.MeleeWeaponFitnessFunction(ch)(item);
		}

		return InternalFunction;
	}

	#endregion
}