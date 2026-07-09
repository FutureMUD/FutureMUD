#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PrimaryProductionSeederSourceTests
{
	[TestMethod]
	public void Phase0Audit_ExistingMaterialSeederProvidesPrimaryProductionInputs()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		{
			"AddMaterial(\"hematite\"",
			"AddMaterial(\"magnetite\"",
			"AddMaterial(\"cassiterite\"",
			"AddMaterial(\"galena\"",
			"AddMaterial(\"malachite\"",
			"AddMaterial(\"native copper\"",
			"AddMaterial(\"limestone\"",
			"AddMaterial(\"clay\"",
			"AddMaterial(\"fire clay\"",
			"AddMaterial(\"peat\"",
			"AddMaterial(\"salt\"",
			"AddMaterial(\"brick\"",
			"AddMaterial(\"glass\"",
			"\"mortar\", \"grout\"",
			"AddMaterial(\"pitch\"",
			"AddMaterial(\"tar\"",
			"AddMaterial(\"charcoal\"",
			"AddMaterial(\"lye\"",
			"AddMaterial(\"slaked lime\"",
			"AddMaterial(\"calcium oxide\"",
			"AddMaterial(\"calcium hydroxide\"",
			"AddMaterial(\"sulfur\"",
			"AddMaterial(\"wrought iron\"",
			"AddMaterial(\"sponge iron\""
		})
		{
			AssertContains(materialSource, expected);
		}
	}

	[TestMethod]
	public void Phase0Audit_ExistingUsefulTagsProvideReusableStockAndToolRoots()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");

		foreach (var expected in new[]
		{
			"AddTag(context, \"Material Functions\", \"Functions\")",
			"AddTag(context, \"Ore Deposit\", \"Material Functions\")",
			"AddTag(context, \"Hot Fire\", \"Material Functions\")",
			"AddTag(context, \"Household Craft Stock\", \"Material Functions\")",
			"AddTag(context, \"Prepared Pitch\", \"Household Craft Stock\")",
			"AddTag(context, \"Glass Batch\", \"Household Craft Stock\")",
			"AddTag(context, \"Tool Blank Stock\", \"Antiquity Equipment Stock\")",
			"AddTag(context, \"Tools\", \"Functions\")",
			"AddTag(context, \"Hammer\", \"Striking Tools\")",
			"AddTag(context, \"Chisel\", \"Cutting and Shaping Tools\")",
			"AddTag(context, \"Wheelbarrow\", \"Construction Tools\")",
			"AddTag(context, \"Professional Tools\", \"Market\")",
			"AddTag(context, \"Construction Materials\", \"Market\")",
			"AddTag(context, \"Stone Blocks\", \"Raw Materials\")",
			"AddTag(context, \"Aggregate\", \"Raw Materials\")",
			"AddTag(context, \"Lime\", \"Construction Materials\")"
		})
		{
			AssertContains(tagSource, expected);
		}
	}

	[TestMethod]
	public void Phase0Audit_CellTagsAndFutureProgSupportResourceDiscovery()
	{
		var cellInterfaceSource = ReadSource("FutureMUDLibrary", "Construction", "ICell.cs");
		var cellSource = ReadSource("MudSharpCore", "Construction", "Cell.cs");
		var isTaggedSource = ReadSource("MudSharpCore", "FutureProg", "Functions", "BuiltIn", "IsTaggedFunction.cs");
		var activeProjectSource = ReadSource("MudSharpCore", "Work", "Projects", "ConcreteTypes", "ActiveProject.cs");

		AssertContains(cellInterfaceSource, "public interface ICell : ILocation, IProgVariable, IHaveMagicResource, IHaveTags");
		AssertContains(cellSource, "public IEnumerable<ITag> Tags => _tags;");
		AssertContains(cellSource, "public bool AddTag(ITag tag)");
		AssertContains(cellSource, "public bool RemoveTag(ITag tag)");
		AssertContains(cellSource, "public bool IsA(ITag tag)");
		AssertContains(cellSource, "FMDB.Context.CellsTags.RemoveRange(cell.CellsTags);");
		AssertContains(isTaggedSource, "new[] { ProgVariableTypes.Location, ProgVariableTypes.Text }");
		AssertContains(activeProjectSource, "case \"location\":");
		AssertContains(activeProjectSource, "return Location;");
	}

	[TestMethod]
	public void Phase2_PrimaryProductionTagsSeedExpectedHierarchy()
	{
		var usefulTagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var coreMaterialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		{
			"AddTag(context, \"Primary Production\", \"Material Functions\")",
			"AddTag(context, \"Primary Production Ore\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Flux\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Fuel\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Stone\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Clay\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Aggregate\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Metal Stock\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Binder\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Glass Stock\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Salt\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Alkali\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Tar And Pitch\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Pigment\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Refractory\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Waste\", \"Primary Production\")",
			"AddTag(context, \"Primary Production Resource\", \"Primary Production\")",
			"AddTag(context, \"Mineral Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Stone Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Clay Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Salt Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Fuel Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Alkali Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Pigment Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Sulfur Resource\", \"Primary Production Resource\")",
			"AddTag(context, \"Visible Resource Deposit\", \"Primary Production Resource\")",
			"AddTag(context, \"Hidden Resource Marker\", \"Primary Production Resource\")",
			"AddTag(context, \"Hematite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Limonite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Magnetite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Cassiterite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Malachite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Galena Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Native Copper Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Gold-Bearing Gravel Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Limestone Resource\", \"Stone Resource\")",
			"AddTag(context, \"Clay Pit Resource\", \"Clay Resource\")",
			"AddTag(context, \"Brine Spring Resource\", \"Salt Resource\")",
			"AddTag(context, \"Peat Bog Resource\", \"Fuel Resource\")",
			"AddTag(context, \"Natron Resource\", \"Alkali Resource\")",
			"AddTag(context, \"Ochre Resource\", \"Pigment Resource\")",
			"AddTag(context, \"Sulfur Deposit Resource\", \"Sulfur Resource\")",
			"AddTag(context, \"Primary Production Commodity\", \"Primary Production\")",
			"AddTag(context, \"Sample Ore Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Raw Ore Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Roasted Ore Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Charcoal Fuel Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Bloom Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Slag Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Rough Stone Block Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Quicklime Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Prepared Clay Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Glass Batch Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Potash Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Tar Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Fireclay Commodity\", \"Primary Production Commodity\")",
			"AddTag(context, \"Mining Tool\", \"Tools\")",
			"AddTag(context, \"Prospecting Tool\", \"Tools\")",
			"AddTag(context, \"Quarrying Tool\", \"Tools\")",
			"AddTag(context, \"Masonry Tool\", \"Tools\")",
			"AddTag(context, \"Charcoal Burning Tool\", \"Tools\")",
			"AddTag(context, \"Kiln Tool\", \"Tools\")",
			"AddTag(context, \"Smelting Tool\", \"Tools\")",
			"AddTag(context, \"Hauling Tool\", \"Tools\")",
			"AddTag(context, \"Surveying Tool\", \"Tools\")",
			"AddTag(context, \"Saltworking Tool\", \"Tools\")",
			"AddTag(context, \"Tar Burning Tool\", \"Tools\")",
			"AddTag(context, \"Alkali Tool\", \"Tools\")",
			"AddTag(context, \"Peat Cutting Tool\", \"Tools\")",
			"AddTag(context, \"Pigment Processing Tool\", \"Tools\")"
		})
		{
			AssertContains(usefulTagSource, expected);
		}

		foreach (var expected in new[]
		{
			"AddTag(\"Functions\", null)",
			"AddTag(\"Material Functions\", \"Functions\")",
			"AddTag(\"Hot Fire\", \"Material Functions\")",
			"AddTag(\"Primary Production\", \"Material Functions\")",
			"AddTag(\"Primary Production Ore\", \"Primary Production\")",
			"AddTag(\"Primary Production Commodity\", \"Primary Production\")"
		})
		{
			AssertContains(coreMaterialSource, expected);
		}
	}

	[TestMethod]
	public void Phase2_PrimaryProductionMaterialsSeedAliasesAndFunctionalTags()
	{
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		foreach (var expected in new[]
		{
			"AddMaterial(\"limonite ore\"",
			"AddMaterial(\"bog iron ore\"",
			"AddMaterial(\"iron bloom\"",
			"AddMaterial(\"wrought iron billet\"",
			"AddMaterial(\"slag\"",
			"AddMaterial(\"dried peat\"",
			"AddMaterial(\"coal\"",
			"AddMaterial(\"coke\"",
			"AddMaterial(\"stone rubble\"",
			"AddMaterial(\"gravel\"",
			"AddMaterial(\"prepared clay\"",
			"AddMaterial(\"green brick\"",
			"AddMaterial(\"fired brick\"",
			"AddMaterial(\"roof tile\"",
			"AddMaterial(\"glass batch\"",
			"AddMaterial(\"glass blank\"",
			"AddMaterial(\"potash\"",
			"AddMaterial(\"natron\"",
			"AddMaterial(\"bitumen\"",
			"AddMaterial(\"malachite pigment\"",
			"AddMaterial(\"azurite pigment\"",
			"AddMaterial(\"cinnabar pigment\"",
			"EnsureTag(materials[\"hematite\"], \"Primary Production Ore\");",
			"EnsureTag(materials[\"cassiterite\"], \"Primary Production Ore\");",
			"EnsureTag(materials[\"galena\"], \"Primary Production Ore\");",
			"EnsureTag(materials[\"malachite\"], \"Primary Production Pigment\");",
			"EnsureTag(materials[\"limestone\"], \"Primary Production Flux\");",
			"EnsureTag(materials[\"fire clay\"], \"Primary Production Refractory\");",
			"EnsureTag(materials[\"charcoal\"], \"Hot Fire\");",
			"EnsureTag(materials[\"calcium oxide\"], \"Primary Production Binder\");",
			"EnsureTag(materials[\"soda ash\"], \"Primary Production Glass Stock\");",
			"EnsureTag(materials[\"wood ash\"], \"Primary Production Waste\");",
			"EnsureTag(materials[\"wrought iron\"], \"Primary Production Metal Stock\");",
			"EnsureAlias(materials[\"hematite\"], \"hematite ore\", \"haematite\", \"red iron ore\");",
			"EnsureAlias(materials[\"cassiterite\"], \"tin ore\", \"ore tin\");",
			"EnsureAlias(materials[\"galena\"], \"lead ore\", \"ore lead\");",
			"EnsureAlias(materials[\"halite\"], \"rock salt\");",
			"EnsureAlias(materials[\"calcium oxide\"], \"quicklime\", \"burnt lime\");",
			"EnsureAlias(materials[\"mortar\"], \"lime mortar\");",
			"EnsureAlias(materials[\"fire clay\"], \"fireclay\", \"refractory clay\");",
			"EnsureAlias(materials[\"tar\"], \"pine tar\", \"wood tar\");",
			"EnsureAlias(materials[\"saltpeter\"], \"nitre\", \"niter\", \"potassium nitrate\");"
		})
		{
			AssertContains(materialSource, expected);
		}
	}

	[TestMethod]
	public void Phase3_PrimaryProductionItemSeederIsWiredIntoHistoricReworkInstall()
	{
		var reworkSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var sharedBaselineSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.PreIndustrialBaseline.cs");
		var primaryProductionSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.PrimaryProductionTools.cs");

		AssertContains(reworkSource, "SeedSharedPreIndustrialBaselineItems();");
		AssertContains(sharedBaselineSource, "SeedPrimaryProductionToolsAndProps();");
		AssertContains(primaryProductionSource, "private void SeedPrimaryProductionToolsAndProps()");
		AssertContains(primaryProductionSource, "SeedEraItemSpecs(PrimaryProductionItemSpecs());");
		AssertContains(primaryProductionSource, "PrimaryProductionItemSpecsForTesting");
	}

	[TestMethod]
	public void Phase3_PrimaryProductionSpecsCoverToolsResourcePropsAndStaticApparatus()
	{
		var specs = ItemSeeder.PrimaryProductionItemSpecsForTesting.ToArray();
		var stableReferences = ItemSeeder.PrimaryProductionItemStableReferencesForTesting.ToArray();

		Assert.AreEqual(specs.Length, stableReferences.Length);
		Assert.AreEqual(stableReferences.Length, stableReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(specs.Length >= 50, "Primary production should seed a substantial first-pass tool, prop, and apparatus catalogue.");
		Assert.IsTrue(specs.All(x => x.StableReference.StartsWith("primary_production_", StringComparison.Ordinal)));
		Assert.IsTrue(specs.All(x => x.Tags.Contains("Eras / Historic")), "Primary production stock should install with the shared historic item package.");
		Assert.IsTrue(specs.All(x => !string.IsNullOrWhiteSpace(x.FullDescription)), "Seeded item descriptions should remain player-facing, not placeholder text.");

		foreach (var expected in new[]
		{
			"primary_production_prospecting_hammer",
			"primary_production_sample_bag",
			"primary_production_mining_pick",
			"primary_production_ore_basket",
			"primary_production_wheelbarrow",
			"primary_production_masons_hammer",
			"primary_production_charcoal_rake",
			"primary_production_crucible_tongs",
			"primary_production_salt_rake",
			"primary_production_ash_hopper",
			"primary_production_pitch_kettle",
			"primary_production_turf_knife",
			"primary_production_pigment_grinding_slab",
			"primary_production_hematite_deposit",
			"primary_production_limonite_deposit",
			"primary_production_cassiterite_deposit",
			"primary_production_malachite_deposit",
			"primary_production_galena_deposit",
			"primary_production_gold_bearing_gravel_deposit",
			"primary_production_limestone_outcrop",
			"primary_production_slate_outcrop",
			"primary_production_clay_bank",
			"primary_production_brine_spring",
			"primary_production_peat_bog",
			"primary_production_natron_flat",
			"primary_production_ochre_bank",
			"primary_production_sulfur_deposit",
			"primary_production_bitumen_seep",
			"primary_production_bloomery_furnace",
			"primary_production_lime_kiln",
			"primary_production_brick_clamp",
			"primary_production_charcoal_clamp_site",
			"primary_production_salt_pan",
			"primary_production_tar_kiln",
			"primary_production_mine_windlass",
			"primary_production_furnace_bellows",
			"primary_production_glass_furnace",
			"primary_production_peat_drying_rack"
		})
		{
			Assert.IsTrue(stableReferences.Contains(expected, StringComparer.OrdinalIgnoreCase), $"Missing seeded item spec {expected}.");
		}
	}

	[TestMethod]
	public void Phase3_PrimaryProductionToolsPropsAndApparatusUseExpectedTagsAndComponents()
	{
		var specs = ItemSeeder.PrimaryProductionItemSpecsForTesting.ToArray();

		foreach (var toolReference in new[]
		{
			"primary_production_prospecting_hammer",
			"primary_production_mining_pick",
			"primary_production_masons_hammer",
			"primary_production_crucible_tongs",
			"primary_production_salt_rake",
			"primary_production_turf_knife"
		})
		{
			var spec = specs.Single(x => x.StableReference == toolReference);
			CollectionAssert.Contains(spec.Components.ToArray(), "Holdable", $"{toolReference} should be portable.");
			Assert.IsTrue(spec.Components.Any(x => x.StartsWith("Destroyable_", StringComparison.Ordinal)),
				$"{toolReference} should use an existing destroyable component.");
			Assert.IsTrue(spec.Tags.Any(x => x.StartsWith("Functions / Tools /", StringComparison.Ordinal)),
				$"{toolReference} should carry a functional tool tag.");
		}

		foreach (var carryingAid in new[]
		{
			"primary_production_sample_bag",
			"primary_production_ore_basket",
			"primary_production_ore_sack",
			"primary_production_wheelbarrow",
			"primary_production_ash_hopper"
		})
		{
			var spec = specs.Single(x => x.StableReference == carryingAid);
			CollectionAssert.Contains(spec.Components.ToArray(), "Holdable", $"{carryingAid} should be loadable and portable.");
			Assert.IsTrue(spec.Components.Any(x => x.StartsWith("Container_", StringComparison.Ordinal)),
				$"{carryingAid} should reuse an existing container component.");
			Assert.IsTrue(spec.Tags.Contains("Functions / Tools / Hauling Tool") ||
			              spec.Tags.Contains("Functions / Tools / Alkali Tool"),
				$"{carryingAid} should carry a hauling or process tool tag.");
		}

		foreach (var resourceReference in new[]
		{
			"primary_production_hematite_deposit",
			"primary_production_clay_bank",
			"primary_production_brine_spring",
			"primary_production_bitumen_seep"
		})
		{
			var spec = specs.Single(x => x.StableReference == resourceReference);
			Assert.AreEqual(0, spec.Components.Count, $"{resourceReference} should not be holdable or destroyable in the first-pass prop model.");
			CollectionAssert.Contains(spec.Tags.ToArray(),
				"Functions / Material Functions / Primary Production / Primary Production Resource / Visible Resource Deposit");
			Assert.IsTrue(spec.BuilderNotes?.Contains("fixed site marker", StringComparison.OrdinalIgnoreCase) == true);
		}

		foreach (var apparatusReference in new[]
		{
			"primary_production_bloomery_furnace",
			"primary_production_lime_kiln",
			"primary_production_salt_pan",
			"primary_production_mine_windlass",
			"primary_production_furnace_bellows"
		})
		{
			var spec = specs.Single(x => x.StableReference == apparatusReference);
			Assert.AreEqual(0, spec.Components.Count, $"{apparatusReference} should be a static room apparatus prototype.");
			Assert.IsTrue(spec.Tags.Any(x => x.StartsWith("Functions / Tools /", StringComparison.Ordinal)),
				$"{apparatusReference} should be discoverable by functional tool tags.");
			Assert.IsTrue(spec.BuilderNotes?.Contains("room fixture", StringComparison.OrdinalIgnoreCase) == true);
		}
	}

	[TestMethod]
	public void Phase3_PrimaryProductionSpecsUseSeededComponentAndTagDependencies()
	{
		var specs = ItemSeeder.PrimaryProductionItemSpecsForTesting.ToArray();
		var componentSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.ItemComponents.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");

		foreach (var component in specs
			         .SelectMany(x => x.Components)
			         .Where(x => !string.IsNullOrWhiteSpace(x))
			         .Distinct(StringComparer.OrdinalIgnoreCase))
		{
			AssertContains(componentSource, component);
		}

		foreach (var expected in new[]
		{
			"AddTag(context, \"Mining Tool\", \"Tools\")",
			"AddTag(context, \"Prospecting Tool\", \"Tools\")",
			"AddTag(context, \"Quarrying Tool\", \"Tools\")",
			"AddTag(context, \"Smelting Tool\", \"Tools\")",
			"AddTag(context, \"Hauling Tool\", \"Tools\")",
			"AddTag(context, \"Visible Resource Deposit\", \"Primary Production Resource\")",
			"AddTag(context, \"Hematite Resource\", \"Mineral Resource\")",
			"AddTag(context, \"Clay Pit Resource\", \"Clay Resource\")",
			"AddTag(context, \"Brine Spring Resource\", \"Salt Resource\")",
			"AddTag(context, \"Bitumen Seep Resource\", \"Fuel Resource\")"
		})
		{
			AssertContains(tagSource, expected);
		}
	}

	[TestMethod]
	public void Phase4_PrimaryProductionCommodityCraftsAreWiredIntoCraftSeeder()
	{
		var craftSeederSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var primaryProductionSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.PrimaryProduction.cs");

		AssertContains(craftSeederSource, "SeedPrimaryProductionCommodityCrafts();");
		AssertContains(primaryProductionSource, "private void SeedPrimaryProductionCommodityCrafts()");
		AssertContains(primaryProductionSource, "PrimaryProductionCraftSpecsForTesting");
		AssertContains(primaryProductionSource, "PrimaryProductionKnowledge = \"Primary Production - Historic Commodity Work\"");
		AssertContains(primaryProductionSource, "ShouldSeedHistoricCrafts()");
	}

	[TestMethod]
	public void Phase4_PrimaryProductionCraftSpecsCoverRepresentativeChains()
	{
		var specs = ItemSeeder.PrimaryProductionCraftSpecsForTesting.ToArray();
		var names = specs.Select(x => x.Name).ToArray();

		Assert.IsTrue(specs.Length >= 35, "Phase 4 should seed a substantial first-pass commodity craft catalogue.");
		Assert.AreEqual(names.Length, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(specs.All(x => x.Name.StartsWith("primary production - ", StringComparison.Ordinal)));
		Assert.IsTrue(specs.All(x => x.Inputs.Any(y => y.StartsWith("Commodity", StringComparison.Ordinal))));
		Assert.IsTrue(specs.All(x => x.Products.Any(y => y.StartsWith("CommodityProduct", StringComparison.Ordinal))));

		foreach (var expected in new[]
		{
			"primary production - assay iron ore sample",
			"primary production - break raw iron ore",
			"primary production - sort broken iron ore",
			"primary production - wash sorted iron ore",
			"primary production - roast washed iron ore",
			"primary production - break raw copper ore",
			"primary production - roast washed copper ore",
			"primary production - break raw tin ore",
			"primary production - wash sorted tin ore",
			"primary production - break raw lead ore",
			"primary production - roast washed lead ore",
			"primary production - dress rough limestone block",
			"primary production - break rubble into aggregate",
			"primary production - slake quicklime",
			"primary production - mix lime mortar",
			"primary production - calcine gypsum plaster",
			"primary production - temper clay body",
			"primary production - mould green bricks",
			"primary production - mould roof tiles",
			"primary production - prepare refractory clay body",
			"primary production - prepare soda lime glass batch",
			"primary production - consolidate iron bloom",
			"primary production - draw wrought iron bar stock",
			"primary production - cast copper ingot",
			"primary production - cast tin ingot",
			"primary production - alloy bronze billet",
			"primary production - crush rock salt",
			"primary production - leach wood ash",
			"primary production - evaporate lye to potash",
			"primary production - boil tar into pitch",
			"primary production - prepare pitch sealant",
			"primary production - grind ochre pigment",
			"primary production - prepare malachite pigment",
			"primary production - cut dried peat blocks"
		})
		{
			Assert.IsTrue(names.Contains(expected, StringComparer.OrdinalIgnoreCase), $"Missing primary-production craft {expected}.");
		}
	}

	[TestMethod]
	public void Phase4_PrimaryProductionCraftsUseSeededCommodityTagsAndSafeTraitGates()
	{
		var specs = ItemSeeder.PrimaryProductionCraftSpecsForTesting.ToArray();
		var products = specs.SelectMany(x => x.Products).Concat(specs.SelectMany(x => x.FailProducts)).ToArray();
		var tools = specs.SelectMany(x => x.Tools).ToArray();
		var allowedTraits = new[]
		{
			"Labouring",
			"Surviving",
			"Masonry",
			"Pottery",
			"Smelting",
			"Blacksmithing",
			"Glassworking",
			"Painting"
		};

		Assert.IsTrue(specs.All(x => allowedTraits.Contains(x.Trait, StringComparer.OrdinalIgnoreCase)));
		Assert.IsFalse(specs.Any(x => x.Trait.Equals("Mining", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(specs.Any(x => x.Trait.Equals("Saltworking", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(specs.Any(x => x.Trait.Equals("Pigment Processing", StringComparison.OrdinalIgnoreCase)));

		foreach (var expectedTag in new[]
		{
			"Sample Ore Commodity",
			"Broken Ore Commodity",
			"Sorted Ore Commodity",
			"Washed Ore Commodity",
			"Roasted Ore Commodity",
			"Ore Tailings Commodity",
			"Dressed Stone Block Commodity",
			"Aggregate Commodity",
			"Slaked Lime Commodity",
			"Mortar Commodity",
			"Prepared Clay Commodity",
			"Green Brick Commodity",
			"Roof Tile Commodity",
			"Crucible Clay Commodity",
			"Glass Batch Commodity",
			"Metal Billet Commodity",
			"Metal Bar Stock Commodity",
			"Metal Ingot Commodity",
			"Salt Commodity",
			"Lye Commodity",
			"Potash Commodity",
			"Pitch Commodity",
			"Pigment Commodity",
			"Peat Fuel Commodity"
		})
		{
			Assert.IsTrue(products.Any(x => x.Contains($"tag {expectedTag}", StringComparison.Ordinal)),
				$"Missing product or fail-product tag {expectedTag}.");
		}

		foreach (var expectedToolTag in new[]
		{
			"Prospecting Tool",
			"Mining Tool",
			"Hauling Tool",
			"Masonry Tool",
			"Quarrying Tool",
			"Kiln Tool",
			"Smelting Tool",
			"Saltworking Tool",
			"Alkali Tool",
			"Tar Burning Tool",
			"Pigment Processing Tool",
			"Peat Cutting Tool"
		})
		{
			Assert.IsTrue(tools.Any(x => x.Contains($"the {expectedToolTag} tag", StringComparison.Ordinal)),
				$"Missing tool requirement for {expectedToolTag}.");
		}
	}

	[TestMethod]
	public void Phase5_ResourceDiscoveryActionIsRegisteredForProjectTemplates()
	{
		var projectFactorySource = ReadSource("MudSharpCore", "Work", "Projects", "ProjectFactory.cs");
		var discoveryActionSource = ReadSource("MudSharpCore", "Work", "Projects", "Actions", "ResourceDiscoveryProjectAction.cs");

		AssertContains(projectFactorySource, "resourcediscovery");
		AssertContains(projectFactorySource, "new Actions.ResourceDiscoveryProjectAction");
		AssertContains(discoveryActionSource, "public class ResourceDiscoveryProjectAction : BaseAction");
		AssertContains(discoveryActionSource, "RequiredLocationTagId");
		AssertContains(discoveryActionSource, "OutputItemProtoId");
		AssertContains(discoveryActionSource, "DuplicatePreventionTagId");
		AssertContains(discoveryActionSource, "MatchingResourceAlreadyPresent");
	}

	[TestMethod]
	public void Phase5_PrimaryProductionProjectSeederIsDedicatedAndRerunnable()
	{
		var seederSource = ReadSource("DatabaseSeeder", "Seeders", "PrimaryProductionSeeder.cs");
		var metadataSource = ReadSource("DatabaseSeeder", "SeederMetadataRegistry.cs");

		AssertContains(seederSource, "public sealed class PrimaryProductionSeeder : IDatabaseSeeder");
		AssertContains(seederSource, "public int SortOrder => 420;");
		AssertContains(seederSource, "public bool SafeToRunMoreThanOnce => true;");
		AssertContains(seederSource, "Stock Primary Production");
		AssertContains(seederSource, "ProjectActionSeedType.ResourceDiscovery");
		AssertContains(seederSource, "ProjectActionSeedType.CommodityOutput");
		AssertContains(metadataSource, "nameof(PrimaryProductionSeeder)");
		AssertContains(metadataSource, "HasTrait(context, \"Labouring\", \"Labourer\", \"Laboring\", \"Laborer\")");
		AssertContains(metadataSource, "HasTrait(context, \"Masonry\", \"Stonecraft\", \"Stoneworking\", \"Mason\")");
		AssertContains(metadataSource, "HasTrait(context, \"Smelting\", \"Smelter\")");
		AssertContains(metadataSource, "Stock primary-production project content is tracked");
	}

	[TestMethod]
	public void Phase5_PrimaryProductionProjectSpecsCoverProspectingAndProductionChains()
	{
		var specs = PrimaryProductionSeeder.PrimaryProductionProjectSpecsForTesting.ToArray();
		var names = specs.Select(x => x.Name).ToArray();

		Assert.IsTrue(specs.Length >= 30, "Phase 5 should seed a broad first-pass local project template catalogue.");
		Assert.AreEqual(names.Length, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(specs.All(x => x.Name.StartsWith("Stock Primary Production: ", StringComparison.Ordinal)));

		foreach (var expected in new[]
		{
			"Stock Primary Production: Survey Mineral Signs",
			"Stock Primary Production: Prospect for Iron Deposits",
			"Stock Primary Production: Prospect for Tin Deposits",
			"Stock Primary Production: Prospect for Copper Deposits",
			"Stock Primary Production: Prospect for Lead And Silver Deposits",
			"Stock Primary Production: Prospect for Quarry Stone",
			"Stock Primary Production: Prospect for Clay",
			"Stock Primary Production: Prospect for Salt Or Brine",
			"Stock Primary Production: Prospect for Fuel Deposits",
			"Stock Primary Production: Prospect for Alkali Deposits",
			"Stock Primary Production: Prospect for Pigment Earth",
			"Stock Primary Production: Burn a Charcoal Clamp",
			"Stock Primary Production: Extract Iron Ore",
			"Stock Primary Production: Extract Copper Ore",
			"Stock Primary Production: Extract Tin Ore",
			"Stock Primary Production: Extract Lead Ore",
			"Stock Primary Production: Quarry Limestone Blocks",
			"Stock Primary Production: Burn a Lime Kiln",
			"Stock Primary Production: Fire a Brick Clamp",
			"Stock Primary Production: Smelt an Iron Bloom",
			"Stock Primary Production: Smelt Copper Ore",
			"Stock Primary Production: Smelt Tin Ore",
			"Stock Primary Production: Boil Brine For Salt",
			"Stock Primary Production: Burn a Tar Kiln",
			"Stock Primary Production: Cut Peat Turves",
			"Stock Primary Production: Collect Ochre Earth",
			"Stock Primary Production: Mine Coal"
		})
		{
			Assert.IsTrue(names.Contains(expected, StringComparer.OrdinalIgnoreCase), $"Missing seeded project {expected}.");
		}
	}

	[TestMethod]
	public void Phase5_PrimaryProductionProjectsUseExpectedActionAndMaterialPatterns()
	{
		var specs = PrimaryProductionSeeder.PrimaryProductionProjectSpecsForTesting.ToArray();
		var actionTypes = specs.SelectMany(x => x.ActionTypes).ToArray();
		var outputTags = specs.SelectMany(x => x.OutputCommodityTags).ToArray();
		var requiredLocationTags = specs.SelectMany(x => x.RequiredLocationTags).ToArray();
		var outputStableReferences = specs.SelectMany(x => x.OutputStableReferences).ToArray();
		var materialTypes = specs.SelectMany(x => x.MaterialTypes).ToArray();
		var allowedTraits = new[] { "Labouring", "Surviving", "Masonry", "Pottery", "Smelting", "Glassworking", "Painting" };

		Assert.IsTrue(specs.All(x => allowedTraits.Contains(x.LabourTrait, StringComparer.OrdinalIgnoreCase)));
		Assert.IsTrue(actionTypes.Contains("resourcediscovery", StringComparer.OrdinalIgnoreCase));
		Assert.IsTrue(actionTypes.Contains("commodityoutput", StringComparer.OrdinalIgnoreCase));
		Assert.IsFalse(materialTypes.Contains("Simple", StringComparer.OrdinalIgnoreCase), "Durable tools should not be consumed as simple material requirements.");

		foreach (var expected in new[]
		{
			"Hematite Resource",
			"Cassiterite Resource",
			"Malachite Resource",
			"Galena Resource",
			"Limestone Resource",
			"Clay Pit Resource",
			"Brine Spring Resource",
			"Peat Bog Resource",
			"Natron Resource",
			"Ochre Resource",
			"Sulfur Deposit Resource",
			"Bitumen Seep Resource"
		})
		{
			Assert.IsTrue(requiredLocationTags.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Missing required location tag {expected}.");
		}

		foreach (var expected in new[]
		{
			"primary_production_hematite_deposit",
			"primary_production_cassiterite_deposit",
			"primary_production_malachite_deposit",
			"primary_production_galena_deposit",
			"primary_production_limestone_outcrop",
			"primary_production_clay_bank",
			"primary_production_brine_spring",
			"primary_production_peat_bog",
			"primary_production_natron_flat",
			"primary_production_ochre_bank",
			"primary_production_sulfur_deposit",
			"primary_production_bitumen_seep"
		})
		{
			Assert.IsTrue(outputStableReferences.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Missing discovery output stable reference {expected}.");
		}

		foreach (var expected in new[]
		{
			"Sample Ore Commodity",
			"Raw Ore Commodity",
			"Mine Spoil Commodity",
			"Rough Stone Block Commodity",
			"Stone Rubble Commodity",
			"Quicklime Commodity",
			"Fired Brick Commodity",
			"Bloom Commodity",
			"Slag Commodity",
			"Metal Ingot Commodity",
			"Salt Commodity",
			"Soda Ash Commodity",
			"Glass Blank Commodity",
			"Tar Commodity",
			"Peat Fuel Commodity",
			"Pigment Commodity",
			"Coal Fuel Commodity"
		})
		{
			Assert.IsTrue(outputTags.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Missing commodity output tag {expected}.");
		}
	}

	[TestMethod]
	public void Phase6_SiteCreationRemainsBuilderConfiguredAndDocumented()
	{
		var seederSource = ReadSource("DatabaseSeeder", "Seeders", "PrimaryProductionSeeder.cs");
		var projectWorkflowDoc = ReadSource("Design Documents", "Crafting", "Projects_System_Builder_Workflows.md");
		var designReference = ReadSource("Design Documents", "Seeding", "Primary_Production_Seeder_Design_Reference.md");
		var createCellSource = ReadSource("MudSharpCore", "FutureProg", "Functions", "Location", "CreateCell.cs");
		var linkCellsSource = ReadSource("MudSharpCore", "FutureProg", "Functions", "Location", "Exits", "LinkCells.cs");
		var progActionSource = ReadSource("MudSharpCore", "Work", "Projects", "Actions", "ProgAction.cs");

		AssertContains(createCellSource, "\"CreateCell\".ToLowerInvariant()");
		AssertContains(createCellSource, "\"CreateRoom\".ToLowerInvariant()");
		AssertContains(linkCellsSource, "\"LinkCells\".ToLowerInvariant()");
		AssertContains(progActionSource, "new[] { ProgVariableTypes.Project }");

		AssertContains(projectWorkflowDoc, "Optional site-creation completion progs");
		AssertContains(projectWorkflowDoc, "CreateOverlay(builder, name)");
		AssertContains(projectWorkflowDoc, "CreateCell(package, zone, template)");
		AssertContains(projectWorkflowDoc, "LinkCells(origin, destination, package, direction)");
		AssertContains(projectWorkflowDoc, "ApproveOverlay(package, builder, comment)");

		AssertContains(designReference, "Phase 6 Optional Site-Creation Progs - Completed 2026-06-18");
		AssertContains(designReference, "Site-creation remains a builder-configured optional workflow");
		Assert.IsFalse(seederSource.Contains("CreateCell", StringComparison.Ordinal),
			"The stock primary-production seeder should not hard-code topology-changing FutureProg creation.");
		Assert.IsFalse(seederSource.Contains("CreateRoom", StringComparison.Ordinal),
			"The stock primary-production seeder should not hard-code topology-changing FutureProg creation.");
		Assert.IsFalse(seederSource.Contains("LinkCells", StringComparison.Ordinal),
			"The stock primary-production seeder should not hard-code topology-changing FutureProg creation.");
	}

	[TestMethod]
	public void Phase7_FinalIntegrationDocsNameSeederAndCrossReferences()
	{
		var designReference = ReadSource("Design Documents", "Seeding", "Primary_Production_Seeder_Design_Reference.md");
		var repeatabilityStrategy = ReadSource("Design Documents", "Seeding", "DatabaseSeeder_Repeatability_Strategy.md");
		var projectWorkflowDoc = ReadSource("Design Documents", "Crafting", "Projects_System_Builder_Workflows.md");
		var projectOverviewDoc = ReadSource("Design Documents", "Crafting", "Projects_System_Overview.md");
		var metadataSource = ReadSource("DatabaseSeeder", "SeederMetadataRegistry.cs");

		AssertContains(designReference, "Phase 7 Documentation And Integration - Completed 2026-06-18");
		AssertContains(designReference, "`PrimaryProductionSeeder`");
		AssertContains(designReference, "`Stock Primary Production: `");
		AssertContains(designReference, "`Primary Production - Historic Commodity Work`");
		AssertContains(designReference, "Agriculture woodland outputs");
		AssertContains(designReference, "downstream antiquity and medieval");
		AssertContains(designReference, "Future world-starter");
		AssertContains(designReference, "resource-site tags on real cells");

		AssertContains(repeatabilityStrategy, "`PrimaryProductionSeeder` | 420");
		AssertContains(repeatabilityStrategy, "stock primary-production local projects");
		AssertContains(repeatabilityStrategy, "`PrimaryProductionSeeder`");
		AssertContains(metadataSource, "nameof(PrimaryProductionSeeder)");
		AssertContains(metadataSource, "Stock primary-production project content is tracked");

		AssertContains(projectWorkflowDoc, "`resourcediscovery`");
		AssertContains(projectWorkflowDoc, "Optional site-creation completion progs");
		AssertContains(projectOverviewDoc, "`resourcediscovery` | `ResourceDiscoveryProjectAction`");
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
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
}
