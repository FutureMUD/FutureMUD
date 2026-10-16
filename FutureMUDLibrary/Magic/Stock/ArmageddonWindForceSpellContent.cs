#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Explicit-exit wind movement; the native effect checks each crossing before relocation.</summary>
public static class ArmageddonWindForceSpellContent
{
	public const string GustHandsKey = "arm.spell.gust_hands";
	public const string DrivingGustKey = "arm.spell.driving_gust";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create() => Array.AsReadOnly(new[]
	{
		Create(GustHandsKey, "Gust Hands", true),
		Create(DrivingGustKey, "Driving Gust", false)
	});

	private static ArmageddonCanonicalSpellContribution Create(string key, string name, bool gradedPath)
	{
		var description = gradedPath
			? "Force a visible local character along the selected exit direction for at most the selected external grade in admitted steps."
			: "Force a visible local character one admitted step through the explicitly selected exit.";
		var content = new ArmageddonUtilitySpellContent(key, name,
			description + " Each native crossing checks the current exit, layer and movement gates; a failed step never relocates the target. " +
			"Fall exits are excluded. The path limit and eligibility are builder-editable defaults.",
			"0", 7, "$0 summon|summons a driving gust against $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(key, "characterexit", resource, cost, filter,
					gradedPath ? 10 : 42, 7, new XElement("Effect", new XAttribute("type", "forcedpathmovement"),
						new XElement("Steps", 1), new XElement("AllowFallExits", false)));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				if (gradedPath)
					definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding",
						new XAttribute("list", "target"), new XAttribute("index", 0),
						new XAttribute("effect", "forcedpathmovement"), new XAttribute("field", "Steps"), "grade"));
				return definition;
			});
		return new("arm.capability.wind", content);
	}
}
