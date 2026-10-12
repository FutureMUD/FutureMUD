#nullable enable
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic;

public sealed record ArmageddonCanonicalSpellContribution(string NativeCapabilityKey, ArmageddonUtilitySpellContent Content);

/// <summary>Approved canonical S4 contributions, shared by installation and runtime qualification.</summary>
public static class ArmageddonStockSpellContent
{
	public const string ReadAttunementKey = "arm.spell.read_attunement";
	public static ArmageddonUtilitySpellContent ReadAttunement() => new(ReadAttunementKey, "Read Attunement",
		"Shows explicit assigned elemental affinities and legitimately learned magical affiliations as separate facts. " +
		"Only the caster can read the selected visible character's overlay; names, species and temporary grants do not imply affinity or learning.",
		"60*grade", 20, "$0 study|studies $1 as $0 shape|shapes a revealing enchantment.",
		(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(ReadAttunementKey, "character",
			resource, cost, filter, 40, 20, new XElement("Effect", new XAttribute("type", "readattunement"))));

	// Presence here means a real definition can be installed. It is not a gameplay qualification or admission list.
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Contributions =>
		[new("arm.capability.water", ReadAttunement())];
}
