# Medieval Crafting Catalogue Audit

This document records the source-backed audit boundary for the first medieval item and craft slice. It follows the antiquity convention: shared production chains and tool prerequisites stay culture-neutral, while final silhouettes are controlled by knowledge gates, stable references, tags, and builder notes.

The medieval option is expected to be one of the most-used installer paths, so v1 aims to feel like a practical world-building starter catalogue rather than a narrow demonstration set.

## Source Boundary

Current medieval item definitions are seeded from:

| Source | Item Surface |
| --- | --- |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.Medieval.cs` | Shared `historic_*` foundations, 18 medieval culture slices, status-role clothing, military goods, household tools, writing/administration, measurement, medical/apothecary, jewellery/devotional, food/beverage, furniture/container, repair kits, and component-gap props. |
| `DatabaseSeeder/Seeders/ItemSeeder.Rework.cs` | Dispatcher wiring, builder-note culture/status inference, stable reference metadata, and cross-era historic foundation seeding when either `antiquity` or `medieval` is selected. |

Current medieval craft definitions are seeded from:

| Source | Craft Surface |
| --- | --- |
| `ItemSeederCrafting.Medieval.cs` | Historic foundations, medieval production-chain tools and intermediate stocks, clothing, equipment, writing/administration, medical/apothecary, jewellery/devotional, furniture/container, food/beverage, repair kits, and component-gap props. |
| `ItemSeederCrafting.cs` | Dispatcher registration through `SeedHistoricFoundationCrafts()`, `SeedMedievalProductionChainCrafts()`, and the medieval suite methods. |
| `ItemSeederCrafting.Antiquity.cs` | Shared stable-reference lookup helpers used by both antiquity and medieval craft products. |

## Culture Slices

The v1 slice uses 18 culture/time profiles:

| Key | Slice |
| --- | --- |
| `early_anglo_saxon` | Early Anglo-Saxon/Insular |
| `anglo_danish` | Late Anglo-Saxon/Anglo-Danish |
| `norse` | Norse |
| `norman` | Norman/Angevin |
| `high_british` | High Medieval Britain/Marcher |
| `gaelic` | Gaelic/Welsh/Highland |
| `carolingian` | Carolingian/Frankish |
| `capetian` | Capetian/Low Countries |
| `german_hre` | German/HRE/Alpine-North Italian |
| `iberian_christian` | Iberian Christian |
| `andalusi` | al-Andalus/Maghreb |
| `byzantine` | Byzantine |
| `abbasid` | Abbasid/Persianate |
| `fatimid` | Fatimid Egypt/Ifriqiya |
| `seljuk_ayyubid` | Seljuk/Ayyubid/early Mamluk |
| `rus_novgorod` | Kyivan Rus/Novgorod |
| `steppe_turkic` | Steppe Turkic/Cuman/Mongol-adjacent |
| `song_china` | Song China |

## Status Roles

Clothing uses status/role as a first-class axis:

| Key | Status/Role | Stable Reference Pattern |
| --- | --- | --- |
| `peasant` | Peasant | `medieval_clothing_{culture}_peasant_{piece}` |
| `artisan` | Artisan | `medieval_clothing_{culture}_artisan_{piece}` |
| `merchant` | Merchant/Burgher | `medieval_clothing_{culture}_merchant_{piece}` |
| `noble` | Noble/Court | `medieval_clothing_{culture}_noble_{piece}` |
| `clergy` | Clergy/Monastic | `medieval_clothing_{culture}_clergy_{piece}` |
| `military` | Military | `medieval_clothing_{culture}_military_{piece}` |

Each status/culture pair includes bodywear plus underlayers, hats/caps/coifs, hoods/cowls, cloaks or mantles, leggings/hose/chausses, gloves or mittens, socks or footwraps, shoes/boots/sandals, belts/girdles, and worn pouches.

## Catalogue Scale

| Surface | Current V1 Shape |
| --- | --- |
| Clothing | 18 cultures x 6 status roles x bodywear plus 10 wardrobe slots. |
| Equipment | 9 military pieces per culture: armour, helmet, coif, weapon, shield, sidearm harness, quiver, pack, banner, plus common crossbow and bolt stock. |
| Food and beverage | 7 foodway pieces per culture plus common dairy, brewing, baking, preserving, and measure stock. |
| Writing and administration | 4 office/record pieces per culture plus common writing, sealing, ledger, trade, and measurement stock. |
| Jewellery/devotional | 1 culture devotional token per culture plus common beads, badges, brooches, rings, reliquaries, pendants, and circlets. |
| Medical/apothecary | Bandages, poultices, apothecary vessels, herb storage, surgical props, mobility aids, and complete kits. |
| Furniture/container | Hall, household, shop, archive, storage, lighting, bedding, seating, security, and market-stall stock. |
| Shared foundations and medieval production tools | Hearths, kilns, looms, drop spindles, needles, shears, awls, dye vats, tanning racks, querns, lamps, anvils, tongs, hammers, bellows, fulling stocks, embroidery frames, tablet-weaving cards, lasts, drawplates, armourer's tools, bowyer tools, papermaking tools, cheese presses, brewing tuns, glassworking tools, and glazing/tile tools. |

## Production Chain Depth

`SeedMedievalProductionChainCrafts()` adds the process changes that distinguish the medieval slice from antiquity. It makes the medieval tools themselves craftable, then creates reusable commodity stock that finished recipes consume or that docs explicitly treat as reusable world-building stock.

| Chain | Intermediate Stock |
| --- | --- |
| Base workshop stock | `Spun Yarn`, `Garment Cloth`, `Fulled Cloth`, `Prepared Leather Panel`, `Leather Strap`, `Furniture Timber Stock`, `Furniture Panel Stock`, `Pottery Clay Body`, `Glass Batch`, `Glass Vessel Blank`, `Tool Blank Stock`, `Weapon Shaft Stock`, `Weapon Blade Stock`, `Weapon Head Stock`, `Fletching Stock`, `Military Cord Stock`, `Shield Board Stock`, `Shield Facing Stock`, `Armour Lamella Stock`, and `Flour Commodity`. These make a medieval-only install self-contained instead of depending on antiquity stock producers. |
| Textile finishing and tailoring | `Broadcloth Stock`, `Embroidered Trim Stock`, `Tablet-Woven Band Stock`, `Quilted Armour Padding`, `Silk Brocade Panel`. These drive luxury textile finishes, status clothing, military padding, tablet-woven edging, and court cloth. |
| Leatherworking and bookbinding | `Turnshoe Upper Stock`, `Scabbard Leather Stock`, `Bookbinding Leather Stock`. These distinguish footwear, weapon harnesses, and codices from generic leather-panel work. |
| Armour and crossbow manufacture | `Mail Wire Stock`, `Armour Ring Stock`, `Mail Panel Stock`, `Crossbow Prod Stock`, `Crossbow Tiller Stock`, `Crossbow Lockwork Stock`. Mail armour now flows through wire/ring/panel stages, while crossbow manufacture uses prod, tiller, lockwork, and bowyer tools. |
| Paper and parchment administration | `Paper Pulp Stock`, `Paper Sheet Stock`, `Parchment Sheet Stock`, `Seal Cord Stock`, `Sealing Wax Stock`. These support paper and parchment documents, books, sealed packets, sealable bales, and office kits. |
| Dairy, brewing, and milling | `Cheese Curd Stock`, `Cheese Wheel Stock`, `Brewing Mash Stock`, `Ale Stock`, `Cider Stock`, `Mead Stock`, plus `Coopered Staves` and `Hoop Stock` for casks and tubs. `Cheese Wheel Stock` is intentionally reusable food stock for dairies, kitchens, and market shelves even before prepared-food nutrition is added. |
| Ceramics, tile, and stained glass | `Glaze Slurry Stock`, `Tile Blank Stock`, `Stained Glass Quarry Stock`, `Lead Came Stock`, `Stained Glass Panel Stock`, `Lantern Pane Stock`. These support glazed measures, roof tiles, lanterns, chapel panels, and wealthy interiors. |
| Guild weights and measures | `Standard Weight Blank`, `Sealable Bale Wrapper Stock`, `Tally Stick Stock`, `Lockwork Stock`. These feed scales, weights, tax/customs kits, sealed bales, tallies, chests, lockplates, and strongboxes. |

## Verified Invariants

- `SealStamp`, `Sealable`, weight/fluid-volume/dry-measure `MeasuringInstrument`, and `OfferingReceiver` are treated as implemented stock components.
- Medieval signets, office seals, notary kits, guild stamps, sealed charters, sealed envelopes, sealed bales, sealed chests, ledger chests, document satchels, balance scales, weights, grain measures, oil measures, wine measures, tax/customs kits, and the devotional offering basin use live stock component prototypes.
- Length/surveying measurement, musical instruments, rules-aware game sets, and animal tack/harness remain prop-only component gaps.
- Shared `historic_*` foundations are culture-neutral and seeded when either antiquity or medieval is selected.
- Visible craft names and phase echoes avoid culture names. Culture-specific access lives in knowledge gates such as `Medieval Clothing Pattern {culture}` and builder metadata.
- `TagTool` leaves used by medieval crafts are backed by seeded historic or medieval prototypes, including spinning, sewing, shearing, awl, fire, metalworking, fulling, embroidery, tablet weaving, shoe-last, bookbinding, drawplate, armourer, bowyer, papermaking, dairy, brewing, milling, glazing, glassworking, and measurement-support tools.
- Reusable commodity tags introduced by the medieval pass are consumed by final crafts or documented as reusable production stock.
- Lit historic items have morph targets and timers back to their unlit forms.

## Stable Reference Families

| Surface | Stable Reference Family |
| --- | --- |
| Historic foundations | `historic_workshop_hearth`, `historic_lit_workshop_hearth`, `historic_updraft_kiln`, `historic_lit_updraft_kiln`, `historic_warp_weighted_loom`, `historic_treadle_loom`, `historic_drop_spindle`, `historic_sewing_needle`, `historic_textile_shears`, `historic_awl_punch`, `historic_dye_vat`, `historic_tanning_rack`, `historic_hand_quern`, `historic_oil_lamp`, `historic_lit_oil_lamp`, `historic_workshop_anvil`, `historic_forge_tongs`, `historic_workshop_hammer`, `historic_bellows` |
| Clothing | `medieval_clothing_{culture}_{status}_{piece}` |
| Military | `medieval_military_{culture}_{equipment_piece}`, `medieval_weapon_{culture}_{weapon}`, `medieval_weapon_common_crossbow`, `medieval_weapon_common_crossbow_bolts`, `medieval_shield_{culture}` |
| Household tools | `medieval_coopers_croze`, `medieval_iron_wood_plane`, `medieval_bookbinder_press`, `medieval_locksmith_file_set`, `medieval_household_{production_tool}` |
| Food and beverage | `medieval_food_{culture}_{foodway_item}`, `medieval_food_grain_measure_sack`, `medieval_food_wine_measure_jug`, `medieval_food_oil_measure_jug`, `medieval_food_cheese_mould`, `medieval_food_butter_churn`, `medieval_food_ale_cask`, `medieval_food_cider_cask`, `medieval_food_mead_crock`, `medieval_food_bakers_peel`, `medieval_food_bakers_tray`, `medieval_food_salt_box`, `medieval_food_spice_box`, `medieval_food_brewing_tub` |
| Furniture and containers | `medieval_household_{furniture_or_container}`, including `medieval_household_stained_glass_panel` and `medieval_household_roof_tile_stack` |
| Jewellery/devotional | `medieval_devotional_{culture}_pilgrim_token`, `medieval_devotional_{devotional_item}`, `medieval_jewellery_{jewellery_item}`, `medieval_offering_basin` |
| Medical/apothecary | `medieval_medical_{medical_item}` |
| Writing/administration | `medieval_writing_{culture}_{administration_item}`, `medieval_writing_{writing_item}` |
| Trade and measures | `medieval_trade_balance_scale`, `medieval_trade_standard_weight_set`, `medieval_trade_false_weight_set`, `medieval_trade_grain_measure`, `medieval_trade_tax_customs_kit`, `medieval_trade_sealable_bale`, `medieval_surveyor_measuring_rope` |
| Repair kits | `medieval_textile_repair_kit`, `medieval_leather_repair_kit`, `medieval_metal_repair_kit` |
| Component-gap props | `medieval_music_psaltery`, `medieval_game_chess_set`, `medieval_horse_tack_display_set` |

## Deferred Scope

The v1 medieval slice does not attempt full plate armour, hand firearms, spectacles, mechanical clocks, printing-press workflows, real prepared-food nutrition, brewery simulation, procedural surgery, cranequin/windlass/goat's-foot loading tools, rules-aware instruments, rules-aware game sets, or animal tack systems. Those belong in later late-medieval, renaissance, or subsystem-specific passes.
