# SkillSeeder

## Start here to add content

Edit [SkillSeeder.cs](../../../DatabaseSeeder/Seeders/SkillSeeder/SkillSeeder.cs): **SeederQuestions / SeedSkills for installer-selected example names**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up templates and examples for skills.
Baseline availability: **Enabled**. The entrypoint is [SkillSeeder.cs](../../../DatabaseSeeder/Seeders/SkillSeeder/SkillSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `branching`, `skillcapmodel`, `skillgainmodel`, `exampleskill`, `skillattribute`, `examplelanguage`, `languageattribute`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- Attributes must already be seeded.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing item and write path.** `SkillSeeder.SeedData` opens transaction and calls `SeedChecks` then `SeedSkills` (127-133). `SeederQuestions` has `exampleskill` (free-form comma-separated names checked only for nonblank input); existing-trait validation belongs to `skillattribute` plus skill-cap model and attribute selection. `SeedSkills` title-cases comma-delimited names from `questionAnswers["exampleskill"]`, creates or reuses cap `TraitExpression` rows by `EnsureTraitExpression`, saves them, and calls `EnsureSkillDefinition` for each name. For each skill it assigns Type 0, general decorator/group, AlwaysTrue availability/learnable, AlwaysFalse teachable, difficulty 7, visible, expression ID/reference, general improver, derived type 0, blank chargen blurb and branch multiplier 1.0, then saves. `EnsureSkillDefinition` writes the `TraitDefinitions` row. If an example language is selected, parallel code creates an Admin skill, Language row and native/foreign Accents, so the language is a linked set rather than just a skill.

**Worked addition: an example skill named Surveying.** Enter `Surveying` in `exampleskill` (or append it to the comma-separated names) and choose an existing attribute through `skillattribute`. No source name catalogue or validator extension is needed: the existing split/titlecase path builds the cap expression for the selected `rpi`, `class` or `flat` model and calls `EnsureSkillDefinition`. Check the cap model's formula and selected attribute references, persisted normalized name, improver/decorator and availability progs. A new gameplay action still needs its runtime check integration; naming a skill does not create an action. If installing a mandatory stock catalogue, use the distinct `SkillPackageSeeder` architecture or deliberately implement an unconditional source definition; there is no existing stock-name list to append here. This example is an unexecuted operator recipe.

**Ownership/failure.** Readiness requires account and one Type=1 attribute, then uses the presence of scaffold markers/skills and classifies missing/partial/full. Existing admin language marker warns `MayAlreadyBeInstalled`. Stable helper names make reruns reuse definitions; transaction Begin/Commit but no explicit catch rollback in this class. Relevant shared [SkillSeederBase.cs](../../../DatabaseSeeder/Seeders/Utilities/Skills/SkillSeederBase.cs) owns helper mechanics, not the concrete selectable example text. Test inventory has [RpiLegacySkillBaselineTests.cs](../../../DatabaseSeeder%20Unit%20Tests/RpiLegacySkillBaselineTests.cs), but no specific SkillSeeder test was confirmed; do not claim coverage beyond the tested assertions after reading that source. None run here.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the shared skill scaffolding and example records by stable names.

This remains an alternative to the full Skill Package seeder, not a companion package.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
