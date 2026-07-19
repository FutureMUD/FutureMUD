#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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
			["FutureMUD_EarlyModern_Culture_Manifest_Reference.md"] = "SeedEarlyModernCultureManifest"
		};

	[TestMethod]
	public void RecommendedBranchReferences_HaveMatchingInvokedItemSeederStubs()
	{
		var dispatcher = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.cs");
		AssertBranches(RenaissanceBranches, dispatcher);
		AssertBranches(EarlyModernBranches, dispatcher);
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
