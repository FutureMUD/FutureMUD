# Medieval Furniture Container Crafting Suite

The medieval furniture and container suite is meant to make the medieval install immediately usable for halls, taverns, homes, shops, courts, monasteries, workshops, schools, archives, and markets. It includes shared historic apparatus plus medieval household, storage, lighting, security, and work-surface stock.

## Shared Historic Foundations

`SeedHistoricCommonWorkshopItems()` creates cross-era apparatus when either antiquity or medieval is selected:

- `historic_workshop_hearth` and `historic_lit_workshop_hearth`.
- `historic_updraft_kiln` and `historic_lit_updraft_kiln`.
- `historic_warp_weighted_loom` and `historic_treadle_loom`.
- `historic_sewing_needle`, `historic_textile_shears`, and `historic_awl_punch`.
- `historic_dye_vat`, `historic_tanning_rack`, and `historic_hand_quern`.
- `historic_oil_lamp` and `historic_lit_oil_lamp`.
- `historic_workshop_anvil`, `historic_forge_tongs`, `historic_workshop_hammer`, and `historic_bellows`.

The lit historic items morph back to their unlit forms on timers.

## Workshop And Household Tools

Workshop and household tool stock includes:

- `medieval_coopers_croze`
- `medieval_iron_wood_plane`
- `medieval_bookbinder_press`
- `medieval_locksmith_file_set`
- `medieval_household_fulling_stocks`, `medieval_household_teasel_frame`, `medieval_household_napping_shears`, `medieval_household_cloth_tenter_frame`
- `medieval_household_embroidery_frame`, `medieval_household_tablet_weaving_cards`, `medieval_household_turnshoe_last`
- `medieval_household_bookbinder_sewing_frame`, `medieval_household_leather_paring_knife`, `medieval_household_drawplate`
- `medieval_household_armourers_anvil`, `medieval_household_planishing_hammer`, `medieval_household_mail_riveting_tongs`
- `medieval_household_bow_press`, `medieval_household_tillering_stick`, `medieval_household_crossbow_tiller_jig`
- `medieval_household_papermakers_mould`, `medieval_household_papermakers_vat`, `medieval_household_wax_spatula`
- `medieval_household_cheese_press`, `medieval_household_lauter_tun`, `medieval_household_millers_sieve`
- `medieval_household_glaziers_grozing_iron`, `medieval_household_glaziers_lead_knife`, `medieval_household_tile_mould`, `medieval_household_glazing_basin`, `medieval_household_lantern_pane_mould`

These tools support coopering, joinery, bookbinding, lock/security, fulling, luxury textile finishes, leather footwear, mail, crossbow manufacture, papermaking, dairy, brewing, milling, glazed pottery, stained glass, and lantern-pane scenes that are common in medieval play spaces.

## Furniture And Container Catalogue

| Family | Stable References |
| --- | --- |
| Work and dining surfaces | `medieval_household_trestle_table`, `medieval_household_market_counter`, `medieval_household_writing_desk`, `medieval_household_lectern`, `medieval_household_market_stall` |
| Seating and sleeping | `medieval_household_plank_bench`, `medieval_household_three_legged_stool`, `medieval_household_lordly_chair`, `medieval_household_rope_bedframe`, `medieval_household_straw_mattress` |
| Storage furniture | `medieval_household_boarded_chest`, `medieval_household_blanket_chest`, `medieval_household_aumbry_cupboard`, `medieval_household_book_shelves`, `medieval_household_wall_shelf` |
| Portable or bulk containers | `medieval_household_lockable_strongbox`, `medieval_household_storage_barrel`, `medieval_household_wicker_basket`, `medieval_household_canvas_sack` |
| Lighting and heating | `medieval_household_iron_lantern`, `medieval_household_charcoal_brazier`, `medieval_household_candle_stand` |
| Building and decorative stock | `medieval_household_stained_glass_panel`, `medieval_household_roof_tile_stack` |
| Security hardware props | `medieval_household_door_bar`, `medieval_household_iron_lockplate`, `medieval_household_keyring` |

## Implemented Components

| Surface | Component Notes |
| --- | --- |
| Tables, benches, counters, desks, beds, shelves, and lecterns | Use existing surface/container components such as `Container_Table`, `Container_Bench_Surface`, `Container_Counter`, `Container_Desk_Surface`, `Container_Bed_Surface`, and shelf components. |
| Chests, cupboards, sacks, baskets, and barrels | Use existing container and liquid-container components where appropriate. |
| Sealed storage | `medieval_household_boarded_chest` and `medieval_household_lockable_strongbox` use `Sealable_Container_Wax`; the strongbox also uses `LockingContainer_Lockbox`. |
| Lighting | `medieval_household_iron_lantern` uses the live `Lantern` component; historic oil lamps use lit/unlit morphing stock. |

## Craft Inputs And Tools

Furniture and container crafts use `Medieval Workshop Practice` with `Carpentry`, `Leathermaking`, `Tailoring`, `Pottery`, `Blacksmithing`, or `Candlemaking` as appropriate.

Common inputs:

- `Furniture Timber Stock` and `Furniture Panel Stock` for furniture, chests, desks, shelves, casks, counters, and stalls.
- `Prepared Leather Panel` for portable bags and document satchels in related suites.
- `Garment Cloth` and `Spun Yarn` for sacks and bedding.
- `Tool Blank Stock` for locks, lanterns, keyrings, candle stands, and metal fittings.
- `Lantern Pane Stock` for iron lanterns.
- `Stained Glass Quarry Stock`, `Lead Came Stock`, and `Stained Glass Panel Stock` for stained glass.
- `Tile Blank Stock` and `Glaze Slurry Stock` for roof tiles and glazed building stock.
- `Lockwork Stock` and `Sealing Wax Stock` for tamper-evident sealed storage and lockable strongboxes.

Required TagTools are backed by shared historic hammers, awls, shears, sewing needles, anvils, forge tongs, hot fires, and lamps, plus medieval glazier, tile, glazing, locksmith, and lantern-pane tools.

## Builder Workflows

Use this suite to furnish:

- Peasant cottages, farm buildings, byres, kitchens, and storerooms.
- Taverns, inns, guild halls, counting rooms, toll booths, and market stalls.
- Castles, great halls, courts, guardrooms, armouries, and noble chambers.
- Monasteries, schools, infirmaries, scriptoria, chapels, and archives.
- Ships, caravanserais, customs houses, bridges, docks, and warehouses.

Door bars, lockplates, and keyrings are visible security props rather than door runtime. They support scenes now and leave room for later richer door-hardware behaviour.
