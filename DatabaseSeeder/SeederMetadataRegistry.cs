#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using MudSharp.Database;

namespace DatabaseSeeder;

public static class SeederMetadataRegistry
{
	public static SeederMetadata GetMetadata(IDatabaseSeeder seeder)
	{
		return seeder.GetType().Name switch
		{
			nameof(CoreDataSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				Array.Empty<SeederPrerequisite>(),
				RerunSummary: "This foundational package does not yet have stock-owned reconciliation rules.",
				OwnershipSummary: "Creates accounts and core world records that are not yet tracked for safe repeatability."
			),
			nameof(TimeSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "Reruns reuse the stock time package by canonical clock, timezone, and calendar identities.",
				UpdateSummary: "Reruns repair or complete stock clocks, calendars, timezones, and shard/zone bindings without deleting older setups.",
				OwnershipSummary: "Seeder-owned time records are tracked by stable names and aliases."
			),
			nameof(CelestialSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Additive,
				SeederUpdateCapability.InstallMissing,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "Designed as an additive package for more suns, moons, and related celestial objects.",
				UpdateSummary: "Reruns are intended to add stock celestial packages rather than reconcile edits to existing objects."
			),
			nameof(AttributeSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "Attribute setup is intentionally treated as a one-shot design choice unless a later plan approves repeatability."
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
				OwnershipSummary: "Stock skill-package records are keyed by check type, template name, decorator name, improver name, and seeded trait/language names."
			),
			nameof(SkillSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("Attributes must already be seeded.", context => context.TraitDefinitions.Any(x => x.Type == 1))
				],
				RerunSummary: "Reruns reuse the shared skill scaffolding and example records by stable names.",
				UpdateSummary: "This remains an alternative to the full Skill Package seeder, not a companion package."
			),
			nameof(CurrencySeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Additive,
				SeederUpdateCapability.InstallMissing,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "Designed as an additive package for installing more stock currencies."
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
				OwnershipSummary: "Stock economy content is tracked by stable era-specific names plus a shared EconomySeeder prefix for helper records."
			),
			nameof(ClanSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Additive,
				SeederUpdateCapability.InstallMissing,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("The Time seeder must have installed at least one clock.", context => context.Clocks.Any()),
					Requirement("The Currency seeder must have installed at least one currency.", context => context.Currencies.Any())
				],
				RerunSummary: "Designed as an additive package for installing more stock clan templates."
			),
			nameof(HumanSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("Skills must already be seeded.", context => context.TraitDefinitions.Any(x => x.Type == 0)),
					Requirement("The Time seeder must have installed at least one calendar.", context => context.Calendars.Any())
				],
				RerunSummary: "Currently treated as a one-shot humanoid race and anatomy bootstrap."
			),
			nameof(CombatSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				[
					Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human"))
				],
				RerunSummary: "Currently treated as a one-shot combat bootstrap pending modular reconciliation work."
			),
			nameof(ChargenSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human"))
				],
				RerunSummary: "Reruns reuse stock chargen resources, helper progs, storyboard stages, and the default starting-location role by stable keys.",
				UpdateSummary: "Existing storyboard XML is preserved when a matching storyboard already exists; reruns focus on repairing missing stock screens and dependencies.",
				OwnershipSummary: "Chargen storyboards are tracked by stage and screen type, and helper progs are tracked by function name."
			),
			nameof(CultureSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Human seeder must have installed the Human race.", context => context.Races.Any(x => x.Name == "Human")),
					Requirement("A skill decorator must already exist.", context => context.TraitDecorators.Any(x => x.Name.Contains("Skill"))),
					Requirement("Chargen height filtering progs must already exist.", context => context.FutureProgs.Any(x => x.FunctionName == "MaximumHeightChargen"))
				]
			),
			nameof(ArenaSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("At least one economic zone must exist.", context => context.EconomicZones.Any())
				],
				RerunSummary: "Reruns reuse the same named arena package and refresh stock-owned combatant classes, event types, event sides, and helper progs.",
				UpdateSummary: "Live arena configuration such as room links, finances, schedules, ratings, and events is preserved."
			),
			nameof(UsefulSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.InstallMissing,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "This package can be rerun to install missing stock kickstart content without duplicating its tracked packages.",
				UpdateSummary: "Current reruns primarily install missing stock records rather than repairing edited ones."
			),
			nameof(AIStorytellerSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any())
				],
				RerunSummary: "This package is designed to be rerun safely.",
				UpdateSummary: "Reruns reuse and update existing stock storyteller sample records."
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
				UpdateSummary: "Forward-only upgrades add or refresh higher-tech stock content without removing lower-tech content."
			),
			nameof(AnimalSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				[
					Requirement("The Human seeder must have installed the Humanoid body.", context => context.BodyProtos.Any(x => x.Name == "Humanoid")),
					Requirement("The Core seeder must have installed the Simple name culture.", context => context.NameCultures.Any(x => x.Name == "Simple"))
				]
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
							.All(strategy => context.HealthStrategies.Any(x => x.Name == strategy)))
				],
				RerunSummary: "Reruns install missing stock mythic races without duplicating existing entries."
			),
			nameof(WeatherSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("The Celestial seeder must have installed at least one celestial object.", context => context.Celestials.Any())
				],
				RerunSummary: "Reruns reuse the canonical weather catalog, seasons, climate models, and regional climates by stable names.",
				UpdateSummary: "Reruns refresh stock climate definitions without auto-retargeting runtime weather controllers or duplicating northern/southern climate rows."
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
				RerunSummary: "Reruns install missing stock robot races, bodies, and procedures without duplicating existing entries."
			),
			nameof(ItemSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.OneShot,
				SeederUpdateCapability.None,
				[
					Requirement("Useful item component prerequisites must already exist.", context =>
						context.GameItemComponentProtos.Any(x => x.Name == "Container_Table") &&
						context.GameItemComponentProtos.Any(x => x.Name == "Insulation_Minor") &&
						context.GameItemComponentProtos.Any(x => x.Name == "Destroyable_Misc") &&
						context.GameItemComponentProtos.Any(x => x.Name == "Torch_Infinite") &&
						context.Tags.Any(x => x.Name == "Functions"))
				]
			),
			nameof(LawSeeder) => new SeederMetadata(
				SeederRepeatabilityMode.Idempotent,
				SeederUpdateCapability.RepairExisting,
				[
					Requirement("The Core seeder must have created at least one account.", context => context.Accounts.Any()),
					Requirement("The Currency seeder must have installed at least one currency.", context => context.Currencies.Any())
				]
			),
			_ => SeederMetadata.Default
		};
	}

	public static SeederAssessment Assess(IDatabaseSeeder seeder, FuturemudDatabaseContext context)
	{
		var metadata = seeder.Metadata;
		var missingPrerequisites = metadata.Prerequisites
			.Where(x => !x.IsSatisfied(context))
			.Select(x => x.Description)
			.ToList();
		var warnings = new List<string>();
		var notes = new List<string>();
		var legacyResult = seeder.ShouldSeedData(context);

		if (!string.IsNullOrWhiteSpace(metadata.OwnershipSummary))
		{
			notes.Add(metadata.OwnershipSummary);
		}

		if (missingPrerequisites.Any() || legacyResult == ShouldSeedResult.PrerequisitesNotMet)
		{
			var explanation = missingPrerequisites.Any()
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

	private static SeederPrerequisite Requirement(string description, Func<FuturemudDatabaseContext, bool> predicate)
	{
		return new SeederPrerequisite(description, predicate);
	}
}
