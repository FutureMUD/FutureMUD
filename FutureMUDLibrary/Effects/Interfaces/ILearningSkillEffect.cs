using MudSharp.Body.Traits;

namespace MudSharp.Effects.Interfaces {
    public interface ILearningSkillEffect : IEffectSubtype {
        ITraitDefinition Trait { get; }
        int TimesAttempted { get; }
        int TimesSucceeded { get; }
        int SuccessesRequired { get; }
        bool LastResultWasSuccess { get; }
    }
}