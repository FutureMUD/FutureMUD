using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static readonly string[] AntiquityRepairKitStableReferences =
	[
		"antiquity_textile_repair_kit",
		"antiquity_leather_repair_kit",
		"antiquity_wood_repair_kit",
		"antiquity_metal_repair_kit",
		"antiquity_stone_repair_kit",
		"antiquity_ceramic_repair_kit",
		"antiquity_hard_organic_repair_kit",
		"antiquity_field_repair_bundle"
	];

	private sealed record AntiquityRepairKitCraftSpec(
		string StableReference,
		string Name,
		string Category,
		string TraitName,
		string Blurb,
		string Action,
		int MinimumTraitValue,
		Difficulty Difficulty,
		IReadOnlyList<string> Inputs,
		IReadOnlyList<string> Tools);
}
