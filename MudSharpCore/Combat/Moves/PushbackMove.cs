using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class PushbackMove : MeleeWeaponAttack
{
	public PushbackMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target) : base(owner, weapon, attack, target)
	{
		Target = target;
	}

	public ICharacter Target { get; }

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.Pushback;
	public override string Description => "Knocking an opponent out of melee range";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var result = base.ResolveMove(defenderMove);
		if (!ShouldResolvePushback(result))
		{
			return result;
		}

		ResolvePushbackContest(result);
		return result;
	}

	protected bool ShouldResolvePushback(CombatMoveResult result)
	{
		return !Target.State.HasFlag(CharacterState.Dead) &&
		       result.MoveWasSuccessful &&
		       result.AttackerOutcome.IsPass() &&
		       Attack is ISecondaryDifficultyAttack;
	}

	protected void ResolvePushbackContest(CombatMoveResult result)
	{
		var attackRoll = Gameworld.GetCheck(CheckType.PushbackCheck)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, Target,
			                          Assailant.OffensiveAdvantage);
		var defenseRoll = Gameworld.GetCheck(CheckType.OpposePushbackCheck)
		                           .CheckAgainstAllDifficulties(Target,
			                           ((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty,
			                           null, Assailant,
			                           Target.DefensiveAdvantage - GetPositionPenalty(Assailant.GetFacingFor(Target)));
		Assailant.OffensiveAdvantage = 0;
		Target.DefensiveAdvantage = 0;

		var opposed = new OpposedOutcome(attackRoll, defenseRoll, CheckDifficulty,
			((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty);
		if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ drive|drives $1 out of melee range!",
					Assailant, Assailant, Target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			CombatForcedMovementUtilities.ApplyPushback(Assailant, Target, (int)opposed.Degree);
			return;
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"$1 keep|keeps &1 footing and resist|resists being driven out of melee range.",
				Assailant, Assailant, Target),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

		if (attackRoll[CheckDifficulty].IsPass())
		{
			Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
				TimeSpan.FromSeconds(Gameworld.GetStaticDouble("PushbackMoveOnGoodFailDelay") *
				                     attackRoll[CheckDifficulty].SuccessDegrees() *
				                     CombatBase.CombatSpeedMultiplier));
		}
	}
}
