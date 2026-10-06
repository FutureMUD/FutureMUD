# CookingSeeder

## Start here to add content

Edit [CookingSeeder.cs](../../../DatabaseSeeder/Seeders/CookingSeeder/CookingSeeder.cs): **EnsureBakedAppleCraft / EnsureItem and their SeedData invocation**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Prepared-food prototypes, direct food items and sample cooking recipes.
Baseline availability: **Enabled**. [CookingSeeder.cs](../../../DatabaseSeeder/Seeders/CookingSeeder/CookingSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Useful item components must already include Holdable and Stack_Number.
- Core food materials must already exist.
- At least one trait definition must exist for cooking craft quality checks.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: EnsureBakedAppleCraft creates bake apple after Craft name guard. It adds a Craft with two CraftPhases, a Tag CraftInput for appleTag.Id, and a CookedFoodProduct CraftProduct for bakedApple.Id. EnsureItem finds GameItemProtos by ShortDescription; if missing it creates GameItemProto with next id, noun, descriptions, material, weight/cost, then adds GameItemProtosTags and GameItemProtosGameItemComponentProtos links and adds context.GameItemProtos. Add a food item using EnsureItem's full argument set: Account, timestamp, Material, noun, short/long/full prose, weight, decimal cost, tags and components. Add a craft using the full EnsureBakedAppleCraft field pattern, valid trait/appearance prog/input tag/product ID, phases and XML. Existing item by short description and craft by name are preserved without refresh. No outer transaction found; multiple SaveChanges.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked additional baked-fruit example, create the matching `PreparedFood_*` component through `EnsurePreparedFoodComponent` / `FoodDefinition`, then add an `EnsureItem` call with a distinct short description and compatible material/tag/components, then a guarded craft with that product ID and a real ingredient tag. Add it to `MissingStockRecords` and the `SeedData` dispatch. Follow EnsureBakedAppleCraft phase, input and CookedFoodProduct XML; changing only a craft name cannot supply its product. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / InstallMissing`.

Reruns install missing prepared-food stock records without mutating the legacy Food component.

This package owns direct prepared-food examples, stackable serving examples, and stock CookedFoodProduct recipe examples by stable names.

Stock prepared-food content is tracked by CookingSeeder component names, item short descriptions, tags, and recipe names.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [CookingSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CookingSeederTests.cs)
- [Food_and_Cooking_System.md](../../../Design%20Documents/Crafting/Food_and_Cooking_System.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
