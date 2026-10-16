#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>Exact S3 stock drug binding for a cast toxin and its owned-dose cure.</summary>
public static class ArmageddonToxinSpellContent
{
	public const string VenomTouchKey = "arm.spell.venom_touch";
	public const string PurgeToxinKey = "arm.spell.purge_toxin";
	public const string OwnedFamily = "arm.effect.poison";
	public const string GramsFormula = "0.001*grade";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(long nativeDrugId)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(nativeDrugId, nameof(nativeDrugId));
		return Array.AsReadOnly(new[]
		{
			Contribution(VenomTouchKey, "Venom Touch", "60*grade", "poison", nativeDrugId,
				"Introduce 0.001 grams per selected grade of the package-owned contact toxin. Its native active and latent dose owns physiological clearance; the spell-origin identity survives reload and permits exact later purging."),
			Contribution(PurgeToxinKey, "Purge Toxin", "0", "removepoison", nativeDrugId,
				"Remove only the package-owned toxin family and its linked native active or latent doses, including residual doses after the parent spell ends. Mundane drugs and other spell families remain untouched.")
		});
	}

	private static ArmageddonCanonicalSpellContribution Contribution(string key, string name, string duration,
		string effectType, long nativeDrugId, string description) => new("arm.capability.water",
		new ArmageddonUtilitySpellContent(key, name, description, duration, 7,
			"$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter, 30, 7,
					new XElement("Effect", new XAttribute("type", effectType), new XElement("Drug", nativeDrugId),
						new XElement("Vector", (int)DrugVector.Touched),
						new XElement("Formula", new XCData(GramsFormula)), new XElement("OwnedFamily", OwnedFamily)));
			}));
}
