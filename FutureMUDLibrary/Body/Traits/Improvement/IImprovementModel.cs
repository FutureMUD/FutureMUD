using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement {
    public interface IImprovementModel : IFrameworkItem {
        double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome, TraitUseType usetype);
        bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType, bool ignoreTemporaryBlockers);
    }
}