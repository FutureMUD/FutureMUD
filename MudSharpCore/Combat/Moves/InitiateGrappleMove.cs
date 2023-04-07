using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class InitiateGrappleMove : NaturalAttackMove
{
	public ICharacter CharacterTarget { get; set; }
	public override CheckType Check => CheckType.InitiateGrapple;

	public InitiateGrappleMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
		CharacterTarget = target;
	}

	public override string Description => "Attempting to initiate a grapple";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTarget };
		}

		WorsenCombatPosition(CharacterTarget, Assailant);
		var attackerSize = Assailant.CurrentContextualSize(SizeContext.GrappleAttack);
		var defenderSize = Assailant.CurrentContextualSize(SizeContext.GrappleDefense);

		var offset = (attackerSize - defenderSize) *
		             Gameworld.GetStaticDouble("InitiateGrappleOffsetPerSizeDifference");
		var attackerDifficulty = CheckDifficulty.ApplyBonus(offset);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .CheckAgainstAllDifficulties(Assailant, attackerDifficulty, null, CharacterTarget,
			                          Assailant.OffensiveAdvantage);
		var attackEmote =
			string.Format(
				      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
					      BuiltInCombatMoveType.InitiateGrapple, attackRoll[attackerDifficulty], null),
				      Bodypart.FullDescription())
			      .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());

		if (defenderMove is HelplessDefenseMove)
		{
			return ResolveMoveHelpless(defenderMove, attackRoll, attackEmote, attackerDifficulty);
		}

		if (defenderMove is DodgeMove)
		{
			return ResolveMoveDodge(defenderMove, attackRoll, attackEmote, offset, attackerDifficulty);
		}

		if (defenderMove is CounterGrappleMove)
		{
			return ResolveMoveCounter(defenderMove, attackRoll, attackEmote, offset, attackerDifficulty);
		}

		throw new NotImplementedException();
	}

	private CombatMoveResult ResolveMoveCounter(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, double offset,
		Difficulty attackerDifficulty)
	{
		var defenderDifficulty = Attack.Profile.BaseParryDifficulty.ApplyBonus(-1 * offset);
		var counterCheck = Gameworld.GetCheck(defenderMove.Check)
		                            .CheckAgainstAllDifficulties(CharacterTarget, defenderDifficulty, null, Assailant,
			                            CharacterTarget.DefensiveAdvantage -
			                            GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		CharacterTarget.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, counterCheck, attackerDifficulty, defenderDifficulty);

		if (result.Outcome == OpposedOutcomeDirection.Proponent || result.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var counterEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant,
				null, Attack,
				BuiltInCombatMoveType.CounterGrapple, counterCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var effect = new Grappling(Assailant, CharacterTarget);
			Assailant.AddEffect(effect);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = counterCheck[defenderDifficulty],
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess
			};
		}
		else
		{
			var counterEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
				null, Attack,
				BuiltInCombatMoveType.CounterGrapple, counterCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var effect = new Grappling(CharacterTarget, Assailant);
			CharacterTarget.AddEffect(effect);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = counterCheck[defenderDifficulty],
				MoveWasSuccessful = false,
				RecoveryDifficulty = RecoveryDifficultyFailure
			};
		}
	}

	private CombatMoveResult ResolveMoveDodge(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, double offset,
		Difficulty attackerDifficulty)
	{
		var defenderDifficulty = Attack.Profile.BaseDodgeDifficulty.ApplyBonus(-1 * offset);
		var dodgeCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .CheckAgainstAllDifficulties(CharacterTarget, defenderDifficulty, null, Assailant,
			                          CharacterTarget.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		CharacterTarget.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, dodgeCheck, attackerDifficulty, defenderDifficulty);

		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant,
				null, Attack,
				BuiltInCombatMoveType.DodgeGrapple, dodgeCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{dodgeEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var effect = new Grappling(Assailant, CharacterTarget);
			Assailant.AddEffect(effect);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = dodgeCheck[defenderDifficulty],
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess
			};
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
				null, Attack,
				BuiltInCombatMoveType.DodgeGrapple, dodgeCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{dodgeEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = dodgeCheck[defenderDifficulty],
				MoveWasSuccessful = false,
				RecoveryDifficulty = RecoveryDifficultyFailure
			};
		}
	}

	private CombatMoveResult ResolveMoveHelpless(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, Difficulty attackerDifficulty)
	{
		WorsenCombatPosition(CharacterTarget, Assailant);
		var result = new OpposedOutcome(attackRoll[attackerDifficulty], Outcome.NotTested);
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is successfully engaged in a grapple!",
					Assailant, Assailant, CharacterTarget, null), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var effect = new Grappling(Assailant, CharacterTarget);
		Assailant.AddEffect(effect);
		return new CombatMoveResult
		{
			AttackerOutcome = attackRoll[attackerDifficulty],
			DefenderOutcome = Outcome.NotTested,
			MoveWasSuccessful = true,
			RecoveryDifficulty = RecoveryDifficultySuccess
		};
	}
}