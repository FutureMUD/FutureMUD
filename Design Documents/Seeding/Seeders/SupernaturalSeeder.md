# SupernaturalSeeder

## Start here to add content

Edit [SupernaturalSeeder.Definitions.cs](../../../DatabaseSeeder/Seeders/SupernaturalSeeder/SupernaturalSeeder.Definitions.cs): **BuildTemplates / Template**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs stock supernatural races, forms, planar bodies and undead examples.
Baseline availability: **Enabled**. [SupernaturalSeeder.cs](../../../DatabaseSeeder/Seeders/SupernaturalSeeder/SupernaturalSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Shared operator questions from [NonHumanSeederQuestions.GetQuestions](../../../DatabaseSeeder/Seeders/Utilities/NonHumans/NonHumanSeederQuestions.cs): `model` (`hp`, `hpplus`, `full`), `random` (`static`, `partial`, `random`) and `messagestyle` (`compact`, `sentences`, `sparse`). These packages use the default structural filter (always true); randomness is suppressed with combat rebalance, and style when a prior choice is recorded. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- Human, animal, and mythical body frameworks must already be installed.
- Human, organic humanoid, and wolf race foundations must already exist.
- Shared humanoid characteristic profiles must already exist.
- Stock natural attacks and non-human health strategies must already exist.
- Stock helper progs, corpse models, and at least one calendar must already exist.
- The complete supernatural foundation, including blood, breathable atmosphere, required attributes and health strategies, must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: SupernaturalSeeder.Definitions.BuildTemplates adds Vampire as Undead, Organic Humanoid body, Normal size, Material plane and NonLiving needs, with authored stats/attacks/disfigurements/corpse/name culture. Before returning the catalogue it applies **`ApplySupernaturalCombatBalance`**. Every new exact name must appear in that transform's combat-tier switch; an unknown name throws `InvalidOperationException` during catalogue construction. Vampire maps to `SeriousThreat` (tier value 2). Strength has named exceptions but otherwise uses `14 + tier * 11`; Vampire consequently receives effective Strength 36. Its tier/family rules yield Constitution 42, Agility 32, Dexterity 32, Willpower 48, Perception 25 and Aura 34. [WithEffectiveTargets](../../../DatabaseSeeder/Seeders/Utilities/NonHumans/NonHumanCombatBalanceProfile.cs) replaces all seven source offsets with target minus 11, retaining intelligence/willpower/perception/aura dice expressions. Editing `Stats(...)` offsets alone therefore does not set final values. The transform also replaces health multiplier (1.25 here) and CombatBalance: Undead Humanoid baseline, pain multiplier 5.5, Substandard natural-armour quality, `undead-will` attack profile, no charge at Normal size and the first template attack as signature fallback. These are source-computed values, not executed gameplay measurements.

`SeedData` calls `SeedOrRefreshRace` for each transformed template. That method finds Race by name or constructs it, adds missing rows and saves, then applies needs/transformed attributes and upserts ethnicity, characteristics, bodypart usages, attacks and descriptions. New custom bodies need BuildBodyCatalogue integration. Reruns refresh named race fields and support rows, so mapped builder changes are vulnerable to overwrite. Explicit transaction.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For `Example Undead Humanoid`, use Vampire as the complete Template shape, deliberately selecting family/body/plane/needs/attacks/corpse/name culture/prose. **Add its exact name to `ApplySupernaturalCombatBalance`'s tier switch**; a proposed Vampire-like `SeriousThreat` entry uses the default Strength target 36 unless deliberately given an explicit Strength exception. Unlike the mythical transform, an additional Strength switch arm is optional here because a formula fallback exists. Review the other tier/family/name/size rules for all effective targets, health, pain, armour quality, baseline, attack profile, charge and signature; these replace the relevant authored values. Add required support definitions before introducing new keys.

For a future executable addition, extend [SupernaturalSeederTemplateTests](../../../DatabaseSeeder%20Unit%20Tests/SupernaturalSeederTemplateTests.cs) to enumerate `TemplatesForTesting` and assert the new transformed tier, seven target-minus-11 offsets, retained dice expressions and resulting health/combat profile. Follow [combat reconciliation tests](../../../DatabaseSeeder%20Unit%20Tests/NonHumanCombatBalanceReconciliationTests.cs) for relevant stock/custom attack boundaries, then verify SeedOrRefreshRace and its first-install/rerun joins; a same-name refresh rewrites mapped stock fields. These are source-backed, unexecuted recommendations, not a qualified recipe or newly admitted race.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns install or refresh the stock supernatural race catalogue, body prototypes, form merits, cultures, name cultures, attacks, and non-breather settings.

Existing builder-customized worlds keep their records; stock-owned supernatural records are repaired by stable names without deleting custom extensions.

Supernatural stock content is tracked by stable race, body, culture, name-culture, merit, attack, and corpse-model names.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [SupernaturalSeederTemplateTests.cs](../../../DatabaseSeeder%20Unit%20Tests/SupernaturalSeederTemplateTests.cs)
- [Supernatural_Seeder.md](../../../Design%20Documents/Magic/Supernatural_Seeder.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
