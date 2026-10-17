#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Pure authored S4 contributions. Callers must select native resistance and target policy.</summary>
public static class ArmageddonVitalTransferSpellContent
{
	public static ArmageddonUtilitySpellContent VitalSiphon(long resistingTraitId, string eligibilitySource,
		DamageType damageType = DamageType.Arcane) => Siphon("arm.spell.vital_siphon", "Vital Siphon", 7,
		resistingTraitId, eligibilitySource, damageType);

	public static ArmageddonUtilitySpellContent DevouringTouch(long resistingTraitId, string eligibilitySource,
		DamageType damageType = DamageType.Arcane) => Siphon("arm.spell.devouring_touch", "Devouring Touch", 10,
		resistingTraitId, eligibilitySource, damageType);

	private static ArmageddonUtilitySpellContent Siphon(string key, string name, double minimum,
		long resistingTraitId, string eligibilitySource, DamageType damageType)
	{
		if (resistingTraitId <= 0) throw new ArgumentOutOfRangeException(nameof(resistingTraitId));
		if (string.IsNullOrWhiteSpace(eligibilitySource)) throw new ArgumentException("Author an explicit target/caster eligibility prog.", nameof(eligibilitySource));
		if (!Enum.IsDefined(damageType)) throw new ArgumentOutOfRangeException(nameof(damageType));
		return new(key, name,
			"Inflict 5*external grade native damage and heal the caster by at most the actual delivered damage and 5*grade. " +
			"The shared linked adapter measures new or aggravated wounds on the exact target body after native armour; item wounds, pain, stun and friendly wounds earn no healing. " +
			"Healing reduces existing Mend-eligible wounds, worst first, without overflow beyond the finite budget. " +
			$"The editable authored native damage choice is {damageType}; it is not a recovered historical damage classification. " +
			"The selected native resistance trait and editable target/caster eligibility prog supply world policy; no species, undead or dragon cosmology is inferred.",
			"0", minimum, "$0 draw|draws vitality from $1.",
			(resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter, 40, minimum,
					new XElement("Effect", new XAttribute("type", "vitalsiphon"),
						new XElement("DamageType", (int)damageType), new XElement("DamagePerGrade", 5),
						new XElement("HealingCapPerGrade", 5)));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				return definition;
			}, eligibilitySource, ProgVariableTypes.Character, resistingTraitId, Difficulty.Normal);
	}

	public static ArmageddonUtilitySpellContent LeechingEdge() => new("arm.spell.leeching_edge", "Leeching Edge",
		"Enchant a native melee weapon for 60*external grade seconds. Its current exact wielder heals at most min(0.1*grade,0.5) of actual eligible weapon damage. " +
		"Melee-only is the authored native scope; builders can edit the transfer fractions and duration. Thrown and ranged payload delivery are not included. " +
		"Misses, fully blocked or absorbed hits, armour and other item wounds, pain, stun and friendly wounds earn nothing. " +
		"Overlapping enchantments select the strongest single fraction per hit. The ordinary spell parent owns persistence, replacement, expiry and dispel.",
		"60*grade", 25, "$0 draw|draws a hungry enchantment along $1.",
		(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition("arm.spell.leeching_edge", "item", resource, cost, filter, 40, 25,
			new XElement("Effect", new XAttribute("type", "leechingedge"),
				new XElement("FractionPerGrade", 0.1), new XElement("MaximumFraction", 0.5))),
		TargetType: ProgVariableTypes.Item);
}
