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

	internal static bool ShouldSeedRenaissanceFinishedItemCraftsForTesting(string? eras)
	{
		return !string.IsNullOrWhiteSpace(eras) && HasAnyEra(eras, "renaissance");
	}

	private bool ShouldSeedRenaissanceFinishedItemCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       ShouldSeedRenaissanceFinishedItemCraftsForTesting(eras);
	}

	private void SeedRenaissanceFinishedItemCrafts()
	{
		if (!ShouldSeedRenaissanceFinishedItemCrafts())
		{
			return;
		}

		ValidateRenaissanceFinishedCraftPrerequisites();
		var usedCraftNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		foreach (var item in _items
		         .Where(x => IsRenaissanceFinishedItemCraftReference(x.Key))
		         .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
		{
			var material = GetMaterialName(item.Value);
			var path = GetRenaissanceCraftPath(item.Key, material);
			var materialAmount = Math.Max(path.MinimumMaterialGrams, item.Value.Weight * path.MaterialRetentionRatio)
				.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
			var displayName = item.Value.ShortDescription;
			var craftName = BuildUniqueVisibleCraftName(usedCraftNames, path.Verb,
				StripLeadingArticle(displayName));

			AddCraft(
				craftName,
				path.Category,
				$"{path.Verb} {displayName}",
				path.Action,
				$"an in-progress {StripLeadingArticle(displayName)} craft",
				RenaissanceFinishedItemCraftKnowledge,
				path.Trait,
				path.MinimumTraitValue,
				path.Difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				RenaissanceFinishedItemCraftingPhases(path.WorkKind, item.Key),
				[$"Commodity - {materialAmount} grams of {material}"],
				path.Tools,
				[$"SimpleProduct - 1x {displayName} (#{item.Value.Id})"],
				[],
				knowledgeSubtype: path.Category,
				knowledgeDescription: path.KnowledgeDescription,
				knowledgeLongDescription: path.KnowledgeDescription);
		}
	}

	private static bool IsRenaissanceFinishedItemCraftReference(string stableReference)
	{
		return stableReference.StartsWith("renaissance_", StringComparison.OrdinalIgnoreCase) &&
		       !stableReference.StartsWith("renaissance_jewellery_", StringComparison.OrdinalIgnoreCase) &&
		       !stableReference.StartsWith("renaissance_door_", StringComparison.OrdinalIgnoreCase);
	}

	private void ValidateRenaissanceFinishedCraftPrerequisites()
	{
		if (_manifestCaptureOnly)
		{
			return;
		}

		var issues = new List<string>();
		foreach (var requirement in RenaissanceCraftToolRequirements)
		{
			if (!_items.TryGetValue(requirement.ProviderStableReference, out var provider))
			{
				issues.Add($"Missing shared tool item {requirement.ProviderStableReference} for the {requirement.Tag} craft tag.");
				continue;
			}

			var matchingTagIds = _tagsByFullPath
				.Where(x => x.Key.Equals(requirement.RequiredTagPath, StringComparison.OrdinalIgnoreCase))
				.Select(x => x.Value.Id)
				.ToHashSet();
			var providerTagIds = provider.GameItemProtosTags
				.Select(x => x.TagId)
				.Concat(_context!.GameItemProtosTags
					.Where(x => x.GameItemProtoId == provider.Id && x.GameItemProtoRevisionNumber == provider.RevisionNumber)
					.Select(x => x.TagId))
				.ToHashSet();
			if (matchingTagIds.Count == 0 || !providerTagIds.Overlaps(matchingTagIds))
			{
				issues.Add($"Shared tool item {requirement.ProviderStableReference} does not provide {requirement.RequiredTagPath}.");
			}
		}

		var missingMaterials = _items
			.Where(x => IsRenaissanceFinishedItemCraftReference(x.Key))
			.Select(x => GetMaterialName(x.Value))
			.Where(x => !_materials.ContainsKey(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		issues.AddRange(missingMaterials.Select(x =>
			$"Missing primary-material commodity source for Renaissance craft material {x}."));

		if (issues.Count > 0)
		{
			throw new InvalidOperationException(
				$"Renaissance finished-item craft prerequisites are incomplete:{Environment.NewLine} - {string.Join(Environment.NewLine + " - ", issues)}");
		}
	}

	private static RenaissanceCraftPath GetRenaissanceCraftPath(string stableReference, string material)
	{
		if (stableReference.Contains("_food_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_drink_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_wine_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_ale_", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Food and drink", "prepare", "preparing a food or drink item", "Cooking", 10,
				Difficulty.Easy, 0.75, 20.0, "cooking",
				["TagTool - InRoom - an item with the Hot Fire tag"],
				"This knowledge covers Renaissance kitchen, bakehouse, brewery, and market-food finishing from primary and processed food commodities.");
		}

		if (stableReference.Contains("_medical_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_drug_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_repair_", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Medical and repair", "prepare", "preparing a medical or repair item", "Apothecary", 15,
				Difficulty.Normal, 0.70, 15.0, "apothecary",
				["TagTool - Held - an item with the Shears tag", "TagTool - Held - an item with the Awl Punch tag"],
				"This knowledge covers Renaissance medical preparation and specialist repair work without adding unseeded treatment mechanics.");
		}

		if (stableReference.Contains("_writing_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_document_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_book_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_print_", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Writing and administration", "bind", "binding a writing or administrative item", "Bookbinding", 15,
				Difficulty.Normal, 0.65, 10.0, "bookbinding",
				["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Sewing Needle tag"],
				"This knowledge covers Renaissance writing, book, document, and administrative forms from prepared material commodities.");
		}

		if (stableReference.Contains("_agriculture_", StringComparison.OrdinalIgnoreCase) ||
		    stableReference.Contains("_commodity_", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Agricultural preparation", "prepare", "preparing an agricultural or commodity item", "Labouring", 10,
				Difficulty.Easy, 0.80, 20.0, "agriculture",
				["TagTool - Held - an item with the Shears tag"],
				"This knowledge covers Renaissance agricultural handling and commodity preparation from agriculture, butchery, and primary-production materials.");
		}

		if (IsSoftMaterial(material))
		{
			return Path("Textile and leather work", "tailor", "tailoring a textile or leather item",
				material.Equals("leather", StringComparison.OrdinalIgnoreCase) ? "Leathermaking" : "Tailoring", 15,
				Difficulty.Normal, 0.75, 12.0, material.Equals("leather", StringComparison.OrdinalIgnoreCase) ? "leather" : "tailoring",
				material.Equals("leather", StringComparison.OrdinalIgnoreCase)
					? ["TagTool - Held - an item with the Awl Punch tag", "TagTool - Held - an item with the Shears tag"]
					: ["TagTool - Held - an item with the Sewing Needle tag", "TagTool - Held - an item with the Shears tag"],
				"This knowledge covers Renaissance textile and leather goods from agricultural fibres and prepared hides.");
		}

		if (IsMetalMaterial(material))
		{
			return Path("Metalwork", "forge", "forging a metal item", "Blacksmithing", 15,
				Difficulty.Hard, 0.82, 25.0, "metalwork",
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
					"TagTool - Held - an item with the Forge Tongs tag"
				],
				"This knowledge covers Renaissance metalwork from smelted and primary-production metal commodities.");
		}

		if (material.Equals("glass", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("faience", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("earthenware", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("porcelain", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Ceramic and glass work", "form", "forming a ceramic or glass item", "Glassworking", 15,
				Difficulty.Normal, 0.78, 20.0, "kilnwork",
				["TagTool - InRoom - an item with the Hot Fire tag", "TagTool - Held - an item with the Hammer tag"],
				"This knowledge covers Renaissance kiln and glass work from primary clay, mineral, and glass commodities.");
		}

		if (material.Equals("wood", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("oak", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("ash", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("beech", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("cedar", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("teak", StringComparison.OrdinalIgnoreCase) ||
		    material.Equals("bamboo", StringComparison.OrdinalIgnoreCase))
		{
			return Path("Carpentry and joinery", "build", "building a wooden item", "Carpentry", 15,
				Difficulty.Normal, 0.80, 50.0, "joinery",
				["TagTool - Held - an item with the Hammer tag", "TagTool - Held - an item with the Saw tag"],
				"This knowledge covers Renaissance carpentry and joinery from forestry and primary-production timber commodities.");
		}

		return Path("General craftwork", "make", "making a finished item", "Crafting", 15,
			Difficulty.Normal, 0.75, 10.0, "general",
			["TagTool - Held - an item with the Hammer tag"],
			"This knowledge covers Renaissance finished goods whose configured material is supplied as a primary commodity or earlier craft stock.");
	}

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

	private static bool IsSoftMaterial(string material)
	{
		return material.Equals("leather", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("wool", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("linen", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("silk", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("cotton", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("hemp", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("felt", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("raffia cloth", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("ramie cloth", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("barkcloth", StringComparison.OrdinalIgnoreCase);
	}

	private static (int Seconds, string Echo, string FailEcho)[] RenaissanceFinishedItemCraftingPhases(
		string workKind,
		string stableReference)
	{
		(int Seconds, string Echo, string FailEcho)[] phases = workKind switch
		{
			"cooking" =>
			[
				(25, "$0 sort|sorts and prepare|prepares $i1 beside $t1.", "$0 sort|sorts and prepare|prepares $i1 beside $t1."),
				(40, "$0 tend|tends the heat and work|works the prepared material into its final form.", "$0 tend|tends the heat and work|works the prepared material into its final form."),
				(25, "$0 portion|portions and set|sets aside $p1 for serving or storage.", "$0 find|finds the batch spoiled and set|sets the material aside.")
			],
			"apothecary" =>
			[
				(25, "$0 inspect|inspects $i1 and trim|trims it with $t1.", "$0 inspect|inspects $i1 and trim|trims it with $t1."),
				(35, "$0 measure|measures the prepared material and work|works it with $t2.", "$0 measure|measures the prepared material and work|works it with $t2."),
				(25, "$0 pack|packs the finished preparation and set|sets aside $p1.", "$0 discard|discards the spoiled preparation and set|sets the material aside.")
			],
			"bookbinding" =>
			[
				(25, "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1.", "$0 arrange|arranges $i1 and pierce|pierces the working points with $t1."),
				(40, "$0 draw|draws $t2 through the prepared work, checking its alignment.", "$0 draw|draws $t2 through the prepared work, checking its alignment."),
				(25, "$0 trim|trims the finished form and set|sets aside $p1.", "$0 find|finds the binding has slipped and set|sets the material aside.")
			],
			"agriculture" =>
			[
				(20, "$0 sort|sorts $i1 and cut|cuts away unsuitable material with $t1.", "$0 sort|sorts $i1 and cut|cuts away unsuitable material with $t1."),
				(35, "$0 grade|grades the prepared material for its intended use.", "$0 grade|grades the prepared material for its intended use."),
				(20, "$0 bundle|bundles the finished stock and set|sets aside $p1.", "$0 reject|rejects the spoiled stock and set|sets the material aside.")
			],
			"tailoring" =>
			[
				(25, "$0 lay|lays out $i1 and mark|marks the working lines with $t1.", "$0 lay|lays out $i1 and mark|marks the working lines with $t1."),
				(45, "$0 cut|cuts and join|joins the shaped material with $t1 and $t2.", "$0 cut|cuts and join|joins the shaped material with $t1 and $t2."),
				(25, "$0 finish|finishes the seams and set|sets aside $p1.", "$0 find|finds the seams have failed and set|sets the material aside.")
			],
			"leather" =>
			[
				(25, "$0 lay|lays out $i1 and open|opens the stitch points with $t1.", "$0 lay|lays out $i1 and open|opens the stitch points with $t1."),
				(40, "$0 cut|cuts the leather to shape with $t2 and work|works its edges true.", "$0 cut|cuts the leather to shape with $t2 and work|works its edges true."),
				(25, "$0 burnish|burnishes the finished leatherwork and set|sets aside $p1.", "$0 split|splits the leatherwork and set|sets the material aside.")
			],
			"metalwork" =>
			[
				(30, "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3.", "$0 heat|heats $i1 at $t1 and lift|lifts it with $t3."),
				(45, "$0 strike|strikes the hot material against $t1 with $t2.", "$0 strike|strikes the hot material against $t1 with $t2."),
				(30, "$0 cool|cools the finished work and set|sets aside $p1 for inspection.", "$0 find|finds the work twisted and set|sets the material aside.")
			],
			"kilnwork" =>
			[
				(25, "$0 shape|shapes $i1 and warm|warms it at $t1.", "$0 shape|shapes $i1 and warm|warms it at $t1."),
				(40, "$0 turn|turns the work carefully, tapping the form true with $t2.", "$0 turn|turns the work carefully, tapping the form true with $t2."),
				(25, "$0 anneal|anneals the finished form and set|sets aside $p1.", "$0 find|finds the form has cracked and set|sets the material aside.")
			],
			"joinery" =>
			[
				(30, "$0 mark|marks $i1 and cut|cuts the fitted pieces with $t2.", "$0 mark|marks $i1 and cut|cuts the fitted pieces with $t2."),
				(45, "$0 fit|fits the joins and secure|secures them with $t1.", "$0 fit|fits the joins and secure|secures them with $t1."),
				(30, "$0 smooth|smooths the finished piece and set|sets aside $p1.", "$0 find|finds the join has opened and set|sets the material aside.")
			],
			_ =>
			[
				(25, "$0 examine|examines $i1 and set|sets up the work with $t1.", "$0 examine|examines $i1 and set|sets up the work with $t1."),
				(40, "$0 shape|shapes the prepared material, checking the form as it develops.", "$0 shape|shapes the prepared material, checking the form as it develops."),
				(25, "$0 finish|finishes the work and set|sets aside $p1.", "$0 find|finds the work unsound and set|sets the material aside.")
			]
		};

		return ApplyRenaissanceWorkbeatVariation(phases, stableReference);
	}

	private static (int Seconds, string Echo, string FailEcho)[] ApplyRenaissanceWorkbeatVariation(
		(int Seconds, string Echo, string FailEcho)[] phases,
		string stableReference)
	{
		var workbeats = new[]
		{
			"$0 pause|pauses to check the proportions before carrying on.",
			"$0 turn|turns the work over to judge its balance.",
			"$0 measure|measures the developing form against the intended finish.",
			"$0 clear|clears the work area before the final steps."
		};
		var workbeatIndex = 0;
		foreach (var character in stableReference)
		{
			workbeatIndex = (workbeatIndex * 31 + character) % workbeats.Length;
		}

		var workPhase = phases[1];
		var variedEcho = $"{workPhase.Echo} {workbeats[workbeatIndex]}";
		phases[1] = (workPhase.Seconds, variedEcho, variedEcho);
		return phases;
	}
}
