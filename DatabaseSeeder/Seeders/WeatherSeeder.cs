using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

readonly record struct WeatherSeederTemperatureVariationTier(
    WeatherEventVariation Variation,
    int OffsetFromNormal);

sealed class WeatherSeederClimateProfile
{
    public required string ClimateModelName { get; init; }
    public required string RegionalClimatePrefix { get; init; }
    public required string KoppenClimateClassification { get; init; }
    public required string ReferenceLocation { get; init; }
    public required string ClimateModelSummary { get; init; }
    public required string RegionalClimateSummary { get; init; }
    public required IReadOnlyDictionary<string, (double Minimum, double Maximum)> SeasonalTemperatureRanges { get; init; }
    public required Func<string, WindLevel, double> WindIncreaseChance { get; init; }
    public required Func<string, WindLevel, double> WindDecreaseChance { get; init; }
    public required Func<string, PrecipitationLevel, int, double> PrecipIncreaseChance { get; init; }
    public required Func<string, PrecipitationLevel, int, double> PrecipDecreaseChance { get; init; }
    public required Func<string, double> TemperatureVariationChance { get; init; }
    public required Func<string, double> WindVariationChance { get; init; }
    public required Func<string, double> CloudIncreaseChance { get; init; }
    public required Func<string, double> CloudDecreaseChance { get; init; }
    public required Func<string, double> CloudyToOvercastChance { get; init; }
    public required Func<string, double> CloudyToHumidChance { get; init; }
    public required Func<string, double> OvercastToLightRainChance { get; init; }
    public required Func<string, double> OvercastToRainChance { get; init; }
    public required Func<string, double> OvercastToLightSnowChance { get; init; }
    public required Func<string, double> CloudyToDryChance { get; init; }
    public required Func<string, double> CloudyToParchedChance { get; init; }
    public required Func<string, double> OvercastToHumidChance { get; init; }
    public required Func<string, double> OvercastToCloudyChance { get; init; }
    public required Func<string, double> MaximumAdditionalChangeChance { get; init; }
    public double IncrementalAdditionalChangeChanceFromStableWeather { get; init; } = 0.0005;
    public double BaseChangeChance { get; init; } = 0.01;
}

sealed class WeatherSeederTemperatureVariationOption
{
    private Dictionary<WeatherEventVariation, int>? _variationIndices;

    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public required IReadOnlyList<WeatherSeederTemperatureVariationTier> Tiers { get; init; }

    private Dictionary<WeatherEventVariation, int> VariationIndices =>
        _variationIndices ??= Tiers
            .Select((tier, index) => new { tier.Variation, Index = index })
            .ToDictionary(x => x.Variation, x => x.Index);

    private int NormalIndex => VariationIndices[WeatherEventVariation.Normal];

    public int GetOffset(WeatherEventVariation variation)
    {
        return Tiers[VariationIndices[variation]].OffsetFromNormal;
    }

    public bool TryGetWarmerVariation(WeatherEventVariation currentVariation, out WeatherEventVariation warmerVariation)
    {
        warmerVariation = default;
        if (Tiers.Count <= 1 || !VariationIndices.TryGetValue(currentVariation, out int index) || index == Tiers.Count - 1)
        {
            return false;
        }

        if (index == NormalIndex)
        {
            warmerVariation = currentVariation;
            return true;
        }

        warmerVariation = Tiers[index + 1].Variation;
        return true;
    }

    public bool TryGetCoolerVariation(WeatherEventVariation currentVariation, out WeatherEventVariation coolerVariation)
    {
        coolerVariation = default;
        if (Tiers.Count <= 1 || !VariationIndices.TryGetValue(currentVariation, out int index) || index == 0)
        {
            return false;
        }

        if (index == NormalIndex)
        {
            coolerVariation = currentVariation;
            return true;
        }

        coolerVariation = Tiers[index - 1].Variation;
        return true;
    }
}

sealed class WeatherSeederEventCatalog
{
    public required IReadOnlyDictionary<string, WeatherEvent> EventsByName { get; init; }
    public required IReadOnlyDictionary<WeatherSeederEventDescriptor, WeatherEvent> EventsByDescriptor { get; init; }
    public required IReadOnlyDictionary<long, WeatherSeederEventDescriptor> DescriptorsById { get; init; }
}

public partial class WeatherSeeder : IDatabaseSeeder
{
    private static readonly string[] StockWeatherEventNames = ["HumidStill", "RainBreeze", "SnowWind"];
    private static readonly string[] StockSeasonNames = ["temperate_mid_winter", "temperate_mid_summer", "temperate_mid_autumn"];
    private static readonly string[] StockClimateModelNames = ["Temperate", "Oceanic Temperate", "Humid Subtropical"];
    private static readonly string[] StockRegionalClimateNames =
    [
        "Oceanic Temperate Northern Hemisphere",
        "Humid Subtropical Northern Hemisphere",
        "Mediterranean Northern Hemisphere"
    ];

    static Regex TempVariationRegex = new("(?:Freezing|VeryCold|Cold|Cool|Cooler|Warmer|Warm|Hot|VeryHot|Sweltering)$");
    static Regex CloudVariationRegex = new("^(?:Cloudy|Overcast)");
    private static readonly string[] ClimateSeasonGroups = { "Winter", "Spring", "Summer", "Autumn" };
    private static readonly WeatherSeederTemperatureVariationOption SingleTemperatureVariationOption = new()
    {
        Id = "base",
        Name = "Base Only",
        Summary = "single seeded weather state per event",
        Tiers =
        [
            new WeatherSeederTemperatureVariationTier(WeatherEventVariation.Normal, 0)
        ]
    };
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
                if (!text.EqualToAny("full", "soak", "none")) { return (false, "Please answer #3full#F, #3soak#F or #3none#f."); } return (true, string.Empty);
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

At the present time, this seeder installs temperate oceanic, humid subtropical, tropical rainforest, humid subcontinental, East Asian dry-winter continental, subarctic, tundra, polar ice cap, mediterranean, hot and cold semi-arid, and hot and cold desert climate templates (e.g. Western Europe / Pacific Northwest US, Köppen Cfa regions such as Atlanta or Brisbane, equatorial Af regions such as Singapore, cool-summer humid continental regions such as Buffalo or southern Ontario, Köppen Dwa regions such as Seoul or Beijing, Siberian Dfc regions such as Bratsk, polar ET/EF regions such as Nuuk or Antarctica, dry-summer regions such as Sacramento or coastal southern Europe, steppe climates such as Del Rio or Denver, and desert climates such as Yuma or Bishop).";

    public bool Enabled => true;

    public FuturemudDatabaseContext _context;
    public IReadOnlyDictionary<string, string> _questionAnswers;
    private bool UseRainEvents { get; set; }
    private Liquid? RainLiquid { get; set; }
    private WeatherSeederTemperatureVariationOption TemperatureVariationOption { get; } = SingleTemperatureVariationOption;

    private static WeatherSeederEventDescriptor CreateEventDescriptor(
        PrecipitationLevel precipitation,
        WindLevel wind,
        WeatherEventVariation temperatureVariation,
        WindVariation windVariation,
        CloudVariation cloudVariation)
    {
        return new WeatherSeederEventDescriptor(
            precipitation,
            wind,
            temperatureVariation,
            wind > WindLevel.Still ? windVariation : WindVariation.Normal,
            cloudVariation);
    }

    private static string GetTemperatureVariationSuffix(WeatherEventVariation variation)
    {
        return variation == WeatherEventVariation.Normal ? string.Empty : variation.DescribeEnum();
    }

    private static WeatherEvent GetRequiredEvent(WeatherSeederEventCatalog eventCatalog, WeatherSeederEventDescriptor descriptor)
    {
        return eventCatalog.EventsByDescriptor[CreateEventDescriptor(
            descriptor.Precipitation,
            descriptor.Wind,
            descriptor.TemperatureVariation,
            descriptor.WindVariation,
            descriptor.CloudVariation)];
    }

    private static ClimateModel GetRequiredBaseClimateModel(
        IReadOnlyDictionary<string, ClimateModel> seededClimateModels,
        WeatherSeederClimateProfile baseProfile)
    {
        return seededClimateModels.TryGetValue(baseProfile.ClimateModelName, out ClimateModel? climateModel)
            ? climateModel
            : throw new InvalidOperationException($"Base climate model {baseProfile.ClimateModelName} was not seeded before its derived climates.");
    }

    private static void AddTransitionForAllSeasons(
        IDictionary<string, CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>> transitionsBySeason,
        WeatherEvent from,
        WeatherEvent to,
        Func<string, double> chanceSelector)
    {
        foreach (string seasonGroup in ClimateSeasonGroups)
        {
            transitionsBySeason[seasonGroup].Add(from, (to, chanceSelector(seasonGroup)));
        }
    }

    private static void AddTransitionForSpecificSeasons(
        IDictionary<string, CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>> transitionsBySeason,
        WeatherEvent from,
        WeatherEvent to,
        params (string SeasonGroup, double Chance)[] seasonChances)
    {
        foreach ((string? seasonGroup, double chance) in seasonChances)
        {
            transitionsBySeason[seasonGroup].Add(from, (to, chance));
        }
    }

    private static (double EffectiveChangeChance, string TransitionXml) BuildTransitionDefinition(
        WeatherEvent currentEvent,
        IEnumerable<(WeatherEvent To, double Chance)> transitions,
        double baseChangeChance)
    {
        List<(WeatherEvent To, double Chance)> groupedTransitions = transitions
            .GroupBy(x => x.To)
            .Select(x => (To: x.Key, Chance: x.Sum(y => y.Chance)))
            .Where(x => x.Chance > 0.0)
            .ToList();
        double totalChance = groupedTransitions.Sum(x => x.Chance);
        List<(WeatherEvent To, double Chance)> nonSelfTransitions = groupedTransitions
            .Where(x => x.To != currentEvent)
            .ToList();
        double nonSelfChance = nonSelfTransitions.Sum(x => x.Chance);
        return (
            totalChance > 0.0 && nonSelfChance > 0.0 ? baseChangeChance * nonSelfChance / totalChance : 0.0,
            new XElement(
                "Transitions",
                from transition in nonSelfTransitions
                select new XElement(
                    "Transition",
                    new XAttribute("id", transition.To.Id),
                    new XAttribute("chance", transition.Chance)))
                .ToString());
    }

    private static (double EffectiveChangeChance, string TransitionXml) BuildTransitionDefinition(
        long currentEventId,
        IEnumerable<(long EventId, double Chance)> transitions,
        double baseChangeChance)
    {
        List<(long EventId, double Chance)> groupedTransitions = transitions
            .GroupBy(x => x.EventId)
            .Select(x => (EventId: x.Key, Chance: x.Sum(y => y.Chance)))
            .Where(x => x.Chance > 0.0)
            .ToList();
        double totalChance = groupedTransitions.Sum(x => x.Chance);
        List<(long EventId, double Chance)> nonSelfTransitions = groupedTransitions
            .Where(x => x.EventId != currentEventId)
            .ToList();
        double nonSelfChance = nonSelfTransitions.Sum(x => x.Chance);
        return (
            totalChance > 0.0 && nonSelfChance > 0.0 ? baseChangeChance * nonSelfChance / totalChance : 0.0,
            new XElement(
                "Transitions",
                from transition in nonSelfTransitions
                select new XElement(
                    "Transition",
                    new XAttribute("id", transition.EventId),
                    new XAttribute("chance", transition.Chance)))
                .ToString());
    }

    private void CreateWeatherEvent(
        FuturemudDatabaseContext context,
        PrecipitationLevel precipitation,
        WindLevel wind,
        Dictionary<WeatherEvent, string> defaultTransitions,
        Dictionary<string, WeatherEvent> eventsByName,
        Dictionary<WeatherSeederEventDescriptor, WeatherEvent> eventsByDescriptor)
    {
        void AddEvent(
            string name,
            string description,
            string roomAddendum,
            double temp,
            double precipTemp,
            double windTemp,
            PrecipitationLevel precipitationLevel,
            WindLevel windLevel,
            WeatherEventVariation temperatureVariation,
            WindVariation windVariation,
            CloudVariation cloudVariation,
            bool obscureSky,
            bool night,
            bool dawn,
            bool morning,
            bool afternoon,
            bool dusk,
            string? countsAs,
            string defaultTransition)
        {
            WeatherEvent weatherEvent = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.WeatherEvents,
                name,
                x => x.Name,
                () =>
                {
                    WeatherEvent created = new();
                    context.WeatherEvents.Add(created);
                    return created;
                });
            weatherEvent.Name = name;
            weatherEvent.WeatherDescription = description;
            weatherEvent.WeatherRoomAddendum = roomAddendum;
            weatherEvent.TemperatureEffect = temp;
            weatherEvent.PrecipitationTemperatureEffect = precipTemp;
            weatherEvent.WindTemperatureEffect = windTemp;
            weatherEvent.Precipitation = (int)precipitationLevel;
            weatherEvent.Wind = (int)windLevel;
            weatherEvent.ObscuresViewOfSky = obscureSky;
            weatherEvent.PermittedAtNight = night;
            weatherEvent.PermittedAtDawn = dawn;
            weatherEvent.PermittedAtMorning = morning;
            weatherEvent.PermittedAtAfternoon = afternoon;
            weatherEvent.PermittedAtDusk = dusk;
            weatherEvent.WeatherEventType = UseRainEvents ? (precipitationLevel.IsRaining() ? "rain" : "simple") : "simple";
            weatherEvent.AdditionalInfo = "";
            if (countsAs is not null)
            {
                weatherEvent.CountsAs = eventsByName[countsAs];
            }

            defaultTransitions[weatherEvent] = defaultTransition;
            eventsByName[name] = weatherEvent;
            eventsByDescriptor[CreateEventDescriptor(precipitationLevel, windLevel, temperatureVariation, windVariation, cloudVariation)] = weatherEvent;
        }

        void AddEventWithTempVariations(
            string name,
            string description,
            string roomAddendum,
            double temp,
            double precipTemp,
            double windTemp,
            double tempPerVariation,
            double precipTempPerVariation,
            double windTempPerVariation,
            PrecipitationLevel precipitationLevel,
            WindLevel windLevel,
            WindVariation windVariation,
            CloudVariation cloudVariation,
            bool obscureSky,
            bool night,
            bool dawn,
            bool morning,
            bool afternoon,
            bool dusk,
            string defaultTransition)
        {
            WeatherSeederTemperatureVariationTier normalTier = TemperatureVariationOption.Tiers.Single(x => x.Variation == WeatherEventVariation.Normal);
            AddEvent(
                name,
                description,
                roomAddendum,
                temp + normalTier.OffsetFromNormal * tempPerVariation,
                precipTemp + normalTier.OffsetFromNormal * precipTempPerVariation,
                windTemp + normalTier.OffsetFromNormal * windTempPerVariation,
                precipitationLevel,
                windLevel,
                normalTier.Variation,
                windVariation,
                cloudVariation,
                obscureSky,
                night,
                dawn,
                morning,
                afternoon,
                dusk,
                null,
                defaultTransition);

            foreach (WeatherSeederTemperatureVariationTier tier in TemperatureVariationOption.Tiers.Where(x => x.Variation != WeatherEventVariation.Normal))
            {
                string eventName = $"{name}{GetTemperatureVariationSuffix(tier.Variation)}";
                AddEvent(
                    eventName,
                    description,
                    roomAddendum,
                    temp + tier.OffsetFromNormal * tempPerVariation,
                    precipTemp + tier.OffsetFromNormal * precipTempPerVariation,
                    windTemp + tier.OffsetFromNormal * windTempPerVariation,
                    precipitationLevel,
                    windLevel,
                    tier.Variation,
                    windVariation,
                    cloudVariation,
                    obscureSky,
                    night,
                    dawn,
                    morning,
                    afternoon,
                    dusk,
                    tier.Variation == WeatherEventVariation.Normal ? null : name,
                    defaultTransition);
            }
        }

        double precipitationTempDeltaPerVariation = 0.0;
        double precipitationTempDelta = 0.0;
        string precipitationDescription = "";
        string precipitationDescriptionCloudy = "";
        bool obscureSky = true;
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

        double windTempDeltaHot = 0.0;
        double windTempDeltaCold = 0.0;
        double windTempDelta = 0.0;
        string windDescription = "";

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

        AddEventWithTempVariations($"{precipitation.DescribeEnum()}{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Normal, CloudVariation.None, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "")}");
        if (wind > WindLevel.Still)
        {
            AddEventWithTempVariations($"{precipitation.DescribeEnum()}Equatorial{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Equatorial, CloudVariation.None, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "hot ")}");
            AddEventWithTempVariations($"{precipitation.DescribeEnum()}Polar{wind.DescribeEnum()}", $"{precipitationDescription}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescription}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Polar, CloudVariation.None, obscureSky, true, true, true, true, true, $"{precipitationDescription}{string.Format(windDescription, "cold ")}");
        }
        if (precipitation == PrecipitationLevel.Humid)
        {
            AddEventWithTempVariations($"Overcast{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Normal, CloudVariation.Overcast, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}");
            if (wind > WindLevel.Still)
            {
                AddEventWithTempVariations($"OvercastEquatorial{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Equatorial, CloudVariation.Overcast, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}");
                AddEventWithTempVariations($"OvercastPolar{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Polar, CloudVariation.Overcast, true, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}");
            }
        }
        if (precipitation == PrecipitationLevel.Dry)
        {
            AddEventWithTempVariations($"Cloudy{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDelta, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Normal, CloudVariation.Cloudy, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "")}");
            if (wind > WindLevel.Still)
            {
                AddEventWithTempVariations($"CloudyEquatorial{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaHot, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Equatorial, CloudVariation.Cloudy, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "hot ")}");
                AddEventWithTempVariations($"CloudyPolar{wind.DescribeEnum()}", $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}", $"{textColour.Colour}{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}{Telnet.RESETALL}", 0.0, precipitationTempDelta, windTempDeltaCold, 1.0, precipitationTempDeltaPerVariation, 0.0, precipitation, wind, WindVariation.Polar, CloudVariation.Cloudy, obscureSky, true, true, true, true, true, $"{precipitationDescriptionCloudy}{string.Format(windDescription, "cold ")}");
            }
        }
    }

    /// <inheritdoc />
    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        _context = context;
        _questionAnswers = questionAnswers;

        Stopwatch totalStopwatch = Stopwatch.StartNew();
        Console.WriteLine("[Weather Seeder] Starting weather and climate seeding.");
        Console.WriteLine("[Weather Seeder] Using a single canonical weather event set without seeded temperature variants.");

        Celestial? celestial = _context.Celestials.FirstOrDefault();
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
                StaticConfiguration? dbsetting = context.StaticConfigurations.Find("PuddlesEnabled");
                if (dbsetting is null)
                {
                    dbsetting = new StaticConfiguration { SettingName = "PuddlesEnabled", Definition = "" };
                    context.StaticConfigurations.Add(dbsetting);
                }
                dbsetting.Definition = "false";
                RainLiquid = context.Liquids.First(x => x.Name == "rain water");
                break;
            case "none":
                UseRainEvents = false;
                break;
        }

        Stopwatch stageStopwatch = Stopwatch.StartNew();
        Console.WriteLine("[Weather Seeder] Creating seasons...");
        List<Season> seasons = CreateSeasons(celestial);
        Console.WriteLine($"[Weather Seeder] Created {seasons.Count:N0} seasons in {stageStopwatch.Elapsed.TotalSeconds:0.0}s.");

        stageStopwatch.Restart();
        Console.WriteLine("[Weather Seeder] Creating weather events...");
        WeatherSeederEventCatalog eventCatalog = CreateWeatherEvents(context);
        Console.WriteLine($"[Weather Seeder] Created {eventCatalog.EventsByDescriptor.Count:N0} weather events in {stageStopwatch.Elapsed.TotalSeconds:0.0}s.");

        List<WeatherSeederClimateProfile> profiles = GetClimateProfiles().ToList();
        Dictionary<string, ClimateModel> seededClimateModels = new(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"[Weather Seeder] Building {profiles.Count:N0} climate models...");
        for (int i = 0; i < profiles.Count; i++)
        {
            WeatherSeederClimateProfile profile = profiles[i];
            stageStopwatch.Restart();
            Console.WriteLine($"[Weather Seeder] [{i + 1}/{profiles.Count}] Building {profile.ClimateModelName} ({profile.KoppenClimateClassification}; {profile.ReferenceLocation})...");
            ClimateModel climateModel = CreateClimateModel(context, seasons, eventCatalog, profile, seededClimateModels);
            seededClimateModels.Add(climateModel.Name, climateModel);
            Console.WriteLine($"[Weather Seeder] [{i + 1}/{profiles.Count}] Built {profile.ClimateModelName} in {stageStopwatch.Elapsed.TotalSeconds:0.0}s.");
        }

        stageStopwatch.Restart();
        Console.WriteLine("[Weather Seeder] Saving climate models...");
        context.SaveChanges();
        Console.WriteLine($"[Weather Seeder] Saved {seededClimateModels.Count:N0} climate models in {stageStopwatch.Elapsed.TotalSeconds:0.0}s.");

        Console.WriteLine($"[Weather Seeder] Creating {profiles.Count:N0} regional climates...");
        for (int i = 0; i < profiles.Count; i++)
        {
            WeatherSeederClimateProfile profile = profiles[i];
            stageStopwatch.Restart();
            Console.WriteLine($"[Weather Seeder] [{i + 1}/{profiles.Count}] Creating regional climate for {profile.RegionalClimatePrefix}...");
            CreateRegionalClimate(context, seasons, seededClimateModels[profile.ClimateModelName], profile);
            Console.WriteLine($"[Weather Seeder] [{i + 1}/{profiles.Count}] Prepared regional climate in {stageStopwatch.Elapsed.TotalSeconds:0.0}s.");
        }

        stageStopwatch.Restart();
        Console.WriteLine("[Weather Seeder] Saving regional climates...");
        context.SaveChanges();
        Console.WriteLine($"[Weather Seeder] Weather seeding complete in {totalStopwatch.Elapsed.TotalSeconds:0.0}s.");

        return string.Empty;
    }

    private static IEnumerable<WeatherSeederClimateProfile> GetClimateProfiles()
    {
        yield return CreateTemperateOceanicProfile();
        yield return CreateSubpolarOceanicProfile();
        yield return CreateHumidSubtropicalProfile();
        yield return CreateDryWinterHumidSubtropicalProfile();
        yield return CreateSubtropicalHighlandProfile();
        yield return CreateColdSummerSubtropicalHighlandProfile();
        yield return CreateMediterraneanProfile();
        yield return CreateWarmSummerMediterraneanProfile();
        yield return CreateColdSummerMediterraneanProfile();
        yield return CreateTropicalRainforestProfile();
        yield return CreateTropicalMonsoonProfile();
        yield return CreateTropicalSavannaDryWinterProfile();
        yield return CreateTropicalSavannaDrySummerProfile();
        yield return CreateHumidSubcontinentalProfile();
        yield return CreateHotSummerHumidSubcontinentalProfile();
        yield return CreateDryWinterHumidSubcontinentalProfile();
        yield return CreateWarmSummerDryWinterHumidSubcontinentalProfile();
        yield return CreateSubarcticProfile();
        yield return CreateSevereWinterSubarcticProfile();
        yield return CreateDryWinterSubarcticProfile();
        yield return CreateSevereWinterDryWinterSubarcticProfile();
        yield return CreateHotSummerDrySummerContinentalProfile();
        yield return CreateWarmSummerDrySummerContinentalProfile();
        yield return CreateDrySummerSubarcticProfile();
        yield return CreateSevereWinterDrySummerSubarcticProfile();
        yield return CreateTundraProfile();
        yield return CreatePolarIceCapProfile();
        yield return CreateHotSemiAridProfile();
        yield return CreateHotDesertProfile();
        yield return CreateColdSemiAridProfile();
        yield return CreateColdDesertProfile();
    }

    private static string DescribeTemperature(double temperature)
    {
        return $"{temperature.ToString("0.#", global::System.Globalization.CultureInfo.InvariantCulture)}C";
    }

    private static string DescribeTemperatureRange((double Minimum, double Maximum) range)
    {
        return $"{DescribeTemperature(range.Minimum)} to {DescribeTemperature(range.Maximum)}";
    }

    private static double EstimateTemperatureFluctuationStandardDeviation(WeatherSeederClimateProfile profile)
    {
        double coldest = profile.SeasonalTemperatureRanges.Min(x => x.Value.Minimum);
        double warmest = profile.SeasonalTemperatureRanges.Max(x => x.Value.Maximum);
        double annualEnvelope = Math.Max(0.0, warmest - coldest);
        return Math.Round(Math.Clamp(0.75 + annualEnvelope / 18.0, 0.6, 4.5), 1);
    }

    private static TimeSpan EstimateTemperatureFluctuationPeriod(WeatherSeederClimateProfile profile)
    {
        double coldest = profile.SeasonalTemperatureRanges.Min(x => x.Value.Minimum);
        double warmest = profile.SeasonalTemperatureRanges.Max(x => x.Value.Maximum);
        double annualEnvelope = Math.Max(0.0, warmest - coldest);
        double days = Math.Clamp(3.0 + annualEnvelope / 12.0, 3.0, 8.0);
        return TimeSpan.FromDays(Math.Round(days * 2.0) / 2.0);
    }

    private static string BuildClimateModelDescription(WeatherSeederClimateProfile profile)
    {
        KeyValuePair<string, (double Minimum, double Maximum)> coldest = profile.SeasonalTemperatureRanges.OrderBy(x => x.Value.Minimum).First();
        KeyValuePair<string, (double Minimum, double Maximum)> warmest = profile.SeasonalTemperatureRanges.OrderByDescending(x => x.Value.Maximum).First();
        (double Minimum, double Maximum) midWinter = profile.SeasonalTemperatureRanges["Mid Winter"];
        (double Minimum, double Maximum) midSummer = profile.SeasonalTemperatureRanges["Mid Summer"];
        return $"{profile.ClimateModelSummary}\n\nThis template matches Koppen climate classification {profile.KoppenClimateClassification} and was calibrated against {profile.ReferenceLocation}. In the seeded regional climate baseline, temperatures usually bottom out around {DescribeTemperature(coldest.Value.Minimum)} in {coldest.Key} and peak around {DescribeTemperature(warmest.Value.Maximum)} in {warmest.Key}, with Mid Winter typically sitting near {DescribeTemperatureRange(midWinter)} and Mid Summer near {DescribeTemperatureRange(midSummer)}. Use the opposite-hemisphere flag on weather controllers to phase shift these seasons by half a year without duplicating the climate data.";
    }

    private static string BuildRegionalClimateDescription(WeatherSeederClimateProfile profile)
    {
        KeyValuePair<string, (double Minimum, double Maximum)> coldest = profile.SeasonalTemperatureRanges.OrderBy(x => x.Value.Minimum).First();
        KeyValuePair<string, (double Minimum, double Maximum)> warmest = profile.SeasonalTemperatureRanges.OrderByDescending(x => x.Value.Maximum).First();
        (double Minimum, double Maximum) midWinter = profile.SeasonalTemperatureRanges["Mid Winter"];
        (double Minimum, double Maximum) midSummer = profile.SeasonalTemperatureRanges["Mid Summer"];
        double fluctuationStdDev = EstimateTemperatureFluctuationStandardDeviation(profile);
        TimeSpan fluctuationPeriod = EstimateTemperatureFluctuationPeriod(profile);
        return $"{profile.RegionalClimateSummary} This is the seeded northern-hemisphere baseline for the {profile.RegionalClimatePrefix} template.\n\nTypical daily temperatures sit around {DescribeTemperatureRange(midWinter)} in Mid Winter and {DescribeTemperatureRange(midSummer)} in Mid Summer, with the full annual envelope running from about {DescribeTemperature(coldest.Value.Minimum)} to {DescribeTemperature(warmest.Value.Maximum)}. Day-to-day conditions are intended to wander by about {DescribeTemperature(fluctuationStdDev)} around the hourly mean over roughly {fluctuationPeriod.TotalDays:0.#} days before tending back toward normal. It matches Koppen climate classification {profile.KoppenClimateClassification} and was based on {profile.ReferenceLocation}. Use the opposite-hemisphere flag on a weather controller to invert the seasonal timing for southern locations.";
    }

    private static XElement CreateDailyTemperatures(double minimum, double maximum)
    {
        List<(int Hour, double Temp)> temps = new();
        double delta = maximum - minimum;
        for (int i = 0; i < 24; i++)
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

    private static void CreateRegionalClimate(FuturemudDatabaseContext context, List<Season> seasons, ClimateModel climateModel, WeatherSeederClimateProfile profile)
    {
        #region Regional Climate
        string regionalClimateName = $"{profile.RegionalClimatePrefix} Northern Hemisphere";
        RegionalClimate regionalClimate = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.RegionalClimates,
            regionalClimateName,
            x => x.Name,
            () =>
            {
                RegionalClimate created = new();
                context.RegionalClimates.Add(created);
                return created;
            });
        regionalClimate.Name = regionalClimateName;
        regionalClimate.Description = BuildRegionalClimateDescription(profile);
        regionalClimate.ClimateModelId = climateModel.Id;
        regionalClimate.TemperatureFluctuationStandardDeviation = EstimateTemperatureFluctuationStandardDeviation(profile);
        regionalClimate.TemperatureFluctuationPeriodMinutes =
            (int)Math.Round(EstimateTemperatureFluctuationPeriod(profile).TotalMinutes);

        foreach (RegionalClimatesSeason? existing in context.RegionalClimatesSeasons.Where(x => x.RegionalClimateId == regionalClimate.Id).ToList())
        {
            context.RegionalClimatesSeasons.Remove(existing);
        }

        foreach (Season season in seasons)
        {
            (double Minimum, double Maximum) temperatures = profile.SeasonalTemperatureRanges[season.DisplayName];
            RegionalClimatesSeason rs = new()
            {
                RegionalClimate = regionalClimate,
                Season = season,
                TemperatureInfo = CreateDailyTemperatures(temperatures.Minimum, temperatures.Maximum).ToString()
            };
            context.RegionalClimatesSeasons.Add(rs);
        }
        #endregion
    }

    private static ClimateModel CreateClimateModels(
        FuturemudDatabaseContext context,
        List<Season> seasons,
        WeatherSeederEventCatalog eventCatalog,
        WeatherSeederClimateProfile profile,
        WeatherSeederTemperatureVariationOption temperatureVariationOption)
    {
        Dictionary<string, CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>> eventsAndTransitionsBySeason = ClimateSeasonGroups.ToDictionary(
            x => x,
            x => new CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>());

        Dictionary<string, WindLevel> seasonalNormalWind = new()
        {
            { "Winter", WindLevel.OccasionalBreeze },
            { "Autumn", WindLevel.Breeze },
            { "Spring", WindLevel.OccasionalBreeze },
            { "Summer", WindLevel.OccasionalBreeze }
        };

        Dictionary<string, PrecipitationLevel> seasonalNormalPrecipitation = new()
        {
            { "Winter", PrecipitationLevel.Humid },
            { "Autumn", PrecipitationLevel.Humid },
            { "Spring", PrecipitationLevel.Humid },
            { "Summer", PrecipitationLevel.Humid }
        };

        double WindIncreaseChance(string seasonName, WindLevel currentWind)
        {
            int delta = currentWind.StepsFrom(seasonalNormalWind[seasonName]);
            if (delta < 0)
            {
                return 4.5 + Math.Abs(delta) * 1.5;
            }

            if (delta == 0)
            {
                return 0.9;
            }

            return Math.Max(0.1, 0.8 - delta * 0.15);
        }

        double WindDecreaseChance(string seasonName, WindLevel currentWind)
        {
            int delta = currentWind.StepsFrom(seasonalNormalWind[seasonName]);
            if (delta > 0)
            {
                return 4.5 + delta * 1.5;
            }

            if (delta == 0)
            {
                return 1.0;
            }

            return Math.Max(0.1, 0.9 - Math.Abs(delta) * 0.15);
        }

        double PrecipIncreaseChance(string seasonName, PrecipitationLevel currentPrecipitation, int stages = 1)
        {
            int delta = currentPrecipitation.StepsFrom(seasonalNormalPrecipitation[seasonName]);
            if (delta < 0)
            {
                return stages == 1
                    ? 5.0 + Math.Abs(delta) * 1.5
                    : 1.75 + Math.Abs(delta) * 0.75;
            }

            if (delta == 0)
            {
                return stages == 1 ? 1.8 : 0.4;
            }

            return Math.Max(
                stages == 1 ? 0.25 : 0.05,
                (stages == 1 ? 0.9 : 0.15) - delta * (stages == 1 ? 0.1 : 0.04)
            );
        }

        double PrecipDecreaseChance(string seasonName, PrecipitationLevel currentPrecipitation, int stages = 1)
        {
            int delta = currentPrecipitation.StepsFrom(seasonalNormalPrecipitation[seasonName]);
            if (delta > 0)
            {
                return stages == 1
                    ? 2.5 + delta * 0.75
                    : 0.8 + delta * 0.4;
            }

            if (delta == 0)
            {
                return stages == 1 ? 0.8 : 0.15;
            }

            return Math.Max(
                stages == 1 ? 0.25 : 0.05,
                (stages == 1 ? 0.9 : 0.15) - Math.Abs(delta) * (stages == 1 ? 0.1 : 0.04)
            );
        }

        double TemperatureVariationChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 3.0,
                "Winter" => 2.5,
                "Spring" => 2.0,
                "Summer" => 1.5,
                _ => 2.0
            };
        }

        double WindVariationChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 1.5,
                "Winter" => 1.0,
                "Spring" => 1.0,
                "Summer" => 0.5,
                _ => 1.0
            };
        }

        double CloudIncreaseChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 4.5,
                "Winter" => 4.0,
                "Spring" => 3.5,
                "Summer" => 3.0,
                _ => 3.5
            };
        }

        double CloudDecreaseChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 2.2,
                "Winter" => 2.4,
                "Spring" => 2.3,
                "Summer" => 2.8,
                _ => 2.4
            };
        }

        foreach ((WeatherSeederEventDescriptor descriptor, WeatherEvent? weatherEvent) in eventCatalog.EventsByDescriptor)
        {
            WindLevel wind = descriptor.Wind;
            PrecipitationLevel precip = descriptor.Precipitation;
            WeatherEventVariation variation = descriptor.TemperatureVariation;
            WindVariation windVariation = descriptor.WindVariation;
            CloudVariation cloudVariation = descriptor.CloudVariation;
            WindVariation stableWindVariation = wind > WindLevel.OccasionalBreeze ? windVariation : WindVariation.Normal;

            WeatherSeederEventDescriptor WithStableWindVariation(
                PrecipitationLevel targetPrecipitation,
                WindLevel targetWind,
                WeatherEventVariation targetVariation,
                CloudVariation targetCloudVariation)
            {
                return CreateEventDescriptor(targetPrecipitation, targetWind, targetVariation, stableWindVariation, targetCloudVariation);
            }

            WeatherSeederEventDescriptor WithCurrentWindVariation(
                PrecipitationLevel targetPrecipitation,
                WindLevel targetWind,
                WeatherEventVariation targetVariation,
                CloudVariation targetCloudVariation)
            {
                return CreateEventDescriptor(targetPrecipitation, targetWind, targetVariation, windVariation, targetCloudVariation);
            }

            void AddWeatherTransition(WeatherSeederEventDescriptor targetDescriptor, Func<string, double> chanceSelector)
            {
                AddTransitionForAllSeasons(
                    eventsAndTransitionsBySeason,
                    weatherEvent,
                    GetRequiredEvent(eventCatalog, targetDescriptor),
                    chanceSelector);
            }

            void AddWeatherTransitionForSpecificSeasons(WeatherSeederEventDescriptor targetDescriptor, params (string SeasonGroup, double Chance)[] seasonChances)
            {
                AddTransitionForSpecificSeasons(
                    eventsAndTransitionsBySeason,
                    weatherEvent,
                    GetRequiredEvent(eventCatalog, targetDescriptor),
                    seasonChances);
            }

            if (wind < WindLevel.MaelstromWind)
            {
                AddWeatherTransition(
                    WithCurrentWindVariation(precip, wind.StageUp(), variation, cloudVariation),
                    seasonName => WindIncreaseChance(seasonName, wind));
            }

            if (wind > WindLevel.None)
            {
                AddWeatherTransition(
                    WithStableWindVariation(precip, wind.StageDown(), variation, cloudVariation),
                    seasonName => WindDecreaseChance(seasonName, wind));
            }

            switch (cloudVariation)
            {
                case CloudVariation.None:
                    if (precip != PrecipitationLevel.TorrentialRain &&
                        precip != PrecipitationLevel.Blizzard &&
                        precip != PrecipitationLevel.Sleet)
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(precip.StageUp(), wind, variation, CloudVariation.None),
                            seasonName => PrecipIncreaseChance(seasonName, precip));

                        if (precip != PrecipitationLevel.HeavyRain && precip != PrecipitationLevel.HeavySnow)
                        {
                            AddWeatherTransition(
                                WithStableWindVariation(precip.StageUp(2), wind, variation, CloudVariation.None),
                                seasonName => PrecipIncreaseChance(seasonName, precip, 2));
                        }
                    }
                    break;
                case CloudVariation.Cloudy:
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.Overcast),
                        seasonName => seasonName switch
                        {
                            "Winter" => 3.5,
                            "Autumn" => 4.0,
                            "Summer" => 2.5,
                            "Spring" => 3.5,
                            _ => 0.0
                        });
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 1.5,
                            "Autumn" => 2.0,
                            "Summer" => 3.5,
                            "Spring" => 2.5,
                            _ => 0.0
                        });
                    break;
                case CloudVariation.Overcast:
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.LightRain, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 4.0,
                            "Autumn" => 4.5,
                            "Summer" => 3.0,
                            "Spring" => 3.5,
                            _ => 0.0
                        });
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Rain, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 1.5,
                            "Autumn" => 2.0,
                            "Summer" => 1.0,
                            "Spring" => 1.2,
                            _ => 0.0
                        });
                    AddWeatherTransitionForSpecificSeasons(
                        WithStableWindVariation(PrecipitationLevel.LightSnow, wind, variation, CloudVariation.None),
                        ("Winter", 0.7),
                        ("Autumn", 0.05));
                    break;
            }

            switch (cloudVariation)
            {
                case CloudVariation.None:
                    if (precip == PrecipitationLevel.Parched)
                    {
                        break;
                    }

                    if (precip == PrecipitationLevel.LightSnow || precip == PrecipitationLevel.Sleet || precip == PrecipitationLevel.LightRain)
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.Overcast),
                            seasonName => PrecipDecreaseChance(seasonName, precip));
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.None),
                            seasonName => PrecipDecreaseChance(seasonName, precip));
                    }
                    else
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(precip.StageDown(), wind, variation, CloudVariation.None),
                            seasonName => PrecipDecreaseChance(seasonName, precip));
                    }

                    if (precip == PrecipitationLevel.Dry)
                    {
                        break;
                    }

                    if (precip == PrecipitationLevel.LightSnow || precip == PrecipitationLevel.Sleet || precip == PrecipitationLevel.LightRain)
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Dry, wind, variation, CloudVariation.Cloudy),
                            seasonName => PrecipDecreaseChance(seasonName, precip, 2));
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Dry, wind, variation, CloudVariation.None),
                            seasonName => PrecipDecreaseChance(seasonName, precip, 2));
                    }
                    else if (precip == PrecipitationLevel.Snow || precip == PrecipitationLevel.Rain)
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.Overcast),
                            seasonName => PrecipDecreaseChance(seasonName, precip, 2));
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.None),
                            seasonName => PrecipDecreaseChance(seasonName, precip, 2));
                    }
                    else
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(precip.StageDown(), wind, variation, CloudVariation.None),
                            seasonName => PrecipDecreaseChance(seasonName, precip, 2));
                    }
                    break;
                case CloudVariation.Cloudy:
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Dry, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 3.5,
                            "Autumn" => 3.0,
                            "Summer" => 4.0,
                            "Spring" => 3.5,
                            _ => 0.0
                        });
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Parched, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 0.2,
                            "Autumn" => 0.1,
                            "Summer" => 1.2,
                            "Spring" => 0.3,
                            _ => 0.0
                        });
                    break;
                case CloudVariation.Overcast:
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.None),
                        seasonName => seasonName switch
                        {
                            "Winter" => 4.0,
                            "Autumn" => 3.5,
                            "Summer" => 4.5,
                            "Spring" => 4.0,
                            _ => 0.0
                        });
                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Dry, wind, variation, CloudVariation.Cloudy),
                        seasonName => seasonName switch
                        {
                            "Winter" => 1.5,
                            "Autumn" => 1.2,
                            "Summer" => 2.0,
                            "Spring" => 1.8,
                            _ => 0.0
                        });
                    break;
            }

            if (temperatureVariationOption.TryGetWarmerVariation(variation, out WeatherEventVariation warmerVariation))
            {
                AddWeatherTransition(
                    WithStableWindVariation(precip, wind, warmerVariation, cloudVariation),
                    TemperatureVariationChance);
            }
            else
            {
                AddWeatherTransition(
                    CreateEventDescriptor(precip, wind, variation, windVariation, cloudVariation),
                    TemperatureVariationChance);
            }

            if (temperatureVariationOption.TryGetCoolerVariation(variation, out WeatherEventVariation coolerVariation))
            {
                AddWeatherTransition(
                    WithStableWindVariation(precip, wind, coolerVariation, cloudVariation),
                    TemperatureVariationChance);
            }
            else
            {
                AddWeatherTransition(
                    CreateEventDescriptor(precip, wind, variation, windVariation, cloudVariation),
                    TemperatureVariationChance);
            }

            if (wind > WindLevel.OccasionalBreeze)
            {
                switch (windVariation)
                {
                    case WindVariation.Normal:
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Polar, cloudVariation),
                            WindVariationChance);
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Equatorial, cloudVariation),
                            WindVariationChance);
                        break;
                    case WindVariation.Polar:
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Normal, cloudVariation),
                            WindVariationChance);
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Equatorial, cloudVariation),
                            WindVariationChance);
                        break;
                    case WindVariation.Equatorial:
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Polar, cloudVariation),
                            WindVariationChance);
                        AddWeatherTransition(
                            CreateEventDescriptor(precip, wind, variation, WindVariation.Normal, cloudVariation),
                            WindVariationChance);
                        break;
                }
            }

            switch (cloudVariation)
            {
                case CloudVariation.None:
                    if (precip > PrecipitationLevel.Humid || precip == PrecipitationLevel.Parched)
                    {
                        break;
                    }

                    if (precip == PrecipitationLevel.Humid)
                    {
                        AddWeatherTransition(
                            WithStableWindVariation(PrecipitationLevel.Humid, wind, variation, CloudVariation.Overcast),
                            CloudIncreaseChance);
                        break;
                    }

                    AddWeatherTransition(
                        WithStableWindVariation(PrecipitationLevel.Dry, wind, variation, CloudVariation.Cloudy),
                        CloudIncreaseChance);
                    break;

                case CloudVariation.Cloudy:
                case CloudVariation.Overcast:
                    AddWeatherTransition(
                        WithStableWindVariation(precip, wind, variation, CloudVariation.None),
                        CloudDecreaseChance);
                    break;
            }
        }

        ClimateModel temperateModel = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.ClimateModels,
            profile.ClimateModelName,
            x => x.Name,
            () =>
            {
                ClimateModel created = new();
                context.ClimateModels.Add(created);
                return created;
            });
        temperateModel.Name = profile.ClimateModelName;
        temperateModel.Description = BuildClimateModelDescription(profile);
        temperateModel.MinuteProcessingInterval = 10;
        temperateModel.MinimumMinutesBetweenFlavourEchoes = 60;
        temperateModel.MinuteFlavourEchoChance = 0.01;
        temperateModel.Type = "terrestrial";
        foreach (ClimateModelSeason? existing in temperateModel.ClimateModelSeasons.ToList())
        {
            foreach (ClimateModelSeasonEvent? seasonEvent in existing.SeasonEvents.ToList())
            {
                existing.SeasonEvents.Remove(seasonEvent);
            }

            temperateModel.ClimateModelSeasons.Remove(existing);
        }
        foreach (Season season in seasons)
        {
            ClimateModelSeason cms = new()
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

            foreach (KeyValuePair<WeatherEvent, List<(WeatherEvent To, double Chance)>> weatherEventTransitions in eventsAndTransitionsBySeason[season.SeasonGroup])
            {
                (double EffectiveChangeChance, string TransitionXml) transitionDefinition = BuildTransitionDefinition(
                    weatherEventTransitions.Key,
                    weatherEventTransitions.Value,
                    0.01);
                cms.SeasonEvents.Add(new ClimateModelSeasonEvent
                {
                    ClimateModel = temperateModel,
                    Season = season,
                    WeatherEvent = weatherEventTransitions.Key,
                    ChangeChance = transitionDefinition.EffectiveChangeChance,
                    Transitions = transitionDefinition.TransitionXml
                });
            }
        }

        return temperateModel;
    }

    private static ClimateModel CreateClimateModels(FuturemudDatabaseContext context, List<Season> seasons, IReadOnlyDictionary<string, WeatherEvent> events, WeatherSeederClimateProfile profile)
    {
        #region Climate Models

        Dictionary<string, CollectionDictionary<WeatherEvent, (WeatherEvent To, double Chance)>> eventsAndTransitionsBySeason = new();
        eventsAndTransitionsBySeason["Winter"] = new();
        eventsAndTransitionsBySeason["Spring"] = new();
        eventsAndTransitionsBySeason["Summer"] = new();
        eventsAndTransitionsBySeason["Autumn"] = new();

        Dictionary<string, WindLevel> seasonalNormalWind = new()
        {
            { "Winter", WindLevel.OccasionalBreeze },
            { "Autumn", WindLevel.Breeze },
            { "Spring", WindLevel.OccasionalBreeze },
            { "Summer", WindLevel.OccasionalBreeze }
        };

        Dictionary<string, PrecipitationLevel> seasonalNormalPrecipitation = new()
        {
            { "Winter", PrecipitationLevel.Humid },
            { "Autumn", PrecipitationLevel.Humid },
            { "Spring", PrecipitationLevel.Humid },
            { "Summer", PrecipitationLevel.Humid }
        };

        double WindIncreaseChance(string seasonName, WindLevel currentWind)
        {
            int delta = currentWind.StepsFrom(seasonalNormalWind[seasonName]);
            if (delta < 0)
            {
                return 4.5 + Math.Abs(delta) * 1.5;
            }

            if (delta == 0)
            {
                return 0.9;
            }

            return Math.Max(0.1, 0.8 - delta * 0.15);
        }

        double WindDecreaseChance(string seasonName, WindLevel currentWind)
        {
            int delta = currentWind.StepsFrom(seasonalNormalWind[seasonName]);
            if (delta > 0)
            {
                return 4.5 + delta * 1.5;
            }

            if (delta == 0)
            {
                return 1.0;
            }

            return Math.Max(0.1, 0.9 - Math.Abs(delta) * 0.15);
        }

        double PrecipIncreaseChance(string seasonName, PrecipitationLevel currentPrecipitation, int stages = 1)
        {
            int delta = currentPrecipitation.StepsFrom(seasonalNormalPrecipitation[seasonName]);
            if (delta < 0)
            {
                return stages == 1
                    ? 5.0 + Math.Abs(delta) * 1.5
                    : 1.75 + Math.Abs(delta) * 0.75;
            }

            if (delta == 0)
            {
                return stages == 1 ? 1.8 : 0.4;
            }

            return Math.Max(
                stages == 1 ? 0.25 : 0.05,
                (stages == 1 ? 0.9 : 0.15) - delta * (stages == 1 ? 0.1 : 0.04)
            );
        }

        double PrecipDecreaseChance(string seasonName, PrecipitationLevel currentPrecipitation, int stages = 1)
        {
            int delta = currentPrecipitation.StepsFrom(seasonalNormalPrecipitation[seasonName]);
            if (delta > 0)
            {
                return stages == 1
                    ? 2.5 + delta * 0.75
                    : 0.8 + delta * 0.4;
            }

            if (delta == 0)
            {
                return stages == 1 ? 0.8 : 0.15;
            }

            return Math.Max(
                stages == 1 ? 0.25 : 0.05,
                (stages == 1 ? 0.9 : 0.15) - Math.Abs(delta) * (stages == 1 ? 0.1 : 0.04)
            );
        }

        double TemperatureVariationChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 3.0,
                "Winter" => 2.5,
                "Spring" => 2.0,
                "Summer" => 1.5,
                _ => 2.0
            };
        }

        double WindVariationChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 1.5,
                "Winter" => 1.0,
                "Spring" => 1.0,
                "Summer" => 0.5,
                _ => 1.0
            };
        }

        double CloudIncreaseChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 4.5,
                "Winter" => 4.0,
                "Spring" => 3.5,
                "Summer" => 3.0,
                _ => 3.5
            };
        }

        double CloudDecreaseChance(string seasonName)
        {
            return seasonName switch
            {
                "Autumn" => 2.2,
                "Winter" => 2.4,
                "Spring" => 2.3,
                "Summer" => 2.8,
                _ => 2.4
            };
        }

        foreach (KeyValuePair<string, WeatherEvent> we in events)
        {
            WindLevel wind = (WindLevel)we.Value.Wind;
            PrecipitationLevel precip = (PrecipitationLevel)we.Value.Precipitation;
            Match match = TempVariationRegex.Match(we.Key);
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

            WindVariation windVariation = we.Key.Contains("Polar") ? WindVariation.Polar :
                (we.Key.Contains("Equatorial") ? WindVariation.Equatorial : WindVariation.Normal);
            CloudVariation cloudVariation = CloudVariationRegex.IsMatch(we.Key) ?
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
                eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindIncreaseChance("Winter", wind)));
                eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindIncreaseChance("Autumn", wind)));
                eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindIncreaseChance("Summer", wind)));
                eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindIncreaseChance("Spring", wind)));
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
                eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindDecreaseChance("Winter", wind)));
                eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindDecreaseChance("Autumn", wind)));
                eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindDecreaseChance("Summer", wind)));
                eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindDecreaseChance("Spring", wind)));
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipIncreaseChance("Winter", precip)));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipIncreaseChance("Autumn", precip)));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipIncreaseChance("Summer", precip)));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipIncreaseChance("Spring", precip)));

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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipIncreaseChance("Winter", precip, 2)));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipIncreaseChance("Autumn", precip, 2)));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipIncreaseChance("Summer", precip, 2)));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipIncreaseChance("Spring", precip, 2)));
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 3.5));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 4.0));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 2.5));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 3.5));

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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 1.5));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 2.0));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.5));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 2.5));
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 4.0));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 4.5));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 3.0));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 3.5));

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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 1.5));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 2.0));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 1.0));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 1.2));

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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 0.7));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 0.05));
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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip)));

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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip)));
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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip)));
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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip, 2)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip, 2)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip, 2)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip, 2)));

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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip, 2)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip, 2)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip, 2)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip, 2)));
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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip, 2)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip, 2)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip, 2)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip, 2)));

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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip, 2)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip, 2)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip, 2)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip, 2)));
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

                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, PrecipDecreaseChance("Winter", precip)));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, PrecipDecreaseChance("Autumn", precip)));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, PrecipDecreaseChance("Summer", precip)));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, PrecipDecreaseChance("Spring", precip)));
                    }
                    break;
                case CloudVariation.Cloudy:
                    // Clear back to the underlying dry state
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 3.5));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 3.0));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 4.0));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 3.5));

                    // Occasionally clear further into notably dry weather
                    to = events[$"Parched{windVariation switch
                    {
                        WindVariation.Polar => "Polar",
                        WindVariation.Equatorial => "Equatorial",
                        _ => ""
                    }}{wind.DescribeEnum()}{variation switch
                    {
                        WeatherEventVariation.Normal => "",
                        _ => variation.DescribeEnum()
                    }}"];
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 0.2));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 0.1));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 1.2));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 0.3));
                    break;
                case CloudVariation.Overcast:
                    // Ease back to humid, with a smaller chance of the cloud deck thinning further
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 4.0));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 3.5));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 4.5));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 4.0));

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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, 1.5));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, 1.2));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, 2.0));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, 1.8));
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
                eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, TemperatureVariationChance("Winter")));
                eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, TemperatureVariationChance("Autumn")));
                eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, TemperatureVariationChance("Summer")));
                eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, TemperatureVariationChance("Spring")));
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
                eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, TemperatureVariationChance("Winter")));
                eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, TemperatureVariationChance("Autumn")));
                eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, TemperatureVariationChance("Summer")));
                eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, TemperatureVariationChance("Spring")));
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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));

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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));
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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));

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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));
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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));

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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, WindVariationChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, WindVariationChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, WindVariationChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, WindVariationChance("Spring")));
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
                        eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, CloudIncreaseChance("Winter")));
                        eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, CloudIncreaseChance("Autumn")));
                        eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, CloudIncreaseChance("Summer")));
                        eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, CloudIncreaseChance("Spring")));
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, CloudIncreaseChance("Winter")));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, CloudIncreaseChance("Autumn")));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, CloudIncreaseChance("Summer")));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, CloudIncreaseChance("Spring")));
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
                    eventsAndTransitionsBySeason["Winter"].Add(we.Value, (to, CloudDecreaseChance("Winter")));
                    eventsAndTransitionsBySeason["Autumn"].Add(we.Value, (to, CloudDecreaseChance("Autumn")));
                    eventsAndTransitionsBySeason["Summer"].Add(we.Value, (to, CloudDecreaseChance("Summer")));
                    eventsAndTransitionsBySeason["Spring"].Add(we.Value, (to, CloudDecreaseChance("Spring")));
                    break;
            }

        }

        ClimateModel temperateModel = new()
        {
            Name = "Temperate",
            Description = BuildClimateModelDescription(profile),
            MinuteProcessingInterval = 10,
            MinimumMinutesBetweenFlavourEchoes = 60,
            MinuteFlavourEchoChance = 0.01,
            Type = "terrestrial"
        };
        foreach (Season season in seasons)
        {
            ClimateModelSeason cms = new()
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

            foreach (KeyValuePair<WeatherEvent, List<(WeatherEvent To, double Chance)>> we in eventsAndTransitionsBySeason[season.SeasonGroup])
            {
                (double EffectiveChangeChance, string TransitionXml) transitionDefinition = BuildTransitionDefinition(
                    we.Key,
                    we.Value,
                    0.01);
                cms.SeasonEvents.Add(new ClimateModelSeasonEvent
                {
                    ClimateModel = temperateModel,
                    Season = season,
                    WeatherEvent = we.Key,
                    ChangeChance = transitionDefinition.EffectiveChangeChance,
                    Transitions = transitionDefinition.TransitionXml
                });
            }
        }
        context.ClimateModels.Add(temperateModel);
        context.SaveChanges();

        #endregion
        return temperateModel;
    }

    private WeatherSeederEventCatalog CreateWeatherEvents(FuturemudDatabaseContext context)
    {
        #region Weather Events

        Dictionary<string, WeatherEvent> eventsByName = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<WeatherSeederEventDescriptor, WeatherEvent> eventsByDescriptor = new();
        Dictionary<WeatherEvent, string> defaultTransitions = new();
        CollectionDictionary<WeatherEvent, string> echoes = new();

        foreach (PrecipitationLevel precipitation in Enum.GetValues<PrecipitationLevel>())
        {
            foreach (WindLevel wind in Enum.GetValues<WindLevel>())
            {
                CreateWeatherEvent(context, precipitation, wind, defaultTransitions, eventsByName, eventsByDescriptor);
            }
        }

        foreach (KeyValuePair<string, WeatherEvent> we in eventsByName)
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
        return new WeatherSeederEventCatalog
        {
            EventsByName = eventsByName,
            EventsByDescriptor = eventsByDescriptor,
            DescriptorsById = eventsByDescriptor.ToDictionary(x => x.Value.Id, x => x.Key)
        };
    }

    private List<Season> CreateSeasons(Celestial? celestial)
    {
        #region Seasons

        List<Season> seasons = new();

        void AddSeason(string name, string group, string displayName, int celestialDayOnset)
        {
            Season season = SeederRepeatabilityHelper.EnsureNamedEntity(
                _context.Seasons,
                name,
                x => x.Name,
                () =>
                {
                    Season created = new();
                    _context.Seasons.Add(created);
                    return created;
                });
            season.Name = name;
            season.DisplayName = displayName;
            season.SeasonGroup = group;
            season.CelestialId = celestial.Id;
            season.CelestialDayOnset = celestialDayOnset;
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
        #endregion
        return seasons;
    }

    /// <inheritdoc />
    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Accounts.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (!context.Celestials.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        return SeederRepeatabilityHelper.ClassifyByPresence(
        [
            StockWeatherEventNames.All(name => context.WeatherEvents.Any(x => x.Name == name)),
            StockSeasonNames.All(name => context.Seasons.Any(x => x.Name == name)),
            StockClimateModelNames.All(name => context.ClimateModels.Any(x => x.Name == name)),
            StockRegionalClimateNames.All(name => context.RegionalClimates.Any(x => x.Name == name))
        ]);
    }

    #endregion
}
