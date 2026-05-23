# Antiquity Food And Beverage Crafting Suite

## Scope

The antiquity food and beverage pass lives in `ItemSeeder` partials and runs in the normal `ItemSeeder` rework path. It assumes the butchery package has already installed raw meat cuts, offal, bone, fat, fish, hides, and animal-product tags, then adds commodity-first processing chains rather than duplicating per-animal recipes.

The implementation files are:

- `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityFood.cs`
- `DatabaseSeeder/Seeders/ItemSeeder.Rework.AntiquityApiary.cs`
- `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityFood.cs`
- `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityApiary.cs`

## Processing Chains

The shared chains are intentionally broad and tag-driven:

- grain threshing, winnowing, milling, flour, meal, bran, and wort stock
- pulse splitting and grinding into pulse meal
- vegetable chopping, direct fruit preparation, olive-style brined fruit, and fruit must pressing
- oilseed crushing, vegetable/olive oil pressing, oilseed cake, and rendered-fat stock preparation
- raw meat and offal breakdown from `AnimalButcherySeeder` outputs
- raw meat cooking, salting, drying, smoking, rendering, and broth boiling
- beer, date beer, wine-style fruit beverages, kumis, spiced luxury beverages, broth, and garum-style sauce filling
- ceramic serving and fermenting amphora finishing from the household pottery blank pipeline
- apiary equipment and raw honeycomb processing into reusable honey and beeswax stock

The skill split is deliberately narrower than Farming: threshing and winnowing use `Threshing`, flour/meal/pulse/oil milling uses `Milling`, and wort or beer work uses `Brewing`. Fruit must pressing uses `Brewing`, while chopped vegetables, fruit serving, brining, cooked meat, broth, preserved foods, and most finished dishes remain cooking-led. Flatbread uses `Baking`.

Crafts prefer commodity inputs such as `Raw Meat Commodity`, `Prepared Meat Commodity`, `Flour Commodity`, and `Pulse Meal Commodity` so one tagged chain can serve beef, lamb, goat, fish, or other seeded meat families without creating species-specific recipe explosions.

## Tool And Vessel Closure

Food tools are tagged with their exact functional tool tags and `Market / Professional Tools / Standard Tools`, so the shared antiquity equipment toolmaking suite discovers and crafts them without duplicating one-off recipes in the food file. This covers the butcher's knife, cooking knife, threshing flail, winnowing basket, hand quern, mortar and pestle, grain sieve, fruit press, oil press, mash tun, drying rack, smoking rack, and salting trough.

Empty food vessels are explicit pottery crafts:

- `antiquity_food_serving_amphora` is finished from fired-clay `Bisque Vessel Blank` stock and sealed with `Prepared Pitch`.
- `antiquity_food_fermenting_amphora` is finished from fired-clay `Bisque Vessel Blank` stock with heavier `Prepared Pitch` lining and carries the fermentation amphora tool tag.
- `antiquity_food_finished_beer_amphora` is intentionally a morph target for `antiquity_food_fermenting_beer_amphora`, not a direct craft output.

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

Each culture gets fourteen foodway crafts: the original flatbread, porridge, pulse stew, meat-grain dish, preserved meat ration, fruit sweet, and beverage amphora plus fresh fruit platter, oilseed cakes, spiced meat stew, honeyed pastry, fish sauce relish, stuffed flatbread, and spiced beverage amphora. Five of the seven new entries are high-end preparations distinguished by imported spices, honey, oil, fermented sauce, broth, brined fruit, or multi-stage cooking. Visible craft names are plain food actions; culture-specific access is enforced by the knowledge gate.

Wine cultures consume `Fruit Must Commodity` and rely on the core liquids `red wine` and `white wine` from `CoreDataSeeder.Materials.cs`; the food seeder does not duplicate those base liquids. The Scythian-Sarmatian kumis path consumes `LiquidUse - 3 litres of milk` for the beverage stock rather than the grain-wort fallback. A future pastoral pass can split out mare's milk as a more specific stock source without changing the current foodway craft surface.

## Reusable Stock Outputs

Most intermediate products are consumed downstream by other food crafts. The following are deliberately reusable stock outputs for later cuisine, economy, or preservation passes:

- `Rendered Fat Commodity`
- `Smoked Meat Commodity`
- `Oilseed Mash Commodity`
- `Brined Fruit Commodity`
- `Pressed Honey`
- `Rendered Beeswax`

These remain commodity stock because they are useful in multiple future recipe families and should not require one-off item prototypes.

`Oilseed Cake Commodity` is now consumed by the culture-gated oilseed cake crafts, but it remains commodity stock because the same press cake can sensibly feed ordinary baking, animal feed, or later cuisine-specific recipes.

## Stable References

The craft source explicitly names the shared vessel references:

- `antiquity_food_serving_amphora`
- `antiquity_food_fermenting_amphora`
- `antiquity_food_fermenting_beer_amphora`
- `antiquity_food_finished_beer_amphora`

The food tool stable references are:

- `antiquity_food_butchers_knife`
- `antiquity_food_cooking_knife`
- `antiquity_food_threshing_flail`
- `antiquity_food_winnowing_basket`
- `antiquity_food_quern`
- `antiquity_food_mortar`
- `antiquity_food_grain_sieve`
- `antiquity_food_fruit_press`
- `antiquity_food_oil_press`
- `antiquity_food_mash_tun`
- `antiquity_food_drying_rack`
- `antiquity_food_smoking_rack`
- `antiquity_food_salting_trough`

The apiary stable references are:

- `antiquity_wicker_beehive`
- `antiquity_clay_tube_hive`
- `antiquity_wooden_hive_stand`
- `antiquity_bee_smoke_pot`
- `antiquity_honey_knife`
- `antiquity_honey_press`
- `antiquity_honey_strainer`

Culture-specific prepared-food prototypes are generated with the `antiquity_food_` stable-reference prefix. The current suffix set is:

- `<culture>_flatbread`
- `<culture>_porridge`
- `<culture>_pulse_stew`
- `<culture>_meat_dish`
- `<culture>_preserved_meat`
- `<culture>_sweet`
- `<culture>_fruit_platter`
- `<culture>_oilseed_cake`
- `<culture>_spiced_meat_stew`
- `<culture>_honeyed_pastry`
- `<culture>_fish_sauce_relish`
- `<culture>_stuffed_flatbread`
- `antiquity_food_prepared_fruit`
- `antiquity_food_brined_fruit`

where `<culture>` is one of `hellenic`, `egyptian`, `roman`, `celtic`, `germanic`, `kushite`, `punic`, `persian`, `etruscan`, `anatolian`, or `scythian_sarmatian`.

## Runtime Adjacent Content

The food pass also seeds builder-owned `CommoditySpoilageRule` rows for raw meat, prepared meat, salted meat, dried meat, smoked meat, and broth-base commodities. These rules turn matching commodity piles into rotten food commodities on heartbeat instead of relying on item morphs or static JSON.

`LiquidProduct` is the craft-product type used where a craft should create a concrete liquid container already filled with a named liquid, such as barley beer, date beer, meat broth, garum sauce, spiced wine, spiced beer, or spiced kumis.

## Deferred Source Systems

This pass closes current ItemSeeder craftability gaps and now has a seeded apiculture source path. Agriculture apiary operations can produce raw honeycomb, pressed honey, and rendered beeswax, and antiquity crafts can build the hives, stands, smoke pots, honey knives, presses, and strainers that support that chain. Existing honeyed food, medical, writing, leather, and household crafts can therefore trace honey and beeswax back to seeded field work.

It still does not add new primary-production systems for pastoral secondary products, mining and quarrying, dye and spice derivative supply, and similar upstream source systems. Current recipes use existing core liquids, materials, agricultural commodities, apiary commodities, butchery outputs, and household pottery stock.
