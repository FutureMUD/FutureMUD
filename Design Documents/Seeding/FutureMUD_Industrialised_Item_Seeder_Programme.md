# FutureMUD Industrialised ItemSeeder Programme

## Status and authority

This is the programme-level implementation plan for extending `ItemSeeder` from Early Modern stock through the Industrial, Modern, Nuclear and Information eras. It records accepted scope and sequencing; the live source, generated data exports and executable manifest remain authoritative for what the engine can currently seed.

The September 2026 first-pass planning brief is an input to this document, not an instruction file and not a catalogue source. Its inventory figures were superseded during tranche-zero audit. The checked-in live baseline is:

| Evidence | Current count |
|---|---:|
| Runtime item component types | 244 |
| Seeded reusable component prototypes | 4,395 |
| Materials | 976 |
| Liquids | 330 |
| Gases | 68 |
| Tags | 2,300 |
| ItemSeeder manifest entries | 33,824 |

These figures come from `Design Documents/Data/Item_Component_Types.json`, the `Seeded_*` exports, `SeededTagHierarchy.csv`, and `Seeded_Item_Manifest.json`. They must be refreshed before each catalogue tranche is admitted.

## Product boundary

The user experience remains entirely inside `ItemSeeder`. Later-era selection, named technology profiles, custom profile composition, scope selection and diagnostics are ItemSeeder questions. Implementation helpers and tabular catalogues may be separate files. Reusable component prototypes remain owned by UsefulSeeder's Modern Item Components package; ItemSeeder declares and diagnoses those prerequisites but does not silently create them.

The ordinary-item programme is separate from VehicleSeeder's canonical vehicle graphs, while ItemSeeder remains the single installer surface for both. The ordinary era names are `industrial`, `modern`, `nuclear` and `information`. Vehicle internals retain the compatibility tokens `revolution`, `modern`, `atomic` and `computer`; ItemSeeder maps the public names and accepts the old names as aliases.

## Catalogue shape

The planning target is 8,800 ordinary item prototypes:

| Layer | Target | Purpose |
|---|---:|---|
| Shared industrialised baseline | 5,800 | Durable goods reusable across two or more later eras |
| Industrial delta | 650 | Mechanised production, steam and early electrical society |
| Modern delta | 700 | Mass electrification, motorisation, consumer and office systems |
| Nuclear delta | 800 | Post-war electronics, institutional and advanced industrial stock |
| Information delta | 850 | Digital networks, contemporary services and portable electronics |

Targets are planning controls, not permission to pad the catalogue. A row is admitted only when it has a distinct form, component graph, use, material, lifecycle, craft, market role or presentation. Existing `preindustrial_*` references are reused for unchanged durable forms. New `industrialised_*` references are for genuinely later-shared forms, not renamed copies.

## Settled programme decisions

- Later-era keys stay non-selectable until their executable manifests contain substantive stock and pass activation gates. Empty module registration is architecture, not advertised functionality.
- The first content milestone is the complete shared baseline plus Industrial delta. Modern work begins only after that milestone is source-backed and validated.
- Catalogue source is domain TSV loaded through typed records. Generated manifests and inventory exports are review evidence, never hand-maintained substitutes for the source rows.
- All prices use one global relative value index. Each domain must document calibration anchors; regional currency conversion is outside catalogue rows.
- Named technology profiles cover neutral, North American, Continental European, British/Irish, Australasian, Japanese and Chinese standards, plus custom composition.
- The selected profile controls compatible families for power, paper, telecommunications, network/media and vehicle-service connectors. It does not introduce real-world brands.
- Controlled substances, weapons support, detonators and other sensitive but ordinary engine stock are part of the core catalogue. Tags, access controls, legality and craft knowledge express restriction; optional installation flags do not erase the category.
- Components are selected by gameplay meaning. Use a dedicated behaviour where the engine models it, `PowerTool` for craft tools, and `PoweredProp` or an inert form only when no genuine interaction is promised. Descriptions must not claim unsupported behaviour.
- Standard vehicle scope includes bicycles and coupled rail stock. Aircraft, submarines and free-coordinate travel are later runtime extensions rather than catalogue promises.

## Stages and gates

### Tranche 0 - foundations

Deliver this programme, the shared and era references, the capability/gap ledger, inactive era registry, manifest modules, ItemSeeder standards-profile contract, replay coverage and tests. No later-era selection is activated.

Exit gate: Debug and Release seeder builds pass; replay and manifest tests pass; the generated manifest is current; all new questions are declared in every replay profile; documentation describes ownership and the activation rule.

### Stage 1 - prerequisites

Audit each required component, material, liquid, gas and tag against live source. Add missing reusable prototypes to UsefulSeeder, with stable names and behavioural tests. Add required materials and substances to their owning seeders. Add only source-backed tags.

Status: the full 244-type registry and near-term resource audit is complete. The pass recovered 22 source-seeded rows from export drift and added 16 valid rerunnable UsefulSeeder profiles. Forty-six runtime types have no same-type exported stock, but each now has an explicit disposition: 18 futuristic/specialist deferrals, 6 system/context-owned types, 6 dependency-bound types, 6 honest alternate satisfactions and 10 owner-specific stock candidates. All directly required audited materials, liquids, gases and tag paths already resolve, so no speculative resource or empty taxonomy additions were made.

Exit gate: every shared/Industrial row dependency resolves by exact exported name; there are no unexplained type-only placeholders or alternate-type assumptions.

### Stage 2 - shared plus Industrial

Implement typed TSV loading, admission validation, the 5,800 shared target and 650 Industrial delta, including lifecycle and craft links where they add meaningful play. Generate review manifests and reconcile through ItemSeeder's existing provenance system.

Exit gate: row identity, dependency, prose, component compatibility, craft graph, repeatability and representative fresh/update/customised-world tests pass. Only then make `industrial` selectable.

### Stages 3 to 5 - Modern, Nuclear and Information

Repeat the source, implementation and activation gate independently for each era. An era may consume the admitted shared layer and all sensible prior forms without duplicating stable identities. Each activation is a product decision backed by populated manifests and tests.

### Stage 6 - integration and release readiness

Run full seeder tests, manifest checks, fresh and populated database replays, export drift checks and representative live-MUD inspection. Review installer wording and release notes. Publication is outside this programme unless separately requested.

## Document map

- [Shared industrialised baseline](./FutureMUD_Industrialised_Shared_Baseline_Design_Reference.md)
- [Industrial era master reference](./FutureMUD_Industrial_Item_Seeder_Master_Era_Design_Reference.md)
- [Modern era master reference](./FutureMUD_Modern_Item_Seeder_Master_Era_Design_Reference.md)
- [Nuclear era master reference](./FutureMUD_Nuclear_Item_Seeder_Master_Era_Design_Reference.md)
- [Information Age master reference](./FutureMUD_InformationAge_Item_Seeder_Master_Era_Design_Reference.md)
- [Capability, material and tag gap ledger](./FutureMUD_Industrialised_Component_Material_Tag_Gap_Reference.md)
- [Runtime component prerequisite audit](./Industrialised_Component_Prerequisite_Audit.tsv)
- [Resource prerequisite audit](./Industrialised_Resource_Prerequisite_Audit.tsv)
- [Shared era architecture](./Era_Seeder_Shared_Architecture.md)
- [Repeatability strategy](./DatabaseSeeder_Repeatability_Strategy.md)
- [Vehicle item design reference](./Vehicle_Item_Seeder_Design_Reference.md)

## Change control

Changes to counts, public keys, ownership, activation gates, technology-profile dimensions or stable-reference policy must update this document and the affected era master in the same change. Domain-level working plans live inside the era masters until their catalogues become large enough to justify dedicated source-adjacent references.
