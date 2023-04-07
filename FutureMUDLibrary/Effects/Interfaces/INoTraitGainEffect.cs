using MudSharp.Body.Traits;

namespace MudSharp.Effects.Interfaces {
    public interface INoTraitGainEffect : IEffectSubtype {
        ITraitDefinition Trait { get; }
    }
}