#nullable enable

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
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var partialItemSource =
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityApiary.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityHouseholdTools.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityMedical.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityWriting.cs");
		var existingCraftSource =
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Antiquity.cs") +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var jewelleryCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityJewellery.cs");
		var allCraftSource =
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs") +
			ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");

		var items = AntiquityReworkMethods
			.SelectMany(method => ParseItemsInMethod(itemSource, method))
			.ToList();

		Assert.AreEqual(1042, items.Count, "The audit should track the current antiquity item catalogue.");
		Assert.AreEqual(29, items.Count(IsAntiquityCraftToolTarget));
		Assert.AreEqual(18, items.Count(x => equipmentCraftSource.Contains($"\"{x.StableReference}\"", StringComparison.Ordinal) &&
		                                     x.MethodName.Equals("SeedAntiquityClothing", StringComparison.Ordinal) &&
		                                     !IsAntiquityCraftToolTarget(x)));
		Assert.AreEqual(193, items.Count(x => IsMilitaryEquipmentTarget(x, existingCraftSource)));
		Assert.AreEqual(398, items.Count(IsHouseholdDynamicTarget));

		var uncovered = items
			.Where(item => !IsCoveredByCraftSuites(item, existingCraftSource, equipmentCraftSource, jewelleryCraftSource,
				allCraftSource))
			.Select(item => $"{item.MethodName}:{item.StableReference}")
			.ToList();

		Assert.AreEqual(0, uncovered.Count,
			$"Expected every antiquity prototype to be covered by an existing or new craft suite. Missing: {string.Join(", ", uncovered)}");

		var partialStableReferences = ExtractExplicitStringStableReferences(partialItemSource)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		AssertContains(partialItemSource, "CreateAntiquityFoodTool");
		AssertContains(partialItemSource, "SeedAntiquityApiaryItems");
		AssertContains(partialItemSource, "CreateAntiquityHouseholdCraftTool");
		AssertContains(partialItemSource, "SeedAntiquityMedicalItems");
		AssertContains(partialItemSource, "SeedAntiquityWritingImplementsAndDocuments");
		AssertContains(partialStableReferences, "antiquity_food_butchers_knife");
		AssertContains(partialStableReferences, "antiquity_wicker_beehive");
		AssertContains(partialStableReferences, "antiquity_honey_press");
		AssertContains(partialStableReferences, "antiquity_food_serving_amphora");
		AssertContains(partialStableReferences, "antiquity_food_finished_beer_amphora");
		AssertContains(partialStableReferences, "antiquity_food_finished_spiced_kumis_amphora");

		var uncoveredPartialReferences = partialStableReferences
			.Where(stableReference => !IsCoveredPartialStableReference(stableReference, partialItemSource,
				allCraftSource, equipmentCraftSource))
			.ToList();

		Assert.AreEqual(0, uncoveredPartialReferences.Count,
			$"Expected every explicitly named item in antiquity partial seeders to be craft-covered, dynamically craftable, or a documented morph target. Missing: {string.Join(", ", uncoveredPartialReferences)}");
	}

	[TestMethod]
	public void AntiquityRepairKitCrafts_RegisterGeneralMaterialCoverage()
	{
		var reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityRepairKits.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var componentSource = SeederSourceTestHelper.ReadPartialFamily("UsefulSeeder.ItemComponents");
		var componentCatalogue = ReadSource("Design Documents", "Data", "Seeded_Item_Components.json");
		var equipmentDoc = ReadSource("Design Documents", "Seeding", "Antiquity_Equipment_Crafting_Suite.md");

		AssertContains(reworkRoot, "SeedAntiquityRepairKits();");
		AssertContains(craftRoot, "SeedAntiquityRepairKitCrafts();");
		AssertContains(itemSource, "private void SeedAntiquityRepairKits()");
		AssertContains(craftSource, "private void SeedAntiquityRepairKitCrafts()");
		AssertContains(craftSource, "AncientToolmakingKnowledge");
		AssertContains(craftSource, "Repair Kits");
		AssertContains(equipmentCraftSource, "!AntiquityRepairKitStableReferences.Contains");

		var repairItems = ParseItemsInMethod(itemSource, "SeedAntiquityRepairKits");
		Assert.AreEqual(8, repairItems.Count, "The antiquity repair-kit stock surface should stay intentionally compact.");

		foreach (var expected in new[]
		{
			(Stable: "antiquity_textile_repair_kit", Component: "Repair_Cloth", Material: "linen", Skill: "Tailoring"),
			(Stable: "antiquity_leather_repair_kit", Component: "Repair_Leather", Material: "deer leather", Skill: "Leathermaking"),
			(Stable: "antiquity_wood_repair_kit", Component: "Repair_Wood", Material: "oak", Skill: "Carpentry"),
			(Stable: "antiquity_metal_repair_kit", Component: "Repair_Metal", Material: "bronze", Skill: "Blacksmithing"),
			(Stable: "antiquity_stone_repair_kit", Component: "Repair_Stone", Material: "limestone", Skill: "Masonry"),
			(Stable: "antiquity_ceramic_repair_kit", Component: "Repair_Ceramic", Material: "earthenware", Skill: "Pottery"),
			(Stable: "antiquity_hard_organic_repair_kit", Component: "Repair_Hard_Organic", Material: "bone", Skill: "Scrimshawing"),
			(Stable: "antiquity_field_repair_bundle", Component: "Repair_Universal", Material: "leather", Skill: "Salvaging")
		})
		{
			var block = ExtractCallBlockContaining(itemSource, expected.Stable);
			AssertContains(block, expected.Component);
			AssertContains(block, expected.Material);
			AssertContains(block, "Market / Professional Tools / Standard Tools");
			AssertContains(block, "Functions / Repairing");
			AssertContains(craftSource, $"\"{expected.Stable}\"");
			AssertContains(craftSource, $"\"{expected.Skill}\"");
			AssertContains(equipmentDoc, $"`{expected.Stable}`");
			AssertContains(componentCatalogue, expected.Component);
		}

		foreach (var expectedComponent in new[]
		{
			"Repair_Wood",
			"Repair_Metal",
			"Repair_Stone",
			"Repair_Ceramic",
			"Repair_Hard_Organic"
		})
		{
			AssertContains(componentSource, $"AddRepairKitType(\"{expectedComponent["Repair_".Length..]}\"");
			AssertContains(componentCatalogue, expectedComponent);
			AssertContains(componentCatalogue, $"{expectedComponent}_Good");
			AssertContains(componentCatalogue, $"{expectedComponent}_Poor");
		}
	}

	[TestMethod]
	public void AntiquityJewelleryCrafts_RegisterDynamicKnowledgeGatedSuiteAndCatalogue()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityJewellery.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var tagHierarchy = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");
		var jewelleryDoc = ReadSource("Design Documents", "Seeding", "Antiquity_Jewellery_Crafting_Suite.md");

		var jewelleryItems = ParseItemsInMethod(itemSource, "SeedAntiquityJewellery");
		Assert.AreEqual(162, jewelleryItems.Count, "The jewellery craft catalogue should track the current stock jewellery set.");
		var usekhBlock = ExtractCallBlockContaining(itemSource, "jewellery_gold_usekh_collar");
		AssertContains(usekhBlock, "carnelian, turquoise and lapis-coloured beads");

		foreach (var expected in new[]
		{
			"SeedAntiquityJewelleryCrafts();",
			"private void SeedAntiquityJewelleryCrafts()",
			"SeedAntiquityJewelleryIntermediateCommodityCrafts();",
			"Ancient Jewellery Crafting",
			"Jewellery Metal Stock",
			"Jewellery Wire Stock",
			"Jewellery Bead Stock",
			"Jewellery Setting Stock",
			"Wooden Bead Jewellery",
			"Functions / Worn Items / Jewellery",
			"var materialScanText = $\"{stableReference} {item.ShortDescription} {item.FullDescription}\".ToLowerInvariant();",
			"GetAntiquityJewelleryCraftPath(material, materialScanText)",
			"BuildAntiquityJewelleryFinalInputs(item, material, materialScanText, path)",
			"(\"lapis\", \"lapis lazuli\")",
			"AddAntiquityCraft(",
			"knowledgeSubtype:",
			"SanitiseAntiquityJewelleryVisibleName(item.ShortDescription)",
			"BuildUniqueAntiquityEquipmentCraftName"
		})
		{
			Assert.IsTrue(craftRoot.Contains(expected, StringComparison.Ordinal) ||
			              craftSource.Contains(expected, StringComparison.Ordinal),
				$"Expected source to contain: {expected}");
		}

		foreach (var expected in new[]
		{
			"Jewellery Craft Stock",
			"Jewellery Metal Stock",
			"Jewellery Wire Stock",
			"Jewellery Bead Stock",
			"Jewellery Setting Stock"
		})
		{
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, expected.Equals("Jewellery Craft Stock", StringComparison.Ordinal)
				? $"Material Functions / {expected}"
				: $"Jewellery Craft Stock / {expected}");
			if (!expected.Equals("Jewellery Craft Stock", StringComparison.Ordinal))
			{
				AssertContains(craftSource, expected);
			}
		}

		foreach (var item in jewelleryItems)
		{
			AssertContains(jewelleryDoc, $"`{item.StableReference}`");
		}
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_RegisterKnowledgeGatedSuitesAndCorrectAccessProgs()
	{
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		AssertContains(craftRoot, "SeedAntiquityEquipmentCrafts();");
		AssertContains(craftRoot, "AddProg(\"HasWeaponcrafting\"");
		AssertContains(craftRoot, "_traits[\"Weaponcrafting\"]?.Id ?? _traits[\"Weaponsmith\"]?.Id");
		AssertContains(craftRoot, "AddProg(\"HasArmourcrafting\"");
		AssertContains(craftRoot, "_traits[\"Armourcrafting\"]?.Id ?? _traits[\"Armourer\"]?.Id");

		foreach (var expected in new[]
		{
			"private void SeedAntiquityEquipmentCrafts()",
			"SeedAntiquityEquipmentIntermediateCommodityCrafts();",
			"Ancient Equipment Crafting",
			"Ancient Weaponcrafting",
			"Ancient Armourcrafting",
			"Ancient Toolmaking",
			"Ancient Common Clothing Crafting",
			"AddAntiquityCraft(",
			"knowledgeSubtype:",
			"knowledgeDescription:",
			"Functions / Military Equipment"
		})
		{
			AssertContains(equipmentCraftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_AddUpstreamCommodityAndTagSurface()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var tagHierarchy = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");

		foreach (var expected in ExpectedEquipmentStockTags())
		{
			AssertContains(craftSource, expected);
			AssertContains(tagSource, $"AddTag(context, \"{expected}\",");
			AssertContains(tagHierarchy, $"Antiquity Equipment Stock / {expected}");
		}

		foreach (var expected in new[]
		{
			"CommodityProduct -",
			"Weapon Blade Stock",
			"Weapon Head Stock",
			"Weapon Shaft Stock",
			"Bow Stave",
			"Fletching Stock",
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
		})
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_MakeSupportToolsAndUnlitApparatusCraftable()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");
		var householdToolSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityHouseholdTools.cs");

		foreach (var expected in new[]
		{
			"craftToolTagIds",
			"AntiquityCraftToolFunctionalRoots",
			"Functions / Tools",
			"Functions / Sharpening",
			"AntiquityUnlitWorkshopApparatusStableReferences",
			"AntiquityLitWorkshopApparatusStableReferences",
			"antiquity_workshop_hearth",
			"antiquity_updraft_kiln",
			"antiquity_glory_hole_furnace",
			"antiquity_annealing_lehr",
			"antiquity_clay_smelting_furnace",
			"Pottery Clay Body",
			"Tool Blank Stock",
			"AntiquityEquipmentToolBlankMaterials",
			"GetToolBlankSkill(material)",
			"ToolBlankShapingTools(material)",
			"\"glass\"",
			"\"shell\"",
			"\"stone\""
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var litOnly in new[]
		{
			"antiquity_lit_workshop_hearth",
			"antiquity_lit_updraft_kiln",
			"antiquity_lit_glory_hole_furnace",
			"antiquity_lit_annealing_lehr",
			"antiquity_lit_clay_smelting_furnace"
		})
		{
			AssertContains(householdToolSource, litOnly);
			AssertContains(craftSource, litOnly);
		}

		var stoneBlankToolBranch = ExtractBlockContaining(craftSource,
			"if (material.Equals(\"stone\", StringComparison.OrdinalIgnoreCase))");
		AssertContains(stoneBlankToolBranch, "TagTool - Held - an item with the Stone Chisel tag");
		AssertContains(stoneBlankToolBranch, "TagTool - Held - an item with the Stone Mallet tag");
		Assert.IsFalse(stoneBlankToolBranch.Contains("Polishing Stone", StringComparison.Ordinal),
			"Stone tool blank stock must not require an existing polishing stone because it bootstraps the polishing stone tool itself.");
	}

	[TestMethod]
	public void AntiquityEquipmentCrafts_KeepVisibleCraftStringsCultureNeutral()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		AssertContains(craftSource, "SanitiseAntiquityEquipmentVisibleName(item.ShortDescription)");
		AssertContains(craftSource, "BuildUniqueAntiquityEquipmentCraftName");
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
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
		var itemSource = ReadSeederSources("ItemSeeder.Rework.Antiquity*.cs");

		var toolTags = Regex.Matches(craftSource,
				@"TagTool\s*-\s*(?:Held|InRoom|In Room|Wielded)\s*-\s*an item with the (?<tag>.+?) tag",
				RegexOptions.IgnoreCase)
			.Select(x => x.Groups["tag"].Value.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var seededToolTagLeafs = Regex.Matches(itemSource, @"""(?<tag>Functions / [^""]+)""")
			.Select(x => x.Groups["tag"].Value.Split(" / ").Last().Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var missing = toolTags
			.Where(x => !seededToolTagLeafs.Contains(x))
			.ToList();

		Assert.AreEqual(0, missing.Count,
			$"Every current antiquity TagTool should have at least one seeded rework item carrying the exact tool tag. Missing: {string.Join(", ", missing)}");
	}

	[TestMethod]
	public void ItemSeederCrafts_RequestedSkillNamesResolveAgainstStockSkillPackage()
	{
		var skillSource = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSeederSources("ItemSeederCrafting*.cs");

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
		var createItemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var householdToolSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityHouseholdTools.cs");
		var equipmentCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityEquipment.cs");

		foreach (var expected in new[]
		{
			"item.MorphGameItemProtoId = morphItem.Id;",
			"item.MorphTimeSeconds = (int)morphTimer.Value.TotalSeconds;",
			"item.MorphEmote = morphEmote;",
			"item.OnDestroyedGameItemProtoId = destroyedItem.Id;"
		})
		{
			AssertContains(createItemSource, expected);
		}

		foreach (var spec in new[]
		{
			(Lit: "antiquity_lit_workshop_hearth", Unlit: "antiquity_workshop_hearth", ToolTag: "Functions / Material Functions / Fire"),
			(Lit: "antiquity_lit_clay_smelting_furnace", Unlit: "antiquity_clay_smelting_furnace", ToolTag: "Functions / Tools / Smelting Tools / Smelting Furnace / Lit Smelting Furnace"),
			(Lit: "antiquity_lit_updraft_kiln", Unlit: "antiquity_updraft_kiln", ToolTag: "Functions / Tools / Pottery Tools / Kiln / Lit Kiln"),
			(Lit: "antiquity_lit_glory_hole_furnace", Unlit: "antiquity_glory_hole_furnace", ToolTag: "Functions / Tools / Glassblowing Tools / Glory Hole / Lit Glory Hole"),
			(Lit: "antiquity_lit_annealing_lehr", Unlit: "antiquity_annealing_lehr", ToolTag: "Functions / Tools / Glassblowing Tools / Annealing Lehr / Lit Annealing Lehr")
		})
		{
			var block = ExtractCallBlockContaining(householdToolSource, spec.Lit);
			AssertContains(block, spec.Unlit);
			AssertContains(block, spec.ToolTag);
			AssertContains(block, "TimeSpan.FromHours(");
		}

		foreach (var expected in new[]
		{
			"SeedAntiquityWorkshopHeatSourceCrafts();",
			"light a workshop hearth",
			"stoke an updraft pottery kiln",
			"stoke a clay smelting furnace",
			"stoke a glassworking glory hole",
			"stoke an annealing lehr",
			"CommodityInput(charcoalGrams, \"charcoal\")",
			"lay|lays $i2 into the fire bed",
			"StableUnusedInputProduct(unlitStableReference, 1)"
		})
		{
			AssertContains(equipmentCraftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityCrafting_ProducedIntermediateTagsAreConsumedOrDocumentedReusableStock()
	{
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
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
		var craftSource = ReadSeederSources("ItemSeederCrafting.Antiquity*.cs");
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
		var medicalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityMedical.cs");

		foreach (var craftName in new[]
		{
			"prepare clean dressing stock",
			"render herbal salve stock",
			"steep medicinal decoction stock"
		})
		{
			var block = ExtractCallBlockContaining(medicalCraftSource, craftName);
			AssertContains(block, "TagTool - InRoom - an item with the Cooking Pot tag");
			AssertContains(block, "TagTool - InRoom - an item with the Fire tag");
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
			.EnumerateFiles(Path.Combine(sourceRoot, "DatabaseSeeder", "Seeders"), pattern)
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
