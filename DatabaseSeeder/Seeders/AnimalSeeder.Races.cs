#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private void CreateEthnicitiesForRace(Race race)
    {
        PopulationBloodModel? model = null;
        switch (race.Name)
        {
            case "Dog":
                model = _context.PopulationBloodModels.FirstOrDefault(x =>
                    x.Name == "Ordinary Canine Blood Type Distribution");
                break;
            case "Cat":
                model = _context.PopulationBloodModels.FirstOrDefault(x =>
                    x.Name == "Ordinary Feline Blood Type Distribution");
                break;
            case "Horse":
                model = _context.PopulationBloodModels.FirstOrDefault(x =>
                    x.Name == "Ordinary Equine Blood Type Distribution");
                break;
            case "Cow":
                model = _context.PopulationBloodModels.FirstOrDefault(x =>
                    x.Name == "Ordinary Bovine Blood Type Distribution");
                break;
            case "Sheep":
                model = _context.PopulationBloodModels.FirstOrDefault(x =>
                    x.Name == "Ordinary Ovine Blood Type Distribution");
                break;
        }

        switch (race.Name)
        {
            case "Dog":
                foreach ((string? name, string? group) in new[]
                         {
                             ("Terrier", "Terrier"),
                             ("Setter", "Setter"),
                             ("Pointer", "Pointer"),
                             ("Retriever", "Retriever"),
                             ("Spaniel", "Spaniel"),
                             ("Water Dog", "Water Dog"),
                             ("Sighthound", "Hound"),
                             ("Scenthound", "Hound"),
                             ("Bulldog", "Bulldog"),
                             ("Mastiff", "Mastiff"),
                             ("Herding Dog", "Herding Dog"),
                             ("Lap Dog", "Lap Dog"),
                             ("Mongrel", "Mongrel")
                         })
                {
                    _context.Ethnicities.Add(new Ethnicity
                    {
                        ParentRace = race,
                        Name = name,
                        ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, name),
                        EthnicGroup = group,
                        TolerableTemperatureFloorEffect = 0,
                        TolerableTemperatureCeilingEffect = 0,
                        PopulationBloodModel = model
                    });
                }

                break;
            case "Bear":
                foreach ((string? name, double floor, double ceiling) in new[]
                         {
                             ("Black Bear", 0.0, 0.0),
                             ("Moon Bear", 0.0, 0.0),
                             ("Brown Bear", -10.0, 0.0),
                             ("Polar Bear", -30.0, -10.0)
                         })
                {
                    _context.Ethnicities.Add(new Ethnicity
                    {
                        ParentRace = race,
                        Name = name,
                        ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, name),
                        EthnicGroup = name,
                        TolerableTemperatureFloorEffect = floor,
                        TolerableTemperatureCeilingEffect = ceiling,
                        PopulationBloodModel = model
                    });
                }

                break;
            default:
                _context.Ethnicities.Add(new Ethnicity
                {
                    ParentRace = race,
                    Name = race.Name,
                    ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, race.Name),
                    EthnicGroup = "Common",
                    TolerableTemperatureFloorEffect = 0,
                    TolerableTemperatureCeilingEffect = 0,
                    PopulationBloodModel = model
                });
                break;
        }
    }

    private void AddRace(string name, string adjective, string? description, BodyProto body, SizeCategory size,
        bool canClimb, double bodypartHealth, string hwMale, string hwFemale, bool sweats = true,
        string breathing = "simple")
    {
        CorpseModel model = _defaultCorpseModel;
        TryGetRaceTemplate(name, out AnimalRaceTemplate? raceTemplate);
        string? seededDescription = raceTemplate is not null
            ? BuildRaceDescriptionForTesting(raceTemplate)
            : description;
        int childage, youthage, youngadultage, adultage, elderage, venerableage;
        if (raceTemplate is not null && AgeProfiles.TryGetValue(raceTemplate.AgeProfileKey, out AnimalAgeProfileTemplate? ageProfile))
        {
            childage = ageProfile.ChildAge;
            youthage = ageProfile.YouthAge;
            youngadultage = ageProfile.YoungAdultAge;
            adultage = ageProfile.AdultAge;
            elderage = ageProfile.ElderAge;
            venerableage = ageProfile.VenerableAge;
        }
        else
        {
            switch (name)
            {
                case "Mouse":
                case "Rat":
                case "Guinea Pig":
                case "Hamster":
                case "Ferret":
                case "Rabbit":
                case "Hare":
                case "Pigeon":
                case "Swallow":
                case "Sparrow":
                case "Quail":
                case "Chicken":
                    childage = 1;
                    youthage = 2;
                    youngadultage = 3;
                    adultage = 4;
                    elderage = 5;
                    venerableage = 6;
                    break;
                default:
                    childage = 1;
                    youthage = 2;
                    youngadultage = 3;
                    adultage = 7;
                    elderage = 12;
                    venerableage = 15;
                    break;
            }
        }

        NonHumanAttributeProfile attributeProfile = raceTemplate is not null
            ? GetAnimalAttributeProfile(raceTemplate)
            : new NonHumanAttributeProfile(0, 0, 0, 0);

        Material driedBlood = new()
        {
            Name = GetRaceBloodMaterialName(name),
            MaterialDescription = "dried blood",
            Density = 1520,
            Organic = true,
            Type = 0,
            BehaviourType = 19,
            ThermalConductivity = 0.2,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420,
            IgnitionPoint = 555.3722,
            HeatDamagePoint = 412.0389,
            ImpactFracture = 1000,
            ImpactYield = 1000,
            ImpactStrainAtYield = 2,
            ShearFracture = 1000,
            ShearYield = 1000,
            ShearStrainAtYield = 2,
            YoungsModulus = 0.1,
            SolventId = 1,
            SolventVolumeRatio = 4,
            ResidueDesc = "It is covered in {0}dried blood",
            ResidueColour = "red",
            Absorbency = 0
        };
        _context.Materials.Add(driedBlood);
        Liquid bloodLiquid = new()
        {
            Name = GetRaceBloodLiquidName(name),
            Description = "blood",
            LongDescription = "a virtually opaque dark red fluid",
            TasteText = "It has a sharply metallic, umami taste",
            VagueTasteText = "It has a metallic taste",
            SmellText = "It has a metallic, coppery smell",
            VagueSmellText = "It has a faintly metallic smell",
            TasteIntensity = 200,
            SmellIntensity = 10,
            AlcoholLitresPerLitre = 0,
            WaterLitresPerLitre = 0.8,
            DrinkSatiatedHoursPerLitre = 6,
            FoodSatiatedHoursPerLitre = 4,
            Viscosity = 1,
            Density = 1,
            Organic = true,
            ThermalConductivity = 0.609,
            ElectricalConductivity = 0.005,
            SpecificHeatCapacity = 4181,
            FreezingPoint = -20,
            BoilingPoint = 100,
            DisplayColour = "bold red",
            DampDescription = "It is damp with blood",
            WetDescription = "It is wet with blood",
            DrenchedDescription = "It is drenched with blood",
            DampShortDescription = "(blood damp)",
            WetShortDescription = "(bloody)",
            DrenchedShortDescription = "(blood drenched)",
            SolventId = 1,
            SolventVolumeRatio = 5,
            InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
            ResidueVolumePercentage = 0.05,
            DriedResidue = driedBlood,
            CountAsQuality = (int)ItemQuality.Legendary,
            CountAs = _bloodLiquid
        };
        _context.Liquids.Add(bloodLiquid);

		Liquid? sweat = null;
        if (sweats)
        {
            Material driedSweat = new()
            {
                Name = GetRaceSweatMaterialName(name),
                MaterialDescription = "dried sweat",
                Density = 1520,
                Organic = true,
                Type = 0,
                BehaviourType = 19,
                ThermalConductivity = 0.2,
                ElectricalConductivity = 0.0001,
                SpecificHeatCapacity = 420,
                IgnitionPoint = 555.3722,
                HeatDamagePoint = 412.0389,
                ImpactFracture = 1000,
                ImpactYield = 1000,
                ImpactStrainAtYield = 2,
                ShearFracture = 1000,
                ShearYield = 1000,
                ShearStrainAtYield = 2,
                YoungsModulus = 0.1,
                SolventId = 1,
                SolventVolumeRatio = 3,
                ResidueDesc = "It is covered in {0}dried sweat",
                ResidueColour = "yellow",
                Absorbency = 0
            };
            _context.Materials.Add(driedSweat);
            sweat = new Liquid
            {
                Name = GetRaceSweatLiquidName(name),
                Description = "sweat",
                LongDescription = "a relatively clear, translucent fluid that smells strongly of wild animal odor",
                TasteText = "It tastes like a pungent, salty lick of someone's underarms",
                VagueTasteText = "It tastes very unpleasant, like underarm stench",
                SmellText = "It has the sharp, pungent smell of animal body odor",
                VagueSmellText = "It has the sharp, pungent smell of animal body odor",
                TasteIntensity = 200,
                SmellIntensity = 200,
                AlcoholLitresPerLitre = 0,
                WaterLitresPerLitre = 0.95,
                DrinkSatiatedHoursPerLitre = 5,
                FoodSatiatedHoursPerLitre = 0,
                Viscosity = 1,
                Density = 1,
                Organic = true,
                ThermalConductivity = 0.609,
                ElectricalConductivity = 0.005,
                SpecificHeatCapacity = 4181,
                FreezingPoint = -20,
                BoilingPoint = 100,
                DisplayColour = "yellow",
                DampDescription = "It is damp with sweat",
                WetDescription = "It is wet and smelly with sweat",
                DrenchedDescription = "It is soaking wet and smelly with sweat",
                DampShortDescription = "(sweat-damp)",
                WetShortDescription = "(sweaty)",
                DrenchedShortDescription = "(sweat-drenched)",
                SolventId = 1,
                SolventVolumeRatio = 5,
                InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
                ResidueVolumePercentage = 0.05,
                DriedResidue = driedSweat,
                CountAsQuality = (int)ItemQuality.Legendary,
                CountAs = _sweatLiquid
            };
            _context.Liquids.Add(sweat);
        }

        BloodModel? bloodModel = null;
        PopulationBloodModel? populationModel = null;
        if (raceTemplate?.BloodProfileKey is not null)
        {
            (BloodModel BloodModel, PopulationBloodModel PopulationBloodModel)? bloodProfile = CreateBloodProfile(raceTemplate.BloodProfileKey);
            if (bloodProfile is not null)
            {
                (bloodModel, populationModel) = bloodProfile.Value;
            }
        }

        Race race = new()
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(description)
                            ? $"{name}s are {adjective.ToLowerInvariant()} creatures"
                            : description,
            BaseBody = body,
            AllowedGenders = "2 3",
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 1.0,
            AvailabilityProg = _alwaysFalse,
            CorpseModel = model,
            DefaultHealthStrategy = _healthStrategy,
            DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, raceTemplate?.CombatStrategyKey ?? "Beast Brawler"),
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NeedsToBreathe = true,
            SizeStanding = (int)size,
            SizeProne = (int)size,
            SizeSitting = (int)size,
            CommunicationStrategyType = "humanoid",
            HandednessOptions = "1 3",
            DefaultHandedness = 1,
            ChildAge = childage,
            YouthAge = youthage,
            YoungAdultAge = youngadultage,
            AdultAge = adultage,
            ElderAge = elderage,
            VenerableAge = venerableage,
            CanClimb = canClimb,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            BodypartHealthMultiplier = ResolveAnimalRaceHealthMultiplier(raceTemplate, size, bodypartHealth),
            BodypartSizeModifier = 0,
            TemperatureRangeCeiling = 40,
            TemperatureRangeFloor = 0,
            CanEatCorpses = false,
            CanEatMaterialsOptIn = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            RaceUsesStamina = true,
            NaturalArmourType = _naturalArmour,
            NaturalArmourQuality = 2,
            SweatLiquid = sweat,
            SweatRateInLitresPerMinute = 0.8,
            BloodLiquid = bloodLiquid,
            BloodModel = bloodModel,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})",
            MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*10000",
            MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*40000",
			MaximumFoodSatiatedHours = raceTemplate?.MaximumFoodSatiatedHours ?? SatiationLimitSeederHelper.MaximumFoodHoursForCadence(12.0),
			MaximumDrinkSatiatedHours = raceTemplate?.MaximumDrinkSatiatedHours ?? SatiationLimitSeederHelper.MaximumDrinkHoursForCadence(6.0),
            DefaultHeightWeightModelMale = _hwModels[hwMale],
            DefaultHeightWeightModelNeuter = _hwModels[hwMale],
            DefaultHeightWeightModelFemale = _hwModels[hwFemale],
            DefaultHeightWeightModelNonBinary = _hwModels[hwFemale]
        };
        _context.Races.Add(race);

        foreach (TraitDefinition attribute in _attributes)
        {
            _context.RacesAttributes.Add(new RacesAttributes
            {
                Race = race,
                Attribute = attribute,
                IsHealthAttribute = attribute.TraitGroup == "Physical",
                AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, attributeProfile),
                DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, attributeProfile)
            });
        }

        CreateEthnicitiesForRace(race);
        _context.SaveChanges();
        CreateDescriptionsForRace(race);
        CreateRaceAttacks(race);

        switch (breathing)
        {
            case "simple":
                _context.RacesBreathableGases.Add(new RacesBreathableGases
                {
                    Race = race,
                    Gas = _breathableAir,
                    Multiplier = 1.0
                });
                race.BreathingModel = "simple";
                break;
            case "insect":
                _context.RacesBreathableGases.Add(new RacesBreathableGases
                {
                    Race = race,
                    Gas = _breathableAir,
                    Multiplier = 1.0
                });
                race.BreathingModel = "partless";
                break;
            case "partless":
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _saltWater,
                    Multiplier = 1.0
                });
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _brackishWater,
                    Multiplier = 0.5
                });
                race.BreathingModel = "partless";
                break;
            case "freshwater":
                foreach (Liquid liquid in _freshWaters)
                {
                    _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                    {
                        Race = race,
                        Liquid = liquid,
                        Multiplier = 1.0
                    });
                }

                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _brackishWater,
                    Multiplier = 0.5
                });
                race.BreathingModel = "gills";
                break;
            case "blowhole":
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _saltWater,
                    Multiplier = 1.0
                });
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _brackishWater,
                    Multiplier = 0.5
                });
                race.BreathingModel = "blowhole";
                break;
            case "saltwater":
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _saltWater,
                    Multiplier = 1.0
                });
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _brackishWater,
                    Multiplier = 0.5
                });
                race.BreathingModel = "gills";
                break;
        }

        if (raceTemplate is not null)
        {
            ApplyAnimalDietSettings(race, raceTemplate);
        }

        if (raceTemplate?.AdditionalBodypartUsages is not null)
        {
            foreach (AnimalBodypartUsageTemplate usage in raceTemplate.AdditionalBodypartUsages)
            {
                AddRacialBodypartUsage(usage.BodypartAlias, usage.Usage, race);
            }
        }
        else
        {
            switch (name)
            {
                case "Cow":
                case "Pig":
                case "Sheep":
                case "Ox":
                case "Bison":
                case "Buffalo":
                case "Deer":
                case "Moose":
                case "Llama":
                case "Alpaca":
                case "Rhinocerous":
                case "Horse":
                    AddRacialBodypartUsage("udder", "female", race);
                    break;
            }

            switch (name)
            {
                case "Ox":
                case "Cow":
                case "Bison":
                case "Buffalo":
                case "Goat":
                case "Sheep":
                    AddRacialBodypartUsage("rhorn", "male", race);
                    AddRacialBodypartUsage("lhorn", "male", race);
                    break;
                case "Rhinocerous":
                    AddRacialBodypartUsage("horn", "general", race);
                    break;
                case "Deer":
                case "Moose":
                    AddRacialBodypartUsage("rantler", "male", race);
                    AddRacialBodypartUsage("lantler", "male", race);
                    break;
                case "Boar":
                case "Warthog":
                    AddRacialBodypartUsage("rtusk", "male", race);
                    AddRacialBodypartUsage("ltusk", "male", race);
                    break;
                case "Elephant":
                case "Hippopotamus":
                    AddRacialBodypartUsage("rtusk", "general", race);
                    AddRacialBodypartUsage("ltusk", "general", race);
                    break;
            }
        }
    }


	private void SetupArmourTypes()
	{
		ConfigureAnimalNaturalArmours();

        _organArmour = new ArmourType
        {
            Name = "Non-Human Natural Organ Armour",
            MinimumPenetrationDegree = 1,
            BaseDifficultyDegrees = 0,
            StackedDifficultyDegrees = 0,
            Definition = @"<ArmourType>

	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">damage-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">damage-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">damage-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">damage-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">damage*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">damage-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">damage-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">damage*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">damage-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">damage-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">damage-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">damage-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">damage-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">damage-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">damage-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">damage*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">damage-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">damage-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">damage-(1.0*quality)</Expression>        <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">pain-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">pain-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">pain-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">pain*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">pain-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">pain-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">pain*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">pain-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">pain-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">pain-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">pain-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">pain-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">pain-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">pain-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">pain*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">pain-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">pain-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">pain-(1.0*quality)</Expression>        <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">stun-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">stun-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">stun-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">stun*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">stun-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">stun-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">stun*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">stun-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">stun-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">stun-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">stun-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">stun-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">stun-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">stun-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">stun*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">stun-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">stun-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">stun-(1.0*quality)</Expression>        <!-- Arcane -->    
	</DissipateExpressionsStun>  
	
	<!-- Note: Organ Damage is final - there's no ""lower layer"" to pass on to, therefore there is no need for Absorb expressions -->
 </ArmourType>"
        };
        _context.ArmourTypes.Add(_organArmour);
        _context.SaveChanges();

        _boneArmour = new ArmourType
        {
            Name = "Non-Human Natural Bone Armour",
            MinimumPenetrationDegree = 1,
            BaseDifficultyDegrees = 0,
            StackedDifficultyDegrees = 0,
            Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
		Chopping = 1
		Crushing = 2
		Piercing = 3
		Ballistic = 4
		Burning = 5
		Freezing = 6
		Chemical = 7
		Shockwave = 8
		Bite = 9
		Claw = 10
		Electrical = 11
		Hypoxia = 12
		Cellular = 13
		Sonic = 14
		Shearing = 15
		ArmourPiercing = 16
		Wrenching = 17
		Shrapnel = 18
		Necrotic = 19
		Falling = 20
		Eldritch = 21
		Arcane = 22
		
		Severity Values:
		
		None = 0
		Superficial = 1
		Minor = 2
		Small = 3
		Moderate = 4
		Severe = 5
		VerySevere = 6
		Grievous = 7
		Horrifying = 8
	-->
	<DamageTransformations>
		<Transform fromtype=""0"" totype=""2"" severity=""5""></Transform> <!-- Slashing to Crushing when <= Severe -->
		<Transform fromtype=""1"" totype=""2"" severity=""5""></Transform> <!-- Chopping to Crushing when <= Severe -->
		<Transform fromtype=""3"" totype=""2"" severity=""4""></Transform> <!-- Piercing to Crushing when <= Moderate -->
		<Transform fromtype=""4"" totype=""2"" severity=""4""></Transform> <!-- Ballistic to Crushing when <= Moderate -->
		<Transform fromtype=""9"" totype=""2"" severity=""5""></Transform> <!-- Bite to Crushing when <= Severe -->
		<Transform fromtype=""10"" totype=""2"" severity=""5""></Transform> <!-- Claw to Crushing when <= Severe -->
		<Transform fromtype=""15"" totype=""2"" severity=""5""></Transform> <!-- Shearing to Crushing when <= Severe -->
		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
	</DamageTransformations>
	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">max(damage*0.1,damage-(quality * 2 * strength/115000)))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage - (quality * 2)</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">damage - (quality * 2)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">damage - (quality * 2)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage - (quality * 2)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">damage - (quality * 2)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 2)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 2)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 2)</Expression>                    <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">pain - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">pain - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">pain - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">pain - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">stun - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">stun - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">stun - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">stun - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsStun>  
	<!-- 
	
		Absorb expressions are applied after dissipate expressions and item/part damage. 
		The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
	<AbsorbExpressions>
		<Expression damagetype=""0"">damage*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">damage*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">damage*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">damage*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">damage*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">damage*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressions>  
	<AbsorbExpressionsPain>
		<Expression damagetype=""0"">pain*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">pain*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">pain*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">pain*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">pain*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">pain*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsPain>  
	<AbsorbExpressionsStun>
		<Expression damagetype=""0"">stun*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">stun*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">stun*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">stun*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">stun*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">stun*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsStun>
 </ArmourType>"
        };
        _context.ArmourTypes.Add(_boneArmour);
        _context.SaveChanges();
    }

    private void SetupShapesAndMaterials()
    {
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodypart Shapes...");
        AddShape("Hoof");
        AddShape("Frog");
        AddShape("Claw");
        AddShape("Flank");
        AddShape("Withers");
        AddShape("Loin");
        AddShape("Rump");
        AddShape("Muzzle");
        AddShape("Upper Foreleg");
        AddShape("Lower Foreleg");
        AddShape("Upper Hindleg");
        AddShape("Lower Hindleg");
        AddShape("Front Hock");
        AddShape("Rear Hock");
        AddShape("Upper Leg");
        AddShape("Lower Leg");
        AddShape("Talon");
        AddShape("Dewclaw");
        AddShape("Tail");
        AddShape("Wing Base");
        AddShape("Wing");
        AddShape("Tusk");
        AddShape("Antler");
        AddShape("Horn");
        AddShape("Udder");
        AddShape("Fang");
        AddShape("Serpent Body");
        AddShape("Fin");
        AddShape("Gill");
        AddShape("Jaw");
        AddShape("Paw");
        AddShape("Beak");
        AddShape("Fin");
        AddShape("Peduncle");
        AddShape("Tendril");
        AddShape("Fluke");
        AddShape("Stock");
        AddShape("Mantle");
        AddShape("Arm");
        AddShape("Tentacle");
        AddShape("Body");
        AddShape("Head");
        AddShape("Front Flipper");
        AddShape("Hind Flipper");
        AddShape("Blowhole");
        AddShape("Insect Thorax");
        AddShape("Insect Abdomen");
        AddShape("Antenna");
        AddShape("Mandible");
        AddShape("Compound Eye");
        AddShape("Stinger");

        _context.SaveChanges();

        foreach (BodypartShape shape in _context.BodypartShapes)
        {
            _cachedShapes[shape.Name] = shape;
        }

        foreach (Material material in _context.Materials)
        {
            _cachedMaterials[material.Name] = material;
        }
    }

    private (BloodModel BloodModel, PopulationBloodModel PopulationBloodModel) SetupBloodModel(string race,
        IEnumerable<string> antigens,
        IEnumerable<(string Name, IEnumerable<string> Antigens, double weight)> types)
    {
        BloodModel model = new()
        {
            Name = $"{race} Blood Model"
        };
        _context.BloodModels.Add(model);

        Dictionary<string, BloodtypeAntigen> dbantigens = new();
        foreach (string antigen in antigens)
        {
            BloodtypeAntigen dbantigen = new()
            {
                Name = antigen
            };
            _context.BloodtypeAntigens.Add(dbantigen);
            dbantigens[antigen] = dbantigen;
        }

        PopulationBloodModel populationModel = new()
        {
            Name = $"Ordinary {race} Blood Type Distribution"
        };
        _context.PopulationBloodModels.Add(populationModel);

        foreach ((string? name, IEnumerable<string>? contained, double weight) in types)
        {
            Bloodtype bloodtype = new()
            {
                Name = name
            };
            foreach (string item in contained)
            {
                bloodtype.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
                {
                    Bloodtype = bloodtype,
                    BloodtypeAntigen = dbantigens[item]
                });
            }

            _context.Bloodtypes.Add(bloodtype);
            model.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
            {
                BloodModel = model,
                Bloodtype = bloodtype
            });
            populationModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
            {
                Bloodtype = bloodtype,
                PopulationBloodModel = populationModel,
                Weight = weight
            });
        }

        return (model, populationModel);
    }
}
