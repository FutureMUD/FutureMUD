#nullable enable

using MudSharp.Body;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

internal sealed record BackstabChoice(IWeaponAttack Attack, IMeleeWeapon? Weapon, INaturalAttack? NaturalAttack)
{
	public ICombatMove CreateMove(ICharacter actor, ICharacter target) => Weapon is not null
		? CombatMoveFactory.CreateWeaponAttack(actor, Weapon, Attack, target)
		: CombatMoveFactory.CreateNaturalWeaponAttack(actor, NaturalAttack!, target);
}

internal static class BackstabCombat
{
	internal static bool IsBackstab(BuiltInCombatMoveType type) =>
		type is BuiltInCombatMoveType.BackstabAttack or BuiltInCombatMoveType.BackstabAttackUnarmed;

	internal static bool IsEligible(ICharacter actor, ICharacter target, bool allowInitiation = true) =>
		actor != target && !target.State.IsDead() &&
		(actor.GetFacingFor(target) == Facing.Rear || UnseenCombatOpening.HasOpening(actor, target) ||
		 allowInitiation && UnseenCombatOpening.IsNewEncounter(actor, target) && !target.CanSee(actor));

	internal static BackstabChoice? FindAttack(ICharacter actor, ICharacter target, out string error)
	{
		error = "You have no usable backstab attack against that target.";
		if (!IsEligible(actor, target))
		{
			error = "You must have an unused unseen opening or be on your target's rear facing to backstab.";
			return null;
		}

		var exhausted = false;
		foreach (var sources in ManualCombatCommandResolver.WeaponSourcesInPreferenceOrder(actor).GroupBy(x => x.Preference))
		{
			var candidates = sources.SelectMany(x => x.Weapon.WeaponType
				.UsableAttacks(actor, x.Weapon.Parent, target, x.Weapon.HandednessForWeapon(actor), false,
					BuiltInCombatMoveType.BackstabAttack)
				.Where(a => a is IBackstabAttack && a.Weighting > 0.0)
				.Select(a => new BackstabChoice(a, x.Weapon, null))).ToList();
			if (!CommandExecutionScope.TryContinue(actor)) return null;
			var affordable = candidates.Where(x => actor.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(actor, x.Attack))).ToList();
			if (affordable.Count > 0) return affordable.GetWeightedRandom(x => x.Attack.Weighting);
			exhausted |= candidates.Count > 0;
		}

		var natural = actor.Race.UsableNaturalWeaponAttacks(actor, target, false, BuiltInCombatMoveType.BackstabAttackUnarmed)
			.Where(x => x.Attack is IBackstabAttack && x.Attack.Weighting > 0.0).ToList();
		if (!CommandExecutionScope.TryContinue(actor)) return null;
		var usable = natural.Where(x => actor.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(actor, x.Attack))).ToList();
		if (usable.Count > 0)
		{
			var selected = usable.GetWeightedRandom(x => x.Attack.Weighting);
			return new BackstabChoice(selected.Attack, null, selected);
		}
		if (exhausted || natural.Count > 0) error = "You are too exhausted to use any available backstab attack.";
		return null;
	}

	internal static bool Validate(ICharacter actor, ICharacter target, BackstabChoice choice, out string error)
	{
		error = "You can no longer backstab that target.";
		if (!CommandExecutionScope.TryContinue(actor) || !CharacterState.Able.HasFlag(actor.State) || actor.Combat is null || actor.Combat != target.Combat ||
		    !actor.Combat.Combatants.Contains(target) || !actor.ColocatedWith(target) || !actor.MeleeRange ||
		    !actor.CanSee(target) || !IsEligible(actor, target, false)) return false;
		if (choice.Weapon is { } weapon)
		{
			if (!ManualCombatCommandResolver.WeaponSourcesInPreferenceOrder(actor).Any(x => ReferenceEquals(x.Weapon, weapon)) ||
			    !weapon.WeaponType.UsableAttacks(actor, weapon.Parent, target, weapon.HandednessForWeapon(actor), false,
				    BuiltInCombatMoveType.BackstabAttack).Contains(choice.Attack)) return false;
		}
		else if (choice.NaturalAttack is not { } natural ||
		         !actor.Race.UsableNaturalWeaponAttacks(actor, target, false, BuiltInCombatMoveType.BackstabAttackUnarmed)
			         .Any(x => ReferenceEquals(x.Attack, natural.Attack) && x.Bodypart == natural.Bodypart)) return false;
		if (!CommandExecutionScope.TryContinue(actor)) return false;
		if (!actor.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(actor, choice.Attack)))
		{
			error = "You are too exhausted to backstab.";
			return false;
		}
		return CommandExecutionScope.TryContinue(actor) && actor.Combat is not null && actor.Combat == target.Combat &&
		       actor.ColocatedWith(target) && actor.CanSee(target) && actor.MeleeRange && IsEligible(actor, target, false);
	}

	internal static IDamage ModifyDamage(IBackstabAttack attack, IDamage damage)
	{
		var bypass = attack.ArmourBypassChance >= 1.0 ||
		             attack.ArmourBypassChance > 0.0 && Constants.Random.NextDouble() < attack.ArmourBypassChance;
		return new Damage(damage) { BypassWornArmour = bypass };
	}

	internal static void ApplyEffects(ICharacter actor, ICharacter target, IBackstabAttack attack, CombatMoveResult result)
	{
		if (!CommandExecutionScope.TryContinue(actor) || !result.MoveWasSuccessful || target.State.IsDead() ||
		    !result.WoundsCaused.Any(x => (ReferenceEquals(x.Parent, target) || ReferenceEquals(x.Parent, target.Body)) &&
		                                 (x.CurrentDamage > 0.0 || x.CurrentPain > 0.0 || x.CurrentStun > 0.0)) ||
		    attack.ReelSecondsPerDegree <= 0.0 && attack.AttackerOffensiveAdvantagePerDegree <= 0.0 &&
		    attack.AttackerDefensiveAdvantagePerDegree <= 0.0 && attack.TargetOffensiveDisadvantagePerDegree <= 0.0 &&
		    attack.TargetDefensiveDisadvantagePerDegree <= 0.0) return;
		var resistance = actor.Gameworld.GetCheck(CheckType.StaggeringBlowDefense)
			.Check(target, attack.SecondaryDifficulty, actor);
		if (!CommandExecutionScope.TryContinue(actor) || !resistance.IsFail()) return;
		var degrees = resistance.FailureDegrees();
		CommandExecutionScope.MarkCommitted();
		actor.OffensiveAdvantage += attack.AttackerOffensiveAdvantagePerDegree * degrees;
		actor.DefensiveAdvantage += attack.AttackerDefensiveAdvantagePerDegree * degrees;
		target.OffensiveAdvantage -= attack.TargetOffensiveDisadvantagePerDegree * degrees;
		target.DefensiveAdvantage -= attack.TargetDefensiveDisadvantagePerDegree * degrees;
		var seconds = attack.ReelSecondsPerDegree * degrees * CombatBase.CombatSpeedMultiplier;
		if (seconds > 0.0 && target.Combat is not null)
		{
			actor.Gameworld.Scheduler.DelayScheduleType(target, ScheduleType.Combat,
				TimeSpan.FromSeconds(Math.Min(seconds, TimeSpan.MaxValue.TotalSeconds / 2.0)));
			target.OutputHandler.Handle(new EmoteOutput(new Emote("@ reel|reels from the backstab!", target), style: OutputStyle.CombatMessage));
		}
	}
}
