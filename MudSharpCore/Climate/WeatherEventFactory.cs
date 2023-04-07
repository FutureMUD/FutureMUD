using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Framework;

namespace MudSharp.Climate;

public static class WeatherEventFactory
{
	public static IWeatherEvent LoadWeatherEvent(Models.WeatherEvent weather, IFuturemud gameworld)
	{
		switch (weather.WeatherEventType.ToLowerInvariant())
		{
			case "simple":
				return new SimpleWeatherEvent(weather, gameworld);
			case "rain":
				return new RainWeatherEvent(weather, gameworld);
		}

		throw new ApplicationException(
			$"Unknown weather event type '{weather.WeatherEventType}' in WeatherEventFactory.LoadWeatherEvent for event {weather.Id}");
	}
}