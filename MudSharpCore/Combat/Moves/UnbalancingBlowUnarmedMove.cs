using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Moves;

public class UnbalancingBlowUnarmedMove : NaturalAttackMove
{
	private static ITraitExpression _unbalancingBlowExpressionAttacker;
	private static ITraitExpression _unbalancingBlowExpressionDefender;

	public ITraitExpression UnbalancingBlowExpressionAttacker => _unbalancingBlowExpressionAttacker ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("UnbalancingBlowExpressionAttacker"),
			Gameworld);

	public ITraitExpression UnbalancingBlowExpressionDefender => _unbalancingBlowExpressionDefender ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("UnbalancingBlowExpressionDefender"),
			Gameworld);

	public override string Description => "Performing an unarmed unbalancing blow";

	public ICharacter Target { get; set; }

	public override BuiltInCombatMoveType MoveType => _clinching
		? BuiltInCombatMoveType.UnbalancingBlowClinch
		: BuiltInCombatMoveType.UnbalancingBlowUnarmed;

	private readonly bool _clinching;

	public UnbalancingBlowUnarmedMove(ICharacter owner, INaturalAttack attack, ICharacter target, bool clinching) :
		base(owner, attack, target)
	{
		Target = target;
		_clinching = clinching;
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
		UnbalancingBlowExpressionDefender.Formula.Parameters["limbs"] = Target.CombinedEffectsOfType<BeingGrappled>()
		                                                                      .Sum(x => x.Grappling.LimbsUnderControl
			                                                                      .Count());

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