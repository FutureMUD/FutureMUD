#nullable enable

using DatabaseSeeder.Seeders;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

public static class SeederMetadataRegistry
{
    public static SeederMetadata GetMetadata(IDatabaseSeeder seeder)
    {
        return seeder.GetType().Name switch
        {
            nameof(CoreDataSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                Array.Empty<SeederPrerequisite>(),
                RerunSummary: "Reruns reconcile only the stock foundation catalogues and do not recreate bootstrap-world data.",
                UpdateSummary: "Materials, fluids, gases, terrain foundations, units, colours, planes and hearing profiles are repaired or completed by stable stock identities.",
                OwnershipSummary: "Core bootstrap accounts, world records, progs and settings remain one-shot; only the documented foundation catalogues are repeatable."
            ),
            nameof(TimeSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "Reruns reuse the stock time package by canonical clock, timezone, and calendar identities.",
                UpdateSummary: "Reruns repair or complete stock clocks, calendars, timezones, and shard/zone bindings without deleting older setups.",
                OwnershipSummary: "Seeder-owned time records are tracked by stable names and aliases.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(CelestialSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Additive,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "Designed as an additive package for more suns, moons, and related celestial objects.",
                UpdateSummary: "Reruns are intended to add stock celestial packages rather than reconcile edits to existing objects.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(AttributeSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.FullReconcile,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "Reruns retain the selected attribute shape and reconcile its stock traits, decorators, improver and expressions.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(SkillPackageSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Attributes must already be seeded.", context => context.TraitDefinitions.Any(x => x.Type == 1))
                ],
                RerunSummary: "Reruns reuse the stock skill package templates, improvers, admin language, checks, and seeded skills by stable names.",
                UpdateSummary: "This remains an alternative to the Skill Example seeder, not a companion package.",
                OwnershipSummary: "Stock skill-package records are keyed by check type, template name, decorator name, improver name, and seeded trait/language names.",
                DependencySeederTypes: [typeof(AttributeSeeder)]
            ),
            nameof(SkillSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Attributes must already be seeded.", context => context.TraitDefinitions.Any(x => x.Type == 1))
                ],
                RerunSummary: "Reruns reuse the shared skill scaffolding and example records by stable names.",
                UpdateSummary: "This remains an alternative to the full Skill Package seeder, not a companion package.",
                DependencySeederTypes: [typeof(AttributeSeeder)]
            ),
            nameof(CurrencySeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Additive,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "Designed as an additive package for installing more stock currencies.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(EconomySeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Additive,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Currency seeder must have installed at least one currency.", context => context.Currencies.Any()),
                    Requirement("The Time seeder must have installed at least one clock and calendar.", context => context.Clocks.Any() && context.Calendars.Any()),
                    Requirement("At least one physical zone must exist.", context => context.Zones.Any()),
                    Requirement("UsefulSeeder market tags must already exist.", context =>
                        context.Tags.Any(x => x.Name == "Market") &&
                        context.Tags.Any(x => x.Parent != null && x.Parent.Name == "Market"))
                ],
                RerunSummary: "Reruns install missing stock economy packages for other eras and can restore missing stock-owned market categories, influence templates, populations, shoppers, and helper progs.",
                UpdateSummary: "Rerunning the same era refreshes the seeded template market, populations, shopper definitions, and stress helper progs without creating duplicates.",
                OwnershipSummary: "Stock economy content is tracked by stable era-specific names plus a shared EconomySeeder prefix for helper records.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(CurrencySeeder), typeof(TimeSeeder), typeof(UsefulSeeder)]
            ),
            nameof(ClanSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Additive,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Time seeder must have installed at least one clock.", context => context.Clocks.Any()),
                    Requirement("The Currency seeder must have installed at least one currency.", context => context.Currencies.Any())
                ],
                RerunSummary: "Designed as an additive package for installing more stock clan templates.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(CurrencySeeder), typeof(TimeSeeder)]
            ),
            nameof(HumanSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.FullReconcile,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Skills must already be seeded.", context => context.TraitDefinitions.Any(x => x.Type == 0)),
                    Requirement("The Time seeder must have installed at least one calendar.", context => context.Calendars.Any())
                ],
                RerunSummary: "Reruns retain the installed humanoid shape and reconcile stock body, race, health, culture and wear foundations.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(TimeSeeder)]
            ),
            nameof(CombatSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.FullReconcile,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Attributes must already be seeded for combat formulas.", context => context.TraitDefinitions.Any(x => x.Type == 1)),
                    Requirement("Shared skill infrastructure (Skill Check, General Skill, Veterancy Skill, Skill Improver, AlwaysTrue and AlwaysFalse) must already exist.", context =>
                        context.CheckTemplates.Any(x => x.Name == "Skill Check") &&
                        context.TraitDecorators.Any(x => x.Name == "General Skill") &&
                        context.TraitDecorators.Any(x => x.Name == "Veterancy Skill") &&
                        context.Improvers.Any(x => x.Name == "Skill Improver") &&
                        context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue") &&
                        context.FutureProgs.Any(x => x.FunctionName == "AlwaysFalse")),
                    Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human")),
                    Requirement("UsefulSeeder crossbow spanning-tool tags must already exist.", context =>
                        new[] { "Cranequin", "Goat's Foot", "Lever", "Spanning Hook", "Windlass" }
                            .All(tag => context.Tags.Any(x => x.Name == tag)))
                ],
                RerunSummary: "Reruns reconcile installed combat modules by their stable stock identities without removing builder content.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(AttributeSeeder), typeof(HumanSeeder), typeof(UsefulSeeder)]
            ),
            nameof(ChargenSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human"))
                ],
                RerunSummary: "Reruns reuse stock chargen resources, special-application static settings, helper progs, canonical storyboard stages, and the default starting-location role by stable keys.",
                UpdateSummary: "Reruns repair missing stock screens, helper progs, dependencies, and special-application settings without creating duplicate storyboard rows for the same chargen stage.",
                OwnershipSummary: "Chargen storyboards are tracked as one canonical row per chargen stage, helper progs are tracked by function name, and the default starting-location role is tracked by stable name.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(HumanSeeder)]
            ),
            nameof(StockMeritsSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human")),
                    Requirement("Chargen must already include a merit or quirk selection screen.", context =>
                        context.ChargenScreenStoryboards.Any(x =>
                            x.ChargenStage == (int)MudSharp.CharacterCreation.ChargenStage.SelectMerits &&
                            (x.ChargenType == "MeritPicker" || x.ChargenType == "QuirkPicker")))
                ],
                RerunSummary: "Reruns reuse the stock merits, flaws, and helper FutureProgs by canonical names.",
                UpdateSummary: "Reruns repair missing stock merits, flaws, and tag-driven helper progs without changing chargen mode or chargen-resource costs.",
                OwnershipSummary: "Stock merit content is tracked by stable merit names and helper FutureProg function names.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(HumanSeeder), typeof(ChargenSeeder)]
            ),
            nameof(CultureSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human")),
                    Requirement("A skill decorator must already exist.", context => context.TraitDecorators.Any(x => x.Name.Contains("Skill"))),
                    Requirement("Chargen height filtering progs must already exist.", context => context.FutureProgs.Any(x => x.FunctionName == "MaximumHeightChargen"))
                ],
                DependencySeederTypes: [typeof(HumanSeeder), typeof(ChargenSeeder)]
            ),
            nameof(ArenaSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("At least one economic zone must exist.", context => context.EconomicZones.Any())
                ],
                RerunSummary: "Reruns reuse the same named arena package and refresh stock-owned combatant classes, event types, event sides, and helper progs.",
                UpdateSummary: "Live arena configuration such as room links, finances, schedules, ratings, and events is preserved.",
                DependencySeederTypes: [typeof(EconomySeeder)]
            ),
            nameof(UsefulSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "This package can be rerun to install missing stock kickstart content without duplicating its tracked packages.",
                UpdateSummary: "Reruns also refresh the stock wilderness autobuilder room template, area template, and supporting terrain-feature tags by stable names.",
                OwnershipSummary: "Kickstart now owns stock items, AI, helper tags, the wilderness autobuilder room+area starter package, ranged covers, hints, and dream content; core terrain foundations are seeded separately.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(AgricultureSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Core utility progs must include AlwaysTrue.", context => context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue")),
                    Requirement("UsefulSeeder agriculture tags must already exist.", context =>
                        new[]
                        {
                            "Seeds", "Seeded Yield", "Agriculture Seedable", "Bee Hive", "Hive Stand",
                            "Raw Honeycomb", "Pressed Honey", "Rendered Beeswax", "Raw Milk",
                            "Raw Textile Fibre", "Egg Product", "Manure Commodity"
                        }.All(tag => context.Tags.Any(x => x.Name == tag))),
                    Requirement("A Farming trait must already exist.", context =>
                        context.TraitDefinitions.Any(x => x.Name == "Farming"))
                ],
                RerunSummary: "Reruns reuse stock agriculture definitions, operation rows, and their project templates by stable names.",
                UpdateSummary: "Reruns refresh stock field profiles, crops, herds, woodlands, operations, and project-backed labour templates without duplicating rows.",
                OwnershipSummary: "Stock agriculture content is tracked by stable profile, crop, herd, woodland, operation, and project names.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(UsefulSeeder)]
            ),
            nameof(PrimaryProductionSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Core utility progs must include AlwaysTrue.", context => context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue")),
                    Requirement("Primary production tags and materials must already exist.", context =>
                        context.Tags.Any(x => x.Name == "Primary Production Commodity") &&
                        context.Tags.Any(x => x.Name == "Visible Resource Deposit") &&
                        context.Materials.Any(x => x.Name == "hematite") &&
                        context.Materials.Any(x => x.Name == "charcoal")),
                    Requirement("The Item seeder must have installed primary-production visible resource props.", context =>
                        context.GameItemProtos.Any(x => x.UniqueName == "primary_production_hematite_deposit") &&
                        context.GameItemProtos.Any(x => x.UniqueName == "primary_production_bloomery_furnace")),
                    Requirement("Required stock traits must already exist.", context =>
                        HasTrait(context, "Labouring", "Labourer", "Laboring", "Laborer") &&
                        HasTrait(context, "Masonry", "Stonecraft", "Stoneworking", "Mason") &&
                        HasTrait(context, "Smelting", "Smelter"))
                ],
                RerunSummary: "Reruns reuse stock primary-production local project templates by deterministic names.",
                UpdateSummary: "Reruns refresh stock project definitions, labour, material requirements, and resource/commodity actions without duplicating templates.",
                OwnershipSummary: "Stock primary-production project content is tracked by the Stock Primary Production project-name prefix.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(UsefulSeeder), typeof(ItemSeeder)]
            ),
            nameof(CookingSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("Useful item components must already include Holdable and Stack_Number.", context =>
                        context.GameItemComponentProtos.Any(x => x.Name == "Holdable") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Stack_Number")),
                    Requirement("Core food materials must already exist.", context =>
                        new[] { "apple", "blueberry", "mushroom", "muffin" }.All(material => context.Materials.Any(x => x.Name == material))),
                    Requirement("At least one trait definition must exist for cooking craft quality checks.", context => context.TraitDefinitions.Any())
                ],
                RerunSummary: "Reruns install missing prepared-food stock records without mutating the legacy Food component.",
                UpdateSummary: "This package owns direct prepared-food examples, stackable serving examples, and stock CookedFoodProduct recipe examples by stable names.",
                OwnershipSummary: "Stock prepared-food content is tracked by CookingSeeder component names, item short descriptions, tags, and recipe names.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(UsefulSeeder)]
            ),
            nameof(AIStorytellerSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
                ],
                RerunSummary: "This package is designed to be rerun safely.",
                UpdateSummary: "Reruns reuse and update existing stock storyteller sample records.",
                DependencySeederTypes: [typeof(CoreDataSeeder)]
            ),
            nameof(HealthSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Human seeder must have installed Organic Humanoid.", context => context.Races.Any(x => x.Name == "Organic Humanoid")),
                    Requirement("Required stock medical tool tags must exist.", context =>
                        new[] { "Scalpel", "Bonesaw", "Forceps", "Arterial Clamp", "Surgical Suture Needle" }
                            .All(tag => context.Tags.Any(x => x.Name == tag)))
                ],
                RerunSummary: "Reruns reuse stock medical knowledges, procedures, phases, and drugs by stable names.",
                UpdateSummary: "Forward-only upgrades add or refresh higher-tech stock content without removing lower-tech content.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(HumanSeeder), typeof(UsefulSeeder)]
            ),
            nameof(AnimalSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.FullReconcile,
                [
                    Requirement("The Human seeder must have installed the Humanoid body.", context => context.BodyProtos.Any(x => x.Name == "Humanoid")),
                    Requirement("The Core seeder must have installed the Simple name culture.", context => context.NameCultures.Any(x => x.Name == "Simple"))
                ],
                RerunSummary: "Reruns retain the installed animal package choices and reconcile stock bodies, races, attacks and supporting content.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(HumanSeeder)]
            ),
            nameof(MythicalAnimalSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("Human and animal body frameworks must already be installed.", context =>
                        new[] { "Organic Humanoid", "Quadruped Base", "Ungulate", "Toed Quadruped", "Avian", "Vermiform", "Serpentine", "Piscine", "Scorpion" }
                            .All(body => context.BodyProtos.Any(x => x.Name == body))),
                    Requirement("Human race foundations must already exist.", context =>
                        new[] { "Human", "Organic Humanoid" }.All(race => context.Races.Any(x => x.Name == race))),
                    Requirement("Shared humanoid characteristic profiles must already exist.", context =>
                        new[] { "All Eye Colours", "All Eye Shapes", "All Noses", "All Ears", "All Hair Colours", "All Facial Hair Colours", "All Hair Styles", "All Skin Colours", "All Frames", "Person Word" }
                            .All(profile =>
                                context.CharacteristicProfiles.Any(x => x.Name == profile) ||
                                context.CharacteristicDefinitions.Any(x => x.Name == profile))),
                    Requirement("Stock organic corpse models and non-human strategies must already exist.", context =>
                        context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
                        context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
                        new[] { "Non-Human HP", "Non-Human HP Plus", "Non-Human Full Model" }
                            .All(strategy => context.HealthStrategies.Any(x => x.Name == strategy))),
                    Requirement("The complete mythical-animal foundation, including height-weight models and Acid Spit, must already exist.",
                        MythicalAnimalSeeder.HasPrerequisites)
                ],
                RerunSummary: "Reruns install missing stock mythic races without duplicating existing entries.",
                DependencySeederTypes: [typeof(HumanSeeder), typeof(AnimalSeeder), typeof(CombatSeeder)]
            ),
            nameof(SupernaturalSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("Human, animal, and mythical body frameworks must already be installed.", context =>
                        new[] { "Organic Humanoid", "Winged Humanoid", "Horned Humanoid", "Quadruped Base", "Toed Quadruped" }
                            .All(body => context.BodyProtos.Any(x => x.Name == body))),
                    Requirement("Human, organic humanoid, and wolf race foundations must already exist.", context =>
                        new[] { "Human", "Organic Humanoid", "Wolf" }.All(race => context.Races.Any(x => x.Name == race))),
                    Requirement("Shared humanoid characteristic profiles must already exist.", context =>
                        new[] { "All Eye Colours", "All Eye Shapes", "All Noses", "All Ears", "All Hair Colours", "All Facial Hair Colours", "All Hair Styles", "All Skin Colours", "All Frames", "Person Word" }
                            .All(profile =>
                                context.CharacteristicProfiles.Any(x => x.Name == profile) ||
                                context.CharacteristicDefinitions.Any(x => x.Name == profile))),
                    Requirement("Stock natural attacks and non-human health strategies must already exist.", context =>
                        new[]
                        {
                            "Bite", "Carnivore Bite", "Carnivore Low Bite", "Claw High Swipe", "Claw Low Swipe",
                            "Animal Barge", "Animal Barge Pushback", "Horn Gore", "Wing Buffet", "Tail Spike",
                            "Acid Spit", "Llama Spit", "Dragonfire Breath", "Tusk Sweep", "Head Ram", "Headbutt",
                            "Claw Clamp", "Tree Haul", "Water Drag", "Tail Slap"
                        }
                            .All(attack => context.WeaponAttacks.Any(x => x.Name == attack)) &&
                        NonHumanSeederHealthStrategyHelper.AllStrategyNames.All(strategy => context.HealthStrategies.Any(x => x.Name == strategy))),
                    Requirement("Stock helper progs, corpse models, and at least one calendar must already exist.", context =>
                        new[] { "AlwaysTrue", "AlwaysFalse", "AlwaysZero" }.All(prog => context.FutureProgs.Any(x => x.FunctionName == prog)) &&
                        context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
                        context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
                        context.Calendars.Any()),
                    Requirement("The complete supernatural foundation, including blood, breathable atmosphere, required attributes and health strategies, must already exist.",
                        SupernaturalSeeder.HasPrerequisites)
                ],
                RerunSummary: "Reruns install or refresh the stock supernatural race catalogue, body prototypes, form merits, cultures, name cultures, attacks, and non-breather settings.",
                UpdateSummary: "Existing builder-customized worlds keep their records; stock-owned supernatural records are repaired by stable names without deleting custom extensions.",
                OwnershipSummary: "Supernatural stock content is tracked by stable race, body, culture, name-culture, merit, attack, and corpse-model names.",
                DependencySeederTypes: [typeof(HumanSeeder), typeof(AnimalSeeder), typeof(MythicalAnimalSeeder), typeof(CombatSeeder), typeof(TimeSeeder)]
            ),
            nameof(WeatherSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Celestial seeder must have installed at least one celestial object.", context => context.Celestials.Any())
                ],
                RerunSummary: "Reruns reuse the canonical weather catalog, seasons, climate models, and regional climates by stable names.",
                UpdateSummary: "Reruns refresh stock climate definitions without auto-retargeting runtime weather controllers or duplicating northern/southern climate rows.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(CelestialSeeder)]
            ),
            nameof(RobotSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.InstallMissing,
                [
                    Requirement("Humanoid and animal body frameworks must already be installed.", context =>
                        new[] { "Humanoid", "Toed Quadruped", "Insectoid", "Arachnid" }
                            .All(body => context.BodyProtos.Any(x => x.Name == body))),
                    Requirement("Human race foundations must already exist.", context =>
                        new[] { "Human", "Humanoid" }.All(race => context.Races.Any(x => x.Name == race))),
                    Requirement("Shared humanoid characteristic profiles must already exist.", context =>
                        new[] { "All Eye Colours", "All Eye Shapes", "All Noses", "All Ears", "All Hair Colours", "All Facial Hair Colours", "All Hair Styles", "All Skin Colours", "All Frames", "Person Word" }
                            .All(profile =>
                                context.CharacteristicProfiles.Any(x => x.Name == profile) ||
                                context.CharacteristicDefinitions.Any(x => x.Name == profile))),
                    Requirement("Core robot progs, corpse models, tool tags, and prerequisite attacks must already exist.", context =>
                        new[] { "AlwaysTrue", "AlwaysFalse" }.All(prog => context.FutureProgs.Any(x => x.FunctionName == prog)) &&
                        context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
                        context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
                        new[] { "Scalpel", "Bonesaw", "Forceps", "Arterial Clamp", "Surgical Suture Needle" }.All(tag => context.Tags.Any(x => x.Name == tag)) &&
                        new[] { "Jab", "Cross", "Hook", "Elbow", "Bite", "Snap Kick", "Carnivore Bite", "Claw Low Swipe", "Claw High Swipe" }
                            .All(attack => context.WeaponAttacks.Any(x => x.Name == attack)))
                ],
                RerunSummary: "Reruns install missing stock robot races, bodies, and procedures without duplicating existing entries.",
                DependencySeederTypes: [typeof(HumanSeeder), typeof(AnimalSeeder), typeof(CombatSeeder), typeof(UsefulSeeder)]
            ),
            nameof(ItemSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.FullReconcile,
                [
                    Requirement("Useful item component prerequisites must already exist.", context =>
                        context.GameItemComponentProtos.Any(x => x.Name == "Container_Table") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Insulation_Minor") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Destroyable_Misc") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Torch_Infinite") &&
                        context.Tags.Any(x => x.Name == "Functions"))
				],
				RerunSummary: "Reruns reconcile the installed ItemSeeder eras and can add newly selected implemented eras without removing prior content.",
				UpdateSummary: "Manifest-owned stock aggregates are repaired when untouched; builder-customized aggregates are preserved and reported.",
				OwnershipSummary: "Durable SeederManagedRecords provenance owns stock aggregate definitions and required stock links. Builder additions and customized aggregate graphs are retained; ambiguous untracked identities block before mutation.",
                DependencySeederTypes: [typeof(UsefulSeeder)]
            ),
            nameof(AnimalButcherySeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("Useful item component prerequisites must already include simple held, destroyable and stackable props.", context =>
                        context.GameItemComponentProtos.Any(x => x.Name == "Holdable") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Destroyable_Misc") &&
                        context.GameItemComponentProtos.Any(x => x.Name == "Stack_Pile")),
                    Requirement("Core animal product, cutting, meat, bone and skin foundations must already exist.", context =>
                        context.Tags.Any(x => x.Name == "Animal Product") &&
                        context.Tags.Any(x => x.Name == "Cutting") &&
                        context.Materials.Any(x => x.Name == "meat") &&
                        context.Materials.Any(x => x.Name == "bone") &&
                        context.Materials.Any(x => x.Name == "animal skin")),
                    Requirement("A dedicated Butchery skill (or the simple package's Survival skill) and at least one stock animal race must already exist.", context =>
                        HasTrait(context, "Butchery", "Butchering", "Survival", "Surviving") &&
                        context.Races.Any())
                ],
                RerunSummary: "Reruns reuse stock animal butchery item, product and profile names, then attach missing eligible stock races.",
                UpdateSummary: "Existing builder-authored butchery profiles are preserved; only stock-owned profiles and unassigned eligible stock races are repaired.",
                OwnershipSummary: "Stock butchery content is tracked by the Stock Butchery profile/product prefix plus the Butchery Output tag.",
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(UsefulSeeder), typeof(AnimalSeeder)]
            ),
            nameof(LawSeeder) => new SeederMetadata(
                SeederRepeatabilityMode.Idempotent,
                SeederUpdateCapability.RepairExisting,
                [
                    Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
                    Requirement("The Currency seeder must have installed at least one currency.", context => context.Currencies.Any())
                ],
                DependencySeederTypes: [typeof(CoreDataSeeder), typeof(CurrencySeeder)]
            ),
            _ => SeederMetadata.Default
        };
    }

    public static SeederAssessment Assess(IDatabaseSeeder seeder, FuturemudDatabaseContext context)
    {
        SeederMetadata metadata = seeder.Metadata;
        List<string> missingPrerequisites = metadata.Prerequisites
            .Where(x => !x.IsSatisfied(context))
            .Select(x => x.Description)
            .ToList();
        List<string> warnings = new();
        List<string> notes = new();
        ShouldSeedResult legacyResult = seeder.ShouldSeedData(context);

        if (!string.IsNullOrWhiteSpace(metadata.OwnershipSummary))
        {
            notes.Add(metadata.OwnershipSummary);
        }

        if (missingPrerequisites.Any() || legacyResult == ShouldSeedResult.PrerequisitesNotMet)
        {
            string explanation = missingPrerequisites.Any()
                ? $"Missing prerequisites: {string.Join("; ", missingPrerequisites)}"
                : "This package reports that its prerequisites are not currently met.";

            return new SeederAssessment(
                SeederAssessmentStatus.Blocked,
                explanation,
                missingPrerequisites,
                warnings,
                notes
            );
        }

        switch (legacyResult)
        {
            case ShouldSeedResult.ReadyToInstall:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                return new SeederAssessment(
                    SeederAssessmentStatus.ReadyToInstall,
                    "This package is ready to install.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            case ShouldSeedResult.ExtraPackagesAvailable:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                if (!string.IsNullOrWhiteSpace(metadata.UpdateSummary))
                {
                    notes.Add(metadata.UpdateSummary);
                }

                return new SeederAssessment(
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.Additive
                        ? SeederAssessmentStatus.AdditiveInstallAvailable
                        : SeederAssessmentStatus.UpdateAvailable,
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.Additive
                        ? "This package can add more stock content on a rerun."
                        : "This package can be rerun to install or refresh missing stock content.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            case ShouldSeedResult.MayAlreadyBeInstalled:
                if (!string.IsNullOrWhiteSpace(metadata.RerunSummary))
                {
                    notes.Add(metadata.RerunSummary);
                }

                if (!string.IsNullOrWhiteSpace(metadata.UpdateSummary))
                {
                    notes.Add(metadata.UpdateSummary);
                }

                if (metadata.RepeatabilityMode == SeederRepeatabilityMode.OneShot &&
                    metadata.UpdateCapability == SeederUpdateCapability.None)
                {
                    warnings.Add("This package appears to already be installed and rerunning it is not currently recommended.");
                }

                return new SeederAssessment(
                    SeederAssessmentStatus.InstalledCurrent,
                    metadata.RepeatabilityMode == SeederRepeatabilityMode.OneShot &&
                    metadata.UpdateCapability == SeederUpdateCapability.None
                        ? "This package appears to already be installed."
                        : "All currently detectable stock records for this package appear to be present.",
                    missingPrerequisites,
                    warnings,
                    notes
                );

            default:
                return new SeederAssessment(
                    SeederAssessmentStatus.Blocked,
                    "This package reported an unknown assessment state.",
                    missingPrerequisites,
                    warnings,
                    notes
                );
        }
    }

    /// <summary>
    /// Produces a stable installation order from the declared seeder dependencies. A dependency is only
    /// an ordering hint; the database-backed prerequisite predicates remain the source of truth for
    /// whether a package can run in a particular world.
    /// </summary>
    public static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders)
    {
        Dictionary<Type, IDatabaseSeeder> seedersByType = seeders
            .GroupBy(x => x.GetType())
            .ToDictionary(x => x.Key, x => x.First());
        Dictionary<Type, HashSet<Type>> prerequisitesBySeeder = seedersByType.Keys
            .ToDictionary(x => x, _ => new HashSet<Type>());
        List<string> errors = new();

        foreach ((Type seederType, IDatabaseSeeder seeder) in seedersByType)
        {
            foreach (Type prerequisiteType in seeder.Metadata.RequiredSeederTypes.Distinct())
            {
                if (prerequisiteType == seederType)
                {
                    errors.Add($"{seeder.Name} cannot depend on itself.");
                    continue;
                }

                if (!seedersByType.ContainsKey(prerequisiteType))
                {
                    errors.Add(
                        $"{seeder.Name} declares unavailable prerequisite seeder {prerequisiteType.Name}.");
                    continue;
                }

                prerequisitesBySeeder[seederType].Add(prerequisiteType);
            }
        }

        List<Type> orderedTypes = new();
        HashSet<Type> remainingTypes = prerequisitesBySeeder.Keys.ToHashSet();
        while (remainingTypes.Any())
        {
            List<Type> readyTypes = remainingTypes
                .Where(x => prerequisitesBySeeder[x].All(prerequisite => !remainingTypes.Contains(prerequisite)))
                .OrderBy(x => seedersByType[x].SortOrder)
                .ThenBy(x => seedersByType[x].Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!readyTypes.Any())
            {
                string cycle = string.Join(", ", remainingTypes
                    .Select(x => seedersByType[x].Name)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList());
                errors.Add($"Seeder dependency cycle detected between: {cycle}.");
                orderedTypes.AddRange(remainingTypes
                    .OrderBy(x => seedersByType[x].SortOrder)
                    .ThenBy(x => seedersByType[x].Name, StringComparer.OrdinalIgnoreCase));
                break;
            }

            orderedTypes.AddRange(readyTypes);
            remainingTypes.ExceptWith(readyTypes);
        }

        return new SeederDependencyPlan(
            orderedTypes.Select(x => seedersByType[x]).ToList(),
            errors);
    }

    private static SeederPrerequisite Requirement(string description, Func<FuturemudDatabaseContext, bool> predicate)
    {
        return new SeederPrerequisite(description, predicate);
    }

    private static bool HasTrait(FuturemudDatabaseContext context, params string[] aliases)
    {
        return context.TraitDefinitions
            .AsEnumerable()
            .Any(x => aliases.Any(alias => NormaliseName(x.Name) == NormaliseName(alias)));
    }

    private static string NormaliseName(string text)
    {
        return new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }
}
