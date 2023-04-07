using System.Collections.Generic;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.RPG.Merits;

namespace MudSharp.Body.Traits {
    public enum TraitBonusContext
    {
        None,
        UnarmedDamageCalculation,
        ArmedDamageCalculation,
        CarryingCapacity,
        DraggingCapacity,
        StaggeringBlowCheck,
        StaggeringBlowDefenseCheck,
        UnbalancingBlowCheck,
        UnbalancingBlowDefenseCheck,
        ProjectLabourQualification,
        BreathingRate,
        HoldBreathLength,
        Buoyancy,
        Encumbrance,
        CombatPowerMoveStamina,
        CombatGraceMoveStamina,
        CombatRelativeStrengthDefenseStamina,
        BreakoutFromGrapple,
        OpposeBreakoutFromGrapple,
        SpellDuration,
        SpellCost,
        JobEffortCalculation
    }

    public interface IHaveTraits : IFrameworkItem, IHaveEffects {
        IEnumerable<ITrait> Traits { get; }
        double TraitValue(ITraitDefinition trait, TraitBonusContext context = TraitBonusContext.None);
        double TraitRawValue(ITraitDefinition trait);
        double TraitMaxValue(ITraitDefinition trait);
        double TraitMaxValue(ITrait trait);
        bool HasTrait(ITraitDefinition trait);
        ITrait GetTrait(ITraitDefinition definition);
        string GetTraitDecorated(ITraitDefinition trait);
        IEnumerable<ITrait> TraitsOfType(TraitType type);
        bool AddTrait(ITraitDefinition trait, double value);
        bool RemoveTrait(ITraitDefinition trait);
        bool SetTraitValue(ITraitDefinition trait, double value);
    }

    public interface IPerceivableHaveTraits : IPerceivable, IHaveTraits, IHaveMerits {
        double GetCurrentBonusLevel();
    }
}