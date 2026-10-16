#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Resolved S3 native infection identity and an existing character-scoped resistance trait.</summary>
public sealed record ArmageddonConditionSpellBindings(long DiseaseKind, long ResistanceTraitId)
{
	internal void Validate()
	{
		if (DiseaseKind != (long)InfectionType.SownSickness)
			throw new ArgumentOutOfRangeException(nameof(DiseaseKind), "The S3 owned disease must resolve to native SownSickness.");
		ArmageddonPrimitiveSpellBindings.RequirePositive(ResistanceTraitId, nameof(ResistanceTraitId));
	}
}

public static class ArmageddonConditionSpellContent
{
	/// <summary>Provides the disease pair; installed-world qualification remains separate. Toxin origin persistence and retained detection need separate adapters.</summary>
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(ArmageddonConditionSpellBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		bindings.Validate();
		return Array.AsReadOnly(new[]
		{
			Contribution("water", "sow_sickness", "Sow Sickness", 8, 30, "60*grade", "disease", bindings,
				"Introduce one native nonspreading Sown Sickness infection through declared contact exposure, at selected grade intensity for 60 seconds per grade. Native Normal virulence and the supplied character resistance trait govern susceptibility.", true),
			Contribution("water", "purge_disease", "Purge Disease", 7, 30, "0", "removedisease", bindings,
				"Remove matching spell-owned Sown Sickness and its exact tracked native infection. Other infections, wounds and drugs remain unchanged.", false)
		});
	}

	private static ArmageddonCanonicalSpellContribution Contribution(string school, string suffix, string name,
		double floor, int opening, string duration, string type, ArmageddonConditionSpellBindings bindings,
		string description, bool resisted)
	{
		var key = "arm.spell." + suffix;
		return new("arm.capability." + school, new ArmageddonUtilitySpellContent(key, name, description, duration,
			floor, "$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter, opening, floor,
					new XElement("Effect", new XAttribute("type", type), new XElement("InfectionType", bindings.DiseaseKind),
						new XElement("VirulenceDifficulty", (int)Difficulty.Normal), new XElement("IntensityFormula", new XCData("grade")),
						new XElement("VirulenceFormula", new XCData("1")), new XElement("OwnedFamily", "arm.effect.disease"),
						new XElement("ExposureVector", (int)DrugVector.Touched)));
			}, ResistingTraitId: resisted ? bindings.ResistanceTraitId : null,
			ResistingDifficulty: resisted ? Difficulty.Normal : null));
	}
}
