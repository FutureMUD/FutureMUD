using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders
{
	public class WeatherSeeder: IDatabaseSeeder
	{
		#region Implementation of IDatabaseSeeder

		/// <inheritdoc />
		public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
		};

		/// <inheritdoc />
		public int SortOrder => 7;

		/// <inheritdoc />
		public string Name => "Weather Seeder";

		/// <inheritdoc />
		public string Tagline => "Sets up Weather and Seasons";

		/// <inheritdoc />
		public string FullDescription => @"This seeder will set up the core components of the weather and climate system in your MUD, including seasons. Note that this assumes terrestrial or earth-like weather. 

Once you have installed this seeder you will need to add the WeatherControllers it installs onto your zones yourself.";

		public bool Enabled => false;

		public FuturemudDatabaseContext _context;
		public IReadOnlyDictionary<string, string> _questionAnswers;

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
			switch (precipitation)
			{
				case PrecipitationLevel.Parched:
					precipitationDescription = "The air is extremely dry";
					break;
				case PrecipitationLevel.Dry:
					precipitationDescription = "The air is dry";
					precipitationTempDelta = 0.1;
					precipitationTempDeltaPerVariation = 0.1;
					break;
				case PrecipitationLevel.Humid:
					precipitationDescription = "The air is humid";
					precipitationTempDelta = 0.5;
					precipitationTempDeltaPerVariation = 0.2;
					break;
				case PrecipitationLevel.LightRain:
					precipitationDescription = "A drizzle of rain falls from the sky";
					precipitationTempDelta = -2.0;
					break;
				case PrecipitationLevel.Rain:
					precipitationDescription = "Rain falls in steady sheets from the sky";
					precipitationTempDelta = -3.0;
					break;
				case PrecipitationLevel.HeavyRain:
					precipitationDescription = "Rain is bucketing down from the sky";
					precipitationTempDelta = -4.0;
					break;
				case PrecipitationLevel.TorrentialRain:
					precipitationDescription = "A torrent of rain falls from the sky";
					precipitationTempDelta = -5.0;
					break;
				case PrecipitationLevel.LightSnow:
					precipitationDescription = "Snowflakes drift down from the clouds overhead in a light snow flurry";
					precipitationTempDelta = -4.0;
					break;
				case PrecipitationLevel.Snow:
					precipitationDescription = "Snow falls with steady regularity from the clouds overhead";
					precipitationTempDelta = -6.0;
					break;
				case PrecipitationLevel.HeavySnow:
					precipitationDescription = "A heavy amount of snow falls from the dark clouds overhead, blanketing the area in snow";
					precipitationTempDelta = -8.0;
					break;
				case PrecipitationLevel.Blizzard:
					precipitationDescription = "A blizzard of snow blankets the area in white, restricting visibility of much of anything";
					precipitationTempDelta = -10.0;
					break;
				case PrecipitationLevel.Sleet:
					precipitationDescription = "Icy rain sleets down here, forming slushy puddles on the ground";
					precipitationTempDelta = -6.0;
					break;
			}

			var windTempDeltaHot = 0.0;
			var windTempDeltaCold = 0.0;
			var windTempDelta = 0.0;
			var windDescription = "";

			switch (wind)
			{
				case WindLevel.None:
					windDescription = " and the air is still, with not even a hint of wind";
					break;
				case WindLevel.Still:
					windDescription = " and the air is still";
					windTempDelta = -0.2;
					windTempDeltaHot = 0.0;
					windTempDeltaCold = -0.4;
					break;
				case WindLevel.OccasionalBreeze:
					windDescription = " with only an occasional {0}breeze";
					windTempDelta = -0.8;
					windTempDeltaHot = 0.5;
					windTempDeltaCold = -2.0;
					break;
				case WindLevel.Breeze:
					windDescription = " with a steady {0}breeze blowing through";
					windTempDelta = -1.8;
					windTempDeltaHot = 1.2;
					windTempDeltaCold = -3.5;
					break;
				case WindLevel.Wind:
					windDescription = " with a consistent {0}wind";
					windTempDelta = -2.5;
					windTempDeltaHot = 2.0;
					windTempDeltaCold = -5.0;
					break;
				case WindLevel.StrongWind:
					break;
				case WindLevel.GaleWind:
					break;
				case WindLevel.HurricaneWind:
					break;
				case WindLevel.MaelstromWind:
					break;
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
			_context.SaveChanges();
			#endregion

			#region Weather Events

			var events = new Dictionary<string, WeatherEvent>(StringComparer.OrdinalIgnoreCase);
			var defaultTransitions = new Dictionary<WeatherEvent, string>();
			var transitions = new CollectionDictionary<WeatherEvent, (WeatherEvent, string)>();
			var echoes = new CollectionDictionary<WeatherEvent, string>();

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

			AddEventWithTempVariations(name: "SunnyStill",
							  description: "The air is still and calm, and warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 0.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, still and sunny.");

			AddEventWithTempVariations(name: "NightStill",
							  description: "The air is still and calm, and the night sky is completely clear.",
							  roomAddendum: "#5The air is still and calm, and the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 0.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is still and clear.");

			AddEventWithTempVariations(name: "DawnStill",
							  description: "The air is still and calm, and the early morning sky is completely clear.",
							  roomAddendum: "#GThe air is still and calm, and the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 0.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is still and clear.");

			AddEventWithTempVariations(name: "DuskStill",
							  description: "The air is still and calm, and the late afternoon sky is completely clear.",
							  roomAddendum: "#GThe air is still and calm, and the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 0.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is still and clear.");

			AddEventWithTempVariations(name: "HumidStill",
							  description: "The air is humid and still, and warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BThe air is humid and still, and warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 1.0,
							  windTemp: 0.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 1.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Humid,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, humid and still.");

			AddEventWithTempVariations(name: "NightHumidStill",
							  description: "The air is humid and still, and the night sky is completely clear.",
							  roomAddendum: "#5The air is humid and still, and the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.5,
							  windTemp: 0.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.8,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is humid and still.");

			AddEventWithTempVariations(name: "DawnHumidStill",
							  description: "The air is humid and still, and the early morning sky is completely clear.",
							  roomAddendum: "#GThe air is humid and still, and the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.5,
							  windTemp: 0.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.9,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is humid and still.");

			AddEventWithTempVariations(name: "DuskHumidStill",
							  description: "The air is humid and still, and the late afternoon sky is completely clear.",
							  roomAddendum: "#GThe air is humid and still, and the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 1.0,
							  windTemp: 0.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.9,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Still,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is humid and still.");

			AddEventWithTempVariations(name: "SunnySlightlyBreezy",
							  description: "There is a slight breeze about, and warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BThere is a slight breeze about, and warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -1.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.OccasionalBreeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, breezy and sunny.");

			AddEventWithTempVariations(name: "NightSlightlyBreezy",
							  description: "There is a slight breeze about, and the night sky is completely clear.",
							  roomAddendum: "#5There is a slight breeze about, and the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -1.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.OccasionalBreeze,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is breezy and clear.");

			AddEventWithTempVariations(name: "DawnSlightlyBreezy",
							  description: "There is a slight breeze about, and the early morning sky is completely clear.",
							  roomAddendum: "#GThere is a slight breeze about, and the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -1.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.OccasionalBreeze,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is breezy and clear.");

			AddEventWithTempVariations(name: "DuskSlightlyBreezy",
							  description: "There is a slight breeze about, and the late afternoon sky is completely clear.",
							  roomAddendum: "#GThere is a slight breeze about, and the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -1.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.OccasionalBreeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is breezy and clear.");

			AddEventWithTempVariations(name: "SunnyBreezy",
							  description: "There is a breeze about with occasional strong gusts, and warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BThere is a breeze about with occasional strong gusts, and warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -2.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, breezy and sunny.");

			AddEventWithTempVariations(name: "NightBreezy",
							  description: "There is a breeze about with occasional strong gusts, and the night sky is completely clear.",
							  roomAddendum: "#5There is a breeze about with occasional strong gusts, and the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -2.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is breezy and clear.");

			AddEventWithTempVariations(name: "DawnBreezy",
							  description: "There is a breeze about with occasional strong gusts, and the early morning sky is completely clear.",
							  roomAddendum: "#GThere is a breeze about with occasional strong gusts, and the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -2.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is breezy and clear.");

			AddEventWithTempVariations(name: "DuskBreezy",
							  description: "There is a breeze about with occasional strong gusts, and the late afternoon sky is completely clear.",
							  roomAddendum: "#GThere is a breeze about with occasional strong gusts, and the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -2.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is breezy and clear.");

			AddEventWithTempVariations(name: "SunnyHotBreezy",
							  description: "There is a hot breeze about with occasional strong gusts, and warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BThere is a hot breeze about with occasional strong gusts, and warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 1.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, breezy and sunny.");

			AddEventWithTempVariations(name: "NightHotBreezy",
							  description: "There is a hot breeze about with occasional strong gusts, and the night sky is completely clear.",
							  roomAddendum: "#5There is a hot breeze about with occasional strong gusts, and the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 1,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is breezy and clear.");

			AddEventWithTempVariations(name: "DawnHotBreezy",
							  description: "There is a hot breeze about with occasional strong gusts, and the early morning sky is completely clear.",
							  roomAddendum: "#GThere is a hot breeze about with occasional strong gusts, and the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 1.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is breezy and clear.");

			AddEventWithTempVariations(name: "DuskHotBreezy",
							  description: "There is a hot breeze about with occasional strong gusts, and the late afternoon sky is completely clear.",
							  roomAddendum: "#GThere is a hot breeze about with occasional strong gusts, and the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: 1.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Breeze,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is breezy and clear.");

			AddEventWithTempVariations(name: "SunnyWindy",
							  description: "A wind is blowing about in steady gusts, but warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BA wind is blowing about in steady gusts, but warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, windy and sunny.");

			AddEventWithTempVariations(name: "NightWindy",
							  description: "A wind is blowing about in steady gusts, but the night sky is completely clear.",
							  roomAddendum: "#5A wind is blowing about in steady gusts, but the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is windy and clear.");

			AddEventWithTempVariations(name: "DawnWindy",
							  description: "A wind is blowing about in steady gusts, but the early morning sky is completely clear.",
							  roomAddendum: "#GA wind is blowing about in steady gusts, but the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is windy and clear.");

			AddEventWithTempVariations(name: "DuskWindy",
							  description: "A wind is blowing about in steady gusts, but the late afternoon sky is completely clear.",
							  roomAddendum: "#GA wind is blowing about in steady gusts, but the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is windy and clear.");

			AddEventWithTempVariations(name: "SunnyVeryWindy",
							  description: "A wind is howling with constant strong gusts, but warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BA wind is howling with constant strong gusts, but warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -4.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.StrongWind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, very windy and sunny.");

			AddEventWithTempVariations(name: "NightVeryWindy",
							  description: "A wind is howling with constant strong gusts, but the night sky is completely clear.",
							  roomAddendum: "#5A wind is howling with constant strong gusts, but the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -4.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.StrongWind,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is very windy and clear.");

			AddEventWithTempVariations(name: "DawnVeryWindy",
							  description: "A wind is howling with constant strong gusts, but the early morning sky is completely clear.",
							  roomAddendum: "#GA wind is howling with constant strong gusts, but the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -4.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.StrongWind,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is very windy and clear.");

			AddEventWithTempVariations(name: "DuskVeryWindy",
							  description: "A wind is howling with constant strong gusts, but the late afternoon sky is completely clear.",
							  roomAddendum: "#GA wind is howling with constant strong gusts, but the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -4.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.StrongWind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is very windy and clear.");

			AddEventWithTempVariations(name: "SunnyHotWindy",
							  description: "A hot wind is blowing about in steady gusts, but warm sunshine washes down from the sky overhead.",
							  roomAddendum: "#BA hot wind is blowing about in steady gusts, but warm sunshine washes down from the sky overhead.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 1.0,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: true,
							  afternoon: true,
							  dusk: false,
							  defaultTransition: "The day is clear, windy and sunny.");

			AddEventWithTempVariations(name: "NightWindy",
							  description: "A wind is blowing about in steady gusts, but the night sky is completely clear.",
							  roomAddendum: "#5A wind is blowing about in steady gusts, but the night sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.5,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: true,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The night sky is windy and clear.");

			AddEventWithTempVariations(name: "DawnWindy",
							  description: "A wind is blowing about in steady gusts, but the early morning sky is completely clear.",
							  roomAddendum: "#GA wind is blowing about in steady gusts, but the early morning sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: true,
							  morning: false,
							  afternoon: false,
							  dusk: false,
							  defaultTransition: "The early morning sky is windy and clear.");

			AddEventWithTempVariations(name: "DuskWindy",
							  description: "A wind is blowing about in steady gusts, but the late afternoon sky is completely clear.",
							  roomAddendum: "#GA wind is blowing about in steady gusts, but the late afternoon sky is completely clear.#0",
							  temp: 0.0,
							  precipTemp: 0.0,
							  windTemp: -3.0,
							  tempPerVariation: 0.66,
							  precipTempPerVariation: 0.0,
							  windTempPerVariation: 0.0,
							  precipitation: PrecipitationLevel.Dry,
							  wind: WindLevel.Wind,
							  obscureSky: false,
							  night: false,
							  dawn: false,
							  morning: false,
							  afternoon: false,
							  dusk: true,
							  defaultTransition: "The late afternoon sky is windy and clear.");

			_context.SaveChanges();
			#endregion

			return string.Empty;
		}

		/// <inheritdoc />
		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

			if (context.Celestials.Any()) return ShouldSeedResult.PrerequisitesNotMet;

			if (context.WeatherControllers.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

			return ShouldSeedResult.ReadyToInstall;
		}

		#endregion
	}
}
