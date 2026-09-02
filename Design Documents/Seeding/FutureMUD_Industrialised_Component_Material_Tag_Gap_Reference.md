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
| Exported reusable component profiles | 4,395 |
| Runtime types without same-type exported stock | 46 |
| Materials | 976 |
| Liquids | 330 |
| Gases | 68 |
| Tag hierarchy paths | 2,300 |

The complete one-row-per-runtime-type evidence is in `Industrialised_Component_Prerequisite_Audit.tsv`. It records canonical database keys, builder aliases, implementation class, technology classification, exclusive interfaces, known sibling requirements, export counts, ownership and disposition. `Item_Component_Types.json` is generated from the same runtime snapshot and is no longer a hand-maintained partial list.

## Closure delivered in this tranche

The maintained component export gained 38 source-backed rows:

- 22 rows recovered from live source that the old export omitted: seven modern magazines, pin-pull and countdown detonators, three shop stalls, three market-good weights and seven measuring instruments;
- 16 new rerunnable UsefulSeeder profiles: reusable and single-use bank payment, a modern cash register, three power-tool classes, a workshop compressor, domestic and commercial refrigerators, a domestic dryer, a USB-C power bank, blank keycard, standard keycard reader and writer, domestic washing machine and standard vending machine.

These profiles use the dedicated runtime families. No `PoweredProp` substitute, invalid foreign-key placeholder, real-world brand or finished ItemSeeder catalogue row was introduced.

## Remaining no-same-type stock dispositions

The 46 remaining type-level differences are not 46 promises to add profiles:

| Disposition | Count | Types and treatment |
| --- | ---: | --- |
| Futuristic or specialist deferment | 18 | Implant, neural, laser and power-pack families remain inventoried but are outside the first Industrialised content milestone. |
| System or context owned | 6 | `Dwelling`, `Prog Light`, `Prog Lock`, `ProgPowerSupply`, `Puddle` and `Stable Ticket` are not ordinary reusable catalogue dependencies. |
| Dependency-bound | 6 | `BreathingFilter`, `FaxMachine`, `Photocopier`, `Vehicle Access Point`, `Vehicle Cargo Space` and `Vehicle Exterior` require finished-item or domain-specific references and must not receive placeholder IDs. |
| Honest alternate satisfied | 6 | `Board`, `Changer`, `Food`, `Fuse`, `Selectable` and `Wieldable` have sufficient current semantic stock for the near-term catalogue. |
| Reusable stock still required | 10 | `BiometricScanner`, `BoltAction`, `ClockDetonator`, `Digital Media Recorder`, `Flare`, `FlareAmmunition`, `RadioDetonator`, `RadioDetonatorTransmitter`, `Salvageable` and `SignalDetonator` need a concrete domain consumer or additional owner-specific closure before Stage 2 uses them. |

No new engine component family is required for the 16 profiles delivered here. The remaining ten stock candidates have runtime support; their gap is reusable configuration and domain admission, not missing engine mechanics.

## Materials, liquids, gases and tags

`Industrialised_Resource_Prerequisite_Audit.tsv` records every resource currently required by the delivered profile set and near-term shared/Industrial infrastructure. All entries resolve exactly in the maintained exports, including structural metals, copper, silicon, plastics, rubber, composite materials, fuels, detergent, machine and hydraulic oils, common industrial gases, R-134a, all four later-era roots, and the existing tool, household, communications, repair, transport and warehousing market paths.

No new material, liquid, gas or tag was added merely to make this tranche appear complete. Copper wire is an item form of copper rather than a new material; electronic waste is an item/category concern rather than a material; and speculative electrical, office or appliance tag branches remain deferred until Stage 2 has real rows that consume them.

## Engine and authoring conclusions

- The engine exposes more dedicated Industrialised behavior than the tranche-zero export suggested: refrigeration, drying, portable power, modern artillery, access control, media, automation and vehicle component families are registered and persisted.
- Canonical database names and builder/help names are not always textually identical. The audit therefore joins by the registration that produced them instead of assuming spaces and casing are interchangeable.
- Context-dependent sibling requirements remain visible even when no single capability can be stated at type level; static requirements such as explosive payloads are exported explicitly.
- ItemSeeder remains the sole installer experience. UsefulSeeder, CombatSeeder and other domain owners supply reusable prerequisites; ItemSeeder only selects finished stock and reports unresolved exact names.

Stage 1 is complete as an engine and stock audit. Its remaining stock candidates are explicit inputs to the first domain catalogue rather than hidden assumptions. Stage 2 must not consume any candidate until its row disposition becomes exported stock, an honest named alternate, or a documented domain-bound dependency.
