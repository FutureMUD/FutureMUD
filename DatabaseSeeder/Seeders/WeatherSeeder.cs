using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public FuturemudDatabaseContext _context;
		public IReadOnlyDictionary<string, string> _questionAnswers;

		/// <inheritdoc />
		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			_context = context;
			_questionAnswers = questionAnswers;

			var celestial = _context.Celestials.Find(long.Parse(questionAnswers["celestial"]));
			if (celestial is null)
			{
				return "Could not proceed because the specified celestial object was not valid. No data was seeded.";
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

			AddEvent("SunnyStill", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 0.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, null, "The day is clear, still and sunny.");
			AddEvent("SunnyStillWarmer", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 1.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillWarm", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 2.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillHot", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 3.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillVeryHot", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 4.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillSweltering", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", 5.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillCooler", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", -1.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillCool", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", -2.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillCold", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", -3.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillVeryCold", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", -4.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
			AddEvent("SunnyStillFreezing", "The air is still and calm, and warm sunshine washes down from the sky overhead.", "#BThe air is still and calm, and warm sunshine washes down from the sky overhead.#0", -5.0, 0.0, 0.0, PrecipitationLevel.Dry, WindLevel.Still, false, false, false, true, true, true, "SunnyStill", "The day is clear, still and sunny.");
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
