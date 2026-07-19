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

public partial class WeatherSeeder
{
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
