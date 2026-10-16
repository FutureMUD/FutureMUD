#nullable enable

using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonReturnTetherSpellContent
{
	public const string Key = "arm.spell.return_tether";
	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.earth", new(Key, "Return Tether",
		"Record your exact physical room and layer, then attempt one admitted return after 30*grade seconds. " +
		"The durable absolute deadline never resets on loading. Death, loss of the recorded body, logout, dispel, invalid endpoints and intervening wards cancel the return. " +
		"A return attempt is durably claimed before native movement; uncertain attempts never replay. Health, inventory and world state are not rewound. " +
		"Only named Say casting is authored; quiet, area and carriers remain disabled.", "30*grade", 7,
		"$0 bind|binds a thread of earth to &0's present position.", (resource, cost, filter) =>
			ArmageddonUtilitySpellContent.Definition(Key, "self", resource, cost, filter, 40, 7,
				new XElement("Effect", new XAttribute("type", "returntether"), new XAttribute("version", 1), new XElement("SecondsPerGrade", 30)))));
}
