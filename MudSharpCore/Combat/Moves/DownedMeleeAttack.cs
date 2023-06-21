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
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class DownedMeleeAttack : MeleeWeaponAttack
{
	private static TraitExpression _downedMeleeExpressionAttacker;
	private static TraitExpression _downedMeleeExpressionDefender;

	public TraitExpression DownedMeleeExpressionAttacker => _downedMeleeExpressionAttacker ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("DownedMeleeExpressionAttacker"),
		Gameworld);

	public TraitExpression DownedMeleeExpressionDefender => _downedMeleeExpressionDefender ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("DownedMeleeExpressionDefender"),
		Gameworld);

	#region Overrides of MeleeWeaponAttack

	public override string Description => "Attacking a downed opponent in melee";

	#endregion

	public ICharacter Target { get; set; }

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.DownedAttack;

	public DownedMeleeAttack(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target) : base(
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
		DownedMeleeExpressionAttacker.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		DownedMeleeExpressionAttacker.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		var attackerStrength = DownedMeleeExpressionAttacker.Evaluate(Assailant);

		DownedMeleeExpressionDefender.Formula.Parameters["damage"] = result.WoundsCaused.Sum(x => x.OriginalDamage);
		DownedMeleeExpressionDefender.Formula.Parameters["stun"] = result.WoundsCaused.Sum(x => x.CurrentStun);
		var defenderStrength = DownedMeleeExpressionDefender.Evaluate(Target);
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
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("DownedMeleeDefensiveAdvantageMinorFailure");
				break;
			case Outcome.Fail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!",
					Target)));
				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeReelTimeFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("DownedMeleeDefensiveAdvantageFailure");
				break;
			case Outcome.MajorFail:
				Target.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is sent reeling by the force of the blow!",
					Target)));
				Target.AddEffect(new Staggered(Target),
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeStaggerEffectLength")));

				Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
					TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("DownedMeleeReelTimeMajorFailure")));
				Target.DefensiveAdvantage -= Gameworld.GetStaticDouble("DownedMeleeDefensiveAdvantageMajorFailure");
				break;
		}

		return result;
	}
}