# PrimaryProductionSeeder

## Start here to add content

Edit [PrimaryProductionSeeder.cs](../../../DatabaseSeeder/Seeders/PrimaryProductionSeeder/PrimaryProductionSeeder.cs): **Projects / ProspectingProject / ProductionProject and their resource/output dependencies**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Stock prospecting, extraction, quarrying, kiln, smelting, salt, tar, peat, and pigment project templates..
Baseline availability: **Disabled**. [PrimaryProductionSeeder.cs](../../../DatabaseSeeder/Seeders/PrimaryProductionSeeder/PrimaryProductionSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Core utility progs must include AlwaysTrue.
- Primary production tags and materials must already exist.
- The Item seeder must have installed primary-production visible resource props.
- Required stock traits must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: Projects() includes Prospect for Iron Deposits via ProspectingProject, with resource/deposit tags, stable marker and output references, Labouring skill, sample hematite output (1500g, Sample Ore Commodity tag). Enabled is false, so normal seeder discovery will not offer it. If enabled, SeedData sends ProjectSeed to EnsureProject; it creates Projects row with Stock Primary Production prefix/rev 0 and XML project definition, then EnsurePhase, labour/supervision, materials and actions. EnsureActions upserts ProjectActions by phase/name; ResourceDiscoveryDefinition resolves stable output prototype and serializes location/duplicate-prevention tags and messages. Add through Projects() using ProspectingProject/ProductionProject with valid refs and action. Stock project rows are actively rewritten on rerun. No explicit transaction found.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Iron Prospect`, add a Projects() definition using ProspectingProject with its own name, the actual Labouring trait, valid resource/deposit/location tags and stable output marker/prototype. Confirm EnsureProject/phase/labour/material/action XML and duplicate-prevention gates. This package is disabled: its source recipe is a development instruction and must not silently enable it as part of content authoring. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse stock primary-production local project templates by deterministic names.

Reruns refresh stock project definitions, labour, material requirements, and resource/commodity actions without duplicating templates.

Stock primary-production project content is tracked by the Stock Primary Production project-name prefix.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [PrimaryProductionSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/PrimaryProductionSeederTests.cs)
- [Primary_Production_Seeder_Design_Reference.md](../../../Design%20Documents/Seeding/Primary_Production_Seeder_Design_Reference.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
