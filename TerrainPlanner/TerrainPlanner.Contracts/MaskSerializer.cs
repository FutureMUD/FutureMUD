namespace TerrainPlanner.Contracts;

public static class MaskSerializer
{
	public static string ExportTerrainMask(PlannerMap map) => string.Join(',', map.Cells.Select(cell => cell.TerrainId));

	public static void ImportTerrainMask(PlannerMap map, string mask, IReadOnlySet<long> validTerrainIds)
	{
		var values = SplitCells(mask);
		EnsureCellCount(map, values.Length, "terrain");
		for (var index = 0; index < values.Length; index++)
		{
			if (!long.TryParse(values[index].Trim(), out var terrainId) || terrainId < 0 ||
				(terrainId != 0 && !validTerrainIds.Contains(terrainId)))
			{
				throw new InvalidDataException($"Terrain mask entry {index + 1} is not a known terrain ID.");
			}

			map.CellAt(index % map.Width, index / map.Width).SetTerrain(terrainId);
		}
	}

	public static string ExportFeatureMask(PlannerMap map, IReadOnlyDictionary<long, TagCatalogueItem> tags)
	{
		return string.Join(',', map.Cells.Select(cell =>
		{
			if (cell.TerrainId == 0)
			{
				return string.Empty;
			}

			var names = cell.TagIds.Select(id => tags.TryGetValue(id, out var tag)
					? ValidateFeatureName(tag.ShortName)
					: throw new InvalidDataException($"Tag #{id} no longer exists in the live catalogue."))
				.Concat(cell.UnresolvedFeatures.Select(ValidateFeatureName))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Order(StringComparer.OrdinalIgnoreCase);
			return string.Join('|', names);
		}));
	}

	public static void ImportFeatureMask(PlannerMap map, string mask,
		IReadOnlyDictionary<string, TagCatalogueItem> tagsByName)
	{
		var values = SplitCells(mask);
		EnsureCellCount(map, values.Length, "feature");
		for (var index = 0; index < values.Length; index++)
		{
			var cell = map.CellAt(index % map.Width, index / map.Width);
			cell.ClearTags();
			if (cell.TerrainId == 0 || string.IsNullOrWhiteSpace(values[index]))
			{
				continue;
			}

			foreach (var feature in values[index].Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
			{
				ValidateFeatureName(feature);
				if (tagsByName.TryGetValue(feature, out var tag))
				{
					cell.AddTag(tag.Id);
				}
				else
				{
					cell.AddUnresolvedFeature(feature);
				}
			}
		}
	}

	public static string ValidateFeatureName(string name)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Contains(',') || name.Contains('|') ||
			name.Contains('\r') || name.Contains('\n'))
		{
			throw new InvalidDataException($"Feature name '{name}' cannot be represented in an autobuilder feature mask.");
		}

		return name;
	}

	private static string[] SplitCells(string mask) =>
		(mask ?? string.Empty).Trim().Split(',', StringSplitOptions.None);

	private static void EnsureCellCount(PlannerMap map, int count, string noun)
	{
		if (count != map.Width * map.Height)
		{
			throw new InvalidDataException($"The {noun} mask has {count} entries; the map requires {map.Width * map.Height}.");
		}
	}
}
