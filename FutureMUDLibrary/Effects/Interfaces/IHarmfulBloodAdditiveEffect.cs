using MudSharp.Form.Material;

namespace MudSharp.Effects.Interfaces {
    public interface IHarmfulBloodAdditiveEffect : IEffectSubtype {
        LiquidInjectionConsequence Consequence { get; }
        double Volume { get; set; }
    }
}