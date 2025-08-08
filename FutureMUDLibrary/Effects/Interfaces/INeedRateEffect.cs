namespace MudSharp.Effects.Interfaces;

public interface INeedRateEffect : IEffectSubtype
{
    double HungerMultiplier { get; }
    double ThirstMultiplier { get; }
    double DrunkennessMultiplier { get; }
    bool AppliesToPassive { get; }
    bool AppliesToActive { get; }
}
