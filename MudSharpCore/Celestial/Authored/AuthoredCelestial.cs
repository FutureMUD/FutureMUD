#nullable enable

using System.Runtime.CompilerServices;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial.Authored;

internal sealed record PreparedAuthoredCelestial(CompiledAuthoredCelestial Compiled, ICalendar Calendar, CelestialState State, long Ticks);

public abstract class AuthoredCelestial : PerceivedItem, ICelestialObject, IAuthoredCelestial, IDisposable
{
	private CompiledAuthoredCelestial _compiled;
	private CelestialState _live;
	private long _liveTicks;
	private bool _hasLive;
	private bool _delivering;
	private bool _disposed;
	private long _generation;
	private AuthoredCelestialCursor _cursor;
	private AuthoredEcho[] _due = [];
	private readonly ConditionalWeakTable<object, Subscriber> _subscribers = new();
	private sealed class Subscriber { public long Generation; }
	public ICalendar Calendar { get; private set; }
	public IClock Clock { get; }
	public CompiledAuthoredCelestial Compiled => _compiled;
	public long IntrinsicEvaluationCount { get; private set; }
	public override string FrameworkItemType => "Celestial";
	public override ProgVariableTypes Type => ProgVariableTypes.CelestialObject;
	public override SizeCategory Size => SizeCategory.Titanic;
	public PerceptionTypes PerceivableTypes => PerceptionTypes.AllVisual;
	public event CelestialUpdateHandler? MinuteUpdateEvent;
	public bool CelestialAngleIsUsedToDetermineTimeOfDay => _compiled.Eligible;
	public double CelestialDaysPerYear => (double)_compiled.AnnualPeriod / _compiled.MinutesPerDay;
	public double CurrentCelestialDay => (double)_live.AnnualMinute / _compiled.MinutesPerDay;
	public CelestialState LiveState => _live;

	protected AuthoredCelestial(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud gameworld) : base(id)
	{
		Gameworld = gameworld;
		Clock = clock ?? throw new ArgumentException("The feed clock does not exist.");
		_compiled = CompileCandidate(definition);
		Calendar = ResolveCalendar(_compiled.CalendarId);
		ApplyDescriptions();
		Refresh(false);
		Clock.MinutesUpdated += AddMinutes;
		Clock.TimeChanged += Resynchronise;
	}

	private ICalendar ResolveCalendar(long id)
	{
		var calendar = Gameworld.Calendars.Get(id) ?? throw new ArgumentException($"Calendar #{id} does not exist.");
		AuthoredMath.Require(calendar.FeedClock?.Id == Clock.Id, "Calendar and feed clock must share a time context.");
		return calendar;
	}

	public CompiledAuthoredCelestial CompileCandidate(AuthoredCelestialDefinition definition)
	{
		_ = ResolveCalendar(definition.CalendarId);
		return new(definition, Clock.SecondsPerMinute, Clock.MinutesPerHour, Clock.HoursPerDay, Limits(Gameworld));
	}

	public static AuthoredCelestialLimits Limits(IFuturemud gameworld)
	{
		int Setting(string key, int fallback)
		{
			var value = gameworld.GetStaticConfiguration(key);
			return int.TryParse(value, out var result) && result > 0 ? result : fallback;
		}
		return new(Setting("AuthoredCelestialSourceBytes", new AuthoredCelestialLimits().SourceBytes), Setting("AuthoredCelestialEntries", 100_000),
			Setting("AuthoredCelestialPreviewSamples", 10_000), Setting("AuthoredCelestialPreviewEvents", 1_000));
	}

	public void Activate(AuthoredCelestialDefinition definition) => Activate(PrepareActivation(definition));
	internal PreparedAuthoredCelestial PrepareActivation(AuthoredCelestialDefinition definition)
	{
		var candidate = CompileCandidate(definition);
		AuthoredMath.Require(candidate.Kind == _compiled.Kind, "An edit cannot change the persisted celestial type; create or clone a new object.");
		var calendar = ResolveCalendar(candidate.CalendarId);
		// Validate live time before changing any state or subscriptions.
		var instant = calendar.CurrentInstant;
		AuthoredMath.Require(!instant.IsNever, "The owning calendar has no current instant.");
		var state = candidate.Evaluate(instant.Ticks);
		var minute = AuthoredMath.FloorDiv(checked(instant.Ticks - candidate.AnchorTicks), candidate.SecondsPerMinute);
		var ticks = checked((long)((Int128)candidate.AnchorTicks + (Int128)minute * candidate.SecondsPerMinute));
		return new(candidate, calendar, state, ticks);
	}
	internal void Activate(PreparedAuthoredCelestial prepared)
	{
		_compiled = prepared.Compiled;
		Calendar = prepared.Calendar;
		_cursor = default;
		_live = prepared.State;
		_liveTicks = prepared.Ticks;
		_hasLive = true;
		IntrinsicEvaluationCount++;
		_generation++;
		_due = [];
		_delivering = false;
		ApplyDescriptions();
		Changed = true;
		MinuteUpdateEvent?.Invoke(this);
	}

	private void ApplyDescriptions()
	{
		_name = _compiled.Name;
		_shortDescription = _compiled.Name;
		_fullDescription = _compiled.Description;
	}

	public bool TryCanonicalise(MudInstant instant, out MudInstant canonical, out string error)
	{
		canonical = MudInstant.Never;
		error = string.Empty;
		if (instant.IsNever || instant.Epoch != MudInstant.CurrentEpoch || !instant.HasSourceContext)
		{
			error = "A supported instant with explicit calendar and clock context is required.";
			return false;
		}
		if (instant.SourceClockId != Clock.Id)
		{
			error = "The source and celestial clocks cannot be converted.";
			return false;
		}
		if (instant.SourceCalendarId == Calendar.Id)
		{
			canonical = instant;
			return true;
		}
		try
		{
			var converted = instant.ToMudDateTime(Calendar, Clock, Clock.PrimaryTimezone);
			if (converted.Date is not null) canonical = MudInstant.FromMudDateTime(converted);
		}
		catch (Exception ex) when (ex is OverflowException or ArgumentException or InvalidOperationException)
		{
			error = ex.Message;
			return false;
		}
		if (!canonical.IsNever) return true;
		error = "The source calendar cannot be converted to the celestial's underlying time basis.";
		return false;
	}

	public CelestialState EvaluateAt(MudInstant instant)
	{
		if (!TryCanonicalise(instant, out var canonical, out var error)) throw new ArgumentException(error);
		return _compiled.Evaluate(canonical.Ticks);
	}

	public CelestialEventResult FindNext(MudInstant reference, CelestialEventRequest request)
	{
		if (!TryCanonicalise(reference, out var canonical, out var error)) return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, error);
		var result = _compiled.FindNext(canonical, request);
		if (result.Found)
		{
			var day = AuthoredMath.FloorDiv(result.Instant.Ticks, checked(_compiled.MinutesPerDay * Clock.SecondsPerMinute));
			if (day < int.MinValue || day > int.MaxValue) return CelestialEventResult.Failure(CelestialEventStatus.OutOfRange, "The result exceeds the display calendar's supported day range.");
		}
		return result;
	}
	public CelestialCapabilities GetCapabilities() => _compiled.Capabilities;
	public void AddMinutes(int numberOfMinutes) => Resynchronise();
	public void AddMinutes() { if (!Clock.IsTimeBeingSet) Refresh(true); }
	private void Resynchronise() => Refresh(false);

	private void Refresh(bool normalTick)
	{
		if (_disposed) return;
		var instant = Calendar.CurrentInstant;
		if (instant.IsNever) throw new InvalidOperationException($"Celestial #{Id} {Name}: owning calendar has no current instant.");
		var minute = AuthoredMath.FloorDiv(checked(instant.Ticks - _compiled.AnchorTicks), Clock.SecondsPerMinute);
		var ticks = checked((long)((Int128)_compiled.AnchorTicks + (Int128)minute * Clock.SecondsPerMinute));
		if (_hasLive && ticks == _liveTicks && normalTick) return;
		var contiguous = normalTick && _hasLive && (Int128)ticks - _liveTicks == Clock.SecondsPerMinute;
		if (!_hasLive || ticks != _liveTicks)
		{
			_live = _compiled.Evaluate(ticks, ref _cursor);
			IntrinsicEvaluationCount++;
		}
		_liveTicks = ticks;
		_hasLive = true;
		_due = contiguous ? _compiled.DueEchoes(ticks) : [];
		_generation++;
		_delivering = contiguous;
		try { MinuteUpdateEvent?.Invoke(this); }
		finally { _delivering = false; _due = []; }
	}

	public double CurrentElevationAngle(GeographicCoordinate geography) => _live.Elevation;
	public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle) => _live.Azimuth;
	public double CurrentIllumination(GeographicCoordinate geography) => _live.SourceLux;
	public CelestialInformation CurrentPosition(GeographicCoordinate geography) => new(this, _live.Azimuth, _live.Elevation, _live.Direction);
	public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography) => AuthoredMath.TimeOfDay(_live, _compiled.Eligible);
	public CelestialInformation ReturnNewCelestialInformation(ILocation location, CelestialInformation celestialStatus, GeographicCoordinate coordinate)
	{
		var subscriber = _subscribers.GetOrCreateValue(location);
		var deliver = celestialStatus is not null && _delivering && subscriber.Generation != _generation;
		subscriber.Generation = _generation;
		if (deliver) foreach (var echo in _due) DeliverEcho(echo, location);
		return CurrentPosition(coordinate);
	}
	public void ForgetSubscriber(object subscriber) => _subscribers.Remove(subscriber);

	public bool CanReceiveEcho(ICharacter character, CelestialEchoAudience audience)
	{
		return character.Location?.Celestials.Contains(this) == true && character.CanSee(this) &&
			character.Location.CurrentWeather(character)?.ObscuresViewOfSky != true &&
			character.Location.OutdoorsType(character) is CellOutdoorsType.Outdoors or CellOutdoorsType.IndoorsWithWindows &&
			(audience == CelestialEchoAudience.SkyVisible || _live.Elevation >= 0);
	}
	protected virtual void DeliverEcho(AuthoredEcho echo, ILocation location)
	{
		var text = echo.Text.Fullstop().SubstituteANSIColour().ProperSentences();
		foreach (var character in location.Characters)
		{
			if (!CanReceiveEcho(character, echo.Audience)) continue;
			character.OutputHandler.Send(character.Location.OutdoorsType(character) == CellOutdoorsType.Outdoors
				? text : $"{"[Outside]".ColourValue()} {text}");
		}
	}

	public string Describe(CelestialInformation information)
	{
		var elevation = information.LastAscensionAngle;
		var compass = new[] { "north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest" };
		var direction = compass[(int)Math.Round(information.LastAzimuthAngle / (Math.PI / 4)) % 8];
		var position = elevation > Math.PI / 2 - 1e-8 ? "overhead" : elevation < -Math.PI / 2 + 1e-8 ? "beneath the world" :
			elevation < 0 ? $"below the {direction} horizon" : $"in the {direction}, {elevation.RadiansToDegrees():N1} degrees above the horizon";
		var motion = _live.Motion switch { CelestialMotion.Rising => "rising", CelestialMotion.Falling => "falling", CelestialMotion.Level => "moving level", _ => "stationary" };
		return $"{Name} is {position}, {motion}{(_live.Phase is { } phase ? $", {phase.Name.Describe()}" : string.Empty)}";
	}

	public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		var description = base.HowSeen(voyeur, proper, type, colour, flags);
		return type == DescriptionType.Full && (voyeur?.CanSee(this, flags) ?? true)
			? $"{description}\n{Describe(new(this, _live.Azimuth, _live.Elevation, _live.Direction)).Fullstop()}"
			: description;
	}

	public override void Register(IOutputHandler handler) { }
	public override object DatabaseInsert() => throw new InvalidOperationException("Use the authored celestial creation workflow.");
	public override void SetIDFromDatabase(object dbitem) => _id = ((Models.Celestial)dbitem).Id;
	public override void Save()
	{
		using (new FMDB())
		{
			var row = FMDB.Context.Celestials.Find(Id) ?? throw new InvalidOperationException($"Celestial #{Id} no longer exists.");
			row.Definition = _compiled.Serialize();
			row.CelestialType = _compiled.Kind.ToString();
			row.FeedClockId = Clock.Id;
			FMDB.Context.SaveChanges();
		}
		Changed = false;
	}
	public void Dispose()
	{
		if (_disposed) return;
		Clock.MinutesUpdated -= AddMinutes;
		Clock.TimeChanged -= Resynchronise;
		MinuteUpdateEvent = null;
		_subscribers.Clear();
		_disposed = true;
	}

	public static AuthoredCelestial Load(Models.Celestial row, IFuturemud gameworld)
	{
		try
		{
			var definition = AuthoredCelestialFormat.Parse(row.Definition, Limits(gameworld));
			AuthoredMath.Require(row.CelestialType == definition.Kind.ToString(), "Envelope type does not match definition type.");
			var clock = gameworld.Clocks.Get(row.FeedClockId) ?? throw new ArgumentException($"Feed clock #{row.FeedClockId} does not exist.");
			return Create(row.Id, definition, clock, gameworld);
		}
		catch (Exception ex) when (ex is ArgumentException or System.Text.Json.JsonException or OverflowException)
		{
			throw new InvalidOperationException($"Celestial #{row.Id} ({row.CelestialType}), Definition: {ex.Message}", ex);
		}
	}
	public static AuthoredCelestial Create(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud world) => definition.Kind switch
	{
		AuthoredCelestialKind.RailSun => new RailSun(id, definition, clock, world),
		AuthoredCelestialKind.RailMoon => new RailMoon(id, definition, clock, world),
		AuthoredCelestialKind.ScriptedSun => new ScriptedSun(id, definition, clock, world),
		AuthoredCelestialKind.ScriptedMoon => new ScriptedMoon(id, definition, clock, world),
		_ => throw new ArgumentException("Unknown authored celestial type.")
	};
}

public sealed class RailSun(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud world) : AuthoredCelestial(id, definition, clock, world);
public sealed class ScriptedSun(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud world) : AuthoredCelestial(id, definition, clock, world);
public sealed class RailMoon(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud world) : AuthoredCelestial(id, definition, clock, world), ILunarPhase
{
	public MoonPhase CurrentPhase() => LiveState.Phase!.Value.Name;
}
public sealed class ScriptedMoon(long id, AuthoredCelestialDefinition definition, IClock clock, IFuturemud world) : AuthoredCelestial(id, definition, clock, world), ILunarPhase
{
	public MoonPhase CurrentPhase() => LiveState.Phase!.Value.Name;
}
