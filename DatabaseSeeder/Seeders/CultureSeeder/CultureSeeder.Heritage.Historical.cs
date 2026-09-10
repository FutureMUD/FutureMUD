#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private sealed record HistoricalEthnicityVariantSeed(
		string Name,
		string TemplateEthnicity,
		string NameCulture,
		string EthnicGroup,
		string EthnicSubgroup,
		string Description);

	private sealed record HistoricalCultureSeed(
		string Name,
		string MaleNameCulture,
		string FemaleNameCulture,
		string Description,
		string FallbackNameCulture = "Given and Family");

	internal static IReadOnlyDictionary<string, string[]> DarkAgesAndMedievalEthnicityCoverageForTesting =>
		BuildHistoricalEthnicityCoverage(
			MedievalPeriodSeeds.Select(x => (x.NameCultureName, x.EthnicityName)),
			DarkAgesAndMedievalEthnicityVariants);

	internal static IReadOnlyDictionary<string, string[]> RenaissanceWorldEthnicityCoverageForTesting =>
		BuildHistoricalEthnicityCoverage(
			RenaissanceWorldSeeds.Select(x => (x.NameCultureName, x.EthnicityName)),
			RenaissanceWorldEthnicityVariants);

	internal static IReadOnlyDictionary<string, string> DarkAgesAndMedievalEthnicityNameCulturesForTesting =>
		DarkAgesAndMedievalEthnicityCoverageForTesting
			.SelectMany(x => x.Value.Select(ethnicity => (ethnicity, x.Key)))
			.ToDictionary(x => x.ethnicity, x => x.Key, StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyDictionary<string, string> RenaissanceWorldEthnicityNameCulturesForTesting =>
		RenaissanceWorldEthnicityCoverageForTesting
			.SelectMany(x => x.Value.Select(ethnicity => (ethnicity, x.Key)))
			.ToDictionary(x => x.ethnicity, x => x.Key, StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyCollection<string> ModernBroadCultureNamesForTesting =>
		ModernBroadCultures.Select(x => x.Name).ToArray();

	internal static IReadOnlyCollection<string> RenaissanceEuropeBroadCultureNamesForTesting =>
		RenaissanceEuropeBroadCultures.Select(x => x.Name).ToArray();

	internal static IReadOnlyList<string> ValidateHistoricalHeritageCataloguesForTesting()
	{
		List<string> issues = [];
		ValidateHistoricalEthnicityVariants(
			"Earth-DarkAgesAndMedieval",
			MedievalPeriodSeeds.Select(x => x.EthnicityName),
			MedievalPeriodSeeds.Select(x => x.NameCultureName),
			DarkAgesAndMedievalEthnicityVariants,
			issues);
		ValidateHistoricalEthnicityVariants(
			"Earth-RenaissanceWorldExpansion",
			RenaissanceWorldSeeds.Select(x => x.EthnicityName),
			RenaissanceWorldSeeds.Select(x => x.NameCultureName),
			RenaissanceWorldEthnicityVariants,
			issues);
		ValidateHistoricalEthnicityVariants(
			"Earth-RenaissanceEurope",
			MedievalEuropeEthnicityNameCultureMappings.Keys,
			MedievalEuropeEthnicityNameCultureMappings
				.SelectMany(x => new[] { x.Value.Male, x.Value.Female })
				.Append("Albanian"),
			RenaissanceEuropeEthnicityVariants,
			issues);
		ValidateHistoricalCultures(
			"Earth-Modern",
			ModernBroadCultures,
			CreateModernEthnicityNameCultureMappings()
				.SelectMany(x => new[] { x.Value.Male, x.Value.Female }),
			issues);
		ValidateHistoricalCultures(
			"Earth-RenaissanceEurope",
			RenaissanceEuropeBroadCultures,
			MedievalEuropeEthnicityNameCultureMappings
				.SelectMany(x => new[] { x.Value.Male, x.Value.Female })
				.Append("Albanian"),
			issues);
		return issues;
	}

	private static IReadOnlyDictionary<string, string[]> BuildHistoricalEthnicityCoverage(
		IEnumerable<(string NameCulture, string Ethnicity)> baseEthnicities,
		IEnumerable<HistoricalEthnicityVariantSeed> variants)
	{
		return baseEthnicities
			.GroupBy(x => x.NameCulture, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => x.Select(y => y.Ethnicity)
					.Concat(variants.Where(y => y.NameCulture.Equals(x.Key, StringComparison.OrdinalIgnoreCase))
						.Select(y => y.Name))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToArray(),
				StringComparer.OrdinalIgnoreCase);
	}

	private void SeedHistoricalEthnicityVariants(IEnumerable<HistoricalEthnicityVariantSeed> variants)
	{
		foreach (HistoricalEthnicityVariantSeed seed in variants)
		{
			Ethnicity template = _context.Ethnicities
				.Include(x => x.EthnicitiesCharacteristics)
					.ThenInclude(x => x.CharacteristicDefinition)
				.Include(x => x.EthnicitiesCharacteristics)
					.ThenInclude(x => x.CharacteristicProfile)
				.Include(x => x.EthnicitiesNameCultures)
					.ThenInclude(x => x.NameCulture)
				.AsEnumerable()
				.First(x => x.Name.Equals(seed.TemplateEthnicity, StringComparison.OrdinalIgnoreCase));
			string bloodModel = _bloodModels
				.First(x => x.Value.Id == template.PopulationBloodModelId)
				.Key;

			AddEthnicity(
				_humanRace,
				seed.Name,
				seed.EthnicGroup,
				bloodModel,
				template.TolerableTemperatureFloorEffect,
				template.TolerableTemperatureCeilingEffect,
				seed.EthnicSubgroup,
				description: seed.Description);

			foreach (EthnicitiesCharacteristics characteristic in template.EthnicitiesCharacteristics)
			{
				AddEthnicityVariable(
					seed.Name,
					characteristic.CharacteristicDefinition.Name,
					characteristic.CharacteristicProfile.Name);
			}

			string nameCulture = ResolveHistoricalHeritageNameCulture(seed.NameCulture, template);
			ReplaceEthnicityNameLinks(
				_ethnicities[seed.Name],
				(Gender.Male, nameCulture),
				(Gender.Female, nameCulture),
				(Gender.Neuter, nameCulture),
				(Gender.NonBinary, nameCulture),
				(Gender.Indeterminate, nameCulture));
		}

		_context.SaveChanges();
	}

	private void SeedHistoricalCultures(IEnumerable<HistoricalCultureSeed> cultures)
	{
		foreach (HistoricalCultureSeed seed in cultures)
		{
			string maleNameCulture = ResolveHistoricalHeritageNameCulture(
				seed.MaleNameCulture,
				seed.FallbackNameCulture);
			string femaleNameCulture = ResolveHistoricalHeritageNameCulture(
				seed.FemaleNameCulture,
				seed.FallbackNameCulture);
			AddCulture(seed.Name, maleNameCulture, femaleNameCulture, seed.Description);
		}

		_context.SaveChanges();
	}

	private string ResolveHistoricalHeritageNameCulture(string preferred, Ethnicity template)
	{
		NameCulture? preferredCulture = _context.NameCultures
			.AsEnumerable()
			.FirstOrDefault(x => x.Name.Equals(preferred, StringComparison.OrdinalIgnoreCase));
		if (preferredCulture is not null)
		{
			return preferredCulture.Name;
		}

		NameCulture? templateCulture = template.EthnicitiesNameCultures
			.Select(x => x.NameCulture)
			.FirstOrDefault();
		return templateCulture?.Name ?? "Simple";
	}

	private string ResolveHistoricalHeritageNameCulture(string preferred, string fallback)
	{
		foreach (string candidate in new[] { preferred, fallback, "Simple" })
		{
			NameCulture? culture = _context.NameCultures
				.AsEnumerable()
				.FirstOrDefault(x => x.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase));
			if (culture is not null)
			{
				return culture.Name;
			}
		}

		throw new InvalidOperationException("Culture heritage seeding requires at least the Simple name culture.");
	}

	private static void ValidateHistoricalEthnicityVariants(
		string pack,
		IEnumerable<string> templateEthnicities,
		IEnumerable<string> nameCultures,
		IReadOnlyCollection<HistoricalEthnicityVariantSeed> variants,
		ICollection<string> issues)
	{
		HashSet<string> templates = templateEthnicities.ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> cultures = nameCultures.ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
		foreach (HistoricalEthnicityVariantSeed seed in variants)
		{
			if (!names.Add(seed.Name))
			{
				issues.Add($"{pack} defines ethnicity {seed.Name} more than once.");
			}

			if (!templates.Contains(seed.TemplateEthnicity))
			{
				issues.Add($"{pack} ethnicity {seed.Name} uses unknown template {seed.TemplateEthnicity}.");
			}

			if (!cultures.Contains(seed.NameCulture))
			{
				issues.Add($"{pack} ethnicity {seed.Name} uses unknown naming culture {seed.NameCulture}.");
			}

			if (string.IsNullOrWhiteSpace(seed.EthnicGroup) ||
				string.IsNullOrWhiteSpace(seed.EthnicSubgroup) ||
				string.IsNullOrWhiteSpace(seed.Description))
			{
				issues.Add($"{pack} ethnicity {seed.Name} is missing required catalogue prose.");
			}
		}
	}

	private static void ValidateHistoricalCultures(
		string pack,
		IReadOnlyCollection<HistoricalCultureSeed> cultures,
		IEnumerable<string> expectedNameCultures,
		ICollection<string> issues)
	{
		HashSet<string> expected = expectedNameCultures.ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> referenced = new(StringComparer.OrdinalIgnoreCase);
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
		foreach (HistoricalCultureSeed seed in cultures)
		{
			if (!names.Add(seed.Name))
			{
				issues.Add($"{pack} defines culture {seed.Name} more than once.");
			}

			if (string.IsNullOrWhiteSpace(seed.MaleNameCulture) ||
				string.IsNullOrWhiteSpace(seed.FemaleNameCulture) ||
				string.IsNullOrWhiteSpace(seed.Description))
			{
				issues.Add($"{pack} culture {seed.Name} is missing required catalogue data.");
			}

			foreach (string nameCulture in new[] { seed.MaleNameCulture, seed.FemaleNameCulture })
			{
				referenced.Add(nameCulture);
				if (!expected.Contains(nameCulture))
				{
					issues.Add($"{pack} culture {seed.Name} references unexpected naming culture {nameCulture}.");
				}
			}
		}

		foreach (string missing in expected.Where(x => !referenced.Contains(x)))
		{
			issues.Add($"{pack} naming culture {missing} has no broader culture coverage.");
		}
	}
}
