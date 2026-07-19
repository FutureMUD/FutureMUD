#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbFutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederLanguageCoverageTests
{
	[TestMethod]
	public void HistoricalCulturePacks_OfferLanguageSeeding()
	{
		using FuturemudDatabaseContext context = BuildContext();
		CultureSeeder seeder = new();
		var question = seeder.SeederQuestions.Single(x => x.Id == "seedlanguages");

		Assert.IsTrue(question.Filter(context, Answers("earth-darkagesandmedieval")));
		Assert.IsTrue(question.Filter(context, Answers("earth-renaissanceworldexpansion")));
	}

	[TestMethod]
	public void HistoricalCulturePacks_AcceptLegacyAnswersWithoutLanguageChoice()
	{
		IReadOnlyDictionary<string, string> legacyAnswers = new Dictionary<string, string>
		{
			["seednames"] = "no",
			["seedheritage"] = "no"
		};

		InvokePrivate(new CultureSeeder(), "SeedDarkAgesAndMedieval", legacyAnswers);
		InvokePrivate(new CultureSeeder(), "SeedRenaissanceWorldExpansion", legacyAnswers);
	}

	[TestMethod]
	public void HistoricalLanguageCatalogues_AreInternallyConsistent()
	{
		IReadOnlyList<string> issues = CultureSeeder.ValidateHistoricalLanguageCataloguesForTesting();

		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));
	}

	[TestMethod]
	public void DarkAgesLanguageCoverage_MatchesEveryGeneratedNamingCulture()
	{
		CollectionAssert.AreEquivalent(
			CultureSeeder.DarkAgesAndMedievalNameCultureNamesForTesting.ToArray(),
			CultureSeeder.DarkAgesAndMedievalLanguageCoverageForTesting.Keys.ToArray());
		Assert.AreEqual(25, CultureSeeder.DarkAgesAndMedievalLanguageCoverageForTesting.Count);
		Assert.AreEqual(36, CultureSeeder.DarkAgesAndMedievalLanguageNamesForTesting.Count);
	}

	[TestMethod]
	public void RenaissanceWorldLanguageCoverage_MatchesEveryGeneratedNamingCulture()
	{
		CollectionAssert.AreEquivalent(
			CultureSeeder.RenaissanceWorldNameCultureNamesForTesting.ToArray(),
			CultureSeeder.RenaissanceWorldLanguageCoverageForTesting.Keys.ToArray());
		Assert.AreEqual(25, CultureSeeder.RenaissanceWorldLanguageCoverageForTesting.Count);
		Assert.AreEqual(52, CultureSeeder.RenaissanceWorldLanguageNamesForTesting.Count);
	}

	[TestMethod]
	public void ExistingPackExpansion_CoversPreviouslyUnrepresentedNamingCultures()
	{
		IReadOnlyDictionary<string, string[]> coverage =
			CultureSeeder.ExistingPackLanguageCoverageExpansionForTesting;

		foreach (string culture in new[]
		{
			"Modern Oceanic",
			"Modern Southeast Asian",
			"Modern Indigenous Latin American",
			"Modern Central Asian",
			"Modern Aboriginal Australian",
			"Ainu ethnicity (Modern Japanese names)",
			"Scythian-Sarmatian",
			"Kushite",
			"Albanian"
		})
		{
			Assert.IsTrue(coverage.TryGetValue(culture, out string[]? languages), culture);
			Assert.IsTrue(languages.Length > 0, culture);
		}
	}

	[TestMethod]
	public void SharedScriptUpsert_PreservesPriorPackLanguageMemberships()
	{
		string sharedSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CultureSeeder.Shared.cs"));
		string languageSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CultureSeederLanguages.cs"));

		Assert.IsFalse(
			sharedSource.Contains("ScriptsDesignedLanguages.Remove", StringComparison.Ordinal),
			"A later pack must not delete a shared script's language memberships.");
		StringAssert.Contains(sharedSource, "SeederRepeatabilityHelper.EnsureLink(");
		StringAssert.Contains(languageSource, ".Concat(existingLanguages.Select(x => x.Name))");
	}

	[TestMethod]
	public void SharedScriptUpsert_UnionsLinksAndAcquisitionProgAcrossReruns()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.FutureProgs.Add(new DbFutureProg
		{
			FunctionName = "AlwaysTrue",
			FunctionComment = "Test helper",
			FunctionText = "return true",
			ReturnType = (long)ProgVariableTypes.Boolean,
			Category = "Test",
			Subcategory = "Test"
		});
		Language first = new()
		{
			Name = "First Language",
			UnknownLanguageDescription = "an unknown first language",
			DifficultyModel = 1,
			LanguageObfuscationFactor = 0.1
		};
		Language second = new()
		{
			Name = "Second Language",
			UnknownLanguageDescription = "an unknown second language",
			DifficultyModel = 1,
			LanguageObfuscationFactor = 0.1
		};
		context.Languages.AddRange(first, second);
		context.SaveChanges();

		CultureSeeder seeder = new();
		typeof(CultureSeeder)
			.GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, context);
		var languages = (Dictionary<string, Language>)typeof(CultureSeeder)
			.GetField("_languages", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(seeder)!;
		languages[first.Name] = first;
		languages[second.Name] = second;

		seeder.AddScript("Shared Test", "the shared test script", "an unknown script",
			"A script used to verify additive language membership.", "Alphabet", 1.0, 1.0, first.Name);
		seeder.AddScript("Shared Test", "the shared test script", "an unknown script",
			"A script used to verify additive language membership.", "Alphabet", 1.0, 1.0, second.Name);
		seeder.AddScript("Shared Test", "the shared test script", "an unknown script",
			"A script used to verify additive language membership.", "Alphabet", 1.0, 1.0, second.Name);

		Script script = context.Scripts.Single(x => x.Name == "Shared Test");
		string[] designedLanguages = context.ScriptsDesignedLanguages
			.Where(x => x.ScriptId == script.Id)
			.Select(x => x.Language.Name)
			.OrderBy(x => x)
			.ToArray();
		CollectionAssert.AreEqual(new[] { first.Name, second.Name }, designedLanguages);

		DbFutureProg prog = context.FutureProgs.Single(x => x.FunctionName == "CanPickSharedTestScriptKnowledge");
		StringAssert.Contains(prog.FunctionText, "case (\"First Language\")");
		StringAssert.Contains(prog.FunctionText, "case (\"Second Language\")");
	}

	[TestMethod]
	public void ModernScriptDefinitions_TargetTheirOwnLanguages()
	{
		string source = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CultureSeederLanguages.cs"));

		StringAssert.Contains(source, "\"Alphabet\", 0.75, 1.5, \"Korean\"");
		StringAssert.Contains(source, "\"Logographic\", 0.5, 2.0, \"Mandarin\", \"Yue\"");
		StringAssert.Contains(source, "\"Abjad\", 0.8, 1.2, \"Hebrew\"");
		StringAssert.Contains(source, "\"Abjad\", 0.8, 1.2, \"Arabic\", \"Farsi\"");
		StringAssert.Contains(source, "\"Abugida\", 0.8, 1.3, \"Hindi\"");
	}

	[TestMethod]
	public void SeedData_ExistingCultureScript_ReconcilesChargenFreeKnowledgeProg()
	{
		using FuturemudDatabaseContext context = BuildContext();
		DbFutureProg target = new()
		{
			Id = 1,
			FunctionName = ChargenFreeKnowledgeProgReconciler.ProgName,
			FunctionComment = "Free knowledges test prog",
			FunctionText = "var knowledges as knowledge collection\n// Preserve this builder rule\nreturn @knowledges",
			ReturnTypeDefinition = (ProgVariableTypes.Knowledge | ProgVariableTypes.Collection).ToStorageString(),
			Category = "Chargen",
			Subcategory = "Skills",
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		target.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = target,
			FutureProgId = target.Id,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterTypeDefinition = ProgVariableTypes.Toon.ToStorageString()
		});
		DbFutureProg scriptGate = new()
		{
			Id = 2,
			FunctionName = "CanPickLatinScriptKnowledge",
			FunctionComment = "Latin test gate",
			FunctionText = "return @skill.Name == \"Latin\"",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
			Category = "Knowledges",
			Subcategory = "Scripts",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		scriptGate.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = scriptGate,
			FutureProgId = scriptGate.Id,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterTypeDefinition = ProgVariableTypes.Chargen.ToStorageString()
		});
		scriptGate.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = scriptGate,
			FutureProgId = scriptGate.Id,
			ParameterIndex = 1,
			ParameterName = "skill",
			ParameterTypeDefinition = ProgVariableTypes.Trait.ToStorageString()
		});
		Knowledge scriptKnowledge = new()
		{
			Id = 1,
			Name = "Latin Script",
			Description = "Latin Script",
			LongDescription = "Latin Script",
			Type = "Script",
			Subtype = "Alphabet",
			LearnableType = 0,
			LearnDifficulty = 0,
			TeachDifficulty = 0,
			LearningSessionsRequired = 0,
			CanAcquireProg = scriptGate,
			CanAcquireProgId = scriptGate.Id
		};
		context.FutureProgs.AddRange(target, scriptGate);
		context.Knowledges.Add(scriptKnowledge);
		context.Scripts.Add(new Script
		{
			Id = 1,
			Name = "Latin",
			KnownScriptDescription = "Latin",
			UnknownScriptDescription = "Latin",
			Knowledge = scriptKnowledge,
			KnowledgeId = scriptKnowledge.Id,
			DocumentLengthModifier = 1.0,
			InkUseModifier = 1.0
		});
		context.SaveChanges();

		string result = new CultureSeeder().SeedData(context, new Dictionary<string, string>
		{
			["culturepacks"] = "none"
		});

		StringAssert.Contains(result, "Updated ChargenFreeKnowledges");
		StringAssert.Contains(target.FunctionText, "// Preserve this builder rule");
		StringAssert.Contains(target.FunctionText, ChargenFreeKnowledgeProgReconciler.CultureStartMarker);
		StringAssert.Contains(target.FunctionText,
			"@ch.Skills.Any(skill, @skill.Name == \"Literacy\") and @ch.Skills.Any(skill, @CanPickLatinScriptKnowledge(@ch, @skill))");
		StringAssert.Contains(target.FunctionText, "ToKnowledge(\"Latin Script\")");
		Assert.AreEqual(ProgVariableTypes.Chargen.ToStorageString(),
			target.FutureProgsParameters.Single().ParameterTypeDefinition);
	}

	private static IReadOnlyDictionary<string, string> Answers(string culturePack)
	{
		return new Dictionary<string, string>
		{
			["culturepacks"] = culturePack
		};
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options =
			new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
				.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static object? InvokePrivate(object target, string methodName, params object[] args)
	{
		return target.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(target, args);
	}

	private static string GetSourcePath(params string[] parts)
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "DatabaseSeeder")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate the repository root from the test output directory.");
		return Path.Combine(new[] { directory.FullName }.Concat(parts).ToArray());
	}
}
