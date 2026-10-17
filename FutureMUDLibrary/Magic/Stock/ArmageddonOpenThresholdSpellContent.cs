#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonOpenThresholdSpellContent
{
	public const string Key = "arm.spell.open_threshold";
	public static ArmageddonCanonicalSpellContribution Create(long restricted, long lethal)
	{
		if (restricted <= 0 || lethal <= 0) throw new ArgumentException("Bind native restricted and lethal room tags.");
		return new("arm.capability.void", new(Key, "Open Threshold",
			"Open one native two-endpoint transient threshold for 120*grade seconds. Select an exact carried owned Arcane Mark or personally recognised visible traveller. " +
			"Arcane Mark use commits once before creation; uncertain commit is held for review and never replayed. Permanent same-plane endpoints, selected native layer and live wards apply. " +
			"Restricted, lethal and temporary locations refuse. Expiry or dispelling unregisters the exit; existing occupants and goods remain in their real rooms. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.", "120*grade", 33,
			"$0 trace|traces a shimmering threshold through the air.", (resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "thresholddestination", resource, cost, filter, 40, 33,
				new XElement("Effect", new XAttribute("type", "openthreshold"), new XAttribute("version", 1), new XElement("RestrictedTag", restricted), new XElement("LethalTag", lethal)))));
	}
}
