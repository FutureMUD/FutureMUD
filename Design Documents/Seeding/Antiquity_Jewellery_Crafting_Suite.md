# Antiquity Jewellery Crafting Suite

This document records the dedicated craft suite for the `SeedAntiquityJewellery` catalogue in `DatabaseSeeder/Seeders/ItemSeeder.Rework.Antiquity.cs`. Jewellery was previously treated as an older covered surface by the broad antiquity audit; the next-pass implementation now gives it a real craft source, upstream commodity chain, knowledge gate, and per-reference catalogue.

## Target Surface

- 162 jewellery prototypes from `SeedAntiquityJewellery`.
- Dynamic product discovery through `Functions / Worn Items / Jewellery` tags and stable references beginning with `jewellery_`.
- Metal, wire, bead, and setting stock for the materials present in the current catalogue.
- Culture-neutral craft names and visible echoes generated from item short descriptions rather than stable reference names.

## Implementation

The implementation lives in `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityJewellery.cs` and is registered from `SeedCrafts()` through `SeedAntiquityJewelleryCrafts()`.

The suite follows the same stock pattern as the other antiquity craft suites:

- `SeedAntiquityJewelleryIntermediateCommodityCrafts()` creates reusable stock commodities first.
- Final crafts discover current jewellery prototypes from seeded item tags and scan the stable reference, short description, and full description for material requirements.
- Final crafts produce the exact seeded prototype by ID and short description.
- Knowledge-gated `AddAntiquityCraft` calls use `Ancient Jewellery Crafting`.
- Visible craft text is sanitised through `SanitiseAntiquityJewelleryVisibleName(item.ShortDescription)`.

## Knowledge And Skills

| Knowledge | Used For |
| --- | --- |
| `Ancient Jewellery Crafting` | Metal jewellery, wire jewellery, bead strings, gem settings, bone/ivory ornaments, shell ornaments, and glass beads. |

| Material or Form | Main Skill | Notes |
| --- | --- | --- |
| Bronze, copper, and wrought iron jewellery | `Blacksmithing` | Uses prepared metal or wire stock with pliers, hammer/anvil, or burnishing tools. |
| Gold, silver, and electrum jewellery | `Silversmithing` | Uses precious-metal stock and finer finishing tools. |
| Glass and glazed ceramic beads | `Glassworking` | Uses the lit glory-hole furnace and polishing tools. |
| Bone and ivory jewellery | `Bonecarving` | Uses drilling and polishing tools. |
| Gem, shell, amber, pearl, agate, carnelian, jasper, quartz, lapis, turquoise, and similar bead stock | `Gemcraft` | Uses drilling, polishing, and setting stock. |
| Wooden bead jewellery | `Carpentry` | Uses drilling and polishing tools for simple wooden ornaments. |

## Commodity Tags

All jewellery stock tags are seeded under `Functions / Material Functions / Jewellery Craft Stock` and exported to `Design Documents/Data/SeededTagHierarchy.csv`.

| Tag | Purpose |
| --- | --- |
| `Jewellery Metal Stock` | Small prepared billets, sheet, strips, plaques, and mounts for non-wire jewellery. |
| `Jewellery Wire Stock` | Drawn wire and coiled wire for torcs, bracelets, rings, anklets, earrings, chains, and fine links. |
| `Jewellery Bead Stock` | Drilled and polished beads or bead-like ornaments made from glass, ceramic, shell, bone, ivory, wood, amber, pearl, and gemstones. |
| `Jewellery Setting Stock` | Cabochons, inlays, plaques, and small polished settings used as accent material in metal jewellery. |

## Source-Audited Product Catalogue

The code discovers this catalogue dynamically, but every current stable reference is listed here so the source-backed tests catch accidental drift.

### Anklets (22)

`jewellery_bronze_ankle_hoops`, `jewellery_bronze_chain_anklets`, `jewellery_bronze_heavy_anklets`, `jewellery_bronze_link_anklets`, `jewellery_bronze_tapered_anklets`, `jewellery_copper_ankle_rings`, `jewellery_copper_ankle_rings_valley`, `jewellery_copper_bell_anklets`, `jewellery_electrum_ankle_rings`, `jewellery_gold_carnelian_anklets`, `jewellery_gold_carnelian_bead_anklets`, `jewellery_gold_inlaid_turquoise_anklets`, `jewellery_gold_pearl_anklets`, `jewellery_gold_rosette_anklets`, `jewellery_gold_rosette_sea_anklets`, `jewellery_gold_spiral_anklets`, `jewellery_gold_turquoise_anklets`, `jewellery_granulated_gold_anklets`, `jewellery_shell_bead_anklets`, `jewellery_shell_copper_anklets`, `jewellery_shell_laced_anklets`, `jewellery_silver_bell_anklets`

### Bracelets (22)

`jewellery_blue_glazed_ceramic_bracelets`, `jewellery_bronze_disc_bracelet`, `jewellery_bronze_open_armlet`, `jewellery_bronze_open_bracelet`, `jewellery_bronze_pair_bracelets`, `jewellery_bronze_rounded_armlet`, `jewellery_bronze_spiral_bracelet`, `jewellery_carved_ivory_bangle`, `jewellery_copper_ram_bracelet`, `jewellery_electrum_spiral_bracelets`, `jewellery_glass_bead_bracelet`, `jewellery_gold_carnelian_bracelet`, `jewellery_gold_carnelian_bracelets`, `jewellery_gold_falcon_bracelets`, `jewellery_gold_griffin_armlet`, `jewellery_gold_griffin_armlets`, `jewellery_gold_lapis_bracelets`, `jewellery_gold_openwork_bracelets`, `jewellery_gold_ribbon_bracelet`, `jewellery_gold_serpent_bracelets`, `jewellery_gold_snake_head_armlet`, `jewellery_iron_wire_bracelet`

### Earrings (23)

`jewellery_bronze_ball_earrings`, `jewellery_bronze_disc_earrings`, `jewellery_bronze_drop_loop_earrings`, `jewellery_bronze_hollow_earrings`, `jewellery_bronze_hoop_earrings`, `jewellery_bronze_lobed_earrings`, `jewellery_bronze_loop_earrings`, `jewellery_bronze_wire_drop_earrings`, `jewellery_copper_hoop_earrings`, `jewellery_copper_wire_earrings`, `jewellery_gold_amber_drop_earrings`, `jewellery_gold_amber_earrings`, `jewellery_gold_bird_drop_earrings`, `jewellery_gold_boat_pendant_earrings`, `jewellery_gold_cage_ball_earrings`, `jewellery_gold_carnelian_earrings`, `jewellery_gold_crescent_earrings`, `jewellery_gold_glass_head_earrings`, `jewellery_gold_pearl_earrings`, `jewellery_gold_quartz_disc_earrings`, `jewellery_gold_ram_head_earrings`, `jewellery_gold_turquoise_drop_earrings`, `jewellery_small_bronze_wire_earrings`

### Necklaces (28)

`jewellery_agate_bead_necklace`, `jewellery_amber_disc_necklace`, `jewellery_blue_glass_eye_bead_necklace`, `jewellery_blue_glass_head_amulet`, `jewellery_blue_glass_ram_amulet`, `jewellery_blue_melon_bead_necklace`, `jewellery_bone_and_amber_necklace`, `jewellery_bone_tooth_necklace`, `jewellery_electrum_winged_pectoral`, `jewellery_glass_pendant_necklace`, `jewellery_glass_tube_necklace`, `jewellery_gold_agate_necklace`, `jewellery_gold_carnelian_broad_collar`, `jewellery_gold_filigrane_glass_necklace`, `jewellery_gold_foil_amber_necklace`, `jewellery_gold_glass_pendant_necklace`, `jewellery_gold_lapis_necklace`, `jewellery_gold_pearl_necklace`, `jewellery_gold_ram_head_pectoral`, `jewellery_gold_strap_necklace`, `jewellery_gold_turquoise_pendant_necklace`, `jewellery_gold_usekh_collar`, `jewellery_heavy_gold_torc`, `jewellery_red_inlaid_silver_torc`, `jewellery_shell_and_bone_necklace`, `jewellery_shell_carnelian_necklace`, `jewellery_simple_bronze_torc`, `jewellery_wooden_bead_necklace`

### Other Jewellery (44)

`jewellery_angular_marked_silver_fibula`, `jewellery_blue_glazed_ceramic_headband`, `jewellery_bone_belt_plaque`, `jewellery_bone_dress_pin`, `jewellery_bone_hair_pin`, `jewellery_bronze_animal_belt_plaques`, `jewellery_bronze_animal_cloak_pin`, `jewellery_bronze_animal_headband`, `jewellery_bronze_bead_headband`, `jewellery_bronze_cloak_pin`, `jewellery_bronze_crossbar_fibula`, `jewellery_bronze_disc_browband`, `jewellery_bronze_dress_fibula`, `jewellery_bronze_granule_headband`, `jewellery_bronze_rosette_headband`, `jewellery_bronze_studded_fibula`, `jewellery_bronze_winged_fibula`, `jewellery_copper_lion_brooch`, `jewellery_cowrie_shell_girdle`, `jewellery_electrum_rosette_diadem`, `jewellery_gold_animal_belt_plaques`, `jewellery_gold_animal_cloak_pin`, `jewellery_gold_animal_plaque_headband`, `jewellery_gold_bossed_circlet`, `jewellery_gold_cobra_frontlet`, `jewellery_gold_floral_brooch`, `jewellery_gold_garnet_diadem`, `jewellery_gold_ivory_belt_plaque`, `jewellery_gold_lapis_hair_pin`, `jewellery_gold_leaf_diadem`, `jewellery_gold_lion_girdle`, `jewellery_gold_palmette_diadem`, `jewellery_gold_ram_head_diadem`, `jewellery_gold_rosette_plate_diadem`, `jewellery_gold_rosette_sea_diadem`, `jewellery_gold_scarab_dress_pin`, `jewellery_gold_sphinx_fibula`, `jewellery_gold_studded_fibula`, `jewellery_penannular_bronze_brooch`, `jewellery_plain_bronze_brow_band`, `jewellery_shell_and_bone_browband`, `jewellery_shell_disc_headband`, `jewellery_silver_gilt_brow_band`, `jewellery_turquoise_bead_headband`

### Rings (23)

`jewellery_bone_scarab_ring`, `jewellery_bronze_animal_ring`, `jewellery_bronze_carnelian_ring`, `jewellery_bronze_coil_ring`, `jewellery_bronze_seal_ring`, `jewellery_bronze_square_seal_ring`, `jewellery_bronze_wire_ring`, `jewellery_copper_ram_ring`, `jewellery_copper_scarab_ring`, `jewellery_copper_seal_ring`, `jewellery_gold_carnelian_seal_ring`, `jewellery_gold_carnelian_signet_ring`, `jewellery_gold_emerald_ring`, `jewellery_gold_garnet_ring`, `jewellery_gold_jasper_ring`, `jewellery_gold_lapis_scarab_ring`, `jewellery_gold_lion_seal_ring`, `jewellery_gold_scarab_ring`, `jewellery_gold_scarab_signet_ring`, `jewellery_gold_serpent_ring`, `jewellery_gold_snake_head_ring`, `jewellery_gold_stag_ring`, `jewellery_iron_signet_ring`

## Verification

`ItemSeederAntiquityRemainingCraftingTests` asserts that the jewellery source count stays at 162 until the source catalogue changes, that `SeedAntiquityJewelleryCrafts()` is registered, that the jewellery stock tags exist in both the seeder and exported tag hierarchy, and that every current jewellery stable reference appears in this document.
