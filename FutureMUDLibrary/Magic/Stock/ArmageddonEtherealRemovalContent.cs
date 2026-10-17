#nullable enable
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonEtherealRemovalContent
{
	public const string Key = "arm.spell.draw_into_flesh";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.shadow",
		new ArmageddonUtilitySpellContent(Key, "Draw into Flesh",
			"Remove one eligible spell-owned ethereal planar overlay from a locally selected character. " +
			"Ordinary planar state and unrelated spell effects remain. A target without such an overlay has no state change. " +
			"Only ordinary named Say casting is authored; quiet and carriers remain disabled.",
			"0", 7, "$0 draw|draws $1 back into flesh.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter,
				40, 7, new XElement("Effect", new XAttribute("type", "removeplanarstate"),
					new XElement("OnlySpellOwnedEthereal", true)))));
}
