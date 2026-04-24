#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbNameCulture = MudSharp.Models.NameCulture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederNameAndHeightDefaultTests
{
	[TestMethod]
	public void CultureRaceAttributeProfilesForTesting_SecondPassFantasyDefaults_AreDistinct()
	{
		IReadOnlyDictionary<string, NonHumanAttributeProfile> profiles =
			CultureSeeder.CultureRaceAttributeProfilesForTesting;

		Assert.AreEqual(new NonHumanAttributeProfile(-1, 0, 2, 3, PerceptionBonus: 2, AuraBonus: 1),
			profiles["Elf"]);
		Assert.AreEqual(new NonHumanAttributeProfile(-3, 2, 1, 2, WillpowerBonus: 1, AuraBonus: 1),
			profiles["Hobbit"]);
		Assert.AreEqual(new NonHumanAttributeProfile(2, 4, -1, 0, WillpowerBonus: 3), profiles["Dwarf"]);
		Assert.AreEqual(new NonHumanAttributeProfile(3, 2, 0, -1, WillpowerBonus: 2, PerceptionBonus: 1,
			AuraBonus: -1), profiles["Orc"]);
		Assert.AreEqual(new NonHumanAttributeProfile(9, 8, -3, -4, WillpowerBonus: 4, PerceptionBonus: -1,
			AuraBonus: -2), profiles["Troll"]);
		Assert.IsTrue(profiles["Troll"].StrengthBonus > profiles["Orc"].StrengthBonus);
		Assert.IsTrue(profiles["Elf"].DexterityBonus > profiles["Dwarf"].DexterityBonus);
		Assert.IsTrue(profiles["Dwarf"].WillpowerBonus > profiles["Elf"].WillpowerBonus);
		Assert.IsTrue(profiles["Elf"].PerceptionBonus > profiles["Dwarf"].PerceptionBonus);
	}

	[TestMethod]
	public void CultureSeeder_FallbackProfiles_MakeBaseNameCulturesReadyAndRemainRerunnable()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.NameCultures.Add(new DbNameCulture
		{
			Name = "Admin",
			Definition = BuildNameCultureDefinition((NameUsage.BirthName, 1, 1))
		});
		context.SaveChanges();

		CultureSeeder seeder = new();
		RunSimpleNameSeeding(seeder, context);
		RunSimpleNameSeeding(seeder, context);

		AssertCultureHasReadyCompatibleProfile(context, "Simple", Gender.Male);
		AssertCultureHasReadyCompatibleProfile(context, "Given and Family", Gender.Male);
		AssertCultureHasReadyCompatibleProfile(context, "Given and Patronym", Gender.Male);
		AssertCultureHasReadyCompatibleProfile(context, "Given and Toponym", Gender.Male);
		AssertCultureHasReadyCompatibleProfile(context, "Admin", Gender.Male);

		Assert.AreEqual(0, context.RandomNameProfiles.Count(x => x.Name == "Simple Fallback"),
			"Simple should continue using its stock seeded profiles rather than an unnecessary fallback.");
		Assert.AreEqual(1, context.RandomNameProfiles.Count(x => x.Name == "Given and Family Fallback"));
		Assert.AreEqual(1, context.RandomNameProfiles.Count(x => x.Name == "Given and Patronym Fallback"));
		Assert.AreEqual(1, context.RandomNameProfiles.Count(x => x.Name == "Given and Toponym Fallback"));
		Assert.AreEqual(1, context.RandomNameProfiles.Count(x => x.Name == "Admin Fallback"));
	}

	[TestMethod]
	public void MedievalEuropeEthnicityMappings_AssignExpectedNameCulturesWithReadyProfiles()
	{
		using FuturemudDatabaseContext context = BuildContext();
		Race humanRace = new()
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
		context.Races.Add(humanRace);

		foreach (string nameCulture in MedievalTargetCultures())
		{
			context.NameCultures.Add(new DbNameCulture
			{
				Name = nameCulture,
				Definition = BuildNameCultureDefinition((NameUsage.BirthName, 1, 1), (NameUsage.Surname, 1, 1))
			});
		}

		foreach (string ethnicityName in MedievalExpectedMappings().Keys)
		{
			context.Ethnicities.Add(new Ethnicity
			{
				Name = ethnicityName,
				ChargenBlurb = string.Empty,
				ParentRace = humanRace,
				EthnicGroup = "Test"
			});
		}

		context.SaveChanges();

		CultureSeeder seeder = new();
		SetSeederContext(seeder, context);
		InvokePrivate(seeder, "EnsureFallbackRandomNameProfiles");
		InvokePrivate(seeder, "ApplyMedievalEuropeEthnicityNameCultureMappings");

		foreach ((string ethnicityName, (string maleCulture, string femaleCulture)) in MedievalExpectedMappings())
		{
			Ethnicity ethnicity = context.Ethnicities.Single(x => x.Name == ethnicityName);
			List<EthnicitiesNameCultures> links = context.EthnicitiesNameCultures
				.Where(x => x.EthnicityId == ethnicity.Id)
				.ToList();

			Assert.AreEqual(5, links.Count, $"Expected five name-culture links for ethnicity {ethnicityName}.");
			AssertGenderLink(links, Gender.Male, maleCulture);
			AssertGenderLink(links, Gender.Female, femaleCulture);
			AssertGenderLink(links, Gender.Neuter, maleCulture);
			AssertGenderLink(links, Gender.NonBinary, femaleCulture);
			AssertGenderLink(links, Gender.Indeterminate, maleCulture);

			AssertCultureHasReadyCompatibleProfile(context, maleCulture, Gender.Male);
			AssertCultureHasReadyCompatibleProfile(context, femaleCulture, Gender.Female);
		}
	}

	[TestMethod]
	public void ModernNameSeeder_AddsExpectedProfilesAndMappings()
	{
		using FuturemudDatabaseContext context = BuildContext();
		Race humanRace = new()
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
		context.Races.Add(humanRace);

		foreach (string ethnicityName in ModernExpectedMappings().Keys)
		{
			context.Ethnicities.Add(new Ethnicity
			{
				Name = ethnicityName,
				ChargenBlurb = string.Empty,
				ParentRace = humanRace,
				EthnicGroup = "Test"
			});
		}

		context.SaveChanges();

		CultureSeeder seeder = new();
		SetSeederContext(seeder, context);
		InvokePrivate(seeder, "SeedEuropeanNames", context);
		InvokePrivate(seeder, "ApplyModernEthnicityNameCultureMappings");

		foreach (string profileName in new[]
		{
			"Modern French Male",
			"Modern French Female",
			"Modern German Male",
			"Modern German Female",
			"Modern Spanish Male",
			"Modern Spanish Female",
			"Modern Portugese Male",
			"Modern Portugese Female"
		})
		{
			Assert.IsTrue(context.RandomNameProfiles.Any(x => x.Name == profileName),
				$"Expected {profileName} to be seeded.");
		}

		foreach (string cultureName in ModernTargetCultures())
		{
			Assert.IsTrue(context.RandomNameProfiles.Any(x => x.Name == $"{cultureName} Male"),
				$"Expected {cultureName} Male to be seeded.");
			Assert.IsTrue(context.RandomNameProfiles.Any(x => x.Name == $"{cultureName} Female"),
				$"Expected {cultureName} Female to be seeded.");

			AssertCultureHasReadyCompatibleProfile(context, cultureName, Gender.Male);
			AssertCultureHasReadyCompatibleProfile(context, cultureName, Gender.Female);
		}

		foreach ((string cultureName, int givenMinimum, int surnameMinimum) in ModernProfileElementMinimums())
		{
			AssertProfileHasMinimumElementCounts(context, $"{cultureName} Male", givenMinimum, surnameMinimum);
			AssertProfileHasMinimumElementCounts(context, $"{cultureName} Female", givenMinimum, surnameMinimum);
		}

		foreach ((string ethnicityName, (string maleCulture, string femaleCulture)) in ModernExpectedMappings())
		{
			Ethnicity ethnicity = context.Ethnicities.Single(x => x.Name == ethnicityName);
			List<EthnicitiesNameCultures> links = context.EthnicitiesNameCultures
				.Where(x => x.EthnicityId == ethnicity.Id)
				.ToList();

			Assert.AreEqual(5, links.Count, $"Expected five name-culture links for ethnicity {ethnicityName}.");
			AssertGenderLink(links, Gender.Male, maleCulture);
			AssertGenderLink(links, Gender.Female, femaleCulture);
			AssertGenderLink(links, Gender.Neuter, maleCulture);
			AssertGenderLink(links, Gender.NonBinary, femaleCulture);
			AssertGenderLink(links, Gender.Indeterminate, maleCulture);
		}
	}

	[TestMethod]
	public void StockRaceSeeders_AssignDefaultHeightWeightModelsForAdvertisedGenders()
	{
		IReadOnlyList<string> animalIssues = AnimalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, animalIssues.Count, string.Join("\n", animalIssues));

		IReadOnlyList<string> mythicalIssues = MythicalAnimalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, mythicalIssues.Count, string.Join("\n", mythicalIssues));

		IReadOnlyList<string> robotIssues = RobotSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, robotIssues.Count, string.Join("\n", robotIssues));

		string humanSeederSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "HumanSeeder.cs"));
		Assert.IsTrue(
			humanSeederSource.IndexOf("SetupHeightWeightModels();", StringComparison.Ordinal) <
			humanSeederSource.IndexOf("SetupRaces(", StringComparison.Ordinal),
			"HumanSeeder should initialise height-weight models before assigning them as race defaults.");
		StringAssert.Contains(humanSeederSource, "organicHumanoidRace.DefaultHeightWeightModelMale = _humanMaleHWModel;");
		StringAssert.Contains(humanSeederSource, "organicHumanoidRace.DefaultHeightWeightModelFemale = _humanFemaleHWModel;");
		StringAssert.Contains(humanSeederSource, "human.DefaultHeightWeightModelMale = _humanMaleHWModel;");
		StringAssert.Contains(humanSeederSource, "human.DefaultHeightWeightModelFemale = _humanFemaleHWModel;");

		string cultureHeritageSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CultureSeederHeritage.cs"));
		foreach (string assignment in new[]
		{
			"elfRace.DefaultHeightWeightModelMale = elfMaleHWModel;",
			"elfRace.DefaultHeightWeightModelFemale = elfFemaleHWModel;",
			"hobbitRace.DefaultHeightWeightModelMale = hobbitMaleHWModel;",
			"hobbitRace.DefaultHeightWeightModelFemale = hobbitFemaleHWModel;",
			"dwarfRace.DefaultHeightWeightModelMale = dwarfMaleHWModel;",
			"dwarfRace.DefaultHeightWeightModelFemale = dwarfFemaleHWModel;",
			"orcRace.DefaultHeightWeightModelMale = orcMaleHWModel;",
			"orcRace.DefaultHeightWeightModelFemale = orcFemaleHWModel;",
			"trollRace.DefaultHeightWeightModelMale = trollFemaleHWModel;",
			"trollRace.DefaultHeightWeightModelFemale = trollFemaleHWModel;"
		})
		{
			StringAssert.Contains(cultureHeritageSource, assignment);
		}
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void RunSimpleNameSeeding(CultureSeeder seeder, FuturemudDatabaseContext context)
	{
		SetSeederContext(seeder, context);
		InvokePrivate(seeder, "SeedSimple", context);
		InvokePrivate(seeder, "EnsureFallbackRandomNameProfiles");
	}

	private static void SetSeederContext(CultureSeeder seeder, FuturemudDatabaseContext context)
	{
		typeof(CultureSeeder)
			.GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, context);
	}

	private static object? InvokePrivate(object target, string methodName, params object[] args)
	{
		return target.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(target, args);
	}

	private static void AssertCultureHasReadyCompatibleProfile(
		FuturemudDatabaseContext context,
		string cultureName,
		Gender targetGender)
	{
		DbNameCulture culture = context.NameCultures.Single(x => x.Name == cultureName);
		bool hasReadyProfile = context.RandomNameProfiles
			.Where(x => x.NameCultureId == culture.Id)
			.AsEnumerable()
			.Any(x => IsCompatibleGender((Gender)x.Gender, targetGender) && IsReadyProfile(context, x.Id));

		Assert.IsTrue(hasReadyProfile, $"Expected {cultureName} to have a ready random name profile compatible with {targetGender.DescribeEnum()}.");
	}

	private static bool IsReadyProfile(FuturemudDatabaseContext context, long profileId)
	{
		List<int> usages = context.RandomNameProfilesDiceExpressions
			.Where(x => x.RandomNameProfileId == profileId)
			.Select(x => x.NameUsage)
			.Distinct()
			.ToList();
		return usages.Count > 0 &&
		       usages.All(usage => context.RandomNameProfilesElements.Any(x =>
			       x.RandomNameProfileId == profileId && x.NameUsage == usage));
	}

	private static bool IsCompatibleGender(Gender profileGender, Gender targetGender)
	{
		return profileGender == Gender.NonBinary ||
		       profileGender == Gender.Indeterminate ||
		       profileGender == targetGender;
	}

	private static void AssertGenderLink(
		IEnumerable<EthnicitiesNameCultures> links,
		Gender gender,
		string expectedCulture)
	{
		EthnicitiesNameCultures link = links.Single(x => x.Gender == (short)gender);
		Assert.AreEqual(expectedCulture, link.NameCulture.Name);
	}

	private static IEnumerable<string> MedievalTargetCultures()
	{
		return MedievalExpectedMappings()
			.SelectMany(x => new[] { x.Value.Male, x.Value.Female })
			.Distinct(StringComparer.OrdinalIgnoreCase);
	}

	private static IEnumerable<string> ModernTargetCultures()
	{
		return ModernExpectedMappings()
			.SelectMany(x => new[] { x.Value.Male, x.Value.Female })
			.Distinct(StringComparer.OrdinalIgnoreCase);
	}

	private static IReadOnlyDictionary<string, (string Male, string Female)> MedievalExpectedMappings()
	{
		return new Dictionary<string, (string Male, string Female)>(StringComparer.OrdinalIgnoreCase)
		{
			["German"] = ("German", "German"),
			["Austrian"] = ("German", "German"),
			["Dutch"] = ("Dutch", "Dutch"),
			["French"] = ("French", "French"),
			["Occitan"] = ("French", "French"),
			["English"] = ("English", "English"),
			["Venetian"] = ("Italian", "Italian"),
			["Florentine"] = ("Italian", "Italian"),
			["Neapolitan"] = ("Italian", "Italian"),
			["Milanese"] = ("Italian", "Italian"),
			["Sicilian"] = ("Italian", "Italian"),
			["Corsican"] = ("Italian", "Italian"),
			["Sardinian"] = ("Italian", "Italian"),
			["Castilian"] = ("Iberian", "Iberian"),
			["Catalan"] = ("Iberian", "Iberian"),
			["Galicians"] = ("Iberian", "Iberian"),
			["Portugese"] = ("Iberian", "Iberian"),
			["Basque"] = ("Basque", "Basque"),
			["Ashkenazi Jewish"] = ("Jewish Male", "Jewish Female"),
			["Mizrahi Jewish"] = ("Jewish Male", "Jewish Female"),
			["Sephardic Jewish"] = ("Jewish Male", "Jewish Female"),
			["Gaelic"] = ("Irish", "Irish"),
			["Welsh"] = ("Welsh", "Welsh"),
			["Breton"] = ("Breton", "Breton"),
			["Polish"] = ("Polish", "Polish"),
			["Czech"] = ("Western Slavic", "Western Slavic"),
			["Slovak"] = ("Western Slavic", "Western Slavic"),
			["Croat"] = ("Western Slavic", "Western Slavic"),
			["Serb"] = ("Western Slavic", "Western Slavic"),
			["Bosniak"] = ("Western Slavic", "Western Slavic"),
			["Vlach"] = ("Western Slavic", "Western Slavic"),
			["Ruthenian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Ukrainian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Russian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Lithuanian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Estonian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Latvian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Prussian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Hungarian"] = ("Hungarian", "Hungarian"),
			["Norse"] = ("Danish", "Danish"),
			["Danish"] = ("Danish", "Danish"),
			["Swedish"] = ("Swedish", "Swedish"),
			["Icelandic"] = ("Danish", "Danish"),
			["Roman"] = ("Hellenic", "Hellenic"),
			["Ottoman"] = ("Turkish", "Turkish"),
			["Cossack"] = ("Turkish", "Turkish"),
			["Arabic"] = ("Levantine", "Levantine"),
			["Persian"] = ("Persian", "Persian"),
			["Moorish"] = ("Morrocan", "Morrocan"),
			["North African"] = ("Morrocan", "Morrocan")
		};
	}

	private static IReadOnlyDictionary<string, (string Male, string Female)> ModernExpectedMappings()
	{
		return new Dictionary<string, (string Male, string Female)>(StringComparer.OrdinalIgnoreCase)
		{
			["Germanic"] = ("Modern Germanic", "Modern Germanic"),
			["Italic"] = ("Modern Italic", "Modern Italic"),
			["Hispanic"] = ("Modern Iberian", "Modern Iberian"),
			["Celtic"] = ("Modern Celtic", "Modern Celtic"),
			["Slavic"] = ("Modern Slavic", "Modern Slavic"),
			["Greek"] = ("Modern Greek", "Modern Greek"),
			["Turkish"] = ("Modern Turkic", "Modern Turkic"),
			["Arabic"] = ("Modern Arabic", "Modern Arabic"),
			["Persian"] = ("Modern Persian", "Modern Persian"),
			["Scandanavian"] = ("Modern Scandinavian", "Modern Scandinavian"),
			["North African"] = ("Modern North African", "Modern North African"),
			["Bantu"] = ("Modern Sub-Saharan", "Modern Sub-Saharan"),
			["Khoisan"] = ("Modern Sub-Saharan", "Modern Sub-Saharan"),
			["Swahili"] = ("Modern Swahili", "Modern Swahili"),
			["Polynesian"] = ("Modern Oceanic", "Modern Oceanic"),
			["Austronesian"] = ("Modern Southeast Asian", "Modern Southeast Asian"),
			["Melanesian"] = ("Modern Oceanic", "Modern Oceanic"),
			["Afro-Carribean"] = ("Modern Afro-Caribbean", "Modern Afro-Caribbean"),
			["Afro-American"] = ("Modern Afro-American", "Modern Afro-American"),
			["Eskimo"] = ("Modern Indigenous North American", "Modern Indigenous North American"),
			["Amerindian"] = ("Modern Indigenous North American", "Modern Indigenous North American"),
			["Mesoamerican"] = ("Modern Indigenous Latin American", "Modern Indigenous Latin American"),
			["Andean"] = ("Modern Indigenous Latin American", "Modern Indigenous Latin American"),
			["Mongolian"] = ("Modern Central Asian", "Modern Central Asian"),
			["Yamato"] = ("Modern Japanese", "Modern Japanese"),
			["Korean"] = ("Modern Korean", "Modern Korean"),
			["Tibetan"] = ("Modern Central Asian", "Modern Central Asian"),
			["Uyghur"] = ("Modern Turkic", "Modern Turkic"),
			["Ainu"] = ("Modern Japanese", "Modern Japanese"),
			["Aboriginal Australian"] = ("Modern Aboriginal Australian", "Modern Aboriginal Australian"),
			["Austro-Asiatic"] = ("Modern Southeast Asian", "Modern Southeast Asian"),
			["Tai-Kadai"] = ("Modern Southeast Asian", "Modern Southeast Asian"),
			["Anglo-Saxon"] = ("Modern Anglo-Saxon", "Modern Anglo-Saxon"),
			["Han"] = ("Modern Chinese", "Modern Chinese"),
			["Dravidian"] = ("Modern Indian", "Modern Indian"),
			["Indo-Aryan"] = ("Modern Indian", "Modern Indian")
		};
	}

	private static IEnumerable<(string CultureName, int GivenMinimum, int SurnameMinimum)> ModernProfileElementMinimums()
	{
		yield return ("Modern Germanic", 50, 100);
		yield return ("Modern Italic", 50, 100);
		yield return ("Modern Iberian", 50, 100);
		yield return ("Modern Celtic", 50, 100);
		yield return ("Modern Slavic", 50, 100);
		yield return ("Modern Greek", 50, 100);
		yield return ("Modern Turkic", 50, 100);
		yield return ("Modern Arabic", 50, 100);
		yield return ("Modern Persian", 50, 100);
		yield return ("Modern Scandinavian", 50, 100);
		yield return ("Modern North African", 50, 100);
		yield return ("Modern Sub-Saharan", 50, 100);
		yield return ("Modern Swahili", 50, 100);
		yield return ("Modern Oceanic", 50, 100);
		yield return ("Modern Southeast Asian", 50, 100);
		yield return ("Modern Afro-Caribbean", 50, 100);
		yield return ("Modern Afro-American", 50, 100);
		yield return ("Modern Indigenous North American", 50, 100);
		yield return ("Modern Indigenous Latin American", 50, 100);
		yield return ("Modern Japanese", 50, 100);
		yield return ("Modern Korean", 50, 40);
		yield return ("Modern Central Asian", 50, 60);
		yield return ("Modern Aboriginal Australian", 50, 100);
		yield return ("Modern Anglo-Saxon", 50, 100);
	}

	private static void AssertProfileHasMinimumElementCounts(
		FuturemudDatabaseContext context,
		string profileName,
		int givenMinimum,
		int surnameMinimum)
	{
		MudSharp.Models.RandomNameProfile profile = context.RandomNameProfiles.Single(x => x.Name == profileName);
		int givenCount = context.RandomNameProfilesElements.Count(x =>
			x.RandomNameProfileId == profile.Id &&
			x.NameUsage == (int)NameUsage.BirthName);
		int surnameCount = context.RandomNameProfilesElements.Count(x =>
			x.RandomNameProfileId == profile.Id &&
			x.NameUsage == (int)NameUsage.Surname);

		Assert.IsTrue(givenCount >= givenMinimum,
			$"Expected {profileName} to have at least {givenMinimum} given names but found {givenCount}.");
		Assert.IsTrue(surnameCount >= surnameMinimum,
			$"Expected {profileName} to have at least {surnameMinimum} surnames but found {surnameCount}.");
	}

	private static string BuildNameCultureDefinition(params (NameUsage Usage, int Minimum, int Maximum)[] elements)
	{
		return
			$"<NameCulture><Patterns><Pattern Style=\"0\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"1\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"2\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"3\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"4\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"5\" Text=\"{{0}}\" Params=\"0\" /></Patterns><Elements>{string.Concat(elements.Select(x => $"<Element Usage=\"{(int)x.Usage}\" MinimumCount=\"{x.Minimum}\" MaximumCount=\"{x.Maximum}\" Name=\"{x.Usage}\"><![CDATA[Test]]></Element>"))}</Elements><NameEntryRegex><![CDATA[^(?<birthname>[\\w '-]+)$]]></NameEntryRegex></NameCulture>";
	}

	private static string GetSourcePath(params string[] parts)
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
