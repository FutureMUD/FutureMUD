using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

readonly record struct WeatherSeederEventDescriptor(
	PrecipitationLevel Precipitation,
	WindLevel Wind,
	WeatherEventVariation TemperatureVariation,
	WindVariation WindVariation,
	CloudVariation CloudVariation);

public partial class WeatherSeeder
{
	private static WeatherSeederClimateProfile CreateTemperateOceanicProfile()
	{
		return new WeatherSeederClimateProfile
		{
			ClimateModelName = "Temperate",
			RegionalClimatePrefix = "Oceanic Temperate",
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

	private static WeatherSeederClimateProfile CreateMediterraneanProfile()
	{
		return new WeatherSeederClimateProfile
		{
			ClimateModelName = "Mediterranean",
			RegionalClimatePrefix = "Mediterranean",
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

	private static ClimateModel CreateClimateModel(FuturemudDatabaseContext context, List<Season> seasons, Dictionary<string, WeatherEvent> events, WeatherSeederClimateProfile profile)
	{
		return profile.RegionalClimatePrefix switch
		{
			"Oceanic Temperate" => CreateClimateModels(context, seasons, events),
			"Humid Subtropical" => CreateHumidSubtropicalClimateModel(context, profile),
			"Mediterranean" => CreateMediterraneanClimateModel(context, profile),
			_ => throw new ArgumentOutOfRangeException(nameof(profile), profile.RegionalClimatePrefix, "Unsupported seeded climate profile.")
		};
	}

	private static ClimateModel CreateHumidSubtropicalClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
	{
		return CreateDerivedClimateModel(context, profile, AdjustHumidSubtropicalTransitionChance);
	}

	private static ClimateModel CreateMediterraneanClimateModel(FuturemudDatabaseContext context, WeatherSeederClimateProfile profile)
	{
		return CreateDerivedClimateModel(context, profile, AdjustMediterraneanTransitionChance);
	}

	private static ClimateModel CreateDerivedClimateModel(
		FuturemudDatabaseContext context,
		WeatherSeederClimateProfile profile,
		Func<string, WeatherSeederEventDescriptor, WeatherSeederEventDescriptor, double, WeatherSeederClimateProfile, WeatherSeederClimateProfile, double> transitionAdjuster)
	{
		var oceanicProfile = CreateTemperateOceanicProfile();
		var oceanicModel = context.ClimateModels
			.Include(x => x.ClimateModelSeasons)
			.ThenInclude(x => x.Season)
			.Include(x => x.ClimateModelSeasons)
			.ThenInclude(x => x.SeasonEvents)
			.ThenInclude(x => x.WeatherEvent)
			.Single(x => x.Name == oceanicProfile.ClimateModelName);

		var climateModel = new ClimateModel
		{
			Name = profile.ClimateModelName,
			MinuteProcessingInterval = oceanicModel.MinuteProcessingInterval,
			MinimumMinutesBetweenFlavourEchoes = oceanicModel.MinimumMinutesBetweenFlavourEchoes,
			MinuteFlavourEchoChance = oceanicModel.MinuteFlavourEchoChance,
			Type = oceanicModel.Type
		};

		foreach (var baseSeason in oceanicModel.ClimateModelSeasons)
		{
			var cms = new ClimateModelSeason
			{
				ClimateModel = climateModel,
				Season = baseSeason.Season,
				IncrementalAdditionalChangeChanceFromStableWeather = profile.IncrementalAdditionalChangeChanceFromStableWeather,
				MaximumAdditionalChangeChanceFromStableWeather = profile.MaximumAdditionalChangeChance(baseSeason.Season.SeasonGroup)
			};
			climateModel.ClimateModelSeasons.Add(cms);

			foreach (var baseSeasonEvent in baseSeason.SeasonEvents)
			{
				var descriptor = DescribeEvent(baseSeasonEvent.WeatherEvent);
				var transitionElements = XElement.Parse(baseSeasonEvent.Transitions).Elements("Transition").ToList();
				cms.SeasonEvents.Add(new ClimateModelSeasonEvent
				{
					ClimateModel = climateModel,
					Season = baseSeason.Season,
					WeatherEvent = baseSeasonEvent.WeatherEvent,
					ChangeChance = profile.BaseChangeChance,
					Transitions = new XElement("Transitions",
						from transition in transitionElements
						let targetId = long.Parse(transition.Attribute("id")!.Value)
						let target = context.WeatherEvents.Find(targetId)
						where target is not null
						let adjustedChance = transitionAdjuster(
							baseSeason.Season.SeasonGroup,
							descriptor,
							DescribeEvent(target),
							double.Parse(transition.Attribute("chance")!.Value),
							oceanicProfile,
							profile)
						where adjustedChance > 0.0
						select new XElement("Transition",
							new XAttribute("id", targetId),
							new XAttribute("chance", adjustedChance))
					).ToString()
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

	private static double ApplyHumidSubtropicalFallback(
		string seasonName,
		WeatherSeederEventDescriptor fromEvent,
		WeatherSeederEventDescriptor toEvent,
		double baseChance)
	{
		var adjusted = baseChance;
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
		var adjusted = baseChance;
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

	private static WeatherSeederEventDescriptor DescribeEvent(WeatherEvent weatherEvent)
	{
		var match = TempVariationRegex.Match(weatherEvent.Name);
		var variation = match.Success
			? Enum.Parse<WeatherEventVariation>(match.Value)
			: WeatherEventVariation.Normal;
		var windVariation = weatherEvent.Name.Contains("Polar", StringComparison.Ordinal)
			? WindVariation.Polar
			: weatherEvent.Name.Contains("Equatorial", StringComparison.Ordinal)
				? WindVariation.Equatorial
				: WindVariation.Normal;
		var cloudVariation = CloudVariationRegex.IsMatch(weatherEvent.Name)
			? weatherEvent.Name.StartsWith("Overcast", StringComparison.Ordinal)
				? CloudVariation.Overcast
				: CloudVariation.Cloudy
			: CloudVariation.None;
		return new WeatherSeederEventDescriptor((PrecipitationLevel)weatherEvent.Precipitation, (WindLevel)weatherEvent.Wind, variation, windVariation, cloudVariation);
	}
}
