# CoreDataSeeder

## Start here to add content

Edit [CoreDataSeeder.Materials.cs](../../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.Materials.cs): **SeedMaterials / AddMaterial; use the owning partial for gases, terrain, units, colours, planes and hearing**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Should be the first package that all MUDs import except for very advanced users in specific circumstances.
Baseline availability: **Enabled**. The entrypoint is [CoreDataSeeder.cs](../../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `gamename`, `environmentalexposure`, `account`, `password`, `email`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- No additional metadata predicate. Read the concrete readiness/preflight before execution.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Concrete definition and route.** The existing `silver` call at `CoreDataSeeder.Materials.cs:1571` belongs to the inner `SeedMaterialsBase` catalogue (`AddMaterial` at 1355), not the outer helper at 117. `SeedMaterials` invokes the base catalogue before its supplementary definitions. The inner helper looks up the canonical name in the materials dictionary, adds missing requested tag joins to an existing row, or constructs `Material` with type 0 (solid), enum behavior, density `1000 * relativeDensity`, mechanical/thermal/electrical values and optional residue/solvent information. It adds `Materials`, related `MaterialsTags`, indexes the new material and saves. Silver carries `Bronze Age` and `Precious Metal` tags. Existing numerical properties are retained; tag completion is explicit.

**Add-one recipe.** To add e.g. “silver-lead alloy”, place an outer-helper `AddMaterial("silver-lead alloy", MaterialBehaviourType.Metal, relativeDensity, false, shearStrength, impactStrength, absorbency, thermalConductivity, electricalConductivity, specificHeatCapacity, "Precious Metal")` call in the supplementary `SeedMaterials` block (the outer helper at line 117) in [CoreDataSeeder.Materials.cs](../../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.Materials.cs), supplying measured/intentional values in the same units and verifying that `Precious Metal` exists in `SeedMaterialsBase`. If it needs aliases, invoke/extend `EnsureAlias` in this method’s post-definition alias logic; if it needs tag placement beyond base categories, use the same `EnsureTag` pattern. Do not add to [CoreDataSeeder.Materials.Definitions.cs](../../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.Materials.Definitions.cs) merely because it is named “Definitions”: that partial defines other materials’ data; the runtime catalogue callsites are in `.Materials.cs`. Do not create a second duplicate name because its key causes a no-op. No question or registration entry is needed: discovery is by seeder reflection and this foundation catalogue is unconditional.

**Persistence/update semantics.** Main `SeedData` calls `SeedMaterials(context)` (`CoreDataSeeder.cs:519`) after bootstrap material setup. Metadata (`SeederMetadataRegistry.cs:40-47`) specifically makes foundation catalogues repairable, while bootstrap account/world/prog/settings remain one-shot. `AddMaterial` creates absent material names and the inner base helper completes requested tags on existing materials: it does not overwrite builder edits to an existing row. The surrounding catalogue does deliberately repair selected tags, aliases, and specific fields by named rules; this is why a newly-added call will seed once but an existing same-name row should not be expected to adopt changed numeric values. Main entrypoint opens one DB transaction and saves at staged points before final commit (`CoreDataSeeder.cs:350-1424`); failure before commit aborts transactional changes, while no per-material transaction exists. Validation pointers: focused material and tag behavior in `DatabaseSeeder Unit Tests/CoreDataSeederMaterialTests.cs` (and adjacent CoreData material test files); not run in mapping pass.

An addition also needs a content sanity check beyond compilation: supplied type must agree with the material's physical role; density is a relative value in this helper (e.g. `gourd shell` uses 0.2, not 200), while thermal/electrical values are passed directly; impact yield uses the authored positive impact strength or falls back to shear strength multiplied by 2.2. Use the tag names accepted by the `tags` map, not guessed display paths. If content should appear as a distinct searchable/resolvable term, aliases are separate `MaterialAlias` relations; changing only `Name` does not reserve synonyms. Tests source-audit catalog counts/required names and association behavior, so new stock content may require updating baseline expectations in the owning `CoreDataSeederMaterialTests` rather than weakening the check. The add helper intentionally uses `ContainsKey` and the dictionary is materialized with case-insensitive comparer, so case-only duplicates won't create another row. That is the precise stable-identity policy for this catalogue.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reconcile only the stock foundation catalogues and do not recreate bootstrap-world data.

Materials, fluids, gases, terrain foundations, units, colours, planes and hearing profiles are repaired or completed by stable stock identities.

Core bootstrap accounts, world records, progs and settings remain one-shot; only the documented foundation catalogues are repeatable.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
