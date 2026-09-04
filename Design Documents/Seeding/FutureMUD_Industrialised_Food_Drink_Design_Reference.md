# Industrialised Food and Drink Design Reference

## Authority, status and production goal

This is the authoritative package specification for food and drink across the Industrial, Modern, Nuclear and Information bands. It governs later implementation through `ItemSeeder`; it is not executable catalogue source.

The production goal is a globally useful, mechanically honest food system built from reusable earlier stock plus genuinely later foods, liquids, intermediates, production crafts and exact serving compositions. Every edible base must be a finished standalone item outside any package. Food skins are exceptional, and no generated prose or nominal variation may be used to reach a quota.

The four states below must never be conflated:

| State | Meaning at this baseline |
| --- | --- |
| **Approved requirements** | Requirements F01-F17 and Gates 1-7 in this document are the accepted design contract. |
| **Draft catalogue content** | The 700 rows in `IndustrialisedCatalogue/Items/food-drink-packaged.items.tsv` are rejected placeholders. Their counts, prose, materials, lifecycle links and identities are not accepted content. |
| **Implemented infrastructure** | Strict Gate 2 sources and preflight, prepared-food factual metadata, health-owned caffeine/illness stock, and persisted liquid freshness are implemented. Migration `20260904015558_AddLiquidFreshness` owns the new columns and restrictive result-liquid references. |
| **Verified production readiness** | Not achieved. Gate 1 passed on 4 September 2026; Gate 2 is awaiting editorial acceptance and Gates 3-7 remain open. Current Industrial selectability is not acceptance evidence. |

Planning evidence lives in the [inventory](./Industrialised_Food_Wave1_Inventory.tsv), [serving manifests](./Industrialised_Food_Wave1_Serving_Manifests.tsv), [complete reuse review](./Industrialised_Food_Wave1_Reuse_Review.tsv), [evidence and coverage register](./Industrialised_Food_Wave1_Evidence_and_Coverage.md), and [Gate 2 dependency audit](./Industrialised_Food_Dependency_Audit.tsv). Run `scripts/sync-industrialised-food-catalogue.ps1 -Check`, `-CheckReview`, or `-SelfTest`; the old Wave 1 checker forwards to it.

Gate 1 was approved by the user on 4 September 2026 against inventory SHA-256 `6386e9f33083ecff0dd6ef34c09d95dd1c6601e449097ff8cd7fa7d17b25f91c`, serving SHA-256 `fec044be93ddb7cdb861a866ab69accfc9e5d5ad95784788d70a0831da8e2791`, and reuse SHA-256 `6ca07dd22b017adbcfd904cb2374f2396620c977785788d56fc187bfed4a3639`. The approved Gate 2 amendment corrects physical counting without changing any of the 464 reserved identities: the exact matrix contains 397 items and 67 liquids. The 59 explicit drink reservations are liquids; eight beverage-intermediate reservations are also pourable liquids, while the remaining beverage intermediates are inventory-counted solids.

## Catalogue and ownership contract

### F01 - Reuse before duplication

Audit every one of the 2,775 existing food-item rows and 225 liquid rows. Give each exactly one disposition: `unchanged-reuse`, `adopted-later-era-dependency`, `superseded-by-explicit-era-version`, `unsuitable-duplication`, or `deferred-dependency`. Reuse does not erase historical admission restrictions, and a later process or package does not automatically require a new edible prototype.

### F02 - Complete twenty-family inventory

The maintained inventory must cover: staples; baking; noodles and dumplings; plant foods; fruit and nuts; dairy and eggs; meat and offal; aquatic foods; composite dishes; preserves and condiments; processed intermediates; desserts; snacks; rations; infant and dietetic foods; processed animal feeds; non-alcoholic drinks; stimulant infusions; alcoholic drinks; and beverage intermediates.

Each proposal records exact era admissions and a justified coverage cell across foodway, climate, class, religion, migration/trade, and household or institutional context. These are review lenses, not a country matrix. Technology profiles do not select food culture.

### F03 - Domain ownership

Agriculture owns raw crops and raw fodder. Butchery owns raw animal outputs. This package owns prepared foods and consumed intermediates, including processed livestock feed and pet food. Medical owns drug-bearing therapeutic products; ordinary infant, fortified, dietetic and oral-nutrition foods remain here. Logistics and retail own containers, labels, wrappers, sealed filled-package graphs and package mechanics.

### F04 - Standalone edible truth

Every food base has truthful noun, short and full descriptions, taste, ingredients, serving mass, nutrition, freshness, quality and economics when no package surrounds it. A biscuit is not a wrapper; canned food is represented by an edible content plus a separate filled-package graph. Package or venue/service markup is excluded from edible pricing.

### F05 - Identity and substitution

Use separate prototypes when defining ingredients, taste, nutrition, preservation, serving form, cultural identity, expected quality or intrinsic economics differ materially. Dynamic substitution is allowed only within the same recognisable dish and mechanical profile. A complete human-authored sentence may substitute whole reviewed ingredient or role values; scripts must never assemble descriptions from vocabulary fragments.

### F06 - Minimal skins

Food skins are exceptional and may change presentation only. They must not change ingredients, taste, portion, freshness, quality, price or mechanics. The unskinned base remains complete production content. Garnish, cooking degree, filling, icing or preservation differences that alter the eaten experience are normally separate bases or controlled craft outputs, not skins.

### F07 - Homogeneous foods and serving compositions

A homogeneous preparation is one food item. Separable sides, courses, condiments and drinks remain separate exact references. A serving manifest records context, ordered courses, item or liquid portions, optional vessels, and a nutrition-plausibility review. It is an ItemSeeder catalogue composition, not a new player-facing meal entity. Alternative menus are separate manifests.

### F08 - Production routes and failures

Every new or deliberately adopted processed product has at least one meaningful household, artisanal, institutional or mechanised route. Crafts require real inputs, tools, phases, outputs and failure behaviour. Mechanised production is an attended craft unless an existing subsystem truthfully models more. Shared underdone, burnt, scorched, curdled, stale or contaminated-result families may be reused; add a failure product only for a materially distinct result or hazard.

### F09 - Nutrition and portions

Use evidence-based nutrition profiles and serving sizes, with sourced exceptions. Profiles must distinguish energy/satiety and relevant macronutrient or hydration behaviour without claiming clinical precision. Direct stock uses its documented typical quality; crafts preserve earned quality. A serving manifest is rejected when its total portion or nutrition is implausible for the stated context.

### F10 - Ingredients, allergens and dietary metadata

Typed source records retain structured ingredient categories, major allergens, animal-feed purpose, and dietary or religiously relevant contents. This metadata supports validation and builders; it does not claim automatic suitability enforcement or universal cultural rules. Use an established food name with a concise English gloss where the name alone is not broadly legible.

### F11 - Price evidence

Price the edible retail unit, standard liquid volume or bulk feed mass using local unskilled-labour purchasing power. Preserve locale, year/range, nominal price and currency, quoted unit, wage basis, source/page, source class, comparable family and confidence. Calculate labour days locally, aggregate comparable observations by weighted median in log space, then quantise `CostIndex = 10 x labour days` to the 1-2-5 ladder. CPI may bridge nearby years within one locale; exchange-rate and present-money conversions are excluded. Every row needs direct evidence or an explicit documented analogue.

### F12 - Authored prose and editorial ownership

An agent or human author writes and stores every complete description and taste/smell text. Scripts may validate, package and export it, but may not generate or overwrite it. Text must agree with ingredients, materials and mechanics; avoid source notes, seeder terminology, brands, unsupported temperature, invented packages or dining accompaniments, and stereotypes about class or culture.

## Wave 2 infrastructure obligations

### F13 - Typed sources

Add normalized typed sources for foods, liquids, nutrition profiles, dietary metadata, crafts and ordered phases, serving manifests and entries, historical evidence, and exceptional skins. Preserve source file/line and review status as editorial metadata without persisting it into player-facing content. Load and validate the full graph before mutation.

### F14 - Health-owned ingestion profiles

Add reusable ingested-caffeine and food-borne-illness profiles through the established health owner. Caffeine, alcohol and illness claims must resolve to actual supported effects. The food catalogue references those profiles; it does not create a second health model.

### F15 - Persisted perishable-liquid freshness

Implement nullable freshness configuration on a liquid, with explicit stale and spoiled result liquids. Liquid-instance XML stores effective age, last resolution and irreversible reached stage. Splitting and transfer preserve proportional state. Same-liquid merging uses volume-weighted effective age but can never restore a stage already reached by either source. Unlike liquids in a mixture age independently.

Freshness resolution uses a new liquid-freshness time-rate channel so refrigeration can slow it independently of solid-item morphs. Legacy instances without freshness XML load fresh. Builder commands, validation, cloning, reference protection, XML compatibility and tests are required. This is persisted model work and therefore requires an EF migration plus blank-database snapshot update in Wave 2.

### F16 - Sealed-package integration boundary

Define an interface by which a sealed logistics-owned package may modify contained food or liquid freshness until irreversibly opened. Do not implement it in this planning tranche and do not fake it with descriptions. Thermal food states, detailed microbial simulation, cross-contamination and general package-opening simulation remain deferred unless separately approved.

### F17 - Exact preflight and provenance

Preflight must resolve stable references, exact admissions, materials, liquids, tags, components, nutrition, dietary metadata, evidence, crafts, failure products and serving entries before persistence. Existing ItemSeeder answer history and managed provenance remain the installer-facing path. No new selectable seeder package is introduced.

## Delivery sequence

1. **Inventory and evidence:** approve the maintained coverage cells, reuse dispositions, serving proposals, evidence routes, inclusions, deferrals and counts.
2. **Infrastructure:** implement F13-F17, including health prerequisites and the liquid-freshness migration.
3. **Representative proving set:** review live examples spanning dynamic ingredients, the rare valid skin case, hand and mechanised production, multipart service, animal feed, caffeine, illness, alcohol and refrigerated liquid ageing.
4. **Catalogue authoring:** write and review complete food/liquid prose, nutrition, exact dependencies, crafts, failures, pricing and admissions in cross-domain waves.
5. **Integration and acceptance:** materialise servings through supported integration points; prove repeatability, audit/manifest parity and live behaviour before activation.

## Completion gates

| Gate | Status | Required proof |
| --- | --- | --- |
| **1. Scope approved** | **Passed 4 September 2026** | Complete family/reuse/coverage matrix, inclusions and deferrals, serving proposals, source-backed evidence routes and approved counts without padding. |
| **2. Dependencies resolved** | **Implemented; awaiting editorial acceptance** | The 464 concepts, 307 exact adopted consumers, 26 serving manifests, typed source graph, health profiles, prepared-food metadata and liquid-freshness persistence validate. Automated evidence cannot accept the editorial matrix. |
| **3. Proving set accepted** | Open | Reviewed prose and live examples cover the difficult interactions listed in delivery step 3. |
| **4. Content complete** | Open | Every admitted item/liquid has reviewed prose, evidence, admissions, mechanics, price, freshness and review state. |
| **5. Crafts and servings complete** | Open | Production/failure paths work; exact ordered serving compositions resolve and can be materialised. |
| **6. Repeatability proven** | Open | Fresh install, identical rerun, pre-industrial addition, source update, customization preservation, retirement, migration compatibility and rollback pass. |
| **7. Production acceptance** | Open | Debug/Release builds, focused/full suites, catalogue/audit/manifest checks, database replays and representative live-MUD inspection pass. |

Automated checks cannot grant editorial approval. No unresolved critical dependency, generated placeholder, unsupported claim or unreviewed row may pass final acceptance. Food skins, reused foods, liquids, crafts, failure products and serving manifests are reported separately and never inflate ordinary-prototype or programme craft/lifecycle percentages.

## Boundaries and count reconciliation

Gate 1 approved the identity scope. The authorised physical-kind correction makes the current Stage 2 totals:

- shared ordinary prototypes: `4,773 + 226 shared food items = 4,999`;
- Industrial-only ordinary prototypes: `550 + 39 Industrial food items = 589`.

The 36 shared and six Industrial liquid reservations are reported separately. The later-only matrix contains 132 items and 25 liquids. Gate 2 source fingerprint `9c7a30b8cfe6045f1ab3f4caadeb7b5be2a811fa45539bf907c44d870302bdc4` covers every Industrialised catalogue TSV consumed by the shared loader; a source change must refresh and re-review the derived audit.

Modern-forward, Nuclear-forward and Information-only food bases are charged to their first-admitting later stage, not Stage 2. Reused foods and liquids remain their existing identities. Planning all four later bands does not activate them. Industrial activation still requires the entire Stage 2 programme gate, not food acceptance alone.
