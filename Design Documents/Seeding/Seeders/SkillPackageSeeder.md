# SkillPackageSeeder

## Start here to add content

Edit [SkillPackageSeeder.cs](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs): **SkillDetails entries in the relevant skill family**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up a complete package of skills.
Baseline availability: **Enabled**. The entrypoint is [SkillPackageSeeder.cs](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Choose one `skillpreset`, using the same named-option presentation as calendar/currency selection. The complete inventory is:

| Package | Non-modern presets | Modern presets |
| --- | --- | --- |
| Simple | `Simple-Gerund`, `Simple-NonGerund` | `Simple-Gerund-Modern`, `Simple-NonGerund-Modern` |
| Complex | `Complex-Gerund`, `Complex-NonGerund` | `Complex-Gerund-Modern`, `Complex-NonGerund-Modern` |
| RPI (Complex plus legacy RPI skills) | `RPI-Gerund`, `RPI-NonGerund` | `RPI-Gerund-Modern`, `RPI-NonGerund-Modern` |
| Armageddon | `Armageddon` | Not offered |

Ordinary presets encode package shape, naming style and inclusion of the existing modern skill list. They retain independent `skillcapmodel` (RPI/Class/Flat), `skillgainmodel` (RPI/LabMUD/Armageddon/SuccessTree) and `branching` controls; the preset does not choose these policies on the builder's behalf. This reduces six fresh prompts to four. Armageddon needs only the preset prompt: it supplies canonical names, class caps, Armageddon improvement, `branching=no` and `modern=no`, then executes the real canonical skills/checks/expressions/static bindings path. Magic mode remains a later Setting choice.

Declared IDs are `skillpreset` plus the six legacy IDs `branching`, `skillcapmodel`, `skillgainmodel`, `complexity`, `gerund`, `modern`. The replaced package/name/modern questions remain declared but inactive for answer/replay compatibility. The three remaining controls are also inactive for Armageddon. [SkillPackageSeeder.Presets.cs](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.Presets.cs) owns the inventory, filters and pure answer expansion.

Complete valid old six-key direct calls remain supported. Complete valid remembered legacy answers suggest their matching new preset; the console requires explicit acceptance, and no preset is guessed from installed skill names or incomplete history. A stale saved preset that conflicts with complete latest legacy history is rejected by both the question validator and installation preflight. Recorded package/name/modern shape is retained on supported reruns; changing it requires a separate migration. New successful runs persist both the preset and expanded legacy answers without deleting old history. Explicit conflicting preset/legacy values fail before writes. Normalization runs at the beginning of installation and again through the shared executor for answer persistence. The three independent ordinary policies must be supplied; missing values do not acquire new defaults. All five Debug replay profiles retain all seven declared IDs, including inactive answers.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- Attributes must already be seeded.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing item and write path.** The package implementation has a large inline `SkillDetails` list beginning near line 130. `new SkillDetails("Swimming", "Swimming", "Athletic", "min(99,2*str + 3*con)", "General", "General", true, 1.0, ...)` (around line 137) defines gerund/imperative labels, group, attribute formula, decorator and trait groups, availability and branch multiplier. `SeedData` at 536 sets up checks, skill improvers, decorators, `SeedSkills`, Admin Speech language and checks, then universal NPC skill package. In `SeedSkills` (1417+), local `AddSkill` resolves a final stock skill name based on `GerundName`/`ImperativeName` plus the `gerund` choice, transforms the formula through `ReplaceAttributeReferences`, ensures a named cap expression, saves it, calls `EnsureSkillDefinition`, assigning decorator, trait group, expression, improver, flags/difficulties and branch multiplier; it may then ensure helpfiles and saves. Thus a record is a template spec transformed into TraitExpression and TraitDefinition, not copied verbatim.

**Add-one recipe.** Insert a new `SkillDetails` in the correct grouping in the list, e.g. `new SkillDetails("Sailing", "Sail", "Transportation", "min(99,3*dex + 2*int)", "General", "General", true, 1.0, ...)`, but match the actual record's full positional signature (inspect declaration at line 130 and existing nearby entries before editing). `Group` controls semantic grouping, formula supports `str/dex/agi/con/int/wil/per` tokens replaced with found stock attribute aliases+IDs, decorator/group labels must match initialized decorator sets, `Available` controls availability prog and branch multiplier affects training. Add a check mapping through check setup only if the game references it. Add corresponding NPC skill package inclusion if NPCs need it; default helper creates the universal package from the returned skills dictionary. Do not add rows to `SkillSeeder` for this full package. No external asset.

The package separately seeds check templates and calls `SeedChecks`; a skill detail itself does not declare a `CheckType` association, so avoid inventing a one-to-one assumption. For help text, `EnsureHelpfile` currently seeds skill help only where content exists in the data path; use the existing logic and avoid fabricating user documentation in a second catalog. Verify duplicate names against `ResolveSeededSkillName`: gerund selection can change the persisted canonical label (e.g. from the imperative form) and existing traits participate in that resolution. `ShouldSeedData` checks shared scaffold markers, attributes and the universal package; partial installs classify differently from a fresh install. A package addition should preserve those markers so subsequent reruns remain detectable.

**Rerun.** Idempotent/RepairExisting by metadata; its stable name/cap helpers reuse rows and it is an alternative to simple `SkillSeeder`, not companion. Presets use the existing installers: they do not broaden generic field preservation or make changing an installed package a supported world conversion. Armageddon rejects switching to an ordinary package and retains owned IDs, builder changes and player values on supported reruns. See [SkillPackagePresetTests.cs](../../../DatabaseSeeder%20Unit%20Tests/SkillPackagePresetTests.cs) for every preset's installed names/shape, old-answer inference/expansion/persistence, preserving resume and conflicts before writes, and [SkillPackageSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/SkillPackageSeederTests.cs) for stock shapes.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the stock skill package templates, improvers, admin language, checks, and seeded skills by stable names.

Check-template difficulty rows reconcile by their persisted `(CheckTemplateId, Difficulty)` key. This loads rows explicitly on a cold context, updates existing stock modifiers in place, adds missing difficulties and removes no-longer-defined difficulties. It avoids the previous cold-rerun duplicate insert caused by assuming an unloaded navigation contained the existing rows. Other ordinary stock refresh semantics are unchanged.

This remains an alternative to the Skill Example seeder, not a companion package.

Stock skill-package records are keyed by check type, template name, decorator name, improver name, and seeded trait/language names.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

The distinct Peek skill is included in both simple and complex packages. Its attempt/reveal bindings use `PeekSeederHelper`, with no-improvement/no-branching revelation and additive explicit existing-world binding. See [Peek covert inspection](../../Characters/Peek_Inspection.md) for command behavior, stable Armageddon keys and upgrade boundaries.

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.

## Staged Armageddon new-world choice

20 October preset verification: full DatabaseSeeder run `20261020T120610Z-758ae8332326` passed2048, failed0, skipped3 platform-specific launcher cases, with stable source. It covers all13 installed preset options, strict replay inventories, old/new answer persistence, stale/conflicting inputs, canonical Armageddon bindings and cold-context resume/rerun. This is automated fixture evidence, not a current native MySQL/profile certificate. An earlier focused run's cold-template failure and alias-count test correction are retained in `20261020T120152Z-9cf01333a7f9`; its failures are superseded by the corrected full run.

See [Armageddon Setting](ArmageddonSettingSeeder.md) for the explicit early choice, immutable canonical trait IDs, bootstrap dependencies, builder-preserving rerun boundary and pending role/heritage stages. This choice does not convert an existing stock world.
