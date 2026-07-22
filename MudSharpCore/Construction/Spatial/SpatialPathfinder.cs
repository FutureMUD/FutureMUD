#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// Dijkstra pathfinder over a hybrid graph of ordinary cells and exact one-dimensional
/// RouteCell waypoints. Route topology snapshots are retained until their authored topology
/// version changes or they are explicitly invalidated.
/// </summary>
public sealed class SpatialPathfinder : ISpatialPathfinder
{
	private const double CostEpsilon = 0.000000001;
	private readonly Dictionary<ICell, RouteTopologySnapshot> _routeTopologyCache =
		new(CellReferenceComparer.Instance);
	private readonly object _cacheLock = new();

	public bool TryFindPath(
		SpatialLocation origin,
		SpatialLocation destination,
		out ISpatialPath? path)
	{
		return TryFindPath(
			origin,
			destination,
			null,
			false,
			double.PositiveInfinity,
			out path);
	}

	public bool TryFindPath(
		SpatialLocation origin,
		SpatialLocation destination,
		Func<ICellExit, bool>? suitabilityFunction,
		bool ignoreLayers,
		double maximumRoomEquivalentCost,
		out ISpatialPath? path)
	{
		path = null;
		if (!TryValidateEndpoint(origin) || !TryValidateEndpoint(destination) ||
			double.IsNaN(maximumRoomEquivalentCost) || maximumRoomEquivalentCost < 0.0)
		{
			return false;
		}

		var originKey = NodeKey.From(origin);
		var destinationKey = NodeKey.From(destination);
		if (NodeKeyComparer.Instance.Equals(originKey, destinationKey))
		{
			path = new SpatialPath(origin, destination, Array.Empty<ISpatialPathStep>());
			return true;
		}

		if (maximumRoomEquivalentCost <= 0.0)
		{
			return false;
		}

		suitabilityFunction ??= static _ => true;
		var costs = new Dictionary<NodeKey, double>(NodeKeyComparer.Instance)
		{
			[originKey] = 0.0
		};
		var predecessors = new Dictionary<NodeKey, PreviousStep>(NodeKeyComparer.Instance);
		var queue = new PriorityQueue<NodeKey, double>();
		queue.Enqueue(originKey, 0.0);

		while (queue.TryDequeue(out var current, out var queuedCost))
		{
			if (!costs.TryGetValue(current, out var currentCost) || queuedCost > currentCost + CostEpsilon)
			{
				continue;
			}

			if (NodeKeyComparer.Instance.Equals(current, destinationKey))
			{
				var steps = BuildSteps(originKey, destinationKey, predecessors);
				path = new SpatialPath(origin, destination, CoalesceLinearSteps(steps));
				return true;
			}

			foreach (var edge in Expand(current, destination, suitabilityFunction, ignoreLayers))
			{
				if (!double.IsFinite(edge.Cost) || edge.Cost < 0.0)
				{
					continue;
				}

				var candidateCost = currentCost + edge.Cost;
				if (candidateCost > maximumRoomEquivalentCost + CostEpsilon)
				{
					continue;
				}

				if (costs.TryGetValue(edge.Destination, out var existingCost) &&
					existingCost <= candidateCost + CostEpsilon)
				{
					continue;
				}

				costs[edge.Destination] = candidateCost;
				predecessors[edge.Destination] = new PreviousStep(current, edge.Step);
				queue.Enqueue(edge.Destination, candidateCost);
			}
		}

		return false;
	}

	public bool TryFindExitOnlyPath(
		SpatialLocation origin,
		SpatialLocation destination,
		Func<ICellExit, bool>? suitabilityFunction,
		bool ignoreLayers,
		double maximumRoomEquivalentCost,
		out IReadOnlyList<ICellExit> exits)
	{
		exits = Array.Empty<ICellExit>();
		if (!TryFindPath(
				origin,
				destination,
				suitabilityFunction,
				ignoreLayers,
				maximumRoomEquivalentCost,
				out var spatialPath) || spatialPath is null ||
			spatialPath.Steps.Any(x => x is ILinearRoutePathStep))
		{
			return false;
		}

		exits = spatialPath.TraversedExits;
		return true;
	}

	public void InvalidateTopology(ICell? changedCell = null)
	{
		lock (_cacheLock)
		{
			if (changedCell is null)
			{
				_routeTopologyCache.Clear();
				return;
			}

			_routeTopologyCache.Remove(changedCell);
		}
	}

	private IEnumerable<SearchEdge> Expand(
		NodeKey current,
		SpatialLocation searchDestination,
		Func<ICellExit, bool> suitabilityFunction,
		bool ignoreLayers)
	{
		var currentLocation = current.ToSpatialLocation();
		var definition = current.Cell.RouteDefinition;
		if (definition is null)
		{
			foreach (var edge in ExpandExits(
					currentLocation,
				null,
				suitabilityFunction,
				ignoreLayers))
			{
				yield return edge;
			}

			yield break;
		}

		if (!current.CoordinateMetres.HasValue || !TryGetRouteTopology(definition, out var topology))
		{
			yield break;
		}

		var position = current.CoordinateMetres.Value;
		var dynamicTarget = ReferenceEquals(current.Cell, searchDestination.Cell) &&
		                    current.Layer == searchDestination.Layer
			? searchDestination.RoutePositionMetres
			: null;
		var waypoints = topology.WaypointsWith(position, dynamicTarget);
		var index = waypoints.BinarySearch(position);
		if (index < 0)
		{
			yield break;
		}

		if (index > 0)
		{
			yield return CreateLinearEdge(currentLocation, waypoints[index - 1], topology);
		}

		if (index < waypoints.Count - 1)
		{
			yield return CreateLinearEdge(currentLocation, waypoints[index + 1], topology);
		}

		foreach (var edge in ExpandExits(
				currentLocation,
			topology,
			suitabilityFunction,
			ignoreLayers))
		{
			yield return edge;
		}
	}

	private IEnumerable<SearchEdge> ExpandExits(
		SpatialLocation origin,
		RouteTopologySnapshot? sourceTopology,
		Func<ICellExit, bool> suitabilityFunction,
		bool ignoreLayers)
	{
		// A null perceiver does not give Cell.ExitsFor a layer to filter against. Enumerate the
		// topology here and apply the source-layer contract explicitly below.
		var exits = origin.Cell.ExitsFor(null, true) ?? Array.Empty<ICellExit>();
		foreach (var exit in exits)
		{
			if (exit?.Destination is null || !suitabilityFunction(exit))
			{
				continue;
			}

			var appearingLayers = (exit.WhichLayersExitAppears() ?? Array.Empty<RoomLayer>())
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
			if (!ignoreLayers && !appearingLayers.Contains(origin.Layer))
			{
				continue;
			}

			if (sourceTopology is not null)
			{
				var sourceAnchor = sourceTopology.FindAnchor(exit);
				if (sourceAnchor is null || !origin.RoutePositionMetres.HasValue ||
					!sourceAnchor.Contains(origin.RoutePositionMetres.Value))
				{
					continue;
				}
			}

			if (!TryResolveExitDestination(
					exit,
					origin,
					appearingLayers,
					ignoreLayers,
					out var destination))
			{
				continue;
			}

			var cost = GetOrdinaryExitCost(exit);
			var step = new ExitTraversalPathStep(origin, destination, exit, cost);
			yield return new SearchEdge(NodeKey.From(destination), cost, step);
		}
	}

	private bool TryResolveExitDestination(
		ICellExit exit,
		SpatialLocation origin,
		IReadOnlyList<RoomLayer> appearingLayers,
		bool ignoreLayers,
		out SpatialLocation destination)
	{
		var transitionLayer = origin.Layer;
		if (!appearingLayers.Contains(transitionLayer))
		{
			if (!ignoreLayers || appearingLayers.Count == 0)
			{
				destination = default;
				return false;
			}

			// Layer-agnostic path queries still need a real exit-side layer from which to
			// evaluate the existing movement-transition contract.
			transitionLayer = appearingLayers[0];
		}

		var transitionPerceiver = new DummyPerceiver(location: origin.Cell)
		{
			RoomLayer = transitionLayer
		};
		var transition = exit.MovementTransition(transitionPerceiver);
		if (transition.TransitionType == CellMovementTransition.NoViableTransition)
		{
			destination = default;
			return false;
		}

		var destinationCell = exit.Destination;
		var destinationDefinition = destinationCell.RouteDefinition;
		if (destinationDefinition is null)
		{
			destination = new SpatialLocation(destinationCell, transition.TargetLayer);
			return true;
		}

		if (!TryGetRouteTopology(destinationDefinition, out var topology))
		{
			destination = default;
			return false;
		}

		var destinationAnchor = topology.FindDestinationAnchor(exit);
		if (destinationAnchor is null ||
			!IsCoordinateInRange(destinationAnchor.ArrivalPositionMetres, topology.LengthMetres))
		{
			destination = default;
			return false;
		}

		destination = new SpatialLocation(
			destinationCell,
			transition.TargetLayer,
			destinationAnchor.ArrivalPositionMetres);
		return true;
	}

	private static SearchEdge CreateLinearEdge(
		SpatialLocation origin,
		double destinationPosition,
		RouteTopologySnapshot topology)
	{
		var originPosition = origin.RoutePositionMetres!.Value;
		var distance = Math.Abs(destinationPosition - originPosition);
		var direction = destinationPosition > originPosition
			? RouteCellDirection.Positive
			: RouteCellDirection.Negative;
		var destination = new SpatialLocation(origin.Cell, origin.Layer, destinationPosition);
		var cost = distance / topology.MetresPerRoomEquivalent;
		var step = new LinearRoutePathStep(
			origin,
			destination,
			topology.Definition,
			direction,
			distance,
			cost);
		return new SearchEdge(NodeKey.From(destination), cost, step);
	}

	private bool TryGetRouteTopology(
		IRouteCellDefinition definition,
		out RouteTopologySnapshot topology)
	{
		lock (_cacheLock)
		{
			if (_routeTopologyCache.TryGetValue(definition.Cell, out var cached) &&
				cached.TopologyVersion == definition.TopologyVersion)
			{
				topology = cached;
				return true;
			}

			if (!RouteTopologySnapshot.TryCreate(definition, out topology))
			{
				_routeTopologyCache.Remove(definition.Cell);
				return false;
			}

			_routeTopologyCache[definition.Cell] = topology;
			return true;
		}
	}

	private static bool TryValidateEndpoint(SpatialLocation location)
	{
		if (location.Cell is null)
		{
			return false;
		}

		var definition = location.Cell.RouteDefinition;
		if (definition is null)
		{
			return !location.RoutePositionMetres.HasValue;
		}

		return location.RoutePositionMetres.HasValue &&
		       IsCoordinateInRange(location.RoutePositionMetres.Value, definition.LengthMetres) &&
		       double.IsFinite(definition.MetresPerRoomEquivalent) &&
		       definition.MetresPerRoomEquivalent > 0.0;
	}

	private static bool IsCoordinateInRange(double coordinate, double length)
	{
		return double.IsFinite(coordinate) && double.IsFinite(length) && length > 0.0 &&
		       coordinate >= 0.0 && coordinate <= length;
	}

	private static double GetOrdinaryExitCost(ICellExit exit)
	{
		var multiplier = exit.Exit?.TimeMultiplier ?? 1.0;
		return double.IsFinite(multiplier) && multiplier > 0.0
			? multiplier
			: 1.0;
	}

	private static IReadOnlyList<ISpatialPathStep> BuildSteps(
		NodeKey origin,
		NodeKey destination,
		IReadOnlyDictionary<NodeKey, PreviousStep> predecessors)
	{
		var reversed = new List<ISpatialPathStep>();
		var current = destination;
		while (!NodeKeyComparer.Instance.Equals(current, origin))
		{
			if (!predecessors.TryGetValue(current, out var previous))
			{
				return Array.Empty<ISpatialPathStep>();
			}

			reversed.Add(previous.Step);
			current = previous.Node;
		}

		reversed.Reverse();
		return reversed;
	}

	private static IEnumerable<ISpatialPathStep> CoalesceLinearSteps(
		IReadOnlyList<ISpatialPathStep> steps)
	{
		var result = new List<ISpatialPathStep>(steps.Count);
		foreach (var step in steps)
		{
			if (step is ILinearRoutePathStep current &&
				result.LastOrDefault() is ILinearRoutePathStep previous &&
				ReferenceEquals(previous.RouteCell.Cell, current.RouteCell.Cell) &&
				previous.Direction == current.Direction &&
				previous.Destination.Equals(current.Origin))
			{
				result[^1] = new LinearRoutePathStep(
					previous.Origin,
					current.Destination,
					previous.RouteCell,
					previous.Direction,
					previous.DistanceMetres + current.DistanceMetres,
					previous.RoomEquivalentCost + current.RoomEquivalentCost);
				continue;
			}

			result.Add(step);
		}

		return result;
	}

	private readonly record struct SearchEdge(
		NodeKey Destination,
		double Cost,
		ISpatialPathStep Step);

	private readonly record struct PreviousStep(
		NodeKey Node,
		ISpatialPathStep Step);

	private readonly record struct NodeKey(
		ICell Cell,
		RoomLayer Layer,
		double? CoordinateMetres)
	{
		public static NodeKey From(SpatialLocation location)
		{
			return new NodeKey(location.Cell, location.Layer, location.RoutePositionMetres);
		}

		public SpatialLocation ToSpatialLocation()
		{
			return new SpatialLocation(Cell, Layer, CoordinateMetres);
		}
	}

	private sealed class NodeKeyComparer : IEqualityComparer<NodeKey>
	{
		public static NodeKeyComparer Instance { get; } = new();

		public bool Equals(NodeKey x, NodeKey y)
		{
			return ReferenceEquals(x.Cell, y.Cell) && x.Layer == y.Layer &&
			       Nullable.Equals(x.CoordinateMetres, y.CoordinateMetres);
		}

		public int GetHashCode(NodeKey obj)
		{
			return HashCode.Combine(
				RuntimeHelpers.GetHashCode(obj.Cell),
				(int)obj.Layer,
				obj.CoordinateMetres);
		}
	}

	private sealed class CellReferenceComparer : IEqualityComparer<ICell>
	{
		public static CellReferenceComparer Instance { get; } = new();

		public bool Equals(ICell? x, ICell? y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(ICell obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}

	private sealed class RouteTopologySnapshot
	{
		private readonly IReadOnlyList<double> _waypoints;
		private readonly IReadOnlyList<IRouteExitAnchor> _anchors;

		private RouteTopologySnapshot(
			IRouteCellDefinition definition,
			double lengthMetres,
			double metresPerRoomEquivalent,
			IReadOnlyList<double> waypoints,
			IReadOnlyList<IRouteExitAnchor> anchors)
		{
			Definition = definition;
			TopologyVersion = definition.TopologyVersion;
			LengthMetres = lengthMetres;
			MetresPerRoomEquivalent = metresPerRoomEquivalent;
			_waypoints = waypoints;
			_anchors = anchors;
		}

		public IRouteCellDefinition Definition { get; }
		public long TopologyVersion { get; }
		public double LengthMetres { get; }
		public double MetresPerRoomEquivalent { get; }

		public static bool TryCreate(
			IRouteCellDefinition definition,
			out RouteTopologySnapshot topology)
		{
			topology = null!;
			if (definition.Cell is null || !double.IsFinite(definition.LengthMetres) ||
				definition.LengthMetres <= 0.0 || !double.IsFinite(definition.MetresPerRoomEquivalent) ||
				definition.MetresPerRoomEquivalent <= 0.0)
			{
				return false;
			}

			var anchors = (definition.ExitAnchors ?? Array.Empty<IRouteExitAnchor>())
				.Where(x => x is not null && ReferenceEquals(x.Cell, definition.Cell))
				.Where(x => IsValidAnchor(x, definition.LengthMetres))
				.ToArray();
			var waypoints = new SortedSet<double>
			{
				0.0,
				definition.LengthMetres
			};

			foreach (var landmark in definition.Landmarks ?? Array.Empty<IRouteCellLandmark>())
			{
				if (landmark is not null && IsCoordinateInRange(landmark.PositionMetres, definition.LengthMetres))
				{
					waypoints.Add(landmark.PositionMetres);
				}
			}

			foreach (var anchor in anchors)
			{
				waypoints.Add(anchor.MinimumPositionMetres);
				waypoints.Add(anchor.MaximumPositionMetres);
				waypoints.Add(anchor.ArrivalPositionMetres);
			}

			topology = new RouteTopologySnapshot(
				definition,
				definition.LengthMetres,
				definition.MetresPerRoomEquivalent,
				waypoints.ToArray(),
				anchors);
			return true;
		}

		public List<double> WaypointsWith(double currentPosition, double? targetPosition)
		{
			var waypoints = new SortedSet<double>(_waypoints)
			{
				currentPosition
			};
			if (targetPosition.HasValue && IsCoordinateInRange(targetPosition.Value, LengthMetres))
			{
				waypoints.Add(targetPosition.Value);
			}

			return waypoints.ToList();
		}

		public IRouteExitAnchor? FindAnchor(ICellExit exit)
		{
			return _anchors.FirstOrDefault(x => ExitSidesMatch(x.Exit, exit));
		}

		public IRouteExitAnchor? FindDestinationAnchor(ICellExit sourceExit)
		{
			return _anchors.FirstOrDefault(x =>
				ReferenceEquals(x.Exit, sourceExit.Opposite) || ExitSidesShareUnderlyingExit(x.Exit, sourceExit));
		}

		private static bool IsValidAnchor(IRouteExitAnchor anchor, double lengthMetres)
		{
			return anchor.Exit is not null &&
			       IsCoordinateInRange(anchor.MinimumPositionMetres, lengthMetres) &&
			       IsCoordinateInRange(anchor.MaximumPositionMetres, lengthMetres) &&
			       IsCoordinateInRange(anchor.ArrivalPositionMetres, lengthMetres) &&
			       anchor.MinimumPositionMetres <= anchor.MaximumPositionMetres;
		}

		private static bool ExitSidesMatch(ICellExit first, ICellExit second)
		{
			return ReferenceEquals(first, second) ||
			       ReferenceEquals(first.Exit, second.Exit) && ReferenceEquals(first.Origin, second.Origin);
		}

		private static bool ExitSidesShareUnderlyingExit(ICellExit first, ICellExit second)
		{
			if (ReferenceEquals(first.Exit, second.Exit))
			{
				return true;
			}

			return first.Exit is not null && second.Exit is not null && first.Exit.Id > 0L &&
			       first.Exit.Id == second.Exit.Id;
		}
	}
}
