#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Fire ingestion curse with a binary timed effect and editable bounded per-sip harm.</summary>
public static class ArmageddonBurningDraughtSpellContent
{
	public const string Key = "arm.spell.burning_draught_curse";

	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.fire",
		new ArmageddonUtilitySpellContent(Key, "Burning Draught Curse",
			"For 60 seconds per selected grade, an accepted drink burns the exact living target instead of refreshing it. " +
			"The default is five native Burning damage per sip, editable from 0.1 to 100.",
			"60*grade", 7, "$0 lays a fiery curse upon $1's next draughts.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 60, 7,
					new XElement("Effect", new XAttribute("type", "burningdraught"),
						new XAttribute("schema", 1), new XElement("DamagePerSip", 5.0)));
			}));
}
