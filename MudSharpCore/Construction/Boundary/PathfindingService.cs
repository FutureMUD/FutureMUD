using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
	private int _lastSliceCellsProcessed;
	private int _lastSliceEdgesScanned;

	public PathfindingService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public int MaximumCellsPerIdleSlice { get; set; } = 512;

	public PathfindingServiceDiagnostics Diagnostics => new()
	{
		CurrentSnapshotVersion = _snapshot.Version,
		IsDirty = _dirty,
		IsBuildQueued = _builder != null || _warmupRequested,
		SnapshotCellCount = _snapshot.CellCount,
		SnapshotClusterCount = _snapshot.ClusterCount,
		SnapshotBoundaryEdgeCount = _snapshot.BoundaryEdgeCount,
		QueuedCellCount = _builder?.QueuedCellCount ?? 0,
		ProcessedCellCount = _builder?.ProcessedCellCount ?? 0,
		LastSliceCellsProcessed = _lastSliceCellsProcessed,
		LastSliceEdgesScanned = _lastSliceEdgesScanned,
		LastBuildDuration = _lastBuildDuration,
		LastSliceDuration = _lastSliceDuration
	};

	public void InvalidateTopology(ICell changedCell = null)
	{
		_dirty = true;
		_warmupRequested = true;
		_builder = null;
	}

	public void RequestIndexWarmup()
	{
		if (_snapshot.Version == 0 || _dirty || HasLiveCellCountChanged())
		{
			_warmupRequested = true;
		}
	}

	private bool HasLiveCellCountChanged()
	{
		return _snapshot.Version > 0 && _snapshot.CellCount != _gameworld.Cells.Count;
	}

	public void DoIdleWork(TimeSpan budget)
	{
		if (budget <= TimeSpan.Zero)
		{
			return;
		}

		if (_builder == null)
		{
			if (!_dirty && HasLiveCellCountChanged())
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

		IndexBuildSlice slice = _builder.DoWork(budget, MaximumCellsPerIdleSlice);
		_lastSliceDuration = slice.Duration;
		_lastSliceCellsProcessed = slice.CellsProcessed;
		_lastSliceEdgesScanned = slice.EdgesScanned;

		if (!slice.Completed)
		{
			return;
		}

		_snapshot = slice.Snapshot;
		_lastBuildDuration = slice.Snapshot.BuildDuration;
		_builder = null;
		_dirty = HasLiveCellCountChanged();
		_warmupRequested = _dirty;
	}

	public bool TryFindLongRangePath(ICell source, IReadOnlyCollection<ICell> targets, uint maximumDistance,
		Func<ICellExit, bool> suitabilityFunction, bool ignoreLayers, PathSearchOptions options,
		out IReadOnlyList<ICellExit> path)
	{
		path = Array.Empty<ICellExit>();
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

		List<ICell> targetList = targets
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
			List<ICellExit> localPath = PerceivedItemExtensions.FindShortestExitPathForPathfinding(source,
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
				    options, blockedEdges, out List<ICellExit> assembledPath))
			{
				path = assembledPath;
				return true;
			}
		}

		return false;
	}

	private bool TryAssembleLivePath(ICell source, IReadOnlyCollection<ICell> targets,
		IReadOnlyList<AbstractEdge> route, uint maximumDistance, Func<ICellExit, bool> suitabilityFunction,
		bool ignoreLayers, PathSearchOptions options, ISet<AbstractEdgeKey> blockedEdges,
		out List<ICellExit> path)
	{
		path = new List<ICellExit>();
		ICell current = source;
		uint remainingDistance = maximumDistance;
		uint segmentLimit = Math.Max(1, Math.Min(options.MaximumExactSegmentDistance, maximumDistance));

		foreach (AbstractEdge edge in route)
		{
			ICell fromCell = _gameworld.Cells.Get(edge.FromCellId);
			ICell toCell = _gameworld.Cells.Get(edge.ToCellId);
			if (fromCell == null || toCell == null)
			{
				blockedEdges.Add(edge.Key);
				return false;
			}

			if (!ReferenceEquals(current, fromCell))
			{
				List<ICellExit> segment = PerceivedItemExtensions.FindShortestExitPathForPathfinding(current,
					[fromCell], Math.Min(remainingDistance, segmentLimit), suitabilityFunction, ignoreLayers);
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

			ICellExit liveExit = fromCell
			                     .ExitsFor(null, ignoreLayers)
			                     .FirstOrDefault(x => x.Destination?.Id == toCell.Id && suitabilityFunction(x));
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
			current = toCell;
		}

		if (targets.Any(x => ReferenceEquals(x, current) || x.Id == current.Id))
		{
			return path.Count <= maximumDistance;
		}

		List<ICellExit> finalSegment = PerceivedItemExtensions.FindShortestExitPathForPathfinding(current,
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
		private readonly IReadOnlyList<ICell> _cells;
		private readonly Stopwatch _buildStopwatch = new();
		private readonly Dictionary<ClusterKey, int> _clusterIds = new();
		private readonly Dictionary<long, int> _cellClusters = new();
		private readonly List<TopologyEdge> _topologyEdges = new();
		private int _cellIndex;

		public PathfindingIndexBuilder(IFuturemud gameworld, long version, int bucketSize)
		{
			_gameworld = gameworld;
			_version = version;
			_bucketSize = bucketSize;
			_cells = _gameworld.Cells.ToList();
			QueuedCellCount = _cells.Count;
			_buildStopwatch.Start();
		}

		public int QueuedCellCount { get; }
		public int ProcessedCellCount { get; private set; }

		public IndexBuildSlice DoWork(TimeSpan budget, int maximumCells)
		{
			Stopwatch sliceStopwatch = Stopwatch.StartNew();
			int cellsProcessed = 0;
			int edgesScanned = 0;
			while (cellsProcessed < maximumCells && sliceStopwatch.Elapsed < budget)
			{
				if (_cellIndex >= _cells.Count)
				{
					_buildStopwatch.Stop();
					return new IndexBuildSlice
					{
						Completed = true,
						CellsProcessed = cellsProcessed,
						EdgesScanned = edgesScanned,
						Duration = sliceStopwatch.Elapsed,
						Snapshot = BuildSnapshot(_buildStopwatch.Elapsed)
					};
				}

				edgesScanned += ProcessCell(_cells[_cellIndex++]);
				cellsProcessed++;
				ProcessedCellCount++;
			}

			return new IndexBuildSlice
			{
				Completed = false,
				CellsProcessed = cellsProcessed,
				EdgesScanned = edgesScanned,
				Duration = sliceStopwatch.Elapsed
			};
		}

		private int ProcessCell(ICell cell)
		{
			if (cell == null || cell.Id == 0)
			{
				return 0;
			}

			int clusterId = GetClusterId(ClusterKeyFor(cell));
			_cellClusters[cell.Id] = clusterId;
			int edgeCount = 0;
			foreach (ICellExit exit in cell.ExitsFor(null, true))
			{
				if (exit?.Destination == null || exit.Destination.Id == 0)
				{
					continue;
				}

				_topologyEdges.Add(new TopologyEdge(cell.Id, exit.Destination.Id, exit.Exit?.Id ?? 0));
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

		private ClusterKey ClusterKeyFor(ICell cell)
		{
			if (cell.Room == null)
			{
				return new ClusterKey(cell.Zone?.Id ?? 0, (int)(cell.Id / 64), 0, 0);
			}

			return new ClusterKey(cell.Zone?.Id ?? 0, FloorDiv(cell.Room.X, _bucketSize),
				FloorDiv(cell.Room.Y, _bucketSize), FloorDiv(cell.Room.Z, _bucketSize));
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
				if (!_cellClusters.TryGetValue(edge.FromCellId, out int fromCluster) ||
				    !_cellClusters.TryGetValue(edge.ToCellId, out int toCluster) ||
				    fromCluster == toCluster)
				{
					continue;
				}

				AbstractEdge abstractEdge = new(fromCluster, toCluster, edge.FromCellId, edge.ToCellId, edge.ExitId);
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

			return new PathfindingSnapshot(_version, _cellClusters, boundaryEdges, _clusterIds.Count,
				_cellClusters.Count, boundaryEdges.Values.Sum(x => x.Count), buildDuration);
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
			CellClusters = cellClusters;
			BoundaryEdges = boundaryEdges;
			ClusterCount = clusterCount;
			CellCount = cellCount;
			BoundaryEdgeCount = boundaryEdgeCount;
			BuildDuration = buildDuration;
		}

		public long Version { get; }
		public IReadOnlyDictionary<long, int> CellClusters { get; }
		public IReadOnlyDictionary<int, List<AbstractEdge>> BoundaryEdges { get; }
		public int ClusterCount { get; }
		public int CellCount { get; }
		public int BoundaryEdgeCount { get; }
		public TimeSpan BuildDuration { get; }

		public bool TryGetCluster(long cellId, out int cluster)
		{
			return CellClusters.TryGetValue(cellId, out cluster);
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

	private readonly record struct TopologyEdge(long FromCellId, long ToCellId, long ExitId);

	private readonly record struct AbstractEdge(int FromCluster, int ToCluster, long FromCellId, long ToCellId,
		long ExitId)
	{
		public AbstractEdgeKey Key => new(FromCluster, ToCluster, FromCellId, ToCellId, ExitId);
	}

	private readonly record struct AbstractEdgeKey(int FromCluster, int ToCluster, long FromCellId, long ToCellId,
		long ExitId);

	private sealed class IndexBuildSlice
	{
		public bool Completed { get; init; }
		public int CellsProcessed { get; init; }
		public int EdgesScanned { get; init; }
		public TimeSpan Duration { get; init; }
		public PathfindingSnapshot Snapshot { get; init; }
	}
}
