using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface ITelepathyEffect : IEffectSubtype {
        bool ShowThinks { get; }
        bool ShowFeels { get; }

        bool ShowDescription(ICharacter thinker);
        bool ShowName(ICharacter thinker);
        bool ShowThinkEmote(ICharacter thinker);
    }
}