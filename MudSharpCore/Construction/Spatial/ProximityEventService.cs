#nullable enable

using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;

namespace MudSharp.Construction;

/// <summary>
/// Maintains locality indexes for the comparatively small set of perceivables that have opted in to proximity
/// change events. Moving ordinary characters therefore queries listeners, not every object in their cell.
/// </summary>
public sealed class ProximityEventService : IProximityEventService
{
	private readonly object _sync = new();
	private readonly Dictionary<IPerceivable, HashSet<Registration>> _registrationsByReceiver =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IPerceivable, HashSet<Registration>> _registrationsByEffectiveHost =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<ICell, Dictionary<RoomLayer, HashSet<Registration>>> _ordinaryCellIndex =
		new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<ICell, Dictionary<RoomLayer, RouteReceiverBucket>> _routeCellIndex =
		new(ReferenceEqualityComparer.Instance);

	public IProximityEventRegistration Register(IPerceivable receiver,
		Proximity maximumObservedProximity = Proximity.VeryDistant)
	{
		ArgumentNullException.ThrowIfNull(receiver);
		if (maximumObservedProximity is < Proximity.Intimate or > Proximity.VeryDistant)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumObservedProximity));
		}

		var registration = new Registration(this, receiver, maximumObservedProximity);
		lock (_sync)
		{
			if (!_registrationsByReceiver.TryGetValue(receiver, out var registrations))
			{
				registrations = new HashSet<Registration>();
				_registrationsByReceiver[receiver] = registrations;
			}

			registrations.Add(registration);
			Index(registration);
		}

		return registration;
	}

	public IProximityChangeBatch BeginChange(ProximityChangeCause cause, params IPerceivable[] affected)
	{
		var batch = new ChangeBatch(this, cause);
		foreach (var perceivable in affected)
		{
			if (perceivable is not null)
			{
				batch.Track(perceivable);
			}
		}

		return batch;
	}

	private void Dispose(Registration registration)
	{
		lock (_sync)
		{
			if (!_registrationsByReceiver.TryGetValue(registration.Receiver, out var registrations) ||
				!registrations.Remove(registration))
			{
				return;
			}

			Unindex(registration);
			if (registrations.Count == 0)
			{
				_registrationsByReceiver.Remove(registration.Receiver);
			}
		}
	}

	private void Complete(ChangeBatch batch)
	{
		List<ProximityChange> changes;
		lock (_sync)
		{
			foreach (var subject in batch.Subjects)
			{
				foreach (var registration in RegistrationsAffectedBy(subject))
				{
					Unindex(registration);
					Index(registration);
				}
			}

			foreach (var subject in batch.Subjects)
			{
				foreach (var registration in CandidateRegistrationsFor(subject))
				{
					batch.TrackAfterChangePair(registration.Receiver, subject);
				}

				foreach (var registration in RegistrationsAffectedBy(subject))
				{
					foreach (var counterpart in CounterpartsForRegisteredReceiver(registration))
					{
						batch.TrackAfterChangePair(registration.Receiver, counterpart);
					}
				}
			}

			changes = batch.BuildChanges();
		}

		foreach (var change in changes)
		{
			change.Receiver.HandleEvent(
				EventType.PerceivableProximityChanged,
				change.Receiver,
				change.Counterpart,
				(double)change.Previous,
				(double)change.Current,
				change.Cause.ToString());
		}
	}

	private IEnumerable<Registration> RegistrationsFor(IPerceivable receiver)
	{
		return _registrationsByReceiver.TryGetValue(receiver, out var registrations)
			? registrations.ToList()
			: [];
	}

	private IEnumerable<Registration> RegistrationsAffectedBy(IPerceivable subject)
	{
		var results = new HashSet<Registration>(RegistrationsFor(subject));
		if (_registrationsByEffectiveHost.TryGetValue(subject, out var hosted))
		{
			results.UnionWith(hosted);
		}

		return results.ToList();
	}

	private IEnumerable<Registration> CandidateRegistrationsFor(IPerceivable subject)
	{
		if (EffectiveLocation(subject) is not { Cell: not null } location)
		{
			return [];
		}

		if (location.Cell.RouteDefinition is null)
		{
			return _ordinaryCellIndex.TryGetValue(location.Cell, out var layers) &&
			       layers.TryGetValue(location.Layer, out var registrations)
				? registrations.ToList()
				: [];
		}

		var configuration = RouteSpatialConfiguration.FromGameworld(subject.Gameworld);
		var position = location.RoutePositionMetres ?? location.Cell.RouteDefinition.DefaultPositionMetres;
		var results = new HashSet<Registration>();
		if (!_routeCellIndex.TryGetValue(location.Cell, out var routeLayers))
		{
			return results;
		}

		foreach ((var layer, var bucket) in routeLayers)
		{
			foreach (var registration in bucket.Between(position - configuration.VeryDistantDistanceMetres,
				         position + configuration.VeryDistantDistanceMetres))
			{
				if (layer == location.Layer || registration.MaximumObservedProximity >= Proximity.VeryDistant)
				{
					results.Add(registration);
				}
			}
		}

		return results;
	}

	private IEnumerable<IPerceivable> CounterpartsForRegisteredReceiver(Registration registration)
	{
		var receiver = registration.Receiver;
		if (EffectiveLocation(receiver) is not { Cell: not null } location)
		{
			return [];
		}

		if (location.Cell.RouteDefinition is null)
		{
			return location.Cell.Perceivables
				.Where(x => x.RoomLayer == location.Layer && !ReferenceEquals(x, receiver))
				.ToList();
		}

		var configuration = RouteSpatialConfiguration.FromGameworld(receiver.Gameworld);
		var maximumDistance = MaximumDistance(registration.MaximumObservedProximity, configuration);
		return RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(location, maximumDistance)
			.Where(x => !ReferenceEquals(x, receiver))
			.ToList();
	}

	private static double MaximumDistance(Proximity proximity, RouteSpatialConfiguration configuration)
	{
		return proximity switch
		{
			Proximity.Intimate or Proximity.Immediate => configuration.ImmediateDistanceMetres,
			Proximity.Proximate => configuration.ProximateDistanceMetres,
			Proximity.Distant => configuration.DistantDistanceMetres,
			_ => configuration.VeryDistantDistanceMetres
		};
	}

	private static IPerceivable EffectiveHost(IPerceivable perceivable)
	{
		return perceivable is IGameItem item
			? item.LocationLevelPerceivable
			: perceivable;
	}

	private static SpatialLocation? EffectiveLocation(IPerceivable perceivable)
	{
		var locationSource = EffectiveHost(perceivable);
		return locationSource.Location is null
			? null
			: RouteSpatialService.Instance.GetEffectiveLocation(locationSource);
	}

	private void Index(Registration registration)
	{
		var host = EffectiveHost(registration.Receiver);
		if (EffectiveLocation(registration.Receiver) is not { Cell: not null } location)
		{
			return;
		}

		registration.Location = location;
		registration.Host = host;
		if (!_registrationsByEffectiveHost.TryGetValue(host, out var hostedRegistrations))
		{
			hostedRegistrations = new HashSet<Registration>();
			_registrationsByEffectiveHost[host] = hostedRegistrations;
		}
		hostedRegistrations.Add(registration);
		if (location.Cell.RouteDefinition is null)
		{
			if (!_ordinaryCellIndex.TryGetValue(location.Cell, out var layers))
			{
				layers = new Dictionary<RoomLayer, HashSet<Registration>>();
				_ordinaryCellIndex[location.Cell] = layers;
			}

			if (!layers.TryGetValue(location.Layer, out var registrations))
			{
				registrations = new HashSet<Registration>();
				layers[location.Layer] = registrations;
			}

			registrations.Add(registration);
			return;
		}

		if (!_routeCellIndex.TryGetValue(location.Cell, out var routeLayers))
		{
			routeLayers = new Dictionary<RoomLayer, RouteReceiverBucket>();
			_routeCellIndex[location.Cell] = routeLayers;
		}

		if (!routeLayers.TryGetValue(location.Layer, out var bucket))
		{
			bucket = new RouteReceiverBucket();
			routeLayers[location.Layer] = bucket;
		}

		bucket.Add(registration, location.RoutePositionMetres ?? location.Cell.RouteDefinition.DefaultPositionMetres);
	}

	private void Unindex(Registration registration)
	{
		if (registration.Host is not null && _registrationsByEffectiveHost.TryGetValue(registration.Host, out var hostedRegistrations))
		{
			hostedRegistrations.Remove(registration);
			if (hostedRegistrations.Count == 0)
			{
				_registrationsByEffectiveHost.Remove(registration.Host);
			}
		}
		registration.Host = null;

		if (registration.Location?.Cell is not { } cell)
		{
			return;
		}

		if (cell.RouteDefinition is null)
		{
			if (_ordinaryCellIndex.TryGetValue(cell, out var layers) &&
				layers.TryGetValue(registration.Location.Value.Layer, out var registrations))
			{
				registrations.Remove(registration);
				if (registrations.Count == 0)
				{
					layers.Remove(registration.Location.Value.Layer);
				}
				if (layers.Count == 0)
				{
					_ordinaryCellIndex.Remove(cell);
				}
			}
		}
		else if (_routeCellIndex.TryGetValue(cell, out var routeLayers) &&
		         routeLayers.TryGetValue(registration.Location.Value.Layer, out var bucket))
		{
			bucket.Remove(registration);
			if (bucket.Count == 0)
			{
				routeLayers.Remove(registration.Location.Value.Layer);
			}
			if (routeLayers.Count == 0)
			{
				_routeCellIndex.Remove(cell);
			}
		}

		registration.Location = null;
	}

	private sealed class Registration(ProximityEventService service, IPerceivable receiver,
		Proximity maximumObservedProximity) : IProximityEventRegistration
	{
		private bool _disposed;
		public IPerceivable Receiver { get; } = receiver;
		public Proximity MaximumObservedProximity { get; } = maximumObservedProximity;
		public SpatialLocation? Location { get; set; }
		public IPerceivable? Host { get; set; }

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			service.Dispose(this);
		}
	}

	private sealed class ChangeBatch(ProximityEventService service, ProximityChangeCause cause) : IProximityChangeBatch
	{
		private readonly Dictionary<Relationship, Proximity> _before = new();
		private readonly Dictionary<IPerceivable, SpatialLocation?> _previousLocations =
			new(ReferenceEqualityComparer.Instance);
		private bool _completed;
		public ProximityChangeCause Cause { get; } = cause;
		public HashSet<IPerceivable> Subjects { get; } = new(ReferenceEqualityComparer.Instance);

		public void Track(IPerceivable perceivable)
		{
			if (!Subjects.Add(perceivable))
			{
				return;
			}

			_previousLocations[perceivable] = EffectiveLocation(perceivable);

			foreach (var registration in service.CandidateRegistrationsFor(perceivable))
			{
				TrackPair(registration.Receiver, perceivable);
			}

			foreach (var registration in service.RegistrationsAffectedBy(perceivable))
			{
				foreach (var counterpart in service.CounterpartsForRegisteredReceiver(registration))
				{
					TrackPair(registration.Receiver, counterpart);
				}
			}

			if (perceivable.PositionTarget is not null)
			{
				TrackPair(perceivable, perceivable.PositionTarget);
				TrackPair(perceivable.PositionTarget, perceivable);
			}

			foreach (var targeter in perceivable.TargetedBy.ToList())
			{
				TrackPair(perceivable, targeter);
				TrackPair(targeter, perceivable);
			}
		}

		public void TrackPair(IPerceivable receiver, IPerceivable counterpart)
		{
			if (receiver is null || counterpart is null || ReferenceEquals(receiver, counterpart) ||
				!service._registrationsByReceiver.ContainsKey(receiver))
			{
				return;
			}

			var relationship = new Relationship(receiver, counterpart);
			_before.TryAdd(relationship, receiver.GetProximity(counterpart));
		}

		internal void TrackAfterChangePair(IPerceivable receiver, IPerceivable counterpart)
		{
			if (receiver is null || counterpart is null || ReferenceEquals(receiver, counterpart) ||
				!service._registrationsByReceiver.ContainsKey(receiver))
			{
				return;
			}

			var relationship = new Relationship(receiver, counterpart);
			if (_before.ContainsKey(relationship))
			{
				return;
			}

			_before[relationship] = HasUnchangedSpatialState(receiver) || HasUnchangedSpatialState(counterpart)
				? receiver.GetProximity(counterpart)
				: Proximity.Unapproximable;
		}

		private bool HasUnchangedSpatialState(IPerceivable perceivable)
		{
			return _previousLocations.TryGetValue(perceivable, out var previous) &&
				Equals(previous, EffectiveLocation(perceivable));
		}

		public void TrackParty(IEnumerable<ICharacter> members)
		{
			var partyMembers = members.Where(x => x is not null).ToList();
			foreach (var receiver in partyMembers.Where(x => service._registrationsByReceiver.ContainsKey(x)))
			{
				foreach (var counterpart in partyMembers.Where(x => !ReferenceEquals(x, receiver)))
				{
					TrackPair(receiver, counterpart);
				}
			}
		}

		public void Complete()
		{
			if (_completed)
			{
				return;
			}

			_completed = true;
			service.Complete(this);
		}

		public List<ProximityChange> BuildChanges()
		{
			return _before
				.Select(x => new ProximityChange(x.Key.Receiver, x.Key.Counterpart, x.Value,
					x.Key.Receiver.GetProximity(x.Key.Counterpart), Cause))
				.Where(x => x.Previous != x.Current)
				.Where(x => x.Previous <= MaximumFor(x.Receiver) || x.Current <= MaximumFor(x.Receiver))
				.ToList();
		}

		private Proximity MaximumFor(IPerceivable receiver)
		{
			return service.RegistrationsFor(receiver).Select(x => x.MaximumObservedProximity).DefaultIfEmpty(Proximity.Intimate).Max();
		}

		public void Dispose()
		{
			Complete();
		}
	}

	private readonly record struct Relationship(IPerceivable Receiver, IPerceivable Counterpart);
	private readonly record struct ProximityChange(IPerceivable Receiver, IPerceivable Counterpart,
		Proximity Previous, Proximity Current, ProximityChangeCause Cause);

	private sealed class RouteReceiverBucket
	{
		private readonly SortedList<double, HashSet<Registration>> _byPosition = new();
		private readonly Dictionary<Registration, double> _positions = [];
		public int Count => _positions.Count;

		public void Add(Registration registration, double position)
		{
			_positions[registration] = position;
			if (!_byPosition.TryGetValue(position, out var registrations))
			{
				registrations = [];
				_byPosition.Add(position, registrations);
			}
			registrations.Add(registration);
		}

		public void Remove(Registration registration)
		{
			if (!_positions.Remove(registration, out var position) ||
				!_byPosition.TryGetValue(position, out var registrations))
			{
				return;
			}

			registrations.Remove(registration);
			if (registrations.Count == 0)
			{
				_byPosition.Remove(position);
			}
		}

		public IEnumerable<Registration> Between(double lower, double upper)
		{
			var keys = _byPosition.Keys;
			var index = LowerBound(keys, lower);
			for (; index < keys.Count && keys[index] <= upper; index++)
			{
				foreach (var registration in _byPosition.Values[index])
				{
					yield return registration;
				}
			}
		}

		private static int LowerBound(IList<double> values, double value)
		{
			var low = 0;
			var high = values.Count;
			while (low < high)
			{
				var middle = low + (high - low) / 2;
				if (values[middle] < value)
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
}
