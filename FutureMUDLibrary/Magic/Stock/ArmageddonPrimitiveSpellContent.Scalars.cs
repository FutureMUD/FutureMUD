#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic;

public static partial class ArmageddonPrimitiveSpellContent
{
	/// <summary>Locked scalar contributions requiring the native roomlight and healingrate typed scalar adapters.</summary>
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> CreateScalarContributions() =>
		Array.AsReadOnly(new[]
		{
			Scalar("fire", "daylight", "Daylight", "room", 4, 30, "60*grade",
				"Increase room illumination by twenty lux per selected grade for 60 seconds per grade.",
				RoomLight(10), "roomlight", "AddedLuxPerPower", "20*grade/power"),
			Scalar("shadow", "deep_darkness", "Deep Darkness", "room", 7, 30, "60*grade",
				"Reduce room illumination by twenty lux per selected grade for 60 seconds per grade, respecting native lower bounds.",
				RoomLight(-10), "roomlight", "AddedLuxPerPower", "-20*grade/power"),
			Scalar("lightning", "quickened_healing", "Quickened Healing", "character", 7, 40, "120*grade",
				"Increase natural healing rates by a multiplier of one plus one tenth per selected grade for 120 seconds per grade. No wound is immediately healed.",
				new XElement("Effect", new XAttribute("type", "healingrate"), new XElement("Multiplier", 1.1),
					new XElement("Stages", 0)), "healingrate", "Multiplier", "1+0.1*grade")
		});

	private static XElement RoomLight(double coefficient) => new("Effect", new XAttribute("type", "roomlight"),
		new XElement("AddedLuxPerPower", coefficient), new XElement("DescAddendum", new XCData("")),
		new XElement("GlowAddendumColour", "bold white"));

	private static ArmageddonCanonicalSpellContribution Scalar(string school, string suffix, string name,
		string trigger, double minimum, int opening, string duration, string description, XElement effect,
		string effectType, string field, string formula)
	{
		var key = "arm.spell." + suffix;
		return new("arm.capability." + school, new ArmageddonUtilitySpellContent(key, name, description,
			duration, minimum, "$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				if (trigger == "room" && filter != 0)
					throw new ArgumentException("The native room trigger does not support an eligibility filter.", nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, trigger, resource, cost, filter,
					opening, minimum, new XElement(effect));
				if (trigger == "room") definition.Element("Trigger")!.Element("TargetFilterProg")!.Remove();
				definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding",
					new XAttribute("list", "target"), new XAttribute("index", 0), new XAttribute("effect", effectType),
					new XAttribute("field", field), new XCData(formula)));
				return definition;
			}));
	}
}
