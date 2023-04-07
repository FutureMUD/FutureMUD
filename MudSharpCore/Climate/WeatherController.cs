using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Database;

namespace MudSharp.Climate;

public class WeatherController : SaveableItem, IWeatherController
{
	public WeatherController(MudSharp.Models.WeatherController controller, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = controller.Id;
		_name = controller.Name;
		FeedClock = gameworld.Clocks.Get(controller.FeedClockId);
		FeedClockTimeZone = FeedClock.Timezones.Get(controller.FeedClockTimeZoneId);
		RegionalClimate = gameworld.RegionalClimates.Get(controller.RegionalClimateId);
		GeographyForTimeOfDay =
			new GeographicCoordinate(controller.Latitude, controller.Longitude, controller.Elevation,
									 controller.Radius);
		Celestial = gameworld.CelestialObjects.Get(controller.CelestialId ?? 0L);
		ConsecutiveUnchangedPeriods = controller.ConsecutiveUnchangedPeriods;
		MinuteCounter = controller.MinutesCounter;
		CurrentSeason = gameworld.Seasons.Get(controller.CurrentSeasonId);
		CurrentWeatherEvent = gameworld.WeatherEvents.Get(controller.CurrentWeatherEventId);
		HighestRecentPrecipitationLevel = (PrecipitationLevel)controller.HighestRecentPrecipitationLevel;
		PeriodsSinceHighestPrecipitation = controller.PeriodsSinceHighestPrecipitation;
		if (CurrentWeatherEvent != null && CurrentWeatherEvent.Precipitation > HighestRecentPrecipitationLevel)
		{
			HighestRecentPrecipitationLevel = CurrentWeatherEvent.Precipitation;
			PeriodsSinceHighestPrecipitation = 0;
		}
		CalculateCurrentTemperature();
		FeedClock.MinutesUpdated += HandleWeatherTick;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandleFiveSecondTick;
	}

	public override string FrameworkItemType => "WeatherController";

	public IClock FeedClock { get; protected set; }
	public IMudTimeZone FeedClockTimeZone { get; protected set; }
	public IRegionalClimate RegionalClimate { get; protected set; }
	public IWeatherEvent CurrentWeatherEvent { get; protected set; }
	public ISeason CurrentSeason { get; protected set; }
	public GeographicCoordinate GeographyForTimeOfDay { get; protected set; }
	public ICelestialObject Celestial { get; protected set; }
	public string DescribeCurrentWeather => CurrentWeatherEvent.WeatherDescription;
	public string CurrentWeatherRoomAddendum => CurrentWeatherEvent.WeatherRoomAddendum;
	public double CurrentTemperature { get; protected set; }
	public int ConsecutiveUnchangedPeriods { get; protected set; }
	public PrecipitationLevel HighestRecentPrecipitationLevel { get; protected set; }
	public int PeriodsSinceHighestPrecipitation { get; protected set; }

	private int _minuteCounter;

	public int MinuteCounter
	{
		get => _minuteCounter;
		protected set
		{
			_minuteCounter = value;
			Changed = true;
		}
	}

	private int _minutesSinceLastFlavourEcho;

	public int MinutesSinceLastFlavourEcho
	{
		get => _minutesSinceLastFlavourEcho;
		protected set
		{
			_minutesSinceLastFlavourEcho = value;
			Changed = true;
		}
	}

	public event WeatherEchoDelegate WeatherEcho;
	public event WeatherChangedDelegate WeatherChanged;
	public event WeatherRoomTickDelegate WeatherRoomTick;

	public void SetWeather(IWeatherEvent newEvent)
	{
		var oldEvent = CurrentWeatherEvent;
		CurrentWeatherEvent = newEvent;
		Changed = true;
		var echo = newEvent.DescribeTransitionTo(oldEvent)?.SubstituteANSIColour();
		if (!string.IsNullOrEmpty(echo))
		{
			WeatherEcho?.Invoke(this, echo);
		}

		WeatherChanged?.Invoke(this, oldEvent, newEvent);

		if (CurrentWeatherEvent != null && CurrentWeatherEvent.Precipitation >= HighestRecentPrecipitationLevel)
		{
			HighestRecentPrecipitationLevel = CurrentWeatherEvent.Precipitation;
			PeriodsSinceHighestPrecipitation = 0;
		}

		CalculateCurrentTemperature();
	}

	private void HandleFiveSecondTick()
	{
		if (CurrentWeatherEvent is null)
		{
			return;
		}

		WeatherRoomTick?.Invoke(CurrentWeatherEvent.OnFiveSecondEvent);
	}

	public void HandleWeatherTick()
	{
		var weatherChanged = false;
		if (CurrentWeatherEvent is not null)
		{
			WeatherRoomTick?.Invoke(CurrentWeatherEvent.OnMinuteEvent);
		}

		if (++MinuteCounter >= RegionalClimate.ClimateModel.MinuteProcessingInterval)
		{
			var currentTimeOfDay = Celestial?.CurrentTimeOfDay(GeographyForTimeOfDay) ?? TimeOfDay.Night;
			CurrentSeason = RegionalClimate.SeasonRotation.Get(Celestial?.CurrentCelestialDay ?? 0);
			MinuteCounter = 0;
			var weather = RegionalClimate.ClimateModel.HandleWeatherTick(CurrentWeatherEvent, CurrentSeason, currentTimeOfDay, ConsecutiveUnchangedPeriods);
			if (weather == CurrentWeatherEvent)
			{
				ConsecutiveUnchangedPeriods++;
			}
			else
			{
				weatherChanged = true;
				var echo = weather.DescribeTransitionTo(CurrentWeatherEvent)?.SubstituteANSIColour();
				var oldWeather = CurrentWeatherEvent;
				CurrentWeatherEvent = weather;
				if (!string.IsNullOrEmpty(echo))
				{
					WeatherEcho?.Invoke(this, echo);
				}
				WeatherChanged?.Invoke(this, oldWeather, weather);
			}
		}

		if (CurrentWeatherEvent != null && CurrentWeatherEvent.Precipitation >= HighestRecentPrecipitationLevel)
		{
			HighestRecentPrecipitationLevel = CurrentWeatherEvent.Precipitation;
			PeriodsSinceHighestPrecipitation = 0;
		}
		else
		{
			PeriodsSinceHighestPrecipitation++;
			if (PeriodsSinceHighestPrecipitation > Gameworld.GetStaticInt("MaximumPeriodsForRecentWeather"))
			{
				HighestRecentPrecipitationLevel = CurrentWeatherEvent?.Precipitation ?? PrecipitationLevel.Parched;
				PeriodsSinceHighestPrecipitation = 0;
			}
		}

		CalculateCurrentTemperature();
		if (weatherChanged)
		{
			Changed = true;
		}
		else
		{
			if (++MinutesSinceLastFlavourEcho >= RegionalClimate.ClimateModel.MinimumMinutesBetweenFlavourEchoes &&
				RandomUtilities.Roll(1.0, RegionalClimate.ClimateModel.MinuteFlavourEchoChance))
			{
				var echo = CurrentWeatherEvent.RandomFlavourEcho();
				if (!string.IsNullOrEmpty(echo))
				{
					WeatherEcho?.Invoke(this, echo);
				}
			}
		}
	}

	private void CalculateCurrentTemperature()
	{
		CurrentTemperature =
			RegionalClimate.HourlyBaseTemperaturesBySeason[
				(CurrentSeason, FeedClock.CurrentTime.GetTimeByTimezone(FeedClockTimeZone).Hours)] +
			CurrentWeatherEvent.TemperatureEffect;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.WeatherControllers.Find(Id);
		dbitem.CurrentSeasonId = CurrentSeason.Id;
		dbitem.CurrentWeatherEventId = CurrentWeatherEvent.Id;
		dbitem.MinutesCounter = MinuteCounter;
		dbitem.ConsecutiveUnchangedPeriods = ConsecutiveUnchangedPeriods;
		dbitem.HighestRecentPrecipitationLevel = (int)HighestRecentPrecipitationLevel;
		dbitem.PeriodsSinceHighestPrecipitation = PeriodsSinceHighestPrecipitation;
		Changed = false;
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Weather Controller #{Id.ToString("N0", voyeur)}: {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Feed Clock: {FeedClock.Id.ToString("N0", voyeur)}");
		sb.AppendLine($"Feed Clock Timezone: {FeedClockTimeZone.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Regional Climate: {RegionalClimate.Name.Colour(Telnet.BoldCyan)}");
		sb.AppendLine(
			$"Geography: Latitude {GeographyForTimeOfDay.Latitude.RadiansToDegrees().ToString("N3").Colour(Telnet.Green)} Longitude {GeographyForTimeOfDay.Longitude.RadiansToDegrees().ToString("N3").Colour(Telnet.Green)} Elevation {$"{GeographyForTimeOfDay.Elevation}m".Colour(Telnet.Green)}");
		sb.AppendLine($"Current Season: {CurrentSeason.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Current Weather: {CurrentWeatherEvent.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Consecutive Unchanged Periods: {ConsecutiveUnchangedPeriods.ToString("N0", voyeur)}");
		sb.AppendLine(
			$"Current Temperature: {Gameworld.UnitManager.DescribeExact(CurrentTemperature, Framework.Units.UnitType.Temperature, voyeur)}");
		sb.AppendLine(
			$"Highest Recent Precipitation: {HighestRecentPrecipitationLevel.Describe().Colour(Telnet.BoldBlue)} ({PeriodsSinceHighestPrecipitation.ToString("N0", voyeur)} periods)");
		return sb.ToString();
	}
}