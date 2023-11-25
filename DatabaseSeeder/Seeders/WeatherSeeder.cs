using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Antlr.Runtime;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

enum WeatherEventVariation
{
	Freezing,
	VeryCold,
	Cold,
	Cool,
	Cooler,
	Normal,
	Warmer,
	Warm,
	Hot,
	VeryHot,
	Sweltering
}

enum WindVariation
{
	Normal,
	Polar,
	Equatorial
}

enum CloudVariation
{
	None,
	Cloudy,
	Overcast
}

public class WeatherSeeder: IDatabaseSeeder
{
	static Regex TempVariationRegex = new("(?:Freezing|VeryCold|Cold|Cool|Cooler|Warmer|Warm|Hot|VeryHot|Sweltering)$");
	static Regex CloudVariationRegex = new("^(?:Cloudy|Overcast)");
	#region Implementation of IDatabaseSeeder

	/// <inheritdoc />
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
	=> new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
		Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
	{
		(
			"rain",
			@"The engine can set up the rain events to soak items and create puddles. This will effectively have the engine automatically add water contamination to items left outside, fill up open containers, and create puddles around things.

Please be aware that both of these options do add to the performance load on the engine, which may be of concern if you have or intend to have a very large world or running your MUD on a strict budget. The main impact is that during rain events there will be a lot more items than usual trying to save themselves. This can be changed later in bulk if you do run into trouble but you may know up front what you want to do.

The possible configurations are as follows:

	#3full#F: Rain events soak items, fill up containers and create puddles on the ground
	#3soak#F: Rain events soak item and fill up containers, but don't create puddles
	#3none#F: Rain events only have temperature and flavour impacts, but don't impact the world

Please answer #3full#F, #3soak#F or #3none#f. ",
			(context, answers) => true, (text, context) =>
			{
				if (!text.EqualToAny("full", "soak", "none")) return (false, "Please answer #3full#F, #3soak#F or #3none#f.");

				return (true, string.Empty);
			}
		),
	};

	/// <inheritdoc />
	public int SortOrder => 300;

	/// <inheritdoc />
	public string Name => "Weather Seeder";

	/// <inheritdoc />
	public string Tagline => "Sets up Weather and Seasons";

	/// <inheritdoc />
	public string FullDescription => @"This seeder will set up the core components of the weather and climate system in your MUD, including seasons. Note that this assumes terrestrial or earth-like weather. 

Once you have installed this seeder you will need to add the WeatherControllers it installs onto your zones yourself.

At the present time, this seeder only installs a temperate oceanic climate (e.g. Western Europe or Pacific Northwest US)";

	public bool Enabled => true;

	public FuturemudDatabaseContext _context;
	public IReadOnlyDictionary<string, string> _questionAnswers;
	private bool UseRainEvents { get; set; }
	private Liquid? RainLiquid { get; set; }

	private void CreateWeatherEvent(FuturemudDatabaseContext context, PrecipitationLevel precipitation, WindLevel wind, Dictionary<WeatherEvent, string> defaultTransitions, Dictionary<string, WeatherEvent> events)
	{
		void AddEvent(string name, string description, string roomAddendum, double temp, double precipTemp, double windTemp, PrecipitationLevel precipitation, WindLevel wind, bool obscureSky, bool night, bool dawn, bool morning, bool afternoon, bool dusk, string? countsAs, string defaultTransition)
		{
			var weatherEvent = new WeatherEvent
			{
				Name = name,
				WeatherDescription = description,
				WeatherRoomAddendum = roomAddendum,
				TemperatureEffect = temp,
				PrecipitationTemperatureEffect = precipTemp,
				WindTemperatureEffect = windTemp,
				Precipitation = (int)precipitation,
				Wind = (int)wind,
				ObscuresViewOfSky = obscureSky,
				PermittedAtNight = night,
				PermittedAtDawn = dawn,
				PermittedAtMorning = morning,
				PermittedAtAfternoon = afternoon,
				PermittedAtDusk = dusk,
				WeatherEventType = UseRainEvents ? (precipitation.IsRaining() ? "rain" : "simple") : "simple",
				AdditionalInfo = "" // This is set later
			};
			if (countsAs is not null)
			{
				weatherEvent.CountsAs = events[countsAs];
			}

			defaultTransitions[weatherEvent] = defaultTransition;
			events.Add(name, weatherEvent);
			_context.WeatherEvents.Add(weatherEvent);
		}

		void AddEventWithTempVariations(string name, string description, string roomAddendum, double temp, double precipTemp, double windTemp, double tempPerVariation, double precipTempPerVariation, double windTempPerVariation, PrecipitationLevel precipitation, WindLevel wind, bool obscureSky, bool night, bool dawn, bool morning, bool afternoon, bool dusk, string defaultTransition)
		{
			AddEvent(name, description, roomAddendum, temp, precipTemp, windTemp, precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, null, defaultTransition);
			AddEvent($"{name}Warmer", description, roomAddendum, temp + (1 * tempPerVariation), precipTemp + (1 * precipTempPerVariation), windTemp + (1 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Warm", description, roomAddendum, temp + (2 * tempPerVariation), precipTemp + (2 * precipTempPerVariation), windTemp + (2 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Hot", description, roomAddendum, temp + (3 * tempPerVariation), precipTemp + (3 * precipTempPerVariation), windTemp + (3 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}VeryHot", description, roomAddendum, temp + (4 * tempPerVariation), precipTemp + (4 * precipTempPerVariation), windTemp + (4 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Sweltering", description, roomAddendum, temp + (5 * tempPerVariation), precipTemp + (5 * precipTempPerVariation), windTemp + (5 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Cooler", description, roomAddendum, temp - (1 * tempPerVariation), precipTemp - (1 * precipTempPerVariation), windTemp - (1 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Cool", description, roomAddendum, temp - (2 * tempPerVariation), precipTemp - (2 * precipTempPerVariation), windTemp - (2 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Cold", description, roomAddendum, temp - (3 * tempPerVariation), precipTemp - (3 * precipTempPerVariation), windTemp - (3 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}VeryCold", description, roomAddendum, temp - (4 * tempPerVariation), precipTemp - (4 * precipTempPerVariation), windTemp - (4 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
			AddEvent($"{name}Freezing", description, roomAddendum, temp - (5 * tempPerVariation), precipTemp - (5 * precipTempPerVariation), windTemp - (5 * windTempPerVariation), precipitation, wind, obscureSky, night, dawn, morning, afternoon, dusk, name, defaultTransition);
		}

		var precipitationTempDeltaPerVariation = 0.0;
		var precipitationTempDelta = 0.0;
		var precipitationDescription = "";
		var precipitationDescriptionCloudy = "";
		var obscureSky = true;
		ANSIColour textColour = Telnet.Yellow;
		switch (precipitation)
		{
			case PrecipitationLevel.Parched:
				precipitationDescription = "The air is extremely dry.";
				obscureSky = false;
				textColour = Telnet.Orange;
				break;
			case PrecipitationLevel.Dry:
				precipitationDescription = "The air is dry.";
				precipitationDescriptionCloudy = "Light, whispy clouds are dotted about the sky.";
				precipitationTempDelta = 0.1;
				precipitationTempDeltaPerVariation = 0.1;
				obscureSky = false;
				break;
			case PrecipitationLevel.Humid:
				precipitationDescription = "The air is humid.";
				precipitationDescriptionCloudy = "A uniform blanket of cloud covers the sky.";
				precipitationTempDelta = 0.5;
				precipitationTempDeltaPerVariation = 0.2;
				obscureSky = false;
				break;
			case PrecipitationLevel.LightRain:
				precipitationDescription = "A drizzle of rain falls from the sky.";
				precipitationTempDelta = -2.0;
				textColour = Telnet.Cyan;
				break;
			case PrecipitationLevel.Rain:
				precipitationDescription = "Rain falls in steady sheets from the sky.";
				precipitationTempDelta = -3.0;
				textColour = Telnet.Cyan;
				break;
			case PrecipitationLevel.HeavyRain:
				precipitationDescription = "Rain is bucketing down from the sky.";
				precipitationTempDelta = -4.0;
				textColour = Telnet.Cyan;
				break;
			case PrecipitationLevel.TorrentialRain:
				precipitationDescription = "A torrent of rain falls from the sky.";
				precipitationTempDelta = -5.0;
				textColour = Telnet.Cyan;
				break;
			case PrecipitationLevel.LightSnow:
				precipitationDescription = "Snowflakes drift down from the clouds overhead in a light snow flurry.";
				precipitationTempDelta = -4.0;
				textColour = Telnet.BoldCyan;
				break;
			case PrecipitationLevel.Snow:
				precipitationDescription = "Snow falls with steady regularity from the clouds overhead.";
				precipitationTempDelta = -6.0;
				textColour = Telnet.BoldCyan;
				break;
			case PrecipitationLevel.HeavySnow:
				precipitationDescription = "A heavy amount of snow falls from the dark clouds overhead, blanketing the area in snow.";
				precipitationTempDelta = -8.0;
				textColour = Telnet.BoldCyan;
				break;
			case PrecipitationLevel.Blizzard:
				precipitationDescription = "A blizzard of snow blankets the area in white, restricting visibility of much of anything.";
				precipitationTempDelta = -10.0;
				textColour = Telnet.BoldWhite;
				break;
			case PrecipitationLevel.Sleet:
				precipitationDescription = "Icy rain sleets down here, forming slushy puddles on the ground.";
				precipitationTempDelta = -6.0;
				textColour = Telnet.BoldCyan;
				break;
		}

		var windTempDeltaHot = 0.0;
		var windTempDeltaCold = 0.0;
		var windTempDelta = 0.0;
		var windDescription = "";

		switch (wind)
		{
			case WindLevel.None:
				windDescription = " There air is completely still, with almost no movement of air";
				break;
			case WindLevel.Still:
				windDescription = " There isn't any wind to speak of";
				windTempDelta = -0.2;
				windTempDeltaHot = 0.0;
				windTempDeltaCold = -0.4;
				break;
			case WindLevel.OccasionalBreeze:
				windDescription = " There is only an occasional {0}breeze";
				windTempDelta = -0.8;
				windTempDeltaHot = 0.5;
				windTempDeltaCold = -2.0;
				break;
			case WindLevel.Breeze:
				windDescription = " There is a steady {0}breeze blowing through";
				windTempDelta = -1.8;
				windTempDeltaHot = 1.2;
				windTempDeltaCold = -3.5;
				break;
			case WindLevel.Wind:
				windDescription = " There is a consistent {0}wind blowing";
				windTempDelta = -2.5;
				windTempDeltaHot = 2.0;
				windTempDeltaCold = -5.0;
				break;
			case WindLevel.StrongWind:
				windDescription = " There is a strong {0}wind blowing";
				windTempDelta = -3.5;
				windTempDeltaHot = 3.0;
				windTempDeltaCold = -7.0;
				break;
			case WindLevel.GaleWind:
				windDescription = " There is a {0}gale-force wind blowing";
				windTempDelta = -5.0;
				windTempDeltaHot = 4.0;
				windTempDeltaCold = -10.0;
				break;
			case WindLevel.HurricaneWind:
				windDescription = " There is a {0}hurricane-force wind blowing";
				windTempDelta = -7.0;
				windTempDeltaHot = 5.0;
				windTempDeltaCold = -13.0;
				break;
			case WindLevel.MaelstromWind:
				windDescription = " There is a {0}maelstrom of wind blowing";
				windTempDelta = -8.0;
				windTempDeltaHot = 6.0;
				windTempDeltaCold = -16.0;
				break;
		}

		AddEventWithTempVariations($"{precipitation.DescribeEnum()}{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "")}");
		if (wind > WindLevel.Still)
		{
			AddEventWithTempVariations($"{precipitation.DescribeEnum()}Equatorial{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "hot ")}");
			AddEventWithTempVariations($"{precipitation.DescribeEnum()}Polar{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "cold ")}");
		}
		if (precipitation == PrecipitationLevel.Humid)
		{
			AddEventWithTempVariations($"Overcast{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}");
			if (wind > WindLevel.Still)
			{
				AddEventWithTempVariations($"OvercastEquatorial{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}");
				AddEventWithTempVariations($"OvercastPolar{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}");
			}	
		}
		if (precipitation == PrecipitationLevel.Dry)
		{
			AddEventWithTempVariations($"Cloudy{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}");
			if (wind > WindLevel.Still)
			{
				AddEventWithTempVariations($"CloudyEquatorial{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}");
				AddEventWithTempVariations($"CloudyPolar{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}");
			}
		}
	}

	/// <inheritdoc />
	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_questionAnswers = questionAnswers;

		var celestial = _context.Celestials.FirstOrDefault();
		if (celestial is null)
		{
			return "Could not proceed because there was not a celestial object. No data was seeded.";
		}

		switch (_questionAnswers["rain"].ToLowerInvariant())
		{
			case "full":
				UseRainEvents = true;
				RainLiquid = context.Liquids.First(x => x.Name == "rain water");
				break;
			case "soak":
				UseRainEvents = true;
				var dbsetting = context.StaticConfigurations.Find("PuddlesEnabled");
				if (dbsetting is null)
				{
					dbsetting = new StaticConfiguration { SettingName = "PuddlesEnabled", Definition = "" };
					context.StaticConfigurations.Add(dbsetting);
				}
				dbsetting.Definition = "false";
				context.SaveChanges();
				RainLiquid = context.Liquids.First(x => x.Name == "rain water");
				break;
			case "none":
				UseRainEvents = false;
				break;
		}

		List<Season> seasons = CreateSeasons(celestial);
		Dictionary<string, WeatherEvent> events = CreateWeatherEvents(context);
		ClimateModel temperateModel = CreateClimateModels(context, seasons, events);
		CreateRegionalClimates(context, seasons, temperateModel);

		return string.Empty;
	}

	private static void CreateRegionalClimates(FuturemudDatabaseContext context, List<Season> seasons, ClimateModel temperateModel)
	{
		#region Regional Climate
		var regionalClimateNH = new RegionalClimate
		{
			Name = "Oceanic Temperate Northern Hemisphere",
			ClimateModelId = temperateModel.Id
		};
		context.RegionalClimates.Add(regionalClimateNH);
		var regionalClimateSH = new RegionalClimate
		{
			Name = "Oceanic Temperate Southern Hemisphere",
			ClimateModelId = temperateModel.Id
		};
		context.RegionalClimates.Add(regionalClimateSH);

		XElement CreateDailyTemperatures(double minimum, double maximum)
		{
			var temps = new List<(int Hour, double Temp)>();
			var delta = maximum - minimum;
			for (var i = 0; i < 24; i++)
			{
				temps.Add((i, minimum + delta * i switch
				{
					0 => 0.0866,
					1 => 0.0472,
					2 => 0.0236,
					3 => 0.0079,
					4 => 0.0,
					5 => 0.0315,
					6 => 0.126,
					7 => 0.2835,
					8 => 0.3622,
					9 => 0.4409,
					10 => 0.5197,
					11 => 0.5984,
					12 => 0.6772,
					13 => 0.7559,
					14 => 0.8346,
					15 => 0.8740,
					16 => 0.9134,
					17 => 0.9370,
					18 => 0.9685,
					19 => 1.0,
					20 => 0.8346,
					21 => 0.6772,
					22 => 0.3622,
					23 => 0.2047,
					_ => minimum
				}));
			}

			return new XElement("Temperatures",
				from temp in temps
				select new XElement("Value", new XAttribute("hour", temp.Hour), temp.Temp)
			);
		}

		foreach (var season in seasons)
		{
			var rs = new RegionalClimatesSeason
			{
				RegionalClimate = season.Name.EndsWith("_south") ? regionalClimateSH : regionalClimateNH,
				Season = season,
				TemperatureInfo = CreateDailyTemperatures(
							season.DisplayName switch
							{
								"Early Winter" => 3.0,
								"Mid Winter" => 2.0,
								"Late Winter" => 2.0,
								"Early Spring" => 4.0,
								"Mid Spring" => 6.0,
								"Late Spring" => 9.0,
								"Early Summer" => 12.0,
								"Mid Summer" => 14.0,
								"Late Summer" => 14.0,
								"Early Autumn" => 11.0,
								"Mid Autumn" => 8.0,
								"Late Autumn" => 3.0,
								_ => 0.0
							},
							season.DisplayName switch
							{
								"Early Winter" => 8.0,
								"Mid Winter" => 8.0,
								"Late Winter" => 8.0,
								"Early Spring" => 11.0,
								"Mid Spring" => 14.0,
								"Late Spring" => 18.0,
								"Early Summer" => 21.0,
								"Mid Summer" => 24.0,
								"Late Summer" => 23.0,
								"Early Autumn" => 20.0,
								"Mid Autumn" => 16.0,
								"Late Autumn" => 10.0,
								_ => 0.0
							}
						).ToString()
			};
			context.RegionalClimatesSeasons.Add(rs);
		}
		context.SaveChanges();
		#endregion
	}

	private static ClimateModel CreateClimateModels(FuturemudDatabaseContext context, List<Season> seasons, Dictionary<string, WeatherEvent> events)
	{
		#region Climate Models

		var eventsAndTransitionsBySeason = new Dictionary<string, CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>>();
		eventsAndTransitionsBySeason["Winter"] = new();
		eventsAndTransitionsBySeason["Spring"] = new();
		eventsAndTransitionsBySeason["Summer"] = new();
		eventsAndTransitionsBySeason["Autumn"] = new();

		var seasonalNormalWind = new Dictionary<string, WindLevel>
		{
			{ "Winter", WindLevel.Still },
			{ "Autumn", WindLevel.Breeze },
			{ "Spring", WindLevel.OccasionalBreeze },
			{ "Summer", WindLevel.Still }
		};

		foreach (var we in events)
		{
			var wind = (WindLevel)we.Value.Wind;
			var precip = (PrecipitationLevel)we.Value.Precipitation;
			var match = TempVariationRegex.Match(we.Key);
			WeatherEventVariation variation = WeatherEventVariation.Normal;
			if (match.Success)
			{
				switch (match.Value)
				{
					case "Freezing":
						variation = WeatherEventVariation.Freezing;
						break;
					case "VeryCold":
						variation = WeatherEventVariation.VeryCold;
						break;
					case "Cold":
						variation = WeatherEventVariation.Cold;
						break;
					case "Cool":
						variation = WeatherEventVariation.Cool;
						break;
					case "Cooler":
						variation = WeatherEventVariation.Cooler;
						break;
					case "Warmer":
						variation = WeatherEventVariation.Warmer;
						break;
					case "Warm":
						variation = WeatherEventVariation.Warm;
						break;
					case "Hot":
						variation = WeatherEventVariation.Hot;
						break;
					case "VeryHot":
						variation = WeatherEventVariation.VeryHot;
						break;
					case "Sweltering":
						variation = WeatherEventVariation.Sweltering;
						break;
				}
			}

			var windVariation = we.Key.Contains("Polar") ? WindVariation.Polar :
				(we.Key.Contains("Equatorial") ? WindVariation.Equatorial : WindVariation.Normal);
			var cloudVariation = CloudVariationRegex.IsMatch(we.Key) ?
				(we.Key.StartsWith("Overcast") ? CloudVariation.Overcast : CloudVariation.Cloudy) : CloudVariation.None;

			WeatherEvent to;

			// 1 up or down on wind type
			// 1 up on wind type
			if (wind < WindLevel.MaelstromWind)
			{
				to = events[$"{cloudVariation switch
				{
					CloudVariation.Overcast => "Overcast",
					CloudVariation.Cloudy => "Cloudy",
					_ => precip.DescribeEnum()
				}}{windVariation switch
				{
					WindVariation.Polar => "Polar",
					WindVariation.Equatorial => "Equatorial",
					_ => ""
				}}{wind.StageUp().DescribeEnum()}{variation switch
				{
					WeatherEventVariation.Normal => "",
					_ => variation.DescribeEnum()
				}}"];
				eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 10.0 - Math.Abs(wind.StepsFrom(seasonalNormalWind["Winter"])))));
				eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 12.0 - Math.Abs(wind.StepsFrom(seasonalNormalWind["Autumn"])))));
				eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 8.0 - Math.Abs(wind.StepsFrom(seasonalNormalWind["Summer"])))));
				eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 10.0 - Math.Abs(wind.StepsFrom(seasonalNormalWind["Spring"])))));
			}

			// 1 down on wind type
			if (wind > WindLevel.None)
			{
				to = events[$"{cloudVariation switch
				{
					CloudVariation.Overcast => "Overcast",
					CloudVariation.Cloudy => "Cloudy",
					_ => precip.DescribeEnum()
				}}{wind switch
				{
					WindLevel.OccasionalBreeze => "",
					WindLevel.Still => "",
					_ => windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}
				}}{wind.StageDown().DescribeEnum()}{variation switch
				{
					WeatherEventVariation.Normal => "",
					_ => variation.DescribeEnum()
				}}"];
				eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(wind.StepsFrom(seasonalNormalWind["Winter"])))));
				eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 5.0 + Math.Abs(wind.StepsFrom(seasonalNormalWind["Autumn"])))));
				eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(wind.StepsFrom(seasonalNormalWind["Summer"])))));
				eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(wind.StepsFrom(seasonalNormalWind["Spring"])))));
			}

			// 1 up or down on precipitation
			// 1 up on precipitation
			switch (cloudVariation)
			{
				case CloudVariation.None:
					if (precip == PrecipitationLevel.TorrentialRain || precip == PrecipitationLevel.Blizzard || precip == PrecipitationLevel.Sleet)
					{
						break;
					}

					// Increase 1 level
					to = events[$"{precip.StageUp().DescribeEnum()}{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 10.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 12.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 7.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 6.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));

					if (precip == PrecipitationLevel.HeavyRain || precip == PrecipitationLevel.HeavySnow)
					{
						break;
					}

					// Increase 2 levels
					to = events[$"{precip.StageUp(2).DescribeEnum()}{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(0.2, 4.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(0.2, 7.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(0.2, 3.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(0.2, 3.0 - Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					break;
				case CloudVariation.Cloudy:
					// Become overcast instead
					to = events[$"Overcast{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));

					// Become humid instead
					to = events[$"Humid{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 1.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 3.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
					break;
				case CloudVariation.Overcast:
					// Start raining lightly
					to = events[$"LightRain{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 15.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 3.0));

					// Start raining
					to = events[$"Rain{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 7.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 7.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 0.5));

					// Start snowing
					to = events[$"LightSnow{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 2.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 0.1));
					break;
			}

			// 1 down on precipitation
			switch (cloudVariation)
			{
				case CloudVariation.None:
					if (precip == PrecipitationLevel.Parched)
					{
						break;
					}

					// Handle snow and sleet separately as they're not sequential on the enum
					if (precip == PrecipitationLevel.LightSnow || precip == PrecipitationLevel.Sleet || precip == PrecipitationLevel.LightRain)
					{
						to = events[$"Overcast{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 5.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 2.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));

						to = events[$"Humid{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 5.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 2.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					}
					else
					{
						to = events[$"{precip.StageDown().DescribeEnum()}{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 5.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 2.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					}

					if (precip == PrecipitationLevel.Dry)
					{
						break;
					}

					// Decrease 2 levels
					// Handle snow and sleet separately as they're not sequential on the enum
					if (precip == PrecipitationLevel.LightSnow || precip == PrecipitationLevel.Sleet || precip == PrecipitationLevel.LightRain)
					{
						to = events[$"Cloudy{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(0.2, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 2.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));

						to = events[$"Dry{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 2.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					}
					else if (precip == PrecipitationLevel.Snow || precip == PrecipitationLevel.Rain)
					{
						to = events[$"Overcast{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 2.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));

						to = events[$"Humid{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 2.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 0.5 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					}
					else
					{
						to = events[$"{precip.StageDown().DescribeEnum()}{windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];

						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, Math.Max(1.0, 3.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, Math.Max(1.0, 5.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, Math.Max(1.0, 2.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Dry)))));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, Math.Max(1.0, 1.0 + Math.Abs(precip.StepsFrom(PrecipitationLevel.Humid)))));
					}
					break;
				case CloudVariation.Cloudy:
					// Become overcast instead
					to = events[$"Overcast{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));

					// Become humid instead
					to = events[$"Humid{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 1.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 3.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
					break;
				case CloudVariation.Overcast:
					// Start raining lightly
					to = events[$"LightRain{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 15.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 3.0));

					// Start raining
					to = events[$"Rain{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 7.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 7.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 0.5));

					// Start snowing
					to = events[$"LightSnow{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 2.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 0.1));

					// Start sleeting
					to = events[$"Sleet{windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 2.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 0.1));
					break;
			}

			// 1 up or down on temp variation
			// 1 up on temp variation
			if (variation < WeatherEventVariation.Sweltering)
			{
				to = events[$"{cloudVariation switch
				{
					CloudVariation.Overcast => "Overcast",
					CloudVariation.Cloudy => "Cloudy",
					_ => precip.DescribeEnum()
				}}{wind switch
				{
					WindLevel.OccasionalBreeze => "",
					WindLevel.Still => "",
					_ => windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}
				}}{wind.DescribeEnum()}{variation switch
				{
					WeatherEventVariation.Normal => "",
					WeatherEventVariation.Cooler => "",
					_ => variation.StageUp().DescribeEnum()
				}}"];
				eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
				eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
				eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
				eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
			}

			// 1 down on temp variation
			if (variation > WeatherEventVariation.Freezing)
			{
				to = events[$"{cloudVariation switch
				{
					CloudVariation.Overcast => "Overcast",
					CloudVariation.Cloudy => "Cloudy",
					_ => precip.DescribeEnum()
				}}{wind switch
				{
					WindLevel.OccasionalBreeze => "",
					WindLevel.Still => "",
					_ => windVariation switch
					{
						WindVariation.Polar => "Polar",
						WindVariation.Equatorial => "Equatorial",
						_ => ""
					}
				}}{wind.DescribeEnum()}{variation switch
				{
					WeatherEventVariation.Normal => "",
					WeatherEventVariation.Warmer => "",
					_ => variation.StageDown().DescribeEnum()
				}}"];
				eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
				eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
				eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
				eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
			}

			// Wind variation change
			if (wind > WindLevel.OccasionalBreeze)
			{
				switch (windVariation)
				{
					case WindVariation.Normal:
						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}Polar{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));

						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}Equatorial{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
						break;
					case WindVariation.Polar:
						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));

						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}Equatorial{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
						break;
					case WindVariation.Equatorial:
						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}Polar{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));

						to = events[$"{cloudVariation switch
						{
							CloudVariation.Overcast => "Overcast",
							CloudVariation.Cloudy => "Cloudy",
							_ => precip.DescribeEnum()
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
						break;
				}

			}

			// Cloud variation change
			switch (cloudVariation)
			{
				case CloudVariation.None:
					if (precip > PrecipitationLevel.Humid || precip == PrecipitationLevel.Parched)
					{
						break;
					}

					if (precip == PrecipitationLevel.Humid)
					{
						to = events[$"Overcast{wind switch
						{
							WindLevel.OccasionalBreeze => "",
							WindLevel.Still => "",
							_ => windVariation switch
							{
								WindVariation.Polar => "Polar",
								WindVariation.Equatorial => "Equatorial",
								_ => ""
							}
						}}{wind.DescribeEnum()}{variation switch
						{
							WeatherEventVariation.Normal => "",
							_ => variation.DescribeEnum()
						}}"];
						eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
						eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
						eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
						eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
						break;
					}

					to = events[$"Cloudy{wind switch
					{
						WindLevel.OccasionalBreeze => "",
						WindLevel.Still => "",
						_ => windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
					break;

				case CloudVariation.Cloudy:
				case CloudVariation.Overcast:
					to = events[$"{precip.DescribeEnum()}{wind switch
					{
						WindLevel.OccasionalBreeze => "",
						WindLevel.Still => "",
						_ => windVariation switch
						{
							WindVariation.Polar => "Polar",
							WindVariation.Equatorial => "Equatorial",
							_ => ""
						}
					}}{wind.DescribeEnum()}{variation switch
					{
						WeatherEventVariation.Normal => "",
						_ => variation.DescribeEnum()
					}}"];
					eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 5.0));
					eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 10.0));
					eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
					eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 5.0));
					break;
			}

		}

		var temperateModel = new ClimateModel
		{
			Name = "Temperate",
			MinuteProcessingInterval = 60,
			MinimumMinutesBetweenFlavourEchoes = 60,
			MinuteFlavourEchoChance = 0.01,
			Type = "terrestrial"
		};
		foreach (var season in seasons)
		{
			var cms = new ClimateModelSeason
			{
				ClimateModel = temperateModel,
				Season = season,
				IncrementalAdditionalChangeChanceFromStableWeather = 0.0005,
				MaximumAdditionalChangeChanceFromStableWeather = season.SeasonGroup switch
				{
					"Autumn" => 0.15,
					"Spring" => 0.10,
					_ => 0.05
				}
			};
			temperateModel.ClimateModelSeasons.Add(cms);

			foreach (var we in eventsAndTransitionsBySeason[season.SeasonGroup])
			{
				cms.SeasonEvents.Add(new ClimateModelSeasonEvent
				{
					ClimateModel = temperateModel,
					Season = season,
					WeatherEvent = we.Key,
					ChangeChance = 0.01,
					Transitions =
					new XElement("Transitions",
										from trans in we.Value
										select new XElement("Transition",
											new XAttribute("id", trans.To.Id),
											new XAttribute("chance", trans.Chance)
										)
									).ToString()
				});
			}
		}
		context.ClimateModels.Add(temperateModel);
		context.SaveChanges();

		#endregion
		return temperateModel;
	}

	private Dictionary<string, WeatherEvent> CreateWeatherEvents(FuturemudDatabaseContext context)
	{
		#region Weather Events

		var events = new Dictionary<string, WeatherEvent>(StringComparer.OrdinalIgnoreCase);
		var defaultTransitions = new Dictionary<WeatherEvent, string>();
		var transitions = new CollectionDictionary<WeatherEvent, (WeatherEvent, string)>();
		var echoes = new CollectionDictionary<WeatherEvent, string>();

		foreach (var precipitation in Enum.GetValues<PrecipitationLevel>())
		{
			foreach (var wind in Enum.GetValues<WindLevel>())
			{
				CreateWeatherEvent(context, precipitation, wind, defaultTransitions, events);
			}
		}

		_context.SaveChanges();

		foreach (var we in events)
		{
			switch ((PrecipitationLevel)we.Value.Precipitation)
			{
				case PrecipitationLevel.LightRain:
					echoes.Add(we.Value, "A light rain falls from the clouds above.");
					break;
				case PrecipitationLevel.Rain:
					echoes.Add(we.Value, "Steady sheets of rain fall from the clouds above.");
					break;
				case PrecipitationLevel.HeavyRain:
					echoes.Add(we.Value, "A heavy rain falls from the clouds above.");
					break;
				case PrecipitationLevel.TorrentialRain:
					echoes.Add(we.Value, "A torrential rain falls from the clouds above.");
					break;
				case PrecipitationLevel.LightSnow:
					echoes.Add(we.Value, "A light sprinkle of snow falls from the clouds above.");
					break;
				case PrecipitationLevel.Snow:
					echoes.Add(we.Value, "A steady snow falls from the clouds above.");
					break;
				case PrecipitationLevel.HeavySnow:
					echoes.Add(we.Value, "A heavy snow falls from the clouds above.");
					break;
				case PrecipitationLevel.Blizzard:
					echoes.Add(we.Value, "A blizzard of snow falls from the clouds above, blanketing the world in white.");
					break;
				case PrecipitationLevel.Sleet:
					echoes.Add(we.Value, "A miserable sleet falls from the clouds above.");
					break;
			}

			switch ((WindLevel)we.Value.Wind)
			{
				case WindLevel.OccasionalBreeze:
					echoes.Add(we.Value, "A light breeze briefly blows through the area.");
					break;
				case WindLevel.Breeze:
					echoes.Add(we.Value, "A gust of wind briefly blows through the area.");
					break;
				case WindLevel.Wind:
					echoes.Add(we.Value, "A strong gust of wind punctuates the already steadily windy weather.");
					break;
				case WindLevel.StrongWind:
					echoes.Add(we.Value, "A sudden, extremely strong gust of wind punctuates the already very windy weather.");
					break;
				case WindLevel.GaleWind:
					echoes.Add(we.Value, "Non-stop gusts of extremely strong wind buffet the area.");
					break;
				case WindLevel.HurricaneWind:
					echoes.Add(we.Value, "Hurricane strength winds buffet the area.");
					break;
				case WindLevel.MaelstromWind:
					echoes.Add(we.Value, "A veritable maelstrom of wind threatens to strip down everything not anchored to the ground.");
					break;
			}

			we.Value.AdditionalInfo = 
				UseRainEvents && ((PrecipitationLevel)we.Value.Precipitation).IsRaining() ?
				new XElement("Event",
					new XElement("Liquid", RainLiquid!.Id),
					new XElement("Echoes",
						from echo in echoes[we.Value]
						select new XElement("Echo", new XCData(echo))
					),
					new XElement("Transitions",
						new XElement("Default", new XCData(defaultTransitions[we.Value]))
					)
				).ToString() :
			new XElement("Event",
					new XElement("Echoes",
						from echo in echoes[we.Value]
						select new XElement("Echo", new XCData(echo))
					),
					new XElement("Transitions",
						new XElement("Default", new XCData(defaultTransitions[we.Value]))
					)
				).ToString();
		}

		context.SaveChanges();
		#endregion
		return events;
	}

	private List<Season> CreateSeasons(Celestial? celestial)
	{
		#region Seasons

		var seasons = new List<Season>();

		void AddSeason(string name, string group, string displayName, int celestialDayOnset)
		{
			var season = new Season
			{
				Name = name,
				DisplayName = displayName,
				SeasonGroup = group,
				CelestialId = celestial.Id,
				CelestialDayOnset = celestialDayOnset
			};
			_context.Seasons.Add(season);
			seasons.Add(season);
		}

		AddSeason("temperate_early_winter", "Winter", "Early Winter", 334);
		AddSeason("temperate_mid_winter", "Winter", "Mid Winter", 0);
		AddSeason("temperate_late_winter", "Winter", "Late Winter", 31);
		AddSeason("temperate_early_spring", "Spring", "Early Spring", 61);
		AddSeason("temperate_mid_spring", "Spring", "Mid Spring", 92);
		AddSeason("temperate_late_spring", "Spring", "Late Spring", 122);
		AddSeason("temperate_early_summer", "Summer", "Early Summer", 153);
		AddSeason("temperate_mid_summer", "Summer", "Mid Summer", 183);
		AddSeason("temperate_late_summer", "Summer", "Late Summer", 214);
		AddSeason("temperate_early_autumn", "Autumn", "Early Autumn", 244);
		AddSeason("temperate_mid_autumn", "Autumn", "Mid Autumn", 274);
		AddSeason("temperate_late_autumn", "Autumn", "Late Autumn", 304);
		AddSeason("temperate_early_winter_south", "Winter", "Early Winter", 153);
		AddSeason("temperate_mid_winter_south", "Winter", "Mid Winter", 183);
		AddSeason("temperate_late_winter_south", "Winter", "Late Winter", 214);
		AddSeason("temperate_early_spring_south", "Spring", "Early Spring", 244);
		AddSeason("temperate_mid_spring_south", "Spring", "Mid Spring", 274);
		AddSeason("temperate_late_spring_south", "Spring", "Late Spring", 304);
		AddSeason("temperate_early_summer_south", "Summer", "Early Summer", 334);
		AddSeason("temperate_mid_summer_south", "Summer", "Mid Summer", 0);
		AddSeason("temperate_late_summer_south", "Summer", "Late Summer", 31);
		AddSeason("temperate_early_autumn_south", "Autumn", "Early Autumn", 61);
		AddSeason("temperate_mid_autumn_south", "Autumn", "Mid Autumn", 92);
		AddSeason("temperate_late_autumn_south", "Autumn", "Late Autumn", 122);
		_context.SaveChanges();
		#endregion
		return seasons;
	}

	/// <inheritdoc />
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (!context.Celestials.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.ClimateModels.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	#endregion
}
