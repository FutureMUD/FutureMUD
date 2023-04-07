using MudSharp.Body.Traits;

namespace MudSharp.Effects.Interfaces {
    public interface ITraitBonusEffect : IEffectSubtype {
        bool AppliesToTrait(ITraitDefinition trait);
        bool AppliesToTrait(ITrait trait);
        double GetBonus(ITrait trait, TraitBonusContext context = TraitBonusContext.None);
    }
}
