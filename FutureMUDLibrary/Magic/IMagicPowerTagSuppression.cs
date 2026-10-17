#nullable enable

using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Effects;

namespace MudSharp.Magic;

/// <summary>Content may suppress activation of an explicit set of native power tags.</summary>
public interface IMagicPowerTagSuppression : IEffectSubtype
{
	IReadOnlyCollection<string> SuppressedPowerTags { get; }
	bool IsActiveFor(ICharacter actor);
}
