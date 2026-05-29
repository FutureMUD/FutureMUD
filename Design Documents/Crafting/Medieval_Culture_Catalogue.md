# Medieval Culture Catalogue

This document is the authoritative exact catalogue for explicit medieval culture material-culture targets. The generated common/status wardrobe remains a generic baseline only; it does not satisfy explicit culture coverage unless an outfit spec deliberately references a shared slot and records that sharing.

The complete outfit list lives in `Medieval_Outfit_Catalogue.md`. This file remains the exact catalogue for culture-specific clothing targets and for military, food and beverage, writing and administration, household, devotional, and luxury goods.

## Catalogue Rules

- Exact outfit references live in `Medieval_Outfit_Catalogue.md`.
- Explicit culture items should be implemented as named item specs and final product crafts, not as cue text appended to generic templates.
- Generic baseline items must be labelled as generic baseline.
- Pattern-only documentation is not sufficient for explicit culture items; exact stable references are listed below.
- Food items should be actual food/beverage items unless explicitly marked as tableware or vessel stock.
- Wooden, wax, birchbark, and non-paper writing surfaces should use `InscribableSurface`-style components where available.
- Military, writing, food, household, and devotional catalogues should coordinate with outfit role items.

## Implementation Status Legend

- `ImplementedItem`: the exact catalogue stable reference is seeded as an item spec and must have craft coverage or an explicit craft exemption.
- `CoveredByOutfitPiece`: the exact clothing target is represented by one or more explicit outfit-piece item specs rather than by an independent item spec with the catalogue stable reference.
- `AliasOfExistingStableReference`: the exact catalogue target is intentionally covered by a broader seeded stable reference; the bullet records both the catalogue reference and the implementation reference.
- `Deferred`: the exact catalogue target is retained as a future material-culture target and includes the reason it is not seeded yet.

## Generic Baseline Patterns

These patterns document generated baseline or family coverage only. They are not substitutes for the exact explicit culture references in the catalogue sections below.

- `medieval_clothing_{culture}_`
- `medieval_clothing_{culture}_{status}_{piece}`
- `medieval_food_{culture}_{foodway_item}`
- `medieval_writing_{culture}_{administration_item}`
- `medieval_military_{culture}_{equipment_piece}`
- `medieval_weapon_{culture}_{weapon}`
- `medieval_shield_{culture}`
- `medieval_devotional_{culture}_pilgrim_token`
- `medieval_household_{furniture_or_container}`
- `medieval_medical_{medical_item}`
- `medieval_jewellery_{jewellery_item}`

Generated status-role wardrobe keys use `peasant`, `artisan`, `merchant`, `noble`, `clergy`, and `military` as baseline status buckets. Outfit references use the separate role axis `religious` where the complete outfit needs a clerical/devotional role item.

Production-chain coverage includes crossbow manufacture, paper and parchment, stained glass, guild weights and measures, and luxury textile finishes. These are documented here as family gates while exact seeded stable references remain listed in the relevant suite docs.

Common baseline and shared support items include:

- `historic_workshop_hearth`
- `historic_lit_workshop_hearth`
- `historic_updraft_kiln`
- `historic_lit_updraft_kiln`
- `historic_warp_weighted_loom`
- `historic_treadle_loom`
- `historic_drop_spindle`
- `historic_sewing_needle`
- `historic_textile_shears`
- `historic_awl_punch`
- `historic_dye_vat`
- `historic_tanning_rack`
- `historic_hand_quern`
- `historic_oil_lamp`
- `historic_lit_oil_lamp`
- `historic_workshop_anvil`
- `historic_forge_tongs`
- `historic_workshop_hammer`
- `historic_bellows`
- `medieval_coopers_croze`
- `medieval_iron_wood_plane`
- `medieval_bookbinder_press`
- `medieval_locksmith_file_set`
- `medieval_devotional_wooden_rosary`
- `medieval_devotional_reliquary_locket`
- `medieval_devotional_icon_pendant`
- `medieval_devotional_pilgrim_badge`
- `medieval_devotional_reliquary_box`
- `medieval_devotional_scripture_tablet`
- `medieval_textile_repair_kit`
- `medieval_leather_repair_kit`
- `medieval_metal_repair_kit`

## Suite Pointers

- Builder Workflows
- Food and beverage
- Furniture and containers
- Jewellery/devotional
- Medical/apothecary
- Writing/administration

## Outfit Coverage Summary

Every culture requires 12 complete outfit definitions: male and female presentations for peasant, artisan, merchant, noble, religious, and military roles. See `Medieval_Outfit_Catalogue.md` for exact `medieval_outfit_{culture}_{sex}_{class}` references.

## Exact Culture Catalogue

### Early Anglo-Saxon/Insular (`early_anglo_saxon`)

#### Clothing

- `medieval_clothing_early_anglo_saxon_tablet_banded_wool_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_male_peasant_tablet_banded_wool_tunic; medieval_outfit_piece_early_anglo_saxon_female_peasant_tablet_banded_wool_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_square_cloak_disc_brooch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_military_brooch_fastened_war_cloak; medieval_outfit_piece_early_anglo_saxon_female_military_cloak_brooch; medieval_outfit_piece_early_anglo_saxon_female_peasant_simple_cloak_brooch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_linen_head_veil` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_peasant_linen_head_veil; medieval_outfit_piece_early_anglo_saxon_female_merchant_linen_veil; medieval_outfit_piece_early_anglo_saxon_female_noble_long_linen_veil`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_seax_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_male_artisan_seax_belt; medieval_outfit_piece_early_anglo_saxon_male_military_seax_belt; medieval_outfit_piece_early_anglo_saxon_male_noble_seax_belt_with_mounts`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_bead_necklace` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_merchant_bead_necklace; medieval_outfit_piece_early_anglo_saxon_female_noble_bead_necklace; medieval_outfit_piece_early_anglo_saxon_male_noble_bead_necklace`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_monastic_wool_habit` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_male_religious_monastic_wool_habit`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_embroidered_noble_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_merchant_lined_mantle; medieval_outfit_piece_early_anglo_saxon_female_noble_brooch_fastened_mantle; medieval_outfit_piece_early_anglo_saxon_female_noble_embroidered_noble_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_brooch_fastened_riding_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_military_brooch_fastened_war_cloak; medieval_outfit_piece_early_anglo_saxon_female_military_cloak_brooch; medieval_outfit_piece_early_anglo_saxon_female_noble_brooch_fastened_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_plain_work_smock` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_early_anglo_saxon_wool_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_male_artisan_wool_cap; medieval_outfit_piece_early_anglo_saxon_male_merchant_felt_cap; medieval_outfit_piece_early_anglo_saxon_male_military_padded_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_wool_leg_wraps` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_military_wool_trews_or_skirted_leg_wraps; medieval_outfit_piece_early_anglo_saxon_male_artisan_wool_leg_wraps; medieval_outfit_piece_early_anglo_saxon_male_military_leg_wraps`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_early_anglo_saxon_soft_leather_ankle_shoes` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_early_anglo_saxon_female_peasant_soft_ankle_shoes; medieval_outfit_piece_early_anglo_saxon_male_peasant_soft_ankle_shoes; medieval_outfit_piece_early_anglo_saxon_female_artisan_leather_shoes`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_early_anglo_saxon_broad_seax` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_early_anglo_saxon_seax`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_early_anglo_saxon_ash_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_early_anglo_saxon_bossed_round_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_early_anglo_saxon`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_early_anglo_saxon_mail_shirt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_early_anglo_saxon_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_early_anglo_saxon_spangenhelm` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_early_anglo_saxon_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_early_anglo_saxon_shield_wall_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_early_anglo_saxon_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_early_anglo_saxon_leather_archer_bracer` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_early_anglo_saxon_war_cloak` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_early_anglo_saxon_oat_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_barley_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_smoked_eel_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_ale_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_curd_cheese_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_honeyed_oat_cake` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_monastery_loaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_early_anglo_saxon_wooden_trencher` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_early_anglo_saxon_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_early_anglo_saxon_wax_diptych` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_early_anglo_saxon_charter_strip` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_early_anglo_saxon_monastic_manuscript_leaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_early_anglo_saxon_reeve_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_early_anglo_saxon_seal_tag_bundle` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_early_anglo_saxon_gospel_book_pouch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_early_anglo_saxon_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_early_anglo_saxon_carved_hall_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_early_anglo_saxon_hanging_clay_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_early_anglo_saxon_monastic_book_stand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_lectern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_jewellery_early_anglo_saxon_brooch_display_pin` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_jewellery_bronze_ring_pin`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_early_anglo_saxon_wooden_reliquary` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_reliquary_box`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Late Anglo-Saxon/Anglo-Danish (`anglo_danish`)

#### Clothing

- `medieval_clothing_anglo_danish_panelled_wool_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_noble_panelled_noble_tunic; medieval_outfit_piece_anglo_danish_male_peasant_panelled_wool_tunic; medieval_outfit_piece_anglo_danish_female_noble_panelled_overgown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_long_seax_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_artisan_long_seax_belt; medieval_outfit_piece_anglo_danish_male_military_long_seax_belt; medieval_outfit_piece_anglo_danish_male_noble_mounted_seax_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_wrapped_trews` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_military_wrapped_trews; medieval_outfit_piece_anglo_danish_male_military_wrapped_trews; medieval_outfit_piece_anglo_danish_male_peasant_wrapped_trews`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_embroidered_collar_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_merchant_embroidered_collar_tunic; medieval_outfit_piece_anglo_danish_female_merchant_embroidered_collar_overgown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_ring_pin_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_peasant_bronze_ring_pin; medieval_outfit_piece_anglo_danish_male_artisan_iron_cloak_pin; medieval_outfit_piece_anglo_danish_male_merchant_silver_ring_pin`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_leather_war_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_artisan_leather_belt; medieval_outfit_piece_anglo_danish_female_military_arming_belt; medieval_outfit_piece_anglo_danish_female_military_war_brooch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_padded_shield_wall_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_military_padded_shield_wall_tunic; medieval_outfit_piece_anglo_danish_female_military_padded_shield_wall_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_wool_hood` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_peasant_wool_hood`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_trader_purse_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_male_merchant_purse_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_head_rail` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_artisan_head_rail; medieval_outfit_piece_anglo_danish_female_merchant_linen_head_rail; medieval_outfit_piece_anglo_danish_female_military_head_rail_under_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_heavy_sea_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_military_heavy_cloak; medieval_outfit_piece_anglo_danish_female_noble_fur_edged_cloak; medieval_outfit_piece_anglo_danish_female_peasant_rough_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_anglo_danish_leather_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_anglo_danish_female_military_boots; medieval_outfit_piece_anglo_danish_male_military_boots; medieval_outfit_piece_anglo_danish_male_noble_soft_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_anglo_danish_long_seax` - Status: `ImplementedItem`; Implementation: `medieval_weapon_anglo_danish_long_seax`
- `medieval_weapon_anglo_danish_dane_axe` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_anglo_danish_socketed_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_anglo_danish_painted_round_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_anglo_danish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_anglo_danish_mail_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_anglo_danish_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_anglo_danish_nasal_helm` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_anglo_danish_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_anglo_danish_shield_wall_sling` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_anglo_danish_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_anglo_danish_huscarl_weapon_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_anglo_danish_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_anglo_danish_rye_loaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_smoked_fish_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_ale_jack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_pea_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_salt_beef_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_cheese_trencher` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_honey_cake` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_anglo_danish_market_bread_bundle` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_anglo_danish_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_anglo_danish_writ_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_anglo_danish_estate_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_anglo_danish_rune_marked_trade_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_anglo_danish_wax_tablet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_anglo_danish_sealed_rent_record` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_anglo_danish_seax_belt_document_pouch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_anglo_danish_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_anglo_danish_iron_bound_hall_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_anglo_danish_reeve_account_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_anglo_danish_drinking_horn_rack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_wall_shelf`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_anglo_danish_carved_prayer_cross` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_anglo_danish_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_jewellery_anglo_danish_ring_pin` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_jewellery_bronze_ring_pin`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Norse (`norse`)

#### Clothing

- `medieval_clothing_norse_hangerok_apron_dress` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_peasant_hangerok_apron_dress`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_linen_underdress` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_artisan_linen_underdress; medieval_outfit_piece_norse_female_merchant_fine_underdress; medieval_outfit_piece_norse_female_noble_fine_underdress`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_jewellery_norse_oval_brooch_pair` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_artisan_oval_brooch_pair; medieval_outfit_piece_norse_female_merchant_oval_brooch_pair_and_beads; medieval_outfit_piece_norse_female_noble_oval_brooch_pair`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_bead_strung_apron_straps` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_merchant_bead_strung_hangerok`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_heavy_sea_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_military_heavy_sea_cloak; medieval_outfit_piece_norse_female_peasant_sea_cloak; medieval_outfit_piece_norse_male_merchant_fur_edged_sea_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_fur_edged_hood` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_merchant_fur_edged_cloak; medieval_outfit_piece_norse_male_merchant_fur_edged_sea_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_wool_leg_wraps` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_artisan_leg_wraps; medieval_outfit_piece_norse_female_merchant_leg_wraps; medieval_outfit_piece_norse_female_military_leg_wraps`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_high_leather_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_military_high_boots; medieval_outfit_piece_norse_male_military_high_boots; medieval_outfit_piece_norse_male_noble_high_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_trader_kaftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_male_merchant_trader_kaftan; medieval_outfit_piece_norse_male_artisan_trader_tunic; medieval_outfit_piece_norse_male_noble_decorated_kaftan`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_wool_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_military_headwrap_under_cap; medieval_outfit_piece_norse_male_artisan_wool_cap; medieval_outfit_piece_norse_male_merchant_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_arming_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_female_military_arming_hangerok_or_tunic; medieval_outfit_piece_norse_male_military_arming_tunic; medieval_outfit_piece_norse_female_military_arming_shift`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norse_leather_belt_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norse_male_peasant_belt_pouch; medieval_outfit_piece_norse_female_artisan_tool_pouch; medieval_outfit_piece_norse_female_artisan_woven_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_norse_bearded_axe` - Status: `ImplementedItem`; Implementation: `medieval_weapon_norse_bearded_axe`
- `medieval_weapon_norse_broad_axe` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_norse_socketed_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_norse_bossed_round_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_norse`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norse_riveted_mail_shirt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norse_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norse_conical_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norse_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_norse_hunting_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_norse_gorytos_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norse_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_norse_stockfish_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_sour_milk_skin` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_rye_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_preserved_meat_bundle` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_curd_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_shipboard_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_ale_horn` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norse_smoked_fish_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_norse_runic_tally_stick` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norse_merchant_weight_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norse_ship_cargo_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norse_wax_tablet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norse_trade_tablet_wallet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norse_runic_memorial_plaque` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norse_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_norse_sea_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norse_carved_comb_case` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_trade_norse_weight_pouch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_trade_balance_scale`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norse_drinking_horn` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norse_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norse_shipboard_storage_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Norman/Angevin (`norman`)

#### Clothing

- `medieval_clothing_norman_split_riding_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_male_noble_split_riding_tunic; medieval_outfit_piece_norman_female_military_split_riding_skirt_or_chausses`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_long_sleeved_cote` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_male_merchant_long_sleeved_cote`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_short_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_merchant_lined_mantle; medieval_outfit_piece_norman_female_noble_noble_mantle; medieval_outfit_piece_norman_male_artisan_short_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_mail_surcoat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_military_mail_surcoat; medieval_outfit_piece_norman_male_military_mail_surcoat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_arming_coif` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_military_nasal_arming_coif_and_veil; medieval_outfit_piece_norman_male_military_nasal_arming_coif; medieval_outfit_piece_norman_female_military_arming_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_court_bliaut_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_artisan_work_gown; medieval_outfit_piece_norman_female_merchant_bliaut_style_overgown; medieval_outfit_piece_norman_female_merchant_fitted_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_linen_coif` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_military_nasal_arming_coif_and_veil; medieval_outfit_piece_norman_male_artisan_coif; medieval_outfit_piece_norman_male_merchant_coif_and_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_fine_leather_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_artisan_leather_belt; medieval_outfit_piece_norman_female_military_arming_belt; medieval_outfit_piece_norman_female_peasant_belt_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_riding_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_military_riding_boots; medieval_outfit_piece_norman_male_military_riding_boots; medieval_outfit_piece_norman_female_military_split_riding_skirt_or_chausses`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_hooded_travel_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_peasant_hooded_cloak; medieval_outfit_piece_norman_male_peasant_hooded_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_jewellery_norman_cloak_clasp` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_noble_cloak_clasp; medieval_outfit_piece_norman_female_peasant_simple_cloak_clasp; medieval_outfit_piece_norman_male_merchant_cloak_clasp`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_norman_chapel_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_norman_female_religious_religious_robe; medieval_outfit_piece_norman_male_religious_clerical_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_norman_arming_sword` - Status: `ImplementedItem`; Implementation: `medieval_weapon_norman_arming_sword`
- `medieval_weapon_norman_couched_lance` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_norman_military_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_norman_kite_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_norman`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norman_mail_hauberk` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norman_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norman_nasal_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norman_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norman_padded_aketon` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norman_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_norman_arming_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_norman_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_norman_wheaten_loaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_meat_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_cheese_trencher` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_salted_beef_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_feast_roast_platter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_spiced_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_norman_market_pie` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_norman_meal_platter`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_norman_sealed_parchment_charter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norman_exchequer_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norman_writ_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norman_notary_seal` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norman_manorial_account_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_norman_court_summons_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_norman_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_norman_manor_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norman_chapel_lectern` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_lectern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norman_heraldic_shield_rack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_wall_shelf`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_norman_wall_candle_sconce` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_candle_stand`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_norman_pilgrim_badge` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_norman_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### High Medieval Britain/Marcher (`high_british`)

#### Clothing

- `medieval_clothing_high_british_wool_cote` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_male_artisan_work_cote; medieval_outfit_piece_high_british_male_merchant_lined_cote; medieval_outfit_piece_high_british_male_peasant_wool_cote`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_sleeveless_surcoat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_military_surcoat; medieval_outfit_piece_high_british_male_military_surcoat; medieval_outfit_piece_high_british_male_noble_silk_trimmed_surcoat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_linen_coif` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_military_arming_coif_with_headcloth; medieval_outfit_piece_high_british_male_artisan_coif; medieval_outfit_piece_high_british_male_military_arming_coif`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_wimple_and_veil` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_noble_wimple_and_veil; medieval_outfit_piece_high_british_female_religious_veil_and_wimple; medieval_outfit_piece_high_british_female_merchant_wimple`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_braies` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_male_artisan_braies; medieval_outfit_piece_high_british_male_merchant_braies; medieval_outfit_piece_high_british_male_military_braies`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_wool_chausses` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_military_chausses_or_split_riding_skirt; medieval_outfit_piece_high_british_male_military_padded_chausses`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_arming_gambeson` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_military_arming_belt; medieval_outfit_piece_high_british_female_military_arming_coif_with_headcloth; medieval_outfit_piece_high_british_female_military_arming_shift`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_long_belt_purse` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_male_merchant_purse_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_merchant_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_merchant_merchant_gown; medieval_outfit_piece_high_british_female_artisan_work_gown; medieval_outfit_piece_high_british_female_noble_court_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_wool_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_merchant_lined_mantle; medieval_outfit_piece_high_british_female_noble_fur_mantle; medieval_outfit_piece_high_british_male_merchant_travel_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_archer_bracer` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_military_archer_bracer; medieval_outfit_piece_high_british_male_military_archer_bracer`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_high_british_archer_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_high_british_female_artisan_leather_belt; medieval_outfit_piece_high_british_female_military_archer_bracer; medieval_outfit_piece_high_british_female_military_arming_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_high_british_arming_sword` - Status: `ImplementedItem`; Implementation: `medieval_weapon_high_british_arming_sword`
- `medieval_weapon_high_british_war_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_high_british_longbow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_high_british_heater_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_high_british`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_high_british_mail_hauberk` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_high_british_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_high_british_kettle_hat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_high_british_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_high_british_quilted_gambeson` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_high_british_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_high_british_arrow_bag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_high_british_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_high_british_trencher_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_ale_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_cheese_wedge` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_pottage_bowl` - Status: `ImplementedItem`; Implementation: `medieval_food_high_british_pottage_bowl`
- `medieval_food_high_british_salted_fish_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_meat_pie` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_honeyed_pastry` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_high_british_cider_jack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_high_british_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_high_british_manor_account_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_high_british_guild_register_leaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_high_british_sealed_writ` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_high_british_archery_levy_list` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_high_british_chapel_book` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_high_british_toll_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_high_british_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_high_british_guild_counter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_market_counter`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_high_british_wool_merchant_bale` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_trade_sealable_bale`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_high_british_parish_alms_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_high_british_archery_target_bundle` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_devotional_high_british_chapel_candle_stand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_high_british_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Gaelic/Welsh/Highland (`gaelic`)

#### Clothing

- `medieval_clothing_gaelic_brat_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_peasant_brat_mantle; medieval_outfit_piece_gaelic_male_peasant_brat_mantle; medieval_outfit_piece_gaelic_female_merchant_lined_brat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_leine_long_shirt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_male_artisan_work_long_shirt; medieval_outfit_piece_gaelic_male_merchant_fine_long_shirt; medieval_outfit_piece_gaelic_male_peasant_linen_long_shirt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_jewellery_gaelic_ring_pin` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_artisan_bronze_ring_pin; medieval_outfit_piece_gaelic_female_merchant_silver_ring_pin; medieval_outfit_piece_gaelic_female_military_ring_pin`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_deerskin_shoes` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_peasant_deerskin_shoes; medieval_outfit_piece_gaelic_male_peasant_deerskin_shoes; medieval_outfit_piece_gaelic_female_artisan_leather_shoes`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_woven_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_artisan_woven_belt; medieval_outfit_piece_gaelic_female_peasant_woven_belt; medieval_outfit_piece_gaelic_male_peasant_woven_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_hill_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_male_artisan_short_hill_cloak; medieval_outfit_piece_gaelic_female_religious_cowl_cloak; medieval_outfit_piece_gaelic_male_religious_cowl_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_spear_carrier_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_military_spear_carrier_belt; medieval_outfit_piece_gaelic_male_military_spear_carrier_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_linen_headcloth` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_artisan_headcloth; medieval_outfit_piece_gaelic_female_military_headcloth_under_cap; medieval_outfit_piece_gaelic_female_peasant_linen_headcloth`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_wool_trews` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_military_split_skirt_or_trews; medieval_outfit_piece_gaelic_male_artisan_wool_trews; medieval_outfit_piece_gaelic_male_merchant_wool_trews`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_bardic_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_noble_bardic_mantle; medieval_outfit_piece_gaelic_male_noble_bardic_or_lordly_mantle; medieval_outfit_piece_gaelic_female_peasant_brat_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_pastoral_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_gaelic_female_peasant_pastoral_pouch; medieval_outfit_piece_gaelic_male_peasant_pastoral_pouch; medieval_outfit_piece_gaelic_female_artisan_tool_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_gaelic_rough_weather_hood` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`

#### Military

- `medieval_weapon_gaelic_long_spear` - Status: `ImplementedItem`; Implementation: `medieval_weapon_gaelic_long_spear`
- `medieval_weapon_gaelic_javelin_bundle` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_gaelic_short_knife` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_gaelic_small_hide_targe` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_gaelic`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_gaelic_light_padded_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_gaelic_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_gaelic_ring_pin_war_cloak` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_gaelic_hunting_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_gaelic_leather_bracers` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_gaelic_oat_bannock` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_curd_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_smoked_meat_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_ale_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_honey_cake` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_pastoral_cheese` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_fish_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_gaelic_travel_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_gaelic_boundary_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_gaelic_bardic_manuscript_leaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_gaelic_kinship_record_strip` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_gaelic_ring_pin_document_pouch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_gaelic_monastic_note_board` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_gaelic_tribute_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_gaelic_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_devotional_gaelic_ring_pin_shrine_cloth` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_gaelic_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_music_gaelic_harp_prop` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_music_psaltery`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_gaelic_hide_travel_bag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_document_satchel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_gaelic_pastoral_milk_vessel` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_gaelic_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_gaelic_carved_oath_stick` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`


### Carolingian/Frankish (`carolingian`)

#### Clothing

- `medieval_clothing_carolingian_high_belted_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_male_noble_high_belted_noble_tunic; medieval_outfit_piece_carolingian_male_peasant_high_belted_tunic; medieval_outfit_piece_carolingian_female_peasant_high_belted_work_dress`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_broad_banded_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_military_broad_banded_mantle; medieval_outfit_piece_carolingian_female_peasant_broad_banded_mantle; medieval_outfit_piece_carolingian_male_military_broad_banded_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_court_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_military_cloak_pin; medieval_outfit_piece_carolingian_female_religious_cowl_cloak; medieval_outfit_piece_carolingian_male_artisan_short_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_spatha_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_male_military_spatha_belt; medieval_outfit_piece_carolingian_male_noble_spatha_belt; medieval_outfit_piece_carolingian_female_artisan_leather_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_wool_leg_wraps` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_male_artisan_leg_wraps; medieval_outfit_piece_carolingian_male_military_leg_wraps; medieval_outfit_piece_carolingian_male_peasant_leg_wraps`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_clerical_dalmatic_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_male_religious_clerical_dalmatic_style_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_embroidered_noble_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_noble_embroidered_gown; medieval_outfit_piece_carolingian_male_artisan_broad_banded_work_tunic; medieval_outfit_piece_carolingian_male_merchant_broad_banded_tunic`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_heavy_riding_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_military_cloak_pin; medieval_outfit_piece_carolingian_female_religious_cowl_cloak; medieval_outfit_piece_carolingian_male_artisan_short_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_leather_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_military_boots; medieval_outfit_piece_carolingian_male_military_boots; medieval_outfit_piece_carolingian_male_noble_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_jewellery_carolingian_noble_fibula` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_noble_noble_fibula; medieval_outfit_piece_carolingian_male_merchant_silver_fibula; medieval_outfit_piece_carolingian_male_noble_noble_fibula`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_carolingian_palace_servant_tunic` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_carolingian_monastic_cowl` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_carolingian_female_religious_cowl_cloak; medieval_outfit_piece_carolingian_male_religious_monastic_cowl`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_carolingian_spatha` - Status: `ImplementedItem`; Implementation: `medieval_weapon_carolingian_spatha`
- `medieval_weapon_carolingian_war_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_carolingian_light_axe` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_carolingian_large_round_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_carolingian`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_carolingian_mail_shirt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_carolingian_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_carolingian_reinforced_helm` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_carolingian_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_carolingian_riding_spurs` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_carolingian_palace_guard_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_carolingian_barley_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_pork_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_ale_jug` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_monastic_loaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_salted_pork_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_feast_roast` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_cheese_board` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_carolingian_honeyed_pastry` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_carolingian_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_carolingian_capitulary_copy` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_carolingian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_carolingian_estate_polyptych_leaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_carolingian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_carolingian_palace_wax_tablet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_carolingian_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_carolingian_monastic_codex` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_carolingian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_carolingian_seal_tag_packet` - Status: `ImplementedItem`; Implementation: `medieval_writing_carolingian_seal_tag_packet`
- `medieval_writing_carolingian_tax_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_carolingian_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_carolingian_palace_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_carolingian_manor_table` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_trestle_table`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_carolingian_reliquary_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_reliquary_box`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_carolingian_monastic_lectern` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_lectern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_jewellery_carolingian_enamel_brooch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_jewellery_silver_brooch`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Capetian/Low Countries (`capetian`)

#### Clothing

- `medieval_clothing_capetian_wool_cote` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_male_artisan_guild_work_cote; medieval_outfit_piece_capetian_male_noble_silk_trimmed_cote; medieval_outfit_piece_capetian_male_peasant_plain_wool_cote`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_bliaut_style_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_noble_bliaut_style_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_lined_burgher_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_male_merchant_lined_burgher_gown; medieval_outfit_piece_capetian_female_merchant_lined_burgher_overgown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_wimple` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_merchant_wimple; medieval_outfit_piece_capetian_female_noble_wimple_and_veil; medieval_outfit_piece_capetian_female_religious_wimple_and_veil`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_linen_coif` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_military_arming_coif_and_headcloth; medieval_outfit_piece_capetian_male_artisan_coif; medieval_outfit_piece_capetian_male_military_arming_coif`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_fitted_hood` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_merchant_fitted_gown; medieval_outfit_piece_capetian_male_merchant_fitted_hose; medieval_outfit_piece_capetian_male_merchant_hood`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_fine_purse_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_male_merchant_purse_belt; medieval_outfit_piece_capetian_female_artisan_leather_belt; medieval_outfit_piece_capetian_female_merchant_purse`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_guild_apron` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_artisan_guild_apron; medieval_outfit_piece_capetian_male_artisan_guild_apron; medieval_outfit_piece_capetian_female_artisan_guild_token`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_court_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_merchant_mantle; medieval_outfit_piece_capetian_female_noble_rich_mantle; medieval_outfit_piece_capetian_male_merchant_travel_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_silk_trimmed_surcoat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_military_surcoat; medieval_outfit_piece_capetian_female_noble_silk_trimmed_overgown; medieval_outfit_piece_capetian_male_military_surcoat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_wool_hose` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_artisan_hose; medieval_outfit_piece_capetian_female_merchant_fine_hose; medieval_outfit_piece_capetian_female_peasant_hose`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_capetian_winter_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_capetian_female_merchant_mantle; medieval_outfit_piece_capetian_female_noble_rich_mantle; medieval_outfit_piece_capetian_male_merchant_travel_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_capetian_arming_sword` - Status: `ImplementedItem`; Implementation: `medieval_weapon_capetian_arming_sword`
- `medieval_weapon_capetian_town_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_capetian_crossbow` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_common_crossbow`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_shield_capetian_heater_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_capetian`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_capetian_mail_hauberk` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_capetian_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_capetian_padded_aketon` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_capetian_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_capetian_kettle_hat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_capetian_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_capetian_guild_militia_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_capetian_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_capetian_white_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_onion_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_cheese_trencher` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_salted_fish_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_feast_pastry` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_capetian_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_capetian_market_ration`
- `medieval_food_capetian_spiced_meat_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_capetian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_capetian_notarial_note` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_capetian_sealed_letter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_capetian_guild_register` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_capetian_town_toll_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_capetian_merchant_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_capetian_chapel_booklet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_capetian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_capetian_guild_counter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_market_counter`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_capetian_cloth_merchant_bale` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_trade_sealable_bale`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_capetian_town_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_capetian_pilgrim_badge` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_capetian_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_capetian_stained_glass_quarry_panel` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`


### German/HRE/Alpine-North Italian (`german_hre`)

#### Clothing

- `medieval_clothing_german_hre_guild_apron` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_artisan_guild_apron; medieval_outfit_piece_german_hre_male_artisan_guild_apron_over_tunic; medieval_outfit_piece_german_hre_female_artisan_guild_mark`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_civic_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_male_merchant_civic_gown; medieval_outfit_piece_german_hre_female_artisan_wool_gown; medieval_outfit_piece_german_hre_female_merchant_civic_overgown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_fur_lined_mantle` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_merchant_fur_lined_mantle; medieval_outfit_piece_german_hre_female_noble_fur_lined_mantle; medieval_outfit_piece_german_hre_male_merchant_fur_lined_mantle`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_fitted_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_male_peasant_fitted_wool_tunic; medieval_outfit_piece_german_hre_female_merchant_fitted_gown; medieval_outfit_piece_german_hre_female_peasant_fitted_work_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_alpine_felt_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_male_artisan_alpine_felt_cap; medieval_outfit_piece_german_hre_male_peasant_alpine_felt_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_fine_hood` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_merchant_fine_hood; medieval_outfit_piece_german_hre_female_noble_fine_hood_or_veil`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_merchant_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_artisan_tool_belt; medieval_outfit_piece_german_hre_female_military_arming_belt; medieval_outfit_piece_german_hre_female_peasant_woven_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_arming_jack` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_military_arming_jack_gown; medieval_outfit_piece_german_hre_male_military_arming_jack; medieval_outfit_piece_german_hre_female_military_arming_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_town_shoes` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_artisan_leather_shoes; medieval_outfit_piece_german_hre_female_merchant_polished_shoes; medieval_outfit_piece_german_hre_female_military_town_crossbow_militia_hook`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_winter_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_female_military_boots; medieval_outfit_piece_german_hre_female_peasant_winter_cloak; medieval_outfit_piece_german_hre_male_military_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_embroidered_church_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_male_religious_church_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_german_hre_court_hat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_german_hre_male_merchant_town_hat; medieval_outfit_piece_german_hre_male_noble_court_hat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_german_hre_arming_sword` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_german_hre_beaked_war_hammer` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_german_hre_war_hammer`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_german_hre_town_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_german_hre_reinforced_heater_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_german_hre`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_german_hre_mail_hauberk` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_german_hre_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_german_hre_plate_reinforced_fittings` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_german_hre_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_german_hre_town_crossbow` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_common_crossbow`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_german_hre_guild_armour_stand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_german_hre_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_german_hre_rye_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_sausage_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_beer_mug` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_cabbage_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_cheese_board` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_feast_roast` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_german_hre_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_german_hre_market_ration`
- `medieval_food_german_hre_spiced_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_german_hre_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_german_hre_guild_mark_register` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_german_hre_court_seal_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_german_hre_town_account_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_german_hre_trade_weight_note` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_german_hre_monastery_codex` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_german_hre_tax_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_german_hre_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_german_hre_guild_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_german_hre_beer_hall_bench` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_plank_bench`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_german_hre_wall_sconce` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_candle_stand`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_german_hre_reliquary_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_reliquary_box`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_german_hre_locksmith_counter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_market_counter`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Iberian Christian (`iberian_christian`)

#### Clothing

- `medieval_clothing_iberian_christian_saya` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_peasant_simple_saya_gown; medieval_outfit_piece_iberian_christian_male_noble_silk_trimmed_saya; medieval_outfit_piece_iberian_christian_male_peasant_simple_saya`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_pellote` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_merchant_pellote_overgown; medieval_outfit_piece_iberian_christian_female_noble_silk_pellote; medieval_outfit_piece_iberian_christian_male_merchant_pellote_over_tunic`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_manto` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_merchant_lined_manto; medieval_outfit_piece_iberian_christian_female_noble_court_manto; medieval_outfit_piece_iberian_christian_female_peasant_manto`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_toca_head_veil` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_peasant_toca_head_veil; medieval_outfit_piece_iberian_christian_female_military_head_veil_under_cap; medieval_outfit_piece_iberian_christian_female_noble_fine_toca_and_veil`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_frontier_riding_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_military_frontier_riding_cloak; medieval_outfit_piece_iberian_christian_male_military_frontier_riding_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_leather_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_military_riding_boots; medieval_outfit_piece_iberian_christian_male_military_riding_boots; medieval_outfit_piece_iberian_christian_male_noble_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_narrow_sleeved_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_male_artisan_narrow_sleeved_tunic; medieval_outfit_piece_iberian_christian_female_artisan_narrow_sleeved_work_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_knightly_surcoat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_military_knightly_surcoat; medieval_outfit_piece_iberian_christian_male_military_knightly_surcoat`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_noble_silk_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_artisan_leather_belt; medieval_outfit_piece_iberian_christian_female_military_weapon_belt; medieval_outfit_piece_iberian_christian_female_peasant_woven_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_pilgrim_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_religious_pilgrim_cloak; medieval_outfit_piece_iberian_christian_male_religious_pilgrim_cloak; medieval_outfit_piece_iberian_christian_female_military_cloak_clasp`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_court_gown` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_artisan_narrow_sleeved_work_gown; medieval_outfit_piece_iberian_christian_female_artisan_wool_gown; medieval_outfit_piece_iberian_christian_female_merchant_fitted_gown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_iberian_christian_arming_coat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_iberian_christian_female_military_arming_shift; medieval_outfit_piece_iberian_christian_female_military_quilted_coat_gown; medieval_outfit_piece_iberian_christian_male_military_arming_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_iberian_christian_war_sword` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_iberian_christian_frontier_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_iberian_christian_javelin` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_iberian_christian_almond_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_iberian_christian`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_iberian_christian_mail_shirt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_iberian_christian_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_iberian_christian_quilted_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_iberian_christian_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_iberian_christian_cavalry_spurs` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_iberian_christian_frontier_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_iberian_christian_wheat_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_olive_relish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_chickpea_pottage` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_salted_meat_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_honey_pastry` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_feast_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_iberian_christian_market_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_iberian_christian_frontier_charter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_iberian_christian_seal_cord_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_iberian_christian_military_order_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_iberian_christian_toll_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_iberian_christian_notary_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_iberian_christian_chapel_booklet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_iberian_christian_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_iberian_christian_frontier_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_iberian_christian_olive_oil_jug` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_iberian_christian_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_iberian_christian_pilgrim_staff_rack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_wall_shelf`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_iberian_christian_shrine_badge` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_iberian_christian_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_iberian_christian_courtyard_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### al-Andalus/Maghreb (`andalusi`)

#### Clothing

- `medieval_clothing_andalusi_linen_qamis` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_artisan_work_qamis; medieval_outfit_piece_andalusi_male_merchant_fine_qamis; medieval_outfit_piece_andalusi_male_military_arming_qamis`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_wool_sirwal` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_merchant_sirwal_or_under_robe; medieval_outfit_piece_andalusi_female_military_sirwal_or_split_skirt; medieval_outfit_piece_andalusi_female_peasant_loose_sirwal_or_skirt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_burnous_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_merchant_lined_cloak; medieval_outfit_piece_andalusi_female_military_riding_burnous; medieval_outfit_piece_andalusi_female_peasant_light_wrap_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_wrapped_turban` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_artisan_wrapped_turban; medieval_outfit_piece_andalusi_male_religious_wrapped_turban; medieval_outfit_piece_andalusi_male_merchant_turban`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_veiled_headcloth` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_artisan_veiled_headcloth; medieval_outfit_piece_andalusi_female_peasant_veiled_headcloth; medieval_outfit_piece_andalusi_female_military_veiled_headwrap_under_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_qaba_caftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_merchant_qaba_caftan`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_tiraz_banded_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_merchant_tiraz_banded_robe; medieval_outfit_piece_andalusi_male_noble_tiraz_banded_court_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_soft_slippers` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_merchant_soft_slippers; medieval_outfit_piece_andalusi_female_noble_soft_slippers; medieval_outfit_piece_andalusi_male_merchant_soft_slippers`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_merchant_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_merchant_merchant_sash; medieval_outfit_piece_andalusi_female_artisan_sash; medieval_outfit_piece_andalusi_female_merchant_decorated_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_scholar_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_religious_scholar_robe; medieval_outfit_piece_andalusi_female_artisan_work_robe; medieval_outfit_piece_andalusi_female_merchant_sirwal_or_under_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_riding_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_female_merchant_lined_cloak; medieval_outfit_piece_andalusi_female_military_quilted_riding_coat; medieval_outfit_piece_andalusi_female_military_riding_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_andalusi_embroidered_belt_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_andalusi_male_peasant_belt_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_andalusi_saif` - Status: `ImplementedItem`; Implementation: `medieval_weapon_andalusi_saif`
- `medieval_weapon_andalusi_light_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_andalusi_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_andalusi_hide_adarga` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_andalusi`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_andalusi_quilted_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_andalusi_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_andalusi_turban_helm` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_andalusi_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_andalusi_horseman_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_andalusi_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_andalusi_leather_bracers` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_andalusi_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_lentil_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_date_sweet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_yogurt_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_spiced_meat_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_syrup_drink` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_oil_herb_relish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_andalusi_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_andalusi_market_ration`

#### Writing and Administration

- `medieval_writing_andalusi_paper_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_andalusi_office_seal_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_andalusi_scholar_notebook` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_andalusi_market_ledger_leaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_andalusi_quran_stand_note` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_andalusi_tax_order` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_andalusi_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_andalusi_glazed_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_andalusi_brass_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_andalusi_writing_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_andalusi_carved_screen_panel` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_household_andalusi_perfume_oil_flask` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_andalusi_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Byzantine (`byzantine`)

#### Clothing

- `medieval_clothing_byzantine_silk_dalmatic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_male_noble_silk_dalmatic`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_skaramangion_riding_coat` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_byzantine_sagion_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_male_peasant_short_sagion_cloak; medieval_outfit_piece_byzantine_female_religious_cowl_cloak; medieval_outfit_piece_byzantine_male_artisan_short_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_court_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_artisan_tool_belt; medieval_outfit_piece_byzantine_female_military_military_belt; medieval_outfit_piece_byzantine_female_peasant_woven_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_formal_silk_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_merchant_formal_gown; medieval_outfit_piece_byzantine_female_military_military_padded_robe; medieval_outfit_piece_byzantine_female_noble_embroidered_silk_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_veil_headcloth` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_artisan_head_veil; medieval_outfit_piece_byzantine_female_merchant_veil; medieval_outfit_piece_byzantine_female_military_head_veil_under_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_embroidered_cuffs` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_noble_embroidered_silk_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_icon_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_noble_icon_pouch; medieval_outfit_piece_byzantine_female_artisan_icon_token; medieval_outfit_piece_byzantine_female_artisan_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_military_padded_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_male_military_military_padded_tunic; medieval_outfit_piece_byzantine_female_military_military_padded_robe; medieval_outfit_piece_byzantine_female_peasant_plain_long_tunic`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_lamellar_under_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_male_artisan_work_under_robe; medieval_outfit_piece_byzantine_male_merchant_fine_under_robe; medieval_outfit_piece_byzantine_male_military_arming_under_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_soft_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_military_boots; medieval_outfit_piece_byzantine_male_military_boots; medieval_outfit_piece_byzantine_male_noble_soft_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_byzantine_monastic_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_byzantine_female_military_military_padded_robe; medieval_outfit_piece_byzantine_female_noble_embroidered_silk_robe; medieval_outfit_piece_byzantine_female_religious_monastic_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_byzantine_paramerion` - Status: `ImplementedItem`; Implementation: `medieval_weapon_byzantine_paramerion`
- `medieval_weapon_byzantine_guard_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_byzantine_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_byzantine_painted_oval_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_byzantine`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_byzantine_lamellar_corselet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_byzantine_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_byzantine_mail_coif` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_byzantine_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_byzantine_military_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_byzantine_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_byzantine_icon_marked_banner` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_byzantine_war_banner`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_byzantine_wheat_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_olive_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_wine_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_fish_sauce_relish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_cheese_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_feast_fish_platter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_monastery_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_byzantine_spiced_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_byzantine_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_byzantine_chrysobull_copy` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_byzantine_icon_label_tablet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_byzantine_monastery_codex` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_byzantine_sealed_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_byzantine_tax_register` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_byzantine_court_order_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_byzantine_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_devotional_byzantine_icon_panel` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_icon_pendant`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_byzantine_hanging_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_jewellery_byzantine_enamel_pendant` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_icon_pendant`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_byzantine_silk_altar_cloth` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_byzantine_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_byzantine_bronze_censer` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_byzantine_pilgrim_token`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Abbasid/Persianate (`abbasid`)

#### Clothing

- `medieval_clothing_abbasid_linen_qamis` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_male_artisan_work_qamis; medieval_outfit_piece_abbasid_male_merchant_fine_qamis; medieval_outfit_piece_abbasid_male_military_arming_qamis`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_qaba_caftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_male_merchant_qaba_caftan; medieval_outfit_piece_abbasid_male_artisan_work_caftan`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_sirwal` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_military_sirwal_or_split_skirt; medieval_outfit_piece_abbasid_female_peasant_loose_sirwal_or_skirt; medieval_outfit_piece_abbasid_male_artisan_sirwal`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_wrapped_turban` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_male_artisan_turban; medieval_outfit_piece_abbasid_male_merchant_turban; medieval_outfit_piece_abbasid_male_military_turban_helm_liner`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_scholar_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_religious_scholar_or_devotional_robe; medieval_outfit_piece_abbasid_male_religious_scholar_robe; medieval_outfit_piece_abbasid_female_artisan_work_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_belted_court_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_male_noble_belted_court_robe; medieval_outfit_piece_abbasid_female_artisan_work_robe; medieval_outfit_piece_abbasid_female_merchant_fine_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_wrapped_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_artisan_sash; medieval_outfit_piece_abbasid_female_merchant_decorated_sash; medieval_outfit_piece_abbasid_female_military_weapon_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_soft_slippers` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_merchant_soft_slippers; medieval_outfit_piece_abbasid_female_noble_soft_slippers; medieval_outfit_piece_abbasid_male_merchant_soft_slippers`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_military_riding_coat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_military_lamellar_sleeved_riding_coat; medieval_outfit_piece_abbasid_female_military_riding_boots; medieval_outfit_piece_abbasid_female_military_riding_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_fine_headcloth` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_abbasid_merchant_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_artisan_work_robe; medieval_outfit_piece_abbasid_female_merchant_fine_robe; medieval_outfit_piece_abbasid_female_merchant_under_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_abbasid_book_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_abbasid_female_religious_book_pouch; medieval_outfit_piece_abbasid_male_religious_book_pouch; medieval_outfit_piece_abbasid_female_artisan_tool_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_abbasid_straight_sword` - Status: `ImplementedItem`; Implementation: `medieval_weapon_abbasid_straight_sword`
- `medieval_weapon_abbasid_guard_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_abbasid_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_abbasid_round_dhal` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_abbasid`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_abbasid_lamellar_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_abbasid_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_abbasid_padded_sleeves` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_abbasid_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_abbasid_horseman_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_abbasid_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_abbasid_guard_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_abbasid_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_abbasid_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_rice_pilaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_sour_milk_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_date_syrup_sweet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_spiced_meat_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_sherbet_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_lentil_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_abbasid_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_abbasid_market_ration`

#### Writing and Administration

- `medieval_writing_abbasid_paper_decree` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_abbasid_scholar_notebook` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_abbasid_chancery_seal_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_abbasid_ink_case` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_abbasid_waqf_record` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_abbasid_market_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_abbasid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_abbasid_brass_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_abbasid_glazed_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_abbasid_writing_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_abbasid_book_stand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_lectern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_abbasid_perfume_flask` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_abbasid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Fatimid Egypt/Ifriqiya (`fatimid`)

#### Clothing

- `medieval_clothing_fatimid_linen_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_merchant_cotton_under_robe; medieval_outfit_piece_fatimid_female_merchant_tiraz_banded_robe; medieval_outfit_piece_fatimid_female_noble_court_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_tiraz_banded_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_male_merchant_tiraz_banded_tunic; medieval_outfit_piece_fatimid_female_merchant_tiraz_banded_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_cotton_wrap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_artisan_cotton_wrap_skirt; medieval_outfit_piece_fatimid_female_peasant_cotton_wrap_skirt; medieval_outfit_piece_fatimid_female_peasant_shoulder_wrap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_wrapped_turban` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_male_artisan_turban; medieval_outfit_piece_fatimid_male_merchant_turban; medieval_outfit_piece_fatimid_male_military_turban_helm_liner`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_court_kaftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_male_noble_court_kaftan`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_veiled_headcloth` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_peasant_veiled_headcloth; medieval_outfit_piece_fatimid_female_military_veiled_headwrap; medieval_outfit_piece_fatimid_male_peasant_headcloth`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_linen_merchant_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_merchant_cotton_under_robe; medieval_outfit_piece_fatimid_female_merchant_tiraz_banded_robe; medieval_outfit_piece_fatimid_female_noble_court_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_light_sandals` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_artisan_sandals; medieval_outfit_piece_fatimid_female_merchant_soft_sandals; medieval_outfit_piece_fatimid_female_peasant_sandals`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_noble_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_artisan_sash; medieval_outfit_piece_fatimid_female_merchant_decorated_sash; medieval_outfit_piece_fatimid_female_noble_silk_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_scribe_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_merchant_cotton_under_robe; medieval_outfit_piece_fatimid_female_merchant_tiraz_banded_robe; medieval_outfit_piece_fatimid_female_noble_court_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_guard_padded_coat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_military_padded_guard_coat; medieval_outfit_piece_fatimid_male_military_padded_coat_with_scale_panels`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_fatimid_devotional_amulet_cord` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_fatimid_female_peasant_amulet_cord; medieval_outfit_piece_fatimid_male_peasant_amulet_cord`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_fatimid_guard_spear` - Status: `ImplementedItem`; Implementation: `medieval_weapon_fatimid_guard_spear`
- `medieval_weapon_fatimid_saif` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_fatimid_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_fatimid_round_hide_dhal` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_fatimid`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_fatimid_padded_coat_scale_panels` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_fatimid_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_fatimid_guard_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_fatimid_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_fatimid_archer_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_fatimid_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_fatimid_palace_guard_belt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_fatimid_sidearm_harness`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_fatimid_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_lentil_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_date_sweet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_oil_relish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_spiced_fish_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_syrup_drink` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_fatimid_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_fatimid_market_ration`
- `medieval_food_fatimid_feast_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_fatimid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_fatimid_tax_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_fatimid_tiraz_workshop_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_fatimid_paper_order` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_fatimid_merchant_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_fatimid_mosque_endowment_note` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_fatimid_seal_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_fatimid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_fatimid_glass_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_fatimid_ivory_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_fatimid_linen_bale` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_trade_sealable_bale`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_fatimid_red_sea_trade_casket` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_devotional_fatimid_prayer_bead_strand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_devotional_wooden_rosary`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Seljuk/Ayyubid/early Mamluk (`seljuk_ayyubid`)

#### Clothing

- `medieval_clothing_seljuk_ayyubid_riding_caftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_male_merchant_riding_caftan; medieval_outfit_piece_seljuk_ayyubid_female_merchant_fine_caftan_robe; medieval_outfit_piece_seljuk_ayyubid_female_military_high_riding_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_quilted_coat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_male_artisan_quilted_coat; medieval_outfit_piece_seljuk_ayyubid_male_military_quilted_riding_coat; medieval_outfit_piece_seljuk_ayyubid_female_artisan_quilted_work_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_sirwal` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_artisan_sirwal_or_skirt; medieval_outfit_piece_seljuk_ayyubid_female_military_sirwal_or_split_skirt; medieval_outfit_piece_seljuk_ayyubid_female_peasant_loose_sirwal_or_skirt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_wrapped_turban` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_male_artisan_turban; medieval_outfit_piece_seljuk_ayyubid_male_merchant_turban; medieval_outfit_piece_seljuk_ayyubid_male_military_turban_helm_liner`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_high_riding_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_military_high_riding_boots; medieval_outfit_piece_seljuk_ayyubid_male_military_high_riding_boots; medieval_outfit_piece_seljuk_ayyubid_male_noble_high_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_bowcase_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_military_bowcase_belt; medieval_outfit_piece_seljuk_ayyubid_male_military_bowcase_belt; medieval_outfit_piece_seljuk_ayyubid_male_noble_belt_plaques`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_lamellar_coat_cover` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_military_lamellar_coat_cover; medieval_outfit_piece_seljuk_ayyubid_male_military_lamellar_coat_cover`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_scholar_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_male_religious_scholar_robe; medieval_outfit_piece_seljuk_ayyubid_female_artisan_quilted_work_robe; medieval_outfit_piece_seljuk_ayyubid_female_merchant_fine_caftan_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_merchant_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_male_merchant_merchant_sash; medieval_outfit_piece_seljuk_ayyubid_female_artisan_sash; medieval_outfit_piece_seljuk_ayyubid_female_merchant_decorated_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_fur_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_military_veiled_headwrap_under_cap; medieval_outfit_piece_seljuk_ayyubid_female_noble_fur_edged_mantle; medieval_outfit_piece_seljuk_ayyubid_male_noble_fur_edged_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_cavalry_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_peasant_felt_cloak; medieval_outfit_piece_seljuk_ayyubid_male_artisan_short_cloak; medieval_outfit_piece_seljuk_ayyubid_male_merchant_lined_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_seljuk_ayyubid_leather_gloves` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_seljuk_ayyubid_female_artisan_gloves; medieval_outfit_piece_seljuk_ayyubid_female_merchant_gloves; medieval_outfit_piece_seljuk_ayyubid_female_military_gloves`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_seljuk_ayyubid_flanged_mace` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_seljuk_ayyubid_curved_sabre` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_seljuk_ayyubid_cavalry_lance` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_seljuk_ayyubid_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_seljuk_ayyubid_round_cavalry_dhal` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_seljuk_ayyubid`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_seljuk_ayyubid_lamellar_riding_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_seljuk_ayyubid_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_seljuk_ayyubid_mail_aventail_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_seljuk_ayyubid_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_seljuk_ayyubid_bowcase_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_seljuk_ayyubid_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_seljuk_ayyubid_flatbread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_yogurt_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_pilaf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_spiced_meat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_dried_fruit_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_sour_milk_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_seljuk_ayyubid_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_seljuk_ayyubid_market_ration`
- `medieval_food_seljuk_ayyubid_feast_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_seljuk_ayyubid_iqta_record` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_seljuk_ayyubid_madrasa_book` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_seljuk_ayyubid_sealed_order` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_seljuk_ayyubid_cavalry_muster_roll` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_seljuk_ayyubid_paper_contract` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_seljuk_ayyubid_court_seal_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_seljuk_ayyubid_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_seljuk_ayyubid_riding_saddlebag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_document_satchel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_seljuk_ayyubid_brass_lamp` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_iron_lantern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_seljuk_ayyubid_prayer_rug_prop` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_household_seljuk_ayyubid_inlaid_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_seljuk_ayyubid_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_seljuk_ayyubid_madrasa_book_stand` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_lectern`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Kyivan Rus/Novgorod (`rus_novgorod`)

#### Clothing

- `medieval_clothing_rus_novgorod_rubakha_tunic` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_peasant_rubakha_style_gown; medieval_outfit_piece_rus_novgorod_male_artisan_work_rubakha; medieval_outfit_piece_rus_novgorod_male_merchant_fine_rubakha`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_fur_edged_kaftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_male_merchant_fur_edged_kaftan; medieval_outfit_piece_rus_novgorod_female_merchant_fur_edged_overgown; medieval_outfit_piece_rus_novgorod_female_military_fur_edged_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_porty_trousers` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_male_artisan_porty_trousers; medieval_outfit_piece_rus_novgorod_male_merchant_porty_trousers; medieval_outfit_piece_rus_novgorod_male_peasant_porty_trousers`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_onuchi_footwraps` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_peasant_onuchi_footwraps; medieval_outfit_piece_rus_novgorod_male_peasant_onuchi_footwraps; medieval_outfit_piece_rus_novgorod_female_artisan_onuchi`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_fur_hat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_male_merchant_fur_hat; medieval_outfit_piece_rus_novgorod_male_noble_fur_hat; medieval_outfit_piece_rus_novgorod_female_merchant_fur_edged_overgown`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_leather_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_artisan_boots; medieval_outfit_piece_rus_novgorod_female_merchant_boots; medieval_outfit_piece_rus_novgorod_female_military_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_cloak_with_pin` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_merchant_lined_cloak; medieval_outfit_piece_rus_novgorod_female_military_fur_edged_cloak; medieval_outfit_piece_rus_novgorod_female_peasant_rough_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_birchbark_document_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_male_merchant_birchbark_document_pouch; medieval_outfit_piece_rus_novgorod_female_merchant_document_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_orthodox_pendant_cord` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_noble_orthodox_pendant; medieval_outfit_piece_rus_novgorod_male_noble_orthodox_pendant`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_warrior_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_rus_novgorod_female_military_warrior_belt; medieval_outfit_piece_rus_novgorod_male_military_warrior_belt; medieval_outfit_piece_rus_novgorod_female_artisan_tool_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_rus_novgorod_river_trader_coat` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_rus_novgorod_wool_mittens` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`

#### Military

- `medieval_weapon_rus_novgorod_war_axe` - Status: `ImplementedItem`; Implementation: `medieval_weapon_rus_novgorod_war_axe`
- `medieval_weapon_rus_novgorod_socketed_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_rus_novgorod_broad_sword` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_rus_novgorod_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_rus_novgorod_tall_oval_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_rus_novgorod`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_rus_novgorod_mail_shirt` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_rus_novgorod_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_rus_novgorod_fur_trimmed_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_rus_novgorod_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_rus_novgorod_river_guard_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_rus_novgorod_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_rus_novgorod_rye_bread` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_mushroom_fish_stew` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_honey_drink_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_smoked_fish_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_curd_cheese_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_river_trader_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_pirog_pastry` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_rus_novgorod_pickle_crock` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_meal_platter`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_rus_novgorod_birchbark_letter` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_rus_novgorod_princely_seal_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_rus_novgorod_river_trade_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_rus_novgorod_orthodox_prayer_slip` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_rus_novgorod_wax_tablet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_rus_novgorod_fur_tax_record` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_rus_novgorod_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_rus_novgorod_icon_shelf` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_wall_shelf`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_rus_novgorod_birchbark_letter_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_rus_novgorod_fur_lined_chest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_rus_novgorod_honey_drink_crock` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_rus_novgorod_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_trade_rus_novgorod_river_balance_case` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_trade_balance_scale`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.


### Steppe Turkic/Cuman/Mongol-adjacent (`steppe_turkic`)

#### Clothing

- `medieval_clothing_steppe_turkic_felt_riding_caftan` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_male_merchant_felt_riding_caftan; medieval_outfit_piece_steppe_turkic_female_merchant_long_riding_caftan; medieval_outfit_piece_steppe_turkic_male_noble_embroidered_riding_caftan`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_tied_riding_coat` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_peasant_tied_long_riding_coat; medieval_outfit_piece_steppe_turkic_male_peasant_tied_riding_coat; medieval_outfit_piece_steppe_turkic_female_military_lamellar_riding_coat_cover`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_riding_trousers` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_merchant_riding_trousers_or_skirt; medieval_outfit_piece_steppe_turkic_female_military_riding_trousers; medieval_outfit_piece_steppe_turkic_female_peasant_riding_trousers_or_split_skirt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_high_boots` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_merchant_high_boots; medieval_outfit_piece_steppe_turkic_female_military_high_boots; medieval_outfit_piece_steppe_turkic_female_noble_high_boots`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_fur_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_artisan_fur_cap_headwrap; medieval_outfit_piece_steppe_turkic_female_military_fur_cap_headwrap; medieval_outfit_piece_steppe_turkic_female_peasant_fur_cap_or_headwrap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_bowcase_quiver_belt` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_military_bowcase_and_quiver_belt; medieval_outfit_piece_steppe_turkic_male_military_bowcase_and_quiver_belt`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_felt_cloak` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_military_felt_war_cloak; medieval_outfit_piece_steppe_turkic_female_peasant_felt_cloak; medieval_outfit_piece_steppe_turkic_female_religious_felt_cloak`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_artisan_tool_sash; medieval_outfit_piece_steppe_turkic_female_merchant_decorated_sash; medieval_outfit_piece_steppe_turkic_female_noble_silk_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_lamellar_coat_cover` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_military_lamellar_riding_coat_cover; medieval_outfit_piece_steppe_turkic_male_military_lamellar_coat_cover`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_horseman_gloves` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_military_horseman_gloves; medieval_outfit_piece_steppe_turkic_male_military_horseman_gloves; medieval_outfit_piece_steppe_turkic_female_artisan_gloves`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_steppe_turkic_winter_mittens` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_steppe_turkic_travel_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_steppe_turkic_female_peasant_travel_pouch; medieval_outfit_piece_steppe_turkic_male_peasant_travel_pouch; medieval_outfit_piece_steppe_turkic_female_artisan_tool_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_steppe_turkic_curved_sabre` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_steppe_turkic_sabre`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_steppe_turkic_composite_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_steppe_turkic_lance` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_steppe_turkic_mace` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_shield_steppe_turkic_compact_hide_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_steppe_turkic`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_steppe_turkic_lamellar_coat` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_steppe_turkic_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_steppe_turkic_fur_cap_helmet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_steppe_turkic_helmet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_steppe_turkic_bowcase_quiver` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_steppe_turkic_arrow_quiver`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Food and Beverage

- `medieval_food_steppe_turkic_millet_gruel` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_dried_curds` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_kumis_skin` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_dried_meat_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_riding_ration` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_boiled_meat_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_felt_wrapped_travel_food` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_market_ration`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_steppe_turkic_mare_milk_vessel` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_steppe_turkic_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Writing and Administration

- `medieval_writing_steppe_turkic_paiza_tag_prop` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_steppe_turkic_sealed_leather_pouch` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_steppe_turkic_herd_tally` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_tally_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_steppe_turkic_tribute_strip` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_steppe_turkic_bowcase_ownership_tag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_steppe_turkic_messenger_packet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_steppe_turkic_seal_tag_packet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_steppe_turkic_felt_tent_panel` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_household_steppe_turkic_saddlebag` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_document_satchel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_steppe_turkic_kumis_skin` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_household_steppe_turkic_bowcase_rack` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_steppe_turkic_felt_carpet` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`


### Song China (`song_china`)

#### Clothing

- `medieval_clothing_song_china_cross_collar_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_merchant_cross_collar_merchant_robe; medieval_outfit_piece_song_china_female_noble_elegant_cross_collar_robe; medieval_outfit_piece_song_china_female_peasant_cross_collar_work_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_scholar_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_male_noble_scholar_robe; medieval_outfit_piece_song_china_male_religious_scholar_monastic_robe; medieval_outfit_piece_song_china_female_artisan_work_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_official_cap` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_male_noble_official_cap; medieval_outfit_piece_song_china_female_merchant_headcloth_or_cap; medieval_outfit_piece_song_china_female_military_headcloth_under_cap`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_cloth_shoes` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_artisan_cloth_shoes; medieval_outfit_piece_song_china_female_merchant_cloth_shoes; medieval_outfit_piece_song_china_female_peasant_cloth_shoes`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_padded_winter_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_noble_padded_winter_robe; medieval_outfit_piece_song_china_male_noble_padded_winter_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_silk_sash` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_artisan_tool_sash; medieval_outfit_piece_song_china_female_merchant_silk_sash; medieval_outfit_piece_song_china_female_military_weapon_sash`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_tea_house_robe` - Status: `Deferred`; Reason: Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.; Craft exemption: `Deferred`
- `medieval_clothing_song_china_merchant_robe` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_merchant_cross_collar_merchant_robe; medieval_outfit_piece_song_china_male_merchant_merchant_robe; medieval_outfit_piece_song_china_female_artisan_work_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_servant_jacket` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_male_artisan_work_jacket; medieval_outfit_piece_song_china_male_peasant_short_working_jacket`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_narrow_trousers` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_male_artisan_narrow_trousers; medieval_outfit_piece_song_china_male_peasant_narrow_trousers; medieval_outfit_piece_song_china_female_artisan_skirt_or_trousers`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_writing_sleeve_pouch` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_artisan_sleeve_pouch; medieval_outfit_piece_song_china_female_merchant_account_sleeve_pouch; medieval_outfit_piece_song_china_male_artisan_sleeve_pouch`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.
- `medieval_clothing_song_china_military_padded_vest` - Status: `CoveredByOutfitPiece`; Implementation: `medieval_outfit_piece_song_china_female_military_padded_military_vest; medieval_outfit_piece_song_china_male_military_padded_military_vest; medieval_outfit_piece_song_china_female_noble_padded_winter_robe`; Reason: Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.

#### Military

- `medieval_weapon_song_china_single_edged_dao` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_song_china_dao`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_song_china_qiang_spear` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_weapon_song_china_military_crossbow` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_weapon_common_crossbow`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_shield_song_china_lacquered_rattan_shield` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_shield_song_china`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_song_china_lamellar_vest` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_song_china_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_military_song_china_padded_military_robe` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_military_song_china_armour`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_weapon_song_china_militia_bow` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`
- `medieval_military_song_china_guard_baton` - Status: `Deferred`; Reason: Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.; Craft exemption: `Deferred`

#### Food and Beverage

- `medieval_food_song_china_rice_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_wheat_noodle_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_staple_bread`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_tea_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_pickled_greens_jar` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_pottage_bowl`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_steamed_bun` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_fish_dish` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_scholar_snack_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_feast_dish`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_food_song_china_market_ration` - Status: `ImplementedItem`; Implementation: `medieval_food_song_china_market_ration`

#### Writing and Administration

- `medieval_writing_song_china_paper_register` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_song_china_printed_notice` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_record_tablet`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_song_china_scholar_notebook` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_song_china_official_chop_document` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_song_china_tea_house_account_slip` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_writing_song_china_examination_essay_booklet` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_writing_song_china_office_bundle`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.

#### Household and Devotional

- `medieval_household_song_china_tea_cup` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_song_china_lacquer_writing_box` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_household_boarded_chest`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_song_china_porcelain_bowl` - Status: `AliasOfExistingStableReference`; Implementation: `medieval_food_song_china_drinking_vessel`; Reason: Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.
- `medieval_household_song_china_scholar_brush_rest` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
- `medieval_household_song_china_printed_notice_board` - Status: `Deferred`; Reason: Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.; Craft exemption: `Deferred`
