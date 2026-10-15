#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static class ArmageddonStormLocusSpellContent
{
	public const string Key = "arm.spell.storm_locus";
	public static ArmageddonCanonicalSpellContribution Create(long water, long empty, long defiler)
	{
		if (water <= 0 || empty <= 0 || defiler <= 0) throw new ArgumentException("Bind exact Water/Void planes and the Defiler merit.");
		return new("arm.capability.lightning", new(Key, "Storm Locus",
			"Maintain one outdoor room hazard for 30*grade seconds. Each due 10-second pulse inflicts 2*grade native Electrical damage on distinct eligible physical primary occupants, including the caster. " +
			"Staff, ethereal bodies and incoming wards are protected. Water refuses; Void requires the exact Defiler merit. Duplicate loci refuse. " +
			"A durable cursor commits before injury. Reload skips missed pulses, offline creators never load lazily, and pulse damage grants no training. " +
			"Only named Say casting is authored; quiet, area and carriers remain disabled.", "30*grade", 20,
			"$0 call|calls a crackling electrical storm into being.", (resource, cost, filter) =>
				ArmageddonUtilitySpellContent.Definition(Key, "room", resource, cost, filter, 40, 20,
					new XElement("Effect", new XAttribute("type", "stormlocus"), new XAttribute("version", 1), new XElement("WaterPlane", water),
						new XElement("VoidPlane", empty), new XElement("DefilerMerit", defiler)))));
	}
}
