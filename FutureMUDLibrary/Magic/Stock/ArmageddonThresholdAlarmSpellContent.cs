#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonThresholdAlarmSpellContent
{
	public const string Key = "arm.spell.threshold_alarm";

	public static ArmageddonCanonicalSpellContribution Create()
	{
		var content = new ArmageddonUtilitySpellContent(Key, "Threshold Alarm",
			"Mark the caster's current room for 120*external grade seconds. Each eligible native entry event sends " +
			"one generic notice to the loaded caster without revealing the entrant's identity. An offline caster receives no queued notice. " +
			"Only ordinary named Say casting is authored; quiet and carriers remain disabled.",
			"120*grade", 33, "$0 shapes|shape a quiet warning at the threshold.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter != 0) throw new ArgumentOutOfRangeException(nameof(filter), "The room alarm has no target filter.");
				return ArmageddonUtilitySpellContent.Definition(Key, "room", resource, cost, 0,
					40, 33, new XElement("Effect", new XAttribute("type", "thresholdalarm")));
			});
		return new("arm.capability.earth", content);
	}
}
