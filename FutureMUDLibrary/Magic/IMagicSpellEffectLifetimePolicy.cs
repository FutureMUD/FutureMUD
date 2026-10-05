#nullable enable

namespace MudSharp.Magic;

/// <summary>Optional recipient-local, quantised accumulation of one persistent effect.</summary>
public interface IMagicSpellEffectLifetimePolicy
{
	MagicSpellLifetimePolicy? LifetimePolicy { get; }
	string? LifetimePolicyError { get; }
}

public sealed record MagicSpellLifetimePolicy(string Group, int UnitSeconds, int MaximumUnits);

/// <summary>Retained source strength is independent of editable native power mappings.</summary>
public sealed record MagicSpellLifetimeState(MagicSpellLifetimePolicy Policy, int Grade);
