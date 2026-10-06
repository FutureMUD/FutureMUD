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

Named entry: MythicalAnimalSeeder.Definitions.BuildTemplates includes Giant Ant as BeastRace("Giant Ant","Insectoid",Large,"Giant Insect",GreatBeast(), prose, variants, attacks Carnivore Bite and Bite mapped to mandibles, BestialStats, health multiplier and Beast Clincher strategy). Before returning the catalogue, `BuildTemplates` applies `ApplyMythicalSatiationLimits`, then **`ApplyMythicalCombatBalance`**. The latter requires two exact-name switch mappings: combat tier and effective Strength. Either missing name throws `InvalidOperationException` during catalogue construction, before race persistence. Giant Ant maps to `EliteThreat` (tier value 3) and Strength 60. Tier/size rules supply effective Constitution 38, Agility 21, Dexterity 21, Willpower 30, Perception 26 and Aura 29. [WithEffectiveTargets](../../../DatabaseSeeder/Seeders/Utilities/NonHumans/NonHumanCombatBalanceProfile.cs) replaces the seven attribute offsets with target minus 11, while retaining the source intelligence/willpower/perception/aura dice expressions. Thus the authored `BestialStats` offsets are not the final offsets. The transform also replaces `BodypartHealthMultiplier` (1.45 here) and `CombatBalance`: Arthropod baseline, pain multiplier 1.8, Standard natural-armour quality, `arthropod` attack profile, no behemoth charge and no signature action for this entry. These source-computed targets are not executed gameplay measurements.

`SeedData` consumes the transformed catalogue, selects names absent from Races, resolves bodies and calls `SeedRace`. It constructs Race with the transformed attribute/health/combat values, adds context.Races and saves, then writes RacesAttributes, ethnicity, characteristics, bodypart usages, natural attacks and descriptions. New body keys need BuildBodyCatalogue support. Explicit transaction; race creation is missing-only. Existing breathing, mobility, diet, defaults and combat fields refresh through RefreshExistingMythicalRaceDefaults and combat helpers, and auxiliary disfigurement/AI support also reconciles stock data.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For `Example Giant Beetle`, start from Giant Ant with a compatible Insectoid body and deliberately authored physical/prose/attack definitions. Add the template **and its exact new name to both switches in `ApplyMythicalCombatBalance`**. A proposal matching Giant Ant would select `EliteThreat` and Strength 60; these are illustrative reviewed-choice placeholders, not automatically approved balance. Review the baseline switch too: include the name in its Arthropod group to retain that behavior, otherwise its fallback is Animal. Tier/size formulas override the seven offsets, the health multiplier and the combat profile, so changing `BestialStats` or the original health argument alone cannot set their final values. Review pain/armour quality, charge and signature rules against the intended result. If retaining Giant Ant's 72/24-hour satiation cadence, add the new name to `GetMythicalSatiationCadence`; an unmapped Large non-humanoid instead falls back to 24/12 hours.

For a future executable addition, extend [MythicalAnimalSeederTemplateTests](../../../DatabaseSeeder%20Unit%20Tests/MythicalAnimalSeederTemplateTests.cs) to enumerate `TemplatesForTesting` (catching missing name mappings) and assert the new entry's transformed tier, baseline, seven offsets, retained dice expressions, health/pain/armour values and intended satiation/attack behavior. Follow the existing [combat reconciliation tests](../../../DatabaseSeeder%20Unit%20Tests/NonHumanCombatBalanceReconciliationTests.cs) for selected first-install/rerun and stock/custom attack boundaries; they do not by themselves qualify a new race. Trace missing-race selection, SeedRace and support joins separately. Existing-race refresh does not recreate anatomy simply because a template changed. These are unexecuted validation recommendations; no template or balance mapping was changed here.

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
