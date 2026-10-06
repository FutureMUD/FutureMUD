# WildlifeCatalogueSeeder

## Start here to add content

Edit [WildlifeCatalogue.Predators.cs](../../../DatabaseSeeder/Seeders/WildlifeCatalogueSeeder/WildlifeCatalogue.Predators.cs): **BuildIndividualProfiles for predators; corresponding catalogue builders own groups and other families**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Finished wild, mythical-beast and managed-animal controllers with group templates.
Baseline availability: **Enabled**. [WildlifeCatalogueSeeder.cs](../../../DatabaseSeeder/Seeders/WildlifeCatalogueSeeder/WildlifeCatalogueSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core foundation must include an account, FutureProgs, materials, a craft trait and the Holdable component.
- The Animal seeder must have installed its animal foundations.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: WildlifeCatalogue.Predators.BuildIndividualProfiles adds Leopard from ArborealPredator with Ambush/Extract/Desperate/Balanced/InTrees values. SeedData calls EnsureAnimalProfiles; it iterates IndividualProfiles, resolves the persistent profile name (Leopard maps to `Wildlife - Tree Ambush Hunter`) and uses EnsureNamedEntity keyed by that name in context.ArtificialIntelligences, sets Name, Type="Animal", Definition=profile.BuildDefinition(...), then SaveChanges. Add an individual AI using the profile construction helper with a unique name and valid strategy/response/terrain values; The local Add helper derives the persisted Name through `PredatorProfileFor(species)`, so a new species argument needs that mapping or an explicitly named profile. Race/controller recommendations are an additional consumer integration. Existing rows with same name are overwritten from static definitions. Transaction rolls back on caught exception.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Tree Hunter` controller, start from the existing Leopard profile constructor, give the persistent AI a unique stock-scoped name, retain valid strategy/response values and author the habitat/terrain behavior. Update `PredatorProfileFor` for the new species argument so the local Add helper resolves its controller name; recommendation/automatic race assignment is a separate consumer integration. Check emitted Animal XML, AI name and assigned consumers, plus the distinct group-template catalogue if coordinated pack behavior is wanted. These instructions were source-checked but not executed.

Detailed runtime/builder contracts: [NPC AI and Group AI Runtime](../../AI/NPC_AI_and_Group_AI_Runtime.md), [Predator Hunting](../../AI/Predator_Hunting.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / FullReconcile`.

Reruns repair the stock-owned Wildlife and Managed Animal controller catalogue, group templates, habitat tags, shelter anchors and supporting progs without changing legacy Animal examples.

Finished wildlife rows are canonical stock definitions; clone them before intentional customisation.

Only Wildlife- and Managed Animal-prefixed rows plus named wildlife support records are reconciled. Builder-created terrain tags and legacy examples are preserved.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [WildlifeCatalogueSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/WildlifeCatalogueSeederTests.cs)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
