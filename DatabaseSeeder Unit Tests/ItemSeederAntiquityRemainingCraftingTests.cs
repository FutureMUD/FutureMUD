#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityRemainingCraftingTests
{
	private static readonly string[] AntiquityReworkMethods =
	[
		"SeedAntiquityClothing",
		"SeedAntiquityJewellery",
		"SeedAntiquityArmour",
		"SeedAntiquityContainers",
		"SeedAntiquityDoorsAndLocks",
		"SeedAntiquityRepairKits",
		"SeedAntiquityHouseholdFurniture",
		"SeedAntiquityWeaponsShieldsAccessories"
	];

	private static readonly string[] HouseholdCraftFunctionalRoots =
	[
		"Functions / Household Items",
		"Functions / Writing Goods"
	];

	private static readonly string[] CraftToolFunctionalRoots =
	[
		"Functions / Tools",
		"Functions / Separation",
		"Functions / Joining",
		"Functions / Sharpening"
	];

	[TestMethod]
	public void AntiquityRemainingCrafts_SourceAuditCoversAllCurrentTargets()
	{
		var items = AntiquityReworkMethods.SelectMany(Recipes.OriginalGroup).ToArray();
		Assert.AreEqual(1046, items.Length);
		Recipes.AssertNativeCraftCoverage(items.Select(item => item.Metadata.StableKey),
			"antiquity_bronze_brazier", "antiquity_earthenware_brazier", "antiquity_iron_fire_basket",
			"antiquity_accessory_kushite_tall_arrow_quiver");
	}

	[TestMethod]
	public void AntiquityRepairKitCrafts_RegisterGeneralMaterialCoverage()
	{
		var items = Recipes.OriginalGroup("SeedAntiquityRepairKits"); Assert.AreEqual(8, items.Length);
		Recipes.ItemsAreCrafted(items.Select(item => item.Metadata.StableKey));
		foreach (var item in items) Assert.IsTrue(item.Components.Any(component => component.StartsWith("Repair_", StringComparison.Ordinal)), item.Metadata.StableKey);
		foreach (var expected in new[] { ("antiquity_textile_repair_kit", "Repair_Cloth", "linen"), ("antiquity_leather_repair_kit", "Repair_Leather", "deer leather"), ("antiquity_wood_repair_kit", "Repair_Wood", "oak"), ("antiquity_metal_repair_kit", "Repair_Metal", "bronze"), ("antiquity_stone_repair_kit", "Repair_Stone", "limestone"), ("antiquity_ceramic_repair_kit", "Repair_Ceramic", "earthenware"), ("antiquity_hard_organic_repair_kit", "Repair_Hard_Organic", "bone"), ("antiquity_field_repair_bundle", "Repair_Universal", "leather") })
		{ var item = Recipes.Item(expected.Item1); CollectionAssert.Contains(item.Components.ToArray(), expected.Item2); Assert.AreEqual(expected.Item3, item.Material); }
	}

	[TestMethod]
	public void AntiquityJewelleryCrafts_RegisterDynamicKnowledgeGatedSuiteAndCatalogue()
	{
		var items = Recipes.OriginalGroup("SeedAntiquityJewellery"); Assert.AreEqual(162, items.Length);
		Recipes.ItemsAreCrafted(items.Select(item => item.Metadata.StableKey)); Recipes.Knowledge("Ancient Jewellery Crafting");
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_RegisterKnowledgeGatedSuitesAndCorrectAccessProgs()
	{
		foreach (var knowledge in new[] { "Ancient Equipment Crafting", "Ancient Weaponcrafting", "Ancient Armourcrafting", "Ancient Toolmaking", "Ancient Common Clothing Crafting" }) Recipes.Knowledge(knowledge);
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_AddUpstreamCommodityAndTagSurface()
	{
		Recipes.RequiresTags(ExpectedEquipmentStockTags().ToArray());
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_MakeSupportToolsAndUnlitApparatusCraftable()
	{
		var keys = new[] { "antiquity_workshop_hearth", "antiquity_clay_smelting_furnace", "antiquity_updraft_kiln", "antiquity_glory_hole_furnace", "antiquity_annealing_lehr", "antiquity_bronze_textile_shears", "antiquity_bronze_sewing_needle", "antiquity_bronze_awl_punch" };
		Recipes.ItemsAreCrafted(keys);
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_KeepVisibleCraftStringsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.Crafting.AntiquityEquipment.cs");

		AssertContains(craftSource, "SanitiseAntiquityEquipmentVisibleName(item.ShortDescription)");
		AssertContains(craftSource, "BuildUniqueVisibleCraftName");
		Assert.IsFalse(craftSource.Contains("stableReference[\"antiquity_\"", StringComparison.Ordinal),
			"Visible equipment craft names should not be generated from stable references because those may contain culture names.");

		var visibleConstructionRegion = ExtractMethodBody(craftSource, "SeedAntiquityEquipmentFinishedCraft");
		foreach (var banned in new[] { "roman", "hellenic", "egyptian", "celtic", "germanic", "kushite", "punic", "persian", "etruscan", "anatolian", "scythian", "sarmatian" })
		{
			Assert.IsFalse(visibleConstructionRegion.Contains(banned, StringComparison.OrdinalIgnoreCase),
				$"Visible equipment craft construction should not contain culture term {banned}.");
		}
	}

	[TestMethod]
	public void AntiquityCrafting_AllCurrentTagToolsHaveSeededItemCoverage()
	{
		var tools = Recipes.Recipes.Where(recipe => recipe.Metadata.EraAdmissions.Contains("antiquity"))
			.SelectMany(recipe => recipe.Tools.OfType<CatalogueTagRecipeTool>()).Select(tool => tool.Tag).Distinct().ToArray();
		foreach (var tool in tools) Assert.IsTrue(Recipes.Items.Any(item => item.Tags.Any(tag => tag == tool || tag.EndsWith(" / " + tool, StringComparison.OrdinalIgnoreCase))), tool);
	}

	[TestMethod]
	public void ItemSeederCrafts_RequestedSkillNamesResolveAgainstStockSkillPackage()
	{
		var skillSource = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder", "SkillPackageSeeder.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.Crafting.cs");
		var craftSource = ReadSeederSources("ItemSeeder.Crafting*.cs");

		var supportedSkillNames = ExtractSkillPackageNames(skillSource);
		supportedSkillNames.UnionWith(ExtractStockSkillPackageTraitAliases(craftRoot));

		var requestedSkillNames = ExtractItemCraftSkillRequests(craftSource);
		var unresolved = requestedSkillNames
			.Where(x => !supportedSkillNames.Contains(x))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, unresolved.Count,
			$"Every item-seeder craft skill request should resolve to a stock Skill Package skill or supported stock alias. Missing: {string.Join(", ", unresolved)}");
		Assert.IsFalse(requestedSkillNames.Contains("Crafting"),
			"Crafting is a skill category/decorator name in the stock package, not an actual seeded skill.");
		Assert.IsFalse(craftSource.Contains("\"Crafting\", \"Crafting\", AncientHouseholdCraftingKnowledge", StringComparison.Ordinal),
			"The household fallback craft path must use a concrete seeded skill rather than the Crafting category name.");
	}

	[TestMethod]
	public void AntiquityCrafting_LitWorkshopItemsHaveMorphTargetsTimersAndToolTags()
	{
		foreach (var spec in new[] {
		("antiquity_lit_workshop_hearth", "antiquity_workshop_hearth", "Functions / Material Functions / Fire"),
		("antiquity_lit_clay_smelting_furnace", "antiquity_clay_smelting_furnace", "Functions / Tools / Smelting Tools / Smelting Furnace / Lit Smelting Furnace"),
		("antiquity_lit_updraft_kiln", "antiquity_updraft_kiln", "Functions / Tools / Pottery Tools / Kiln / Lit Kiln"),
		("antiquity_lit_glory_hole_furnace", "antiquity_glory_hole_furnace", "Functions / Tools / Glassblowing Tools / Glory Hole / Lit Glory Hole"),
		("antiquity_lit_annealing_lehr", "antiquity_annealing_lehr", "Functions / Tools / Glassblowing Tools / Annealing Lehr / Lit Annealing Lehr") })
		{ var item = Recipes.Item(spec.Item1); Assert.AreEqual(spec.Item2, item.MorphToStableReference); Assert.IsTrue(item.MorphSeconds > 0); CollectionAssert.Contains(item.Tags.ToArray(), spec.Item3); Recipes.ItemsAreCrafted([spec.Item1]); }
	}

	[TestMethod]
	public void AntiquityCrafting_ProducedIntermediateTagsAreConsumedOrDocumentedReusableStock()
	{
		var craftSource = ReadSeederSources("ItemSeeder.Crafting.Antiquity*.cs");
		var docsSource =
			ReadSource("Design Documents", "Seeding", "Antiquity_Equipment_Crafting_Suite.md") +
			ReadSource("Design Documents", "Seeding", "Antiquity_Food_Beverage_Crafting_Suite.md") +
			ReadSource("Design Documents", "Seeding", "Antiquity_Furniture_Container_Crafting_Suite.md") +
			ReadSource("Design Documents", "Seeding", "Antiquity_Jewellery_Crafting_Suite.md") +
			ReadSource("Design Documents", "Seeding", "Antiquity_Writing_Implements_Crafting_Suite.md") +
			ReadSource("Design Documents", "Seeding", "Antiquity_Medical_Crafting_Suite.md");

		var producedTags = ExtractLiteralCommodityProductTags(craftSource);
		var consumedTags = ExtractLiteralCommodityInputTags(craftSource);

		var undocumentedReusableOutputs = producedTags
			.Where(x => !consumedTags.Contains(x))
			.Where(x => !docsSource.Contains($"`{x}`", StringComparison.Ordinal))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, undocumentedReusableOutputs.Count,
			$"Produced intermediate tags should be consumed downstream or documented as reusable stock outputs. Missing docs: {string.Join(", ", undocumentedReusableOutputs)}");
	}

	[TestMethod]
	public void AntiquityCrafting_DocumentationMentionsEveryCraftSourceStableReference()
	{
		var craftSource = ReadSeederSources("ItemSeeder.Crafting.Antiquity*.cs");
		var docsSource = ReadDesignDocumentSources("Antiquity_*.md");

		var craftStableReferences = ExtractStableReferences(craftSource)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var documentedStableReferences = ExtractStableReferences(docsSource);

		var missing = craftStableReferences
			.Where(x => !documentedStableReferences.Contains(x))
			.ToList();

		Assert.AreEqual(0, missing.Count,
			$"Every stable reference explicitly named by current antiquity craft source should be catalogued in the antiquity docs. Missing: {string.Join(", ", missing)}");
	}

	[TestMethod]
	public void AntiquityMedicalCookingPotCrafts_RequireLitFireState()
	{
		foreach (var name in new[] { "prepare clean dressing stock", "render herbal salve stock", "steep medicinal decoction stock" })
		{
			var recipe = Recipes.Named(name);
			Assert.IsTrue(recipe.Tools.OfType<CatalogueTagRecipeTool>().Any(tool => tool.Tag == "Cooking Pot" && tool.State == MudSharp.GameItems.Inventory.Plans.DesiredItemState.InRoom), name);
			Assert.IsTrue(recipe.Tools.OfType<CatalogueTagRecipeTool>().Any(tool => tool.Tag == "Fire" && tool.State == MudSharp.GameItems.Inventory.Plans.DesiredItemState.InRoom), name);
		}
	}

	[TestMethod]
	public void AntiquityCrafting_CatalogueAuditDocumentsSecondPassResolution()
	{
		var auditDoc = ReadSource("Design Documents", "Seeding", "Antiquity_Crafting_Audit.md");

		foreach (var expected in new[]
		{
			"Second-Pass Resolution",
			"SeedAntiquityJewelleryCrafts",
			"Support tools and unlit workshop apparatus are now craftable",
			"glassworking glory-hole furnace",
			"source-backed regression tests"
		})
		{
			AssertContains(auditDoc, expected);
		}
	}

	private static bool IsCoveredByCraftSuites(SeededAntiquityItem item, string existingCraftSource,
		string equipmentCraftSource, string jewelleryCraftSource, string allCraftSource)
	{
		return IsJewelleryDynamicTarget(item, jewelleryCraftSource) ||
		       existingCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal) ||
		       IsHouseholdDynamicTarget(item) ||
		       equipmentCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal) ||
		       IsMilitaryEquipmentTarget(item, existingCraftSource) ||
		       allCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal);
	}

	private static bool IsJewelleryDynamicTarget(SeededAntiquityItem item, string jewelleryCraftSource)
	{
		return item.MethodName.Equals("SeedAntiquityJewellery", StringComparison.Ordinal) &&
		       HasRoot(item.Tags, "Functions / Worn Items / Jewellery") &&
		       jewelleryCraftSource.Contains("SeedAntiquityJewelleryCrafts()", StringComparison.Ordinal);
	}

	private static bool IsHouseholdDynamicTarget(SeededAntiquityItem item)
	{
		return item.MethodName is "SeedAntiquityContainers" or "SeedAntiquityDoorsAndLocks" or "SeedAntiquityHouseholdFurniture" &&
		       HasAnyRoot(item.Tags, HouseholdCraftFunctionalRoots);
	}

	private static bool IsMilitaryEquipmentTarget(SeededAntiquityItem item, string existingCraftSource)
	{
		return HasRoot(item.Tags, "Functions / Military Equipment") &&
		       !existingCraftSource.Contains($"\"{item.StableReference}\"", StringComparison.Ordinal);
	}

	private static bool IsAntiquityCraftToolTarget(SeededAntiquityItem item)
	{
		return item.MethodName.Equals("SeedAntiquityClothing", StringComparison.Ordinal) &&
		       HasAnyRoot(item.Tags, CraftToolFunctionalRoots);
	}

	private static bool IsCoveredPartialStableReference(string stableReference, string partialItemSource,
		string allCraftSource, string equipmentCraftSource)
	{
		return allCraftSource.Contains($"\"{stableReference}\"", StringComparison.Ordinal) ||
		       IsDynamicStandardToolHelperReference(stableReference, partialItemSource, equipmentCraftSource) ||
		       IsFoodFinishedBeverageMorphTarget(stableReference, partialItemSource);
	}

	private static bool IsDynamicStandardToolHelperReference(string stableReference, string partialItemSource,
		string equipmentCraftSource)
	{
		if (!equipmentCraftSource.Contains("AntiquityCraftToolFunctionalRoots", StringComparison.Ordinal))
		{
			return false;
		}

		var escapedStableReference = Regex.Escape(stableReference);
		if (Regex.IsMatch(partialItemSource, $@"CreateAntiquityHouseholdCraftTool\(\s*""{escapedStableReference}""") &&
		    partialItemSource.Contains("tags.AddRange(functionalTags);", StringComparison.Ordinal))
		{
			return true;
		}

		if (Regex.IsMatch(partialItemSource, $@"CreateAntiquityFoodTool\(\s*""{escapedStableReference}""") &&
		    partialItemSource.Contains("[toolTag, \"Market / Professional Tools / Standard Tools\"]", StringComparison.Ordinal))
		{
			return true;
		}

		if (Regex.IsMatch(partialItemSource, $@"AddWritingCraftTool\(\s*""{escapedStableReference}""") &&
		    partialItemSource.Contains(".. functionalTags", StringComparison.Ordinal))
		{
			return true;
		}

		var itemBlock = TryExtractCallBlockContaining(partialItemSource, $"\"{stableReference}\"");
		return itemBlock is not null &&
		       CraftToolFunctionalRoots.Any(root => itemBlock.Contains(root, StringComparison.Ordinal));
	}

	private static bool IsFoodFinishedBeverageMorphTarget(string stableReference, string partialItemSource)
	{
		return stableReference.StartsWith("antiquity_food_finished_", StringComparison.OrdinalIgnoreCase) &&
		       stableReference.EndsWith("_amphora", StringComparison.OrdinalIgnoreCase) &&
		       partialItemSource.Contains($"\"{stableReference}\"", StringComparison.Ordinal) &&
		       partialItemSource.Contains("finished is null ? null : finishedStableReference", StringComparison.Ordinal);
	}

	private static IEnumerable<string> ExpectedEquipmentStockTags()
	{
		return
		[
			"Weapon Blade Stock",
			"Weapon Head Stock",
			"Weapon Shaft Stock",
			"Bow Stave",
			"Fletching Stock",
			"Military Cord Stock",
			"Shield Board Stock",
			"Shield Facing Stock",
			"Helmet Bowl Stock",
			"Armour Plate Stock",
			"Armour Ring Stock",
			"Armour Scale Stock",
			"Armour Lamella Stock",
			"Armour Textile Padding",
			"Tool Blank Stock",
			"Door Hardware Stock"
		];
	}

	private static List<SeededAntiquityItem> ParseItemsInMethod(string source, string methodName)
	{
		var body = ExtractMethodBody(source, methodName);
		var matches = Regex.Matches(body,
			@"CreateItem\s*\(\s*""(?<ref>[^""]+)""\s*,\s*""[^""]*""\s*,\s*""(?<short>[^""]*)""[\s\S]*?\n\s*""(?<material>[^""]+)""\s*,\s*\n\s*\[(?<tags>[\s\S]*?)\]\s*,",
			RegexOptions.Multiline);

		return matches
			.Select(match => new SeededAntiquityItem(
				methodName,
				match.Groups["ref"].Value,
				match.Groups["short"].Value,
				match.Groups["material"].Value,
				ExpandInferredFunctionalTags(Regex.Matches(match.Groups["tags"].Value, @"""(?<tag>[^""]+)""")
					.Select(tagMatch => tagMatch.Groups["tag"].Value))
					.ToList()))
			.ToList();
	}

	private static IEnumerable<string> ExpandInferredFunctionalTags(IEnumerable<string> tags)
	{
		var tagList = tags.ToList();
		return tagList
			.Concat(ItemSeeder.InferReworkFunctionalTagsForTesting(tagList))
			.Distinct(StringComparer.OrdinalIgnoreCase);
	}

	private static bool HasAnyRoot(IEnumerable<string> tags, IEnumerable<string> roots)
	{
		return roots.Any(root => HasRoot(tags, root));
	}

	private static bool HasRoot(IEnumerable<string> tags, string root)
	{
		return tags.Any(tag => tag.Equals(root, StringComparison.OrdinalIgnoreCase) ||
		                       tag.StartsWith($"{root} /", StringComparison.OrdinalIgnoreCase));
	}

	private static string ExtractMethodBody(string source, string methodName)
	{
		var marker = $"private void {methodName}(";
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find method {methodName}.");
		var openBrace = source.IndexOf('{', start);
		Assert.IsTrue(openBrace >= 0, $"Could not find body for method {methodName}.");

		var depth = 0;
		for (var i = openBrace; i < source.Length; i++)
		{
			switch (source[i])
			{
				case '{':
					depth++;
					break;
				case '}':
					depth--;
					if (depth == 0)
					{
						return source[(openBrace + 1)..i];
					}
					break;
			}
		}

		Assert.Fail($"Could not extract body for method {methodName}.");
		return string.Empty;
	}

	private static string ExtractBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source block for {marker}.");
		var openBrace = source.IndexOf('{', start);
		Assert.IsTrue(openBrace >= 0, $"Could not find body for {marker}.");

		var depth = 0;
		for (var i = openBrace; i < source.Length; i++)
		{
			switch (source[i])
			{
				case '{':
					depth++;
					break;
				case '}':
					depth--;
					if (depth == 0)
					{
						return source[(openBrace + 1)..i];
					}
					break;
			}
		}

		Assert.Fail($"Could not extract block for {marker}.");
		return string.Empty;
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static void AssertContains(IReadOnlyCollection<string> source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparer.OrdinalIgnoreCase),
			$"Expected source collection to contain: {expected}");
	}

	private static string ExtractCallBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source block for {marker}.");
		var end = source.IndexOf(");", start, StringComparison.Ordinal);
		Assert.IsTrue(end >= 0, $"Could not find end of source block for {marker}.");
		return source[start..end];
	}

	private static string? TryExtractCallBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		if (start < 0)
		{
			return null;
		}

		var end = source.IndexOf(");", start, StringComparison.Ordinal);
		return end < 0 ? null : source[start..end];
	}

	private static HashSet<string> ExtractLiteralCommodityProductTags(string source)
	{
		return Regex.Matches(source, @"CommodityProduct\s*-\s*[^""\]]*?;\s*tag\s+(?<tag>[^;""\]\}]+)")
			.Select(x => x.Groups["tag"].Value.Trim())
			.Where(IsLiteralTagReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static HashSet<string> ExtractLiteralCommodityInputTags(string source)
	{
		var pileTags = Regex.Matches(source, @"piletag\s+(?<tag>[^;""\]\)]+)")
			.Select(x => x.Groups["tag"].Value.Trim());
		var helperTags = Regex.Matches(source, @"CommodityInput\([^,\)]*,\s*""[^""]+""\s*,\s*""(?<tag>[^""]+)""")
			.Select(x => x.Groups["tag"].Value.Trim());

		return pileTags
			.Concat(helperTags)
			.Where(IsLiteralTagReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsLiteralTagReference(string tag)
	{
		return !string.IsNullOrWhiteSpace(tag) &&
		       !tag.Contains('{', StringComparison.Ordinal) &&
		       !tag.Contains('$', StringComparison.Ordinal);
	}

	private static HashSet<string> ExtractStableReferences(string source)
	{
		return Regex.Matches(source, @"\b(?:antiquity|adjacent_antiquity|jewellery)_[a-z0-9_]+\b")
			.Select(x => x.Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static HashSet<string> ExtractExplicitStringStableReferences(string source)
	{
		return Regex.Matches(source,
				@"""(?<stable>(?:antiquity|adjacent_antiquity|jewellery)_[a-z0-9]+(?:_[a-z0-9]+)+)""")
			.Select(x => x.Groups["stable"].Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static HashSet<string> ExtractSkillPackageNames(string source)
	{
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (Match match in Regex.Matches(source,
			         @"new\s+SkillDetails\(\s*""(?<gerund>[^""]+)""\s*,\s*""(?<imperative>[^""]+)"""))
		{
			names.Add(match.Groups["gerund"].Value);
			names.Add(match.Groups["imperative"].Value);
		}

		return names;
	}

	private static HashSet<string> ExtractStockSkillPackageTraitAliases(string source)
	{
		var marker = source.IndexOf("StockSkillPackageTraitAliases", StringComparison.Ordinal);
		Assert.IsTrue(marker >= 0, "Could not find StockSkillPackageTraitAliases.");
		var end = source.IndexOf("];", marker, StringComparison.Ordinal);
		Assert.IsTrue(end > marker, "Could not find the end of StockSkillPackageTraitAliases.");
		var aliasBlock = source[marker..end];

		return Regex.Matches(aliasBlock, @"""(?<name>[^""]+)""")
			.Select(x => x.Groups["name"].Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static HashSet<string> ExtractItemCraftSkillRequests(string source)
	{
		var names = Regex.Matches(source, @"_traits\[""(?<name>[^""]+)""\]")
			.Select(x => x.Groups["name"].Value)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var argumentList in ExtractInvocationArgumentLists(source, "AddCraft"))
		{
			var arguments = SplitTopLevelArguments(argumentList).ToArray();
			if (arguments.Length > 10 && arguments[10].StartsWith("Difficulty.", StringComparison.Ordinal))
			{
				AddTraitReferences(arguments[9], names);
				continue;
			}

			if (arguments.Length > 8 && arguments[8].StartsWith("Difficulty.", StringComparison.Ordinal))
			{
				AddStringLiteralTrait(arguments[6], names);
				continue;
			}

			if (arguments.Length > 7 && arguments[7].StartsWith("Difficulty.", StringComparison.Ordinal))
			{
				AddStringLiteralTrait(arguments[5], names);
			}
		}

		foreach (var argumentList in ExtractInvocationArgumentLists(source, "AddAntiquityCraft"))
		{
			var arguments = SplitTopLevelArguments(argumentList).ToArray();
			if (arguments.Length > 6)
			{
				AddStringLiteralTrait(arguments[6], names);
			}
		}

		foreach (var helperName in new[]
		         {
			         "AddAntiquityHeatSourceCraft",
			         "AddAntiquityEquipmentCommodityCraft",
			         "AddAntiquityJewelleryCommodityCraft",
			         "AddAntiquityMedicalCommodityCraft",
			         "AddAntiquityHouseholdCommodityCraft",
			         "AddAntiquityWritingCommodityCraft"
		         })
		{
			foreach (var argumentList in ExtractInvocationArgumentLists(source, helperName))
			{
				var arguments = SplitTopLevelArguments(argumentList).ToArray();
				if (arguments.Length > 2)
				{
					AddStringLiteralTrait(arguments[2], names);
				}
			}
		}

		return names;
	}

	private static void AddTraitReferences(string source, HashSet<string> names)
	{
		foreach (Match match in Regex.Matches(source, @"_traits\[""(?<name>[^""]+)""\]"))
		{
			if (!string.IsNullOrWhiteSpace(match.Groups["name"].Value))
			{
				names.Add(match.Groups["name"].Value);
			}
		}

		AddStringLiteralTrait(source, names);
	}

	private static void AddStringLiteralTrait(string argument, HashSet<string> names)
	{
		var match = Regex.Match(argument.Trim(), @"^""(?<name>(?:\\.|[^""\\])*)""$");
		if (match.Success && !string.IsNullOrWhiteSpace(match.Groups["name"].Value))
		{
			names.Add(match.Groups["name"].Value);
		}
	}

	private static IEnumerable<string> ExtractInvocationArgumentLists(string source, string methodName)
	{
		foreach (Match match in Regex.Matches(source, $@"\b{Regex.Escape(methodName)}\s*\("))
		{
			var openParen = source.IndexOf('(', match.Index);
			var depth = 0;
			var inString = false;
			var escaped = false;
			for (var i = openParen; i < source.Length; i++)
			{
				var ch = source[i];
				if (inString)
				{
					if (escaped)
					{
						escaped = false;
					}
					else if (ch == '\\')
					{
						escaped = true;
					}
					else if (ch == '"')
					{
						inString = false;
					}

					continue;
				}

				if (ch == '"')
				{
					inString = true;
					continue;
				}

				if (ch == '(')
				{
					depth++;
					continue;
				}

				if (ch != ')')
				{
					continue;
				}

				depth--;
				if (depth == 0)
				{
					yield return source[(openParen + 1)..i];
					break;
				}
			}
		}
	}

	private static IEnumerable<string> SplitTopLevelArguments(string arguments)
	{
		var start = 0;
		var parenDepth = 0;
		var bracketDepth = 0;
		var braceDepth = 0;
		var inString = false;
		var escaped = false;

		for (var i = 0; i < arguments.Length; i++)
		{
			var ch = arguments[i];
			if (inString)
			{
				if (escaped)
				{
					escaped = false;
				}
				else if (ch == '\\')
				{
					escaped = true;
				}
				else if (ch == '"')
				{
					inString = false;
				}

				continue;
			}

			switch (ch)
			{
				case '"':
					inString = true;
					break;
				case '(':
					parenDepth++;
					break;
				case ')':
					parenDepth--;
					break;
				case '[':
					bracketDepth++;
					break;
				case ']':
					bracketDepth--;
					break;
				case '{':
					braceDepth++;
					break;
				case '}':
					braceDepth--;
					break;
				case ',' when parenDepth == 0 && bracketDepth == 0 && braceDepth == 0:
					yield return arguments[start..i].Trim();
					start = i + 1;
					break;
			}
		}

		yield return arguments[start..].Trim();
	}

	private static string ReadSeederSources(string pattern)
	{
		var sourceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
		return string.Concat(Directory
			.EnumerateFiles(Path.Combine(sourceRoot, "DatabaseSeeder", "Seeders", "ItemSeeder"), pattern)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Select(File.ReadAllText));
	}

	private static string ReadDesignDocumentSources(string pattern)
	{
		var sourceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
		return string.Concat(Directory
			.EnumerateFiles(Path.Combine(sourceRoot, "Design Documents", "Seeding"), pattern)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Select(File.ReadAllText));
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}

	private sealed record SeededAntiquityItem(
		string MethodName,
		string StableReference,
		string ShortDescription,
		string Material,
		IReadOnlyList<string> Tags);
}
