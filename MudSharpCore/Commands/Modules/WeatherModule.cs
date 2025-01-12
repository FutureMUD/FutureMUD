using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Commands.Helpers;

namespace MudSharp.Commands.Modules;

internal class WeatherModule : BaseBuilderModule
{
	private WeatherModule() : base("Weather")
	{
		IsNecessary = true;
	}

	public static WeatherModule Instance { get; } = new();

	private const string WeatherPlayerHelp = @"The #3weather#0 command is used to see information about the weather and temperature at your current location. You may not be able to see everything about the weather from some locations inside buildings or underground.

The syntax is simply #3weather#0.";

	private const string WeatherAdminHelp = @"The #3weather#0 command is used to see information about the weather and temperature at your current location. You may not be able to see everything about the weather from some locations inside buildings or underground.

The syntax is simply #3weather#0.

As an admin, you can also do #3weather transition <event>#0 to force a transition to a particular weather event at the next tick.";

	[PlayerCommand("Weather", "weather")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("weather", WeatherPlayerHelp, AutoHelp.HelpArg, WeatherAdminHelp)]
	[CustomModuleName("World")]
	protected static void Weather(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!actor.IsAdministrator() || ss.IsFinished)
		{
			PlayerWeather(actor);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "transition":
			case "trans":
				WeatherTransition(actor, ss);
				return;
		}
	}

	private static void WeatherTransition(ICharacter actor, StringStack ss)
	{
		if (actor.Location.WeatherController == null)
		{
			actor.OutputHandler.Send("This location doesn't have any weather.");
			return;
		}

		if (ss.IsFinished)
		{
			var newWeather = actor.Location.WeatherController.RegionalClimate.ClimateModel.PermittedTransitions(
				actor.Location.WeatherController.CurrentWeatherEvent, actor.Location.WeatherController.CurrentSeason,
				actor.Location.CurrentTimeOfDay)
			                      .ToList();
			if (!newWeather.Any())
			{
				actor.OutputHandler.Send("There are no valid transitions for the current weather event.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine(
				$"The following weather events are valid transitions for the current {actor.Location.WeatherController.CurrentWeatherEvent.Name.ColourName()} event:");
			sb.AppendLine();
			foreach (var item in newWeather)
			{
				sb.AppendLine(
					$"\t[#{item.Id.ToString("N0", actor)}] {item.Name.ColourName()} (T: {actor.Gameworld.UnitManager.DescribeBonus(item.TemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}, WindT: {actor.Gameworld.UnitManager.DescribeBonus(item.WindTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}, RainT: {actor.Gameworld.UnitManager.DescribeBonus(item.PrecipitationTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}, {item.Wind.DescribeEnum(true).ColourValue()}, {item.Precipitation.DescribeEnum(true).ColourValue()})");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var choices = actor.Location.WeatherController.RegionalClimate.ClimateModel.PermittedTransitions(
			actor.Location.WeatherController.CurrentWeatherEvent, actor.Location.WeatherController.CurrentSeason,
			actor.Location.CurrentTimeOfDay);
		var choice = long.TryParse(ss.PopSpeech(), out var value)
			? choices.FirstOrDefault(x => x.Id == value)
			: choices.FirstOrDefault(x => x.Name.EqualTo(ss.Last));
		if (choice == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid choice. See {"weather transition".ColourCommand()} for a list of valid transitions and enter either an ID# or a name.");
			return;
		}

		actor.Location.WeatherController.SetWeather(choice);
	}

	private static void PlayerWeather(ICharacter actor)
	{
		var weather = actor.Location.CurrentWeather(actor);
		var temperature = actor.Location.CurrentTemperature(actor);
		var season = actor.Location.CurrentSeason(actor);
		var (floor, ceiling) = actor.Body.TolerableTemperatures(true);
		var (floorNatural, ceilingNatural) = actor.Body.TolerableTemperatures(false);
		var sb = new StringBuilder();
		if (season is not null)
		{
			sb.AppendLine($"The current season is {season.DisplayName.ColourValue()}{season.SeasonGroup?.ColourName().ParenthesesSpace() ?? ""}.");
		}
		sb.AppendLine(weather?.WeatherDescription ?? "There doesn't appear to be any weather here.");
		sb.AppendLine(
			$"The current temperature here is {TemperatureExtensions.SubjectiveTemperature(temperature, floorNatural, ceilingNatural).DescribeColour()}{(actor.IsAdministrator() ? $" ({actor.Gameworld.UnitManager.Describe(temperature, UnitType.Temperature, actor)})" : "")}.");
		sb.AppendLine(
			$"You are currently feeling {TemperatureExtensions.SubjectiveTemperature(temperature, floor, ceiling).DescribeColour()}.");
		actor.OutputHandler.Send(sb.ToString());
	}

	public const string SeasonHelp = @"The #3season#0 command is used to view, edit and create seasons. Seasons are used mostly in the weather system.

The syntax is as follows:

	#3season list [<filters>]#0 - lists all of the seasons
	#3season edit <which>#0 - begins editing a season
	#3season edit new <name> <onsetday>#0 - creates a new season
	#3season clone <old> <name>#0 - creates a new season as a clone of another
	#3season close#0 - stops editing a season
	#3season show <which>#0 - views information about a season
	#3season show#0 - views information about your currently editing season
	#3season set name <name>#0 - sets the name of the season
	#3season set display <name>#0 - sets the display name in the time command
	#3season set onset <##>#0 - sets the day number that this season begins
	#3season set group <group>#0 - sets the group that this season belongs to
	#3season set group none#0 - sets the season to belong to no group
	#3season set celestial <id|name>#0 - sets the celestial this season is tied to";

	[PlayerCommand("Season", "season")]
	[HelpInfo("season", SeasonHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Season(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.SeasonHelper);
	}

	public const string WeatherControllerHelp = @"The #3weathercontroller#0 command is used to view, edit and create weather controllers.

Weather controllers are used by zones, areas and individual rooms to work out what their current weather is, handle transitions between events, and everything else. They are the ""brains"" of the weather system.

The syntax is as follows:

	#3wc list [<filters>]#0 - lists all of the controllers
	#3wc edit <which>#0 - begins editing a controller
	#3wc edit new <name> <climate> <zone>#0 - creates a new controller
	#3wc close#0 - stops editing a controller
	#3wc show <which>#0 - views information about a controller
	#3wc show#0 - views information about your currently editing controller
	#3wc set name <name>#0 - rename the weather controller
	#3wc set clock <clock>#0 - set the clock
	#3wc set timezone <tz>#0 - set the timezone
	#3wc set longitude <degrees>#0 - set the longitude
	#3wc set latitude <degrees>#0 - set the latitude
	#3wc set elevation <height>#0 - sets the height above sea level
	#3wc set radius <measurement>#0 - sets the planetary radius
	#3wc set celestial <which>#0 - changes which celestial this is tied to";

	[PlayerCommand("WeatherController", "weathercontroller", "wc")]
	[HelpInfo("weathercontroller", WeatherControllerHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void WeatherController(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.WeatherControllerHelper);
	}

	public const string RegionalClimateHelp = @"The #3regionalclimate#0 command is used to view, edit and create regional climates, which link climate models, seasons and temperature data together. They are primarily used by weather controllers to control how their simulation works.

The syntax is as follows:

	#3rc list [<filters>]#0 - lists all of the climates
	#3rc edit <which>#0 - begins editing a climate
	#3rc edit new <name> <model>#0 - creates a new climate
	#3rc close#0 - stops editing a climate
	#3rc show <which>#0 - views information about a climate
	#3rc show#0 - views information about your currently editing climate
	#3rc set name <name>#0 - renames this regional climate
	#3rc set model <model>#0 - changes the climate model
	#3rc set season <which>#0 - toggles a season belonging to this regional climate
	#3rc set temp <season> <hour> <temp>#0 - sets an hourly temperature
	#3rc set temps <season> [<temp0> <temp1> ... <tempn>]#0 - bulk edits the seasonal temperatures";

	[PlayerCommand("RegionalClimate", "regionalclimate", "rc")]
	[HelpInfo("regionalclimate", RegionalClimateHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void RegionalClimate(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.RegionalClimateHelper);
	}

	public const string ClimateModelHelp = @"The #3climatemodel#0 command is used to view, edit and create climate models, which control weather patterns, events and transitions.

The syntax is as follows:

	#3clm list [<filters>]#0 - lists all of the climate models
	#3clm edit <which>#0 - begins editing a climate model
	#3clm edit new <name> <model>#0 - creates a new climate model
	#3clm close#0 - stops editing a climate model
	#3clm show <which>#0 - views information about a climate model
	#3clm show#0 - views information about your currently editing climate model
	#3clm set name <name>#0 - sets the name of this climate model
	#3clm set ticks <minutes>#0 - sets the minutes between weather ticks
	#3clm set echochance <%>#0 - sets the per-minute chance of echoes
	#3clm set echoticks <minutes>#0 - sets the minimum number of minutes between echoes
	#3clm set addseason <season>#0 - adds a season to this climate model
	#3clm set cloneseason <clone> <season>#0 - adds a season to this climate model with all the data from another
	#3clm set removeseason <season>#0 - removes a season from the climate model
	#3clm set season <season> chance <event> <%>#0 - sets the base chance of changing per tick for an event/season combo
	#3clm set season <season> transition <from> <to> <weight>#0 - sets a transition from one weather event to another
	#3clm set season <season> transition <from> <to> 0#0 - removes a transition from one weather event to another";
	[HelpInfo("climatemodel", ClimateModelHelp, AutoHelp.HelpArgOrNoArg)]

	[PlayerCommand("ClimateModel", "climatemodel", "clm")]
	protected static void ClimateModel(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.ClimateModelHelper);
	}

	public const string WeatherEventHelp = @"The #3weatherevent#0 command is used to view, edit and create weather events.

The syntax is as follows:

	#3weatherevent list [<filters>]#0 - lists all of the weather events
	#3weatherevent edit <which>#0 - begins editing a weather event
	#3weatherevent edit new <name> simple|rain#0 - creates a new weather event
	#3weatherevent clone <old> <name>#0 - clones a weather event from another
	#3weatherevent close#0 - stops editing a weather event
	#3weatherevent show <which>#0 - views information about a weather event
	#3weatherevent show#0 - views information about your currently editing weather event
	#3weatherevent set name <name>#0 - renames the weather event
	#3weatherevent set desc <desc>#0 - the description of this weather event
	#3weatherevent set countas <other>#0 - sets this event counting as another one
	#3weatherevent set countas none#0 - clears this event counting as another one
	#3weatherevent set room <desc>#0 - the room description addendum
	#3weatherevent set room#0 - clears the room description addendum
	#3weatherevent set wind <type>#0 - sets the wind associated with this event
	#3weatherevent set rain <type>#0 - sets the rain associated with this event
	#3weatherevent set temp <value>#0 - sets the temperature change from this event
	#3weatherevent set raintemp <value>#0 - sets the temperature change from rain from this event
	#3weatherevent set windtemp <value>#0 - sets the temperature change from wind from this event
	#3weatherevent set light <%>#0 - sets how much natural light is let through
	#3weatherevent set sky#0 - toggles the sky being visible with this event
	#3weatherevent set morning|afternoon|dusk|dawn|night#0 - toggles a time of day applying
	#3weatherevent set default <echo>#0 - sets the default transition echo to this
	#3weatherevent set echo add <weight> <echo>#0 - adds a flavour echo to the rotation
	#3weatherevent set echo remove <##>#0 - removes the certain numbered flavour echo
	#3weatherevent set transition <from> <echo>#0 - sets a custom transition echo for another event
	#3weatherevent set transition none <from>#0 - removes a custom transition echo

Rain events only:

	#3weatherevent set liquid <liquid>#0 - sets the liquid that falls as rain";
	[HelpInfo("weatherevent", WeatherEventHelp, AutoHelp.HelpArgOrNoArg)]

	[PlayerCommand("WeatherEvent", "weatherevent")]
	protected static void WeatherEvent(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.WeatherEventHelper);
	}
}