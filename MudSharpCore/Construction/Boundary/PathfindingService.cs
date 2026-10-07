using System.Diagnostics;

namespace MudSharp.Construction.Boundary;

public class PathfindingService : IPathfindingService
{
	private const int CoordinateBucketSize = 10;

	private readonly IFuturemud _gameworld;
	private PathfindingSnapshot _snapshot = PathfindingSnapshot.Empty;
	private PathfindingIndexBuilder _builder;
	private bool _dirty = true;
	private bool _warmupRequested = true;
	private long _nextSnapshotVersion = 1;
	private TimeSpan _lastBuildDuration;
	private TimeSpan _lastSliceDuration;
	private int _lastSliceRoomsProcessed;
	private int _lastSliceEdgesScanned;

	public PathfindingService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public int MaximumRoomsPerIdleSlice { get; set; } = 512;

	public PathfindingServiceDiagnostics Diagnostics => new()
	{
		CurrentSnapshotVersion = _snapshot.Version,
		IsDirty = _dirty,
		IsBuildQueued = _builder != null || _warmupRequested,
		SnapshotRoomCount = _snapshot.RoomCount,
		SnapshotClusterCount = _snapshot.ClusterCount,
		SnapshotBoundaryEdgeCount = _snapshot.BoundaryEdgeCount,
		QueuedRoomCount = _builder?.QueuedRoomCount ?? 0,
		ProcessedRoomCount = _builder?.ProcessedRoomCount ?? 0,
		LastSliceRoomsProcessed = _lastSliceRoomsProcessed,
		LastSliceEdgesScanned = _lastSliceEdgesScanned,
		LastBuildDuration = _lastBuildDuration,
		LastSliceDuration = _lastSliceDuration
	};

	public void InvalidateTopology(IRoom changedRoom = null)
	{
		_dirty = true;
		_warmupRequested = true;
		_builder = null;
	}

	public void RequestIndexWarmup()
	{
		if (_snapshot.Version == 0 || _dirty || HasLiveRoomCountChanged())
		{
			_warmupRequested = true;
		}
	}

	private bool HasLiveRoomCountChanged()
	{
		return _snapshot.Version > 0 && _snapshot.RoomCount != _gameworld.Rooms.Count;
	}

	public void DoIdleWork(TimeSpan budget)
	{
		if (budget <= TimeSpan.Zero)
		{
			return;
		}

		if (_builder == null)
		{
			if (!_dirty && HasLiveRoomCountChanged())
			{
				_dirty = true;
			}

			if (!_dirty && !_warmupRequested)
			{
				return;
			}

			_builder = new PathfindingIndexBuilder(_gameworld, _nextSnapshotVersion++, CoordinateBucketSize);
			_warmupRequested = false;
		}

		IndexBuildSlice slice = _builder.DoWork(budget, MaximumRoomsPerIdleSlice);
		_lastSliceDuration = slice.Duration;
		_lastSliceRoomsProcessed = slice.RoomsProcessed;
		_lastSliceEdgesScanned = slice.EdgesScanned;

		if (!slice.Completed)
		{
			return;
		}

		_snapshot = slice.Snapshot;
		_lastBuildDuration = slice.Snapshot.BuildDuration;
		_builder = null;
		_dirty = HasLiveRoomCountChanged();
		_warmupRequested = _dirty;
	}

	public bool TryFindLongRangePath(IRoom source, IReadOnlyCollection<IRoom> targets, uint maximumDistance,
		Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers, PathSearchOptions options,
		out IReadOnlyList<IRoomExit> path)
	{
		path = Array.Empty<IRoomExit>();
		if (source == null || targets == null || targets.Count == 0 || suitabilityFunction == null ||
		    maximumDistance == 0)
		{
			return false;
		}

		options ??= PathSearchOptions.Hierarchical;
		RequestIndexWarmup();

		PathfindingSnapshot snapshot = _snapshot;
		if (snapshot.Version == 0 || !snapshot.TryGetCluster(source.Id, out int sourceCluster))
		{
			return false;
		}

		List<IRoom> targetList = targets
		                         .Where(x => x != null)
		                         .DistinctBy(x => x.Id)
		                         .ToList();
		if (!targetList.Any())
		{
			return false;
		}

		HashSet<int> targetClusters = targetList
		                              .Select(x => snapshot.TryGetCluster(x.Id, out int cluster) ? cluster : -1)
		                              .Where(x => x >= 0)
		                              .ToHashSet();
		if (!targetClusters.Any())
		{
			return false;
		}

		if (targetClusters.Contains(sourceCluster))
		{
			List<IRoomExit> localPath = PerceivedItemExtensions.FindShortestExitPathForPathfinding(source,
				targetList, maximumDistance, suitabilityFunction, ignoreLayers);
			if (localPath.Count > 0)
			{
				path = localPath;
				return true;
			}

			return false;
		}

		HashSet<AbstractEdgeKey> blockedEdges = new();
		int retries = Math.Max(1, options.MaximumHierarchicalRetries);
		for (int i = 0; i < retries; i++)
		{
			List<AbstractEdge> route = snapshot.FindClusterRoute(sourceCluster, targetClusters, blockedEdges);
			if (route.Count == 0)
			{
				return false;
			}

			if (TryAssembleLivePath(source, targetList, route, maximumDistance, suitabilityFunction, ignoreLayers,
				    options, blockedEdges, out List<IRoomExit> assembledPath))
			{
				path = assembledPath;
				return true;
			}
		}

		return false;
	}

	private bool TryAssembleLivePath(IRoom source, IReadOnlyCollection<IRoom> targets,
		IReadOnlyList<AbstractEdge> route, uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction,
		bool ignoreLayers, PathSearchOptions options, ISet<AbstractEdgeKey> blockedEdges,
		out List<IRoomExit> path)
	{
		path = new List<IRoomExit>();
		IRoom current = source;
		uint remainingDistance = maximumDistance;
		uint segmentLimit = Math.Max(1, Math.Min(options.MaximumExactSegmentDistance, maximumDistance));

		foreach (AbstractEdge edge in route)
		{
			IRoom fromRoom = _gameworld.Rooms.Get(edge.FromRoomId);
			IRoom toRoom = _gameworld.Rooms.Get(edge.ToRoomId);
			if (fromRoom == null || toRoom == null)
			{
				blockedEdges.Add(edge.Key);
				return false;
			}

			if (!ReferenceEquals(current, fromRoom))
			{
				List<IRoomExit> segment = PerceivedItemExtensions.FindShortestExitPathForPathfinding(current,
					[fromRoom], Math.Min(remainingDistance, segmentLimit), suitabilityFunction, ignoreLayers);
				if (segment.Count == 0)
				{
					blockedEdges.Add(edge.Key);
					return false;
				}

				path.AddRange(segment);
				remainingDistance = maximumDistance >= path.Count ? maximumDistance - (uint)path.Count : 0;
				if (remainingDistance == 0)
				{
					return false;
				}
			}

			IRoomExit liveExit = fromRoom
			                     .ExitsFor(null, ignoreLayers)
			                     .FirstOrDefault(x => x.Destination?.Id == toRoom.Id && suitabilityFunction(x));
			if (liveExit == null)
			{
				blockedEdges.Add(edge.Key);
				return false;
			}

			path.Add(liveExit);
			if (path.Count > maximumDistance)
			{
				return false;
			}

			remainingDistance = maximumDistance - (uint)path.Count;
			current = toRoom;
		}

		if (targets.Any(x => ReferenceEquals(x, current) || x.Id == current.Id))
		{
			return path.Count <= maximumDistance;
		}

		List<IRoomExit> finalSegment = PerceivedItemExtensions.FindShortestExitPathForPathfinding(current,
			targets, Math.Min(remainingDistance, segmentLimit), suitabilityFunction, ignoreLayers);
		if (finalSegment.Count == 0)
		{
			if (route.Count > 0)
			{
				blockedEdges.Add(route[^1].Key);
			}

			return false;
		}

		path.AddRange(finalSegment);
		if (path.Count <= maximumDistance)
		{
			return true;
		}

		if (route.Count > 0)
		{
			blockedEdges.Add(route[^1].Key);
		}

		return false;
	}

	private sealed class PathfindingIndexBuilder
	{
		private readonly IFuturemud _gameworld;
		private readonly long _version;
		private readonly int _bucketSize;
		private readonly IReadOnlyList<IRoom> _rooms;
		private readonly Stopwatch _buildStopwatch = new();
		private readonly Dictionary<ClusterKey, int> _clusterIds = new();
		private readonly Dictionary<long, int> _roomClusters = new();
		private readonly List<TopologyEdge> _topologyEdges = new();
		private int _roomIndex;

		public PathfindingIndexBuilder(IFuturemud gameworld, long version, int bucketSize)
		{
			_gameworld = gameworld;
			_version = version;
			_bucketSize = bucketSize;
			_rooms = _gameworld.Rooms.ToList();
			QueuedRoomCount = _rooms.Count;
			_buildStopwatch.Start();
		}

		public int QueuedRoomCount { get; }
		public int ProcessedRoomCount { get; private set; }

		public IndexBuildSlice DoWork(TimeSpan budget, int maximumRooms)
		{
			Stopwatch sliceStopwatch = Stopwatch.StartNew();
			int cellsProcessed = 0;
			int edgesScanned = 0;
			while (cellsProcessed < maximumRooms && sliceStopwatch.Elapsed < budget)
			{
				if (_roomIndex >= _rooms.Count)
				{
					_buildStopwatch.Stop();
					return new IndexBuildSlice
					{
						Completed = true,
						RoomsProcessed = cellsProcessed,
						EdgesScanned = edgesScanned,
						Duration = sliceStopwatch.Elapsed,
						Snapshot = BuildSnapshot(_buildStopwatch.Elapsed)
					};
				}

				edgesScanned += ProcessRoom(_rooms[_roomIndex++]);
				cellsProcessed++;
				ProcessedRoomCount++;
			}

			return new IndexBuildSlice
			{
				Completed = false,
				RoomsProcessed = cellsProcessed,
				EdgesScanned = edgesScanned,
				Duration = sliceStopwatch.Elapsed
			};
		}

		private int ProcessRoom(IRoom room)
		{
			if (room == null || room.Id == 0)
			{
				return 0;
			}

			int clusterId = GetClusterId(ClusterKeyFor(room));
			_roomClusters[room.Id] = clusterId;
			int edgeCount = 0;
			foreach (IRoomExit exit in room.ExitsFor(null, true))
			{
				if (exit?.Destination == null || exit.Destination.Id == 0)
				{
					continue;
				}

				_topologyEdges.Add(new TopologyEdge(room.Id, exit.Destination.Id, exit.Exit?.Id ?? 0));
				edgeCount++;
			}

			return edgeCount;
		}

		private int GetClusterId(ClusterKey key)
		{
			if (_clusterIds.TryGetValue(key, out int existing))
			{
				return existing;
			}

			int id = _clusterIds.Count;
			_clusterIds[key] = id;
			return id;
		}

		private ClusterKey ClusterKeyFor(IRoom room)
		{
			if (room == null)
			{
				return new ClusterKey(room.Zone?.Id ?? 0, (int)(room.Id / 64), 0, 0);
			}

			return new ClusterKey(room.Zone?.Id ?? 0, FloorDiv(room.StoredCoordinates.X, _bucketSize),
				FloorDiv(room.StoredCoordinates.Y, _bucketSize), FloorDiv(room.StoredCoordinates.Z, _bucketSize));
		}

		private static int FloorDiv(int value, int divisor)
		{
			return value >= 0 ? value / divisor : -((-value + divisor - 1) / divisor);
		}

		private PathfindingSnapshot BuildSnapshot(TimeSpan buildDuration)
		{
			Dictionary<int, List<AbstractEdge>> boundaryEdges = new();
			HashSet<AbstractEdgeKey> seenEdges = new();
			foreach (TopologyEdge edge in _topologyEdges)
			{
				if (!_roomClusters.TryGetValue(edge.FromRoomId, out int fromCluster) ||
				    !_roomClusters.TryGetValue(edge.ToRoomId, out int toCluster) ||
				    fromCluster == toCluster)
				{
					continue;
				}

				AbstractEdge abstractEdge = new(fromCluster, toCluster, edge.FromRoomId, edge.ToRoomId, edge.ExitId);
				if (!seenEdges.Add(abstractEdge.Key))
				{
					continue;
				}

				if (!boundaryEdges.TryGetValue(fromCluster, out List<AbstractEdge> edges))
				{
					edges = new List<AbstractEdge>();
					boundaryEdges[fromCluster] = edges;
				}

				edges.Add(abstractEdge);
			}

			return new PathfindingSnapshot(_version, _roomClusters, boundaryEdges, _clusterIds.Count,
				_roomClusters.Count, boundaryEdges.Values.Sum(x => x.Count), buildDuration);
		}
	}

	private sealed class PathfindingSnapshot
	{
		public static PathfindingSnapshot Empty { get; } = new(0, new Dictionary<long, int>(),
			new Dictionary<int, List<AbstractEdge>>(), 0, 0, 0, TimeSpan.Zero);

		public PathfindingSnapshot(long version, IReadOnlyDictionary<long, int> cellClusters,
			IReadOnlyDictionary<int, List<AbstractEdge>> boundaryEdges, int clusterCount, int cellCount,
			int boundaryEdgeCount, TimeSpan buildDuration)
		{
			Version = version;
			RoomClusters = cellClusters;
			BoundaryEdges = boundaryEdges;
			ClusterCount = clusterCount;
			RoomCount = cellCount;
			BoundaryEdgeCount = boundaryEdgeCount;
			BuildDuration = buildDuration;
		}

		public long Version { get; }
		public IReadOnlyDictionary<long, int> RoomClusters { get; }
		public IReadOnlyDictionary<int, List<AbstractEdge>> BoundaryEdges { get; }
		public int ClusterCount { get; }
		public int RoomCount { get; }
		public int BoundaryEdgeCount { get; }
		public TimeSpan BuildDuration { get; }

		public bool TryGetCluster(long cellId, out int cluster)
		{
			return RoomClusters.TryGetValue(cellId, out cluster);
		}

		public List<AbstractEdge> FindClusterRoute(int sourceCluster, ISet<int> targetClusters,
			ISet<AbstractEdgeKey> blockedEdges)
		{
			Queue<ClusterSearchStep> queue = new();
			HashSet<int> seen = new()
			{
				sourceCluster
			};
			queue.Enqueue(new ClusterSearchStep(sourceCluster, null, null));

			while (queue.Count > 0)
			{
				ClusterSearchStep step = queue.Dequeue();
				if (targetClusters.Contains(step.Cluster))
				{
					return BuildClusterRoute(step);
				}

				if (!BoundaryEdges.TryGetValue(step.Cluster, out List<AbstractEdge> edges))
				{
					continue;
				}

				foreach (AbstractEdge edge in edges)
				{
					if (blockedEdges.Contains(edge.Key) || !seen.Add(edge.ToCluster))
					{
						continue;
					}

					queue.Enqueue(new ClusterSearchStep(edge.ToCluster, edge, step));
				}
			}

			return new List<AbstractEdge>();
		}

		private static List<AbstractEdge> BuildClusterRoute(ClusterSearchStep step)
		{
			List<AbstractEdge> route = new();
			ClusterSearchStep current = step;
			while (current?.ViaEdge != null)
			{
				route.Add(current.ViaEdge.Value);
				current = current.Parent;
			}

			route.Reverse();
			return route;
		}
	}

	private sealed record ClusterSearchStep(int Cluster, AbstractEdge? ViaEdge, ClusterSearchStep Parent);

	private readonly record struct ClusterKey(long ZoneId, int X, int Y, int Z);

	private readonly record struct TopologyEdge(long FromRoomId, long ToRoomId, long ExitId);

	private readonly record struct AbstractEdge(int FromCluster, int ToCluster, long FromRoomId, long ToRoomId,
		long ExitId)
	{
		public AbstractEdgeKey Key => new(FromCluster, ToCluster, FromRoomId, ToRoomId, ExitId);
	}

	private readonly record struct AbstractEdgeKey(int FromCluster, int ToCluster, long FromRoomId, long ToRoomId,
		long ExitId);

	private sealed class IndexBuildSlice
	{
		public bool Completed { get; init; }
		public int RoomsProcessed { get; init; }
		public int EdgesScanned { get; init; }
		public TimeSpan Duration { get; init; }
		public PathfindingSnapshot Snapshot { get; init; }
	}
}
