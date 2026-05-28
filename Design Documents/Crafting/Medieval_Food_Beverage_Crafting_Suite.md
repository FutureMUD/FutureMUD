# Medieval Food Beverage Crafting Suite

The medieval food and beverage suite gives each culture a fuller foodway shelf: everyday staples, vessels, preserved provisions, feasting goods, and market rations. It also adds common dairy, brewing, baking, preserving, and measurement support stock.

This is still a lightweight foodway overlay rather than a full cuisine or nutrition system. Items are serving vessels, market stock, storage pieces, and builder cues that can support taverns, halls, monasteries, markets, camps, ships, manors, and households.

## Culture Foodway Stock

For every culture key, the seeder creates `medieval_food_{culture}_{foodway_item}` entries:

| Token | Use |
| --- | --- |
| `meal_platter` | A broad prepared-food cue for the culture's cuisine. |
| `staple_bread` | Bread, cakes, rice cakes, noodles, flatbread, or other staple-serving surface. |
| `pottage_bowl` | Everyday cooked food: stew, porridge, broth, sauce, or similar. |
| `preserved_provision` | Dried, smoked, salted, pickled, wrapped, or travel-ready food stock. |
| `drinking_vessel` | Cup, small jug, or vessel for the culture's ordinary drink cues. |
| `feast_dish` | Higher-status hall, court, monastic, or festival presentation. |
| `market_ration` | Portable ration for workers, soldiers, travellers, and vendors. |

Examples:

- Early Anglo-Saxon/Insular: oat cakes, ale, and pottage.
- Norse: flatbread, stockfish, and sour milk.
- Norman/Angevin: wheaten bread, wine, and stewed meat.
- al-Andalus/Maghreb: flatbread, oil, dates, and spiced stew.
- Song China: rice, wheat noodles, tea, and pickled greens.

## Common Food Production Stock

The suite also seeds common production and serving support:

| Stable Reference | Use |
| --- | --- |
| `medieval_food_cheese_mould` | Dairy and cheese-making scenes. |
| `medieval_food_butter_churn` | Household, farm, and monastic dairy work. |
| `medieval_food_ale_cask` | Ale, small beer, tavern, hall, or monastery brewing. |
| `medieval_food_cider_cask` | Orchard and fruit-drink storage. |
| `medieval_food_mead_crock` | Mead, syrups, vinegar, or small fermentation vessels. |
| `medieval_food_bakers_peel` | Hearth baking and oven-work scenes. |
| `medieval_food_bakers_tray` | Carrying loaves, pies, trenchers, or market food. |
| `medieval_food_salt_box` | Salt, preserving, kitchen, ship, or infirmary stock. |
| `medieval_food_spice_box` | Costly spices, herbs, saffron, and apothecary overlap. |
| `medieval_food_brewing_tub` | Mash, soaking, washing, brewing, and large kitchen prep. |

## Measurement Surfaces

| Stable Reference | Component Use |
| --- | --- |
| `medieval_food_grain_measure_sack` | Container/prop for measured grain stock. |
| `medieval_food_wine_measure_jug` | Uses `MeasuringInstrument_Antiquity_WineCup`. |
| `medieval_food_oil_measure_jug` | Uses `MeasuringInstrument_Antiquity_OilCup`. |
| `medieval_trade_grain_measure` | Uses `MeasuringInstrument_Antiquity_GrainMeasure`. |

The `WineCup`, `OilCup`, and `GrainMeasure` prototypes are live weight/fluid-volume or dry-measure stock components from UsefulSeeder. Length/surveying measurement remains out of scope.

## Craft Inputs And Tools

Culture foodway crafts use `Medieval Foodway Pattern {culture}` with `Cooking`, `Pottery`, `Tailoring`, or `Carpentry` depending on the surface. Generic food-production stock uses `Medieval Workshop Practice`.

Common inputs and tools include:

- `Cheese Curd Stock` and `Cheese Wheel Stock` for dairy shelves, cheese presses, monastic dairies, and market scenes. `Cheese Wheel Stock` is reusable food stock until prepared-food nutrition is added.
- `Brewing Mash Stock`, `Ale Stock`, `Cider Stock`, and `Mead Stock` for brewery, tavern, hall, orchard, and monastery storage.
- `Coopered Staves` and `Hoop Stock` for butter churns, ale/cider casks, brewing tubs, and other coopering-heavy containers.
- `Glaze Slurry Stock` for marked wine/oil measure jugs and other glazed ceramic food vessels.
- `Furniture Panel Stock`, `Furniture Timber Stock`, and willow/oak stock for trays, moulds, tubs, and casks.
- `Pottery Clay Body` and `Hot Fire` for bowls, cups, crocks, and measure jugs.
- `Garment Cloth`, `Spun Yarn`, `Sewing Needle`, and `Shears` for sacks, rations, and wrapped provisions.
- `Rotary Quern`, `Cheese Press`, `Lauter Tun`, `Grain Sieve`, `Croze`, and `Glazing Basin` as backed TagTools for staple, dairy, brewing, milling, coopering, and glazed-vessel workflows.

## Builder Workflows

Use the culture foodway stock to place food cues in inns, halls, camps, monasteries, households, markets, and ships without needing a bespoke prepared-food chain for every setting. Use the common production stock for kitchens, breweries, dairies, granaries, bakehouses, and customs spaces.

Future depth passes can add real prepared-food definitions, nutrition, spoilage, cheese aging, brewery simulation, fasting calendars, dietary law enforcement, and crop-to-kitchen pipelines. The current seeder represents dairy, milling, brewing, glazed measure-jugs, and cask production at item/commodity-stock depth.
