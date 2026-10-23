#nullable enable

using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string RenaissanceFinishedItemCraftKnowledge = "Renaissance Finished Item Crafting";

	internal sealed record RenaissanceCraftToolRequirementTestData(
		string Tag,
		string ProviderStableReference,
		string RequiredTagPath);

	private sealed record RenaissanceCraftPath(
		string Category,
		string Verb,
		string Action,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		double MaterialRetentionRatio,
		double MinimumMaterialGrams,
		string WorkKind,
		IReadOnlyList<string> Tools,
		string KnowledgeDescription);

	private static readonly IReadOnlyList<RenaissanceCraftToolRequirementTestData> RenaissanceCraftToolRequirements =
	[
		new("Hammer", "historic_workshop_hammer", "Functions / Tools / Striking Tools / Hammer"),
		new("Saw", "preindustrial_tool_hand_saw", "Functions / Tools / Woodcrafting Tools / Saws / Hand Saw"),
		new("Sewing Needle", "historic_sewing_needle", "Functions / Tools / Textilecraft Tools / Sewing Needle"),
		new("Shears", "historic_textile_shears", "Functions / Separation / Shearing / Shears"),
		new("Awl Punch", "historic_awl_punch", "Functions / Tools / Leatherworking Tools / Awl Punch"),
		new("Anvil", "historic_workshop_anvil", "Functions / Tools / Metalworking Tools / Anvil"),
		new("Forge Tongs", "historic_forge_tongs", "Functions / Tools / Metalworking Tools / Forge Tongs"),
		new("Hot Fire", "historic_lit_workshop_hearth", "Functions / Material Functions / Hot Fire")
	];

	internal static IReadOnlyList<RenaissanceCraftToolRequirementTestData> RenaissanceCraftToolRequirementsForTesting =>
		RenaissanceCraftToolRequirements;

	private static RenaissanceCraftPath Path(
		string category,
		string verb,
		string action,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		double materialRetentionRatio,
		double minimumMaterialGrams,
		string workKind,
		IReadOnlyList<string> tools,
		string knowledgeDescription)
	{
		return new RenaissanceCraftPath(category, verb, action, trait, minimumTraitValue, difficulty,
			materialRetentionRatio, minimumMaterialGrams, workKind, tools, knowledgeDescription);
	}
}
