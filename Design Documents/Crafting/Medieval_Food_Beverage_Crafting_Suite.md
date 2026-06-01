# Medieval Food and Beverage Crafting Suite

The medieval food suite must distinguish between:

1. **Food vessels and tableware**: platters, bowls, cups, trenchers, jars, crocks, casks.
2. **Actual prepared foods and beverages**: breads, pottages, stews, preserved foods, dairy, fermented drinks, tea, sweets, feast dishes, and market rations.

The first merged implementation overused tableware and generic cue props. The second pass must add actual foodway items and crafts.

## Design Principle

Foodway culture should be playable. A builder should be able to stock an inn, monastery, market stall, feast table, military camp, ship, caravan, pastoral household, or scholar’s room with recognisable food and drink from the culture.

## Minimum Culture Foodway Surface

Each culture should receive at least eight explicit food/beverage items:

| Slot | Description |
| --- | --- |
| Staple bread or grain dish | Bread, flatbread, bannock, rice bowl, noodle bowl, millet gruel, rye loaf, etc. |
| Everyday cooked dish | Pottage, stew, pulse dish, fish stew, mushroom stew, pilaf, etc. |
| Preserved/travel food | Stockfish, smoked meat, salted fish, dried curds, market ration, shipboard ration. |
| Elite/feast dish | Feast roast, spiced meat, honey pastry, fish platter, noble stew. |
| Beverage | Ale, wine, mead, cider, sour milk, kumis, tea, syrup drink. |
| Dairy/oil/spice/condiment item | Cheese, curd, yogurt, oil relish, fish sauce, pickle, spice box use, date sweet. |
| Tableware/vessel | Cup, trencher, crock, cask, skin, bowl, jar. |
| Market/ration item | Portable or purchasable bundle. |

Tableware counts only for the vessel slot. It does not count as prepared food.

## Culture Foodway Examples

Exact stable references are listed in `Medieval_Culture_Catalogue.md`. Examples:

| Culture | Foodway Direction |
| --- | --- |
| Early Anglo-Saxon | oat flatbread, barley pottage, smoked eel/fish, ale, curd cheese, honeyed oat cake |
| Norse | stockfish, sour milk, rye flatbread, dried meat, curd bowl, ale horn |
| Norman/British/Capetian | wheaten bread, pottage, cheese trenchers, ale/cider/wine, meat pies, salted beef/fish |
| Gaelic/Welsh/Highland | oat bannocks, curds, smoked meat, ale, honey cakes, fish stew |
| Andalusi/Fatimid/Abbasid/Seljuk | flatbread, lentils, dates, yogurt, pilaf, spiced meat, oil relish, syrup drinks |
| Byzantine | bread, olives, wine, fish sauce relish, cheese, fish platters, monastery rations |
| Rus/Novgorod | rye bread, fish and mushroom stew, honey drink, smoked fish, curds, pickles |
| Steppe Turkic | millet gruel, dried curds, kumis, dried meat, riding rations, mare’s milk vessels |
| Song China | rice, wheat noodles, tea, pickled greens, steamed buns, scholar snack boxes |

## Craft Input Rules

Bread, pottage, stew, feast, ration, and beverage crafts must consume food commodities or liquids. They must not consume only `Furniture Panel Stock`.

Acceptable food inputs include:

- `Flour Commodity`
- grain commodity
- pulse commodity
- prepared meat or fish stock
- smoked/salted/dried meat stock
- `Cheese Curd Stock`
- `Cheese Wheel Stock`
- `Ale Stock`
- `Cider Stock`
- `Mead Stock`
- milk
- tea-compatible liquid or herb stock where available
- wine or wine-style liquids where available
- oil
- honey
- salt
- spice
- fruit
- vegetables
- broth or pottage base stock

`Furniture Panel Stock` is appropriate only for:

- wooden trenchers
- platters
- boards
- tableware
- food service furniture

## Common Production Chains

The existing medieval production chains should remain and expand as needed:

| Chain | Stock |
| --- | --- |
| Milling | `Flour Commodity` |
| Dairy | `Cheese Curd Stock`, `Cheese Wheel Stock` |
| Brewing | `Brewing Mash Stock`, `Ale Stock`, `Cider Stock`, `Mead Stock` |
| Coopering | `Coopered Staves`, `Hoop Stock` |
| Vessels | `Pottery Clay Body`, `Glaze Slurry Stock`, tableware/vessel final items |
| Sealing/preservation | sealing wax and cord for sealed crocks, ration packets, trade food bales if appropriate |

## Component Expectations

Where runtime prepared-food components exist and are appropriate, use them. If the current engine stock is insufficient for a specific prepared food, seed the item as a food prop, but:

- Tag it under food and drink.
- Note the limitation in builder notes.
- Do not pretend the prop is a full nutrition/runtime food item.
- Keep the craft input materially plausible.

## Stable Reference Rules

Use exact culture references such as:

```text
medieval_food_norse_stockfish_packet
medieval_food_song_china_tea_cup
medieval_food_rus_novgorod_mushroom_fish_stew
medieval_food_andalusi_lentil_stew
```

Avoid generic references such as:

```text
medieval_food_{culture}_meal_platter
medieval_food_{culture}_staple_bread
```

unless these are retained as clearly generic baseline items.

## Test Requirements

Add tests that verify:

- Each culture has at least 8 explicit food/beverage references.
- At least 6 of those are actual food/beverage concepts, not tableware.
- Food crafts for bread, pottage, stew, feast, ration, and beverage items consume food commodities or liquids.
- `Furniture Panel Stock` is not the primary input for prepared food crafts.
- Exact foodway stable references appear in `Medieval_Culture_Catalogue.md`.
