#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Water's bounded, resistible native breathing interruption.</summary>
public static class ArmageddonDrowningGripSpellContent
{
	public const string Key = "arm.spell.drowning_grip";

	public static ArmageddonCanonicalSpellContribution Create(long resistanceTraitId)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(resistanceTraitId, nameof(resistanceTraitId));
		var content = new ArmageddonUtilitySpellContent(Key, "Drowning Grip",
			"Interrupt an eligible local character's native breathing for at most 5*external grade seconds. " +
			"The selected native Water resistance trait opposes the cast. The target's current body must need and be able to breathe before payment; " +
			"native breath holding and hypoxia resolve normally. This is an authored FutureMUD adaptation of the source entry without a bound function. " +
			"Expiry and dispel remove the spell-owned interruption; no direct damage or component is supplied.",
			"5*grade", 8, "$0 closes|close a grip of water around $1.",
			(resource, cost, filter) =>
			{
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "character", resource, cost, filter,
					30, 8, new XElement("Effect", new XAttribute("type", "drowninggrip")));
				definition.Element("Trigger")!.SetElementValue("CanTargetSelf", false);
				return definition;
			}, ResistingTraitId: resistanceTraitId, ResistingDifficulty: Difficulty.Normal);
		return new("arm.capability.water", content);
	}
}
