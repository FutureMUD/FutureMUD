using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    public interface IPertainToBodypartEffect : IEffectSubtype {
        IBodypart Bodypart { get; }
    }
}
