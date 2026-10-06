# CultureSeeder

## Start here to add content

Edit [CultureSeeder.Languages.Modern.cs](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.Languages.Modern.cs): **SeedModernLanguages for a legacy modern language; historical toolkit inputs are separate**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Add Name Cultures, Random Names and optionally culture packs.
Baseline availability: **Enabled**. The entrypoint is [CultureSeeder.cs](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `seedsignedlanguages`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Human seeder must have installed the Human race.
- A skill decorator must already exist.
- Chargen height filtering progs must already exist.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Legacy named entry and persistence.** [CultureSeeder.cs](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.cs) dispatches to toolkit when `ToolkitEra(culturepacks)` maps the answer (antiquity, darkages, medieval, renaissance, earlymodern). If it returns null (e.g. legacy variants/none), it sets `_context`, calls `SeedSimple`, conditionally `SeedCulturePacks`, runs accent-role, satiation, fallback name-profile and free-knowledge reconciliations, saves and commits. Legacy language catalogues use helpers in [CultureSeeder.Languages.cs](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.Languages.cs); it calls `EnsureLanguage(name, unknownDescription, canSelectProg)`, then `EnsureAccent` for native/fallback accents. For example, historical language entries are data calls `AddLanguage(language.Name, language.UnknownDescription)` in [CultureSeeder.Languages.Historical.cs](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.Languages.Historical.cs). Shared `CultureSeeder.Shared.cs:132-215` ensures a `TraitDefinition` skill and `Language` row (`_context.Languages.Add(created)`), then ensures `Accent` rows (`_context.Accents.Add(created)`). Adding a simple standalone legacy language means adding to a source family list/`AddLanguage` call; add all accents with pronunciation prose/group/role/difficulty; ensure required selectable prog and associated language skill. A language row alone is incomplete for player use.

**Toolkit pack path and authoring constraints.** Toolkit path (`SeedData` branch) calls `CultureToolkitInstaller.Install(context, era, seednames, seedlanguages, seedheritage, progress)` and commits a managed install. Toolkit content comes from `CultureToolkitCatalogue` and source-qualified modules, plus `CultureToolkit/ReviewedSourceProse.json`; do not author a new supported toolkit pack merely by adding a legacy language. `CultureToolkitLanguageSeeder` binds canonical source keys, finds existing source-row languages, resolves localized label by era, creates/updates skill-cap and linked trait through `CultureToolkitEntityWriter`, updates language with difficulty model and unknown description, then brings over/reconciles source accents and managed records. To add toolkit language content, supply a retained source language with `CultureToolkitLanguageBindings` mapping and a catalogue specification under the canonical key (localized labels/descriptions in catalogue); add reviewed prose translations/rewrites only at source prose JSON. Make source aliases/crosswalk and existing-binding preflight explicit: ambiguous same-label language is an exception rather than silently overwritten. Accent keys are source-qualified (`language key + module + accent name`); for preexisting packs a label-only match does not bind an accent without matching full source fingerprint.

**Coverage boundary.** Current installer toggles keys `seednames`, `seedlanguages`, `seedheritage` default true when answers absent. Pack behavior also changes by `culturepacks`; signed modern languages are separately filtered. Legacy `SeedData` and toolkit transaction both commit after save/report conflicts; toolkit handles managed records/baselines and preserves builder changes. `CultureToolkit*Tests.cs`, `CultureSeeder*CoverageTests.cs` cover different contracts; consult both for the path changed. No tests run.

### Authoritative toolkit assets and a named source trace

For historical toolkits, the lowest-friction input edits are the linked JSONs under [CultureSeederRedesignHandoff/data](../CultureSeederRedesignHandoff/data) and [research](../CultureSeederRedesignHandoff/research), not the modern legacy language partial. `DatabaseSeeder.csproj` embeds them as `CultureToolkit.data.*` / `CultureToolkit.research.*`; [CultureToolkitCatalogue](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureToolkit/CultureToolkitCatalogue.cs) checks required documents and compiles era selections. `ReviewedSourceProse.json` is a separate embedded reviewed overlay. Legacy source generators are retained C# partials, evaluated as source stages by the toolkit; those stages are input definitions, not a second live install of the world.

A concrete toolkit language is `latin`: [CultureToolkitLanguageBindings](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureToolkit/CultureToolkitLanguageBindings.cs) maps `earthantiquity|Latin|latin` and later retained modules to that key. Catalogue labels/era admissions join the retained source language; `CultureToolkitLanguageSeeder.Upsert` resolves source-qualified binding, constructs a cap with stock floor 200 unless shared/overridden, uses `CultureToolkitEntityWriter.Upsert` for cap and linked trait, then language and source-qualified accents. The writer persists EF entities and managed logical identities/baselines; ambiguous unbound labels fail rather than get adopted.

Worked legacy addition: add `AddLanguage("Example Language", "an unknown example language")` beside English in `SeedModernLanguages`, then authored `AddAccent` entries in the same method using that language variable/helper shape. Trace `AddLanguage` → shared language/trait ensure helpers and `AddAccent` → Accent writes; choose actual naming/prose/skill-cap data before admission. For a historical toolkit addition, add its language-specification key, labels and era admissions in `language_identity_and_labels.json`, retained source definition and explicit crosswalk if applicable, then unknown-description/prose review, native/foreign accent and grant/group/script/name policy joins as appropriate. Test required-document/era selection, source binding, managed identity/baseline conflicts and native activation with `CultureToolkit*Tests`. One JSON label alone cannot manufacture the linked source/trait/accent graph.

See [language pack reference](../Culture_Seeder_Language_Pack_Reference.md), [heritage pack reference](../Culture_Seeder_Heritage_Pack_Reference.md) and [native accent review](../Culture_Seeder_Native_Language_Accent_Review.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Historical toolkits reconcile unchanged stock fields by source identity and preserve builder edits. Existing Modern, Middle-Earth and legacy world-pack workflows retain their prior behavior.

One historical era is installed; changing an installed toolkit's era requires an explicit migration. Unresolved native bindings remain inactive and original source material remains recoverable.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
