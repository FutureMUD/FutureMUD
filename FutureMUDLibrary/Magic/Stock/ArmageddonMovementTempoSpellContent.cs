#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic;

/// <summary>Locked S4 movement timing defaults; native movement still decides whether a step is legal.</summary>
public static class ArmageddonMovementTempoSpellContent
{
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(long ordinaryAgilityTraitId)
	{
		if (ordinaryAgilityTraitId <= 0) throw new ArgumentOutOfRangeException(nameof(ordinaryAgilityTraitId));
		return Array.AsReadOnly(new[]
	{
		Create("earth", "fleet_step", "Fleet Step", 10, false, true, ordinaryAgilityTraitId),
		Create("lightning", "lightning_pace", "Lightning Pace", 33, false, true, ordinaryAgilityTraitId),
		Create("lightning", "leadfoot", "Leadfoot", 33, true, false, ordinaryAgilityTraitId)
	});
	}

	private static ArmageddonCanonicalSpellContribution Create(string school, string suffix, string name,
		double minimum, bool slow, bool selfOnly, long ordinaryAgilityTraitId)
	{
		var key = "arm.spell." + suffix;
		var description = slow
			? "Increase the recipient's ordinary native movement time by 1+0.05*external grade for 60 seconds per grade."
			: "Reduce ordinary native movement time by 1/(1+0.05*external grade) for 60 seconds per grade.";
		description += suffix == "fleet_step" ? " Reduce stamina expenditure to 95% while active." :
			$" Apply an ordinary agility modifier of {(slow ? "-grade" : "+grade")}.";
		var content = new ArmageddonUtilitySpellContent(key, name,
			description + " Native posture, terrain, injury, exits and other movement gates still apply. " +
			"The direction and per-grade rate are editable spell defaults.",
			"60*grade", minimum, "$0 shape|shapes a movement enchantment around $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (selfOnly && filter <= 0) throw new ArgumentOutOfRangeException(nameof(filter), "Self-only movement stock requires its exact eligibility prog.");
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
					30, minimum, new XElement("Effect", new XAttribute("type", "movementtempo"),
						new XElement("Slow", slow), new XElement("RatePerGrade", "0.05")));
				if (suffix == "fleet_step")
					definition.Element("Effects")!.Add(new XElement("Effect", new XAttribute("type", "staminaexpendrate"),
						new XElement("Multiplier", "0.95")));
				else
				{
					definition.Element("Effects")!.Add(new XElement("Effect", new XAttribute("type", "boost"),
						new XAttribute("trait", ordinaryAgilityTraitId), new XAttribute("bonus", 0),
						new XAttribute("context", (int)TraitBonusContext.None)));
					definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding",
						new XAttribute("list", "target"), new XAttribute("index", 1), new XAttribute("effect", "boost"),
						new XAttribute("field", "Bonus"), slow ? "-grade" : "grade"));
				}
				return definition;
			}, selfOnly ? "return @target == @caster" : null);
		return new("arm.capability." + school, content);
	}
}
