# SkillPackageSeeder

## Start here to add content

Edit [SkillPackageSeeder.cs](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs): **SkillDetails entries in the relevant skill family**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up a complete package of skills.
Baseline availability: **Enabled**. The entrypoint is [SkillPackageSeeder.cs](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `branching`, `skillcapmodel`, `skillgainmodel`, `complexity`, `gerund`, `modern`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- Attributes must already be seeded.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing item and write path.** The package implementation has a large inline `SkillDetails` list beginning near line 130. `new SkillDetails("Swimming", "Swimming", "Athletic", "min(99,2*str + 3*con)", "General", "General", true, 1.0, ...)` (around line 137) defines gerund/imperative labels, group, attribute formula, decorator and trait groups, availability and branch multiplier. `SeedData` at 536 sets up checks, skill improvers, decorators, `SeedSkills`, Admin Speech language and checks, then universal NPC skill package. In `SeedSkills` (1417+), local `AddSkill` resolves a final stock skill name based on `GerundName`/`ImperativeName` plus the `gerund` choice, transforms the formula through `ReplaceAttributeReferences`, ensures a named cap expression, saves it, calls `EnsureSkillDefinition`, assigning decorator, trait group, expression, improver, flags/difficulties and branch multiplier; it may then ensure helpfiles and saves. Thus a record is a template spec transformed into TraitExpression and TraitDefinition, not copied verbatim.

**Add-one recipe.** Insert a new `SkillDetails` in the correct grouping in the list, e.g. `new SkillDetails("Sailing", "Sail", "Transportation", "min(99,3*dex + 2*int)", "General", "General", true, 1.0, ...)`, but match the actual record's full positional signature (inspect declaration at line 130 and existing nearby entries before editing). `Group` controls semantic grouping, formula supports `str/dex/agi/con/int/wil/per` tokens replaced with found stock attribute aliases+IDs, decorator/group labels must match initialized decorator sets, `Available` controls availability prog and branch multiplier affects training. Add a check mapping through check setup only if the game references it. Add corresponding NPC skill package inclusion if NPCs need it; default helper creates the universal package from the returned skills dictionary. Do not add rows to `SkillSeeder` for this full package. No external asset.

The package separately seeds check templates and calls `SeedChecks`; a skill detail itself does not declare a `CheckType` association, so avoid inventing a one-to-one assumption. For help text, `EnsureHelpfile` currently seeds skill help only where content exists in the data path; use the existing logic and avoid fabricating user documentation in a second catalog. Verify duplicate names against `ResolveSeededSkillName`: gerund selection can change the persisted canonical label (e.g. from the imperative form) and existing traits participate in that resolution. `ShouldSeedData` checks shared scaffold markers, attributes and the universal package; partial installs classify differently from a fresh install. A package addition should preserve those markers so subsequent reruns remain detectable.

**Rerun.** Idempotent/RepairExisting by metadata; its stable name/cap helpers reuse rows and it is an alternative to simple `SkillSeeder`, not companion. Check tests [SkillPackageSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/SkillPackageSeederTests.cs) (package presence and stock shapes); no test run.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the stock skill package templates, improvers, admin language, checks, and seeded skills by stable names.

This remains an alternative to the Skill Example seeder, not a companion package.

Stock skill-package records are keyed by check type, template name, decorator name, improver name, and seeded trait/language names.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
