# Antiquity Food And Beverage Crafting Suite

## Scope

The antiquity food and beverage pass lives in `ItemSeeder` partials and runs in the normal `ItemSeeder` rework path. It assumes the butchery package has already installed raw meat cuts, offal, bone, fat, fish, hides, and animal-product tags, then adds commodity-first processing chains rather than duplicating per-animal recipes.

The implementation files are:

- `DatabaseSeeder/Seeders/ItemSeeder.AntiquityFood.cs`
- `DatabaseSeeder/Seeders/ItemSeeder.AntiquityApiary.cs`
- `DatabaseSeeder/Seeders/ItemSeeder.Crafting.AntiquityFood.cs`
- `DatabaseSeeder/Seeders/ItemSeeder.Crafting.AntiquityApiary.cs`
- `DatabaseSeeder/Seeders/ItemSeeder.Crafting.AntiquityAgriculture.cs`

## Processing Chains

The shared chains are intentionally broad and tag-driven:

- grain threshing, winnowing, milling, flour, meal, bran, and wort stock
- pulse splitting and grinding into pulse meal
- vegetable chopping, direct fruit preparation, olive-style brined fruit, and fruit must pressing
- oilseed crushing, vegetable/olive oil pressing, oilseed cake, and rendered-fat stock preparation
- raw meat and offal breakdown from `AnimalButcherySeeder` outputs
- raw meat cooking, salting, drying, smoking, rendering, and broth boiling
- beer, date beer, wine-style fruit beverages, kumis, spiced luxury beverages, and garum-style sauce fermenting or aging amphora workflows, plus broth filling
- ceramic serving and fermenting amphora finishing from the household pottery blank pipeline
- apiary equipment and raw honeycomb processing into reusable honey and beeswax stock
- agricultural seed-stock selection from `Seeded Yield` commodity into `Seeds` commodity
- pastoral milk straining from raw herd milk commodity into liquid milk amphorae
- pastoral manure composting from raw herd manure and crop refuse
- agricultural derivative processing for indigo dye cake, pomegranate rind, walnut hull, and saffron stock, with crop or woodland sources for the common antiquity dye plants

The skill split is deliberately narrower than Farming: threshing and winnowing use `Threshing`, flour/meal/pulse/oil milling uses `Milling`, and wort or beer work uses `Brewing`. Fruit must pressing uses `Brewing`, dye derivatives use `Dyeing`, seed selection uses `Farming`, while chopped vegetables, fruit serving, milk straining, brining, cooked meat, broth, preserved foods, and most finished dishes remain cooking-led. Flatbread uses `Baking`.

Crafts prefer commodity inputs such as `Raw Meat Commodity`, `Prepared Meat Commodity`, `Flour Commodity`, and `Pulse Meal Commodity` so one tagged chain can serve beef, lamb, goat, fish, or other seeded meat families without creating species-specific recipe explosions.

## Tool And Vessel Closure

Food tools are tagged with their exact functional tool tags and retain `Market / Professional Tools / Standard Tools` for economy use. The shared antiquity equipment toolmaking suite discovers and crafts them through functional tool roots without duplicating one-off recipes in the food file. This covers the butcher's knife, cooking knife, threshing flail, winnowing basket, pitchfork, hand quern, mortar and pestle, grain sieve, fruit press, oil press, mash tun, drying rack, smoking rack, and salting trough.

Empty food vessels are explicit pottery crafts:

- `antiquity_food_serving_amphora` is finished from fired-clay `Bisque Vessel Blank` stock and sealed with `Prepared Pitch`.
- `antiquity_food_fermenting_amphora` is finished from fired-clay `Bisque Vessel Blank` stock with heavier `Prepared Pitch` lining and carries the fermentation amphora tool tag.
- Finished beverage and sauce amphorae are intentionally morph targets, not direct craft outputs. Active fermenting or aging amphorae cover barley beer, date beer, red wine, white wine, kumis, garum sauce, spiced wine, spiced beer, and spiced kumis.

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

Wine cultures consume `Fruit Must Commodity` and rely on the core liquids `red wine` and `white wine` from `CoreDataSeeder.Materials.cs`; the food seeder does not duplicate those base liquids. The Scythian-Sarmatian kumis path consumes `LiquidUse - 3 litres of milk` for the beverage stock rather than the grain-wort fallback. Beverage foodway crafts now produce sealed fermenting or aging amphorae that morph into finished serving amphorae, so wine, beer, date beer, kumis, garum, and spiced luxury beverages all use the same delayed production pattern. A future species-specific livestock pass can split out mare's milk as a more specific stock source without changing the current foodway craft surface.

The agriculture pass now supplies the broader antiquity luxury inputs used by all cultures: coriander, cumin, saffron crocus, and black pepper are stock crops, and saffron crocus is processed into saffron. Pomegranate and walnut orchard outputs can be stripped into dye stock, indigo crop can be fermented into indigo dye cake, madder, weld, alkanet, and henna are stock dye crops, and managed woodland definitions cover kermes grain, orchil lichen, and lac dye cake. These are culture-neutral upstream crafts and field definitions so Hellenic, Egyptian, Roman, Celtic, Germanic, Kushite, Punic, Persian, Etruscan, Anatolian, and Scythian-Sarmatian recipes can share the same source paths.

## Reusable Stock Outputs

Most intermediate products are consumed downstream by other food crafts. The following are deliberately reusable stock outputs for later cuisine, economy, or preservation passes:

- `Rendered Fat Commodity`
- `Smoked Meat Commodity`
- `Oilseed Mash Commodity`
- `Brined Fruit Commodity`
- `Pressed Honey`
- `Rendered Beeswax`
- `Raw Milk`
- `Egg Product`
- `Manure Commodity`
- `compost` tagged as manure commodity stock
- `Textile Dye Stock`
- `Seeds`

These remain commodity stock because they are useful in multiple future recipe families and should not require one-off item prototypes.

`Oilseed Cake Commodity` is now consumed by the culture-gated oilseed cake crafts, but it remains commodity stock because the same press cake can sensibly feed ordinary baking, animal feed, or later cuisine-specific recipes.

## Stable References

The craft source explicitly names the shared vessel references:

- `antiquity_food_serving_amphora`
- `antiquity_food_fermenting_amphora`
- `antiquity_food_fermenting_beer_amphora`
- `antiquity_food_finished_beer_amphora`
- `antiquity_food_fermenting_date_beer_amphora`
- `antiquity_food_finished_date_beer_amphora`
- `antiquity_food_fermenting_red_wine_amphora`
- `antiquity_food_finished_red_wine_amphora`
- `antiquity_food_fermenting_white_wine_amphora`
- `antiquity_food_finished_white_wine_amphora`
- `antiquity_food_fermenting_kumis_amphora`
- `antiquity_food_finished_kumis_amphora`
- `antiquity_food_fermenting_garum_amphora`
- `antiquity_food_finished_garum_amphora`
- `antiquity_food_aging_spiced_wine_amphora`
- `antiquity_food_finished_spiced_wine_amphora`
- `antiquity_food_aging_spiced_beer_amphora`
- `antiquity_food_finished_spiced_beer_amphora`
- `antiquity_food_aging_spiced_kumis_amphora`
- `antiquity_food_finished_spiced_kumis_amphora`

The food tool stable references are:

- `antiquity_food_butchers_knife`
- `antiquity_food_cooking_knife`
- `antiquity_food_threshing_flail`
- `antiquity_food_winnowing_basket`
- `antiquity_food_pitchfork`
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

`LiquidProduct` remains the craft-product type for non-fermenting filled liquid containers such as oils, meat broth, and strained milk. Fermented beverages, garum sauce, and aged spiced beverages instead produce sealed fermenting or aging amphora item prototypes; those prototypes morph into finished filled amphorae after their configured fermentation or steeping timer.

## Deferred Source Systems

This pass closes current ItemSeeder craftability gaps and now has seeded source paths for apiculture, seed stock, pastoral secondary products, and common agricultural derivatives. Agriculture apiary operations can produce raw honeycomb, pressed honey, and rendered beeswax, while herd operations can produce milk, wool, eggs, and manure from cattle, sheep/goat, horse, pig, and poultry herd definitions. Antiquity crafts build the hives, stands, smoke pots, honey knives, presses, strainers, and agricultural processing steps that support those chains.

It still does not add new primary-production systems for mining and quarrying, marine shellfish harvesting for murex, or every possible regional crop specialization. Current recipes use existing core liquids, materials, agricultural commodities, herd secondary outputs, apiary commodities, butchery outputs, and household pottery stock.
