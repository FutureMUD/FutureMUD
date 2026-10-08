# AnimalSeeder

## Start here to add content

Edit [AnimalSeeder.InsectTemplates.cs](../../../DatabaseSeeder/Seeders/AnimalSeeder/AnimalSeeder.InsectTemplates.cs): **GetInsectRaceTemplates / Insect; choose the corresponding family partial for other species**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs body types for animals.
Baseline availability: **Enabled**. [AnimalSeeder.cs](../../../DatabaseSeeder/Seeders/AnimalSeeder/AnimalSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Shared operator questions from [NonHumanSeederQuestions.GetQuestions](../../../DatabaseSeeder/Seeders/Utilities/NonHumans/NonHumanSeederQuestions.cs): `model` (hp/hpplus/full), `random` (static/partial/random) and `messagestyle` (compact/sentences/sparse). Animal suppresses structural questions once Quadruped Base exists, randomness with combat rebalance, and style when a choice is recorded. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Human seeder must have installed the Humanoid body.
- The Core seeder must have installed the Simple name culture.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: AnimalSeeder.InsectTemplates.GetInsectRaceTemplates yields Ant using Insect("Ant", "Insectoid", Tiny, .05, "Insect", "insect-mandible", InsectPack(...), description). BuildRaceTemplates concatenates this iterator into RaceTemplates. AnimalSeeder dispatches to SeedInsectoid, then SeedAnimalRaces; that resolves template.BodyKey and calls AddRace. AddRace inserts into _context.Races, inserts one RacesAttributes row per attribute, creates ethnicity rows, saves, then creates description rows. To add an insect reusing an existing body, add an Insect(...) item with unique name, body key, size, bodypart health multiplier, height-weight model, attack key, InsectPack prose and optional prose; add name-keyed description/diet/butchery details where applicable. New anatomy requires a body builder and dispatcher entry. SeedData uses a DB transaction and backfill detects missing names; existing stock compatibility and diet/combat support can be refreshed.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked new insect named `Example Field Ant`, add a peer `Insect(...)` call next to Ant, retaining a valid existing Insectoid body/attack/pack shape and supplying deliberately authored size, bodypart health multiplier, height-weight model and prose. Its unique race name is the missing-template key. Confirm family dispatch and bodypart aliases, then inspect Race, RacesAttributes, ethnicity/descriptions and natural attacks. A new body plan is a separate anatomy implementation. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / FullReconcile`.

Reruns retain the installed animal package choices and reconcile stock bodies, races, attacks and supporting content.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

`SetupArmourTypes` emits `Non-Human Natural Bone Armour`. Its chopping dissipation formula is `max(damage*0.1,damage-(quality * 2 * strength/115000))`, matching the existing slashing/piercing reduction and retaining the 10% floor. The previous default's surplus closing parenthesis is corrected at source. [NaturalBoneArmourSeederTests](../../../DatabaseSeeder%20Unit%20Tests/NaturalBoneArmourSeederTests.cs) executes this armour setup against an in-memory context, loads all six runtime formula maps and checks chopping evaluation. The source correction does not overwrite existing builder edits. The exact retained disposable-world repair and its ownership boundary are described in [HumanSeeder](HumanSeeder.md#verification-and-related-references).

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [AnimalSeederTemplateTests.cs](../../../DatabaseSeeder%20Unit%20Tests/AnimalSeederTemplateTests.cs)
- [NonHumanCombatBalanceReconciliationTests.cs](../../../DatabaseSeeder%20Unit%20Tests/NonHumanCombatBalanceReconciliationTests.cs)
- [SeederDisfigurementTemplateUtilityTests.cs](../../../DatabaseSeeder%20Unit%20Tests/SeederDisfigurementTemplateUtilityTests.cs)
- [Damage_Balance_Animal_Mythic_Attribute_Scaling_Report.md](../../../Design%20Documents/Health/Damage_Balance_Animal_Mythic_Attribute_Scaling_Report.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
