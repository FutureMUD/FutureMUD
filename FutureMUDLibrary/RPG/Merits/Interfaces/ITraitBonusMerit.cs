using MudSharp.Body.Traits;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface ITraitBonusMerit : ICharacterMerit {
        double BonusForTrait(ITraitDefinition trait, TraitBonusContext context);
    }
}