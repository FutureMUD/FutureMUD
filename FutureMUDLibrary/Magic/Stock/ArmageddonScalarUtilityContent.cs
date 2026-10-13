#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

/// <summary>Pure scalar content; installed-world qualification remains separate.</summary>
public static class ArmageddonScalarUtilityContent
{
	/// <param name="itemTargetEligibilitySource">Optional caller-authored additional item/caster policy. The native weapon predicate is always required.</param>
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(string? itemTargetEligibilitySource = null)
	{
		var light = new XElement("Effect", new XAttribute("type", "roomlight"),
			new XElement("AddedLuxPerPower", -10.0), new XElement("DescAddendum", new XCData("")),
			new XElement("GlowAddendumColour", "Bold White"));
		var temperature = new XElement("Effect", new XAttribute("type", "roomtemperature"),
			new XElement("TemperatureDeltaPerPower", -0.5), new XElement("DescAddendum", new XCData("")),
			new XElement("AddendumColour", "Bold Blue"));
		var enchantment = new XElement("Effect", new XAttribute("type", "itemenchant"),
			new XElement("SDescAddendum", new XCData("")), new XElement("DescAddendum", new XCData("")),
			new XElement("Colour", "Bold Red"), new XElement("AttackCheckBonus", 0.0));
		return Array.AsReadOnly(new[]
		{
			Build("shadow", "cooling_shade", "Cooling Shade", "room", 20, 60,
				"Reduce room illumination by ten lux and ambient temperature by half a degree Celsius per selected grade for 60 seconds per grade, respecting native bounds.",
				[light, temperature], [Binding(0, "roomlight", "AddedLuxPerPower", "-10*grade/power"),
					Binding(1, "roomtemperature", "TemperatureDeltaPerPower", "-0.5*grade/power")], null),
			Build("fire", "kindle_enchantment", "Kindle Enchantment", "item", 33, 40,
				"Improve a weapon's attack checks by the selected grade for 60 seconds per grade. Base quality, damage, pain and stun are unchanged; identical spell parents are exclusive.",
				[enchantment], [Binding(0, "itemenchant", "AttackCheckBonus", "grade")],
				itemTargetEligibilitySource is null ? "return @target.isweapon" :
					"if (not(@target.isweapon))\nreturn false\nend if\n" + itemTargetEligibilitySource)
		});
	}

	private static XElement Binding(int index, string token, string field, string formula) => new("Binding",
		new XAttribute("list", "target"), new XAttribute("index", index), new XAttribute("effect", token),
		new XAttribute("field", field), new XCData(formula));

	private static ArmageddonCanonicalSpellContribution Build(string school, string suffix, string name,
		string trigger, double minimum, int opening, string description, XElement[] effects, XElement[] bindings,
		string? eligibility)
	{
		var key = "arm.spell." + suffix;
		return new("arm.capability." + school, new ArmageddonUtilitySpellContent(key, name, description,
			"60*grade", minimum, "$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (trigger == "item") ArmageddonPrimitiveSpellBindings.RequirePositive(filter, nameof(filter));
				else if (filter != 0) throw new ArgumentException("Room targets do not accept an eligibility prog.", nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, trigger, resource, cost, filter,
					opening, minimum, new XElement(effects[0]));
				for (var i = 1; i < effects.Length; i++) definition.Element("Effects")!.Add(new XElement(effects[i]));
				if (trigger == "room") definition.Element("Trigger")!.Element("TargetFilterProg")!.Remove();
				foreach (var binding in bindings) definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement(binding));
				return definition;
			}, eligibility, trigger == "item" ? ProgVariableTypes.Item : null));
	}
}
