using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;

namespace MudSharp.Climate.WeatherEvents;

public abstract class WeatherEventBase : SaveableItem, IWeatherEvent, IHaveFuturemud
{
	public PrecipitationLevel Precipitation { get; protected set; }
	public WindLevel Wind { get; protected set; }
	public string WeatherDescription { get; protected set; }
	public string WeatherRoomAddendum { get; protected set; }
	public double TemperatureEffect { get; protected set; }
	public double PrecipitationTemperatureEffect { get; protected set; }
	public double WindTemperatureEffect { get; protected set; }
	public double LightLevelMultiplier { get; protected set; }
	public bool ObscuresViewOfSky { get; protected set; }

	private List<TimeOfDay> _permittedTimesOfDay;

	public IEnumerable<TimeOfDay> PermittedTimesOfDay
	{
		get => _permittedTimesOfDay;
		protected set => _permittedTimesOfDay = value.ToList();
	}

	protected long? _countsAsId;
	protected IWeatherEvent _countsAs;

	public IWeatherEvent CountsAs
	{
		get
		{
			if (_countsAs == null && _countsAsId.HasValue)
			{
				_countsAs = Gameworld.WeatherEvents.Get(_countsAsId.Value);
			}

			return _countsAs;
		}
	}

	public virtual void OnMinuteEvent(ICell cell)
	{
		// Do nothing
	}

	public virtual void OnFiveSecondEvent(ICell cell)
	{
		// Do nothing
	}

	public abstract string RandomFlavourEcho();
	public abstract string DescribeTransitionTo([CanBeNull] IWeatherEvent oldEvent);

	public sealed override string FrameworkItemType => "WeatherEvent";

	public abstract IWeatherEvent Clone(string name);

	protected WeatherEventBase()
	{
	}

	protected WeatherEventBase(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
	}

	protected WeatherEventBase(Models.WeatherEvent weather, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = weather.Id;
		_name = weather.Name;
		WeatherDescription = weather.WeatherDescription;
		WeatherRoomAddendum = weather.WeatherRoomAddendum;
		TemperatureEffect = weather.TemperatureEffect;
		PrecipitationTemperatureEffect = weather.PrecipitationTemperatureEffect;
		WindTemperatureEffect = weather.WindTemperatureEffect;
		Precipitation = (PrecipitationLevel)weather.Precipitation;
		Wind = (WindLevel)weather.Wind;
		LightLevelMultiplier = weather.LightLevelMultiplier;
		ObscuresViewOfSky = weather.ObscuresViewOfSky;
		var list = new List<TimeOfDay>();
		if (weather.PermittedAtNight)
		{
			list.Add(TimeOfDay.Night);
		}

		if (weather.PermittedAtDawn)
		{
			list.Add(TimeOfDay.Dawn);
		}

		if (weather.PermittedAtMorning)
		{
			list.Add(TimeOfDay.Morning);
		}

		if (weather.PermittedAtAfternoon)
		{
			list.Add(TimeOfDay.Afternoon);
		}

		if (weather.PermittedAtDusk)
		{
			list.Add(TimeOfDay.Dusk);
		}

		PermittedTimesOfDay = list;
		_countsAsId = weather.CountsAsId;
	}

	#region IFutureProgVariable implementation

	public ProgVariableTypes Type => ProgVariableTypes.WeatherEvent;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "obscuressky":
				return new BooleanVariable(ObscuresViewOfSky);
			case "lightmultiplier":
				return new NumberVariable(LightLevelMultiplier);
			case "temperatureeffect":
				return new NumberVariable(TemperatureEffect);
			case "precipitationtemperatureeffect":
				return new NumberVariable(PrecipitationTemperatureEffect);
			case "windtemperatureeffect":
				return new NumberVariable(WindTemperatureEffect);
			case "permittednight":
				return new BooleanVariable(PermittedTimesOfDay.Contains(TimeOfDay.Night));
			case "permitteddawn":
				return new BooleanVariable(PermittedTimesOfDay.Contains(TimeOfDay.Dawn));
			case "permittedmorning":
				return new BooleanVariable(PermittedTimesOfDay.Contains(TimeOfDay.Morning));
			case "permittedafternoon":
				return new BooleanVariable(PermittedTimesOfDay.Contains(TimeOfDay.Afternoon));
			case "permitteddusk":
				return new BooleanVariable(PermittedTimesOfDay.Contains(TimeOfDay.Dusk));
			case "description":
				return new TextVariable(WeatherDescription);
			case "windlevel":
				return new NumberVariable((int)Wind);
			case "rainlevel":
				if (!Precipitation.IsRaining())
				{
					return new NumberVariable(0);
				}
				else if (Precipitation == PrecipitationLevel.Sleet)
				{
					return new NumberVariable((int)(PrecipitationLevel.Rain - PrecipitationLevel.Humid));
				}
				else
				{
					return new NumberVariable((int)(Precipitation - PrecipitationLevel.Humid));
				}
			case "snowlevel":
				if (!Precipitation.IsSnowing())
				{
					return new NumberVariable(0);
				}
				else if (Precipitation == PrecipitationLevel.Sleet)
				{
					return new NumberVariable(PrecipitationLevel.LightSnow - PrecipitationLevel.TorrentialRain);
				}
				else
				{
					return new NumberVariable(Precipitation - PrecipitationLevel.TorrentialRain);
				}
			case "wind":
				return new TextVariable(Wind.Describe());
			case "rain":
				if (Precipitation.IsRaining())
				{
					return new TextVariable(Precipitation.Describe());
				}
				else
				{
					return new TextVariable("None");
				}
			case "snow":
				if (Precipitation.IsSnowing())
				{
					return new TextVariable(Precipitation.Describe());
				}
				else
				{
					return new TextVariable("None");
				}
			case "precipitation":
				return new TextVariable(Precipitation.Describe());
		}

		throw new NotSupportedException($"Unsupported property type {property} in {FrameworkItemType}.GetProperty");
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "description", ProgVariableTypes.Text },
			{ "windlevel", ProgVariableTypes.Number },
			{ "rainlevel", ProgVariableTypes.Number },
			{ "snowlevel", ProgVariableTypes.Number },
			{ "wind", ProgVariableTypes.Text },
			{ "rain", ProgVariableTypes.Text },
			{ "snow", ProgVariableTypes.Text },
			{ "precipitation", ProgVariableTypes.Text },
			{ "obscuressky", ProgVariableTypes.Boolean },
			{ "lightmultiplier", ProgVariableTypes.Number },
			{ "temperatureeffect", ProgVariableTypes.Number },
			{ "precipitationtemperatureeffect", ProgVariableTypes.Number },
			{ "windtemperatureeffect", ProgVariableTypes.Number },
			{ "permittednight", ProgVariableTypes.Boolean },
			{ "permitteddawn", ProgVariableTypes.Boolean },
			{ "permittedmorning", ProgVariableTypes.Boolean },
			{ "permittedafternoon", ProgVariableTypes.Boolean },
			{ "permitteddusk", ProgVariableTypes.Boolean }
		};
	}

	private new static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "description", "" },
			{ "windlevel", "" },
			{ "rainlevel", "" },
			{ "snowlevel", "" },
			{ "wind", "" },
			{ "rain", "" },
			{ "snow", "" },
			{ "precipitation", "" },
			{ "obscuressky", "" },
			{ "lightmultiplier", "" },
			{ "temperatureeffect", "" },
			{ "precipitationtemperatureeffect", "" },
			{ "windtemperatureeffect", "" },
			{ "permittednight", "" },
			{ "permitteddawn", "" },
			{ "permittedmorning", "" },
			{ "permittedafternoon", "" },
			{ "permitteddusk", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.WeatherEvent, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Implementation of IEditableItem

	public string HelpInfo => $@"You can use the following options with this command:

	#3name <name>#0 - renames the weather event
	#3desc <desc>#0 - the description of this weather event
	#3countas <other>#0 - sets this event counting as another one
	#3countas none#0 - clears this event counting as another one
	#3room <desc>#0 - the room description addendum
	#3room#0 - clears the room description addendum
	#3wind <type>#0 - sets the wind associated with this event
	#3rain <type>#0 - sets the rain associated with this event
	#3temp <value>#0 - sets the temperature change from this event
	#3raintemp <value>#0 - sets the temperature change from rain from this event
	#3windtemp <value>#0 - sets the temperature change from wind from this event
	#3light <%>#0 - sets how much natural light is let through
	#3sky#0 - toggles the sky being visible with this event
	#3morning|afternoon|dusk|dawn|night#0 - toggles a time of day applying
	{SubtypeHelpText}";

	public abstract string SubtypeHelpText { get; }

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "room":
			case "addendum":
				return BuildingCommandRoomAddendum(actor, command);
			case "wind":
				return BuildingCommandWind(actor, command);
			case "rain":
				return BuildingCommandRain(actor, command);
			case "sky":
				return BuildingCommandSky(actor, command);
			case "light":
				return BuildingCommandLight(actor, command);
			case "temp":
			case "temperature":
				return BuildingCommandTemperature(actor, command);
			case "windtemp":
			case "windtemperature":
				return BuildingCommandWindTemperature(actor, command);
			case "raintemp":
			case "raintemperature":
				return BuildingCommandRainTemperature(actor, command);
			case "morning":
				return BuildingCommandTimeOfDay(actor, TimeOfDay.Morning);
			case "afternoon":
				return BuildingCommandTimeOfDay(actor, TimeOfDay.Afternoon);
			case "dawn":
				return BuildingCommandTimeOfDay(actor, TimeOfDay.Dawn);
			case "dusk":
				return BuildingCommandTimeOfDay(actor, TimeOfDay.Dusk);
			case "night":
				return BuildingCommandTimeOfDay(actor, TimeOfDay.Night);
			case "countsas":
				return BuildingCommandCountsAs(actor, command);
		}

		actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandCountsAs(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either enter another weather event to count as, or #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		var cmd = command.SafeRemainingArgument;
		if (cmd.EqualTo("none"))
		{
			_countsAs = null;
			_countsAsId = null;
			actor.OutputHandler.Send($"This weather event no longer counts as any other weather event.");
			return true;
		}

		var we = Gameworld.WeatherEvents.GetByIdOrName(cmd);
		if (we is null)
		{
			actor.OutputHandler.Send($"There is no weather event identified by the text {cmd.ColourCommand()}.");
			return false;
		}

		_countsAs = we;
		_countsAsId = we.Id;
		actor.OutputHandler.Send($"This weather event now counts as the other event {we.Name.ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandTimeOfDay(ICharacter actor, TimeOfDay time)
	{
		if (_permittedTimesOfDay.Contains(time))
		{
			_permittedTimesOfDay.Remove(time);
			Changed = true;
			actor.OutputHandler.Send($"This weather event will no longer occur during {time.DescribeColour()}.");
			return true;
		}

		_permittedTimesOfDay.Add(time);
		Changed = true;
		actor.OutputHandler.Send($"This weather event can now occur during {time.DescribeColour()}.");
		return true;
	}

	private bool BuildingCommandRainTemperature(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a temperature change.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.TemperatureDelta, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature change.");
			return false;
		}

		PrecipitationTemperatureEffect = value;
		Changed = true;
		actor.OutputHandler.Send($"This weather event now gives a modifier of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.TemperatureDelta, actor).ColourValue()} due to rain when not sheltered.");
		return true;
	}

	private bool BuildingCommandWindTemperature(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a temperature change.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.TemperatureDelta, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature change.");
			return false;
		}

		WindTemperatureEffect = value;
		Changed = true;
		actor.OutputHandler.Send($"This weather event now gives a modifier of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.TemperatureDelta, actor).ColourValue()} due to wind when not sheltered.");
		return true;
	}

	private bool BuildingCommandTemperature(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a temperature change.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.TemperatureDelta, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid temperature change.");
			return false;
		}

		TemperatureEffect = value;
		Changed = true;
		actor.OutputHandler.Send($"This weather event now gives a temperature modifier of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.TemperatureDelta, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandLight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of natural light should be present when this weather event is running?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		LightLevelMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This weather event now allows {value.ToStringP2Colour(actor)} of natural light through.");
		return true;
	}

	private bool BuildingCommandSky(ICharacter actor, StringStack command)
	{
		ObscuresViewOfSky = !ObscuresViewOfSky;
		Changed = true;
		actor.OutputHandler.Send($"This weather event {ObscuresViewOfSky.NowNoLonger()} obscures the view of the sky.");
		return true;
	}

	private bool BuildingCommandRain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What precipitation level should this weather event have? The valid values are {Enum.GetValues<PrecipitationLevel>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<PrecipitationLevel>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid precipitation level. The valid values are {Enum.GetValues<PrecipitationLevel>().ListToColouredString()}.");
			return false;
		}

		Precipitation = value;
		Changed = true;
		actor.OutputHandler.Send($"The precipitation level is now {value.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandWind(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What wind level should this weather event have? The valid values are {Enum.GetValues<WindLevel>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<WindLevel>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid wind level. The valid values are {Enum.GetValues<WindLevel>().ListToColouredString()}.");
			return false;
		}

		Wind = value;
		Changed = true;
		actor.OutputHandler.Send($"The wind level is now {value.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRoomAddendum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			WeatherRoomAddendum = "";
			Changed = true;
			actor.OutputHandler.Send($"This weather event no longer adds a room addendum.");
			return true;
		}

		WeatherRoomAddendum = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"This event now adds the following text to the room description:\n{WeatherRoomAddendum.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the description of this weather event to?");
			return false;
		}

		WeatherDescription = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"The description of this weather event is now: {WeatherDescription.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this weather event?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.WeatherEvents.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a weather event called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this weather event from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public abstract string Show(ICharacter actor);

	#endregion
}