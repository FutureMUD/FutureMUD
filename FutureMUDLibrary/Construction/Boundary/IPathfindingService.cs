using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp.Construction.Boundary;

public enum PathSearchAlgorithm
{
	Exact,
	Automatic,
	Hierarchical
}

public sealed class PathSearchOptions
{
	public static PathSearchOptions Exact { get; } = new();
	public static PathSearchOptions Automatic { get; } = new()
	{
		Algorithm = PathSearchAlgorithm.Automatic
	};
	public static PathSearchOptions Hierarchical { get; } = new()
	{
		Algorithm = PathSearchAlgorithm.Hierarchical
	};

	public PathSearchAlgorithm Algorithm { get; init; } = PathSearchAlgorithm.Exact;
	public uint HierarchicalThreshold { get; init; } = 30;
	public uint MaximumExactSegmentDistance { get; init; } = 64;
	public int MaximumHierarchicalRetries { get; init; } = 4;
}

public sealed class PathfindingServiceDiagnostics
{
	public long CurrentSnapshotVersion { get; init; }
	public bool IsDirty { get; init; }
	public bool IsBuildQueued { get; init; }
	public int SnapshotRoomCount { get; init; }
	public int SnapshotClusterCount { get; init; }
	public int SnapshotBoundaryEdgeCount { get; init; }
	public int QueuedRoomCount { get; init; }
	public int ProcessedRoomCount { get; init; }
	public int LastSliceRoomsProcessed { get; init; }
	public int LastSliceEdgesScanned { get; init; }
	public TimeSpan LastBuildDuration { get; init; }
	public TimeSpan LastSliceDuration { get; init; }
}

public interface IPathfindingService
{
	PathfindingServiceDiagnostics Diagnostics { get; }
	void InvalidateTopology(IRoom changedRoom = null);
	void RequestIndexWarmup();
	void DoIdleWork(TimeSpan budget);
	bool TryFindLongRangePath(IRoom source, IReadOnlyCollection<IRoom> targets, uint maximumDistance,
		Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers, PathSearchOptions options,
		out IReadOnlyList<IRoomExit> path);
}
