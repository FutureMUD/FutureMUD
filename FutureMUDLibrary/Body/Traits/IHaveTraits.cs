using System.Collections.Generic;
using MudSharp.Effects;
using MudSharp.Framework;

namespace MudSharp.Body.Traits
{

	public interface IHaveTraits : IFrameworkItem {
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
}