#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonCompellingRegardSpellContent
{
	public const string Key = "arm.spell.compelling_regard";
	public static ArmageddonCanonicalSpellContribution Create(long undead, long resistance)
	{
		if (undead <= 0 || resistance <= 0) throw new ArgumentException("Bind native undead and resistance identities.");
		return new("arm.capability.void", new(Key, "Compelling Regard",
			"Resistible PC/NPC influence for 30*grade seconds, with a per-spell PC toggle. Exact synchronous IC commands use compel; " +
			"normal native permission/state/delay admission and a final exact-command check apply. No aliases, abbreviations, nested, queued, staff, account or OOC commands. " +
			"Undead, staff and already influenced targets refuse. Exact controller and target body/instance bindings expire or cancel on logout, death or body change. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.", "30*grade", 33,
			"$0 hold|holds $1's attention with a compelling gaze.", (resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 40, 33,
				new XElement("Effect", new XAttribute("type", "compellingregard"), new XAttribute("version", 1), new XElement("UndeadMerit", undead), new XElement("AllowPcs", true))),
			ResistingTraitId: resistance, ResistingDifficulty: Difficulty.Normal));
	}
}
