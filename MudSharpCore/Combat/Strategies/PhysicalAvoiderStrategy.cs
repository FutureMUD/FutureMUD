using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System.Linq;

namespace MudSharp.Combat.Strategies;

public class PhysicalAvoiderStrategy : StandardMeleeStrategy
{
	protected PhysicalAvoiderStrategy()
	{
	}

	public new static PhysicalAvoiderStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.PhysicalAvoider;

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		var move = base.HandleCombatMovement(combatant);
		if (move is not null)
		{
			return move;
		}

		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target)
		{
			return null;
		}

		if (!ch.MeleeRange &&
		    ch.Combat.Combatants.OfType<ICharacter>().All(x => x.CombatTarget != ch || !x.MeleeRange))
		{
			return null;
		}

		return TryPushback(ch, target) ??
		       TryPhysicalControl(ch, target) ??
		       (OpponentCannotOppose(target) && ch.MeleeRange &&
		        ch.EffectsOfType<IRageEffect>().All(x => !x.IsSuperRaging)
			       ? new FleeMove { Assailant = ch }
			       : null);
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target)
		{
			return null;
		}

		if (!WillAttackTarget(ch))
		{
			return null;
		}

		return TryPushback(ch, target) ??
		       TryPhysicalControl(ch, target) ??
		       base.HandleAttacks(combatant);
	}

	private static bool OpponentCannotOppose(ICharacter target)
	{
		return target.IsHelpless ||
		       !target.Race.CombatSettings.CanDefend ||
		       target.State.HasFlag(CharacterState.Sleeping) ||
		       target.State.HasFlag(CharacterState.Unconscious) ||
		       target.State.HasFlag(CharacterState.Paralysed);
	}

	internal static ICombatMove TryPushback(ICharacter ch, ICharacter target)
	{
		var weaponMove = ch.Body.WieldedItems
		                   .SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
		                   .SelectMany(x => x.WeaponType
		                                    .UsableAttacks(ch, x.Parent, target, x.HandednessForWeapon(ch), false,
			                                    BuiltInCombatMoveType.Pushback)
		                                    .Where(y => ch.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(ch, y)))
		                                    .Select(y => (Weapon: x, Attack: y)))
		                   .GetWeightedRandom(x => x.Attack.Weighting);
		if (weaponMove != default)
		{
			return CombatMoveFactory.CreateWeaponAttack(ch, weaponMove.Weapon, weaponMove.Attack, target);
		}

		var naturalMove = ch.Race
		                    .UsableNaturalWeaponAttacks(ch, target, false,
			                    BuiltInCombatMoveType.PushbackUnarmed,
			                    BuiltInCombatMoveType.PushbackClinch)
		                    .Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack)))
		                    .GetWeightedRandom(x => x.Attack.Weighting);
		return naturalMove is null ? null : CombatMoveFactory.CreateNaturalWeaponAttack(ch, naturalMove, target);
	}

	internal static ICombatMove TryPhysicalControl(ICharacter ch, ICharacter target)
	{
		var weaponMove = ch.Body.WieldedItems
		                   .SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
		                   .SelectMany(x => x.WeaponType
		                                    .UsableAttacks(ch, x.Parent, target, x.HandednessForWeapon(ch), false,
			                                    BuiltInCombatMoveType.StaggeringBlow,
			                                    BuiltInCombatMoveType.UnbalancingBlow)
		                                    .Where(y => ch.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(ch, y)))
		                                    .Select(y => (Weapon: x, Attack: y)))
		                   .GetWeightedRandom(x => x.Attack.Weighting);
		if (weaponMove != default)
		{
			return CombatMoveFactory.CreateWeaponAttack(ch, weaponMove.Weapon, weaponMove.Attack, target);
		}

		var naturalMove = ch.Race
		                    .UsableNaturalWeaponAttacks(ch, target, false,
			                    BuiltInCombatMoveType.StaggeringBlowUnarmed,
			                    BuiltInCombatMoveType.StaggeringBlowClinch,
			                    BuiltInCombatMoveType.UnbalancingBlowUnarmed,
			                    BuiltInCombatMoveType.UnbalancingBlowClinch)
		                    .Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack)))
		                    .GetWeightedRandom(x => x.Attack.Weighting);
		return naturalMove is null ? null : CombatMoveFactory.CreateNaturalWeaponAttack(ch, naturalMove, target);
	}
}
