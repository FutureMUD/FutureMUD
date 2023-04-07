using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ISawHiderEffect : IEffectSubtype {
        IPerceivable Hider { get; }
    }
}