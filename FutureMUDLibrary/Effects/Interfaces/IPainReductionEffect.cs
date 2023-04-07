namespace MudSharp.Effects.Interfaces {
    public interface IPainReductionEffect : IEffectSubtype {
        double PainReductionMultiplier { get; }
        double FlatPainReductionAmount { get; }
    }
}