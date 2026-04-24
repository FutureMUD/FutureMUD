#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
    private void SeedKnowledges()
    {
        foreach ((string? name, string? description, string? longDescription, int sessions, MudSharp.RPG.Checks.Difficulty difficulty) in RobotKnowledgeTemplates)
        {
            if (_context.Knowledges.Any(x => x.Name == name))
            {
                continue;
            }

            Knowledge knowledge = new()
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
        foreach (Knowledge? knowledge in _context.Knowledges
                     .Where(x => RobotKnowledgeTemplates.Select(y => y.Name).Contains(x.Name))
                     .ToList())
        {
            _knowledges[knowledge.Name] = knowledge;
        }
    }

    private void SeedRaces(IReadOnlyDictionary<string, BodyProto> bodyCatalogue, RobotSeedSummary summary)
    {
        string CombatStrategyFor(RobotRaceTemplate template)
        {
            return template.CanUseWeapons ? "Melee (Auto)" : template.CombatStrategyKey;
        }

        foreach (RobotRaceTemplate template in Templates.Values)
        {
            if (!bodyCatalogue.TryGetValue(template.BodyKey, out BodyProto? body))
            {
                continue;
            }

            Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
            if (race is null)
            {
                race = new Race
                {
                    Name = template.Name,
                    Description = BuildRaceDescriptionForTesting(template),
                    BaseBody = body,
                    AllowedGenders = template.UsesHumanGenders
                        ? _humanRace.AllowedGenders
                        : $"{(short)Gender.Neuter}",
                    ParentRace = template.ParentRaceName is null
                        ? null
                        : _context.Races.FirstOrDefault(x => x.Name == template.ParentRaceName),
                    AttributeTotalCap = _humanRace.AttributeTotalCap,
                    IndividualAttributeCap = _humanRace.IndividualAttributeCap,
                    DiceExpression = _humanRace.DiceExpression,
                    IlluminationPerceptionMultiplier = 1.0,
                    AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
                    CorpseModel = template.UsesHumanoidCharacteristics ? _robotHumanoidCorpse : _robotAnimalCorpse,
                    DefaultHealthStrategy = _context.HealthStrategies.First(x => x.Name == template.HealthStrategyName),
                    DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, CombatStrategyFor(template)),
                    CanUseWeapons = template.CanUseWeapons,
                    CanAttack = true,
                    CanDefend = true,
					NaturalArmourType = _robotFrameArmour,
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
                    BodypartHealthMultiplier = ResolveRobotHealthMultiplier(template.Size, 1.15),
                    BreathingModel = NonBreatherBreathingModel,
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
            else
            {
                CharacterCombatSetting defaultCombatSetting =
                    CombatStrategySeederHelper.EnsureCombatStrategy(_context, CombatStrategyFor(template));
                if (race.DefaultCombatSettingId != defaultCombatSetting.Id)
                {
                    race.DefaultCombatSetting = defaultCombatSetting;
                }

                if (race.NaturalArmourTypeId != _robotFrameArmour.Id)
                {
                    race.NaturalArmourType = _robotFrameArmour;
                }

                race.NaturalArmourMaterial = _chassisAlloy;
                race.NaturalArmourQuality = 4;
                race.BodypartHealthMultiplier = ResolveRobotHealthMultiplier(template.Size, 1.15);
            }

            ApplyNonBreatherSettings(race);

            CopyRaceAttributes(race, template);
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

    private void ApplyNonBreatherSettings(Race race)
    {
        race.NeedsToBreathe = false;
        race.BreathingModel = NonBreatherBreathingModel;
        race.BreathingVolumeExpression = "0";
        race.HoldBreathLengthExpression = "999999";

        _context.RacesBreathableGases.RemoveRange(_context.RacesBreathableGases.Where(x => x.RaceId == race.Id).ToList());
        _context.RacesBreathableLiquids.RemoveRange(_context.RacesBreathableLiquids.Where(x => x.RaceId == race.Id).ToList());
    }

    private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> RobotSpecificProfiles =
        new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
        {
            ["Spider Crawler Robot"] = new(0, 1, 2, 0),
            ["Circular Saw Robot"] = new(2, 1, 0, 1),
            ["Pneumatic Hammer Robot"] = new(4, 3, -1, -2),
            ["Sword-Hand Robot"] = new(1, 0, 2, 2),
            ["Winged Robot"] = new(-1, -1, 3, 1),
            ["Jet Robot"] = new(0, -1, 4, 0),
            ["Mandible Robot"] = new(2, 1, 0, -1),
            ["Wheeled Robot"] = new(-1, 0, 2, 0),
            ["Tracked Robot"] = new(2, 3, -2, -2),
            ["Cyborg"] = new(-1, -1, 1, 1),
            ["Roomba Robot"] = new(-3, 1, 1, 0),
            ["Tracked Utility Robot"] = new(0, 3, -1, -1),
            ["Robot Dog"] = new(1, 1, 2, 0),
            ["Robot Cockroach"] = new(-3, 3, 3, 1)
        };

    internal static NonHumanAttributeProfile GetRobotAttributeProfileForTesting(RobotRaceTemplate template)
    {
        return GetRobotAttributeProfile(template);
    }

    private static NonHumanAttributeProfile GetRobotAttributeProfile(RobotRaceTemplate template)
    {
        NonHumanAttributeProfile profile = template.Size switch
        {
            SizeCategory.Nanoscopic => new(-10, -8, 4, 3),
            SizeCategory.Microscopic => new(-9, -7, 4, 3),
            SizeCategory.Miniscule => new(-8, -6, 3, 2),
            SizeCategory.Tiny => new(-6, -4, 2, 1),
            SizeCategory.VerySmall => new(-4, -2, 1, 1),
            SizeCategory.Small => new(-1, 0, 1, 0),
            SizeCategory.Normal => new(2, 2, 0, 0),
            SizeCategory.Large => new(5, 5, -1, -1),
            SizeCategory.VeryLarge => new(8, 8, -2, -2),
            SizeCategory.Huge => new(12, 10, -3, -3),
            SizeCategory.Enormous => new(15, 12, -4, -4),
            SizeCategory.Gigantic => new(18, 14, -5, -5),
            SizeCategory.Titanic => new(20, 16, -6, -6),
            _ => new(2, 2, 0, 0)
        };

        if (RobotSpecificProfiles.TryGetValue(template.Name, out var specificProfile))
        {
            profile = profile.Add(specificProfile);
        }

        return profile.Clamp(-10, 20);
    }

    private void CopyRaceAttributes(Race race, RobotRaceTemplate template)
    {
        Dictionary<long, RacesAttributes> humanAttributes = _context.RacesAttributes
            .Where(x => x.RaceId == _humanRace.Id)
            .ToDictionary(x => x.AttributeId);
        var profile = GetRobotAttributeProfile(template);
        foreach (TraitDefinition? attribute in _context.TraitDefinitions
                     .Where(x => x.Type == (int)TraitType.Attribute || x.Type == (int)TraitType.DerivedAttribute)
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
                IsHealthAttribute = humanAttributes.TryGetValue(attribute.Id, out RacesAttributes? humanAttribute) &&
                                    humanAttribute.IsHealthAttribute,
                AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile),
                DiceExpression = humanAttribute?.DiceExpression
            });
        }

        _context.SaveChanges();
    }

    private void CopyHumanAdditionalCharacteristics(Race race)
    {
        foreach (RacesAdditionalCharacteristics? item in _humanRace.RacesAdditionalCharacteristics.ToList())
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

        Ethnicity ethnicity = new()
        {
            Name = $"{template.Name} Stock",
            ChargenBlurb = BuildEthnicityDescriptionForTesting(template),
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
