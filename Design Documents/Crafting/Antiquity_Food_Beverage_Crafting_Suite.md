# Antiquity Food And Beverage Crafting Suite

## Scope

The antiquity food and beverage pass lives in `ItemSeeder` partials and runs in the normal `ItemSeeder` rework path. It assumes the butchery package has already installed raw meat cuts, offal, bone, fat, fish, hides, and animal-product tags, then adds commodity-first processing chains rather than duplicating per-animal recipes.

The implementation files are:

- `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityFood.cs`
- `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityFood.cs`

## Processing Chains

The shared chains are intentionally broad and tag-driven:

- grain threshing, winnowing, milling, flour, meal, bran, and wort stock
- pulse splitting and grinding into pulse meal
- vegetable chopping, direct fruit preparation, olive-style brined fruit, and fruit must pressing
- oilseed crushing, vegetable/olive oil pressing, oilseed cake, and rendered-fat stock preparation
- raw meat and offal breakdown from `AnimalButcherySeeder` outputs
- raw meat cooking, salting, drying, smoking, rendering, and broth boiling
- beer, date beer, wine-style fruit beverages, kumis, broth, and garum-style sauce filling

The skill split is deliberately narrower than Farming: threshing and winnowing use `Threshing`, flour/meal/pulse/oil milling uses `Milling`, and wort or beer work uses `Brewing`. Fruit must pressing uses `Brewing`, while chopped vegetables, fruit serving, brining, cooked meat, broth, preserved foods, and most finished dishes remain cooking-led. Flatbread uses `Baking`.

Crafts prefer commodity inputs such as `Raw Meat Commodity`, `Prepared Meat Commodity`, `Flour Commodity`, and `Pulse Meal Commodity` so one tagged chain can serve beef, lamb, goat, fish, or other seeded meat families without creating species-specific recipe explosions.

## Culture Coverage

The culture-gated final craft suites use `Foodways` knowledge gates:

| Culture | Knowledge Gate | Staple Surface |
| --- | --- | --- |
| Hellenic | `Hellenic Foodways` | barley, chickpea, fig, wine |
| Egyptian | `Egyptian Foodways` | emmer wheat, lentil, date, beer |
| Roman | `Roman Foodways` | spelt wheat, bean, grape, wine |
| Celtic | `Celtic Foodways` | oat, pea, apple, beer |
| Germanic | `Germanic Foodways` | rye, pea, apple, beer |
| Kushite | `Kushite Foodways` | pearl millet, cowpea, date, date beer |
| Punic | `Punic Foodways` | wheat, chickpea, fig, wine |
| Persian | `Persian Foodways` | barley, lentil, grape, wine |
| Etruscan | `Etruscan Foodways` | spelt wheat, bean, grape, wine |
| Anatolian | `Anatolian Foodways` | einkorn wheat, lentil, fig, wine |
| Scythian-Sarmatian | `Scythian-Sarmatian Foodways` | millet, pea, date, kumis |

Each culture gets flatbread, porridge, pulse stew, meat-grain dish, preserved meat ration, fruit sweet, and a beverage amphora craft. Visible craft names are plain food actions; culture-specific access is enforced by the knowledge gate.

## Reusable Stock Outputs

Most intermediate products are consumed downstream by other food crafts. The following are deliberately reusable stock outputs for later cuisine, economy, or preservation passes:

- `Rendered Fat Commodity`
- `Smoked Meat Commodity`
- `Oilseed Mash Commodity`
- `Oilseed Cake Commodity`
- `Brined Fruit Commodity`

These remain commodity stock because they are useful in multiple future recipe families and should not require one-off item prototypes.

## Stable References

The craft source explicitly names the shared vessel references:

- `antiquity_food_serving_amphora`
- `antiquity_food_fermenting_beer_amphora`

Culture-specific prepared-food prototypes are generated with the `antiquity_food_` stable-reference prefix. The current suffix set is:

- `<culture>_flatbread`
- `<culture>_porridge`
- `<culture>_pulse_stew`
- `<culture>_meat_dish`
- `<culture>_preserved_meat`
- `<culture>_sweet`
- `antiquity_food_prepared_fruit`
- `antiquity_food_brined_fruit`

where `<culture>` is one of `hellenic`, `egyptian`, `roman`, `celtic`, `germanic`, `kushite`, `punic`, `persian`, `etruscan`, `anatolian`, or `scythian_sarmatian`.

## Runtime Adjacent Content

The food pass also seeds builder-owned `CommoditySpoilageRule` rows for raw meat, prepared meat, salted meat, dried meat, smoked meat, and broth-base commodities. These rules turn matching commodity piles into rotten food commodities on heartbeat instead of relying on item morphs or static JSON.

`LiquidProduct` is the craft-product type used where a craft should create a concrete liquid container already filled with a named liquid, such as barley beer, date beer, meat broth, or garum sauce.
