# ItemSeeder unified catalogue implementation proposal

Status: implementation of stages 3–8 is recorded in the [completion results](ItemSeeder_Unified_Catalogue_Completion_Results.md). This document preserves the approved proposal and original investigation; its historical findings are not a description of the final source layout. The [stages 1–2](ItemSeeder_Unified_Catalogue_Stage12_Results.md) and [first stage 3 batch](ItemSeeder_Unified_Catalogue_Stage3_Results.md) receipts remain immutable checkpoints.

Source baseline: `d1c38215ed8957f537e89554910f17e7e9a53760`, inspected 22 October 2026.

## Recommendation

Use one **typed, declarative C# catalogue** for every era, compiled into bounded content assemblies bundled with DatabaseSeeder. Author explicit item, craft and supporting graph records. Store era admission, technology bindings and readiness as data. Apply those records through the existing ownership-aware persistence machinery, progressively consolidating its entry points.

Do not replace the current systems with the smallest existing item table. The target must contain the union of their supported data. One authoring method does not mean one flat record with hundreds of nullable fields: item, craft, liquid, food, skin, outfit, vehicle and evidence records belong to one versioned catalogue contract, with typed references between them.

Use ordinary C# compilation, with no required Python or custom catalogue-generation step in the final authoring/build workflow. Keep JSON/TSV documentation exports as derived, developer-facing products. A one-time migration converter is appropriate; its output becomes the maintained source after equivalence checks pass.

The stage 2 source-volume probe exposed the single-assembly string-literal limit and a cold JIT penalty. Partitioned literals with ReadyToRun won the Windows probe against equivalent embedded JSON; unprecompiled literals lost. Qualify bounded assembly layout and native publication before bulk conversion, preserve the executable-only delivery goal, and budget the larger binaries/builds. The [results](ItemSeeder_Unified_Catalogue_Stage12_Results.md) distinguish those synthetic measurements from the current seeder workload and unmeasured MySQL installation behaviour.

## Findings from the current code

### Authoring and application are already partly separate

Most ordinary item paths eventually call `CreateItem`; crafts converge on `AddCraft`. Those are valuable shared seams. The inconsistency is mostly in how definitions are authored, expanded, admitted and adapted before reaching those seams. Food, clothing and vehicles also have supporting graph writers that cannot be replaced by an ordinary item insert.

| Current source family | Present behaviour | Migration treatment |
| --- | --- | --- |
| Antiquity and Medieval inline C# | Direct calls, topic-specific specs and helpers | Extract final definitions into the common typed records |
| Renaissance/Early Modern inline catalogues | `EraCatalogueItemSpec`, `EraItemSpec` and specialised helpers | Merge their fields into the common item contract |
| Generated historical C# | Python-owned data, Markdown tables, mappings and generators produce checked-in C# | Preserve evaluated records and source metadata; make converted C# authoritative |
| Pre-industrial food TSV | Embedded rows become food components, liquids, items, selector progs and crafts | Convert the complete graph, including resolved nutrition/freshness and product-selection behaviour |
| Medical/repair TSV | Python produces embedded TSV; loader validates fixed category/count contracts | Convert rows and metadata, preserving delivery components and repair-target tags |
| Industrial ordinary TSV | Explicit item rows, simple crafts, outfits, technology bindings and price evidence | Preserve every field; expand implicit craft defaults into explicit records |
| Historical/industrial clothing graphs | Bases/reuse, palettes, skins, outfits, physical bindings, recipes and ownership preflights | Use the same catalogue envelope with dedicated typed graph records |
| Vehicles | Example/factory specifications produce exterior items, components and vehicle child graphs | Freeze each fixed example graph; retain database-reference binding and specialised persistence |

The [ItemSeeder guide](Seeders/ItemSeeder.md) gives the detailed source routes. Representative implementation seams are [CreateItem](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.cs), [craft definitions and AddCraft](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.cs), [ordinary era specs](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs), and [industrial records](../../DatabaseSeeder/Seeders/IndustrialisedItemCatalogue.cs).

### Scale and limits of the current inventory

The checked-in `Seeded_Item_Manifest.json`, parsed during this investigation, contains:

| Aggregate | Count |
| --- | ---: |
| Items | 23,572 |
| Crafts | 13,150 |
| Components | 2,609 |
| Outfits | 1,313 |
| Progs | 1,257 |
| Tags | 404 |
| Liquids | 233 |
| Vehicles | 81 |
| Knowledge | 70 |
| Materials | 55 |
| Commodity spoilage | 6 |
| Item skins | 5 |
| Variables | 1 |
| **Total** | **42,756** |

These are checked-in inventory counts, not a fresh install or recapture result. They do not establish completeness of inactive design content. The manifest records fingerprints rather than full definitions, and its capture defaults to Antiquity through Industrial with the neutral technology profile. It cannot reconstruct the catalogue or prove parity for every profile and era combination.

The inspected ItemSeeder tree, root ItemSeeder/Industrialised helper files and IndustrialisedCatalogue tree contain 124 C# files (15,289,219 bytes) and 76 TSV files (7,816,442 bytes). This is a source-size inventory, not an estimate of compiled size or conversion effort. Explicitly expanding every derived craft will increase maintained source volume.

### Performance and packaging facts

The existing Python generators are development tools. They are not launched by the ordinary seeding path. Replacing them improves the authoring workflow but does not itself remove Python work from a user's installation.

Food, medical and industrial TSVs are embedded resources already. Their runtime cost is loading/parsing/validating those resources, not distributing external TSV files. Direct typed C# can remove that parsing; it still has allocation, initialization and JIT costs. Database loading, live fingerprints, graph validation and persistence remain, and their relative costs need measurement.

The published product is configured as single-file and framework-dependent, but [DatabaseSeeder.csproj](../../DatabaseSeeder/DatabaseSeeder.csproj) copies four content assets to output/publish:

- `Assets/Manifests/Seeded_Item_Manifest.json`;
- `Assets/Database/BlankDatabaseSnapshot.sql`;
- `Assets/Database/BlankDatabaseSnapshot.manifest.json`;
- `Assets/Manifests/Monster_AI_Recommendations.json`.

[LoadForRuntime](../../DatabaseSeeder/ItemSeederManifest.cs) reads the external item manifest first and otherwise searches the repository. [Program](../../DatabaseSeeder/Program.cs) and [BlankDatabaseSnapshotManager](../../DatabaseSeeder/BlankDatabaseSnapshotManager.cs) look for the external snapshot. Removing those files without changing readers would break manifest loading or lose the fast snapshot bootstrap path.

### Availability is independent of format

The current registry permits Industrial selection; Modern, Nuclear and Information remain non-selectable. Industrial food preflight rejects concepts that have not reached `ProductionReady`. Industrial clothing tables are header-only, although graph infrastructure exists. Food has substantial design/dependency data without a completed item installation path.

Migrate those definitions and their readiness metadata without activating them. A source-format conversion must not become an implicit completion or release of the modern-content programme. Preserve current admissions and dependency failures first; readiness work remains separately qualified.

## Proposed implementation

### One authoritative source and one contract

Put catalogue code under `DatabaseSeeder/Seeders/ItemSeeder/Catalogue/`, grouped by subject. Era subfolders may help navigation, but folder names must not select parsers, establish identity or determine runtime eligibility.

Suggested responsibilities, all within the existing DatabaseSeeder project initially:

- `Catalogue/Definitions/`: immutable item, craft, reference, metadata and domain-graph records.
- `Catalogue/Content/`: literal records, split into modest subject batches.
- `Catalogue/CatalogueIndex.cs`: explicit batch registration and admission/dependency indexes.
- `Catalogue/CatalogueValidator.cs`: cross-reference, units, completeness and domain validation.
- `Catalogue/CatalogueSelection.cs`: selected eras, remembered world profile and supported package scope.
- `Catalogue/CatalogueInstaller.cs`: orchestration through existing ownership-aware writers.
- `Catalogue/CatalogueExport.cs`: deterministic full-definition and review exports for development/CI.

The pilot implements the definitions, content, index, validator and installer portions within the existing project. Consolidated selection remains future work. The original single-project starting point was appropriate for the pilot; the measured compiler limit and cold-start results now justify bounded content assemblies and ReadyToRun qualification before bulk conversion. A generic plugin framework is unnecessary.

Every definition needs a stable identity, explicit admissions, dependency references, ownership category, source/evidence references and a readiness state. Keep the existing manifest module separately as compatibility metadata during migration. Do not rename `antiquity_*` or other old keys merely because the format is era-independent.

Use named properties, explicit units and real enum types. Content records should contain literals, arrays and references to immutable data. Disallow arbitrary delegates, database calls, clock/random access, prose-building loops and cross-product variant generation in authoring records. Shared named constants are acceptable; behavioural defaults must be explicit or versioned so editing a helper cannot silently rewrite thousands of definitions.

Use a small syntax/invariant check to enforce that declarative subset. Machines can read it through C# syntax/compilation and through a deterministic JSON export of the exact same records. Do not rely on regex extraction of arbitrary C#. JSON exports are not another editable source and need not ship beside the executable.

The final runtime ownership inventory should be a deterministic projection of these compiled records, cached for the selected context, rather than a separately regenerated JSON prerequisite. Adapt `AssessSeedData`, the seeder description and startup validation to consume that projection. Preserve its compatibility fingerprint rules, profile resolution and admission semantics. Developer capture/check commands can still export a review inventory, but adding a literal record and building must be sufficient to include it in the executable. Measure inventory construction during menu assessment as well as during installation; do not replace one full JSON load with repeated full-catalogue hashing.

### The schema must preserve the richest existing definitions

| Record family | Required preservation |
| --- | --- |
| Item | Stable key; noun, all prose and keyword policy; size/quality; grams and currency cost; material; tags; component references; skin/visibility flags; builder notes; morph target/emote/duration; destruction target; legacy-match policy |
| Craft | Existing stable key, display/category/prose; traits and knowledge; all access and lifecycle progs; difficulty, threshold, free checks, fail phase, interruptibility; ordered phases with timing/echo/exertion/stamina; ordered inputs/tools/products/failure products; quality weights/formula; tool state/duration; material-defining input links; variable mappings and product-specific options |
| Components and supporting definitions | Type-specific parameters, revision references, dependency ownership, tags, materials/liquids, characteristics, progs and knowledge; preserve their current external-provider versus ItemSeeder-owner distinction |
| Food/liquids | Ingredient and production relationships; nutrition, dietary, freshness and taste/smell fields; density, viscosity, alcohol/water/satiation; vessels/servings; cooked-food product semantics and failure relationships |
| Clothing/outfits | Physical base/reuse source, colour definitions/palettes/mappings, wear geometry/layer references, skins, outfit entries and order, placement/container keys/load arguments, craft graphs and production-route review |
| Vehicles | Exterior, projections, compartments, links, occupants, control stations, propulsion, movement/fuel/trait bindings and all existing child definitions |
| Research and review | Price evidence and methodology links, supported claims, provenance, deliberate exclusions, dependency/readiness status and inventory classifications |

Use typed craft input/tool/product variants instead of permanently retaining several import-string dialects. Existing string parsers can be temporary migration adapters. Reuse the existing XML serializers/persistence semantics where possible, with late binding of symbolic references to database IDs. A database ID substituted into an XML definition is not item-type composition.

Do not put an opaque `ExtraData` dictionary behind the common item contract to avoid modelling supported fields. Conversely, do not discard reviewed research just because it does not affect an EF row: retain it in structured evidence records and the linked explanatory documents.

### Explicit variants and permitted runtime decisions

Each fixed physical item variant and each fixed recipe gets a concrete catalogue record. Freeze the current outputs of material/form/culture loops, generated household allocations, implicit recipe generation and fixed vehicle factories during migration.

Runtime retains:

- selection of already-authored definitions for selected eras/package scope;
- technology component bindings driven by the user's remembered world profile;
- existing player choices such as colour/variable craft products;
- reference resolution, revision selection, ownership checks and XML serialization;
- existing gameplay rules such as quality checks, failure products and selection among an explicitly authored set of outputs.

The last category must be distinguished from creating types. For example, the food selector in [Crafting.PreIndustrialFoodCatalogue](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.PreIndustrialFoodCatalogue.cs) emits `collectionfirst(collectionshuffle(@products))`. It randomly chooses among existing prototypes; it does not author new types. Preserve its explicit output set and selection behaviour during conversion. Replacing it with deterministic per-dish recipes would be a gameplay change requiring a separately reviewed conversion, not a lossless format change.

The [Renaissance finished-item craft path](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.Crafting.Renaissance.cs) currently derives names, material quantities and recipes from items held in the live seeder cache. Fixed catalogue recipes intentionally stop recomputing those definitions from builder-modified item properties. Preserve existing customized crafts; diagnose incompatible customized prerequisites rather than silently rewriting them. Record this behavioural difference in acceptance fixtures, including renamed items and changed weights/materials. Exact preservation of arbitrary live-state synthesis would conflict with the requirement for explicit fixed definitions.

### Shared application pipeline

1. Select admitted, ready definitions using the existing answers and installed-era union rules.
2. Resolve external prerequisites and actual retained/customized records; validate the whole selected batch before mutation where existing domain rules require it.
3. Apply owned supporting definitions and components in dependency order.
4. Apply item bases and required item relationships through ownership-aware writers.
5. Resolve and apply skins, outfits, craft support and complete crafts; use explicit dependency ordering rather than assuming every graph has the same fixed order.
6. Apply vehicle/domain graphs and deferred lifecycle relationships at their required flush points.
7. Save and record ownership within the existing transaction and permanent-ID lifecycle.

A lightweight registration/index pass can precede loading heavy content batches. Avoid constructing every non-selected era's full catalogue and avoid one enormous static initializer. Reuse dependency caches, stable ordering and existing deferred save boundaries. Do not introduce per-record `SaveChanges`, bypass EF ownership with bulk SQL, or cache away live checks of mutable database data to win a benchmark.

## Lossless migration strategy

### Capture definitions, not just counts

Pin the old revision and add a development-only full-definition capture at the shared item/craft/supporting-graph seams. Capture normalized values after legacy defaulting and transformations, as well as original authoring metadata and alias/provenance mappings. Resolve generated database IDs back to stable symbolic identities for comparison. Preserve order where it has meaning; normalize only transport details and truly unordered sets.

Use the old executable behaviour plus a source-field inventory. Neither alone is sufficient: capture mode skips some checks and omits some persisted fields, while inactive research has no installed output. For example, the manifest's item definition does not include builder notes. A matching fingerprint therefore cannot establish that every source field survived.

Produce a migration ledger for every source definition/field: old location and identity, target location/identity, mapping or preserved evidence reference, and comparison result. Include dormant Modern/Nuclear/Information records and unfinished food graphs separately from installable content. Use mechanical conversion for literals/tables; use evaluated capture for generator outputs and procedural recipes. Do not reconstruct rows from descriptions or from the manifest hash.

### Preserve installed worlds

- Keep `(entity type, stable key)`, existing logical IDs/revisions, aliases, relationship identities and ownership policy semantics.
- Freeze legacy category/name-derived craft keys as explicit stable keys. Test the change in target resolution caused by providing an explicit key to `AddCraft`; this is more than assigning a field.
- Preserve current module/admission behaviour during conversion, including first-registration semantics and shared aliases. Do not union admissions or collapse lookalike records without proving equivalent selection for existing era combinations.
- Reuse existing provenance and aggregate customization checks. Do not delete/reinsert item or craft prototypes, invalidate live items, or repoint outfits and vehicles wholesale.
- Keep old fingerprint projections during the initial conversion. Source-format hashes may change; that must not cause every live aggregate to be treated as customized or freshly owned. If a fingerprint contract must change, implement an explicit old/new compatibility check that only upgrades provenance after proving the live old stock signature.
- Treat missing managed links, unmanaged exact matches, unmanaged drift, duplicate/ambiguous identities, renamed current revisions and builder-added extensions as explicit test cases.
- Preserve externally owned same-name liquids/components and the rule against claiming them merely by display name.

There is no inherent need for an EF schema migration to change the authoring format. Source conversion and safe seeder reconciliation are the expected migration. Reassess only if the final identity/ownership design proves that additional durable metadata is necessary.

### Existing hazards that affect the migration

1. [UpsertPreIndustrialFoodItem](../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs) assigns scalar fields and reconciles links after `CreateItem` returns. `CreateItem` can return a customized item. This is a concrete source path that can bypass preservation; reproduce it with a customized-food rerun fixture before moving that family. Correct preservation at the shared writer boundary is necessary to satisfy the no-data-loss requirement, and should have its own regression and explicit change receipt.
2. [ComputeSourceFingerprint](../../DatabaseSeeder/ItemSeederManifest.cs) includes `ItemSeeder*.cs` inside the ItemSeeder folder and root `Industrialised*.cs`, but omits root `ItemSeeder.*.cs` helpers and other relevant support files. A new nested catalogue would also escape the current top-directory glob. Replace this with an explicit, tested source-input inventory covering definitions, adapters and schema; retain cross-platform path/newline normalization.
3. Existing fixed-count/allocation guards are meaningful content contracts. Keep them as conversion checks; do not lower or delete them to make the migrated catalogue pass. The Early Modern household generator's allocation scheme makes casual source additions capable of changing existing rows.
4. The checked-in inventory and capture mode do not prove successful Industrial installation: retain the current negative readiness test until the food programme is separately completed.

## Single-executable delivery

Compile the catalogue into bounded managed content assemblies and bundle them into the single executable. Qualify ReadyToRun for those assemblies and track their string-literal budgets; splitting files within one assembly does not address the observed compiler limit. During staged migration, embed the existing ownership inventory if legacy readers still require it; the final projection described above removes that independent runtime asset. Embed snapshot SQL/metadata and any genuinely required recommendation asset; audit whether the recommendation JSON has a runtime consumer before retaining it. Remove redundant copied content only once its reader uses the compiled/embedded source or the file is proven unnecessary.

Remove repository discovery from runtime inventory loading. Keep explicit developer export/check commands separate. Read the snapshot and metadata from streams; the current upgrade-coordinator API takes a path, so add a stream/text-reader import seam or, if required by its implementation, materialize an owned temporary file with bounded cleanup. The latter still needs no distributed sidecar; prefer streaming if practical.

Keep the fast blank-database import and migration validation/fallback. Do not meet the packaging requirement by dropping the snapshot and forcing a full migration chain.

The release configuration currently assumes an installed .NET runtime. Preserve that framework-dependent contract unless a fully self-contained executable is also desired; it is a separate size/distribution decision. The delivery criterion here is one shipped seeder executable with no required bundled data files. User-created configuration, backups and logs remain legitimate outputs, and the database service remains external.

Acceptance must copy only the executable into a clean directory outside the repository, run from a different working directory, and exercise catalogue/resource loading and actual blank-database bootstrap. Inspect the full publish output for transitive content. Test every supported release RID (`win-x64`, `linux-x64`, `linux-arm64`) on an appropriate host; a successful cross-publish is not a successful execution test.

## Delivery stages and acceptance

| Stage | Concrete work | Completion gate |
| --- | --- | --- |
| 1. Freeze baseline | Full definition/source-field inventory, captures by selection, ownership fixtures, timing harness | Every source family accounted for; installable versus blocked/development surfaces labelled |
| 2. Prove common contract | Typed schema, validators, exporter, adapters and representative ordinary/rich-craft/food/clothing/vehicle records | No omitted fields; old/new semantic parity for slice; representative performance result |
| 3. Convert explicit stock | Inline items/specs, generated historical C#, medical rows, industrial ordinary rows and evidence | Stable-key and full-value comparisons; all original count/quality/admission contracts retained |
| 4. Convert recipes | Expand implicit recipes, replace import dialects with typed records, freeze defaulting/progs/knowledge | Ordered craft graphs and actual products equivalent; customized-item derivation difference tested and documented |
| 5. Convert specialised graphs | Food/liquids, historical outfits/skins, vehicles; migrate inactive industrial graph data without activation | Complete child graphs, ownership preflights and domain behaviour verified |
| 6. Consolidate application | Remove migrated authoring dispatchers/parsers/generators; retain only required compatibility writers | One authoritative format; no legacy generator required to add content or build |
| 7. Complete packaging | Compiled ownership inventory, embedded snapshot assets, readers and release smoke tests | Executable-only fresh install and rerun work outside repository on supported platforms |
| 8. Final qualification | Full seeder suite, differential installations, performance comparison and documentation | No unaccounted loss/change; ownership and readiness gates pass; measured costs reported |

Keep each family conversion independently reviewable. Temporary adapters are migration scaffolding, not the final three-method architecture under a new name. Only remove old authoritative inputs after their content and non-runtime metadata are reconciled.

### Verification matrix

- Compare source and normalized full definitions by stable identity, not only row totals. Include all 13 existing manifest aggregate types and every source-only research graph.
- Check each selectable pre-industrial era alone and all 15 non-empty pre-industrial combinations for catalogue selection equivalence; exercise staged era additions in the database to cover existing-world ownership. Preserve focused black-powder scope.
- Cover Industrial/default and named/custom technology profiles with source/preflight tests; verify present readiness failures rather than claiming installs pass. Assert non-selectable modern bands stay non-selectable.
- Use real MySQL fixtures for fresh install, unchanged rerun, stock update, missing-link repair, customized aggregate preservation, renames/revisions, conflicts, rollback and permanent generated IDs. In-memory capture cannot prove these.
- Use existing `ItemSeederManifestTests`, `ItemSeederAddCraftTests`, era crafting suites, food tests, household allocation tests, historical colour/outfit tests, industrial graph/ownership tests and `VehicleSeederTests`. Replace source-layout assertions with equivalent catalogue contract assertions when needed; preserve their substantive coverage.
- Run representative engine-level craft/food/clothing/vehicle checks for serialized data that changes construction path. Assert actual product type/count, colour transfer, failure outputs, wearability and graph references; successful seeder insertion alone is insufficient.
- Benchmark identical Release builds, prerequisites, database settings and selections. Measure cold process startup, catalogue initialization, validation, fingerprinting, database reads/writes, total fresh install and unchanged rerun, peak memory, allocations, clean/incremental build, test time and executable size. Use repeated runs, median and spread, and separate compilation from execution. Gate on no reproducible installer regression beyond measured noise; do not invent a speedup percentage in advance.
- Include a full-size construction/export benchmark early, not just a handful of simple records, to expose C# initialization and source-volume costs before the bulk migration.

## Alternatives considered

| Option | Fit and trade-off |
| --- | --- |
| **Typed C# literals, compiled normally** | Recommended: explicit records, compiler checking, no custom generation prerequisite, no runtime text parser. More source/IL and potentially longer builds/JIT; measure those costs. |
| Embedded JSON/TSV | Can also ship in one executable and may reduce compilation cost. Runtime parsing remains; TSV is awkward for rich nested graphs. Viable if measurements overturn the C# choice, but not the preferred initial direction. |
| JSON plus generated C# or binary | Offers language-neutral authoring and a compiled payload, but adds a generator, build invalidation and another artifact to reconcile. Does not automatically run faster than handwritten C# producing the same data. |
| Direct bespoke EF add statements | Avoids a specification layer but repeats validation, identity and rerun logic; risks data loss. Keep a common writer even if data uses direct compiled literals. |
| One embedded database snapshot of all stock | Fast loading in a narrow case but incompatible with selective additions, custom-world dependencies, variable bindings and safe reruns. Keep the blank-schema snapshot for its existing purpose. |

## Investigation evidence and limits

The original investigation below predates implementation. See [stages 1–2 results](ItemSeeder_Unified_Catalogue_Stage12_Results.md) for the subsequent capture, representative conversions, checks and measurements. Stages 3–8 have not been implemented by that pilot.

Completed: read-only source tracing of historical and industrial authoring/application paths, ownership and manifest review, packaging-reader review, checked-in JSON counts and source-size inventory. Findings above distinguish executable behaviour from design-only content and planned work.

Not performed: generator runs, fresh manifest capture, compilation, performance benchmarks, test-suite execution, MySQL installation or gameplay smoke tests. This proposal does not assert runtime parity or a measured performance gain. No source content or installed database was changed.

Stages 1–2 were the recommended next implementation at the time of that investigation and are now complete within the qualification boundary recorded in the results. The subsequent stage 3 authorisation covers independently reviewable stock conversions against that baseline; the first batch introduces bounded content assemblies and publication measurements. The subsequent full-plan authorisation covers stages 3–8; implementation and qualification are recorded in the completion results.
