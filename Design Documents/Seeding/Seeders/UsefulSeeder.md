# UsefulSeeder

## Start here to add content

Edit [UsefulSeeder.ItemComponents.ContainersAndWriting.cs](../../../DatabaseSeeder/Seeders/UsefulSeeder/UsefulSeeder.ItemComponents.ContainersAndWriting.cs): **CreateContainer calls for container components; other item/component/tag/dream/autobuilder families have distinct partials**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

A collection of useful stock items, AI, tags, covers and helpers.
Baseline availability: **Enabled**. The entrypoint is [UsefulSeeder.cs](../../../DatabaseSeeder/Seeders/UsefulSeeder/UsefulSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `ai`, `covers`, `items`, `modernitems`, `tags`, `autobuilder`, `hints`, `dreams`, `dream-eras`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- The Human seeder must have installed the characteristic profiles required by stock item components.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Representative existing component.** For example `Container_Table` is created by `CreateContainer("Container_Table", "Allows a table to have items 'on' it", 200000, SizeCategory.Large, false, true, "on", ref nextId)` in `UsefulSeeder.ItemComponents.ContainersAndWriting.cs:1447`. Its component creation flows to shared `AddGameItemComponent` in `.ItemComponents.cs`; that helper checks `_itemProtos`/existing editable component, otherwise calls `context.GameItemComponentProtos.Add(component)` and caches it. Component helper also handles editable Item revision identity and preserves/reuses the existing prototype contract. Later this component is referenced by `ItemSeeder` prerequisites and item `Container` components; adding a component name alone may leave references missing.

**Add-one recipes depend on subcatalogue.** For a new container component: add a `CreateContainer` call in the appropriate `UsefulSeeder.ItemComponents.*` family partial, with unique component name, user-facing description, capacity/size constraints, behavior switches, and item-facing preposition; ensure it flows through `AddGameItemComponent` and any necessary parent/child constraints. For health tools, edit `.ItemComponents.Health.cs`; wearables in `.Wearables.cs`; tool/utility prototypes `.ToolsAndUtilities.cs`; furniture/variables `.FurnitureAndVariables.cs`; tags in [UsefulSeeder.Tags.cs](../../../DatabaseSeeder/Seeders/UsefulSeeder/UsefulSeeder.Tags.cs); dreams in `.Dreams.cs`; wilderness grouped terrain/autobuilder definitions in `.Autobuilder.WildernessGroupedTerrain.cs` or `.Autobuilder.cs`. An autobuilder addition is not an item component: follow the matching spec record into validation, dependency resolution and prototype projection. Tag is not just text either—ensure parent path hierarchy using tag helper. A new question-gated family also requires a `SeederQuestions` row, validator/filter, `SeedData` branch and relevant stable answer key. The catalogue is all C#; no stock data file is the authoring surface.

For the example `Container_Table`, its `CreateContainer` call routes into `CreateItemProto` and then `AddGameItemComponent`; add-helper cache makes duplicate component names in the same pass return the first object, and editable revision lookup adopts an existing row only when `EditableItem.RevisionStatus==4`. The key used by downstream ItemSeeder is exactly `Container_Table`, so renaming it can break a prerequisite even if the component type remains valid. A focused addition should trace whether any required `GameItemProtosGameItemComponentProtos` link is created later (as with A4/A3/A5 paper examples in ContainersAndWriting), and whether tags are seeded in [UsefulSeeder.Tags.cs](../../../DatabaseSeeder/Seeders/UsefulSeeder/UsefulSeeder.Tags.cs). Autobuilder records have their own identity and reconciliation; never reuse a component helper as evidence of autobuilder ownership. The test families listed below are separated by such content type and should be followed when the concrete addition changes a tested path.

**Transaction/ownership.** `UsefulSeeder.SeedData` wraps install in a transaction and many helper calls. Shared item-proto creation, tags and other package families have different stable identity/reconciliation behaviors; metadata at registry 220+ calls install-missing and lists prerequisites/dependencies, not blanket author-edit preservation. `AddGameItemComponent` returns cached or approved same-name prototypes without rewriting their fields; source changes therefore do not update that component on an ordinary rerun. Tests are split among `UsefulSeederItemPackageTests`, `UsefulSeederAutobuilderTests`, `UsefulSeederAiPackageTests`, `UsefulSeederModernPackageTests`; no execution here.

## Declared rerun and ownership contract

Metadata declares `Idempotent / InstallMissing`.

This package can be rerun to install missing stock kickstart content without duplicating its tracked packages.

Reruns also refresh the stock wilderness autobuilder room template, area template, and supporting terrain-feature tags by stable names.

Kickstart now owns stock items, AI, helper tags, the wilderness autobuilder room+area starter package, ranged covers, hints, and dream content; core terrain foundations are seeded separately.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
