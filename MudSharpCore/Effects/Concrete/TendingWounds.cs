using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
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

namespace MudSharp.Effects.Concrete;

public class TendingWounds : CharacterActionWithTarget, IAffectProximity
{
    private static string _effectDurationDiceExpression;

    private static string EffectDurationDiceExpression
    {
        get
        {
            if (_effectDurationDiceExpression == null)
            {
                _effectDurationDiceExpression = Futuremud.Games.First()
                                                         .GetStaticConfiguration("TendingEffectDurationDiceExpression");
            }

            return _effectDurationDiceExpression;
        }
    }

    public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

    public IInventoryPlan OriginalInventoryPlan { get; set; }

    private readonly HashSet<(IWound Wound, TreatmentType Type)> _treatedWounds = new();

    #region Overrides of Effect

    public override string Describe(IPerceiver voyeur)
    {
        return $"Tending to the wounds of {TargetCharacter.HowSeen(voyeur)}.";
    }

    protected override string SpecificEffectType => "TendingWounds";

    #endregion

    public TendingWounds(ICharacter owner, ICharacter target) : base(owner, target)
    {
        WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is tending to $1's wounds.";
        CancelEmoteString = "@ $0|stop|stops tending to $1's wounds.";
        LDescAddendum = "tending to $1's wounds";
        ActionDescription = "tending to $1's wounds";
        _blocks.Add("general");
        _blocks.Add("movement");
    }

    public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
    {
        if (TargetCharacter == thing)
        {
            return (true, Proximity.Immediate);
        }

        return (false, Proximity.Unapproximable);
    }

    #region Overrides of TargetedBlockingDelayedAction

    /// <summary>
    ///     Fires when an effect is removed, including a matured scheduled effect
    /// </summary>
    public override void RemovalEffect()
    {
        OriginalInventoryPlan?.FinalisePlan();
        ReleaseEventHandlers();
    }

    #endregion

    public override void ExpireEffect()
    {
        List<IWound> wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination).ToList();
        (IWound Wound, TreatmentType Type, Difficulty Difficulty) treatment = GetPendingTreatment(wounds);
        if (treatment.Wound == null)
        {
            CharacterOwner.OutputHandler.Handle(new EmoteOutput(
                new Emote(
                    "@ have|has finished providing all of the wound care that $1's visible wounds currently need.",
                    CharacterOwner, CharacterOwner, TargetCharacter)));
            Owner.RemoveEffect(this, true);
            return;
        }

        IInventoryPlan inventoryPlan = Gameworld.TendInventoryPlanTemplate.CreatePlan(CharacterOwner);
        if (OriginalInventoryPlan == null)
        {
            OriginalInventoryPlan = inventoryPlan;
        }

        if (inventoryPlan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
        {
            inventoryPlan.ExecuteWholePlan();
        }

        if (inventoryPlan != OriginalInventoryPlan)
        {
            inventoryPlan.FinalisePlanNoRestore();
        }

        ITreatment treatmentItem = GetTreatmentItem(treatment.Type, treatment.Difficulty);

        if (treatmentItem == null)
        {
            CharacterOwner.OutputHandler.Handle(new EmoteOutput(
                new Emote(
                    "@ stop|stops tending to $1's wounds because #0 no longer has the correct tools.",
                    CharacterOwner, CharacterOwner, TargetCharacter)));
            Owner.RemoveEffect(this, true);
            return;
        }

        ICheck check = Gameworld.GetCheck(treatment.Type == TreatmentType.AntiInflammatory
            ? CheckType.CleanWoundCheck
            : CheckType.TendWoundCheck);
        treatment.Wound.Treat(CharacterOwner, treatment.Type, treatmentItem,
            check.Check(CharacterOwner, treatment.Difficulty), false);
        _treatedWounds.Add((treatment.Wound, treatment.Type));

        if (GetPendingTreatment(TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination).ToList())
            .Wound != null)
        {
            CharacterOwner.OutputHandler.Handle(new EmoteOutput(
                new Emote(
                    "@ continue|continues &0's medical efforts, as $1 still $1|have|has wounds that need attention.",
                    CharacterOwner, CharacterOwner, TargetCharacter)));
            CharacterOwner.Reschedule(this, TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression)));
            return;
        }

        CharacterOwner.OutputHandler.Handle(new EmoteOutput(
            new Emote(
                "@ have|has finished providing all of the wound care that $1's visible wounds currently need.",
                CharacterOwner, CharacterOwner, TargetCharacter)));
        Owner.RemoveEffect(this, true);
    }

    private (IWound Wound, TreatmentType Type, Difficulty Difficulty) GetPendingTreatment(List<IWound> wounds)
    {
        (IWound Wound, TreatmentType Type, Difficulty Difficulty) tend = wounds
                   .Where(x => x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible)
                   .Where(x => !_treatedWounds.Contains((x, TreatmentType.Tend)))
                   .OrderByDescending(x => x.Severity)
                   .ThenBy(x => x.CanBeTreated(TreatmentType.Tend))
                   .Select(x => (Wound: x, Type: TreatmentType.Tend, Difficulty: x.CanBeTreated(TreatmentType.Tend)))
                   .FirstOrDefault();
        if (tend.Wound != null)
        {
            return tend;
        }

        return wounds
               .Where(x => x.CanBeTreated(TreatmentType.AntiInflammatory) != Difficulty.Impossible)
               .Where(x => !_treatedWounds.Contains((x, TreatmentType.AntiInflammatory)))
               .OrderByDescending(x => x.Severity)
               .ThenBy(x => x.CanBeTreated(TreatmentType.AntiInflammatory))
               .Select(x => (Wound: x, Type: TreatmentType.AntiInflammatory,
                   Difficulty: x.CanBeTreated(TreatmentType.AntiInflammatory)))
               .FirstOrDefault();
    }

    private ITreatment GetTreatmentItem(TreatmentType type, Difficulty difficulty)
    {
        return CharacterOwner.Body.HeldItems
                             .SelectNotNull(x => x.GetItemType<ITreatment>())
                             .Where(x => x.IsTreatmentType(type))
                             .FirstMin(x => x.GetTreatmentDifficulty(difficulty));
    }
}
