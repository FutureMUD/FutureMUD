using MudSharp.RPG.Knowledge;

namespace MudSharp.Effects.Interfaces {
    public interface ILearningKnowledgeEffect : IEffectSubtype {
        IKnowledge Knowledge { get; }
        int TimesAttempted { get; }
        int TimesSucceeded { get; }
        int SuccessesRequired { get; }
        bool LastResultWasSuccess { get; }
    }
}