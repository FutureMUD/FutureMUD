# Medieval Food, Beverage, and Preservation Foundation

**Status:** implemented current source of truth.
**Date:** 30 July 2026.
**Era:** Medieval.
**Item source:** `DatabaseSeeder/Seeders/ItemSeeder.MedievalFoodProduction.cs`.
**Craft source:** `DatabaseSeeder/Seeders/ItemSeeder.Crafting.MedievalFood.cs`.
**Shared source:** `DatabaseSeeder/Seeders/ItemSeeder.PreIndustrialFoodFoundation.cs`.

This slice activates `SeedMedievalFoodBeverageCrafts` as the second Medieval craft launcher. Together with `SeedMedievalProductionChainCrafts`, it supplies the first complete dependency path from shared agricultural, butchery, primary-production, and workshop foundations to Medieval staple food, preserved provisions, ale, wine, and cooking oil.

## Runtime catalogue

The launcher seeds exactly 48 phase-ordered crafts under one `Medieval Food Production` knowledge:

| Phase | Craft count | Coverage |
|---:|---:|---|
| 0 | external only | Agriculture crops, Animal Butchery cuts, Primary Production salt, existing water/fuel/liquids, Historic tools, Medieval production stock, and existing vessels |
| 1 | 17 | Six new food-production items plus eleven existing Medieval food and brewing tools or apparatus |
| 2 | 18 | Grain cleaning and milling, malting, dough, oilseed and fruit pressing, wort, meat/fish breakdown, and salt/dry/smoke preservation |
| 3 | 13 | Eleven prepared foods plus filled amber-ale and red-wine casks |
| **Total** | **48** | — |

The seven knowledge subtypes are `Food Tools`, `Grain Processing`, `Baking and Pottage`, `Oil and Fruit Pressing`, `Meat Preservation`, `Fish Preservation`, and `Brewing and Winemaking`.

All crafts use `MinorFail`, five free checks, failure phase 3, three execution phases, and non-interruptible execution. Routine work uses skill 10/Easy, material conversion and cooking generally use skill 15/Normal, brewing uses skill 20/Normal or Hard, and major apparatus uses skill 25/Hard.

## New item prototypes

The foundation adds six tool or apparatus prototypes:

- `medieval_tool_butchers_knife`
- `medieval_tool_cooking_knife`
- `medieval_tool_threshing_flail`
- `medieval_tool_winnowing_basket`
- `medieval_tool_cooking_pot`
- `medieval_workshop_lauter_tun`

The eleven prepared-food prototypes are:

- `medieval_food_coarse_bread_loaf`
- `medieval_food_flatbread`
- `medieval_food_hard_bread`
- `medieval_food_grain_pottage`
- `medieval_food_meat_pottage`
- `medieval_food_salted_meat_ration`
- `medieval_food_dried_meat_ration`
- `medieval_food_smoked_meat_ration`
- `medieval_food_salted_fish_ration`
- `medieval_food_dried_fish_ration`
- `medieval_food_smoked_fish_ration`

The food items reuse four idempotently seeded `PreparedFood` component prototypes:

| Component | Freshness window |
|---|---|
| `PreparedFood_Medieval_Bread` | stale after 3 days; spoiled after 8 days |
| `PreparedFood_Medieval_HardBread` | stale after 30 days; spoiled after 120 days |
| `PreparedFood_Medieval_Pottage` | stale after 2 days; spoiled after 5 days |
| `PreparedFood_Medieval_PreservedProvision` | stale after 14 days; spoiled after 60 days |

`CookedFoodProduct` transfers configured ingredient roles into every prepared-food output.

## Shared commodity and butchery contracts

Nineteen generic stock tags are owned by `Materials / Food Products / Pre-Industrial Food Commodities`:

`Grain Cleaning Stock`, `Cleaned Grain Commodity`, `Flour Commodity`, `Meal Commodity`, `Bran Commodity`, `Malted Grain Commodity`, `Dough Commodity`, `Oilseed Mash Commodity`, `Oilseed Cake Commodity`, `Fruit Must Commodity`, `Wort Commodity`, `Raw Meat Commodity`, `Salted Meat Commodity`, `Dried Meat Commodity`, `Smoked Meat Commodity`, `Raw Fish Commodity`, `Salted Fish Commodity`, `Dried Fish Commodity`, and `Smoked Fish Commodity`.

Existing tag rows are reparented by name so their IDs survive upgrades. Antiquity consumers resolve these shared paths while Antiquity-only commodities remain under `Antiquity Food Commodities`; no duplicate leaf tags are created.

Animal Butchery retains `Raw Meat Cut` as the compatibility parent and adds `Raw Non-Fish Meat Cut` and `Raw Fish Cut`. Fish, shark, crustacean, and cephalopod stock outputs receive the fish child classification; the remaining ordinary raw cuts receive the non-fish classification.

## Tools, apparatus, and liquids

The existing bake oven, brew copper, mash tun, fermenting gyle tun, flour sieve, kneading trough, salting trough, smoking rack, oil press, fruit press, and mashing paddle receive active phase-1 crafts. The smoking rack supplies both `Smoking Rack` and the canonical cooking `Drying Rack` function.

Finished liquids use `LiquidProduct` directly:

- one litre of existing `vegetable oil` in `medieval_tableware_oil_amphora`;
- 3.5 litres of existing `amber ale` in `medieval_tableware_table_beer_cask`;
- 3.5 litres of existing `red wine` in `medieval_tableware_small_wine_cask`.

These volumes fit the existing amphora and gallon-cask liquid-container profiles. The slice introduces no new liquid, material, component type, database migration, or fermentation morph.

## Dependency and repeatability contracts

Every exact Medieval dependency comes from an earlier phase or from the already-completed `Medieval Industry Foundations` phase-0 contract. Reusable tool dependencies resolve through functional tags with explicit Historic Foundation, Primary Production, Medieval Industry Foundations, or Medieval Food Production ownership.

The craft specifications expose a read-only test projection containing phase, category, trait, minimum skill, knowledge subtype, inputs, tools, products, difficulty, dependencies, and source ownership. Tests verify the exact 48 outputs, phase monotonicity, source classification, vessel capacity, prepared-food ingredient transfer, direct liquid products, and double-seed idempotency.

## Maintained data and deferrals

`Seeded_Item_Components.json` and `SeededTagHierarchy.csv` change with the new live component and tag sources. `Seeded_Liquids.json` and `Seeded_Materials.json` were audited and remain unchanged because all liquid and material references already exist.

Regional dishes, feasts, sweets, dairy processing, cheese, butter, mead, vinegar, dried fruit, specialist condiments, and culture-specific beverages remain outside this generic subsistence foundation. Civilian transport and tack is the next cross-manifest gap, followed by games and toys. The other eight Medieval craft launchers remain explicit no-ops.
