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
public class ItemSeederRenaissanceEarlyModernScaffoldingTests
{
	private static readonly IReadOnlyDictionary<string, string> RenaissanceBranches =
		new Dictionary<string, string>
		{
			["FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md"] = "SeedRenaissanceSharedBaselineAdmissionManifest",
			["FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md"] = "SeedRenaissanceClothingAndAccessories",
			["FutureMUD_Renaissance_Military_Firearms_Armour_Design_Reference.md"] = "SeedRenaissanceMilitaryFirearmsAndArmour",
			["FutureMUD_Renaissance_Writing_Print_Administration_Design_Reference.md"] = "SeedRenaissanceWritingPrintAndAdministration",
			["FutureMUD_Renaissance_Household_Urban_Trade_Design_Reference.md"] = "SeedRenaissanceHouseholdUrbanAndTrade",
			["FutureMUD_Renaissance_Art_Craft_Science_Navigation_Design_Reference.md"] = "SeedRenaissanceArtCraftScienceAndNavigation",
			["FutureMUD_Renaissance_Agriculture_Food_Drink_Commodities_Design_Reference.md"] = "SeedRenaissanceAgricultureFoodDrinkAndCommodities",
			["FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md"] = "SeedRenaissancePrimaryIndustryAndUsefulSeederImpacts",
			["FutureMUD_Renaissance_Jewellery_Devotional_Seeder_Design_Reference.md"] = "SeedRenaissanceJewelleryAndDevotionalGoods",
			["FutureMUD_Renaissance_Doors_Locks_Gates_Seeder_Design_Reference.md"] = "SeedRenaissanceDoorsLocksAndGates",
			["FutureMUD_Renaissance_Culture_Manifest_Reference.md"] = "SeedRenaissanceCultureManifest"
		};

	private static readonly IReadOnlyDictionary<string, string> EarlyModernBranches =
		new Dictionary<string, string>
		{
			["FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md"] = "SeedEarlyModernSharedBaselineAdmissionManifest",
			["FutureMUD_EarlyModern_Clothing_Accessories_Design_Reference.md"] = "SeedEarlyModernClothingAndAccessories",
			["FutureMUD_EarlyModern_Military_Firearms_Uniforms_Naval_Design_Reference.md"] = "SeedEarlyModernMilitaryFirearmsUniformsAndNaval",
			["FutureMUD_EarlyModern_Writing_Print_Administration_Finance_Design_Reference.md"] = "SeedEarlyModernWritingPrintAdministrationAndFinance",
			["FutureMUD_EarlyModern_Household_Coffeehouse_Tavern_Trade_Design_Reference.md"] = "SeedEarlyModernHouseholdCoffeehouseTavernAndTrade",
			["FutureMUD_EarlyModern_Science_Navigation_Optics_Measurement_Design_Reference.md"] = "SeedEarlyModernScienceNavigationOpticsAndMeasurement",
			["FutureMUD_EarlyModern_Agriculture_Food_Drink_Commodities_Design_Reference.md"] = "SeedEarlyModernAgricultureFoodDrinkAndCommodities",
			["FutureMUD_EarlyModern_PrimaryIndustry_UsefulSeeder_Impact_Reference.md"] = "SeedEarlyModernPrimaryIndustryAndUsefulSeederImpacts",
			["FutureMUD_EarlyModern_Jewellery_Devotional_Seeder_Design_Reference.md"] = "SeedEarlyModernJewelleryAndDevotionalGoods",
			["FutureMUD_EarlyModern_Doors_Locks_Gates_Seeder_Design_Reference.md"] = "SeedEarlyModernDoorsLocksAndGates",
			["FutureMUD_EarlyModern_Culture_Manifest_Reference.md"] = "SeedEarlyModernCultureManifest"
		};

	[TestMethod]
	public void RecommendedBranchReferences_HaveMatchingInvokedItemSeederStubs()
	{
		var dispatcher = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.cs");
		AssertBranches(RenaissanceBranches, dispatcher);
		AssertBranches(EarlyModernBranches, dispatcher);
	}

	[TestMethod]
	public void GeneratedHouseholdAndMilitaryReferencesAreProductFocused()
	{
		var renaissanceReferences = ItemSeeder.RenaissanceHouseholdItemSpecsForTesting
			.Select(x => x.StableReference)
			.ToArray();
		var earlyModernReferences = ItemSeeder.EarlyModernSupportedMilitaryItemSpecsForTesting
			.Select(x => x.StableReference)
			.ToArray();

		Assert.AreEqual(renaissanceReferences.Length, renaissanceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(earlyModernReferences.Length, earlyModernReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsFalse(renaissanceReferences.Any(x => x.Contains("_expansion_", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(earlyModernReferences.Any(x => x.Contains("_naval_naval_", StringComparison.OrdinalIgnoreCase)));
		CollectionAssert.Contains(renaissanceReferences, "renaissance_furniture_nac_lamp_table_02");
		CollectionAssert.Contains(earlyModernReferences, "earlymodern_military_naval_rope_coil_issue");
	}

	[TestMethod]
	public void EraItemStableReferencesAvoidProcessProvenanceAndRepeatedSegments()
	{
		var stableReferences = Directory
			.GetFiles(SourcePath("DatabaseSeeder", "Seeders"), "ItemSeeder*.cs")
			.SelectMany(File.ReadLines)
			.SelectMany(line => Regex.Matches(line, "\"((?:antiquity|medieval|renaissance|earlymodern)_[a-z0-9_]+)\"", RegexOptions.IgnoreCase)
				.Select(match => match.Groups[1].Value))
			.ToArray();

		Assert.IsTrue(stableReferences.Length > 0);
		Assert.IsFalse(stableReferences.Any(reference =>
			reference.Contains("_expansion_", StringComparison.OrdinalIgnoreCase) ||
			reference.Contains("_rework_", StringComparison.OrdinalIgnoreCase) ||
			reference.Contains("_pass_", StringComparison.OrdinalIgnoreCase) ||
			reference.Contains("_content_pass_", StringComparison.OrdinalIgnoreCase)),
			"Item stable references must identify the product, not the design or content pass that introduced it.");
		Assert.IsFalse(stableReferences.Any(HasRepeatedReferenceSegment),
			"Item stable references must not repeat an adjacent package or culture segment.");
	}

	private static bool HasRepeatedReferenceSegment(string stableReference)
	{
		var segments = stableReference.Split('_');
		return segments.Zip(segments.Skip(1), (first, second) =>
			string.Equals(first, second, StringComparison.OrdinalIgnoreCase)).Any(x => x);
	}

	private static void AssertBranches(IReadOnlyDictionary<string, string> branches, string dispatcher)
	{
		foreach (var (document, method) in branches)
		{
			Assert.IsTrue(File.Exists(SourcePath("Design Documents", "Seeding", document)),
				$"Missing recommended design reference {document}.");
			StringAssert.Contains(dispatcher, $"{method}();",
				$"The era dispatcher does not invoke {method}.");

			var methodFile = FindSeederFileContaining(method);
			Assert.IsNotNull(methodFile, $"No ItemSeeder partial declares {method}.");
			var methodSource = File.ReadAllText(methodFile);
			StringAssert.Contains(methodSource, $"private void {method}()",
				$"The ItemSeeder partial does not declare the expected {method} stub.");
		}
	}

	private static string? FindSeederFileContaining(string method)
	{
		foreach (var file in Directory.GetFiles(SourcePath("DatabaseSeeder", "Seeders"), "ItemSeeder.*.cs"))
		{
			if (File.ReadAllText(file).Contains($"private void {method}()", StringComparison.Ordinal))
			{
				return file;
			}
		}

		return null;
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
