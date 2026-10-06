# ChargenSeeder

## Start here to add content

Edit [ChargenSeeder.cs](../../../DatabaseSeeder/Seeders/ChargenSeeder/ChargenSeeder.cs): **AddStage call and serialized definition for an existing screen; RequiredChargenStages for a new stage**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up Character Creation and Guest Logins.
Baseline availability: **Enabled**. The entrypoint is [ChargenSeeder.cs](../../../DatabaseSeeder/Seeders/ChargenSeeder/ChargenSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `rpp`, `rppname`, `bp`, `class`, `subclass`, `role-first`, `attributemode`, `skillmode`, `merits`, `customdescs`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- The Human seeder must have installed the Human race.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing item and write path.** `RequiredChargenStages` in `ChargenSeeder.cs:16-42` begins with `Welcome`, and `SeedData` creates/repairs stage records in the main transaction (`SeedData` at 223, commits at 921). The `AddStage` local function around line 675 takes a `ChargenStage`, chargen type string, next stage, stage XML definition, and dependencies. It queries existing `ChargenScreenStoryboards` for that stage, prefers a row already using the requested type, otherwise uses first existing, else creates a row and calls `context.ChargenScreenStoryboards.Add`. It removes duplicate storyboards, sets stage/order/nextstage and assigns `ChargenType` for new rows, a blank type or a case-insensitive same-type match; a differing nonblank type is retained, fills `StageDefinition` only when currently blank, records it in stage lookup, and attaches dependency rows. The concrete Welcome entry invokes `AddStage(ChargenStage.Welcome, "WelcomeScreen", ChargenStage.SpecialApplication, "<Screen>…")`; its XML contains player welcome blurbs. The seeder later saves and provisions remaining required progs/roles/configuration. This is a storyboard table entry with a persisted XML contract, not merely an enum.

**Add-one recipe.** A new optional screen step requires (1) a runtime `ChargenStage` enum value and its consumers/handler before the seeder can reference it, (2) insert it into `RequiredChargenStages` in correct sequence, (3) add a matching `AddStage(newStage, "NewScreen", nextStage, valid serialized screen definition, dependencies)` call in the setup chain, and (4) update stage progression/readiness expectations. Existing `AddStage` is the registration point; a new string alone is not enough because stage order/next stage are explicitly stored. Provide a legitimate XML stage body for the runtime chargen type and preserve hand-edited nonempty definitions: updater only fills definition when empty, while it does deliberately remove duplicate stage rows. If the new stage requires question-specific settings, add validation and seed those separately with the matching stable key. The installer questions already ask feature toggles such as classes/subclasses and account resource; adding another choice requires prompt, filter, validator, `questionAnswers` read, and the branch that consumes it.

**Safety/tests.** Readiness requires an account and `Human` race; without any chargen anchor it offers install, otherwise classifies completeness over the required stages, default starting-location role, always-required progs and conditional point-buy/skill-picker progs (927+). `SeedData` is transactional, multiple `SaveChanges`, commits at 921. `SafeToRunMoreThanOnce=true`; registry says idempotent/repair-existing. Existing storyboards retain nonempty authored definitions for same type; progression fields are normalized, but a differing nonblank chargen type is retained. [ChargenSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ChargenSeederTests.cs) exercises stage/prog contract; no test run in this mapping.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse stock chargen resources, special-application static settings, helper progs, canonical storyboard stages, and the default starting-location role by stable keys.

Reruns repair missing stock screens, helper progs, dependencies, and special-application settings without creating duplicate storyboard rows for the same chargen stage.

Chargen storyboards are tracked as one canonical row per chargen stage, helper progs are tracked by function name, and the default starting-location role is tracked by stable name.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
