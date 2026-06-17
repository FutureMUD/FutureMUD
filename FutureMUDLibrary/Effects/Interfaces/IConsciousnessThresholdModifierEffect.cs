namespace MudSharp.Effects.Interfaces;

public interface IConsciousnessThresholdModifierEffect : IEffectSubtype
{
	double PainPassOutThresholdMultiplier { get; }
	double StunUnconsciousThresholdMultiplier { get; }
	double AnesthesiaUnconsciousThresholdMultiplier { get; }
}
