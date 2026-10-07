# AgricultureSeeder

## Start here to add content

Edit [AgricultureSeeder.cs](../../../DatabaseSeeder/Seeders/AgricultureSeeder/AgricultureSeeder.cs): **Crops / CropSeed / Yield; Orchards, Herds, Woodlands and profiles own other content**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Adds stock field profiles, crop, herd, woodland, and agriculture project-operation definitions..
Baseline availability: **Enabled**. [AgricultureSeeder.cs](../../../DatabaseSeeder/Seeders/AgricultureSeeder/AgricultureSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Core utility progs must include AlwaysTrue.
- UsefulSeeder agriculture tags must already exist.
- A Farming trait must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry Wheat is new("Wheat","A cool-season bread or flour wheat crop.","grain",110,18,35,75,2,32,[Yield("wheat",2500000),Yield("straw",1200000)]). SeedData loops Crops into EnsureCrop. Helper upserts AgricultureCropDefinitions by Name; writes description/category and Crop XML for growth/window/perennial/cycle/moisture/temperature/pollination/planting windows/seed commodity/output commodities. Add one CropSeed with all required climate/growth fields and Yield material/weight values, ensure material exists; detection derives names from catalogues. No outer transaction found; same-name rows are fully refreshed from C# definitions.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Bread Wheat`, add a Crops entry using Wheat as the complete CropSeed signature, author climate/growth/planting values and Yield material/weight quantities, and ensure those commodities exist. Crops is enumerated by SeedData/readiness, so no manual menu registration is needed. Check AgricultureCropDefinitions XML and exact output materials/weights; the example is not agronomic calibration. These instructions were source-checked but not executed.

Detailed runtime/builder contracts: [Agriculture System Overview](../../Agriculture/Agriculture_System_Overview.md), [Agriculture Runtime Model](../../Agriculture/Agriculture_Runtime_Model.md), [Agriculture Builder Workflows](../../Agriculture/Agriculture_Builder_Workflows.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse stock agriculture definitions, operation rows, and their project templates by stable names.

Reruns refresh stock field profiles, crops, herds, woodlands, operations, and project-backed labour templates without duplicating rows.

Stock agriculture content is tracked by stable profile, crop, herd, woodland, operation, and project names.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [AgricultureSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/AgricultureSeederTests.cs)
- [DatabaseSeeder_System_Gap_Audit.md](../../../Design%20Documents/Seeding/DatabaseSeeder_System_Gap_Audit.md)
- [FutureMUD_Renaissance_Agriculture_Food_Drink_Commodities_Design_Reference.md](../../../Design%20Documents/Seeding/FutureMUD_Renaissance_Agriculture_Food_Drink_Commodities_Design_Reference.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
