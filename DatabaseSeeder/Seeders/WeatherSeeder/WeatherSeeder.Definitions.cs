#nullable enable

using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;

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

readonly record struct WeatherSeederEventDescriptor(
    PrecipitationLevel Precipitation,
    WindLevel Wind,
    WeatherEventVariation TemperatureVariation,
    WindVariation WindVariation,
    CloudVariation CloudVariation);
