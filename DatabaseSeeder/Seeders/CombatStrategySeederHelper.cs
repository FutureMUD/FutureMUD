using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static class CombatStrategySeederHelper
{
    internal static readonly IReadOnlyCollection<string> CanonicalStrategyNames =
    [
        "Melee (Auto)",
        "Beast Brawler",
        "Beast Clincher",
        "Beast Behemoth",
        "Beast Skirmisher",
        "Beast Swooper",
        "Beast Drowner",
        "Beast Dropper",
        "Beast Physical Avoider",
        "Beast Artillery",
        "Beast Coward",
        "Construct Brawler",
        "Construct Skirmisher",
        "Construct Artillery"
    ];

    public static bool IsKnownStrategyName(string strategyName)
    {
        return CanonicalStrategyNames.Contains(strategyName, StringComparer.OrdinalIgnoreCase);
    }

    public static CharacterCombatSetting EnsureCombatStrategy(MudSharp.Database.FuturemudDatabaseContext context, string strategyName)
    {
        CharacterCombatSetting? existing = context.CharacterCombatSettings.FirstOrDefault(x => x.Name == strategyName);
        if (existing is not null)
        {
            return existing;
        }

        FutureProg alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        FutureProg humanoidProg = context.FutureProgs.First(x => x.FunctionName == "IsHumanoid");
        List<MeleeAttackOrderPreference> defaultOrder = new()
        {
            MeleeAttackOrderPreference.Weapon,
            MeleeAttackOrderPreference.Implant,
            MeleeAttackOrderPreference.Prosthetic,
            MeleeAttackOrderPreference.Magic,
            MeleeAttackOrderPreference.Psychic
        };

        CharacterCombatSetting CreateStrategy(string name, string description, double weaponUse, double naturalUse,
            double auxiliaryUse, bool preferFavourite, bool preferArmed, bool preferNonContact, bool preferShields,
            bool attackCritical, bool attackUnarmed, bool skirmish, bool fallbackToUnarmed,
            bool automaticallyMoveToTarget, bool manualPositionManagement, bool moveToMeleeIfCannotRange,
            PursuitMode pursuit, CombatStrategyMode melee, CombatStrategyMode ranged,
            AutomaticInventorySettings inventory, AutomaticMovementSettings movement,
            AutomaticRangedSettings rangedSettings, AttackHandednessOptions setup, GrappleResponse grapple,
            double requiredMinimumAim, double minimumStamina, DefenseType defaultDefenseType,
            IEnumerable<MeleeAttackOrderPreference> order, FutureProg availabilityProg,
            CombatMoveIntentions forbiddenIntentions = CombatMoveIntentions.None,
            CombatMoveIntentions preferredIntentions = CombatMoveIntentions.None)
        {
            return new CharacterCombatSetting
            {
                Name = name,
                Description = description,
                GlobalTemplate = true,
                AvailabilityProg = availabilityProg,
                PriorityProg = null,
                WeaponUsePercentage = weaponUse,
                MagicUsePercentage = 0.0,
                PsychicUsePercentage = 0.0,
                NaturalWeaponPercentage = naturalUse,
                AuxiliaryPercentage = auxiliaryUse,
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
                AttackUnarmed = attackUnarmed,
                SkirmishToOtherLocations = skirmish,
                PursuitMode = (int)pursuit,
                DefaultPreferredDefenseType = (int)defaultDefenseType,
                PreferredMeleeMode = (int)melee,
                PreferredRangedMode = (int)ranged,
                FallbackToUnarmedIfNoWeapon = fallbackToUnarmed,
                AutomaticallyMoveTowardsTarget = automaticallyMoveToTarget,
                InventoryManagement = (int)inventory,
                MovementManagement = (int)movement,
                RangedManagement = (int)rangedSettings,
                ManualPositionManagement = manualPositionManagement,
                MinimumStaminaToAttack = minimumStamina,
                MoveToMeleeIfCannotEngageInRangedCombat = moveToMeleeIfCannotRange,
                PreferredWeaponSetup = (int)setup,
                RequiredMinimumAim = requiredMinimumAim,
                MeleeAttackOrderPreference = order.Select(x => ((int)x).ToString()).ListToCommaSeparatedValues(" "),
                GrappleResponse = (int)grapple
            };
        }

        CharacterCombatSetting strategy = strategyName switch
        {
            "Melee (Auto)" => CreateStrategy(
                "Melee (Auto)",
                "Fight with a weapon, move to melee, not afraid of using unarmed if disarmed. Fully automated, designed for NPCs.",
                1.0, 0.0, 0.0, false, true, true, true, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.5, 5.0, DefenseType.None, defaultOrder, humanoidProg),
            "Beast Brawler" => CreateStrategy(
                "Beast Brawler",
                "Fully automatic natural-weapon brawler for animals and beasts.",
                0.0, 1.0, 0.0, false, false, false, false, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Clincher" => CreateStrategy(
                "Beast Clincher",
                "Fully automatic clinch-focused beast strategy for bites, constriction, and clamp attacks.",
                0.0, 1.0, 0.0, false, false, false, false, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.Clinch, CombatStrategyMode.FullAdvance,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Behemoth" => CreateStrategy(
                "Beast Behemoth",
                "Fully automatic heavy beast strategy for big chargers, stompers, and rammers.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Skirmisher" => CreateStrategy(
                "Beast Skirmisher",
                "Fully automatic hit-and-run beast strategy for agile natural attackers.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.FullSkirmish, CombatStrategyMode.FireNoCover,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.4, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Swooper" => CreateStrategy(
                "Beast Swooper",
                "Fully automatic aerial hunter strategy for diving and buffeting creatures.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.Swooper, CombatStrategyMode.Swooper,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.4, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Drowner" => CreateStrategy(
                "Beast Drowner",
                "Fully automatic aquatic ambusher strategy for forcing air-breathers into water and grappling them there.",
                0.0, 1.0, 0.0, false, false, false, false, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.Drowner, CombatStrategyMode.Drowner,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Dropper" => CreateStrategy(
                "Beast Dropper",
                "Fully automatic aerial grappler strategy for lifting prey upward and dropping them when possible.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.Dropper, CombatStrategyMode.Dropper,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.4, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Physical Avoider" => CreateStrategy(
                "Beast Physical Avoider",
                "Fully automatic range-preserving beast strategy that uses trips, staggers and pushbacks to disengage.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, false, true, true, false, false,
                PursuitMode.NeverPursue, CombatStrategyMode.PhysicalAvoider, CombatStrategyMode.PhysicalAvoider,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.4, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue,
                preferredIntentions: CombatMoveIntentions.Disadvantage),
            "Beast Artillery" => CreateStrategy(
                "Beast Artillery",
                "Fully automatic ranged-natural strategy for breath, spit, spike, and bombardment attackers.",
                0.0, 0.85, 0.15, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.FullSkirmish, CombatStrategyMode.StandardRange,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Beast Coward" => CreateStrategy(
                "Beast Coward",
                "Fully automatic flee-first strategy for timid prey animals.",
                0.0, 0.1, 0.9, false, false, false, false, true, true, false, true, false, false, false,
                PursuitMode.NeverPursue, CombatStrategyMode.Flee, CombatStrategyMode.Flee,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Avoidance,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Construct Brawler" => CreateStrategy(
                "Construct Brawler",
                "Fully automatic close-range construct strategy for integrated melee tools and chassis attacks.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, false, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.StandardMelee, CombatStrategyMode.FullAdvance,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.ContinueFiringOnly, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Construct Skirmisher" => CreateStrategy(
                "Construct Skirmisher",
                "Fully automatic mobility-first construct strategy.",
                0.0, 1.0, 0.0, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.FullSkirmish, CombatStrategyMode.FireNoCover,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.4, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            "Construct Artillery" => CreateStrategy(
                "Construct Artillery",
                "Fully automatic ranged construct strategy for projection and built-in ranged attacks.",
                0.0, 0.8, 0.2, false, false, true, false, true, true, true, true, true, false, true,
                PursuitMode.AlwaysPursue, CombatStrategyMode.FullSkirmish, CombatStrategyMode.StandardRange,
                AutomaticInventorySettings.FullyAutomatic, AutomaticMovementSettings.FullyAutomatic,
                AutomaticRangedSettings.FullyAutomatic, AttackHandednessOptions.Any, GrappleResponse.Counter,
                0.5, 5.0, DefenseType.Dodge, defaultOrder, alwaysTrue),
            _ => throw new InvalidOperationException($"No combat strategy definition exists for {strategyName}.")
        };

        context.CharacterCombatSettings.Add(strategy);
        context.SaveChanges();
        return strategy;
    }
}
