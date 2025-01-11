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
using MudSharp.Framework.Units;

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

	public WeatherController(IFuturemud gameworld, string name, IRegionalClimate climate, IZone zone)
	{
		Gameworld = gameworld;
		_name = name;
		FeedClock = Gameworld.Clocks.First();
		FeedClock.MinutesUpdated += HandleWeatherTick;
		FeedClockTimeZone = FeedClock.PrimaryTimezone;
		RegionalClimate = climate;
		Celestial = Gameworld.CelestialObjects.FirstOrDefault();
		GeographyForTimeOfDay = new GeographicCoordinate(zone.Geography.Latitude, zone.Geography.Longitude, zone.Geography.Elevation, 6371000);

		var currentTimeOfDay = Celestial?.CurrentTimeOfDay(GeographyForTimeOfDay) ?? TimeOfDay.Night;
		CurrentSeason = RegionalClimate.SeasonRotation.Get(Celestial?.CurrentCelestialDay ?? 0);
		CurrentWeatherEvent = RegionalClimate.ClimateModel.HandleWeatherTick(null, CurrentSeason, currentTimeOfDay, ConsecutiveUnchangedPeriods);
		CalculateCurrentTemperature();
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandleFiveSecondTick;
		using (new FMDB())
		{
			var dbitem = new Models.WeatherController
			{
				Name = Name,
				FeedClockId = FeedClock.Id,
				FeedClockTimeZoneId = FeedClockTimeZone.Id,
				RegionalClimateId = RegionalClimate.Id,
				CelestialId = Celestial?.Id,
				Latitude = GeographyForTimeOfDay.Latitude,
				Longitude = GeographyForTimeOfDay.Longitude,
				Elevation = GeographyForTimeOfDay.Elevation,
				Radius = GeographyForTimeOfDay.Radius,
				CurrentSeasonId = CurrentSeason.Id,
				CurrentWeatherEventId = CurrentWeatherEvent.Id,
				MinutesCounter = 0,
				HighestRecentPrecipitationLevel = 0,
				PeriodsSinceHighestPrecipitation = 0,
				ConsecutiveUnchangedPeriods = 0
			};
			FMDB.Context.WeatherControllers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
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
		dbitem.CelestialId = Celestial?.Id;
		dbitem.FeedClockId = FeedClock.Id;
		dbitem.FeedClockTimeZoneId = FeedClockTimeZone.Id;
		dbitem.Longitude = GeographyForTimeOfDay.Longitude;
		dbitem.Latitude = GeographyForTimeOfDay.Latitude;
		dbitem.Elevation = GeographyForTimeOfDay.Elevation;
		dbitem.Radius = GeographyForTimeOfDay.Radius;
		Changed = false;
	}

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - rename the weather controller
	#3clock <clock>#0 - set the clock
	#3timezone <tz>#0 - set the timezone
	#3longitude <degrees>#0 - set the longitude
	#3latitude <degrees>#0 - set the latitude
	#3elevation <height>#0 - sets the height above sea level
	#3radius <measurement>#0 - sets the planetary radius
	#3celestial <which>#0 - changes which celestial this is tied to";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "clock":
				return BuildingCommandClock(actor, command);
			case "timezone":
				return BuildingCommandTimezone(actor, command);
			case "longitude":
			case "long":
				return BuildingCommandLongitude(actor, command);
			case "latitude":
			case "lat":
				return BuildingCommandLatitude(actor, command);
			case "elevation":
			case "height":
				return BuildingCommandElevation(actor, command);
			case "radius":
				return BuildingCommandRadius(actor, command);
			case "celestial":
				return BuildingCommandCelestial(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandRadius(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the radius of the planet for this weather controller?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Length, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid length measurement.");
			return false;
		}

		var valueAsMetres = Gameworld.UnitManager.BaseHeightToMetres * value;
		GeographyForTimeOfDay = new GeographicCoordinate(GeographyForTimeOfDay.Latitude, GeographyForTimeOfDay.Longitude, GeographyForTimeOfDay.Elevation, valueAsMetres);
		Changed = true;
		actor.OutputHandler.Send($"The radius of the planet for this controller is now {Gameworld.UnitManager.DescribeMostSignificantExact(value, UnitType.Length, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandCelestial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which celestial should this controller be tied to?");
			return false;
		}

		var celestial = Gameworld.CelestialObjects.GetByIdOrName(command.SafeRemainingArgument);
		if (celestial is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid celestial object.");
			return false;
		}

		Celestial = celestial;
		Changed = true;
		actor.OutputHandler.Send($"This controller is now tied to the {celestial.Name.ColourValue()} celestial object. Make sure to also change the seasons.");
		return true;
	}

	private bool BuildingCommandElevation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the elevation above sea level for this weather controller?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Length, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid length measurement.");
			return false;
		}

		var valueAsMetres = Gameworld.UnitManager.BaseHeightToMetres * value;
		GeographyForTimeOfDay = new GeographicCoordinate(GeographyForTimeOfDay.Latitude, GeographyForTimeOfDay.Longitude, valueAsMetres, GeographyForTimeOfDay.Radius);
		Changed = true;
		actor.OutputHandler.Send($"This controller is now {Gameworld.UnitManager.DescribeMostSignificantExact(value, UnitType.Length, actor).ColourValue()} above sea level.");
		return true;
	}

	private bool BuildingCommandLatitude(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the latitude for this weather controller?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value >= 90 || value <= -90)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid latitude.");
			return false;
		}

		GeographyForTimeOfDay = new GeographicCoordinate(value.DegreesToRadians(), GeographyForTimeOfDay.Longitude, GeographyForTimeOfDay.Elevation, GeographyForTimeOfDay.Radius);
		Changed = true;
		actor.OutputHandler.Send($"This controller is now at {value.ToString("F6", actor).ColourValue()} degrees latitude");
		return true;
	}

	private bool BuildingCommandLongitude(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the longitude for this weather controller?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value >= 180 || value <= -180)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid longitude.");
			return false;
		}

		GeographyForTimeOfDay = new GeographicCoordinate(GeographyForTimeOfDay.Latitude, value.DegreesToRadians(), GeographyForTimeOfDay.Elevation, GeographyForTimeOfDay.Radius);
		Changed = true;
		actor.OutputHandler.Send($"This controller is now at {value.ToString("F6", actor).ColourValue()} degrees longitude");
		return true;
	}

	private bool BuildingCommandTimezone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which timezone should this weather controller be tied to?");
			return false;
		}

		var timezone = FeedClock.Timezones.GetByIdOrNames(command.SafeRemainingArgument);
		if (timezone is null)
		{
			actor.OutputHandler.Send($"The {FeedClock.Name.ColourValue()} clock has no timezone identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		FeedClockTimeZone = timezone;
		Changed = true;
		actor.OutputHandler.Send($"This weather controller is now in the {timezone.Name.ColourValue()} timezone.");
		return true;
	}

	private bool BuildingCommandClock(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which clock should this weather controller be tied to?");
			return false;
		}

		var clock = Gameworld.Clocks.GetByIdOrName(command.SafeRemainingArgument);
		if (clock is null)
		{
			actor.OutputHandler.Send($"There is no clock identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		FeedClock.MinutesUpdated -= HandleWeatherTick;
		FeedClock = clock;
		FeedClock.MinutesUpdated += HandleWeatherTick;
		FeedClockTimeZone = clock.PrimaryTimezone;
		actor.OutputHandler.Send($"This weather controller is now tied to the {clock.Name.ColourValue()} clock.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this controller?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.WeatherControllers.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a controller with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the controller from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Weather Controller #{Id.ToString("N0", voyeur)}: {Name}".GetLineWithTitleInner(voyeur, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Feed Clock: {FeedClock.Name.ColourValue()}");
		sb.AppendLine($"Feed Clock Timezone: {FeedClockTimeZone.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Regional Climate: {RegionalClimate.Name.Colour(Telnet.BoldCyan)}");
		sb.AppendLine($"Latitude {GeographyForTimeOfDay.Latitude.RadiansToDegrees().ToString("N3").Colour(Telnet.Green)}");
		sb.AppendLine($"Longitude {GeographyForTimeOfDay.Longitude.RadiansToDegrees().ToString("N3").Colour(Telnet.Green)}");
		sb.AppendLine($"Elevation {$"{GeographyForTimeOfDay.Elevation}m".Colour(Telnet.Green)}");
		sb.AppendLine($"Planetary Radius: {Gameworld.UnitManager.DescribeMostSignificantExact(GeographyForTimeOfDay.Radius, UnitType.Length, voyeur).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Current Season: {CurrentSeason.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Current Weather: {CurrentWeatherEvent.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Consecutive Unchanged Periods: {ConsecutiveUnchangedPeriods.ToString("N0", voyeur)}");
		sb.AppendLine($"Current Temperature: {Gameworld.UnitManager.DescribeExact(CurrentTemperature, Framework.Units.UnitType.Temperature, voyeur)}");
		sb.AppendLine($"Highest Recent Precipitation: {HighestRecentPrecipitationLevel.Describe().Colour(Telnet.BoldBlue)} ({PeriodsSinceHighestPrecipitation.ToString("N0", voyeur)} periods)");
		return sb.ToString();
	}
}