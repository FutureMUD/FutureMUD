using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement {
    public interface IImprovementModel : IEditableItem {
        double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome, TraitUseType usetype);
        bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType, bool ignoreTemporaryBlockers);
        string ImproverType { get; }
        IImprovementModel Clone(string name);
    }
}