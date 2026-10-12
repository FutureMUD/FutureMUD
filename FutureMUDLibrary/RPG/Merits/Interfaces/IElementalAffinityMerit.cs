#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Merits;

/// <summary>Explicit descriptive affinities, independent of learned capabilities and prog applicability.</summary>
public interface IElementalAffinityMerit : ICharacterMerit
{
	IReadOnlyList<string> ElementalAffinities { get; }
}

public static class ElementalAffinityExtensions
{
	/// <summary>Reads assigned metadata only; never runs progs or infers affiliations from other traits.</summary>
	public static IReadOnlyList<string> DeclaredElementalAffinities(this IHaveMerits owner) => owner.Merits
		.OfType<IElementalAffinityMerit>()
		.SelectMany(x => x.ElementalAffinities)
		.Distinct(System.StringComparer.OrdinalIgnoreCase)
		.OrderBy(x => x, System.StringComparer.OrdinalIgnoreCase)
		.ToArray();
}
