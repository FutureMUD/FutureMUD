using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.Combat.Strategies;

public class ClinchStrategy : StandardMeleeStrategy
{
	protected ClinchStrategy()
	{
	}

	public new static ClinchStrategy Instance { get; } = new();

	#region Overrides of StandardMeleeStrategy

	public override CombatStrategyMode Mode => CombatStrategyMode.Clinch;

	protected override ICombatMove ResponseToStartClinch(StartClinchMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (assailant == defender.CombatTarget)
		{
			return new HelplessDefenseMove { Assailant = defender };
		}

		return base.ResponseToStartClinch(move, defender, assailant);
	}

	protected override ICombatMove ResponseToBreakClinch(BreakClinchMove move, ICharacter defender,
	        IPerceiver assailant)
	{
		if (defender.PositionState.CompareTo(PositionStanding.Instance) != PositionHeightComparison.Equivalent ||
	            !defender.CanSpendStamina(StartClinchMove.MoveStaminaCost(defender)))
		{
			return new HelplessDefenseMove
			{
				Assailant = defender
			};
		}

		if (move.Assailant is not ICharacter target ||
	            !StartClinchMove.CanClinchWhileMounted(defender, target))
		{
			return new HelplessDefenseMove
			{
				Assailant = defender
			};
		}

		return new StartClinchMove(target) { Assailant = defender };
	}

	#endregion

	protected virtual ICombatMove AttemptClinchAttack(ICharacter ch)
	{
		var attacks = new List<IWeaponAttack>();
		IMeleeWeapon weapon = null;
		var weapons = new List<IMeleeWeapon>();
		var hadPossibleAttacks = false;
		foreach (var preference in ch.CombatSettings.MeleeAttackOrderPreferences)
		{
			switch (preference)
			{
				case MeleeAttackOrderPreference.Weapon:
					weapons = ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).ToList();
					if (!weapons.Any())
					{
						var wieldableClinchItems = ch
						                           .Body.HeldItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
						                           .Where(
							                           x =>
								                           x.WeaponType.UsableAttacks(
									                           ch, x.Parent, ch.CombatTarget,
									                           x.HandednessForWeapon(ch), false,
									                           BuiltInCombatMoveType.ClinchAttack).Any())
						                           .Where(x => IsUseableWeapon(ch, x) && ch.Body.CanWield(x.Parent))
						                           .ToList();

						if (wieldableClinchItems.Any())
						{
							return new WieldMove { Assailant = ch, Item = wieldableClinchItems.GetRandomElement() };
						}

						var drawableClinchItems = ch.Body.ExternalItems
						                            .SelectNotNull(x => x.GetItemType<ISheath>())
						                            .SelectNotNull(
							                            x => x.Content?.Parent.GetItemType<IMeleeWeapon>())
						                            .Where(x => IsUseableWeapon(ch, x))
						                            .Select(x => (Weapon: x,
							                            Options: x.WeaponType.UseableHandednessOptions(
								                            ch, x.Parent, ch.CombatTarget,
								                            BuiltInCombatMoveType
									                            .ClinchAttack)))
						                            .Select(x => x.Weapon).ToList();

						if (drawableClinchItems.Any())
						{
							return new DrawAndWieldMove
							{
								Assailant = ch,
								Weapon = drawableClinchItems.GetRandomElement()
							};
						}

						continue;
					}

					break;
				case MeleeAttackOrderPreference.Implant:
					weapons = ch.Body.Implants
					            .OfType<IImplantMeleeWeapon>()
					            .Where(x => x.WeaponIsActive)
					            .Cast<IMeleeWeapon>()
					            .ToList();
					if (!weapons.Any())
					{
						continue;
					}

					break;
				case MeleeAttackOrderPreference.Prosthetic:
					weapons = ch.Body.Prosthetics
					            .OfType<IProstheticMeleeWeapon>()
					            .Where(x => x.WeaponIsActive)
					            .Cast<IMeleeWeapon>()
					            .ToList();
					if (!weapons.Any())
					{
						continue;
					}

					break;
				default:
					continue;
			}

			while (true)
			{
				weapon = weapons.GetRandomElement();
				if (weapon == null)
				{
					continue;
				}

				weapons.Remove(weapon);
				var possibleAttacks = weapon
				                      .WeaponType.UsableAttacks(ch, weapon.Parent, ch.CombatTarget,
					                      weapon.HandednessForWeapon(ch), false,
					                      BuiltInCombatMoveType.ClinchAttack).ToList();
				attacks = possibleAttacks.Where(x => ch.CanSpendStamina(x.StaminaCost)).ToList();
				if (possibleAttacks.Any())
				{
					hadPossibleAttacks = true;
				}

				if (!attacks.Any() && weapons.Any())
				{
					continue;
				}

				break;
			}
		}

		if (!attacks.Any())
		{
			if (hadPossibleAttacks)
			{
				return new TooExhaustedMove { Assailant = ch };
			}

			if (
				ch.CombatStrategyMode == CombatStrategyMode.Clinch &&
				!HasViableClinchAttack(ch) &&
				HasViableMeleeAttack(ch)
			)
			{
				ch.Send("You realise you have no way to attack your target up close and resolve to move back to distance.");
				ch.CombatStrategyMode = CombatStrategyMode.StandardMelee;
				return CombatStrategyFactory.GetStrategy(CombatStrategyMode.StandardMelee).ChooseMove(ch);
			}

			return null;
		}

		var preferredAttacks = attacks.Where(x => x.Intentions.HasFlag(ch.CombatSettings.PreferredIntentions)).ToList();
		if (preferredAttacks.Any() && Dice.Roll(1, 2) == 1)
		{
			attacks = preferredAttacks;
		}

		var attack = attacks.GetWeightedRandom(x => x.Weighting);
		if (attack == null)
		{
			return null;
		}

		if (ch.CombatTarget is ICharacter charTarget)
		{
			return new ClinchAttackMove(ch, charTarget, attack, weapon);
		}

		throw new NotImplementedException("Unimplemented Combatant type in ClinchStrategy.AttemptClinchAttack - " +
		                                  ch.CombatTarget.GetType().FullName);
	}

	private ICombatMove AttemptUnarmedClinchAttack(ICharacter ch)
	{
		var possibleAttacks =
			ch.Race.UsableNaturalWeaponAttacks(ch, ch.CombatTarget, false, BuiltInCombatMoveType.ClinchUnarmedAttack,
				BuiltInCombatMoveType.StaggeringBlowClinch, BuiltInCombatMoveType.UnbalancingBlowClinch,
				BuiltInCombatMoveType.EnvenomingAttackClinch).ToList();
		var attacks = possibleAttacks.Where(x => ch.CanSpendStamina(x.Attack.StaminaCost)).ToList();

		if (!attacks.Any())
		{
			return possibleAttacks.Any() ? new TooExhaustedMove { Assailant = ch } : null;
		}

		var preferredAttacks =
			attacks.Where(x => x.Attack.Intentions.HasFlag(ch.CombatSettings.PreferredIntentions)).ToList();
		if (preferredAttacks.Any() && Dice.Roll(1, 2) == 1)
		{
			attacks = preferredAttacks;
		}

		var attack = attacks.GetWeightedRandom(x => x.Attack.Weighting);
		if (attack == null)
		{
			return null;
		}

		if (ch.CombatTarget is ICharacter charTarget)
		{
			return new ClinchNaturalAttackMove(ch, charTarget, attack, null);
		}

		throw new NotImplementedException(
			"Unimplemented Combatant type in ClinchStrategy.AttemptUnarmedClinchAttack - " +
			ch.CombatTarget.GetType().FullName);
	}

	public bool HasViableClinchAttack(ICharacter ch)
	{
		foreach (var preference in ch.CombatSettings.MeleeAttackOrderPreferences)
		{
			IEnumerable<IMeleeWeapon> weapons = Enumerable.Empty<IMeleeWeapon>();
			switch (preference)
			{
				case MeleeAttackOrderPreference.Weapon:
					weapons = ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>());
					break;
				case MeleeAttackOrderPreference.Implant:
					weapons = ch.Body.Implants.OfType<IImplantMeleeWeapon>().Where(x => x.WeaponIsActive);
					break;
				case MeleeAttackOrderPreference.Prosthetic:
					weapons = ch.Body.Prosthetics.OfType<IProstheticMeleeWeapon>().Where(x => x.WeaponIsActive);
					break;
				default:
					continue;
			}

			if (weapons.Any(x => x.WeaponType.UsableAttacks(ch, x.Parent, ch.CombatTarget,
					x.HandednessForWeapon(ch), true, BuiltInCombatMoveType.ClinchAttack).Any()))
			{
				return true;
			}
		}

		if (ch.Body.HeldItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
				.Any(x => x.WeaponType.UsableAttacks(ch, x.Parent, ch.CombatTarget, x.HandednessForWeapon(ch), true, BuiltInCombatMoveType.ClinchAttack).Any() && IsUseableWeapon(ch, x)))
		{
			return true;
		}

		if (ch.Body.ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
				.SelectNotNull(x => x.Content?.Parent.GetItemType<IMeleeWeapon>())
				.Any(x => IsUseableWeapon(ch, x) && x.WeaponType.UsableAttacks(ch, x.Parent, ch.CombatTarget,
						x.HandednessForWeapon(ch), true, BuiltInCombatMoveType.ClinchAttack).Any()))
		{
			return true;
		}

		var natAttacks = ch.Race.UsableNaturalWeaponAttacks(ch, ch.CombatTarget, false,
				BuiltInCombatMoveType.ClinchUnarmedAttack, BuiltInCombatMoveType.StaggeringBlowClinch,
				BuiltInCombatMoveType.UnbalancingBlowClinch, BuiltInCombatMoveType.EnvenomingAttackClinch);
		return natAttacks.Any();
	}

	private ICombatMove CheckClinching(ICharacter ch)
	{
		if (ch.PositionState.Upright &&
	            !ch.EffectsOfType<ClinchEffect>().Any() &&
	            !ch.EffectsOfType<ClinchCooldown>().Any() &&
	            ch.CombatTarget is ICharacter charTarget &&
	            charTarget.EffectsOfType<ClinchEffect>().All(x => x.Target != ch))
		{
			if (!StartClinchMove.CanClinchWhileMounted(ch, charTarget))
			{
				if (ch.CombatStrategyMode == CombatStrategyMode.Clinch && HasViableMeleeAttack(ch))
				{
					ch.Send($"You cannot reach {charTarget.HowSeen(ch)} well enough to clinch while you are mounted, so you fight normally.");
					ch.CombatStrategyMode = CombatStrategyMode.StandardMelee;
					return CombatStrategyFactory.GetStrategy(CombatStrategyMode.StandardMelee).ChooseMove(ch);
				}

				return null;
			}

			if (ch.CanSpendStamina(StartClinchMove.MoveStaminaCost(ch)))
			{
				return new StartClinchMove(charTarget) { Assailant = ch };
			}
		}

		if (ch.EffectsOfType<ClinchEffect>().Any(x => x.Target == ch.CombatTarget))
		{
			var roll = Constants.Random.NextDouble();
			if (ch.CombatSettings.WeaponUsePercentage > 0 &&
	                    roll <= ch.CombatSettings.WeaponUsePercentage)
			{
				return AttemptClinchAttack(ch);
			}

			roll -= ch.CombatSettings.WeaponUsePercentage;
			if (ch.CombatSettings.NaturalWeaponPercentage > 0.0 &&
	                    roll <= ch.CombatSettings.NaturalWeaponPercentage)
			{
				return AttemptUnarmedClinchAttack(ch);
			}
		}

		return null;
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		ICombatMove move;
		if (combatant is ICharacter ch && (move = CheckClinching(ch)) != null)
		{
			return move;
		}

		return base.HandleAttacks(combatant);
	}

	protected override ICombatMove HandleClinchBreaking(ICharacter ch, bool canMove)
	{
		return null;
	}

	protected override Func<IGameItem, double> MeleeWeaponFitnessFunction(ICharacter ch)
	{
		double InternalFunction(IGameItem item)
		{
			return item?.GetItemType<IMeleeWeapon>()?.WeaponType.UsableAttacks(ch, item, ch.CombatTarget,
				AttackHandednessOptions.Any, false, BuiltInCombatMoveType.ClinchAttack).Any() == true
				? 100
				: 1 * base.MeleeWeaponFitnessFunction(ch)(item);
		}

		return InternalFunction;
	}
}