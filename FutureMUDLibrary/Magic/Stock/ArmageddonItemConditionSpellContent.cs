#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Health;

namespace MudSharp.Magic;

/// <summary>Authored finite item damage and repair definitions; target eligibility remains native and editable.</summary>
public static class ArmageddonItemConditionSpellContent
{
	public const string ShatterStoneKey = "arm.spell.shatter_stone";
	public const string RestoreObjectKey = "arm.spell.restore_object";
	public const string ErodeObjectKey = "arm.spell.erode_object";
	public const string WitherGrowthKey = "arm.spell.wither_growth";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create() => Array.AsReadOnly(new[]
	{
		Create(ShatterStoneKey, "Shatter Stone", "earth", "damage", 6, DamageType.Crushing,
			(int)MaterialBehaviourType.Stone, false,
			"Damage one local stone-behaviour item for 5*external grade native condition damage."),
		Create(RestoreObjectKey, "Restore Object", "earth", "repair", 7, DamageType.Crushing,
			-1, false,
			"Repair up to 5*external grade native damage on an eligible local item without changing material, wear or quality."),
		Create(ErodeObjectKey, "Erode Object", "void", "damage", 7, DamageType.Chemical,
			-1, true,
			"Damage one local organic-material item for 5*external grade native condition damage."),
		Create(WitherGrowthKey, "Wither Growth", "water", "damage", 8, DamageType.Chemical,
			(int)MaterialBehaviourType.Plant, true,
			"Damage one local plant-behaviour item for 5*external grade native condition damage. This item-only definition does not debit living land or crops, or transfer sap.")
	});

	private static ArmageddonCanonicalSpellContribution Create(string key, string name, string school, string mode,
		double minimum, DamageType damageType, int materialBehaviour, bool organicOnly, string description)
	{
		var content = new ArmageddonUtilitySpellContent(key, name,
			description + " The editable material gate and exact selected-grade budget are checked before payment. " +
			"Damage refuses items containing other items and items too fragile for the full budget; it does not recursively damage contents. " +
			"Repair reduces existing repairable wounds only; it does not create matter or improve item quality. " +
			"Only ordinary named Say casting is authored; quiet and carriers are not admitted by this definition.",
			"0", minimum, "$0 shape|shapes " + name.ToLowerInvariant() + " upon $1.",
			(resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(key, "item", resource, cost, filter, 40, minimum,
					new XElement("Effect", new XAttribute("type", "boundeditemcondition"),
						new XElement("Mode", mode), new XElement("BudgetPerGrade", 5),
						new XElement("DamageType", (int)damageType),
						new XElement("MaterialBehaviour", materialBehaviour), new XElement("OrganicOnly", organicOnly)));
			});
		return new("arm.capability." + school, content);
	}
}
