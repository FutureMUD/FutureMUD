using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Effects.Interfaces {
    public interface ISecondWindExhaustedEffect : IEffectSubtype {
        ISecondWindMerit Merit { get; }
    }
}
