#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record EraCatalogueItemSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		string[] Tags,
		string[] Components,
		string BuilderNotes,
		bool Skinnable = false,
		bool HideFromPlayers = false);

	private void SeedStraightforwardEraCatalogueItems(
		string catalogueName,
		IEnumerable<EraCatalogueItemSpec> specifications)
	{
		var specs = specifications.ToArray();
		var issues = specs
			.Where(x => !_materials.ContainsKey(x.Material))
			.Select(x => $"Missing material {x.Material} for {x.StableReference}")
			.Concat(specs.SelectMany(x => x.Tags
				.Where(tag => !_tagsByFullPath.ContainsKey(tag))
				.Select(tag => $"Missing tag {tag} for {x.StableReference}")))
			.Concat(specs.SelectMany(x => x.Components
				.Where(component => !_components.ContainsKey(component))
				.Select(component => $"Missing component {component} for {x.StableReference}")))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		if (issues.Length > 0)
		{
			throw new InvalidOperationException(
				$"{catalogueName} cannot be seeded because required dependencies are missing:{Environment.NewLine}" +
				string.Join(Environment.NewLine, issues.Select(x => $" - {x}")));
		}

		foreach (var spec in specs)
		{
			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				spec.Skinnable,
				spec.HideFromPlayers,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes,
				allowLegacyShortDescriptionMatch: false);
		}
	}
}
