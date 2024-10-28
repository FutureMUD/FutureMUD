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

namespace MudSharp.Climate.WeatherEvents;

public abstract class WeatherEventBase : FrameworkItem, IWeatherEvent, IHaveFuturemud
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
	public IEnumerable<TimeOfDay> PermittedTimesOfDay { get; protected set; }
	private long? _countsAsId;
	private IWeatherEvent _countsAs;

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

	public IFuturemud Gameworld { get; }
}