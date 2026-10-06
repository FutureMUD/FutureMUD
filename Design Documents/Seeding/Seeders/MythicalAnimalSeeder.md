# MythicalAnimalSeeder

## Start here to add content

Edit [MythicalAnimalSeeder.Definitions.cs](../../../DatabaseSeeder/Seeders/MythicalAnimalSeeder/MythicalAnimalSeeder.Definitions.cs): **Templates and the owning support partial**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs a catalogue of mythic beasts and hybrid folk.
Baseline availability: **Enabled**. [MythicalAnimalSeeder.cs](../../../DatabaseSeeder/Seeders/MythicalAnimalSeeder/MythicalAnimalSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Shared operator questions from [NonHumanSeederQuestions.GetQuestions](../../../DatabaseSeeder/Seeders/Utilities/NonHumans/NonHumanSeederQuestions.cs): `model` (`hp`, `hpplus`, `full`), `random` (`static`, `partial`, `random`) and `messagestyle` (`compact`, `sentences`, `sparse`). These packages use the default structural filter (always true); randomness is suppressed with combat rebalance, and style when a prior choice is recorded. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- Human and animal body frameworks must already be installed.
- Human race foundations must already exist.
- Shared humanoid characteristic profiles must already exist.
- Stock organic corpse models and non-human strategies must already exist.
- The complete mythical-animal foundation, including height-weight models and Acid Spit, must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: MythicalAnimalSeeder.Definitions.BuildTemplates includes Giant Ant as BeastRace("Giant Ant","Insectoid",Large,"Giant Insect",GreatBeast(), prose, variants, attacks Carnivore Bite and Bite mapped to mandibles, BestialStats, health multiplier and Beast Clincher strategy). SeedData selects template names absent from Races, resolves body catalogue, calls SeedRace. SeedRace constructs Race with BaseBody/strategy/corpse/progs/size/height-weight/combat values; adds context.Races and saves, then adds RacesAttributes, ethnicity, characteristics, bodypart usages, natural attacks and descriptions, followed by save. Add a BeastRace catalogue record with an existing BodyKey, attack aliases valid in that body, description variants, attribute/combat profile and food/water parameters as required by record. New body keys need BuildBodyCatalogue support. Explicit transaction; race creation is missing-only. Existing breathing, mobility, diet, defaults and combat fields refresh through RefreshExistingMythicalRaceDefaults and combat helpers, and auxiliary disfigurement/AI support also reconciles stock data.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Giant Beetle` race, start from Giant Ant and reuse only a compatible admitted body key, adjusting authored physical/stat/prose/attack mappings deliberately. Add the template to Templates and needed name-keyed support content. Trace missing-race selection, SeedRace, bodypart aliases and combat/AI/support refresh; the existing-race path does not recreate anatomy simply because a template changed. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / InstallMissing`.

Reruns install missing stock mythic races without duplicating existing entries.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [MythicalAnimalSeederTemplateTests.cs](../../../DatabaseSeeder%20Unit%20Tests/MythicalAnimalSeederTemplateTests.cs)
- [NonHumanCombatBalanceReconciliationTests.cs](../../../DatabaseSeeder%20Unit%20Tests/NonHumanCombatBalanceReconciliationTests.cs)
- [Mythical_Supernatural_Combat_Balance_Pass.md](../../../Design%20Documents/Combat/Mythical_Supernatural_Combat_Balance_Pass.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
