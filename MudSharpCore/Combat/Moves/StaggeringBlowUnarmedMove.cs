using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class StaggeringBlowUnarmedMove : NaturalAttackMove
{
	private static ITraitExpression _staggeringBlowExpressionAttacker;
	private static ITraitExpression _staggeringBlowExpressionDefender;

	public ITraitExpression StaggeringBlowExpressionAttacker => _staggeringBlowExpressionAttacker ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("StaggeringBlowExpressionAttacker"),
			Gameworld);

	public ITraitExpression StaggeringBlowExpressionDefender => _staggeringBlowExpressionDefender ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("StaggeringBlowExpressionDefender"),
			Gameworld);

	public override string Description => $"Performing a staggering blow";

	public ICharacter Target { get; set; }

	#region Overrides of NaturalAttackMove

	public override BuiltInCombatMoveType MoveType => _clinching
		? BuiltInCombatMoveType.StaggeringBlowClinch
		: BuiltInCombatMoveType.StaggeringBlowUnarmed;

	private readonly bool _clinching;

	#endregion

	public StaggeringBlowUnarmedMove(ICharacter owner, INaturalAttack attack, ICharacter target, bool clinching) : base(
		owner, attack, target)
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
		StaggeringBlowExpressionAttacker.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		StaggeringBlowExpressionAttacker.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		StaggeringBlowExpressionAttacker.Formula.Parameters["weight"] = Assailant.Weight;
		StaggeringBlowExpressionAttacker.Formula.Parameters["height"] = Assailant.Height;
		var attackerStrength =
			StaggeringBlowExpressionAttacker.Evaluate(Assailant, context: TraitBonusContext.StaggeringBlowCheck);

		StaggeringBlowExpressionDefender.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		StaggeringBlowExpressionDefender.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		StaggeringBlowExpressionDefender.Formula.Parameters["weight"] = Target.Weight;
		StaggeringBlowExpressionDefender.Formula.Parameters["height"] = Target.Height;
		var defenderStrength =
			StaggeringBlowExpressionDefender.Evaluate(Target, context: TraitBonusContext.StaggeringBlowDefenseCheck);
		var cr = check.Check(Target, ((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty, Assailant,
			externalBonus: (defenderStrength - attackerStrength) *
			               Gameworld.GetStaticDouble("StaggeringBlowPenaltyMultiplier"));
		switch (cr.Outcome)
		{
			case Outcome.MinorFail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is staggered by the force of the blow!",
					Target)));
				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowReelTimeMinorFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("StaggeringBlowDefensiveAdvantageMinorFailure");
				break;
			case Outcome.Fail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!",
					Target)));
				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowReelTimeFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("StaggeringBlowDefensiveAdvantageFailure");
				break;
			case Outcome.MajorFail:
				if (Target.PositionState.Upright)
				{
					Target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ are|is knocked to the ground by the force of the blow!", Target)));
					Target.DoCombatKnockdown();
				}
				else
				{
					Target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!", Target)));
				}

				Target.AddEffect(new Staggered(Target),
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowStaggerEffectLength")));

				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowReelTimeMajorFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("StaggeringBlowDefensiveAdvantageMajorFailure");
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