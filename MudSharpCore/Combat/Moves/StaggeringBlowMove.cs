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

public class StaggeringBlowMove : MeleeWeaponAttack
{
	private static TraitExpression _staggeringBlowExpressionAttacker;
	private static TraitExpression _staggeringBlowExpressionDefender;

	public TraitExpression StaggeringBlowExpressionAttacker => _staggeringBlowExpressionAttacker ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("StaggeringBlowExpressionAttacker"),
			Gameworld);

	public TraitExpression StaggeringBlowExpressionDefender => _staggeringBlowExpressionDefender ??=
		new TraitExpression(Gameworld.GetStaticConfiguration("StaggeringBlowExpressionDefender"),
			Gameworld);

	public override string Description => "Performing a staggering blow";

	public ICharacter Target { get; set; }

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.StaggeringBlow;

	public StaggeringBlowMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target) : base(
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
			if (Target.Combat != null)
			{
				Target.AddEffect(new ClinchCooldown(Target, Target.Combat),
					TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
			}
		}

		return result;
	}
}