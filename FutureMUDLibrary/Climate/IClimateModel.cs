using MudSharp.Celestial;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework.Revision;

namespace MudSharp.Climate
{
	public interface IClimateModel : IEditableItem {
		IWeatherEvent HandleWeatherTick(IWeatherEvent currentWeather, ISeason currentSeason, TimeOfDay currentTime, int consecutiveUnchangedPeriods);
		/// <summary>
		/// The number of in-character minutes between checking for weather changes
		/// </summary>
		int MinuteProcessingInterval { get; }

		/// <summary>
		/// The minimum number of in-character minutes between flavour echoes for current weather being sent
		/// </summary>
		int MinimumMinutesBetweenFlavourEchoes { get; }

		/// <summary>
		/// The chance (0.0-1.0) of a flavour echo being issued every in-character minute
		/// </summary>
		double MinuteFlavourEchoChance { get; }

		IEnumerable<IWeatherEvent> PermittedTransitions(IWeatherEvent currentEvent, ISeason currentSeason, TimeOfDay timeOfDay);
		IClimateModel Clone(string name);
		IEnumerable<ISeason> Seasons { get; }
		IEnumerable<IWeatherEvent> WeatherEvents { get; }

	}
}
