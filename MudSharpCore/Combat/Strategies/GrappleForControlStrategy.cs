using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Strategies;

public class GrappleForControlStrategy : ClinchStrategy
{
    public override CombatStrategyMode Mode => CombatStrategyMode.GrappleForControl;

    protected GrappleForControlStrategy()
    {
    }

    public new static GrappleForControlStrategy Instance { get; } = new();

    protected override ICombatMove AttemptClinchAttack(ICharacter ch)
    {
        return AttemptGrapple(ch) ?? base.AttemptClinchAttack(ch);
    }

    protected virtual ICombatMove AttemptGrappleInProgress(ICharacter ch, IGrappling grapple,
        IEnumerable<(ILimb Limb, bool Grappled)> limbStates)
    {
        if (!ch.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Trip) &&
            limbStates.Count(x => x.Grappled) > 1)
        {
            List<INaturalAttack> possibleTakedowns = ch.Race
                                      .UsableNaturalWeaponAttacks(ch, grapple.Target, false,
                                          BuiltInCombatMoveType.TakedownMove).ToList();
            List<INaturalAttack> takedowns = possibleTakedowns.Where(x => ch.CanSpendStamina(x.Attack.StaminaCost)).ToList();
            if (takedowns.Any())
            {
                List<INaturalAttack> preferredAttacks = takedowns
                                       .Where(x => x.Attack.Intentions.HasFlag(ch.CombatSettings.PreferredIntentions))
                                       .ToList();
                if (preferredAttacks.Any() && Dice.Roll(1, 2) == 1)
                {
                    takedowns = preferredAttacks;
                }

                INaturalAttack attack = takedowns.GetWeightedRandom(x => x.Attack.Weighting);
                return new TakedownMove(ch, attack, grapple.Target);
            }
        }

        List<ILimb> potentialLimbs = limbStates.Where(x => !x.Grappled).Select(x => x.Limb).ToList();
        if (!potentialLimbs.Any())
        {
            return null;
        }

        List<INaturalAttack> possibleAttacks = ch.Race
                                .UsableNaturalWeaponAttacks(ch, grapple.Target, false,
                                    BuiltInCombatMoveType.ExtendGrapple).Where(x =>
                                    potentialLimbs.Any(y =>
                                        y.LimbType == ((ITargetLimbWeaponAttack)x.Attack).TargetLimbType)).ToList();
        List<INaturalAttack> attacks = possibleAttacks.Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack)))
                                     .ToList();
        if (!attacks.Any())
        {
            if (possibleAttacks.Any())
            {
                return new TooExhaustedMove { Assailant = ch };
            }

            return null;
        }

        return new ExtendGrappleMove(ch, attacks.GetWeightedRandom(x => x.Attack.Weighting), grapple.Target);
    }

    protected ICombatMove AttemptGrapple(ICharacter ch)
    {
        if (!(ch.CombatTarget is ICharacter tch))
        {
            return null;
        }

        IGrappling grapple = ch.EffectsOfType<IGrappling>().FirstOrDefault();
        if (grapple != null)
        {
            List<(ILimb Limb, bool Grappled)> potentialLimbs = grapple.Target.Body.Limbs
                                        .Where(x => grapple.Target.Body.BodypartsForLimb(x).Any())
                                        .Select(x => (Limb: x,
                                            Grappled: grapple.Target.Body.EffectsOfType<ILimbIneffectiveEffect>().Any(
                                                y =>
                                                    y.Reason == LimbIneffectiveReason.Grappling && y.AppliesToLimb(x))))
                                        .ToList();
            if (potentialLimbs.Any(x => !x.Grappled))
            {
                return AttemptGrappleInProgress(ch, grapple, potentialLimbs);
            }

            return null;
        }

        List<INaturalAttack> possibleAttacks = ch.Race.UsableNaturalWeaponAttacks(ch, tch, false, BuiltInCombatMoveType.InitiateGrapple)
                                .ToList();
        List<INaturalAttack> attacks = possibleAttacks.Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack)))
                                     .ToList();
        if (!attacks.Any())
        {
            if (possibleAttacks.Any())
            {
                return new TooExhaustedMove { Assailant = ch };
            }

            return null;
        }

        return new InitiateGrappleMove(ch, attacks.GetWeightedRandom(x => x.Attack.Weighting),
            (ICharacter)ch.CombatTarget);
    }
}