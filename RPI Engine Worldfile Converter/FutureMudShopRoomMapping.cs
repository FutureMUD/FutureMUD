#nullable enable

using System.Text.Json;

namespace RPI_Engine_Worldfile_Converter;

public static class FutureMudShopRoomMapping
{
	public static IReadOnlyDictionary<int, long> Read(string path, IEnumerable<ConvertedRoomDefinition> sourceRooms)
	{
		using var stream = File.OpenRead(path);
		using var audit = JsonDocument.Parse(stream);
		return FromAudit(audit.RootElement, sourceRooms);
	}

	public static IReadOnlyDictionary<int, long> FromAudit(JsonElement audit, IEnumerable<ConvertedRoomDefinition> sourceRooms)
	{
		if (!audit.TryGetProperty("Execute", out var execute) || execute.ValueKind != JsonValueKind.True ||
		    !audit.TryGetProperty("Rooms", out var rooms) || rooms.ValueKind != JsonValueKind.Array)
		{
			throw new InvalidDataException("Shop import requires an executed apply-rooms audit with room mappings.");
		}

		var sourceByVnum = sourceRooms.GroupBy(x => x.Vnum)
			.Where(x => x.Count() == 1)
			.ToDictionary(x => x.Key, x => x.Single().SourceKey);
		var candidates = new List<(int Vnum, long RoomId)>();
		foreach (var room in rooms.EnumerateArray())
		{
			if (room.ValueKind != JsonValueKind.Object ||
			    !room.TryGetProperty("Vnum", out var vnumValue) || vnumValue.ValueKind != JsonValueKind.Number || !vnumValue.TryGetInt32(out var vnum) ||
			    !sourceByVnum.TryGetValue(vnum, out var sourceKey) ||
			    !room.TryGetProperty("SourceKey", out var source) || source.ValueKind != JsonValueKind.String ||
			    !string.Equals(source.GetString(), sourceKey, StringComparison.OrdinalIgnoreCase) ||
			    !room.TryGetProperty("Action", out var action) || action.ValueKind != JsonValueKind.String || action.GetString() != "created")
			{
				continue;
			}

			// Historical RoomId named a removed parent. CellId is its surviving playable child.
			JsonElement id;
			if (room.TryGetProperty("CellId", out var cellId))
			{
				id = cellId;
			}
			else if (room.TryGetProperty("LegacyRoomId", out var legacyId) && legacyId.ValueKind == JsonValueKind.Null &&
			         room.TryGetProperty("RoomId", out var roomId))
			{
				id = roomId;
			}
			else
			{
				continue;
			}

			if (id.ValueKind == JsonValueKind.Number && id.TryGetInt64(out var actualId) && actualId > 0)
			{
				candidates.Add((vnum, actualId));
			}
		}

		var duplicateRoomIds = candidates.GroupBy(x => x.RoomId)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToHashSet();
		return candidates.GroupBy(x => x.Vnum)
			.Where(x => x.Count() == 1 && !duplicateRoomIds.Contains(x.Single().RoomId))
			.ToDictionary(x => x.Key, x => x.Single().RoomId);
	}
}
