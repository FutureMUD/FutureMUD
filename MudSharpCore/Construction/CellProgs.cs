using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Climate;
using MudSharp.Construction.Grids;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Construction;

public partial class Cell
{
	#region IFutureProgVariable Implementation

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "characters", ProgVariableTypes.Collection | ProgVariableTypes.Character },
			{ "items", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "surrounds", ProgVariableTypes.Collection | ProgVariableTypes.Location },
			{ "exits", ProgVariableTypes.Collection | ProgVariableTypes.Exit },
			{ "zone", ProgVariableTypes.Zone },
			{ "shard", ProgVariableTypes.Shard },
			{ "terrain", ProgVariableTypes.Terrain },
			{ "windlevel", ProgVariableTypes.Number },
			{ "rainlevel", ProgVariableTypes.Number },
			{ "snowlevel", ProgVariableTypes.Number },
			{ "temperature", ProgVariableTypes.Number },
			{ "temperaturec", ProgVariableTypes.Number },
			{ "temperaturef", ProgVariableTypes.Number },
			{ "temperaturek", ProgVariableTypes.Number },
			{ "wind", ProgVariableTypes.Text },
			{ "rain", ProgVariableTypes.Text },
			{ "snow", ProgVariableTypes.Text },
			{ "precipitation", ProgVariableTypes.Text },
			{ "weather", ProgVariableTypes.WeatherEvent },
			{ "tags", ProgVariableTypes.Text | ProgVariableTypes.Collection },
			{ "addedlight", ProgVariableTypes.Number },
			{ "addedlightfactor", ProgVariableTypes.Number },
			{ "atmosphereid", ProgVariableTypes.Number },
			{ "atmospheretype", ProgVariableTypes.Text },
			{ "atmospherename", ProgVariableTypes.Text },
			{ "outdoors", ProgVariableTypes.Number },
			{ "overlay", ProgVariableTypes.OverlayPackage },
			{ "overlays", ProgVariableTypes.OverlayPackage | ProgVariableTypes.Collection },
			{ "grids", ProgVariableTypes.Number | ProgVariableTypes.Collection },
			{ "electricgrids", ProgVariableTypes.Number | ProgVariableTypes.Collection },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "effects", ProgVariableTypes.Effect | ProgVariableTypes.Collection },
			{ "light", ProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the room" },
			{ "characters", "A collection of characters who are in the room (all layers)" },
			{ "items", "A collection of items that are in the room (all layers)" },
			{ "surrounds", "A collection of all rooms directly adjacent to this one" },
			{ "exits", "A collection of all the exits from this room" },
			{ "zone", "The zone that this room is in" },
			{ "shard", "The shard that this room is in" },
			{ "terrain", "The terrain type of this room" },
			{
				"windlevel",
				"A numerical representation of the wind in this room. Values as follows: 0=none, 1=still, 2=occasionalbreeze, 3=breeze, 4=wind, 5=strongwind, 6=galewind, 7=hurricanewind, 8=maelstromwind"
			},
			{
				"rainlevel",
				"A numerical representation of the rain in this room. Values as follows: 0=not raining, 1=lightrain, 2=rain, 3=heavyrain, 4=torrentialrain"
			},
			{
				"snowlevel",
				"A numerical representation of the snow in this room. Values as follows: 0=not snowing, 1=lightsnow/sleet, 2=snow, 3=heavysnow, blizzard=4"
			},
			{ "temperature", "The temperature (in celcius) in the room" },
			{ "temperaturec", "The temperature (in celcius) in the room" },
			{ "temperaturef", "The temperature (in farenheit) in the room" },
			{ "temperaturek", "The temperature (in kelvin) in the room" },
			{
				"wind",
				"A description of the wind in this room. Values are none, still, occasional breeze, breeze, wind, strong wind, gale wind, hurricane wind, maelstrom wind"
			},
			{
				"rain",
				"A description of the rain in this room. Values are none, light rain, rain, heavy rain, torrential rain, sleet"
			},
			{
				"snow",
				"A description of the snow in this room. Values are none, light snow, snow, heavy snow, blizzard, sleet"
			},
			{
				"precipitation",
				"A description of the precipitation in this room. Values are parched, dry, humid, light rain, rain, heavy rain, torrential rain, light snow, snow, heavy snow, blizzard, sleet"
			},
			{ "weather", "The current weather event at this room, if there is one (can be null)" },
			{ "tags", "A collection of the names of the tags applied to this room" },
			{ "addedlight", "The amount of light added for this room" },
			{ "addedlightfactor", "The multiplier for natural light in this room" },
			{ "atmosphereid", "The id of the atmosphere gas or liquid, 0 if none" },
			{ "atmospheretype", "Either Gas, Liquid or None depending on the atmosphere" },
			{ "atmospherename", "The name of the gas or liquid atmosphere, or None if none" },
			{
				"outdoors",
				"0=Indoors, 1=Indoors with Windows, 2=Outdoors, 3=Indoors No Light (Cave), 4=Indoors Climate Exposed (Shelter)"
			},
			{ "overlay", "The current cell overlay" },
			{ "overlays", "A collection of all the cell overlays for this room" },
			{ "grids", "A collection of the IDs of any grids in this location" },
			{ "electricgrids", "A collection of the IDs of any electrical grids in this location" },
			{ "name", "The name of this room" },
			{ "type", "Returns the name of the framework item type, for example, character or gameitem or clan" },
			{ "effects", "A collection of all the effects that are on this room" },
			{ "light", "The current light level of this location at ground level, in lux" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Location, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public override IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "characters":
				returnVar = new CollectionVariable(Characters.ToList(), ProgVariableTypes.Character);
				break;
			case "items":
				returnVar = new CollectionVariable(GameItems.ToList(), ProgVariableTypes.Item);
				break;
			case "surrounds":
				returnVar = new CollectionVariable(Surrounds.ToList(), ProgVariableTypes.Location);
				break;
			case "exits":
				returnVar = new CollectionVariable(
					Gameworld.ExitManager.GetExitsFor(this, CurrentOverlay).ToList(), ProgVariableTypes.Exit);
				break;
			case "zone":
				returnVar = Room.Zone;
				break;
			case "shard":
				returnVar = Room.Shard;
				break;
			case "terrain":
				returnVar = CurrentOverlay.Terrain;
				break;
			case "windlevel":
				returnVar = new NumberVariable((int)(CurrentWeather(null)?.Wind ?? 0));
				break;
			case "rainlevel":
				var weather = CurrentWeather(null);
				returnVar = weather?.Precipitation.IsRaining() != true
					? new NumberVariable(0)
					: new NumberVariable(
						(int)((weather.Precipitation == PrecipitationLevel.Sleet
							? PrecipitationLevel.Rain
							: weather.Precipitation) - PrecipitationLevel.Humid));
				break;
			case "snowlevel":
				weather = CurrentWeather(null);
				returnVar = weather?.Precipitation.IsSnowing() != true
					? new NumberVariable(0)
					: new NumberVariable(
						(weather.Precipitation == PrecipitationLevel.Sleet
							? PrecipitationLevel.LightSnow
							: weather.Precipitation) - PrecipitationLevel.TorrentialRain);
				break;
			case "temperature":
				returnVar = new NumberVariable(
					CurrentTemperature(null) * Gameworld.UnitManager.BaseTemperatureToCelcius);
				break;
			case "temperaturec":
				returnVar = new NumberVariable(
					CurrentTemperature(null) * Gameworld.UnitManager.BaseTemperatureToCelcius);
				break;
			case "temperaturef":
				returnVar = new NumberVariable(CurrentTemperature(null) *
					Gameworld.UnitManager.BaseTemperatureToCelcius * 9.0 / 5.0 + 32.0);
				break;
			case "temperaturek":
				returnVar = new NumberVariable(
					CurrentTemperature(null) * Gameworld.UnitManager.BaseTemperatureToCelcius - 273.15);
				break;
			case "wind":
				weather = CurrentWeather(null);
				returnVar = new TextVariable(weather?.Wind.Describe() ?? "None");
				break;
			case "rain":
				weather = CurrentWeather(null);
				returnVar = weather?.Precipitation.IsRaining() == true
					? new TextVariable(weather.Precipitation.Describe())
					: new TextVariable("None");
				break;
			case "snow":
				weather = CurrentWeather(null);
				returnVar = weather?.Precipitation.IsSnowing() == true
					? new TextVariable(weather.Precipitation.Describe())
					: new TextVariable("None");
				break;
			case "precipitation":
				weather = CurrentWeather(null);
				returnVar = new TextVariable(weather?.Precipitation.Describe() ?? "None");
				break;
			case "weather":
				return CurrentWeather(null);
			case "tags":
				return new CollectionVariable(Tags.Select(x => new TextVariable(x.Name)).ToList(),
					ProgVariableTypes.Text);
			case "addedlight":
				return new NumberVariable(CurrentOverlay.AddedLight);
			case "addedlightfactor":
				return new NumberVariable(CurrentOverlay.AmbientLightFactor);
			case "atmosphereid":
				return new NumberVariable(Atmosphere?.Id ?? 0);
			case "atmosphere":
				return new TextVariable(Atmosphere?.Name ?? "None");
			case "atmospheretype":
				return new TextVariable(Atmosphere is null ? "None" : Atmosphere is IGas ? "Gas" : "Liquid");
			case "outdoors":
				return new NumberVariable((int)CurrentOverlay.OutdoorsType);
			case "overlay":
				return CurrentOverlay.Package;
			case "overlays":
				return new CollectionVariable(Overlays.Select(x => x.Package).Distinct().ToList(),
					ProgVariableTypes.OverlayPackage);
			case "grids":
				return new CollectionVariable(
					Gameworld.Grids.Where(x => x.Locations.Contains(this)).Select(x => new NumberVariable(x.Id))
					         .ToList(), ProgVariableTypes.Number);
			case "electricgrids":
				return new CollectionVariable(
					Gameworld.Grids.Where(x => x is IElectricalGrid && x.Locations.Contains(this))
					         .Select(x => new NumberVariable(x.Id)).ToList(), ProgVariableTypes.Number);
			case "light":
				return new NumberVariable(CurrentIllumination(null));
			default:
				return base.GetProperty(property);
		}

		return returnVar;
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Location;

	#endregion
}