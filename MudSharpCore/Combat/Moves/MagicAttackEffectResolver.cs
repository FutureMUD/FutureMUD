#nullable enable

using MudSharp.Combat.AuxiliaryEffects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

internal static class MagicAttackEffectResolver
{
	internal static bool IsApplicable(ICharacter actor, ICharacter target, MagicAttackEffectType type) => type switch
	{
		MagicAttackEffectType.Pull => actor.ColocatedWith(target),
		MagicAttackEffectType.BreakClinch => actor.EffectsOfType<ClinchEffect>().Any(x => x.Target == target),
		MagicAttackEffectType.Disarm => target.Body.WieldedItems.Any(x => target.Body.CanBeDisarmed(x, actor)),
		_ => true
	};
	internal static CheckType ResistanceCheck(MagicAttackEffectType type) => type switch
	{
		MagicAttackEffectType.BreakClinch => CheckType.ResistBreakClinch,
		MagicAttackEffectType.Stagger => CheckType.StaggeringBlowDefense,
		MagicAttackEffectType.Pushback => CheckType.OpposePushbackCheck,
		_ => CheckType.OpposeForcedMovementCheck
	};

	public static void Apply(ICharacter actor, ICharacter target, IMagicAttackPower power)
	{
		foreach (var effect in power.AttackEffects.OrderBy(x => x.Type))
		{
			if (target.State.HasFlag(CharacterState.Dead)) return;
			if (!IsApplicable(actor, target, effect.Type)) continue;
			if (effect.Type == MagicAttackEffectType.Pull && !actor.ColocatedWith(target)) continue;
			if (effect.Type == MagicAttackEffectType.BreakClinch && !actor.EffectsOfType<ClinchEffect>().Any(x => x.Target == target)) continue;
			var item = effect.Type == MagicAttackEffectType.Disarm
				? target.Body.WieldedItems.FirstOrDefault(x => target.Body.CanBeDisarmed(x, actor)) : null;
			if (effect.Type == MagicAttackEffectType.Disarm && item is null) continue;
			var attack = actor.Gameworld.GetCheck(CheckType.GenericSkillCheck).Check(actor,
				power.WeaponAttack.Profile.BaseAttackerDifficulty, power.AttackerTrait, target);
			var defense = actor.Gameworld.GetCheck(ResistanceCheck(effect.Type)).Check(target, effect.Resistance, null, actor);
			var opposed = new OpposedOutcome(attack, defense);
			if (opposed.Outcome != OpposedOutcomeDirection.Proponent || attack.IsFail())
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote(effect.ResistEmote, actor, actor, target), style: OutputStyle.CombatMessage));
				continue;
			}
			var degrees = Math.Max(1, (int)Math.Ceiling((int)opposed.Degree * effect.Strength));
			switch (effect.Type)
			{
				case MagicAttackEffectType.BreakClinch:
					actor.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == target, true);
					target.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == actor, true);
					if (target.Combat is not null) target.AddEffect(new ClinchCooldown(target, target.Combat), TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
					break;
				case MagicAttackEffectType.Disarm:
					target.Body.Take(item!);
					Disarm.PlaceDisarmedItem(item!, target);
					if (effect.DurationSeconds > 0 && target.Combat is not null)
						item!.AddEffect(new CombatNoGetEffect(item, target.Combat), TimeSpan.FromSeconds(effect.DurationSeconds));
					break;
				case MagicAttackEffectType.Stagger:
					target.AddEffect(new Staggered(target), TimeSpan.FromSeconds(effect.DurationSeconds));
					actor.Gameworld.Scheduler.DelayScheduleType(target, ScheduleType.Combat, TimeSpan.FromSeconds(effect.DurationSeconds * effect.Strength * CombatBase.CombatSpeedMultiplier));
					break;
				case MagicAttackEffectType.Knockdown: target.DoCombatKnockdown(degrees); break;
				case MagicAttackEffectType.Pushback: CombatForcedMovementUtilities.ApplyPushback(actor, target, degrees); break;
				case MagicAttackEffectType.Pull: PullToMeleeMove.PullTargetIntoMelee(actor, target); break;
			}
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(effect.SuccessEmote, actor, actor, target), style: OutputStyle.CombatMessage));
		}
	}
}
