using MudSharp.Combat.Moves;
using MudSharp.GameItems;
using MudSharp.Vehicles;

namespace MudSharp.Combat;

public static class CombatMoveFactory
{
	public static ICombatMove CreateWeaponAttack(ICharacter assailant, IMeleeWeapon weapon, IWeaponAttack attack,
		ICharacter target)
	{
		ICombatMove CreateSingleTargetMove(ICharacter singleTarget)
		{
			switch (attack.MoveType)
			{
				case BuiltInCombatMoveType.CoupDeGrace:
					return new CoupDeGrace(attack, singleTarget) { Assailant = assailant, Weapon = weapon };
				case BuiltInCombatMoveType.UseWeaponAttack:
					return new MeleeWeaponAttack(assailant, weapon, attack, singleTarget);
				case BuiltInCombatMoveType.StaggeringBlow:
					return new StaggeringBlowMove(assailant, weapon, attack, singleTarget);
				case BuiltInCombatMoveType.UnbalancingBlow:
					return new UnbalancingBlowMove(assailant, weapon, attack, singleTarget);
				case BuiltInCombatMoveType.DownedAttack:
					return new DownedMeleeAttack(assailant, weapon, attack, singleTarget);
				case BuiltInCombatMoveType.Pushback:
					return new PushbackMove(assailant, weapon, attack, singleTarget);
				case BuiltInCombatMoveType.PullToMelee:
					return new PullToMeleeMove(assailant, weapon, attack, singleTarget);
			}

			throw new ApplicationException(
				$"Invalid Move Type in CombatMoveFactory.CreateWeaponAttack: {attack.MoveType.Describe()}");
		}

		return MultiTargetCombatMove.WrapWeaponAttack(assailant, target, attack, weapon.Parent,
			CreateSingleTargetMove);
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
		ICombatMove CreateSingleTargetMove(ICharacter singleTarget)
		{
			switch (attack.Attack.MoveType)
			{
				case BuiltInCombatMoveType.NaturalWeaponAttack:
					return new NaturalAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.EnvenomingAttack:
					return new EnvenomingAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.EnvenomingAttackClinch:
					return new EnvenomingClinchAttack(assailant, singleTarget, attack, null);
				case BuiltInCombatMoveType.StaggeringBlowUnarmed:
					return new StaggeringBlowUnarmedMove(assailant, attack, singleTarget, false);
				case BuiltInCombatMoveType.StaggeringBlowClinch:
					return new StaggeringBlowUnarmedMove(assailant, attack, singleTarget, true);
				case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
					return new UnbalancingBlowUnarmedMove(assailant, attack, singleTarget, false);
				case BuiltInCombatMoveType.UnbalancingBlowClinch:
					return new UnbalancingBlowUnarmedMove(assailant, attack, singleTarget, true);
				case BuiltInCombatMoveType.PushbackUnarmed:
					return new PushbackUnarmedMove(assailant, attack, singleTarget, false);
				case BuiltInCombatMoveType.PushbackClinch:
					return new PushbackUnarmedMove(assailant, attack, singleTarget, true);
				case BuiltInCombatMoveType.PullToMeleeUnarmed:
					return new PullToMeleeUnarmedMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.ScreechAttack:
					return new ScreechAttackMove(assailant, attack, null);
				case BuiltInCombatMoveType.RangedNaturalAttack:
					return new RangedNaturalAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.BreathWeaponAttack:
					return new BreathWeaponAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.SpitNaturalAttack:
					return new SpitAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.ExplosiveNaturalAttack:
					return new ExplosiveNaturalAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.BuffetingNaturalAttack:
					return new BuffetingRangedAttackMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.DownedAttackUnarmed:
					return new UnarmedDownedMeleeAttack(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.StrangleAttack:
					return new StrangleAttack(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.TakedownMove:
					return new TakedownMove(assailant, attack, singleTarget);
				case BuiltInCombatMoveType.AquaticVehicleAttack:
					var vehicle = VehicleCombatService.Instance.VehicleFor(singleTarget);
					if (vehicle?.ExteriorItem is not null)
					{
						return new AquaticVehicleAttackMove(assailant, attack, vehicle.ExteriorItem);
					}

					break;
			}

			throw new ApplicationException(
				$"Invalid Move Type in CombatMoveFactory.CreateNaturalWeaponAttack: {attack.Attack.MoveType.Describe()}");
		}

		return MultiTargetCombatMove.WrapWeaponAttack(assailant, target, attack.Attack, null,
			CreateSingleTargetMove);
	}

    public static ICombatMove CreateNaturalWeaponAttack(ICharacter assailant, INaturalAttack attack, IGameItem target)
    {
		switch (attack.Attack.MoveType)
		{
			case BuiltInCombatMoveType.RangedNaturalAttack:
				return new RangedNaturalAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.BreathWeaponAttack:
				return new BreathWeaponAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.SpitNaturalAttack:
				return new SpitAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.ExplosiveNaturalAttack:
				return new ExplosiveNaturalAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.BuffetingNaturalAttack:
				return new BuffetingRangedAttackMove(assailant, attack, target);
			case BuiltInCombatMoveType.AquaticVehicleAttack:
				return new AquaticVehicleAttackMove(assailant, attack, target);
		}

		throw new ApplicationException(
			$"Invalid Move Type in CombatMoveFactory.CreateNaturalWeaponAttack: {attack.Attack.MoveType.Describe()}");
    }
}
