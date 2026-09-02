# FutureMUD Industrialised Shared Baseline Design Reference

## Purpose

The shared industrialised layer contains ordinary goods that remain useful across at least two of Industrial, Modern, Nuclear and Information eras. It is a 5,800-row planning target and a dependency of every later-era module. It does not replace compatible `preindustrial_*` stock.

## Admission rules

A shared row must have one stable identity and one authored final presentation. Admit it when physical form and gameplay meaning remain substantially stable across later eras. Put genuinely period-defining technology, regulation, construction or presentation in an era delta. Do not manufacture nominal colours, sizes, brands or regional labels merely to meet a count.

Identity policy:

- reuse an existing `preindustrial_*` row when the durable form is unchanged;
- use `industrialised_<domain>_<name>` for a new form shared by later eras;
- use `<era>_<domain>_<name>` for an era delta;
- keep identity independent of technology profile; a profile chooses compatible component families rather than renaming the finished good;
- retire removed managed definitions through ItemSeeder provenance; never delete builder-customised aggregates.

## Source format

Catalogue content will be stored as domain TSV files and parsed into typed records before persistence. Each row must carry enough information to validate the finished item without patch-after-create logic. The minimum schema is:

| Field | Contract |
|---|---|
| `StableReference` | Globally unique lowercase snake case identity |
| `Domain` | Controlled catalogue domain |
| `EraAdmission` | `shared` or explicit canonical era keys |
| `Name`, `ShortDescription`, `FullDescription` | Final player-facing prose |
| `Material` | Exact seeded material name |
| `Size`, `Weight`, `CostIndex` | Localisable physical/value data |
| `Tags` | Exact tag paths without redundant parents |
| `Components` | Typed component bindings and parameters |
| `TechnologyRequirement` | Optional profile dimension and compatible family |
| `Lifecycle` | Optional predecessor, successor, waste or refill relationships |
| `Craft` | Optional craft identity and input/tool/product bindings |
| `SourceNote` | Maintainer-facing evidence summary, not a persisted builder comment |

Loaders must reject duplicate references, unknown era keys, unresolved materials/components/tags, impossible component combinations, invalid numeric ranges, unsupported description claims and profile-dependent rows without a declared dimension. Generated manifest rows must point back to their source file and row identity.

## Domain allocation

The 5,800 target is governed by domains rather than a single monolithic list. Exact allocations are set when Stage 2 source work begins, but coverage must include domestic and personal goods; clothing and textiles; food service and storage; workshops and construction; agriculture and extraction; retail and offices; institutional and medical stock; transport support; electrical and communications; leisure and media; civic/safety equipment; and containers, packaging, waste and lifecycle supplies.

Cross-domain forms belong in the domain that owns their primary gameplay interaction. Alternate presentation alone does not justify a duplicate.

## Technology profiles

ItemSeeder owns the profile questions and remembers one world choice for all later-era catalogue passes. Helpers may resolve profile data outside the question declaration. UsefulSeeder owns every reusable component prototype named by a profile.

The fixed dimensions are:

| Dimension | Controls |
|---|---|
| Power | Mains plugs, sockets, converters and ordinary supply compatibility |
| Paper | Common office and institutional sheet families |
| Telecommunications | Telephone and signalling interfaces |
| Network/media | Data, local media and connector families |
| Vehicle service | Charging and service connectors used by ordinary support stock |

`neutral` installs non-regional stock families suitable for a fictional world. Regional profiles use descriptive compatibility families, never trademarks. `custom` requires exact existing component prototype names for component-backed dimensions and explicit text families for paper. Missing custom components produce an ItemSeeder diagnostic directing the operator to UsefulSeeder's Modern Item Components package.

Once later-era content is active, a non-empty world's selected profile is immutable by the normal reconcile path. Changing it can invalidate installed component graphs and requires a separately designed migration.

## Value, prose and mechanics

`CostIndex` is one global relative value index. Stage 2 must publish anchors such as a basic meal, day-labour tool, ordinary garment, domestic appliance and motor vehicle. Era inflation and currencies are presentation/economy concerns, not alternate catalogue prices.

Descriptions follow the item authoring guide: overwhelmingly single-word noun heads, substantive full descriptions, no real brands, no claims of working controls where the component graph is inert, and no catalogue/provenance comments persisted for builders.

Prefer dedicated components for meaningful interactions. A powered craft tool uses `PowerTool`; a powered device with only switch/state presentation can use `PoweredProp`; an inert item must read as inert. Refill, consumable, battery, media, key/lock and lifecycle relationships must be explicit when gameplay depends on them.

## Activation evidence

The shared module is not independently selectable. It is installed when at least one activated later era is selected. Its admission requires typed-source validation, generated manifest review, dependency export checks, repeatable fresh/current/update runs, preservation of customised managed records and representative in-game inspection.
