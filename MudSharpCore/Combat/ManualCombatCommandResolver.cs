using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat;

public sealed record ManualCombatMoveResolution(bool Success, ICombatMove Move, string Error)
{
	public static ManualCombatMoveResolution Failed(string error)
	{
		return new ManualCombatMoveResolution(false, null, error);
	}

	public static ManualCombatMoveResolution Resolved(ICombatMove move)
	{
		return new ManualCombatMoveResolution(true, move, string.Empty);
	}
}

public static class ManualCombatCommandResolver
{
	public static ManualCombatMoveResolution TryResolve(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove = false)
	{
		if (actor.Combat is null)
		{
			return ManualCombatMoveResolution.Failed("This command is only usable when you are in combat.");
		}

		if (target is null)
		{
			return ManualCombatMoveResolution.Failed("You must specify a target for that command.");
		}

		if (actor == target)
		{
			return ManualCombatMoveResolution.Failed("You cannot target yourself with that combat command.");
		}

		if (!actor.Combat.Combatants.Contains(target))
		{
			return ManualCombatMoveResolution.Failed($"{target.HowSeen(actor, true)} is not in your combat.");
		}

		if (!command.IsUsableBy(actor, target))
		{
			return ManualCombatMoveResolution.Failed("You cannot use that combat command right now.");
		}

		var result = command.ActionKind switch
		{
			ManualCombatActionKind.AuxiliaryAction => ResolveAuxiliary(actor, command, target, returnTooExhaustedMove),
			ManualCombatActionKind.WeaponAttack => ResolveWeaponAttack(actor, command, target, returnTooExhaustedMove),
			_ => ManualCombatMoveResolution.Failed("That manual combat command is not configured with a valid action kind.")
		};
		if (result.Success)
		{
			actor.CombatTarget = target;
		}

		return result;
	}

	public static double AiWeightMultiplier(ICharacter combatant, IWeaponAttack attack)
	{
		if (combatant.CombatTarget is not ICharacter target)
		{
			return 1.0;
		}

		var multipliers = combatant.Gameworld.ManualCombatCommands
		                           .Where(x => x.NpcUsable)
		                           .Where(x => x.ActionKind == ManualCombatActionKind.WeaponAttack)
		                           .Where(x => x.WeaponAttack?.Id == attack.Id)
		                           .Where(x => x.IsUsableBy(combatant, target))
		                           .Select(combatant.CombatSettings.ManualCombatCommandWeightMultiplier)
		                           .ToList();
		return multipliers.Any() ? multipliers.Max() : 1.0;
	}

	public static double AiWeightMultiplier(ICharacter combatant, IAuxiliaryCombatAction action)
	{
		if (combatant.CombatTarget is not ICharacter target)
		{
			return 1.0;
		}

		var multipliers = combatant.Gameworld.ManualCombatCommands
		                           .Where(x => x.NpcUsable)
		                           .Where(x => x.ActionKind == ManualCombatActionKind.AuxiliaryAction)
		                           .Where(x => x.AuxiliaryAction?.Id == action.Id)
		                           .Where(x => x.IsUsableBy(combatant, target))
		                           .Select(combatant.CombatSettings.ManualCombatCommandWeightMultiplier)
		                           .ToList();
		return multipliers.Any() ? multipliers.Max() : 1.0;
	}

	private static ManualCombatMoveResolution ResolveAuxiliary(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove)
	{
		var action = command.AuxiliaryAction;
		if (action is null)
		{
			return ManualCombatMoveResolution.Failed("That manual combat command does not have an auxiliary action configured.");
		}

		var available = actor.Race.UsableAuxiliaryMoves(actor, target, false)
		                     .FirstOrDefault(x => x.Id == action.Id);
		if (available is null)
		{
			return ManualCombatMoveResolution.Failed("Your race cannot use that auxiliary move against that target right now.");
		}

		if (!actor.CanSpendStamina(AuxiliaryMove.MoveStaminaCost(actor, available)))
		{
			return returnTooExhaustedMove
				? ManualCombatMoveResolution.Resolved(new TooExhaustedMove { Assailant = actor })
				: ManualCombatMoveResolution.Failed("You are too exhausted to use that auxiliary move right now.");
		}

		return ManualCombatMoveResolution.Resolved(new AuxiliaryMove(actor, target, available));
	}

	private static ManualCombatMoveResolution ResolveWeaponAttack(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove)
	{
		var attack = command.WeaponAttack;
		if (attack is null)
		{
			return ManualCombatMoveResolution.Failed("That manual combat command does not have a weapon attack configured.");
		}

		var possibleWeaponAttacks = WeaponSourcesInPreferenceOrder(actor)
		                            .SelectMany(x => x.Weapon.WeaponType
		                                             .UsableAttacks(actor, x.Weapon.Parent, target,
			                                             x.Weapon.HandednessForWeapon(actor), false, attack.MoveType)
		                                             .Where(y => y.Id == attack.Id)
		                                             .Select(y => (x.Weapon, Attack: y)))
		                            .ToList();
		var possibleNaturalAttacks = actor.Race
		                                  .UsableNaturalWeaponAttacks(actor, target, false, attack.MoveType)
		                                  .Where(x => x.Attack.Id == attack.Id)
		                                  .ToList();

		var weaponAttack = possibleWeaponAttacks
		                   .FirstOrDefault(x => actor.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(actor, x.Attack)));
		if (weaponAttack.Attack is not null)
		{
			return ManualCombatMoveResolution.Resolved(
				CombatMoveFactory.CreateWeaponAttack(actor, weaponAttack.Weapon, weaponAttack.Attack, target));
		}

		var naturalAttack = possibleNaturalAttacks
		                    .FirstOrDefault(x => actor.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(actor, x.Attack)));
		if (naturalAttack is not null)
		{
			return ManualCombatMoveResolution.Resolved(
				CombatMoveFactory.CreateNaturalWeaponAttack(actor, naturalAttack, target));
		}

		if (possibleWeaponAttacks.Any() || possibleNaturalAttacks.Any())
		{
			return returnTooExhaustedMove
				? ManualCombatMoveResolution.Resolved(new TooExhaustedMove { Assailant = actor })
				: ManualCombatMoveResolution.Failed("You are too exhausted to use that attack right now.");
		}

		return ManualCombatMoveResolution.Failed("You do not currently have an available attack matching that manual combat command.");
	}

	private static IEnumerable<(IMeleeWeapon Weapon, int Preference)> WeaponSourcesInPreferenceOrder(ICharacter actor)
	{
		var preferenceIndex = 0;
		foreach (var preference in actor.CombatSettings.MeleeAttackOrderPreferences)
		{
			IEnumerable<IMeleeWeapon> weapons = preference switch
			{
				MeleeAttackOrderPreference.Weapon => actor.Body.WieldedItems
				                                           .SelectNotNull(x => x.GetItemType<IMeleeWeapon>()),
				MeleeAttackOrderPreference.Implant => actor.Body.Implants
				                                            .OfType<IImplantMeleeWeapon>()
				                                            .Where(x => x.WeaponIsActive)
				                                            .Cast<IMeleeWeapon>(),
				MeleeAttackOrderPreference.Prosthetic => actor.Body.Prosthetics
				                                               .OfType<IProstheticMeleeWeapon>()
				                                               .Where(x => x.WeaponIsActive)
				                                               .Cast<IMeleeWeapon>(),
				_ => Enumerable.Empty<IMeleeWeapon>()
			};

			foreach (var weapon in weapons.Where(x => CanUseWeaponSource(actor, x)))
			{
				yield return (weapon, preferenceIndex);
			}

			preferenceIndex++;
		}
	}

	private static bool CanUseWeaponSource(ICharacter actor, IMeleeWeapon weapon)
	{
		return weapon is not null &&
		       actor.CombatSettings.ClassificationsAllowed.Contains(weapon.Classification) &&
		       (actor.Combat?.Friendly != true ||
		        weapon.Classification.In(WeaponClassification.Training, WeaponClassification.NonLethal));
	}
}
