using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class RescueMove : CombatMoveBase
{
	public ICharacter Target { get; init; }
	public override string Description => $"Trying to rescue {Target.HowSeen(Assailant)}";

	#region Overrides of CombatMoveBase

	public override double BaseDelay => Gameworld.GetStaticDouble("RescueMoveDelay");

	#endregion

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var targets =
			Target.Combat.Combatants.Where(
				      x => x.CombatTarget == Target && x.MeleeRange && x.ColocatedWith(Target) &&
				           x.Location == Assailant.Location)
			      .OfType<ICharacter>()
			      .ToList();
		var targetAssailant = targets.GetRandomElement();
		if (targetAssailant == null)
		{
			Assailant.RemoveAllEffects(x => x.IsEffectType<IRescueEffect>());
			return CombatMoveResult.Irrelevant;
		}

		var check = Gameworld.GetCheck(CheckType.RescueCheck);
		var difficultyRescuer = Difficulty.Normal;
		difficultyRescuer =
			difficultyRescuer.StageUp(Assailant.Combat.Combatants.Count(x => x.CombatTarget == Assailant));
		var resultRescuer = check.Check(Assailant, difficultyRescuer, targetAssailant);
		var defenseCheck = Gameworld.GetCheck(CheckType.OpposeRescueCheck);
		var resultAssailant = defenseCheck.Check(targetAssailant, Difficulty.Hard, Assailant);

		var outcome = new OpposedOutcome(resultRescuer, resultAssailant);
		if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetFailMessageFor(Assailant, targetAssailant, null, null,
							BuiltInCombatMoveType.Rescue, resultRescuer, null), Assailant, Assailant, Target,
						targetAssailant), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = resultRescuer.Outcome,
				DefenderOutcome = resultAssailant.Outcome
			};
		}

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(Assailant, targetAssailant, null, null,
						BuiltInCombatMoveType.Rescue, resultRescuer, null), Assailant, Assailant, Target,
					targetAssailant), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		targetAssailant.CombatTarget = Assailant;
		Assailant.RemoveAllEffects(x => x.IsEffectType<IRescueEffect>());
		var effect =
			targetAssailant.EffectsOfType<RecentlyRescuedTarget>()
			               .FirstOrDefault(x => x.Rescued == Target && x.Rescuer == Assailant);
		if (effect == null)
		{
			effect = new RecentlyRescuedTarget(targetAssailant, Target, Assailant);
			targetAssailant.AddEffect(effect, TimeSpan.FromSeconds((1 + (int)outcome.Degree) * 10));
		}
		else
		{
			targetAssailant.RescheduleIfLonger(effect, TimeSpan.FromSeconds((1 + (int)outcome.Degree) * 10));
		}

		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = RecoveryDifficultySuccess,
			AttackerOutcome = resultRescuer.Outcome,
			DefenderOutcome = resultAssailant.Outcome
		};
	}
}