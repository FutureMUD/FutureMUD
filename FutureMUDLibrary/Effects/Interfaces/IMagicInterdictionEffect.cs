using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMagicInterdictionEffect : IEffectSubtype
{
	IMagicSchool School { get; }
	MagicInterdictionCoverage Coverage { get; }
	MagicInterdictionMode Mode { get; }
	bool IncludesSubschools { get; }
	bool ShouldInterdict(ICharacter source, IMagicSchool school);
}
