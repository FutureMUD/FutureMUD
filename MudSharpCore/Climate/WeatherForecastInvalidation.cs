#nullable enable

namespace MudSharp.Climate;

internal static class WeatherForecastInvalidation
{
	internal static void CalendarChanged(IFuturemud world, MudSharp.TimeAndDate.Date.ICalendar calendar)
	{
		if (world?.WeatherControllers is null) return;
		foreach (var controller in world.WeatherControllers.OfType<WeatherController>().Where(x =>
			ReferenceEquals(x.FeedClock, calendar.FeedClock) || UsesCelestial(x.Celestial, calendar)))
			controller.ResynchroniseForecast();
	}

	internal static void Invalidate(IFuturemud world, object definition)
	{
		if (world?.WeatherControllers is null) return;
		if (world.RegionalClimates is not null)
			foreach (var climate in world.RegionalClimates.OfType<RegionalClimate>().Where(x =>
				x.Seasons.Any(s => ReferenceEquals(s, definition) || UsesCelestial(s.Celestial, definition))))
				climate.RebuildSeasonRotation();
		if (definition is IWeatherEvent && world.ClimateModels is not null)
			foreach (var model in world.ClimateModels.OfType<ClimateModels.TerrestrialClimateModel>()) model.RecalculateCaches();
		foreach (var controller in world.WeatherControllers)
			if (ReferenceEquals(controller.RegionalClimate, definition) || ReferenceEquals(controller.RegionalClimate.ClimateModel, definition) ||
				UsesCelestial(controller.Celestial, definition) || definition is IWeatherEvent ||
				controller.RegionalClimate.Seasons.Any(s => ReferenceEquals(s, definition) || UsesCelestial(s.Celestial, definition)))
				(controller as IWeatherForecastSource)?.InvalidateForecast();
	}

	private static bool UsesCelestial(MudSharp.Celestial.ICelestialObject? celestial, object definition) =>
		ReferenceEquals(celestial, definition) || celestial switch
		{
			MudSharp.Celestial.IAuthoredCelestial authored => ReferenceEquals(authored.Calendar, definition),
			MudSharp.Celestial.SunFromPlanetaryMoon sun => ReferenceEquals(sun.Sun, definition) || ReferenceEquals(sun.Moon, definition),
			MudSharp.Celestial.PlanetFromMoon planet => ReferenceEquals(planet.Moon, definition) || UsesCelestial(planet.Sun, definition),
			_ => false
		};
}
