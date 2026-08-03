#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceMilitarySeederTests
{
	private static readonly IReadOnlyDictionary<string, int> ExpectedCategoryCounts =
		new Dictionary<string, int>(StringComparer.Ordinal)
		{
			["Melee weapons"] = 14,
			["Ranged weapons"] = 11,
			["Firearms & ammunition"] = 15,
			["Artillery"] = 17,
			["Armour & barding"] = 18,
			["Shields"] = 8,
			["Military support & field gear"] = 13
		};

	[TestMethod]
	public void CanonicalCatalogue_HasUniqueStableReferencesExactFamilyCountsAndAuthoredDescriptions()
	{
		var specs = ItemSeeder.RenaissanceMilitaryItemSpecsForTesting.ToArray();

		Assert.AreEqual(96, specs.Length);
		Assert.AreEqual(specs.Length,
			specs.Select(x => x.StableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		CollectionAssert.AreEquivalent(
			ExpectedCategoryCounts.OrderBy(x => x.Key).ToArray(),
			specs.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key).ToArray());
		Assert.IsFalse(specs.Any(x => x.StableReference.Contains("_pass_", StringComparison.OrdinalIgnoreCase) ||
		                              x.StableReference.Contains("_expansion_", StringComparison.OrdinalIgnoreCase)));

		foreach (var spec in specs)
		{
			Assert.IsTrue(Regex.IsMatch(spec.ShortDescription, "^(a|an) [a-z0-9' -]+$", RegexOptions.IgnoreCase),
				$"{spec.StableReference} does not have an article-led SDesc.");
			Assert.IsTrue(spec.ShortDescription.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length is >= 3 and <= 6,
				$"{spec.StableReference} SDesc is not three to six words.");
			Assert.AreEqual(3, Regex.Matches(spec.FullDescription, @"[.!?](?:\s|$)").Count,
				$"{spec.StableReference} must have exactly three full-description sentences.");
			Assert.IsFalse(spec.FullDescription.Contains("seed", StringComparison.OrdinalIgnoreCase));
			Assert.IsFalse(spec.FullDescription.Contains("builder", StringComparison.OrdinalIgnoreCase));
			Assert.IsFalse(spec.FullDescription.Contains("component", StringComparison.OrdinalIgnoreCase));
			Assert.IsFalse(spec.FullDescription.Contains("documented form", StringComparison.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void CanonicalDesignTable_AgreesWithGeneratedCatalogueAndCultureDecisionMatrix()
	{
		var design = ReadSource("Design Documents", "Seeding",
			"FutureMUD_Renaissance_Military_Firearms_Armour_Design_Reference.md");
		var specs = ItemSeeder.RenaissanceMilitaryItemSpecsForTesting.ToArray();

		Assert.AreEqual(specs.Length,
			Regex.Matches(design, @"^\| renaissance_military_[a-z0-9_]+ \|", RegexOptions.Multiline).Count);
		foreach (var spec in specs)
		{
			StringAssert.Contains(design, $"| {spec.StableReference} |");
			StringAssert.Contains(design, spec.FullDescription);
		}

		foreach (var (category, count) in ExpectedCategoryCounts)
		{
			StringAssert.Contains(design, $"| {category} | {count} |");
		}

		StringAssert.Contains(design, "## Culture and admission matrix");
		StringAssert.Contains(design, "Handgonnes are not seeded");
		StringAssert.Contains(design, "FutureMUD/FutureMUD#575 supplied the required item-component groundwork");
	}

	[TestMethod]
	public void MaintainedCatalogues_ContainEveryRenaissanceMilitaryDependency()
	{
		using var materialsDocument = JsonDocument.Parse(ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		var materials = materialsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceMilitaryMaterialsForTesting.ToArray(), materials.ToArray());

		using var componentsDocument = JsonDocument.Parse(ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var components = componentsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceMilitaryComponentsForTesting.ToArray(), components.ToArray());

		var tags = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv")
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceMilitaryTagsForTesting.ToArray(), tags.ToArray());
	}

	[TestMethod]
	public void GeneratedManifest_CopiesEveryAuthoredDescriptionAndHasNoGenericDescriptionTemplate()
	{
		var generated = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Renaissance.MilitaryManifestData.Generated.cs");
		StringAssert.Contains(generated, "Generated by scripts/generate-renaissance-military-manifest.py");
		Assert.IsFalse(generated.Contains("documented Renaissance military form", StringComparison.OrdinalIgnoreCase));
		foreach (var spec in ItemSeeder.RenaissanceMilitaryItemSpecsForTesting)
		{
			StringAssert.Contains(generated, spec.StableReference);
			StringAssert.Contains(generated, spec.FullDescription);
		}
	}

	[TestMethod]
	public void CompositionInvariants_UseTheExpectedSupportedProfiles()
	{
		var specs = ItemSeeder.RenaissanceMilitaryItemSpecsForTesting
			.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);

		CollectionAssert.Contains(specs["renaissance_military_matchlock_arquebus"].Components.ToArray(),
			"Musket_Matchlock_Arquebus55");
		CollectionAssert.Contains(specs["renaissance_military_wheellock_pistol"].Components.ToArray(), "Pistol_Wheellock45");
		CollectionAssert.Contains(specs["renaissance_military_falconet_bronze"].Components.ToArray(), "Artillery_Falconet");
		CollectionAssert.Contains(specs["renaissance_military_swivel_gun_pivot"].Components.ToArray(), "ArtilleryMount_Swivel");
		CollectionAssert.Contains(specs["renaissance_military_corselet_breast_and_back"].Components.ToArray(), "Armour_PlateMedium");
		CollectionAssert.Contains(specs["renaissance_military_buckler_fist_grip"].Components.ToArray(), "Shield_Buckler");
		CollectionAssert.Contains(specs["renaissance_military_barding_chanfron"].Components.ToArray(), "Wear_Chanfron");
		CollectionAssert.Contains(specs["renaissance_military_cartridge_bandolier"].Components.ToArray(),
			"Container_CartridgeBandolier");
		Assert.IsTrue(specs["renaissance_military_matchlock_arquebus"].Skinnable);
		Assert.IsFalse(specs["renaissance_military_paper_cartridge_055"].Skinnable);
	}

	[TestMethod]
	public void DependencyValidation_ReportsExactEarlyGunProfileAndRerunGuidance()
	{
		var missing = ItemSeeder.ValidateRenaissanceMilitaryDependenciesForTesting(
			ItemSeeder.RenaissanceMilitaryMaterialsForTesting,
			ItemSeeder.RenaissanceMilitaryTagsForTesting,
			ItemSeeder.RenaissanceMilitaryComponentsForTesting
				.Where(x => !x.Equals("Musket_Matchlock_Arquebus55", StringComparison.OrdinalIgnoreCase)));

		CollectionAssert.Contains(missing.ToArray(), "Missing seeded component: Musket_Matchlock_Arquebus55");
		StringAssert.Contains(ItemSeeder.RenaissanceMilitaryPrerequisiteRerunGuidanceForTesting, "early-gun packages");
	}

	[TestMethod]
	public void MilitarySeed_IsIdempotentAndPreservesRepresentativeStockMetadata()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);

		new ItemSeeder().SeedRenaissanceMilitaryForTesting(context);
		context.SaveChanges();
		var firstCount = context.GameItemProtos.Count(x => x.UniqueName!.StartsWith("renaissance_military_"));

		new ItemSeeder().SeedRenaissanceMilitaryForTesting(context);
		context.SaveChanges();
		var stock = context.GameItemProtos
			.Where(x => x.UniqueName!.StartsWith("renaissance_military_"))
			.ToDictionary(x => x.UniqueName, StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual(96, firstCount);
		Assert.AreEqual(firstCount, stock.Count);
		Assert.IsTrue(stock["renaissance_military_matchlock_arquebus"].PermitPlayerSkins);
		Assert.IsFalse(stock["renaissance_military_paper_cartridge_055"].PermitPlayerSkins);
		StringAssert.Contains(stock["renaissance_military_falconet_bronze"].BuilderNotes, "component profile");
	}

	[TestMethod]
	public void MissingPrerequisite_ThrowsBeforeTheMilitaryStageWritesAnyPrototype()
	{
		using var context = BuildContext();
		SeedPrerequisites(context, "Musket_Matchlock_Arquebus55");

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			new ItemSeeder().SeedRenaissanceMilitaryForTesting(context));

		StringAssert.Contains(exception.Message, "Missing seeded component: Musket_Matchlock_Arquebus55");
		StringAssert.Contains(exception.Message, "no Renaissance item stage has been written");
		Assert.AreEqual(0, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void SeedData_ValidatesRenaissanceMilitaryPrerequisitesBeforeReworkItems()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.cs");
		var gate = source.IndexOf("RunSeedStage(\"Validating Renaissance military prerequisites\"", StringComparison.Ordinal);
		var rework = source.IndexOf("SeedReworkItems();", StringComparison.Ordinal);

		Assert.IsTrue(gate >= 0, "Renaissance prerequisite gate was not registered in SeedData.");
		Assert.IsTrue(rework >= 0 && gate < rework,
			"Renaissance prerequisite gate must run before any rework item stage can write.");
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context, string? omittedComponent = null)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "RenaissanceSeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			HasBeenActiveInWeek = true,
			HintsEnabled = true
		});

		var materialId = 1L;
		foreach (var name in ItemSeeder.RenaissanceMilitaryMaterialsForTesting)
		{
			context.Materials.Add(new Material
			{
				Id = materialId++,
				Name = name,
				MaterialDescription = name,
				Density = 1.0,
				Organic = true,
				Type = 0,
				BehaviourType = 0,
				ResidueSdesc = string.Empty,
				ResidueDesc = string.Empty,
				ResidueColour = "grey"
			});
		}

		var tagId = 1L;
		foreach (var path in ItemSeeder.RenaissanceMilitaryTagsForTesting)
		{
			context.Tags.Add(new Tag { Id = tagId++, Name = path });
		}

		var componentId = 1L;
		foreach (var name in ItemSeeder.RenaissanceMilitaryComponentsForTesting
			         .Where(x => !x.Equals(omittedComponent, StringComparison.OrdinalIgnoreCase)))
		{
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = componentId++,
				Name = name,
				Type = "test",
				Definition = "<Definition />",
				Description = name,
				EditableItemId = componentId,
				RevisionNumber = 0
			});
		}

		context.SaveChanges();
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
