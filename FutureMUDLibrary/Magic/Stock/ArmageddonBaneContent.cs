#nullable enable

using System;
using System.Globalization;
using System.Xml.Linq;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonBaneContent
{
	public const string Key = "arm.spell.apex_bane";
	public static ArmageddonUtilitySpellContent Create(long resistanceTrait, Difficulty resistanceDifficulty,
		double damagePerGrade, double maximumDamage, DamageType damageType)
	{
		if (resistanceTrait <= 0 || !Enum.IsDefined(resistanceDifficulty) || !Enum.IsDefined(damageType) ||
			!double.IsFinite(damagePerGrade) || damagePerGrade <= 0 || !double.IsFinite(damagePerGrade * 7) ||
			!double.IsFinite(maximumDamage) || maximumDamage <= 0)
			throw new ArgumentException("Bind native resistance and damage with positive finite per-grade and maximum amounts.");
		var formula = $"min({maximumDamage.ToString("R", CultureInfo.InvariantCulture)},{damagePerGrade.ToString("R", CultureInfo.InvariantCulture)}*grade)";
		return new(Key, "Apex Bane", "A resisted native attack against an explicitly authored magical-creature eligibility prog. " +
			"Ineligible targets are refused before payment. Warded or resisting eligible targets remain normal paid failures. " +
			"Native damage is capped by the authored maximum and scales with selected grade; native armour and health determine actual wounds.",
			"0", 12, "$0 direct|directs a baneful enchantment at $1.", (resource, cost, filter) =>
				ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter, 30, 12,
					new XElement("Effect", new XAttribute("type", "damage"), new XElement("DamageType", (int)damageType),
						new XElement("DamageExpression", new XCData(formula)), new XElement("Bodypart", 0), new XElement("Limb", -1))),
			ResistingTraitId: resistanceTrait, ResistingDifficulty: resistanceDifficulty);
	}
}
