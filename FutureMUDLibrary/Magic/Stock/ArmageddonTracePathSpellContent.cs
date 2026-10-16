#nullable enable

using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonTracePathSpellContent
{
	public const string Key = "arm.spell.trace_the_path";
	public static ArmageddonCanonicalSpellContribution Create() => new("arm.capability.earth", new(Key, "Trace the Path",
		"Reveal a permitted native route of at most 10*grade edges toward one visible, personally recognised traveller. " +
		"The target requires an unobscured verified identity dub; legacy unknown, concealed, offline, body-only and ambiguous dubs cannot authorise it. " +
		"Doors, hidden exits and normal movement restrictions remain in force. Only named Say casting is authored; quiet, area and carriers remain disabled.",
		"0", 6, "$0 listen|listens for a path through the earth.", (resource, cost, filter) =>
			ArmageddonUtilitySpellContent.Definition(Key, "recognisedcharacter", resource, cost, filter, 40, 6,
				new XElement("Effect", new XAttribute("type", "tracepath"), new XAttribute("version", 1), new XElement("EdgesPerGrade", 10)))));
}
