# Medieval Furniture, Container, Household, and Devotional Crafting Suite

The medieval household suite should support both common medieval rooms and culture-specific environments.

The current scaffold contains useful common furniture, containers, lighting, chests, shelves, counters, beds, strongboxes, market stalls, stained glass, and roof tiles. The second pass should keep those common objects while adding explicit culture-specific household, devotional, and luxury goods.

## Design Principle

A furnished room should have visible cultural identity even when no NPCs are present. Clothing should not be the only way to distinguish a Norse hall, Song study, Byzantine chapel, Andalusi house, Rus merchant room, or High Medieval guildhall.

## Common Household Surface To Retain

The common medieval furniture/container surface remains useful:

- Trestle tables
- Boarded chests
- Lockable strongboxes
- Aumbry cupboards
- Benches and stools
- Chairs
- Rope beds and mattresses
- Blanket chests
- Shelves and book shelves
- Market counters and stalls
- Writing desks and lecterns
- Storage barrels, sacks, baskets
- Lanterns, braziers, candle stands
- Stained glass panels and roof tiles
- Door bars, lockplates, keyrings

These should be labelled as common medieval stock, not explicit culture content.

## Minimum Culture Household Surface

Each culture should receive at least five explicit non-clothing, non-military household/devotional/luxury items.

Each culture should include at least two of:

| Category | Examples |
| --- | --- |
| Household storage/furniture | sea chests, hall chests, book stands, writing boxes, guild counters |
| Lighting | hanging lamps, brass lamps, wall sconces, glass lamps |
| Devotional/religious object | icons, reliquaries, shrine cloths, prayer beads, crosses, censers |
| Tableware/luxury vessel | glazed bowls, porcelain bowls, drinking horns, oil flasks, tea cups |
| Trade or craft prop | weight pouches, account boxes, cloth bales, notice boards |
| Textile or wall furnishing | felt carpets, tent panels, altar cloths, screen panels |

## Culture Directions

Exact stable references are listed in `Medieval_Culture_Catalogue.md`.

| Culture | Household/Devotional Direction |
| --- | --- |
| Anglo-Saxon / Anglo-Danish | carved hall chests, hanging lamps, monastic stands, reliquaries, ring-pins |
| Norse | sea chests, carved comb cases, drinking horns, shipboard boxes, trade weights |
| Norman / British / Capetian | manor chests, chapel lecterns, shield racks, guild counters, parish alms boxes |
| Gaelic / Welsh | shrine cloths, harp/lyre props, hide travel bags, pastoral milk vessels, oath sticks |
| Andalusi / Abbasid / Fatimid / Seljuk | glazed bowls, brass lamps, writing boxes, carved screens, perfume/oil flasks, prayer rugs |
| Byzantine | icons, hanging lamps, enamel pendants, altar cloths, bronze censers |
| Rus / Novgorod | icon shelves, birchbark letter boxes, fur-lined chests, honey drink crocks, river-trade balance cases |
| Steppe Turkic | felt tent panels, saddlebags, kumis skins, bowcase racks, felt carpets |
| Song China | tea cups, lacquer writing boxes, porcelain bowls, scholar brush rests, printed notice boards |

## Craft Inputs

Use existing stock:

| Product Type | Suggested Inputs |
| --- | --- |
| wood furniture/storage | `Furniture Timber Stock`, `Furniture Panel Stock`, metal fittings or `Lockwork Stock` if locking |
| leather/felt bags | `Prepared Leather Panel`, `Leather Strap`, `Garment Cloth` or hair/fur stock |
| lamps/metal vessels | metal `Tool Blank Stock`, glass/pane stock, oil/wick stock where relevant |
| glazed bowls/tiles | `Pottery Clay Body`, `Glaze Slurry Stock`, lit kiln/hot fire |
| icons/devotional panels | wood panel stock, paint/pigment stock where available |
| censers/basins/metal devotional goods | bronze/silver `Tool Blank Stock`, hammer/anvil tools |
| porcelain/Song ceramics | ceramic/glaze stock; if porcelain material is absent, seed as fine ceramic/earthenware substitute and document limitation |

## Devotional and Jewellery Boundary

Small worn devotional items may remain in `Medieval_Jewellery_Devotional_Crafting_Suite.md`. Room, shrine, chapel, household, or furniture devotional goods belong here.

Examples:

- Worn pendant: jewellery/devotional.
- Reliquary box on a shelf: furniture/household/devotional.
- Icon pendant: jewellery/devotional.
- Icon panel: household/devotional.

## Test Requirements

Add tests that verify:

- Each culture has at least 5 explicit household/devotional/luxury references.
- Generic `regional devotional token` items do not count toward explicit culture household/devo requirements.
- Exact stable references appear in `Medieval_Culture_Catalogue.md`.
- Culture-specific room goods include vocabulary appropriate to the culture.
