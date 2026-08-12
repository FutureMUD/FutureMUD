#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialFoodCatalogueTests
{
	private const int ExpectedItemCount = 2775;
	private const int ExpectedLiquidCount = 225;
	private static readonly double[] StandardAlcoholValues =
		[0.0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.08, 0.1, 0.12, 0.15, 0.18, 0.2, 0.25, 0.3, 0.4, 0.5];
	private static readonly double[] StandardWaterValues =
		[0.0, 0.1, 0.25, 0.5, 0.65, 0.75, 0.85, 0.9, 0.95, 1.0];
	private static readonly double[] StandardSatiationValues =
		[0.0, 0.25, 0.5, 1.0, 1.5, 2.0, 3.0, 4.0, 5.0, 6.0];
	private static readonly double[] StandardThirstValues =
		[0.0, 0.25, 0.5, 1.0, 1.5, 2.0, 3.0, 4.0];

	[TestMethod]
	public void Catalogue_HasExactThreeThousandRecordAllocation()
	{
		var items = ItemSeeder.PreIndustrialFoodItemsForTesting;
		var liquids = ItemSeeder.PreIndustrialFoodLiquidsForTesting;

		Assert.AreEqual(ExpectedItemCount, items.Count);
		Assert.AreEqual(ExpectedLiquidCount, liquids.Count);
		Assert.AreEqual(3000, items.Count + liquids.Count);

		AssertScopeCount(items, liquids, FoodCatalogueScope.Shared, 2100, 150);
		AssertScopeCount(items, liquids, FoodCatalogueScope.Medieval, 225, 25);
		AssertScopeCount(items, liquids, FoodCatalogueScope.Renaissance, 225, 25);
		AssertScopeCount(items, liquids, FoodCatalogueScope.EarlyModern, 225, 25);
	}

	[TestMethod]
	public void Catalogue_StableReferencesAndPlayerFacingProseAreUniqueAndAuthored()
	{
		var items = ItemSeeder.PreIndustrialFoodItemsForTesting;
		var liquids = ItemSeeder.PreIndustrialFoodLiquidsForTesting;

		AssertUnique(items.Select(x => x.StableReference), "item stable reference");
		AssertUnique(liquids.Select(x => x.StableReference), "liquid stable reference");
		AssertUnique(
			items.Select(x => x.StableReference).Concat(liquids.Select(x => x.StableReference)),
			"catalogue stable reference");
		AssertUnique(items.Select(x => x.ShortDescription), "item short description");
		AssertUnique(items.Select(x => NormaliseProse(x.FullDescription)), "item full description");
		AssertUnique(items.Where(x => x.Kind == FoodCatalogueKind.Prepared)
			.Select(x => NormaliseProse(x.Taste)), "item taste description");
		AssertUnique(liquids.Select(x => x.Name), "liquid name");
		AssertUnique(liquids.Select(x => NormaliseProse(x.LongDescription)), "liquid long description");
		AssertUnique(liquids.Select(x => NormaliseProse(x.Taste)), "liquid taste description");
		AssertUnique(liquids.Select(x => NormaliseProse(x.Smell)), "liquid smell description");
		AssertNoRepeatedScaffolds(items.Select(x => x.FullDescription), "item full descriptions");
		AssertNoRepeatedScaffolds(items.Where(x => x.Kind == FoodCatalogueKind.Prepared).Select(x => x.Taste),
			"item taste descriptions");
		AssertNoRepeatedScaffolds(liquids.Select(x => x.LongDescription), "liquid long descriptions");
		AssertNoRepeatedScaffolds(liquids.Select(x => x.Taste), "liquid taste descriptions");

		foreach (var item in items)
		{
			Assert.IsTrue(item.FullDescription.Length >= 45,
				$"{item.StableReference} needs a substantive hand-authored full description.");
			Assert.IsFalse(item.FullDescription.Contains('$') || item.FullDescription.Contains('{'),
				$"{item.StableReference} contains description-template markup.");
			Assert.IsFalse(item.ShortDescription.Contains('\t') || item.FullDescription.Contains('\t'),
				$"{item.StableReference} contains an embedded tab.");
			if (item.Kind == FoodCatalogueKind.Prepared)
			{
				Assert.IsTrue(item.Taste.Length >= 20,
					$"{item.StableReference} needs an authored taste description.");
			}
		}

		foreach (var liquid in liquids)
		{
			Assert.IsTrue(liquid.LongDescription.Length >= 45);
			Assert.IsTrue(liquid.Taste.Length >= 20);
			Assert.IsTrue(liquid.Smell.Length >= 15);
			Assert.IsFalse(liquid.Description.Contains('$') || liquid.LongDescription.Contains('$') ||
			               liquid.Description.Contains('{') || liquid.LongDescription.Contains('{'),
				$"{liquid.StableReference} contains description-template markup.");
			Assert.IsNotNull(Telnet.GetColour(liquid.Colour),
				$"{liquid.StableReference} has invalid ANSI display colour {liquid.Colour}.");
		}
	}

	[TestMethod]
	public void Catalogue_NounsAreConciseGrammaticalHeads()
	{
		var items = ItemSeeder.PreIndustrialFoodItemsForTesting;
		Assert.IsTrue(items.All(x => Regex.IsMatch(x.Noun, @"^[\p{L}]+(?:['-][\p{L}]+)*$")),
			"Food catalogue nouns must be single lexical head nouns rather than phrases or stable-reference slugs.");
		Assert.AreEqual("pottage", items.Single(x =>
			x.StableReference == "preindustrial_food_root_and_fish_pottage").Noun);
		Assert.AreEqual("cluster", items.Single(x =>
			x.StableReference == "preindustrial_food_fresh_date_cluster").Noun);
		Assert.AreEqual("bowl", items.Single(x =>
			x.StableReference == "preindustrial_food_jellied_eel_bowl").Noun);

		var normalizer = ReadSource("scripts", "normalise-preindustrial-food-nouns.py");
		StringAssert.Contains(normalizer, "def head_noun(short_description: str)");
		StringAssert.Contains(normalizer, "--check");
	}

	[TestMethod]
	public void Catalogue_UsesMaintainedMaterialsAndStandardNutritionQualityPolicies()
	{
		var materials = ReadMaterialCatalogue();
		foreach (var item in ItemSeeder.PreIndustrialFoodItemsForTesting)
		{
			Assert.IsTrue(materials.Contains(item.Material),
				$"{item.StableReference} uses unmaintained material {item.Material}.");
			Assert.IsTrue(double.IsFinite(item.WeightInGrams) && item.WeightInGrams > 0.0,
				$"{item.StableReference} has an invalid item weight.");
			Assert.IsTrue(item.Cost >= 0.0m, $"{item.StableReference} has an invalid cost.");

			if (item.Kind == FoodCatalogueKind.Prepared)
			{
				Assert.AreNotEqual(FoodNutritionBand.None, item.Nutrition);
				Assert.AreNotEqual(FoodFreshnessBand.None, item.Freshness);
			}
			else
			{
				Assert.AreEqual(FoodNutritionBand.None, item.Nutrition);
				Assert.AreEqual(FoodFreshnessBand.None, item.Freshness);
				Assert.IsTrue(string.IsNullOrWhiteSpace(item.Taste));
			}

			if (item.Nutrition is FoodNutritionBand.BleakThin or FoodNutritionBand.BleakSolid)
			{
				Assert.IsTrue(item.Quality <= ItemQuality.Standard,
					$"{item.StableReference} is bleak food above Standard quality.");
			}

			if (item.Nutrition is FoodNutritionBand.Rich or FoodNutritionBand.Feast)
			{
				Assert.IsTrue(item.Quality > ItemQuality.Standard,
					$"{item.StableReference} is rich food without above-Standard quality.");
			}
		}

		foreach (var liquid in ItemSeeder.PreIndustrialFoodLiquidsForTesting)
		{
			CollectionAssert.Contains(StandardAlcoholValues, liquid.AlcoholLitresPerLitre);
			CollectionAssert.Contains(StandardWaterValues, liquid.WaterLitresPerLitre);
			CollectionAssert.Contains(StandardSatiationValues, liquid.FoodSatiatedHoursPerLitre);
			CollectionAssert.Contains(StandardThirstValues, liquid.DrinkSatiatedHoursPerLitre);
		}
	}

	[TestMethod]
	public void PreparedFoodDefinitions_PreserveAuthoredItemDescriptionsAtRuntime()
	{
		foreach (var item in ItemSeeder.PreIndustrialFoodItemsForTesting
			         .Where(x => x.Kind == FoodCatalogueKind.Prepared))
		{
			var definition = XElement.Parse(
				ItemSeeder.PreIndustrialPreparedFoodDefinitionForTesting(item));
			Assert.AreEqual(string.Empty, definition.Element("Short")?.Value,
				$"{item.StableReference} overrides its authored short description.");
			Assert.AreEqual(string.Empty, definition.Element("Full")?.Value,
				$"{item.StableReference} overrides its authored full description.");
			Assert.AreEqual(item.Taste, definition.Element("Taste")?.Value);
		}
	}

	[TestMethod]
	public void Catalogue_SharedAndEraSpecificRowsFollowIdentityAndAdmissionPolicy()
	{
		foreach (var (stableReference, scope, admissionProfile) in ItemSeeder.PreIndustrialFoodItemsForTesting
			         .Select(x => (x.StableReference, x.Scope, x.AdmissionProfile))
			         .Concat(ItemSeeder.PreIndustrialFoodLiquidsForTesting
				         .Select(x => (x.StableReference, x.Scope, x.AdmissionProfile))))
		{
			var expectedPrefix = scope switch
			{
				FoodCatalogueScope.Shared => "preindustrial_food_",
				FoodCatalogueScope.Medieval => "medieval_food_",
				FoodCatalogueScope.Renaissance => "renaissance_food_",
				FoodCatalogueScope.EarlyModern => "earlymodern_food_",
				_ => throw new ArgumentOutOfRangeException()
			};
			StringAssert.StartsWith(stableReference, expectedPrefix);
			Assert.IsTrue(Regex.IsMatch(stableReference, "^[a-z0-9_]+$"));

			if (scope == FoodCatalogueScope.Shared)
			{
				Assert.AreNotEqual(FoodAdmissionProfile.EraSpecific, admissionProfile);
			}
			else
			{
				Assert.AreEqual(FoodAdmissionProfile.EraSpecific, admissionProfile);
			}
		}
	}

	[TestMethod]
	public void ItemSeeder_DispatchesSharedAndEraSpecificFoodCatalogues()
	{
		var dispatcher = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.cs");
		StringAssert.Contains(dispatcher, "SeedSharedPreIndustrialFoodCatalogue();");
		StringAssert.Contains(dispatcher, "SeedMedievalFoodCatalogue();");

		var renaissance = ReadSource(
			"DatabaseSeeder",
			"Seeders",
			"ItemSeeder.Renaissance.AgricultureFoodDrinkCommodities.cs");
		StringAssert.Contains(renaissance, "SeedRenaissanceFoodCatalogue();");

		var earlyModern = ReadSource(
			"DatabaseSeeder",
			"Seeders",
			"ItemSeeder.EarlyModern.AgricultureFoodDrinkCommodities.cs");
		StringAssert.Contains(earlyModern, "SeedEarlyModernFoodCatalogue();");
	}

	[TestMethod]
	public void ExistingStockFoodLiquids_AreReusedWithoutTakingOwnership()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialFoodCatalogue.cs");
		StringAssert.Contains(source, "var hasExistingLiquid = _liquids.TryGetValue(entry.Name, out var existingLiquid);");
		StringAssert.Contains(source, "if (hasExistingLiquid)");
		StringAssert.Contains(source,
			"FindManagedRecord(manifestEntry.EntityType, manifestEntry.StableKey) is null");
		StringAssert.Contains(source, "Core and Kickstart seeders own their stock liquids.");
	}

	[TestMethod]
	public void MaintainedExports_CoverPreparedComponentsLiquidsAndTags()
	{
		var items = ItemSeeder.PreIndustrialFoodItemsForTesting;
		var liquids = ItemSeeder.PreIndustrialFoodLiquidsForTesting;
		using var componentDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var actualComponents = componentDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString())
			.Where(x => x?.StartsWith("PreparedFood_Catalogue_", StringComparison.OrdinalIgnoreCase) == true)
			.Select(x => x!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(componentDocument.RootElement
			.EnumerateArray()
			.Any(x => x.GetProperty("Component Name").GetString() == "Holdable"),
			"Food export synchronisation must preserve pre-existing item components.");
		var expectedComponents = items
			.Where(x => x.Kind == FoodCatalogueKind.Prepared)
			.Select(x => $"PreparedFood_Catalogue_{x.StableReference}")
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		AssertSetsEqual(expectedComponents, actualComponents, "prepared-food component export");

		using var liquidDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Liquids.json"));
		var exportedLiquids = liquidDocument.RootElement
			.EnumerateArray()
			.ToDictionary(
				x => x.GetProperty("Liquid Name").GetString()!,
				x => x.GetProperty("Tags")
					.EnumerateArray()
					.Select(y => y.GetString()!)
					.ToHashSet(StringComparer.OrdinalIgnoreCase),
				StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(exportedLiquids.ContainsKey("water"),
			"Food export synchronisation must preserve pre-existing liquids.");
		var catalogueLiquidNames = exportedLiquids
			.Where(x => x.Value.Any(tag => tag.StartsWith(
				"Food and Drink / Food Liquids / Pre-Industrial Catalogue",
				StringComparison.OrdinalIgnoreCase)))
			.Select(x => x.Key)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		AssertSetsEqual(
			liquids.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase),
			catalogueLiquidNames,
			"food-liquid export");
		foreach (var liquid in liquids)
		{
			Assert.IsTrue(exportedLiquids.TryGetValue(liquid.Name, out var tags),
				$"Seeded_Liquids.json is missing {liquid.Name}.");
			Assert.IsTrue(tags.Contains(
				$"Food and Drink / Food Liquids / Pre-Industrial Catalogue / Scope / {ScopeDisplay(liquid.Scope)}"));
			Assert.IsTrue(tags.Contains(
				$"Food and Drink / Food Liquids / Pre-Industrial Catalogue / Family / {FamilyDisplay(liquid.Family)}"));
		}

		var tagPaths = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv")
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Skip(1)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length == 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var path in ExpectedTagPaths(items, liquids))
		{
			Assert.IsTrue(tagPaths.Contains(path), $"SeededTagHierarchy.csv is missing {path}.");
		}
	}

	[TestMethod]
	public void SharedAdmissionManifests_ExactlyMatchSharedCatalogue()
	{
		var expected = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Where(x => x.Scope == FoodCatalogueScope.Shared)
			.Select(x => x.StableReference)
			.Concat(ItemSeeder.PreIndustrialFoodLiquidsForTesting
				.Where(x => x.Scope == FoodCatalogueScope.Shared)
				.Select(x => x.StableReference))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var file in new[]
		         {
			         "FutureMUD_Medieval_Shared_Food_Admission_Manifest.md",
			         "FutureMUD_Renaissance_Shared_Food_Admission_Manifest.md",
			         "FutureMUD_EarlyModern_Shared_Food_Admission_Manifest.md"
		         })
		{
			var actual = Regex.Matches(
					ReadSource("Design Documents", "Seeding", file),
					@"^\| `(?<reference>[a-z0-9_]+)` \|",
					RegexOptions.Multiline)
				.Cast<Match>()
				.Select(x => x.Groups["reference"].Value)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			AssertSetsEqual(expected, actual, file);
		}
	}

	[TestMethod]
	public void CatalogueCrafts_GroupOutputsThroughTheCookedFoodSelectorContract()
	{
		var specs = ItemSeeder.PreIndustrialFoodCatalogueCraftSpecsForTesting.ToArray();
		var outputs = specs.SelectMany(x => x.Products).ToArray();
		var expected = ItemSeeder.PreIndustrialFoodCatalogueOutputContractsForTesting.ToArray();

		Assert.AreEqual(547, specs.Length);
		Assert.AreEqual(322, specs.Count(x => x.Products.All(y => !y.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase))));
		Assert.AreEqual(225, specs.Count(x => x.Products.Any(y => y.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase))));
		Assert.IsTrue(specs.Length < expected.Length,
			"The catalogue should use generalized selector crafts instead of one craft per output.");
		Assert.AreEqual(expected.Length, outputs.Length);
		CollectionAssert.AreEquivalent(expected, outputs);
		Assert.AreEqual(outputs.Length, outputs.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(specs.All(x => x.Inputs.Count > 0));
		Assert.IsTrue(specs.All(x => x.SourceOwnership.Count > 0));
		var catalogueItemReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var itemOutputReferences = outputs
			.Where(x => !x.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		AssertSetsEqual(catalogueItemReferences, itemOutputReferences, "catalogue item craft outputs");
		var catalogueLiquidReferences = ItemSeeder.PreIndustrialFoodLiquidsForTesting
			.Select(x => $"liquid:{x.StableReference}:{x.Name}:10")
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var liquidOutputReferences = outputs
			.Where(x => x.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		AssertSetsEqual(catalogueLiquidReferences, liquidOutputReferences, "catalogue liquid craft outputs");
		var knownStableReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var input in specs.SelectMany(x => x.Dependencies))
		{
			Assert.IsTrue(
				new[] { "agriculture", "animal_butchery" }
					.Contains(input.SourceOwner, StringComparer.Ordinal),
				$"Unknown catalogue input source owner {input.SourceOwner}.");
			Assert.AreEqual(0, input.SourcePhase);
			if (input.StableReference is not null)
			{
				Assert.IsTrue(knownStableReferences.Contains(input.StableReference));
			}
		}
		foreach (var input in specs.SelectMany(x => x.Dependencies)
			         .Where(x => x.SourceOwner == "agriculture" &&
			                     x.Import.StartsWith("CommodityTag", StringComparison.OrdinalIgnoreCase)))
		{
			Assert.IsTrue(
				input.Import.Contains("piletag Seeded Yield", StringComparison.Ordinal) ||
				input.Import.Contains("piletag Raw Milk", StringComparison.Ordinal) ||
				input.Import.Contains("piletag Egg Product", StringComparison.Ordinal) ||
				input.Import.Contains("piletag Pressed Honey", StringComparison.Ordinal),
				$"Agriculture input {input.Import} does not use a seeded-yield or animal-product pile tag.");
		}
		Assert.IsTrue(specs.Where(x => x.Products.Any(y => y.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase)))
			.All(x => x.SourceOwnership.Contains("preindustrial_food_liquid_vessel", StringComparer.Ordinal)));
		Assert.IsTrue(specs.Where(x => x.Products.Any(y => !y.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase)))
			.All(x => x.Tools.Count > 0));

		var treeNutReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Where(x => x.Material.Equals("tree nut", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(specs.Where(x => x.Products.Any(treeNutReferences.Contains))
			.All(x => x.Inputs.Any(y => y.Contains("Food Crop", StringComparison.Ordinal))));

		var chickpeaReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Where(x => x.Material.Equals("chickpea", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(specs.Where(x => x.Products.Any(chickpeaReferences.Contains))
			.All(x => x.Inputs.Any(y => y.Contains("Food Crop", StringComparison.Ordinal))));

		var honeyReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Where(x => x.Material.Equals("honey", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(specs.Where(x => x.Products.Any(honeyReferences.Contains))
			.All(x => x.Inputs.Any(y => y.Contains("Pressed Honey", StringComparison.Ordinal))));

		Assert.IsTrue(specs.Where(x => x.Products.Any(y => y.StartsWith("liquid:", StringComparison.OrdinalIgnoreCase)))
			.Where(x => x.Family is FoodCatalogueFamily.GrainDrink or FoodCatalogueFamily.FermentedDrink or
				FoodCatalogueFamily.Wine or FoodCatalogueFamily.Spirit)
			.All(x => x.Tools.Contains("TagTool - InRoom - an item with the Brew Copper tag", StringComparer.Ordinal)));
	}

	private static void AssertScopeCount(
		IReadOnlyCollection<PreIndustrialFoodItemCatalogueEntry> items,
		IReadOnlyCollection<PreIndustrialFoodLiquidCatalogueEntry> liquids,
		FoodCatalogueScope scope,
		int expectedItems,
		int expectedLiquids)
	{
		Assert.AreEqual(expectedItems, items.Count(x => x.Scope == scope), $"{scope} item count");
		Assert.AreEqual(expectedLiquids, liquids.Count(x => x.Scope == scope), $"{scope} liquid count");
	}

	private static void AssertUnique(IEnumerable<string> values, string label)
	{
		var array = values.ToArray();
		Assert.AreEqual(
			array.Length,
			array.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			$"Duplicate {label}.");
	}

	private static void AssertSetsEqual(
		IReadOnlySet<string> expected,
		IReadOnlySet<string> actual,
		string label)
	{
		var missing = expected.Except(actual, StringComparer.OrdinalIgnoreCase).Take(5).ToArray();
		var extra = actual.Except(expected, StringComparer.OrdinalIgnoreCase).Take(5).ToArray();
		Assert.IsTrue(
			missing.Length == 0 && extra.Length == 0 && expected.Count == actual.Count,
			$"{label} mismatch. Missing: {string.Join(", ", missing)}. Extra: {string.Join(", ", extra)}.");
	}

	private static string NormaliseProse(string value)
	{
		return Regex.Replace(value.Trim().ToLowerInvariant(), @"\s+", " ");
	}

	private static void AssertNoRepeatedScaffolds(IEnumerable<string> values, string label)
	{
		const int ngramLength = 6;
		const int maximumOccurrences = 8;
		var repeated = values
			.SelectMany(value =>
			{
				var words = Regex.Matches(value.ToLowerInvariant(), @"[a-z]+(?:'[a-z]+)?")
					.Cast<Match>()
					.Select(x => x.Value)
					.ToArray();
				return Enumerable.Range(0, Math.Max(0, words.Length - ngramLength + 1))
					.Select(index => string.Join(" ", words.Skip(index).Take(ngramLength)))
					.Distinct();
			})
			.GroupBy(x => x, StringComparer.Ordinal)
			.Where(x => x.Count() > maximumOccurrences)
			.OrderByDescending(x => x.Count())
			.FirstOrDefault();

		Assert.IsNull(repeated,
			$"{label} repeat the six-word scaffold '{repeated?.Key}' {repeated?.Count()} times.");
	}

	private static HashSet<string> ReadMaterialCatalogue()
	{
		using var document = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		return document.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static IEnumerable<string> ExpectedTagPaths(
		IEnumerable<PreIndustrialFoodItemCatalogueEntry> items,
		IEnumerable<PreIndustrialFoodLiquidCatalogueEntry> liquids)
	{
		foreach (var item in items)
		{
			var root = item.Kind == FoodCatalogueKind.Prepared
				? "Food and Drink / Prepared Foods / Pre-Industrial Catalogue"
				: "Materials / Food Products / Pre-Industrial Food Commodities";
			yield return $"{root} / Scope / {ScopeDisplay(item.Scope)}";
			yield return $"{root} / Family / {FamilyDisplay(item.Family)}";
			if (item.Kind == FoodCatalogueKind.Prepared)
			{
				var register = item.Quality switch
				{
					<= ItemQuality.Substandard => "Bleak",
					ItemQuality.Standard => "Ordinary",
					_ => "Rich"
				};
				yield return $"{root} / Social Register / {register}";
			}
		}

		foreach (var liquid in liquids)
		{
			const string root = "Food and Drink / Food Liquids / Pre-Industrial Catalogue";
			yield return $"{root} / Scope / {ScopeDisplay(liquid.Scope)}";
			yield return $"{root} / Family / {FamilyDisplay(liquid.Family)}";
		}
	}

	private static string ScopeDisplay(FoodCatalogueScope scope)
	{
		return scope == FoodCatalogueScope.EarlyModern ? "Early Modern" : scope.ToString();
	}

	private static string FamilyDisplay(FoodCatalogueFamily family)
	{
		return Regex.Replace(family.ToString(), "([a-z])([A-Z])", "$1 $2");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray()));
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
