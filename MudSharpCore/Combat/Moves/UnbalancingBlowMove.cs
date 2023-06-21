using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Moves;

public class UnbalancingBlowMove : MeleeWeaponAttack
{
	private static TraitExpression _unbalancingBlowExpressionAttacker;
	private static TraitExpression _unbalancingBlowExpressionDefender;

	public TraitExpression UnbalancingBlowExpressionAttacker => _unbalancingBlowExpressionAttacker ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("UnbalancingBlowExpressionAttacker"),
			Gameworld);

	public TraitExpression UnbalancingBlowExpressionDefender => _unbalancingBlowExpressionDefender ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("UnbalancingBlowExpressionDefender"),
			Gameworld);

	public override string Description => "Performing an unbalancing blow";

	public ICharacter Target { get; set; }

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.UnbalancingBlow;

	public UnbalancingBlowMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target) : base(
		owner, weapon, attack, target)
	{
		Target = target;
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var result = base.ResolveMove(defenderMove);
		if (Target.State.HasFlag(CharacterState.Dead) || !result.MoveWasSuccessful || result.AttackerOutcome.IsFail() ||
		    (Target.Body.GetLimbFor(TargetBodypart)?.LimbType.In(LimbType.Appendage, LimbType.Arm, LimbType.Wing) ??
		     false))
		{
			return result;
		}

		var check = Gameworld.GetCheck(CheckType.StaggeringBlowDefense);
		UnbalancingBlowExpressionAttacker.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		UnbalancingBlowExpressionAttacker.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		UnbalancingBlowExpressionAttacker.Formula.Parameters["pain"] = result.WoundsCaused.Sum(x => x.CurrentPain);
		UnbalancingBlowExpressionAttacker.Formula.Parameters["degree"] = result.AttackerOutcome.CheckDegrees();
		var attackerStrength =
			UnbalancingBlowExpressionAttacker.Evaluate(Assailant, context: TraitBonusContext.UnbalancingBlowCheck);

		UnbalancingBlowExpressionDefender.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		UnbalancingBlowExpressionDefender.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		UnbalancingBlowExpressionDefender.Formula.Parameters["pain"] = result.WoundsCaused.Sum(x => x.CurrentPain);
		UnbalancingBlowExpressionDefender.Formula.Parameters["degree"] = result.DefenderOutcome.CheckDegrees();
		UnbalancingBlowExpressionDefender.Formula.Parameters["limbs"] = 0;
		var defenderStrength =
			UnbalancingBlowExpressionDefender.Evaluate(Target, context: TraitBonusContext.UnbalancingBlowDefenseCheck);
		var cr = check.Check(Target, ((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty, Assailant,
			externalBonus: (defenderStrength - attackerStrength) *
			               Gameworld.GetStaticDouble("UnbalancingBlowPenaltyMultiplier"));
		switch (cr.Outcome)
		{
			case Outcome.MinorFail:
				if (Target.PositionState.Upright)
				{
					Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is knocked prone by the attack!",
						Target)));
					Target.SetPosition(PositionProne.Instance, PositionModifier.None, Target.PositionTarget, null);
					Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("UnbalancingBlowReelTimeMinorFailure")));
					Target.AddEffect(new Staggered(Target),
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowStaggerEffectLength")));
				}

				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("UnbalancingBlowDefensiveAdvantageMinorFailure");
				break;
			case Outcome.Fail:
				if (Target.PositionState.Upright)
				{
					Target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ are|is sent sprawling to the ground by the attack!", Target)));
					Target.SetPosition(PositionSprawled.Instance, PositionModifier.None, Target.PositionTarget, null);
					Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("UnbalancingBlowReelTimeFailure")));
					Target.AddEffect(new Staggered(Target),
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowStaggerEffectLength")));
				}

				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("UnbalancingBlowDefensiveAdvantageFailure");
				break;
			case Outcome.MajorFail:
				if (Target.PositionState.Upright)
				{
					Target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ are|is sent sprawling to the ground by the attack!", Target)));
					Target.SetPosition(PositionSprawled.Instance, PositionModifier.None, Target.PositionTarget, null);
					Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("UnbalancingBlowReelTimeMajorFailure")));
					Target.AddEffect(new Staggered(Target),
						TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowStaggerEffectLength")));
				}

				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("UnbalancingBlowDefensiveAdvantageMajorFailure");
				break;
		}

		if (cr.Outcome.IsFail())
		{
			Target.RemoveAllEffects(x =>
				x.GetSubtype<ClinchEffect>() is ClinchEffect ce && ce.Clincher == Target && ce.Target == Assailant);
			Target.RemoveAllEffects(x => x.GetSubtype<Grappling>() is Grappling gr && gr.Target == Assailant);
			Assailant.RemoveAllEffects(x =>
				x.GetSubtype<ClinchEffect>() is ClinchEffect ce && ce.Clincher == Assailant && ce.Target == Target);
			Assailant.RemoveAllEffects(x => x.GetSubtype<Grappling>() is Grappling gr && gr.Target == Target);
			Target.AddEffect(new ClinchCooldown(Target, Target.Combat),
				TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
		}

		return result;
	}
}