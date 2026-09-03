# FutureMUD Industrialised ItemSeeder Programme

## Status and authority

This is the programme-level implementation plan for extending `ItemSeeder` from Early Modern stock through the Industrial, Modern, Nuclear and Information eras. It records accepted scope and sequencing; the live source, generated data exports and executable manifest remain authoritative for what the engine can currently seed.

The September 2026 first-pass planning brief is an input to this document, not an instruction file and not a catalogue source. Its inventory figures were superseded during tranche-zero audit. The checked-in live baseline is:

| Evidence | Current count |
|---|---:|
| Runtime item component types | 244 |
| Seeded reusable component prototypes | 4,402 |
| Materials | 976 |
| Liquids | 330 |
| Gases | 68 |
| Tags | 2,300 |
| ItemSeeder manifest entries | 42,754 |

These figures come from `Design Documents/Data/Item_Component_Types.json`, the `Seeded_*` exports, `SeededTagHierarchy.csv`, and `Seeded_Item_Manifest.json`. They must be refreshed before each catalogue tranche is admitted.

## Product boundary

The user experience remains entirely inside `ItemSeeder`. Later-era selection, named technology profiles, custom profile composition, scope selection and diagnostics are ItemSeeder questions. Implementation helpers and tabular catalogues may be separate files. Reusable component prototypes remain owned by UsefulSeeder's Modern Item Components package; ItemSeeder declares and diagnoses those prerequisites but does not silently create them.

The ordinary-item programme is separate from VehicleSeeder's canonical vehicle graphs, while ItemSeeder remains the single installer surface for both. The ordinary era names are `industrial`, `modern`, `nuclear` and `information`. Vehicle internals retain the compatibility tokens `revolution`, `modern`, `atomic` and `computer`; ItemSeeder maps the public names and accepts the old names as aliases.

## Catalogue shape

The original planning allocation was 8,800 ordinary item prototypes. The table below retains that baseline for reconciliation, not as a production-acceptance claim:

| Layer | Target | Purpose |
|---|---:|---|
| Shared industrialised baseline | 5,800 | Durable goods reusable across two or more later eras |
| Industrial delta | 650 | Mechanised production, steam and early electrical society |
| Modern delta | 700 | Mass electrification, motorisation, consumer and office systems |
| Nuclear delta | 800 | Post-war electronics, institutional and advanced industrial stock |
| Information delta | 850 | Digital networks, contemporary services and portable electronics |

Targets are planning controls, not permission to pad the catalogue. A new base needs a distinct form, component graph, use, material, lifecycle, craft or intrinsic economic role; compatible presentation differences belong in skins. Existing `preindustrial_*` references are reused for unchanged durable forms. New `industrialised_*` references are for genuinely later-shared forms, not renamed copies.

The [clothing, footwear and uniforms specification](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md) supersedes the 600 shared/70 Industrial clothing prototype quotas with coverage-based approval. With other Stage 2 allocations unchanged, revised totals are `5,200 + C_shared` and `580 + C_industrial`; the combined ordinary-item total is `5,780 + C_shared + C_industrial`. Clothing Gate 1 has approved the replacement planning inventory and counts below; reconcile tests, source and audits together during implementation. Count new bases, reused bases, skins, crafts and outfits separately; skins never count as ordinary prototypes. Later-era allocations also require reconciliation when their clothing inventories are approved.

The [Wave 1 inventory and coverage review](./Industrialised_Clothing_Wave1_Evidence_and_Coverage.md) has user scope/count approval dated 2026-09-03 (Gate 1 passed): 251 new bases (223 shared, 20 Industrial-only, two Modern-only and six Information-only), 113 reused bases, 697 additional skin briefs and 134 outfit proposals across all four bands. Each base includes its complete standalone unskinned presentation; additional-skin totals exclude a duplicate plain default. This approved planning scope replaces the clothing allocation and yields 5,423 shared plus 600 Industrial-only ordinary prototypes (6,023 total); the eight later-only deltas are outside that Stage 2 total. These planning counts do not yet replace source/test quotas or establish production acceptance. The user's colour clarification is incorporated: conventional colours are overridable outfit defaults, with no approved Wave 1 fixed-colour locks; Gates 2–7 remain open. The review records overlapping per-era counts, recipe obligations and required craft/lifecycle recalculation; 84 of the clothing outfits admit Industrial, and whole-programme loadouts still require reconciliation.

## Settled programme decisions

- Later-era keys stay non-selectable until their executable manifests contain substantive stock and pass activation gates. Empty module registration is architecture, not advertised functionality.
- The first content milestone is the complete shared baseline plus Industrial delta. Planning and research may cover all four later eras; later-era delta implementation/activation follows that milestone's source-backed validation.
- Catalogue source is domain TSV loaded through typed records. Generated manifests and inventory exports are review evidence, never hand-maintained substitutes for the source rows.
- All prices use the [historical pricing methodology](./FutureMUD_Industrialised_Historical_Pricing_Methodology.md): local unskilled-labour affordability, comparable-observation aggregation and `CostIndex = 10 × labour days` on the 1–2–5 ladder. Anchors are cross-checks, not an alternative pricing formula. No universal handmade/machine-made price or quality ranking applies.
- Named technology profiles cover neutral, North American, Continental European, British/Irish, Australasian, Japanese and Chinese standards, plus custom composition.
- The selected profile controls compatible families for power, paper, telecommunications, network/media and vehicle-service connectors. It does not introduce real-world brands.
- Controlled substances, weapons support, detonators and other sensitive but ordinary engine stock are part of the core catalogue. Tags, access controls, legality and craft knowledge express restriction; optional installation flags do not erase the category.
- Components are selected by gameplay meaning. Use a dedicated behaviour where the engine models it, `PowerTool` for craft tools, and `PoweredProp` or an inert form only when no genuine interaction is promised. Descriptions must not claim unsupported behaviour.
- Standard vehicle scope includes bicycles and independently route-bound rail stock. Coupled rail consists, aircraft, submarines and free-coordinate travel are later runtime extensions rather than catalogue promises.

## Stages and gates

### Tranche 0 - foundations

Deliver this programme, the shared and era references, the capability/gap ledger, inactive era registry, manifest modules, ItemSeeder standards-profile contract, replay coverage and tests. No later-era selection is activated.

Exit gate: Debug and Release seeder builds pass; replay and manifest tests pass; the generated manifest is current; all new questions are declared in every replay profile; documentation describes ownership and the activation rule.

### Stage 1 - prerequisites

Audit each required component, material, liquid, gas and tag against live source. Add missing reusable prototypes to UsefulSeeder, with stable names and behavioural tests. Add required materials and substances to their owning seeders. Add only source-backed tags.

Status: complete. The full 244-type registry and near-term resource audit recovered 22 source-seeded rows from export drift, added 16 valid rerunnable UsefulSeeder profiles, and added seven valid owner-controlled profiles for media recording, bolt actions, flares and remote or clock detonators. Thirty-nine runtime types have no same-type exported stock, and every one now has an explicit disposition: 18 futuristic/specialist deferrals, 6 system/context-owned types, 9 dependency-bound types and 6 honest alternate satisfactions. No type remains marked `reusable-stock-required`. All directly required audited materials, liquids, gases and tag paths resolve, so no speculative resource or empty taxonomy additions were made.

Exit gate: every runtime type has one deterministic structural-seam audit and one explicit stock disposition; no `reusable-stock-required` disposition remains; and every declared Stage 1 resource resolves by exact exported name. Exact dependencies in future shared/Industrial source rows are a Stage 2 admission check because those rows do not exist during Stage 1. Stage 2 must reject each row before persistence if an exact component, resource or tag dependency is unresolved.

### Stage 2 - shared plus Industrial

Implement typed TSV loading, admission validation and the shared/Industrial catalogue, including lifecycle and craft links where they add meaningful play. The original 5,800/650 allocations are subject to the approved clothing coverage reconciliation above. Generate review manifests and reconcile through ItemSeeder's existing provenance system.

Status: infrastructure implemented; production acceptance outstanding. The draft embedded source contains 5,800 `shared-industrialised` and 650 `industrial` ordinary rows across 24 domains, with recorded draft counts of 2,337 craft products (36.2%), 1,290 lifecycle participants (20.0%), 100 outfits and 40 canonical `vehicle_revolution_*` graphs. These are inventory/structural counts, not certification of substantive prose, meaningful craft/lifecycle coverage or physically complete outfits. The generated clothing rows specifically remain unaccepted drafts. The current typed preflight consumes Stage 1 metadata, but still needs the clothing specification's authored skin, colour, craft and outfit contracts. The canonical manifest contract is version 2 and currently contains 42,754 aggregates.

Current code makes `industrial` selectable, with `revolution` as its alias; Modern, Nuclear and Information remain inactive. This is premature relative to the outstanding production gates, not evidence that those gates passed. The documentation-only clothing change does not alter that flag. Resolving activation belongs to implementation/activation work, and clothing acceptance alone cannot close the whole Stage 2 gate.

Exit gate: row identity, dependency, editorial prose review, component compatibility, meaningful craft/lifecycle graphs, complete outfits, repeatability and fresh/update/customised-world tests pass, including all seven clothing gates. Recalculate at least 35% direct craftability and a distinct additional 20% supported lifecycle participation against accepted ordinary base items, not skins or repeated recipes. Only then approve Industrial activation.

Recorded infrastructure evidence: the source/audit command is `scripts/sync-industrialised-item-catalogue.ps1 -Check`, and the two Debug replay profiles are `industrial-neutral` and `industrial-custom`. The earlier implementation record reports that on 2 September 2026 both profiles completed the full 29-seeder fresh-database chain; each admitted 6,270 ordinary Stage 2 rows, with 180 shared computing/network rows reserved for later-era admissions. It also records fixes for manifest-query scaling and EF reattachment, a 22-stage populated update on 3 September 2026 with no blocked aggregates, and live presence checks across 23 Industrial-visible domains and 40 vehicles. Preserve that history, but do not treat presence checks or draft-generator parity as editorial, manufacturing, colour or outfit acceptance. The replacement clothing catalogue requires new evidence against its own source fingerprint. The current refresh generator overwrites source rows and must be made safe for authored clothing before bulk authoring.

### Stages 3 to 5 - Modern, Nuclear and Information

Repeat the source, implementation and activation gate independently for each era. An era may consume the admitted shared layer and all sensible prior forms without duplicating stable identities. Each activation is a product decision backed by populated manifests and tests.

### Stage 6 - integration and release readiness

Run full seeder tests, manifest checks, fresh and populated database replays, export drift checks and representative live-MUD inspection. Review installer wording and release notes. Publication is outside this programme unless separately requested.

## Document map

- [Shared industrialised baseline](./FutureMUD_Industrialised_Shared_Baseline_Design_Reference.md)
- [Clothing, footwear and uniforms specification and production gates](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md)
- [Industrial era master reference](./FutureMUD_Industrial_Item_Seeder_Master_Era_Design_Reference.md)
- [Modern era master reference](./FutureMUD_Modern_Item_Seeder_Master_Era_Design_Reference.md)
- [Nuclear era master reference](./FutureMUD_Nuclear_Item_Seeder_Master_Era_Design_Reference.md)
- [Information Age master reference](./FutureMUD_InformationAge_Item_Seeder_Master_Era_Design_Reference.md)
- [Capability, material and tag gap ledger](./FutureMUD_Industrialised_Component_Material_Tag_Gap_Reference.md)
- [Runtime component prerequisite audit](./Industrialised_Component_Prerequisite_Audit.tsv)
- [Resource prerequisite audit](./Industrialised_Resource_Prerequisite_Audit.tsv)
- [Historical pricing methodology](./FutureMUD_Industrialised_Historical_Pricing_Methodology.md)
- [Generated Stage 2 catalogue audit](./Industrialised_Item_Catalogue_Audit.tsv)
- [Shared era architecture](./Era_Seeder_Shared_Architecture.md)
- [Repeatability strategy](./DatabaseSeeder_Repeatability_Strategy.md)
- [Vehicle item design reference](./Vehicle_Item_Seeder_Design_Reference.md)

## Change control

Changes to counts, public keys, ownership, activation gates, technology-profile dimensions or stable-reference policy must update this document and the affected era master in the same change. Domain-level working plans live inside the era masters until their catalogues become large enough to justify dedicated source-adjacent references.
