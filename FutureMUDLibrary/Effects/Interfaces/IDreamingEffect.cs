using MudSharp.RPG.Dreams;

namespace MudSharp.Effects.Interfaces {
    public interface IDreamingEffect : IEffectSubtype {
        IDream Dream { get; }
    }
}