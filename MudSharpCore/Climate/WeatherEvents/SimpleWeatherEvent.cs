using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Units;

namespace MudSharp.Climate.WeatherEvents;

public class SimpleWeatherEvent : WeatherEventBase
{
	public SimpleWeatherEvent(Models.WeatherEvent weather, IFuturemud gameworld) : base(weather, gameworld)
	{
		var definition = XElement.Parse(weather.AdditionalInfo);
		foreach (var element in definition.Element("Echoes")?.Elements("Echo") ?? Enumerable.Empty<XElement>())
		{
			_randomEchoes.Add((element.Value, double.Parse(element.Attribute("chance")?.Value ?? "1.0")));
		}

		var transition = definition.Element("Transitions");
		_defaultTransitionEcho = transition.Element("Default").Value;
		foreach (var element in transition.Elements("Transition"))
		{
			_transitionEchoOverrides[long.Parse(element.Attribute("other").Value)] = element.Value;
		}
	}
	
	protected virtual XElement SaveToXml()
	{
		return new XElement("Info",
			new XElement("Echoes",
				from item in _randomEchoes
				select new XElement("Echo",
					new XAttribute("chance", item.Chance),
					new XCData(item.Echo)
				)
			),
			new XElement("Transitions",
				new XElement("Default", new XCData(_defaultTransitionEcho)),
				from item in _transitionEchoOverrides
				select new XElement("Transition",
					new XAttribute("other", item.Key),
					new XCData(item.Value)
				)
			)
		);
	}

	protected virtual void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.WeatherEvent
			{
				WeatherEventType = "simple",
				Name = Name,
				CountsAsId = CountsAs?.Id,
				LightLevelMultiplier = LightLevelMultiplier,
				ObscuresViewOfSky = ObscuresViewOfSky,
				PermittedAtAfternoon = PermittedTimesOfDay.Contains(TimeOfDay.Afternoon),
				PermittedAtDawn = PermittedTimesOfDay.Contains(TimeOfDay.Dawn),
				PermittedAtDusk = PermittedTimesOfDay.Contains(TimeOfDay.Dusk),
				PermittedAtMorning = PermittedTimesOfDay.Contains(TimeOfDay.Morning),
				PermittedAtNight = PermittedTimesOfDay.Contains(TimeOfDay.Night),
				Precipitation = (int)Precipitation,
				Wind = (int)Wind,
				TemperatureEffect = TemperatureEffect,
				PrecipitationTemperatureEffect = PrecipitationTemperatureEffect,
				WindTemperatureEffect = WindTemperatureEffect,
				WeatherDescription = WeatherDescription,
				WeatherRoomAddendum = WeatherRoomAddendum,
				AdditionalInfo = SaveToXml().ToString()
			};
			FMDB.Context.WeatherEvents.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected SimpleWeatherEvent()
	{
	}

	public SimpleWeatherEvent(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		Precipitation = PrecipitationLevel.Parched;
		Wind = WindLevel.None;
		WeatherDescription = "An undescribed weather event";
		WeatherRoomAddendum = "";
		TemperatureEffect = 0.0;
		PrecipitationTemperatureEffect = 0.0;
		WindTemperatureEffect = 0.0;
		LightLevelMultiplier = 1.0;
		ObscuresViewOfSky = false;
		PermittedTimesOfDay = [TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Morning, TimeOfDay.Night];
		DoDatabaseInsert();
	}

	private SimpleWeatherEvent(SimpleWeatherEvent rhs, string name) : base(rhs.Gameworld, name)
	{
		Precipitation = rhs.Precipitation;
		Wind = rhs.Wind;
		WeatherDescription = rhs.WeatherDescription;
		WeatherRoomAddendum = rhs.WeatherRoomAddendum;
		TemperatureEffect = rhs.TemperatureEffect;
		PrecipitationTemperatureEffect = rhs.PrecipitationTemperatureEffect;
		WindTemperatureEffect = rhs.WindTemperatureEffect;
		LightLevelMultiplier = rhs.LightLevelMultiplier;
		ObscuresViewOfSky = rhs.ObscuresViewOfSky;
		PermittedTimesOfDay = rhs.PermittedTimesOfDay.ToList();
		_defaultTransitionEcho = rhs._defaultTransitionEcho;
		_randomEchoes.AddRange(rhs._randomEchoes);
		foreach (var item in rhs._transitionEchoOverrides)
		{
			_transitionEchoOverrides[item.Key] = item.Value;
		}
		DoDatabaseInsert();
	}

	#region Overrides of WeatherEventBase

	/// <inheritdoc />
	public override IWeatherEvent Clone(string name)
	{
		return new SimpleWeatherEvent(this, name);
	}

	/// <inheritdoc />
	public override string SubtypeHelpText => @"	#3default <echo>#0 - sets the default transition echo to this
	#3echo add <weight> <echo>#0 - adds a flavour echo to the rotation
	#3echo remove <##>#0 - removes the certain numbered flavour echo
	#3transition <from> <echo>#0 - sets a custom transition echo for another event
	#3transition none <from>#0 - removes a custom transition echo";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "default":
				return BuildingCommandDefault(actor, command);
			case "transition":
				return BuildingCommandTransition(actor, command);
			case "echo":
				return BuildingCommandEcho(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either #3add#0 or #3remove#0 an echo.".SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "add":
				break;
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandRemoveEcho(actor, command);
			default:
				actor.OutputHandler.Send("You must either #3add#0 or #3remove#0 an echo.".SubstituteANSIColour());
				return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the relative weight of this random echo compared to others?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var chance) || chance <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What echo do you want to add to this weather event?");
			return false;
		}

		var echo = command.SafeRemainingArgument.ProperSentences();
		_randomEchoes.Add((echo, chance));
		var total = _randomEchoes.Sum(x => x.Chance);
		actor.OutputHandler.Send($"You add the following echo with a weight of {chance.ToStringN2Colour(actor)} ({(chance / total).ToStringP2Colour(actor)}): {echo.SubstituteANSIColour()}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRemoveEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which numbered echo do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 || value > _randomEchoes.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid number between {1.ToStringN0Colour(actor)} and {_randomEchoes.Count.ToStringN0Colour(actor)}.");
			return false;
		}

		var echo = _randomEchoes[value - 1];
		_randomEchoes.RemoveAt(value - 1);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {value.ToOrdinal().ColourValue()} echo: {echo.Echo.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandTransition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify another weather event to set a custom transition from, or use #3none#0 to clear one.".SubstituteANSIColour());
			return false;
		}

		IWeatherEvent we;
		var which = command.PopSpeech();
		if (which.EqualTo("none"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which other weather event do you want to remove the custom transition for?");
				return false;
			}

			we = Gameworld.WeatherEvents.GetByIdOrName(command.SafeRemainingArgument);
			if (we is null)
			{
				actor.OutputHandler.Send("There is no such weather event.");
				return false;
			}

			if (!_transitionEchoOverrides.ContainsKey(we.Id))
			{
				actor.OutputHandler.Send($"This weather event has no custom transition echo for the {we.Name.ColourValue()} weather event.");
				return false;
			}

			_transitionEchoOverrides.Remove(we.Id);
			Changed = true;
			actor.OutputHandler.Send($"This weather event no longer has any custom transition echo for the {we.Name.ColourValue()} weather event.");
			return true;

			
		}

		we = Gameworld.WeatherEvents.GetByIdOrName(command.SafeRemainingArgument);
		if (we is null)
		{
			actor.OutputHandler.Send("There is no such weather event.");
			return false;
		}

		if (we == this)
		{
			actor.OutputHandler.Send("You cannot set a custom transition for an event to itself.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the custom transition echo for the {we.Name.ColourValue()} weather event?");
			return false;
		}

		var echo = command.SafeRemainingArgument.ProperSentences();
		_transitionEchoOverrides[we.Id] = echo;
		Changed = true;
		actor.OutputHandler.Send($"The transition from the {we.Name.ColourValue()} weather event now has the following custom echo:\n{echo.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandDefault(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the default transition echo for this weather event be?");
			return false;
		}

		_defaultTransitionEcho = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"The default transition echo for this event is now: {_defaultTransitionEcho.SubstituteANSIColour()}");
		return true;
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Simple Weather Event #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Counts As: {CountsAs?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Description: {WeatherDescription.ColourCommand()}");
		sb.AppendLine($"Room Addendum: {WeatherRoomAddendum?.SubstituteANSIColour()}");
		sb.AppendLine($"Precipitation: {Precipitation.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Wind: {Wind.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Temperature Effect: {Gameworld.UnitManager.DescribeDecimal(TemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Wind Temperature Effect: {Gameworld.UnitManager.DescribeDecimal(WindTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Precipitation Temperature Effect: {Gameworld.UnitManager.DescribeDecimal(PrecipitationTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Obscure Sky: {ObscuresViewOfSky.ToColouredString()}");
		sb.AppendLine($"Light Level Multiplier: {LightLevelMultiplier.ToStringP2Colour(actor)}");
		sb.AppendLine($"Permitted Times: {PermittedTimesOfDay.ListToColouredString()}");
		sb.AppendLine($"Default Transition Echo:");
		sb.AppendLine();
		sb.AppendLine($"\t{_defaultTransitionEcho?.SubstituteANSIColour()}");
		sb.AppendLine();
		sb.AppendLine("Special Transition Echoes:");
		sb.AppendLine();
		if (_transitionEchoOverrides.Count > 0)
		{
			foreach (var item in _transitionEchoOverrides)
			{
				var we = Gameworld.WeatherEvents.Get(item.Key);
				if (we is null)
				{
					continue;
				}
				sb.AppendLine($"\t{we.Name.ColourName()} (#{we.Id.ToStringN0(actor)}): {item.Value.SubstituteANSIColour()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}
		sb.AppendLine();
		sb.AppendLine("Random Flavour Echoes:");
		sb.AppendLine();
		var total = _randomEchoes.Sum(x => x.Chance);
		if (total > 0.0)
		{
			foreach (var echo in _randomEchoes)
			{
				sb.AppendLine($"\t{echo.Chance.ToStringN2Colour(actor)} ({(echo.Chance/total).ToStringP2Colour(actor)}): {echo.Echo.SubstituteANSIColour()}");
			}
		}
		return sb.ToString();
	}

	#endregion

	protected readonly List<(string Echo, double Chance)> _randomEchoes = new();
	protected string _defaultTransitionEcho;
	protected readonly Dictionary<long, string> _transitionEchoOverrides = new();

	public override string RandomFlavourEcho()
	{
		return _randomEchoes.GetWeightedRandom(x => x.Chance).Echo?.SubstituteANSIColour();
	}

	public override string DescribeTransitionTo([CanBeNull] IWeatherEvent oldEvent)
	{
		if (oldEvent == null)
		{
			return _defaultTransitionEcho;
		}

		if (_transitionEchoOverrides.ContainsKey(oldEvent.Id))
		{
			return _transitionEchoOverrides[oldEvent.Id];
		}

		if (oldEvent.CountsAs != null && _transitionEchoOverrides.ContainsKey(oldEvent.CountsAs.Id))
		{
			return _transitionEchoOverrides[oldEvent.CountsAs.Id];
		}

		return _defaultTransitionEcho;
	}

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.WeatherEvents.Find(Id);
		dbitem.Name = Name;
		dbitem.CountsAsId = CountsAs?.Id;
		dbitem.LightLevelMultiplier = LightLevelMultiplier;
		dbitem.ObscuresViewOfSky = ObscuresViewOfSky;
		dbitem.PermittedAtAfternoon = PermittedTimesOfDay.Contains(TimeOfDay.Afternoon);
		dbitem.PermittedAtDawn = PermittedTimesOfDay.Contains(TimeOfDay.Dawn);
		dbitem.PermittedAtDusk = PermittedTimesOfDay.Contains(TimeOfDay.Dusk);
		dbitem.PermittedAtMorning = PermittedTimesOfDay.Contains(TimeOfDay.Morning);
		dbitem.PermittedAtNight = PermittedTimesOfDay.Contains(TimeOfDay.Night);
		dbitem.Precipitation = (int)Precipitation;
		dbitem.Wind = (int)Wind;
		dbitem.TemperatureEffect = TemperatureEffect;
		dbitem.PrecipitationTemperatureEffect = PrecipitationTemperatureEffect;
		dbitem.WindTemperatureEffect = WindTemperatureEffect;
		dbitem.WeatherDescription = WeatherDescription;
		dbitem.WeatherRoomAddendum = WeatherRoomAddendum;
		dbitem.AdditionalInfo = SaveToXml().ToString();
		Changed = false;
	}

	#endregion
}