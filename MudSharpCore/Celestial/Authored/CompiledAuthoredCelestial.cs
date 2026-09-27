#nullable enable

using MudSharp.TimeAndDate;

namespace MudSharp.Celestial.Authored;

public struct AuthoredCelestialCursor
{
	internal int Path;
	internal int Direction;
	internal int Light;
	internal int Phase;
}

public sealed class CompiledAuthoredCelestial
{
	private readonly AuthoredCelestialDefinition _source;
	private readonly AuthoredCelestialLimits _limits;
	private readonly CompiledScalarTrack? _light;
	private readonly CompiledScalarTrack? _phase;
	private readonly LightKnot[] _profile;
	private readonly Dictionary<AstronomicalEventType, AuthoredEventIndex> _canonical = new();
	private readonly Dictionary<string, AuthoredEventIndex> _named = new(StringComparer.OrdinalIgnoreCase);
	private readonly (AuthoredEcho Echo, AuthoredEventIndex Index)[] _echoes;
	public CompiledSkyPath Path { get; }
	public int SecondsPerMinute { get; }
	public long MinutesPerDay { get; }
	public long AnchorTicks => _source.AnchorTicks;
	public long AnnualPeriod => _source.AnnualPeriod;
	public bool IsMoon => _source.Kind is AuthoredCelestialKind.RailMoon or AuthoredCelestialKind.ScriptedMoon;
	public bool Eligible => _source.TimeOfDay;
	public string Name => _source.Name;
	public string Description => _source.Description;
	public AuthoredCelestialKind Kind => _source.Kind;
	public long CalendarId => _source.CalendarId;
	public long? CrescentSunId => _source.Phase?.CrescentSunId;
	public IEnumerable<string> NamedEventKeys => _named.Keys;
	public int StoredEntries => Path.StoredEntries + (_light?.StoredEntries ?? 0) + (_phase?.StoredEntries ?? 0) + _profile.Length + _echoes.Length + _named.Count;
	public AuthoredCelestialDefinition CopyDefinition() => AuthoredCelestialFormat.Parse(AuthoredCelestialFormat.Serialize(_source), _limits);
	public string Serialize() => AuthoredCelestialFormat.Serialize(_source);
	public IReadOnlyList<long> EventOffsets(AstronomicalEventType type) => _canonical.TryGetValue(type, out var index) ? index.Offsets : Array.Empty<long>();

	public CompiledAuthoredCelestial(AuthoredCelestialDefinition source, int secondsPerMinute, int minutesPerHour, int hoursPerDay, AuthoredCelestialLimits? limits = null)
	{
		limits ??= new();
		_limits = limits;
		_source = AuthoredCelestialFormat.Parse(AuthoredCelestialFormat.Serialize(source), limits);
		source = _source;
		AuthoredMath.Require(source.Version == 1, $"Unsupported authored celestial definition version {source.Version}.");
		AuthoredMath.Require(Enum.IsDefined(source.Kind) && Enum.IsDefined(source.LightMode), "Unknown celestial type or light mode.");
		AuthoredMath.Require(source.CalendarId > 0, "A supported calendar is required.");
		AuthoredMath.Require(secondsPerMinute > 0 && minutesPerHour > 0 && hoursPerDay > 0, "Clock dimensions must be positive.");
		SecondsPerMinute = secondsPerMinute;
		MinutesPerDay = checked((long)minutesPerHour * hoursPerDay);
		_ = checked(MinutesPerDay * SecondsPerMinute);
		AuthoredMath.Require(source.AnchorTicks % SecondsPerMinute == 0, "Anchor must be a feed-clock minute boundary.");
		AuthoredMath.Require(!string.IsNullOrWhiteSpace(source.Name) && !string.IsNullOrWhiteSpace(source.Description), "Name and description are required.");
		AuthoredMath.Require(source.Path is not null && source.LightProfile is not null && source.Milestones is not null && source.Echoes is not null, "Missing authored definition channels.");
		var entries = checked((long)source.Path!.Keys.Count + source.LightProfile!.Count + (source.LightTrack?.Keys.Count ?? 0) +
			(source.Phase?.Track.Keys.Count ?? 0) + source.Milestones!.Count + source.Echoes!.Count);
		AuthoredMath.Require(entries <= limits.Entries, $"Authored entries exceed the configured {limits.Entries:N0} limit.");
		void Period(long period)
		{
			AuthoredMath.Require(period > 0, "Channel periods must be positive.");
			_ = checked(period * secondsPerMinute);
		}
		Period(source.Path.Period);
		Period(source.AnnualPeriod);
		var rail = source.Kind is AuthoredCelestialKind.RailSun or AuthoredCelestialKind.RailMoon;
		AuthoredMath.Require(rail == (source.Path.Mode is AuthoredPathMode.SimpleRail or AuthoredPathMode.ThreePointRail), "Persisted type and path mode do not agree.");
		AuthoredMath.Require(IsMoon == (source.Phase is not null), "Moons require a phase channel; suns cannot have one.");
		AuthoredMath.Require(!source.SyntheticLongitude || !IsMoon, "Synthetic solar longitude is only available to suns.");
		AuthoredMath.Finite(source.LongitudeAtEpoch, "Annual longitude");
		AuthoredMath.Require(IsMoon || source.LightMode == AuthoredLightMode.Absolute, "Phase-scaled light requires a moon.");
		Path = new(source.Path, limits.Entries);
		_canonical[AstronomicalEventType.Sunrise] = new(Path.Period, Path.RiseOffsets, secondsPerMinute);
		_canonical[AstronomicalEventType.Sunset] = new(Path.Period, Path.SetOffsets, secondsPerMinute);
		AuthoredMath.Require((source.LightTrack is not null) != (source.LightProfile.Count > 0), "Specify exactly one light profile or independent light timeline.");
		if (source.LightTrack is { } light)
		{
			Period(light.Period);
			_light = new(light, false, limits.Entries);
		}
		_profile = source.LightProfile.OrderBy(x => x.Elevation).ToArray();
		for (var i = 0; i < _profile.Length; i++)
		{
			var knot = _profile[i];
			AuthoredMath.Finite(knot.Elevation, "Light elevation");
			AuthoredMath.Finite(knot.Lux, "Source lux");
			AuthoredMath.Require(knot.Elevation >= -90 && knot.Elevation <= 90 && knot.Lux >= 0, "Invalid elevation/lux profile knot.");
			AuthoredMath.Require(i == 0 || knot.Elevation > _profile[i - 1].Elevation, "Light profile elevations must be unique.");
		}
		if (source.Phase is { } phase)
		{
			AuthoredMath.Require(Enum.IsDefined(phase.Mode), "Unknown phase mode.");
			AuthoredMath.Finite(phase.FixedTurns, "Fixed phase");
			long[] full = [], @new = [];
			var phasePeriod = 1L;
			if (phase.Mode != AuthoredPhaseMode.Fixed)
			{
				Period(phase.Track.Period);
				phasePeriod = phase.Track.Period;
				if (phase.Mode == AuthoredPhaseMode.Regular)
				{
					AuthoredMath.Require(phasePeriod >= 2, "Regular phase cycles require at least two sampled minutes.");
					phase.FullMoonMinute = AuthoredMath.Mod(phase.FullMoonMinute, phasePeriod);
					full = [phase.FullMoonMinute];
					@new = [(long)(((Int128)phase.FullMoonMinute + phasePeriod / 2 + phasePeriod % 2) % phasePeriod)];
				}
				else
				{
					_phase = new(phase.Track, true, limits.Entries);
					full = _phase.PhaseMilestones(0);
					@new = _phase.PhaseMilestones(.5);
				}
			}
			_canonical[AstronomicalEventType.FullMoon] = new(phasePeriod, full, secondsPerMinute);
			_canonical[AstronomicalEventType.NewMoon] = new(phasePeriod, @new, secondsPerMinute);
		}
		foreach (var milestone in source.Milestones)
		{
			Period(milestone.Period);
			AuthoredMath.Require(!string.IsNullOrWhiteSpace(milestone.Key) && milestone.Key.StartsWith("custom:", StringComparison.OrdinalIgnoreCase), "Named milestones use the reserved custom: namespace, e.g. custom:defeated.");
			AuthoredMath.Require(_named.TryAdd(milestone.Key, new(milestone.Period, [milestone.Minute], secondsPerMinute)), "Milestone keys must be unique.");
		}
		if (source.Phase is { } crescent)
		{
			AuthoredMath.Require(crescent.CrescentMilestones.Count == 0 ? crescent.CrescentSunId is null : crescent.CrescentSunId > 0, "Authored crescent markers require a positive associated sun ID and named milestones.");
			AuthoredMath.Require(crescent.CrescentMilestones.Distinct(StringComparer.OrdinalIgnoreCase).Count() == crescent.CrescentMilestones.Count && crescent.CrescentMilestones.All(_named.ContainsKey), "Crescent milestone references must exist and be unique.");
			if (crescent.CrescentMilestones.Count > 0)
			{
				var markers = source.Milestones.Where(x => crescent.CrescentMilestones.Contains(x.Key, StringComparer.OrdinalIgnoreCase)).ToArray();
				AuthoredMath.Require(markers.All(x => x.Period == markers[0].Period), "Crescent markers must share their own event-track period.");
				_canonical[AstronomicalEventType.VisibleCrescent] = new(markers[0].Period, markers.Select(x => x.Minute), secondsPerMinute);
			}
		}
		var echoIds = new HashSet<string>(StringComparer.Ordinal);
		var echoes = new List<(AuthoredEcho, AuthoredEventIndex)>();
		foreach (var echo in source.Echoes)
		{
			AuthoredMath.Require(!string.IsNullOrWhiteSpace(echo.Id) && echoIds.Add(echo.Id), "Echo IDs must be nonempty and unique.");
			AuthoredMath.Require(!string.IsNullOrWhiteSpace(echo.Text) && Enum.IsDefined(echo.Audience) && Enum.IsDefined(echo.Direction), "Invalid echo text, audience or direction.");
			AuthoredMath.Require((echo.Milestone is not null ? 1 : 0) + (echo.Threshold.HasValue ? 1 : 0) + (echo.Period != 0 ? 1 : 0) == 1, "Echo requires exactly one milestone, threshold or explicit event-track period.");
			AuthoredEventIndex index;
			if (echo.Milestone is { } key)
			{
				AuthoredMath.Require(_named.ContainsKey(key), "Echo references an unknown milestone.");
				index = _named[key];
			}
			else if (echo.Threshold is { } threshold)
			{
				AuthoredMath.Finite(threshold, "Echo threshold");
				AuthoredMath.Require(rail && threshold >= -90 && threshold <= 90, "Threshold echoes require a rail and an elevation in -90..90.");
				var crossings = Path.Crossings(threshold);
				index = new(Path.Period, echo.Direction == CelestialMoveDirection.Ascending ? crossings.Rises : crossings.Sets, secondsPerMinute);
			}
			else index = new(echo.Period, [echo.Minute], secondsPerMinute);
			echoes.Add((echo, index));
		}
		_echoes = echoes.OrderBy(x => x.Item1.Order).ThenBy(x => x.Item1.Id, StringComparer.Ordinal).ToArray();
	}

	public CelestialCapabilities Capabilities => CelestialCapabilities.GeographyIndependent | CelestialCapabilities.HorizonEvents |
		(IsMoon ? CelestialCapabilities.Lunar | CelestialCapabilities.PhaseEvents : CelestialCapabilities.Solar) |
		(_source.SyntheticLongitude ? CelestialCapabilities.SyntheticLongitude : 0) |
		(_canonical.ContainsKey(AstronomicalEventType.VisibleCrescent) ? CelestialCapabilities.AuthoredCrescent : 0) |
		(_named.Count > 0 ? CelestialCapabilities.NamedEvents : 0);

	public CelestialState Evaluate(long ticks) { var cursor = new AuthoredCelestialCursor(); return Evaluate(ticks, ref cursor); }
	public CelestialState Evaluate(long ticks, ref int pathCursor)
	{
		var cursor = new AuthoredCelestialCursor { Path = pathCursor };
		var result = Evaluate(ticks, ref cursor);
		pathCursor = cursor.Path;
		return result;
	}
	public CelestialState Evaluate(long ticks, ref AuthoredCelestialCursor cursor)
	{
		var minute = AuthoredMath.FloorDiv(checked(ticks - AnchorTicks), SecondsPerMinute);
		var pathMinute = AuthoredMath.Mod(minute, Path.Period);
		var position = Path.At(pathMinute, ref cursor.Path);
		var previousCursor = cursor.Path;
		var previous = Path.At(AuthoredMath.Mod(pathMinute - 1, Path.Period), ref previousCursor);
		var (vertical, direction) = Path.MotionAt(pathMinute, ref cursor.Direction);
		var motion = vertical > 0 ? CelestialMotion.Rising : vertical < 0 ? CelestialMotion.Falling :
			(position - previous).Length > 1e-12 ? CelestialMotion.Level : CelestialMotion.Stationary;
		CelestialPhase? phase = null;
		if (_source.Phase is { } definition)
		{
			var turns = definition.Mode switch
			{
				AuthoredPhaseMode.Fixed => definition.FixedTurns,
				AuthoredPhaseMode.Regular => (double)AuthoredMath.Mod(checked(minute - definition.FullMoonMinute), definition.Track.Period) / definition.Track.Period,
				_ => _phase!.At(minute, ref cursor.Phase)
			};
			phase = AuthoredMath.Phase(turns);
		}
		var lux = _light is { } light ? light.At(minute, ref cursor.Light) : ProfileLux(position.Elevation / AuthoredMath.Radians);
		if (_source.LightMode == AuthoredLightMode.PhaseScaled) lux *= phase!.Value.IlluminationFraction;
		var annual = AuthoredMath.Mod(minute, AnnualPeriod);
		return new(position.Public, position.Azimuth, position.Elevation, lux, motion, direction, phase,
			pathMinute, annual, _source.SyntheticLongitude ? AuthoredMath.Wrap(_source.LongitudeAtEpoch * AuthoredMath.Radians + AuthoredMath.Tau * ((double)annual / AnnualPeriod)) : null);
	}

	private double ProfileLux(double elevation)
	{
		if (elevation <= _profile[0].Elevation) return _profile[0].Lux;
		if (elevation >= _profile[^1].Elevation) return _profile[^1].Lux;
		var low = 1;
		var high = _profile.Length - 1;
		while (low < high)
		{
			var mid = low + (high - low) / 2;
			if (_profile[mid].Elevation < elevation) low = mid + 1; else high = mid;
		}
		var a = _profile[low - 1];
		var b = _profile[low];
		return a.Lux + (b.Lux - a.Lux) * ((elevation - a.Elevation) / (b.Elevation - a.Elevation));
	}

	public AuthoredEcho[] DueEchoes(long ticks)
	{
		var relative = checked(ticks - AnchorTicks);
		if (relative % SecondsPerMinute != 0) return [];
		return _echoes.Where(x => x.Index.IsDue(relative)).Select(x => x.Echo).ToArray();
	}

	public CelestialEventResult FindNext(MudInstant reference, CelestialEventRequest request)
	{
		if (reference.IsNever || request.Occurrence < 1 || !double.IsFinite(request.TargetLongitude) ||
			(request.Type.HasValue && !Enum.IsDefined(request.Type.Value)) ||
			(request.Type.HasValue == (request.EventKey is not null)))
			return CelestialEventResult.Failure(CelestialEventStatus.InvalidRequest, "A finite reference/longitude and positive integral occurrence are required.");
		try
		{
			AuthoredEventIndex? index;
			if (request.EventKey is { } key)
			{
				if (!_named.TryGetValue(key, out index)) return CelestialEventResult.Failure(CelestialEventStatus.Unsupported, "No such authored milestone.");
			}
			else if (request.Type == AstronomicalEventType.SolarLongitude && _source.SyntheticLongitude)
			{
				var progress = AuthoredMath.Wrap(request.TargetLongitude - _source.LongitudeAtEpoch * AuthoredMath.Radians) / AuthoredMath.Tau;
				var offset = QuantizedLongitudeMinute(progress);
				// One-offset recurrence computed directly: no temporary index allocation in numerical event queries.
				var period = checked(AnnualPeriod * SecondsPerMinute);
				var relative = checked(reference.Ticks - AnchorTicks);
				var cycle = AuthoredMath.FloorDiv(relative, period);
				var within = AuthoredMath.Mod(relative, period);
				var offsetSeconds = checked(offset * SecondsPerMinute);
				var rank = checked(request.Occurrence - (within < offsetSeconds ? 1 : 0));
				var ticks = checked((long)((Int128)AnchorTicks + ((Int128)cycle + rank) * period + offsetSeconds));
				return new(CelestialEventStatus.Found, reference.WithTicks(ticks), string.Empty);
			}
			else if (request.Type is not { } type || !_canonical.TryGetValue(type, out index))
			{
				return CelestialEventResult.Failure(CelestialEventStatus.Unsupported, "This object does not support that event capability.");
			}
			if (request.Type == AstronomicalEventType.VisibleCrescent && request.AssociatedSunId != CrescentSunId)
				return CelestialEventResult.Failure(CelestialEventStatus.Unsupported, "The authored crescent is associated with a different sun.");
			return index.TryNext(reference.Ticks, AnchorTicks, request.Occurrence, out var answer)
				? new(CelestialEventStatus.Found, reference.WithTicks(answer), string.Empty)
				: CelestialEventResult.Failure(CelestialEventStatus.NoFutureOccurrence, "The repeating sampled track has no occurrence of this event.");
		}
		catch (OverflowException)
		{
			return CelestialEventResult.Failure(CelestialEventStatus.OutOfRange, "The requested event is outside representable time.");
		}
	}

	private long QuantizedLongitudeMinute(double progress)
	{
		var low = 0L;
		var high = AnnualPeriod;
		while (low < high)
		{
			var mid = low + (high - low) / 2;
			if ((double)mid / AnnualPeriod >= progress) high = mid; else low = mid + 1;
		}
		return low % AnnualPeriod;
	}
}
