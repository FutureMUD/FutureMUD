#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
    private void SeedCombatStrategies(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        FutureProg humanProg = context.FutureProgs.First(predicate: x => x.FunctionName == "IsHumanoid");
        FutureProg? humanPCProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "IsHumanoidPC");
        if (humanPCProg is null)
        {
            humanPCProg = new FutureProg
            {
                FunctionName = "IsHumanoidPC",
                Category = "Character",
                Subcategory = "Descriptions",
                FunctionComment = "True if the character is a type of humanoid and is a PC",
                ReturnType = 4,
                StaticType = 0,
                FunctionText = """return SameRace(@ch.Race, ToRace("Humanoid")) and @ch.PC"""
            };
            humanPCProg.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = humanPCProg,
                ParameterIndex = 0,
                ParameterType = 8200,
                ParameterName = "ch"
            });
            context.FutureProgs.Add(humanPCProg);
        }

        void SeedCombatStrategy(string name, string description, double weaponUse, double naturalUse,
            double auxilliaryUse, bool preferFavourite, bool preferArmed, bool preferNonContact, bool preferShields,
            bool attackCritical, bool attackUnarmed, bool skirmish, bool fallbackToUnarmed,
            bool automaticallyMoveToTarget, bool manualPositionManagement, bool moveToMeleeIfCannotRange,
            PursuitMode pursuit, CombatStrategyMode melee, CombatStrategyMode ranged,
            AutomaticInventorySettings inventory, AutomaticMovementSettings movement,
            AutomaticRangedSettings rangesettings, AttackHandednessOptions setup, GrappleResponse grapple,
            double requiredMinimumAim, double minmumStamina, DefenseType defaultDefenseType,
            IEnumerable<MeleeAttackOrderPreference> order,
            CombatMoveIntentions forbiddenIntentions = CombatMoveIntentions.Savage | CombatMoveIntentions.Cruel,
            CombatMoveIntentions preferredIntentions = CombatMoveIntentions.None,
            FutureProg? prog = null)
        {
            if (context.CharacterCombatSettings.Any(x => x.Name == name))
            {
                return;
            }

            CharacterCombatSetting strategy = new()
            {
                Name = name,
                Description = description,
                GlobalTemplate = true,
                AvailabilityProg = prog ?? humanProg,
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
            context.CharacterCombatSettings.Add(entity: strategy);
            context.SaveChanges();
        }

        List<MeleeAttackOrderPreference> defaultOrder = new()
        {
            MeleeAttackOrderPreference.Weapon,
            MeleeAttackOrderPreference.Implant,
            MeleeAttackOrderPreference.Prosthetic,
            MeleeAttackOrderPreference.Magic,
            MeleeAttackOrderPreference.Psychic
        };

        SeedCombatStrategy(name: "Melee", description: "Fight with a weapon, move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Melee (Auto)", description: "Fight with a weapon, move to melee, not afraid of using unarmed if disarmed. Fully automated, designed for NPCs.", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.AlwaysPursue, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Cautious Melee", description: "Fight with a weapon, seek cover and then move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.CoverAndAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Cautious Melee (Auto)", description: "Fight with a weapon, seek cover and then move to melee, not afraid of using unarmed if disarmed. Fully automated, designed for NPCs.", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.AlwaysPursue, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.CoverAndAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Juggernaut", description: "Fight in melee with maximum power and using your size and body", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: false, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Throw, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Juggernaut (Auto)", description: "Fight in melee with maximum power and using your size and body. Fully automated, designed for NPCs.", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: false, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.AlwaysPursue, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Throw, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Manual",
            description: "A fighting style that is FULLY MANUAL. This means that all inventory management, ranged combat and combat movement is fully manual and controlled by the player. While this gives you the greatest degree of control, it is assuredly slower than using the automatic systems. Use with caution - only for advanced players.",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: true, moveToMeleeIfCannotRange: false,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyManual, movement: AutomaticMovementSettings.FullyManual,
            rangesettings: AutomaticRangedSettings.FullyManual, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Shielder",
            description: "Fight with a weapon and shield, move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0,
            preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.SwordAndBoardOnly,
            grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 10.0, defaultDefenseType: DefenseType.Block, order: defaultOrder);
        SeedCombatStrategy(name: "Shielder (Auto)",
           description: "Fight with a weapon and shield, move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0,
           preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
           melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
           inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
           rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.SwordAndBoardOnly,
           grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 10.0, defaultDefenseType: DefenseType.Block, order: defaultOrder);

        SeedCombatStrategy(name: "Zweihander",
            description: "Fight with a 2-hand weapon, move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.TwoHandedOnly,
            grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 0.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Zweihander (Auto)",
            description: "Fight with a 2-hand weapon, move to melee, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.TwoHandedOnly,
            grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 0.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Clincher", description: "Fight with a weapon, move into clinch, not afraid of using unarmed if disarmed",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Clincher (Auto)", description: "Fight with a weapon, move into clinch, not afraid of using unarmed if disarmed",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Warder",
            description: "Fight with a weapon, move to melee, ward, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FullAdvance, inventory: AutomaticInventorySettings.AutomaticButDontDiscard,
            movement: AutomaticMovementSettings.SeekCoverOnly, rangesettings: AutomaticRangedSettings.ContinueFiringOnly,
            setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 10.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Warder (Auto)",
           description: "Fight with a weapon, move to melee, ward, not afraid of using unarmed if disarmed", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
           preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
           melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FullAdvance, inventory: AutomaticInventorySettings.FullyAutomatic,
           movement: AutomaticMovementSettings.FullyAutomatic, rangesettings: AutomaticRangedSettings.FullyAutomatic,
           setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 10.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Defender", description: "Try to stay out of fights and full defend if you get into them", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0,
            preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.FullDefense, ranged: CombatStrategyMode.FullCover,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 20.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Defender (Auto)", description: "Try to stay out of fights and full defend if you get into them", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0,
           preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
           melee: CombatStrategyMode.FullDefense, ranged: CombatStrategyMode.FullCover,
           inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
           rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
           minmumStamina: 20.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Pistolier",
            description: "Try to fight at range, but keep using a pistol in melee if you get engaged there", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.MeleeShooter, ranged: CombatStrategyMode.StandardRange,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.2, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Pistolier (Auto)",
            description: "Try to fight at range, but keep using a pistol in melee if you get engaged there", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.MeleeShooter, ranged: CombatStrategyMode.StandardRange,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.2, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Musketeer", description: "Try to fight at range, seek no cover, and ward if you do get into melee", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FireNoCover, inventory: AutomaticInventorySettings.AutomaticButDontDiscard,
            movement: AutomaticMovementSettings.SeekCoverOnly, rangesettings: AutomaticRangedSettings.FullyAutomatic,
            setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.7, minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Musketeer (Auto)", description: "Try to fight at range, seek no cover, and ward if you do get into melee", weaponUse: 1.0,
            naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FireNoCover, inventory: AutomaticInventorySettings.FullyAutomatic,
            movement: AutomaticMovementSettings.FullyAutomatic, rangesettings: AutomaticRangedSettings.FullyAutomatic,
            setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.7, minmumStamina: 5.0, defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Infantryman", description: "Find cover if attacked suddely, return fire, but fight in melee if engaged.",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.NeverPursue, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.StandardRange,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.7, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Infantryman (Auto)", description: "Find cover if attacked suddely, return fire, but fight in melee if engaged.",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.NeverPursue, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.StandardRange,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.7, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Skirmisher", description: "Stay out of melee and fire your weapon. Prioritise mobility over safety.",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.NeverPursue, melee: CombatStrategyMode.FullSkirmish, ranged: CombatStrategyMode.FireNoCover,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Skirmisher (Auto)", description: "Stay out of melee and fire your weapon. Prioritise mobility over safety.",
            weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.NeverPursue, melee: CombatStrategyMode.FullSkirmish, ranged: CombatStrategyMode.FireNoCover,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Marksman", description: "Get into cover, aim well, and make your shots count.", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
            melee: CombatStrategyMode.FullSkirmish, ranged: CombatStrategyMode.StandardRange,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 1.0, minmumStamina: 5.0,
            defaultDefenseType: DefenseType.None, order: defaultOrder);
        SeedCombatStrategy(name: "Marksman (Auto)", description: "Get into cover, aim well, and make your shots count.", weaponUse: 1.0, naturalUse: 0.0, auxilliaryUse: 0.0, preferFavourite: false,
           preferArmed: true, preferNonContact: true, preferShields: true, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.NeverPursue,
           melee: CombatStrategyMode.FullSkirmish, ranged: CombatStrategyMode.StandardRange,
           inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
           rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 1.0, minmumStamina: 5.0,
           defaultDefenseType: DefenseType.None, order: defaultOrder);

        SeedCombatStrategy(name: "Brawler", description: "Fight unarmed in melee using a wide variety of moves", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Brawler (Auto)", description: "Fight unarmed in melee using a wide variety of moves", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Pitfighter", description: "Fight unarmed in melee with no holds barred and nothing off the table", weaponUse: 0.0,
            naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder, forbiddenIntentions: CombatMoveIntentions.None);
        SeedCombatStrategy(name: "Pitfighter (Auto)", description: "Fight unarmed in melee with no holds barred and nothing off the table", weaponUse: 0.0,
            naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true,
            pursuit: PursuitMode.OnlyAttemptToStop, melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder, forbiddenIntentions: CombatMoveIntentions.None);

        SeedCombatStrategy(name: "Swarmer", description: "Fight unarmed in melee and try to get into clinches", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Swarmer (Auto)", description: "Fight unarmed in melee and try to get into clinches", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
           preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
           melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
           inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
           rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
           minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Outboxer", description: "Fight unarmed in melee and try to keep range and use counter attacks", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FullAdvance, inventory: AutomaticInventorySettings.AutomaticButDontDiscard,
            movement: AutomaticMovementSettings.SeekCoverOnly, rangesettings: AutomaticRangedSettings.ContinueFiringOnly,
            setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Outboxer (Auto)", description: "Fight unarmed in melee and try to keep range and use counter attacks", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.Ward, ranged: CombatStrategyMode.FullAdvance, inventory: AutomaticInventorySettings.FullyAutomatic,
            movement: AutomaticMovementSettings.FullyAutomatic, rangesettings: AutomaticRangedSettings.FullyAutomatic,
            setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5, minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Grappler", description: "Fight unarmed in melee and try to grapple your opponent into control", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForControl, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Grappler (Auto)", description: "Fight unarmed in melee and try to grapple your opponent into control", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForControl, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Bonebreaker",
            description: "Fight unarmed in melee and try to grapple your opponent and break their limbs", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForIncapacitation, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Bonebreaker (Auto)",
            description: "Fight unarmed in melee and try to grapple your opponent and break their limbs", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
            preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForIncapacitation, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);

        SeedCombatStrategy(name: "Strangler", description: "Fight unarmed in melee and try to grapple, strangle and kill them", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForKill, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.AutomaticButDontDiscard, movement: AutomaticMovementSettings.SeekCoverOnly,
            rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
        SeedCombatStrategy(name: "Strangler (Auto)", description: "Fight unarmed in melee and try to grapple, strangle and kill them", weaponUse: 0.0, naturalUse: 1.0,
            auxilliaryUse: 0.0, preferFavourite: false, preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.OnlyAttemptToStop,
            melee: CombatStrategyMode.GrappleForKill, ranged: CombatStrategyMode.FullAdvance,
            inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
            rangesettings: AutomaticRangedSettings.FullyAutomatic, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
            minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
    }
}
