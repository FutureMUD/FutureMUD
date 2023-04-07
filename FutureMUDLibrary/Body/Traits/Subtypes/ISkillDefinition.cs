using MudSharp.Body.Traits.Improvement;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes {
    public interface ISkillDefinition : ITraitDefinition {
        Difficulty TeachDifficulty { get; }
        Difficulty LearnDifficulty { get; }
        bool CanTeach(ICharacter character);
        bool CanLearn(ICharacter character);
        string Show(ICharacter voyeur);
        IImprovementModel Improver { get; }
        IFutureProg AvailabilityProg { get; }
        IFutureProg TeachableProg { get; }
        IFutureProg LearnableProg { get; }
        ITraitExpression Cap { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
    }
}