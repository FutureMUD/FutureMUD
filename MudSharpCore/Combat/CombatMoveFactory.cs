using System;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat;

public static class CombatMoveFactory
{
	public static ICombatMove CreateWeaponAttack(ICharacter assailant, IMeleeWeapon weapon, IWeaponAttack attack,
		ICharacter target)
	{
		switch (attack.MoveType)
		{
			case BuiltInCombatMoveType.CoupDeGrace:
				return new CoupDeGrace(attack, target) { Assailant = assailant, Weapon = weapon };
			case BuiltInCombatMoveType.UseWeaponAttack:
				return new MeleeWeaponAttack(assailant, weapon, attack, target);
			case BuiltInCombatMoveType.StaggeringBlow:
				return new StaggeringBlowMove(assailant, weapon, attack, target);
			case BuiltInCombatMoveType.UnbalancingBlow:
				return new UnbalancingBlowMove(assailant, weapon, attack, target);
			case BuiltInCombatMoveType.DownedAttack:
				return new DownedMeleeAttack(assailant, weapon, attack, target);
		}

		throw new ApplicationException(
			$"Invalid Move Type in CombatMoveFactory.CreateWeaponAttack: {attack.MoveType.Describe()}");
	}

	public static ICombatMove CreateWeaponAttack(ICharacter assailant, IMeleeWeapon weapon, IWeaponAttack attack,
		IGameItem target)
	{
		throw new ApplicationException(
			$"Invalid Move Type in CombatMoveFactory.CreateWeaponAttack: {attack.MoveType.Describe()}");
	}

	public static ICombatMove CreateNaturalWeaponAttack(ICharacter assailant, INaturalAttack attack,
		ICharacter target)
	{
		switch (attack.Attack.MoveType)
		{
			case BuiltInCombatMoveType.NaturalWeaponAttack:
				return new NaturalAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.EnvenomingAttack:
				return new EnvenomingAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				return new EnvenomingClinchAttack(assailant, target, attack, null);
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
				return new StaggeringBlowUnarmedMove(assailant, attack, target, false);
			case BuiltInCombatMoveType.StaggeringBlowClinch:
				return new StaggeringBlowUnarmedMove(assailant, attack, target, true);
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
				return new UnbalancingBlowUnarmedMove(assailant, attack, target, false);
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
				return new UnbalancingBlowUnarmedMove(assailant, attack, target, true);
			case BuiltInCombatMoveType.ScreechAttack:
				return new ScreechAttackMove(assailant, attack, null);
			case BuiltInCombatMoveType.DownedAttackUnarmed:
				return new UnarmedDownedMeleeAttack(assailant, attack, target);
			case BuiltInCombatMoveType.StrangleAttack:
				return new StrangleAttack(assailant, attack, target);
			case BuiltInCombatMoveType.TakedownMove:
				return new TakedownMove(assailant, attack, target);
		}

		throw new ApplicationException(
			$"Invalid Move Type in CombatMoveFactory.CreateNaturalWeaponAttack: {attack.Attack.MoveType.Describe()}");
	}

	public static ICombatMove CreateNaturalWeaponAttack(ICharacter assailant, INaturalAttack attack, IGameItem target)
	{
		throw new ApplicationException(
			$"Invalid Move Type in CombatMoveFactory.CreateNaturalWeaponAttack: {attack.Attack.MoveType.Describe()}");
	}
}