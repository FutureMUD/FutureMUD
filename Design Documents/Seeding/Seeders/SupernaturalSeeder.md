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

Named entry: SupernaturalSeeder.Definitions.BuildTemplates adds Vampire as family Undead, Organic Humanoid body, Normal size, Material plane, NonLiving needs, stats, attacks, disfigurements, Undead Remnant corpse and Undead Name culture. SeedData calls SeedOrRefreshRace for every template. That method finds Race by name or constructs it; on missing adds _context.Races and saves. Then it applies needs, attributes, upserts ethnicity, characteristics, racial bodypart usages, natural attacks and descriptions, followed by save. Add a Template(...) call with supported body/family/planar/needs profiles, Stats, attack/bodypart mappings, corpse and name culture keys, and prose; a new custom body must be added to BuildBodyCatalogue. Reruns refresh named race fields and support rows, so mapped builder changes are vulnerable to overwrite. Explicit transaction.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Undead Humanoid`, use Vampire as the complete Template shape, then deliberately choose family, body, plane, needs, stats, attacks, corpse/name-culture and prose. Add required named support definitions before introducing new keys. Verify SeedOrRefreshRace and its ethnicity/bodypart/attack/description joins; a same-name template refresh rewrites mapped stock fields. These instructions were source-checked but not executed.

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
