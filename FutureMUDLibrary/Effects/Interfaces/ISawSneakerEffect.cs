using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ISawSneakerEffect : IEffectSubtype {
        IPerceivable Sneaker { get; }
        bool Success { get; }
    }
}