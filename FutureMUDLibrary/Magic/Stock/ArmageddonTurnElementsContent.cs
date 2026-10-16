#nullable enable
using System;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

/// <summary>Single-use reflection, with an explicit caller-resolved Fire school. No other schools are inferred.</summary>
public static class ArmageddonTurnElementsContent
{
	public static ArmageddonCanonicalSpellContribution Create(long fireSchoolId, string? eligibilitySource = null)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(fireSchoolId, nameof(fireSchoolId));
		if (eligibilitySource is not null && string.IsNullOrWhiteSpace(eligibilitySource)) throw new ArgumentException("Eligibility source is empty.");
		return new("arm.capability.void", new ArmageddonUtilitySpellContent("arm.spell.turn_the_elements", "Turn the Elements",
			"Reflect at most one eligible incoming attack of the explicitly selected Fire school to its source for 30 seconds per selected grade. Native recursion fencing prevents a second redirect.",
			"30*grade", 7, "$0 shape|shapes a ward around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				if (eligibilitySource is not null) ArmageddonPrimitiveSpellBindings.RequirePositive(filter, nameof(filter));
				return ArmageddonUtilitySpellContent.Definition("arm.spell.turn_the_elements", "character", resource, cost, filter, 40, 7,
					new XElement("Effect", new XAttribute("type", "personalward"), new XElement("School", fireSchoolId),
						new XElement("Mode", "Reflect"), new XElement("Coverage", "Incoming"), new XElement("IncludesSubschools", false),
						new XElement("Prog", 0L), new XElement("OneUse", true)));
			}, eligibilitySource, ProgVariableTypes.Character));
	}
}
