#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public sealed class VehicleServiceSchedule : IVehicleServiceSchedule
{
	public VehicleServiceSchedule(DB.VehicleServiceSchedule dbitem, IFuturemud gameworld, long serviceId, string serviceName)
	{
		ReferenceDeparture = MudDateTime.FromStoredStringOrFallback(
			dbitem.ReferenceDeparture,
			gameworld,
			StoredMudDateTimeFallback.CurrentDateTime,
			"VehicleService",
			serviceId,
			serviceName,
			nameof(ReferenceDeparture));
		NextDeparture = MudDateTime.FromStoredStringOrFallback(
			dbitem.NextDeparture,
			gameworld,
			StoredMudDateTimeFallback.CurrentDateTime,
			"VehicleService",
			serviceId,
			serviceName,
			nameof(NextDeparture));
		Recurrence = new RecurringInterval
		{
			Type = (IntervalType)dbitem.RecurrenceType,
			IntervalAmount = dbitem.RecurrenceIntervalAmount,
			Modifier = dbitem.RecurrenceModifier,
			SecondaryModifier = dbitem.RecurrenceSecondaryModifier,
			OrdinalFallbackMode = (OrdinalFallbackMode)dbitem.RecurrenceFallbackMode
		};
		if (Recurrence.IntervalAmount <= 0)
		{
			throw new InvalidOperationException($"Vehicle service #{serviceId:N0} {serviceName} has a non-positive recurrence interval.");
		}
	}

	public VehicleServiceSchedule(MudDateTime referenceDeparture, RecurringInterval recurrence)
	{
		ArgumentNullException.ThrowIfNull(referenceDeparture);
		ArgumentNullException.ThrowIfNull(recurrence);
		if (recurrence.IntervalAmount <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(recurrence), "A vehicle service recurrence interval must be positive.");
		}
		ReferenceDeparture = new MudDateTime(referenceDeparture);
		Recurrence = recurrence;
		NextDeparture = recurrence.GetNextDateTime(referenceDeparture);
	}

	public MudDateTime ReferenceDeparture { get; private set; }
	public RecurringInterval Recurrence { get; private set; }
	public MudDateTime NextDeparture { get; private set; }

	public bool NormaliseToNextFuture()
	{
		var changed = false;
		var current = ReferenceDeparture.Calendar.CurrentDateTime;
		while (NextDeparture <= current)
		{
			NextDeparture = Recurrence.GetNextDateTimeAfter(NextDeparture);
			changed = true;
		}
		return changed;
	}

	public MudDateTime ConsumeAndAdvance()
	{
		var consumed = new MudDateTime(NextDeparture);
		NextDeparture = Recurrence.GetNextDateTimeAfter(NextDeparture);
		NormaliseToNextFuture();
		return consumed;
	}

	internal void RestoreNextDeparture(MudDateTime departure)
	{
		NextDeparture = new MudDateTime(departure);
	}

	public void Replace(MudDateTime referenceDeparture, RecurringInterval recurrence)
	{
		ReferenceDeparture = new MudDateTime(referenceDeparture);
		Recurrence = recurrence;
		NextDeparture = recurrence.GetNextDateTime(referenceDeparture);
		NormaliseToNextFuture();
	}
}

public sealed class VehicleService : SavableKeywordedItem, IVehicleService
{
	private string _keywordsText;
	private IVehicleRoute _route;
	private IVehicle _vehicle;
	private VehicleServiceSchedule _schedule;
	private VehicleServiceOperatorMode _operatorMode;
	private TimeSpan _retryInterval;
	private TimeSpan _maximumHold;
	private bool _enabled;
	private ITemporalListener? _departureListener;

	public VehicleService(DB.VehicleService dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_keywordsText = dbitem.Keywords;
		_keywords = new Lazy<List<string>>(() => _keywordsText
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToList());
		_route = gameworld.VehicleRoutes.Get(dbitem.VehicleRouteId, dbitem.VehicleRouteRevision) ??
			throw new InvalidOperationException($"Vehicle service #{dbitem.Id:N0} references missing route {dbitem.VehicleRouteId:N0}r{dbitem.VehicleRouteRevision:N0}.");
		_vehicle = gameworld.Vehicles.Get(dbitem.VehicleId) ??
			throw new InvalidOperationException($"Vehicle service #{dbitem.Id:N0} references missing vehicle #{dbitem.VehicleId:N0}.");
		_schedule = new VehicleServiceSchedule(dbitem.Schedule, gameworld, Id, Name);
		_operatorMode = (VehicleServiceOperatorMode)dbitem.OperatorMode;
		_retryInterval = TimeSpan.FromMilliseconds(dbitem.RetryIntervalMilliseconds);
		_maximumHold = TimeSpan.FromMilliseconds(dbitem.MaximumHoldMilliseconds);
		_enabled = dbitem.Enabled;
	}

	public VehicleService(IFuturemud gameworld, string name, IVehicleRoute route, IVehicle vehicle,
		MudDateTime referenceDeparture, RecurringInterval recurrence)
	{
		Gameworld = gameworld;
		_id = gameworld.VehicleServices.NextID();
		_name = name.TitleCase();
		_keywordsText = name.ToLowerInvariant();
		_keywords = new Lazy<List<string>>(() => GetKeywordsFromSDesc(_keywordsText).ToList());
		_route = route;
		_vehicle = vehicle;
		_schedule = new VehicleServiceSchedule(referenceDeparture, recurrence);
		_operatorMode = VehicleServiceOperatorMode.Automatic;
		_retryInterval = TimeSpan.FromSeconds(Math.Max(1.0,
			gameworld.GetStaticDouble("VehicleServiceRetrySeconds")));
		_maximumHold = TimeSpan.FromMinutes(Math.Max(0.0,
			gameworld.GetStaticDouble("VehicleServiceDefaultMaximumHoldMinutes")));
		_enabled = false;

		using (new FMDB())
		{
			FMDB.Context.VehicleServices.Add(new DB.VehicleService
			{
				Id = Id,
				Name = Name,
				Keywords = _keywordsText,
				VehicleRouteId = route.Id,
				VehicleRouteRevision = route.RevisionNumber,
				VehicleId = vehicle.Id,
				OperatorMode = (int)_operatorMode,
				RetryIntervalMilliseconds = (long)_retryInterval.TotalMilliseconds,
				MaximumHoldMilliseconds = (long)_maximumHold.TotalMilliseconds,
				Enabled = _enabled,
				Schedule = BuildDatabaseSchedule()
			});
			FMDB.Context.SaveChanges();
		}
	}

	public override string FrameworkItemType => "VehicleService";
	public IVehicleRoute Route => _route;
	public IVehicle Vehicle => _vehicle;
	public IVehicleServiceSchedule Schedule => _schedule;
	public VehicleServiceOperatorMode OperatorMode => _operatorMode;
	public TimeSpan RetryInterval => _retryInterval;
	public TimeSpan MaximumHold => _maximumHold;
	public bool Enabled => _enabled;
	public IVehicleJourney? ActiveJourney => Gameworld.VehicleJourneys
		.Where(x => x.Service.Id == Id)
		.OrderByDescending(x => x.Id)
		.FirstOrDefault(x => x.State is not (VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted));
	public bool IsReady => AuditErrors(includeActiveJourney: true).Count == 0;
	public string ReadinessReason => AuditErrors(includeActiveJourney: true).FirstOrDefault() ?? string.Empty;

	public IReadOnlyList<string> AuditErrors(bool includeActiveJourney)
	{
		var errors = new List<string>();
		if (!IsApprovedPinnedRevision(_route.Status))
		{
			errors.Add("The service route is not an approved revision.");
		}
		if (_route is VehicleRoute concreteRoute && !concreteRoute.TopologyIsCurrent)
		{
			errors.Add("The service route has stale RouteCell topology pins.");
		}
		if (_vehicle is null)
		{
			errors.Add("The assigned vehicle is missing.");
			return errors;
		}
		if (_vehicle.Destroyed || _vehicle.Disabled)
		{
			errors.Add("The assigned vehicle is disabled or destroyed.");
		}
		errors.AddRange(AuditPlatformBindings(_route, _vehicle));
		var routeProfiles = _vehicle.Prototype.MovementProfiles
			.Where(x => x.MovementType == VehicleMovementProfileType.Route)
			.ToList();
		if (!routeProfiles.Any())
		{
			errors.Add("The assigned vehicle has no Route movement profile.");
		}
		else if (_operatorMode == VehicleServiceOperatorMode.Automatic &&
		         routeProfiles.All(x => !x.AutomaticOperationCapable))
		{
			errors.Add("The assigned vehicle has no Route movement profile capable of automatic operation.");
		}
		else if (_operatorMode == VehicleServiceOperatorMode.Onboard && _vehicle.Controller is null)
		{
			errors.Add("Onboard operation requires a current vehicle controller.");
		}
		if (includeActiveJourney && Gameworld.VehicleJourneys.Any(x => x.Vehicle.Id == _vehicle.Id &&
		    x.Service.Id != Id && x.State is not (VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted)))
		{
			errors.Add("The assigned vehicle is already committed to another active service journey.");
		}
		return errors;
	}

	internal static bool IsApprovedPinnedRevision(RevisionStatus status)
	{
		return status is RevisionStatus.Current or RevisionStatus.Revised;
	}

	internal static IReadOnlyList<string> AuditPlatformBindings(IVehicleRoute route, IVehicle vehicle)
	{
		var errors = new List<string>();
		var authoredAccessPointIds = (vehicle.Prototype.AccessPoints ?? [])
			.Select(x => x.Id)
			.ToHashSet();
		var liveAccessPoints = (vehicle.AccessPoints ?? []).ToList();
		foreach (var stop in route.Stops.OrderBy(x => x.Sequence))
		{
			var bindings = stop.PlatformBindings.OrderBy(x => x.Id).ToList();
			if (vehicle.Prototype.Scale == VehicleScale.RoomScale && bindings.Count == 0)
			{
				errors.Add(
					$"Route stop #{stop.Id:N0} {stop.Name} has no platform binding for assigned RoomScale vehicle #{vehicle.Id:N0} {vehicle.Name}. Author a platform and compatible access-point binding before enabling this service.");
				continue;
			}

			foreach (var binding in bindings)
			{
				if (binding.AccessPoint is null)
				{
					errors.Add(
						$"Route stop #{stop.Id:N0} {stop.Name} platform binding #{binding.Id:N0} has no access-point prototype. Assign a valid access point before enabling this service.");
					continue;
				}

				if (!authoredAccessPointIds.Contains(binding.AccessPoint.Id))
				{
					errors.Add(
						$"Route stop #{stop.Id:N0} {stop.Name} platform binding #{binding.Id:N0} references access-point prototype #{binding.AccessPoint.Id:N0} {binding.AccessPoint.Name}, which does not belong to assigned vehicle prototype #{vehicle.Prototype.Id:N0} {vehicle.Prototype.Name}. Assign a compatible vehicle or correct the platform binding.");
					continue;
				}

				if (liveAccessPoints.Any(x =>
					x.Prototype?.Id == binding.AccessPoint.Id &&
					x.Vehicle is not null &&
					(ReferenceEquals(x.Vehicle, vehicle) || x.Vehicle.Id == vehicle.Id)))
				{
					continue;
				}

				errors.Add(
					$"Route stop #{stop.Id:N0} {stop.Name} platform binding #{binding.Id:N0} requires access-point prototype #{binding.AccessPoint.Id:N0} {binding.AccessPoint.Name}, but assigned vehicle #{vehicle.Id:N0} {vehicle.Name} has no matching live access point. Assign a compatible vehicle or repair/recreate its access-point records.");
			}
		}

		return errors;
	}

	public bool TrySetRoute(IVehicleRoute route, out string reason)
	{
		if (!CanReassign(out reason))
		{
			return false;
		}
		_route = route;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public bool TrySetVehicle(IVehicle vehicle, out string reason)
	{
		if (!CanReassign(out reason))
		{
			return false;
		}
		_vehicle = vehicle;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	private bool CanReassign(out string reason)
	{
		if (_enabled)
		{
			reason = "Disable this service before changing its route or vehicle.";
			return false;
		}
		if (ActiveJourney is not null)
		{
			reason = "This service has an active journey; cancel or complete it before changing its route or vehicle.";
			return false;
		}
		reason = string.Empty;
		return true;
	}

	public void SetOperatorMode(VehicleServiceOperatorMode mode)
	{
		_operatorMode = mode;
		Changed = true;
	}

	public void SetRetryInterval(TimeSpan interval)
	{
		_retryInterval = interval;
		Changed = true;
	}

	public void SetMaximumHold(TimeSpan interval)
	{
		_maximumHold = interval;
		Changed = true;
	}

	public void SetEnabled(bool enabled)
	{
		_enabled = enabled;
		Changed = true;
		if (!enabled)
		{
			_departureListener?.CancelListener();
			_departureListener = null;
		}
	}

	public void SetNameAndKeywords(string name, string? keywords = null)
	{
		_name = name.TitleCase();
		_keywordsText = string.IsNullOrWhiteSpace(keywords) ? name.ToLowerInvariant() : keywords.ToLowerInvariant();
		_keywords = new Lazy<List<string>>(() => _keywordsText
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToList());
		Changed = true;
	}

	public void SetSchedule(MudDateTime reference, RecurringInterval recurrence)
	{
		_schedule.Replace(reference, recurrence);
		Changed = true;
	}

	public void StartScheduling(VehicleJourneyCoordinator coordinator)
	{
		_departureListener?.CancelListener();
		_departureListener = null;
		if (!_enabled)
		{
			return;
		}

		if (_schedule.NormaliseToNextFuture())
		{
			Changed = true;
			Gameworld.SaveManager.Flush();
		}

		_departureListener = ListenerFactory.CreateDateTimeListener(
			_schedule.NextDeparture,
			_ => DepartureDue(coordinator),
			[],
			$"Vehicle service #{Id:N0} departure");
	}

	private void DepartureDue(VehicleJourneyCoordinator coordinator)
	{
		_departureListener = null;
		var departure = _schedule.ConsumeAndAdvance();
		VehicleJourney? journey = null;
		try
		{
			using (new FMDB())
			{
				using var transaction = FMDB.Context.Database.BeginTransaction();
				var schedule = FMDB.Context.VehicleServiceSchedules.Find(Id) ??
				               throw new InvalidOperationException($"Vehicle service #{Id:N0} has no durable schedule row.");
				ApplyDatabaseSchedule(schedule);
				if (_enabled && ActiveJourney is null)
				{
					journey = coordinator.CreateJourneyWithoutRegistration(this, departure);
				}
				FMDB.Context.SaveChanges();
				transaction.Commit();
			}
			Changed = false;
		}
		catch
		{
			_schedule.RestoreNextDeparture(departure);
			throw;
		}

		if (journey is not null)
		{
			coordinator.RegisterJourney(journey);
			coordinator.StartOrResume(journey);
		}

		StartScheduling(coordinator);
	}

	private DB.VehicleServiceSchedule BuildDatabaseSchedule()
	{
		return new DB.VehicleServiceSchedule
		{
			ReferenceDeparture = _schedule.ReferenceDeparture.GetDateTimeString(),
			NextDeparture = _schedule.NextDeparture.GetDateTimeString(),
			RecurrenceType = (int)_schedule.Recurrence.Type,
			RecurrenceIntervalAmount = _schedule.Recurrence.IntervalAmount,
			RecurrenceModifier = _schedule.Recurrence.Modifier,
			RecurrenceSecondaryModifier = _schedule.Recurrence.SecondaryModifier,
			RecurrenceFallbackMode = (int)_schedule.Recurrence.OrdinalFallbackMode
		};
	}

	private void ApplyDatabaseSchedule(DB.VehicleServiceSchedule schedule)
	{
		schedule.ReferenceDeparture = _schedule.ReferenceDeparture.GetDateTimeString();
		schedule.NextDeparture = _schedule.NextDeparture.GetDateTimeString();
		schedule.RecurrenceType = (int)_schedule.Recurrence.Type;
		schedule.RecurrenceIntervalAmount = _schedule.Recurrence.IntervalAmount;
		schedule.RecurrenceModifier = _schedule.Recurrence.Modifier;
		schedule.RecurrenceSecondaryModifier = _schedule.Recurrence.SecondaryModifier;
		schedule.RecurrenceFallbackMode = (int)_schedule.Recurrence.OrdinalFallbackMode;
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleServices.Find(Id);
			if (dbitem is null)
			{
				Changed = false;
				return;
			}
			dbitem.Name = Name;
			dbitem.Keywords = _keywordsText;
			dbitem.VehicleRouteId = _route.Id;
			dbitem.VehicleRouteRevision = _route.RevisionNumber;
			dbitem.VehicleId = _vehicle.Id;
			dbitem.OperatorMode = (int)_operatorMode;
			dbitem.RetryIntervalMilliseconds = (long)_retryInterval.TotalMilliseconds;
			dbitem.MaximumHoldMilliseconds = (long)_maximumHold.TotalMilliseconds;
			dbitem.Enabled = _enabled;
			var schedule = FMDB.Context.VehicleServiceSchedules.Find(Id);
			if (schedule is null)
			{
				schedule = BuildDatabaseSchedule();
				schedule.VehicleServiceId = Id;
				FMDB.Context.VehicleServiceSchedules.Add(schedule);
			}
			else
			{
				ApplyDatabaseSchedule(schedule);
			}
			FMDB.Context.SaveChanges();
		}
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var audit = AuditErrors(includeActiveJourney: true);
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle Service #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Route: #{Route.Id.ToString("N0", actor)}r{Route.RevisionNumber.ToString("N0", actor)} {Route.Name.ColourName()}");
		sb.AppendLine($"Vehicle: #{Vehicle.Id.ToString("N0", actor)} {Vehicle.Name.ColourName()}");
		sb.AppendLine($"Operator: {OperatorMode.DescribeEnum().ColourName()}");
		sb.AppendLine($"Schedule: {Schedule.Recurrence.Describe(Schedule.ReferenceDeparture.Calendar).ColourValue()} from {Schedule.ReferenceDeparture.ToString().ColourValue()}");
		sb.AppendLine($"Next departure: {Schedule.NextDeparture.ToString().ColourValue()}");
		sb.AppendLine($"Retry: {RetryInterval.Describe(actor).ColourValue()}; maximum hold: {MaximumHold.Describe(actor).ColourValue()}");
		sb.AppendLine($"Enabled: {Enabled.ToColouredString()}");
		if (ActiveJourney is { } active)
		{
			sb.AppendLine($"Active journey: #{active.Id.ToString("N0", actor)} {active.State.DescribeEnum().ColourName()}, delay {active.Delay.Describe(actor).ColourValue()}");
		}
		sb.AppendLine(audit.Any() ? "Audit:".ColourError() : "Audit: ready".Colour(Telnet.Green));
		foreach (var error in audit)
		{
			sb.AppendLine($"\t{error.ColourError()}");
		}
		return sb.ToString();
	}

	public VehicleService Clone(string name)
	{
		var clone = new VehicleService(Gameworld, name, Route, Vehicle,
			_schedule.ReferenceDeparture, _schedule.Recurrence);
		clone.SetOperatorMode(_operatorMode);
		clone.SetRetryInterval(_retryInterval);
		clone.SetMaximumHold(_maximumHold);
		clone.SetEnabled(false);
		clone.Save();
		return clone;
	}

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"route" => Route,
			"vehicleid" => new NumberVariable(Vehicle.Id),
			"operator" => new TextVariable(OperatorMode.DescribeEnum()),
			"enabled" => new BooleanVariable(Enabled),
			"nextdeparture" => Schedule.NextDeparture,
			"currentjourney" => ActiveJourney is { } activeJourney
				? activeJourney
				: new NullVariable(ProgVariableTypes.VehicleJourney),
			"ready" => new BooleanVariable(IsReady),
			"readinessreason" => new TextVariable(ReadinessReason),
			_ => throw new NotSupportedException($"Invalid VehicleService property {property}.")
		};
	}

	public ProgVariableTypes Type => ProgVariableTypes.VehicleService;
	public object GetObject => this;

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.VehicleService,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["route"] = ProgVariableTypes.VehicleRoute,
				["vehicleid"] = ProgVariableTypes.Number,
				["operator"] = ProgVariableTypes.Text,
				["enabled"] = ProgVariableTypes.Boolean,
				["nextdeparture"] = ProgVariableTypes.MudDateTime,
				["currentjourney"] = ProgVariableTypes.VehicleJourney,
				["ready"] = ProgVariableTypes.Boolean,
				["readinessreason"] = ProgVariableTypes.Text
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The service identity.",
				["name"] = "The service name.",
				["route"] = "The pinned approved route revision.",
				["vehicleid"] = "The assigned vehicle identity.",
				["operator"] = "The automatic or onboard operator mode.",
				["enabled"] = "Whether future departures are enabled.",
				["nextdeparture"] = "The next in-world departure time.",
				["currentjourney"] = "The active journey, or null.",
				["ready"] = "Whether this service is operationally ready.",
				["readinessreason"] = "The first fail-closed readiness reason, or empty text."
			});
	}
}
