# ItemSeeder unified catalogue: stages 1 and 2

This implements the capture and representative-contract stages of the [implementation proposal](ItemSeeder_Unified_Catalogue_Implementation_Proposal.md). Bulk migration has not begun. The target remains explicit, typed C# definitions compiled into the seeder; era is admission metadata. No catalogue generator or new external data file is required by the installer.

The pilot is deliberately bounded. Existing dispatch still selects eras, prerequisites and ownership scopes. The new metadata records that information but does not yet replace selection or reconciliation. Existing literals, TSVs and factories remain as the comparison oracle and as the authoritative inputs for unconverted content. Executable-only packaging is stage 7, not a completed part of this change.

## Captured baseline

Source revision: `d1c38215ed8957f537e89554910f17e7e9a53760`, with the stage 1–2 working-tree changes. Captures use all currently admitted era bands and the neutral technology profile. These are authoring captures with synthetic IDs, not successful Industrial installations: existing readiness gates remain in force for actual installation.

Final capture/benchmark binary SHA-256: `8b754fa84e421773a844cb48188ba4c333dff5dd984e779f397b700553ed26f6`. Source fingerprint: `f9ada895e119d3fb5b6b6f17a5af582ff37ab278c8d85735b768a93124972418`.

| Aggregate | Definitions |
| --- | ---: |
| Item | 23,572 |
| Craft | 13,150 |
| Component | 2,609 |
| Outfit | 1,313 |
| Prog | 1,257 |
| Tag | 404 |
| Liquid | 233 |
| Vehicle | 81 |
| Knowledge | 70 |
| Material | 55 |
| Commodity spoilage | 6 |
| Item skin | 5 |
| Variable | 1 |
| **Total** | **42,756** |

Each captured definition retains its complete registration payload, identity, dependencies, admissions, ownership policy, source location and supplemental fields. Supplements include item builder notes and craft defaults absent from the ownership manifest. Craft product references also have a stable-key projection for comparison; the original values remain available.

The source archive retains exact bytes and SHA-256 hashes for 987 files. Its source-only graph export includes the industrial document, all eight era and technology choices, historical outfits, alias/reuse mappings, food and medical rows, and all 93 vehicle specifications, including 12 not registered by the selected installable bands. It retains 6,450 industrial item rows, 2,337 craft rows, 100 outfit rows, 464 development food concepts, technology bindings and price evidence. Header-only clothing tables remain explicitly empty. Modern, Nuclear and Information remain non-selectable.

The field ledger lists fields in every evaluated aggregate and the target or retained legacy location. A separate ledger lists every retained TSV row and its source columns. This is a migration inventory, not a claim that every archived source field already has a typed replacement: dormant/research metadata remains preserved evidence until its family is converted.

Both final captures compare equal, including source-only graphs. The generated ownership manifest changes only its source fingerprint and generation timestamp; all 42,756 aggregate fingerprints remain unchanged. Decimal scale is preserved where it affects the existing fingerprint representation.

## Representative compiled content

| Stable identity | Contract exercised |
| --- | --- |
| `medieval_linen_braies` | Ordinary item fields, garment components and genuine variable-colour choice |
| `Basketry/weave wicker beehive` | Four ordered phases, three commodity inputs, two tools, knowledge gates and a typed product reference |
| `medieval_food_lenten_almond_fish_pottage` | Food source values, nutrition/freshness bands, taste and builder notes through the shared food writer |
| `medieval_outfit_0001` | Seven explicitly ordered clothing entries and placement/load metadata |
| `vehicle_antiquity_two_wheeled_handcart` | Expanded exterior/component graph, compartment, cargo, towing and damage children |

Definitions live under `DatabaseSeeder/Seeders/ItemSeeder/Catalogue/Content`. They contain literal data and constructors; a recursive syntax check rejects runtime expansion, calls, spreads and generated-property references. Fixed variants must be authored explicitly. The vehicle graph was expanded once during conversion and is now checked-in C# data.

The typed craft pilot supports commodity inputs, tag tools and simple products. Other existing input/tool/product kinds are captured completely and retain their legacy writers until their conversion stage. The pilot retains the old craft target-resolution rule and fingerprint projection. It does not silently switch category/name-derived crafts to explicit-key target lookup.

Validators check readiness, identities, physical values, phase bounds and quantities before applying representative definitions. Adapters reuse the existing ownership-aware writers. Typed craft values serialize directly to the existing runtime XML shape rather than being parsed back out of import text.

Antiquity-only synthetic capture supplies its missing external Core `Meat` prerequisite only for the EF InMemory provider. A provider regression verifies that this setup never queries tags or flushes changes on a relational context. Real databases must supply their own prerequisites.

Two preservation prerequisites are included: customized food now exits before the food writer can overwrite scalars or reconcile links after `CreateItem` returns a customized aggregate; source fingerprints now include nested catalogue C# and root ItemSeeder helpers. Neither change lowers existing content-count or readiness contracts.

## Reproducing the evidence

Build Release first, then run from the repository root. Use new or empty output directories; capture commands refuse to overwrite existing evidence.

```powershell
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1
dotnet DatabaseSeeder/bin/Release/net10.0/DatabaseSeeder.dll --capture-item-catalogue --legacy --output .artifacts/catalogue-before
dotnet DatabaseSeeder/bin/Release/net10.0/DatabaseSeeder.dll --capture-item-catalogue --output .artifacts/catalogue-after
dotnet DatabaseSeeder/bin/Release/net10.0/DatabaseSeeder.dll --compare-item-catalogue --before .artifacts/catalogue-before --after .artifacts/catalogue-after --output .artifacts/catalogue-comparison
pwsh -NoProfile -File scripts/benchmark-item-catalogue.ps1 -Iterations 5 -OutputDirectory .artifacts/catalogue-performance
```

Capture also accepts `--eras` and `--technology-profile` for named profiles. `definitions.ndjson` carries raw and symbolic values; `field-ledger.ndjson`, `source-field-ledger.ndjson`, `source-graphs.json`, `item-references.json`, `sources.zip`, `source-inventory.json` and `receipt.json` complete the evidence. Receipts hash every artifact and identify the binary and source fingerprint. These developer exports are not seeder deployment assets.

## Measurements

Measurements use .NET SDK 10.0.401, Release, Windows 10.0.26300, on the same host. No tests or other builds ran alongside the runtime samples. The benchmark harness rejects changed binaries and mismatched aggregate digests.

### Current full seeder workload

Five alternating fresh-process pairs construct the existing complete in-memory authoring/ownership workload. Only the five representative definitions switch format; the rest remain legacy. The timer excludes relational writes, export serialization and process launch.

| Measure | Legacy | Pilot |
| --- | ---: | ---: |
| Median construction/capture | 12,074 ms | 12,243 ms |
| Range | 11,659–12,686 ms | 11,624–12,687 ms |
| Median total managed allocations | 7,948,748,592 bytes | 7,948,609,904 bytes |

The pilot median is 1.4% higher with overlapping ranges; these five samples establish neither a reliable speedup nor a clear regression beyond noise. They are not fresh-install or unchanged-rerun MySQL timings.

A separate, warmed factory-loop microbenchmark constructs 42,756 copies of one garment definition. Median compiled construction is 42.10 ms / 17,456,352 allocated bytes, versus 269.71 ms / 123,491,984 bytes for JSON deserialization. This isolates parser/allocation overhead. It does **not** model the cold code-generation cost of thousands of distinct initializer bodies.

### Build cost of the implemented slice

Three alternating seeder-only rebuilds use the archived original source and the pilot, against the same unchanged dependency outputs. Restore is outside the samples. `BuildProjectReferences=false` prevents rebuilding dependencies; this is a scoped `Rebuild`, not a full solution clean/build.

| Measure | Original | Pilot |
| --- | ---: | ---: |
| Median rebuild | 33.49 s | 33.43 s |
| Rebuild range | 33.49–34.60 s | 32.79–33.72 s |
| Median incremental check | 1.602 s | 1.616 s |
| Seeder DLL | 40,761,344 bytes | 40,861,184 bytes |

The slice adds 99,840 DLL bytes (0.245%) without a meaningful measured rebuild change. Each mode produced the same DLL hash across its three builds. The earlier 280.65-second baseline build included dependencies and is retained as evidence, but is not compared with these scoped samples. These DLL sizes are not published executable sizes.

The three-pair build comparison precedes the final capture-provider safety guard; the final DLL byte count is unchanged. The full-workload runtime benchmark and delivery captures were refreshed for the final binary named above.

### Source-volume limit and cold construction

The disposable source-volume probe expands the ordinary-item schema to 42,756 explicit records with unique stable keys and description suffixes: about 55.4 MB of C# source. This is synthetic data using the production item shape, not a conversion of all heterogeneous production aggregates. It has no database writes, ownership checks or rich craft graphs.

One assembly fails with compiler `CS8103` (combined string-literal limit). Splitting the identical literals into four data assemblies succeeds. No experimental compiler flag is enabled. This establishes a scaling risk; it does not establish that production requires exactly four assemblies. Even the captured distinct item full descriptions and craft blurbs/phase echoes alone contain 15,146,368 UTF-16 payload bytes, before other fields or metadata overhead.

The partitioned probe uses a public copy of the ordinary-item schema so both formats share identical types. Embedded JSON contains exactly the same values. All five alternating cold-process pairs produce the same digest.

| Measure | Partitioned literals, IL | Embedded JSON, IL |
| --- | ---: | ---: |
| Median construction | 3,819 ms | 341 ms |
| Construction range | 3,787–4,122 ms | 336–347 ms |
| Median process time | 4.086 s | 0.596 s |
| Reported managed construction allocations | 17,786,544 bytes | 107,147,056 bytes |
| Catalogue assemblies, excluding common dependencies | 44,379,136 bytes | 51,887,616 bytes |
| Build, one sample per format | 116.76 s | 1.93 s |

Process time includes output-digest serialization after the construction timer. Peak working set is also sampled after that serialization and cannot be presented as construction-only memory. Managed allocation counters do not capture all native code-generation or loader costs. These synthetic results contradict using the warmed one-factory microbenchmark as a prediction of bulk cold startup.

The scaling decision must therefore include compiled assembly partitioning and cold execution, rather than adopting a larger version of the pilot unchanged.

### Published probes: ReadyToRun off and on

The same two formats were published for `win-x64`, framework-dependent, single-file and untrimmed, with ReadyToRun off and on. Five alternating fresh-process runs per artifact loaded from separate temporary directories outside the repository, each containing only its executable. All 23 outputs match, including three additional runs that disable ReadyToRun on the same compiled native artifact. Native PE headers confirm precompilation of Data0–Data3 and Schema; no publish warnings were emitted.

| Measure | Literals, IL | Literals, ReadyToRun | JSON, IL | JSON, ReadyToRun |
| --- | ---: | ---: | ---: | ---: |
| Median construction | 3,993 ms | **110 ms** | 368 ms | 365 ms |
| Construction range | 3,981–4,095 ms | 109–114 ms | 365–374 ms | 364–368 ms |
| Median process time | 4.285 s | **0.391 s** | 0.636 s | 0.632 s |
| Executable bytes | 145,535,124 | 191,836,820 | 152,771,036 | 158,505,948 |

Disabling ReadyToRun on the exact same compiled executable returns construction to approximately four seconds. Together with the native headers, this supports avoidable cold code generation as the principal cause of the IL literal penalty in this probe. The larger native executable wins construction and whole-probe process time here. Its construction reports 17.8 MB of managed allocations versus 107.1 MB for native-published JSON; the allocation caveats above still apply.

Executable sizes include common FutureMUD library/database dependencies, not just the catalogue. Publish directories also contain optional dependency PDBs because the probe references prebuilt dependency DLLs; those symbols were not copied to the executable-only run directories. This establishes the probe's loading behaviour, not completion of the real seeder's packaging work.

Literal IL publication took 122.70 seconds including recompilation. The subsequent ReadyToRun publish reused IL and took 12.08 seconds; JSON publishes took 2.81 and 7.10 seconds. These are sequential single samples with different cache states, not independent clean-build comparisons. The scoped three-pair build measurements above and the source-volume build sample remain the compile-cost evidence. The initial ReadyToRun restore failed at the default unwritable HTTP cache; restoring with a workspace-owned NuGet cache succeeded, and the qualified run used that cache.

**Recommendation:** retain typed C# authoring, but introduce bounded data assemblies and qualify ReadyToRun before expanding the production catalogue. This preserves the runtime-first choice supported by the Windows probe, at the cost of larger executables and longer builds. Do not adopt unbounded literals or rely on the warmed microbenchmark. Track string budgets per assembly, load only admitted batches, and repeat representative publication/runtime checks on supported platforms before making the publishing adjustment. Production publishing settings remain unchanged in this stage. This is not a measured claim about full MySQL install time or every rich-graph family.

## Validation and retained evidence

The final Release build and manifest currency check passed. The complete `DatabaseSeeder Unit Tests` run finished successfully: **2,097 passed, 3 skipped, 0 failed** out of 2,100 cases, in 15.5553 minutes. All 12 new catalogue tests passed, including full capture, the 15-selection differential matrix, actual shared-writer customization/identity fixtures, literal-authoring checks and the relational-provider safety regression. The three skips are existing Unix launcher permission cases (`existingState` 0, 1 and 2) that require a Unix host; Windows does not qualify those permissions.

The final full-suite [TRX](../../.artifacts/item-catalogue-stage12/full-seeder-delivery-tests/full-seeder-delivery.trx) and [compact receipt](../../.artifacts/item-catalogue-stage12/test-delivery-receipt.json) identify the exact command, timing and tested binary hashes. Earlier interrupted exploratory runs are retained locally but are not counted as complete validation. Independent receipt verification checked both captures' artifact hashes/sizes, all archived source-file hashes and their definition counts. The complete [evidence bundle](../../.artifacts/item-catalogue-stage12/ItemSeeder-Stage12-Evidence.zip) contains those captures, raw samples, build/publish logs, final test evidence, original source snapshot, reproduction tools and an inner hash inventory. It is a local, ignored developer artifact; it is not shipped with the seeder.

The frozen capture source archives contain the production code/content used by the final binary and an earlier draft of this results document. The bundle's separate `implementation` copies contain the final report and proposal. This documentation-only finalization does not change catalogue fingerprints or the tested executable. `git diff --check` and local Markdown link validation complete the delivery checks.

## Qualification boundary

EF InMemory fixtures exercise actual shared item, food, craft and vehicle writers, including preserved IDs and customized reruns. They do not prove MySQL constraints, transaction rollback, permanent-ID behaviour or gameplay. The all-selection differential test covers all 15 non-empty combinations of the four pre-industrial eras. Existing industrial/profile/readiness tests retain their own qualification contracts.

Before deleting a family's legacy oracle, require full-value and identity comparisons, its ownership fixtures and its specialized graph checks. Real MySQL differential installations and engine-level craft, food, clothing and vehicle checks remain required by the later qualification stages. Production seeder packaging must still be tested outside the repository on supported release platforms. No native database, gameplay or production seeder executable-only publication result is claimed here; the published synthetic probes qualify only their own loading and measurements.
