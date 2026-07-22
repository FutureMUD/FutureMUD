#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// A perceivable reached by an exact hybrid spatial path.
/// </summary>
internal sealed record SpatialReachablePerceivable(
	IPerceivable Perceivable,
	SpatialLocation Location,
	ISpatialPath Path);

/// <summary>
/// Discovers perceivables within a bounded room-equivalent cost without flattening a
/// RouteCell into one ordinary room. RouteCell occupants are obtained from the ordered
/// spatial index; topology traversal is limited to authored exit anchors and ordinary exits.
/// Every discovered candidate is finally checked by the hybrid pathfinder.
/// </summary>
internal sealed class SpatialPerceivableReachability
{
	private const double CostEpsilon = 0.000000001;
	private readonly IRouteSpatialService _spatialService;
	private readonly ISpatialPathfinder? _pathfinder;

	public SpatialPerceivableReachability(
		IRouteSpatialService spatialService,
		ISpatialPathfinder? pathfinder = null)
	{
		_spatialService = spatialService ?? throw new ArgumentNullException(nameof(spatialService));
		_pathfinder = pathfinder;
	}

	public static SpatialPerceivableReachability Instance { get; } =
		new(RouteSpatialService.Instance);

	public IReadOnlyCollection<SpatialReachablePerceivable> Find(
		SpatialLocation origin,
		double maximumRoomEquivalentCost,
		Func<IPerceivable, bool>? predicate = null,
		bool ignoreLayers = false)
	{
		if (!_spatialService.TryValidateLocation(origin, out var error))
		{
			throw new ArgumentException(error, nameof(origin));
		}

		if (!double.IsFinite(maximumRoomEquivalentCost) || maximumRoomEquivalentCost < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumRoomEquivalentCost));
		}

		var pathfinder = _pathfinder ?? origin.Cell.Gameworld.ExitManager.SpatialPathfinder;
		var candidateSet = new HashSet<IPerceivable>(ReferenceEqualityComparer.Instance);
		var costs = new Dictionary<TraversalState, double>(TraversalStateComparer.Instance)
		{
			[TraversalState.From(origin)] = 0.0
		};
		var queue = new PriorityQueue<TraversalState, double>();
		queue.Enqueue(TraversalState.From(origin), 0.0);

		while (queue.TryDequeue(out var state, out var queuedCost))
		{
			if (!costs.TryGetValue(state, out var stateCost) || queuedCost > stateCost + CostEpsilon)
			{
				continue;
			}

			CollectCandidates(
				state,
				stateCost,
				maximumRoomEquivalentCost,
				predicate,
				ignoreLayers,
				candidateSet);

			foreach (var edge in Expand(state, ignoreLayers))
			{
				var candidateCost = stateCost + edge.Cost;
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
				queue.Enqueue(edge.Destination, candidateCost);
			}
		}

		var results = new List<SpatialReachablePerceivable>();
		foreach (var perceivable in candidateSet)
		{
			var actualLocation = _spatialService.GetEffectiveLocation(perceivable);
			if (!_spatialService.TryValidateLocation(actualLocation, out _))
			{
				continue;
			}

			var pathDestination = ignoreLayers
				? new SpatialLocation(actualLocation.Cell, origin.Layer, actualLocation.RoutePositionMetres)
				: actualLocation;
			if (!pathfinder.TryFindPath(
					origin,
					pathDestination,
					static _ => true,
					ignoreLayers,
					maximumRoomEquivalentCost,
					out var path) ||
				path is null ||
				path.RoomEquivalentCost > maximumRoomEquivalentCost + CostEpsilon)
			{
				continue;
			}

			results.Add(new SpatialReachablePerceivable(perceivable, actualLocation, path));
		}

		return results
			.OrderBy(x => x.Path.RoomEquivalentCost)
			.ThenBy(x => x.Perceivable.Id)
			.ToArray();
	}

	private void CollectCandidates(
		TraversalState state,
		double stateCost,
		double maximumCost,
		Func<IPerceivable, bool>? predicate,
		bool ignoreLayers,
		ISet<IPerceivable> candidates)
	{
		var remainingCost = Math.Max(0.0, maximumCost - stateCost);
		if (state.Cell.RouteDefinition is not { } route)
		{
			foreach (var perceivable in state.Cell.Perceivables
				.Where(x => ignoreLayers || x.RoomLayer == state.Layer)
				.Where(x => predicate?.Invoke(x) != false))
			{
				candidates.Add(perceivable);
			}

			return;
		}

		var radiusMetres = remainingCost * route.MetresPerRoomEquivalent;
		var origin = state.ToSpatialLocation();
		var local = ignoreLayers
			? _spatialService.GetPerceivablesWithinAcrossLayers(origin, radiusMetres, predicate)
			: _spatialService.GetPerceivablesWithin(origin, radiusMetres, predicate);
		foreach (var perceivable in local)
		{
			candidates.Add(perceivable);
		}
	}

	private IEnumerable<TraversalEdge> Expand(TraversalState state, bool ignoreLayers)
	{
		if (state.Cell.RouteDefinition is not { } route)
		{
			foreach (var exit in state.Cell.ExitsFor(null, true) ?? Array.Empty<ICellExit>())
			{
				if (!TryResolveDestination(
						exit,
						state.Cell,
						state.Layer,
						ignoreLayers,
						out var destination))
				{
					continue;
				}

				yield return new TraversalEdge(destination, GetExitCost(exit));
			}

			yield break;
		}

		if (!state.CoordinateMetres.HasValue)
		{
			yield break;
		}

		foreach (var anchor in route.ExitAnchors ?? Array.Empty<IRouteExitAnchor>())
		{
			var exit = anchor.Exit;
			if (exit?.Destination is null ||
				!ReferenceEquals(anchor.Cell, state.Cell) ||
				!IsValidAnchor(anchor, route.LengthMetres) ||
				!TryResolveDestination(
					exit,
					state.Cell,
					state.Layer,
					ignoreLayers,
					out var destination))
			{
				continue;
			}

			var nearestBandPosition = Math.Clamp(
				state.CoordinateMetres.Value,
				anchor.MinimumPositionMetres,
				anchor.MaximumPositionMetres);
			var longitudinalCost = Math.Abs(nearestBandPosition - state.CoordinateMetres.Value) /
			                       route.MetresPerRoomEquivalent;
			yield return new TraversalEdge(destination, longitudinalCost + GetExitCost(exit));
		}
	}

	private bool TryResolveDestination(
		ICellExit exit,
		ICell originCell,
		RoomLayer layer,
		bool ignoreLayers,
		out TraversalState destination)
	{
		if (exit.Destination is not { } destinationCell)
		{
			destination = default;
			return false;
		}

		var appearingLayers = (exit.WhichLayersExitAppears() ?? Array.Empty<RoomLayer>())
			.Distinct()
			.OrderBy(x => x)
			.ToArray();
		var transitionLayer = layer;
		if (!appearingLayers.Contains(transitionLayer))
		{
			if (!ignoreLayers || appearingLayers.Length == 0)
			{
				destination = default;
				return false;
			}

			transitionLayer = appearingLayers[0];
		}

		var transitionPerceiver = new DummyPerceiver(location: originCell)
		{
			RoomLayer = transitionLayer
		};
		var transition = exit.MovementTransition(transitionPerceiver);
		if (transition.TransitionType == CellMovementTransition.NoViableTransition)
		{
			destination = default;
			return false;
		}

		if (destinationCell.RouteDefinition is null)
		{
			destination = new TraversalState(destinationCell, transition.TargetLayer, null);
			return true;
		}

		if (!_spatialService.TryGetExitAnchor(exit, destinationCell, out var anchor) ||
			anchor is null ||
			!double.IsFinite(anchor.ArrivalPositionMetres) ||
			anchor.ArrivalPositionMetres < 0.0 ||
			anchor.ArrivalPositionMetres > destinationCell.RouteDefinition.LengthMetres)
		{
			destination = default;
			return false;
		}

		destination = new TraversalState(
			destinationCell,
			transition.TargetLayer,
			anchor.ArrivalPositionMetres);
		return true;
	}

	private static bool IsValidAnchor(IRouteExitAnchor anchor, double routeLengthMetres)
	{
		return double.IsFinite(routeLengthMetres) && routeLengthMetres > 0.0 &&
		       double.IsFinite(anchor.MinimumPositionMetres) &&
		       double.IsFinite(anchor.MaximumPositionMetres) &&
		       double.IsFinite(anchor.ArrivalPositionMetres) &&
		       anchor.MinimumPositionMetres >= 0.0 &&
		       anchor.MinimumPositionMetres <= anchor.MaximumPositionMetres &&
		       anchor.MaximumPositionMetres <= routeLengthMetres &&
		       anchor.ArrivalPositionMetres >= 0.0 &&
		       anchor.ArrivalPositionMetres <= routeLengthMetres;
	}

	private static double GetExitCost(ICellExit exit)
	{
		var multiplier = exit.Exit?.TimeMultiplier ?? 1.0;
		return double.IsFinite(multiplier) && multiplier > 0.0
			? multiplier
			: 1.0;
	}

	private readonly record struct TraversalEdge(TraversalState Destination, double Cost);

	private readonly record struct TraversalState(
		ICell Cell,
		RoomLayer Layer,
		double? CoordinateMetres)
	{
		public static TraversalState From(SpatialLocation location)
		{
			return new TraversalState(location.Cell, location.Layer, location.RoutePositionMetres);
		}

		public SpatialLocation ToSpatialLocation()
		{
			return new SpatialLocation(Cell, Layer, CoordinateMetres);
		}
	}

	private sealed class TraversalStateComparer : IEqualityComparer<TraversalState>
	{
		public static TraversalStateComparer Instance { get; } = new();

		public bool Equals(TraversalState x, TraversalState y)
		{
			return ReferenceEquals(x.Cell, y.Cell) &&
			       x.Layer == y.Layer &&
			       Nullable.Equals(x.CoordinateMetres, y.CoordinateMetres);
		}

		public int GetHashCode(TraversalState obj)
		{
			return HashCode.Combine(
				RuntimeHelpers.GetHashCode(obj.Cell),
				(int)obj.Layer,
				obj.CoordinateMetres);
		}
	}
}
