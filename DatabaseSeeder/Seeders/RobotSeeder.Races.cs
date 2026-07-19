#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System;

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

    private const string RobotAuraDiceExpression = "0";

    private static readonly NonHumanAttributeProfile RobotMentalBaseline =
        new(0, 0, 0, 0,
            WillpowerBonus: 8,
            PerceptionBonus: 2,
            AuraDiceExpression: RobotAuraDiceExpression);

    private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> RobotSpecificProfiles =
        new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
        {
            ["Spider Crawler Robot"] = new(0, 1, 2, 0, WillpowerBonus: 1, PerceptionBonus: 2),
            ["Circular Saw Robot"] = new(2, 1, 0, 1, WillpowerBonus: 1, PerceptionBonus: 1),
            ["Pneumatic Hammer Robot"] = new(4, 3, -1, -2, WillpowerBonus: 2),
            ["Sword-Hand Robot"] = new(1, 0, 2, 2, WillpowerBonus: 1, PerceptionBonus: 1),
            ["Winged Robot"] = new(-1, -1, 3, 1, PerceptionBonus: 2),
            ["Jet Robot"] = new(0, -1, 4, 0, PerceptionBonus: 2),
            ["Mandible Robot"] = new(2, 1, 0, -1, WillpowerBonus: 1, PerceptionBonus: 1),
            ["Wheeled Robot"] = new(-1, 0, 2, 0, PerceptionBonus: 1),
            ["Tracked Robot"] = new(2, 3, -2, -2, WillpowerBonus: 2),
            ["Cyborg"] = new(-1, -1, 1, 1, PerceptionBonus: 1),
            ["Roomba Robot"] = new(-3, 1, 1, 0, PerceptionBonus: -1),
            ["Tracked Utility Robot"] = new(0, 3, -1, -1, WillpowerBonus: 1),
            ["Robot Dog"] = new(1, 1, 2, 0, WillpowerBonus: 1, PerceptionBonus: 2),
            ["Robot Cockroach"] = new(-3, 3, 3, 1, WillpowerBonus: 1, PerceptionBonus: 3)
        };

    internal static NonHumanAttributeProfile GetRobotAttributeProfileForTesting(RobotRaceTemplate template)
    {
        return GetRobotAttributeProfile(template);
    }

    private static NonHumanAttributeProfile GetRobotAttributeProfile(RobotRaceTemplate template)
    {
        NonHumanAttributeProfile profile = RobotMentalBaseline.Add(template.Size switch
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
        });

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
            humanAttributes.TryGetValue(attribute.Id, out RacesAttributes? humanAttribute);
            var attributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile);
            var diceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, profile) ??
                                 humanAttribute?.DiceExpression;
            RacesAttributes? alteration = _context.RacesAttributes
                .FirstOrDefault(x => x.RaceId == race.Id && x.AttributeId == attribute.Id);
            if (alteration is null)
            {
                _context.RacesAttributes.Add(new RacesAttributes
                {
                    Race = race,
                    Attribute = attribute,
                    IsHealthAttribute = humanAttribute?.IsHealthAttribute ?? false,
                    AttributeBonus = attributeBonus,
                    DiceExpression = diceExpression
                });
                continue;
            }

            alteration.IsHealthAttribute = humanAttribute?.IsHealthAttribute ?? false;
            alteration.AttributeBonus = attributeBonus;
            alteration.DiceExpression = diceExpression;
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

private void EnsureOrganicHumanoidDescriptionProgScope()
    {
        bool dirty = false;
        foreach (FutureProg? prog in _context.FutureProgs
                     .Where(x => x.Subcategory == "Descriptions" && x.FunctionName.StartsWith("IsHumanoid"))
                     .ToList())
        {
            string updatedText = HumanSeeder.UpdateHumanoidDescriptionProgScope(prog.FunctionText);
            if (updatedText == prog.FunctionText)
            {
                continue;
            }

            prog.FunctionText = updatedText;
            dirty = true;
        }

        if (dirty)
        {
            _context.SaveChanges();
        }
    }

    private void SeedSimpleEthnicity(Race race, RobotRaceTemplate template)
    {
        if (_context.Ethnicities.Any(x => x.ParentRaceId == race.Id && x.Name == $"{template.Name} Stock"))
        {
            return;
        }

        _context.Ethnicities.Add(new Ethnicity
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
        });
        _context.SaveChanges();
        ApplyRobotNameCultures(_context.Ethnicities.First(x =>
            x.ParentRaceId == race.Id &&
            x.Name == $"{template.Name} Stock"));
    }

    private void AddEthnicityCharacteristic(Ethnicity ethnicity, string definitionName, string profileName)
    {
        CharacteristicDefinition? definition = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == definitionName);
        CharacteristicProfile? profile = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
        if (definition is null || profile is null)
        {
            return;
        }

        if (_context.EthnicitiesCharacteristics.Any(x =>
                x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
        {
            return;
        }

        _context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
        {
            Ethnicity = ethnicity,
            CharacteristicDefinition = definition,
            CharacteristicProfile = profile
        });
        _context.SaveChanges();
    }

    private void SeedRacialBodypartUsages(Race race, RobotRaceTemplate template)
    {
        if (template.BodypartUsages is null)
        {
            return;
        }

        foreach (RobotBodypartUsageTemplate usage in template.BodypartUsages)
        {
            if (FindBodypartOnBody(race.BaseBody, usage.BodypartAlias) is not { } bodypart)
            {
                continue;
            }

            if (_context.RacesAdditionalBodyparts.Any(x => x.RaceId == race.Id && x.BodypartId == bodypart.Id &&
                                                          x.Usage == usage.Usage))
            {
                continue;
            }

            _context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
            {
                Race = race,
                Bodypart = bodypart,
                Usage = usage.Usage
            });
        }

        _context.SaveChanges();
    }

    private void SeedNaturalAttacks(Race race, RobotRaceTemplate template)
    {
        foreach (RobotAttackBindingTemplate attackTemplate in template.Attacks)
        {
            WeaponAttack attack = _context.WeaponAttacks.First(x => x.Name == attackTemplate.AttackName);
            foreach (string alias in attackTemplate.BodypartAliases)
            {
                if (FindBodypartOnBody(race.BaseBody, alias) is not { } bodypart)
                {
                    continue;
                }

                if (_context.RacesWeaponAttacks.Any(x =>
                        x.RaceId == race.Id && x.WeaponAttackId == attack.Id && x.BodypartId == bodypart.Id))
                {
                    continue;
                }

                _context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
                {
                    Race = race,
                    Bodypart = bodypart,
                    WeaponAttack = attack,
                    Quality = (int)ItemQuality.Standard
                });
            }
        }

        _context.SaveChanges();
    }

    private void SeedDefaultDescriptions(Race race, RobotRaceTemplate template)
    {
        IReadOnlyList<StockDescriptionVariant>? variants = template.UsesHumanoidCharacteristics
            ? template.OverlayDescriptionVariants
            : template.DescriptionVariants;
        if (variants == null || variants.Count == 0)
        {
            return;
        }

        FutureProg prog = EnsureRaceDescriptionApplicabilityProg(race);
        EnsureEntityDescriptionPatterns(prog, variants, !template.UsesHumanoidCharacteristics);
        _context.SaveChanges();
    }

    private FutureProg EnsureRaceDescriptionApplicabilityProg(Race race)
    {
        string functionName = $"Is{race.Name.CollapseString()}";
        FutureProg? prog = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
        if (prog is null)
        {
            prog = new FutureProg
            {
                FunctionName = functionName
            };
            _context.FutureProgs.Add(prog);
        }

        prog.Category = "Character";
        prog.Subcategory = "Descriptions";
        prog.FunctionComment = $"True if the character is a {race.Name}";
        prog.ReturnType = (long)ProgVariableTypes.Boolean;
        prog.StaticType = 0;
        prog.AcceptsAnyParameters = false;
        prog.Public = true;
        prog.FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")";

        if (!prog.FutureProgsParameters.Any(x => x.ParameterIndex == 0))
        {
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
        }

        _context.SaveChanges();
        return prog;
    }

    private void EnsureEntityDescriptionPatterns(FutureProg prog, IEnumerable<StockDescriptionVariant> variants,
        bool replaceExisting)
    {
        if (replaceExisting)
        {
            List<EntityDescriptionPattern> existingPatterns = _context.EntityDescriptionPatterns
                .Where(x => x.ApplicabilityProgId == prog.Id && (x.Type == 0 || x.Type == 1))
                .ToList();
            if (existingPatterns.Any())
            {
                _context.EntityDescriptionPatterns.RemoveRange(existingPatterns);
                _context.SaveChanges();
            }
        }

        List<EntityDescriptionPattern> existingShortPatterns = _context.EntityDescriptionPatterns
            .Where(x => x.ApplicabilityProgId == prog.Id && x.Type == 0)
            .OrderBy(x => x.Id)
            .ToList();
        List<EntityDescriptionPattern> existingFullPatterns = _context.EntityDescriptionPatterns
            .Where(x => x.ApplicabilityProgId == prog.Id && x.Type == 1)
            .OrderBy(x => x.Id)
            .ToList();

        foreach (StockDescriptionVariant variant in variants)
        {
            if (!existingShortPatterns.Any(x => x.Pattern == variant.ShortDescription))
            {
                _context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
                {
                    Type = 0,
                    ApplicabilityProg = prog,
                    RelativeWeight = 100,
                    Pattern = variant.ShortDescription
                });
            }

            if (!existingFullPatterns.Any(x => x.Pattern == variant.FullDescription))
            {
                _context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
                {
                    Type = 1,
                    ApplicabilityProg = prog,
                    RelativeWeight = 100,
                    Pattern = variant.FullDescription
                });
            }
        }
    }
}
