#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonMindSilenceSpellContent
{
	public const string Key = "arm.spell.silence_the_mind";
	public static ArmageddonCanonicalSpellContribution Create(long undead, long resistance)
	{
		if (undead <= 0 || resistance <= 0) throw new ArgumentException("Bind the exact native undead marker and resistance trait.");
		return new("arm.capability.void", new(Key, "Silence the Mind",
			"After native resistance, suppress activation of powers explicitly tagged psionic for 30*grade seconds. " +
			"The existing explicit IsPsionic classification contributes that tag; no power name or school is inferred. " +
			"Sever matching native outbound and reciprocal mind contacts. Undead, staff and duplicate overlapping suppression refuse. " +
			"Saved identity, physical instance, body and absolute expiry bound the effect. Ordinary commands and account controls are unaffected. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.", "30*grade", 20,
			"$0 impose|imposes a profound silence upon $1's mind.", (resource, cost, filter) =>
				ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 40, 20,
					new XElement("Effect", new XAttribute("type", "powertagsuppression"), new XAttribute("version", 1), new XElement("UndeadMerit", undead),
						new XElement("PowerTags", new XElement("Tag", "psionic")), new XElement("SecondsPerGrade", 30), new XElement("SeverMatchingMindContacts", true))),
			ResistingTraitId: resistance, ResistingDifficulty: Difficulty.Normal));
	}
}
