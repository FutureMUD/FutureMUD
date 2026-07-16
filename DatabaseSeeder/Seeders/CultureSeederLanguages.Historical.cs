#nullable enable

using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private sealed record HistoricalAccentSeed(
		string Name,
		string Group,
		string Description,
		Difficulty Difficulty,
		string? Suffix = null,
		string? VagueSuffix = null);

	private sealed record HistoricalLanguageSeed(
		string Name,
		string UnknownDescription,
		HistoricalAccentSeed[] Accents);

	private sealed record HistoricalScriptSeed(
		string Name,
		string KnownDescription,
		string UnknownDescription,
		string Description,
		string Subtype,
		double DocumentLengthModifier,
		double InkUseModifier,
		string[] Languages);

	private sealed record HistoricalMutualIntelligibilitySeed(
		string FirstLanguage,
		string SecondLanguage,
		Difficulty Difficulty,
		bool TwoWay = true);

	internal static IReadOnlyCollection<string> DarkAgesAndMedievalNameCultureNamesForTesting =>
		MedievalPeriodSeeds
			.Select(x => x.NameCultureName)
			.ToArray();

	internal static IReadOnlyCollection<string> RenaissanceWorldNameCultureNamesForTesting =>
		RenaissanceWorldSeeds
			.Select(x => x.NameCultureName)
			.ToArray();

	internal static IReadOnlyDictionary<string, string[]> DarkAgesAndMedievalLanguageCoverageForTesting =>
		DarkAgesAndMedievalLanguageCoverage;

	internal static IReadOnlyDictionary<string, string[]> RenaissanceWorldLanguageCoverageForTesting =>
		RenaissanceWorldLanguageCoverage;

	internal static IReadOnlyCollection<string> DarkAgesAndMedievalLanguageNamesForTesting =>
		DarkAgesAndMedievalLanguages
			.Select(x => x.Name)
			.ToArray();

	internal static IReadOnlyCollection<string> RenaissanceWorldLanguageNamesForTesting =>
		RenaissanceWorldLanguages
			.Select(x => x.Name)
			.ToArray();

	internal static IReadOnlyList<string> ValidateHistoricalLanguageCataloguesForTesting()
	{
		List<string> issues = [];
		ValidateHistoricalLanguagePack(
			"Earth-DarkAgesAndMedieval",
			DarkAgesAndMedievalLanguageCoverage,
			DarkAgesAndMedievalLanguages,
			DarkAgesAndMedievalScripts,
			DarkAgesAndMedievalMutualIntelligibilities,
			issues);
		ValidateHistoricalLanguagePack(
			"Earth-RenaissanceWorldExpansion",
			RenaissanceWorldLanguageCoverage,
			RenaissanceWorldLanguages,
			RenaissanceWorldScripts,
			RenaissanceWorldMutualIntelligibilities,
			issues);
		ValidateHistoricalLanguagePack(
			"Earth-Modern coverage expansion",
			ModernLanguageCoverageExpansionMap,
			ModernLanguageCoverageExpansion,
			ModernScriptCoverageExpansion,
			ModernMutualIntelligibilityExpansion,
			issues);
		ValidateHistoricalLanguagePack(
			"Earth-Antiquity coverage expansion",
			AntiquityLanguageCoverageExpansionMap,
			AntiquityLanguageCoverageExpansion,
			AntiquityScriptCoverageExpansion,
			AntiquityMutualIntelligibilityExpansion,
			issues);
		ValidateHistoricalLanguagePack(
			"Earth-RenaissanceEurope coverage expansion",
			RenaissanceEuropeLanguageCoverageExpansionMap,
			RenaissanceEuropeLanguageCoverageExpansion,
			RenaissanceEuropeScriptCoverageExpansion,
			[],
			issues);
		return issues;
	}

	private static HistoricalAccentSeed HistoricalAccent(
		string name,
		string group,
		string description,
		Difficulty difficulty = Difficulty.VeryEasy,
		string? suffix = null,
		string? vagueSuffix = null)
	{
		return new HistoricalAccentSeed(name, group, description, difficulty, suffix, vagueSuffix);
	}

	private void SeedHistoricalLanguagePack(
		HistoricalLanguageSeed[] languages,
		HistoricalScriptSeed[] scripts,
		HistoricalMutualIntelligibilitySeed[] mutualIntelligibilities)
	{
		foreach (HistoricalLanguageSeed language in languages)
		{
			AddLanguage(language.Name, language.UnknownDescription);
			foreach (HistoricalAccentSeed accent in language.Accents)
			{
				AddAccent(
					language.Name,
					accent.Name,
					accent.Suffix ?? $"in the {accent.Name} variety",
					accent.VagueSuffix ?? $"in a {accent.Group} variety",
					accent.Difficulty,
					accent.Description,
					accent.Group);
			}

			AddAccent(
				language.Name,
				"crude",
				"with a crude accent",
				"with a crude accent",
				Difficulty.Hard,
				$"A crude accent typical of someone who has only recently learned {language.Name}.",
				"crude");
		}

		foreach (HistoricalMutualIntelligibilitySeed mutual in mutualIntelligibilities)
		{
			AddMutualIntelligability(
				mutual.FirstLanguage,
				mutual.SecondLanguage,
				mutual.Difficulty,
				mutual.TwoWay);
		}

		foreach (HistoricalScriptSeed script in scripts)
		{
			AddScript(
				script.Name,
				script.KnownDescription,
				script.UnknownDescription,
				script.Description,
				script.Subtype,
				script.DocumentLengthModifier,
				script.InkUseModifier,
				script.Languages);
		}
	}

	private static void ValidateHistoricalLanguagePack(
		string packName,
		IReadOnlyDictionary<string, string[]> coverage,
		HistoricalLanguageSeed[] languages,
		HistoricalScriptSeed[] scripts,
		HistoricalMutualIntelligibilitySeed[] mutualIntelligibilities,
		ICollection<string> issues)
	{
		HashSet<string> languageNames = new(StringComparer.OrdinalIgnoreCase);
		foreach (HistoricalLanguageSeed language in languages)
		{
			if (!languageNames.Add(language.Name))
			{
				issues.Add($"{packName} defines the language {language.Name} more than once.");
			}

			if (language.Accents.Length == 0)
			{
				issues.Add($"{packName} language {language.Name} has no historical native accent or dialect.");
			}

			if (language.Accents
					.Select(x => x.Name)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.Count() != language.Accents.Length)
			{
				issues.Add($"{packName} language {language.Name} repeats an accent name.");
			}
		}

		foreach ((string culture, string[] cultureLanguages) in coverage)
		{
			if (cultureLanguages.Length == 0)
			{
				issues.Add($"{packName} naming culture {culture} has no language coverage.");
			}

			foreach (string language in cultureLanguages.Where(x => !languageNames.Contains(x)))
			{
				issues.Add($"{packName} naming culture {culture} references undefined language {language}.");
			}
		}

		HashSet<string> scriptNames = new(StringComparer.OrdinalIgnoreCase);
		foreach (HistoricalScriptSeed script in scripts)
		{
			if (!scriptNames.Add(script.Name))
			{
				issues.Add($"{packName} defines the script {script.Name} more than once.");
			}

			foreach (string language in script.Languages.Where(x => !languageNames.Contains(x)))
			{
				issues.Add($"{packName} script {script.Name} references undefined language {language}.");
			}
		}

		foreach (HistoricalMutualIntelligibilitySeed mutual in mutualIntelligibilities)
		{
			if (!languageNames.Contains(mutual.FirstLanguage))
			{
				issues.Add($"{packName} mutual intelligibility references undefined language {mutual.FirstLanguage}.");
			}

			if (!languageNames.Contains(mutual.SecondLanguage))
			{
				issues.Add($"{packName} mutual intelligibility references undefined language {mutual.SecondLanguage}.");
			}

			if (mutual.FirstLanguage.Equals(mutual.SecondLanguage, StringComparison.OrdinalIgnoreCase))
			{
				issues.Add($"{packName} mutual intelligibility links {mutual.FirstLanguage} to itself.");
			}
		}
	}
}
