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
    #region Combat
    private void SeedCombatStrategy(string name, string description, double weaponUse, double naturalUse,
            double auxilliaryUse, bool preferFavourite, bool preferArmed, bool preferNonContact, bool preferShields,
            bool attackCritical, bool attackUnarmed, bool skirmish, bool fallbackToUnarmed,
            bool automaticallyMoveToTarget, bool manualPositionManagement, bool moveToMeleeIfCannotRange,
            PursuitMode pursuit, CombatStrategyMode melee, CombatStrategyMode ranged,
            AutomaticInventorySettings inventory, AutomaticMovementSettings movement,
            AutomaticRangedSettings rangesettings, AttackHandednessOptions setup, GrappleResponse grapple,
            double requiredMinimumAim, double minmumStamina, DefenseType defaultDefenseType,
            IEnumerable<MeleeAttackOrderPreference> order,
            CombatMoveIntentions forbiddenIntentions = CombatMoveIntentions.None,
            CombatMoveIntentions preferredIntentions = CombatMoveIntentions.None)
    {
        if (_context.CharacterCombatSettings.Any(x => x.Name == name))
        {
            return;
        }

        CharacterCombatSetting strategy = new()
        {
            Name = name,
            Description = description,
            GlobalTemplate = true,
            AvailabilityProg = _alwaysTrue,
            WeaponUsePercentage = weaponUse,
            MagicUsePercentage = 0.0,
            PsychicUsePercentage = 0.0,
            NaturalWeaponPercentage = naturalUse,
            AuxiliaryPercentage = auxilliaryUse,
            PreferFavouriteWeapon = preferFavourite,
            PreferToFightArmed = preferArmed,
            PreferNonContactClinchBreaking = preferNonContact,
            PreferShieldUse = preferShields,
            ClassificationsAllowed = "1 2 3 4 5 7",
            RequiredIntentions = 0,
            ForbiddenIntentions = (long)forbiddenIntentions,
            PreferredIntentions = (long)preferredIntentions,
            AttackCriticallyInjured = attackCritical,
            AttackHelpless = attackUnarmed,
            SkirmishToOtherLocations = skirmish,
            PursuitMode = (int)pursuit,
            DefaultPreferredDefenseType = (int)defaultDefenseType,
            PreferredMeleeMode = (int)melee,
            PreferredRangedMode = (int)ranged,
            FallbackToUnarmedIfNoWeapon = fallbackToUnarmed,
            AutomaticallyMoveTowardsTarget = automaticallyMoveToTarget,
            InventoryManagement = (int)inventory,
            MovementManagement = (int)movement,
            RangedManagement = (int)rangesettings,
            ManualPositionManagement = manualPositionManagement,
            MinimumStaminaToAttack = minmumStamina,
            MoveToMeleeIfCannotEngageInRangedCombat = moveToMeleeIfCannotRange,
            PreferredWeaponSetup = (int)setup,
            RequiredMinimumAim = requiredMinimumAim,
            MeleeAttackOrderPreference = order.Select(selector: x => ((int)x).ToString()).ListToCommaSeparatedValues(separator: " "),
            GrappleResponse = (int)grapple
        };
        _context.CharacterCombatSettings.Add(entity: strategy);
        _context.SaveChanges();
    }

    private void SeedCombatStrategies()
    {
        List<MeleeAttackOrderPreference> defaultOrder = new()
        {
            MeleeAttackOrderPreference.Weapon,
            MeleeAttackOrderPreference.Implant,
            MeleeAttackOrderPreference.Prosthetic,
            MeleeAttackOrderPreference.Magic,
            MeleeAttackOrderPreference.Psychic
        };

        SeedCombatStrategy(name: "Animal", description: "Fully automatic brawler designed for use with animals", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Biter", description: "Fully automatic clinch-brawler designed for use with animals with bite attacks", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Behemoth", description: "Fully automatic brawler designed for use with big, strong animals", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Counter, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Swooper", description: "Fully automatic flying hunter designed for creatures that dive through enemies with breath and wing attacks", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: true, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.Swooper, ranged: CombatStrategyMode.Swooper,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.4,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Beast Drowner", description: "Fully automatic aquatic ambusher designed to drag air-breathers underwater and grapple them there", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.Drowner, ranged: CombatStrategyMode.Drowner,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Counter, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Beast Dropper", description: "Fully automatic aerial grappler designed to lift opponents upward and drop them", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: true, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
            melee: CombatStrategyMode.Dropper, ranged: CombatStrategyMode.Dropper,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Counter, requiredMinimumAim: 0.4,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Beast Physical Avoider", description: "Fully automatic avoider that uses pushbacks, trips and staggers to keep enemies out of melee range", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: false, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.PhysicalAvoider, ranged: CombatStrategyMode.PhysicalAvoider,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.4,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder,
            preferredIntentions: CombatMoveIntentions.Disadvantage);
        SeedCombatStrategy(name: "Wimpy Animal", description: "Fully automatic wimpy designed for use with animals", weaponUse: 0.0, naturalUse: 0.1, auxilliaryUse: 0.9, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: false, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.Flee, ranged: CombatStrategyMode.Flee,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
    }

    private WeaponAttack AddAttack(string name, BuiltInCombatMoveType moveType,
        MeleeWeaponVerb verb, Difficulty attacker, Difficulty dodge, Difficulty parry, Difficulty block,
        Alignment alignment, Orientation orientation, double stamina, double relativeSpeed,
        BodypartShape shape, TraitExpression damage, string attackMessage,
        DamageType damageType = DamageType.Crushing, double weighting = 100,
        CombatMoveIntentions intentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
		string? additionalInfo = null)
    {
        string formattedAttackMessage = CombatSeederMessageStyleHelper.FormatAttackMessage(
            attackMessage,
            CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

        WeaponAttack attack = new()
        {
            Verb = (int)verb,
            BaseAttackerDifficulty = (int)attacker,
            BaseBlockDifficulty = (int)block,
            BaseDodgeDifficulty = (int)dodge,
            BaseParryDifficulty = (int)parry,
            MoveType = (int)moveType,
            RecoveryDifficultySuccess = (int)Difficulty.Easy,
            RecoveryDifficultyFailure = (int)Difficulty.Hard,
            Intentions = (long)intentions,
            Weighting = weighting,
            ExertionLevel = (int)ExertionLevel.Heavy,
            DamageType = (int)damageType,
            DamageExpression = damage,
            StunExpression = damage,
            PainExpression = damage,
            BodypartShapeId = shape.Id,
            StaminaCost = stamina,
            BaseDelay = relativeSpeed,
            Name = name,
            Orientation = (int)orientation,
            Alignment = (int)alignment,
            HandednessOptions = 0,
            AdditionalInfo = additionalInfo
        };
        _context.WeaponAttacks.Add(attack);
        _context.SaveChanges();

        CombatMessage message = new()
        {
            Type = (int)moveType,
            Message = formattedAttackMessage,
            Priority = 50,
            Verb = (int)verb,
            Chance = 1.0,
            FailureMessage = formattedAttackMessage
        };
        message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
        { CombatMessage = message, WeaponAttack = attack });
        _context.CombatMessages.Add(message);
        _context.SaveChanges();
        return attack;
    }

    private void AddAttackToRace(string whichAttack, Race race, ItemQuality quality)
    {
        List<long> bodies = new();
        bodies.Add(race.BaseBodyId);
        BodyProto body = race.BaseBody.CountsAs;
        while (body != null)
        {
            bodies.Add(body.Id);
            body = body.CountsAs;
        }

        WeaponAttack attack = _attacks[whichAttack];
        foreach (BodypartProto? bodypart in _context.BodypartProtos.Where(x =>
                     bodies.Contains(x.BodyId) && x.BodypartShapeId == attack.BodypartShapeId))
        {
            _context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
            {
                Bodypart = bodypart,
                Race = race,
                WeaponAttack = attack,
                Quality = (int)quality
            });
        }
    }

    private void AddJellyfishAttack(Race race)
    {
        string attackAddendum =
            CombatSeederMessageStyleHelper.AttackSuffix(
                CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

        BodypartShape tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");
        List<BodypartProto> tendrils = _context.BodypartProtos.Where(x => x.Body == race.BaseBody && x.BodypartShape == tendrilShape)
            .ToList();

        Drug venom = new()
        {
            Name = $"{race.Name} Venom",
            IntensityPerGram = 10,
            DrugVectors = (int)(DrugVector.Touched | DrugVector.Injected),
            RelativeMetabolisationRate = 0.05
        };
        _context.Drugs.Add(venom);
        _context.SaveChanges();

        Liquid liquid = new()
        {
            Name = $"{race.Name} Venom".ToLowerInvariant(),
            Description = "a clear liquid",
            LongDescription = "a clear, translucent liquid",
            TasteText =
                "It has one of the most intense bitter tastes and instantly makes your tongue and lips wrack with pain",
            VagueTasteText =
                "It has one of the most intense bitter tastes and instantly makes your tongue and lips wrack with pain",
            SmellText =
                "It smells extremely bitter and actually makes your nose and throat burn like hell just from smelling it",
            VagueSmellText =
                "It smells extremely bitter and actually makes your nose and throat burn like hell just from smelling it",
            TasteIntensity = 2000,
            SmellIntensity = 2000,
            AlcoholLitresPerLitre = 0,
            WaterLitresPerLitre = 0.5,
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
            DisplayColour = "bold pink",
            DampDescription = "It is damp",
            WetDescription = "It is wet",
            DrenchedDescription = "It is drenched",
            DampShortDescription = "(damp)",
            WetShortDescription = "(wet)",
            DrenchedShortDescription = "(drenched)",
            SolventId = 1,
            SolventVolumeRatio = 5,
            InjectionConsequence = (int)LiquidInjectionConsequence.Deadly,
            ResidueVolumePercentage = 0.05,
            DriedResidue = null,
            Drug = venom,
            DrugGramsPerUnitVolume = 1000
        };
        _context.Liquids.Add(liquid);
        _context.SaveChanges();

        WeaponAttack attack = AddAttack($"{race.Name} Sting", BuiltInCombatMoveType.EnvenomingAttackClinch,
            MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
            Alignment.Front, Orientation.Low, 0.5, 1.5, tendrilShape, _snakeBiteDamage,
            $"@ drift|drifts close and brush|brushes a stinging tendril across $1{attackAddendum}", DamageType.Cellular,
            additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>0.08</MaximumQuantity>
   <MinimumWoundSeverity>0</MinimumWoundSeverity>
 </Data>");


        foreach (BodypartProto part in tendrils)
        {
            _context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
            {
                Bodypart = part,
                Race = race,
                WeaponAttack = attack,
                Quality = (int)ItemQuality.Standard
            });
        }

        _context.SaveChanges();
    }

    private void AddSerpentAttack(Race race, bool venomous)
    {
        string attackAddendum =
            CombatSeederMessageStyleHelper.AttackSuffix(
                CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

        BodypartShape fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
        BodypartProto fang = _context.BodypartProtos.First(x => x.Body == race.BaseBody && x.BodypartShape == fangShape);

        WeaponAttack attack;
        if (venomous)
        {
            Drug venom = new()
            {
                Name = $"{race.Name} Venom",
                IntensityPerGram = 1,
                DrugVectors = (int)DrugVector.Injected,
                RelativeMetabolisationRate = 0.05
            };
            _context.Drugs.Add(venom);
            _context.SaveChanges();

            Liquid liquid = new()
            {
                Name = $"{race.Name} Venom".ToLowerInvariant(),
                Description = "a clear liquid",
                LongDescription = "a clear, translucent liquid",
                TasteText = "It has one of the most intense bitter tastes and instantly makes your tongue go numb",
                VagueTasteText =
                    "It has one of the most intense bitter tastes and instantly makes your tongue go numb",
                SmellText =
                    "It smells extremely bitter and actually makes your nose and throat hurt just from smelling it",
                VagueSmellText =
                    "It smells extremely bitter and actually makes your nose and throat hurt just from smelling it",
                TasteIntensity = 2000,
                SmellIntensity = 2000,
                AlcoholLitresPerLitre = 0,
                WaterLitresPerLitre = 0.5,
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
                DisplayColour = "bold pink",
                DampDescription = "It is damp",
                WetDescription = "It is wet",
                DrenchedDescription = "It is drenched",
                DampShortDescription = "(damp)",
                WetShortDescription = "(wet)",
                DrenchedShortDescription = "(drenched)",
                SolventId = 1,
                SolventVolumeRatio = 5,
                InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
                ResidueVolumePercentage = 0.05,
                DriedResidue = null,
                Drug = venom,
                DrugGramsPerUnitVolume = 1000
            };
            _context.Liquids.Add(liquid);
            _context.SaveChanges();

            attack = AddAttack($"{race.Name} Bite", BuiltInCombatMoveType.EnvenomingAttackClinch,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Low, 4.0, 1.0, fangShape, _snakeBiteDamage,
                $"@ strike|strikes with a quick bite at $1{attackAddendum}", DamageType.Bite,
                additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>0.005</MaximumQuantity>
   <MinimumWoundSeverity>3</MinimumWoundSeverity>
 </Data>");
        }
        else
        {
            attack = AddAttack($"{race.Name} Bite", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite,
                Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
                Orientation.Low, 4.0, 1.0, fangShape, _snakeBiteDamage,
                $"@ dart|darts in and sink|sinks &0's fangs into $1{attackAddendum}", DamageType.Bite);
        }

        _context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
        {
            Bodypart = fang,
            Race = race,
            WeaponAttack = attack,
            Quality = (int)ItemQuality.Standard
        });
        _context.SaveChanges();
    }

    private void AddAttacks(Race race, ItemQuality quality, bool carnivoreBites = false,
        bool herbivoreBites = false, bool smallAnimalBites = false, bool clawAttacks = false,
        bool hoofAttacks = false, bool chargeAttacks = false, bool hornAttacks = false, bool tuskAttacks = false,
        bool antlerAttacks = false, bool fishBites = false, bool crabAttacks = false, bool sharkAttacks = false)
    {
        if (fishBites)
        {
            AddAttackToRace("fishbite", race, quality);
            AddAttackToRace("fishquickbite", race, quality);
        }

        if (sharkAttacks)
        {
            AddAttackToRace("sharkbite", race, quality);
            AddAttackToRace("sharkreelbite", race, quality);
        }

        if (crabAttacks)
        {
            AddAttackToRace("crabpinch", race, quality);
        }

        if (carnivoreBites)
        {
            AddAttackToRace("carnivorebite", race, quality);
            AddAttackToRace("carnivoresmashbite", race, quality);
            AddAttackToRace("carnivorelowbite", race, quality);
            AddAttackToRace("carnivorehighbite", race, quality);
            AddAttackToRace("carnivorelowestbite", race, quality);
            AddAttackToRace("carnivoreclinchbite", race, quality);
            AddAttackToRace("carnivoreclinchhighbite", race, quality);
            AddAttackToRace("carnivoreclinchhighestbite", race, quality);
            AddAttackToRace("carnivoredownbite", race, quality);
        }

        if (herbivoreBites)
        {
            AddAttackToRace("herbivorebite", race, quality);
            AddAttackToRace("herbivoresmashbite", race, quality);
        }

        if (smallAnimalBites)
        {
            AddAttackToRace("smallbite", race, quality);
            AddAttackToRace("smallsmashbite", race, quality);
            AddAttackToRace("smalllowbite", race, quality);
            AddAttackToRace("smalldownedbite", race, quality);
        }

        if (clawAttacks)
        {
            AddAttackToRace("clawswipe", race, quality);
            AddAttackToRace("clawsmashswipe", race, quality);
            AddAttackToRace("clawlowswipe", race, quality);
            AddAttackToRace("clawhighswipe", race, quality);
        }

        if (hoofAttacks)
        {
            AddAttackToRace("hoofstomp", race, quality);
            AddAttackToRace("hoofstompsmash", race, quality);
        }

        if (chargeAttacks)
        {
            AddAttackToRace("barge", race, quality);
            AddAttackToRace("bargesmash", race, quality);
            AddAttackToRace("clinchbarge", race, quality);
        }

        if (hornAttacks)
        {
            AddAttackToRace("gorehorn", race, quality);
        }

        if (antlerAttacks)
        {
            AddAttackToRace("goreantler", race, quality);
        }

        if (tuskAttacks)
        {
            AddAttackToRace("goretusk", race, quality);
            AddAttackToRace("tusksweep", race, quality);
        }
    }

    private void SetupAttacks(bool firstTime)
    {
        IReadOnlyDictionary<string, string> damageExpressions = BuildAnimalDamageExpressions();
        if (!firstTime)
        {
            TraitExpression GetOrCreateExpression(string name, string expression)
            {
                TraitExpression? existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
                if (existing is not null)
                {
                    existing.Expression = expression;
                    return existing;
                }

                TraitExpression created = new()
                {
                    Name = name,
                    Expression = expression
                };
                _context.TraitExpressions.Add(created);
                _context.SaveChanges();
                return created;
            }

            TraitExpression peckDamage = GetOrCreateExpression("Animal Peck Damage",
                damageExpressions["Animal Peck Damage"]);
            TraitExpression talonDamage = GetOrCreateExpression("Animal Talon Damage",
                damageExpressions["Animal Talon Damage"]);
            TraitExpression mandibleDamage = GetOrCreateExpression("Animal Mandible Damage",
                damageExpressions["Animal Mandible Damage"]);
            TraitExpression ramDamage = GetOrCreateExpression("Animal Ram Damage",
                damageExpressions["Animal Ram Damage"]);
            GetOrCreateExpression("Small Animal Bite Damage", damageExpressions["Small Animal Bite Damage"]);
            GetOrCreateExpression("Fish Bite Damage", damageExpressions["Fish Bite Damage"]);
            GetOrCreateExpression("Herbivorous Animal Bite Damage",
                damageExpressions["Herbivorous Animal Bite Damage"]);
            GetOrCreateExpression("Carnivorous Animal Bite Damage",
                damageExpressions["Carnivorous Animal Bite Damage"]);
            TraitExpression sharkBite = GetOrCreateExpression("Shark Bite Damage", damageExpressions["Shark Bite Damage"]);
            TraitExpression clawDamage = GetOrCreateExpression("Animal Claw Damage", damageExpressions["Animal Claw Damage"]);
            TraitExpression smashDamage = GetOrCreateExpression("Animal Smash Damage", damageExpressions["Animal Smash Damage"]);
            GetOrCreateExpression("Animal Coup De Grace Damage",
                damageExpressions["Animal Coup De Grace Damage"]);
            RefreshDragonfireBreathDamageExpression(damageExpressions);
            _snakeBiteDamage = GetOrCreateExpression("Snake Bite Damage",
                damageExpressions["Snake Bite Damage"]);

            _attacks["carnivorebite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Bite");
            _attacks["carnivoresmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Smash Bite");
            _attacks["carnivorelowbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Low Bite");
            _attacks["carnivorehighbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore High Bite");
            _attacks["carnivorelowestbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Lowest Bite");
            _attacks["carnivoreclinchbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Clinch Bite");
            _attacks["carnivoreclinchhighbite"] =
                _context.WeaponAttacks.First(x => x.Name == "Carnivore High Clinch Bite");
            _attacks["carnivoreclinchhighestbite"] =
                _context.WeaponAttacks.First(x => x.Name == "Carnivore Highest Clinch Bite");
            _attacks["carnivoredownbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Downed Bite");
            _attacks["herbivorebite"] = _context.WeaponAttacks.First(x => x.Name == "Herbivore Bite");
            _attacks["herbivoresmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Herbivore Smash Bite");
            _attacks["smallbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Bite");
            _attacks["smallsmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Smash Bite");
            _attacks["smalllowbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Low Bite");
            _attacks["smalldownedbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Downed Bite");
            _attacks["clawswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Swipe");
            _attacks["clawsmashswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Smash Swipe");
            _attacks["clawlowswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Low Swipe");
            _attacks["clawhighswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw High Swipe");
            _attacks["hoofstomp"] = _context.WeaponAttacks.First(x => x.Name == "Hoof Stomp");
            _attacks["hoofstompsmash"] = _context.WeaponAttacks.First(x => x.Name == "Hoof Stomp Smash");
            _attacks["barge"] = _context.WeaponAttacks.First(x => x.Name == "Animal Barge");
            _attacks["bargesmash"] = _context.WeaponAttacks.First(x => x.Name == "Animal Barge Smash");
            _attacks["clinchbarge"] = _context.WeaponAttacks.First(x => x.Name == "Animal Clinch Barge");
            _attacks["gorehorn"] = _context.WeaponAttacks.First(x => x.Name == "Horn Gore");
            _attacks["goreantler"] = _context.WeaponAttacks.First(x => x.Name == "Antler Gore");
            _attacks["goretusk"] = _context.WeaponAttacks.First(x => x.Name == "Tusk Gore");
            _attacks["tusksweep"] = _context.WeaponAttacks.First(x => x.Name == "Tusk Sweep");
            _attacks["crabpinch"] = _context.WeaponAttacks.First(x => x.Name == "Crab Pinch");
            _attacks["fishbite"] = _context.WeaponAttacks.First(x => x.Name == "Fish Bite");
            _attacks["fishquickbite"] = _context.WeaponAttacks.First(x => x.Name == "Fish Quick Bite");
            _attacks["sharkbite"] = _context.WeaponAttacks.First(x => x.Name == "Shark Bite");
            _attacks["sharkreelbite"] = _context.WeaponAttacks.First(x => x.Name == "Shark Reel Bite");

            BodypartShape beakShape = _context.BodypartShapes.First(x => x.Name == "Beak");
            BodypartShape talonShape = _context.BodypartShapes.First(x => x.Name == "Talon");
            BodypartShape fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
            BodypartShape mandibleShape = _context.BodypartShapes.First(x => x.Name == "Mandible");
            BodypartShape mouthShape = _context.BodypartShapes.First(x => x.Name == "Mouth");
            BodypartShape headShape = _context.BodypartShapes.First(x => x.Name == "Head");
            BodypartShape tailShape = _context.BodypartShapes.First(x => x.Name == "Tail");
            BodypartShape tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");
            BodypartShape clawShape = _context.BodypartShapes.First(x => x.Name == "Claw");
            BodypartShape shoulderShape = _context.BodypartShapes.First(x => x.Name == "Shoulder");

            string attackAddendum = _questionAnswers["messagestyle"].ToLowerInvariant() switch
            {
                "sentences" => ".",
                "sparse" => ".",
                _ => ""
            };

            static string ForcedMovementAttackData(Difficulty resist, ForcedMovementTypes types,
                ForcedMovementVerbs verbs, ForcedMovementRange range)
            {
                return new XElement("Data",
                    new XElement("Resist", resist.ToString()),
                    new XElement("Types", types.ToString()),
                    new XElement("Verbs", verbs.ToString()),
                    new XElement("Range", range.ToString())
                ).ToString();
            }

            _attacks["beakpeck"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Peck") ??
                AddAttack("Beak Peck", BuiltInCombatMoveType.NaturalWeaponAttack,
                    MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
                    Alignment.Front, Orientation.High, 2.5, 0.7, beakShape, peckDamage,
                    $"@ dart|darts forward and peck|pecks sharply at $1 with &0's {{0}}{attackAddendum}",
                    DamageType.Piercing);
            _attacks["talonstrike"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Talon Strike") ??
                AddAttack("Talon Strike", BuiltInCombatMoveType.NaturalWeaponAttack,
                    MeleeWeaponVerb.Claw, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                    Alignment.FrontRight, Orientation.Low, 3.5, 0.9, talonShape, talonDamage,
                    $"@ slash|slashes at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
            _attacks["taloncarry"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Talon Carry") ??
                AddAttack("Talon Carry", BuiltInCombatMoveType.ForcedMovementUnarmed,
                    MeleeWeaponVerb.Claw, Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
                    Alignment.FrontRight, Orientation.Centre, 5.0, 1.1, talonShape, talonDamage,
                    $"@ hook|hooks $1 with &0's {{0}} and beat|beats upward{attackAddendum}", DamageType.Claw,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                    additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.Layer,
                        ForcedMovementVerbs.Pull, ForcedMovementRange.Grapple));
            _attacks["fangbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Fang Bite") ??
                AddAttack("Fang Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.Low, 3.0, 0.7, fangShape, _snakeBiteDamage,
                    $"@ dart|darts in and try|tries to bite $1 with &0's {{0}}{attackAddendum}", DamageType.Bite);
            _attacks["bite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bite") ??
                AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.VeryHard, Difficulty.Normal, Difficulty.ExtremelyHard,
                    Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0, 1.4, mouthShape, ramDamage,
                    $"@ lean|leans in and try|tries to bite $1{attackAddendum}", DamageType.Bite);
            _attacks["headbutt"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Headbutt") ??
                AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch,
                    MeleeWeaponVerb.Strike, Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.ExtremelyHard,
                    Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 5.0, 1.0, headShape, ramDamage,
                    $"@ jerk|jerks forward and crack|cracks &0's head into $1{attackAddendum}", DamageType.Crushing,
                    additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["mandiblebite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Mandible Bite") ??
                AddAttack("Mandible Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
                    Alignment.Front, Orientation.Centre, 1.5, 0.4, mandibleShape, mandibleDamage,
                    $"@ snap|snaps &0's {{0}} at $1{attackAddendum}", DamageType.Shearing);
            _attacks["beakbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Bite") ??
                AddAttack("Beak Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy,
                    Alignment.Front, Orientation.High, 2.5, 0.6, beakShape, peckDamage,
                    $"@ dart|darts in close and jab|jabs &0's beak into $1{attackAddendum}", DamageType.Piercing);
            _attacks["arachnidclaw"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Arachnid Claw") ??
                AddAttack("Arachnid Claw", BuiltInCombatMoveType.NaturalWeaponAttack,
                    MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                    Alignment.FrontRight, Orientation.Centre, 2.5, 0.8, clawShape, clawDamage,
                    $"@ lash|lashes out with &0's {{0}} at $1{attackAddendum}", DamageType.Claw);
            _attacks["treehaul"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tree Haul") ??
                AddAttack("Tree Haul", BuiltInCombatMoveType.ForcedMovementUnarmed,
                    MeleeWeaponVerb.Claw, Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
                    Alignment.FrontRight, Orientation.Centre, 5.0, 1.2, clawShape, clawDamage,
                    $"@ seize|seizes $1 with &0's {{0}} and haul|hauls &1 upward{attackAddendum}", DamageType.Claw,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                    additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.Layer,
                        ForcedMovementVerbs.Pull, ForcedMovementRange.Melee));
            _attacks["clawclamp"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Claw Clamp") ??
                AddAttack("Claw Clamp", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Claw, Difficulty.Easy, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.Easy,
                    Alignment.FrontRight, Orientation.Centre, 2.5, 0.7, clawShape, mandibleDamage,
                    $"@ clamp|clamps &0's {{0}} onto $1{attackAddendum}", DamageType.Shearing);
            _attacks["headram"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Head Ram") ??
                AddAttack("Head Ram", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                    MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                    Alignment.Front, Orientation.High, 5.0, 1.1, headShape, ramDamage,
                    $"@ surge|surges forward and slam|slams &0's head into $1{attackAddendum}", DamageType.Crushing,
                    additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["bargepushback"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Animal Barge Pushback") ??
                AddAttack("Animal Barge Pushback", BuiltInCombatMoveType.PushbackUnarmed,
                    MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                    Alignment.Front, Orientation.Centre, 7.0, 1.4, shoulderShape, smashDamage,
                    $"@ drive|drives &0's bulk into $1 and force|forces &1 back{attackAddendum}", DamageType.Crushing,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                    additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["tailslap"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tail Slap") ??
                AddAttack("Tail Slap", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                    MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Rear, Orientation.Centre, 4.5, 1.1, tailShape, ramDamage,
                    $"@ whip|whips &0's tail around at $1{attackAddendum}", DamageType.Crushing,
                    additionalInfo: ((int)Difficulty.Normal).ToString());
            _attacks["tendrillash"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tendril Lash") ??
                AddAttack("Tendril Lash", BuiltInCombatMoveType.NaturalWeaponAttack,
                    MeleeWeaponVerb.Sweep, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
                    Alignment.Front, Orientation.Centre, 3.5, 0.5, tendrilShape, peckDamage,
                    $"@ lash|lashes a tendril at $1{attackAddendum}", DamageType.Cellular);
            _attacks["waterdrag"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Water Drag") ??
                AddAttack("Water Drag", BuiltInCombatMoveType.ForcedMovementUnarmed,
                    MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.Centre, 5.0, 1.0, mouthShape, sharkBite,
                    $"@ clamp|clamps onto $1 and wrench|wrenches &1 toward the water{attackAddendum}", DamageType.Bite,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                    additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.All,
                        ForcedMovementVerbs.Pull, ForcedMovementRange.Melee));
        }
        else
        {
            TraitExpression smallBite = new()
            {
                Name = "Small Animal Bite Damage",
                Expression = damageExpressions["Small Animal Bite Damage"]
            };
            _context.TraitExpressions.Add(smallBite);
            _context.SaveChanges();

            TraitExpression fishBite = new()
            {
                Name = "Fish Bite Damage",
                Expression = damageExpressions["Fish Bite Damage"]
            };
            _context.TraitExpressions.Add(fishBite);
            _context.SaveChanges();

            TraitExpression herbivoreBite = new()
            {
                Name = "Herbivorous Animal Bite Damage",
                Expression = damageExpressions["Herbivorous Animal Bite Damage"]
            };
            _context.TraitExpressions.Add(herbivoreBite);
            _context.SaveChanges();

            TraitExpression carnivoreBite = new()
            {
                Name = "Carnivorous Animal Bite Damage",
                Expression = damageExpressions["Carnivorous Animal Bite Damage"]
            };
            _context.TraitExpressions.Add(carnivoreBite);
            _context.SaveChanges();

            TraitExpression sharkBite = new()
            {
                Name = "Shark Bite Damage",
                Expression = damageExpressions["Shark Bite Damage"]
            };
            _context.TraitExpressions.Add(sharkBite);
            _context.SaveChanges();

            TraitExpression clawDamage = new()
            {
                Name = "Animal Claw Damage",
                Expression = damageExpressions["Animal Claw Damage"]
            };
            _context.TraitExpressions.Add(clawDamage);
            _context.SaveChanges();

            TraitExpression peckDamage = new()
            {
                Name = "Animal Peck Damage",
                Expression = damageExpressions["Animal Peck Damage"]
            };
            _context.TraitExpressions.Add(peckDamage);
            _context.SaveChanges();

            TraitExpression talonDamage = new()
            {
                Name = "Animal Talon Damage",
                Expression = damageExpressions["Animal Talon Damage"]
            };
            _context.TraitExpressions.Add(talonDamage);
            _context.SaveChanges();

            TraitExpression mandibleDamage = new()
            {
                Name = "Animal Mandible Damage",
                Expression = damageExpressions["Animal Mandible Damage"]
            };
            _context.TraitExpressions.Add(mandibleDamage);
            _context.SaveChanges();

            TraitExpression ramDamage = new()
            {
                Name = "Animal Ram Damage",
                Expression = damageExpressions["Animal Ram Damage"]
            };
            _context.TraitExpressions.Add(ramDamage);
            _context.SaveChanges();

            TraitExpression smashDamage = new()
            {
                Name = "Animal Smash Damage",
                Expression = damageExpressions["Animal Smash Damage"]
            };
            _context.TraitExpressions.Add(smashDamage);
            _context.SaveChanges();

            TraitExpression dragonfireDamage = new()
            {
                Name = "Dragonfire Breath Damage",
                Expression = damageExpressions["Dragonfire Breath Damage"]
            };
            _context.TraitExpressions.Add(dragonfireDamage);
            _context.SaveChanges();

            TraitExpression coupDamage = new()
            {
                Name = "Animal Coup De Grace Damage",
                Expression = damageExpressions["Animal Coup De Grace Damage"]
            };
            _context.TraitExpressions.Add(coupDamage);
            _context.SaveChanges();

            _snakeBiteDamage = new TraitExpression
            {
                Name = "Snake Bite Damage",
                Expression = damageExpressions["Snake Bite Damage"]
            };
            _context.TraitExpressions.Add(_snakeBiteDamage);
            _context.SaveChanges();

            BodypartShape mouthshape = _context.BodypartShapes.First(x => x.Name == "Mouth");
            BodypartShape clawShape = _context.BodypartShapes.First(x => x.Name == "Claw");
            BodypartShape hoofShape = _context.BodypartShapes.First(x => x.Name == "Hoof");
            BodypartShape shoulderShape = _context.BodypartShapes.First(x => x.Name == "Shoulder");
            BodypartShape antlerShape = _context.BodypartShapes.First(x => x.Name == "Antler");
            BodypartShape tuskShape = _context.BodypartShapes.First(x => x.Name == "Tusk");
            BodypartShape hornShape = _context.BodypartShapes.First(x => x.Name == "Horn");
            BodypartShape beakShape = _context.BodypartShapes.First(x => x.Name == "Beak");
            BodypartShape talonShape = _context.BodypartShapes.First(x => x.Name == "Talon");
            BodypartShape fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
            BodypartShape mandibleShape = _context.BodypartShapes.First(x => x.Name == "Mandible");
            BodypartShape headShape = _context.BodypartShapes.First(x => x.Name == "Head");
            BodypartShape tailShape = _context.BodypartShapes.First(x => x.Name == "Tail");
            BodypartShape tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");

            string attackAddendum =
                CombatSeederMessageStyleHelper.AttackSuffix(
                    CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));
            Liquid defaultWater = _context.Liquids.FirstOrDefault(x => x.Name == "water") ?? _freshWaters.Last();
            Liquid? animalSpittle = _context.Liquids.FirstOrDefault(x => x.Name == "animal spittle");
            if (animalSpittle is null)
            {
                animalSpittle = new Liquid
                {
                    Name = "animal spittle",
                    Description = "spittle",
                    LongDescription = "a cloudy, stringy animal spittle",
                    TasteText = "It tastes rank and faintly salty.",
                    VagueTasteText = "It tastes rank and faintly salty.",
                    SmellText = "It smells of musk and stale saliva.",
                    VagueSmellText = "It smells faintly musky.",
                    TasteIntensity = 60,
                    SmellIntensity = 30,
                    AlcoholLitresPerLitre = 0,
                    WaterLitresPerLitre = 0.98,
                    FoodSatiatedHoursPerLitre = 0,
                    DrinkSatiatedHoursPerLitre = 0,
                    Viscosity = 1.2,
                    Density = 1.0,
                    Organic = true,
                    ThermalConductivity = 0.609,
                    ElectricalConductivity = 0.005,
                    SpecificHeatCapacity = 4181,
                    FreezingPoint = -2,
                    BoilingPoint = 100,
                    DisplayColour = "bold green",
                    DampDescription = "It is slimed with spittle",
                    WetDescription = "It is wet with spittle",
                    DrenchedDescription = "It is drenched in spittle",
                    DampShortDescription = "(slimed)",
                    WetShortDescription = "(spat on)",
                    DrenchedShortDescription = "(drenched in spit)",
                    SolventId = defaultWater.Id,
                    CountAsId = defaultWater.Id,
                    SolventVolumeRatio = 1,
                    InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
                    ResidueVolumePercentage = 0.05
                };
                _context.Liquids.Add(animalSpittle);
                _context.SaveChanges();
            }

            Liquid? animalAcid = _context.Liquids.FirstOrDefault(x => x.Name == "animal acid");
            if (animalAcid is null)
            {
                animalAcid = new Liquid
                {
                    Name = "animal acid",
                    Description = "acid",
                    LongDescription = "a smoking, yellow-green acidic slurry",
                    TasteText = "It is searingly sour and painfully caustic.",
                    VagueTasteText = "It tastes painfully caustic.",
                    SmellText = "It smells sharp, mineral and corrosive enough to sting the sinuses.",
                    VagueSmellText = "It smells harsh and corrosive.",
                    TasteIntensity = 120,
                    SmellIntensity = 80,
                    AlcoholLitresPerLitre = 0,
                    WaterLitresPerLitre = 0.7,
                    FoodSatiatedHoursPerLitre = 0,
                    DrinkSatiatedHoursPerLitre = 0,
                    Viscosity = 1.1,
                    Density = 1.05,
                    Organic = true,
                    ThermalConductivity = 0.609,
                    ElectricalConductivity = 0.005,
                    SpecificHeatCapacity = 4181,
                    FreezingPoint = -8,
                    BoilingPoint = 108,
                    DisplayColour = "bold yellow",
                    DampDescription = "It is hissing with acid",
                    WetDescription = "It is slick with burning acid",
                    DrenchedDescription = "It is drenched in corrosive acid",
                    DampShortDescription = "(acid-splashed)",
                    WetShortDescription = "(acid-burned)",
                    DrenchedShortDescription = "(acid-drenched)",
                    SolventId = defaultWater.Id,
                    CountAsId = defaultWater.Id,
                    SolventVolumeRatio = 1,
                    InjectionConsequence = (int)LiquidInjectionConsequence.Deadly,
                    ResidueVolumePercentage = 0.05
                };
                _context.Liquids.Add(animalAcid);
                _context.SaveChanges();
            }

            string RangedAttackData(int rangeInRooms, RangedScatterType scatterType)
            {
                return new XElement("Data",
                    new XElement("RangeInRooms", rangeInRooms),
                    new XElement("ScatterType", scatterType.ToString())
                ).ToString();
            }

            string SpitAttackData(int rangeInRooms, RangedScatterType scatterType, long liquidId, double maximumQuantity)
            {
                return new XElement("Data",
                    new XElement("RangeInRooms", rangeInRooms),
                    new XElement("ScatterType", scatterType.ToString()),
                    new XElement("Liquid", liquidId),
                    new XElement("MaximumQuantity", maximumQuantity)
                ).ToString();
            }

            string BreathAttackData(int rangeInRooms, RangedScatterType scatterType, int additionalTargets,
                int bodypartsPerTarget, double igniteChance, string fireName, double damagePerTick, double painPerTick,
                double stunPerTick, double thermalLoadPerTick, double spreadChance, double minimumOxidation,
                bool selfOxidising, long extinguishTagId)
            {
                return new XElement("Data",
                    new XElement("RangeInRooms", rangeInRooms),
                    new XElement("ScatterType", scatterType.ToString()),
                    new XElement("AdditionalTargetLimit", additionalTargets),
                    new XElement("BodypartsHitPerTarget", bodypartsPerTarget),
                    new XElement("IgniteChance", igniteChance),
                    new XElement("FireProfile",
                        new XElement("Name", new XCData(fireName)),
                        new XElement("DamageType", (int)DamageType.Burning),
                        new XElement("DamagePerTick", damagePerTick),
                        new XElement("PainPerTick", painPerTick),
                        new XElement("StunPerTick", stunPerTick),
                        new XElement("ThermalLoadPerTick", thermalLoadPerTick),
                        new XElement("SpreadChance", spreadChance),
                        new XElement("MinimumOxidation", minimumOxidation),
                        new XElement("SelfOxidising", selfOxidising),
                        new XElement("TickFrequencySeconds", 10),
                        new XElement("ExtinguishTags", new XElement("Tag", extinguishTagId))))
                .ToString();
            }

            string ExplosiveAttackData(int rangeInRooms, RangedScatterType scatterType, SizeCategory explosionSize,
                Proximity maximumProximity)
            {
                return new XElement("Data",
                    new XElement("RangeInRooms", rangeInRooms),
                    new XElement("ScatterType", scatterType.ToString()),
                    new XElement("ExplosionSize", explosionSize.ToString()),
                    new XElement("MaximumProximity", maximumProximity.ToString())
                ).ToString();
            }

            string BuffetingAttackData(int rangeInRooms, RangedScatterType scatterType, int maximumPushDistance,
                double offensiveAdvantagePerDegree, double defensiveAdvantagePerDegree, bool inflictsDamage)
            {
                return new XElement("Data",
                    new XElement("RangeInRooms", rangeInRooms),
                    new XElement("ScatterType", scatterType.ToString()),
                    new XElement("MaximumPushDistance", maximumPushDistance),
                    new XElement("OffensiveAdvantagePerDegree", offensiveAdvantagePerDegree),
                    new XElement("DefensiveAdvantagePerDegree", defensiveAdvantagePerDegree),
                    new XElement("InflictsDamage", inflictsDamage)
                ).ToString();
            }

            string ForcedMovementAttackData(Difficulty resist, ForcedMovementTypes types,
                ForcedMovementVerbs verbs, ForcedMovementRange range)
            {
                return new XElement("Data",
                    new XElement("Resist", resist.ToString()),
                    new XElement("Types", types.ToString()),
                    new XElement("Verbs", verbs.ToString()),
                    new XElement("Range", range.ToString())
                ).ToString();
            }

            _attacks["carnivorebite"] = AddAttack("Carnivore Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, carnivoreBite,
                $"@ snap|snaps &0's jaws at $1{attackAddendum}", DamageType.Bite);
            _attacks["carnivoresmashbite"] = AddAttack("Carnivore Smash Bite",
                BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
                Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape,
                carnivoreBite, $"@ lunge|lunges in and clamp|clamps &0's jaws onto $1{attackAddendum}", DamageType.Bite);
            _attacks["carnivorelowbite"] = AddAttack("Carnivore Low Bite",
                BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
                Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, mouthshape,
                carnivoreBite, $"@ dart|darts low and snap|snaps at $1's legs{attackAddendum}",
                DamageType.Bite);
            _attacks["carnivorelowestbite"] = AddAttack("Carnivore Lowest Bite",
                BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Hard, Difficulty.Hard,
                Difficulty.VeryEasy, Difficulty.Easy, Alignment.Front, Orientation.Lowest, 4.0, 0.9, mouthshape,
                carnivoreBite,
                $"@ surge|surges in ankle-low and snap|snaps at $1's feet in an attempt to drag &1 down{attackAddendum}",
                DamageType.Bite);
            _attacks["carnivorehighbite"] = AddAttack("Carnivore High Bite",
                BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Bite, Difficulty.Normal,
                Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.Front, Orientation.High, 6.0, 1.3,
                mouthshape, carnivoreBite,
                $"@ spring|springs up at $1 and snap|snaps for &1's upper body{attackAddendum}",
                DamageType.Bite, additionalInfo: "7");
            _attacks["carnivoreclinchbite"] = AddAttack("Carnivore Clinch Bite",
                BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 0.6,
                mouthshape, carnivoreBite, $"@ wrench|wrenches &0's head sideways and try|tries to savage $1 with a close bite{attackAddendum}", DamageType.Bite);
            _attacks["carnivoreclinchhighbite"] = AddAttack("Carnivore High Clinch Bite",
                BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 3.0, 0.6,
                mouthshape, carnivoreBite, $"@ crane|cranes &0's head up and snap|snaps for $1's throat{attackAddendum}", DamageType.Bite);
            _attacks["carnivoreclinchhighestbite"] = AddAttack("Carnivore Highest Clinch Bite",
                BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Highest, 3.0, 0.6,
                mouthshape, carnivoreBite, $"@ surge|surges up and snap|snaps for $1's face{attackAddendum}", DamageType.Bite);
            _attacks["carnivoredownbite"] = AddAttack("Carnivore Downed Bite",
                BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
                mouthshape, carnivoreBite,
                $"@ savage|savages $1 with a bite while #1 %1|are|is down{attackAddendum}", DamageType.Bite,
                additionalInfo: ((int)Difficulty.ExtremelyEasy).ToString());

            _attacks["herbivorebite"] = AddAttack("Herbivore Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape, herbivoreBite,
                $"@ nip|nips at $1{attackAddendum}", DamageType.Bite);
            _attacks["herbivoresmashbite"] = AddAttack("Herbivore Smash Bite",
                BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard,
                Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape,
                herbivoreBite, $"@ lunge|lunges in and bite|bites at $1{attackAddendum}", DamageType.Bite);

            _attacks["smallbite"] = AddAttack("Small Animal Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, smallBite,
                $"@ scamper|scampers in and nip|nips at $1{attackAddendum}", DamageType.Bite);
            _attacks["smallsmashbite"] = AddAttack("Small Animal Smash Bite",
                BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Low, 2.0, 0.5,
                mouthshape, smallBite, $"@ dart|darts in and bite|bites at $1{attackAddendum}", DamageType.Bite);
            _attacks["smalllowbite"] = AddAttack("Small Animal Low Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.Lowest, 2.0, 0.5, mouthshape, smallBite,
                $"@ scamper|scampers low and snap|snaps at $1's ankles{attackAddendum}", DamageType.Bite);
            _attacks["smalldownedbite"] = AddAttack("Small Animal Downed Bite",
                BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
                Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
                mouthshape, smallBite, $"@ worry|worries at $1 with quick bites while #1 %1|are|is down{attackAddendum}",
                DamageType.Bite, additionalInfo: ((int)Difficulty.Trivial).ToString());

            _attacks["clawswipe"] = AddAttack("Claw Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
                $"@ rear|rears up and rake|rakes &0's {{0}} across $1{attackAddendum}", DamageType.Claw);
            _attacks["clawsmashswipe"] = AddAttack("Claw Swipe Smash", BuiltInCombatMoveType.UnarmedSmashItem,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
                $"@ rear|rears up and rake|rakes &0's {{0}} hard across $1{attackAddendum}", DamageType.Claw);
            _attacks["clawhighswipe"] = AddAttack("Claw High Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.Highest, 4.0, 1.3, clawShape, clawDamage,
                $"@ rear|rears up and slash|slashes high at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
            _attacks["clawlowswipe"] = AddAttack("Claw Low Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.Low, 4.0, 1.3, clawShape, clawDamage,
                $"@ crouch|crouches low and rake|rakes at $1's legs with &0's {{0}}{attackAddendum}", DamageType.Claw);
            _attacks["treehaul"] = AddAttack("Tree Haul", BuiltInCombatMoveType.ForcedMovementUnarmed,
                MeleeWeaponVerb.Claw, Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
                Alignment.FrontRight, Orientation.Centre, 5.0, 1.2, clawShape, clawDamage,
                $"@ seize|seizes $1 with &0's {{0}} and haul|hauls &1 upward{attackAddendum}", DamageType.Claw,
                intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.Layer,
                    ForcedMovementVerbs.Pull, ForcedMovementRange.Melee));

            _attacks["hoofstomp"] = AddAttack("Hoof Stomp", BuiltInCombatMoveType.DownedAttackUnarmed,
                MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
                $"@ rear|rears and stamp|stamps at $1 while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["hoofstompsmash"] = AddAttack("Hoof Stomp Smash", BuiltInCombatMoveType.UnarmedSmashItem,
                MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
                $"@ hammer|hammers at $1 with stomping blows while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["barge"] = AddAttack("Animal Barge", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                Alignment.Front, Orientation.Centre, 8.0, 1.8, shoulderShape, smashDamage,
                $"@ charge|charges forward and throw|throws &0's bulk at $1{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.VeryHard).ToString());
            _attacks["bargepushback"] = AddAttack("Animal Barge Pushback", BuiltInCombatMoveType.PushbackUnarmed,
                MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                Alignment.Front, Orientation.Centre, 7.0, 1.4, shoulderShape, smashDamage,
                $"@ drive|drives &0's bulk into $1 and force|forces &1 back{attackAddendum}", DamageType.Crushing,
                intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["bargesmash"] = AddAttack("Animal Barge Smash", BuiltInCombatMoveType.UnarmedSmashItem,
                MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                Alignment.Front, Orientation.Centre, 8.0, 1.8, shoulderShape, smashDamage,
                $"@ charge|charges forward and throw|throws &0's bulk at $1{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.VeryHard).ToString());
            _attacks["clinchbarge"] = AddAttack("Animal Clinch Barge", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                Alignment.Front, Orientation.Centre, 8.0, 1.2, shoulderShape, smashDamage,
                $"@ thrash|thrashes and slam|slams &0's weight into $1{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.Hard).ToString());

            _attacks["gorehorn"] = AddAttack("Horn Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
                Alignment.Front, Orientation.High, 5.0, 1.3, hornShape, smashDamage,
                $"@ lower|lowers &0's head and drive|drives &0's horns toward $1{attackAddendum}",
                DamageType.Piercing);
            _attacks["goreantler"] = AddAttack("Antler Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
                Alignment.Front, Orientation.High, 5.0, 1.3, antlerShape, smashDamage,
                $"@ dip|dips &0's antlers and hook|hooks them toward $1{attackAddendum}",
                DamageType.Piercing);
            _attacks["goretusk"] = AddAttack("Tusk Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
                Alignment.Front, Orientation.High, 5.0, 1.3, tuskShape, smashDamage,
                $"@ surge|surges forward and drive|drives &0's tusks at $1{attackAddendum}",
                DamageType.Piercing);
            _attacks["tusksweep"] = AddAttack("Tusk Sweep", BuiltInCombatMoveType.UnbalancingBlowUnarmed,
                MeleeWeaponVerb.Sweep, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard,
                Alignment.FrontRight, Orientation.Low, 5.0, 1.5, tuskShape, smashDamage,
                $"@ sweep|sweeps &0's tusks low across $1 in an attempt to topple &1{attackAddendum}",
                DamageType.Crushing, additionalInfo: ((int)Difficulty.Hard).ToString());

            _attacks["fishbite"] = AddAttack("Fish Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, fishBite,
                $"@ dart|darts in and snap|snaps at $1{attackAddendum}", DamageType.Bite);
            _attacks["fishquickbite"] = AddAttack("Fish Quick Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Low, 2.0, 0.3, mouthshape, fishBite,
                $"@ flash|flashes in and nip|nips at $1{attackAddendum}", DamageType.Bite);
            _attacks["bite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bite") ??
                AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.VeryHard, Difficulty.Normal, Difficulty.ExtremelyHard,
                    Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0, 1.4, mouthshape, carnivoreBite,
                    $"@ lean|leans in and try|tries to bite $1{attackAddendum}", DamageType.Bite);
            _attacks["sharkbite"] = AddAttack("Shark Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, sharkBite,
                $"@ surge|surges in and bite|bites down on $1{attackAddendum}", DamageType.Bite);
            _attacks["sharkreelbite"] = AddAttack("Shark Reel Bite", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Centre, 5.0, 1.2, mouthshape, sharkBite,
                $"@ surge|surges in, bite|bites down on $1, and wrench|wrenches away{attackAddendum}", DamageType.Bite,
                additionalInfo: ((int)Difficulty.VeryHard).ToString());
            _attacks["waterdrag"] = AddAttack("Water Drag", BuiltInCombatMoveType.ForcedMovementUnarmed,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Centre, 5.0, 1.0, mouthshape, sharkBite,
                $"@ clamp|clamps onto $1 and wrench|wrenches &1 toward the water{attackAddendum}", DamageType.Bite,
                intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.All,
                    ForcedMovementVerbs.Pull, ForcedMovementRange.Melee));

            _attacks["crabpinch"] = AddAttack("Crab Pinch", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.Low, 4.0, 1.3, clawShape, clawDamage,
                $"@ snap|snaps &0's {{0}} shut on $1{attackAddendum}", DamageType.Shearing);
            _attacks["beakpeck"] = AddAttack("Beak Peck", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
                Alignment.Front, Orientation.High, 2.5, 0.7, beakShape, peckDamage,
                $"@ dart|darts forward and peck|pecks sharply at $1 with &0's {{0}}{attackAddendum}", DamageType.Piercing);
            _attacks["beakbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Bite") ??
                AddAttack("Beak Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy,
                    Alignment.Front, Orientation.High, 2.5, 0.6, beakShape, peckDamage,
                    $"@ dart|darts in close and jab|jabs &0's beak into $1{attackAddendum}", DamageType.Piercing);
            _attacks["talonstrike"] = AddAttack("Talon Strike", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Claw, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.Low, 3.5, 0.9, talonShape, talonDamage,
                $"@ rake|rakes at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
            _attacks["taloncarry"] = AddAttack("Talon Carry", BuiltInCombatMoveType.ForcedMovementUnarmed,
                MeleeWeaponVerb.Claw, Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
                Alignment.FrontRight, Orientation.Centre, 5.0, 1.1, talonShape, talonDamage,
                $"@ hook|hooks $1 with &0's {{0}} and beat|beats upward{attackAddendum}", DamageType.Claw,
                intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                additionalInfo: ForcedMovementAttackData(Difficulty.Hard, ForcedMovementTypes.Layer,
                    ForcedMovementVerbs.Pull, ForcedMovementRange.Grapple));
            _attacks["headbutt"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Headbutt") ??
                AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch,
                    MeleeWeaponVerb.Strike, Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.ExtremelyHard,
                    Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 5.0, 1.0, headShape, ramDamage,
                    $"@ jerk|jerks forward and crack|cracks &0's head into $1{attackAddendum}", DamageType.Crushing,
                    additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["fangbite"] = AddAttack("Fang Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                Alignment.Front, Orientation.Low, 3.0, 0.7, fangShape, _snakeBiteDamage,
                $"@ dart|darts in and sink|sinks &0's {{0}} into $1{attackAddendum}", DamageType.Bite);
            _attacks["mandiblebite"] = AddAttack("Mandible Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
                MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
                Alignment.Front, Orientation.Centre, 1.5, 0.4, mandibleShape, mandibleDamage,
                $"@ clamp|clamps &0's {{0}} on $1{attackAddendum}", DamageType.Shearing);
            _attacks["clawclamp"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Claw Clamp") ??
                AddAttack("Claw Clamp", BuiltInCombatMoveType.ClinchUnarmedAttack,
                    MeleeWeaponVerb.Claw, Difficulty.Easy, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.Easy,
                    Alignment.FrontRight, Orientation.Centre, 2.5, 0.7, clawShape, mandibleDamage,
                    $"@ clamp|clamps &0's {{0}} onto $1{attackAddendum}", DamageType.Shearing);
            _attacks["arachnidclaw"] = AddAttack("Arachnid Claw", BuiltInCombatMoveType.NaturalWeaponAttack,
                MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
                Alignment.FrontRight, Orientation.Centre, 2.5, 0.8, clawShape, clawDamage,
                $"@ rake|rakes $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
            _attacks["headram"] = AddAttack("Head Ram", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
                Alignment.Front, Orientation.High, 5.0, 1.1, headShape, ramDamage,
                $"@ lunge|lunges in and slam|slams &0's head into $1{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.Hard).ToString());
            _attacks["tailslap"] = AddAttack("Tail Slap", BuiltInCombatMoveType.StaggeringBlowUnarmed,
                MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Hard,
                Alignment.Rear, Orientation.Centre, 4.5, 1.1, tailShape, ramDamage,
                $"@ whip|whips &0's tail across $1{attackAddendum}", DamageType.Crushing,
                additionalInfo: ((int)Difficulty.Normal).ToString());
            _attacks["tendrillash"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tendril Lash") ??
                AddAttack("Tendril Lash", BuiltInCombatMoveType.NaturalWeaponAttack,
                    MeleeWeaponVerb.Sweep, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
                    Alignment.Front, Orientation.Centre, 3.5, 0.5, tendrilShape, peckDamage,
                    $"@ lash|lashes a tendril at $1{attackAddendum}", DamageType.Cellular);
            _attacks["llamaspit"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Llama Spit") ??
                AddAttack("Llama Spit", BuiltInCombatMoveType.SpitNaturalAttack,
                    MeleeWeaponVerb.Blast, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.High, 2.0, 0.9, mouthshape, peckDamage,
                    $"@ rear|rears back and spit|spits a foul gobbet at $1{attackAddendum}", DamageType.Chemical,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
                    additionalInfo: SpitAttackData(1, RangedScatterType.Arcing, animalSpittle.Id, 0.025));
            _attacks["acidspit"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Acid Spit") ??
                AddAttack("Acid Spit", BuiltInCombatMoveType.SpitNaturalAttack,
                    MeleeWeaponVerb.Blast, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.High, 3.0, 1.0, mandibleShape, mandibleDamage,
                    $"@ rear|rears up and spit|spits a hissing jet of acid at $1{attackAddendum}", DamageType.Chemical,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Disadvantage,
                    additionalInfo: SpitAttackData(2, RangedScatterType.Arcing, animalAcid.Id, 0.04));
            _attacks["dragonfirebreath"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Dragonfire Breath") ??
                AddAttack("Dragonfire Breath", BuiltInCombatMoveType.BreathWeaponAttack,
                    MeleeWeaponVerb.Blast, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.High, 10.0, 1.6, mouthshape, dragonfireDamage,
                    $"@ rear|rears up and unleash|unleashes a roaring cone of dragonfire at $1{attackAddendum}", DamageType.Burning,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Burning | CombatMoveIntentions.Hard | CombatMoveIntentions.Slow,
                    additionalInfo: BreathAttackData(2, RangedScatterType.Light, 3, 2, 0.35, "Dragonfire", 0.45, 0.35, 0.1, 2.5, 0.2, 0.05, true,
						_context.Tags.First(x => x.Name == "Water").Id));
            _attacks["wingbuffet"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Wing Buffet") ??
                AddAttack("Wing Buffet", BuiltInCombatMoveType.BuffetingNaturalAttack,
                    MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Front, Orientation.Centre, 5.0, 1.2, shoulderShape, ramDamage,
                    $"@ beat|beats &0's wings and buffet|buffets $1 with a crashing gust{attackAddendum}", DamageType.Crushing,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Advantage | CombatMoveIntentions.Disadvantage,
                    additionalInfo: BuffetingAttackData(1, RangedScatterType.Light, 1, 0.1, 0.15, true));
            _attacks["tailspike"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tail Spike") ??
                AddAttack("Tail Spike", BuiltInCombatMoveType.RangedNaturalAttack,
                    MeleeWeaponVerb.Stab, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Rear, Orientation.Centre, 3.0, 0.8, tailShape, peckDamage,
                    $"@ snap|snaps &0's tail and launch|launches a wicked spike at $1{attackAddendum}", DamageType.Piercing,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast,
                    additionalInfo: RangedAttackData(2, RangedScatterType.Ballistic));
            _attacks["bombardierspray"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bombardier Spray") ??
                AddAttack("Bombardier Spray", BuiltInCombatMoveType.ExplosiveNaturalAttack,
                    MeleeWeaponVerb.Blast, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
                    Alignment.Rear, Orientation.Centre, 2.0, 0.8, mandibleShape, peckDamage,
                    $"@ vent|vents a crackling chemical burst toward $1{attackAddendum}", DamageType.Burning,
                    intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Burning | CombatMoveIntentions.Fast,
                    additionalInfo: ExplosiveAttackData(1, RangedScatterType.Arcing, SizeCategory.VerySmall, Proximity.Immediate));
        }

		EnsureNaturalRangedAttackSeedData(damageExpressions);
    }

	private void EnsureNaturalRangedAttackSeedData(IReadOnlyDictionary<string, string> damageExpressions)
	{
		var waterTag = _context.Tags.First(x => x.Name == "Water");
		var animalSkinTag = _context.Tags.First(x => x.Name == "Animal Skin");
		var defaultWater = _context.Liquids.FirstOrDefault(x => x.Name == "water") ?? _freshWaters.Last();
		var animalSpittle = _context.Liquids.FirstOrDefault(x => x.Name == "animal spittle");
		if (animalSpittle is null)
		{
			animalSpittle = CreateNaturalRangedLiquid("animal spittle", "spittle",
				"a cloudy, stringy animal spittle", "It tastes rank and faintly salty.",
				"It smells of musk and stale saliva.", "bold green", "slimed with spittle", "spat on",
				defaultWater, LiquidInjectionConsequence.Harmful);
		}

		var animalAcid = _context.Liquids.FirstOrDefault(x => x.Name == "animal acid");
		if (animalAcid is null)
		{
			animalAcid = CreateNaturalRangedLiquid("animal acid", "acid",
				"a smoking, yellow-green acidic slurry", "It is searingly sour and painfully caustic.",
				"It smells sharp, mineral and corrosive enough to sting the sinuses.", "bold yellow",
				"hissing with acid", "acid-burned", defaultWater, LiquidInjectionConsequence.Deadly);
		}

		bool hasSurfaceReaction;
		try
		{
			hasSurfaceReaction = !string.IsNullOrWhiteSpace(animalAcid.SurfaceReactionInfo) &&
				XElement.Parse(animalAcid.SurfaceReactionInfo).Elements("Reaction").Any();
		}
		catch (XmlException)
		{
			hasSurfaceReaction = false;
		}

		if (!hasSurfaceReaction)
		{
			animalAcid.SurfaceReactionInfo = new XElement("Reactions",
				new XElement("Reaction",
					new XAttribute("DamageType", (int)DamageType.Chemical),
					new XAttribute("DamagePerTick", 125.0),
					new XAttribute("PainPerTick", 175.0),
					new XAttribute("StunPerTick", 0.0),
					new XElement("Tags", new XElement("Tag", animalSkinTag.Id))))
				.ToString();
		}

		TraitExpression EnsureExpression(string name)
		{
			var expression = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
			if (expression is not null)
			{
				return expression;
			}

			expression = new TraitExpression { Name = name, Expression = damageExpressions[name] };
			_context.TraitExpressions.Add(expression);
			_context.SaveChanges();
			return expression;
		}

		var peckDamage = EnsureExpression("Animal Peck Damage");
		var mandibleDamage = EnsureExpression("Animal Mandible Damage");
		var ramDamage = EnsureExpression("Animal Ram Damage");
		var dragonfireDamage = EnsureExpression("Dragonfire Breath Damage");
		var mouthShape = _context.BodypartShapes.First(x => x.Name == "Mouth");
		var mandibleShape = _context.BodypartShapes.First(x => x.Name == "Mandible");
		var shoulderShape = _context.BodypartShapes.First(x => x.Name == "Shoulder");
		var tailShape = _context.BodypartShapes.First(x => x.Name == "Tail");
		var attackAddendum = CombatSeederMessageStyleHelper.AttackSuffix(
			CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

		string RangedData(int range, RangedScatterType scatter)
		{
			return new XElement("Data", new XElement("RangeInRooms", range),
				new XElement("ScatterType", scatter.ToString())).ToString();
		}

		string SpitData(int range, RangedScatterType scatter, long liquidId, double maximumQuantity)
		{
			return new XElement("Data", new XElement("RangeInRooms", range),
				new XElement("ScatterType", scatter.ToString()), new XElement("Liquid", liquidId),
				new XElement("MaximumQuantity", maximumQuantity)).ToString();
		}

		string BreathData()
		{
			return new XElement("Data", new XElement("RangeInRooms", 2),
				new XElement("ScatterType", RangedScatterType.Light.ToString()),
				new XElement("AdditionalTargetLimit", 3), new XElement("BodypartsHitPerTarget", 2),
				new XElement("IgniteChance", 0.35),
				new XElement("FireProfile", new XElement("Name", new XCData("Dragonfire")),
					new XElement("DamageType", (int)DamageType.Burning), new XElement("DamagePerTick", 0.45),
					new XElement("PainPerTick", 0.35), new XElement("StunPerTick", 0.1),
					new XElement("ThermalLoadPerTick", 2.5), new XElement("SpreadChance", 0.2),
					new XElement("MinimumOxidation", 0.05), new XElement("SelfOxidising", true),
					new XElement("TickFrequencySeconds", 10),
					new XElement("ExtinguishTags", new XElement("Tag", waterTag.Id)))).ToString();
		}

		string BuffetingData()
		{
			return new XElement("Data", new XElement("RangeInRooms", 1),
				new XElement("ScatterType", RangedScatterType.Light.ToString()),
				new XElement("MaximumPushDistance", 1), new XElement("OffensiveAdvantagePerDegree", 0.1),
				new XElement("DefensiveAdvantagePerDegree", 0.15), new XElement("InflictsDamage", true)).ToString();
		}

		string ExplosiveData()
		{
			return new XElement("Data", new XElement("RangeInRooms", 1),
				new XElement("ScatterType", RangedScatterType.Arcing.ToString()),
				new XElement("ExplosionSize", SizeCategory.VerySmall.ToString()),
				new XElement("MaximumProximity", Proximity.Immediate.ToString())).ToString();
		}

		_attacks["llamaspit"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Llama Spit") ??
			AddAttack("Llama Spit", BuiltInCombatMoveType.SpitNaturalAttack, MeleeWeaponVerb.Blast,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
				Orientation.High, 2.0, 0.9, mouthShape, peckDamage,
				$"@ rear|rears back and spit|spits a foul gobbet at $1{attackAddendum}", DamageType.Chemical,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
				additionalInfo: SpitData(1, RangedScatterType.Arcing, animalSpittle.Id, 0.025));
		_attacks["acidspit"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Acid Spit") ??
			AddAttack("Acid Spit", BuiltInCombatMoveType.SpitNaturalAttack, MeleeWeaponVerb.Blast,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
				Orientation.High, 3.0, 1.0, mandibleShape, mandibleDamage,
				$"@ rear|rears up and spit|spits a hissing jet of acid at $1{attackAddendum}", DamageType.Chemical,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Disadvantage,
				additionalInfo: SpitData(2, RangedScatterType.Arcing, animalAcid.Id, 0.04));
		_attacks["dragonfirebreath"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Dragonfire Breath") ??
			AddAttack("Dragonfire Breath", BuiltInCombatMoveType.BreathWeaponAttack, MeleeWeaponVerb.Blast,
				Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
				Orientation.High, 10.0, 1.6, mouthShape, dragonfireDamage,
				$"@ rear|rears up and unleash|unleashes a roaring cone of dragonfire at $1{attackAddendum}",
				DamageType.Burning,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
				            CombatMoveIntentions.Burning | CombatMoveIntentions.Hard | CombatMoveIntentions.Slow,
				additionalInfo: BreathData());
		_attacks["wingbuffet"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Wing Buffet") ??
			AddAttack("Wing Buffet", BuiltInCombatMoveType.BuffetingNaturalAttack, MeleeWeaponVerb.Sweep,
				Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
				Orientation.Centre, 5.0, 1.2, shoulderShape, ramDamage,
				$"@ beat|beats &0's wings and buffet|buffets $1 with a crashing gust{attackAddendum}",
				DamageType.Crushing,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Advantage |
				            CombatMoveIntentions.Disadvantage, additionalInfo: BuffetingData());
		_attacks["tailspike"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tail Spike") ??
			AddAttack("Tail Spike", BuiltInCombatMoveType.RangedNaturalAttack, MeleeWeaponVerb.Stab,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Rear,
				Orientation.Centre, 3.0, 0.8, tailShape, peckDamage,
				$"@ snap|snaps &0's tail and launch|launches a wicked spike at $1{attackAddendum}",
				DamageType.Piercing,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast,
				additionalInfo: RangedData(2, RangedScatterType.Ballistic));
		_attacks["bombardierspray"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bombardier Spray") ??
			AddAttack("Bombardier Spray", BuiltInCombatMoveType.ExplosiveNaturalAttack, MeleeWeaponVerb.Blast,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Rear,
				Orientation.Centre, 2.0, 0.8, mandibleShape, peckDamage,
				$"@ vent|vents a crackling chemical burst toward $1{attackAddendum}", DamageType.Burning,
				intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
				            CombatMoveIntentions.Burning | CombatMoveIntentions.Fast,
				additionalInfo: ExplosiveData());

		XElement breathXml;
		if (string.IsNullOrWhiteSpace(_attacks["dragonfirebreath"].AdditionalInfo))
		{
			breathXml = XElement.Parse(BreathData());
		}
		else try
		{
			breathXml = XElement.Parse(_attacks["dragonfirebreath"].AdditionalInfo);
		}
		catch (XmlException)
		{
			breathXml = XElement.Parse(BreathData());
		}
		var fireProfile = breathXml.Element("FireProfile");
		if (fireProfile is null)
		{
			fireProfile = XElement.Parse(BreathData()).Element("FireProfile")!;
			breathXml.Add(fireProfile);
		}

		var extinguishTags = fireProfile.Element("ExtinguishTags");
		if (extinguishTags is null)
		{
			extinguishTags = new XElement("ExtinguishTags");
			fireProfile.Add(extinguishTags);
		}

		if (!extinguishTags.Elements("Tag").Any(x => long.TryParse(x.Value, out var id) && id == waterTag.Id))
		{
			extinguishTags.Add(new XElement("Tag", waterTag.Id));
			_attacks["dragonfirebreath"].AdditionalInfo = breathXml.ToString();
		}

		_context.SaveChanges();
	}

	private Liquid CreateNaturalRangedLiquid(string name, string description, string longDescription, string taste,
		string smell, string displayColour, string wetDescription, string wetShortDescription, Liquid defaultWater,
		LiquidInjectionConsequence injectionConsequence)
	{
		var liquid = new Liquid
		{
			Name = name,
			Description = description,
			LongDescription = longDescription,
			TasteText = taste,
			VagueTasteText = taste,
			SmellText = smell,
			VagueSmellText = smell,
			TasteIntensity = 60,
			SmellIntensity = 50,
			WaterLitresPerLitre = 0.8,
			Viscosity = 1.2,
			Density = 1.0,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -2,
			BoilingPoint = 100,
			DisplayColour = displayColour,
			DampDescription = $"It is {wetDescription}",
			WetDescription = $"It is {wetDescription}",
			DrenchedDescription = $"It is drenched in {description}",
			DampShortDescription = $"({wetShortDescription})",
			WetShortDescription = $"({wetShortDescription})",
			DrenchedShortDescription = $"(drenched in {description})",
			SolventId = defaultWater.Id,
			CountAsId = defaultWater.Id,
			SolventVolumeRatio = 1,
			InjectionConsequence = (int)injectionConsequence,
			ResidueVolumePercentage = 0.05
		};
		_context.Liquids.Add(liquid);
		_context.SaveChanges();
		return liquid;
	}

    private void CreateRaceAttacks(Race race)
    {
        if (TryApplyTemplateRaceAttacks(race))
        {
            return;
        }

        switch (race.Name)
        {
            case "Python":
            case "Tree Python":
            case "Boa":
            case "Anaconda":
                AddSerpentAttack(race, false);
                break;
            case "Cobra":
            case "Adder":
            case "Rattlesnake":
            case "Viper":
            case "Mamba":
            case "Coral Snake":
            case "Moccasin":
                AddSerpentAttack(race, true);
                break;
            case "Mouse":
            case "Rat":
            case "Guinea Pig":
            case "Hamster":
            case "Ferret":
            case "Rabbit":
            case "Hare":
                AddAttacks(race, ItemQuality.ExtremelyBad, smallAnimalBites: true);
                break;
            case "Otter":
            case "Fox":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true);
                AddAttacks(race, ItemQuality.Poor, clawAttacks: true);
                break;
            case "Beaver":
                AddAttacks(race, ItemQuality.Substandard, true);
                AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
                break;
            case "Cat":
                AddAttacks(race, ItemQuality.Bad, true);
                AddAttacks(race, ItemQuality.Bad, clawAttacks: true);
                break;
            case "Dog":
                AddAttacks(race, ItemQuality.Poor, true);
                AddAttacks(race, ItemQuality.Bad, clawAttacks: true);
                break;
            case "Wolf":
            case "Coyote":
            case "Hyena":
            case "Jackal":
            case "Cheetah":
            case "Leopard":
            case "Panther":
            case "Jaguar":
                AddAttacks(race, ItemQuality.Standard, true);
                AddAttacks(race, ItemQuality.Substandard, clawAttacks: true);
                break;
            case "Lion":
            case "Tiger":
                AddAttacks(race, ItemQuality.VeryGood, true);
                AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
                break;
            case "Wolverine":
            case "Badger":
                AddAttacks(race, ItemQuality.Poor, true);
                AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
                break;
            case "Bear":
                AddAttacks(race, ItemQuality.VeryGood, true);
                AddAttacks(race, ItemQuality.VeryGood, clawAttacks: true);
                AddAttacks(race, ItemQuality.Standard, chargeAttacks: true);
                break;
            case "Goat":
                AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Substandard, hornAttacks: true);
                break;
            case "Sheep":
            case "Pig":
            case "Llama":
            case "Alpaca":
            case "Giraffe":
                AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
                break;
            case "Boar":
            case "Warthog":
                AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Standard, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Substandard, tuskAttacks: true);
                break;
            case "Horse":
                AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Good, chargeAttacks: true);
                break;
            case "Deer":
                AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Standard, antlerAttacks: true);
                break;
            case "Moose":
                AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Substandard, chargeAttacks: true);
                AddAttacks(race, ItemQuality.VeryGood, antlerAttacks: true);
                break;
            case "Cow":
            case "Ox":
            case "Buffalo":
            case "Bison":
                AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
                AddAttacks(race, ItemQuality.VeryGood, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Standard, hornAttacks: true);
                break;
            case "Hippopotamus":
                AddAttacks(race, ItemQuality.Great, true);
                AddAttacks(race, ItemQuality.VeryGood, chargeAttacks: true);
                break;
            case "Rhinocerous":
                AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Great, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Standard, hornAttacks: true);
                break;
            case "Elephant":
                AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
                AddAttacks(race, ItemQuality.Great, chargeAttacks: true);
                AddAttacks(race, ItemQuality.Great, tuskAttacks: true);
                break;
            case "Carp":
            case "Cod":
            case "Haddock":
            case "Koi":
            case "Pilchard":
            case "Perch":
            case "Herring":
            case "Mackerel":
            case "Anchovy":
            case "Sardine":
            case "Pollock":
            case "Salmon":
            case "Tuna":
                AddAttacks(race, ItemQuality.Substandard, fishBites: true);
                break;
            case "Shark":
                AddAttacks(race, ItemQuality.Great, sharkAttacks: true);
                break;
            case "Small Crab":
                AddAttacks(race, ItemQuality.Bad, crabAttacks: true);
                break;
            case "Crab":
            case "Lobster":
                AddAttacks(race, ItemQuality.Substandard, crabAttacks: true);
                break;
            case "Giant Crab":
                AddAttacks(race, ItemQuality.Great, crabAttacks: true);
                break;
            case "Jellyfish":
                AddJellyfishAttack(race);
                break;
            case "Octopus":
            case "Squid":
                AddAttacks(race, ItemQuality.Bad, fishBites: true);
                break;
            case "Giant Squid":
                AddAttacks(race, ItemQuality.Standard, fishBites: true);
                break;
            case "Sea Lion":
            case "Seal":
            case "Walrus":
                AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
                break;
            case "Dolphin":
            case "Porpoise":
                AddAttacks(race, ItemQuality.Standard, fishBites: true);
                break;
            case "Orca":
                AddAttacks(race, ItemQuality.Heroic, sharkAttacks: true);
                break;
            case "Baleen Whale":
            case "Toothed Whale":
                break;
            case "Pigeon":
                AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Parrot":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Swallow":
                AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Sparrow":
                AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Quail":
                AddAttacks(race, ItemQuality.Terrible, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Duck":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Goose":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Swan":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Grouse":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Pheasant":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Chicken":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Turkey":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Seagull":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Albatross":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Heron":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Crane":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Flamingo":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Peacock":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Ibis":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Pelican":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Crow":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Raven":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Emu":
                AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Ostrich":
                AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Moa":
                AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Vulture":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Hawk":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Eagle":
                AddAttacks(race, ItemQuality.Good, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Falcon":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Woodpecker":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Owl":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Kingfisher":
                AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Stork":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
            case "Penguin":
                AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
                break;
        }
    }

    #endregion
}
