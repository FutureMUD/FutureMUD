#nullable enable

using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public sealed class VehicleJourney : SaveableItem, IVehicleJourney
{
	private readonly List<IVehicleJourneyEvent> _events = [];
	private VehicleJourneyState _state;
	private IVehicleRouteStop? _currentStop;
	private IVehicleRouteStop? _nextStop;
	private TimeSpan _delay;
	private DateTimeOffset _lastCheckpointUtc;

	public VehicleJourney(DB.VehicleJourney dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = $"Vehicle Journey #{dbitem.Id:N0}";
		OperationId = Guid.Parse(dbitem.OperationId);
		Service = gameworld.VehicleServices.Get(dbitem.VehicleServiceId) ??
			throw new InvalidOperationException($"Vehicle journey #{dbitem.Id:N0} references missing service #{dbitem.VehicleServiceId:N0}.");
		Route = gameworld.VehicleRoutes.Get(dbitem.VehicleRouteId, dbitem.VehicleRouteRevision) ??
			throw new InvalidOperationException($"Vehicle journey #{dbitem.Id:N0} references missing route {dbitem.VehicleRouteId:N0}r{dbitem.VehicleRouteRevision:N0}.");
		Vehicle = gameworld.Vehicles.Get(dbitem.VehicleId) ??
			throw new InvalidOperationException($"Vehicle journey #{dbitem.Id:N0} references missing vehicle #{dbitem.VehicleId:N0}.");
		var journeyRowState = (VehicleJourneyState)dbitem.State;
		_currentStop = Route.Stops.FirstOrDefault(x => x.Id == dbitem.CurrentStopId);
		_nextStop = Route.Stops.FirstOrDefault(x => x.Id == dbitem.NextStopId);
		ScheduledDeparture = ParseTime(dbitem.ScheduledDeparture, gameworld, dbitem.Id, nameof(ScheduledDeparture));
		ExpectedDeparture = ParseTime(dbitem.ExpectedDeparture, gameworld, dbitem.Id, nameof(ExpectedDeparture));
		_delay = TimeSpan.FromMilliseconds(dbitem.DelayMilliseconds);
		_lastCheckpointUtc = new DateTimeOffset(DateTime.SpecifyKind(dbitem.LastCheckpointUtc, DateTimeKind.Utc));
		_events.AddRange(dbitem.Events.OrderBy(x => x.Sequence)
			.Select(x => (IVehicleJourneyEvent)new VehicleJourneyEvent(this, x, gameworld)));
		var latestEvent = _events.LastOrDefault();
		_state = VehicleJourneyStateRules.ResolveDurableState(journeyRowState, latestEvent?.State);
		if (latestEvent?.OccurredAtUtc > _lastCheckpointUtc)
		{
			_lastCheckpointUtc = latestEvent.OccurredAtUtc;
		}
	}

	public VehicleJourney(IFuturemud gameworld, IVehicleService service, MudDateTime scheduledDeparture)
	{
		Gameworld = gameworld;
		OperationId = Guid.NewGuid();
		Service = service;
		Route = service.Route;
		Vehicle = service.Vehicle;
		_state = VehicleJourneyState.Scheduled;
		_currentStop = Route.Stops.OrderBy(x => x.Sequence).FirstOrDefault();
		_nextStop = Route.Stops.OrderBy(x => x.Sequence).Skip(1).FirstOrDefault();
		ScheduledDeparture = new MudDateTime(scheduledDeparture);
		ExpectedDeparture = new MudDateTime(scheduledDeparture);
		_lastCheckpointUtc = DateTimeOffset.UtcNow;
		using (new FMDB())
		{
			var row = BuildDatabaseRow();
			FMDB.Context.VehicleJourneys.Add(row);
			FMDB.Context.SaveChanges();
			_id = row.Id;
			_name = $"Vehicle Journey #{Id:N0}";
		}
		RecordEvent(VehicleJourneyEventType.Scheduled, "Journey scheduled.");
	}

	private static MudDateTime ParseTime(string text, IFuturemud gameworld, long id, string field)
	{
		return MudDateTime.FromStoredStringOrFallback(text, gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "VehicleJourney", id,
			$"Vehicle Journey #{id:N0}", field);
	}

	public override string FrameworkItemType => "VehicleJourney";
	public Guid OperationId { get; }
	public IVehicleService Service { get; }
	public IVehicleRoute Route { get; }
	public IVehicle Vehicle { get; }
	public VehicleJourneyState State => _state;
	public IVehicleRouteStop? CurrentStop => _currentStop;
	public IVehicleRouteStop? NextStop => _nextStop;
	public MudDateTime ScheduledDeparture { get; }
	public MudDateTime ExpectedDeparture { get; private set; }
	public TimeSpan Delay => _delay;
	public DateTimeOffset LastCheckpointUtc => _lastCheckpointUtc;
	public bool BoardingOpen => _state is VehicleJourneyState.Boarding or VehicleJourneyState.Dwelling ||
		Vehicle.Dockings.Any(x => x.State == VehicleDockingState.BoardingOpen);
	public string StatusReason => _state is VehicleJourneyState.Held or VehicleJourneyState.Faulted or VehicleJourneyState.Cancelled
		? _events.LastOrDefault()?.Message ?? string.Empty
		: string.Empty;
	public IReadOnlyList<IVehicleJourneyEvent> Events => _events;
	public event VehicleJourneyStateChangedEvent? StateChanged;

	internal void StartOrResume(VehicleJourneyCoordinator coordinator)
	{
		if (_state is VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted)
		{
			return;
		}
		if (Route is VehicleRoute route && !route.TopologyIsCurrent)
		{
			Transition(VehicleJourneyState.Faulted, VehicleJourneyEventType.Faulted,
				"The pinned RouteCell topology changed; the journey failed closed.");
			return;
		}
		switch (_state)
		{
			case VehicleJourneyState.Scheduled:
				BeginBoarding(coordinator, _currentStop?.DwellDuration ?? TimeSpan.Zero);
				break;
			case VehicleJourneyState.Boarding:
			case VehicleJourneyState.Dwelling:
			case VehicleJourneyState.Held:
				Schedule(coordinator, AttemptDeparture, TimeSpan.Zero, "vehicle journey resume");
				break;
			case VehicleJourneyState.Departing:
			case VehicleJourneyState.EnRoute:
				BeginCurrentLeg(coordinator);
				break;
		}
	}

	private void BeginBoarding(VehicleJourneyCoordinator coordinator, TimeSpan boardingWindow)
	{
		if (!coordinator.TryOpenBoarding(this, out var reason))
		{
			Hold(coordinator, reason);
			return;
		}
		if (_state != VehicleJourneyState.Boarding)
		{
			Transition(VehicleJourneyState.Boarding, VehicleJourneyEventType.BoardingOpened,
				$"Boarding opened at {_currentStop?.Name ?? "the current stop"}.");
		}
		Schedule(coordinator, AttemptDeparture, boardingWindow, "vehicle journey boarding window");
	}

	private void AttemptDeparture(VehicleJourneyCoordinator coordinator)
	{
		if (CancelIfMaximumHoldExpired(coordinator))
		{
			return;
		}
		if (_nextStop is null)
		{
			Transition(VehicleJourneyState.Arrived, VehicleJourneyEventType.Completed, "Journey completed.");
			return;
		}
		if (Service is VehicleService service && service.AuditErrors(includeActiveJourney: false) is { Count: > 0 } errors)
		{
			Hold(coordinator, errors[0]);
			return;
		}
		if (!coordinator.TryCloseBoarding(this, out var reason))
		{
			Hold(coordinator, reason);
			return;
		}
		Transition(VehicleJourneyState.Departing, VehicleJourneyEventType.BoardingClosed,
			"Boarding closed for departure.");
		BeginCurrentLeg(coordinator);
	}

	private void BeginCurrentLeg(VehicleJourneyCoordinator coordinator)
	{
		var leg = Route.Legs.FirstOrDefault(x => x.OriginStop == _currentStop && x.DestinationStop == _nextStop);
		if (leg is null)
		{
			Transition(VehicleJourneyState.Faulted, VehicleJourneyEventType.Faulted,
				"The next compiled route leg is missing.");
			return;
		}
		if (!coordinator.TryBeginLeg(this, leg,
				result => Schedule(coordinator, _ => CompleteLeg(coordinator, result), TimeSpan.Zero,
					"vehicle journey leg completion"), out var reason))
		{
			Hold(coordinator, reason);
			return;
		}
		if (_state != VehicleJourneyState.EnRoute)
		{
			Transition(VehicleJourneyState.EnRoute, VehicleJourneyEventType.Departed,
				$"Departed {_currentStop?.Name ?? "the current stop"} for {_nextStop?.Name ?? "the next stop"}.");
		}
		Checkpoint("Physical route leg started.");
	}

	private void CompleteLeg(VehicleJourneyCoordinator coordinator, VehicleJourneyLegResult result)
	{
		if (!result.Succeeded)
		{
			Transition(VehicleJourneyState.Faulted, VehicleJourneyEventType.Faulted,
				string.IsNullOrWhiteSpace(result.Reason) ? "Physical route movement failed." : result.Reason);
			return;
		}
		_currentStop = _nextStop;
		var ordered = Route.Stops.OrderBy(x => x.Sequence).ToList();
		var index = ordered.IndexOf(_currentStop!);
		_nextStop = index >= 0 && index + 1 < ordered.Count ? ordered[index + 1] : null;
		Changed = true;
		var arrivalMessage = $"Arrived at {_currentStop?.Name ?? "a route stop"}.";
		RecordEvent(VehicleJourneyEventType.StopArrived, arrivalMessage);
		VehicleJourneyFutureProgEvents.Dispatch(this, VehicleJourneyEventType.StopArrived, arrivalMessage);
		if (!coordinator.TryOpenBoarding(this, out var boardingReason))
		{
			var stopName = _currentStop?.Name ?? "the route stop";
			Transition(
				VehicleJourneyStateRules.ArrivalState(false, _nextStop is null),
				VehicleJourneyEventType.Faulted,
				$"The vehicle reached {stopName}, but boarding could not be opened: {boardingReason}");
			return;
		}

		if (VehicleJourneyStateRules.ArrivalState(true, _nextStop is null) == VehicleJourneyState.Arrived)
		{
			Transition(VehicleJourneyState.Arrived, VehicleJourneyEventType.Completed,
				"Journey completed at the terminal stop.");
			return;
		}

		Transition(VehicleJourneyState.Dwelling, VehicleJourneyEventType.Dwelling,
			$"Dwelling at {_currentStop!.Name}.");
		Schedule(coordinator, AttemptDeparture, _currentStop.DwellDuration, "vehicle journey stop dwell");
	}

	private void Hold(VehicleJourneyCoordinator coordinator, string reason)
	{
		if (_state != VehicleJourneyState.Held)
		{
			Transition(VehicleJourneyState.Held, VehicleJourneyEventType.Held,
				string.IsNullOrWhiteSpace(reason) ? "Departure is temporarily held." : reason);
		}
		if (CancelIfMaximumHoldExpired(coordinator))
		{
			return;
		}
		AddDelay(Service.RetryInterval,
			$"Departure remains held: {(string.IsNullOrWhiteSpace(reason) ? "readiness has not been restored" : reason)}");
		Schedule(coordinator, AttemptDeparture, Service.RetryInterval, "vehicle journey held retry");
	}

	private bool CancelIfMaximumHoldExpired(VehicleJourneyCoordinator coordinator)
	{
		if (_state != VehicleJourneyState.Held ||
		    VehicleJourneyStateRules.HoldEpisodeStartedAt(_events) is not { } holdStarted ||
		    !VehicleJourneyStateRules.HoldHasExpired(holdStarted, DateTimeOffset.UtcNow, Service.MaximumHold))
		{
			return false;
		}

		var heldReason = _events
			.LastOrDefault(x => x.EventType == VehicleJourneyEventType.Held)?
			.Message;
		Cancel(coordinator, string.IsNullOrWhiteSpace(heldReason)
			? "Maximum hold time exceeded."
			: $"Maximum hold time exceeded: {heldReason}");
		return true;
	}

	private void Schedule(VehicleJourneyCoordinator coordinator, Action<VehicleJourneyCoordinator> action,
		TimeSpan delay, string description)
	{
		Gameworld.Scheduler.Destroy(this, ScheduleType.VehicleJourney);
		Gameworld.Scheduler.AddSchedule(new Schedule<VehicleJourneyCoordinator>(coordinator, action,
			ScheduleType.VehicleJourney, delay < TimeSpan.Zero ? TimeSpan.Zero : delay, description));
	}

	internal void Cancel(VehicleJourneyCoordinator coordinator, string reason)
	{
		if (_state is VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted)
		{
			return;
		}
		((IVehicleJourneyOperations)coordinator).Cancel(this);
		Gameworld.Scheduler.Destroy(this, ScheduleType.VehicleJourney);
		Transition(VehicleJourneyState.Cancelled, VehicleJourneyEventType.Cancelled,
			string.IsNullOrWhiteSpace(reason) ? "Journey cancelled." : reason);
	}

	public void AddDelay(TimeSpan amount, string reason)
	{
		if (amount <= TimeSpan.Zero)
		{
			return;
		}
		_delay += amount;
		ExpectedDeparture = ScheduledDeparture + MudTimeSpan.FromSeconds(_delay.TotalSeconds);
		_lastCheckpointUtc = DateTimeOffset.UtcNow;
		Changed = true;
		RecordEvent(VehicleJourneyEventType.DelayChanged, reason);
		VehicleJourneyFutureProgEvents.Dispatch(this, VehicleJourneyEventType.DelayChanged, reason);
	}

	public void Checkpoint(string message)
	{
		_lastCheckpointUtc = DateTimeOffset.UtcNow;
		Changed = true;
		RecordEvent(VehicleJourneyEventType.Checkpointed, message);
	}

	private void Transition(VehicleJourneyState state, VehicleJourneyEventType eventType, string message)
	{
		var previous = _state;
		if (!VehicleJourneyStateRules.CanTransition(previous, state))
		{
			throw new InvalidOperationException($"Illegal vehicle journey transition {previous} -> {state}.");
		}
		_state = state;
		_lastCheckpointUtc = DateTimeOffset.UtcNow;
		Changed = true;
		var item = RecordEvent(eventType, message);
		StateChanged?.Invoke(this, previous, state, item);
		VehicleJourneyFutureProgEvents.Dispatch(this, eventType, message);
	}

	private IVehicleJourneyEvent RecordEvent(VehicleJourneyEventType type, string message)
	{
		var sequence = _events.Any() ? _events.Max(x => x.Sequence) + 1 : 0;
		DB.VehicleJourneyEvent row;
		using (new FMDB())
		{
			var journeyRow = FMDB.Context.VehicleJourneys.Find(Id) ??
				throw new InvalidOperationException($"Vehicle journey #{Id:N0} no longer has a durable database row.");
			ApplyRuntimeState(journeyRow);
			row = new DB.VehicleJourneyEvent
			{
				VehicleJourneyId = Id,
				Sequence = sequence,
				IdempotencyKey = $"{OperationId:N}:{sequence}",
				EventType = (int)type,
				State = (int)_state,
				OccurredAtUtc = DateTime.UtcNow,
				WorldTime = ScheduledDeparture.Calendar?.CurrentDateTime?.GetDateTimeString(),
				Message = message
			};
			FMDB.Context.VehicleJourneyEvents.Add(row);
			FMDB.Context.SaveChanges();
		}
		var runtime = new VehicleJourneyEvent(this, row, Gameworld);
		_events.Add(runtime);
		Changed = false;
		return runtime;
	}

	private void ApplyRuntimeState(DB.VehicleJourney row)
	{
		row.State = (int)_state;
		row.CurrentStopId = _currentStop?.Id;
		row.NextStopId = _nextStop?.Id;
		row.ExpectedDeparture = ExpectedDeparture.GetDateTimeString();
		row.DelayMilliseconds = (long)_delay.TotalMilliseconds;
		row.LastCheckpointUtc = _lastCheckpointUtc.UtcDateTime;
	}

	private DB.VehicleJourney BuildDatabaseRow()
	{
		return new DB.VehicleJourney
		{
			OperationId = OperationId.ToString("D"), VehicleServiceId = Service.Id,
			VehicleRouteId = Route.Id, VehicleRouteRevision = Route.RevisionNumber,
			VehicleId = Vehicle.Id, State = (int)_state, CurrentStopId = _currentStop?.Id,
			NextStopId = _nextStop?.Id, ScheduledDeparture = ScheduledDeparture.GetDateTimeString(),
			ExpectedDeparture = ExpectedDeparture.GetDateTimeString(), DelayMilliseconds = (long)_delay.TotalMilliseconds,
			LastCheckpointUtc = _lastCheckpointUtc.UtcDateTime
		};
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var row = FMDB.Context.VehicleJourneys.Find(Id);
			if (row is not null)
			{
				ApplyRuntimeState(row);
				FMDB.Context.SaveChanges();
			}
		}
		Changed = false;
	}

	public IProgVariable GetProperty(string property) => property.ToLowerInvariant() switch
	{
		"id" => new NumberVariable(Id), "operationid" => new TextVariable(OperationId.ToString("D")),
		"service" => Service, "route" => Route, "vehicleid" => new NumberVariable(Vehicle.Id),
		"state" => new TextVariable(State.DescribeEnum()),
		"currentstop" => CurrentStop is { } currentStop
			? currentStop.Location.Cell
			: new NullVariable(ProgVariableTypes.Location),
		"nextstop" => NextStop is { } nextStop
			? nextStop.Location.Cell
			: new NullVariable(ProgVariableTypes.Location),
		"delay" => new TimeSpanVariable(Delay), "scheduleddeparture" => ScheduledDeparture,
		"expecteddeparture" => ExpectedDeparture, "boardingopen" => new BooleanVariable(BoardingOpen),
		"statusreason" => new TextVariable(StatusReason),
		_ => throw new NotSupportedException($"Invalid VehicleJourney property {property}.")
	};
	public ProgVariableTypes Type => ProgVariableTypes.VehicleJourney;
	public object GetObject => this;

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.VehicleJourney,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number, ["operationid"] = ProgVariableTypes.Text,
				["service"] = ProgVariableTypes.VehicleService, ["route"] = ProgVariableTypes.VehicleRoute,
				["vehicleid"] = ProgVariableTypes.Number, ["state"] = ProgVariableTypes.Text,
				["currentstop"] = ProgVariableTypes.Location, ["nextstop"] = ProgVariableTypes.Location,
				["delay"] = ProgVariableTypes.TimeSpan, ["scheduleddeparture"] = ProgVariableTypes.MudDateTime,
				["expecteddeparture"] = ProgVariableTypes.MudDateTime, ["boardingopen"] = ProgVariableTypes.Boolean,
				["statusreason"] = ProgVariableTypes.Text
			}, new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The durable journey identity.", ["operationid"] = "The idempotent operation identity.",
				["service"] = "The owning service.", ["route"] = "The pinned route revision.",
				["vehicleid"] = "The vehicle identity.", ["state"] = "The current journey state.",
				["currentstop"] = "The current stop cell.", ["nextstop"] = "The next stop cell.",
				["delay"] = "The accumulated delay.", ["scheduleddeparture"] = "The scheduled departure.",
				["expecteddeparture"] = "The delay-adjusted departure.", ["boardingopen"] = "Whether boarding is open.",
				["statusreason"] = "The hold, cancellation, or fault reason."
			});
	}
}

public sealed class VehicleJourneyEvent : FrameworkItem, IVehicleJourneyEvent
{
	public VehicleJourneyEvent(IVehicleJourney journey, DB.VehicleJourneyEvent row, IFuturemud gameworld)
	{
		Journey = journey; _id = row.Id; _name = $"Vehicle Journey Event #{row.Id:N0}"; Sequence = row.Sequence;
		EventType = (VehicleJourneyEventType)row.EventType; State = (VehicleJourneyState)row.State;
		OccurredAtUtc = new DateTimeOffset(DateTime.SpecifyKind(row.OccurredAtUtc, DateTimeKind.Utc)); Message = row.Message;
		WorldTime = string.IsNullOrWhiteSpace(row.WorldTime) ? null : MudDateTime.FromStoredStringOrFallback(row.WorldTime,
			gameworld, StoredMudDateTimeFallback.CurrentDateTime, "VehicleJourneyEvent", row.Id, _name, nameof(WorldTime));
	}
	public override string FrameworkItemType => "VehicleJourneyEvent";
	public IVehicleJourney Journey { get; }
	public long Sequence { get; }
	public VehicleJourneyEventType EventType { get; }
	public VehicleJourneyState State { get; }
	public DateTimeOffset OccurredAtUtc { get; }
	public MudDateTime? WorldTime { get; }
	public string Message { get; }
}
