using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
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

public class UnarmedDownedMeleeAttack : NaturalAttackMove
{
	private static ITraitExpression _downedMeleeExpressionAttacker;
	private static ITraitExpression _downedMeleeExpressionDefender;

	public ITraitExpression DownedMeleeExpressionAttacker => _downedMeleeExpressionAttacker ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("DownedMeleeExpressionAttacker"),
		Gameworld);

	public ITraitExpression DownedMeleeExpressionDefender => _downedMeleeExpressionDefender ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("DownedMeleeExpressionDefender"),
		Gameworld);

	public override string Description => $"Attacking a downed melee opponent";

	public ICharacter Target { get; set; }
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.DownedAttackUnarmed;

	public UnarmedDownedMeleeAttack(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack,
		target)
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
		DownedMeleeExpressionAttacker.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		DownedMeleeExpressionAttacker.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		var attackerStrength =
			DownedMeleeExpressionAttacker.Evaluate(Assailant, context: TraitBonusContext.StaggeringBlowCheck);

		DownedMeleeExpressionDefender.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		DownedMeleeExpressionDefender.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		var defenderStrength =
			DownedMeleeExpressionDefender.Evaluate(Target, context: TraitBonusContext.StaggeringBlowDefenseCheck);
		var cr = check.Check(Target, ((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty, Assailant,
			externalBonus: (defenderStrength - attackerStrength) *
			               Gameworld.GetStaticDouble("StaggeringBlowPenaltyMultiplier"));
		switch (cr.Outcome)
		{
			case Outcome.MinorFail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is staggered by the force of the blow!",
					Target)));
				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeReelTimeMinorFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticInt("DownedMeleeDefensiveAdvantageMinorFailure");
				break;
			case Outcome.Fail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!",
					Target)));
				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeReelTimeFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticInt("DownedMeleeDefensiveAdvantageFailure");
				break;
			case Outcome.MajorFail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!",
					Target)));
				Target.AddEffect(new Staggered(Target),
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeStaggerEffectLength")));

				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeReelTimeMajorFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticInt("DownedMeleeDefensiveAdvantageMajorFailure");
				break;
		}

		return result;
	}
}