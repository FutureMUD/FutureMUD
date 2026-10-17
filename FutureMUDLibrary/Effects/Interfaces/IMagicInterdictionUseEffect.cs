#nullable enable
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

/// <summary>An opt-in bounded interdiction. Queries capture authority but never spend it.</summary>
public interface IMagicInterdictionUseEffect : IMagicInterdictionEffect
{
	bool UsesAreLimited { get; }
	IMagicInterdictionUse? CaptureUse(MagicInterdictionContext context);
}

public interface IMagicInterdictionUse
{
	bool IsCurrent { get; }
	/// <summary>False means stale authority before a write. Uncertainty after a write throws and must not be replayed or refunded.</summary>
	bool TryCommit();
}
