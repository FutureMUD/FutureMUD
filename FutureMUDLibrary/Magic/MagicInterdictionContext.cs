using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic;

#nullable enable

public sealed record MagicInterdictionTag(string Tag, string Value);

public sealed class MagicInterdictionContext
{
	public MagicInterdictionContext(ICharacter source, IPerceivable? target, IMagicSchool school,
		IReadOnlyCollection<MagicInterdictionTag> tags, IReadOnlyCollection<SpellAdditionalParameter> additionalParameters)
	{
		Source = source;
		Target = target;
		School = school;
		Tags = tags;
		AdditionalParameters = additionalParameters;
	}

	public ICharacter Source { get; }
	public IPerceivable? Target { get; }
	public IMagicSchool School { get; }
	public IReadOnlyCollection<MagicInterdictionTag> Tags { get; }
	public IReadOnlyCollection<SpellAdditionalParameter> AdditionalParameters { get; }
}

public interface IMagicInterdictionTagProvider
{
	IEnumerable<MagicInterdictionTag> MagicInterdictionTags { get; }
}
