using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMagicContextualInterdictionEffect : IEffectSubtype
{
	bool ShouldInterdict(MagicInterdictionContext context);
}
