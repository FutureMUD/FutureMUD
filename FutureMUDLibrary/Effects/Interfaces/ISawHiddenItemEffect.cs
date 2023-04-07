using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ISawHiddenItemEffect : IEffectSubtype {
        IPerceivable Item { get; }
    }
}