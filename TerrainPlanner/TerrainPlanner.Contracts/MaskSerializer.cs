namespace TerrainPlanner.Contracts;

public static class MaskSerializer
{
	public static string ExportTerrainMask(PlannerMap map) => string.Join(',', map.Rooms.Select(room => room.TerrainId));

	public static void ImportTerrainMask(PlannerMap map, string mask, IReadOnlySet<long> validTerrainIds)
	{
		var values = SplitRooms(mask);
		EnsureRoomCount(map, values.Length, "terrain");
		for (var index = 0; index < values.Length; index++)
		{
			if (!long.TryParse(values[index].Trim(), out var terrainId) || terrainId < 0 ||
				(terrainId != 0 && !validTerrainIds.Contains(terrainId)))
			{
				throw new InvalidDataException($"Terrain mask entry {index + 1} is not a known terrain ID.");
			}

			map.RoomAt(index % map.Width, index / map.Width).SetTerrain(terrainId);
		}
	}

	public static string ExportFeatureMask(PlannerMap map)
	{
		return string.Join(',', map.Rooms.Select(room =>
		{
			if (room.TerrainId == 0)
			{
				return string.Empty;
			}

			var tagIds = room.TagIds
				.Concat(room.UnresolvedFeatures.Select(ParseFeatureTagId))
				.Distinct()
				.Order();
			return string.Join('|', tagIds);
		}));
	}

	public static void ImportFeatureMask(PlannerMap map, string mask,
		IReadOnlyDictionary<long, TagCatalogueItem> tagsById)
	{
		var values = SplitRooms(mask);
		EnsureRoomCount(map, values.Length, "feature");
		for (var index = 0; index < values.Length; index++)
		{
			var room = map.RoomAt(index % map.Width, index / map.Width);
			room.ClearTags();
			if (room.TerrainId == 0 || string.IsNullOrWhiteSpace(values[index]))
			{
				continue;
			}

			foreach (var value in values[index].Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
			{
				var tagId = ParseFeatureTagId(value);
				if (tagsById.ContainsKey(tagId))
				{
					room.AddTag(tagId);
				}
				else
				{
					room.AddUnresolvedFeature(tagId.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}
		}
	}

	public static long ParseFeatureTagId(string value)
	{
		if (!long.TryParse(value?.Trim(), System.Globalization.NumberStyles.None,
			System.Globalization.CultureInfo.InvariantCulture, out var tagId) || tagId <= 0)
		{
			throw new InvalidDataException($"Feature tag ID '{value}' must be a positive integer.");
		}

		return tagId;
	}

	private static string[] SplitRooms(string mask) =>
		(mask ?? string.Empty).Trim().Split(',', StringSplitOptions.None);

	private static void EnsureRoomCount(PlannerMap map, int count, string noun)
	{
		if (count != map.Width * map.Height)
		{
			throw new InvalidDataException($"The {noun} mask has {count} entries; the map requires {map.Width * map.Height}.");
		}
	}
}
