# Antiquity Equipment Crafting Suite

This document records the craft suite that closes the remaining non-household antiquity gaps from `ItemSeeder.Rework.Antiquity.cs`: common clothing/accessories, textile and leather craft tools, support tools, unlit workshop apparatus, non-leather armour, weapons, shields, ammunition, slings, and the construction hardware used by doors and gates.

## Target Surface

- 29 antiquity craft-tool prototypes from `SeedAntiquityClothing`.
- Support tool prototypes discovered from `Market / Professional Tools / Standard Tools`, excluding writing and medical products already owned by their dedicated suites.
- Unlit workshop apparatus prototypes for the hearth, kiln, glory-hole furnace, annealing lehr, and smelting furnace.
- 18 common culture-neutral clothing/accessory prototypes from `SeedAntiquityClothing`.
- 76 non-leather armour prototypes from `SeedAntiquityArmour`.
- 117 weapon, shield, ammunition, sling, and military-accessory prototypes from `SeedAntiquityWeaponsShieldsAccessories`.
- Shared `Door Hardware Stock` used by the expanded household/door suite.
- Eight portable repair-kit prototypes from `SeedAntiquityRepairKits`.

Existing jewellery, culture-specific textile clothing, leather clothing, leather armour, leather containers, leather furnishings, medical goods, writing goods, and household goods stay on their existing suites. The equipment suite discovers military goods dynamically from `Market / Military Goods` tags, discovers support tools dynamically from `Market / Professional Tools / Standard Tools`, and excludes stable references already handled by the culture/leather/writing/medical suites.

## Implementation

The implementation lives primarily in `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityEquipment.cs` and is registered from `SeedCrafts()` through `SeedAntiquityEquipmentCrafts()`. Portable repair kits live in `ItemSeederCrafting.AntiquityRepairKits.cs` and are registered through `SeedAntiquityRepairKitCrafts()`.

The suite follows the same stock pattern as the textile, leather, jewellery, and household work:

- Shared upstream commodity crafts run first.
- Reusable workshop heat-source crafts run before stock crafts. They consume an unlit apparatus and `charcoal`, produce the lit apparatus, and rely on the lit item's morph timer to cool back to the unlit prototype.
- Final crafts produce the exact seeded prototype by ID and short description.
- Variable products copy colour characteristics from commodity inputs where the item has variable colour components.
- Craft names and visible echoes are generated from sanitised short descriptions, not stable references, so visible craft text remains culture-neutral.
- Knowledge-gated `AddCraft` overloads are used for both upstream and final crafts.

## One-Step-Back Heat And Tool Audit

The May 2026 one-step-back pass audits the full `ItemSeederCrafting.Antiquity*.cs` source set, not only the clothing/equipment anchor file. The audit boundary is:

- every current `TagTool` requirement in the antiquity craft suite
- every literal commodity product tag produced by an antiquity craft
- every literal commodity input pile tag that should be produced upstream or intentionally remain a reusable stock output

The seeded tool closure now includes the previously missing heat and metal tools (`Fire`, `Bellows`, `Forge Tongs`, `Anvil`, `Hammer`, `Pliers`, `Cooking Pot`, `Sharpening`, and lit high-heat apparatus), dye/textile/leather tools (`Dye Strainer`, `Indigo Beating Paddle`, `Leather Paring Knife`, `Rope Hook`, `Twine Shuttle`), writing/book/pigment tools, and medical preparation tools. The second-pass implementation now makes the equipment-owned support tools and unlit workshop apparatus craftable; writing and medical support items remain owned by their dedicated craft files.

Lit workshop objects are craft-tool state markers, not runtime heat/light components. Current stock pairs are:

| Unlit item | Lit item | Lit tool tags |
| --- | --- | --- |
| `antiquity_workshop_hearth` | `antiquity_lit_workshop_hearth` | `Fire` |
| `antiquity_clay_smelting_furnace` | `antiquity_lit_clay_smelting_furnace` | `Lit Smelting Furnace`, `Hot Fire` |
| `antiquity_updraft_kiln` | `antiquity_lit_updraft_kiln` | `Lit Kiln`, `Hot Fire` |
| `antiquity_glory_hole_furnace` | `antiquity_lit_glory_hole_furnace` | `Lit Glory Hole`, `Hot Fire` |
| `antiquity_annealing_lehr` | `antiquity_lit_annealing_lehr` | `Lit Annealing Lehr`, `Hot Fire` |

Reusable intermediate outputs that are intentionally allowed to sit at a stock boundary are: `Armour Lamella Stock`, `Armour Plate Stock`, `Armour Ring Stock`, `Armour Scale Stock`, `Bisque Vessel Blank`, `Bookbinding Stock`, `Bow Stave`, `Carved Wood Stock`, `Coopered Staves`, `Decoction Stock`, `Door Hardware Stock`, `Dyed Cloth`, `Fulled Cloth`, `Glass Batch`, `Glass Vessel Blank`, `Helmet Bowl Stock`, `Herbal Remedy Stock`, `Ink Stock`, `Jewellery Bead Stock`, `Jewellery Metal Stock`, `Jewellery Setting Stock`, `Jewellery Wire Stock`, `Lake Pigment`, `Leather Scale`, `Paint Pigment`, `Papyrus Sheet Stock`, `Pen Blank`, `Prosthetic Stock`, `Sealed Leather Panel`, `Shield Board Stock`, `Shield Facing Stock`, `Splint Stock`, `Stone Vessel Blank`, `Surgical Tool Blank`, `Suture Stock`, `Tablet Blank`, `Tool Blank Stock`, `Waxed Tablet Board`, `Weapon Blade Stock`, `Weapon Head Stock`, `Weapon Shaft Stock`, `Wet Vessel Blank`, and `Woven Cloth`.

## Knowledge Gates

| Knowledge | Used For |
| --- | --- |
| `Ancient Equipment Crafting` | Shared construction and equipment stock, especially door and gate hardware. |
| `Ancient Weaponcrafting` | Weapon blade/head stock, shafts, bow staves, fletching stock, cord stock, ammunition, bows, slings, spears, blades, hafted weapons, and wooden weapons. |
| `Ancient Armourcrafting` | Helmet bowls, armour plates, rings, scales, lamellae, textile padding, shields, and final armour assembly. |
| `Ancient Toolmaking` | Metal, wooden, textile, leather, stone, bone, shell, glass, support-tool, and unlit apparatus prototypes. |
| `Ancient Common Clothing Crafting` | Culture-neutral common clothing and accessories left outside the culture garment suites. |

## Repair Kits

Repair kits use general material-family `RepairKit` components instead of era-specific components. The stock antiquity items are standard-quality kits; good and poor component grades are also seeded for future item variants.

| Stable Reference | Component | Primary Coverage |
| --- | --- | --- |
| `antiquity_textile_repair_kit` | `Repair_Cloth` | fabric armour padding, clothing, soft furnishings, and textile tools |
| `antiquity_leather_repair_kit` | `Repair_Leather` | leather armour, containers, straps, furnishings, and tools |
| `antiquity_wood_repair_kit` | `Repair_Wood` | wooden weapons, shields, furniture, doors, tool handles, and fixtures |
| `antiquity_metal_repair_kit` | `Repair_Metal` | metal furniture, doors, fittings, tools, and general hardware |
| `antiquity_stone_repair_kit` | `Repair_Stone` | stone tools, work surfaces, furniture, and fittings |
| `antiquity_ceramic_repair_kit` | `Repair_Ceramic` | ceramic vessels, lamps, tablets, containers, and fired-clay fittings |
| `antiquity_hard_organic_repair_kit` | `Repair_Hard_Organic` | bone, horn, shell, tooth, scale, claw, and beak fittings or tools |
| `antiquity_field_repair_bundle` | `Repair_Universal` | low-grade mixed emergency repair across odd or composite items |

## Commodity Tags

All equipment stock tags are seeded under `Functions / Material Functions / Antiquity Equipment Stock` and exported to `Design Documents/Data/SeededTagHierarchy.csv`.

| Tag | Purpose |
| --- | --- |
| `Weapon Blade Stock` | Blanks for swords, knives, daggers, and other blade weapons. |
| `Weapon Head Stock` | Spearheads, javelin heads, axe heads, mace heads, arrowheads, bolts, bullets, and bosses. |
| `Weapon Shaft Stock` | Spears, javelins, lances, arrows, bolts, clubs, staffs, and hafts. |
| `Bow Stave` | Worked bow blanks. |
| `Fletching Stock` | Light fletching material for arrows and bolts. |
| `Military Cord Stock` | Cordage for slings, bindings, bowstrings, lacing, and equipment ties. |
| `Shield Board Stock` | Wooden shield bodies. |
| `Shield Facing Stock` | Leather or hide shield faces that can carry colour variables. |
| `Helmet Bowl Stock` | Raised or formed helmet shells. |
| `Armour Plate Stock` | Plate, cuirass, greave, bracer, and similar metal armour stock. |
| `Armour Ring Stock` | Mail ring stock. |
| `Armour Scale Stock` | Scale armour stock. |
| `Armour Lamella Stock` | Lamellar armour stock. |
| `Armour Textile Padding` | Layered textile padding for armour and armour linings. |
| `Tool Blank Stock` | Reusable metal, wooden, textile, leather, ceramic, stone, bone, shell, and glass stock for craft tools. |
| `Door Hardware Stock` | Hinges, straps, latch plates, bars, and fittings for doors and gates. |

## Source-Audited Product Catalogue

The equipment suite has two product surfaces. The military surface is discovered dynamically from `Market / Military Goods` tags, while common clothing and craft-tool products are explicitly named in `ItemSeederCrafting.AntiquityEquipment.cs`.

### Armour

`antiquity_celtic_peaked_bronze_helmet`, `antiquity_celtic_crested_bronze_helmet`, `antiquity_celtic_riveted_iron_mail_shirt`, `antiquity_celtic_fine_riveted_mail_shirt`, `antiquity_germanic_iron_banded_helmet`, `antiquity_germanic_decorated_spangenhelm`, `antiquity_germanic_iron_mail_coat`, `antiquity_germanic_padded_wool_undercoat`, `antiquity_etruscan_cheek_guarded_bronze_helmet`, `antiquity_etruscan_anatomical_bronze_cuirass`, `antiquity_etruscan_bronze_left_greave`, `antiquity_etruscan_pair_bronze_greaves`, `antiquity_etruscan_round_bronze_pectoral`, `antiquity_etruscan_silver_inlaid_bronze_cuirass`, `antiquity_roman_montefortino_bronze_helmet`, `antiquity_roman_crested_montefortino_helmet`, `antiquity_roman_coolus_bronze_helmet`, `antiquity_roman_imperial_gallic_iron_helmet`, `antiquity_roman_plumed_iron_officer_helmet`, `antiquity_roman_intercisa_iron_ridge_helmet`, `antiquity_roman_bronze_pectoral_plate`, `antiquity_roman_triple_disc_bronze_cuirass`, `antiquity_roman_lorica_hamata_mail_shirt`, `antiquity_roman_lorica_hamata_fine_mail_shirt`, `antiquity_roman_lorica_squamata_scale_cuirass`, `antiquity_roman_lorica_squamata_gilded_cuirass`, `antiquity_roman_lorica_segmentata_banded_cuirass`, `antiquity_roman_padded_subarmalis`, `antiquity_roman_fine_padded_subarmalis`, `antiquity_roman_bronze_greaves`, `antiquity_roman_segmented_iron_armguard`, `antiquity_roman_late_mail_coat`, `antiquity_roman_late_scale_coat`, `antiquity_roman_bronze_muscle_cuirass`, `antiquity_hellenic_corinthian_bronze_helmet`, `antiquity_hellenic_crested_bronze_helmet`, `antiquity_hellenic_bronze_bell_cuirass`, `antiquity_hellenic_bronze_muscle_cuirass`, `antiquity_hellenic_linen_linothorax`, `antiquity_hellenic_fine_painted_linothorax`, `antiquity_hellenic_bronze_greaves`, `antiquity_hellenic_bronze_vambraces`, `antiquity_punic_conical_bronze_helmet`, `antiquity_punic_crested_bronze_helmet`, `antiquity_punic_linen_tube_cuirass`, `antiquity_punic_fine_painted_cuirass`, `antiquity_punic_iron_mail_cuirass`, `antiquity_punic_gilded_scale_cuirass`, `antiquity_punic_bronze_greaves`, `antiquity_persian_iron_scale_corselet`, `antiquity_persian_bronze_scale_corselet`, `antiquity_persian_padded_riding_coat`, `antiquity_persian_iron_helmet_with_aventail`, `antiquity_persian_felt_armoured_kyrbasia`, `antiquity_persian_lamellar_vambraces`, `antiquity_persian_lamellar_greaves`, `antiquity_egyptian_padded_linen_war_cap`, `antiquity_egyptian_blue_war_crown_cap`, `antiquity_egyptian_linen_corselet`, `antiquity_egyptian_bronze_scale_cuirass`, `antiquity_anatolian_forward_curved_bronze_helmet`, `antiquity_anatolian_ionian_bronze_helmet`, `antiquity_anatolian_bronze_chest_cuirass`, `antiquity_anatolian_linen_arrow_cuirass`, `antiquity_anatolian_bronze_greaves`, `antiquity_anatolian_fine_winged_bronze_helmet`, `antiquity_anatolian_fine_painted_linen_cuirass`, `antiquity_scythian_felt_padded_cap`, `antiquity_sarmatian_iron_scale_cuirass`, `antiquity_sarmatian_lamellar_cuirass`, `antiquity_sarmatian_iron_laminar_armguards`, `antiquity_sarmatian_lamellar_greaves`, `antiquity_sarmatian_mail_aventail_helmet`, `antiquity_kushite_padded_linen_archer_cap`, `antiquity_kushite_reinforced_linen_corselet`, `antiquity_kushite_bronze_scale_royal_cuirass`

### Weapons, Shields, and Ammunition

Arrows: `antiquity_ammo_common_field_point_arrow`, `antiquity_ammo_common_broadhead_arrow`, `antiquity_ammo_common_target_arrow`, `antiquity_ammo_persian_reed_arrow`, `antiquity_ammo_egyptian_reed_arrow`, `antiquity_ammo_scythian_barbed_arrow`, `antiquity_ammo_kushite_cane_arrow`, `antiquity_ammo_kushite_barbed_arrow`

Axes: `antiquity_weapon_celtic_iron_war_axe`, `antiquity_weapon_germanic_throwing_axe`, `antiquity_weapon_roman_dolabra`, `antiquity_weapon_persian_sagaris_axe`, `antiquity_weapon_egyptian_bronze_war_axe`, `antiquity_weapon_anatolian_crescent_axe`, `antiquity_weapon_scythian_sagaris`, `antiquity_weapon_kushite_hand_axe`

Bolts and bullets: `antiquity_ammo_common_field_point_bolt`, `antiquity_ammo_common_broadhead_bolt`, `antiquity_ammo_common_stone_sling_bullet`, `antiquity_ammo_common_lead_sling_bullet`

Bows and crossbows: `antiquity_weapon_common_short_self_bow`, `antiquity_weapon_roman_composite_bow`, `antiquity_weapon_hellenic_short_war_bow`, `antiquity_weapon_punic_composite_bow`, `antiquity_weapon_persian_composite_bow`, `antiquity_weapon_egyptian_simple_self_bow`, `antiquity_weapon_egyptian_composite_bow`, `antiquity_weapon_anatolian_composite_bow`, `antiquity_weapon_scythian_composite_bow`, `antiquity_weapon_kushite_long_self_bow`, `antiquity_weapon_kushite_compact_bow`, `antiquity_weapon_hellenic_belly_drawn_bow`

Daggers, swords, and knives: `antiquity_weapon_common_bronze_utility_knife`, `antiquity_weapon_common_iron_utility_knife`, `antiquity_weapon_common_bronze_dagger`, `antiquity_weapon_common_iron_dagger`, `antiquity_weapon_roman_pugio_dagger`, `antiquity_weapon_etruscan_bronze_dagger`, `antiquity_weapon_anatolian_long_dagger`, `antiquity_weapon_kushite_iron_dagger`, `antiquity_weapon_celtic_long_iron_slashing_sword`, `antiquity_weapon_celtic_fine_long_iron_sword`, `antiquity_weapon_germanic_broad_seax`, `antiquity_weapon_germanic_long_war_sword`, `antiquity_weapon_roman_republican_gladius`, `antiquity_weapon_roman_mainz_gladius`, `antiquity_weapon_roman_pompeii_gladius`, `antiquity_weapon_roman_late_spatha`, `antiquity_weapon_hellenic_xiphos`, `antiquity_weapon_hellenic_kopis`, `antiquity_weapon_punic_short_straight_sword`, `antiquity_weapon_punic_curved_mercenary_sword`, `antiquity_weapon_persian_akinakes`, `antiquity_weapon_egyptian_khopesh`, `antiquity_weapon_etruscan_short_sword`, `antiquity_weapon_etruscan_curved_sword`, `antiquity_weapon_anatolian_curved_short_sword`, `antiquity_weapon_scythian_akinakes`, `antiquity_weapon_kushite_short_sword`

Maces, clubs, slings, and polearms: `antiquity_weapon_egyptian_stone_mace`, `antiquity_weapon_egyptian_bronze_mace`, `antiquity_weapon_common_hardwood_club`, `antiquity_weapon_common_hardwood_quarterstaff`, `antiquity_weapon_common_simple_sling`, `antiquity_weapon_common_staff_sling`, `antiquity_weapon_roman_weighted_plumbata`, `antiquity_weapon_punic_balearic_sling`, `antiquity_weapon_hellenic_sarissa_pike`

Shields: `antiquity_shield_common_round_wooden_shield`, `antiquity_shield_common_wicker_shield`, `antiquity_shield_common_small_bossed_shield`, `antiquity_shield_celtic_long_oval_painted_shield`, `antiquity_shield_celtic_round_bossed_shield`, `antiquity_shield_germanic_round_linden_shield`, `antiquity_shield_germanic_painted_round_shield`, `antiquity_shield_roman_round_clipeus`, `antiquity_shield_roman_republican_oval_scutum`, `antiquity_shield_roman_curved_rectangular_scutum`, `antiquity_shield_roman_auxiliary_oval_shield`, `antiquity_shield_roman_parma`, `antiquity_shield_roman_late_oval_shield`, `antiquity_shield_hellenic_aspis`, `antiquity_shield_hellenic_pelta`, `antiquity_shield_hellenic_thureos`, `antiquity_shield_punic_oval_infantry_shield`, `antiquity_shield_punic_wicker_light_shield`, `antiquity_shield_persian_wicker_spara`, `antiquity_shield_egyptian_papyrus_shield`, `antiquity_shield_etruscan_round_bronze_shield`, `antiquity_shield_etruscan_large_aspis`, `antiquity_shield_anatolian_wicker_pelta`, `antiquity_shield_kushite_wicker_hide_shield`

Spears and javelins: `antiquity_weapon_celtic_leaf_bladed_spear`, `antiquity_weapon_celtic_light_throwing_javelin`, `antiquity_weapon_germanic_framea_spear`, `antiquity_weapon_germanic_barbed_angon_javelin`, `antiquity_weapon_roman_early_hasta_spear`, `antiquity_weapon_roman_verutum_javelin`, `antiquity_weapon_roman_heavy_pilum`, `antiquity_weapon_roman_contus_lance`, `antiquity_weapon_hellenic_dory_spear`, `antiquity_weapon_hellenic_light_javelin`, `antiquity_weapon_punic_socketed_infantry_spear`, `antiquity_weapon_punic_light_javelin`, `antiquity_weapon_punic_heavy_iron_javelin`, `antiquity_weapon_persian_long_spear`, `antiquity_weapon_persian_light_javelin`, `antiquity_weapon_egyptian_short_spear`, `antiquity_weapon_egyptian_light_javelin`, `antiquity_weapon_etruscan_bronze_tipped_spear`, `antiquity_weapon_etruscan_light_javelin`, `antiquity_weapon_anatolian_bronze_spear`, `antiquity_weapon_anatolian_light_javelin`, `antiquity_weapon_sarmatian_kontos_lance`, `antiquity_weapon_scythian_light_javelin`, `antiquity_weapon_kushite_iron_spear`, `antiquity_weapon_kushite_light_javelin`

### Explicit Common Clothing and Tool Products

| Stable Reference | Source Catalogue Name |
| --- | --- |
| `antiquity_bronze_awl_punch` | a bronze awl punch |
| `antiquity_bronze_dehairing_knife` | a bronze dehairing knife |
| `antiquity_bronze_edge_beveller` | a bronze leather edge beveller |
| `antiquity_bronze_hide_scraper` | a bronze hide scraper |
| `antiquity_bronze_leather_creaser` | a bronze leather creaser |
| `antiquity_bronze_leather_gouge` | a bronze leather gouge |
| `antiquity_bronze_leather_wax_pot` | a bronze leather wax pot |
| `antiquity_bronze_sewing_needle` | a bronze sewing needle |
| `antiquity_bronze_textile_shears` | a pair of bronze textile shears |
| `antiquity_clay_loom_weight` | a clay loom weight |
| `antiquity_conical_felt_cap` | a conical felt cap |
| `antiquity_distaff` | an oak distaff |
| `antiquity_drop_spindle` | a wooden drop spindle with a clay whorl |
| `antiquity_dye_vat` | an earthenware dye vat |
| `antiquity_fibre_hackle` | a bronze-toothed fibre hackle |
| `antiquity_fine_conical_felt_cap` | a fine conical cap |
| `antiquity_fine_front_knotted_girdle` | a fine knotted girdle |
| `antiquity_fine_linen_headcloth` | a fine headcloth |
| `antiquity_fine_linen_shoulder_shawl` | a fine linen shawl |
| `antiquity_fine_papyrus_sandals` | a pair of fine papyrus sandals |
| `antiquity_fine_woven_sash` | a fine sash |
| `antiquity_flax_break` | an oak flax break |
| `antiquity_fluted_felt_hat` | a fluted felt hat |
| `antiquity_front_knotted_girdle` | a front-knotted girdle |
| `antiquity_fullers_mallet` | an oak fuller's mallet |
| `antiquity_fullers_trough` | an oak fuller's trough |
| `antiquity_glass_usekh_collar` | a glass usekh collar |
| `antiquity_linen_breastband` | a linen breastband |
| `antiquity_linen_loincloth` | a linen loincloth |
| `antiquity_linen_shoulder_shawl` | a linen shawl |
| `antiquity_mordant_cauldron` | a bronze mordant cauldron |
| `antiquity_oak_brain_tanning_bucket` | an oak brain-tanning bucket |
| `antiquity_oak_shoe_last` | an oak shoe last |
| `antiquity_oak_stitching_pony` | an oak leather stitching clamp |
| `antiquity_oak_tanning_beam` | an oak tanning beam |
| `antiquity_oak_tanning_paddle` | an oak tanning paddle |
| `antiquity_oak_tanning_rack` | an oak tanning rack |
| `antiquity_papyrus_sandals` | a pair of papyrus sandals |
| `antiquity_retting_trough` | an oak retting trough |
| `antiquity_rounded_felt_cap` | a rounded felt cap |
| `antiquity_simple_woven_sash` | a woven sash |
| `antiquity_tall_kyrbasia` | a tall kyrbasia |
| `antiquity_tenter_frame` | an oak cloth tenter frame |
| `antiquity_warp_weighted_loom` | an oak warp-weighted loom |
| `antiquity_weavers_sword` | an oak weaver's sword |
| `antiquity_weaving_shuttle` | an oak weaving shuttle |
| `antiquity_wrapped_linen_headcloth` | a linen headcloth |

## Verification

`ItemSeederAntiquityRemainingCraftingTests` parses `ItemSeeder.Rework.Antiquity.cs`, applies the same coverage rules as the seeder, and fails if a current prototype is not covered by either an existing suite or the new equipment/expanded household pass. It also asserts upstream commodity tags, full antiquity `TagTool` item coverage, lit workshop morph metadata, reusable intermediate documentation, knowledge-gated registration, culture-neutral visible string construction, and the corrected fresh-install `HasWeaponcrafting` / `HasArmourcrafting` source definitions.
