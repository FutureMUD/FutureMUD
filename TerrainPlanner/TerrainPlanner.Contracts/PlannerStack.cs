using System.Text.Json;
using TerrainPlanner.Contracts.StackTransport;

namespace TerrainPlanner.Contracts;

/// <summary>Shared-size XY maps indexed by signed Z, with overrides keyed by the lower endpoint.</summary>
public sealed class PlannerStack
{
	private readonly SortedDictionary<int, PlannerMap> _levels = [];
	private readonly Dictionary<(int X, int Y, int Z), TerrainStackLink> _links = [];
	public PlannerStack(int width, int height) => _levels.Add(0, new PlannerMap(width, height));
	public IReadOnlyDictionary<int, PlannerMap> Levels => _levels;
	public int Width => _levels.First().Value.Width;
	public int Height => _levels.First().Value.Height;
	public IEnumerable<PlannerRoom> Rooms => _levels.Values.SelectMany(map => map.Rooms);
	public int OccupiedCount => Rooms.Count(room => room.TerrainId != 0);
	public IEnumerable<TerrainStackLink> Links => _links.Values;

	public void AddLevel(int z, int? duplicate = null)
	{
		ValidateSize(Width, Height, _levels.Count + 1);
		if (_levels.ContainsKey(z)) throw new InvalidDataException($"Z={z} already exists.");
		var map = duplicate.HasValue
			? PlannerMap.FromProject(_levels[duplicate.Value].ToProject("", null, new Dictionary<long, TagCatalogueItem>(), new Dictionary<long, string>()))
			: new PlannerMap(Width, Height);
		if (OccupiedCount + map.Rooms.Count(room => room.TerrainId != 0) > TerrainStackCodec.MaximumRooms)
			throw new InvalidDataException("A stack can contain at most 40,000 occupied rooms.");
		_levels.Add(z, map);
	}

	public void DeleteLevel(int z)
	{
		if (_levels.Count == 1) throw new InvalidDataException("Keep at least one Z level.");
		_levels.Remove(z);
		PruneLinks();
	}

	public void RenumberLevel(int from, int to)
	{
		if (from == to) return;
		if (_levels.ContainsKey(to)) throw new InvalidDataException($"Z={to} already exists.");
		var map = _levels[from];
		_levels.Remove(from);
		_levels.Add(to, map);
		// Overrides belong to pairs of endpoints; moving a level discards its old pairs.
		foreach (var key in _links.Keys.Where(key => key.Z == from || (long)key.Z + 1 == from).ToArray()) _links.Remove(key);
		PruneLinks();
	}

	public void Resize(int width, int height)
	{
		ValidateSize(width, height, _levels.Count);
		foreach (var z in _levels.Keys.ToArray()) _levels[z] = _levels[z].Resize(width, height);
		PruneLinks();
	}

	public bool HasConnection(int x, int y, int lowerZ) => lowerZ != int.MaxValue &&
		Occupied(x, y, lowerZ) && Occupied(x, y, lowerZ + 1);
	private bool Occupied(int x, int y, int z) => _levels.TryGetValue(z, out var map) && map.Contains(x, y) && map.RoomAt(x, y).TerrainId != 0;
	public TerrainStackLink GetLink(int x, int y, int lowerZ) =>
		_links.GetValueOrDefault((x, y, lowerZ)) ?? new TerrainStackLink(x, y, lowerZ);
	public void SetLink(TerrainStackLink link)
	{
		if (!HasConnection(link.X, link.Y, link.LowerZ) || link.Difficulty is < 0 or > 10)
			throw new InvalidDataException("A vertical link needs two adjacent occupied rooms and a valid climb difficulty.");
		_links[(link.X, link.Y, link.LowerZ)] = link;
	}
	public void PruneLinks()
	{
		foreach (var key in _links.Keys.Where(key => !HasConnection(key.X, key.Y, key.Z)).ToArray()) _links.Remove(key);
	}
	public static void ValidateSize(int width, int height, int levels)
	{
		if (width is < 1 or > TerrainStackCodec.MaximumDimension || height is < 1 or > TerrainStackCodec.MaximumDimension ||
			levels is < 1 or > TerrainStackCodec.MaximumLevels || (long)width * height * levels > TerrainStackCodec.MaximumPositions)
			throw new InvalidDataException("Use dimensions 1–200, at most 32 levels and at most 200,000 total grid positions.");
	}

	public PlannerProject ToProject(string name, string? revision, IReadOnlyDictionary<long, TagCatalogueItem> tags,
		IReadOnlyDictionary<long, string> colours) => new()
	{
		Name = name, CatalogueRevision = revision, Width = Width, Height = Height, TagColours = colours.ToDictionary(),
		Levels = _levels.Select(pair => new PlannerProjectLevel
		{
			Z = pair.Key, Rooms = pair.Value.ToProject(name, revision, tags, colours).Rooms.Where(room => room.TerrainId != 0).ToList()
		}).ToList(),
		VerticalLinks = _links.Values.ToList()
	};

	public static PlannerStack FromProject(PlannerProject project)
	{
		if (project.SchemaVersion is not (1 or PlannerProject.CurrentSchemaVersion)) throw new InvalidDataException("Unsupported planner project schema.");
		var levels = project.SchemaVersion == 1 || project.Levels is null
			? new List<PlannerProjectLevel> { new() { Z = 0, Rooms = project.Rooms } } : project.Levels;
		ValidateSize(project.Width, project.Height, levels.Count);
		var stack = new PlannerStack(project.Width, project.Height);
		stack._levels.Clear();
		foreach (var level in levels)
		{
			if (level is null || level.Rooms is null || stack._levels.ContainsKey(level.Z) ||
				level.Rooms.Select(room => (room.X, room.Y)).Distinct().Count() != level.Rooms.Count ||
				level.Rooms.Any(room => room.TerrainId < 0 || room.Tags is null || room.Tags.Any(tag => tag.Id <= 0)))
				throw new InvalidDataException("Invalid or duplicate project levels, cells or tag IDs.");
			stack._levels.Add(level.Z, PlannerMap.FromProject(new PlannerProject { Width = project.Width, Height = project.Height, Rooms = level.Rooms }));
		}
		if (stack.OccupiedCount > TerrainStackCodec.MaximumRooms) throw new InvalidDataException("A stack can contain at most 40,000 occupied rooms.");
		foreach (var link in project.VerticalLinks)
		{
			if (stack._links.ContainsKey((link.X, link.Y, link.LowerZ))) throw new InvalidDataException("Duplicate vertical link.");
			stack.SetLink(link);
		}
		return stack;
	}

	public TerrainStackPayload Export(IReadOnlySet<long> terrains, IReadOnlySet<long> tags)
	{
		var rooms = _levels.SelectMany(pair => pair.Value.Rooms.Where(room => room.TerrainId != 0).Select(room =>
		{
			if (!terrains.Contains(room.TerrainId) || room.TagIds.Any(id => !tags.Contains(id)) || room.UnresolvedFeatures.Count != 0)
				throw new InvalidDataException("Resolve missing terrain/tag IDs and unresolved features before exporting the stack.");
			return new TerrainStackRoom(room.X, room.Y, pair.Key, room.TerrainId, room.TagIds.Order().ToArray());
		})).ToArray();
		var payload = new TerrainStackPayload(Width, Height, _levels.Keys.ToArray(), rooms, _links.Values.ToArray());
		TerrainStackCodec.Validate(payload);
		return payload;
	}
}

/// <summary>Whole-stack undo, bounded by both operation count and serialized snapshot bytes.</summary>
public sealed class PlannerStackHistory
{
	private readonly LinkedList<byte[]> _undo = new();
	private readonly Stack<byte[]> _redo = new();
	private const int Budget = 32 * 1024 * 1024;
	private long _bytes;
	public bool CanUndo => _undo.Count != 0;
	public bool CanRedo => _redo.Count != 0;
	public static byte[] Capture(PlannerStack stack) => JsonSerializer.SerializeToUtf8Bytes(stack.ToProject("", null,
		new Dictionary<long, TagCatalogueItem>(), new Dictionary<long, string>()));
	public void Record(byte[] before)
	{
		if (before.Length > Budget) { Clear(); return; }
		_bytes -= _redo.Sum(item => (long)item.Length);
		_redo.Clear();
		_undo.AddLast(before);
		_bytes += before.Length;
		Trim();
	}
	private void Trim()
	{
		while (_undo.Count + _redo.Count > 100 || _bytes > Budget)
		{
			if (_undo.First is { } oldest)
			{
				_bytes -= oldest.Value.Length;
				_undo.RemoveFirst();
			}
			else
			{
				var retained = _redo.ToArray();
				_bytes -= retained[^1].Length;
				_redo.Clear();
				foreach (var item in retained.SkipLast(1).Reverse()) _redo.Push(item);
			}
		}
	}
	public PlannerStack Undo(PlannerStack current)
	{
		if (!CanUndo) return current;
		var before = _undo.Last!.Value;
		_undo.RemoveLast();
		var after = Capture(current);
		_redo.Push(after);
		_bytes += after.Length - before.Length;
		Trim();
		return Restore(before);
	}
	public PlannerStack Redo(PlannerStack current)
	{
		if (!_redo.TryPop(out var after)) return current;
		var before = Capture(current);
		_undo.AddLast(before);
		_bytes += before.Length - after.Length;
		Trim();
		return Restore(after);
	}
	public void Clear() { _undo.Clear(); _redo.Clear(); _bytes = 0; }
	private static PlannerStack Restore(byte[] json) => PlannerStack.FromProject(JsonSerializer.Deserialize<PlannerProject>(json)!);
}
