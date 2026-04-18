using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;

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
        GameItems.SizeCategory attackerSize = Assailant.CurrentContextualSize(SizeContext.GrappleAttack);
        GameItems.SizeCategory defenderSize = Assailant.CurrentContextualSize(SizeContext.GrappleDefense);

        double offset = (attackerSize - defenderSize) *
                     Gameworld.GetStaticDouble("InitiateGrappleOffsetPerSizeDifference");
        Difficulty attackerDifficulty = CheckDifficulty.ApplyBonus(offset);
        Dictionary<Difficulty, CheckOutcome> attackRoll = Gameworld.GetCheck(Check)
                                  .CheckAgainstAllDifficulties(Assailant, attackerDifficulty, null, CharacterTarget,
                                      Assailant.OffensiveAdvantage);
        string attackEmote =
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
        Difficulty defenderDifficulty = Attack.Profile.BaseParryDifficulty.ApplyBonus(-1 * offset);
        Dictionary<Difficulty, CheckOutcome> counterCheck = Gameworld.GetCheck(defenderMove.Check)
                                    .CheckAgainstAllDifficulties(CharacterTarget, defenderDifficulty, null, Assailant,
                                        CharacterTarget.DefensiveAdvantage -
                                        GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
        CharacterTarget.DefensiveAdvantage = 0;
        OpposedOutcome result = new(attackRoll, counterCheck, attackerDifficulty, defenderDifficulty);

        if (result.Outcome == OpposedOutcomeDirection.Proponent || result.Outcome == OpposedOutcomeDirection.Stalemate)
        {
            string counterEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant,
                null, Attack,
                BuiltInCombatMoveType.CounterGrapple, counterCheck[defenderDifficulty], Bodypart);
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            Grappling effect = new(Assailant, CharacterTarget);
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
            string counterEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
                null, Attack,
                BuiltInCombatMoveType.CounterGrapple, counterCheck[defenderDifficulty], Bodypart);
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            Grappling effect = new(CharacterTarget, Assailant);
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
        Difficulty defenderDifficulty = Attack.Profile.BaseDodgeDifficulty.ApplyBonus(-1 * offset);
        Dictionary<Difficulty, CheckOutcome> dodgeCheck = Gameworld.GetCheck(defenderMove.Check)
                                  .CheckAgainstAllDifficulties(CharacterTarget, defenderDifficulty, null, Assailant,
                                      CharacterTarget.DefensiveAdvantage -
                                      GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
        CharacterTarget.DefensiveAdvantage = 0;
        OpposedOutcome result = new(attackRoll, dodgeCheck, attackerDifficulty, defenderDifficulty);

        if (result.Outcome == OpposedOutcomeDirection.Proponent)
        {
            string dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant,
                null, Attack,
                BuiltInCombatMoveType.DodgeGrapple, dodgeCheck[defenderDifficulty], Bodypart);
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{dodgeEmote}", Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            Grappling effect = new(Assailant, CharacterTarget);
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
            string dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
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
        OpposedOutcome result = new(attackRoll[attackerDifficulty], Outcome.NotTested);
        Assailant.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"{attackEmote}, and #1 %1|are|is successfully engaged in a grapple!",
                    Assailant, Assailant, CharacterTarget, null), style: OutputStyle.CombatMessage,
                flags: OutputFlags.InnerWrap));
        Grappling effect = new(Assailant, CharacterTarget);
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