using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Climate.Analysis;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SeasonModel = MudSharp.Models.Season;

namespace MudSharp_Unit_Tests;

[TestClass]
[TestCategory("ClimateSeeder")]
public class WeatherSeederClimateTests
{
    private static readonly InMemoryDatabaseRoot DatabaseRoot = new();
    private static readonly object SeededContextLock = new();
    private static bool _sharedDatabaseSeeded;

    [TestMethod]
    public void WeatherSeeder_TemperateOceanicProfile_RemainsWithinBroadLondonBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Oceanic Temperate Northern Hemisphere",
            "Seeded Oceanic Temperate");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double lateAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_late_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 20.0 and <= 40.0, $"Expected annual wet occupancy to be broadly oceanic, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 22.0 and <= 45.0, $"Expected mid-winter wet occupancy to be believable for London, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 15.0 and <= 35.0, $"Expected mid-summer wet occupancy to avoid both drought and constant rain, but got {summerWet:F2}%.");
        Assert.IsTrue(winterWet > summerWet, $"Expected winter to be wetter than summer, but winter={winterWet:F2}% and summer={summerWet:F2}%.");
        Assert.IsTrue(annualSnow is > 0.0 and <= 3.0, $"Expected snow to remain uncommon in London-like conditions, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 6.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.5, $"Expected hurricane-force or worse wind to be vanishingly rare, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 70.0, $"Expected most weather to stay at breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 3.0 and <= 7.0, $"Expected mid-winter mean sheltered temperature to be mild, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 16.0 and <= 20.5, $"Expected mid-summer mean sheltered temperature to be warm without heat, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(lateAutumnShelterMean is >= 7.0 and <= 11.5, $"Expected late autumn mean sheltered temperature to stay cool but not harsh, but got {lateAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_HumidSubtropicalProfile_RemainsWithinBroadAtlantaBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Humid Subtropical Northern Hemisphere",
            "Seeded Humid Subtropical");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double lateAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_late_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 25.0 and <= 45.0, $"Expected annual wet occupancy to stay in a humid subtropical band, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 18.0 and <= 40.0, $"Expected mid-winter wet occupancy to remain present without a dry season, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 28.0 and <= 53.0, $"Expected mid-summer wet occupancy to reflect a convective summer maximum, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 15.0 and <= 32.0, $"Expected mid-autumn wet occupancy to be the comparatively drier season, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > autumnWet, $"Expected summer to be wetter than autumn, but summer={summerWet:F2}% and autumn={autumnWet:F2}%.");
        Assert.IsTrue(annualSnow <= 0.8, $"Expected snow to remain very rare in Atlanta-like conditions, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 3.0, $"Expected gale-force or worse wind to remain rare, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 78.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 5.5 and <= 9.5, $"Expected mid-winter mean sheltered temperature to stay mild, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 24.0 and <= 28.8, $"Expected mid-summer mean sheltered temperature to be hot and humid, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(lateAutumnShelterMean is >= 10.0 and <= 14.5, $"Expected late autumn mean sheltered temperature to stay mild, but got {lateAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_MediterraneanProfile_RemainsWithinBroadSacramentoBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Mediterranean Northern Hemisphere",
            "Seeded Mediterranean");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 8.0 and <= 21.0, $"Expected annual wet occupancy to stay low for a dry-summer climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 20.0 and <= 45.0, $"Expected mid-winter wet occupancy to carry the mediterranean rainy season, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 0.0 and <= 4.0, $"Expected mid-summer wet occupancy to be near-zero, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 3.0 and <= 18.0, $"Expected mid-autumn wet occupancy to stay below winter while beginning the rainy season, but got {autumnWet:F2}%.");
        Assert.IsTrue(winterWet > autumnWet, $"Expected winter to be wetter than autumn, but winter={winterWet:F2}% and autumn={autumnWet:F2}%.");
        Assert.IsTrue(autumnWet > summerWet, $"Expected autumn to be wetter than summer, but autumn={autumnWet:F2}% and summer={summerWet:F2}%.");
        Assert.IsTrue(annualSnow <= 0.1, $"Expected snow to be essentially absent in Sacramento-like conditions, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 2.0, $"Expected gale-force or worse wind to remain rare, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 80.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 7.0 and <= 10.5, $"Expected mid-winter mean sheltered temperature to stay cool and mild, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 22.5 and <= 26.5, $"Expected mid-summer mean sheltered temperature to be hot and dry, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 16.5 and <= 20.5, $"Expected mid-autumn mean sheltered temperature to stay warm, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_HumidSubcontinentalProfile_RemainsWithinBroadBuffaloBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Humid Subcontinental Northern Hemisphere",
            "Seeded Humid Subcontinental");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 18.0 and <= 35.0, $"Expected annual wet occupancy to stay in a humid subcontinental band, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 18.0 and <= 38.0, $"Expected mid-winter wet occupancy to stay active without a dry season, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 18.0 and <= 38.0, $"Expected mid-summer wet occupancy to remain present in a no-dry-season climate, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 15.0 and <= 35.0, $"Expected mid-autumn wet occupancy to remain moderate, but got {autumnWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 2.0 and <= 12.0, $"Expected snow to be a noticeable part of the annual climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 10.0 and <= 40.0, $"Expected mid-winter snow occupancy to be substantial, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 4.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.15, $"Expected hurricane-force or worse wind to be vanishingly rare, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 75.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -6.0 and <= -1.0, $"Expected mid-winter mean sheltered temperature to stay well below freezing, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 20.0 and <= 24.5, $"Expected mid-summer mean sheltered temperature to be warm without deep subtropical heat, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 9.0 and <= 13.5, $"Expected mid-autumn mean sheltered temperature to be cool and brisk, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_DryWinterHumidSubcontinentalProfile_RemainsWithinBroadSeoulBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Dry Winter Humid Subcontinental Northern Hemisphere",
            "Seeded Dry Winter Humid Subcontinental");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 18.0 and <= 40.0, $"Expected annual wet occupancy to remain monsoonal but not constant, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 2.0 and <= 15.0, $"Expected mid-winter wet occupancy to stay clearly drier than other seasons, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 40.0 and <= 70.0, $"Expected mid-summer wet occupancy to reflect the East Asian monsoon, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 12.0 and <= 35.0, $"Expected mid-autumn wet occupancy to ease back from summer while remaining wetter than winter, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > autumnWet, $"Expected summer to be wetter than autumn, but summer={summerWet:F2}% and autumn={autumnWet:F2}%.");
        Assert.IsTrue(autumnWet > winterWet, $"Expected autumn to be wetter than winter, but autumn={autumnWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 0.4 and <= 3.0, $"Expected annual snow occupancy to remain present but clearly secondary to rain, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 1.5 and <= 10.0, $"Expected mid-winter snow occupancy to remain noticeable, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 3.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.10, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 80.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -4.5 and <= 1.5, $"Expected mid-winter mean sheltered temperature to stay cold but not subarctic, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 23.5 and <= 27.5, $"Expected mid-summer mean sheltered temperature to be hot and humid, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 13.0 and <= 18.0, $"Expected mid-autumn mean sheltered temperature to stay mild, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_SubarcticProfile_RemainsWithinBroadBratskBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Subarctic Northern Hemisphere",
            "Seeded Subarctic");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 12.0 and <= 30.0, $"Expected annual wet occupancy to stay moderate in a continental subarctic climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 8.0 and <= 25.0, $"Expected mid-winter wet occupancy to stay lower than warmer humid continental climates, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 18.0 and <= 40.0, $"Expected mid-summer wet occupancy to reflect the wettest season without becoming monsoonal, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 12.0 and <= 35.0, $"Expected mid-autumn wet occupancy to remain noticeable during the cooling season, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > winterWet, $"Expected summer to be wetter than winter, but summer={summerWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(autumnWet > winterWet, $"Expected autumn to be wetter than winter, but autumn={autumnWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 2.0 and <= 12.0, $"Expected snow to be a significant part of the annual climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 6.0 and <= 25.0, $"Expected mid-winter snow occupancy to be a major component of the climate, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 2.5, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 82.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -22.0 and <= -17.0, $"Expected mid-winter mean sheltered temperature to stay deeply subfreezing, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 17.0 and <= 21.0, $"Expected mid-summer mean sheltered temperature to remain mild rather than hot, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= -1.5 and <= 4.0, $"Expected mid-autumn mean sheltered temperature to hover near freezing, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_TundraProfile_RemainsWithinBroadNuukBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Tundra Northern Hemisphere",
            "Seeded Tundra");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 15.0 and <= 35.0, $"Expected annual rain occupancy to stay moderate in a coastal tundra climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 3.0 and <= 20.5, $"Expected mid-winter rain occupancy to stay low because winter precipitation should mostly fall as snow, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 15.0 and <= 35.0, $"Expected mid-summer rain occupancy to remain clearly present in Nuuk-like conditions, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 18.0 and <= 40.0, $"Expected mid-autumn rain occupancy to stay noticeable in a maritime tundra climate, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > winterWet, $"Expected summer to be rainier than winter, but summer={summerWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(autumnWet > winterWet, $"Expected autumn to be rainier than winter, but autumn={autumnWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 4.0 and <= 24.0, $"Expected snow to be a major part of the annual precipitation mix, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 10.0 and <= 55.0, $"Expected mid-winter snow occupancy to be high in Nuuk-like conditions, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 5.0, $"Expected gale-force or worse wind to remain uncommon rather than dominant, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.10, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 70.0, $"Expected most weather to remain at breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -10.0 and <= -4.0, $"Expected mid-winter mean sheltered temperature to be cold but not polar-desert cold, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 5.0 and <= 9.5, $"Expected mid-summer mean sheltered temperature to stay below the tree-line threshold, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 0.0 and <= 5.5, $"Expected mid-autumn mean sheltered temperature to hover just above freezing, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_PolarIceCapProfile_RemainsWithinBroadSouthPoleBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Polar Ice Cap Northern Hemisphere",
            "Seeded Polar Ice Cap");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercentOrZero(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercentOrZero(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercentOrZero(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet <= 1.5, $"Expected liquid rain occupancy to be effectively absent in an Antarctic ice-cap climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet <= 2.5, $"Expected mid-winter rain occupancy to be absent in an Antarctic ice-cap climate, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet <= 1.5, $"Expected mid-summer rain occupancy to remain absent in an Antarctic ice-cap climate, but got {summerWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 0.2 and <= 8.0, $"Expected annual snow occupancy to stay low but non-zero in an extremely dry polar climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow <= 6.0, $"Expected mid-winter snow occupancy to stay limited in a polar desert, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 8.0, $"Expected gale-force or worse wind to stay uncommon rather than constant, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 1.0, $"Expected hurricane-force or worse wind to remain rare even in Antarctica, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 55.0, $"Expected a majority of weather to remain breeze strength or calmer at South Pole scale, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -62.0 and <= -48.0, $"Expected mid-winter mean sheltered temperature to be extremely cold, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= -32.0 and <= -20.0, $"Expected mid-summer mean sheltered temperature to stay well below freezing, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= -51.0 and <= -34.0, $"Expected mid-autumn mean sheltered temperature to remain deeply subfreezing, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_HotSemiAridProfile_RemainsWithinBroadDelRioBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Hot Semi-Arid Northern Hemisphere",
            "Seeded Hot Semi-Arid");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 5.0 and <= 25.0, $"Expected annual wet occupancy to stay clearly steppe-like, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 0.0 and <= 8.0, $"Expected mid-winter wet occupancy to remain low in Del Rio-like conditions, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 8.0 and <= 32.0, $"Expected mid-summer wet occupancy to reflect warm-season thunderstorms without becoming humid subtropical, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 6.0 and <= 22.0, $"Expected mid-autumn wet occupancy to remain present in the late warm-season storm period, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > winterWet, $"Expected summer to be wetter than winter, but summer={summerWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(autumnWet > winterWet, $"Expected autumn to be wetter than winter, but autumn={autumnWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(annualSnow <= 0.2, $"Expected snow to be effectively absent in a hot semi-arid climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 4.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.10, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 72.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 10.0 and <= 16.0, $"Expected mid-winter mean sheltered temperature to stay mild, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 28.0 and <= 33.5, $"Expected mid-summer mean sheltered temperature to be very hot, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 20.0 and <= 26.5, $"Expected mid-autumn mean sheltered temperature to stay hot to warm, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_ColdSemiAridProfile_RemainsWithinBroadDenverBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Cold Semi-Arid Northern Hemisphere",
            "Seeded Cold Semi-Arid");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 8.0 and <= 22.0, $"Expected annual wet occupancy to stay low but not desert-dry, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 0.0 and <= 12.0, $"Expected mid-winter wet occupancy to remain limited in a cold steppe climate, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 10.0 and <= 28.0, $"Expected mid-summer wet occupancy to reflect the wetter convective season, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 4.0 and <= 20.0, $"Expected mid-autumn wet occupancy to stay below summer in Denver-like conditions, but got {autumnWet:F2}%.");
        Assert.IsTrue(summerWet > winterWet, $"Expected summer to be wetter than winter, but summer={summerWet:F2}% and winter={winterWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 0.8 and <= 6.0, $"Expected snow to remain present but secondary in a cold semi-arid climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 2.0 and <= 16.0, $"Expected mid-winter snow occupancy to remain noticeable, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 5.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.10, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 68.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= -2.5 and <= 4.5, $"Expected mid-winter mean sheltered temperature to hover around freezing, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 22.0 and <= 26.5, $"Expected mid-summer mean sheltered temperature to be warm to hot without hot-desert heat, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 8.0 and <= 15.5, $"Expected mid-autumn mean sheltered temperature to be mild to cool, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_HotDesertProfile_RemainsWithinBroadYumaBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Hot Desert Northern Hemisphere",
            "Seeded Hot Desert");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet <= 6.0, $"Expected annual wet occupancy to stay extremely low in a hot desert climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet <= 4.0, $"Expected mid-winter wet occupancy to stay very low in Yuma-like conditions, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet <= 10.5, $"Expected mid-summer wet occupancy to stay very low even with desert thunderstorms, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet <= 8.0, $"Expected mid-autumn wet occupancy to stay very low in Yuma-like conditions, but got {autumnWet:F2}%.");
        Assert.IsTrue(annualSnow <= 0.01, $"Expected snow to be absent in a hot desert climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 3.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 78.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 11.5 and <= 17.5, $"Expected mid-winter mean sheltered temperature to stay mild, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 32.5 and <= 38.0, $"Expected mid-summer mean sheltered temperature to be extremely hot, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 22.0 and <= 28.5, $"Expected mid-autumn mean sheltered temperature to stay very warm, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_ColdDesertProfile_RemainsWithinBroadBishopBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Cold Desert Northern Hemisphere",
            "Seeded Cold Desert");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 0.5 and <= 8.0, $"Expected annual wet occupancy to stay very low in a cold desert climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet <= 8.0, $"Expected mid-winter wet occupancy to stay limited in Bishop-like conditions, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet <= 5.0, $"Expected mid-summer wet occupancy to stay low in a cold desert climate, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet <= 5.0, $"Expected mid-autumn wet occupancy to stay low in a cold desert climate, but got {autumnWet:F2}%.");
        Assert.IsTrue(annualSnow is >= 0.10 and <= 5.0, $"Expected snow to remain present but sparse in a cold desert climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(winterSnow is >= 0.4 and <= 12.0, $"Expected mid-winter snow occupancy to remain noticeable but limited, but got {winterSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 4.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 75.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 2.0 and <= 7.5, $"Expected mid-winter mean sheltered temperature to stay chilly but usually above severe-freeze levels, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 23.0 and <= 28.5, $"Expected mid-summer mean sheltered temperature to be hot but not tropical, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 12.0 and <= 18.5, $"Expected mid-autumn mean sheltered temperature to stay mild, but got {midAutumnShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_TropicalRainforestProfile_RemainsWithinBroadSingaporeBounds()
    {
        WeatherStatisticsResult result = AnalyzeSeededNorthernHemisphereClimate(
            "Tropical Rainforest Northern Hemisphere",
            "Seeded Tropical Rainforest");

        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        Assert.IsTrue(annualWet is >= 28.0 and <= 55.0, $"Expected annual wet occupancy to stay high in an equatorial rainforest climate, but got {annualWet:F2}%.");
        Assert.IsTrue(winterWet is >= 28.0 and <= 60.0, $"Expected mid-winter wet occupancy to stay high during the wetter northeast monsoon season, but got {winterWet:F2}%.");
        Assert.IsTrue(summerWet is >= 22.0 and <= 55.0, $"Expected mid-summer wet occupancy to stay high without a dry season, but got {summerWet:F2}%.");
        Assert.IsTrue(autumnWet is >= 26.0 and <= 60.0, $"Expected mid-autumn wet occupancy to stay high in Singapore-like conditions, but got {autumnWet:F2}%.");
        Assert.IsTrue(annualSnow <= 0.01, $"Expected snow to be absent in an equatorial rainforest climate, but got {annualSnow:F2}%.");
        Assert.IsTrue(annualGaleOrWorse <= 2.0, $"Expected gale-force or worse wind to remain rare, but got {annualGaleOrWorse:F2}%.");
        Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
        Assert.IsTrue(annualBreezeOrCalmer >= 80.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
        Assert.IsTrue(midWinterShelterMean is >= 25.0 and <= 28.5, $"Expected mid-winter mean sheltered temperature to remain tropical, but got {midWinterShelterMean:F2}C.");
        Assert.IsTrue(midSummerShelterMean is >= 25.0 and <= 28.5, $"Expected mid-summer mean sheltered temperature to remain tropical, but got {midSummerShelterMean:F2}C.");
        Assert.IsTrue(midAutumnShelterMean is >= 25.0 and <= 28.5, $"Expected mid-autumn mean sheltered temperature to remain tropical, but got {midAutumnShelterMean:F2}C.");
        Assert.IsTrue(Math.Abs(midSummerShelterMean - midWinterShelterMean) <= 1.5, $"Expected the tropical rainforest annual temperature range to stay small, but winter={midWinterShelterMean:F2}C and summer={midSummerShelterMean:F2}C.");
    }

    [TestMethod]
    public void WeatherSeeder_SeededClimateDescriptions_ArePopulatedAndIncludeReferenceMetadata()
    {
        using FuturemudDatabaseContext context = CreateSeededWeatherContext();
        Assert.AreEqual(31, context.ClimateModels.Count(), "Expected the seeded weather data to create thirty-one climate models.");
        Assert.AreEqual(31, context.RegionalClimates.Count(), "Expected the seeded weather data to create one regional climate baseline per climate model.");
        Assert.AreEqual(12, context.Seasons.Count(), "Expected the seeded weather data to create one shared set of twelve seasons.");
        Assert.IsFalse(context.Seasons.Any(x => x.Name.EndsWith("_south", StringComparison.OrdinalIgnoreCase)), "Expected the seeded weather data to stop creating duplicated southern seasons.");

        foreach (ClimateModel climateModel in context.ClimateModels)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(climateModel.Description), $"Expected climate model {climateModel.Name} to have a description.");
            Assert.IsTrue(climateModel.Description.Contains("Koppen climate classification", StringComparison.Ordinal), $"Expected climate model {climateModel.Name} to mention its Koppen classification.");
        }

        foreach (MudSharp.Models.RegionalClimate regionalClimate in context.RegionalClimates)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(regionalClimate.Description), $"Expected regional climate {regionalClimate.Name} to have a description.");
            Assert.IsTrue(regionalClimate.Description.Contains("Koppen climate classification", StringComparison.Ordinal), $"Expected regional climate {regionalClimate.Name} to mention its Koppen classification.");
            Assert.IsTrue(regionalClimate.Description.Contains("Mid Winter", StringComparison.Ordinal), $"Expected regional climate {regionalClimate.Name} to mention Mid Winter temperatures.");
            Assert.IsTrue(regionalClimate.Description.Contains("Mid Summer", StringComparison.Ordinal), $"Expected regional climate {regionalClimate.Name} to mention Mid Summer temperatures.");
        }

        ClimateModel oceanicModel = context.ClimateModels.Single(x => x.Name == "Temperate");
        Assert.IsTrue(oceanicModel.Description.Contains("London", StringComparison.Ordinal), "Expected the oceanic climate model description to reference London.");
        Assert.IsTrue(oceanicModel.Description.Contains("Cfb", StringComparison.Ordinal), "Expected the oceanic climate model description to reference Cfb.");

        ClimateModel dryWinterModel = context.ClimateModels.Single(x => x.Name == "Dry Winter Humid Subcontinental");
        Assert.IsTrue(dryWinterModel.Description.Contains("Seoul", StringComparison.Ordinal), "Expected the dry winter humid subcontinental climate model description to reference Seoul.");
        Assert.IsTrue(dryWinterModel.Description.Contains("Dwa", StringComparison.Ordinal), "Expected the dry winter humid subcontinental climate model description to reference Dwa.");

        MudSharp.Models.RegionalClimate tropicalRegional = context.RegionalClimates.Single(x => x.Name == "Tropical Rainforest Northern Hemisphere");
        Assert.IsTrue(tropicalRegional.Description.Contains("Singapore", StringComparison.Ordinal), "Expected the tropical rainforest regional climate description to reference Singapore.");
        Assert.IsTrue(tropicalRegional.Description.Contains("opposite-hemisphere", StringComparison.OrdinalIgnoreCase), "Expected the tropical rainforest regional climate description to explain opposite-hemisphere controller usage.");
    }

    [TestMethod]
    public void WeatherSeeder_SeedsSingleCanonicalWeatherEventSet()
    {
        using FuturemudDatabaseContext context = CreateFreshSeededWeatherContext();
        Assert.AreEqual(322, context.WeatherEvents.Count(), "Expected the weather seeder to create the canonical single-tier weather event set.");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetMaritimeAndTropicalClimateExpectations), DynamicDataSourceType.Method)]
    public void WeatherSeeder_MaritimeAndTropicalKoppenProfiles_RemainWithinBroadReferenceBounds(ClimateExpectation expectation)
    {
        AssertClimateExpectation(expectation);
    }

    [DataTestMethod]
    [DynamicData(nameof(GetWarmTemperateClimateExpectations), DynamicDataSourceType.Method)]
    public void WeatherSeeder_WarmTemperateKoppenProfiles_RemainWithinBroadReferenceBounds(ClimateExpectation expectation)
    {
        AssertClimateExpectation(expectation);
    }

    [DataTestMethod]
    [DynamicData(nameof(GetHotAndMonsoonalContinentalClimateExpectations), DynamicDataSourceType.Method)]
    public void WeatherSeeder_HotAndMonsoonalContinentalKoppenProfiles_RemainWithinBroadReferenceBounds(ClimateExpectation expectation)
    {
        AssertClimateExpectation(expectation);
    }

    [DataTestMethod]
    [DynamicData(nameof(GetSevereContinentalClimateExpectations), DynamicDataSourceType.Method)]
    public void WeatherSeeder_SevereContinentalKoppenProfiles_RemainWithinBroadReferenceBounds(ClimateExpectation expectation)
    {
        AssertClimateExpectation(expectation);
    }

    private static void AssertClimateExpectation(ClimateExpectation expectation)
    {
        WeatherStatisticsResult result = AnalyzeSeededClimate(expectation.RegionalClimateName, expectation.ControllerName, years: 3, burnInYears: 1);
        double annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
        double winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
        double summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
        double autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
        double annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
        double winterSnow = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Snow");
        double annualGaleOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Gale Wind",
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualHurricaneOrWorse = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "Hurricane Wind",
                "Maelstrom Wind"
            });
        double annualBreezeOrCalmer = WeightedAveragePercent(
            result.Rows,
            "Wind",
            new[]
            {
                "None",
                "Still",
                "Occasional Breeze",
                "Breeze"
            });
        double midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
        double midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
        double midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

        List<string> failures = new();

        CheckBetween(annualWet, expectation.AnnualWetMin, expectation.AnnualWetMax, $"{expectation.RegionalClimateName} annual wet occupancy", failures);
        CheckBetween(winterWet, expectation.WinterWetMin, expectation.WinterWetMax, $"{expectation.RegionalClimateName} mid-winter wet occupancy", failures);
        CheckBetween(summerWet, expectation.SummerWetMin, expectation.SummerWetMax, $"{expectation.RegionalClimateName} mid-summer wet occupancy", failures);
        CheckBetween(autumnWet, expectation.AutumnWetMin, expectation.AutumnWetMax, $"{expectation.RegionalClimateName} mid-autumn wet occupancy", failures);
        CheckBetween(annualSnow, expectation.AnnualSnowMin, expectation.AnnualSnowMax, $"{expectation.RegionalClimateName} annual snow occupancy", failures);
        CheckBetween(winterSnow, expectation.WinterSnowMin, expectation.WinterSnowMax, $"{expectation.RegionalClimateName} mid-winter snow occupancy", failures);
        CheckMaximum(annualGaleOrWorse, expectation.AnnualGaleOrWorseMax, $"{expectation.RegionalClimateName} annual gale-force-or-worse wind occupancy", failures);
        CheckMaximum(annualHurricaneOrWorse, expectation.AnnualHurricaneOrWorseMax, $"{expectation.RegionalClimateName} annual hurricane-force-or-worse wind occupancy", failures);
        CheckMinimum(annualBreezeOrCalmer, expectation.AnnualBreezeOrCalmerMin, $"{expectation.RegionalClimateName} annual breeze-or-calmer wind occupancy", failures);
        CheckBetween(midWinterShelterMean, expectation.MidWinterMin, expectation.MidWinterMax, $"{expectation.RegionalClimateName} mid-winter sheltered temperature", failures);
        CheckBetween(midSummerShelterMean, expectation.MidSummerMin, expectation.MidSummerMax, $"{expectation.RegionalClimateName} mid-summer sheltered temperature", failures);
        CheckBetween(midAutumnShelterMean, expectation.MidAutumnMin, expectation.MidAutumnMax, $"{expectation.RegionalClimateName} mid-autumn sheltered temperature", failures);

        Assert.IsFalse(failures.Any(), string.Join(Environment.NewLine, failures));
    }

    private static WeatherStatisticsResult AnalyzeSeededNorthernHemisphereClimate(string regionalClimateName, string controllerName)
    {
        return AnalyzeSeededClimate(regionalClimateName, controllerName);
    }

    public static IEnumerable<object[]> GetAdditionalClimateExpectations()
    {
        yield return new object[] { new ClimateExpectation("Subpolar Oceanic Northern Hemisphere", "Seeded Subpolar Oceanic", 20.0, 45.0, 20.0, 45.0, 14.0, 35.0, 20.0, 45.0, 0.4, 8.0, 1.0, 15.0, 7.0, 0.10, 72.0, -2.0, 4.0, 8.0, 12.0, 4.0, 8.0) };
        yield return new object[] { new ClimateExpectation("Dry Winter Humid Subtropical Northern Hemisphere", "Seeded Dry Winter Humid Subtropical", 18.0, 38.0, 2.0, 12.0, 35.0, 65.0, 15.0, 35.0, 0.0, 1.0, 0.0, 3.0, 3.0, 0.05, 78.0, 1.0, 6.0, 25.0, 29.5, 14.0, 20.0) };
        yield return new object[] { new ClimateExpectation("Subtropical Highland Northern Hemisphere", "Seeded Subtropical Highland", 15.0, 35.0, 0.0, 10.0, 25.0, 57.5, 10.0, 25.0, 0.0, 0.5, 0.0, 1.0, 3.0, 0.05, 80.0, 9.0, 14.0, 16.0, 20.0, 11.0, 16.0) };
        yield return new object[] { new ClimateExpectation("Cold Summer Subtropical Highland Northern Hemisphere", "Seeded Cold Summer Subtropical Highland", 10.0, 30.0, 0.0, 8.0, 20.0, 50.0, 10.0, 25.0, 0.0, 6.0, 0.0, 4.0, 3.0, 0.05, 80.0, 0.5, 6.0, 8.0, 12.0, 4.0, 9.0) };
        yield return new object[] { new ClimateExpectation("Warm-Summer Mediterranean Northern Hemisphere", "Seeded Warm-Summer Mediterranean", 8.0, 25.0, 18.0, 40.0, 0.0, 8.0, 0.5, 20.0, 0.0, 0.2, 0.0, 0.5, 2.0, 0.05, 82.0, 9.0, 13.0, 15.0, 18.5, 15.0, 19.0) };
        yield return new object[] { new ClimateExpectation("Cold-Summer Mediterranean Northern Hemisphere", "Seeded Cold-Summer Mediterranean", 10.0, 30.0, 20.0, 45.0, 0.0, 10.0, 0.0, 25.0, 0.0, 6.0, 0.0, 12.0, 3.0, 0.05, 78.0, 1.0, 5.0, 10.0, 13.5, 6.0, 10.0) };
        yield return new object[] { new ClimateExpectation("Tropical Monsoon Northern Hemisphere", "Seeded Tropical Monsoon", 25.0, 55.0, 10.0, 35.0, 35.0, 65.0, 20.0, 60.0, 0.0, 0.01, 0.0, 0.01, 2.0, 0.05, 80.0, 19.0, 25.0, 27.0, 30.0, 25.0, 28.5) };
        yield return new object[] { new ClimateExpectation("Tropical Savanna Dry Winter Northern Hemisphere", "Seeded Tropical Savanna Dry Winter", 10.0, 35.0, 0.0, 8.0, 30.0, 65.0, 20.0, 50.0, 0.0, 0.01, 0.0, 0.01, 2.0, 0.05, 82.0, 23.0, 28.0, 28.0, 31.5, 25.0, 29.0) };
        yield return new object[] { new ClimateExpectation("Tropical Savanna Dry Summer Northern Hemisphere", "Seeded Tropical Savanna Dry Summer", 10.0, 35.0, 15.0, 45.0, 0.0, 32.0, 25.0, 70.0, 0.0, 0.01, 0.0, 0.01, 2.0, 0.05, 82.0, 22.0, 26.5, 29.0, 33.5, 25.0, 29.0) };
        yield return new object[] { new ClimateExpectation("Hot Summer Humid Subcontinental Northern Hemisphere", "Seeded Hot Summer Humid Subcontinental", 20.0, 45.0, 18.0, 35.0, 24.0, 50.0, 18.0, 35.0, 1.0, 8.0, 4.0, 18.0, 5.0, 0.05, 78.0, -5.0, 0.0, 24.0, 29.0, 11.0, 16.0) };
        yield return new object[] { new ClimateExpectation("Warm Summer Dry Winter Humid Subcontinental Northern Hemisphere", "Seeded Warm Summer Dry Winter Humid Subcontinental", 15.0, 35.0, 0.0, 12.0, 28.0, 55.0, 20.0, 40.0, 0.0, 6.0, 0.0, 10.0, 3.0, 0.05, 80.0, -12.0, -6.0, 18.0, 22.0, 6.0, 12.0) };
        yield return new object[] { new ClimateExpectation("Severe Winter Subarctic Northern Hemisphere", "Seeded Severe Winter Subarctic", 10.0, 25.0, 4.0, 15.0, 15.0, 35.0, 10.0, 25.0, 1.5, 10.0, 4.0, 18.0, 2.5, 0.05, 84.0, -35.0, -25.0, 17.0, 21.0, -9.0, -1.0) };
        yield return new object[] { new ClimateExpectation("Dry Winter Subarctic Northern Hemisphere", "Seeded Dry Winter Subarctic", 10.0, 25.0, 1.0, 8.0, 18.0, 40.0, 10.0, 25.0, 1.0, 7.0, 2.0, 12.0, 2.5, 0.05, 84.0, -23.0, -15.0, 16.0, 20.0, -5.0, 1.0) };
        yield return new object[] { new ClimateExpectation("Severe Winter Dry Winter Subarctic Northern Hemisphere", "Seeded Severe Winter Dry Winter Subarctic", 8.0, 22.0, 0.0, 6.0, 15.0, 35.0, 8.0, 22.0, 0.0, 6.0, 0.0, 8.0, 2.5, 0.05, 85.0, -37.0, -26.0, 17.0, 21.0, -11.0, -2.0) };
        yield return new object[] { new ClimateExpectation("Hot Summer Dry-Summer Continental Northern Hemisphere", "Seeded Hot Summer Dry-Summer Continental", 8.0, 25.0, 18.0, 40.0, 0.0, 14.0, 5.0, 30.0, 1.0, 5.0, 3.0, 15.0, 3.0, 0.25, 80.0, -2.0, 4.0, 25.0, 30.0, 12.0, 19.0) };
        yield return new object[] { new ClimateExpectation("Warm Summer Dry-Summer Continental Northern Hemisphere", "Seeded Warm Summer Dry-Summer Continental", 8.0, 25.0, 20.0, 45.0, 0.0, 8.0, 5.0, 25.0, 2.5, 12.0, 8.0, 25.0, 3.0, 0.05, 80.0, -4.0, 1.0, 16.0, 21.0, 7.0, 12.0) };
        yield return new object[] { new ClimateExpectation("Dry-Summer Subarctic Northern Hemisphere", "Seeded Dry-Summer Subarctic", 10.0, 25.0, 8.5, 28.0, 2.0, 15.0, 8.0, 22.0, 1.5, 12.0, 5.5, 25.0, 2.5, 0.05, 82.0, -19.0, -10.0, 14.0, 18.0, -3.0, 3.0) };
        yield return new object[] { new ClimateExpectation("Severe Winter Dry-Summer Subarctic Northern Hemisphere", "Seeded Severe Winter Dry-Summer Subarctic", 8.0, 20.0, 4.0, 18.0, 1.0, 14.0, 6.0, 23.0, 1.0, 10.0, 3.0, 20.0, 2.5, 0.05, 84.0, -36.0, -25.0, 17.0, 21.0, -10.0, -2.0) };
    }

    public static IEnumerable<object[]> GetMaritimeAndTropicalClimateExpectations()
    {
        HashSet<string> names = new()
        {
            "Subpolar Oceanic Northern Hemisphere",
            "Tropical Monsoon Northern Hemisphere",
            "Tropical Savanna Dry Winter Northern Hemisphere",
            "Tropical Savanna Dry Summer Northern Hemisphere"
        };

        return GetAdditionalClimateExpectations()
            .Where(x => names.Contains(((ClimateExpectation)x[0]).RegionalClimateName));
    }

    public static IEnumerable<object[]> GetWarmTemperateClimateExpectations()
    {
        HashSet<string> names = new()
        {
            "Dry Winter Humid Subtropical Northern Hemisphere",
            "Subtropical Highland Northern Hemisphere",
            "Cold Summer Subtropical Highland Northern Hemisphere",
            "Warm-Summer Mediterranean Northern Hemisphere",
            "Cold-Summer Mediterranean Northern Hemisphere"
        };

        return GetAdditionalClimateExpectations()
            .Where(x => names.Contains(((ClimateExpectation)x[0]).RegionalClimateName));
    }

    public static IEnumerable<object[]> GetHotAndMonsoonalContinentalClimateExpectations()
    {
        HashSet<string> names = new()
        {
            "Hot Summer Humid Subcontinental Northern Hemisphere",
            "Warm Summer Dry Winter Humid Subcontinental Northern Hemisphere",
            "Hot Summer Dry-Summer Continental Northern Hemisphere",
            "Warm Summer Dry-Summer Continental Northern Hemisphere"
        };

        return GetAdditionalClimateExpectations()
            .Where(x => names.Contains(((ClimateExpectation)x[0]).RegionalClimateName));
    }

    public static IEnumerable<object[]> GetSevereContinentalClimateExpectations()
    {
        HashSet<string> names = new()
        {
            "Severe Winter Subarctic Northern Hemisphere",
            "Dry Winter Subarctic Northern Hemisphere",
            "Severe Winter Dry Winter Subarctic Northern Hemisphere",
            "Dry-Summer Subarctic Northern Hemisphere",
            "Severe Winter Dry-Summer Subarctic Northern Hemisphere"
        };

        return GetAdditionalClimateExpectations()
            .Where(x => names.Contains(((ClimateExpectation)x[0]).RegionalClimateName));
    }

    private static FuturemudDatabaseContext CreateSeededWeatherContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(nameof(WeatherSeederClimateTests), DatabaseRoot)
            .Options;
        FuturemudDatabaseContext context = new(options);

        if (_sharedDatabaseSeeded)
        {
            return context;
        }

        lock (SeededContextLock)
        {
            if (_sharedDatabaseSeeded)
            {
                return context;
            }

            SeedWeatherContext(context);
            _sharedDatabaseSeeded = true;
        }

        return context;
    }

    private static FuturemudDatabaseContext CreateFreshSeededWeatherContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase($"{nameof(WeatherSeederClimateTests)}_{Guid.NewGuid():N}")
            .Options;
        FuturemudDatabaseContext context = new(options);
        SeedWeatherContext(context);
        return context;
    }

    private static void SeedWeatherContext(FuturemudDatabaseContext context)
    {
        context.Celestials.Add(new Celestial
        {
            Definition = "<Celestial />",
            Minutes = 0,
            FeedClockId = 1,
            CelestialYear = 0,
            LastYearBump = 0,
            CelestialType = "test"
        });
        context.SaveChanges();

        WeatherSeeder seeder = new();
        string error = seeder.SeedData(context, new Dictionary<string, string>
        {
            ["rain"] = "none"
        });
        Assert.AreEqual(string.Empty, error);
    }

    private static WeatherStatisticsResult AnalyzeSeededClimate(string regionalClimateName, string controllerName)
    {
        return AnalyzeSeededClimate(regionalClimateName, controllerName, years: 12, burnInYears: 2);
    }

    private static WeatherStatisticsResult AnalyzeSeededClimate(string regionalClimateName, string controllerName, int years, int burnInYears)
    {
        using FuturemudDatabaseContext context = CreateSeededWeatherContext();

        MudSharp.Models.RegionalClimate regionalClimate = context.RegionalClimates
            .Include(x => x.RegionalClimatesSeasons)
            .ThenInclude(x => x.Season)
            .Single(x => x.Name == regionalClimateName);
        ClimateModel climateModel = context.ClimateModels
            .Include(x => x.ClimateModelSeasons)
            .ThenInclude(x => x.Season)
            .Include(x => x.ClimateModelSeasons)
            .ThenInclude(x => x.SeasonEvents)
            .Single(x => x.Id == regionalClimate.ClimateModelId);
        List<WeatherEvent> weatherEvents = context.WeatherEvents.ToList();
        Dictionary<long, WeatherEvent> eventMap = weatherEvents.ToDictionary(x => x.Id);

        List<SeasonModel> seasons = regionalClimate.RegionalClimatesSeasons
            .Select(x => x.Season)
            .OrderBy(x => x.CelestialDayOnset)
            .ThenBy(x => x.Name)
                .ToList();
        Dictionary<long, ISeason> seasonMocks = seasons.ToDictionary(
            x => x.Id,
            CreateSeasonMock);
        CircularRange<ISeason> seasonRotation = new(
            365.0,
            seasons.Select(x => (seasonMocks[x.Id], (double)x.CelestialDayOnset)));
        Dictionary<(long SeasonId, int Hour), double> baseTemperatures = regionalClimate.RegionalClimatesSeasons
            .SelectMany(x => ParseTemperatures(x.SeasonId, x.TemperatureInfo))
            .ToDictionary(x => (x.SeasonId, x.Hour), x => x.Temp);
        WeatherTransitionSnapshot snapshot = BuildSnapshot(climateModel, eventMap);
        SeederSimulationClock clock = new();
        WeatherStatisticsAnalyzer analyzer = new();
        return analyzer.AnalyzeSimulation(new WeatherStatisticsSimulationRequest
        {
            Years = years,
            BurnInYears = burnInYears,
            Seed = 123456,
            SeasonInfo = seasonMocks.ToDictionary(x => x.Key, x => x.Value),
            TransitionSnapshot = snapshot,
            SimulationContext = new WeatherSimulationContext
            {
                Geography = new GeographicCoordinate(51.5074, -0.1278, 11.0, 6_371_000.0),
                SeasonRotation = seasonRotation,
                FeedHoursPerDay = 24,
                FeedMinutesPerHour = 60,
                YearLengthDays = 365.0,
                GetCelestialDay = () => clock.CelestialDay,
                GetCurrentTimeOfDay = () => clock.GetTimeOfDay(),
                GetFeedLocalHour = () => clock.Hour,
                AdvanceOneFeedMinute = clock.AdvanceMinute,
                ConsumeYearBoundary = clock.ConsumeYearBoundary,
                GetBaseTemperature = (seasonId, hour) => baseTemperatures[(seasonId, hour)]
            },
            InitialSeasonId = seasonMocks.Values.First().Id,
            InitialWeatherEventId = null,
            InitialConsecutiveUnchangedPeriods = 0,
            InitialMinuteCounter = 0
        }, controllerName: controllerName);
    }

    private static void AssertBetween(double value, double minimum, double maximum, string label)
    {
        Assert.IsTrue(value >= minimum && value <= maximum, $"Expected {label} to stay between {minimum:F2} and {maximum:F2}, but got {value:F2}.");
    }

    private static void CheckBetween(double value, double minimum, double maximum, string label, ICollection<string> failures)
    {
        if (value < minimum || value > maximum)
        {
            failures.Add($"Expected {label} to stay between {minimum:F2} and {maximum:F2}, but got {value:F2}.");
        }
    }

    private static void CheckMaximum(double value, double maximum, string label, ICollection<string> failures)
    {
        if (value > maximum)
        {
            failures.Add($"Expected {label} to stay at or below {maximum:F2}, but got {value:F2}.");
        }
    }

    private static void CheckMinimum(double value, double minimum, string label, ICollection<string> failures)
    {
        if (value < minimum)
        {
            failures.Add($"Expected {label} to stay at or above {minimum:F2}, but got {value:F2}.");
        }
    }

    public sealed record ClimateExpectation(
        string RegionalClimateName,
        string ControllerName,
        double AnnualWetMin,
        double AnnualWetMax,
        double WinterWetMin,
        double WinterWetMax,
        double SummerWetMin,
        double SummerWetMax,
        double AutumnWetMin,
        double AutumnWetMax,
        double AnnualSnowMin,
        double AnnualSnowMax,
        double WinterSnowMin,
        double WinterSnowMax,
        double AnnualGaleOrWorseMax,
        double AnnualHurricaneOrWorseMax,
        double AnnualBreezeOrCalmerMin,
        double MidWinterMin,
        double MidWinterMax,
        double MidSummerMin,
        double MidSummerMax,
        double MidAutumnMin,
        double MidAutumnMax);

    private static WeatherTransitionSnapshot BuildSnapshot(ClimateModel climateModel, IReadOnlyDictionary<long, WeatherEvent> eventMap)
    {
        Dictionary<long, WeatherEventInfo> eventInfo = eventMap.Values.ToDictionary(
            x => x.Id,
            x => new WeatherEventInfo
            {
                Id = x.Id,
                Name = x.Name,
                Precipitation = (PrecipitationLevel)x.Precipitation,
                Wind = (WindLevel)x.Wind,
                TemperatureEffect = x.TemperatureEffect,
                PrecipitationTemperatureEffect = x.PrecipitationTemperatureEffect,
                WindTemperatureEffect = x.WindTemperatureEffect,
                PermittedTimesOfDay = GetPermittedTimes(x)
            });
        Dictionary<long, double> maxAdditional = new();
        Dictionary<long, double> incremental = new();
        Dictionary<(long SeasonId, long EventId), double> baseChangeChance = new();
        Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>> transitions = new();

        foreach (ClimateModelSeason season in climateModel.ClimateModelSeasons)
        {
            maxAdditional[season.SeasonId] = season.MaximumAdditionalChangeChanceFromStableWeather;
            incremental[season.SeasonId] = season.IncrementalAdditionalChangeChanceFromStableWeather;

            foreach (ClimateModelSeasonEvent seasonEvent in season.SeasonEvents)
            {
                baseChangeChance[(season.SeasonId, seasonEvent.WeatherEventId)] = seasonEvent.ChangeChance;
                transitions[(season.SeasonId, seasonEvent.WeatherEventId)] = WeatherStatisticsAnalyzer.ParseTransitions(seasonEvent.Transitions)
                    .Where(x => x.EventId != seasonEvent.WeatherEventId)
                    .ToList();
            }
        }

        return new WeatherTransitionSnapshot(
            climateModel.MinuteProcessingInterval,
            eventInfo,
            maxAdditional,
            incremental,
            baseChangeChance,
            transitions);
    }

    private static HashSet<TimeOfDay> GetPermittedTimes(WeatherEvent weatherEvent)
    {
        HashSet<TimeOfDay> permitted = new();
        if (weatherEvent.PermittedAtNight)
        {
            permitted.Add(TimeOfDay.Night);
        }

        if (weatherEvent.PermittedAtDawn)
        {
            permitted.Add(TimeOfDay.Dawn);
        }

        if (weatherEvent.PermittedAtMorning)
        {
            permitted.Add(TimeOfDay.Morning);
        }

        if (weatherEvent.PermittedAtAfternoon)
        {
            permitted.Add(TimeOfDay.Afternoon);
        }

        if (weatherEvent.PermittedAtDusk)
        {
            permitted.Add(TimeOfDay.Dusk);
        }

        return permitted;
    }

    private static IEnumerable<(long SeasonId, int Hour, double Temp)> ParseTemperatures(long seasonId, string xml)
    {
        XElement root = XElement.Parse(xml);
        return root.Elements("Value")
            .Select(x => (
                SeasonId: seasonId,
                Hour: int.Parse(x.Attribute("hour")!.Value),
                Temp: double.Parse(x.Value)));
    }

    private static ISeason CreateSeasonMock(SeasonModel season)
    {
        Mock<ISeason> mock = new();
        mock.SetupGet(x => x.Id).Returns(season.Id);
        mock.SetupGet(x => x.Name).Returns(season.Name);
        mock.SetupGet(x => x.DisplayName).Returns(season.DisplayName);
        mock.SetupGet(x => x.SeasonGroup).Returns(season.SeasonGroup);
        mock.SetupGet(x => x.CelestialDayOnset).Returns(season.CelestialDayOnset);
        return mock.Object;
    }

    private static double GetPercent(IEnumerable<WeatherStatisticsCsvRow> rows, string season, string metricType, string key)
    {
        return rows.Single(x =>
            x.Season == season &&
            x.MetricType == metricType &&
            x.Key == key &&
            x.Statistic == "Percent").Value;
    }

    private static double GetPercentOrZero(IEnumerable<WeatherStatisticsCsvRow> rows, string season, string metricType, string key)
    {
        return rows
            .Where(x =>
                x.Season == season &&
                x.MetricType == metricType &&
                x.Key == key &&
                x.Statistic == "Percent")
            .Select(x => x.Value)
            .DefaultIfEmpty(0.0)
            .Single();
    }

    private static double AverageHourlyMean(IEnumerable<WeatherStatisticsCsvRow> rows, string season, string key)
    {
        return rows
            .Where(x =>
                x.Season == season &&
                x.MetricType == "TemperatureHourly" &&
                x.Key == key &&
                x.Statistic == "Mean")
            .Average(x => x.Value);
    }

    private static double WeightedAveragePercent(IEnumerable<WeatherStatisticsCsvRow> rows, string metricType, string key)
    {
        return WeightedAveragePercent(rows, metricType, new[] { key });
    }

    private static double WeightedAveragePercent(IEnumerable<WeatherStatisticsCsvRow> rows, string metricType, IEnumerable<string> keys)
    {
        HashSet<string> keySet = keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var seasonRows = rows
            .Where(x => x.MetricType == metricType && x.Statistic == "Percent" && keySet.Contains(x.Key))
            .GroupBy(x => x.Season)
            .Select(x => new
            {
                Percent = x.Sum(y => y.Value),
                SeasonMinutes = x.First().SeasonMinutes
            })
            .ToList();
        long totalMinutes = seasonRows.Sum(x => x.SeasonMinutes);
        return totalMinutes == 0
            ? 0.0
            : seasonRows.Sum(x => x.Percent * x.SeasonMinutes) / totalMinutes;
    }

    private sealed class SeederSimulationClock
    {
        private int _minute;
        private int _day;
        private bool _yearBoundary;

        public int Hour { get; private set; }

        public double CelestialDay => _day + ((Hour * 60 + _minute) / 1_440.0);

        public void AdvanceMinute()
        {
            _yearBoundary = false;
            if (++_minute < 60)
            {
                return;
            }

            _minute = 0;
            if (++Hour < 24)
            {
                return;
            }

            Hour = 0;
            if (++_day < 365)
            {
                return;
            }

            _day = 0;
            _yearBoundary = true;
        }

        public bool ConsumeYearBoundary()
        {
            bool value = _yearBoundary;
            _yearBoundary = false;
            return value;
        }

        public TimeOfDay GetTimeOfDay()
        {
            return Hour switch
            {
                < 5 => TimeOfDay.Night,
                5 => TimeOfDay.Dawn,
                < 12 => TimeOfDay.Morning,
                < 18 => TimeOfDay.Afternoon,
                18 => TimeOfDay.Dusk,
                _ => TimeOfDay.Night
            };
        }
    }
}
