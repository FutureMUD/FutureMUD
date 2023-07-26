using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Commands.Modules;

internal class WeatherModule : Module<ICharacter>
{
	private WeatherModule() : base("Weather")
	{
		IsNecessary = true;
	}

	public static WeatherModule Instance { get; } = new();

	private const string WeatherPlayerHelp = @"";
	private const string WeatherAdminHelp = @"";

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
}