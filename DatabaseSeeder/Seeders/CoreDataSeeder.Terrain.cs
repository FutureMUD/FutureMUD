#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private static readonly (string Name, string Parent)[] StockTerrainTagDefinitions =
	[
		("Terrain", ""),
		("Wild", "Terrain"),
		("Human Influenced", "Terrain"),
		("Urban", "Human Influenced"),
		("Rural", "Human Influenced"),
		("Public", "Urban"),
		("Private", "Urban"),
		("Commercial", "Urban"),
		("Residential", "Urban"),
		("Administrative", "Urban"),
		("Industrial", "Urban"),
		("Natural", "Urban"),
		("Diggable Soil", "Terrain"),
		("Foragable Clay", "Terrain"),
		("Foragable Sand", "Terrain"),
		("Terrestrial", "Wild"),
		("Riparian", "Wild"),
		("Littoral", "Wild"),
		("Aquatic", "Wild")
	];

	internal static IReadOnlyCollection<string> StockTerrainTagNamesForTesting =>
		StockTerrainTagDefinitions.Select(x => x.Name).ToArray();

	internal static void SeedTerrainFoundationsForTesting(FuturemudDatabaseContext context)
	{
		SeedTerrainFoundationsCore(context);
	}

	private void SeedTerrainFoundations(FuturemudDatabaseContext context)
	{
		SeedTerrainFoundationsCore(context);
	}

	private static void SeedTerrainFoundationsCore(FuturemudDatabaseContext context)
	{
		var tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		foreach (var (name, parent) in StockTerrainTagDefinitions)
		{
			AddTerrainTag(context, tags, name, parent);
		}

		context.SaveChanges();
		UsefulSeeder.SeedStockTerrainCatalogue(context, tags);
	}

	private static void AddTerrainTag(FuturemudDatabaseContext context, DictionaryWithDefault<string, Tag> tags,
		string name, string parent)
	{
		if (tags.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			return;
		}

		var tag = new Tag
		{
			Name = name,
			Parent = string.IsNullOrEmpty(parent) ? null : tags[parent]
		};
		tags[name] = tag;
		context.Tags.Add(tag);
	}
}
