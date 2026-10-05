#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;
namespace MudSharp.GameItems.Prototypes;

/// <summary>One pure XML writer for the native builder and reviewed blank stock devices.</summary>
public static class MagicDeviceDefinition
{
	public static string Serialize(MagicDeviceKind kind, MagicDeviceRole role, MagicDeviceEligibility eligibility,
		int capacity, int seconds, long capability, long usability, long checkTrait, int minimumGrade,
		Difficulty difficulty, Outcome outcome, IEnumerable<long> spells, XElement productionPlan) =>
		new XElement("Definition", new XAttribute("version", 1),
			new XElement("Kind", (int)kind), new XElement("Role", (int)role), new XElement("Eligibility", (int)eligibility),
			new XElement("Capacity", capacity), new XElement("Seconds", seconds), new XElement("Capability", capability),
			new XElement("UseProg", usability), new XElement("CheckTrait", checkTrait), new XElement("MinimumUseGrade", minimumGrade),
			new XElement("Difficulty", (int)difficulty), new XElement("Outcome", (int)outcome),
			spells.Order().Select(x => new XElement("Spell", x)), productionPlan).ToString();
	public static string ReviewedBlank(MagicDeviceKind kind, long mendFlesh) => Serialize(kind, MagicDeviceRole.Dual,
		MagicDeviceEligibility.Caster, kind == MagicDeviceKind.Staff ? 10 : 5, 60, 0, 0, 0, 0, Difficulty.Normal,
		Outcome.MinorPass, [mendFlesh], new XElement("Plan", new XElement("Phase")));
}
