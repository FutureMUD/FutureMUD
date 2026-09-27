#nullable enable

using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Climate;

internal readonly record struct WeatherSky(double Day, TimeOfDay Time);

/// <summary>Samples the same astronomical frame as a controller without touching any live clock.</summary>
internal static class WeatherAstronomy
{
	internal static TimeOfDay Classify(double altitude, double previousAltitude) => altitude > 0.0
		? altitude >= previousAltitude ? TimeOfDay.Morning : TimeOfDay.Afternoon
		: altitude < -0.20943951023931953 ? TimeOfDay.Night
		: altitude >= previousAltitude ? TimeOfDay.Dawn : TimeOfDay.Dusk;

	internal static double Days(double seconds, IClock clock) => seconds * clock.InGameSecondsPerRealSecond /
		((double)clock.SecondsPerMinute * clock.MinutesPerHour * clock.HoursPerDay);

	internal static WeatherSky Sample(ICelestialObject? celestial, double realSeconds, GeographicCoordinate geography)
	{
		switch (celestial)
		{
			case null:
				return new(0.0, TimeOfDay.Night);
			case NewSun sun:
				var sunDays = Days(realSeconds, sun.Clock);
				var dayNumber = sun.CurrentDayNumber + sunDays;
				return new((sun.CurrentCelestialDay + sunDays).Modulus(sun.CelestialDaysPerYear),
					Classify(sun.Altitude(dayNumber, geography), sun.Altitude(dayNumber -
						1.0 / (sun.Clock.HoursPerDay * sun.Clock.MinutesPerHour), geography)));
			case AuthoredCelestial authored:
				var instant = authored.Calendar.CurrentInstant.AddSeconds(checked((long)Math.Floor(realSeconds * authored.Clock.InGameSecondsPerRealSecond)));
				var state = authored.EvaluateAt(instant);
				return new((double)state.AnnualMinute / authored.Compiled.MinutesPerDay,
					AuthoredMath.TimeOfDay(state, authored.CelestialAngleIsUsedToDetermineTimeOfDay));
			case PlanetaryMoon moon:
				return new((moon.CurrentCelestialDay + Days(realSeconds, moon.Clock)).Modulus(moon.CelestialDaysPerYear), TimeOfDay.Night);
			case SunFromPlanetaryMoon moonSun:
				return moonSun.PredictWeatherSky(realSeconds, geography);
			case PlanetFromMoon planet:
				return new(Sample(planet.Moon, realSeconds, geography).Day, Sample(planet.Sun, realSeconds, geography).Time);
			default:
				throw new NotSupportedException($"Weather prediction does not support {celestial.GetType().Name}.");
		}
	}
}
