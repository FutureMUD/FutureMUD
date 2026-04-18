using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public abstract class NaturalRangedAttackMoveBase : WeaponAttackMove, IRangedAttackMove
{
    protected NaturalRangedAttackMoveBase(ICharacter owner, INaturalAttack attack, IPerceiver target) : base(attack.Attack)
    {
        Assailant = owner;
        NaturalAttack = attack;
        if (target is ICharacter tch)
        {
            _characterTargets.Add(tch);
        }

        if (target is not null)
        {
            _targets.Add(target);
        }
    }

    public INaturalAttack NaturalAttack { get; }
    public IBodypart Bodypart => NaturalAttack.Bodypart;
    protected IRangedNaturalAttack RangedAttack => Attack.GetAttackType<IRangedNaturalAttack>();

    public override int Reach => 0;
    public override double BaseDelay => Attack.BaseDelay;
    public override ExertionLevel AssociatedExertion => Attack.ExertionLevel;
    public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
    public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;
    public override Difficulty CheckDifficulty => Attack.Profile.BaseAttackerDifficulty;
    public override double StaminaCost => NaturalAttackMove.MoveStaminaCost(Assailant, Attack);

    protected abstract CheckType RangedCheck { get; }

    public override CheckType Check => RangedCheck;

    protected virtual IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
        OpposedOutcomeDegree degree, IBodypart bodypart)
    {
        if (target is not ICharacter tch)
        {
            return Enumerable.Empty<IWound>();
        }

        Tuple<IDamage, IDamage> damages = GetDamagePlusSelfDamage(tch, Bodypart, bodypart, null, attackOutcome, Attack.Profile.DamageType,
            Attack.Profile.BaseAngleOfIncidence, NaturalAttack, degree);
        List<IWound> wounds = tch.PassiveSufferDamage(damages.Item1).ToList();
        Assailant.PassiveSufferDamage(damages.Item2).ProcessPassiveWounds();
        return wounds;
    }

    protected virtual CombatMoveResult HandleMiss(IPerceiver originalTarget, CheckOutcome attackOutcome)
    {
        List<IWound> wounds = new();
        List<ICellExit> path = Assailant.PathBetween(originalTarget, (uint)RangedAttack.RangeInRooms, false, false, true)?.ToList() ??
                   new List<ICellExit>();
        RangedScatterResult scatter = RangedScatterStrategyFactory.GetStrategy(RangedAttack.ScatterType)
                                                 .GetScatterTarget(Assailant, originalTarget, path);
        if (scatter?.Target is not null)
        {
            wounds.AddRange(ApplySuccessfulHit(scatter.Target, attackOutcome, OpposedOutcomeDegree.None, null));
            HandleScatterImpact(scatter, attackOutcome);
        }
        else if (scatter is not null)
        {
            HandleScatterImpact(scatter, attackOutcome);
        }

        wounds.ProcessPassiveWounds();
        return new CombatMoveResult
        {
            MoveWasSuccessful = wounds.Any(),
            RecoveryDifficulty = RecoveryDifficultyFailure,
            AttackerOutcome = attackOutcome.Outcome,
            DefenderOutcome = Outcome.NotTested,
            WoundsCaused = wounds
        };
    }

    protected virtual void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
    {
        // Default: no additional effect when the scattered shot lands in a cell.
    }

    protected virtual string BuildAttackEmote(IPerceiver target, Outcome outcome)
    {
        return string.Format(
            Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, Attack, MoveType, outcome, Bodypart),
            Bodypart.FullDescription(), TargetBodypart?.FullDescription() ?? "body");
    }

    private CombatMoveResult ResolveBlockMove(ICharacter target, BlockMove block, CheckOutcome attackOutcome)
    {
        Difficulty defenderDifficulty = Attack.Profile.BaseBlockDifficulty.StageUp(block.DifficultStageUps);
        CheckOutcome blockCheck = Gameworld.GetCheck(block.Check).Check(target, defenderDifficulty, block.Shield.ShieldType.BlockTrait, Assailant);
        OpposedOutcome opposed = new(attackOutcome, blockCheck);
        if (opposed.Outcome == OpposedOutcomeDirection.Opponent)
        {
            return new CombatMoveResult
            {
                RecoveryDifficulty = RecoveryDifficultyFailure,
                AttackerOutcome = attackOutcome.Outcome,
                DefenderOutcome = blockCheck.Outcome
            };
        }

        List<IWound> wounds = ApplySuccessfulHit(target, attackOutcome, opposed.Degree, TargetBodypart).ToList();
        wounds.ProcessPassiveWounds();
        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            RecoveryDifficulty = RecoveryDifficultySuccess,
            AttackerOutcome = attackOutcome.Outcome,
            DefenderOutcome = blockCheck.Outcome,
            WoundsCaused = wounds
        };
    }

    private CombatMoveResult ResolveDodgeMove(ICharacter target, DodgeRangeMove dodge, CheckOutcome attackOutcome)
    {
        CheckOutcome dodgeCheck = Gameworld.GetCheck(dodge.Check).Check(target, Attack.Profile.BaseDodgeDifficulty, Assailant);
        OpposedOutcome opposed = new(attackOutcome, dodgeCheck);
        if (opposed.Outcome == OpposedOutcomeDirection.Opponent)
        {
            return HandleMiss(target, attackOutcome);
        }

        List<IWound> wounds = ApplySuccessfulHit(target, attackOutcome, opposed.Degree, TargetBodypart).ToList();
        wounds.ProcessPassiveWounds();
        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            RecoveryDifficulty = RecoveryDifficultySuccess,
            AttackerOutcome = attackOutcome.Outcome,
            DefenderOutcome = dodgeCheck.Outcome,
            WoundsCaused = wounds
        };
    }

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        ICharacter target = CharacterTargets.FirstOrDefault();
        if (target is null)
        {
            return CombatMoveResult.Irrelevant;
        }

        defenderMove ??= new HelplessDefenseMove { Assailant = target };
        Dictionary<Difficulty, CheckOutcome> attackRoll = Gameworld.GetCheck(Check)
                                  .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, target,
                                      Assailant.OffensiveAdvantage);
        Assailant.OffensiveAdvantage = 0;
        DetermineTargetBodypart(defenderMove, attackRoll[CheckDifficulty]);

        string attackEmote = BuildAttackEmote(target, attackRoll[CheckDifficulty].Outcome);
        Assailant.OutputHandler.Handle(new EmoteOutput(
            new Emote(attackEmote.Fullstop(), Assailant, Assailant, target),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        if (attackRoll[CheckDifficulty].IsFail())
        {
            return HandleMiss(target, attackRoll[CheckDifficulty]);
        }

        switch (defenderMove)
        {
            case HelplessDefenseMove:
            case TooExhaustedMove:
                {
                    List<IWound> wounds = ApplySuccessfulHit(target, attackRoll[CheckDifficulty], OpposedOutcomeDegree.Total, TargetBodypart)
                                 .ToList();
                    wounds.ProcessPassiveWounds();
                    return new CombatMoveResult
                    {
                        MoveWasSuccessful = true,
                        RecoveryDifficulty = RecoveryDifficultySuccess,
                        AttackerOutcome = attackRoll[CheckDifficulty].Outcome,
                        DefenderOutcome = Outcome.NotTested,
                        WoundsCaused = wounds
                    };
                }
            case DodgeRangeMove dodge:
                return ResolveDodgeMove(target, dodge, attackRoll[CheckDifficulty]);
            case BlockMove block:
                return ResolveBlockMove(target, block, attackRoll[CheckDifficulty]);
            default:
                {
                    List<IWound> wounds = ApplySuccessfulHit(target, attackRoll[CheckDifficulty], OpposedOutcomeDegree.Moderate, TargetBodypart)
                                 .ToList();
                    wounds.ProcessPassiveWounds();
                    return new CombatMoveResult
                    {
                        MoveWasSuccessful = true,
                        RecoveryDifficulty = RecoveryDifficultySuccess,
                        AttackerOutcome = attackRoll[CheckDifficulty].Outcome,
                        DefenderOutcome = Outcome.NotTested,
                        WoundsCaused = wounds
                    };
                }
        }
    }
}
