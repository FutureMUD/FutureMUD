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
		var definition = XElement.Parse(model.Definition);
		foreach (var element in definition.Element("Seasons").Elements())
		{
			var season = gameworld.Seasons.Get(long.Parse(element.Attribute("id").Value));
			_maximumAdditionalChangeChanceFromStableWeather.Add(season,
				double.Parse(element.Element("MaximumAdditionalChangeChanceFromStableWeather").Value));
			_incrementalAdditionalChangeChanceFromStableWeather.Add(season,
				double.Parse(element.Element("IncrementalAdditionalChangeChanceFromStableWeather").Value));
			foreach (var subelement in element.Element("Events").Elements())
			{
				var weather = gameworld.WeatherEvents.Get(long.Parse(subelement.Attribute("id").Value));
				_weatherEventChangeChance[(season, weather)] = double.Parse(subelement.Attribute("changechance").Value);
				var transitions = new List<(IWeatherEvent, double)>();
				foreach (var transition in subelement.Element("Transitions").Elements())
				{
					var other = gameworld.WeatherEvents.Get(long.Parse(transition.Attribute("id").Value));
					transitions.Add((other, double.Parse(transition.Attribute("chance").Value)));
				}

				_newWeatherEventChances[(season, weather)] = transitions;
			}
		}
	}
}