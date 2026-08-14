#nullable enable

using System.Runtime.CompilerServices;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Construction;

internal sealed record ReceivedNoisePath(
	ICharacter Listener,
	SpatialLocation Location,
	double Cost,
	IReadOnlyList<ICellExit> TraversedExits);

/// <summary>
/// Bounded cheapest-path propagation for structured noise. Coordinates only price authored
/// exits; they never create adjacency.
/// </summary>
internal sealed class StructuredNoisePropagation
{
	private const double CostEpsilon = 0.000000001;
	public const int DefaultTraversalCeiling = 4096;
	private readonly IRouteSpatialService _spatialService;
	private readonly int _traversalCeiling;

	public StructuredNoisePropagation(
		IRouteSpatialService spatialService,
		int traversalCeiling = DefaultTraversalCeiling)
	{
		_spatialService = spatialService ?? throw new ArgumentNullException(nameof(spatialService));
		_traversalCeiling = traversalCeiling > 0
			? traversalCeiling
			: throw new ArgumentOutOfRangeException(nameof(traversalCeiling));
	}

	public static StructuredNoisePropagation Instance { get; } = new(RouteSpatialService.Instance);

	public bool Propagate(
		ICell originCell,
		string audioText,
		AudioVolume volume,
		double propagationBudget,
		AudioPropagationMode propagationMode,
		IPerceiver source,
		RoomLayer originalLayer,
		bool ignoreOriginLayer,
		string noiseType)
	{
		if (volume == AudioVolume.Silent ||
			!double.IsFinite(propagationBudget) ||
			propagationBudget <= 0.0 ||
			!TryResolveSourceLocation(originCell, source, originalLayer, out var origin))
		{
			return false;
		}

		foreach (var result in Find(origin, propagationBudget, propagationMode))
		{
			if (ReferenceEquals(result.Listener, source))
			{
				continue;
			}

			var receivedVolume = Attenuate(volume, result.Cost, propagationBudget);
			var proximity = ReferenceEquals(originCell, result.Location.Cell)
				? result.Listener.GetProximity(source)
				: Proximity.VeryDistant;
			var direction = DescribeDirection(origin, result.Location, result.TraversedExits);
			NoiseEmission.RaiseReceivedEvent(
				result.Listener,
				originCell,
				source,
				receivedVolume,
				proximity,
				noiseType,
				direction,
				audioText);

			if (ignoreOriginLayer && result.Cost <= CostEpsilon)
			{
				continue;
			}

			var output = new AudioOutput(
				new Emote(
					string.Format(audioText, direction, receivedVolume.DescribeEnum(true)),
					source),
				receivedVolume,
				flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers);
			result.Listener.OutputHandler?.Send(
				output,
				!output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		return true;
	}

	internal IReadOnlyCollection<ReceivedNoisePath> Find(
		SpatialLocation origin,
		double propagationBudget,
		AudioPropagationMode propagationMode)
	{
		if (!_spatialService.TryValidateLocation(origin, out var error))
		{
			throw new ArgumentException(error, nameof(origin));
		}

		if (!double.IsFinite(propagationBudget) || propagationBudget <= 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(propagationBudget));
		}

		var initial = TraversalState.From(origin);
		var bestStates = new Dictionary<TraversalState, TraversalRoute>(TraversalStateComparer.Instance)
		{
			[initial] = new TraversalRoute(0.0, [])
		};
		var bestListeners = new Dictionary<ICharacter, ReceivedNoisePath>(ReferenceEqualityComparer.Instance);
		var queue = new PriorityQueue<TraversalState, double>();
		queue.Enqueue(initial, 0.0);
		var traversedStates = 0;

		while (queue.TryDequeue(out var state, out var queuedCost) && traversedStates < _traversalCeiling)
		{
			if (!bestStates.TryGetValue(state, out var route) || queuedCost > route.Cost + CostEpsilon)
			{
				continue;
			}

			traversedStates++;
			CollectListeners(state, route, propagationBudget, bestListeners);
			foreach (var edge in Expand(state, propagationMode))
			{
				var candidateCost = route.Cost + edge.Cost;
				if (candidateCost > propagationBudget + CostEpsilon ||
					bestStates.TryGetValue(edge.Destination, out var existing) &&
					existing.Cost <= candidateCost + CostEpsilon)
				{
					continue;
				}

				var exits = route.Exits.Append(edge.Exit).ToArray();
				bestStates[edge.Destination] = new TraversalRoute(candidateCost, exits);
				queue.Enqueue(edge.Destination, candidateCost);
			}
		}

		return bestListeners.Values
			.OrderBy(x => x.Cost)
			.ThenBy(x => x.Listener.Id)
			.ToArray();
	}

	private void CollectListeners(
		TraversalState state,
		TraversalRoute route,
		double propagationBudget,
		IDictionary<ICharacter, ReceivedNoisePath> listeners)
	{
		foreach (var listener in state.Cell.Characters.Where(x => x.RoomLayer == state.Layer))
		{
			var location = _spatialService.GetEffectiveLocation(listener);
			if (!_spatialService.TryValidateLocation(location, out _) ||
				!ReferenceEquals(location.Cell, state.Cell) ||
				location.Layer != state.Layer)
			{
				continue;
			}

			var localCost = 0.0;
			if (state.Cell.RouteDefinition is { } routeDefinition)
			{
				if (!state.CoordinateMetres.HasValue || !location.RoutePositionMetres.HasValue)
				{
					continue;
				}

				localCost = Math.Abs(location.RoutePositionMetres.Value - state.CoordinateMetres.Value) /
				            routeDefinition.MetresPerRoomEquivalent;
			}

			var cost = route.Cost + localCost;
			if (cost > propagationBudget + CostEpsilon ||
				listeners.TryGetValue(listener, out var existing) && existing.Cost <= cost + CostEpsilon)
			{
				continue;
			}

			listeners[listener] = new ReceivedNoisePath(listener, location, cost, route.Exits);
		}
	}

	private IEnumerable<TraversalEdge> Expand(TraversalState state, AudioPropagationMode mode)
	{
		foreach (var exit in state.Cell.ExitsFor(null, true) ?? Array.Empty<ICellExit>())
		{
			if (!TryResolveDestination(exit, state, out var destination))
			{
				continue;
			}

			var cost = GetExitCost(state, destination, exit, mode);
			if (double.IsFinite(cost) && cost > 0.0)
			{
				yield return new TraversalEdge(destination, cost, exit);
			}
		}
	}

	private bool TryResolveDestination(ICellExit exit, TraversalState state, out TraversalState destination)
	{
		if (exit.Destination is not { } destinationCell)
		{
			destination = default;
			return false;
		}

		var layers = (exit.WhichLayersExitAppears() ?? Array.Empty<RoomLayer>()).ToArray();
		if (!layers.Contains(state.Layer))
		{
			destination = default;
			return false;
		}

		var perceiver = new DummyPerceiver(location: state.Cell) { RoomLayer = state.Layer };
		var transition = exit.MovementTransition(perceiver);
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

		if (!_spatialService.TryGetExitAnchor(exit, destinationCell, out var anchor) || anchor is null)
		{
			destination = default;
			return false;
		}

		destination = new TraversalState(destinationCell, transition.TargetLayer, anchor.ArrivalPositionMetres);
		return _spatialService.TryValidateLocation(destination.ToSpatialLocation(), out _);
	}

	private double GetExitCost(
		TraversalState origin,
		TraversalState destination,
		ICellExit exit,
		AudioPropagationMode mode)
	{
		if (origin.Cell.RouteDefinition is { } route)
		{
			if (!origin.CoordinateMetres.HasValue ||
				!_spatialService.TryGetExitAnchor(exit, origin.Cell, out var anchor) ||
				anchor is null)
			{
				return double.PositiveInfinity;
			}

			var nearest = Math.Clamp(origin.CoordinateMetres.Value, anchor.MinimumPositionMetres,
				anchor.MaximumPositionMetres);
			return Math.Abs(nearest - origin.CoordinateMetres.Value) / route.MetresPerRoomEquivalent +
			       ValidExitMultiplier(exit);
		}

		if (destination.Cell.RouteDefinition is not null)
		{
			return ValidExitMultiplier(exit);
		}

		return mode == AudioPropagationMode.CoordinateAware
			? Math.Max(1.0, origin.Cell.EstimatedDirectDistanceTo(destination.Cell))
			: 1.0;
	}

	private static double ValidExitMultiplier(ICellExit exit)
	{
		var multiplier = exit.Exit?.TimeMultiplier ?? 1.0;
		return double.IsFinite(multiplier) && multiplier > 0.0 ? multiplier : 1.0;
	}

	internal static AudioVolume Attenuate(AudioVolume volume, double cost, double budget)
	{
		if (cost <= CostEpsilon)
		{
			return volume;
		}

		var steps = Math.Min((int)volume - 1, (int)Math.Floor(cost * (int)volume / budget));
		return volume.StageDown((uint)Math.Max(0, steps));
	}

	private bool TryResolveSourceLocation(
		ICell sourceCell,
		IPerceiver source,
		RoomLayer originalLayer,
		out SpatialLocation origin)
	{
		var effective = _spatialService.GetEffectiveLocation(source);
		if (ReferenceEquals(effective.Cell, sourceCell))
		{
			origin = new SpatialLocation(sourceCell, originalLayer, effective.RoutePositionMetres);
			return _spatialService.TryValidateLocation(origin, out _);
		}

		if (source is IGameItem item)
		{
			var owner = item.InInventoryOf ?? (ILocateable?)item.ContainedIn;
			var inherited = _spatialService.GetInheritedRoutePosition(item, owner);
			if (inherited.HasValue)
			{
				origin = new SpatialLocation(sourceCell, originalLayer, inherited);
				return _spatialService.TryValidateLocation(origin, out _);
			}
		}

		origin = default;
		return false;
	}

	private static string DescribeDirection(
		SpatialLocation origin,
		SpatialLocation listener,
		IReadOnlyList<ICellExit> exits)
	{
		var reverse = exits.Reverse().Select(x => x.Opposite).Where(x => x is not null).Cast<ICellExit>().ToArray();
		if (reverse.Length > 0)
		{
			return reverse.DescribeDirectionsToFrom();
		}

		if (ReferenceEquals(origin.Cell, listener.Cell) &&
			origin.RoutePositionMetres.HasValue && listener.RoutePositionMetres.HasValue &&
			Math.Abs(origin.RoutePositionMetres.Value - listener.RoutePositionMetres.Value) > CostEpsilon)
		{
			var route = origin.Cell.RouteDefinition!;
			return $"from {(origin.RoutePositionMetres > listener.RoutePositionMetres ? route.PositiveDirectionName : route.NegativeDirectionName)}";
		}

		return "here";
	}

	private sealed record TraversalRoute(double Cost, IReadOnlyList<ICellExit> Exits);
	private readonly record struct TraversalEdge(TraversalState Destination, double Cost, ICellExit Exit);
	private readonly record struct TraversalState(ICell Cell, RoomLayer Layer, double? CoordinateMetres)
	{
		public static TraversalState From(SpatialLocation location) =>
			new(location.Cell, location.Layer, location.RoutePositionMetres);
		public SpatialLocation ToSpatialLocation() => new(Cell, Layer, CoordinateMetres);
	}

	private sealed class TraversalStateComparer : IEqualityComparer<TraversalState>
	{
		public static TraversalStateComparer Instance { get; } = new();
		public bool Equals(TraversalState x, TraversalState y) =>
			ReferenceEquals(x.Cell, y.Cell) && x.Layer == y.Layer &&
			Nullable.Equals(x.CoordinateMetres, y.CoordinateMetres);
		public int GetHashCode(TraversalState obj) =>
			HashCode.Combine(RuntimeHelpers.GetHashCode(obj.Cell), (int)obj.Layer, obj.CoordinateMetres);
	}
}
