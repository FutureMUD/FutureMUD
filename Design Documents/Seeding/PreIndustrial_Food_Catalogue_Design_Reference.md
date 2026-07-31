# Pre-Industrial Food Catalogue Design Reference

## Scope

The pre-industrial food catalogue supplies cooked and prepared foods, edible food liquids, and intermediate food products for the Medieval, Renaissance, and Early Modern ItemSeeder selections. It deliberately treats broadly recurring dishes as shared prototypes and uses admission data to control culture, contact, and date availability.

A local grain substitution does not by itself justify another prototype. For example, ordinary gruel, pulse stew, roast fish, fresh cheese, and preserved meat are shared food forms. Era-prefixed rows are reserved for genuinely distinct recipes, processing methods, service traditions, or historically bounded ingredient combinations.

## Catalogue target

The completed source catalogue contains approximately 3,000 independently reviewable records:

| Scope | Prepared or intermediate item prototypes | Food liquids | Total records |
| --- | ---: | ---: | ---: |
| Shared pre-industrial | 2,100 | 150 | 2,250 |
| Medieval-specific | 225 | 25 | 250 |
| Renaissance-specific | 225 | 25 | 250 |
| Early Modern-specific | 225 | 25 | 250 |
| **Total** | **2,775** | **225** | **3,000** |

Shared rows therefore make up 75% of the catalogue. This is intentional: culture and period should govern access to common food forms without creating English gruel, French gruel, and German gruel as separate base prototypes.

The 2,100 shared item rows are kept in review-sized culinary files:

| Shared source | Rows |
| --- | ---: |
| grain and bread | 300 |
| porridge, noodles, and dumplings | 250 |
| pulses and vegetables | 250 |
| soups and stews | 250 |
| meat, poultry, and offal | 300 |
| fish and shellfish | 200 |
| dairy and eggs | 150 |
| preserved and travel foods | 150 |
| fruit, nuts, and sweets | 150 |
| condiments and discrete intermediates | 100 |
| **Shared item total** | **2,100** |

## Source and packaging

Authored catalogue rows live under `DatabaseSeeder/Seeders/FoodCatalogue` as tab-separated source files embedded into `DatabaseSeeder.dll`. `PreIndustrialFoodCatalogue` parses those resources through a typed schema. Embedding keeps deployment self-contained while preserving compact, diffable source data.

The item schema records:

- stable reference and shared/era scope;
- prepared-food or intermediate kind;
- culinary family;
- noun and individually authored short, full, and taste descriptions;
- material;
- standard nutrition and freshness bands;
- item quality, weight, and cost;
- admission profile.

The liquid schema records:

- source stable reference and shared/era scope;
- culinary family and persistent liquid name;
- individually authored display, long, taste, and smell descriptions;
- display colour;
- alcohol, water, food-satiation, and drink-satiation values per litre;
- admission profile.

No description text is assembled from ingredient-name templates. The loader serialises component XML and common metadata, but every catalogue row supplies its own player-facing prose.

## Stable references and ownership

- Shared item rows use `preindustrial_food_*`.
- Medieval-only rows use `medieval_food_*`.
- Renaissance-only rows use `renaissance_food_*`.
- Early Modern-only rows use `earlymodern_food_*`.
- Every edible item owns a deterministic `PreparedFood_Catalogue_<stable reference>` component prototype.
- Liquid names are the persisted identity used by the engine; their catalogue stable references provide source and manifest identity.

The food catalogue is stock-owned. Rerunning ItemSeeder reconciles authored names, descriptions, materials, weights, costs, quality, PreparedFood definitions, required components, and liquid nutrition rather than only suppressing duplicates.

PreparedFood short/full templates are intentionally empty. This causes the runtime decorator to preserve the item prototype's authored descriptions while still appending bite and freshness state; a literal `$sdesc` is not a supported PreparedFood template token.

## Prepared foods and intermediates

A prepared-food row is a directly loadable, sellable, forageable, or craft-output item with exactly one `PreparedFood` component.

Use intermediate rows for countable or inspectable stages such as:

- shaped dough portions;
- bundles of noodles;
- pressed curd cakes;
- wrapped spice or herb mixtures;
- prepared stuffing;
- dried fruit sheets;
- discrete fermentation starters.

Use the engine commodity system, rather than another item prototype, for fungible bulk stages such as flour, meal, bran, malt, wort stock, mash, rendered fat, bulk curds, chopped vegetables, and prepared meat. Commodity identity is material plus commodity tag; it is not an edible item prototype.

## Nutrition standards

Satiation and thirst values are in the runtime's normal hours-based units. Catalogue rows choose a named band; they do not introduce pseudo-precise one-off values.

| Band | Satiation | Water | Thirst | Bites | Typical use |
| --- | ---: | ---: | ---: | ---: | --- |
| `BleakThin` | 1.5 | 0.35 | 0.10 | 6 | watery gruel, famine broth |
| `BleakSolid` | 2.5 | 0.05 | -0.05 | 6 | coarse cake, poor ration |
| `Light` | 2.5 | 0.12 | 0.00 | 4 | snack or small bowl |
| `Standard` | 4.0 | 0.12 | 0.00 | 6 | ordinary serving |
| `Staple` | 5.0 | 0.08 | -0.05 | 8 | bread, dumpling, dense grain staple |
| `Hearty` | 6.0 | 0.25 | 0.05 | 8 | substantial stew or meal |
| `Rich` | 6.5 | 0.20 | 0.00 | 8 | enriched meat, dairy, or pastry dish |
| `Feast` | 8.0 | 0.25 | 0.00 | 10 | large elite feast portion |
| `Sweet` | 3.5 | 0.05 | -0.05 | 5 | pastry, fruit or nut sweet |
| `Preserved` | 3.5 | 0.02 | -0.25 | 6 | salted, dried, or smoked ration |
| `Fresh` | 2.0 | 0.25 | 0.15 | 5 | prepared fruit or fresh vegetable |
| `Condiment` | 0.5 | 0.02 | -0.15 | 3 | relish, paste, or small sauce serving |

Prepared foods use a quality nutrition scale of `0.08` per quality step. Portion size remains the main nutrition control: high quality does not turn a small sweet into a full day's meal.

Food liquids also use a deliberately small numeric vocabulary rather than per-row pseudo-precision:

- alcohol litres per litre: `0`, `0.01`, `0.02`, `0.03`, `0.04`, `0.05`, `0.06`, `0.08`, `0.10`, `0.12`, `0.15`, `0.18`, `0.20`, `0.25`, `0.30`, `0.40`, or `0.50`;
- water litres per litre: `0`, `0.10`, `0.25`, `0.50`, `0.65`, `0.75`, `0.85`, `0.90`, `0.95`, or `1.00`;
- food-satiation hours per litre: `0`, `0.25`, `0.50`, `1`, `1.50`, `2`, `3`, `4`, `5`, or `6`;
- drink-satiation hours per litre: `0`, `0.25`, `0.50`, `1`, `1.50`, `2`, `3`, or `4`.

Thin broths and ordinary drinks sit at the low end of food satiation; thick dairy, cereal, chocolate, and pulse liquids sit higher. Strong alcohol supplies less water and thirst relief than table drink. Oils, concentrated syrups, and sauces are culinary liquids rather than normal thirst-quenching beverages.

## Quality policy

- Bleak or famine foods may be `Poor`, `Substandard`, or `Standard`, never above `Standard`.
- Ordinary common dishes are normally `Standard`.
- Competently enriched or socially rich dishes are `Good`.
- Refined banquet, court, guild-feast, or high confectionery foods may be `VeryGood`.
- `Great` is reserved for exceptional showpiece dishes and is intentionally rare.
- Rich and feast nutrition bands must have quality above `Standard`.

This separates ingredient abundance and preparation quality from raw serving size.

## Freshness standards

Rows choose one of the maintained freshness bands:

- `Fresh`: stale after 24 hours, spoiled after 72 hours;
- `Cooked`: stale after 48 hours, spoiled after 96 hours;
- `Bread`: stale after 72 hours, spoiled after 168 hours;
- `Dry`: stale after 168 hours, spoiled after 720 hours;
- `Preserved`: stale after 336 hours, spoiled after 2,160 hours;
- `Fermented`: stale after 168 hours, spoiled after 720 hours;
- `ShelfStable`: stale after 720 hours, spoiled after 4,320 hours.

Bulk commodities use `CommoditySpoilageRule` records instead.

## Admission model

Shared rows carry an admission profile rather than an era prefix. The generated Medieval, Renaissance, and Early Modern shared-food admission manifests expand those profiles into:

- culture/contact scope;
- date window;
- admitting household, profession, shop, institution, military system, or craft;
- ordinary, specialist, elite, imported, export-only, or not-admitted availability;
- production and trade status;
- component reality.

Admission profiles include universal forms, regional Old World forms, European, Islamicate, South Asian, East Asian, Sub-Saharan African, Indigenous American, Mesoamerican, Andean, maritime-trade, sugar-trade, tea-trade, coffee-trade, cacao-trade, and New World post-contact forms.

Ingredient history still applies to a generic dish. A shared stew does not authorise potato, tomato, maize, cacao, tea, or coffee inputs before the matching crop/contact admission.

## Seeder flow

When any of Medieval, Renaissance, or Early Modern is selected:

1. ItemSeeder installs the shared food catalogue once.
2. The selected era method installs its genuinely era-specific rows.
3. Prepared-food component definitions and liquid records are reconciled from stock source.
4. The matching admission manifest governs normal builder/craft/shop/culture use.

The catalogue now also has a generalized craft path. Item rows are grouped by scope, kind, family, and source material rather than receiving one craft each. A generated FutureProg loads the group's possible prototypes, shuffles them, and returns one selected item; `ProgCookedFoodProduct` creates that prepared-food prototype through `CookedFoodProduct`, preserving its ingredient-ledger and effect-transfer behavior. The product also accepts a collection-returning selector for future recipes that intentionally yield several prepared servings from one craft. Intermediate rows use the same single-selection contract through `ProgProduct`, and each liquid row uses a direct `LiquidProduct` into the shared 13.1-litre amphora. This reduces the 3,000 output records to 322 grouped item crafts plus 225 liquid crafts while preserving a repeatable craftable path for every record.

All catalogue crafts declare agriculture or animal-butchery tag-based inputs, including crop, fruit, oilseed, dairy, egg, honey, meat, fish, and offal sources. Agriculture commodity inputs use the live `Seeded Yield`, `Raw Milk`, `Egg Product`, or `Pressed Honey` pile contracts rather than material taxonomy names that are not produced piles. Tree-nut and chickpea rows use the Agriculture `Food Crop` plus `Seeded Yield` contract, while apiary honey rows use `Pressed Honey`; this keeps the generalized recipes attached to live upstream production rather than inventing fallback stock. Each craft has explicit source ownership, a Cooking tool dependency for general preparations or a Brew Copper dependency for grain drinks, fermented drinks, wine, and spirits, and operation-specific in-character preparation echoes for milling, baking, simmering, preservation, pressing, brewing, and drinks. They are seeded only for Medieval, Renaissance, and Early Modern installations.

The Antiquity foodways package follows the same selector-product principle while retaining its culture-specific item identities. Its two shared prepared dishes keep their direct cooked-food recipes, while the eleven culture families use twelve generalized `ProgCookedFoodProduct` crafts. Each selector loads the applicable culture prototypes, chooses one at craft completion, and passes that prototype through the normal `CookedFoodProduct` ingredient-ledger path. This keeps the 132 culture dishes craftable without maintaining one nearly identical craft per culture output; the culture beverage amphora recipes remain two per culture because their vessel morph targets and source liquids are distinct.

Antiquity is not automatically given the Medieval/Renaissance/Early Modern catalogue. Its independent foodway package is seeded by the Antiquity ItemSeeder selection and now uses the grouped culture selectors described above.

## Validation contract

Regression tests enforce:

- the exact 3,000-record allocation;
- unique stable references, liquid names, short descriptions, full descriptions, tastes, and smell/long-description prose where applicable;
- no six-word description scaffold repeated more than eight times across a prose field family;
- valid embedded TSV schemas;
- source-backed materials, tags, and required base components;
- one stock-owned PreparedFood component per edible prototype;
- no legacy `Food` component on catalogue dishes;
- standard nutrition/freshness values only;
- bleak food at or below `Standard` and rich food above `Standard`;
- shared-versus-era stable-reference and admission-profile consistency;
- exact admission-manifest parity with the shared catalogue;
- stock-owned rerun reconciliation without duplicate items, components, liquids, or tags.
- generalized craft coverage for every item and liquid output, including selector-prog validity, Antiquity culture-selector coverage, and repeatable craft/knowledge/prog seeding.
