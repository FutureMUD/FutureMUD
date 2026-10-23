#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{


	internal sealed record RenaissanceHouseholdItemSpecTestData(
		string StableReference,
		string ShortDescription,
		string? LongDescription,
		string FullDescription,
		string Size,
		string Quality,
		string Material,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components);

	internal static IReadOnlyCollection<RenaissanceHouseholdItemSpecTestData> RenaissanceHouseholdItemSpecsForTesting =>
		RenaissanceHouseholdItemSpecs
			.Select(x => new RenaissanceHouseholdItemSpecTestData(
				x.Metadata.StableKey,
				x.ShortDescription,
				x.LongDescription,
				x.FullDescription,
				x.Size.ToString(),
				x.Quality.ToString(),
				x.Material,
				x.Tags,
				x.Components))
			.ToArray();

	private void SeedRenaissanceHouseholdUrbanAndTrade()
	{
		var dependencyIssues = ValidateRenaissanceHouseholdDependencies(RenaissanceHouseholdItemSpecs);
		if (!_manifestCaptureOnly && dependencyIssues.Count > 0)
		{
			throw new InvalidOperationException(
				"Renaissance household catalogue cannot be seeded because required dependencies are missing:" +
				Environment.NewLine + string.Join(Environment.NewLine, dependencyIssues.Select(x => $" - {x}")));
		}

		foreach (var spec in RenaissanceHouseholdItemSpecs)
		{
			InstallCatalogueItem(spec);
		}
	}

	private IReadOnlyList<string> ValidateRenaissanceHouseholdDependencies(
		IEnumerable<CatalogueItemDefinition> specs)
	{
		var issues = new List<string>();
		foreach (var spec in specs)
		{
			if (!_materials.ContainsKey(spec.Material))
			{
				issues.Add($"Missing material {spec.Material} for {spec.Metadata.StableKey}");
			}

			issues.AddRange(spec.Tags
				.Where(x => !_tagsByFullPath.ContainsKey(x))
				.Select(x => $"Missing tag {x} for {spec.Metadata.StableKey}"));
			issues.AddRange(spec.Components
				.Where(x => !_components.ContainsKey(x))
				.Select(x => $"Missing component {x} for {spec.Metadata.StableKey}"));
		}

		return issues
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}
}
