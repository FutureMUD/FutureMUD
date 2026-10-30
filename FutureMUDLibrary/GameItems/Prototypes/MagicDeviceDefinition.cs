#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using MudSharp.RPG.Checks;
namespace MudSharp.GameItems.Prototypes;

/// <summary>One pure XML writer for the native builder and reviewed blank stock devices.</summary>
public static class MagicDeviceDefinition
{
	public static string Serialize(MagicDeviceKind kind, MagicDeviceRole role, MagicDeviceEligibility eligibility,
		int capacity, int seconds, long capability, long usability, long checkTrait, int minimumGrade,
		Difficulty difficulty, Outcome outcome, IEnumerable<long> spells, XElement productionPlan,
		MagicDeviceMode mode = MagicDeviceMode.StoredCharges, long reservoirSpell = 0, long resource = 0,
		int maximumEnergy = 0, int initialEnergy = 0, bool allowsWornUse = false,
		MagicDeviceDelivery delivery = MagicDeviceDelivery.OtherCharacters,
		IEnumerable<KeyValuePair<int, int>>? gradeCosts = null, bool requiresOpenSky = false) =>
		new XElement("Definition", new XAttribute("version", 1),
			new XElement("Kind", (int)kind), new XElement("Role", (int)role), new XElement("Eligibility", (int)eligibility),
			new XElement("Capacity", capacity), new XElement("Seconds", seconds), new XElement("Capability", capability),
			new XElement("UseProg", usability), new XElement("CheckTrait", checkTrait), new XElement("MinimumUseGrade", minimumGrade),
			new XElement("Difficulty", (int)difficulty), new XElement("Outcome", (int)outcome),
			spells.Order().Select(x => new XElement("Spell", x)), productionPlan,
			new XElement("Reservoir", new XAttribute("mode", (int)mode), new XAttribute("spell", reservoirSpell),
				new XAttribute("resource", resource), new XAttribute("maximum", maximumEnergy),
				new XAttribute("initial", initialEnergy), new XAttribute("worn", allowsWornUse),
				new XAttribute("delivery", (int)delivery), new XAttribute("openSky", requiresOpenSky),
				(gradeCosts ?? []).OrderBy(x => x.Key).Select(x => new XElement("GradeCost",
					new XAttribute("grade", x.Key), new XAttribute("cost", x.Value))))).ToString();
	public static string ReviewedBlank(MagicDeviceKind kind, long mendFlesh) => Serialize(kind, MagicDeviceRole.Dual,
		MagicDeviceEligibility.Caster, kind == MagicDeviceKind.Staff ? 10 : 5, 60, 0, 0, 0, 0, Difficulty.Normal,
		Outcome.MinorPass, [mendFlesh], new XElement("Plan", new XElement("Phase")));
	public static string ReservoirState(int energy)
	{
		var root = new XElement("ChargedMagicDevice", new XAttribute("version", 1), new XAttribute("capability", 0),
			new XAttribute("trait", 0), new XAttribute("raw", 0d), new XAttribute("energy", energy));
		var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting))));
		root.Add(new XAttribute("checksum", checksum));
		return root.ToString(SaveOptions.DisableFormatting);
	}
}
