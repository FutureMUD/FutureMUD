#nullable enable

using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.FutureProg.Functions.Celestials;

internal sealed class AstronomicalEventFunction : BuiltInFunction
{
	private readonly AstronomicalEventType? _eventType;
	private readonly IFuturemud _gameworld;

	public AstronomicalEventFunction(IList<IFunction> parameters, AstronomicalEventType? eventType, IFuturemud gameworld)
		: base(parameters)
	{
		_eventType = eventType;
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.MudDateTime;

	public override string ErrorMessage => ParameterFunctions.First().ErrorMessage;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var zone = ResolveZone(ParameterFunctions[0].Result?.GetObject);
		if (zone is null)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		var calendarIndex = _eventType == AstronomicalEventType.VisibleCrescent ? 3 : 2;
		if (ParameterFunctions[calendarIndex].Result?.GetObject is not ICalendar calendar || calendar.FeedClock is null)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		var primary = ResolveCelestial(ParameterFunctions[1].Result?.GetObject, zone);
		if (primary is null)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		var occurrenceIndex = 3;
		var targetLongitude = 0.0;
		ICelestialObject? secondary = null;
		string? eventKey = null;
		if (_eventType == AstronomicalEventType.SolarLongitude)
		{
			if (!TryFiniteNumber(ParameterFunctions[3].Result?.GetObject, out var degrees))
			{
				Result = MudDateTime.Never;
				return StatementResult.Normal;
			}
			targetLongitude = (degrees % 360).DegreesToRadians();
			occurrenceIndex = 4;
		}
		else if (_eventType == AstronomicalEventType.VisibleCrescent)
		{
			secondary = ResolveCelestial(ParameterFunctions[2].Result?.GetObject, zone);
			occurrenceIndex = 4;
			if (secondary is null)
			{
				Result = MudDateTime.Never;
				return StatementResult.Normal;
			}
		}

		if (_eventType is null)
		{
			eventKey = ParameterFunctions[3].Result?.GetObject as string;
			occurrenceIndex = 4;
			if (string.IsNullOrWhiteSpace(eventKey)) { Result = MudDateTime.Never; return StatementResult.Normal; }
		}
		var occurrence = 1L;
		if (ParameterFunctions.Count > occurrenceIndex && !TryPositiveInteger(ParameterFunctions[occurrenceIndex].Result?.GetObject, int.MaxValue, out occurrence))
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		try
		{
			var result = AstronomicalEventService.Instance.FindNextForCelestial(calendar.CurrentInstant,
				new(_eventType, occurrence, targetLongitude, eventKey), primary, zone.Geography, secondary);
			Result = result.Found ? result.Instant.ToMudDateTime(calendar, calendar.FeedClock, zone.TimeZone(calendar.FeedClock)) : MudDateTime.Never;
		}
		catch (Exception ex) when (ex is OverflowException or ArgumentException or InvalidOperationException)
		{
			Result = MudDateTime.Never;
		}
		return StatementResult.Normal;
	}

	private ICelestialObject? ResolveCelestial(object? value, IZone zone) => value is ICelestialObject celestial ? celestial :
		TryPositiveInteger(value, long.MaxValue, out var id) ? zone.Celestials.FirstOrDefault(x => x.Id == id) ?? _gameworld.CelestialObjects.Get(id) : null;

	internal static bool TryPositiveInteger(object? value, long maximum, out long result)
	{
		result = 0;
		if (value is null) return false;
		try
		{
			var number = Convert.ToDecimal(value);
			if (number < 1 || number > maximum || decimal.Truncate(number) != number) return false;
			result = (long)number;
			return true;
		}
		catch (Exception ex) when (ex is OverflowException or FormatException or InvalidCastException) { return false; }
	}

	private static bool TryFiniteNumber(object? value, out double number)
	{
		number = 0;
		if (value is null) return false;
		try { number = Convert.ToDouble(value); return double.IsFinite(number); }
		catch (Exception ex) when (ex is OverflowException or FormatException or InvalidCastException) { return false; }
	}

	private static IZone? ResolveZone(object? value)
	{
		return value switch
		{
			IZone zone => zone,
			ICell cell => cell.Zone,
			_ => null
		};
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterSolar("nextsunrise", AstronomicalEventType.Sunrise,
			"Returns the next sunrise for a solar celestial at the supplied room or zone. The optional occurrence parameter returns the nth next sunrise. Returns Never if no event is found.");
		RegisterSolar("nextsunset", AstronomicalEventType.Sunset,
			"Returns the next sunset for a solar celestial at the supplied room or zone. The optional occurrence parameter returns the nth next sunset. Returns Never if no event is found.");
		RegisterSolarLongitude();
		RegisterLunar("nextnewmoon", AstronomicalEventType.NewMoon,
			"Returns the next deterministic lunar conjunction/new moon for a lunar celestial. The optional occurrence parameter returns the nth next occurrence. Returns Never if no event is found.");
		RegisterLunar("nextfullmoon", AstronomicalEventType.FullMoon,
			"Returns the next deterministic full moon for a lunar celestial. The optional occurrence parameter returns the nth next occurrence. Returns Never if no event is found.");
		RegisterVisibleCrescent();
		foreach (var objectType in new[] { ProgVariableTypes.Number, ProgVariableTypes.CelestialObject })
		{
			RegisterLocationAndZone("nextcelestialevent", null, [objectType, ProgVariableTypes.Calendar, ProgVariableTypes.Text],
				["celestial", "calendar", "eventKey"], ["Celestial object or ID.", "Display calendar.", "Explicit custom: milestone key."],
				"Returns the next authored milestone without delivering its echo, or Never when unavailable.");
			RegisterLocationAndZone("nextcelestialevent", null, [objectType, ProgVariableTypes.Calendar, ProgVariableTypes.Text, ProgVariableTypes.Number],
				["celestial", "calendar", "eventKey", "occurrence"], ["Celestial object or ID.", "Display calendar.", "Explicit custom: milestone key.", "Positive integral occurrence."],
				"Returns the nth strictly-next authored milestone without delivering its echo, or Never when unavailable.");
		}
	}

	private static void RegisterSolar(string name, AstronomicalEventType type, string description)
	{
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar],
			["celestialId", "calendar"],
			["The ID of the solar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestialId", "calendar", "occurrence"],
			["The ID of the solar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar],
			["celestial", "calendar"],
			["The resolved solar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestial", "calendar", "occurrence"],
			["The resolved solar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
	}

	private static void RegisterLunar(string name, AstronomicalEventType type, string description)
	{
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar],
			["celestialId", "calendar"],
			["The ID of the lunar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestialId", "calendar", "occurrence"],
			["The ID of the lunar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar],
			["celestial", "calendar"],
			["The resolved lunar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone(name, type,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestial", "calendar", "occurrence"],
			["The resolved lunar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
	}

	private static void RegisterSolarLongitude()
	{
		const string description = "Returns the next time a solar celestial crosses the supplied ecliptic longitude in degrees. The optional occurrence parameter returns the nth next occurrence. Returns Never if no event is found.";
		RegisterLocationAndZone("nextsolarlongitude", AstronomicalEventType.SolarLongitude,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestialId", "calendar", "longitudeDegrees"],
			["The ID of the solar celestial.", "The calendar used to display the returned mud datetime.", "The target solar longitude in degrees."],
			description);
		RegisterLocationAndZone("nextsolarlongitude", AstronomicalEventType.SolarLongitude,
			[ProgVariableTypes.Number, ProgVariableTypes.Calendar, ProgVariableTypes.Number, ProgVariableTypes.Number],
			["celestialId", "calendar", "longitudeDegrees", "occurrence"],
			["The ID of the solar celestial.", "The calendar used to display the returned mud datetime.", "The target solar longitude in degrees.", "The nth next occurrence to return."],
			description);
		RegisterLocationAndZone("nextsolarlongitude", AstronomicalEventType.SolarLongitude,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["celestial", "calendar", "longitudeDegrees"],
			["The resolved solar celestial.", "The calendar used to display the returned mud datetime.", "The target solar longitude in degrees."],
			description);
		RegisterLocationAndZone("nextsolarlongitude", AstronomicalEventType.SolarLongitude,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar, ProgVariableTypes.Number, ProgVariableTypes.Number],
			["celestial", "calendar", "longitudeDegrees", "occurrence"],
			["The resolved solar celestial.", "The calendar used to display the returned mud datetime.", "The target solar longitude in degrees.", "The nth next occurrence to return."],
			description);
	}

	private static void RegisterVisibleCrescent()
	{
		const string description = "Returns the next physical visible-crescent approximation, or an authored moon's explicit crescent marker associated with the supplied sun. Authored markers add no geometric or weather tests. The optional positive integral occurrence selects the nth event. Returns Never when unavailable or incompatible.";
		RegisterLocationAndZone("nextvisiblecrescent", AstronomicalEventType.VisibleCrescent,
			[ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Calendar],
			["sunId", "moonId", "calendar"],
			["The ID of the solar celestial.", "The ID of the lunar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone("nextvisiblecrescent", AstronomicalEventType.VisibleCrescent,
			[ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["sunId", "moonId", "calendar", "occurrence"],
			["The ID of the solar celestial.", "The ID of the lunar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
		RegisterLocationAndZone("nextvisiblecrescent", AstronomicalEventType.VisibleCrescent,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar],
			["sun", "moon", "calendar"],
			["The resolved solar celestial.", "The resolved lunar celestial.", "The calendar used to display the returned mud datetime."],
			description);
		RegisterLocationAndZone("nextvisiblecrescent", AstronomicalEventType.VisibleCrescent,
			[ProgVariableTypes.CelestialObject, ProgVariableTypes.CelestialObject, ProgVariableTypes.Calendar, ProgVariableTypes.Number],
			["sun", "moon", "calendar", "occurrence"],
			["The resolved solar celestial.", "The resolved lunar celestial.", "The calendar used to display the returned mud datetime.", "The nth next occurrence to return."],
			description);
	}

	private static void RegisterLocationAndZone(string name, AstronomicalEventType? type, ProgVariableTypes[] trailingTypes,
		List<string> trailingNames, List<string> trailingDescriptions, string description)
	{
		RegisterForFirstParameter(name, type, ProgVariableTypes.Location, trailingTypes, trailingNames, trailingDescriptions, description);
		RegisterForFirstParameter(name, type, ProgVariableTypes.Zone, trailingTypes, trailingNames, trailingDescriptions, description);
	}

	private static void RegisterForFirstParameter(string name, AstronomicalEventType? type, ProgVariableTypes firstType,
		ProgVariableTypes[] trailingTypes, List<string> trailingNames, List<string> trailingDescriptions, string description)
	{
		var parameterTypes = new[] { firstType }.Concat(trailingTypes).ToArray();
		var parameterNames = new[] { "locationOrZone" }.Concat(trailingNames).ToList();
		var parameterDescriptions = new[] { "The room or zone whose geography is used as the observer location." }
			.Concat(trailingDescriptions)
			.ToList();

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameterTypes,
			(pars, gameworld) => new AstronomicalEventFunction(pars, type, gameworld),
			parameterNames,
			parameterDescriptions,
			description,
			"Celestials",
			ProgVariableTypes.MudDateTime
		));
	}
}
