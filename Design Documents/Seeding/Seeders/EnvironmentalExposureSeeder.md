# EnvironmentalExposureSeeder

## Start here to add content

Edit [EnvironmentalExposureSeeder.Profiles.cs](../../../DatabaseSeeder/Seeders/EnvironmentalExposureSeeder/EnvironmentalExposureSeeder.Profiles.cs): **Profiles and Response for hazards; preparations/equipment have their own partials**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Material-selective hazards, protective preparations and demonstration equipment.
Baseline availability: **Enabled**. The entrypoint is [EnvironmentalExposureSeeder.cs](../../../DatabaseSeeder/Seeders/EnvironmentalExposureSeeder/EnvironmentalExposureSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `natural`, `fantasy`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- Core materials and fluids must be installed.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Catalogue-to-persistence.** `Profiles` contains real `HazardProfile("lava", false, false, "heat", Persistent: true)`. `SeedData` → `InstallProfiles(false)` → `Liquid("lava", "natural")` resolves an existing material or uses `Owned` to create an owned liquid → each installed solid material is classified by `ExposureCatalogue` → `Response` supplies game-balance rates → reaction XML receives stable IDs derived from profile/material/route → `ReconcileRules("Liquid", liquid.Id, ...)` merges rules and persistence against `SeederManagedRecord.SeedBaseline` → assigns `Liquid.SurfaceReactionInfo` → SaveChanges/commit. Gas profiles instead write `Gas.SurfaceReactionInfo`; tissue gas contact and inhalation have separate rule IDs/rates. These are overlays on fluids, not rows in a standalone hazard-profile table.

`Owned` resolves persisted logical identity, rejects unowned name collisions for whole-row creation and uses `SeederManagedRecordReconciler` for scalar baselines. `ReconcileRules` separately owns reaction-key baselines and protects malformed/edited XML. Existing fluids may be reused for overlays without the seeder owning every scalar. Preparations/equipment use their own inline specifications and entity writes in the corresponding partials.

**Add-one recipe.** To add a new natural hazard profile, append a `HazardProfile` with unique exact `Name`, correct `Gas`, `Fantasy=false`, supported category (follow existing “heat”, “chemical”, “spectral”, “holy”), and `MinimumTemperature` only if meaningful; set `Persistent` intentionally. Then inspect `InstallProfiles` to ensure every profile field is represented in desired XML and is compatible with runtime parser. For actual material damage, profile listing is insufficient: extend `Response(profile,family,route)` switch for each relevant material family/route and add/update response/resistance semantics; if it's gas, add the gas material setup where appropriate. Add equipment/preparation only when desired, in their separate partial catalogs. Stable `StableId(key)` is SHA-256 namespace-based; `Record` key should use a permanent semantic key, not a spelling that may later change. No external catalogue data file.

**Operator/failure.** Natural and fantasy yes/no keys; prerequisites require at least one material and liquid. The seeder never enables global exposure or creates rooms. Transaction `using` ensures disposal and explicit commit after save; profile name collision with no owned record is preserved and reported instead of adopted. Metadata says idempotent/repair-existing and optional nature. Tests [EnvironmentalExposureSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/EnvironmentalExposureSeederTests.cs) include profiles/material response and ownership paths; not run.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reconciles exposure-owned fields, reactions, preparations and demonstrations; preserves builder changes and reports conflicts.

Optional natural and fantasy exposure content; never enables an existing world or creates live hazardous rooms.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
