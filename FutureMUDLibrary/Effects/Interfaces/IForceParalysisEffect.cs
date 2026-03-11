#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface IForceParalysisEffect : IEffectSubtype
{
	bool ShouldParalyse { get; }
}
