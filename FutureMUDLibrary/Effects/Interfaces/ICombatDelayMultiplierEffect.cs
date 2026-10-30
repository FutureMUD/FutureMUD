#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Provides a multiplier for a character's native combat recovery delay.</summary>
public interface ICombatDelayMultiplierEffect : IEffectSubtype
{
	double Multiplier { get; }
}
