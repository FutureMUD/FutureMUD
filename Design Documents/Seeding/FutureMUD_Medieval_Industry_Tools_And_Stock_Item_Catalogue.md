# FutureMUD Medieval Industry Tools and Intermediate Stock Item Catalogue

**Status:** implemented item-catalogue source for the first medieval industry prerequisite item pass.  
**Date:** 1 July 2026.  
**Era band:** approximately 500-1300 CE.  
**Implementation files:** `DatabaseSeeder/Seeders/ItemSeeder.MedievalHouseholdTools.cs` and `DatabaseSeeder/Seeders/ItemSeeder.MedievalComponentGaps.cs`.

This catalogue is the concrete item-prototype layer that sits between the merged medieval industry-chain design and the future medieval craft definitions. It creates the tools, workshop apparatus, and shared intermediate stock that later crafts can consume while recursively closing finished items back to butchery, forage, agricultural, or primary-industry products.

This pass does **not** implement medieval crafts. The craft authoring target remains `DatabaseSeeder/Seeders/ItemSeeder.Crafting.Medieval.cs`.

---

## Implementation summary

- `SeedMedievalHouseholdCraftTools` now seeds the first medieval industry tool and workshop-apparatus catalogue.
- `SeedMedievalComponentGapItems` now seeds the first medieval intermediate-stock catalogue.
- The first item pass creates **168 tool/workshop prototypes** and **50 intermediate stock prototypes**, for **218 total item prototypes**.
- Tool items use the shared `Tool_*_General` HandTool component profiles created by the merged prerequisite pass, plus exact functional tool tags where available.
- Intermediate stock rows are player-visible, portable craft stock. They are intentionally not finished consumer goods and are expected to become inputs or outputs of production-chain crafts in the next pass.
- The catalogue reuses the existing `historic_*` foundation tools where appropriate rather than cloning basic hearths, kilns, looms, spindles, needles, shears, awls, dye vats, tanning racks, querns, oil lamps, anvils, tongs, hammers, and bellows.

## Catalogue distribution

| Slice | Row count | Owning method |
|---|---:|---|
| Heat, furnace, glasshouse, brewing, paper, bookbinding, fulling, ropewalk, and other workshop apparatus | 19 | `SeedMedievalHouseholdCraftTools` |
| Hand tools and specialist portable tools | 149 | `SeedMedievalHouseholdCraftTools` |
| Wood, textile, leather, writing, metal, clay, glass, jewellery, medical, and repair stock | 50 | `SeedMedievalComponentGapItems` |
| **Total** | **218** | — |

---

## Tool and workshop coverage

| Production chain | Representative seeded references | Notes |
|---|---|---|
| Heat, smelting, forge, lime, and glasshouse work | `medieval_workshop_forge`, `medieval_workshop_lit_forge`, `medieval_workshop_smelting_furnace`, `medieval_workshop_lit_smelting_furnace`, `medieval_workshop_lime_kiln`, `medieval_workshop_glass_glory_hole`, `medieval_workshop_annealing_lehr` | Provides the medieval-specific high-heat apparatus not covered by the shared historic hearth and kiln. |
| Ore, furnace, crucible, and alkali preparation | `medieval_tool_ore_crusher`, `medieval_tool_ore_roaster`, `medieval_tool_charging_bucket`, `medieval_tool_slag_hammer`, `medieval_tool_slag_skimmer`, `medieval_tool_tap_rod`, `medieval_tool_crucible`, `medieval_tool_crucible_tongs`, `medieval_tool_lye_leaching_barrel` | Supports ore-to-bloom, small crucible, pigment, glass, and alkali preparation chains. |
| Forestry, carpentry, joinery, and coopering | `medieval_tool_felling_axe`, `medieval_tool_splitting_axe`, `medieval_tool_hand_saw`, `medieval_tool_bow_saw`, `medieval_tool_forest_saw`, `medieval_tool_fine_saw`, `medieval_tool_adze`, `medieval_tool_drawknife`, `medieval_tool_spokeshave`, `medieval_tool_wood_chisel`, `medieval_tool_wood_auger`, `medieval_tool_wood_file`, `medieval_tool_wood_clamp`, `medieval_tool_coopers_adze`, `medieval_tool_coopers_jointer`, `medieval_tool_croze`, `medieval_tool_hoop_driver`, `medieval_tool_bung_borer`, `medieval_workshop_pole_lathe` | Covers timber conversion, boards, handles, staves, barrels, furniture, boxes, doors, bow staves, shields, and book boards. |
| Textile, dyeing, fulling, basketry, and cordage | `medieval_tool_distaff`, `medieval_tool_wool_combs`, `medieval_tool_flax_hackle`, `medieval_tool_scutching_knife`, `medieval_tool_niddy_noddy`, `medieval_tool_warping_board`, `medieval_tool_shuttle`, `medieval_tool_beater_batten`, `medieval_tool_heddle_rod`, `medieval_tool_lease_rod`, `medieval_tool_weaving_reed`, `medieval_tool_tablet_weaving_cards`, `medieval_tool_dye_strainer`, `medieval_tool_mordant_cauldron`, `medieval_tool_dye_stirring_pole`, `medieval_tool_skein_rack`, `medieval_tool_dye_drying_line`, `medieval_tool_fullers_trough`, `medieval_tool_fullers_mallet`, `medieval_workshop_fulling_stocks`, `medieval_tool_tenter_frame`, `medieval_tool_teasel_frame`, `medieval_tool_napping_shears`, `medieval_tool_reed_splitter`, `medieval_tool_basket_knife`, `medieval_tool_weaving_bodkin`, `medieval_workshop_ropewalk`, `medieval_tool_rope_hook`, `medieval_tool_rope_top`, `medieval_tool_marlinespike` | Covers fibre preparation, spinning, weaving, band weaving, dyeing, fulling, finishing, basketry, and rope work. |
| Leather, parchment, bone, and animal-product processing | `medieval_tool_skinning_knife`, `medieval_tool_fleshing_knife`, `medieval_tool_hide_scraper`, `medieval_tool_tanning_beam`, `medieval_tool_dehairing_knife`, `medieval_tool_tanning_paddle`, `medieval_tool_currying_knife`, `medieval_tool_saddlers_clamp`, `medieval_tool_leather_creaser`, `medieval_tool_burnisher`, `medieval_tool_bone_saw`, `medieval_tool_bone_file`, `medieval_tool_parchment_stretching_frame`, `medieval_tool_parchment_lunellum`, `medieval_tool_parchment_pumice`, `medieval_tool_pounce_bag` | Supports hides, leather panels, straps, parchment sheets, bone blanks, horn blanks, bindings, covers, footwear, tack, and repair stock. |
| Smithing, weaponsmithing, armouring, locks, jewellery, and lapidary | `medieval_tool_smithing_punch_set`, `medieval_tool_drawplate`, `medieval_tool_grindstone`, `medieval_tool_whetstone`, `medieval_tool_quenching_trough`, `medieval_tool_fuller_tool`, `medieval_tool_tang_punch`, `medieval_tool_sword_vise`, `medieval_tool_crossguard_fixture`, `medieval_tool_armourers_stake`, `medieval_tool_ball_stake`, `medieval_tool_dishing_form`, `medieval_tool_forming_bag`, `medieval_tool_planishing_hammer`, `medieval_tool_raising_hammer`, `medieval_tool_plate_snips`, `medieval_tool_armourers_pliers`, `medieval_tool_locksmithing_fabrication_kit`, `medieval_tool_locksmithing_installation_kit`, `medieval_tool_jewellers_anvil`, `medieval_tool_jewellers_crimping_pliers`, `medieval_tool_jewellers_burnisher`, `medieval_tool_lapidary_saw`, `medieval_tool_lapidary_wheel`, `medieval_tool_drill_bow` | Supports forged stock, weapons, armour plates, mail, locks, keys, jewellery settings, wire, beads, cabochons, signets, and small metal fittings. |
| Pottery, masonry, glassblowing, paper, bookbinding, printing, food, and medicine | `medieval_tool_potters_wheel`, `medieval_tool_potters_rib`, `medieval_tool_clay_knife`, `medieval_tool_wire_cutter`, `medieval_tool_press_mold`, `medieval_tool_clay_stamp`, `medieval_tool_masons_hammer`, `medieval_tool_point_chisel`, `medieval_tool_tooth_chisel`, `medieval_tool_masons_trowel`, `medieval_tool_masons_line`, `medieval_tool_masons_square`, `medieval_tool_glass_blowpipe`, `medieval_tool_pontil_rod`, `medieval_tool_marver_table`, `medieval_tool_glass_jacks`, `medieval_tool_glass_shears`, `medieval_tool_glass_blocks`, `medieval_tool_mould_and_deckle`, `medieval_tool_couching_blanket`, `medieval_tool_press_felt`, `medieval_tool_lay_press`, `medieval_tool_rag_sorting_knife`, `medieval_tool_paper_sizing_brush`, `medieval_tool_bookbinders_needle`, `medieval_tool_bookbinders_punch`, `medieval_tool_backing_hammer`, `medieval_tool_leather_paring_knife`, `medieval_tool_qalam_cutter`, `medieval_tool_quill_curing_sand`, `medieval_tool_ruling_board`, `medieval_tool_manuscript_pricker`, `medieval_tool_block_carving_knife`, `medieval_tool_block_clearing_chisel`, `medieval_tool_printing_baren`, `medieval_tool_ink_dauber`, `medieval_tool_impression_spoon`, `medieval_tool_flour_sieve`, `medieval_tool_kneading_trough`, `medieval_tool_salting_trough`, `medieval_tool_smoking_rack`, `medieval_tool_oil_press`, `medieval_tool_fruit_press`, `medieval_tool_mashing_paddle`, `medieval_tool_mortar_and_pestle`, `medieval_tool_medicine_strainer`, `medieval_tool_ointment_spatula`, `medieval_tool_cupping_vessel`, `medieval_tool_cautery_iron`, `medieval_tool_suture_needle`, `medieval_tool_surgical_probe`, `medieval_tool_forceps` | Covers the high-volume craft families not already represented by the shared historic foundation tool pass. |

---

## Intermediate stock coverage

| Stock family | Seeded references | Notes |
|---|---|---|
| Wood and timber | `medieval_industry_stock_plank_bundle`, `medieval_industry_stock_timber_beam`, `medieval_industry_stock_shaft_blanks`, `medieval_industry_stock_bow_stave`, `medieval_industry_stock_shield_board`, `medieval_industry_stock_handle_blanks` | Shared stock for furniture, doors, containers, weapons, shields, bows, tools, and book boards. |
| Textile and cordage | `medieval_industry_stock_yarn_skein`, `medieval_industry_stock_sewing_thread`, `medieval_industry_stock_cord_bundle`, `medieval_industry_stock_rope_coil`, `medieval_industry_stock_plain_cloth_bolt`, `medieval_industry_stock_canvas_bolt`, `medieval_industry_stock_felt_sheet` | Shared stock for clothing, bags, padded armour, repair kits, ropes, bands, and soft household goods. |
| Leather, parchment, and book stock | `medieval_industry_stock_leather_panel`, `medieval_industry_stock_hardened_leather_panel`, `medieval_industry_stock_leather_strap_bundle`, `medieval_industry_stock_leather_thong_bundle`, `medieval_industry_stock_rawhide_cord_bundle`, `medieval_industry_stock_parchment_sheet`, `medieval_industry_stock_rag_paper_sheet`, `medieval_industry_stock_book_board_pair` | Bridges butchery and writing/bookmaking chains into finished leather goods, documents, codices, armour, and repairs. |
| Metal and military stock | `medieval_industry_stock_iron_bar`, `medieval_industry_stock_bronze_bar`, `medieval_industry_stock_wire_coil`, `medieval_industry_stock_rivet_packet`, `medieval_industry_stock_nail_packet`, `medieval_industry_stock_sheet_metal`, `medieval_industry_stock_blade_blank`, `medieval_industry_stock_weapon_head_blank`, `medieval_industry_stock_armour_ring_packet`, `medieval_industry_stock_armour_scale_bundle` | Provides common metal and armour subassemblies that finished weapons, armour, locks, containers, furniture, jewellery, and tools can consume. |
| Clay, building, and glass stock | `medieval_industry_stock_clay_body_lump`, `medieval_industry_stock_fired_brick_stack`, `medieval_industry_stock_roof_tile_stack`, `medieval_industry_stock_lime_mortar_lump`, `medieval_industry_stock_glass_batch`, `medieval_industry_stock_glass_pane_blank` | Supports ceramic, masonry, glasshouse, construction, lighting, and household vessel craft chains. |
| Writing, jewellery, and administrative stock | `medieval_industry_stock_ink_cake`, `medieval_industry_stock_glue_cake`, `medieval_industry_stock_seal_wax_stick`, `medieval_industry_stock_wax_tablet_blank`, `medieval_industry_stock_glass_bead_blanks`, `medieval_industry_stock_cabochon_blank`, `medieval_industry_stock_jewellery_setting_packet` | Provides reusable stock for manuscripts, seals, books, jewellery, beads, signets, and bindings. |
| Medical and repair stock | `medieval_industry_stock_bandage_roll`, `medieval_industry_stock_suture_thread`, `medieval_industry_stock_splint_blank`, `medieval_industry_stock_salve_base_pot`, `medieval_industry_stock_poultice_stock_packet`, `medieval_industry_stock_repair_supply_packet` | Provides common inputs for treatment, apothecary, and repair-kit craft passes. |

---

## Craft follow-up

The next implementation pass should not jump directly to finished catalogue items. It should first create production-chain crafts in dependency order:

1. terminal-source products to intermediate stock;
2. intermediate stock to tool and apparatus rows;
3. tool-supported stock refinement;
4. category-specific finished item crafts.

Any craft that discovers a missing terminal source should route that gap to the owning upstream seeder: butchery, forage, agriculture, pastoral/apiary/sericulture, mining, quarrying, forestry, clay, salt, gem, or other primary production.
