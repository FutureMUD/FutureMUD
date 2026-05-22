#nullable enable

using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Celestials;

internal sealed class AstronomicalEventFunction : BuiltInFunction
{
	private readonly AstronomicalEventType _eventType;
	private readonly IFuturemud _gameworld;

	public AstronomicalEventFunction(IList<IFunction> parameters, AstronomicalEventType eventType, IFuturemud gameworld)
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

		var primaryId = Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0L);
		var primary = zone.Celestials.FirstOrDefault(x => x.Id == primaryId) as ICelestialEphemeris ??
		              _gameworld.CelestialObjects.Get(primaryId) as ICelestialEphemeris;
		if (primary is null)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		var occurrenceIndex = 3;
		var targetLongitude = 0.0;
		ICelestialEphemeris? secondary = null;
		if (_eventType == AstronomicalEventType.SolarLongitude)
		{
			targetLongitude = Convert.ToDouble(ParameterFunctions[3].Result?.GetObject ?? 0.0).DegreesToRadians();
			occurrenceIndex = 4;
		}
		else if (_eventType == AstronomicalEventType.VisibleCrescent)
		{
			var secondaryId = Convert.ToInt64(ParameterFunctions[2].Result?.GetObject ?? 0L);
			secondary = zone.Celestials.FirstOrDefault(x => x.Id == secondaryId) as ICelestialEphemeris ??
			            _gameworld.CelestialObjects.Get(secondaryId) as ICelestialEphemeris;
			occurrenceIndex = 4;
			if (secondary is null)
			{
				Result = MudDateTime.Never;
				return StatementResult.Normal;
			}
		}

		var occurrence = ParameterFunctions.Count > occurrenceIndex
			? Convert.ToInt32(ParameterFunctions[occurrenceIndex].Result?.GetObject ?? 1)
			: 1;

		if (!AstronomicalEventService.Instance.TryFindNext(_eventType, calendar.CurrentInstant, occurrence, primary,
			    zone.Geography, out var instant, out _, targetLongitude, secondary))
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		Result = instant.ToMudDateTime(calendar, calendar.FeedClock, zone.TimeZone(calendar.FeedClock));
		return StatementResult.Normal;
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
	}

	private static void RegisterVisibleCrescent()
	{
		const string description = "Returns the next deterministic visible-crescent approximation after sunset using geometric thresholds only. The optional occurrence parameter returns the nth next occurrence. Returns Never if no event is found.";
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
	}

	private static void RegisterLocationAndZone(string name, AstronomicalEventType type, ProgVariableTypes[] trailingTypes,
		List<string> trailingNames, List<string> trailingDescriptions, string description)
	{
		RegisterForFirstParameter(name, type, ProgVariableTypes.Location, trailingTypes, trailingNames, trailingDescriptions, description);
		RegisterForFirstParameter(name, type, ProgVariableTypes.Zone, trailingTypes, trailingNames, trailingDescriptions, description);
	}

	private static void RegisterForFirstParameter(string name, AstronomicalEventType type, ProgVariableTypes firstType,
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
