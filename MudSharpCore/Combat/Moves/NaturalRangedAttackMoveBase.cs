using MudSharp.Body;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

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

	internal static bool TargetIsInRange(ICharacter assailant, IPerceivable target, int rangeInRooms)
	{
		if (assailant.Location == null || target?.Location == null)
		{
			return false;
		}

		if (assailant.Location == target.Location)
		{
			return true;
		}

		if (rangeInRooms <= 0)
		{
			return false;
		}

		List<ICellExit> path = assailant
			.PathBetween(target, (uint)rangeInRooms, false, false, true)?
			.ToList() ?? [];
		return path.Count > 0 && path.Count <= rangeInRooms;
	}

    protected virtual IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
        OpposedOutcomeDegree degree, IBodypart bodypart)
    {
        if (target is not IHaveWounds targetWithWounds)
        {
            return Enumerable.Empty<IWound>();
        }

        var targetMaterial = (target as IGameItem)?.Material as ISolid;
        Tuple<IDamage, IDamage> damages = GetDamagePlusSelfDamage(target, Bodypart, bodypart, targetMaterial, attackOutcome, Attack.Profile.DamageType,
            Attack.Profile.BaseAngleOfIncidence, NaturalAttack, degree);
		return targetWithWounds.PassiveSufferDamage(damages.Item1).ToList();
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
			IBodypart scatterBodypart = scatter.Target is ICharacter scatterCharacter
				? scatterCharacter.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment,
					Assailant.GetFacingFor(scatterCharacter, true), false)
				: null;
			wounds.AddRange(ApplySuccessfulHit(scatter.Target, attackOutcome, OpposedOutcomeDegree.None,
				scatterBodypart));
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
            (block.Shield as IConditionDegradingComponent)?.UseCondition(
                new ItemConditionUseContext(ItemConditionUseKind.ShieldBlock, blockCheck, (int)opposed.Degree));
            return new CombatMoveResult
            {
                RecoveryDifficulty = RecoveryDifficultyFailure,
                AttackerOutcome = attackOutcome.Outcome,
                DefenderOutcome = blockCheck.Outcome
            };
        }

        List<IWound> wounds = ApplySuccessfulHit(target, attackOutcome, opposed.Degree, TargetBodypart).ToList();
        wounds.ProcessPassiveWounds();
        (block.Shield as IConditionDegradingComponent)?.UseCondition(
            new ItemConditionUseContext(ItemConditionUseKind.ShieldBlock, blockCheck, (int)opposed.Degree));
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
		IPerceiver target = Targets.FirstOrDefault();
		if (target is null || !TargetIsInRange(Assailant, target, RangedAttack.RangeInRooms))
		{
			return CombatMoveResult.Irrelevant;
		}

		if (target is not ICharacter characterTarget)
		{
			CheckOutcome attackOutcome = Gameworld.GetCheck(Check)
				.Check(Assailant, CheckDifficulty, null, target, Assailant.OffensiveAdvantage);
			Assailant.OffensiveAdvantage = 0;
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote(BuildAttackEmote(target, attackOutcome.Outcome).Fullstop(), Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			if (attackOutcome.IsFail())
			{
				return HandleMiss(target, attackOutcome);
			}

			List<IWound> itemWounds = ApplySuccessfulHit(target, attackOutcome, OpposedOutcomeDegree.Total, null)
				.ToList();
			itemWounds.ProcessPassiveWounds();
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess,
				AttackerOutcome = attackOutcome.Outcome,
				DefenderOutcome = Outcome.NotTested,
				WoundsCaused = itemWounds
			};
		}

		ICharacter targetCharacter = characterTarget;
        defenderMove ??= new HelplessDefenseMove { Assailant = targetCharacter };
        Dictionary<Difficulty, CheckOutcome> attackRoll = Gameworld.GetCheck(Check)
                                  .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, targetCharacter,
                                      Assailant.OffensiveAdvantage);
        Assailant.OffensiveAdvantage = 0;
        DetermineTargetBodypart(defenderMove, attackRoll[CheckDifficulty]);
		var effectiveCover = VehicleCombatService.Instance.ResolveEffectiveRangedCover(Assailant, targetCharacter);
		var coverDifficulty = effectiveCover?.Cover.MinimumRangedDifficulty ?? CheckDifficulty;
		if (coverDifficulty < CheckDifficulty)
		{
			coverDifficulty = CheckDifficulty;
		}

        string attackEmote = BuildAttackEmote(targetCharacter, attackRoll[CheckDifficulty].Outcome);
        Assailant.OutputHandler.Handle(new EmoteOutput(
            new Emote(attackEmote.Fullstop(), Assailant, Assailant, targetCharacter),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        if (attackRoll[CheckDifficulty].IsFail())
        {
            return HandleMiss(targetCharacter, attackRoll[CheckDifficulty]);
        }

		if (effectiveCover is not null && attackRoll[coverDifficulty].IsFail() &&
		    (effectiveCover.Cover.CoverType == CoverType.Hard ||
		     attackRoll[CheckDifficulty].Outcome == Outcome.MajorPass ||
		     attackRoll[coverDifficulty].Outcome == Outcome.MinorFail))
		{
			targetCharacter.OutputHandler.Handle(new EmoteOutput(
				new Emote("The attack strikes $?1|$1, ||$$0's cover!", targetCharacter, targetCharacter,
					effectiveCover.Provider),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));
			return new CombatMoveResult
			{
				MoveWasSuccessful = false,
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = attackRoll[CheckDifficulty].Outcome,
				DefenderOutcome = attackRoll[coverDifficulty].Outcome
			};
		}

        switch (defenderMove)
        {
            case HelplessDefenseMove:
            case TooExhaustedMove:
                {
                    List<IWound> wounds = ApplySuccessfulHit(targetCharacter, attackRoll[CheckDifficulty], OpposedOutcomeDegree.Total, TargetBodypart)
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
                return ResolveDodgeMove(targetCharacter, dodge, attackRoll[CheckDifficulty]);
            case BlockMove block:
                return ResolveBlockMove(targetCharacter, block, attackRoll[CheckDifficulty]);
            default:
                {
                    List<IWound> wounds = ApplySuccessfulHit(targetCharacter, attackRoll[CheckDifficulty], OpposedOutcomeDegree.Moderate, TargetBodypart)
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
