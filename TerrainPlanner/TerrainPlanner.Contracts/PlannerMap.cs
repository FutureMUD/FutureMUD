using System.Collections.ObjectModel;

namespace TerrainPlanner.Contracts;

public sealed class PlannerMap
{
	public const int MaximumDimension = 200;
	private readonly PlannerCell[] _cells;

	public PlannerMap(int width, int height)
	{
		ValidateDimensions(width, height);
		Width = width;
		Height = height;
		_cells = Enumerable.Range(0, checked(width * height))
			.Select(index => new PlannerCell(index % width, index / width))
			.ToArray();
	}

	public int Width { get; }
	public int Height { get; }
	public IReadOnlyList<PlannerCell> Cells => new ReadOnlyCollection<PlannerCell>(_cells);

	public PlannerCell CellAt(int x, int y)
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
				resized.CellAt(x, y).Restore(CellAt(x, y).Snapshot());
			}
		}

		return resized;
	}

	public MapChangeSet PaintTerrain(IEnumerable<GridCoordinate> coordinates, long terrainId)
	{
		var edits = new MapChangeBuilder(this);
		foreach (var coordinate in DistinctValid(coordinates))
		{
			var cell = CellAt(coordinate.X, coordinate.Y);
			edits.CaptureBefore(cell);
			cell.SetTerrain(terrainId);
			edits.CaptureAfter(cell);
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
			var cell = CellAt(coordinate.X, coordinate.Y);
			if (cell.TerrainId == 0)
			{
				continue;
			}

			edits.CaptureBefore(cell);
			if (add)
			{
				cell.AddTag(tagId);
			}
			else
			{
				cell.RemoveTag(tagId);
			}
			edits.CaptureAfter(cell);
		}

		return edits.Build();
	}

	public MapChangeSet FillTerrain(GridCoordinate origin, long terrainId)
	{
		var target = CellAt(origin.X, origin.Y).TerrainId;
		if (target == terrainId)
		{
			return MapChangeSet.Empty;
		}

		var coordinates = Flood(origin, cell => cell.TerrainId == target);
		return PaintTerrain(coordinates, terrainId);
	}

	public MapChangeSet FillTag(GridCoordinate origin, long tagId, bool add)
	{
		var start = CellAt(origin.X, origin.Y);
		if (start.TerrainId == 0)
		{
			return MapChangeSet.Empty;
		}

		var targetTerrain = start.TerrainId;
		var targetPresence = start.TagIds.Contains(tagId);
		var coordinates = Flood(origin,
			cell => cell.TerrainId == targetTerrain && cell.TagIds.Contains(tagId) == targetPresence);
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
		foreach (var cell in _cells)
		{
			edits.CaptureBefore(cell);
			switch (layer)
			{
				case PlannerLayer.Tags:
					cell.ClearTags();
					break;
				case PlannerLayer.Terrain:
				case null:
					cell.SetTerrain(0);
					break;
			}
			edits.CaptureAfter(cell);
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
			Cells = _cells.Select(cell => new PlannerProjectCell
			{
				X = cell.X,
				Y = cell.Y,
				TerrainId = cell.TerrainId,
				Tags = cell.TagIds
					.Select(id => new PlannerTagReference(id, tags.GetValueOrDefault(id)?.ShortName ?? $"Missing tag #{id}"))
					.ToList(),
				UnresolvedFeatures = cell.UnresolvedFeatures.ToList()
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
		foreach (var projectCell in project.Cells)
		{
			if (!map.Contains(projectCell.X, projectCell.Y))
			{
				throw new InvalidDataException($"Project cell ({projectCell.X}, {projectCell.Y}) is outside the map.");
			}

			map.CellAt(projectCell.X, projectCell.Y).Restore(new PlannerCellState(
				projectCell.TerrainId,
				projectCell.Tags.Select(tag => tag.Id).Distinct().Order().ToArray(),
				projectCell.UnresolvedFeatures.Distinct(StringComparer.OrdinalIgnoreCase).Order().ToArray()));
		}

		return map;
	}

	private IEnumerable<GridCoordinate> DistinctValid(IEnumerable<GridCoordinate> coordinates) =>
		coordinates.Where(coordinate => Contains(coordinate.X, coordinate.Y)).Distinct();

	private IReadOnlyList<GridCoordinate> Flood(GridCoordinate origin, Func<PlannerCell, bool> matches)
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

			var cell = CellAt(coordinate.X, coordinate.Y);
			if (!matches(cell))
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

public sealed class PlannerCell
{
	private readonly HashSet<long> _tagIds = [];
	private readonly HashSet<string> _unresolvedFeatures = new(StringComparer.OrdinalIgnoreCase);

	internal PlannerCell(int x, int y)
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

	internal PlannerCellState Snapshot() => new(
		TerrainId,
		_tagIds.Order().ToArray(),
		_unresolvedFeatures.Order(StringComparer.OrdinalIgnoreCase).ToArray());

	internal void Restore(PlannerCellState state)
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

public sealed record PlannerCellState(long TerrainId, long[] TagIds, string[] UnresolvedFeatures);

public sealed record MapCellChange(int X, int Y, PlannerCellState Before, PlannerCellState After);

public sealed class MapChangeSet
{
	public static MapChangeSet Empty { get; } = new([]);

	public MapChangeSet(IReadOnlyList<MapCellChange> changes)
	{
		Changes = changes;
	}

	public IReadOnlyList<MapCellChange> Changes { get; }
	public bool HasChanges => Changes.Count > 0;

	public static MapChangeSet Merge(IEnumerable<MapChangeSet> changeSets)
	{
		var merged = new Dictionary<GridCoordinate, MapCellChange>();
		foreach (var change in changeSets.SelectMany(item => item.Changes))
		{
			var coordinate = new GridCoordinate(change.X, change.Y);
			merged[coordinate] = merged.TryGetValue(coordinate, out var existing)
				? new MapCellChange(change.X, change.Y, existing.Before, change.After)
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
			map.CellAt(change.X, change.Y).Restore(change.Before);
		}
	}

	public void Redo(PlannerMap map)
	{
		foreach (var change in Changes)
		{
			map.CellAt(change.X, change.Y).Restore(change.After);
		}
	}
}

internal sealed class MapChangeBuilder
{
	private readonly PlannerMap _map;
	private readonly Dictionary<GridCoordinate, PlannerCellState> _before = [];
	private readonly Dictionary<GridCoordinate, PlannerCellState> _after = [];

	public MapChangeBuilder(PlannerMap map)
	{
		_map = map;
	}

	public void CaptureBefore(PlannerCell cell) =>
		_before.TryAdd(new GridCoordinate(cell.X, cell.Y), cell.Snapshot());

	public void CaptureAfter(PlannerCell cell) =>
		_after[new GridCoordinate(cell.X, cell.Y)] = cell.Snapshot();

	public MapChangeSet Build()
	{
		var changes = _before
			.Select(pair => new MapCellChange(pair.Key.X, pair.Key.Y, pair.Value, _after[pair.Key]))
			.Where(change => !Equals(change.Before, change.After) &&
				(change.Before.TerrainId != change.After.TerrainId ||
				 !change.Before.TagIds.SequenceEqual(change.After.TagIds) ||
				 !change.Before.UnresolvedFeatures.SequenceEqual(change.After.UnresolvedFeatures)))
			.ToList();
		return changes.Count == 0 ? MapChangeSet.Empty : new MapChangeSet(changes);
	}
}
