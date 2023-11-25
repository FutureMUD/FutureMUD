using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Framework;

namespace MudSharp.Climate.ClimateModels;

public class TerrestrialClimateModel : ClimateModelBase
{
	private readonly Dictionary<(ISeason Season, IWeatherEvent Event), double> _weatherEventChangeChance = new();

	private readonly
		Dictionary<(ISeason Season, IWeatherEvent OldEvent), IEnumerable<(IWeatherEvent NewEvent, double Chance)>>
		_newWeatherEventChances = new();

	private readonly Dictionary<ISeason, double> _maximumAdditionalChangeChanceFromStableWeather = new();
	private readonly Dictionary<ISeason, double> _incrementalAdditionalChangeChanceFromStableWeather = new();

	#region Overrides of ClimateModelBase

	public override IWeatherEvent HandleWeatherTick(IWeatherEvent currentWeather, ISeason currentSeason,
		TimeOfDay currentTime, int consecutiveUnchangedPeriods)
	{
		if (!currentWeather.PermittedTimesOfDay.Contains(currentTime) || RandomUtilities.Roll(1.0,
			    _weatherEventChangeChance[(currentSeason, currentWeather)] + Math.Min(
				    _maximumAdditionalChangeChanceFromStableWeather[currentSeason],
				    _incrementalAdditionalChangeChanceFromStableWeather[currentSeason] * consecutiveUnchangedPeriods)))
		{
			var permittedChanges = _newWeatherEventChances[(currentSeason, currentWeather)]
			                       .Where(x => x.NewEvent.PermittedTimesOfDay.Contains(currentTime)).ToList();
			if (!permittedChanges.Any())
			{
				return currentWeather;
			}

			return permittedChanges.GetWeightedRandom(x => x.Chance)
			                       .NewEvent;
		}

		return currentWeather;
	}

	public override IEnumerable<IWeatherEvent> PermittedTransitions(IWeatherEvent currentEvent, ISeason currentSeason,
		TimeOfDay timeOfDay)
	{
		return _newWeatherEventChances[(currentSeason, currentEvent)]
		       .Where(x => x.NewEvent.PermittedTimesOfDay.Contains(timeOfDay))
		       .Select(x => x.NewEvent)
		       .ToList();
	}

	#endregion

	public TerrestrialClimateModel(Models.ClimateModel model, IFuturemud gameworld)
	{
		_id = model.Id;
		_name = model.Name;
		MinuteProcessingInterval = model.MinuteProcessingInterval;
		MinimumMinutesBetweenFlavourEchoes = model.MinimumMinutesBetweenFlavourEchoes;
		MinuteFlavourEchoChance = model.MinuteFlavourEchoChance;
		foreach (var dbseason in model.ClimateModelSeasons)
		{
			var season = gameworld.Seasons.Get(dbseason.SeasonId);
			_maximumAdditionalChangeChanceFromStableWeather[season] = dbseason.MaximumAdditionalChangeChanceFromStableWeather;
			_incrementalAdditionalChangeChanceFromStableWeather[season] = dbseason.IncrementalAdditionalChangeChanceFromStableWeather;
			foreach (var dbevent in dbseason.SeasonEvents)
			{
				var we = gameworld.WeatherEvents.Get(dbevent.WeatherEventId);
				_weatherEventChangeChance[(season, we)] = dbevent.ChangeChance;
				var transitions = new List<(IWeatherEvent, double)>();
				foreach (var transition in XElement.Parse(dbevent.Transitions).Elements())
				{
					var other = gameworld.WeatherEvents.Get(long.Parse(transition.Attribute("id").Value));
					transitions.Add((other, double.Parse(transition.Attribute("chance").Value)));
				}

				_newWeatherEventChances[(season, we)] = transitions;
			}
		}
	}
}