# Industrialised food and drink - Wave 1 evidence, coverage and scope review

## Status and authority

This is the maintained Gate 1 approval package for the [food and drink specification](./FutureMUD_Industrialised_Food_Drink_Design_Reference.md). It records a complete planning inventory, exact virtual identity reservations, all 3,000 historical reuse dispositions, serving proposals and research routes. It does not contain finished player prose, prices or executable food definitions.

**Gate 1 status: passed by explicit user approval on 4 September 2026.** Approval is pinned to inventory SHA-256 `6386e9f33083ecff0dd6ef34c09d95dd1c6601e449097ff8cd7fa7d17b25f91c`, serving SHA-256 `fec044be93ddb7cdb861a866ab69accfc9e5d5ad95784788d70a0831da8e2791`, and reuse SHA-256 `6ca07dd22b017adbcfd904cb2374f2396620c977785788d56fc187bfed4a3639`. The 700 current generated rows remain rejected draft content. This approval does not approve Gate 2 editorial content, later content, Industrial activation, or any later-era activation.

Gate 2 records an authorised accounting correction without changing the approved 464 identities: 397 are inventory-counted items and 67 are liquids. All 59 explicit drink reservations and eight pourable beverage intermediates are liquids; the other intermediates remain items. See the [dependency audit](./Industrialised_Food_Dependency_Audit.tsv).

## Audited source baseline

The live source under `DatabaseSeeder/Seeders/FoodCatalogue` contains 2,775 food items and 225 liquids. The [reuse review](./Industrialised_Food_Wave1_Reuse_Review.tsv) reproduces every stable reference, file and line exactly once. It treats 202 intermediate item rows and 105 reusable stock/brewing/cooking liquids as adopted later-era production dependencies; the other 2,693 records remain unchanged reusable food or liquid stock. No current row has been declared superseded, unsuitable or deferred without an item-level editorial decision.

`unchanged-reuse` means that the existing identity remains valid and may continue to exist; it does not make the food universal. `adopted-later-era-dependency` means later production may intentionally consume the existing stock while preserving its admissions and identity. Neither disposition counts as a new ordinary prototype.

## Coverage matrix

The proposed bases are coverage reservations rather than finished descriptions. Each row in the [inventory](./Industrialised_Food_Wave1_Inventory.tsv) defines one of the twenty required families and reserves exact, non-overlapping identities for its planned bases.

| Family | Shared IMNF | Industrial I | Modern-forward MNF | Nuclear-forward NF | Information F | Principal omissions being filled |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| staples | 10 | 2 | 3 | 1 | 1 | later milling, fortification, instant and institutional staple forms |
| baking | 16 | 3 | 5 | 2 | 2 | chemical leavening, standardised baking and convenience products |
| noodles-and-dumplings | 14 | 2 | 4 | 1 | 2 | dried, extruded, instant, frozen and factory-filled forms |
| plant-foods | 14 | 2 | 4 | 1 | 2 | preserved, textured, convenience and institutional preparations |
| fruit-and-nuts | 10 | 1 | 3 | 1 | 2 | industrial drying, pureeing and portioned transformations |
| dairy-and-eggs | 14 | 2 | 4 | 1 | 2 | standardised, concentrated, dried and convenience forms |
| meat-and-offal | 16 | 3 | 5 | 2 | 2 | processed, comminuted, cured and convenience foods |
| aquatic-foods | 10 | 2 | 3 | 1 | 1 | later preservation, processing and transport-ready service forms |
| composite-dishes | 28 | 5 | 9 | 3 | 5 | global household, street, restaurant, institutional and convenience dishes |
| preserves-and-condiments | 14 | 3 | 4 | 1 | 2 | standardised, emulsified and concentrated preparations |
| processed-intermediates | 14 | 3 | 4 | 2 | 2 | extracts, concentrates, cultures and powders with identified consumers |
| desserts | 18 | 3 | 6 | 2 | 4 | later textures, stabilisation, cold-chain identity and production forms |
| snacks | 16 | 2 | 5 | 2 | 3 | portable street, workplace, school, leisure and transport foods |
| rations | 12 | 3 | 4 | 2 | 3 | military, naval, expedition, emergency, relief and institutional components |
| infant-and-dietetic | 8 | 1 | 3 | 2 | 3 | ordinary infant, fortified, convalescent and texture-modified nutrition |
| processed-animal-feeds | 8 | 2 | 2 | 1 | 2 | compounded livestock, working-animal and pet feeds |
| non-alcoholic-drinks | 12 | 2 | 4 | 2 | 3 | processed dairy, fruit, grain, carbonated and nutrition drinks |
| stimulant-infusions | 8 | 1 | 3 | 1 | 2 | later coffee, tea, cacao and other evidenced stimulant preparations |
| alcoholic-drinks | 12 | 2 | 4 | 1 | 2 | later fermentation, distillation, blending and reduced-alcohol forms |
| beverage-intermediates | 8 | 1 | 2 | 1 | 1 | roasted, dried, concentrated, cultured and dispensing inputs |
| **Total** | **262** | **45** | **81** | **30** | **46** | **464 new bases across the four-band plan** |

Every family row explicitly covers production, reuse and evidence strategy. Editorial expansion at Gate 4 must assign each reserved identity a precise food concept, ingredient profile and source; it may delete an unjustified reservation but may not fill it with a nominal flavour, shape, brand or package variant. Any count change returns Gate 1 to review.

### Cross-cutting acceptance lenses

| Lens | Gate 1 decision |
| --- | --- |
| Exact era admissions | Shared identities admit two or more later bands; the current reservation set uses all four. Industrial, Modern, Nuclear and Information reservations are sole-band deltas. Later-only bases do not enter Stage 2. |
| Global foodways | The whole inventory is tested across connected regional traditions and migration/trade routes; no country quota or technology-profile culture mapping is used. |
| Climate | Temperate, tropical, arid, cold, maritime and highland storage/service constraints are review lenses. Temperature claims need actual support. |
| Class and economy | Household, subsistence, working, middle-income, luxury and public provisioning contexts are covered without stereotypes. Price is attached to the edible unit, not venue prestige. |
| Religion and diet | Fasting, ritual, abstention and ingredient relevance are metadata and serving decisions, never universal suitability claims. |
| Institutions | Workplace, school, hospital, prison, military, maritime, rail, aviation, emergency and care settings appear in inventory or serving proposals. |
| Packaging | Packages, labels, filled graphs and seal mechanics are owned by logistics/retail. Food remains complete outside them. |
| Production | Household, artisanal, institutional and mechanised routes are planned. Every eventual processed product needs at least one real route and failure behavior. |

## Serving-manifest review

The [serving source](./Industrialised_Food_Wave1_Serving_Manifests.tsv) proposes 26 exact compositions spanning factory, mine, rail, maritime, school, hospital, prison, religious, hospitality, household, aviation, emergency, care and animal-feeding contexts. Entries name reserved item or liquid identities, explicit gram or millilitre portions, and course order. Vessels and packages are deliberately absent until exact reusable logistics items are selected at Gate 2.

These are composition proposals, not claims that placeholder identities already have the described contents. Gate 4 assigns the identity concepts and Gate 5 must revalidate every portion, nutrition profile and ordered course. Alternative menus require distinct manifests rather than conditional prose.

## Evidence register

Evidence codes in the inventory route later row-level research. They are not permission to copy prose or infer unsupported dates.

| Code | Research package | Intended authoritative sources and decision |
| --- | --- | --- |
| NUT | nutrition and portions | [USDA FoodData Central](https://fdc.nal.usda.gov/), national food-composition tables, institutional ration specifications and contemporary serving evidence; select an analogue and document deviations |
| PRICE | labour-relative prices | [Bank of England research datasets](https://www.bankofengland.co.uk/statistics/research-datasets), [NBER Macrohistory prices](https://www.nber.org/research/data/nber-macrohistory-iv-prices), ONS long-run series, wage series and dated catalogues; apply the programme labour-day method |
| GLOBAL | foodway, migration and regional form | scholarly food histories, museum collections, primary cookbooks and trade records; establish ingredient/process identity and admission without a national matrix |
| TRADE | crop, stimulant and commodity movement | primary trade statistics and scholarly histories; establish producing regions, adoption windows and ordinary versus specialist availability |
| BAKE | baking and cereal processing | milling/baking technical manuals, standards and trade catalogues; distinguish leavening, flour, texture, process and unit economics |
| DAIRY | dairy and egg processing | dairy science manuals, agricultural experiment reports and trade standards; establish treatment, composition, yield and freshness |
| MEAT | meat and offal processing | butchery/food-industry manuals and standards; distinguish edible product from raw butchery output and package |
| AQUATIC | aquatic processing | fisheries reports, preservation manuals and institutional specifications; establish species category, process, yield and storage |
| PRESERVE | preservation and condiments | canning, fermentation, dehydration and sauce standards; separate food chemistry from container mechanics |
| PROCESS | intermediates and manufacturing | food-processing manuals and ingredient standards; require at least one identified consumer and supported craft route |
| SWEET | confectionery and desserts | confectionery/baking manuals and trade catalogues; establish composition, portion, texture and process without flavour padding |
| RATION | military, relief and transport provisioning | dated official ration schedules, procurement specifications and relief standards; model edible components separately from issued package |
| CARE | infant and dietetic foods | dated public-health guidance and institutional specifications; avoid clinical claims not supported by runtime mechanics |
| FEED | processed animal feed | agricultural extension, feed standards and veterinary nutrition sources; record feed purpose without universal enforcement |
| BEVERAGE | non-alcoholic and beverage processing | beverage technical standards, soda/dairy/juice manuals and service evidence; establish liquid composition and freshness |
| STIM | caffeine | analytical food data and health references; bind caffeine-bearing products to the future reusable health profile |
| ALCOHOL | alcoholic strength and serving | production standards and measured strength evidence; use exact alcohol-per-litre values and culturally bounded admissions |

Each future catalogue row must cite a source/page or an approved analogue row. High-value, mechanised, fortified, therapeutic-adjacent and institutionally specified products require direct contemporary evidence. The evidence ledger will preserve original observations; a category code alone cannot pass Gate 4.

## Explicit inclusions and deferrals

Included are standalone edible foods; food liquids; consumed intermediates; processed animal feeds; ordinary infant/dietetic foods; exceptional mechanics-neutral skins; production crafts and material failure products; exact serving compositions; structured ingredients, allergens and dietary/religious contents; caffeine and food-borne-illness prerequisites; and persisted perishable-liquid freshness.

Deferred or externally owned are raw crops/fodder, raw animal outputs, drugs, packages/labels/wrappers, venue markup, player-facing meal entities, universal dietary enforcement, contamination networks, cross-contamination, microbial simulation, general thermal food state and package-opening freshness. Sealed-package preservation is only a logistics integration contract until separately implemented.

## Counts and programme reconciliation

| Measure | Count |
| --- | ---: |
| Shared IMNF new bases | 262 |
| Industrial-only new bases | 45 |
| Modern-forward MNF new bases | 81 |
| Nuclear-forward NF new bases | 30 |
| Information-only new bases | 46 |
| All proposed new bases | 464 |
| Existing food items audited | 2775 |
| Existing liquids audited | 225 |
| Adopted later-era dependencies | 307 |
| Unchanged reusable records | 2693 |
| Exceptional skins | 0 at Gate 1; any proposal returns for explicit review |
| Crafts | 0 implemented; one or more required per new or deliberately adopted processed product |
| Failure families | 0 accepted; reuse-first mapping is a Gate 2 deliverable |
| Serving manifests proposed | 26 |
| Proposed Stage 2 shared ordinary total | 5035 |
| Proposed Stage 2 Industrial-only ordinary total | 595 |

The Stage 2 formulas are `4,773 + 262 = 5,035` shared ordinary prototypes and `550 + 45 = 595` Industrial-only ordinary prototypes. The 157 Modern-forward, Nuclear-forward and Information-only bases are charged to their first-admitting later stage. Existing foods, liquids, future transformation liquids, skins, crafts, failure products and serving manifests remain separate measures.

## Maintained fingerprints

These hashes bind explicit user approval to the exact planning package. Updating a source requires editorial re-review and replacement of the corresponding row; mentioning a new hash elsewhere is not approval.

| Artefact | SHA-256 |
| --- | --- |
| Inventory | `6386e9f33083ecff0dd6ef34c09d95dd1c6601e449097ff8cd7fa7d17b25f91c` |
| Serving manifests | `fec044be93ddb7cdb861a866ab69accfc9e5d5ad95784788d70a0831da8e2791` |
| Reuse review | `6ca07dd22b017adbcfd904cb2374f2396620c977785788d56fc187bfed4a3639` |

## Gate 1 approval record

Structural validation proves exact headers, 20 unique family rows, 464 non-overlapping virtual identity reservations, exact scope totals, 26 valid serving graphs, 3,000 one-row dispositions, current live source/file/line parity, required documentation links and maintained fingerprints. `-SelfTest` proves malformed identity and stale-review detection.

Gate 1 **passed by explicit user approval on 4 September 2026** against the fingerprint set above. Gate 2's authorised physical-kind accounting correction does not change the approved 464 identities; any substantive identity or scope change reopens Gate 1.
