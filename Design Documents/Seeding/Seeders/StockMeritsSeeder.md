# StockMeritsSeeder

## Start here to add content

Edit [StockMeritsSeeder.cs](../../../DatabaseSeeder/Seeders/StockMeritsSeeder/StockMeritsSeeder.cs): **StockMerits blueprint array and matching effect factory**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Stock merits, flaws, and helper progs for character creation.
Baseline availability: **Enabled**. The entrypoint is [StockMeritsSeeder.cs](../../../DatabaseSeeder/Seeders/StockMeritsSeeder/StockMeritsSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: none in the concrete question sequence. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- The Human seeder must have installed the Human race.
- Chargen must already include a merit or quirk selection screen.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Definition-to-write trace.** `StockMerits` inline catalogue near lines 67-80 contains `new("Steady Presence", "All Check Bonus", MeritType.Merit, context => AllCheckMerit(context, 1.0, blurb, description))`. `SeedData` builds a `StockMeritContext`, ensures helper progs, loops blueprints and calls `EnsureCharacterMerit`. That helper (632+) skips if `IsApplicable` false or factory returns null; obtains XML definition, then uses `SeederRepeatabilityHelper.EnsureNamedEntity(context.Merits, blueprint.Name, x=>x.Name, factory)`. It then assigns name, merit/flaw type, character scope, clears parent ID, and **always assigns the generated `Definition`**, so a canonical-name existing row's definition is overwritten on rerun. Canonical names are treated as stock-owned by this helper. For “Steady Presence”, `AllCheckMerit` calls `MeritRoot` with a `bonus=1.0` XML attribute and carries blurb/description. The stored object is a `Merit` row with serialized effect XML.

**Add-one recipe.** Add a `StockMeritBlueprint` with a unique exact name, existing merit-type string understood by runtime merit factory, `MeritType.Merit` or `.Flaw`, and a factory matching a supported effect family. For a simple all-check bonus, use `AllCheckMerit(context, numericBonus, userFacingBlurb, descriptionTemplate)` as neighbor. The template helper constructs XML with scope and required attributes; for conditional applicability, ensure referenced `StockMeritContext` prog exists and pass its ID into factory helper. More involved effect types require a corresponding runtime-supported `MeritType` and well-formed XML attributes, not just a new label in the seeder list. No registration or external asset. Extend `StockMeritNamesForTesting`-based tests / dependency audit as appropriate, without weakening other catalogue assertions.

**Rerun/readiness.** Registry entry at 182 says Idempotent/RepairExisting (not additive). Code's name upsert and unconditional field assignment mean definition/type updates for matched names; applicability false skips definition and does not delete an old row. `SeedData` has no explicit transaction. It saves helper progs/planes first, then merits, so a later exception can leave earlier saves committed. Readiness scans helper progs and checks for missing dependencies, then package presence. [StockMeritsSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/StockMeritsSeederTests.cs); not run.

This matters for balance edits: changing the factory for `Steady Presence` is a stock correction that the next seeder run will write onto the same canonical `Merit` row by name. It will also reset `ParentId` to null and rescope it to character-level, so this helper treats that named merit as seeder-controlled data. A custom merit should use a distinct name rather than replacing a stock blueprint name. A blueprint whose `Definition` returns null or is inapplicable is skipped, not deleted; removing a name from the list does not imply cleanup. The tests source confirms a name catalogue and an expected dependency set; that does not prove every merit effect has been exercised by runtime integration tests.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the stock merits, flaws, and helper FutureProgs by canonical names.

Reruns repair missing stock merits, flaws, and tag-driven helper progs without changing chargen mode or chargen-resource costs.

Stock merit content is tracked by stable merit names and helper FutureProg function names.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
