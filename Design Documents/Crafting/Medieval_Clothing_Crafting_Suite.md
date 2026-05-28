# Medieval Clothing Crafting Suite

The medieval clothing suite expands the antiquity model by adding status/role as a first-class axis. The shared textile chain remains culture-neutral; final garment assembly is knowledge-gated by culture and records status in tags and builder notes.

## Knowledge Gates

Each culture has a `Medieval Clothing Pattern {culture}` knowledge gate. Visible craft names use neutral regional-pattern numbering, for example `sew work tunic regional pattern 01`, while the knowledge gate and builder notes identify the culture.

Status roles are:

- `peasant`: work tunic.
- `artisan`: apron-front tunic.
- `merchant`: lined gown.
- `noble`: silk surcoat.
- `clergy`: clerical robe.
- `military`: padded arming coat.

The bodywear item is no longer the whole outfit. Each culture/status pair also receives a practical wardrobe around it:

- Underlayers: shirts, chemises, undertunics, and arming shirts.
- Headwear: caps, hats, coifs, court caps, and padded arming caps.
- Hoods and cowls: rough hoods, work hoods, lined hoods, fur-lined hoods, monastic cowls, and arming hoods.
- Outerwear: cloaks, hooded work cloaks, travel mantles, fur-lined mantles, cowl-cloaks, and military weather cloaks.
- Legwear: leggings, hose, court hose, chausses, and padded arming chausses.
- Handwear: mittens, work gloves, lined gloves, fine gloves, and arming gloves.
- Sockwear: footwraps, wool socks, fine socks, silk socks, plain socks, and boot socks.
- Footwear: turnshoes, work boots, town shoes, court shoes, sandals, and riding boots.
- Waistwear: rope belts, tool belts, purse belts, fine girdles, cord belts, and arming belts.
- Worn containers: belt pouches, tool pouches, belt purses, alms purses, book pouches, and field pouches.

## Stable Reference Matrix

For every culture key in the medieval audit, the seeder creates the following bodywear:

- `medieval_clothing_{culture}_peasant_work_tunic`
- `medieval_clothing_{culture}_artisan_apron_tunic`
- `medieval_clothing_{culture}_merchant_lined_gown`
- `medieval_clothing_{culture}_noble_silk_surcoat`
- `medieval_clothing_{culture}_clergy_clerical_robe`
- `medieval_clothing_{culture}_military_arming_coat`

For every culture/status pair, it also creates the wardrobe pattern:

- `medieval_clothing_{culture}_{status}_linen_shirt`, `work_shirt`, `fine_linen_shirt`, `silk_lined_chemise`, `linen_undertunic`, or `arming_shirt`.
- `medieval_clothing_{culture}_{status}_wool_cap`, `work_cap`, `lined_hat`, `court_cap`, `plain_coif`, or `padded_arming_cap`.
- `medieval_clothing_{culture}_{status}_rough_hood`, `work_hood`, `lined_hood`, `fur_lined_hood`, `monastic_cowl`, or `arming_hood`.
- `medieval_clothing_{culture}_{status}_rough_cloak`, `hooded_work_cloak`, `travel_mantle`, `fur_lined_mantle`, `wool_cowl_cloak`, or `weather_cloak`.
- `medieval_clothing_{culture}_{status}_wool_leggings`, `work_hose`, `fitted_hose`, `fine_hose`, `plain_chausses`, or `padded_chausses`.
- `medieval_clothing_{culture}_{status}_wool_mittens`, `leather_work_gloves`, `lined_gloves`, `fine_gloves`, `plain_mittens`, or `arming_gloves`.
- `medieval_clothing_{culture}_{status}_wool_footwraps`, `wool_socks`, `fine_socks`, `silk_socks`, `plain_socks`, or `boot_socks`.
- `medieval_clothing_{culture}_{status}_rough_turnshoes`, `work_boots`, `town_shoes`, `fine_court_shoes`, `plain_sandals`, or `riding_boots`.
- `medieval_clothing_{culture}_{status}_rope_belt`, `tool_belt`, `purse_belt`, `fine_girdle`, `cord_belt`, or `arming_belt`.
- `medieval_clothing_{culture}_{status}_belt_pouch`, `tool_pouch`, `belt_purse`, `alms_purse`, `book_pouch`, or `field_pouch`.

The v1 culture keys are `early_anglo_saxon`, `anglo_danish`, `norse`, `norman`, `high_british`, `gaelic`, `carolingian`, `capetian`, `german_hre`, `iberian_christian`, `andalusi`, `byzantine`, `abbasid`, `fatimid`, `seljuk_ayyubid`, `rus_novgorod`, `steppe_turkic`, and `song_china`.

## Craft Inputs And Tools

`SeedMedievalProductionChainCrafts()` now adds medieval textile finishing before final garment assembly. The clothing suite still shares the antiquity fibre/yarn/cloth base, but status and silhouette determine which medieval stock is consumed:

- `Garment Cloth` commodity stock.
- `Broadcloth Stock` for merchant and clerical respectability.
- `Silk Brocade Panel` and `Embroidered Trim Stock` for noble/court and prosperous town clothing.
- `Tablet-Woven Band Stock` for culture cues that call out bands, braid, or tablet-woven edging.
- `Quilted Armour Padding` for military arming shirts, arming caps, arming hoods, arming coats, and padded chausses.
- `Turnshoe Upper Stock` for shoes, boots, sandals, and turnshoes.
- `Spun Yarn` commodity stock.
- `Drop Spindle`, `Sewing Needle`, and `Shears` `TagTool` requirements backed by `historic_drop_spindle`, `historic_sewing_needle`, and `historic_textile_shears`.
- Leather wardrobe items consume `Prepared Leather Panel` and also require `Awl Punch`.

The backed medieval production tools include `medieval_household_fulling_stocks`, `medieval_household_teasel_frame`, `medieval_household_napping_shears`, `medieval_household_cloth_tenter_frame`, `medieval_household_embroidery_frame`, `medieval_household_tablet_weaving_cards`, and `medieval_household_turnshoe_last`.

The visible craft text stays culture-neutral. Culture, status, and role are enforced through the knowledge name, `Eras / Medieval / Cultures / ...` tags, `Eras / Medieval / Status Roles / ...` tags, and builder notes.
