namespace MudSharp.Effects.Interfaces;

public interface IStaminaRegenerationRateEffect : IEffectSubtype
{
    double Multiplier { get; }
}
