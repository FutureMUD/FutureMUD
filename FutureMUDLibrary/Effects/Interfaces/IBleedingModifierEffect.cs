namespace MudSharp.Effects.Interfaces;

public interface IBleedingModifierEffect : IEffectSubtype
{
	double ExternalBleedingMultiplier { get; }
	double WoundReopenMultiplier { get; }
	double InternalBleedingMultiplier { get; }
}
