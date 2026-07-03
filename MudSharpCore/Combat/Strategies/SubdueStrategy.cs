#nullable enable

using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Strategies;

public class SubdueStrategy : StandardMeleeStrategy
{
	private static readonly BuiltInCombatMoveType[] SubdualWeaponMoveTypes =
	{
		BuiltInCombatMoveType.UseWeaponAttack,
		BuiltInCombatMoveType.StaggeringBlow,
		BuiltInCombatMoveType.UnbalancingBlow,
		BuiltInCombatMoveType.Pushback
	};

	private static readonly BuiltInCombatMoveType[] InherentlySubdualWeaponMoveTypes =
	{
		BuiltInCombatMoveType.StaggeringBlow,
		BuiltInCombatMoveType.UnbalancingBlow,
		BuiltInCombatMoveType.Pushback
	};

	private static readonly BuiltInCombatMoveType[] SubdualNaturalMoveTypes =
	{
		BuiltInCombatMoveType.NaturalWeaponAttack,
		BuiltInCombatMoveType.StaggeringBlowUnarmed,
		BuiltInCombatMoveType.StaggeringBlowClinch,
		BuiltInCombatMoveType.UnbalancingBlowUnarmed,
		BuiltInCombatMoveType.UnbalancingBlowClinch,
		BuiltInCombatMoveType.PushbackUnarmed,
		BuiltInCombatMoveType.PushbackClinch
	};

	private static readonly BuiltInCombatMoveType[] InherentlySubdualNaturalMoveTypes =
	{
		BuiltInCombatMoveType.StaggeringBlowUnarmed,
		BuiltInCombatMoveType.StaggeringBlowClinch,
		BuiltInCombatMoveType.UnbalancingBlowUnarmed,
		BuiltInCombatMoveType.UnbalancingBlowClinch,
		BuiltInCombatMoveType.PushbackUnarmed,
		BuiltInCombatMoveType.PushbackClinch
	};

	private const CombatMoveIntentions SubdualIntentions =
		CombatMoveIntentions.Submit |
		CombatMoveIntentions.Grapple |
		CombatMoveIntentions.Disadvantage |
		CombatMoveIntentions.Advantage |
		CombatMoveIntentions.Attention |
		CombatMoveIntentions.Trip |
		CombatMoveIntentions.Stun |
		CombatMoveIntentions.Hinder |
		CombatMoveIntentions.Disarm;

	protected SubdueStrategy()
	{
	}

	public new static SubdueStrategy Instance { get; } = new();

	public override CombatStrategyMode Mode => CombatStrategyMode.Subdue;

	protected override ICombatMove? HandleAttacks(IPerceiver combatant)
	{
		if (combatant is not ICharacter ch || ch.CombatTarget is not ICharacter target)
		{
			return base.HandleAttacks(combatant);
		}

		if (ShouldGrappleForControl(ch, target))
		{
			var grappleMove = GrappleForControlStrategy.Instance.AttemptGrappleForControlOnly(ch);
			if (grappleMove is not null)
			{
				return grappleMove;
			}

			return HandleSubdualAttacksOnly(ch);
		}

		return base.HandleAttacks(combatant);
	}

	private ICombatMove? HandleSubdualAttacksOnly(ICharacter ch)
	{
		if (!WillAttackTarget(ch))
		{
			return null;
		}

		var move = CheckFixedAttacks(ch);
		if (move is not null)
		{
			return move;
		}

		if (ch.CurrentStamina < ch.CombatSettings.MinimumStaminaToAttack)
		{
			return null;
		}

		return TrySubdualAttack(ch);
	}

	protected override ICombatMove? HandleGeneralAttacks(ICharacter combatant)
	{
		return TrySubdualAttack(combatant) ?? base.HandleGeneralAttacks(combatant);
	}

	internal static bool ShouldGrappleForControl(ICharacter ch, ICharacter target)
	{
		if (ch.EffectsOfType<IGrappling>().Any(x => x.Target == target))
		{
			return true;
		}

		return IsSecondaryCombatant(ch, target) ||
		       IsTargetDisadvantaged(target) ||
		       IsTargetInjured(target);
	}

	private static bool IsSecondaryCombatant(ICharacter ch, ICharacter target)
	{
		return target.CombatTarget is not null && target.CombatTarget != ch;
	}

	private static bool IsTargetDisadvantaged(ICharacter target)
	{
		return target.IsHelpless ||
		       !CharacterState.Able.HasFlag(target.State) ||
		       !target.PositionState.Upright ||
		       target.DefensiveAdvantage < 0.0 ||
		       target.OffensiveAdvantage < 0.0 ||
		       target.EffectsOfType<Staggered>().Any();
	}

	private static bool IsTargetInjured(ICharacter target)
	{
		return target.HealthStrategy.IsCriticallyInjured(target) ||
		       target.Wounds.Any(x =>
			       x.CurrentDamage > 0.0 ||
			       x.CurrentPain > 0.0 ||
			       x.CurrentStun > 0.0 ||
			       x.CurrentShock > 0.0) ||
		       target.Body.EffectsOfType<ILimbIneffectiveEffect>().Any(x =>
			       x.Reason.In(
				       LimbIneffectiveReason.Pain,
				       LimbIneffectiveReason.Damage,
				       LimbIneffectiveReason.SpinalDamage,
				       LimbIneffectiveReason.Severing));
	}

	private static bool HasSubdualPriority(IWeaponAttack attack)
	{
		return InherentlySubdualWeaponMoveTypes.Contains(attack.MoveType) ||
		       (attack.Intentions & SubdualIntentions) != 0;
	}

	private static bool HasSubdualPriority(INaturalAttack attack)
	{
		return InherentlySubdualNaturalMoveTypes.Contains(attack.Attack.MoveType) ||
		       (attack.Attack.Intentions & SubdualIntentions) != 0;
	}

	private static bool HasSubdualPriority(IAuxiliaryCombatAction action)
	{
		return (action.Intentions & SubdualIntentions) != 0;
	}

	private static ICombatMove? TrySubdualAttack(ICharacter combatant)
	{
		if (combatant.CombatTarget is not ICharacter target)
		{
			return null;
		}

		var candidates = new List<(Func<ICombatMove> Factory, double Weight)>();
		if (combatant.CombatSettings.WeaponUsePercentage > 0.0)
		{
			AddWeaponCandidates(combatant, target, candidates);
		}

		if (combatant.CombatSettings.AuxiliaryPercentage > 0.0)
		{
			AddAuxiliaryCandidates(combatant, target, candidates);
		}

		if (combatant.PositionState.Upright &&
		    (combatant.CombatSettings.NaturalWeaponPercentage > 0.0 ||
		     (combatant.CombatSettings.FallbackToUnarmedIfNoWeapon && !candidates.Any())))
		{
			AddNaturalCandidates(combatant, target, candidates);
		}

		if (!candidates.Any())
		{
			return null;
		}

		return candidates.GetWeightedRandom(x => x.Weight).Factory();
	}

	private static void AddWeaponCandidates(ICharacter combatant, ICharacter target,
		ICollection<(Func<ICombatMove> Factory, double Weight)> candidates)
	{
		var iterations = 0;
		foreach (var preference in combatant.CombatSettings.MeleeAttackOrderPreferences.AsEnumerable().Reverse())
		{
			var weaponsForThisIteration = preference switch
			{
				MeleeAttackOrderPreference.Weapon => combatant.Body.WieldedItems
				                                            .SelectNotNull(x => x!.GetItemType<IMeleeWeapon>()),
				MeleeAttackOrderPreference.Implant => combatant.Body.Implants
				                                             .OfType<IImplantMeleeWeapon>()
				                                             .Where(x => x.WeaponIsActive)
				                                             .Cast<IMeleeWeapon>(),
				MeleeAttackOrderPreference.Prosthetic => combatant.Body.Prosthetics
				                                                .OfType<IProstheticMeleeWeapon>()
				                                                .Where(x => x.WeaponIsActive)
				                                                .Cast<IMeleeWeapon>(),
				_ => Enumerable.Empty<IMeleeWeapon>()
			};

			iterations++;
			var preferenceMultiplier = iterations;
			foreach (var weapon in weaponsForThisIteration)
			{
				var attacks = weapon.WeaponType
				                    .UsableAttacks(combatant, weapon.Parent, target, weapon.HandednessForWeapon(combatant),
					                    false, SubdualWeaponMoveTypes)
				                    .Where(HasSubdualPriority)
				                    .Where(x => combatant.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(combatant, x)))
				                    .ToList();
				foreach (var attack in attacks)
				{
					var weight = attack.Weighting *
					             preferenceMultiplier *
					             ManualCombatCommandResolver.AiWeightMultiplier(combatant, attack) *
					             (attack.MoveType == BuiltInCombatMoveType.Pushback ? 1.5 : 4.0);
					if (weight <= 0.0)
					{
						continue;
					}

					candidates.Add((() => CombatMoveFactory.CreateWeaponAttack(combatant, weapon, attack, target), weight));
				}
			}
		}
	}

	private static void AddAuxiliaryCandidates(ICharacter combatant, ICharacter target,
		ICollection<(Func<ICombatMove> Factory, double Weight)> candidates)
	{
		foreach (var action in combatant.Race
		                                .UsableAuxiliaryMoves(combatant, target, false)
		                                .Where(HasSubdualPriority)
		                                .Where(x =>
			                                combatant.CanSpendStamina(AuxiliaryMove.MoveStaminaCost(combatant, x))))
		{
			var weight = action.Weighting * ManualCombatCommandResolver.AiWeightMultiplier(combatant, action) * 2.0;
			if (weight <= 0.0)
			{
				continue;
			}

			candidates.Add((() => new AuxiliaryMove(combatant, target, action), weight));
		}
	}

	private static void AddNaturalCandidates(ICharacter combatant, ICharacter target,
		ICollection<(Func<ICombatMove> Factory, double Weight)> candidates)
	{
		foreach (var attack in combatant.Race
		                                .UsableNaturalWeaponAttacks(combatant, target, false, SubdualNaturalMoveTypes)
		                                .Where(HasSubdualPriority)
		                                .Where(x => combatant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(combatant,
			                                x.Attack))))
		{
			var weight = attack.Attack.Weighting *
			             ManualCombatCommandResolver.AiWeightMultiplier(combatant, attack.Attack);
			if (weight <= 0.0)
			{
				continue;
			}

			candidates.Add((() => CombatMoveFactory.CreateNaturalWeaponAttack(combatant, attack, target), weight));
		}
	}
}
