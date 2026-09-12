# FutureMUD Industrialised Component, Resource and Engine Audit

## Authority and repeatable method

This is the Stage 1 audit record for the later-era ItemSeeder programme. Runtime registration is authoritative for available component families; the seeder sources are authoritative for reusable stock; the maintained JSON, TSV and CSV files are review and drift-detection surfaces.

Run `scripts/audit-industrialised-prerequisites.ps1` to refresh the factual audit columns and `scripts/audit-industrialised-prerequisites.ps1 -Check -NoBuild` to reject drift. A deliberately maintained disposition must use a `manual:` prefix; ordinary factual dispositions are recalculated from live registration and export state.

The September 2026 audited baseline is:

| Evidence | Count |
| --- | ---: |
| Canonical runtime component types | 244 |
| Modern-tagged runtime types | 109 |
| Futuristic-tagged runtime types | 18 |
| General runtime types | 117 |
| Exported reusable component profiles | 4,557 |
| Runtime types without same-type exported stock | 39 |
| Materials | 977 |
| Liquids | 330 |
| Gases | 68 |
| Tag hierarchy paths | 2,300 |

The complete one-row-per-runtime-type evidence is in `Industrialised_Component_Prerequisite_Audit.tsv`. It records canonical database keys, builder aliases, prototype and runtime implementation classes, technology classification, exclusive interfaces, known sibling requirements, XML load/save availability, create/load/revision-copy paths, builder-command availability, runtime copy support, export counts, ownership and disposition. `Item_Component_Types.json` is generated from the same runtime snapshot and is no longer a hand-maintained partial list. It also records whether a prototype overrides `PreventManualLoad`, so catalogue/outfit preflight can exclude runtime-generated-only items from manual materialisation without a separately maintained type list.

The original Stage 1 closure held 4,402 profiles. Wave 2 clothing prerequisites add 43 HumanSeeder wearable configurations (41 missing geometry families, a lowered-hood alternative and corrected paired garters), 109 consumed layer variants and three UsefulSeeder Variable configurations for leather, wood and lacquer colours. The current total is 4,557, including 360 Wearable and 25 Variable profiles. CoreDataSeeder supplies the three associated characteristic profiles from existing colour values. Wearable stock ownership is correctly attributed to HumanSeeder and AnimalSeeder, not UsefulSeeder. Runtime type counts and the 39 type-level dispositions are unchanged. These profiles are prerequisites, not additional finished garments or an extra installer package; full clothing dependencies remain governed by the [Wave 2 gate register](./Industrialised_Clothing_Wave2_Infrastructure_and_Gate2.md).

## Closure delivered in Stage 1

The maintained component export gained 45 source-backed rows:

- 22 rows recovered from live source that the old export omitted: seven modern magazines, pin-pull and countdown detonators, three shop stalls, three market-good weights and seven measuring instruments;
- 16 new rerunnable UsefulSeeder profiles: reusable and single-use bank payment, a modern cash register, three power-tool classes, a workshop compressor, domestic and commercial refrigerators, a domestic dryer, a USB-C power bank, blank keycard, standard keycard reader and writer, domestic washing machine and standard vending machine;
- seven final closure profiles: a UsefulSeeder audio-visual digital media recorder plus CombatSeeder bolt-action rifle, handheld flare, flare ammunition, clock detonator, radio detonator and radio transmitter profiles.

These profiles use the dedicated runtime families. No `PoweredProp` substitute, invalid foreign-key placeholder, real-world brand or finished ItemSeeder catalogue row was introduced.

## Final no-same-type stock dispositions

The 39 remaining type-level differences are not 39 promises to add profiles:

| Disposition | Count | Types and treatment |
| --- | ---: | --- |
| Futuristic or specialist deferment | 18 | Implant, neural, laser and power-pack families remain inventoried but are outside the first Industrialised content milestone. |
| System or context owned | 6 | `Dwelling`, `Prog Light`, `Prog Lock`, `ProgPowerSupply`, `Puddle` and `Stable Ticket` are not ordinary reusable catalogue dependencies. |
| Dependency-bound | 9 | `BiometricScanner` requires a selected anatomy shape; `BreathingFilter`, `FaxMachine` and `Photocopier` require finished-item references; `Salvageable` requires concrete material/item outputs; `SignalDetonator` requires a concrete signal source and endpoint; and the three vehicle types belong to VehicleSeeder graphs. None may receive placeholder IDs. |
| Honest alternate satisfied | 6 | `Board`, `Changer`, `Food`, `Fuse`, `Selectable` and `Wieldable` have sufficient current semantic stock for the near-term catalogue. |
| Reusable stock still required | 0 | Every straightforward reusable profile is now supplied by its established owner. |

No new engine component family is required for the profiles delivered here. Dependency-bound profiles move to their owning Stage 2 domain only alongside the exact references that make them valid.

## Materials, liquids, gases and tags

`Industrialised_Resource_Prerequisite_Audit.tsv` records every resource currently required by the delivered profile set and near-term shared/Industrial infrastructure. All entries resolve exactly in the maintained exports, including structural metals, copper, silicon, plastics, rubber, composite materials, fuels, detergent, machine and hydraulic oils, common industrial gases, R-134a, all four later-era roots, and the existing tool, household, communications, repair, transport and warehousing market paths.

No new material, liquid, gas or tag was added merely to make this tranche appear complete. Copper wire is an item form of copper rather than a new material; electronic waste is an item/category concern rather than a material; and speculative electrical, office or appliance tag branches remain deferred until Stage 2 has real rows that consume them.

Wave 2's approved clothing inventory identifies `industrialised_clothing_pith_helmet` as the consumer of distinct plant-pith stock. CoreDataSeeder now supplies `pith` and three shola/sola aliases, preserving cork as a separate material. The [evidence and parameter rationale](./Industrialised_Clothing_Wave2_Infrastructure_and_Gate2.md#pith-material-prerequisite) distinguish the genus-density analogue and gameplay estimates from measured data. Existing tag paths are reused. Fresh/rerun tests verify aliases, custom-property preservation and maintained-export parity; all 364 planned clothing material references resolve to 26 unique stock solids. This does not complete garment composition, native colours or outfit validation.

## Engine and authoring conclusions

- The engine exposes more dedicated Industrialised behavior than the tranche-zero export suggested: refrigeration, drying, portable power, modern artillery, access control, media, automation and vehicle component families are registered and persisted.
- Canonical database names and builder/help names are not always textually identical. The audit therefore joins by the registration that produced them instead of assuming spaces and casing are interchangeable.
- Context-dependent sibling requirements remain visible even when no single capability can be stated at type level; static requirements such as explosive payloads are exported explicitly.
- The structural seam columns prove that every registered prototype has XML load/save, create, component-load, revision-copy and builder-command paths and that every runtime component has a copy path. Focused tests cover the newly adopted stock definitions and the corrected duration-aware power contract.
- `IProducePower` now has a duration-aware spike overload. Power tools pass their instantaneous wattage separately from use duration, finite battery stores debit watt-hours, and continuous providers retain their established instantaneous-capacity behavior.
- ItemSeeder remains the sole installer experience. UsefulSeeder, CombatSeeder and other domain owners supply reusable prerequisites; ItemSeeder only selects finished stock and reports unresolved exact names.

Stage 1 is complete as an engine, structural-seam, stock and resource audit. Check mode fails if any registration returns to `reusable-stock-required` or if a maintained factual artifact drifts. Stage 2 owns exact row-level dependency validation and must not persist a row until all named dependencies resolve.

The [Industrialised food and drink specification](./FutureMUD_Industrialised_Food_Drink_Design_Reference.md) records Gate 2 dependencies rather than retroactively reopening Stage 1. Typed nutrition/dietary sources, health-owned caffeine and food-borne-illness profiles, persisted perishable-liquid freshness, and migration `20260904015558_AddLiquidFreshness` are now implemented. The [derived dependency audit](./Industrialised_Food_Dependency_Audit.tsv) resolves all 464 concepts, 307 adopted consumers and 26 serving manifests structurally; the matrix awaits editorial acceptance and later gates still own exact prose, prices and executable production crafts. Sealed-package preservation remains a logistics integration contract, not an available component.
