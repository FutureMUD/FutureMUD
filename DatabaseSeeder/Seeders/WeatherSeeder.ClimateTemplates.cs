using Microsoft.EntityFrameworkCore;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

readonly record struct WeatherSeederEventDescriptor(
    PrecipitationLevel Precipitation,
    WindLevel Wind,
    WeatherEventVariation TemperatureVariation,
    WindVariation WindVariation,
    CloudVariation CloudVariation);

public partial class WeatherSeeder
{
    private static double SeasonFactor(
        string seasonName,
        double winter = 1.0,
        double spring = 1.0,
        double summer = 1.0,
        double autumn = 1.0)
    {
        return seasonName switch
        {
            "Winter" => winter,
            "Spring" => spring,
            "Summer" => summer,
            "Autumn" => autumn,
            _ => 1.0
        };
    }

    private static Func<string, double> ScaleSeasonalChance(
        Func<string, double> baseFunc,
        double winter = 1.0,
        double spring = 1.0,
        double summer = 1.0,
        double autumn = 1.0,
        double add = 0.0,
        double minimum = 0.0)
    {
        return seasonName =>
            Math.Max(
                minimum,
                baseFunc(seasonName) * SeasonFactor(seasonName, winter, spring, summer, autumn) + add);
    }

    private static Func<string, WindLevel, double> ScaleSeasonalChance(
        Func<string, WindLevel, double> baseFunc,
        double winter = 1.0,
        double spring = 1.0,
        double summer = 1.0,
        double autumn = 1.0,
        double add = 0.0,
        double minimum = 0.0)
    {
        return (seasonName, wind) =>
            Math.Max(
                minimum,
                baseFunc(seasonName, wind) * SeasonFactor(seasonName, winter, spring, summer, autumn) + add);
    }

    private static Func<string, PrecipitationLevel, int, double> ScaleSeasonalChance(
        Func<string, PrecipitationLevel, int, double> baseFunc,
        double winter = 1.0,
        double spring = 1.0,
        double summer = 1.0,
        double autumn = 1.0,
        double add = 0.0,
        double minimum = 0.0)
    {
        return (seasonName, precipitation, stages) =>
            Math.Max(
                minimum,
                baseFunc(seasonName, precipitation, stages) * SeasonFactor(seasonName, winter, spring, summer, autumn) + add);
    }

    private static WeatherSeederClimateProfile CreateTemperateOceanicProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Temperate",
            RegionalClimatePrefix = "Oceanic Temperate",
            KoppenClimateClassification = "Cfb",
            ReferenceLocation = "London, United Kingdom",
            ClimateModelSummary = "This model represents a mild marine west coast climate with frequent cloud cover, regular rain in every season, and only modest swings between winter and summer. Weather changes most often in autumn and winter, when Atlantic-style fronts keep the sky unsettled and winds can turn blustery.",
            RegionalClimateSummary = "Expect cool summers, chilly winters, and plenty of damp grey days. Snow is possible but usually brief, while early and mid autumn are the most likely times for windy, stormier spells.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (4.0, 9.0),
                ["Mid Winter"] = (2.0, 8.0),
                ["Late Winter"] = (2.0, 8.0),
                ["Early Spring"] = (4.0, 11.0),
                ["Mid Spring"] = (6.0, 14.0),
                ["Late Spring"] = (9.0, 18.0),
                ["Early Summer"] = (12.0, 21.0),
                ["Mid Summer"] = (14.0, 24.0),
                ["Late Summer"] = (14.0, 23.0),
                ["Early Autumn"] = (11.0, 20.0),
                ["Mid Autumn"] = (8.0, 16.0),
                ["Late Autumn"] = (6.0, 13.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Autumn" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.5, 1.5, 0.9, 0.8, 0.15, 0.1),
                _ => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.5, 1.5, 0.9, 0.8, 0.15, 0.1)
            },
            WindDecreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Autumn" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.5, 1.5, 1.0, 0.9, 0.15, 0.1),
                _ => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.5, 1.5, 1.0, 0.9, 0.15, 0.1)
            },
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(currentPrecipitation.StepsFrom(PrecipitationLevel.Humid), stages, 5.0, 1.5, 1.75, 0.75, 1.8, 0.4, 0.9, 0.15, 0.1, 0.04, 0.25, 0.05),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(currentPrecipitation.StepsFrom(PrecipitationLevel.Humid), stages, 2.5, 0.75, 0.8, 0.4, 0.8, 0.15, 0.9, 0.15, 0.1, 0.04, 0.25, 0.05),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Autumn" => 3.0,
                "Winter" => 2.5,
                "Spring" => 2.0,
                "Summer" => 1.5,
                _ => 2.0
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Autumn" => 1.5,
                "Winter" => 1.0,
                "Spring" => 1.0,
                "Summer" => 0.5,
                _ => 1.0
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Autumn" => 4.5,
                "Winter" => 4.0,
                "Spring" => 3.5,
                "Summer" => 3.0,
                _ => 3.5
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Autumn" => 2.2,
                "Winter" => 2.4,
                "Spring" => 2.3,
                "Summer" => 2.8,
                _ => 2.4
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 3.5,
                "Autumn" => 4.0,
                "Summer" => 2.5,
                "Spring" => 3.5,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.5,
                "Autumn" => 2.0,
                "Summer" => 3.5,
                "Spring" => 2.5,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 4.0,
                "Autumn" => 4.5,
                "Summer" => 3.0,
                "Spring" => 3.5,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 1.5,
                "Autumn" => 2.0,
                "Summer" => 1.0,
                "Spring" => 1.2,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 0.7,
                "Autumn" => 0.05,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 3.5,
                "Autumn" => 3.0,
                "Summer" => 4.0,
                "Spring" => 3.5,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.2,
                "Autumn" => 0.1,
                "Summer" => 1.2,
                "Spring" => 0.3,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 4.0,
                "Autumn" => 3.5,
                "Summer" => 4.5,
                "Spring" => 4.0,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 1.5,
                "Autumn" => 1.2,
                "Summer" => 2.0,
                "Spring" => 1.8,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Autumn" => 0.15,
                "Spring" => 0.10,
                _ => 0.05
            }
        };
    }

    private static WeatherSeederClimateProfile CreateHumidSubtropicalProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Humid Subtropical",
            RegionalClimatePrefix = "Humid Subtropical",
            KoppenClimateClassification = "Cfa",
            ReferenceLocation = "Atlanta, Georgia, United States",
            ClimateModelSummary = "This model represents a warm, moisture-heavy climate with hot summers, muggy air, and rain in every season. Summer should feel thunderstorm-prone and sticky rather than dry, while winter stays generally mild with only brief cold snaps and rare snow.",
            RegionalClimateSummary = "Expect long warm-to-hot seasons, short mild winters, and a year-round sense of humidity. Spring and summer are the wettest-feeling parts of the year, while autumn turns somewhat calmer without becoming truly dry.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (4.0, 13.5),
                ["Mid Winter"] = (2.0, 12.0),
                ["Late Winter"] = (4.0, 15.0),
                ["Early Spring"] = (7.0, 19.0),
                ["Mid Spring"] = (11.0, 23.0),
                ["Late Spring"] = (16.0, 27.0),
                ["Early Summer"] = (20.0, 31.0),
                ["Mid Summer"] = (22.0, 32.5),
                ["Late Summer"] = (22.0, 32.0),
                ["Early Autumn"] = (19.0, 29.0),
                ["Mid Autumn"] = (13.0, 24.0),
                ["Late Autumn"] = (7.0, 18.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 3.8, 1.2, seasonName == "Spring" ? 0.75 : seasonName == "Summer" ? 0.65 : 0.6, 0.55, 0.12, 0.05),
            WindDecreaseChance = (seasonName, currentWind) => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 5.0, 1.8, 1.3, 1.0, 0.12, 0.15),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName is "Spring" or "Summer" ? PrecipitationLevel.LightRain : PrecipitationLevel.Humid),
                    stages,
                    5.3,
                    1.75,
                    2.0,
                    0.9,
                    seasonName == "Summer" ? 2.3 : seasonName == "Spring" ? 2.1 : 1.5,
                    seasonName == "Summer" ? 0.75 : seasonName == "Spring" ? 0.65 : 0.3,
                    seasonName == "Summer" ? 1.2 : seasonName == "Spring" ? 1.1 : 0.7,
                    seasonName == "Summer" ? 0.25 : seasonName == "Spring" ? 0.22 : 0.10,
                    0.12,
                    0.05,
                    0.2,
                    0.04),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName is "Spring" or "Summer" ? PrecipitationLevel.LightRain : PrecipitationLevel.Humid),
                    stages,
                    2.1,
                    0.65,
                    0.65,
                    0.35,
                    seasonName == "Summer" ? 0.65 : seasonName == "Spring" ? 0.7 : 0.9,
                    seasonName == "Summer" ? 0.10 : seasonName == "Spring" ? 0.12 : 0.18,
                    0.75,
                    0.12,
                    0.08,
                    0.03,
                    0.15,
                    0.03),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Winter" => 2.2,
                "Spring" => 1.9,
                "Summer" => 1.4,
                "Autumn" => 2.1,
                _ => 1.8
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Spring" => 0.8,
                "Summer" => 0.45,
                "Autumn" => 0.7,
                "Winter" => 0.6,
                _ => 0.6
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Winter" => 3.8,
                "Spring" => 4.3,
                "Summer" => 4.8,
                "Autumn" => 3.6,
                _ => 3.8
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Winter" => 2.1,
                "Spring" => 1.8,
                "Summer" => 1.5,
                "Autumn" => 1.9,
                _ => 1.9
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 3.8,
                "Spring" => 4.4,
                "Summer" => 4.8,
                "Autumn" => 3.4,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.7,
                "Spring" => 1.4,
                "Summer" => 1.1,
                "Autumn" => 1.8,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 3.4,
                "Spring" => 4.5,
                "Summer" => 5.2,
                "Autumn" => 3.1,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 1.1,
                "Spring" => 1.8,
                "Summer" => 2.3,
                "Autumn" => 1.2,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 0.12,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 2.4,
                "Spring" => 2.0,
                "Summer" => 1.4,
                "Autumn" => 2.0,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.03,
                "Spring" => 0.05,
                "Summer" => 0.10,
                "Autumn" => 0.08,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 4.3,
                "Spring" => 3.8,
                "Summer" => 3.2,
                "Autumn" => 4.1,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 1.1,
                "Spring" => 1.3,
                "Summer" => 1.5,
                "Autumn" => 1.2,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Spring" => 0.10,
                "Summer" => 0.08,
                "Autumn" => 0.07,
                _ => 0.05
            },
            IncrementalAdditionalChangeChanceFromStableWeather = 0.0004
        };
    }

    private static WeatherSeederClimateProfile CreateHumidSubcontinentalProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Humid Subcontinental",
            RegionalClimatePrefix = "Humid Subcontinental",
            KoppenClimateClassification = "Dfb",
            ReferenceLocation = "Buffalo, New York, United States",
            ClimateModelSummary = "This model represents a cool-summer humid continental climate with cold snowy winters, warm summers, and year-round precipitation. Storm tracks can bring mixed conditions in spring and autumn, while winter supports regular snow cover without shutting rain out entirely.",
            RegionalClimateSummary = "Expect a large seasonal contrast, with freezing winters and comfortable to warm summers. Snow and overcast conditions are common in the cold half of the year, while summer is greener, wetter, and less severe than a dry continental interior.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-5.0, 3.0),
                ["Mid Winter"] = (-8.0, 1.0),
                ["Late Winter"] = (-8.0, 2.0),
                ["Early Spring"] = (-3.0, 7.0),
                ["Mid Spring"] = (3.0, 13.0),
                ["Late Spring"] = (9.0, 20.0),
                ["Early Summer"] = (14.0, 24.0),
                ["Mid Summer"] = (17.0, 27.0),
                ["Late Summer"] = (16.0, 26.0),
                ["Early Autumn"] = (12.0, 22.0),
                ["Mid Autumn"] = (7.0, 15.0),
                ["Late Autumn"] = (2.0, 9.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Winter" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.8, 1.6, 1.0, 0.9, 0.14, 0.10),
                "Autumn" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.6, 1.5, 0.95, 0.85, 0.13, 0.10),
                "Spring" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.4, 1.4, 0.85, 0.75, 0.12, 0.08),
                _ => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.0, 1.3, 0.75, 0.65, 0.10, 0.08)
            },
            WindDecreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Winter" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.8, 1.6, 1.2, 1.0, 0.14, 0.12),
                "Autumn" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.6, 1.5, 1.1, 0.95, 0.13, 0.12),
                "Spring" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.4, 1.3, 1.0, 0.95, 0.12, 0.12),
                _ => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.2, 1.2, 0.95, 0.9, 0.10, 0.12)
            },
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(PrecipitationLevel.Humid),
                    stages,
                    5.4,
                    1.6,
                    2.1,
                    0.85,
                    seasonName switch
                    {
                        "Winter" => 1.9,
                        "Spring" => 1.8,
                        "Summer" => 2.0,
                        "Autumn" => 1.8,
                        _ => 1.8
                    },
                    seasonName switch
                    {
                        "Winter" => 0.45,
                        "Spring" => 0.40,
                        "Summer" => 0.48,
                        "Autumn" => 0.40,
                        _ => 0.40
                    },
                    seasonName switch
                    {
                        "Winter" => 1.0,
                        "Spring" => 0.95,
                        "Summer" => 1.0,
                        "Autumn" => 0.95,
                        _ => 0.95
                    },
                    seasonName switch
                    {
                        "Winter" => 0.18,
                        "Spring" => 0.16,
                        "Summer" => 0.18,
                        "Autumn" => 0.16,
                        _ => 0.16
                    },
                    0.12,
                    0.05,
                    0.16,
                    0.04),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(PrecipitationLevel.Humid),
                    stages,
                    2.3,
                    0.75,
                    0.80,
                    0.35,
                    seasonName switch
                    {
                        "Winter" => 0.85,
                        "Spring" => 0.80,
                        "Summer" => 0.75,
                        "Autumn" => 0.80,
                        _ => 0.80
                    },
                    seasonName switch
                    {
                        "Winter" => 0.14,
                        "Spring" => 0.12,
                        "Summer" => 0.12,
                        "Autumn" => 0.12,
                        _ => 0.12
                    },
                    0.72,
                    0.12,
                    0.08,
                    0.03,
                    0.16,
                    0.03),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Winter" => 2.8,
                "Spring" => 2.2,
                "Summer" => 1.4,
                "Autumn" => 2.3,
                _ => 2.0
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Winter" => 1.0,
                "Spring" => 0.9,
                "Summer" => 0.5,
                "Autumn" => 0.9,
                _ => 0.8
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Winter" => 4.4,
                "Spring" => 3.6,
                "Summer" => 3.5,
                "Autumn" => 4.1,
                _ => 3.8
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Winter" => 1.8,
                "Spring" => 2.1,
                "Summer" => 2.3,
                "Autumn" => 2.0,
                _ => 2.0
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 4.4,
                "Spring" => 3.5,
                "Summer" => 3.2,
                "Autumn" => 4.0,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.8,
                "Spring" => 2.2,
                "Summer" => 2.0,
                "Autumn" => 2.0,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 1.2,
                "Spring" => 3.8,
                "Summer" => 4.0,
                "Autumn" => 3.4,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 0.2,
                "Spring" => 1.4,
                "Summer" => 1.6,
                "Autumn" => 1.2,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 5.8,
                "Autumn" => 0.35,
                "Spring" => 0.22,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 1.4,
                "Spring" => 2.0,
                "Summer" => 2.1,
                "Autumn" => 1.6,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.01,
                "Spring" => 0.02,
                "Summer" => 0.05,
                "Autumn" => 0.02,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 4.4,
                "Spring" => 4.0,
                "Summer" => 3.8,
                "Autumn" => 4.2,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 0.9,
                "Spring" => 1.1,
                "Summer" => 1.2,
                "Autumn" => 1.0,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Winter" => 0.09,
                "Spring" => 0.08,
                "Summer" => 0.06,
                "Autumn" => 0.10,
                _ => 0.08
            },
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00045
        };
    }

    private static WeatherSeederClimateProfile CreateDryWinterHumidSubcontinentalProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Dry Winter Humid Subcontinental",
            RegionalClimatePrefix = "Dry Winter Humid Subcontinental",
            KoppenClimateClassification = "Dwa",
            ReferenceLocation = "Seoul, South Korea",
            ClimateModelSummary = "This model represents the East Asian dry-winter monsoonal continental pattern: cold dry winters, hot humid summers, and a sharp ramp-up of rain once the warm season arrives. Spring should feel changeable and breezy, while the heaviest wet weather is concentrated in high summer.",
            RegionalClimateSummary = "Expect clear, comparatively dry winters followed by a steep seasonal swing into hot, muggy summer weather. Late spring and summer feel stormier and wetter, while winter snow remains possible but the air is usually too dry for constant precipitation.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-2.0, 5.0),
                ["Mid Winter"] = (-6.0, 2.0),
                ["Late Winter"] = (-3.5, 5.0),
                ["Early Spring"] = (1.5, 11.0),
                ["Mid Spring"] = (8.0, 18.0),
                ["Late Spring"] = (13.5, 23.5),
                ["Early Summer"] = (18.5, 28.0),
                ["Mid Summer"] = (22.0, 30.5),
                ["Late Summer"] = (22.5, 31.0),
                ["Early Autumn"] = (18.0, 27.0),
                ["Mid Autumn"] = (11.0, 21.0),
                ["Late Autumn"] = (4.0, 13.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Spring" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.8, 1.6, 0.95, 0.85, 0.14, 0.08),
                "Winter" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.6, 1.5, 0.90, 0.80, 0.12, 0.08),
                "Autumn" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.2, 1.3, 0.80, 0.70, 0.12, 0.08),
                _ => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 3.7, 1.1, 0.65, 0.55, 0.10, 0.06)
            },
            WindDecreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Spring" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.8, 1.6, 1.2, 1.0, 0.14, 0.12),
                "Winter" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.Breeze), 4.6, 1.5, 1.1, 0.95, 0.12, 0.12),
                "Autumn" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.4, 1.3, 1.0, 0.95, 0.12, 0.12),
                _ => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.0, 1.2, 0.95, 0.90, 0.10, 0.12)
            },
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Dry,
                        "Spring" => PrecipitationLevel.Humid,
                        "Summer" => PrecipitationLevel.LightRain,
                        "Autumn" => PrecipitationLevel.Humid,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    5.4,
                    1.6,
                    2.1,
                    0.85,
                    seasonName switch
                    {
                        "Winter" => 0.8,
                        "Spring" => 1.5,
                        "Summer" => 2.7,
                        "Autumn" => 1.7,
                        _ => 1.5
                    },
                    seasonName switch
                    {
                        "Winter" => 0.05,
                        "Spring" => 0.25,
                        "Summer" => 1.0,
                        "Autumn" => 0.25,
                        _ => 0.20
                    },
                    seasonName switch
                    {
                        "Winter" => 0.55,
                        "Spring" => 0.75,
                        "Summer" => 1.30,
                        "Autumn" => 0.85,
                        _ => 0.75
                    },
                    seasonName switch
                    {
                        "Winter" => 0.04,
                        "Spring" => 0.10,
                        "Summer" => 0.25,
                        "Autumn" => 0.12,
                        _ => 0.10
                    },
                    0.08,
                    0.02,
                    0.08,
                    0.02),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Dry,
                        "Spring" => PrecipitationLevel.Humid,
                        "Summer" => PrecipitationLevel.LightRain,
                        "Autumn" => PrecipitationLevel.Humid,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    2.6,
                    0.85,
                    0.9,
                    0.4,
                    seasonName switch
                    {
                        "Winter" => 1.5,
                        "Spring" => 0.95,
                        "Summer" => 0.55,
                        "Autumn" => 0.85,
                        _ => 0.85
                    },
                    seasonName switch
                    {
                        "Winter" => 0.30,
                        "Spring" => 0.14,
                        "Summer" => 0.08,
                        "Autumn" => 0.12,
                        _ => 0.12
                    },
                    0.85,
                    0.14,
                    0.08,
                    0.03,
                    0.16,
                    0.03),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Winter" => 2.7,
                "Spring" => 2.2,
                "Summer" => 1.4,
                "Autumn" => 2.0,
                _ => 2.0
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Spring" => 1.1,
                "Winter" => 0.9,
                "Autumn" => 0.8,
                "Summer" => 0.5,
                _ => 0.8
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Winter" => 2.1,
                "Spring" => 2.6,
                "Summer" => 5.6,
                "Autumn" => 3.0,
                _ => 2.8
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Winter" => 3.8,
                "Spring" => 2.8,
                "Summer" => 1.2,
                "Autumn" => 2.2,
                _ => 2.5
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 1.2,
                "Spring" => 2.3,
                "Summer" => 5.4,
                "Autumn" => 2.8,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.0,
                "Spring" => 1.1,
                "Summer" => 0.8,
                "Autumn" => 1.5,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 0.35,
                "Spring" => 1.7,
                "Summer" => 5.8,
                "Autumn" => 2.4,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 0.05,
                "Spring" => 0.4,
                "Summer" => 2.6,
                "Autumn" => 0.8,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 2.4,
                "Spring" => 0.05,
                "Autumn" => 0.12,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 4.2,
                "Spring" => 2.6,
                "Summer" => 0.7,
                "Autumn" => 2.0,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.20,
                "Spring" => 0.08,
                "Summer" => 0.00,
                "Autumn" => 0.05,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.6,
                "Spring" => 2.0,
                "Summer" => 2.8,
                "Autumn" => 2.2,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 2.0,
                "Spring" => 1.6,
                "Summer" => 1.0,
                "Autumn" => 1.4,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Winter" => 0.06,
                "Spring" => 0.08,
                "Summer" => 0.09,
                "Autumn" => 0.07,
                _ => 0.07
            },
            IncrementalAdditionalChangeChanceFromStableWeather = 0.0004
        };
    }

    private static WeatherSeederClimateProfile CreateSubarcticProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Subarctic",
            RegionalClimatePrefix = "Subarctic",
            KoppenClimateClassification = "Dfc",
            ReferenceLocation = "Bratsk, Russia",
            ClimateModelSummary = "This model represents a boreal subarctic climate with very long, bitter winters, a short mild summer, and modest overall precipitation. Snow dominates much of the year, but the wettest-feeling weather arrives in the brief warm season rather than midwinter.",
            RegionalClimateSummary = "Expect an extreme annual temperature range, with deep winter cold and only a short window of gentle summer warmth. Autumn cools rapidly, spring stays hesitant, and windy or snowy weather can return quickly outside the core summer period.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-21.0, -13.0),
                ["Mid Winter"] = (-24.5, -15.0),
                ["Late Winter"] = (-21.0, -11.0),
                ["Early Spring"] = (-13.0, -3.0),
                ["Mid Spring"] = (-4.0, 6.0),
                ["Late Spring"] = (3.0, 13.0),
                ["Early Summer"] = (10.0, 20.0),
                ["Mid Summer"] = (13.0, 24.0),
                ["Late Summer"] = (10.5, 21.0),
                ["Early Autumn"] = (4.0, 13.0),
                ["Mid Autumn"] = (-2.5, 4.5),
                ["Late Autumn"] = (-13.0, -5.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Winter" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.4, 1.4, 0.75, 0.65, 0.10, 0.05),
                "Spring" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.2, 1.3, 0.75, 0.65, 0.10, 0.05),
                "Autumn" => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.0, 1.3, 0.70, 0.60, 0.10, 0.05),
                _ => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 3.8, 1.2, 0.60, 0.50, 0.08, 0.05)
            },
            WindDecreaseChance = (seasonName, currentWind) => seasonName switch
            {
                "Winter" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 5.0, 1.6, 1.15, 1.0, 0.12, 0.12),
                "Spring" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.8, 1.5, 1.10, 0.95, 0.12, 0.12),
                "Autumn" => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.8, 1.5, 1.05, 0.95, 0.12, 0.12),
                _ => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.6, 1.4, 1.0, 0.90, 0.10, 0.12)
            },
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Dry,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    5.2,
                    1.5,
                    2.0,
                    0.8,
                    seasonName switch
                    {
                        "Winter" => 1.1,
                        "Spring" => 1.6,
                        "Summer" => 1.9,
                        "Autumn" => 1.7,
                        _ => 1.5
                    },
                    seasonName switch
                    {
                        "Winter" => 0.08,
                        "Spring" => 0.26,
                        "Summer" => 0.32,
                        "Autumn" => 0.22,
                        _ => 0.2
                    },
                    seasonName switch
                    {
                        "Winter" => 0.70,
                        "Spring" => 0.85,
                        "Summer" => 0.95,
                        "Autumn" => 0.90,
                        _ => 0.85
                    },
                    seasonName switch
                    {
                        "Winter" => 0.08,
                        "Spring" => 0.12,
                        "Summer" => 0.14,
                        "Autumn" => 0.12,
                        _ => 0.12
                    },
                    0.10,
                    0.04,
                    0.12,
                    0.04),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Dry,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    2.8,
                    0.9,
                    0.95,
                    0.45,
                    seasonName switch
                    {
                        "Winter" => 1.25,
                        "Spring" => 0.95,
                        "Summer" => 0.80,
                        "Autumn" => 0.90,
                        _ => 0.9
                    },
                    seasonName switch
                    {
                        "Winter" => 0.22,
                        "Spring" => 0.16,
                        "Summer" => 0.14,
                        "Autumn" => 0.16,
                        _ => 0.16
                    },
                    0.85,
                    0.14,
                    0.08,
                    0.03,
                    0.18,
                    0.04),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Winter" => 3.2,
                "Spring" => 2.5,
                "Summer" => 1.6,
                "Autumn" => 2.6,
                _ => 2.2
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Winter" => 0.8,
                "Spring" => 0.7,
                "Summer" => 0.4,
                "Autumn" => 0.7,
                _ => 0.6
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Winter" => 2.5,
                "Spring" => 3.1,
                "Summer" => 3.8,
                "Autumn" => 3.7,
                _ => 3.2
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Winter" => 3.0,
                "Spring" => 2.5,
                "Summer" => 2.2,
                "Autumn" => 2.2,
                _ => 2.4
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 2.2,
                "Spring" => 2.7,
                "Summer" => 3.5,
                "Autumn" => 3.6,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 0.9,
                "Spring" => 1.6,
                "Summer" => 1.9,
                "Autumn" => 1.7,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 0.25,
                "Spring" => 2.0,
                "Summer" => 2.5,
                "Autumn" => 2.0,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 0.03,
                "Spring" => 0.55,
                "Summer" => 0.80,
                "Autumn" => 0.50,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 10.0,
                "Spring" => 0.8,
                "Autumn" => 1.25,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 3.6,
                "Spring" => 2.3,
                "Summer" => 1.6,
                "Autumn" => 1.8,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.12,
                "Spring" => 0.05,
                "Summer" => 0.03,
                "Autumn" => 0.05,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 2.6,
                "Spring" => 3.0,
                "Summer" => 3.3,
                "Autumn" => 3.2,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 1.5,
                "Spring" => 1.3,
                "Summer" => 1.1,
                "Autumn" => 1.1,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Winter" => 0.06,
                "Spring" => 0.07,
                "Summer" => 0.06,
                "Autumn" => 0.08,
                _ => 0.07
            },
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00038
        };
    }

    private static WeatherSeederClimateProfile CreateMediterraneanProfile()
    {
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Mediterranean",
            RegionalClimatePrefix = "Mediterranean",
            KoppenClimateClassification = "Csa",
            ReferenceLocation = "Sacramento, California, United States",
            ClimateModelSummary = "This model represents a dry-summer Mediterranean climate with clear, stable warm seasons and a rainy cool season. Most unsettled weather should cluster in late autumn through winter, while summer is dominated by dry air, brighter skies, and only rare interruptions.",
            RegionalClimateSummary = "Expect hot dry summers, mild wetter winters, and a shoulder season pattern where autumn marks the return of the rains. Snow is rare, summer cloud cover is limited, and the climate should feel much calmer and sunnier than an oceanic coast.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (4.0, 13.0),
                ["Mid Winter"] = (4.0, 13.5),
                ["Late Winter"] = (5.0, 16.0),
                ["Early Spring"] = (7.0, 19.0),
                ["Mid Spring"] = (9.0, 22.0),
                ["Late Spring"] = (12.0, 27.0),
                ["Early Summer"] = (15.0, 32.0),
                ["Mid Summer"] = (14.0, 34.0),
                ["Late Summer"] = (14.0, 33.5),
                ["Early Autumn"] = (13.0, 32.0),
                ["Mid Autumn"] = (9.0, 26.0),
                ["Late Autumn"] = (6.0, 18.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => WindChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 3.6, 1.1, 0.6, 0.5, 0.10, 0.05),
            WindDecreaseChance = (seasonName, currentWind) => WindDecreaseChanceByDelta(currentWind.StepsFrom(WindLevel.OccasionalBreeze), 4.8, 1.5, 1.2, 0.9, 0.10, 0.15),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipIncreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Humid,
                        "Spring" => PrecipitationLevel.Humid,
                        "Summer" => PrecipitationLevel.Dry,
                        "Autumn" => PrecipitationLevel.Dry,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    5.4,
                    1.6,
                    2.1,
                    0.8,
                    seasonName switch
                    {
                        "Winter" => 2.2,
                        "Spring" => 1.4,
                        "Summer" => 0.35,
                        "Autumn" => 0.8,
                        _ => 1.2
                    },
                    seasonName switch
                    {
                        "Winter" => 0.8,
                        "Spring" => 0.35,
                        "Summer" => 0.03,
                        "Autumn" => 0.10,
                        _ => 0.2
                    },
                    seasonName switch
                    {
                        "Winter" => 1.1,
                        "Spring" => 0.85,
                        "Summer" => 0.25,
                        "Autumn" => 0.4,
                        _ => 0.7
                    },
                    seasonName switch
                    {
                        "Winter" => 0.22,
                        "Spring" => 0.14,
                        "Summer" => 0.02,
                        "Autumn" => 0.04,
                        _ => 0.10
                    },
                    0.12,
                    0.05,
                    0.1,
                    0.02),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) =>
                PrecipDecreaseChanceByDelta(
                    currentPrecipitation.StepsFrom(seasonName switch
                    {
                        "Winter" => PrecipitationLevel.Humid,
                        "Spring" => PrecipitationLevel.Humid,
                        "Summer" => PrecipitationLevel.Dry,
                        "Autumn" => PrecipitationLevel.Dry,
                        _ => PrecipitationLevel.Humid
                    }),
                    stages,
                    2.4,
                    0.8,
                    0.8,
                    0.35,
                    seasonName switch
                    {
                        "Winter" => 0.95,
                        "Spring" => 1.0,
                        "Summer" => 1.6,
                        "Autumn" => 1.4,
                        _ => 0.9
                    },
                    seasonName switch
                    {
                        "Winter" => 0.12,
                        "Spring" => 0.18,
                        "Summer" => 0.35,
                        "Autumn" => 0.30,
                        _ => 0.16
                    },
                    0.75,
                    0.14,
                    0.08,
                    0.03,
                    0.18,
                    0.04),
            TemperatureVariationChance = seasonName => seasonName switch
            {
                "Winter" => 2.1,
                "Spring" => 2.0,
                "Summer" => 1.7,
                "Autumn" => 2.4,
                _ => 2.0
            },
            WindVariationChance = seasonName => seasonName switch
            {
                "Winter" => 0.7,
                "Spring" => 0.6,
                "Summer" => 0.4,
                "Autumn" => 0.6,
                _ => 0.6
            },
            CloudIncreaseChance = seasonName => seasonName switch
            {
                "Winter" => 4.2,
                "Spring" => 2.4,
                "Summer" => 0.9,
                "Autumn" => 1.7,
                _ => 2.6
            },
            CloudDecreaseChance = seasonName => seasonName switch
            {
                "Winter" => 1.5,
                "Spring" => 3.2,
                "Summer" => 5.0,
                "Autumn" => 4.3,
                _ => 2.8
            },
            CloudyToOvercastChance = seasonName => seasonName switch
            {
                "Winter" => 4.0,
                "Spring" => 2.1,
                "Summer" => 0.4,
                "Autumn" => 1.5,
                _ => 0.0
            },
            CloudyToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 1.4,
                "Spring" => 1.8,
                "Summer" => 0.6,
                "Autumn" => 0.8,
                _ => 0.0
            },
            OvercastToLightRainChance = seasonName => seasonName switch
            {
                "Winter" => 4.2,
                "Spring" => 2.0,
                "Summer" => 0.15,
                "Autumn" => 0.85,
                _ => 0.0
            },
            OvercastToRainChance = seasonName => seasonName switch
            {
                "Winter" => 1.6,
                "Spring" => 0.8,
                "Summer" => 0.0,
                "Autumn" => 0.28,
                _ => 0.0
            },
            OvercastToLightSnowChance = seasonName => seasonName switch
            {
                "Winter" => 0.08,
                _ => 0.0
            },
            CloudyToDryChance = seasonName => seasonName switch
            {
                "Winter" => 1.6,
                "Spring" => 3.0,
                "Summer" => 5.2,
                "Autumn" => 4.5,
                _ => 0.0
            },
            CloudyToParchedChance = seasonName => seasonName switch
            {
                "Winter" => 0.02,
                "Spring" => 0.15,
                "Summer" => 2.2,
                "Autumn" => 1.2,
                _ => 0.0
            },
            OvercastToHumidChance = seasonName => seasonName switch
            {
                "Winter" => 4.8,
                "Spring" => 2.8,
                "Summer" => 1.6,
                "Autumn" => 2.0,
                _ => 0.0
            },
            OvercastToCloudyChance = seasonName => seasonName switch
            {
                "Winter" => 0.8,
                "Spring" => 1.8,
                "Summer" => 3.0,
                "Autumn" => 2.2,
                _ => 0.0
            },
            MaximumAdditionalChangeChance = seasonName => seasonName switch
            {
                "Winter" => 0.08,
                "Spring" => 0.09,
                "Summer" => 0.06,
                "Autumn" => 0.10,
                _ => 0.08
            },
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00045
        };
    }

    private static WeatherSeederClimateProfile CreateTundraProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Tundra",
            RegionalClimatePrefix = "Tundra",
            KoppenClimateClassification = "ET",
            ReferenceLocation = "Nuuk, Greenland",
            ClimateModelSummary = "This model represents a polar tundra climate where even summer stays cool, winters are harsh, and the atmosphere often hovers near freezing. Rain is limited and much of the year is spent in snow, sleet, damp cold, and strong coastal winds rather than in heavy warm-season storms.",
            RegionalClimateSummary = "Expect a cold maritime polar environment with a brief thaw season and a long, raw cold season. Summers are cool rather than warm, snow can linger far outside winter, and wind exposure makes autumn and winter feel especially severe.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-10.0, -2.0),
                ["Mid Winter"] = (-12.0, -3.0),
                ["Late Winter"] = (-11.0, -2.0),
                ["Early Spring"] = (-8.0, 0.0),
                ["Mid Spring"] = (-3.0, 4.0),
                ["Late Spring"] = (1.0, 7.0),
                ["Early Summer"] = (4.0, 10.0),
                ["Mid Summer"] = (5.0, 11.0),
                ["Late Summer"] = (4.5, 10.5),
                ["Early Autumn"] = (2.0, 7.0),
                ["Mid Autumn"] = (-1.0, 4.0),
                ["Late Autumn"] = (-6.0, 0.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.15, 1.05, 1.0, 1.15),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.90, 0.95, 1.0, 0.90),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.25, 1.10, 1.05, 1.30),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.82, 0.92, 0.95, 0.82),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 0.95, 0.95, 0.90, 0.95),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.05, 1.0, 1.0, 1.05),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 1.28, 1.18, 1.10, 1.30),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 0.72, 0.80, 0.85, 0.72),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 1.25, 1.15, 1.10, 1.30),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 1.25, 1.15, 1.10, 1.25),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 0.65, 0.95, 1.05, 1.20),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 0.30, 0.80, 0.95, 1.10),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonalScale(seasonName, 1.55, 1.25, 0.05, 1.80),
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 0.70, 0.78, 0.82, 0.70),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 0.18, 0.20, 0.18, 0.18),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 1.18, 1.10, 1.05, 1.20),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 0.88, 0.90, 0.95, 0.88),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.95, 0.95, 0.95, 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00036
        };
    }

    private static WeatherSeederClimateProfile CreatePolarIceCapProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateTundraProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Polar Ice Cap",
            RegionalClimatePrefix = "Polar Ice Cap",
            KoppenClimateClassification = "EF",
            ReferenceLocation = "Amundsen-Scott South Pole Station, Antarctica",
            ClimateModelSummary = "This model represents a permanent ice-cap climate where thaw conditions are exceptional, the air is extremely dry, and snow is usually light rather than wet. The system suppresses rain almost entirely and keeps the world in a state of cold, barren, wind-scoured weather.",
            RegionalClimateSummary = "Expect relentless cold in every season. Even the warmest part of the year remains below freezing, snowfall is usually fine and dry, and any increase in wind quickly turns the climate from merely hostile to punishing.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-42.0, -32.0),
                ["Mid Winter"] = (-60.0, -50.0),
                ["Late Winter"] = (-55.0, -45.0),
                ["Early Spring"] = (-58.0, -46.0),
                ["Mid Spring"] = (-54.0, -42.0),
                ["Late Spring"] = (-46.0, -34.0),
                ["Early Summer"] = (-38.0, -26.0),
                ["Mid Summer"] = (-33.0, -20.0),
                ["Late Summer"] = (-40.0, -28.0),
                ["Early Autumn"] = (-48.0, -36.0),
                ["Mid Autumn"] = (-56.0, -44.0),
                ["Late Autumn"] = (-50.0, -38.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.25, 1.20, 1.15, 1.25),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.82, 0.85, 0.88, 0.82),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.03, 0.03, 0.04, 0.03),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.65, 1.60, 1.55, 1.60),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 1.05, 1.05, 1.0, 1.05),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.15, 1.10, 1.05, 1.15),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 0.24, 0.22, 0.26, 0.24),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 1.55, 1.50, 1.40, 1.50),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 0.22, 0.20, 0.24, 0.22),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 0.01, 0.01, 0.015, 0.01),
            OvercastToLightRainChance = seasonName => 0.0,
            OvercastToRainChance = seasonName => 0.0,
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonalScale(seasonName, 0.80, 0.65, 0.90, 0.75),
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 1.70, 1.85, 1.65, 1.80),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 3.0, 3.2, 3.0, 3.0),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 0.05, 0.04, 0.06, 0.05),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 1.45, 1.45, 1.35, 1.40),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.85, 0.85, 0.85, 0.85),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00032
        };
    }

    private static WeatherSeederClimateProfile CreateHotSemiAridProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubtropicalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Hot Semi-Arid",
            RegionalClimatePrefix = "Hot Semi-Arid",
            KoppenClimateClassification = "BSh",
            ReferenceLocation = "Del Rio, Texas, United States",
            ClimateModelSummary = "This model represents a hot steppe climate on the dry side of subtropical weather, with long hot seasons, scarce rainfall, and occasional bursts of storm activity rather than dependable wet periods. Dry and parched states should dominate, but the region still receives more meaningful rain than a true desert.",
            RegionalClimateSummary = "Expect hot, thirsty weather for most of the year, with brief greener periods after rain rather than a lasting wet season. Winters are milder and somewhat more forgiving, but the overall feel remains open, dry, and heat-stressed.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (7.0, 19.0),
                ["Mid Winter"] = (6.0, 18.0),
                ["Late Winter"] = (9.0, 22.0),
                ["Early Spring"] = (14.0, 27.0),
                ["Mid Spring"] = (18.0, 31.0),
                ["Late Spring"] = (22.0, 35.0),
                ["Early Summer"] = (25.0, 38.0),
                ["Mid Summer"] = (26.0, 39.0),
                ["Late Summer"] = (25.0, 38.0),
                ["Early Autumn"] = (22.0, 35.0),
                ["Mid Autumn"] = (16.0, 29.0),
                ["Late Autumn"] = (10.0, 23.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.05, 1.10, 1.12, 1.08),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.95, 0.95, 0.92, 0.95),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.20, 0.34, 0.25, 0.38),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.80, 1.52, 1.28, 1.38),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 1.0, 1.08, 1.10, 1.05),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.05, 1.10, 1.10, 1.05),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 0.28, 0.42, 0.35, 0.46),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 1.58, 1.38, 1.20, 1.28),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 0.28, 0.42, 0.30, 0.48),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 0.20, 0.28, 0.22, 0.32),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 0.30, 0.50, 0.45, 0.72),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 0.18, 0.38, 0.35, 0.60),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonalScale(seasonName, 0.04, 0.02, 0.0, 0.01),
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 1.95, 1.68, 1.42, 1.56),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 3.0, 2.5, 2.0, 2.2),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 0.30, 0.42, 0.32, 0.48),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 1.18, 1.12, 1.08, 1.12),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.95, 0.95, 0.90, 0.92),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00042
        };
    }

    private static WeatherSeederClimateProfile CreateColdSemiAridProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubcontinentalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Cold Semi-Arid",
            RegionalClimatePrefix = "Cold Semi-Arid",
            KoppenClimateClassification = "BSk",
            ReferenceLocation = "Denver, Colorado, United States",
            ClimateModelSummary = "This model represents a cool continental steppe climate with dry air, broad temperature swings, and limited precipitation split between occasional rain and winter snow. It should feel open and inland, with enough moisture to avoid true desert sterility but not enough to support lush conditions.",
            RegionalClimateSummary = "Expect warm to hot summers, cold winters, and a generally dry sky in every season. Snow can arrive in winter, spring can be sharp and windy, and even the wetter spells tend to be brief rather than persistent.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-6.0, 7.0),
                ["Mid Winter"] = (-8.0, 6.0),
                ["Late Winter"] = (-5.0, 8.0),
                ["Early Spring"] = (0.0, 12.0),
                ["Mid Spring"] = (5.0, 18.0),
                ["Late Spring"] = (10.0, 24.0),
                ["Early Summer"] = (15.0, 28.0),
                ["Mid Summer"] = (18.0, 30.0),
                ["Late Summer"] = (17.0, 29.0),
                ["Early Autumn"] = (11.0, 24.0),
                ["Mid Autumn"] = (4.0, 17.0),
                ["Late Autumn"] = (-2.0, 10.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.10, 1.15, 1.05, 1.10),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.95, 0.92, 0.98, 0.95),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.16, 0.34, 0.42, 0.28),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.78, 1.45, 1.30, 1.52),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 1.05, 1.08, 1.0, 1.05),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.10, 1.10, 1.0, 1.08),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 0.34, 0.52, 0.46, 0.34),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 1.42, 1.22, 1.28, 1.36),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 0.32, 0.48, 0.44, 0.32),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 0.18, 0.34, 0.28, 0.18),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 0.14, 0.50, 0.58, 0.30),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 0.08, 0.34, 0.42, 0.18),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonalScale(seasonName, 0.85, 0.22, 0.0, 0.10),
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 1.85, 1.50, 1.42, 1.72),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 2.4, 2.0, 1.9, 2.2),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 0.34, 0.55, 0.52, 0.36),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 1.10, 1.05, 1.02, 1.08),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.95, 0.95, 0.92, 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00044
        };
    }

    private static WeatherSeederClimateProfile CreateHotDesertProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHotSemiAridProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Hot Desert",
            RegionalClimatePrefix = "Hot Desert",
            KoppenClimateClassification = "BWh",
            ReferenceLocation = "Yuma, Arizona, United States",
            ClimateModelSummary = "This model represents a subtropical hot desert with extreme summer heat, very low humidity, and rare but sometimes dramatic bursts of rain. Clear skies and parched air should dominate, with cloud build-up or wet conditions feeling unusual rather than routine.",
            RegionalClimateSummary = "Expect scorching summers, mild winters, and a landscape that spends most of the year dry to parched. Daytime heat climbs hard in the warm season, nights cool back off quickly, and most rain arrives as short-lived relief rather than a real wet spell.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (8.0, 22.0),
                ["Mid Winter"] = (7.0, 21.0),
                ["Late Winter"] = (10.0, 25.0),
                ["Early Spring"] = (15.0, 29.0),
                ["Mid Spring"] = (19.0, 33.0),
                ["Late Spring"] = (24.0, 38.0),
                ["Early Summer"] = (29.0, 43.0),
                ["Mid Summer"] = (31.0, 45.0),
                ["Late Summer"] = (30.0, 44.0),
                ["Early Autumn"] = (25.0, 39.0),
                ["Mid Autumn"] = (17.0, 31.0),
                ["Late Autumn"] = (10.0, 24.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.0, 1.0, 1.05, 1.0),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.98, 0.96, 0.95, 0.98),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.10, 0.12, 0.20, 0.22),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.85, 1.80, 1.65, 1.55),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 1.02, 1.08, 1.15, 1.08),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.0, 1.0, 1.05, 1.0),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 0.10, 0.09, 0.12, 0.16),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 1.85, 2.0, 1.82, 1.65),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 0.08, 0.08, 0.12, 0.18),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 0.05, 0.04, 0.08, 0.12),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 0.06, 0.08, 0.18, 0.24),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 0.02, 0.04, 0.10, 0.14),
            OvercastToLightSnowChance = seasonName => 0.0,
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 1.85, 2.15, 2.30, 1.95),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 3.4, 3.6, 3.9, 3.0),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 0.08, 0.08, 0.12, 0.16),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 1.35, 1.35, 1.28, 1.25),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.90, 0.90, 0.88, 0.90),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00040
        };
    }

    private static WeatherSeederClimateProfile CreateColdDesertProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateColdSemiAridProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Cold Desert",
            RegionalClimatePrefix = "Cold Desert",
            KoppenClimateClassification = "BWk",
            ReferenceLocation = "Bishop, California, United States",
            ClimateModelSummary = "This model represents a mid-latitude cold desert with sparse precipitation, strong diurnal temperature swings, and winters cold enough for snow. It is drier than steppe country but cooler and more seasonally varied than a subtropical hot desert.",
            RegionalClimateSummary = "Expect dry bright weather in most seasons, hot afternoons in summer, and cold winters with only occasional snow. The air usually feels thin and arid, with short wet spells giving way quickly to clearer skies again.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-3.0, 11.0),
                ["Mid Winter"] = (-4.0, 10.0),
                ["Late Winter"] = (0.0, 13.0),
                ["Early Spring"] = (4.0, 17.0),
                ["Mid Spring"] = (8.0, 23.0),
                ["Late Spring"] = (13.0, 29.0),
                ["Early Summer"] = (18.0, 34.0),
                ["Mid Summer"] = (20.0, 35.0),
                ["Late Summer"] = (19.0, 34.0),
                ["Early Autumn"] = (13.0, 29.0),
                ["Mid Autumn"] = (6.0, 21.0),
                ["Late Autumn"] = (0.0, 14.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.0, 1.05, 1.08, 1.02),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.98, 0.95, 0.94, 0.98),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.24, 0.28, 0.20, 0.24),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.45, 1.60, 1.70, 1.58),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 1.0, 1.05, 1.08, 1.02),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 1.0, 1.02, 1.05, 1.0),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 0.38, 0.32, 0.22, 0.28),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 1.42, 1.55, 1.75, 1.62),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 0.38, 0.32, 0.18, 0.24),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 0.20, 0.16, 0.10, 0.12),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 0.22, 0.20, 0.10, 0.18),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 0.10, 0.10, 0.04, 0.08),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonalScale(seasonName, 0.62, 0.12, 0.0, 0.03),
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 1.55, 1.80, 2.0, 1.82),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 2.1, 2.3, 2.7, 2.3),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 0.26, 0.22, 0.18, 0.20),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 1.28, 1.30, 1.24, 1.25),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.90, 0.88, 0.86, 0.88),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00038
        };
    }

    private static WeatherSeederClimateProfile CreateTropicalRainforestProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubtropicalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Tropical Rainforest",
            RegionalClimatePrefix = "Tropical Rainforest",
            KoppenClimateClassification = "Af",
            ReferenceLocation = "Singapore",
            ClimateModelSummary = "This model represents an equatorial rainforest climate with very little seasonal temperature change, persistent humidity, and rain that can arrive at almost any time. Cloud cover and wet conditions are common enough that genuinely dry weather should feel temporary.",
            RegionalClimateSummary = "This region should feel hot, lush, and damp all year. Days and nights stay warm, rain and overcast skies are common, and even the calmer parts of the year still feel humid rather than dry.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (24.0, 30.0),
                ["Mid Winter"] = (24.0, 30.0),
                ["Late Winter"] = (25.0, 31.0),
                ["Early Spring"] = (25.0, 31.0),
                ["Mid Spring"] = (25.0, 32.0),
                ["Late Spring"] = (25.0, 32.0),
                ["Early Summer"] = (25.0, 31.0),
                ["Mid Summer"] = (25.0, 31.0),
                ["Late Summer"] = (25.0, 31.0),
                ["Early Autumn"] = (25.0, 31.0),
                ["Mid Autumn"] = (25.0, 31.0),
                ["Late Autumn"] = (24.0, 30.0)
            },
            WindIncreaseChance = (seasonName, currentWind) => baseProfile.WindIncreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 0.75, 0.80, 0.82, 0.78),
            WindDecreaseChance = (seasonName, currentWind) => baseProfile.WindDecreaseChance(seasonName, currentWind) * SeasonalScale(seasonName, 1.20, 1.15, 1.12, 1.18),
            PrecipIncreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipIncreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 1.28, 1.10, 1.04, 1.22),
            PrecipDecreaseChance = (seasonName, currentPrecipitation, stages) => baseProfile.PrecipDecreaseChance(seasonName, currentPrecipitation, stages) * SeasonalScale(seasonName, 0.74, 0.80, 0.84, 0.76),
            TemperatureVariationChance = seasonName => baseProfile.TemperatureVariationChance(seasonName) * SeasonalScale(seasonName, 0.75, 0.75, 0.72, 0.75),
            WindVariationChance = seasonName => baseProfile.WindVariationChance(seasonName) * SeasonalScale(seasonName, 0.72, 0.75, 0.78, 0.72),
            CloudIncreaseChance = seasonName => baseProfile.CloudIncreaseChance(seasonName) * SeasonalScale(seasonName, 1.32, 1.15, 1.10, 1.28),
            CloudDecreaseChance = seasonName => baseProfile.CloudDecreaseChance(seasonName) * SeasonalScale(seasonName, 0.64, 0.74, 0.80, 0.68),
            CloudyToOvercastChance = seasonName => baseProfile.CloudyToOvercastChance(seasonName) * SeasonalScale(seasonName, 1.34, 1.16, 1.08, 1.28),
            CloudyToHumidChance = seasonName => baseProfile.CloudyToHumidChance(seasonName) * SeasonalScale(seasonName, 1.30, 1.15, 1.12, 1.24),
            OvercastToLightRainChance = seasonName => baseProfile.OvercastToLightRainChance(seasonName) * SeasonalScale(seasonName, 1.26, 1.12, 1.08, 1.22),
            OvercastToRainChance = seasonName => baseProfile.OvercastToRainChance(seasonName) * SeasonalScale(seasonName, 1.22, 1.12, 1.08, 1.18),
            OvercastToLightSnowChance = seasonName => 0.0,
            CloudyToDryChance = seasonName => baseProfile.CloudyToDryChance(seasonName) * SeasonalScale(seasonName, 0.36, 0.38, 0.40, 0.36),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonalScale(seasonName, 0.05, 0.05, 0.06, 0.05),
            OvercastToHumidChance = seasonName => baseProfile.OvercastToHumidChance(seasonName) * SeasonalScale(seasonName, 1.20, 1.15, 1.12, 1.18),
            OvercastToCloudyChance = seasonName => baseProfile.OvercastToCloudyChance(seasonName) * SeasonalScale(seasonName, 0.82, 0.85, 0.88, 0.84),
            MaximumAdditionalChangeChance = seasonName => baseProfile.MaximumAdditionalChangeChance(seasonName) * SeasonalScale(seasonName, 0.95, 0.95, 0.95, 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00042
        };
    }

    private static WeatherSeederClimateProfile CreateSubpolarOceanicProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateTemperateOceanicProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Subpolar Oceanic",
            RegionalClimatePrefix = "Subpolar Oceanic",
            KoppenClimateClassification = "Cfc",
            ReferenceLocation = "Adak, Alaska, United States",
            ClimateModelSummary = "This model represents a cool maritime climate with short chilly summers, stormy shoulders, and persistent cloud and precipitation through the year. It should feel oceanic first and polar second, with raw wind and dampness more defining than deep continental cold.",
            RegionalClimateSummary = "Expect cold grey seas, brief cool summers, and long stretches of damp windy weather. Snow and sleet appear regularly in the colder half of the year, but the climate remains too maritime for the brutal extremes of true subarctic interiors.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-1.0, 4.0),
                ["Mid Winter"] = (-2.0, 3.0),
                ["Late Winter"] = (-2.0, 3.0),
                ["Early Spring"] = (-1.0, 5.0),
                ["Mid Spring"] = (1.0, 7.0),
                ["Late Spring"] = (3.0, 10.0),
                ["Early Summer"] = (5.0, 12.0),
                ["Mid Summer"] = (7.0, 14.0),
                ["Late Summer"] = (7.0, 14.0),
                ["Early Autumn"] = (5.0, 11.0),
                ["Mid Autumn"] = (3.0, 8.0),
                ["Late Autumn"] = (1.0, 6.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.10, spring: 1.05, summer: 0.98, autumn: 1.18),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 0.96, spring: 0.98, summer: 1.02, autumn: 0.92),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 1.08, spring: 1.02, summer: 0.92, autumn: 1.12),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 0.94, spring: 0.98, summer: 1.06, autumn: 0.90),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.92, spring: 0.95, summer: 0.90, autumn: 0.95),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 1.18, spring: 1.08, summer: 0.95, autumn: 1.25),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 1.12, spring: 1.05, summer: 0.95, autumn: 1.16),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 0.92, spring: 0.96, summer: 1.08, autumn: 0.90),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 1.10, spring: 1.05, summer: 0.95, autumn: 1.14),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 1.10, spring: 1.04, summer: 0.95, autumn: 1.10),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 1.05, spring: 1.02, summer: 0.92, autumn: 1.12),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.95, spring: 0.95, summer: 0.85, autumn: 1.02),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.8, 0.6, 0.0, 0.45),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 0.78, spring: 0.86, summer: 1.05, autumn: 0.76),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.10, 0.12, 0.16, 0.10),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 1.08, spring: 1.04, summer: 0.94, autumn: 1.12),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 0.95, spring: 0.95, summer: 1.06, autumn: 0.92),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 1.05, spring: 1.02, summer: 0.95, autumn: 1.08),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00055
        };
    }

    private static WeatherSeederClimateProfile CreateDryWinterHumidSubtropicalProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubtropicalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Dry Winter Humid Subtropical",
            RegionalClimatePrefix = "Dry Winter Humid Subtropical",
            KoppenClimateClassification = "Cwa",
            ReferenceLocation = "Daegu, South Korea",
            ClimateModelSummary = "This model represents the warm temperate monsoon pattern with hot humid summers but a distinctly drier, clearer winter than year-round-humid subtropical climates. Spring should feel transitional and breezy, while the warm season carries most of the heavier rain and thunderstorm activity.",
            RegionalClimateSummary = "Expect hot sticky summers, cool comparatively dry winters, and a dramatic seasonal swing in rainfall. The climate should feel brighter and crisper in winter than a Cfa region, but still much milder than a true continental interior.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (0.0, 10.0),
                ["Mid Winter"] = (-3.0, 8.0),
                ["Late Winter"] = (-1.0, 10.0),
                ["Early Spring"] = (4.0, 16.0),
                ["Mid Spring"] = (9.0, 22.0),
                ["Late Spring"] = (15.0, 27.0),
                ["Early Summer"] = (20.0, 31.0),
                ["Mid Summer"] = (23.0, 33.0),
                ["Late Summer"] = (22.0, 32.0),
                ["Early Autumn"] = (17.0, 28.0),
                ["Mid Autumn"] = (10.0, 22.0),
                ["Late Autumn"] = (3.0, 15.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.08, summer: 0.95, autumn: 0.98),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 0.95, summer: 1.0, autumn: 1.02),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.45, spring: 0.90, summer: 1.25, autumn: 0.82),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.50, spring: 1.05, summer: 0.82, autumn: 1.05),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.12, spring: 1.04, summer: 0.95, autumn: 1.02),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.92, spring: 1.10, summer: 0.95, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.62, spring: 0.95, summer: 1.18, autumn: 0.88),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.28, spring: 1.0, summer: 0.88, autumn: 1.05),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.58, spring: 0.95, summer: 1.15, autumn: 0.88),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.52, spring: 0.88, summer: 1.12, autumn: 0.85),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.42, spring: 0.88, summer: 1.25, autumn: 0.85),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.38, spring: 0.92, summer: 1.32, autumn: 0.90),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 2.0, 0.15, 0.0, 0.02),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.55, spring: 1.05, summer: 0.70, autumn: 1.05),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 4.5, 2.2, 0.65, 1.1),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.68, spring: 0.92, summer: 1.08, autumn: 0.92),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.18, spring: 1.02, summer: 0.90, autumn: 1.02),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 1.0, summer: 1.02, autumn: 0.98),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00042
        };
    }

    private static WeatherSeederClimateProfile CreateSubtropicalHighlandProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateDryWinterHumidSubtropicalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Subtropical Highland",
            RegionalClimatePrefix = "Subtropical Highland",
            KoppenClimateClassification = "Cwb",
            ReferenceLocation = "Mexico City, Mexico",
            ClimateModelSummary = "This model represents a highland monsoon climate with dry sunny winters, mild days, and wet summers tempered by elevation. It should feel seasonally tropical in rainfall but not in heat, with cool nights and a softer annual temperature curve than lowland subtropical climates.",
            RegionalClimateSummary = "Expect spring warmth, summer rains, and dry bright winters, but without oppressive heat. The overall feel is upland and elevated, with comfortable daytime temperatures, noticeable nighttime cooling, and only rare brushes with frost or snow.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (5.0, 20.0),
                ["Mid Winter"] = (4.0, 19.0),
                ["Late Winter"] = (5.0, 21.0),
                ["Early Spring"] = (7.0, 23.0),
                ["Mid Spring"] = (9.0, 25.0),
                ["Late Spring"] = (11.0, 26.0),
                ["Early Summer"] = (12.0, 24.0),
                ["Mid Summer"] = (12.0, 23.0),
                ["Late Summer"] = (12.0, 23.0),
                ["Early Autumn"] = (11.0, 22.0),
                ["Mid Autumn"] = (8.0, 21.0),
                ["Late Autumn"] = (6.0, 20.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.92, spring: 1.05, summer: 0.92, autumn: 0.96),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 0.98, summer: 1.05, autumn: 1.02),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.35, spring: 0.80, summer: 1.18, autumn: 0.92),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.60, spring: 1.10, summer: 0.88, autumn: 0.98),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.08, spring: 1.02, summer: 0.90, autumn: 0.98),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.95, spring: 1.05, summer: 0.90, autumn: 0.96),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.52, spring: 0.85, summer: 1.10, autumn: 0.90),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.35, spring: 1.05, summer: 0.92, autumn: 1.0),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.50, spring: 0.85, summer: 1.08, autumn: 0.92),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.45, spring: 0.82, summer: 1.06, autumn: 0.90),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.30, spring: 0.78, summer: 1.15, autumn: 0.95),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.22, spring: 0.72, summer: 1.10, autumn: 0.92),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.65, 0.10, 0.0, 0.0),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.70, spring: 1.12, summer: 0.76, autumn: 1.0),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 2.6, 1.8, 0.75, 1.0),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.56, spring: 0.88, summer: 1.04, autumn: 0.95),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.20, spring: 1.05, summer: 0.95, autumn: 1.0),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.92, spring: 0.95, summer: 0.95, autumn: 0.92),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00040
        };
    }

    private static WeatherSeederClimateProfile CreateColdSummerSubtropicalHighlandProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSubtropicalHighlandProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Cold Summer Subtropical Highland",
            RegionalClimatePrefix = "Cold Summer Subtropical Highland",
            KoppenClimateClassification = "Cwc",
            ReferenceLocation = "El Alto, Bolivia",
            ClimateModelSummary = "This model represents a rare cold-summer highland monsoon climate, with a marked dry winter, a short damp summer, and temperatures kept low year-round by altitude. It should feel stark and elevated, more alpine than subtropical despite the monsoonal rainfall rhythm.",
            RegionalClimateSummary = "Expect chilly mornings in every season, a modest wet season in the warmer months, and a long clear dry season. Summer never becomes truly warm, and winter often feels bright, thin-aired, and near freezing rather than dark and stormy.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-5.0, 9.0),
                ["Mid Winter"] = (-6.0, 8.0),
                ["Late Winter"] = (-5.0, 10.0),
                ["Early Spring"] = (-3.0, 12.0),
                ["Mid Spring"] = (-1.0, 14.0),
                ["Late Spring"] = (1.0, 16.0),
                ["Early Summer"] = (2.0, 16.0),
                ["Mid Summer"] = (2.0, 15.0),
                ["Late Summer"] = (2.0, 15.0),
                ["Early Autumn"] = (1.0, 14.0),
                ["Mid Autumn"] = (-1.0, 12.0),
                ["Late Autumn"] = (-3.0, 10.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.98, spring: 1.08, summer: 0.92, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.04, spring: 0.98, summer: 1.04, autumn: 1.02),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.28, spring: 0.72, summer: 1.15, autumn: 0.88),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.75, spring: 1.18, summer: 0.90, autumn: 1.02),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.10, spring: 1.04, summer: 0.92, autumn: 1.0),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.95, spring: 1.05, summer: 0.92, autumn: 0.98),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.45, spring: 0.80, summer: 1.10, autumn: 0.88),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.45, spring: 1.10, summer: 0.92, autumn: 1.05),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.42, spring: 0.82, summer: 1.08, autumn: 0.90),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.38, spring: 0.78, summer: 1.05, autumn: 0.88),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.20, spring: 0.65, summer: 1.12, autumn: 0.85),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.12, spring: 0.52, summer: 1.02, autumn: 0.80),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 2.2, 0.6, 0.04, 0.22),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.90, spring: 1.18, summer: 0.78, autumn: 1.05),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 3.2, 2.0, 0.72, 1.08),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.48, spring: 0.82, summer: 1.02, autumn: 0.90),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.24, spring: 1.08, summer: 0.96, autumn: 1.02),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.90, spring: 0.92, summer: 0.92, autumn: 0.90),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00038
        };
    }

    private static WeatherSeederClimateProfile CreateWarmSummerMediterraneanProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateMediterraneanProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Warm-Summer Mediterranean",
            RegionalClimatePrefix = "Warm-Summer Mediterranean",
            KoppenClimateClassification = "Csb",
            ReferenceLocation = "San Francisco, California, United States",
            ClimateModelSummary = "This model represents a mild dry-summer Mediterranean climate shaped by marine influence rather than interior heat. Winters are cool and wetter, summers are comfortable and notably dry, and the warm season should feel bright, stable, and often breezy without the furnace heat of Csa country.",
            RegionalClimateSummary = "Expect mild wet winters, cool-to-warm dry summers, and a climate that often feels tempered by nearby ocean water. Summer skies stay comparatively clear, frost is rare, and the transition back to wetter weather comes gradually through autumn.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (7.0, 14.0),
                ["Mid Winter"] = (7.0, 14.0),
                ["Late Winter"] = (8.0, 16.0),
                ["Early Spring"] = (8.0, 17.0),
                ["Mid Spring"] = (9.0, 18.0),
                ["Late Spring"] = (10.0, 20.0),
                ["Early Summer"] = (11.0, 21.0),
                ["Mid Summer"] = (12.0, 22.0),
                ["Late Summer"] = (12.0, 23.0),
                ["Early Autumn"] = (12.0, 24.0),
                ["Mid Autumn"] = (10.0, 22.0),
                ["Late Autumn"] = (8.0, 17.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.0, summer: 1.08, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.02, spring: 1.0, summer: 0.96, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 1.0, spring: 0.88, summer: 0.72, autumn: 0.86),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.0, spring: 1.08, summer: 1.18, autumn: 1.05),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.92, spring: 0.95, summer: 0.85, autumn: 0.92),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.95, spring: 1.0, summer: 1.08, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 1.0, spring: 0.92, summer: 0.72, autumn: 0.82),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.0, spring: 1.05, summer: 1.18, autumn: 1.08),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 1.0, spring: 0.92, summer: 0.60, autumn: 0.80),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 1.0, spring: 0.90, summer: 0.72, autumn: 0.82),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.96, spring: 0.86, summer: 0.45, autumn: 0.76),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.90, spring: 0.78, summer: 0.25, autumn: 0.65),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.15, 0.0, 0.0, 0.0),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 0.95, spring: 1.12, summer: 1.25, autumn: 1.12),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.08, 0.12, 0.45, 0.18),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 1.0, spring: 0.95, summer: 0.85, autumn: 0.90),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.0, spring: 1.02, summer: 1.08, autumn: 1.05),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.92, summer: 0.90, autumn: 0.92),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00036
        };
    }

    private static WeatherSeederClimateProfile CreateColdSummerMediterraneanProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateWarmSummerMediterraneanProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Cold-Summer Mediterranean",
            RegionalClimatePrefix = "Cold-Summer Mediterranean",
            KoppenClimateClassification = "Csc",
            ReferenceLocation = "Rost, Norway",
            ClimateModelSummary = "This model represents a rare cool dry-summer maritime climate where even the warm season stays gentle, but winter remains wet and often raw rather than deeply continental. It should feel like a windswept ocean fringe or upland coast with a Mediterranean precipitation rhythm but much colder air.",
            RegionalClimateSummary = "Expect cool summers, damp winters, and a noticeably drier but not truly hot warm season. Wind and cloud still matter, snow can appear in winter, and the whole climate should feel sparse, ocean-facing, and strongly seasonal without huge heat.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (0.0, 6.0),
                ["Mid Winter"] = (-1.0, 5.0),
                ["Late Winter"] = (0.0, 6.0),
                ["Early Spring"] = (1.0, 8.0),
                ["Mid Spring"] = (3.0, 10.0),
                ["Late Spring"] = (5.0, 12.0),
                ["Early Summer"] = (7.0, 14.0),
                ["Mid Summer"] = (8.0, 15.0),
                ["Late Summer"] = (8.0, 15.0),
                ["Early Autumn"] = (6.0, 13.0),
                ["Mid Autumn"] = (4.0, 10.0),
                ["Late Autumn"] = (2.0, 8.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.04, spring: 1.02, summer: 1.0, autumn: 1.08),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 0.98, spring: 1.0, summer: 1.0, autumn: 0.96),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 1.05, spring: 0.95, summer: 0.82, autumn: 1.0),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 0.96, spring: 1.02, summer: 1.10, autumn: 1.0),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.92, spring: 0.95, summer: 0.88, autumn: 0.92),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 1.02, spring: 1.0, summer: 1.02, autumn: 1.06),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 1.02, spring: 0.98, summer: 0.85, autumn: 1.0),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 0.96, spring: 1.0, summer: 1.08, autumn: 0.98),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 1.02, spring: 0.98, summer: 0.75, autumn: 0.95),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 1.04, spring: 1.0, summer: 0.88, autumn: 0.96),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 1.0, spring: 0.92, summer: 0.62, autumn: 0.90),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.95, spring: 0.88, summer: 0.32, autumn: 0.82),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 3.2, 0.25, 0.0, 0.18),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 0.92, spring: 1.10, summer: 1.18, autumn: 1.05),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.05, 0.08, 0.22, 0.10),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 1.02, spring: 0.98, summer: 0.92, autumn: 0.96),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 0.98, spring: 1.02, summer: 1.08, autumn: 1.02),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.92, summer: 0.90, autumn: 0.94),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00036
        };
    }

    private static WeatherSeederClimateProfile CreateTropicalMonsoonProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateTropicalRainforestProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Tropical Monsoon",
            RegionalClimatePrefix = "Tropical Monsoon",
            KoppenClimateClassification = "Am",
            ReferenceLocation = "Miami, Florida, United States",
            ClimateModelSummary = "This model represents a tropical monsoon climate with rainforest warmth, a very wet warm season, and a short but noticeable drier interval. Rain should still feel common compared with non-tropical climates, but the year has more structure and a clearer break than a true equatorial rainforest.",
            RegionalClimateSummary = "Expect hot humid weather all year, with a long stormier wet season and a shorter brighter season that is drier without becoming arid. Heavy showers and overcast skies are still common, but the climate breathes a little more than Af rainforest conditions.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (18.0, 26.0),
                ["Mid Winter"] = (17.0, 25.0),
                ["Late Winter"] = (18.0, 26.0),
                ["Early Spring"] = (20.0, 28.0),
                ["Mid Spring"] = (22.0, 30.0),
                ["Late Spring"] = (24.0, 31.0),
                ["Early Summer"] = (25.0, 32.0),
                ["Mid Summer"] = (25.0, 32.0),
                ["Late Summer"] = (25.0, 32.0),
                ["Early Autumn"] = (24.0, 31.0),
                ["Mid Autumn"] = (22.0, 30.0),
                ["Late Autumn"] = (20.0, 28.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 0.98, summer: 1.0, autumn: 1.05),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.04, spring: 1.0, summer: 0.98, autumn: 0.95),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.72, spring: 0.85, summer: 1.0, autumn: 1.15),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.20, spring: 1.08, summer: 0.95, autumn: 0.90),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.88, spring: 0.92, summer: 0.92, autumn: 0.90),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.85, spring: 0.90, summer: 0.92, autumn: 0.88),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.85, spring: 0.95, summer: 1.02, autumn: 1.08),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.10, spring: 1.02, summer: 0.96, autumn: 0.92),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.82, spring: 0.94, summer: 1.0, autumn: 1.10),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.88, spring: 0.95, summer: 1.0, autumn: 1.08),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.78, spring: 0.90, summer: 1.0, autumn: 1.12),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.82, spring: 0.92, summer: 1.0, autumn: 1.12),
            OvercastToLightSnowChance = seasonName => 0.0,
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.55, spring: 1.18, summer: 0.82, autumn: 0.68),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 6.0, 3.0, 0.45, 0.25),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.88, spring: 0.98, summer: 1.02, autumn: 1.10),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.10, spring: 1.02, summer: 0.96, autumn: 0.90),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.95, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00040
        };
    }

    private static WeatherSeederClimateProfile CreateTropicalSavannaDryWinterProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateTropicalMonsoonProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Tropical Savanna Dry Winter",
            RegionalClimatePrefix = "Tropical Savanna Dry Winter",
            KoppenClimateClassification = "Aw",
            ReferenceLocation = "Bangkok, Thailand",
            ClimateModelSummary = "This model represents a tropical wet-and-dry climate with a pronounced monsoonal summer and a much drier winter. Heat stays present all year, but the atmosphere swings between a storm-heavy wet season and a clearer, dustier, more open dry season.",
            RegionalClimateSummary = "Expect a strong contrast between a humid rainy warm season and a drier sunnier winter. Even the dry half of the year stays warm, but vegetation and sky cover should react sharply to the monsoon rather than feeling uniformly tropical and wet.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (20.0, 31.0),
                ["Mid Winter"] = (19.0, 31.0),
                ["Late Winter"] = (21.0, 33.0),
                ["Early Spring"] = (24.0, 35.0),
                ["Mid Spring"] = (26.0, 36.0),
                ["Late Spring"] = (26.0, 35.0),
                ["Early Summer"] = (25.0, 34.0),
                ["Mid Summer"] = (25.0, 33.0),
                ["Late Summer"] = (25.0, 33.0),
                ["Early Autumn"] = (24.0, 33.0),
                ["Mid Autumn"] = (23.0, 32.0),
                ["Late Autumn"] = (21.0, 31.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.92, spring: 0.98, summer: 1.02, autumn: 1.05),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.08, spring: 1.02, summer: 0.96, autumn: 0.95),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.28, spring: 0.55, summer: 1.35, autumn: 1.18),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.90, spring: 1.25, summer: 0.75, autumn: 0.88),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.92, spring: 1.05, summer: 0.95, autumn: 0.92),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.90, spring: 0.98, summer: 0.95, autumn: 0.92),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.42, spring: 0.68, summer: 1.20, autumn: 1.12),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.55, spring: 1.18, summer: 0.88, autumn: 0.90),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.35, spring: 0.62, summer: 1.18, autumn: 1.08),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.38, spring: 0.65, summer: 1.12, autumn: 1.05),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.22, spring: 0.55, summer: 1.35, autumn: 1.15),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.18, spring: 0.48, summer: 1.42, autumn: 1.18),
            OvercastToLightSnowChance = seasonName => 0.0,
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 2.25, spring: 1.55, summer: 0.60, autumn: 0.75),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 9.0, 5.0, 0.25, 0.18),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.55, spring: 0.78, summer: 1.08, autumn: 1.02),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.35, spring: 1.12, summer: 0.95, autumn: 0.92),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.92, spring: 0.95, summer: 0.98, autumn: 0.98),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00038
        };
    }

    private static WeatherSeederClimateProfile CreateTropicalSavannaDrySummerProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateTropicalMonsoonProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Tropical Savanna Dry Summer",
            RegionalClimatePrefix = "Tropical Savanna Dry Summer",
            KoppenClimateClassification = "As",
            ReferenceLocation = "Chennai, India",
            ClimateModelSummary = "This model represents the rarer tropical savanna pattern with heat year-round but a dry warm season and a much wetter autumn and early winter. It should feel tropical and monsoonal, but with the rainfall peak arriving later than in Aw climates.",
            RegionalClimateSummary = "Expect hot conditions through the year, a noticeably drier summer, and a stormier wetter turn in autumn and early winter. The region should still feel lush and humid during its wet phase, but the timing of the rains differs sharply from the more common dry-winter savanna.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (22.0, 29.0),
                ["Mid Winter"] = (21.0, 28.0),
                ["Late Winter"] = (22.0, 30.0),
                ["Early Spring"] = (24.0, 32.0),
                ["Mid Spring"] = (27.0, 35.0),
                ["Late Spring"] = (28.0, 37.0),
                ["Early Summer"] = (28.0, 37.0),
                ["Mid Summer"] = (27.0, 35.0),
                ["Late Summer"] = (26.0, 34.0),
                ["Early Autumn"] = (25.0, 33.0),
                ["Mid Autumn"] = (24.0, 31.0),
                ["Late Autumn"] = (23.0, 29.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 0.98, summer: 0.92, autumn: 1.05),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.02, spring: 1.0, summer: 1.08, autumn: 0.95),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 1.08, spring: 0.72, summer: 0.35, autumn: 1.55),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 0.90, spring: 1.18, summer: 1.90, autumn: 0.72),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 0.92, spring: 1.0, summer: 0.95, autumn: 0.90),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.90, spring: 0.95, summer: 0.92, autumn: 0.92),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 1.05, spring: 0.85, summer: 0.42, autumn: 1.35),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 0.95, spring: 1.05, summer: 1.62, autumn: 0.80),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 1.08, spring: 0.78, summer: 0.30, autumn: 1.60),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 1.05, spring: 0.85, summer: 0.35, autumn: 1.40),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 1.02, spring: 0.72, summer: 0.20, autumn: 1.65),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 1.12, spring: 0.65, summer: 0.18, autumn: 1.75),
            OvercastToLightSnowChance = seasonName => 0.0,
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 0.82, spring: 1.22, summer: 2.20, autumn: 0.45),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.50, 1.8, 8.0, 0.15),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 1.05, spring: 0.85, summer: 0.45, autumn: 1.45),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 0.95, spring: 1.0, summer: 1.30, autumn: 0.82),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.90, autumn: 1.0),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00038
        };
    }

    private static WeatherSeederClimateProfile CreateHotSummerHumidSubcontinentalProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubcontinentalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Hot Summer Humid Subcontinental",
            RegionalClimatePrefix = "Hot Summer Humid Subcontinental",
            KoppenClimateClassification = "Dfa",
            ReferenceLocation = "Chicago, Illinois, United States",
            ClimateModelSummary = "This model represents the hotter-summer humid continental pattern, with cold winters, large seasonal contrast, and frequent warm-season thunderstorms. Compared with Dfb, the warm half of the year should feel longer, more humid, and decisively hot rather than merely warm.",
            RegionalClimateSummary = "Expect snowy winters, stormy spring and summer weather, and hot often humid midsummers. The climate should still feel continental and strongly seasonal, but summer now carries real heat and heavier convective rain.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-4.0, 4.0),
                ["Mid Winter"] = (-7.0, 1.0),
                ["Late Winter"] = (-6.0, 3.0),
                ["Early Spring"] = (0.0, 10.0),
                ["Mid Spring"] = (6.0, 17.0),
                ["Late Spring"] = (12.0, 24.0),
                ["Early Summer"] = (18.0, 29.0),
                ["Mid Summer"] = (21.0, 31.0),
                ["Late Summer"] = (20.0, 30.0),
                ["Early Autumn"] = (15.0, 25.0),
                ["Mid Autumn"] = (9.0, 18.0),
                ["Late Autumn"] = (2.0, 10.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.0, spring: 1.02, summer: 1.0, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.0, spring: 1.0, summer: 1.0, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.95, spring: 1.02, summer: 1.12, autumn: 1.0),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.05, spring: 1.0, summer: 0.92, autumn: 1.0),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.0, spring: 1.02, summer: 1.05, autumn: 1.0),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.98, spring: 1.0, summer: 1.02, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.98, spring: 1.02, summer: 1.08, autumn: 1.0),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.0, spring: 1.0, summer: 0.95, autumn: 1.0),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 1.0, spring: 1.02, summer: 1.05, autumn: 1.0),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.95, spring: 1.0, summer: 1.05, autumn: 1.0),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.95, spring: 1.0, summer: 1.10, autumn: 1.0),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.90, spring: 1.0, summer: 1.15, autumn: 1.0),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.72, 0.18, 0.0, 0.04),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.02, spring: 1.0, summer: 0.92, autumn: 0.98),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.75, 0.85, 0.95, 0.85),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.95, spring: 0.98, summer: 1.02, autumn: 1.0),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.0, spring: 1.0, summer: 0.98, autumn: 1.0),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.98, spring: 1.0, summer: 1.02, autumn: 1.0),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00048
        };
    }

    private static WeatherSeederClimateProfile CreateWarmSummerDryWinterHumidSubcontinentalProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateDryWinterHumidSubcontinentalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Warm Summer Dry Winter Humid Subcontinental",
            RegionalClimatePrefix = "Warm Summer Dry Winter Humid Subcontinental",
            KoppenClimateClassification = "Dwb",
            ReferenceLocation = "Vladivostok, Russia",
            ClimateModelSummary = "This model represents a monsoon-influenced continental climate with cold dry winters, wet summers, and warm rather than hot summer conditions. It should feel more maritime and cloudy in the wet season than Dwa, but still sharply continental and winter-dry.",
            RegionalClimateSummary = "Expect bright dry cold winters and a wetter, foggier, stormier summer half of the year. Seasonal contrast remains strong, but the warm season is milder than a Dwa interior and the wettest weather is concentrated around summer and early autumn.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-10.0, -1.0),
                ["Mid Winter"] = (-14.0, -4.0),
                ["Late Winter"] = (-11.0, -2.0),
                ["Early Spring"] = (-3.0, 6.0),
                ["Mid Spring"] = (3.0, 12.0),
                ["Late Spring"] = (8.0, 18.0),
                ["Early Summer"] = (13.0, 22.0),
                ["Mid Summer"] = (17.0, 24.0),
                ["Late Summer"] = (16.0, 24.0),
                ["Early Autumn"] = (11.0, 20.0),
                ["Mid Autumn"] = (4.0, 13.0),
                ["Late Autumn"] = (-3.0, 5.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.0, summer: 0.98, autumn: 1.05),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 1.0, summer: 1.0, autumn: 0.98),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.70, spring: 0.88, summer: 1.12, autumn: 1.05),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.25, spring: 1.05, summer: 0.90, autumn: 0.95),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.0, spring: 1.02, summer: 0.95, autumn: 1.0),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.95, spring: 1.0, summer: 0.95, autumn: 1.02),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.78, spring: 0.92, summer: 1.12, autumn: 1.08),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.12, spring: 1.02, summer: 0.95, autumn: 0.98),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.78, spring: 0.92, summer: 1.12, autumn: 1.08),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.70, spring: 0.88, summer: 1.15, autumn: 1.05),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.68, spring: 0.85, summer: 1.15, autumn: 1.10),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.58, spring: 0.82, summer: 1.12, autumn: 1.10),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.35, 0.06, 0.0, 0.0),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.45, spring: 1.08, summer: 0.82, autumn: 0.92),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 2.0, 1.4, 0.55, 0.60),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.80, spring: 0.92, summer: 1.08, autumn: 1.05),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.15, spring: 1.05, summer: 0.95, autumn: 0.92),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.98, spring: 1.0, summer: 1.0, autumn: 1.0),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00044
        };
    }

    private static WeatherSeederClimateProfile CreateSevereWinterSubarcticProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Severe Winter Subarctic",
            RegionalClimatePrefix = "Severe Winter Subarctic",
            KoppenClimateClassification = "Dfd",
            ReferenceLocation = "Oymyakon, Russia",
            ClimateModelSummary = "This model represents the most severe fully forested continental cold, with a very long bitter winter, a short mild summer, and relatively modest precipitation. The key note is not wetness but the extraordinary winter cold, which should dominate the climate’s personality.",
            RegionalClimateSummary = "Expect a short growing season, a huge annual temperature range, and winter cold that is genuinely punishing. Snow remains part of the climate, but the real impression is extreme interior dryness and cold rather than frequent storminess.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-27.0, -16.0),
                ["Mid Winter"] = (-39.0, -28.0),
                ["Late Winter"] = (-35.0, -21.0),
                ["Early Spring"] = (-22.0, -7.0),
                ["Mid Spring"] = (-10.0, 4.0),
                ["Late Spring"] = (0.0, 14.0),
                ["Early Summer"] = (8.0, 21.0),
                ["Mid Summer"] = (12.0, 24.0),
                ["Late Summer"] = (8.0, 20.0),
                ["Early Autumn"] = (-1.0, 10.0),
                ["Mid Autumn"] = (-14.0, -3.0),
                ["Late Autumn"] = (-24.0, -12.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.0, summer: 0.95, autumn: 1.02),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 1.0, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.80, spring: 0.88, summer: 1.0, autumn: 0.92),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.18, spring: 1.05, summer: 0.98, autumn: 1.0),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.18, spring: 1.10, summer: 1.0, autumn: 1.10),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.98, spring: 1.0, summer: 0.95, autumn: 1.02),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.88, spring: 0.92, summer: 1.0, autumn: 0.95),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.10, spring: 1.05, summer: 1.0, autumn: 1.0),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.85, spring: 0.90, summer: 1.0, autumn: 0.95),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.72, spring: 0.80, summer: 1.0, autumn: 0.95),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.22, spring: 0.75, summer: 1.0, autumn: 0.78),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.08, spring: 0.55, summer: 0.98, autumn: 0.58),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.25, 0.95, 0.0, 1.05),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.18, spring: 1.05, summer: 0.95, autumn: 1.0),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 1.35, 1.18, 0.92, 1.05),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.78, spring: 0.86, summer: 0.98, autumn: 0.95),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.12, spring: 1.05, summer: 1.0, autumn: 1.0),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.92, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00034
        };
    }

    private static WeatherSeederClimateProfile CreateDryWinterSubarcticProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSevereWinterSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Dry Winter Subarctic",
            RegionalClimatePrefix = "Dry Winter Subarctic",
            KoppenClimateClassification = "Dwc",
            ReferenceLocation = "Delta Junction, Alaska, United States",
            ClimateModelSummary = "This model represents a cold continental climate with a very dry winter, a short mild summer, and a precipitation peak in the warm season. It should feel inland and high-latitude, but with the monsoonal dry-winter signature superimposed on subarctic temperatures.",
            RegionalClimateSummary = "Expect long cold clear winters, a brief greener summer, and a marked seasonal rainfall swing. Snow remains important in winter, but the overall air mass should feel relatively dry until the warm season turns showery again.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-20.0, -9.0),
                ["Mid Winter"] = (-26.0, -14.0),
                ["Late Winter"] = (-23.0, -10.0),
                ["Early Spring"] = (-12.0, 0.0),
                ["Mid Spring"] = (-2.0, 10.0),
                ["Late Spring"] = (4.0, 18.0),
                ["Early Summer"] = (9.0, 22.0),
                ["Mid Summer"] = (11.0, 24.0),
                ["Late Summer"] = (8.0, 21.0),
                ["Early Autumn"] = (1.0, 12.0),
                ["Mid Autumn"] = (-8.0, 2.0),
                ["Late Autumn"] = (-17.0, -6.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.0, summer: 0.95, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 1.0, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.55, spring: 0.82, summer: 1.08, autumn: 0.95),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.42, spring: 1.10, summer: 0.92, autumn: 1.0),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.12, spring: 1.05, summer: 0.98, autumn: 1.05),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.98, spring: 1.0, summer: 0.95, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.62, spring: 0.82, summer: 1.05, autumn: 0.95),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.22, spring: 1.05, summer: 1.0, autumn: 1.0),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.58, spring: 0.80, summer: 1.02, autumn: 0.95),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.50, spring: 0.75, summer: 1.05, autumn: 0.98),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.15, spring: 0.60, summer: 1.08, autumn: 0.85),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.04, spring: 0.40, summer: 1.02, autumn: 0.65),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.05, 0.55, 0.0, 0.72),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.55, spring: 1.12, summer: 0.90, autumn: 1.0),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 3.0, 1.9, 0.75, 0.80),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.55, spring: 0.80, summer: 1.02, autumn: 0.95),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.30, spring: 1.08, summer: 0.98, autumn: 1.02),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.92, spring: 0.95, summer: 0.92, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00032
        };
    }

    private static WeatherSeederClimateProfile CreateSevereWinterDryWinterSubarcticProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateDryWinterSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Severe Winter Dry Winter Subarctic",
            RegionalClimatePrefix = "Severe Winter Dry Winter Subarctic",
            KoppenClimateClassification = "Dwd",
            ReferenceLocation = "Delyankir, Russia",
            ClimateModelSummary = "This model represents one of the rarest and harshest inhabited climate types: extremely severe continental winter cold combined with a distinctly dry winter and only a short mild summer. The weather graph should emphasize clarity and cold more than frequent precipitation.",
            RegionalClimateSummary = "Expect a very long dry brutal winter, a short thaw season, and only modest precipitation concentrated in the warm months. Even by subarctic standards the annual temperature range should feel enormous.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-30.0, -18.0),
                ["Mid Winter"] = (-42.0, -30.0),
                ["Late Winter"] = (-39.0, -24.0),
                ["Early Spring"] = (-24.0, -10.0),
                ["Mid Spring"] = (-10.0, 4.0),
                ["Late Spring"] = (0.0, 15.0),
                ["Early Summer"] = (8.0, 21.0),
                ["Mid Summer"] = (11.0, 24.0),
                ["Late Summer"] = (7.0, 20.0),
                ["Early Autumn"] = (-3.0, 9.0),
                ["Mid Autumn"] = (-16.0, -4.0),
                ["Late Autumn"] = (-27.0, -15.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.92, spring: 1.0, summer: 0.95, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.08, spring: 1.0, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.42, spring: 0.78, summer: 1.02, autumn: 0.92),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.55, spring: 1.12, summer: 0.95, autumn: 1.02),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.18, spring: 1.08, summer: 1.0, autumn: 1.08),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.98, spring: 1.0, summer: 0.95, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.52, spring: 0.78, summer: 1.0, autumn: 0.92),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.30, spring: 1.08, summer: 1.0, autumn: 1.02),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.45, spring: 0.72, summer: 1.0, autumn: 0.92),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.38, spring: 0.65, summer: 0.98, autumn: 0.92),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.08, spring: 0.50, summer: 1.0, autumn: 0.72),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.02, spring: 0.28, summer: 0.95, autumn: 0.50),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.92, 0.45, 0.0, 0.62),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.72, spring: 1.15, summer: 0.92, autumn: 1.02),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 4.0, 2.2, 0.82, 0.88),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.45, spring: 0.72, summer: 0.98, autumn: 0.92),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.40, spring: 1.10, summer: 1.0, autumn: 1.02),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.90, spring: 0.92, summer: 0.90, autumn: 0.92),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00030
        };
    }

    private static WeatherSeederClimateProfile CreateHotSummerDrySummerContinentalProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHotSummerHumidSubcontinentalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Hot Summer Dry-Summer Continental",
            RegionalClimatePrefix = "Hot Summer Dry-Summer Continental",
            KoppenClimateClassification = "Dsa",
            ReferenceLocation = "Salt Lake City, Utah, United States",
            ClimateModelSummary = "This model represents a continental climate with hot summers, cold winters, and a strongly Mediterranean dry-summer signal. It should feel inland and high-contrast, with winter snow and spring weather systems giving way to a stable dry warm season.",
            RegionalClimateSummary = "Expect cold winters, hot dry summers, and a shoulder-season pattern where spring is more active and summer becomes bright and thirsty. Snow matters in winter, but summer behaves far more like an interior Mediterranean than a humid continental climate.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-4.0, 6.0),
                ["Mid Winter"] = (-6.0, 4.0),
                ["Late Winter"] = (-3.0, 8.0),
                ["Early Spring"] = (1.0, 13.0),
                ["Mid Spring"] = (6.0, 19.0),
                ["Late Spring"] = (11.0, 26.0),
                ["Early Summer"] = (16.0, 31.0),
                ["Mid Summer"] = (20.0, 34.0),
                ["Late Summer"] = (19.0, 33.0),
                ["Early Autumn"] = (13.0, 28.0),
                ["Mid Autumn"] = (6.0, 19.0),
                ["Late Autumn"] = (0.0, 10.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.0, spring: 1.05, summer: 1.0, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.0, spring: 0.98, summer: 1.0, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.92, spring: 0.88, summer: 0.35, autumn: 0.72),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.02, spring: 1.10, summer: 1.85, autumn: 1.18),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.0, spring: 1.02, summer: 1.10, autumn: 1.02),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 0.98, spring: 1.02, summer: 1.02, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.92, spring: 0.85, summer: 0.32, autumn: 0.62),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.02, spring: 1.10, summer: 1.45, autumn: 1.20),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.90, spring: 0.82, summer: 0.22, autumn: 0.55),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.95, spring: 0.85, summer: 0.28, autumn: 0.52),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.82, spring: 0.78, summer: 0.18, autumn: 0.55),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.78, spring: 0.72, summer: 0.08, autumn: 0.42),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.90, 0.30, 0.0, 0.08),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.05, spring: 1.18, summer: 1.95, autumn: 1.32),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 1.05, 1.35, 2.8, 1.55),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.98, spring: 0.92, summer: 0.35, autumn: 0.62),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.0, spring: 1.05, summer: 1.22, autumn: 1.08),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.95, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00044
        };
    }

    private static WeatherSeederClimateProfile CreateWarmSummerDrySummerContinentalProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateHumidSubcontinentalProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Warm Summer Dry-Summer Continental",
            RegionalClimatePrefix = "Warm Summer Dry-Summer Continental",
            KoppenClimateClassification = "Dsb",
            ReferenceLocation = "Truckee, California, United States",
            ClimateModelSummary = "This model represents a cool-summer continental climate with dry bright summers and snowy wet winters. It combines a mountain-Mediterranean precipitation rhythm with a fully continental winter and should feel alpine rather than coastal.",
            RegionalClimateSummary = "Expect snowy winters, pleasantly warm but dry summers, and a sharp spring snowmelt shoulder season. Summer should feel much clearer and calmer than a Dfb region, but the winter half of the year remains markedly continental.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-8.0, 4.0),
                ["Mid Winter"] = (-10.0, 3.0),
                ["Late Winter"] = (-8.0, 5.0),
                ["Early Spring"] = (-3.0, 10.0),
                ["Mid Spring"] = (1.0, 15.0),
                ["Late Spring"] = (5.0, 21.0),
                ["Early Summer"] = (8.0, 25.0),
                ["Mid Summer"] = (10.0, 27.0),
                ["Late Summer"] = (9.0, 26.0),
                ["Early Autumn"] = (5.0, 22.0),
                ["Mid Autumn"] = (0.0, 15.0),
                ["Late Autumn"] = (-4.0, 8.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.0, spring: 1.05, summer: 1.0, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.0, spring: 0.98, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.95, spring: 0.90, summer: 0.28, autumn: 0.65),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.0, spring: 1.10, summer: 1.95, autumn: 1.18),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.0, spring: 1.02, summer: 1.02, autumn: 1.0),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 1.0, spring: 1.02, summer: 1.0, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.95, spring: 0.88, summer: 0.28, autumn: 0.60),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.0, spring: 1.08, summer: 1.55, autumn: 1.22),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.95, spring: 0.85, summer: 0.18, autumn: 0.52),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.92, spring: 0.82, summer: 0.22, autumn: 0.50),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.85, spring: 0.72, summer: 0.15, autumn: 0.48),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.70, spring: 0.62, summer: 0.05, autumn: 0.35),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.25, 0.55, 0.0, 0.22),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.02, spring: 1.20, summer: 2.15, autumn: 1.30),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 0.92, 1.25, 2.4, 1.25),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.95, spring: 0.88, summer: 0.32, autumn: 0.58),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.0, spring: 1.08, summer: 1.22, autumn: 1.10),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.92, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00042
        };
    }

    private static WeatherSeederClimateProfile CreateDrySummerSubarcticProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Dry-Summer Subarctic",
            RegionalClimatePrefix = "Dry-Summer Subarctic",
            KoppenClimateClassification = "Dsc",
            ReferenceLocation = "McCarthy, Alaska, United States",
            ClimateModelSummary = "This model represents a dry-summer boreal climate with very cold winters, a short mild summer, and a notable reduction in warm-season rain compared with typical subarctic interiors. It should feel mountainous or rain-shadowed while still remaining unmistakably subarctic.",
            RegionalClimateSummary = "Expect long cold snowy winters, brief mild summers, and clearer drier weather in the warmest part of the year than in a standard Dfc region. Snow and freezing conditions still dominate the calendar, but summer offers a comparatively stable break.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-18.0, -7.0),
                ["Mid Winter"] = (-24.0, -12.0),
                ["Late Winter"] = (-21.0, -9.0),
                ["Early Spring"] = (-11.0, 0.0),
                ["Mid Spring"] = (-2.0, 9.0),
                ["Late Spring"] = (4.0, 17.0),
                ["Early Summer"] = (8.0, 20.0),
                ["Mid Summer"] = (10.0, 22.0),
                ["Late Summer"] = (8.0, 20.0),
                ["Early Autumn"] = (2.0, 12.0),
                ["Mid Autumn"] = (-7.0, 2.0),
                ["Late Autumn"] = (-15.0, -5.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 1.0, spring: 1.02, summer: 0.95, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.0, spring: 1.0, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.95, spring: 0.92, summer: 0.45, autumn: 0.82),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.0, spring: 1.02, summer: 1.55, autumn: 1.15),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.0, spring: 1.02, summer: 0.95, autumn: 1.0),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 1.0, spring: 1.0, summer: 0.98, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.98, spring: 0.92, summer: 0.45, autumn: 0.82),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.0, spring: 1.02, summer: 1.35, autumn: 1.08),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.95, spring: 0.90, summer: 0.35, autumn: 0.72),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.95, spring: 0.90, summer: 0.42, autumn: 0.78),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.40, spring: 0.72, summer: 0.45, autumn: 0.72),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.15, spring: 0.52, summer: 0.28, autumn: 0.48),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 1.12, 0.72, 0.0, 0.82),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.02, spring: 1.12, summer: 1.55, autumn: 1.15),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 1.02, 1.22, 1.60, 1.22),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.92, spring: 0.88, summer: 0.55, autumn: 0.82),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.0, spring: 1.05, summer: 1.15, autumn: 1.08),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.95, spring: 0.95, summer: 0.92, autumn: 0.95),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00034
        };
    }

    private static WeatherSeederClimateProfile CreateSevereWinterDrySummerSubarcticProfile()
    {
        WeatherSeederClimateProfile baseProfile = CreateSevereWinterSubarcticProfile();
        return new WeatherSeederClimateProfile
        {
            ClimateModelName = "Severe Winter Dry-Summer Subarctic",
            RegionalClimatePrefix = "Severe Winter Dry-Summer Subarctic",
            KoppenClimateClassification = "Dsd",
            ReferenceLocation = "Verkhoyansk, Russia",
            ClimateModelSummary = "This model represents an exceptionally cold dry-summer continental climate where summer stays brief and relatively dry, but winter temperatures are among the most severe on Earth. It should feel sparse, inland, and punishingly seasonal.",
            RegionalClimateSummary = "Expect extraordinary winter cold, limited precipitation, and a short comparatively stable summer. Snow and freezing dominate much of the year, but the main personality is the sheer intensity of the cold rather than constant storm activity.",
            SeasonalTemperatureRanges = new Dictionary<string, (double Minimum, double Maximum)>
            {
                ["Early Winter"] = (-29.0, -17.0),
                ["Mid Winter"] = (-41.0, -29.0),
                ["Late Winter"] = (-37.0, -23.0),
                ["Early Spring"] = (-23.0, -9.0),
                ["Mid Spring"] = (-10.0, 4.0),
                ["Late Spring"] = (0.0, 16.0),
                ["Early Summer"] = (9.0, 22.0),
                ["Mid Summer"] = (12.0, 25.0),
                ["Late Summer"] = (8.0, 20.0),
                ["Early Autumn"] = (-2.0, 10.0),
                ["Mid Autumn"] = (-15.0, -3.0),
                ["Late Autumn"] = (-26.0, -14.0)
            },
            WindIncreaseChance = ScaleSeasonalChance(baseProfile.WindIncreaseChance, winter: 0.95, spring: 1.0, summer: 0.92, autumn: 1.0),
            WindDecreaseChance = ScaleSeasonalChance(baseProfile.WindDecreaseChance, winter: 1.05, spring: 1.0, summer: 1.02, autumn: 1.0),
            PrecipIncreaseChance = ScaleSeasonalChance(baseProfile.PrecipIncreaseChance, winter: 0.60, spring: 0.82, summer: 0.35, autumn: 0.75),
            PrecipDecreaseChance = ScaleSeasonalChance(baseProfile.PrecipDecreaseChance, winter: 1.30, spring: 1.08, summer: 1.65, autumn: 1.12),
            TemperatureVariationChance = ScaleSeasonalChance(baseProfile.TemperatureVariationChance, winter: 1.18, spring: 1.08, summer: 1.0, autumn: 1.08),
            WindVariationChance = ScaleSeasonalChance(baseProfile.WindVariationChance, winter: 1.0, spring: 1.0, summer: 0.95, autumn: 1.0),
            CloudIncreaseChance = ScaleSeasonalChance(baseProfile.CloudIncreaseChance, winter: 0.70, spring: 0.82, summer: 0.32, autumn: 0.72),
            CloudDecreaseChance = ScaleSeasonalChance(baseProfile.CloudDecreaseChance, winter: 1.15, spring: 1.05, summer: 1.35, autumn: 1.08),
            CloudyToOvercastChance = ScaleSeasonalChance(baseProfile.CloudyToOvercastChance, winter: 0.62, spring: 0.75, summer: 0.22, autumn: 0.62),
            CloudyToHumidChance = ScaleSeasonalChance(baseProfile.CloudyToHumidChance, winter: 0.55, spring: 0.70, summer: 0.30, autumn: 0.68),
            OvercastToLightRainChance = ScaleSeasonalChance(baseProfile.OvercastToLightRainChance, winter: 0.06, spring: 0.45, summer: 0.25, autumn: 0.52),
            OvercastToRainChance = ScaleSeasonalChance(baseProfile.OvercastToRainChance, winter: 0.02, spring: 0.30, summer: 0.12, autumn: 0.32),
            OvercastToLightSnowChance = seasonName => baseProfile.OvercastToLightSnowChance(seasonName) * SeasonFactor(seasonName, 0.92, 0.55, 0.0, 0.68),
            CloudyToDryChance = ScaleSeasonalChance(baseProfile.CloudyToDryChance, winter: 1.35, spring: 1.15, summer: 1.55, autumn: 1.15),
            CloudyToParchedChance = seasonName => baseProfile.CloudyToParchedChance(seasonName) * SeasonFactor(seasonName, 2.0, 1.55, 1.42, 1.20),
            OvercastToHumidChance = ScaleSeasonalChance(baseProfile.OvercastToHumidChance, winter: 0.62, spring: 0.75, summer: 0.40, autumn: 0.72),
            OvercastToCloudyChance = ScaleSeasonalChance(baseProfile.OvercastToCloudyChance, winter: 1.22, spring: 1.08, summer: 1.15, autumn: 1.08),
            MaximumAdditionalChangeChance = ScaleSeasonalChance(baseProfile.MaximumAdditionalChangeChance, winter: 0.90, spring: 0.92, summer: 0.90, autumn: 0.92),
            IncrementalAdditionalChangeChanceFromStableWeather = 0.00030
        };
    }

    private static double SeasonalScale(string seasonName, double winter, double spring, double summer, double autumn)
    {
        return seasonName switch
        {
            "Winter" => winter,
            "Spring" => spring,
            "Summer" => summer,
            "Autumn" => autumn,
            _ => 1.0
        };
    }

    private static double WindChanceByDelta(int delta, double belowBase, double belowStep, double equalChance, double aboveBase, double aboveStep, double minimumChance)
    {
        if (delta < 0)
        {
            return belowBase + Math.Abs(delta) * belowStep;
        }

        if (delta == 0)
        {
            return equalChance;
        }

        return Math.Max(minimumChance, aboveBase - delta * aboveStep);
    }

    private static double WindDecreaseChanceByDelta(int delta, double aboveBase, double aboveStep, double equalChance, double belowBase, double belowStep, double minimumChance)
    {
        if (delta > 0)
        {
            return aboveBase + delta * aboveStep;
        }

        if (delta == 0)
        {
            return equalChance;
        }

        return Math.Max(minimumChance, belowBase - Math.Abs(delta) * belowStep);
    }

    private static double PrecipIncreaseChanceByDelta(
        int delta,
        int stages,
        double belowBase,
        double belowStep,
        double belowBaseTwoStage,
        double belowStepTwoStage,
        double equalChance,
        double equalChanceTwoStage,
        double aboveBase,
        double aboveBaseTwoStage,
        double aboveStep,
        double aboveStepTwoStage,
        double minimumChance,
        double minimumChanceTwoStage)
    {
        if (delta < 0)
        {
            return stages == 1
                ? belowBase + Math.Abs(delta) * belowStep
                : belowBaseTwoStage + Math.Abs(delta) * belowStepTwoStage;
        }

        if (delta == 0)
        {
            return stages == 1 ? equalChance : equalChanceTwoStage;
        }

        return Math.Max(
            stages == 1 ? minimumChance : minimumChanceTwoStage,
            (stages == 1 ? aboveBase : aboveBaseTwoStage) - delta * (stages == 1 ? aboveStep : aboveStepTwoStage));
    }

    private static double PrecipDecreaseChanceByDelta(
        int delta,
        int stages,
        double aboveBase,
        double aboveStep,
        double aboveBaseTwoStage,
        double aboveStepTwoStage,
        double equalChance,
        double equalChanceTwoStage,
        double belowBase,
        double belowBaseTwoStage,
        double belowStep,
        double belowStepTwoStage,
        double minimumChance,
        double minimumChanceTwoStage)
    {
        if (delta > 0)
        {
            return stages == 1
                ? aboveBase + delta * aboveStep
                : aboveBaseTwoStage + delta * aboveStepTwoStage;
        }

        if (delta == 0)
        {
            return stages == 1 ? equalChance : equalChanceTwoStage;
        }

        return Math.Max(
            stages == 1 ? minimumChance : minimumChanceTwoStage,
            (stages == 1 ? belowBase : belowBaseTwoStage) - Math.Abs(delta) * (stages == 1 ? belowStep : belowStepTwoStage));
    }

    private static ClimateModel CreateClimateModel(
        FuturemudDatabaseContext context,
        List<Season> seasons,
        WeatherSeederEventCatalog eventCatalog,
        WeatherSeederClimateProfile profile,
        IReadOnlyDictionary<string, ClimateModel> seededClimateModels)
    {
        return profile.RegionalClimatePrefix switch
        {
            "Oceanic Temperate" => CreateClimateModels(context, seasons, eventCatalog.EventsByName, profile),
            "Subpolar Oceanic" => CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustSubpolarOceanicTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Humid Subtropical" => CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustHumidSubtropicalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Dry Winter Humid Subtropical" => CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustDryWinterHumidSubtropicalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Subtropical Highland" => CreateDerivedClimateModel(context, CreateDryWinterHumidSubtropicalProfile(), profile, AdjustSubtropicalHighlandTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Cold Summer Subtropical Highland" => CreateDerivedClimateModel(context, CreateSubtropicalHighlandProfile(), profile, AdjustColdSummerSubtropicalHighlandTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Tropical Monsoon" => CreateDerivedClimateModel(context, CreateTropicalRainforestProfile(), profile, AdjustTropicalMonsoonTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Tropical Savanna Dry Winter" => CreateDerivedClimateModel(context, CreateTropicalMonsoonProfile(), profile, AdjustTropicalSavannaDryWinterTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Tropical Savanna Dry Summer" => CreateDerivedClimateModel(context, CreateTropicalMonsoonProfile(), profile, AdjustTropicalSavannaDrySummerTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Tropical Rainforest" => CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustTropicalRainforestTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Warm-Summer Mediterranean" => CreateDerivedClimateModel(context, CreateMediterraneanProfile(), profile, AdjustWarmSummerMediterraneanTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Cold-Summer Mediterranean" => CreateDerivedClimateModel(context, CreateWarmSummerMediterraneanProfile(), profile, AdjustColdSummerMediterraneanTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Humid Subcontinental" => CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustHumidSubcontinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Hot Summer Humid Subcontinental" => CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustHotSummerHumidSubcontinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Dry Winter Humid Subcontinental" => CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustDryWinterHumidSubcontinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Warm Summer Dry Winter Humid Subcontinental" => CreateDerivedClimateModel(context, CreateDryWinterHumidSubcontinentalProfile(), profile, AdjustWarmSummerDryWinterHumidSubcontinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Subarctic" => CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Severe Winter Subarctic" => CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustSevereWinterSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Dry Winter Subarctic" => CreateDerivedClimateModel(context, CreateSevereWinterSubarcticProfile(), profile, AdjustDryWinterSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Severe Winter Dry Winter Subarctic" => CreateDerivedClimateModel(context, CreateDryWinterSubarcticProfile(), profile, AdjustSevereWinterDryWinterSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Hot Summer Dry-Summer Continental" => CreateDerivedClimateModel(context, CreateHotSummerHumidSubcontinentalProfile(), profile, AdjustHotSummerDrySummerContinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Warm Summer Dry-Summer Continental" => CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustWarmSummerDrySummerContinentalTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Dry-Summer Subarctic" => CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustDrySummerSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Severe Winter Dry-Summer Subarctic" => CreateDerivedClimateModel(context, CreateSevereWinterSubarcticProfile(), profile, AdjustSevereWinterDrySummerSubarcticTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Tundra" => CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustTundraTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Polar Ice Cap" => CreateDerivedClimateModel(context, CreateTundraProfile(), profile, AdjustPolarIceCapTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Mediterranean" => CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustMediterraneanTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Hot Semi-Arid" => CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustHotSemiAridTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Cold Semi-Arid" => CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustColdSemiAridTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Hot Desert" => CreateDerivedClimateModel(context, CreateHotSemiAridProfile(), profile, AdjustHotDesertTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            "Cold Desert" => CreateDerivedClimateModel(context, CreateColdSemiAridProfile(), profile, AdjustColdDesertTransitionChance, seededClimateModels, eventCatalog.DescriptorsById),
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile.RegionalClimatePrefix, "Unsupported seeded climate profile.")
        };
    }

    private static ClimateModel CreateHumidSubtropicalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustHumidSubtropicalTransitionChance);
    }

    private static ClimateModel CreateSubpolarOceanicClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustSubpolarOceanicTransitionChance);
    }

    private static ClimateModel CreateDryWinterHumidSubtropicalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustDryWinterHumidSubtropicalTransitionChance);
    }

    private static ClimateModel CreateSubtropicalHighlandClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateDryWinterHumidSubtropicalProfile(), profile, AdjustSubtropicalHighlandTransitionChance);
    }

    private static ClimateModel CreateColdSummerSubtropicalHighlandClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSubtropicalHighlandProfile(), profile, AdjustColdSummerSubtropicalHighlandTransitionChance);
    }

    private static ClimateModel CreateTropicalRainforestClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustTropicalRainforestTransitionChance);
    }

    private static ClimateModel CreateTropicalMonsoonClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTropicalRainforestProfile(), profile, AdjustTropicalMonsoonTransitionChance);
    }

    private static ClimateModel CreateTropicalSavannaDryWinterClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTropicalMonsoonProfile(), profile, AdjustTropicalSavannaDryWinterTransitionChance);
    }

    private static ClimateModel CreateTropicalSavannaDrySummerClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTropicalMonsoonProfile(), profile, AdjustTropicalSavannaDrySummerTransitionChance);
    }

    private static ClimateModel CreateMediterraneanClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustMediterraneanTransitionChance);
    }

    private static ClimateModel CreateWarmSummerMediterraneanClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateMediterraneanProfile(), profile, AdjustWarmSummerMediterraneanTransitionChance);
    }

    private static ClimateModel CreateColdSummerMediterraneanClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateWarmSummerMediterraneanProfile(), profile, AdjustColdSummerMediterraneanTransitionChance);
    }

    private static ClimateModel CreateHumidSubcontinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTemperateOceanicProfile(), profile, AdjustHumidSubcontinentalTransitionChance);
    }

    private static ClimateModel CreateHotSummerHumidSubcontinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustHotSummerHumidSubcontinentalTransitionChance);
    }

    private static ClimateModel CreateDryWinterHumidSubcontinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustDryWinterHumidSubcontinentalTransitionChance);
    }

    private static ClimateModel CreateWarmSummerDryWinterHumidSubcontinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateDryWinterHumidSubcontinentalProfile(), profile, AdjustWarmSummerDryWinterHumidSubcontinentalTransitionChance);
    }

    private static ClimateModel CreateSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustSubarcticTransitionChance);
    }

    private static ClimateModel CreateSevereWinterSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustSevereWinterSubarcticTransitionChance);
    }

    private static ClimateModel CreateDryWinterSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSevereWinterSubarcticProfile(), profile, AdjustDryWinterSubarcticTransitionChance);
    }

    private static ClimateModel CreateSevereWinterDryWinterSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateDryWinterSubarcticProfile(), profile, AdjustSevereWinterDryWinterSubarcticTransitionChance);
    }

    private static ClimateModel CreateHotSummerDrySummerContinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHotSummerHumidSubcontinentalProfile(), profile, AdjustHotSummerDrySummerContinentalTransitionChance);
    }

    private static ClimateModel CreateWarmSummerDrySummerContinentalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustWarmSummerDrySummerContinentalTransitionChance);
    }

    private static ClimateModel CreateDrySummerSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustDrySummerSubarcticTransitionChance);
    }

    private static ClimateModel CreateSevereWinterDrySummerSubarcticClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSevereWinterSubarcticProfile(), profile, AdjustSevereWinterDrySummerSubarcticTransitionChance);
    }

    private static ClimateModel CreateTundraClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateSubarcticProfile(), profile, AdjustTundraTransitionChance);
    }

    private static ClimateModel CreatePolarIceCapClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateTundraProfile(), profile, AdjustPolarIceCapTransitionChance);
    }

    private static ClimateModel CreateHotSemiAridClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubtropicalProfile(), profile, AdjustHotSemiAridTransitionChance);
    }

    private static ClimateModel CreateColdSemiAridClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHumidSubcontinentalProfile(), profile, AdjustColdSemiAridTransitionChance);
    }

    private static ClimateModel CreateHotDesertClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateHotSemiAridProfile(), profile, AdjustHotDesertTransitionChance);
    }

    private static ClimateModel CreateColdDesertClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
    {
        return CreateDerivedClimateModel(context, CreateColdSemiAridProfile(), profile, AdjustColdDesertTransitionChance);
    }

    private static ClimateModel CreateDerivedClimateModel(
        FuturemudDatabaseContext context,
        WeatherSeederClimateProfile baseProfile,
        WeatherSeederClimateProfile profile,
        Func<string, WeatherSeederEventDescriptor, WeatherSeederEventDescriptor, double, WeatherSeederClimateProfile, WeatherSeederClimateProfile, double> transitionAdjuster,
        IReadOnlyDictionary<string, ClimateModel> seededClimateModels,
        IReadOnlyDictionary<long, WeatherSeederEventDescriptor> descriptorsById)
    {
        ClimateModel baseModel = GetRequiredBaseClimateModel(seededClimateModels, baseProfile);
        ClimateModel climateModel = new()
        {
            Name = profile.ClimateModelName,
            Description = BuildClimateModelDescription(profile),
            MinuteProcessingInterval = baseModel.MinuteProcessingInterval,
            MinimumMinutesBetweenFlavourEchoes = baseModel.MinimumMinutesBetweenFlavourEchoes,
            MinuteFlavourEchoChance = baseModel.MinuteFlavourEchoChance,
            Type = baseModel.Type
        };

        foreach (ClimateModelSeason? baseSeason in baseModel.ClimateModelSeasons)
        {
            ClimateModelSeason cms = new()
            {
                ClimateModel = climateModel,
                Season = baseSeason.Season,
                IncrementalAdditionalChangeChanceFromStableWeather = profile.IncrementalAdditionalChangeChanceFromStableWeather,
                MaximumAdditionalChangeChanceFromStableWeather = profile.MaximumAdditionalChangeChance(baseSeason.Season.SeasonGroup)
            };
            climateModel.ClimateModelSeasons.Add(cms);

            foreach (ClimateModelSeasonEvent? baseSeasonEvent in baseSeason.SeasonEvents)
            {
                WeatherSeederEventDescriptor descriptor = descriptorsById[baseSeasonEvent.WeatherEvent.Id];
                List<XElement> transitionElements = XElement.Parse(baseSeasonEvent.Transitions).Elements("Transition").ToList();
                List<(long EventId, double Chance)> transitionPairs =
                    (from transition in transitionElements
                     let targetId = long.Parse(transition.Attribute("id")!.Value)
                     where descriptorsById.ContainsKey(targetId)
                     let adjustedChance = transitionAdjuster(
                         baseSeason.Season.SeasonGroup,
                         descriptor,
                         descriptorsById[targetId],
                         double.Parse(transition.Attribute("chance")!.Value),
                         baseProfile,
                         profile)
                     where adjustedChance > 0.0
                     select (EventId: targetId, Chance: adjustedChance))
                    .ToList();
                double collapsedTemperatureChance = GetCollapsedTemperatureVariationReallocationChance(
                    profile,
                    baseSeason.Season.SeasonGroup,
                    descriptor);
                if (collapsedTemperatureChance > 0.0)
                {
                    transitionPairs.Add((baseSeasonEvent.WeatherEvent.Id, collapsedTemperatureChance));
                }
                (double EffectiveChangeChance, string TransitionXml) transitionDefinition = BuildTransitionDefinition(
                    baseSeasonEvent.WeatherEvent.Id,
                    transitionPairs,
                    profile.BaseChangeChance);
                cms.SeasonEvents.Add(new ClimateModelSeasonEvent
                {
                    ClimateModel = climateModel,
                    Season = baseSeason.Season,
                    WeatherEvent = baseSeasonEvent.WeatherEvent,
                    ChangeChance = transitionDefinition.EffectiveChangeChance,
                    Transitions = transitionDefinition.TransitionXml
                });
            }
        }

        context.ClimateModels.Add(climateModel);
        return climateModel;
    }

    private static ClimateModel CreateDerivedClimateModel(
        FuturemudDatabaseContext context,
        WeatherSeederClimateProfile baseProfile,
        WeatherSeederClimateProfile profile,
        Func<string, WeatherSeederEventDescriptor, WeatherSeederEventDescriptor, double, WeatherSeederClimateProfile, WeatherSeederClimateProfile, double> transitionAdjuster)
    {
        ClimateModel baseModel = context.ClimateModels
            .Include(x => x.ClimateModelSeasons)
            .ThenInclude(x => x.Season)
            .Include(x => x.ClimateModelSeasons)
            .ThenInclude(x => x.SeasonEvents)
            .ThenInclude(x => x.WeatherEvent)
            .Single(x => x.Name == baseProfile.ClimateModelName);

        ClimateModel climateModel = new()
        {
            Name = profile.ClimateModelName,
            Description = BuildClimateModelDescription(profile),
            MinuteProcessingInterval = baseModel.MinuteProcessingInterval,
            MinimumMinutesBetweenFlavourEchoes = baseModel.MinimumMinutesBetweenFlavourEchoes,
            MinuteFlavourEchoChance = baseModel.MinuteFlavourEchoChance,
            Type = baseModel.Type
        };

        foreach (ClimateModelSeason? baseSeason in baseModel.ClimateModelSeasons)
        {
            ClimateModelSeason cms = new()
            {
                ClimateModel = climateModel,
                Season = baseSeason.Season,
                IncrementalAdditionalChangeChanceFromStableWeather = profile.IncrementalAdditionalChangeChanceFromStableWeather,
                MaximumAdditionalChangeChanceFromStableWeather = profile.MaximumAdditionalChangeChance(baseSeason.Season.SeasonGroup)
            };
            climateModel.ClimateModelSeasons.Add(cms);

            foreach (ClimateModelSeasonEvent? baseSeasonEvent in baseSeason.SeasonEvents)
            {
                WeatherSeederEventDescriptor descriptor = DescribeEvent(baseSeasonEvent.WeatherEvent);
                List<XElement> transitionElements = XElement.Parse(baseSeasonEvent.Transitions).Elements("Transition").ToList();
                List<(long EventId, double Chance)> transitionPairs =
                    (from transition in transitionElements
                     let targetId = long.Parse(transition.Attribute("id")!.Value)
                     let target = context.WeatherEvents.Find(targetId)
                     where target is not null
                     let adjustedChance = transitionAdjuster(
                         baseSeason.Season.SeasonGroup,
                         descriptor,
                         DescribeEvent(target),
                         double.Parse(transition.Attribute("chance")!.Value),
                         baseProfile,
                         profile)
                     where adjustedChance > 0.0
                     select (EventId: targetId, Chance: adjustedChance))
                    .ToList();
                double collapsedTemperatureChance = GetCollapsedTemperatureVariationReallocationChance(
                    profile,
                    baseSeason.Season.SeasonGroup,
                    descriptor);
                if (collapsedTemperatureChance > 0.0)
                {
                    transitionPairs.Add((baseSeasonEvent.WeatherEvent.Id, collapsedTemperatureChance));
                }
                (double EffectiveChangeChance, string TransitionXml) transitionDefinition = BuildTransitionDefinition(
                    baseSeasonEvent.WeatherEvent.Id,
                    transitionPairs,
                    profile.BaseChangeChance);
                cms.SeasonEvents.Add(new ClimateModelSeasonEvent
                {
                    ClimateModel = climateModel,
                    Season = baseSeason.Season,
                    WeatherEvent = baseSeasonEvent.WeatherEvent,
                    ChangeChance = transitionDefinition.EffectiveChangeChance,
                    Transitions = transitionDefinition.TransitionXml
                });
            }
        }

        context.ClimateModels.Add(climateModel);
        context.SaveChanges();
        return climateModel;
    }

    private static double AdjustHumidSubtropicalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile oceanicProfile,
        WeatherSeederClimateProfile humidProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            oceanicProfile,
            humidProfile,
            ApplyHumidSubtropicalFallback);
    }

    private static double AdjustTropicalRainforestTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubtropicalProfile,
        WeatherSeederClimateProfile tropicalRainforestProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            humidSubtropicalProfile,
            tropicalRainforestProfile,
            ApplyTropicalRainforestFallback);
    }

    private static double AdjustMediterraneanTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile oceanicProfile,
        WeatherSeederClimateProfile mediterraneanProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            oceanicProfile,
            mediterraneanProfile,
            ApplyMediterraneanFallback);
    }

    private static double AdjustHumidSubcontinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile oceanicProfile,
        WeatherSeederClimateProfile subcontinentalProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            oceanicProfile,
            subcontinentalProfile,
            ApplyHumidSubcontinentalFallback);
    }

    private static double AdjustDryWinterHumidSubcontinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subcontinentalProfile,
        WeatherSeederClimateProfile dryWinterProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            subcontinentalProfile,
            dryWinterProfile,
            ApplyDryWinterHumidSubcontinentalFallback);
    }

    private static double AdjustSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subcontinentalProfile,
        WeatherSeederClimateProfile subarcticProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            subcontinentalProfile,
            subarcticProfile,
            ApplySubarcticFallback);
    }

    private static double AdjustTundraTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subarcticProfile,
        WeatherSeederClimateProfile tundraProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            subarcticProfile,
            tundraProfile,
            ApplyTundraFallback);
    }

    private static double AdjustPolarIceCapTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile tundraProfile,
        WeatherSeederClimateProfile polarIceCapProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            tundraProfile,
            polarIceCapProfile,
            ApplyPolarIceCapFallback);
    }

    private static double AdjustHotSemiAridTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubtropicalProfile,
        WeatherSeederClimateProfile hotSemiAridProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            humidSubtropicalProfile,
            hotSemiAridProfile,
            ApplyHotSemiAridFallback);
    }

    private static double AdjustColdSemiAridTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubcontinentalProfile,
        WeatherSeederClimateProfile coldSemiAridProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            humidSubcontinentalProfile,
            coldSemiAridProfile,
            ApplyColdSemiAridFallback);
    }

    private static double AdjustHotDesertTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile hotSemiAridProfile,
        WeatherSeederClimateProfile hotDesertProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            hotSemiAridProfile,
            hotDesertProfile,
            ApplyHotDesertFallback);
    }

    private static double AdjustColdDesertTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile coldSemiAridProfile,
        WeatherSeederClimateProfile coldDesertProfile)
    {
        return AdjustDerivedTransitionChance(
            seasonName,
            fromEvent,
            toEvent,
            baseChance,
            coldSemiAridProfile,
            coldDesertProfile,
            ApplyColdDesertFallback);
    }

    private static double AdjustSubpolarOceanicTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile oceanicProfile,
        WeatherSeederClimateProfile subpolarOceanicProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, oceanicProfile, subpolarOceanicProfile, ApplySubpolarOceanicFallback);
    }

    private static double AdjustDryWinterHumidSubtropicalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubtropicalProfile,
        WeatherSeederClimateProfile dryWinterHumidSubtropicalProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, humidSubtropicalProfile, dryWinterHumidSubtropicalProfile, ApplyDryWinterHumidSubtropicalFallback);
    }

    private static double AdjustSubtropicalHighlandTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile dryWinterHumidSubtropicalProfile,
        WeatherSeederClimateProfile subtropicalHighlandProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, dryWinterHumidSubtropicalProfile, subtropicalHighlandProfile, ApplySubtropicalHighlandFallback);
    }

    private static double AdjustColdSummerSubtropicalHighlandTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subtropicalHighlandProfile,
        WeatherSeederClimateProfile coldSummerSubtropicalHighlandProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, subtropicalHighlandProfile, coldSummerSubtropicalHighlandProfile, ApplyColdSummerSubtropicalHighlandFallback);
    }

    private static double AdjustTropicalMonsoonTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile tropicalRainforestProfile,
        WeatherSeederClimateProfile tropicalMonsoonProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, tropicalRainforestProfile, tropicalMonsoonProfile, ApplyTropicalMonsoonFallback);
    }

    private static double AdjustTropicalSavannaDryWinterTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile tropicalMonsoonProfile,
        WeatherSeederClimateProfile tropicalSavannaProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, tropicalMonsoonProfile, tropicalSavannaProfile, ApplyTropicalSavannaDryWinterFallback);
    }

    private static double AdjustTropicalSavannaDrySummerTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile tropicalMonsoonProfile,
        WeatherSeederClimateProfile tropicalSavannaProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, tropicalMonsoonProfile, tropicalSavannaProfile, ApplyTropicalSavannaDrySummerFallback);
    }

    private static double AdjustWarmSummerMediterraneanTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile mediterraneanProfile,
        WeatherSeederClimateProfile warmSummerMediterraneanProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, mediterraneanProfile, warmSummerMediterraneanProfile, ApplyWarmSummerMediterraneanFallback);
    }

    private static double AdjustColdSummerMediterraneanTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile warmSummerMediterraneanProfile,
        WeatherSeederClimateProfile coldSummerMediterraneanProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, warmSummerMediterraneanProfile, coldSummerMediterraneanProfile, ApplyColdSummerMediterraneanFallback);
    }

    private static double AdjustHotSummerHumidSubcontinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubcontinentalProfile,
        WeatherSeederClimateProfile hotSummerHumidSubcontinentalProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, humidSubcontinentalProfile, hotSummerHumidSubcontinentalProfile, ApplyHotSummerHumidSubcontinentalFallback);
    }

    private static double AdjustWarmSummerDryWinterHumidSubcontinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile dryWinterHumidSubcontinentalProfile,
        WeatherSeederClimateProfile warmSummerDryWinterHumidSubcontinentalProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, dryWinterHumidSubcontinentalProfile, warmSummerDryWinterHumidSubcontinentalProfile, ApplyWarmSummerDryWinterHumidSubcontinentalFallback);
    }

    private static double AdjustSevereWinterSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subarcticProfile,
        WeatherSeederClimateProfile severeWinterSubarcticProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, subarcticProfile, severeWinterSubarcticProfile, ApplySevereWinterSubarcticFallback);
    }

    private static double AdjustDryWinterSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile severeWinterSubarcticProfile,
        WeatherSeederClimateProfile dryWinterSubarcticProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, severeWinterSubarcticProfile, dryWinterSubarcticProfile, ApplyDryWinterSubarcticFallback);
    }

    private static double AdjustSevereWinterDryWinterSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile dryWinterSubarcticProfile,
        WeatherSeederClimateProfile severeWinterDryWinterSubarcticProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, dryWinterSubarcticProfile, severeWinterDryWinterSubarcticProfile, ApplySevereWinterDryWinterSubarcticFallback);
    }

    private static double AdjustHotSummerDrySummerContinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile hotSummerHumidSubcontinentalProfile,
        WeatherSeederClimateProfile hotSummerDrySummerContinentalProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, hotSummerHumidSubcontinentalProfile, hotSummerDrySummerContinentalProfile, ApplyHotSummerDrySummerContinentalFallback);
    }

    private static double AdjustWarmSummerDrySummerContinentalTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile humidSubcontinentalProfile,
        WeatherSeederClimateProfile warmSummerDrySummerContinentalProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, humidSubcontinentalProfile, warmSummerDrySummerContinentalProfile, ApplyWarmSummerDrySummerContinentalFallback);
    }

    private static double AdjustDrySummerSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile subarcticProfile,
        WeatherSeederClimateProfile drySummerSubarcticProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, subarcticProfile, drySummerSubarcticProfile, ApplyDrySummerSubarcticFallback);
    }

    private static double AdjustSevereWinterDrySummerSubarcticTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile severeWinterSubarcticProfile,
        WeatherSeederClimateProfile severeWinterDrySummerSubarcticProfile)
    {
        return AdjustDerivedTransitionChance(seasonName, fromEvent, toEvent, baseChance, severeWinterSubarcticProfile, severeWinterDrySummerSubarcticProfile, ApplySevereWinterDrySummerSubarcticFallback);
    }

    private static double AdjustDerivedTransitionChance(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        WeatherSeederClimateProfile oceanicProfile,
        WeatherSeederClimateProfile targetProfile,
        Func<string, WeatherSeederEventDescriptor, WeatherSeederEventDescriptor, double, double> fallbackAdjuster)
    {
        double Scale(double oceanicChance, double humidChance)
        {
            if (oceanicChance <= 0.0)
            {
                return baseChance;
            }

            return baseChance * humidChance / oceanicChance;
        }

        if (fromEvent.CloudVariation == toEvent.CloudVariation &&
            fromEvent.Precipitation == toEvent.Precipitation &&
            fromEvent.WindVariation == toEvent.WindVariation &&
            fromEvent.TemperatureVariation == toEvent.TemperatureVariation)
        {
            if (toEvent.Wind == fromEvent.Wind.StageUp())
            {
                return Scale(
                    oceanicProfile.WindIncreaseChance(seasonName, fromEvent.Wind),
                    targetProfile.WindIncreaseChance(seasonName, fromEvent.Wind));
            }

            if (toEvent.Wind == fromEvent.Wind.StageDown())
            {
                return Scale(
                    oceanicProfile.WindDecreaseChance(seasonName, fromEvent.Wind),
                    targetProfile.WindDecreaseChance(seasonName, fromEvent.Wind));
            }
        }

        if (fromEvent.CloudVariation == CloudVariation.None &&
            toEvent.CloudVariation == CloudVariation.None &&
            fromEvent.Wind == toEvent.Wind &&
            fromEvent.WindVariation == toEvent.WindVariation &&
            fromEvent.TemperatureVariation == toEvent.TemperatureVariation)
        {
            if (toEvent.Precipitation == fromEvent.Precipitation.StageUp())
            {
                return Scale(
                    oceanicProfile.PrecipIncreaseChance(seasonName, fromEvent.Precipitation, 1),
                    targetProfile.PrecipIncreaseChance(seasonName, fromEvent.Precipitation, 1));
            }

            if (toEvent.Precipitation == fromEvent.Precipitation.StageUp(2))
            {
                return Scale(
                    oceanicProfile.PrecipIncreaseChance(seasonName, fromEvent.Precipitation, 2),
                    targetProfile.PrecipIncreaseChance(seasonName, fromEvent.Precipitation, 2));
            }

            if (toEvent.Precipitation == fromEvent.Precipitation.StageDown())
            {
                return Scale(
                    oceanicProfile.PrecipDecreaseChance(seasonName, fromEvent.Precipitation, 1),
                    targetProfile.PrecipDecreaseChance(seasonName, fromEvent.Precipitation, 1));
            }
        }

        if (fromEvent.Wind == toEvent.Wind &&
            fromEvent.WindVariation == toEvent.WindVariation &&
            fromEvent.TemperatureVariation == toEvent.TemperatureVariation)
        {
            if (fromEvent.CloudVariation == CloudVariation.Cloudy && toEvent.CloudVariation == CloudVariation.Overcast)
            {
                return Scale(oceanicProfile.CloudyToOvercastChance(seasonName), targetProfile.CloudyToOvercastChance(seasonName));
            }

            if (fromEvent.CloudVariation == CloudVariation.Cloudy && toEvent.CloudVariation == CloudVariation.None && toEvent.Precipitation == PrecipitationLevel.Humid)
            {
                return Math.Abs(baseChance - oceanicProfile.CloudyToHumidChance(seasonName)) < Math.Abs(baseChance - oceanicProfile.CloudDecreaseChance(seasonName))
                    ? Scale(oceanicProfile.CloudyToHumidChance(seasonName), targetProfile.CloudyToHumidChance(seasonName))
                    : Scale(oceanicProfile.CloudDecreaseChance(seasonName), targetProfile.CloudDecreaseChance(seasonName));
            }

            if (fromEvent.CloudVariation == CloudVariation.Cloudy && toEvent.CloudVariation == CloudVariation.None && toEvent.Precipitation == PrecipitationLevel.Dry)
            {
                return Math.Abs(baseChance - oceanicProfile.CloudyToDryChance(seasonName)) < Math.Abs(baseChance - oceanicProfile.CloudDecreaseChance(seasonName))
                    ? Scale(oceanicProfile.CloudyToDryChance(seasonName), targetProfile.CloudyToDryChance(seasonName))
                    : Scale(oceanicProfile.CloudDecreaseChance(seasonName), targetProfile.CloudDecreaseChance(seasonName));
            }

            if (fromEvent.CloudVariation == CloudVariation.Cloudy && toEvent.CloudVariation == CloudVariation.None && toEvent.Precipitation == PrecipitationLevel.Parched)
            {
                return Scale(oceanicProfile.CloudyToParchedChance(seasonName), targetProfile.CloudyToParchedChance(seasonName));
            }

            if (fromEvent.CloudVariation == CloudVariation.Overcast && toEvent.CloudVariation == CloudVariation.None)
            {
                if (toEvent.Precipitation == PrecipitationLevel.LightRain)
                {
                    return Scale(oceanicProfile.OvercastToLightRainChance(seasonName), targetProfile.OvercastToLightRainChance(seasonName));
                }

                if (toEvent.Precipitation == PrecipitationLevel.Rain)
                {
                    return Scale(oceanicProfile.OvercastToRainChance(seasonName), targetProfile.OvercastToRainChance(seasonName));
                }

                if (toEvent.Precipitation == PrecipitationLevel.LightSnow)
                {
                    return Scale(oceanicProfile.OvercastToLightSnowChance(seasonName), targetProfile.OvercastToLightSnowChance(seasonName));
                }

                if (toEvent.Precipitation == PrecipitationLevel.Humid)
                {
                    return Math.Abs(baseChance - oceanicProfile.OvercastToHumidChance(seasonName)) < Math.Abs(baseChance - oceanicProfile.CloudDecreaseChance(seasonName))
                        ? Scale(oceanicProfile.OvercastToHumidChance(seasonName), targetProfile.OvercastToHumidChance(seasonName))
                        : Scale(oceanicProfile.CloudDecreaseChance(seasonName), targetProfile.CloudDecreaseChance(seasonName));
                }
            }

            if (fromEvent.CloudVariation == CloudVariation.Overcast && toEvent.CloudVariation == CloudVariation.Cloudy)
            {
                return Scale(oceanicProfile.OvercastToCloudyChance(seasonName), targetProfile.OvercastToCloudyChance(seasonName));
            }
        }

        if (fromEvent.CloudVariation == CloudVariation.None &&
            fromEvent.Wind == toEvent.Wind &&
            fromEvent.WindVariation == toEvent.WindVariation &&
            fromEvent.TemperatureVariation == toEvent.TemperatureVariation &&
            fromEvent.Precipitation == toEvent.Precipitation &&
            (toEvent.CloudVariation == CloudVariation.Overcast || toEvent.CloudVariation == CloudVariation.Cloudy))
        {
            return Scale(oceanicProfile.CloudIncreaseChance(seasonName), targetProfile.CloudIncreaseChance(seasonName));
        }

        if (fromEvent.Precipitation == toEvent.Precipitation &&
            fromEvent.CloudVariation == toEvent.CloudVariation &&
            fromEvent.Wind == toEvent.Wind &&
            fromEvent.TemperatureVariation == toEvent.TemperatureVariation &&
            fromEvent.WindVariation != toEvent.WindVariation)
        {
            return Scale(oceanicProfile.WindVariationChance(seasonName), targetProfile.WindVariationChance(seasonName));
        }

        if (fromEvent.Precipitation == toEvent.Precipitation &&
            fromEvent.CloudVariation == toEvent.CloudVariation &&
            fromEvent.Wind == toEvent.Wind &&
            fromEvent.WindVariation == toEvent.WindVariation &&
            fromEvent.TemperatureVariation != toEvent.TemperatureVariation)
        {
            return Scale(oceanicProfile.TemperatureVariationChance(seasonName), targetProfile.TemperatureVariationChance(seasonName));
        }

        return fallbackAdjuster(seasonName, fromEvent, toEvent, baseChance);
    }

    private static double ApplyGenericFallback(
        string seasonName,
        WeatherSeederEventDescriptor toEvent,
        double baseChance,
        (double Winter, double Spring, double Summer, double Autumn) rainFactors,
        (double Winter, double Spring, double Summer, double Autumn) humidFactors,
        (double Winter, double Spring, double Summer, double Autumn) snowFactors,
        (double Winter, double Spring, double Summer, double Autumn) overcastFactors,
        (double Winter, double Spring, double Summer, double Autumn) dryFactors,
        double strongWindFactor,
        double galeWindFactor,
        bool blockRain = false,
        bool blockSnow = false)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            if (blockSnow)
            {
                return 0.0;
            }

            adjusted *= SeasonFactor(seasonName, snowFactors.Winter, snowFactors.Spring, snowFactors.Summer, snowFactors.Autumn);
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            if (blockRain)
            {
                return 0.0;
            }

            adjusted *= SeasonFactor(seasonName, rainFactors.Winter, rainFactors.Spring, rainFactors.Summer, rainFactors.Autumn);
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= SeasonFactor(seasonName, humidFactors.Winter, humidFactors.Spring, humidFactors.Summer, humidFactors.Autumn);
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= SeasonFactor(seasonName, overcastFactors.Winter, overcastFactors.Spring, overcastFactors.Summer, overcastFactors.Autumn);
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= SeasonFactor(seasonName, dryFactors.Winter, dryFactors.Spring, dryFactors.Summer, dryFactors.Autumn);
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= galeWindFactor;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= strongWindFactor;
        }

        return adjusted;
    }

    private static double ApplySubpolarOceanicFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (1.08, 1.02, 0.92, 1.12), (1.10, 1.04, 0.95, 1.10), (1.8, 0.6, 0.0, 0.45), (1.12, 1.05, 0.95, 1.16), (0.78, 0.86, 1.05, 0.76), 0.72, 0.38);
    }

    private static double ApplyDryWinterHumidSubtropicalFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.45, 0.95, 1.30, 0.88), (0.52, 0.90, 1.12, 0.88), (1.8, 0.12, 0.0, 0.0), (0.58, 0.95, 1.18, 0.88), (1.48, 1.05, 0.72, 1.02), 0.78, 0.42);
    }

    private static double ApplySubtropicalHighlandFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.30, 0.78, 1.15, 0.95), (0.45, 0.82, 1.06, 0.90), (0.65, 0.10, 0.0, 0.0), (0.50, 0.85, 1.08, 0.92), (1.70, 1.12, 0.76, 1.0), 0.78, 0.40);
    }

    private static double ApplyColdSummerSubtropicalHighlandFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.20, 0.65, 1.12, 0.85), (0.38, 0.78, 1.05, 0.88), (2.2, 0.6, 0.04, 0.22), (0.42, 0.82, 1.08, 0.90), (1.90, 1.18, 0.78, 1.05), 0.78, 0.42);
    }

    private static double ApplyTropicalMonsoonFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.78, 0.90, 1.0, 1.12), (0.88, 0.98, 1.02, 1.10), (0.0, 0.0, 0.0, 0.0), (0.82, 0.94, 1.0, 1.10), (1.55, 1.18, 0.82, 0.68), 0.60, 0.25, blockSnow: true);
    }

    private static double ApplyTropicalSavannaDryWinterFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.22, 0.55, 1.35, 1.18), (0.38, 0.65, 1.12, 1.05), (0.0, 0.0, 0.0, 0.0), (0.35, 0.62, 1.18, 1.08), (2.25, 1.55, 0.60, 0.75), 0.62, 0.26, blockSnow: true);
    }

    private static double ApplyTropicalSavannaDrySummerFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (1.02, 0.72, 0.20, 1.65), (1.05, 0.85, 0.45, 1.45), (0.0, 0.0, 0.0, 0.0), (1.08, 0.78, 0.30, 1.60), (0.82, 1.22, 2.20, 0.45), 0.62, 0.26, blockSnow: true);
    }

    private static double ApplyWarmSummerMediterraneanFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.96, 0.86, 0.45, 0.76), (1.0, 0.90, 0.72, 0.82), (0.15, 0.0, 0.0, 0.0), (1.0, 0.92, 0.60, 0.80), (0.95, 1.12, 1.25, 1.12), 0.78, 0.42);
    }

    private static double ApplyColdSummerMediterraneanFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (1.0, 0.92, 0.62, 0.90), (1.04, 1.0, 0.88, 0.96), (3.2, 0.25, 0.0, 0.18), (1.02, 0.98, 0.75, 0.95), (0.92, 1.10, 1.18, 1.05), 0.76, 0.40);
    }

    private static double ApplyHotSummerHumidSubcontinentalFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.95, 1.02, 1.12, 1.0), (0.95, 0.98, 1.02, 1.0), (2.4, 0.18, 0.0, 0.04), (0.98, 1.02, 1.08, 1.0), (1.02, 1.0, 0.92, 0.98), 0.80, 0.55);
    }

    private static double ApplyWarmSummerDryWinterHumidSubcontinentalFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.70, 0.88, 1.12, 1.05), (0.70, 0.88, 1.15, 1.05), (1.35, 0.06, 0.0, 0.0), (0.78, 0.92, 1.12, 1.08), (1.45, 1.08, 0.82, 0.92), 0.78, 0.45);
    }

    private static double ApplySevereWinterSubarcticFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.22, 0.75, 1.0, 0.78), (0.72, 0.80, 1.0, 0.95), (1.25, 0.95, 0.0, 1.05), (0.88, 0.92, 1.0, 0.95), (1.18, 1.05, 0.95, 1.0), 0.68, 0.32);
    }

    private static double ApplyDryWinterSubarcticFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.15, 0.60, 1.08, 0.85), (0.50, 0.75, 1.05, 0.98), (1.05, 0.55, 0.0, 0.72), (0.62, 0.82, 1.05, 0.95), (1.55, 1.12, 0.90, 1.0), 0.70, 0.34);
    }

    private static double ApplySevereWinterDryWinterSubarcticFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.08, 0.50, 1.0, 0.72), (0.38, 0.65, 0.98, 0.92), (0.92, 0.45, 0.0, 0.62), (0.52, 0.78, 1.0, 0.92), (1.72, 1.15, 0.92, 1.02), 0.68, 0.32);
    }

    private static double ApplyHotSummerDrySummerContinentalFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.82, 0.78, 0.18, 0.55), (0.95, 0.85, 0.28, 0.52), (0.90, 0.30, 0.0, 0.08), (0.90, 0.82, 0.22, 0.55), (1.05, 1.18, 1.95, 1.32), 0.80, 0.48);
    }

    private static double ApplyWarmSummerDrySummerContinentalFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.85, 0.72, 0.15, 0.48), (0.92, 0.82, 0.22, 0.50), (1.25, 0.55, 0.0, 0.22), (0.95, 0.85, 0.18, 0.52), (1.02, 1.20, 2.15, 1.30), 0.78, 0.45);
    }

    private static double ApplyDrySummerSubarcticFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.40, 0.72, 0.45, 0.72), (0.95, 0.90, 0.42, 0.78), (1.12, 0.72, 0.0, 0.82), (0.95, 0.90, 0.35, 0.72), (1.02, 1.12, 1.55, 1.15), 0.72, 0.36);
    }

    private static double ApplySevereWinterDrySummerSubarcticFallback(string seasonName, WeatherSeederEventDescriptor fromEvent, WeatherSeederEventDescriptor toEvent, double baseChance)
    {
        return ApplyGenericFallback(seasonName, toEvent, baseChance, (0.06, 0.45, 0.25, 0.52), (0.55, 0.70, 0.30, 0.68), (0.92, 0.55, 0.0, 0.68), (0.70, 0.82, 0.32, 0.72), (1.35, 1.15, 1.55, 1.15), 0.68, 0.32);
    }

    private static double ApplyHumidSubtropicalFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName == "Winter" ? 0.20 : 0.02;
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Summer" => 1.30,
                "Spring" => 1.15,
                "Autumn" => 0.95,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= 1.10;
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName == "Summer" ? 1.20 : 1.10;
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= 0.65;
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.35;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.7;
        }

        return adjusted;
    }

    private static double ApplyMediterraneanFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName == "Winter" ? 0.12 : 0.01;
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.05,
                "Spring" => 0.82,
                "Summer" => 0.20,
                "Autumn" => 0.60,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.05,
                "Spring" => 1.0,
                "Summer" => 0.55,
                "Autumn" => 0.62,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.02,
                "Spring" => 0.90,
                "Summer" => 0.35,
                "Autumn" => 0.60,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.55,
                "Spring" => 1.0,
                "Summer" => 1.75,
                "Autumn" => 1.55,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.40;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.75;
        }

        return adjusted;
    }

    private static double ApplyHumidSubcontinentalFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 3.2,
                "Autumn" => 0.35,
                "Spring" => 0.22,
                _ => 0.02
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.68,
                "Spring" => 1.10,
                "Summer" => 1.15,
                "Autumn" => 1.05,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.10,
                "Autumn" => 1.06,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.15,
                "Autumn" => 1.10,
                "Spring" => 1.05,
                "Summer" => 0.95,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.45,
                "Spring" => 0.70,
                "Summer" => 0.85,
                "Autumn" => 0.60,
                _ => 0.7
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.55;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.80;
        }

        return adjusted;
    }

    private static double ApplyDryWinterHumidSubcontinentalFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 2.8,
                "Autumn" => 0.08,
                "Spring" => 0.04,
                _ => 0.01
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.45,
                "Spring" => 0.95,
                "Summer" => 1.40,
                "Autumn" => 0.85,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.75,
                "Spring" => 1.0,
                "Summer" => 1.15,
                "Autumn" => 0.95,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.70,
                "Spring" => 0.95,
                "Summer" => 1.25,
                "Autumn" => 1.0,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.45,
                "Spring" => 0.95,
                "Summer" => 0.45,
                "Autumn" => 0.90,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.45;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.80;
        }

        return adjusted;
    }

    private static double ApplySubarcticFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 6.8,
                "Spring" => 0.80,
                "Autumn" => 1.25,
                _ => 0.02
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.18,
                "Spring" => 0.90,
                "Summer" => 1.05,
                "Autumn" => 0.80,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.75,
                "Spring" => 0.95,
                "Summer" => 1.0,
                "Autumn" => 1.05,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.85,
                "Spring" => 0.95,
                "Summer" => 1.05,
                "Autumn" => 1.10,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.35,
                "Spring" => 1.0,
                "Summer" => 0.80,
                "Autumn" => 0.85,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.35;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.70;
        }

        return adjusted;
    }

    private static double ApplyTundraFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.55,
                "Spring" => 1.18,
                "Autumn" => 1.75,
                _ => 0.04
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.40,
                "Spring" => 1.0,
                "Summer" => 1.12,
                "Autumn" => 1.28,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.18,
                "Spring" => 1.10,
                "Summer" => 1.02,
                "Autumn" => 1.20,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.22,
                "Spring" => 1.12,
                "Summer" => 1.05,
                "Autumn" => 1.24,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.58,
                "Spring" => 0.72,
                "Summer" => 0.78,
                "Autumn" => 0.62,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.45;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.80;
        }

        return adjusted;
    }

    private static double ApplyPolarIceCapFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.00,
                "Spring" => 0.85,
                "Summer" => 1.10,
                "Autumn" => 0.90,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            return 0.0;
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= 0.18;
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= 0.65;
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.55,
                "Spring" => 1.70,
                "Summer" => 1.45,
                "Autumn" => 1.65,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.85;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.95;
        }

        return adjusted;
    }

    private static double ApplyHotSemiAridFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= 0.02;
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.25,
                "Spring" => 0.58,
                "Summer" => 0.55,
                "Autumn" => 0.78,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.30,
                "Spring" => 0.42,
                "Summer" => 0.36,
                "Autumn" => 0.48,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.40,
                "Spring" => 0.56,
                "Summer" => 0.45,
                "Autumn" => 0.58,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.62,
                "Spring" => 1.48,
                "Summer" => 1.55,
                "Autumn" => 1.38,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.55;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.85;
        }

        return adjusted;
    }

    private static double ApplyColdSemiAridFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.30,
                "Spring" => 0.32,
                "Autumn" => 0.16,
                _ => 0.02
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.20,
                "Spring" => 0.62,
                "Summer" => 0.72,
                "Autumn" => 0.48,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.24,
                "Spring" => 0.42,
                "Summer" => 0.44,
                "Autumn" => 0.28,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.54,
                "Spring" => 0.62,
                "Summer" => 0.58,
                "Autumn" => 0.48,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.52,
                "Spring" => 1.32,
                "Summer" => 1.24,
                "Autumn" => 1.46,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.70;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.90;
        }

        return adjusted;
    }

    private static double ApplyHotDesertFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            return 0.0;
        }

        if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.08,
                "Spring" => 0.10,
                "Summer" => 0.22,
                "Autumn" => 0.28,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.08,
                "Spring" => 0.08,
                "Summer" => 0.12,
                "Autumn" => 0.16,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.10,
                "Spring" => 0.10,
                "Summer" => 0.14,
                "Autumn" => 0.18,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.80,
                "Spring" => 1.90,
                "Summer" => 2.05,
                "Autumn" => 1.72,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.60;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.85;
        }

        return adjusted;
    }

    private static double ApplyColdDesertFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.85,
                "Spring" => 0.20,
                "Autumn" => 0.08,
                _ => 0.01
            };
        }
        else if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.20,
                "Spring" => 0.24,
                "Summer" => 0.16,
                "Autumn" => 0.22,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.26,
                "Spring" => 0.22,
                "Summer" => 0.18,
                "Autumn" => 0.20,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 0.36,
                "Spring" => 0.28,
                "Summer" => 0.22,
                "Autumn" => 0.24,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.50,
                "Spring" => 1.58,
                "Summer" => 1.70,
                "Autumn" => 1.56,
                _ => 1.0
            };
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.75;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.90;
        }

        return adjusted;
    }

    private static double ApplyTropicalRainforestFallback(
        string seasonName,
        WeatherSeederEventDescriptor fromEvent,
        WeatherSeederEventDescriptor toEvent,
        double baseChance)
    {
        double adjusted = baseChance;
        if (toEvent.Precipitation.IsSnowing())
        {
            return 0.0;
        }

        if (toEvent.Precipitation.IsRaining())
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.35,
                "Spring" => 1.18,
                "Summer" => 1.12,
                "Autumn" => 1.30,
                _ => 1.0
            };
        }
        else if (toEvent.Precipitation == PrecipitationLevel.Humid)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.35,
                "Spring" => 1.20,
                "Summer" => 1.18,
                "Autumn" => 1.28,
                _ => 1.0
            };
        }

        if (toEvent.CloudVariation == CloudVariation.Overcast)
        {
            adjusted *= seasonName switch
            {
                "Winter" => 1.30,
                "Spring" => 1.15,
                "Summer" => 1.12,
                "Autumn" => 1.26,
                _ => 1.0
            };
        }

        if (toEvent.Precipitation == PrecipitationLevel.Dry || toEvent.Precipitation == PrecipitationLevel.Parched)
        {
            adjusted *= 0.25;
        }

        if (toEvent.Wind >= WindLevel.GaleWind)
        {
            adjusted *= 0.25;
        }
        else if (toEvent.Wind >= WindLevel.StrongWind)
        {
            adjusted *= 0.60;
        }

        return adjusted;
    }

    private static WeatherSeederEventDescriptor DescribeEvent(WeatherEvent weatherEvent)
    {
        Match match = TempVariationRegex.Match(weatherEvent.Name);
        WeatherEventVariation variation = match.Success
            ? Enum.Parse<WeatherEventVariation>(match.Value)
            : WeatherEventVariation.Normal;
        WindVariation windVariation = weatherEvent.Name.Contains("Polar", StringComparison.Ordinal)
            ? WindVariation.Polar
            : weatherEvent.Name.Contains("Equatorial", StringComparison.Ordinal)
                ? WindVariation.Equatorial
                : WindVariation.Normal;
        CloudVariation cloudVariation = CloudVariationRegex.IsMatch(weatherEvent.Name)
            ? weatherEvent.Name.StartsWith("Overcast", StringComparison.Ordinal)
                ? CloudVariation.Overcast
                : CloudVariation.Cloudy
            : CloudVariation.None;
        return new WeatherSeederEventDescriptor((PrecipitationLevel)weatherEvent.Precipitation, (WindLevel)weatherEvent.Wind, variation, windVariation, cloudVariation);
    }

    private static double GetCollapsedTemperatureVariationReallocationChance(
        WeatherSeederClimateProfile profile,
        string seasonName,
        WeatherSeederEventDescriptor descriptor)
    {
        int reallocationCount = 0;
        if (descriptor.TemperatureVariation < WeatherEventVariation.Sweltering)
        {
            reallocationCount++;
        }

        if (descriptor.TemperatureVariation > WeatherEventVariation.Freezing)
        {
            reallocationCount++;
        }

        return reallocationCount * profile.TemperatureVariationChance(seasonName);
    }
}
