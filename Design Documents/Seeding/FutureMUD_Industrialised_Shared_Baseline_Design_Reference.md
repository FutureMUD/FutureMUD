# FutureMUD Industrialised Shared Baseline Design Reference

## Purpose

The shared industrialised layer contains ordinary goods that remain useful across at least two of Industrial, Modern, Nuclear and Information eras. Its draft catalogue contains 5,800 rows and supplies later-era modules according to exact admissions. That count is not production acceptance. It does not replace compatible `preindustrial_*` stock. The [clothing, footwear and uniforms specification](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md) governs clothing coverage, authored skins, production and acceptance.

The [approved Wave 1 scope](./Industrialised_Clothing_Wave1_Evidence_and_Coverage.md) replaces the former 600 shared clothing planning quota with 223 new shared bases, giving 5,423 shared ordinary prototypes while other allocations remain unchanged. Of those 223 clothing designs, 112 admit Industrial and 111 have exact later-era shared admissions. Reused bases and additional skins are counted separately. The user approved these planning counts and overall scope on 2026-09-03 (Gate 1 passed), with conventional colours moved to overridable outfit defaults and no approved fixed locks. This is not an implemented source change or activation permission.

## Admission rules

A shared base row must have one stable identity and a finished authored default presentation; it may support many separately authored compatible skins. Admit it when physical form, intrinsic economic properties and gameplay meaning remain substantially stable across later eras. Put genuinely period-defining technology, regulation or construction in an era delta; presentation alone normally belongs in a skin with appropriate admissions. Do not manufacture nominal colours, sizes, brands or regional labels merely to meet a count.

Identity policy:

- reuse an existing `preindustrial_*` row when the durable form is unchanged;
- use `industrialised_<domain>_<name>` for a new form shared by later eras;
- use `<era>_<domain>_<name>` for an era delta;
- keep identity independent of technology profile; a profile chooses compatible component families rather than renaming the finished good;
- retire removed managed definitions through ItemSeeder provenance; never delete builder-customised aggregates.

## Source format

Draft catalogue content is stored in 24 embedded domain TSV files and parsed into typed records before persistence. The current schema below is not yet the complete clothing authoring contract:

| Field | Contract |
|---|---|
| `StableReference` | Globally unique lowercase snake case identity |
| `Domain` | Controlled catalogue domain |
| `Layer`, `EraAdmissions` | `shared-industrialised` or `industrial`, plus exact canonical era keys |
| `Noun`, `ShortDescription`, `FullDescription` | Player-facing text; production acceptance requires finished authored prose |
| `Material` | Exact seeded material name |
| `Size`, `Quality`, `WeightGrams`, `CostIndex` | Physical, quality and intrinsic value data |
| `Tags` | Exact tag paths without redundant parents |
| `FixedComponents`, `ProfileBindings`, `SupportedClaims` | Stock components, profile bindings and declared behaviour claims |
| `MorphTo`, `MorphSeconds`, `MorphEmote`, `DestroyedItem`, `LifecycleKind`, `Craftable` | Existing lifecycle/production fields; labels alone do not prove working maintenance or crafting |
| `PriceEvidence` | Explicit evidence references, distinct from editorial notes |
| craft TSV | Currently one product reference and material-input mass plus skill metadata; richer ordered inputs/tools/skin products remain implementation work |
| outfit TSV | Currently an ordered list of item references; explicit skin/colour/placement entries remain implementation work |
| `SourceNote` | Maintainer-facing evidence summary, including the historical-pricing evidence ID, dated source locator, quoted amount/currency/unit and wage-relative calculation; never a persisted builder comment |

Loaders must reject duplicate references, unknown era keys, unresolved materials/components/tags, impossible component combinations, invalid numeric ranges, unsupported description claims and profile-dependent rows without a declared dimension. Generated manifest rows must point back to their source file and row identity.

Clothing implementation must extend the typed model for authored skins, variable colour bindings and overridable outfit defaults (separate metadata for any future explicitly approved fixed exception), production-route and quality rationale, exact craft products and complete ordered outfit entries, using existing runtime capabilities. This documentation change does not modify the schema. Authored TSV text is the source of truth; generators must not assemble or overwrite it.

## Domain allocation

The existing 5,800-row draft allocation is recorded in `Industrialised_Item_Catalogue_Audit.tsv` and enforced by current tests. Clothing's former 600-base allocation is superseded by the approved coverage-first specification, leaving `5,200 + C_shared` as the revised shared planning total when other domains are unchanged. Clothing Gate 1 approved `C_shared = 223` and the separately recorded new/reused base, skin, recipe-obligation and outfit counts on 2026-09-03. Implementation must reconcile the old tests/audit constraints rather than padding rows to satisfy them. Other domain allocations are not changed by this documentation pass.

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

`neutral` installs non-regional stock families suitable for a fictional world. Regional profiles use descriptive compatibility families, never trademarks. The seven named profiles have five explicit binding rows each. Where the runtime represents a connector generically, regional profiles intentionally resolve the same reusable component; paper-family text remains profile-specific. `custom` requires exact existing component prototype names for component-backed dimensions and explicit text families for paper. Missing custom components produce an ItemSeeder diagnostic directing the operator to UsefulSeeder's Modern Item Components package.

Once later-era content is active, a non-empty world's selected profile is immutable by the normal reconcile path. Changing it can invalidate installed component graphs and requires a separately designed migration.

## Value, prose and mechanics

`CostIndex` is one global relative value index calculated as `10 × labour days`, after local wage normalisation and comparable-observation aggregation, then quantised to the 1–2–5 ladder. [The historical pricing methodology](./FutureMUD_Industrialised_Historical_Pricing_Methodology.md) governs evidence, analogues and rounding; generic anchors do not replace the calculation. Currency conversion and ordinary inflation do not create alternate identities. Justified garment/production/period differences in intrinsic value may require explicit base identities and admissions; seed order must never reprice one identity. Price and quality are assessed independently, without a universal handmade premium.

Descriptions follow the item authoring guide: overwhelmingly single-word noun heads, substantive full descriptions, no real brands, no claims of working controls where the component graph is inert, and no catalogue/provenance comments persisted for builders.

Prefer dedicated components for meaningful interactions. A powered craft tool uses `PowerTool`; a powered device with only switch/state presentation can use `PoweredProp`; an inert item must read as inert. Refill, consumable, battery, media, key/lock and lifecycle relationships must be explicit when gameplay depends on them.

## Activation evidence

The shared module is not independently selectable. Current code installs admitted shared rows when Industrial is selected and retains later-only admissions for their eras. `scripts/sync-industrialised-item-catalogue.ps1 -Check` checks current draft-generator parity, not editorial acceptance or the new clothing gates. Refresh currently regenerates source rows and must be made safe before curated clothing is introduced. Existing preflight/provenance/profile-lock infrastructure must be extended and retested against the accepted replacement catalogue; its presence does not certify clothing completeness. Recalculate programme craft/lifecycle coverage on accepted ordinary bases, excluding skins and repeated recipe/outfit use.
