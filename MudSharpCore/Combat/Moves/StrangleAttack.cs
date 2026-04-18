using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class StrangleAttack : NaturalAttackMove
{
    public ICharacter CharacterTarget { get; set; }

    public StrangleAttack(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
    {
        CharacterTarget = target;
    }

    private BuiltInCombatMoveType _moveType = BuiltInCombatMoveType.StrangleAttack;

    public override BuiltInCombatMoveType MoveType => _moveType;

    public override CheckType Check => CheckType.StrangleCheck;

    public override string Description => "Attempting to strangle an opponent";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        // Find a target limb which contains breathing organs
        ILimb targetLimb = CharacterTarget.Body.Limbs
                                        .Select(x => (Limb: x, Parts: CharacterTarget.Body.BodypartsForLimb(x)))
                                        .FirstOrDefault(x => x.Parts.Any(y => y.Organs.Any(z => z is TracheaProto)))
                                        .Limb;
        if (targetLimb == null)
        {
            return CombatMoveResult.Irrelevant;
        }

        // If the target limb is already grappled, we can strangle. Otherwise, we do a special version of extend grapple (to allow strangle-specific emotes)
        if (CharacterTarget.EffectsOfType<ILimbIneffectiveEffect>().Any(x =>
                x.Reason == LimbIneffectiveReason.Grappling && x.AppliesToLimb(targetLimb)))
        {
            return ResolveMoveChoke(targetLimb);
        }

        return ResolveMoveExtendGrapple(targetLimb);
    }

    private CombatMoveResult ResolveMoveExtendGrapple(ILimb targetLimb)
    {
        _moveType = BuiltInCombatMoveType.StrangleAttackExtendGrapple;
        ICombatMove defenderMove = CharacterTarget.ResponseToMove(this, Assailant);
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, CharacterTarget, null,
                                      Assailant.OffensiveAdvantage);
        string attackEmote =
            string.Format(
                      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
                          BuiltInCombatMoveType.StrangleAttackExtendGrapple, attackRoll.Outcome, null),
                      Bodypart.FullDescription(), targetLimb.Name.ToLowerInvariant())
                  .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());

        if (defenderMove is HelplessDefenseMove)
        {
            return ResolveMoveExtendGrappleHelplessResponse(defenderMove, attackRoll, attackEmote, targetLimb);
        }

        if (defenderMove is DodgeMove)
        {
            return ResolveMoveExtendGrappleDodgeResponse(defenderMove, attackRoll, attackEmote, targetLimb);
        }

        if (defenderMove is CounterGrappleMove)
        {
            return ResolveMoveExtendGrappleCounterResponse(defenderMove, attackRoll, attackEmote, targetLimb);
        }

        throw new NotImplementedException();
    }

    private CombatMoveResult ResolveMoveChoke(ILimb targetLimb)
    {
        IBodypart targetBodypart = CharacterTarget.Body.BodypartsForLimb(targetLimb)
                                            .Where(x => x.Organs.Any(y => y is TracheaProto)).GetRandomElement();
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, CharacterTarget, null,
                                      Assailant.OffensiveAdvantage);
        OpposedOutcome outcome = new(attackRoll, Outcome.NotTested);
        OpposedOutcomeDegree degree = outcome.Degree;
        string attackEmote =
            string.Format(
                      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
                          BuiltInCombatMoveType.StrangleAttack, attackRoll.Outcome, null), Bodypart.FullDescription(),
                      targetBodypart.Name.ToLowerInvariant(), targetLimb.Name.ToLowerInvariant())
                  .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());
        Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant, CharacterTarget),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        List<IWound> wounds = new();
        Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
        Attack.Profile.DamageExpression.Formula.Parameters["quality"] = Assailant.NaturalWeaponQuality(NaturalAttack);
        Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
        Attack.Profile.StunExpression.Formula.Parameters["quality"] = Assailant.NaturalWeaponQuality(NaturalAttack);
        Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
        Attack.Profile.PainExpression.Formula.Parameters["quality"] = Assailant.NaturalWeaponQuality(NaturalAttack);

        Damage finalDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = null,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = targetBodypart,
            DamageAmount =
                Attack.Profile.DamageExpression.Evaluate(Assailant),
            DamageType = Attack.Profile.DamageType,
            PainAmount =
                Attack.Profile.PainExpression.Evaluate(Assailant),
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount = Attack.Profile.DamageExpression.Evaluate(Assailant)
        };

        IEffect effect = Assailant.EffectsOfType<Strangling>().FirstOrDefault(x => x.Target == CharacterTarget);
        if (effect == null)
        {
            effect = new Strangling(Assailant, CharacterTarget);
            Assailant.AddEffect(effect);
        }

        foreach (TracheaProto trachea in targetBodypart.Organs.OfType<TracheaProto>())
        {
            effect = CharacterTarget.Body.EffectsOfType<BeingStrangled>().FirstOrDefault(x => x.Bodypart == trachea);
            if (effect == null)
            {
                effect = new BeingStrangled(CharacterTarget.Body, Assailant) { Bodypart = trachea };
                CharacterTarget.Body.AddEffect(effect);
            }
        }

        wounds.AddRange(CharacterTarget.SufferDamage(finalDamage));

        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            AttackerOutcome = attackRoll,
            DefenderOutcome = Outcome.NotTested,
            RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
            WoundsCaused = wounds
        };
    }

    private CombatMoveResult ResolveMoveExtendGrappleCounterResponse(ICombatMove defenderMove, CheckOutcome attackRoll,
        string attackEmote, ILimb targetLimb)
    {
        CheckOutcome counterCheck = Gameworld.GetCheck(defenderMove.Check)
                                    .Check(CharacterTarget, Attack.Profile.BaseParryDifficulty, Assailant, null,
                                        CharacterTarget.DefensiveAdvantage -
                                        GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
        CharacterTarget.DefensiveAdvantage = 0;
        OpposedOutcome result = new(attackRoll, counterCheck);

        if (result.Outcome == OpposedOutcomeDirection.Proponent || result.Outcome == OpposedOutcomeDirection.Stalemate)
        {
            string counterEmote = string.Format(Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget,
                    Assailant,
                    null, Attack,
                    BuiltInCombatMoveType.CounterGrapple, counterCheck.Outcome, Bodypart), Bodypart.FullDescription(),
                targetLimb.Name.ToLowerInvariant());
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            IGrappling grapple = Assailant.EffectsOfType<IGrappling>().First();
            grapple.AddLimb(targetLimb);
            return new CombatMoveResult
            {
                AttackerOutcome = attackRoll,
                DefenderOutcome = counterCheck.Outcome,
                MoveWasSuccessful = true,
                RecoveryDifficulty = RecoveryDifficultySuccess
            };
        }
        else
        {
            IGrappling grapple = Assailant.EffectsOfType<IGrappling>().First();
            Assailant.RemoveEffect(grapple);
            Grappling effect = new(CharacterTarget, Assailant);
            CharacterTarget.AddEffect(effect);
            int freeLimbs = 0;
            switch (result.Degree)
            {
                case OpposedOutcomeDegree.Moderate:
                    freeLimbs = 1;
                    break;
                case OpposedOutcomeDegree.Major:
                    freeLimbs = 2;
                    break;
                case OpposedOutcomeDegree.Total:
                    freeLimbs = 3;
                    break;
            }

            List<ILimb> potentialLimbs = CharacterTarget.Body.Limbs.Where(x =>
                !CharacterTarget.Body.EffectsOfType<ILimbIneffectiveEffect>().Any(y =>
                    y.Reason == LimbIneffectiveReason.Grappling && y.AppliesToLimb(x))).ToList();
            for (int i = 0; i < freeLimbs; i++)
            {
                if (!potentialLimbs.Any())
                {
                    break;
                }

                ILimb limb = potentialLimbs.GetRandomElement();
                effect.AddLimb(limb);
                potentialLimbs.Remove(limb);
            }

            string counterEmote = string.Format(Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
                    null, Attack,
                    BuiltInCombatMoveType.CounterGrapple, counterCheck.Outcome, Bodypart),
                effect.LimbsUnderControl.Select(x => x.Name.ToLowerInvariant()).ListToString());
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

            return new CombatMoveResult
            {
                AttackerOutcome = attackRoll,
                DefenderOutcome = counterCheck.Outcome,
                MoveWasSuccessful = false,
                RecoveryDifficulty = RecoveryDifficultyFailure
            };
        }
    }

    private CombatMoveResult ResolveMoveExtendGrappleDodgeResponse(ICombatMove defenderMove, CheckOutcome attackRoll,
        string attackEmote, ILimb targetLimb)
    {
        ICheck check = Gameworld.GetCheck(CheckType.DodgeCheck);
        CheckOutcome result = check.Check(CharacterTarget, Difficulty.Normal, Assailant,
            externalBonus: CharacterTarget.DefensiveAdvantage);
        CharacterTarget.DefensiveAdvantage = 0;
        OpposedOutcome opposed = new(attackRoll, result);

        if (opposed.Outcome == OpposedOutcomeDirection.Proponent ||
            opposed.Outcome == OpposedOutcomeDirection.Stalemate)
        {
            string failEmote = string.Format(
                Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant, null, null,
                    BuiltInCombatMoveType.DodgeExtendGrapple, result.Outcome, null), Bodypart.FullDescription(),
                targetLimb.Name.ToLowerInvariant());
            Assailant.OutputHandler.Handle(new EmoteOutput(
                new Emote($"{attackEmote}{failEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            IGrappling grapple = Assailant.EffectsOfType<IGrappling>().First();
            grapple.AddLimb(targetLimb);
            return new CombatMoveResult
            {
                AttackerOutcome = attackRoll,
                DefenderOutcome = result.Outcome,
                MoveWasSuccessful = true,
                RecoveryDifficulty = RecoveryDifficultySuccess
            };
        }

        string defenseEmote =
            string.Format(
                Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant, null, null,
                    BuiltInCombatMoveType.DodgeExtendGrapple, result.Outcome, null), Bodypart.FullDescription(),
                targetLimb.Name.ToLowerInvariant());
        Assailant.OutputHandler.Handle(new EmoteOutput(
            new Emote($"{attackEmote}{defenseEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        return new CombatMoveResult
        {
            AttackerOutcome = attackRoll,
            DefenderOutcome = result.Outcome,
            MoveWasSuccessful = false,
            RecoveryDifficulty = RecoveryDifficultyFailure
        };
    }

    private CombatMoveResult ResolveMoveExtendGrappleHelplessResponse(ICombatMove defenderMove, CheckOutcome attackRoll,
        string attackEmote, ILimb targetLimb)
    {
        WorsenCombatPosition(CharacterTarget, Assailant);
        OpposedOutcome result = new(attackRoll, Outcome.NotTested);
        Assailant.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"{attackEmote}, and #0 %0|are|is successful!",
                    Assailant, Assailant, CharacterTarget, null), style: OutputStyle.CombatMessage,
                flags: OutputFlags.InnerWrap));
        IGrappling grapple = Assailant.EffectsOfType<IGrappling>().First();
        grapple.AddLimb(targetLimb);
        return new CombatMoveResult
        {
            AttackerOutcome = attackRoll,
            DefenderOutcome = Outcome.NotTested,
            MoveWasSuccessful = true,
            RecoveryDifficulty = Difficulty.Automatic
        };
    }
}