using System.Collections.ObjectModel;

namespace TerrainPlanner.Contracts;

public sealed class PlannerMap
{
	public const int MaximumDimension = 200;
	private readonly PlannerRoom[] _cells;

	public PlannerMap(int width, int height)
	{
		ValidateDimensions(width, height);
		Width = width;
		Height = height;
		_cells = Enumerable.Range(0, checked(width * height))
			.Select(index => new PlannerRoom(index % width, index / width))
			.ToArray();
	}

	public int Width { get; }
	public int Height { get; }
	public IReadOnlyList<PlannerRoom> Rooms => new ReadOnlyCollection<PlannerRoom>(_cells);

	public PlannerRoom RoomAt(int x, int y)
	{
		if (!Contains(x, y))
		{
			throw new ArgumentOutOfRangeException(nameof(x), $"Coordinate ({x}, {y}) is outside the map.");
		}

		return _cells[x + y * Width];
	}

	public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

	public PlannerMap Resize(int width, int height)
	{
		ValidateDimensions(width, height);
		var resized = new PlannerMap(width, height);
		for (var y = 0; y < Math.Min(Height, height); y++)
		{
			for (var x = 0; x < Math.Min(Width, width); x++)
			{
				resized.RoomAt(x, y).Restore(RoomAt(x, y).Snapshot());
			}
		}

		return resized;
	}

	public MapChangeSet PaintTerrain(IEnumerable<GridCoordinate> coordinates, long terrainId)
	{
		var edits = new MapChangeBuilder(this);
		foreach (var coordinate in DistinctValid(coordinates))
		{
			var room = RoomAt(coordinate.X, coordinate.Y);
			edits.CaptureBefore(room);
			room.SetTerrain(terrainId);
			edits.CaptureAfter(room);
		}

		return edits.Build();
	}

	public MapChangeSet PaintTag(IEnumerable<GridCoordinate> coordinates, long tagId, bool add)
	{
		if (tagId <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(tagId));
		}

		var edits = new MapChangeBuilder(this);
		foreach (var coordinate in DistinctValid(coordinates))
		{
			var room = RoomAt(coordinate.X, coordinate.Y);
			if (room.TerrainId == 0)
			{
				continue;
			}

			edits.CaptureBefore(room);
			if (add)
			{
				room.AddTag(tagId);
			}
			else
			{
				room.RemoveTag(tagId);
			}
			edits.CaptureAfter(room);
		}

		return edits.Build();
	}

	public MapChangeSet FillTerrain(GridCoordinate origin, long terrainId)
	{
		var target = RoomAt(origin.X, origin.Y).TerrainId;
		if (target == terrainId)
		{
			return MapChangeSet.Empty;
		}

		var coordinates = Flood(origin, room => room.TerrainId == target);
		return PaintTerrain(coordinates, terrainId);
	}

	public MapChangeSet FillTag(GridCoordinate origin, long tagId, bool add)
	{
		var start = RoomAt(origin.X, origin.Y);
		if (start.TerrainId == 0)
		{
			return MapChangeSet.Empty;
		}

		var targetTerrain = start.TerrainId;
		var targetPresence = start.TagIds.Contains(tagId);
		var coordinates = Flood(origin,
			room => room.TerrainId == targetTerrain && room.TagIds.Contains(tagId) == targetPresence);
		return PaintTag(coordinates, tagId, add);
	}

	public MapChangeSet PaintRectangle(GridCoordinate first, GridCoordinate second, PlannerLayer layer,
		long value, bool add = true)
	{
		var minX = Math.Min(first.X, second.X);
		var maxX = Math.Max(first.X, second.X);
		var minY = Math.Min(first.Y, second.Y);
		var maxY = Math.Max(first.Y, second.Y);
		var coordinates = from y in Enumerable.Range(minY, maxY - minY + 1)
			from x in Enumerable.Range(minX, maxX - minX + 1)
			select new GridCoordinate(x, y);
		return layer == PlannerLayer.Terrain
			? PaintTerrain(coordinates, value)
			: PaintTag(coordinates, value, add);
	}

	public MapChangeSet Clear(PlannerLayer? layer = null)
	{
		var edits = new MapChangeBuilder(this);
		foreach (var room in _cells)
		{
			edits.CaptureBefore(room);
			switch (layer)
			{
				case PlannerLayer.Tags:
					room.ClearTags();
					break;
				case PlannerLayer.Terrain:
				case null:
					room.SetTerrain(0);
					break;
			}
			edits.CaptureAfter(room);
		}

		return edits.Build();
	}

	public PlannerProject ToProject(string name, string? catalogueRevision,
		IReadOnlyDictionary<long, TagCatalogueItem> tags, IReadOnlyDictionary<long, string> tagColours)
	{
		return new PlannerProject
		{
			Name = name,
			Width = Width,
			Height = Height,
			CatalogueRevision = catalogueRevision,
			TagColours = tagColours.ToDictionary(),
			Rooms = _cells.Select(room => new PlannerProjectRoom
			{
				X = room.X,
				Y = room.Y,
				TerrainId = room.TerrainId,
				Tags = room.TagIds
					.Select(id => new PlannerTagReference(id, tags.GetValueOrDefault(id)?.ShortName ?? $"Missing tag #{id}"))
					.ToList(),
				UnresolvedFeatures = room.UnresolvedFeatures.ToList()
			}).ToList()
		};
	}

	public static PlannerMap FromProject(PlannerProject project)
	{
		if (project.SchemaVersion != PlannerProject.CurrentSchemaVersion)
		{
			throw new InvalidDataException($"Planner project schema {project.SchemaVersion} is not supported.");
		}

		var map = new PlannerMap(project.Width, project.Height);
		foreach (var projectRoom in project.Rooms)
		{
			if (!map.Contains(projectRoom.X, projectRoom.Y))
			{
				throw new InvalidDataException($"Project cell ({projectRoom.X}, {projectRoom.Y}) is outside the map.");
			}

			map.RoomAt(projectRoom.X, projectRoom.Y).Restore(new PlannerRoomState(
				projectRoom.TerrainId,
				projectRoom.Tags.Select(tag => tag.Id).Distinct().Order().ToArray(),
				projectRoom.UnresolvedFeatures.Distinct(StringComparer.OrdinalIgnoreCase).Order().ToArray()));
		}

		return map;
	}

	private IEnumerable<GridCoordinate> DistinctValid(IEnumerable<GridCoordinate> coordinates) =>
		coordinates.Where(coordinate => Contains(coordinate.X, coordinate.Y)).Distinct();

	private IReadOnlyList<GridCoordinate> Flood(GridCoordinate origin, Func<PlannerRoom, bool> matches)
	{
		var result = new List<GridCoordinate>();
		var queued = new Queue<GridCoordinate>();
		var visited = new HashSet<GridCoordinate>();
		queued.Enqueue(origin);
		while (queued.TryDequeue(out var coordinate))
		{
			if (!visited.Add(coordinate) || !Contains(coordinate.X, coordinate.Y))
			{
				continue;
			}

			var room = RoomAt(coordinate.X, coordinate.Y);
			if (!matches(room))
			{
				continue;
			}

			result.Add(coordinate);
			queued.Enqueue(new GridCoordinate(coordinate.X - 1, coordinate.Y));
			queued.Enqueue(new GridCoordinate(coordinate.X + 1, coordinate.Y));
			queued.Enqueue(new GridCoordinate(coordinate.X, coordinate.Y - 1));
			queued.Enqueue(new GridCoordinate(coordinate.X, coordinate.Y + 1));
		}

		return result;
	}

	private static void ValidateDimensions(int width, int height)
	{
		if (width is < 1 or > MaximumDimension)
		{
			throw new ArgumentOutOfRangeException(nameof(width), $"Width must be from 1 to {MaximumDimension}.");
		}

		if (height is < 1 or > MaximumDimension)
		{
			throw new ArgumentOutOfRangeException(nameof(height), $"Height must be from 1 to {MaximumDimension}.");
		}
	}
}

public sealed class PlannerRoom
{
	private readonly HashSet<long> _tagIds = [];
	private readonly HashSet<string> _unresolvedFeatures = new(StringComparer.OrdinalIgnoreCase);

	internal PlannerRoom(int x, int y)
	{
		X = x;
		Y = y;
	}

	public int X { get; }
	public int Y { get; }
	public long TerrainId { get; private set; }
	public IReadOnlySet<long> TagIds => _tagIds;
	public IReadOnlySet<string> UnresolvedFeatures => _unresolvedFeatures;

	internal void SetTerrain(long terrainId)
	{
		TerrainId = Math.Max(0, terrainId);
		if (TerrainId == 0)
		{
			ClearTags();
		}
	}

	internal void AddTag(long tagId) => _tagIds.Add(tagId);
	internal void RemoveTag(long tagId) => _tagIds.Remove(tagId);
	internal void AddUnresolvedFeature(string feature) => _unresolvedFeatures.Add(feature);
	internal void ClearTags()
	{
		_tagIds.Clear();
		_unresolvedFeatures.Clear();
	}

	internal PlannerRoomState Snapshot() => new(
		TerrainId,
		_tagIds.Order().ToArray(),
		_unresolvedFeatures.Order(StringComparer.OrdinalIgnoreCase).ToArray());

	internal void Restore(PlannerRoomState state)
	{
		TerrainId = state.TerrainId;
		_tagIds.Clear();
		_tagIds.UnionWith(state.TagIds);
		_unresolvedFeatures.Clear();
		_unresolvedFeatures.UnionWith(state.UnresolvedFeatures);
		if (TerrainId == 0)
		{
			ClearTags();
		}
	}
}

public sealed record PlannerRoomState(long TerrainId, long[] TagIds, string[] UnresolvedFeatures);

public sealed record MapRoomChange(int X, int Y, PlannerRoomState Before, PlannerRoomState After);

public sealed class MapChangeSet
{
	public static MapChangeSet Empty { get; } = new([]);

	public MapChangeSet(IReadOnlyList<MapRoomChange> changes)
	{
		Changes = changes;
	}

	public IReadOnlyList<MapRoomChange> Changes { get; }
	public bool HasChanges => Changes.Count > 0;

	public static MapChangeSet Merge(IEnumerable<MapChangeSet> changeSets)
	{
		var merged = new Dictionary<GridCoordinate, MapRoomChange>();
		foreach (var change in changeSets.SelectMany(item => item.Changes))
		{
			var coordinate = new GridCoordinate(change.X, change.Y);
			merged[coordinate] = merged.TryGetValue(coordinate, out var existing)
				? new MapRoomChange(change.X, change.Y, existing.Before, change.After)
				: change;
		}

		var changes = merged.Values
			.Where(change => change.Before.TerrainId != change.After.TerrainId ||
				!change.Before.TagIds.SequenceEqual(change.After.TagIds) ||
				!change.Before.UnresolvedFeatures.SequenceEqual(change.After.UnresolvedFeatures))
			.ToList();
		return changes.Count == 0 ? Empty : new MapChangeSet(changes);
	}

	public void Undo(PlannerMap map)
	{
		foreach (var change in Changes)
		{
			map.RoomAt(change.X, change.Y).Restore(change.Before);
		}
	}

	public void Redo(PlannerMap map)
	{
		foreach (var change in Changes)
		{
			map.RoomAt(change.X, change.Y).Restore(change.After);
		}
	}
}

internal sealed class MapChangeBuilder
{
	private readonly PlannerMap _map;
	private readonly Dictionary<GridCoordinate, PlannerRoomState> _before = [];
	private readonly Dictionary<GridCoordinate, PlannerRoomState> _after = [];

	public MapChangeBuilder(PlannerMap map)
	{
		_map = map;
	}

	public void CaptureBefore(PlannerRoom room) =>
		_before.TryAdd(new GridCoordinate(room.X, room.Y), room.Snapshot());

	public void CaptureAfter(PlannerRoom room) =>
		_after[new GridCoordinate(room.X, room.Y)] = room.Snapshot();

	public MapChangeSet Build()
	{
		var changes = _before
			.Select(pair => new MapRoomChange(pair.Key.X, pair.Key.Y, pair.Value, _after[pair.Key]))
			.Where(change => !Equals(change.Before, change.After) &&
				(change.Before.TerrainId != change.After.TerrainId ||
				 !change.Before.TagIds.SequenceEqual(change.After.TagIds) ||
				 !change.Before.UnresolvedFeatures.SequenceEqual(change.After.UnresolvedFeatures)))
			.ToList();
		return changes.Count == 0 ? MapChangeSet.Empty : new MapChangeSet(changes);
	}
}
