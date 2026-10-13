#nullable enable
using System;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

/// <summary>Pure stock contribution. Native armour/material profiles and the ordinary agility trait
/// are resolved by the caller; no identity is inferred from an engine ID or setting-specific name.</summary>
public static class ArmageddonArmourSpellContent
{
	public static ArmageddonUtilitySpellContent Create(string key, long armourTypeId, long solidMaterialId,
		long ordinaryAgilityTraitId)
	{
		if (armourTypeId <= 0 || solidMaterialId <= 0) throw new ArgumentOutOfRangeException(nameof(armourTypeId));
		var (name, minimum, budget, description) = key switch
		{
			"arm.spell.flame_mantle" => ("Flame Mantle", 7, 10, "A warm glow surrounds $0."),
			"arm.spell.sheltering_veil" => ("Sheltering Veil", 7, 10, "A protective veil surrounds $0."),
			"arm.spell.unyielding_veil" => ("Unyielding Veil", 7, 20, "A dense protective veil surrounds $0."),
			"arm.spell.mist_shield" => ("Mist Shield", 20, 10, "Protective mist surrounds $0."),
			"arm.spell.earthen_ward" => ("Earthen Ward", 7, 10, "$0 is|are surrounded by an earthen ward."),
			"arm.spell.stone_skin" => ("Stone Skin", 7, 10, "$0 has|have stone-like skin."),
			"arm.spell.wind_mantle" => ("Wind Mantle", 7, 10, "A protective mantle of wind surrounds $0."),
			"arm.spell.wind_shield" => ("Wind Shield", 20, 10, "A shield of wind surrounds $0."),
			"arm.spell.shadow_mantle" => ("Shadow Mantle", 7, 10, "Protective shadows surround $0."),
			"arm.spell.charged_aegis" => ("Charged Aegis", 7, 10, "A charged protective aegis surrounds $0."),
			_ => throw new ArgumentException("Unknown stock armour spell.", nameof(key))
		};
		var stone = key == "arm.spell.stone_skin";
		if (stone && ordinaryAgilityTraitId <= 0) throw new ArgumentOutOfRangeException(nameof(ordinaryAgilityTraitId));
		return new(key, name,
			$"Apply {budget}*external grade finite native protection for 60*grade seconds. Native armour type and solid material are editable authored package choices; their configured damage channels determine protection. " +
			(stone ? "Apply an ordinary agility penalty of -grade through the same spell parent. " : "") +
			"The native parent owns expiry, dispel and saved remaining life. Spent protection never refills.",
			"60*grade", minimum, "$0 surround|surrounds $1 with magical protection.", (resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter, 40, minimum,
					new XElement("Effect", new XAttribute("type", "spellarmour"), new XElement("FiniteBudget", true),
						new XElement("ArmourAppliesProg", filter), new XElement("ArmourType", armourTypeId),
						new XElement("ArmourMaterial", solidMaterialId), new XElement("Quality", (int)ItemQuality.Standard),
						new XElement("MaximumDamageAbsorbed", $"{budget}*grade"), new XElement("FullDescriptionAddendum", description),
						new XElement("CanBeObscuredByInventory", false)));
				if (stone)
				{
					definition.Element("Effects")!.Add(new XElement("Effect", new XAttribute("type", "boost"),
						new XAttribute("trait", ordinaryAgilityTraitId), new XAttribute("bonus", 0), new XAttribute("context", (int)TraitBonusContext.None)));
					definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding",
						new XAttribute("list", "target"), new XAttribute("index", 1), new XAttribute("effect", "boost"),
						new XAttribute("field", "Bonus"), "-grade"));
				}
				return definition;
			}, EligibilitySource: "return true", TargetType: ProgVariableTypes.Character);
	}
}
