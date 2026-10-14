#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Fixed S4 item-output spells. The installer supplies exact owned, current native prototypes.</summary>
public static class ArmageddonBoundConsumableSpellContent
{
	public const string EmberSeedKey = "arm.spell.ember_seed";
	public const string RestorativeMudKey = "arm.spell.restorative_mud";

	public static ArmageddonUtilitySpellContent EmberSeed(long prototypeId)
	{
		if (prototypeId <= 0) throw new ArgumentOutOfRangeException(nameof(prototypeId));
		return new(EmberSeedKey, "Ember Seed",
			"Creates one temporary, spell-owned Ember Seed for an eligible character. Its exact paid grade, creator, configured grade-bound burning payload and 300*grade-second deadline are bound to the item before exposure. Application commits its one-use state before harming eligible local targets; expiry and removal use the native owned-item lifecycle. Creation grants no remote detonation or unapproved carrier delivery.",
			"0", 20, "$0 shape|shapes a smouldering seed for $1.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(EmberSeedKey, "character", resource, cost, filter, 30, 20,
				Item(prototypeId, "ember-seed", "TemporaryCleanup", "300*grade")));
	}

	public static ArmageddonUtilitySpellContent RestorativeMud(long prototypeId)
	{
		if (prototypeId <= 0) throw new ArgumentOutOfRangeException(nameof(prototypeId));
		return new(RestorativeMudKey, "Restorative Mud",
			"Creates one permanent, spell-owned one-gram Restorative Mud dose at a selected local item anchor. The item is not transformed; a compatible container may receive the dose. Its exact paid grade and creator are bound to the topical component before exposure. Applying the full dose once to an accessible wounded bodypart spends at most 10*grade native wound units, committing consumption before healing. No recurring refill or unqualified substance delivery is supplied.",
			"0", 7, "$0 shape|shapes restorative mud near $1.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(RestorativeMudKey, "item", resource, cost, filter, 30, 7,
				Item(prototypeId, "restorative-mud", "Permanent", null)));
	}

	private static XElement Item(long prototypeId, string family, string mode, string? lifetime) =>
		new("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"),
			new XElement("ItemPrototypeId", prototypeId), new XElement("ItemSkinId", 0),
			new XElement("Quantity", 1), new XElement("LoadString", ""),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", mode),
				new XElement("Family", family), lifetime is null ? null : new XElement("Seconds", lifetime)));
}
