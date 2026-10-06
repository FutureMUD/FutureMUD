# AnimalButcherySeeder

## Start here to add content

Edit [AnimalButcherySeeder.cs](../../../DatabaseSeeder/Seeders/AnimalButcherySeeder/AnimalButcherySeeder.cs): **BuildGlobalItems / BuildFamilies and the consuming product assignments**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Stock butchery profiles, carcass cuts, hides and trophy parts for animal races.
Baseline availability: **Enabled**. [AnimalButcherySeeder.cs](../../../DatabaseSeeder/Seeders/AnimalButcherySeeder/AnimalButcherySeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Useful item component prerequisites must already include simple held, destroyable and stackable props.
- Core animal product, cutting, meat, bone and skin foundations must already exist.
- A dedicated Butchery skill (or the simple package's Survival skill) and at least one stock animal race must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: BuildGlobalItems declares global:offal with noun offal, short/full descriptions, Normal size, 1800 grams, cost 2M, meat material, edible flag and Offal tag. SeedData builds race assignments from AnimalSeeder and MythicalAnimalSeeder template catalogues, then `EnsureItems` creates/reuses GameItemProto outputs before `EnsureProducts`, `EnsureProfiles` and race profile assignment. EnsureProducts writes ButcheryProducts, ButcheryProductsBodypartProtos and ButcheryProductItems; EnsureProfiles writes RaceButcheryProfiles and breakdown-check/emote children. Add a global output by adding a stable key and StockButcheryItemSpec with noun/prose/size/weight/cost/material/edible/tags, then ensure a consumer references that key. For a race-specific family, add/update its BuildFamilies StockButcheryFamilySpec and valid bodypart aliases. Prerequisite checks include output components/tags/materials; multiple SaveChanges and no explicit outer transaction were found, so a partial failure may leave earlier work.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked output `example:offal-portion`, add a distinct `StockButcheryItemSpec` using the existing offal material/component/tag shape and authored physical values, then reference that stable key from a selected family/product. An unreferenced global item is not sufficient to expose a harvest. Verify item identity, product/bodypart/item links and assigned RaceButcheryProfile; each family must match real bodypart aliases. These instructions were source-checked but not executed.

Detailed runtime/builder contracts: [Butchering System](../../Crafting/Butchering_System.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse stock animal butchery item, product and profile names, then attach missing eligible stock races.

Existing builder-authored butchery profiles are preserved; only stock-owned profiles and unassigned eligible stock races are repaired.

Stock butchery content is tracked by the Stock Butchery profile/product prefix plus the Butchery Output tag.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [AnimalButcherySeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/AnimalButcherySeederTests.cs)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
