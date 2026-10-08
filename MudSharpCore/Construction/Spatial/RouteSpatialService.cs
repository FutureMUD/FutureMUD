#nullable enable

using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp.Construction;

public sealed record RouteSpatialConfiguration(
	double ImmediateDistanceMetres,
	double ProximateDistanceMetres,
	double DistantDistanceMetres,
	double VeryDistantDistanceMetres,
	double DefaultRoomEquivalentMetres)
{
	public static RouteSpatialConfiguration Default { get; } = new(3.0, 10.0, 100.0, 500.0, 100.0);

	public static RouteSpatialConfiguration FromGameworld(IFuturemud? gameworld)
	{
		if (gameworld is null)
		{
			return Default;
		}

		return new RouteSpatialConfiguration(
			gameworld.GetStaticDouble("RouteCellImmediateDistanceMetres"),
			gameworld.GetStaticDouble("RouteCellProximateDistanceMetres"),
			gameworld.GetStaticDouble("RouteCellDistantDistanceMetres"),
			gameworld.GetStaticDouble("RouteCellVeryDistantDistanceMetres"),
			gameworld.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres"));
	}

	public void Validate()
	{
		if (!double.IsFinite(ImmediateDistanceMetres) || ImmediateDistanceMetres <= 0.0 ||
			!double.IsFinite(ProximateDistanceMetres) || ProximateDistanceMetres <= ImmediateDistanceMetres ||
			!double.IsFinite(DistantDistanceMetres) || DistantDistanceMetres <= ProximateDistanceMetres ||
			!double.IsFinite(VeryDistantDistanceMetres) || VeryDistantDistanceMetres <= DistantDistanceMetres ||
			!double.IsFinite(DefaultRoomEquivalentMetres) || DefaultRoomEquivalentMetres <= 0.0)
		{
			throw new InvalidOperationException(
				"RouteRoom distance settings must be finite and positive, and proximity thresholds must be strictly increasing.");
		}
	}
}

/// <summary>
/// Resolves one-dimensional RouteRoom geometry and owns the ordered locality index.
/// </summary>
public sealed class RouteSpatialService : IRouteSpatialService
{
	private readonly object _sync = new();
	private readonly TimeProvider _timeProvider;
	private readonly RouteSpatialConfiguration? _fixedConfiguration;
	private readonly Dictionary<IRoom, Dictionary<RoomLayer, SpatialBucket>> _stationaryIndex =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IPerceivable, IndexedPosition> _indexedPositions =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<ILocateable, ActiveMovementState> _activeMovements =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IRoom, Dictionary<RoomLayer, HashSet<ILocateable>>> _activeMovementIndex =
		new(ReferenceEqualityComparer.Instance);
	private readonly HashSet<IRoom> _initialisedRooms = new(ReferenceEqualityComparer.Instance);

	public RouteSpatialService(
		RouteSpatialConfiguration? fixedConfiguration = null,
		TimeProvider? timeProvider = null)
	{
		fixedConfiguration?.Validate();
		_fixedConfiguration = fixedConfiguration;
		_timeProvider = timeProvider ?? TimeProvider.System;
	}

	public static RouteSpatialService Instance { get; } = new();

	public SpatialLocation GetEffectiveLocation(ILocateable locateable)
	{
		ArgumentNullException.ThrowIfNull(locateable);

		ActiveMovementSnapshot? snapshot;
		lock (_sync)
		{
			snapshot = _activeMovements.TryGetValue(locateable, out var movement)
				? movement.Snapshot(_timeProvider, _timeProvider.GetTimestamp())
				: null;
		}

		if (snapshot is null)
		{
			return locateable.SpatialLocation;
		}

		var location = snapshot.Segment.PositionAt(snapshot.Elapsed);
		if (!TryValidateLocation(location, out var error))
		{
			throw new InvalidOperationException($"Active RouteRoom movement produced an invalid location: {error}");
		}

		return location;
	}

	public bool TryValidateLocation(SpatialLocation location, out string error)
	{
		if (location.Room is null)
		{
			error = "A spatial location must specify a room.";
			return false;
		}

		var routeRoom = location.Room.RouteDefinition;
		if (routeRoom is null)
		{
			if (location.RoutePositionMetres.HasValue)
			{
				error = $"Ordinary Room #{location.Room.Id:N0} cannot have a route coordinate.";
				return false;
			}

			error = string.Empty;
			return true;
		}

		if (!double.IsFinite(routeRoom.LengthMetres) || routeRoom.LengthMetres <= 0.0)
		{
			error = $"RouteRoom #{location.Room.Id:N0} has an invalid length.";
			return false;
		}

		if (!location.RoutePositionMetres.HasValue)
		{
			error = $"A location in RouteRoom #{location.Room.Id:N0} requires a route coordinate.";
			return false;
		}

		var position = location.RoutePositionMetres.Value;
		if (!double.IsFinite(position) || position < 0.0 || position > routeRoom.LengthMetres)
		{
			error =
				$"Route coordinate {position} is outside RouteRoom #{location.Room.Id:N0} (0-{routeRoom.LengthMetres} metres).";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public double ClampPosition(IRouteRoomDefinition routeRoom, double positionMetres)
	{
		ArgumentNullException.ThrowIfNull(routeRoom);
		if (!double.IsFinite(positionMetres))
		{
			throw new ArgumentOutOfRangeException(nameof(positionMetres), "A route coordinate must be finite.");
		}

		if (!double.IsFinite(routeRoom.LengthMetres) || routeRoom.LengthMetres <= 0.0)
		{
			throw new ArgumentException("The RouteRoom must have a finite positive length.", nameof(routeRoom));
		}

		return Math.Clamp(positionMetres, 0.0, routeRoom.LengthMetres);
	}

	public double? GetExactSeparation(SpatialLocation first, SpatialLocation second)
	{
		if (first.Room is null ||
			second.Room is null ||
			!ReferenceEquals(first.Room, second.Room) ||
			first.Layer != second.Layer ||
			first.Room.RouteDefinition is null ||
			!TryValidateLocation(first, out _) ||
			!TryValidateLocation(second, out _))
		{
			return null;
		}

		return Math.Abs(first.RoutePositionMetres!.Value - second.RoutePositionMetres!.Value);
	}

	public Proximity GetProximity(ILocateable first, ILocateable second)
	{
		ArgumentNullException.ThrowIfNull(first);
		ArgumentNullException.ThrowIfNull(second);

		if (ReferenceEquals(first, second))
		{
			return Proximity.Intimate;
		}

		var firstLocation = GetEffectiveLocation(first);
		var secondLocation = GetEffectiveLocation(second);
		if (firstLocation.Room is null || secondLocation.Room is null)
		{
			return ReferenceEquals(firstLocation.Room, secondLocation.Room)
				? Proximity.Distant
				: Proximity.Unapproximable;
		}

		if (!ReferenceEquals(firstLocation.Room, secondLocation.Room))
		{
			return Proximity.Unapproximable;
		}

		if (firstLocation.Room.RouteDefinition is null)
		{
			return Proximity.Distant;
		}

		if (!TryValidateLocation(firstLocation, out _) || !TryValidateLocation(secondLocation, out _))
		{
			return Proximity.Unapproximable;
		}

		var separation = Math.Abs(
			firstLocation.RoutePositionMetres!.Value - secondLocation.RoutePositionMetres!.Value);
		var proximity = ProximityForSeparation(separation, ResolveConfiguration(first, second));
		if (firstLocation.Layer != secondLocation.Layer && proximity < Proximity.VeryDistant)
		{
			return Proximity.VeryDistant;
		}

		return proximity;
	}

	public bool CanReach(ILocateable source, ILocateable target, double maximumDistanceMetres)
	{
		if (!double.IsFinite(maximumDistanceMetres) || maximumDistanceMetres < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumDistanceMetres));
		}

		var sourceLocation = GetEffectiveLocation(source);
		var targetLocation = GetEffectiveLocation(target);
		if (sourceLocation.Room is null ||
			targetLocation.Room is null ||
			!ReferenceEquals(sourceLocation.Room, targetLocation.Room) ||
			sourceLocation.Layer != targetLocation.Layer)
		{
			return false;
		}

		if (sourceLocation.Room.RouteDefinition is null)
		{
			return true;
		}

		return GetExactSeparation(sourceLocation, targetLocation) is { } separation &&
		       separation <= maximumDistanceMetres;
	}

	public IReadOnlyCollection<IPerceivable> GetPerceivablesWithin(
		SpatialLocation origin,
		double maximumDistanceMetres,
		Func<IPerceivable, bool>? predicate = null)
	{
		if (!double.IsFinite(maximumDistanceMetres) || maximumDistanceMetres < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumDistanceMetres));
		}

		if (!TryValidateLocation(origin, out var error))
		{
			throw new ArgumentException(error, nameof(origin));
		}

		if (origin.Room.RouteDefinition is null)
		{
			return origin.Room.Perceivables
				.Where(x => x.RoomLayer == origin.Layer)
				.Where(x => predicate?.Invoke(x) != false)
				.ToArray();
		}

		EnsureRoomIndexed(origin.Room);
		var lower = origin.RoutePositionMetres!.Value - maximumDistanceMetres;
		var upper = origin.RoutePositionMetres.Value + maximumDistanceMetres;
		List<IPerceivable> stationary;
		List<IPerceivable> active;
		lock (_sync)
		{
			stationary = _stationaryIndex.TryGetValue(origin.Room, out var layers) &&
			             layers.TryGetValue(origin.Layer, out var bucket)
				? bucket.Between(lower, upper).ToList()
				: [];
			active = _activeMovementIndex.TryGetValue(origin.Room, out var activeLayers) &&
			         activeLayers.TryGetValue(origin.Layer, out var activeBucket)
				? activeBucket.OfType<IPerceivable>().ToList()
				: [];
		}

		var results = new HashSet<IPerceivable>(stationary, ReferenceEqualityComparer.Instance);
		foreach (var perceivable in active)
		{
			var location = GetEffectiveLocation(perceivable);
			if (!ReferenceEquals(location.Room, origin.Room) ||
				location.Layer != origin.Layer ||
				!location.RoutePositionMetres.HasValue ||
				Math.Abs(location.RoutePositionMetres.Value - origin.RoutePositionMetres.Value) >
				maximumDistanceMetres)
			{
				continue;
			}

			results.Add(perceivable);
		}

		if (predicate is not null)
		{
			results.RemoveWhere(x => !predicate(x));
		}

		return results.ToArray();
	}

	public IReadOnlyCollection<IPerceivable> GetPerceivablesWithinAcrossLayers(
		SpatialLocation origin,
		double maximumDistanceMetres,
		Func<IPerceivable, bool>? predicate = null)
	{
		if (!double.IsFinite(maximumDistanceMetres) || maximumDistanceMetres < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumDistanceMetres));
		}

		if (!TryValidateLocation(origin, out var error))
		{
			throw new ArgumentException(error, nameof(origin));
		}

		if (origin.Room.RouteDefinition is null)
		{
			return origin.Room.Perceivables
				.Where(x => predicate?.Invoke(x) != false)
				.ToArray();
		}

		EnsureRoomIndexed(origin.Room);
		var lower = origin.RoutePositionMetres!.Value - maximumDistanceMetres;
		var upper = origin.RoutePositionMetres.Value + maximumDistanceMetres;
		List<IPerceivable> stationary;
		List<IPerceivable> active;
		lock (_sync)
		{
			stationary = _stationaryIndex.TryGetValue(origin.Room, out var layers)
				? layers.Values.SelectMany(x => x.Between(lower, upper)).ToList()
				: [];
			active = _activeMovementIndex.TryGetValue(origin.Room, out var activeLayers)
				? activeLayers.Values.SelectMany(x => x).OfType<IPerceivable>().Distinct().ToList()
				: [];
		}

		var results = new HashSet<IPerceivable>(stationary, ReferenceEqualityComparer.Instance);
		foreach (var perceivable in active)
		{
			var location = GetEffectiveLocation(perceivable);
			if (!ReferenceEquals(location.Room, origin.Room) ||
				!location.RoutePositionMetres.HasValue ||
				Math.Abs(location.RoutePositionMetres.Value - origin.RoutePositionMetres.Value) >
				maximumDistanceMetres)
			{
				continue;
			}

			results.Add(perceivable);
		}

		if (predicate is not null)
		{
			results.RemoveWhere(x => !predicate(x));
		}

		return results.ToArray();
	}

	public bool TryGetExitAnchor(IRoomExit exit, IRoom routeRoom, out IRouteExitAnchor? anchor)
	{
		ArgumentNullException.ThrowIfNull(exit);
		ArgumentNullException.ThrowIfNull(routeRoom);

		var exitId = exit.Exit.Id;
		anchor = routeRoom.RouteDefinition?.ExitAnchors.FirstOrDefault(x =>
			x is RouteRoomExitAnchor concrete
				? concrete.ExitId == exitId
				: x.Exit.Exit.Id == exitId);
		return anchor is not null;
	}

	public bool IsExitVisible(
		IPerceiver voyeur,
		IRoomExit exit,
		double maximumDistanceMetres,
		PerceptionTypes type = PerceptionTypes.DirectVisual,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		ArgumentNullException.ThrowIfNull(voyeur);
		ArgumentNullException.ThrowIfNull(exit);
		if (!double.IsFinite(maximumDistanceMetres) || maximumDistanceMetres < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumDistanceMetres));
		}

		var location = GetEffectiveLocation(voyeur);
		if (location.Room is null || !location.Room.IsExitVisible(voyeur, exit, type, flags))
		{
			return false;
		}

		if (location.Room.RouteDefinition is null)
		{
			return true;
		}

		return location.RoutePositionMetres.HasValue &&
		       TryGetExitAnchor(exit, location.Room, out var anchor) &&
		       DistanceToBand(location.RoutePositionMetres.Value, anchor!) <= maximumDistanceMetres;
	}

	public bool IsExitAccessible(ILocateable locateable, IRoomExit exit)
	{
		var location = GetEffectiveLocation(locateable);
		if (location.Room is null)
		{
			return false;
		}

		if (location.Room.RouteDefinition is null)
		{
			return true;
		}

		return TryGetExitAnchor(exit, location.Room, out var anchor) &&
		       location.RoutePositionMetres.HasValue &&
		       anchor!.Contains(location.RoutePositionMetres.Value);
	}

	public double? GetNearestAccessiblePosition(SpatialLocation origin, IRoomExit exit)
	{
		if (!TryValidateLocation(origin, out _) ||
			origin.Room.RouteDefinition is null ||
			!TryGetExitAnchor(exit, origin.Room, out var anchor))
		{
			return null;
		}

		return Math.Clamp(
			origin.RoutePositionMetres!.Value,
			anchor!.MinimumPositionMetres,
			anchor.MaximumPositionMetres);
	}

	private static double DistanceToBand(double positionMetres, IRouteExitAnchor anchor)
	{
		if (anchor.Contains(positionMetres))
		{
			return 0.0;
		}

		return positionMetres < anchor.MinimumPositionMetres
			? anchor.MinimumPositionMetres - positionMetres
			: positionMetres - anchor.MaximumPositionMetres;
	}

	public double? GetInheritedRoutePosition(ILocateable locateable, ILocateable? owner)
	{
		var ownLocation = GetEffectiveLocation(locateable);
		if (ownLocation.Room is not null &&
			ownLocation.Room.RouteDefinition is not null &&
			TryValidateLocation(ownLocation, out _))
		{
			return ownLocation.RoutePositionMetres;
		}

		if (owner is null)
		{
			return null;
		}

		var ownerLocation = GetEffectiveLocation(owner);
		return ownerLocation.Room is not null &&
		       ownerLocation.Room.RouteDefinition is not null &&
		       TryValidateLocation(ownerLocation, out _)
			? ownerLocation.RoutePositionMetres
			: null;
	}

	public void TrackPerceivable(IPerceivable perceivable)
	{
		ArgumentNullException.ThrowIfNull(perceivable);
		lock (_sync)
		{
			RemoveIndexedPosition(perceivable);
			if (_activeMovements.ContainsKey(perceivable))
			{
				return;
			}

			var location = perceivable.SpatialLocation;
			if (location.Room?.RouteDefinition is null ||
				!location.RoutePositionMetres.HasValue ||
				!double.IsFinite(location.RoutePositionMetres.Value) ||
				location.RoutePositionMetres.Value < 0.0 ||
				location.RoutePositionMetres.Value > location.Room.RouteDefinition.LengthMetres)
			{
				return;
			}

			if (!_stationaryIndex.TryGetValue(location.Room, out var layers))
			{
				layers = new Dictionary<RoomLayer, SpatialBucket>();
				_stationaryIndex[location.Room] = layers;
			}

			if (!layers.TryGetValue(location.Layer, out var bucket))
			{
				bucket = new SpatialBucket();
				layers[location.Layer] = bucket;
			}

			bucket.Add(location.RoutePositionMetres.Value, perceivable);
			_indexedPositions[perceivable] = new IndexedPosition(
				location.Room,
				location.Layer,
				location.RoutePositionMetres.Value);
		}
	}

	public void UntrackPerceivable(IPerceivable perceivable)
	{
		lock (_sync)
		{
			RemoveIndexedPosition(perceivable);
			RemoveActiveMovement(perceivable);
		}
	}

	public void BeginActiveMovement(
		ILocateable locateable,
		ISpatialMovementSegment segment,
		TimeSpan? elapsed = null,
		TimeSpan? delay = null)
	{
		ArgumentNullException.ThrowIfNull(locateable);
		ArgumentNullException.ThrowIfNull(segment);
		ValidateMovementSegment(segment);
		if (delay < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(delay));
		}

		var initialElapsed = ClampElapsed(elapsed ?? TimeSpan.Zero, segment.Duration);
		var start = _timeProvider.GetTimestamp();

		lock (_sync)
		{
			RemoveActiveMovement(locateable);
			if (locateable is IPerceivable perceivable)
			{
				RemoveIndexedPosition(perceivable);
			}

			var movement = new ActiveMovementState(
				segment,
				start,
				initialElapsed,
				delay ?? TimeSpan.Zero);
			_activeMovements[locateable] = movement;
			AddActiveMovement(locateable, segment.Origin.Room, segment.Origin.Layer);
		}
	}

	public bool PauseActiveMovement(ILocateable locateable)
	{
		lock (_sync)
		{
			return _activeMovements.TryGetValue(locateable, out var movement) &&
			       movement.Pause(_timeProvider, _timeProvider.GetTimestamp());
		}
	}

	public bool ResumeActiveMovement(ILocateable locateable, TimeSpan? delay = null)
	{
		if (delay < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(delay));
		}

		lock (_sync)
		{
			return _activeMovements.TryGetValue(locateable, out var movement) &&
			       movement.Resume(_timeProvider.GetTimestamp(), delay ?? TimeSpan.Zero);
		}
	}

	public bool DelayActiveMovement(ILocateable locateable, TimeSpan delay)
	{
		if (delay < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(delay));
		}

		lock (_sync)
		{
			return _activeMovements.TryGetValue(locateable, out var movement) && movement.Delay(delay);
		}
	}

	public SpatialLocation? MaterialiseActiveMovement(ILocateable locateable, bool noSave = false)
	{
		ActiveMovementSnapshot snapshot;
		lock (_sync)
		{
			if (!_activeMovements.TryGetValue(locateable, out var movement))
			{
				return null;
			}

			snapshot = movement.Snapshot(_timeProvider, _timeProvider.GetTimestamp());
			RemoveActiveMovement(locateable);
		}

		var location = snapshot.Segment.PositionAt(snapshot.Elapsed);
		if (locateable is PerceivedItem perceivedItem)
		{
			if (!perceivedItem.TrySetRoutePosition(location.RoutePositionMetres, out var error, noSave))
			{
				throw new InvalidOperationException(error);
			}
		}
		else if (locateable is IPerceivable perceivable)
		{
			TrackPerceivable(perceivable);
		}

		return location;
	}

	/// <summary>
	/// Ends lazy interpolation without sampling the clock or changing the locateable's persisted
	/// coordinate. Atomic cohort movers use this after sampling their single shared coordinate so
	/// every participant commits exactly the same point in space.
	/// </summary>
	public bool ClearActiveMovement(ILocateable locateable)
	{
		ArgumentNullException.ThrowIfNull(locateable);
		lock (_sync)
		{
			return RemoveActiveMovement(locateable);
		}
	}

	private void AddActiveMovement(ILocateable locateable, IRoom room, RoomLayer layer)
	{
		if (!_activeMovementIndex.TryGetValue(room, out var layers))
		{
			layers = new Dictionary<RoomLayer, HashSet<ILocateable>>();
			_activeMovementIndex[room] = layers;
		}

		if (!layers.TryGetValue(layer, out var bucket))
		{
			bucket = new HashSet<ILocateable>(ReferenceEqualityComparer.Instance);
			layers[layer] = bucket;
		}

		bucket.Add(locateable);
	}

	private bool RemoveActiveMovement(ILocateable locateable)
	{
		if (!_activeMovements.Remove(locateable, out var movement))
		{
			return false;
		}

		var room = movement.Segment.Origin.Room;
		var layer = movement.Segment.Origin.Layer;
		if (!_activeMovementIndex.TryGetValue(room, out var layers) ||
		    !layers.TryGetValue(layer, out var bucket))
		{
			return true;
		}

		bucket.Remove(locateable);
		if (bucket.Count == 0)
		{
			layers.Remove(layer);
		}
		if (layers.Count == 0)
		{
			_activeMovementIndex.Remove(room);
		}

		return true;
	}

	private RouteSpatialConfiguration ResolveConfiguration(params ILocateable[] locateables)
	{
		if (_fixedConfiguration is not null)
		{
			return _fixedConfiguration;
		}

		var gameworld = locateables
			.OfType<IHaveFuturemud>()
			.Select(x => x.Gameworld)
			.FirstOrDefault(x => x is not null);
		var configuration = RouteSpatialConfiguration.FromGameworld(gameworld);
		configuration.Validate();
		return configuration;
	}

	private static Proximity ProximityForSeparation(
		double separation,
		RouteSpatialConfiguration configuration)
	{
		if (separation <= configuration.ImmediateDistanceMetres)
		{
			return Proximity.Immediate;
		}

		if (separation <= configuration.ProximateDistanceMetres)
		{
			return Proximity.Proximate;
		}

		if (separation <= configuration.DistantDistanceMetres)
		{
			return Proximity.Distant;
		}

		return separation <= configuration.VeryDistantDistanceMetres
			? Proximity.VeryDistant
			: Proximity.Unapproximable;
	}

	private void EnsureRoomIndexed(IRoom room)
	{
		lock (_sync)
		{
			if (!_initialisedRooms.Add(room))
			{
				return;
			}
		}

		foreach (var perceivable in room.Perceivables.ToList())
		{
			TrackPerceivable(perceivable);
		}
	}

	private void RemoveIndexedPosition(IPerceivable perceivable)
	{
		if (!_indexedPositions.Remove(perceivable, out var indexed) ||
			!_stationaryIndex.TryGetValue(indexed.Room, out var layers) ||
			!layers.TryGetValue(indexed.Layer, out var bucket))
		{
			return;
		}

		bucket.Remove(indexed.PositionMetres, perceivable);
		if (bucket.Count == 0)
		{
			layers.Remove(indexed.Layer);
		}

		if (layers.Count == 0)
		{
			_stationaryIndex.Remove(indexed.Room);
		}
	}

	private static void ValidateMovementSegment(ISpatialMovementSegment segment)
	{
		if (segment.Origin.Room is null ||
			segment.Destination.Room is null ||
			!ReferenceEquals(segment.Origin.Room, segment.Destination.Room) ||
			segment.Origin.Layer != segment.Destination.Layer ||
			segment.Origin.Room.RouteDefinition is null ||
			!segment.Origin.RoutePositionMetres.HasValue ||
			!segment.Destination.RoutePositionMetres.HasValue ||
			!double.IsFinite(segment.Origin.RoutePositionMetres.Value) ||
			!double.IsFinite(segment.Destination.RoutePositionMetres.Value) ||
			segment.Origin.RoutePositionMetres.Value < 0.0 ||
			segment.Destination.RoutePositionMetres.Value < 0.0 ||
			segment.Origin.RoutePositionMetres.Value > segment.Origin.Room.RouteDefinition.LengthMetres ||
			segment.Destination.RoutePositionMetres.Value > segment.Origin.Room.RouteDefinition.LengthMetres ||
			segment.Duration < TimeSpan.Zero ||
			!double.IsFinite(segment.DistanceMetres) ||
			segment.DistanceMetres < 0.0 ||
			!double.IsFinite(segment.SpeedMetresPerSecond) ||
			segment.SpeedMetresPerSecond <= 0.0)
		{
			throw new ArgumentException("The spatial movement segment is not a valid linear RouteRoom movement.",
				nameof(segment));
		}
	}

	private static TimeSpan ClampElapsed(TimeSpan elapsed, TimeSpan duration)
	{
		if (elapsed <= TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}

		return elapsed >= duration ? duration : elapsed;
	}

	private sealed class SpatialBucket
	{
		private readonly SortedList<double, HashSet<IPerceivable>> _positions = new();

		public int Count => _positions.Count;

		public void Add(double position, IPerceivable perceivable)
		{
			if (!_positions.TryGetValue(position, out var perceivables))
			{
				perceivables = new HashSet<IPerceivable>(ReferenceEqualityComparer.Instance);
				_positions.Add(position, perceivables);
			}

			perceivables.Add(perceivable);
		}

		public void Remove(double position, IPerceivable perceivable)
		{
			if (!_positions.TryGetValue(position, out var perceivables))
			{
				return;
			}

			perceivables.Remove(perceivable);
			if (perceivables.Count == 0)
			{
				_positions.Remove(position);
			}
		}

		public IEnumerable<IPerceivable> Between(double minimum, double maximum)
		{
			var start = LowerBound(_positions.Keys, minimum);
			for (var index = start; index < _positions.Count && _positions.Keys[index] <= maximum; index++)
			{
				foreach (var perceivable in _positions.Values[index])
				{
					yield return perceivable;
				}
			}
		}

		private static int LowerBound(IList<double> values, double target)
		{
			var low = 0;
			var high = values.Count;
			while (low < high)
			{
				var middle = low + (high - low) / 2;
				if (values[middle] < target)
				{
					low = middle + 1;
				}
				else
				{
					high = middle;
				}
			}

			return low;
		}
	}

	private sealed class ActiveMovementState
	{
		private long _startedAtTimestamp;
		private TimeSpan _delay;
		private TimeSpan _elapsedBeforeStart;
		private bool _paused;

		public ActiveMovementState(
			ISpatialMovementSegment segment,
			long startedAtTimestamp,
			TimeSpan elapsedBeforeStart,
			TimeSpan delay)
		{
			Segment = segment;
			_startedAtTimestamp = startedAtTimestamp;
			_elapsedBeforeStart = elapsedBeforeStart;
			_delay = delay;
		}

		public ISpatialMovementSegment Segment { get; }

		public ActiveMovementSnapshot Snapshot(TimeProvider timeProvider, long nowTimestamp)
		{
			return new ActiveMovementSnapshot(Segment, ElapsedAt(timeProvider, nowTimestamp));
		}

		public bool Pause(TimeProvider timeProvider, long nowTimestamp)
		{
			if (_paused)
			{
				return false;
			}

			_elapsedBeforeStart = ElapsedAt(timeProvider, nowTimestamp);
			_paused = true;
			return true;
		}

		public bool Resume(long nowTimestamp, TimeSpan delay)
		{
			if (!_paused || delay < TimeSpan.Zero)
			{
				return false;
			}

			_startedAtTimestamp = nowTimestamp;
			_delay = delay;
			_paused = false;
			return true;
		}

		public bool Delay(TimeSpan delay)
		{
			if (_paused)
			{
				return false;
			}

			_delay += delay;
			return true;
		}

		private TimeSpan ElapsedAt(TimeProvider timeProvider, long nowTimestamp)
		{
			if (_paused)
			{
				return _elapsedBeforeStart;
			}

			var elapsedSinceStart = timeProvider.GetElapsedTime(_startedAtTimestamp, nowTimestamp) - _delay;
			if (elapsedSinceStart <= TimeSpan.Zero)
			{
				return _elapsedBeforeStart;
			}

			return ClampElapsed(_elapsedBeforeStart + elapsedSinceStart, Segment.Duration);
		}
	}

	private sealed record ActiveMovementSnapshot(ISpatialMovementSegment Segment, TimeSpan Elapsed);
	private sealed record IndexedPosition(IRoom Room, RoomLayer Layer, double PositionMetres);
}
