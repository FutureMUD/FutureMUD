namespace MudSharp.Effects.Interfaces;

public interface IPrioritisedOverrideDescEffect : IEffectSubtype
{
	int OverridePriority { get; }
	string OverrideKey { get; }
}
