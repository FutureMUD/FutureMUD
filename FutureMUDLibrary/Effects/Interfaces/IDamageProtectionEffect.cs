#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Post-armour protection against one incoming body damage invocation, including its internal descendants.</summary>
public interface IDamageProtectionEffect : IEffectSubtype
{
	/// <summary>Pure finite multiplier in [0,1]. Exhausted protection returns 1.</summary>
	double DamageMultiplier { get; }
	/// <summary>Commits one qualifying positive-damage hit. Previews must never call this.</summary>
	bool TryCommitProtection();
}
