#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private void SeedKnowledges()
	{
		foreach (var (name, description, longDescription, sessions, difficulty) in RobotKnowledgeTemplates)
		{
			if (_context.Knowledges.Any(x => x.Name == name))
			{
				continue;
			}

			var knowledge = new MudSharp.Models.Knowledge
			{
				Name = name,
				Type = "Engineering",
				Subtype = "Robotics",
				Description = description,
				LongDescription = longDescription,
				LearnableType = (int)(MudSharp.RPG.Knowledge.LearnableType.LearnableFromTeacher |
				                      MudSharp.RPG.Knowledge.LearnableType.LearnableAtChargen),
				LearnDifficulty = (int)difficulty,
				TeachDifficulty = (int)difficulty,
				LearningSessionsRequired = sessions,
				CanAcquireProg = _alwaysTrue,
				CanLearnProg = _alwaysTrue
			};
			_context.Knowledges.Add(knowledge);
		}

		_context.SaveChanges();
		foreach (var knowledge in _context.Knowledges
			         .Where(x => RobotKnowledgeTemplates.Select(y => y.Name).Contains(x.Name))
			         .ToList())
		{
			_knowledges[knowledge.Name] = knowledge;
		}
	}

	private void SeedRaces(IReadOnlyDictionary<string, BodyProto> bodyCatalogue, RobotSeedSummary summary)
	{
		foreach (var template in Templates.Values)
		{
			if (!bodyCatalogue.TryGetValue(template.BodyKey, out var body))
			{
				continue;
			}

			var race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null)
			{
				race = new Race
				{
					Name = template.Name,
					Description = template.Description,
					BaseBody = body,
					AllowedGenders = template.UsesHumanGenders
						? _humanRace.AllowedGenders
						: $"{(short)Gender.Neuter}",
					ParentRace = template.ParentRaceName is null
						? null
						: _context.Races.FirstOrDefault(x => x.Name == template.ParentRaceName),
					AttributeBonusProg = _humanRace.AttributeBonusProg,
					AttributeTotalCap = _humanRace.AttributeTotalCap,
					IndividualAttributeCap = _humanRace.IndividualAttributeCap,
					DiceExpression = _humanRace.DiceExpression,
					IlluminationPerceptionMultiplier = 1.0,
					AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
					CorpseModel = template.UsesHumanoidCharacteristics ? _robotHumanoidCorpse : _robotAnimalCorpse,
					DefaultHealthStrategy = _context.HealthStrategies.First(x => x.Name == template.HealthStrategyName),
					CanUseWeapons = template.CanUseWeapons,
					CanAttack = true,
					CanDefend = true,
					NaturalArmourType = template.Size <= SizeCategory.Small ? _robotLightPlatingArmour : _robotPlatingArmour,
					NaturalArmourMaterial = _chassisAlloy,
					NaturalArmourQuality = 4,
					BloodLiquid = _context.Liquids.First(x => x.Name == template.BloodLiquidName),
					NeedsToBreathe = false,
					SweatLiquid = null,
					SweatRateInLitresPerMinute = 0.0,
					SizeStanding = (int)template.Size,
					SizeProne = (int)template.Size,
					SizeSitting = (int)template.Size,
					CommunicationStrategyType = "robot",
					DefaultHandedness = _humanRace.DefaultHandedness,
					HandednessOptions = template.CanUseWeapons ? _humanRace.HandednessOptions : "1 3",
					MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*50000",
					MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*15000",
					BloodModel = _humanRace.BloodModel,
					RaceUsesStamina = true,
					CanEatCorpses = false,
					EatCorpseEmoteText = string.Empty,
					CanEatMaterialsOptIn = false,
					BiteWeight = 1000,
					TemperatureRangeFloor = -50,
					TemperatureRangeCeiling = 120,
					BodypartSizeModifier = 0,
					BodypartHealthMultiplier = 1.15,
					BreathingModel = "simple",
					BreathingVolumeExpression = "0",
					HoldBreathLengthExpression = "999999",
					CanClimb = template.CanClimb,
					CanSwim = template.CanSwim,
					MinimumSleepingPosition = _humanRace.MinimumSleepingPosition,
					ChildAge = _humanRace.ChildAge,
					YouthAge = _humanRace.YouthAge,
					YoungAdultAge = _humanRace.YoungAdultAge,
					AdultAge = _humanRace.AdultAge,
					ElderAge = _humanRace.ElderAge,
					VenerableAge = _humanRace.VenerableAge,
					DefaultHeightWeightModelMale = _humanRace.DefaultHeightWeightModelMale,
					DefaultHeightWeightModelFemale = _humanRace.DefaultHeightWeightModelFemale,
					DefaultHeightWeightModelNeuter = _humanRace.DefaultHeightWeightModelNeuter ?? _humanRace.DefaultHeightWeightModelMale,
					DefaultHeightWeightModelNonBinary = _humanRace.DefaultHeightWeightModelNonBinary ?? _humanRace.DefaultHeightWeightModelFemale,
					HungerRate = 0.0,
					ThirstRate = 0.0,
					TrackIntensityVisual = _humanRace.TrackIntensityVisual,
					TrackIntensityOlfactory = 0.0,
					TrackingAbilityVisual = _humanRace.TrackingAbilityVisual,
					TrackingAbilityOlfactory = 0.0
				};
				_context.Races.Add(race);
				_context.SaveChanges();
				summary.RacesAdded++;
			}

			CopyRaceAttributes(race);
			if (template.UsesHumanoidCharacteristics)
			{
				CopyHumanAdditionalCharacteristics(race);
				SeedHumanoidEthnicity(race, template);
			}
			else
			{
				SeedSimpleEthnicity(race, template);
			}

			SeedDefaultDescriptions(race, template);
			SeedRacialBodypartUsages(race, template);
			SeedNaturalAttacks(race, template);
		}
	}

	private void CopyRaceAttributes(Race race)
	{
		var humanHealthAttributes = _context.RacesAttributes
			.Where(x => x.RaceId == _humanRace.Id)
			.ToDictionary(x => x.AttributeId, x => x.IsHealthAttribute);
		foreach (var attribute in _context.TraitDefinitions
			         .Where(x => x.Type == (int)TraitType.Attribute)
			         .ToList())
		{
			if (_context.RacesAttributes.Any(x => x.RaceId == race.Id && x.AttributeId == attribute.Id))
			{
				continue;
			}

			_context.RacesAttributes.Add(new RacesAttributes
			{
				Race = race,
				Attribute = attribute,
				IsHealthAttribute = humanHealthAttributes.GetValueOrDefault(attribute.Id)
			});
		}

		_context.SaveChanges();
	}

	private void CopyHumanAdditionalCharacteristics(Race race)
	{
		foreach (var item in _humanRace.RacesAdditionalCharacteristics.ToList())
		{
			if (_context.RacesAdditionalCharacteristics.Any(x =>
				    x.RaceId == race.Id && x.CharacteristicDefinitionId == item.CharacteristicDefinitionId))
			{
				continue;
			}

			_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{
				Race = race,
				CharacteristicDefinition = item.CharacteristicDefinition,
				Usage = item.Usage
			});
		}

		_context.SaveChanges();
	}

	private void SeedHumanoidEthnicity(Race race, RobotRaceTemplate template)
	{
		if (_context.Ethnicities.Any(x => x.ParentRaceId == race.Id && x.Name == $"{template.Name} Stock"))
		{
			return;
		}

		var ethnicity = new Ethnicity
		{
			Name = $"{template.Name} Stock",
			ChargenBlurb = template.Description,
			AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
			ParentRace = race,
			EthnicGroup = template.Name,
			EthnicSubgroup = "Stock",
			PopulationBloodModel = _humanRace.Ethnicities.First().PopulationBloodModel,
			TolerableTemperatureFloorEffect = 0,
			TolerableTemperatureCeilingEffect = 0
		};
		_context.Ethnicities.Add(ethnicity);
		_context.SaveChanges();
		ApplyRobotNameCultures(ethnicity);

		AddEthnicityCharacteristic(ethnicity, "Eye Colour", "All Eye Colours");
		AddEthnicityCharacteristic(ethnicity, "Eye Shape", "All Eye Shapes");
		AddEthnicityCharacteristic(ethnicity, "Nose", "All Noses");
		AddEthnicityCharacteristic(ethnicity, "Ears", "All Ears");
		AddEthnicityCharacteristic(ethnicity, "Hair Colour", "All Hair Colours");
		AddEthnicityCharacteristic(ethnicity, "Facial Hair Colour", "All Facial Hair Colours");
		AddEthnicityCharacteristic(ethnicity, "Hair Style", "All Hair Styles");
		AddEthnicityCharacteristic(ethnicity, "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityCharacteristic(ethnicity, "Skin Colour", "All Skin Colours");
		AddEthnicityCharacteristic(ethnicity, "Frame", "All Frames");
		AddEthnicityCharacteristic(ethnicity, "Person Word", "Person Word");
	}
}
