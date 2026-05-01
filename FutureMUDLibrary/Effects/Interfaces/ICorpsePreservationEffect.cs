#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface ICorpsePreservationEffect : IEffectSubtype
{
	bool PreserveCorpse { get; }
}
