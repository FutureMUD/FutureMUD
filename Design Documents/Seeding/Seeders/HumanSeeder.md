# HumanSeeder

## Start here to add content

Edit [HumanSeeder.Bodyparts.cs](../../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.Bodyparts.cs): **SetupBodyparts / CreateBodypart for anatomy; use the corresponding wear/characteristic/liquid partial for those families**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Adds a human race and associated data to the game.
Baseline availability: **Enabled**. The entrypoint is [HumanSeeder.cs](../../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `balance`, `model`, `inventory`, `sever`, `bones`, `distinctive`, `nonbinary`, `includeextraperson`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- Skills must already be seeded.
- The Time seeder must have installed at least one calendar.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Important branch distinction.** `HumanSeeder.SeedData` begins a transaction, merges previously recorded choices, then tests whether `context.Races` contains a race named `Humanoid`. That branch is a *repair/refresh-only fast path*: it calls `RefreshExistingHumanCombatBalance`, satiation/admin-avatar fixes, and wear-profile refresh for existing `BodyProto("Humanoid")`; optionally seeds missing disfigurement templates on `Organic Humanoid`; then commits and returns at roughly lines 214-260. The initial install path below this `if` is not run afterward. Therefore the initial setup's later transaction commit(s) are alternatives to this early branch, not sequential commits in a single normal call. The two commit locations are alternative branches.

**Body part trace and add recipe.** An existing part such as `abdomen` is authored in `SetupBodyparts` in [HumanSeeder.Bodyparts.cs](../../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.Bodyparts.cs) around line 650 as `CreateBodypart(baseHumanoid, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front, ...)`; bodypart creator (around 260-310) populates `Name` from alias, `Description` from name, `BodypartShape` from shape, and type/alignment/material/size/anatomy/damage/health fields and links it to parent `BodyProto`, then `_context.BodypartProtos.Add(bodypart)`. Organic body is separate and often copies framework parts plus organs. A new body part needs an anatomy placement call with unique alias, player descriptions, type, parent alias, alignment, hit chance/health/armor details, followed by any needed organ linkage and corresponding organic-body mapping. Search all consumers keyed by alias: hit chance (`GetHumanRelativeHitChance`), cranial armor selection, characteristics, wear profiles, health target lists, and disfigurements. A part that only compiles can still be absent from bodily coverage/armor, or reference a missing parent. IDs for editable disfigurement templates and nested body structures have distinct handling; follow existing helpers, not manually assigned IDs unless the specific table requires it.

**Other authoring surfaces and lifetime.** Characteristics definitions/profiles in [HumanSeeder.Characteristics.cs](../../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.Characteristics.cs), wear layers in top-level [HumanSeeder.WearLayers.cs](../../../DatabaseSeeder/Seeders/HumanSeeder.WearLayers.cs), wear profile data in `HumanSeeder/HumanSeeder.WearProfiles.cs`, industrialised profiles specifically in top-level sibling `Seeders/HumanSeeder.WearProfiles.Industrialised.cs`, and liquid/disfigurement/combat content have their own helpers. `SeedData` initial route creates body/race/health/characteristic structures with multiple SaveChanges before its final commit (but not all are executed after early Humanoid refresh). Existing branch only adds newly missing disfigurement templates and refreshes a few owned surfaces. Add-one guidance must follow the relevant subcatalogue and linked-use grep; there is no single Human record list. Read the human seeder description markup guide when changing generated prose. Tests include `HumanSeederWearProfileTests`, `HumanSeederLiquidTests`, `HumanSeederAdminAvatarLanguageTests`; none run.

## Declared rerun and ownership contract

Metadata declares `Idempotent / FullReconcile`.

Reruns retain the installed humanoid shape and reconcile stock body, race, health, culture and wear foundations.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

The full-bones initial install emits `Human Natural Bone Armour` through `BuildHumanBoneArmourDefinition`. Its chopping dissipation formula is `max(damage*0.1,damage-(quality * 2 * strength/115000))`: it retains the 10% damage floor and the existing quality/material reduction. The surplus closing parenthesis in the previous default is corrected at source. [NaturalBoneArmourSeederTests](../../../DatabaseSeeder%20Unit%20Tests/NaturalBoneArmourSeederTests.cs) loads all six formula maps and checks chopping results through the runtime expression engine. This source correction does not reconcile existing builder-edited armour definitions on a normal seeder rerun.

The Armageddon owned-world qualification fixture has a separate, deliberately narrow repair mode in [OwnedBoneArmourRepair](../../../scripts/FuryCalmSmokeWorld/OwnedBoneArmourRepair.cs). It pins the complete malformed definitions from the retained failed receipt, checks identities and metadata, updates both rows atomically, proves rollback and a no-op rerun, and checks preservation of all other table contents. Any other definition or metadata is refused. This fixture is restricted to its retained disposable world and is not a production migration.

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
