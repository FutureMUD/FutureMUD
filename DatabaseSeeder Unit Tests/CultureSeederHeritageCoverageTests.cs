#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederHeritageCoverageTests
{
	[TestMethod]
	public void HistoricalHeritageCatalogues_AreInternallyConsistent()
	{
		IReadOnlyList<string> issues = CultureSeeder.ValidateHistoricalHeritageCataloguesForTesting();

		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));
	}

	[TestMethod]
	public void DarkAgesEthnicityCoverage_MatchesEveryNamingCultureAndIncludesSpecificGroups()
	{
		IReadOnlyDictionary<string, string[]> coverage =
			CultureSeeder.DarkAgesAndMedievalEthnicityCoverageForTesting;

		CollectionAssert.AreEquivalent(
			CultureSeeder.DarkAgesAndMedievalNameCultureNamesForTesting.ToArray(),
			coverage.Keys.ToArray());
		Assert.AreEqual(25, coverage.Count);
		Assert.AreEqual(89, coverage.Sum(x => x.Value.Length));
		Assert.IsTrue(coverage.All(x => x.Value.Length >= 2));

		CollectionAssert.IsSubsetOf(
			new[]
			{
				"High Medieval Bavarian",
				"High Medieval Swabian",
				"High Medieval Franconian",
				"High Medieval Saxon",
				"High Medieval Thuringian",
				"High Medieval Frisian",
				"High Medieval Low German"
			},
			coverage["Medieval Imperial German"]);
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"High Medieval Castilian",
				"High Medieval Leonese",
				"High Medieval Aragonese",
				"High Medieval Catalan",
				"High Medieval Galician",
				"High Medieval Portuguese",
				"High Medieval Basque"
			},
			coverage["Medieval Christian Iberian"]);
	}

	[TestMethod]
	public void RenaissanceWorldEthnicityCoverage_MatchesEveryNamingCultureAndIncludesSpecificGroups()
	{
		IReadOnlyDictionary<string, string[]> coverage =
			CultureSeeder.RenaissanceWorldEthnicityCoverageForTesting;

		CollectionAssert.AreEquivalent(
			CultureSeeder.RenaissanceWorldNameCultureNamesForTesting.ToArray(),
			coverage.Keys.ToArray());
		Assert.AreEqual(25, coverage.Count);
		Assert.AreEqual(125, coverage.Sum(x => x.Value.Length));
		Assert.IsTrue(coverage.All(x => x.Value.Length >= 3));

		CollectionAssert.IsSubsetOf(
			new[]
			{
				"Renaissance Egyptian Arab",
				"Renaissance Syrian Arab",
				"Renaissance Egyptian Copt",
				"Renaissance Circassian Mamluk",
				"Renaissance Turkic Mamluk"
			},
			coverage["Renaissance Mamluk Arabic"]);
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"Renaissance Northern Han",
				"Renaissance Wu-Speaking Han",
				"Renaissance Hakka Han",
				"Renaissance Yue-Speaking Han",
				"Renaissance Southern Min Han",
				"Renaissance Eastern Min Han",
				"Renaissance Northern Min Han"
			},
			coverage["Renaissance Ming Chinese"]);
	}

	[TestMethod]
	public void ExistingPacks_AddBroadRegionalCultures()
	{
		CollectionAssert.Contains(CultureSeeder.ModernBroadCultureNamesForTesting.ToArray(), "Modern Chinese");
		CollectionAssert.Contains(CultureSeeder.ModernBroadCultureNamesForTesting.ToArray(), "Modern South Asian");
		CollectionAssert.Contains(CultureSeeder.ModernBroadCultureNamesForTesting.ToArray(), "Modern Oceanic");
		Assert.AreEqual(26, CultureSeeder.ModernBroadCultureNamesForTesting.Count);

		CollectionAssert.Contains(
			CultureSeeder.RenaissanceEuropeBroadCultureNamesForTesting.ToArray(),
			"Renaissance German Imperial");
		CollectionAssert.Contains(
			CultureSeeder.RenaissanceEuropeBroadCultureNamesForTesting.ToArray(),
			"Renaissance Iberian");
		CollectionAssert.Contains(
			CultureSeeder.RenaissanceEuropeBroadCultureNamesForTesting.ToArray(),
			"Renaissance Albanian");
		Assert.AreEqual(29, CultureSeeder.RenaissanceEuropeBroadCultureNamesForTesting.Count);
	}

	[TestMethod]
	public void DarkAgesEthnicityExpansion_RerunsWithoutDuplicatesAndRepairsCharacteristics()
	{
		using FuturemudDatabaseContext context = BuildContext();
		Race human = BuildHumanRace();
		PopulationBloodModel bloodModel = new() { Name = "Test Blood Model" };
		FutureProg alwaysTrue = new()
		{
			FunctionName = "AlwaysTrue",
			FunctionComment = "Test helper",
			FunctionText = "return true",
			ReturnType = 1,
			Category = "Test",
			Subcategory = "Test"
		};
		CharacteristicDefinition definition = new()
		{
			Name = "Test Feature",
			Type = 0,
			Pattern = ".*",
			Description = "Test feature",
			Model = "standard",
			Definition = "<Definition />"
		};
		CharacteristicProfile expectedProfile = new()
		{
			Name = "Test Profile",
			Definition = "<Definition />",
			Type = "standard",
			Description = "Expected profile",
			TargetDefinition = definition
		};
		CharacteristicProfile wrongProfile = new()
		{
			Name = "Wrong Profile",
			Definition = "<Definition />",
			Type = "standard",
			Description = "Wrong profile",
			TargetDefinition = definition
		};
		context.AddRange(human, bloodModel, alwaysTrue, definition, expectedProfile, wrongProfile);

		foreach ((string nameCulture, string[] ethnicities) in
				CultureSeeder.DarkAgesAndMedievalEthnicityCoverageForTesting)
		{
			NameCulture culture = new() { Name = nameCulture, Definition = "<NameCulture />" };
			context.NameCultures.Add(culture);
			Ethnicity template = new()
			{
				Name = ethnicities[0],
				ChargenBlurb = "Template ethnicity",
				ParentRace = human,
				EthnicGroup = "Template",
				EthnicSubgroup = "Template",
				PopulationBloodModel = bloodModel,
				AvailabilityProg = alwaysTrue
			};
			template.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
			{
				Ethnicity = template,
				CharacteristicDefinition = definition,
				CharacteristicProfile = expectedProfile
			});
			template.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
			{
				Ethnicity = template,
				NameCulture = culture,
				Gender = (short)Gender.Indeterminate
			});
			context.Ethnicities.Add(template);
		}

		context.SaveChanges();
		CultureSeeder seeder = new();
		SetPrivateField(seeder, "_context", context);
		SetPrivateField(seeder, "_humanRace", human);
		SetPrivateField(seeder, "_alwaysTrueProg", alwaysTrue);
		GetPrivateDictionary<string, PopulationBloodModel>(seeder, "_bloodModels")[bloodModel.Name] = bloodModel;
		GetPrivateDictionary<string, CharacteristicDefinition>(seeder, "_definitions")[definition.Name] = definition;
		GetPrivateDictionary<string, CharacteristicProfile>(seeder, "_profiles")[expectedProfile.Name] = expectedProfile;
		GetPrivateDictionary<string, CharacteristicProfile>(seeder, "_profiles")[wrongProfile.Name] = wrongProfile;

		InvokePrivate(seeder, "SeedDarkAgesAndMedievalHeritageExpansion");

		Ethnicity bavarian = context.Ethnicities.Single(x => x.Name == "High Medieval Bavarian");
		EthnicitiesCharacteristics originalCharacteristic = bavarian.EthnicitiesCharacteristics.Single();
		context.EthnicitiesCharacteristics.Remove(originalCharacteristic);
		context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = bavarian,
			CharacteristicDefinition = definition,
			CharacteristicProfile = wrongProfile
		});
		context.SaveChanges();

		InvokePrivate(seeder, "SeedDarkAgesAndMedievalHeritageExpansion");

		Assert.AreEqual(1, context.Ethnicities.Count(x => x.Name == "High Medieval Bavarian"));
		Assert.AreEqual(
			CultureSeeder.DarkAgesAndMedievalEthnicityCoverageForTesting.Sum(x => x.Value.Length),
			context.Ethnicities.Count());
		Assert.AreEqual(expectedProfile.Id,
			context.EthnicitiesCharacteristics.Single(x => x.EthnicityId == bavarian.Id).CharacteristicProfileId);
		List<EthnicitiesNameCultures> links = context.EthnicitiesNameCultures
			.Where(x => x.EthnicityId == bavarian.Id)
			.ToList();
		Assert.AreEqual(5, links.Count);
		Assert.IsTrue(links.All(x => x.NameCulture.Name == "Medieval Imperial German"));
	}

	[TestMethod]
	public void ModernCultureExpansion_RerunsWithoutDuplicatesAndKeepsNamingLinks()
	{
		using FuturemudDatabaseContext context = BuildContext();
		FutureProg alwaysTrue = new()
		{
			FunctionName = "AlwaysTrue",
			FunctionComment = "Test helper",
			FunctionText = "return true",
			ReturnType = 1,
			Category = "Test",
			Subcategory = "Test"
		};
		FutureProg skillStartingValue = new()
		{
			FunctionName = "SkillStartingValue",
			FunctionComment = "Test helper",
			FunctionText = "return 20",
			ReturnType = 2,
			Category = "Test",
			Subcategory = "Test"
		};
		context.FutureProgs.AddRange(alwaysTrue, skillStartingValue);
		foreach (string name in new[]
		{
			"Modern Germanic", "Modern Italic", "Modern Iberian", "Modern Celtic", "Modern Slavic",
			"Modern Greek", "Modern Turkic", "Modern Arabic", "Modern Persian", "Modern Scandinavian",
			"Modern North African", "Modern Sub-Saharan", "Modern Swahili", "Modern Oceanic", "Modern Indian",
			"Modern Southeast Asian", "Modern Afro-Caribbean", "Modern Afro-American",
			"Modern Indigenous North American", "Modern Indigenous Latin American", "Modern Central Asian",
			"Modern Chinese", "Modern Japanese", "Modern Korean", "Modern Aboriginal Australian",
			"Modern Anglo-Saxon", "Given and Family", "Simple"
		})
		{
			context.NameCultures.Add(new NameCulture { Name = name, Definition = "<NameCulture />" });
		}
		context.SaveChanges();

		CultureSeeder seeder = new();
		SetPrivateField(seeder, "_context", context);
		SetPrivateField(seeder, "_alwaysTrueProg", alwaysTrue);
		SetPrivateField(seeder, "_skillStartProg", skillStartingValue);

		InvokePrivate(seeder, "SeedModernCultureCoverage");
		InvokePrivate(seeder, "SeedModernCultureCoverage");

		Assert.AreEqual(26, context.Cultures.Count());
		Culture southAsian = context.Cultures.Single(x => x.Name == "Modern South Asian");
		List<CulturesNameCultures> links = context.CulturesNameCultures
			.Where(x => x.CultureId == southAsian.Id)
			.ToList();
		Assert.AreEqual(5, links.Count);
		Assert.IsTrue(links.All(x => x.NameCulture.Name == "Modern Indian"));
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options =
			new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static Race BuildHumanRace()
	{
		return new Race
		{
			Name = "Human",
			Description = "Test race",
			AllowedGenders = "0 1 2 3 4",
			DiceExpression = "1d1",
			CommunicationStrategyType = "humanoid",
			MaximumDragWeightExpression = "1",
			MaximumLiftWeightExpression = "1",
			HoldBreathLengthExpression = "1",
			BreathingVolumeExpression = "1",
			EatCorpseEmoteText = string.Empty,
			HandednessOptions = "1"
		};
	}

	private static void SetPrivateField(object target, string name, object value)
	{
		target.GetType()
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(target, value);
	}

	private static Dictionary<TKey, TValue> GetPrivateDictionary<TKey, TValue>(object target, string name)
		where TKey : notnull
	{
		return (Dictionary<TKey, TValue>)target.GetType()
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(target)!;
	}

	private static object? InvokePrivate(object target, string methodName, params object[] args)
	{
		return target.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(target, args);
	}
}
