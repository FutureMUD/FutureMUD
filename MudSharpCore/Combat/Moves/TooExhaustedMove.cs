using System;
using System.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Combat.Moves;

public class TooExhaustedMove : CombatMoveBase, IDefenseMove
{
	#region Overrides of CombatMoveBase

	#region Overrides of CombatMoveBase

	public override double BaseDelay { get; } = 0.5;

	#endregion

	public override string Description => "Too exhausted to do anything";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var availableSecondWind = Assailant.Merits.OfType<ISecondWindMerit>().FirstOrDefault(x =>
			x.Applies(Assailant) && !Assailant.AffectedBy<ISecondWindExhaustedEffect>(x));
		if (availableSecondWind != null)
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(availableSecondWind.Emote, Assailant, Assailant),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.CurrentStamina = Assailant.MaximumStamina;
			Assailant.AddEffect(new SecondWindExhausted(Assailant, availableSecondWind),
				availableSecondWind.RecoveryDuration);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.VeryEasy
			};
		}

		if (Assailant.EffectsOfType<ISecondWindExhaustedEffect>().All(x => x.Merit != null))
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote(Gameworld.GetStaticString("DefaultSecondWindEmote"), Assailant, Assailant),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.CurrentStamina = Assailant.MaximumStamina;
			Assailant.AddEffect(new SecondWindExhausted(Assailant, null),
				TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultSecondWindRecoveryTime")));
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.VeryEasy
			};
		}

		Assailant.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ are|is too exhausted to do much of anything!", Assailant),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		return new CombatMoveResult
		{
			RecoveryDifficulty = Difficulty.VeryHard
		};
	}

	public int DifficultStageUps => 0;

	public void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}

	#endregion
}