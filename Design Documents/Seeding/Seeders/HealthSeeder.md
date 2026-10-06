# HealthSeeder

## Start here to add content

Edit [HealthSeeder.cs](../../../DatabaseSeeder/Seeders/HealthSeeder/HealthSeeder.cs): **The selected Seed*HumanSurgery or drug helper and its companion catalogue-name list**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up surgeries, drugs, and medical starter content.
Baseline availability: **Enabled**. The entrypoint is [HealthSeeder.cs](../../../DatabaseSeeder/Seeders/HealthSeeder/HealthSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `techlevel`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- The Human seeder must have installed Organic Humanoid.
- Required stock medical tool tags must exist.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Named record route.** The source has multiple tech-level lists (`PrimitiveHealthProcedures`, etc.) but those arrays supply catalogue/audit names, not complete emitted surgery definitions by themselves. In `HealthSeeder.SeedData`, required `Organic Humanoid` body is resolved; transaction starts; it seeds knowledge, surgery, drugs and era gases, plus era-specific liquids/medical vessels, then reconciles FreeKnowledge and commits. `SeedSurgery` dispatches selected `techlevel` to primitive/pre-modern/modern routines. `SeedPrimitiveHumanSurgery` contains `AddHastyTriage("Medicine", "Human Medicine", _humanBody, -3.0)`; primitive path writes human procedure through AddHastyTriage helper. `AddHastyTriage` family helper ultimately uses `_context.SurgicalProcedures` and `SeederRepeatabilityHelper.EnsureNamedEntity` (around 1735-1745), creating `SurgicalProcedure` when absent, then associates the required phases (`SurgicalProcedurePhases`) and anatomy/knowledge references. Thus “Hasty Triage” exact name is represented in code call and generated row/phase graph.

**Add-one recipe.** To add an era-appropriate procedure, add the name to only the intended matching procedure array (`PrimitiveHealthProcedures`, `PreModernHealthProcedures`, or `ModernHealthProcedures`, and veterinary list only if applicable), then add a call in the corresponding `Seed*HumanSurgery` routine to the semantically appropriate helper (`AddHastyTriage`, `AddTriage`, `AddPrimitiveStitching`, etc.) with real required knowledge name, “Human Medicine” category, `_humanBody`, success/difficulty modifier and description where helper signature requests them. Ensure matching knowledge exists in `SeedKnowledges` and optional tech-only requirements are conditionally provisioned; make the helper-generated phase sequence suitable to the procedure, rather than adding only an audit string. When a new procedure is not merely an alias of an existing one, inspect diagnosis, required tool tags and body part targets in specific helper. A worked HastyTriage-derived addition called `Example Field Assessment` must explicitly pass `name: "Example Field Assessment"` and `procedureName: "example field assessment"` to `AddHastyTriage`; otherwise the defaults target the existing Hasty Triage row. Include that same exact name in the selected audit list. `AddSurgicalProcedure` ensures by name, rewrites mapped fields and removes/replaces the phase graph, so those named procedure fields and phases are stock-controlled on rerun. Use this helper only when its actual triage behavior is appropriate; a different operation needs a matching runtime procedure type and serialized definition.

**Selection/prerequisites.** `SelectedTechLevel` has primitive, medieval/pre-modern/Renaissance/early modern, modern branches, with extra liquid/vessel modules for medieval and later. `ShouldSeedData` checks account, selected medical skill/knowledge/tools/corpse setup; metadata imposes prerequisites (see registry 308-318). The top-level `Enabled=true`. Main transaction has explicit Begin/Commit but no visible catch rollback in cited entry block. [HealthSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/HealthSeederTests.cs) for starter pack; no test run.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse stock medical knowledges, procedures, phases, and drugs by stable names.

Forward-only upgrades add or refresh higher-tech stock content without removing lower-tech content.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
