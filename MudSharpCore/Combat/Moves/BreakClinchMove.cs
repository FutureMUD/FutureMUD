using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class BreakClinchMove : CombatMoveBase
{
	public BreakClinchMove(ICharacter assailant, ICharacter target)
	{
		Assailant = assailant;
		CharacterTarget = target;
	}

	public override string Description => "Attempting to break free of a clinch";

	public ICharacter CharacterTarget { get; set; }

	public override Difficulty RecoveryDifficultySuccess => Difficulty.Easy;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Normal;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	public override double BaseDelay => 0.5; // TODO - programmatically loaded

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return gameworld.GetStaticDouble("BreakClinchMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTarget };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(CheckType.BreakClinch)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		var attackEmote = Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, null, null,
			BuiltInCombatMoveType.BreakClinch, attackRoll[CheckDifficulty], null);

		if (defenderMove is HelplessDefenseMove)
		{
			return ResolveHelplessDefense(attackEmote);
		}

		if (defenderMove is StartClinchMove clinch)
		{
			return ResolveClinchMove(attackRoll, attackEmote);
		}

		throw new NotImplementedException();
	}

	private CombatMoveResult ResolveClinchMove(IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		string attackEmote)
	{
		var dodgeCheck = Gameworld.GetCheck(CheckType.ResistBreakClinch)
		                          .CheckAgainstAllDifficulties(CharacterTarget, Difficulty.Normal, null, Assailant,
			                          CharacterTarget.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		CharacterTarget.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, dodgeCheck, CheckDifficulty, Difficulty.Normal);
#if DEBUG
		Console.WriteLine($"BreakClinchMove Clinch Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant, null, null,
				BuiltInCombatMoveType.ResistBreakClinch, dodgeCheck[Difficulty.Normal], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{dodgeEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == CharacterTarget);
			CharacterTarget.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == Assailant);
			CharacterTarget.AddEffect(new ClinchCooldown(CharacterTarget, CharacterTarget.Combat),
				TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultySuccess,
				AttackerOutcome = attackRoll[CheckDifficulty],
				DefenderOutcome = dodgeCheck[Difficulty.Normal],
				MoveWasSuccessful = true
			};
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant, null, null,
				BuiltInCombatMoveType.ResistBreakClinch, dodgeCheck[Difficulty.Normal], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{dodgeEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			if (attackRoll[CheckDifficulty].IsPass())
			{
				Gameworld.Scheduler.DelayScheduleType(CharacterTarget, ScheduleType.Combat,
					TimeSpan.FromSeconds(Gameworld.GetStaticDouble("BreakClinchMoveOnGoodFailDelay") *
					                     attackRoll[CheckDifficulty].SuccessDegrees()));
			}

			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = attackRoll[CheckDifficulty],
				DefenderOutcome = dodgeCheck[Difficulty.Normal]
			};
		}
	}

	private CombatMoveResult ResolveHelplessDefense(string attackEmote)
	{
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and $1 $1|are|is unable to stop &0.", Assailant, Assailant,
					CharacterTarget), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		Assailant.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == CharacterTarget);
		CharacterTarget.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>()?.Target == Assailant);
		CharacterTarget.AddEffect(new ClinchCooldown(CharacterTarget, CharacterTarget.Combat),
			TimeSpan.FromSeconds(30 * CombatBase.CombatSpeedMultiplier));
		return new CombatMoveResult { RecoveryDifficulty = Difficulty.Automatic };
	}
}