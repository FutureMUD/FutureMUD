#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>Selected-grade native needs changes with ordinary body and damage accounting.</summary>
public static class ArmageddonNeedHarmSpellContent
{
	public const string DesiccateKey = "arm.spell.desiccate";
	public const string HeadlongRevelKey = "arm.spell.headlong_revel";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create() => Array.AsReadOnly(new[]
	{
		Desiccate(), HeadlongRevel()
	});

	private static ArmageddonCanonicalSpellContribution Desiccate() => new("arm.capability.fire",
		new ArmageddonUtilitySpellContent(DesiccateKey, "Desiccate",
			"Reduce one visible local character's native drink satiation by at most five points per external grade, " +
			"then inflict two native fire damage per grade. The native needs model bounds hydration; body, armour " +
			"and prosthetics remain in the fire damage path. Both editable magnitudes use the selected grade.",
			"0", 7, "$0 parch|parches $1 with dry heat.", (resource, cost, filter) =>
			{
				Validate(resource, cost, filter);
				var definition = ArmageddonUtilitySpellContent.Definition(DesiccateKey, "character", resource, cost, filter,
					10, 7, NeedDelta());
				definition.Element("Effects")!.Add(new XElement("Effect", new XAttribute("type", "damage"),
					new XElement("DamageType", (int)DamageType.Burning), new XElement("DamageExpression", "2*grade"),
					new XElement("PainExpression", "0"), new XElement("StunExpression", "0"),
					new XElement("Bodypart", 0), new XElement("Limb", -1),
					new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1))));
				definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(Binding("ThirstDelta", "-5*grade"));
				return definition;
			}));

	private static ArmageddonCanonicalSpellContribution HeadlongRevel() => new("arm.capability.water",
		new ArmageddonUtilitySpellContent(HeadlongRevelKey, "Headlong Revel",
			"Introduce at most 0.01 litres of alcohol per selected external grade through the recipient's native needs and delayed exposure accounting. " +
			"No drink object is created; the editable alcohol magnitude remains within the native need limit.",
			"0", 7, "$0 evoke|evokes a headlong revel in $1.", (resource, cost, filter) =>
			{
				Validate(resource, cost, filter);
				var definition = ArmageddonUtilitySpellContent.Definition(HeadlongRevelKey, "character", resource, cost, filter,
					60, 7, NeedDelta());
				definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(Binding("DrunkDelta", "0.01*grade"));
				return definition;
			}));

	private static XElement NeedDelta() => new("Effect", new XAttribute("type", "needdelta"),
		new XElement("Hunger", 0), new XElement("Thirst", 0), new XElement("Drunk", 0),
		new XElement("ReportObservedChange", true));

	private static XElement Binding(string field, string formula) => new("Binding",
		new XAttribute("list", "target"), new XAttribute("index", 0),
		new XAttribute("effect", "needdelta"), new XAttribute("field", field), formula);

	private static void Validate(long resource, long cost, long filter)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
		ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
		if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
	}
}
