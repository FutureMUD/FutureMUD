# FutureMUD Renaissance Household, Urban, Trade, Containers, and Liquid Containers Design Reference

## Status and scope

First-pass implementation catalogue for Renaissance household, urban, merchant, dockside, personal, furniture, domestic, and liquid-container goods, approximately 1400-1600 CE. It contains **400 unique proposed prototypes** in twenty complete functional packages: 100 trade containers, 100 furniture containers/surfaces, 80 personal containers, and 120 domestic/liquid-service containers.

This pass supplies stable references, short descriptions, exact primary materials, size, quality, empty weight, farthing cost, exact component sets, exact tag profiles, date/culture/institution admission, and shared-stock reuse. Nouns/keywords, long and full descriptions, skins, crafts, shops, room packages, and C# calls are deferred.

## Stable-reference and row syntax

A catalogue row is `category|slug|sdesc|material|size/quality|empty-g/cost-f|component-code|tag-codes`. The stable reference is `renaissance_<category-name>_<package-prefix>_<slug>`, with `T=trade`, `F=furniture`, `P=personal`, and `D=domestic`. Size codes are `T=Tiny`, `VS=VerySmall`, `S=Small`, `N=Normal`, `L=Large`, `VL=VeryLarge`, `H=Huge`; quality codes are `P=Poor`, `S=Standard`, `G=Good`, `E=Excellent`. Every row inherits `Era / Renaissance Era` and the exact culture tag in its package manifest.

## Shared pre-industrial reuse boundary

The branch reuses **all 147 non-regional `preindustrial_trade_*` aliases** rather than cloning them. The exact source-to-alias manifest remains authoritative in `PreIndustrial_Item_Seeder_Alias_Catalogue.md`; this catalogue deliberately does not duplicate that generated table. Reuse covers generic sacks, bales, bundles, crates, packing boxes, dry barrels/casks/tubs/jars, merchant/customs/guild/tax/warehouse security containers, sample/measure boxes and trays, and ordinary craft/shop stock. A Renaissance row is justified only by distinct form, capacity, component, installation, institution, material behaviour, or contact/export construction; decoration, terminology, marks, script, heraldry, glaze/lacquer pattern, and ordinary status variation are skins.

The named shared global-trade packages also remain dependencies and require explicit date/culture/trade admission:

```text
preindustrial_trade_tea_chest  preindustrial_trade_coffee_sack  preindustrial_trade_cacao_sack  preindustrial_trade_tobacco_bale
preindustrial_trade_sugar_hogshead  preindustrial_trade_indigo_cake_box  preindustrial_trade_porcelain_packing_crate  preindustrial_trade_glass_bottle_crate
preindustrial_trade_silk_bale  preindustrial_trade_cotton_bale  preindustrial_trade_spice_chest
```

## Component-set codes

Portable rows include `Holdable`; fixed furniture does not. Each set contains exactly one dry-container, locking-container, liquid-container, or till provider.

```text
CP01=Destroyable_Furniture;Container_Armoire
CP02=Destroyable_Furniture;Container_Blanket_Box
CP03=Destroyable_Furniture;Container_Blanket_Box;Bench_Triple
CP04=Destroyable_Furniture;Container_Counter
CP05=Destroyable_Furniture;Container_Cupboard
CP06=Destroyable_Furniture;Container_Display_Shelves
CP07=Destroyable_Furniture;Container_Document_Bookcase_Shelves
CP08=Destroyable_Furniture;Container_Dresser
CP09=Destroyable_Furniture;Container_Glass_Cabinet
CP10=Destroyable_Furniture;Container_Large_Cabinet
CP11=Destroyable_Furniture;Container_Large_Table;Table_Ten
CP12=Destroyable_Furniture;Container_Open_Bin
CP13=Destroyable_Furniture;Container_Sideboard
CP14=Destroyable_Furniture;Container_Small_Cabinet
CP15=Destroyable_Furniture;Container_Small_Table;Table_Four
CP16=Destroyable_Furniture;Container_Table;Table_Six
CP17=Destroyable_Furniture;Container_Wardrobe
CP18=Destroyable_Furniture;Container_Wide_Shelves
CP19=Destroyable_Furniture;Container_Writing_Desk_Drawers;Table_Four
CP20=Destroyable_Furniture;LContainer_PreIndustrial_Vat_125L
CP21=Destroyable_Furniture;LContainer_PreIndustrial_Vat_500L
CP22=Holdable;Destroyable_Furniture;Container_Archive_Chest
CP23=Holdable;Destroyable_Furniture;Container_Archive_Chest;Sealable_Container_Wax
CP24=Holdable;Destroyable_Furniture;Container_Trunk
CP25=Holdable;Destroyable_Furniture;LockingContainer_Footlocker
CP26=Holdable;Destroyable_Furniture;LockingContainer_SafeChest
CP27=Holdable;Destroyable_Glassware;Container_Plate
CP28=Holdable;Destroyable_Glassware;Container_PreIndustrial_CompartmentBox
CP29=Holdable;Destroyable_Glassware;LContainer_DrinkingGlass
CP30=Holdable;Destroyable_Glassware;LContainer_Flask
CP31=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Basin_5L
CP32=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Bowl_750ml
CP33=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Cup_150ml
CP34=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Ewer_2L
CP35=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Pitcher_4L
CP36=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_Pot_12L
CP37=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_StorageJar_12L
CP38=Holdable;Destroyable_Glassware;LContainer_PreIndustrial_StorageJar_30L
CP39=Holdable;Destroyable_Glassware;LContainer_SmallWineGlass
CP40=Holdable;Destroyable_Glassware;LContainer_Stein
CP41=Holdable;Destroyable_Glassware;LContainer_UKPint
CP42=Holdable;Destroyable_Glassware;LContainer_WineBottle
CP43=Holdable;Destroyable_HeavyMetal;Container_Plate
CP44=Holdable;Destroyable_HeavyMetal;Container_PreIndustrial_CompartmentBox
CP45=Holdable;Destroyable_HeavyMetal;Container_Seal_Box
CP46=Holdable;Destroyable_HeavyMetal;Container_Seal_Box;Beltable
CP47=Holdable;Destroyable_HeavyMetal;Container_Tray
CP48=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Basin_5L
CP49=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Bowl_750ml
CP50=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Cup_150ml
CP51=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Ewer_2L
CP52=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Pitcher_4L
CP53=Holdable;Destroyable_HeavyMetal;LContainer_PreIndustrial_Pot_12L
CP54=Holdable;Destroyable_HeavyMetal;LContainer_Stein
CP55=Holdable;Destroyable_HeavyMetal;LockingContainer_SafeChest
CP56=Holdable;Destroyable_Misc;CashRegister_PreIndustrial_TillChest
CP57=Holdable;Destroyable_Misc;Container_Archive_Box
CP58=Holdable;Destroyable_Misc;Container_Archive_Box;Sealable_Container_Wax
CP59=Holdable;Destroyable_Misc;Container_Archive_Box;Wear_Shoulder
CP60=Holdable;Destroyable_Misc;Container_Document_Pouch;Beltable
CP61=Holdable;Destroyable_Misc;Container_Document_Satchel;Wear_Shoulder
CP62=Holdable;Destroyable_Misc;Container_Open_Bin
CP63=Holdable;Destroyable_Misc;Container_Pack;Wear_Backpack
CP64=Holdable;Destroyable_Misc;Container_Plate
CP65=Holdable;Destroyable_Misc;Container_Pouch;Beltable
CP66=Holdable;Destroyable_Misc;Container_PreIndustrial_CompartmentBox
CP67=Holdable;Destroyable_Misc;Container_PreIndustrial_LiddedBasket
CP68=Holdable;Destroyable_Misc;Container_PreIndustrial_LiddedBasket;Wear_Shoulder
CP69=Holdable;Destroyable_Misc;Container_PreIndustrial_LiddedHamper
CP70=Holdable;Destroyable_Misc;Container_Purse;Wear_Waist
CP71=Holdable;Destroyable_Misc;Container_Scroll_Tube
CP72=Holdable;Destroyable_Misc;Container_Scroll_Tube;Wear_Shoulder
CP73=Holdable;Destroyable_Misc;Container_Seal_Box
CP74=Holdable;Destroyable_Misc;Container_Seal_Box;Beltable
CP75=Holdable;Destroyable_Misc;Container_Tote;Wear_Shoulder
CP76=Holdable;Destroyable_Misc;Container_Tray
CP77=Holdable;Destroyable_Misc;LContainer_PreIndustrial_Bowl_750ml
CP78=Holdable;Destroyable_Misc;LContainer_PreIndustrial_Cup_150ml
CP79=Holdable;Destroyable_Misc;LContainer_PreIndustrial_Pitcher_4L
CP80=Holdable;Destroyable_Misc;LContainer_Rundlet
CP81=Holdable;Destroyable_Misc;LContainer_Waterskin
CP82=Holdable;Destroyable_Misc;LContainer_Waterskin;Wear_Shoulder
CP83=Holdable;Destroyable_Misc;LockingContainer_Footlocker
CP84=Holdable;Destroyable_Misc;LockingContainer_Lockbox
CP85=Holdable;Destroyable_Paper;Container_Archive_Box;Wear_Shoulder
CP86=Holdable;Destroyable_Paper;Container_Document_Pouch;Beltable
CP87=Holdable;Destroyable_Paper;Container_Document_Satchel;Wear_Shoulder
CP88=Holdable;Destroyable_Paper;Container_Tray
```

## Tag codes

```text
TR-S=Functions / Container;Market / Household Goods / Simple Wares
TR-N=Functions / Container;Market / Household Goods / Standard Wares
TR-L=Functions / Container;Market / Household Goods / Luxury Wares
FU-S=Functions / Container;Functions / Household Items / Household Furniture;Market / Household Goods / Simple Furniture
FU-N=Functions / Container;Functions / Household Items / Household Furniture;Market / Household Goods / Standard Furniture
FU-L=Functions / Container;Functions / Household Items / Household Furniture;Market / Household Goods / Luxury Furniture
PC-S=Functions / Container;Market / Household Goods / Simple Wares
PC-N=Functions / Container;Market / Household Goods / Standard Wares
PC-L=Functions / Container;Market / Household Goods / Luxury Wares
DW-S=Functions / Container;Functions / Household Items / Household Wares;Market / Household Goods / Simple Wares
DW-N=Functions / Container;Functions / Household Items / Household Wares;Market / Household Goods / Standard Wares
DW-L=Functions / Container;Functions / Household Items / Household Wares;Market / Household Goods / Luxury Wares
OPEN=Functions / Container / Open Container
WATER=Functions / Container / Watertight Container
POROUS=Functions / Container / Porous Container
AIRTIGHT=Functions / Container / Airtight Container
COURT=Institution / Court
RELIGIOUS=Institution / Religious
GUILD=Institution / Guild
MARITIME=Institution / Maritime
PERFORMANCE=Institution / Performance
SERVICE=Institution / Service Household
```

## Requested reusable dependencies

The dependency agent should create the following reusable component prototypes and materials exactly as specified in `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`; no new runtime component type is required.

- Components: `CashRegister_PreIndustrial_TillChest`, `Container_PreIndustrial_CompartmentBox`, `Container_PreIndustrial_LiddedBasket`, `Container_PreIndustrial_LiddedHamper`, `LContainer_PreIndustrial_Basin_5L`, `LContainer_PreIndustrial_Bowl_750ml`, `LContainer_PreIndustrial_Cup_150ml`, `LContainer_PreIndustrial_Ewer_2L`, `LContainer_PreIndustrial_Pitcher_4L`, `LContainer_PreIndustrial_Pot_12L`, `LContainer_PreIndustrial_StorageJar_12L`, `LContainer_PreIndustrial_StorageJar_30L`, `LContainer_PreIndustrial_Vat_125L`, `LContainer_PreIndustrial_Vat_500L`.
- Materials: `birch bark`, `gourd shell`, `mother of pearl`, `papier-mache`.

## Package manifest

| Package | Prefix | Exact culture tag | Date | Admission context |
| --- | --- | --- | --- | --- |
| P01 — Western merchant counting house | `western_counting_house` | `Culture / Renaissance / Shared / Western European Renaissance` | 1450-1600 | merchant counting house, banking room, exchange office, or wholesale shop |
| P02 — Civic, guild, and customs office | `civic_guild` | `Culture / Renaissance / Shared / Central European` | 1450-1600 | guildhall, customs house, civic weigh-house, seal office, or municipal chamber |
| P03 — Warehouse and dockside handling | `warehouse_dock` | `Culture / Renaissance / Shared / Global Maritime` | 1450-1600 | bonded warehouse, quay, dockside counting shed, export store, or river port |
| P04 — Shipboard steward and purser stores | `shipboard_purser` | `Culture / Renaissance / Shared / Global Maritime` | 1450-1600 | ocean-going ship, galley, armed merchantman, naval store, or harbour victualling office |
| P05 — Tavern, alehouse, and inn service | `tavern_inn` | `Culture / Renaissance / Shared / Western European Renaissance` | 1450-1600 | urban tavern, alehouse, roadside inn, guild drinking room, or wine shop |
| P06 — Urban food shop and household kitchen | `urban_kitchen` | `Culture / Renaissance / Shared / Western European Renaissance` | 1450-1600 | urban grocer, bakery, cookshop, prosperous kitchen, dairy stall, or household pantry |
| P07 — Elite Western and Iberian household | `elite_household` | `Culture / Renaissance / Shared / Iberian Atlantic` | 1450-1600 | courtly chamber, wealthy urban palace, embassy household, noble dining room, or bridal household |
| P08 — Apothecary and perfumer shop | `apothecary_perfumer` | `Culture / Renaissance / Shared / Western European Renaissance` | 1450-1600 | apothecary shop, physician household, perfumer stall, convent infirmary, or court dispensary |
| P09 — Ottoman bazaar and urban household | `ottoman_bazaar` | `Culture / Renaissance / Shared / Ottoman Islamicate` | 1450-1600 | bazaar shop, caravanserai office, urban household, guild room, or late-sixteenth-century coffee setting |
| P10 — Persianate and Indo-Persian court household | `persianate_court` | `Culture / Renaissance / Shared / Persianate Indo-Persian` | 1450-1600 | Persianate court chamber, Indo-Persian merchant house, manuscript household, garden pavilion, or embassy |
| P11 — South Asian merchant and service household | `south_asian_merchant` | `Culture / Renaissance / Shared / South Asian` | 1450-1600 | merchant household, temple-service store, coastal trading office, or court kitchen |
| P12 — Ming and Joseon scholar-merchant household | `east_asian_literati` | `Culture / Renaissance / Shared / East Asian Literati` | 1400-1600 | scholar household, merchant office, medicine shop, or literati reception room |
| P13 — Japanese and Ryukyuan merchant household | `japanese_ryukyuan` | `Culture / Renaissance / Shared / Japanese` | 1450-1600 | late Muromachi, Sengoku, Momoyama, or Ryukyuan merchant and service context |
| P14 — Mainland and maritime South-east Asian port household | `southeast_asian_port` | `Culture / Renaissance / Shared / Maritime South-east Asian` | 1450-1600 | maritime sultanate port, mainland court market, temple store, or spice-trade household |
| P15 — Steppe and caravan household | `steppe_caravan` | `Culture / Renaissance / Shared / Steppe and Caravan` | 1450-1600 | caravan camp, mobile court, horse-trader household, or frontier merchant stop |
| P16 — West African court and Atlantic trade household | `west_african_court` | `Culture / Renaissance / Shared / African Court Atlantic` | 1450-1600 | court storehouse, brassworker or merchant household, market, or early Atlantic trade context |
| P17 — Sahelian, Red Sea, and Swahili merchant household | `sahel_redsea_indianocean` | `Culture / Renaissance / Shared / Indian Ocean` | 1450-1600 | trans-Saharan merchant house, Red Sea customs station, manuscript household, or Swahili port store |
| P18 — Mesoamerican market and tribute household | `mesoamerican_market` | `Culture / Renaissance / Shared / Mesoamerican` | 1400-1600 | market, tribute store, elite household, cacao service, or early contact context |
| P19 — Andean household and storehouse | `andean_storehouse` | `Culture / Renaissance / Shared / Andean` | 1400-1600 | household, storehouse, quipu office, chicha service, or early colonial contact setting |
| P20 — Caribbean, North American, and colonial Atlantic contact post | `atlantic_contact` | `Culture / Renaissance / Shared / Colonial Atlantic` | 1450-1600 | mission store, contact post, early colonial port, Caribbean household, or Indigenous exchange setting |

Each package contains exactly 5T / 5F / 4P / 6D. Northern Baltic, Central/Eastern Frontier, and Eastern Orthodox Northern deliberately admit credible rows from the closest packages rather than clone them: chiefly P02-P06 for Northern Baltic, P02/P06/P15 for Central/Eastern Frontier, and P01/P02/P06/P12 for Eastern Orthodox Northern. Local timber, joinery, motifs, script, and terminology remain skins unless behaviour changes.

## Catalogue — 400 proposed prototypes

### P01 — Western merchant counting house

Complete Western counting-house package. Generic merchant strongboxes, money-changing cash boxes, sample caskets, counting trays, document chests, and weight boxes remain shared dependencies. Retained rows add till mechanics, fitted exchange/document capacity, installed counting furniture, or exact service-vessel behaviour.

```text
T|merchant_till_chest|a walnut merchant's till chest|walnut|N/G|17000/326|CP56|TR-L,GUILD
T|exchange_bill_coffer|a wax-sealable exchange-bill coffer|oak|N/G|14000/130|CP23|TR-L,GUILD
T|cloth_sample_case|a fitted cloth-sample case|walnut|S/G|2400/98|CP66|TR-L,GUILD
T|coin_weight_casket|a brass-fitted coin-weight casket|boxwood|VS/G|550/35|CP66|TR-L,GUILD
T|correspondence_chest|a shallow correspondence chest|cedar|N/S|14000/92|CP22|TR-N,GUILD
F|counting_table|a broad walnut counting table|walnut|VL/G|62000/597|CP19|FU-L,GUILD
F|ledger_bookcase|a tall merchant's ledger bookcase|oak|VL/G|78000/368|CP07|FU-L,GUILD
F|account_cupboard|a panelled account cupboard|walnut|VL/G|78000/570|CP05|FU-L,GUILD
F|sample_display_shelves|a tiered sample-display shelf|oak|VL/G|58000/298|CP06|FU-L,GUILD
F|strongbox_bench|a lockroom coffer bench|oak|VL/G|75000/385|CP03|FU-L,GUILD
P|cashiers_purse|a divided cashier's waist purse|leather|VS/G|220/26|CP70|PC-L,GUILD
P|seal_case|a fitted merchant-seal belt case|leather|VS/G|340/39|CP74|PC-L,GUILD
P|account_satchel|a flap-front account satchel|leather|S/G|940/57|CP61|PC-L,GUILD
P|sample_wallet|a folding cloth-sample wallet|linen|VS/S|160/10|CP60|PC-N,GUILD
D|venetian_beaker|a clear ribbed glass beaker|soda-lime glass|VS/G|310/33|CP29|DW-L,GUILD,WATER
D|pewter_plate|a broad pewter dining plate|pewter|S/G|980/32|CP43|DW-L,GUILD,OPEN
D|painted_bowl|a painted faience table bowl|faience|S/G|600/22|CP32|DW-L,GUILD,WATER
D|brass_ewer|a slender hammered brass ewer|brass|N/G|2450/108|CP51|DW-L,GUILD,WATER
D|counting_room_tray|a narrow walnut refreshment tray|walnut|N/G|2000/49|CP76|DW-L,GUILD,OPEN
D|merchant_salt_box|a compartmented merchant's salt box|boxwood|VS/G|550/35|CP66|DW-L,GUILD
```

### P02 — Civic, guild, and customs office

Complete civic and guild package. Shared customs chests, guild strong chests, tax and toll chests, tally-seal lockboxes, weights lockboxes, and market-scale trays remain dependencies. New rows supply functional till interaction, fitted official standards, installed archival furniture, and distinct communal service capacities.

```text
T|civic_till_chest|a civic dues till chest|oak|N/G|17000/210|CP56|TR-L,GUILD
T|seal_register_coffer|a wax-sealable seal-register coffer|oak|N/G|14000/130|CP23|TR-L,GUILD
T|standard_weight_box|a fitted standard-weight box|brass|S/G|3500/139|CP44|TR-L,GUILD
T|charter_packet_chest|a shallow charter-packet chest|walnut|N/G|14000/184|CP22|TR-L,GUILD
T|customs_sample_case|a divided customs sample case|beech|S/S|2400/36|CP66|TR-N,GUILD
F|charter_cupboard|a deep charter cupboard|oak|VL/G|78000/368|CP05|FU-L,GUILD
F|guild_counting_table|a six-place guild counting table|oak|VL/G|70000/315|CP16|FU-L,GUILD
F|municipal_bookcase|a tall municipal record bookcase|oak|VL/G|78000/368|CP07|FU-L,GUILD
F|standard_measure_shelves|a fitted standard-measure shelf|oak|VL/G|58000/298|CP06|FU-L,GUILD
F|seal_office_sideboard|a panelled seal-office sideboard|walnut|H/G|95000/814|CP13|FU-L,GUILD
P|clerks_document_pouch|a clerk's beltable document pouch|leather|VS/S|260/18|CP60|PC-N,GUILD
P|seal_officers_case|a brass-mounted seal officer's case|leather|VS/G|340/39|CP74|PC-L,GUILD
P|tally_satchel|a stout civic tally satchel|canvas|S/S|690/18|CP61|PC-N,GUILD
P|fee_purse|a reinforced fee-collector's purse|leather|VS/G|220/26|CP70|PC-L,GUILD
D|guild_tankard|a broad pewter guild tankard|pewter|S/G|910/38|CP54|DW-L,GUILD,WATER
D|service_pitcher|a hammered brass service pitcher|brass|N/G|3500/92|CP52|DW-L,GUILD,WATER
D|communal_bowl|a deep stoneware communal bowl|stoneware|S/S|690/8|CP32|DW-N,GUILD,WATER
D|serving_board|a long oak guildhall serving board|oak|N/S|2000/18|CP76|DW-N,GUILD,OPEN
D|pewter_charger|a wide civic pewter charger|pewter|S/G|980/32|CP43|DW-L,GUILD,OPEN
D|ceramic_salt_cellar|a lidded faience salt cellar|faience|VS/G|600/44|CP28|DW-L,GUILD
```

### P03 — Warehouse and dockside handling

Complete warehouse package. Generic dock bond chests, export crates, bottle crates, casks, sacks, bales, and packing boxes must use shared references; retained rows add stronger secure capacity, fitted gauge/sample storage, installed racks, or exact liquid profiles.

```text
T|dock_tally_chest|a wax-sealable dock tally chest|oak|N/S|14000/74|CP23|TR-N,MARITIME
T|bonded_cargo_coffer|a heavily secured bonded cargo coffer|oak|L/G|52000/402|CP26|TR-L,MARITIME
T|bottle_sample_chest|a padded bottle-sample chest|pine|S/S|2400/29|CP66|TR-N,MARITIME
T|cask_gauge_box|a divided cask-gauge box|ash|S/S|2400/36|CP66|TR-N,MARITIME
T|marked_export_rundlet|a cooper-marked export rundlet|oak|L/S|35000/72|CP80|TR-N,MARITIME,WATER
F|warehouse_bin_rack|a many-bayed warehouse bin rack|pine|VL/S|55000/120|CP18|FU-N,MARITIME
F|bottle_rack|a slotted warehouse bottle rack|oak|VL/S|55000/150|CP18|FU-N,MARITIME
F|cask_cradle_rack|a heavy cask-cradle rack|oak|VL/S|55000/150|CP18|FU-N,MARITIME
F|dockside_counter|a tar-darkened dockside counter|oak|H/S|105000/260|CP04|FU-N,MARITIME
F|cask_rinsing_vat|a fixed cask-rinsing vat|oak|H/S|68000/180|CP20|FU-N,MARITIME,WATER
P|stevedore_belt_pouch|a stout stevedore's belt pouch|leather|VS/S|190/10|CP65|PC-N,MARITIME
P|warehouse_key_pouch|a reinforced warehouse key pouch|leather|VS/S|190/10|CP65|PC-N,MARITIME
P|tallyman_satchel|a waxed tallyman's shoulder satchel|canvas|S/S|690/18|CP61|PC-N,MARITIME
P|porter_back_basket|a deep wicker porter's back basket|wicker|N/S|1700/22|CP63|PC-N,MARITIME,OPEN
D|dockside_mug|a thick stoneware dockside mug|stoneware|S/S|560/7|CP41|DW-N,MARITIME,WATER
D|communal_water_pitcher|a large earthenware dock pitcher|earthenware|N/S|2750/16|CP35|DW-N,MARITIME,WATER
D|mess_trencher|a rough oak warehouse trencher|oak|S/P|700/6|CP64|DW-S,MARITIME,OPEN
D|salt_box|a small lidded dockside salt box|oak|S/S|2400/20|CP57|DW-N,MARITIME
D|ration_tray|a broad pine ration tray|pine|N/S|2000/14|CP76|DW-N,MARITIME,OPEN
D|vinegar_flask|a squat stoneware vinegar flask|stoneware|S/S|810/15|CP30|DW-N,MARITIME,WATER
```

### P04 — Shipboard steward and purser stores

Complete shipboard package. Shared biscuit barrels, salted-food casks, bottle crates, rope crates, cargo chests, document cases, and global commodity packages remain dependencies. New rows are sea-braced, bulkhead-installed, fitted for shipboard inventories, or tied to exact finite liquid capacities.

```text
T|pursers_till_chest|a sea-braced purser's till chest|oak|N/G|17000/210|CP56|TR-L,MARITIME
T|captains_chart_chest|a shallow captain's chart chest|oak|N/G|14000/119|CP22|TR-L,MARITIME
T|bottle_tally_case|a fitted ship's bottle-tally case|beech|S/S|2400/36|CP66|TR-N,MARITIME
T|medicine_store_chest|a partitioned shipboard medicine chest|cedar|S/G|2400/85|CP66|TR-L,MARITIME
T|provision_rundlet|a tarred shipboard provision rundlet|oak|L/S|35000/72|CP80|TR-N,MARITIME,WATER
F|purser_locker_wall|a bank of shipboard purser lockers|oak|H/G|108000/542|CP10|FU-L,MARITIME
F|chart_cupboard|a narrow bulkhead chart cupboard|oak|VL/G|78000/368|CP05|FU-L,MARITIME
F|bottle_cradle_shelves|a braced shipboard bottle shelf|oak|VL/S|55000/150|CP18|FU-N,MARITIME
F|mess_table|a bolted ship's mess table|oak|VL/S|70000/180|CP16|FU-N,MARITIME
F|water_butt|a fixed shipboard water butt|oak|H/S|210000/480|CP21|FU-N,MARITIME,WATER
P|pursers_coin_purse|a close-fastened purser's coin purse|leather|VS/G|220/26|CP70|PC-L,MARITIME
P|boatswains_key_pouch|a broad boatswain's key pouch|leather|VS/S|190/10|CP65|PC-N,MARITIME
P|chart_case|a shoulder-slung leather chart case|leather|S/G|750/44|CP72|PC-L,MARITIME
P|stewards_provision_bag|a waxed steward's provision bag|canvas|N/S|660/13|CP75|PC-N,MARITIME
D|mess_bowl|a deep pewter shipboard bowl|pewter|S/S|770/18|CP49|DW-N,MARITIME,WATER
D|sailors_tankard|a heavy-lipped pewter sea tankard|pewter|S/S|910/22|CP54|DW-N,MARITIME,WATER
D|shipboard_pitcher|a squat stoneware shipboard pitcher|stoneware|N/S|3000/20|CP35|DW-N,MARITIME,WATER
D|galley_platter|a broad wooden galley platter|oak|N/S|2000/18|CP76|DW-N,MARITIME,OPEN
D|cabin_salt_casket|a brass-bound cabin salt casket|boxwood|VS/G|650/32|CP73|DW-L,MARITIME
D|oil_flask|a stoppered glazed oil flask|stoneware|S/S|810/15|CP30|DW-N,MARITIME,WATER
```

### P05 — Tavern, alehouse, and inn service

Complete tavern package. Generic casks, biscuit barrels, salted-food containers, coin boxes, and ordinary serving wares should reuse shared or common-form stock where unchanged. Retained rows introduce functional till mechanics, fitted gaming/sample storage, installed public-house furniture, and differentiated drinking capacities.

```text
T|tavern_till_chest|a brass-bound tavern till chest|oak|N/G|17000/210|CP56|TR-L,SERVICE
T|gaming_stakes_box|a divided gaming-stakes box|walnut|S/G|2400/98|CP66|TR-L,SERVICE
T|cask_sample_case|a fitted taproom cask-sample case|oak|VS/S|550/20|CP66|TR-N,SERVICE
T|wine_account_chest|a wax-sealable wine-account chest|oak|N/G|14000/130|CP23|TR-L,SERVICE
T|cellar_rundlet|a marked cellar wine rundlet|oak|L/S|35000/72|CP80|TR-N,SERVICE,WATER
F|bar_counter|a broad oak serving counter|oak|H/G|105000/455|CP04|FU-L,SERVICE
F|bottle_shelves|a tall set of tavern bottle shelves|oak|VL/S|55000/150|CP18|FU-N,SERVICE
F|service_cupboard|a deep inn service cupboard|oak|VL/S|78000/210|CP05|FU-N,SERVICE
F|settle_chest|a high-backed settle chest|oak|VL/G|75000/385|CP03|FU-L,SERVICE
F|tankard_washing_vat|a fixed tankard-washing vat|oak|H/S|68000/180|CP20|FU-N,SERVICE,WATER
P|tapsters_purse|a greasy tapster's waist purse|leather|VS/S|220/15|CP70|PC-N,SERVICE
P|servers_tote|a broad server's shoulder tote|canvas|N/S|660/13|CP75|PC-N,SERVICE
P|cellar_key_pouch|a ring-stained cellar key pouch|leather|VS/S|190/10|CP65|PC-N,SERVICE
P|score_wallet|a folding tavern score wallet|leather|VS/S|260/18|CP60|PC-N,SERVICE
D|pewter_tankard|a straight-sided pewter tankard|pewter|S/S|910/22|CP54|DW-N,SERVICE,WATER
D|stoneware_mug|a salt-glazed stoneware mug|stoneware|S/G|560/12|CP41|DW-L,SERVICE,WATER
D|wine_glass|a green-tinted stemmed wine glass|glass|VS/G|280/36|CP39|DW-L,SERVICE,WATER
D|ale_pitcher|a broad-bellied earthenware ale pitcher|earthenware|N/S|2750/16|CP35|DW-N,SERVICE,WATER
D|service_tray|a round oak tavern service tray|oak|N/S|2000/18|CP76|DW-N,SERVICE,OPEN
D|inn_plate|a shallow pewter inn plate|pewter|S/S|980/18|CP43|DW-N,SERVICE,OPEN
```

### P06 — Urban food shop and household kitchen

Complete urban kitchen package. Shared flour sacks, bean sacks, grain casks, spice containers, food barrels, market baskets, and packing boxes remain dependencies. New rows add precise liquid handling, fitted measuring storage, installed pantry/scullery furniture, and wearable household carrying forms.

```text
T|grocers_till_chest|a grocer's divided till chest|oak|N/S|17000/120|CP56|TR-N,SERVICE
T|pastry_delivery_box|a slatted pastry delivery box|pine|S/S|2400/16|CP57|TR-N,SERVICE
T|spice_measure_box|a fitted kitchen spice-measure box|boxwood|S/G|2400/63|CP66|TR-L,SERVICE
T|oil_transport_jar|a stoppered oil transport jar|stoneware|L/S|6900/26|CP37|TR-N,SERVICE,WATER
T|dairy_pail|a handled dairy seller's pail|oak|N/S|2400/24|CP79|TR-N,SERVICE,WATER
F|pantry_cupboard|a deep panelled pantry cupboard|oak|VL/S|78000/210|CP05|FU-N,SERVICE
F|flour_meal_bin|a fixed flour-and-meal bin|pine|L/S|38000/76|CP12|FU-N,SERVICE,OPEN
F|kitchen_dresser|a broad open-shelved kitchen dresser|oak|H/S|105000/300|CP08|FU-N,SERVICE
F|preparation_table|a scarred kitchen preparation table|oak|VL/S|70000/180|CP16|FU-N,SERVICE
F|scullery_vat|a fixed wooden scullery vat|oak|H/S|68000/180|CP20|FU-N,SERVICE,WATER
P|market_purse|a drawstring household market purse|leather|VS/S|220/15|CP70|PC-N,SERVICE
P|recipe_wallet|a grease-spotted recipe wallet|parchment|VS/S|160/15|CP86|PC-N,SERVICE
P|shopping_tote|a broad linen shopping tote|linen|N/S|540/14|CP75|PC-N,SERVICE
P|kitchen_waterskin|a small kitchen water sling|leather|S/S|1100/18|CP82|PC-N,SERVICE,WATER
D|kitchen_pot|a broad earthenware kitchen pot|earthenware|L/S|6000/18|CP36|DW-N,SERVICE,WATER
D|mixing_bowl|a deep glazed mixing bowl|faience|S/S|600/12|CP32|DW-N,SERVICE,WATER
D|kitchen_pitcher|a handled stoneware kitchen pitcher|stoneware|N/S|3000/20|CP35|DW-N,SERVICE,WATER
D|wash_basin|a broad copper kitchen basin|copper|N/G|2950/77|CP48|DW-L,SERVICE,WATER
D|wooden_plate|a round beech household plate|beech|S/S|700/10|CP64|DW-N,SERVICE,OPEN
D|serving_board|a long carved kitchen serving board|beech|N/S|2000/18|CP76|DW-N,SERVICE,OPEN
```

### P07 — Elite Western and Iberian household

Complete elite household package. Shared merchant coffers, linen chests, packing cases, and commodity containers remain dependencies when their behaviour is unchanged. Retained rows are cassone/credenza/armoire forms, fitted luxury-service cases, transparent display furniture, or materially distinct elite tableware.

```text
T|silver_service_coffer|a fitted silver-service coffer|walnut|N/E|18000/428|CP25|TR-L,COURT,SERVICE
T|linen_travel_chest|a panelled household linen chest|cedar|L/G|28000/232|CP24|TR-L,COURT,SERVICE
T|scent_bottle_case|a padded scent-bottle case|walnut|S/E|2400/167|CP66|TR-L,COURT,SERVICE
T|portrait_casket|a miniature portrait casket|ivory|VS/E|720/324|CP73|TR-L,COURT,SERVICE
T|plate_transport_chest|a felt-lined plate transport chest|oak|S/G|2400/63|CP66|TR-L,COURT,SERVICE
F|bridal_cassone|a painted walnut bridal cassone|walnut|VL/E|48000/721|CP02|FU-L,COURT,SERVICE
F|display_credenza|a carved two-tier display credenza|walnut|H/E|95000/1395|CP13|FU-L,COURT,SERVICE
F|chamber_armoire|a tall panelled chamber armoire|walnut|H/E|150000/1814|CP01|FU-L,COURT,SERVICE
F|glass_display_cabinet|an early glazed display cabinet|walnut|H/E|120000/2092|CP09|FU-L,COURT,SERVICE
F|banquet_table|a long walnut banquet table|walnut|H/G|118000/814|CP11|FU-L,COURT,SERVICE
P|jewel_purse|a velvet-lined jewel purse|silk|VS/E|300/36|CP70|PC-L,COURT,SERVICE
P|fan_case|a slim embroidered fan case|silk|VS/G|450/32|CP74|PC-L,COURT,SERVICE
P|scent_case|a fitted leather scent case|leather|VS/E|340/68|CP74|PC-L,COURT,SERVICE
P|portable_writing_case|a shoulder-slung portable writing case|walnut|S/G|1100/65|CP59|PC-L,COURT,SERVICE
D|painted_ewer|a painted faience banquet ewer|faience|N/E|1850/105|CP34|DW-L,COURT,SERVICE,WATER
D|matching_basin|a broad painted faience basin|faience|N/E|2400/82|CP31|DW-L,COURT,SERVICE,WATER
D|crystal_goblet|a clear lead-glass goblet|lead glass|VS/E|340/101|CP39|DW-L,COURT,SERVICE,WATER
D|export_bowl|a fine blue-and-white porcelain bowl|porcelain|S/E|580/66|CP32|DW-L,COURT,SERVICE,WATER
D|silver_charger|a broad chased silver charger|silver|S/E|950/240|CP43|DW-L,COURT,SERVICE,OPEN
D|inlaid_tray|a mother-of-pearl inlaid serving tray|mother of pearl|N/E|1700/405|CP76|DW-L,COURT,SERVICE,OPEN
```

### P08 — Apothecary and perfumer shop

Complete apothecary/perfumer package. Shared ingredient boxes, medicine trade chests, ceramic spice jars, herb packets, sachets, and merchant lockboxes remain dependencies. New rows add functional shop till, fitted vial capacity, installed drawer/bottle furniture, and precise medicinal liquid-vessel sizes.

```text
T|herb_sample_chest|a shallow herb-sample chest|cedar|S/G|2400/47|CP57|TR-L,GUILD
T|vial_transport_case|a padded glass-vial transport case|walnut|S/G|2400/98|CP66|TR-L,GUILD
T|dispensary_till_chest|a compact dispensary till chest|walnut|N/G|17000/326|CP56|TR-L,GUILD
T|prescription_archive|a wax-sealable prescription archive chest|cedar|N/G|14000/175|CP23|TR-L,GUILD
T|scent_oil_flask|a narrow stoppered scent-oil flask|glass|S/G|720/54|CP30|TR-L,GUILD,WATER
F|poison_cupboard|a small poison cupboard|walnut|L/G|42000/393|CP14|FU-L,GUILD
F|apothecary_drawers|a many-drawered apothecary cabinet|walnut|H/E|108000/1442|CP10|FU-L,GUILD
F|bottle_display_shelves|a tiered apothecary bottle shelf|walnut|VL/G|58000/461|CP06|FU-L,GUILD
F|dispensary_counter|a polished dispensary counter|walnut|H/G|105000/705|CP04|FU-L,GUILD
F|maceration_vat|a fixed copper maceration vat|copper|H/G|91800/630|CP20|FU-L,GUILD,WATER
P|physicians_satchel|a compartmented physician's satchel|leather|S/G|940/57|CP61|PC-L,GUILD
P|scent_vial_case|a small fitted scent-vial belt case|leather|VS/G|340/39|CP74|PC-L,GUILD
P|herb_pouch|a waxed linen herb pouch|linen|VS/S|110/6|CP65|PC-N,GUILD,POROUS
P|formula_wallet|a folding apothecary formula wallet|parchment|VS/G|160/27|CP86|PC-L,GUILD
D|albarello|a tall painted faience drug jar|faience|L/G|6050/66|CP37|DW-L,GUILD,WATER
D|ointment_pot|a squat stoneware ointment pot|stoneware|S/S|690/8|CP32|DW-N,GUILD,WATER
D|syrup_bottle|a dark green glass syrup bottle|glass|S/G|830/54|CP42|DW-L,GUILD,WATER
D|mixing_basin|a shallow glazed dispensary basin|faience|N/G|2400/48|CP31|DW-L,GUILD,WATER
D|medicine_cup|a tiny pewter medicine cup|pewter|T/G|250/19|CP50|DW-L,GUILD,WATER
D|instrument_tray|a narrow brass instrument tray|brass|N/G|2900/69|CP47|DW-L,GUILD,OPEN
```

### P09 — Ottoman bazaar and urban household

Complete Ottoman urban package. Shared bazaar cash boxes, spice chests, tea tubes, merchant caskets, and caravan chests remain dependencies. Retained rows add till interaction, carved installed furniture, Ottoman service silhouettes, exact oil/dye capacities, and a strictly late-sixteenth-century coffee cup admission.

```text
T|bazaar_till_chest|a brass-faced bazaar till chest|cedar|N/G|17000/284|CP56|TR-L,GUILD
T|spice_sample_chest|a many-celled cedar spice chest|cedar|S/G|2400/85|CP66|TR-L,GUILD
T|textile_sample_coffer|a shallow textile-sample coffer|walnut|L/G|28000/266|CP24|TR-L,GUILD
T|perfume_casket|a pierced brass perfume casket|brass|VS/E|940/119|CP45|TR-L,GUILD
T|oil_merchants_jar|a glazed oil merchant's jar|stoneware|VL/S|12500/41|CP38|TR-N,GUILD,WATER
F|wall_cupboard|a carved cedar wall cupboard|cedar|VL/G|78000/496|CP05|FU-L,GUILD
F|brassware_shelves|a tiered brassware display shelf|cedar|VL/G|58000/402|CP06|FU-L,GUILD
F|low_service_table|a low inlaid service table|walnut|L/G|40000/326|CP15|FU-L,GUILD
F|bazaar_counter|a narrow cedar bazaar counter|cedar|H/S|105000/351|CP04|FU-N,GUILD
F|dye_mixing_vat|a fixed copper dye-mixing vat|copper|H/G|91800/630|CP20|FU-L,GUILD,WATER
P|coin_purse|a tasseled leather bazaar purse|leather|VS/G|220/26|CP70|PC-L,GUILD
P|document_satchel|a tooled leather document satchel|leather|S/G|940/57|CP61|PC-L,GUILD
P|scent_flask_case|a fitted brass scent-flask case|brass|VS/G|650/69|CP46|PC-L,GUILD
P|urban_waterskin|a shoulder-slung urban waterskin|leather|S/S|1100/18|CP82|PC-N,GUILD,WATER
D|iznik_tankard|a painted faience handled tankard|faience|S/E|720/45|CP40|DW-L,GUILD,WATER
D|long_spouted_ewer|a long-spouted hammered copper ewer|copper|N/G|2300/98|CP51|DW-L,GUILD,WATER
D|painted_bowl|a deep painted faience bazaar bowl|faience|S/G|600/22|CP32|DW-L,GUILD,WATER
D|brass_tray|a broad engraved brass serving tray|brass|N/G|2900/69|CP47|DW-L,GUILD,OPEN
D|storage_jar|a tall glazed pantry storage jar|stoneware|VL/S|12500/41|CP38|DW-N,GUILD,WATER
D|coffee_cup|a tiny late-century faience coffee cup|faience|T/G|200/13|CP33|DW-L,GUILD,WATER
```

### P10 — Persianate and Indo-Persian court household

Complete Persianate/Indo-Persian package. Shared medicine chests, spice boxes, document cases, merchant strongboxes, and textile bales remain dependencies. New rows add manuscript and scent fittings, courtly low furniture, perfume preparation, and distinctive pouring or service forms.

```text
T|jewel_coffer|a lacquered jewel merchant's coffer|walnut|N/E|18000/428|CP25|TR-L,COURT,SERVICE
T|manuscript_chest|a wax-sealable manuscript chest|cedar|N/G|14000/175|CP23|TR-L,COURT,SERVICE
T|textile_sample_trunk|a fitted brocade-sample trunk|teak|L/G|28000/283|CP24|TR-L,COURT,SERVICE
T|scent_sample_box|a divided sandalwood scent box|sandalwood|S/E|2400/238|CP66|TR-L,COURT,SERVICE
T|rosewater_transport_jar|a sealed rosewater transport jar|stoneware|L/G|6900/45|CP37|TR-L,COURT,SERVICE,WATER
F|carpet_chest|a broad cedar carpet chest|cedar|VL/G|48000/366|CP02|FU-L,COURT,SERVICE
F|manuscript_cupboard|a painted manuscript cupboard|walnut|VL/G|78000/570|CP05|FU-L,COURT,SERVICE
F|niche_display_cabinet|a pierced niche-display cabinet|walnut|VL/E|58000/790|CP06|FU-L,COURT,SERVICE
F|garden_service_table|an inlaid low garden table|walnut|L/E|40000/558|CP15|FU-L,COURT,SERVICE
F|perfume_preparation_vat|a fixed copper perfume vat|copper|H/G|91800/630|CP20|FU-L,COURT,SERVICE,WATER
P|signet_pouch|a silk-lined signet pouch|silk|VS/E|250/24|CP65|PC-L,COURT,SERVICE
P|manuscript_case|a shoulder-slung lacquer manuscript case|lacquer|S/G|1100/84|CP59|PC-L,COURT,SERVICE
P|perfume_case|a fitted sandalwood perfume case|sandalwood|VS/E|450/119|CP74|PC-L,COURT,SERVICE
P|court_shoulder_bag|an embroidered court shoulder bag|silk|N/G|1200/32|CP75|PC-L,COURT,SERVICE
D|engraved_ewer|an engraved brass rosewater ewer|brass|N/E|2450/185|CP51|DW-L,COURT,SERVICE,WATER
D|washing_basin|a broad chased copper washing basin|copper|N/G|2950/77|CP48|DW-L,COURT,SERVICE,WATER
D|lustre_bowl|a lustre-painted faience serving bowl|faience|S/E|600/38|CP32|DW-L,COURT,SERVICE,WATER
D|rosewater_flask|a long-necked glass rosewater flask|glass|S/G|720/54|CP30|DW-L,COURT,SERVICE,WATER
D|inlaid_serving_tray|an inlaid papier-mache serving tray|papier-mache|N/E|1000/51|CP88|DW-L,COURT,SERVICE,OPEN
D|court_cup|a small silver court drinking cup|silver|T/E|240/144|CP50|DW-L,COURT,SERVICE,WATER
```

### P11 — South Asian merchant and service household

Complete South Asian household package. Shared teak spice chests, cotton hampers, sample bags, weight boxes, and merchant strongboxes remain dependencies; retained rows add till mechanics, palm-leaf manuscript capacity, wet-climate teak furniture, compartmented dining/storage forms, or exact liquid capacities.

```text
T|cash_chest|a compact teak merchant cash chest|teak|N/G|17000/346|CP56|TR-L,GUILD,SERVICE
T|textile_sample_trunk|a partitioned textile-sample trunk|teak|L/G|28000/283|CP24|TR-L,GUILD,SERVICE
T|pepper_sample_box|a many-celled pepper sample box|teak|S/G|2400/104|CP66|TR-L,GUILD,SERVICE
T|balance_weight_box|a fitted brass balance-weight box|brass|S/G|3500/139|CP44|TR-L,GUILD,SERVICE
T|coastal_account_chest|a wax-sealable coastal account chest|teak|N/G|14000/214|CP23|TR-L,GUILD,SERVICE
F|dowry_chest|a broad carved teak dowry chest|teak|VL/G|48000/448|CP02|FU-L,GUILD,SERVICE
F|vessel_shelf|a deep brass-vessel shelf stand|teak|VL/G|58000/491|CP06|FU-L,GUILD,SERVICE
F|palm_leaf_cupboard|a low palm-leaf manuscript cupboard|teak|VL/G|78000/606|CP05|FU-L,GUILD,SERVICE
F|cloth_bolt_rack|a long cloth-bolt storage rack|teak|VL/S|55000/248|CP18|FU-N,GUILD,SERVICE
F|dining_table|a low teak household dining table|teak|L/G|40000/346|CP15|FU-L,GUILD,SERVICE
P|betel_box|a small fitted brass betel box|brass|VS/G|650/69|CP46|PC-L,GUILD,SERVICE
P|pilgrim_bag|a broad cotton pilgrim's shoulder bag|cotton|N/S|540/15|CP75|PC-N,GUILD,SERVICE
P|manuscript_wrap|a tied cotton manuscript wrap|cotton|VS/S|160/12|CP60|PC-N,GUILD,SERVICE
P|spice_pouch|a small tooled leather spice pouch|leather|VS/S|190/10|CP65|PC-N,GUILD,SERVICE
D|water_pot|a rounded brass household water pot|brass|N/G|3500/92|CP52|DW-L,GUILD,SERVICE,WATER
D|dining_tray|a broad brass dining tray|brass|N/G|2900/69|CP47|DW-L,GUILD,SERVICE,OPEN
D|rice_bowl|a deep brass rice bowl|brass|S/G|800/38|CP49|DW-L,GUILD,SERVICE,WATER
D|water_jar|a tall earthenware courtyard water jar|earthenware|VL/S|11500/31|CP38|DW-N,GUILD,SERVICE,WATER
D|spice_box|a round compartmented spice box|brass|VS/G|800/77|CP44|DW-L,GUILD,SERVICE
D|ghee_jar|a narrow stoneware ghee jar|stoneware|L/S|6900/26|CP37|DW-N,GUILD,SERVICE,WATER
```

### P12 — Ming and Joseon scholar-merchant household

Complete East Asian literati package. Shared bamboo tubes, tea canisters, silk samples, merchant boxes, packing crates, and writing stock remain dependencies; retained rows add scholar/medicine furniture, fitted seal and porcelain capacity, compound cabinetry, or distinct porcelain service forms.

```text
T|cash_chest|a lacquered scholar-merchant cash chest|walnut|N/G|17000/326|CP56|TR-L
T|silk_bolt_chest|a long fitted silk-bolt chest|cedar|L/G|28000/232|CP24|TR-L
T|seal_chop_box|a divided seal-and-chop box|boxwood|S/G|2400/63|CP66|TR-L
T|porcelain_sample_chest|a padded porcelain-sample chest|pine|S/G|2400/50|CP66|TR-L
T|invoice_scroll_case|a bamboo invoice scroll case|bamboo|S/S|1100/18|CP71|TR-N
F|garment_cabinet|a tall compound garment cabinet|walnut|H/G|125000/922|CP17|FU-L
F|scholar_book_chest|a low scholar's book chest|cedar|VL/G|48000/366|CP02|FU-L
F|medicine_cabinet|a many-drawered medicine cabinet|walnut|H/G|108000/841|CP10|FU-L
F|display_shelf_stand|an open literati display shelf stand|walnut|VL/G|58000/461|CP06|FU-L
F|writing_table|a low East Asian writing table|walnut|VL/G|62000/597|CP19|FU-L
P|brush_roll|a tied silk calligrapher's brush roll|silk|VS/G|350/24|CP60|PC-L
P|seal_pouch|a padded silk chop-seal pouch|silk|VS/G|250/14|CP65|PC-L
P|document_wallet|a folding paper document wallet|paper|VS/S|120/8|CP86|PC-N
P|scroll_tube|a shoulder-slung bamboo scroll tube|bamboo|S/S|1000/20|CP72|PC-N
D|stem_cup|a small blue-and-white porcelain stem cup|porcelain|T/G|190/23|CP33|DW-L,WATER
D|porcelain_ewer|a pear-shaped blue-and-white porcelain ewer|porcelain|N/E|1800/185|CP34|DW-L,WATER
D|covered_box|a small covered porcelain box|porcelain|VS/G|580/77|CP28|DW-L
D|storage_jar|a broad glazed stoneware scholar's jar|stoneware|VL/S|12500/41|CP38|DW-N,WATER
D|serving_tray|a black-lacquer scholar's serving tray|lacquer|N/G|2000/63|CP76|DW-L,OPEN
D|white_bowl|a deep white porcelain service bowl|porcelain|S/G|580/38|CP32|DW-L,WATER
```

### P13 — Japanese and Ryukyuan merchant household

Complete Japanese/Ryukyuan package. Generic bamboo storage, tea tubes, merchant caskets, porcelain packing, and shared tea packaging remain dependencies. Tiered medicine cases are late-sixteenth-century gated; exact lacquer, cabinet, and service forms are retained where silhouette or component differs.

```text
T|merchant_cash_chest|a low lacquered Japanese merchant chest|cedar|N/G|17000/284|CP56|TR-L
T|tribute_document_box|a wax-sealable Ryukyuan tribute box|lacquer|S/G|2700/91|CP58|TR-L
T|tea_utensil_chest|a partitioned tea-utensil transport chest|cedar|S/G|2400/85|CP66|TR-L
T|lacquerware_shipping_nest|a nested lacquerware shipping set|lacquer|S/G|2400/126|CP66|TR-L
T|shell_faced_export_casket|a shell-faced Ryukyuan export casket|mother of pearl|S/E|2700/765|CP84|TR-L
F|stacked_clothing_chest|a stacked cedar clothing chest|cedar|H/G|105000/709|CP08|FU-L
F|tea_utensil_cupboard|a low tea-utensil cupboard|cedar|VL/G|78000/496|CP05|FU-L
F|display_shelf|an asymmetrical alcove display shelf|cedar|VL/G|58000/402|CP06|FU-L
F|document_chest|a broad lacquered household document chest|cedar|VL/G|48000/366|CP02|FU-L
F|folding_serving_table|a low folding Japanese serving table|cedar|L/G|40000/284|CP15|FU-L
P|tiered_medicine_case|a small tiered lacquer medicine case|lacquer|VS/E|450/108|CP74|PC-L
P|fan_case|a slim silk folding-fan case|silk|VS/G|450/32|CP74|PC-L
P|brush_case|a narrow lacquered brush case|lacquer|S/G|1000/70|CP72|PC-L
P|cloth_bundle_bag|a tied cotton traveller's bundle bag|cotton|N/S|540/15|CP75|PC-N
D|tea_bowl|a deep stoneware tea bowl|stoneware|S/G|690/15|CP32|DW-L,WATER
D|freshwater_jar|a broad glazed freshwater jar|stoneware|L/G|6900/45|CP37|DW-L,WATER
D|sake_flask|a narrow ceramic sake flask|stoneware|S/G|810/27|CP30|DW-L,WATER
D|lacquer_bowl|a deep red-lacquer serving bowl|lacquer|S/G|550/35|CP77|DW-L,WATER
D|lacquer_tray|a rectangular Japanese lacquer tray|lacquer|N/G|2000/63|CP76|DW-L,OPEN
D|serving_dish|a shallow porcelain serving dish|porcelain|S/G|740/38|CP27|DW-L,OPEN
```

### P14 — Mainland and maritime South-east Asian port household

Complete South-east Asian port package. Shared spice sacks/chests, bamboo baskets, tea tubes, cotton hampers, and generic cargo chests remain dependencies; retained rows add rattan pannier form, palm-leaf document capacity, large fermentation mechanics, or distinctive local service vessels.

```text
T|spice_pannier|a lidded rattan spice pannier|rattan|L/S|4400/21|CP69|TR-N,MARITIME,POROUS
T|tribute_chest|a lacquered mainland tribute chest|teak|N/G|18000/266|CP25|TR-L,MARITIME,COURT
T|maritime_storage_jar|a massive glazed maritime storage jar|stoneware|VL/G|12500/71|CP38|TR-L,MARITIME,WATER
T|betel_merchants_box|a divided betel merchant's box|teak|S/G|2400/104|CP66|TR-L,MARITIME
T|palm_leaf_document_chest|a fitted palm-leaf document chest|teak|N/G|14000/214|CP23|TR-L,MARITIME
F|temple_chest|a painted lacquer temple chest|teak|VL/G|48000/448|CP02|FU-L,MARITIME,RELIGIOUS
F|rattan_shelves|a broad rattan storage shelf|rattan|VL/S|35750/105|CP18|FU-N,MARITIME
F|bronze_vessel_rack|a tiered bronze-vessel rack|teak|VL/G|58000/491|CP06|FU-L,MARITIME
F|market_counter|a low coastal market counter|teak|H/S|105000/429|CP04|FU-N,MARITIME
F|fermentation_vat|a vast earthenware fermentation vat|earthenware|H/S|241500/312|CP21|FU-N,MARITIME,WATER
P|shoulder_basket|a lidded rattan shoulder basket|rattan|N/S|1800/15|CP68|PC-N,MARITIME,POROUS
P|betel_pouch|a small embroidered betel pouch|cotton|VS/G|110/12|CP65|PC-L,MARITIME
P|document_satchel|a woven palm-fibre document satchel|raffia cloth|S/S|500/17|CP61|PC-N,MARITIME
P|gourd_bottle_sling|a netted gourd bottle sling|gourd shell|S/S|490/5|CP82|PC-N,MARITIME,WATER
D|kendi|a long-spouted earthenware kendi|earthenware|N/G|1950/32|CP34|DW-L,MARITIME,WATER
D|bronze_tray|a shallow coastal bronze serving tray|bronze|N/G|3000/76|CP47|DW-L,MARITIME,OPEN
D|lacquer_bowl|a deep red-lacquer port bowl|lacquer|S/G|550/35|CP77|DW-L,MARITIME,WATER
D|rice_basket|a lidded rattan rice basket|rattan|N/S|1800/13|CP67|DW-N,MARITIME,POROUS
D|water_jar|a tall earthenware port water jar|earthenware|VL/S|11500/31|CP38|DW-N,MARITIME,WATER
D|gourd_cup|a polished South-east Asian gourd cup|gourd shell|T/S|60/2|CP78|DW-N,MARITIME,WATER
```

### P15 — Steppe and caravan household

Complete mobile-caravan package. Generic caravan cargo/pay chests, wagon bins, sacks, bales, and merchant strongboxes remain shared. Retained rows are saddle-narrow, felt-insulated, paired pannier, courier, folding-furniture, or wearable forms.

```text
T|saddle_caravan_coffer|a narrow saddle-borne caravan coffer|leather|N/G|13500/201|CP83|TR-L
T|felt_lined_food_chest|a felt-lined dried-goods chest|cedar|N/S|14000/92|CP22|TR-N
T|coin_weight_casket|a fitted steppe coin-and-weight casket|bronze|S/G|3600/151|CP44|TR-L
T|courier_dispatch_tube|a hide-covered courier dispatch tube|leather|S/G|820/39|CP71|TR-L
T|double_pannier|a matched pair of leather cargo panniers|leather|L/S|5250/35|CP62|TR-N,OPEN
F|campaign_chest_table|a folding campaign chest-table|cedar|L/G|40000/284|CP15|FU-L
F|felt_storage_chest|a felt-lined mobile storage chest|cedar|VL/S|48000/209|CP02|FU-N
F|low_folding_table|a low folding birch service table|birch|L/S|40000/120|CP15|FU-N
F|supply_cupboard|a compact travelling supply cupboard|cedar|L/S|42000/196|CP14|FU-N
F|bedding_chest|a stackable caravan bedding chest|cedar|VL/S|48000/209|CP02|FU-N
P|felt_shoulder_bag|a soft felt caravan shoulder bag|felt|N/S|660/14|CP75|PC-N
P|document_pouch|a hard leather courier document pouch|leather|VS/G|260/31|CP60|PC-L
P|coin_purse|a reinforced steppe belt purse|leather|VS/S|220/15|CP70|PC-N
P|medicine_gourd_sling|a small medicine-gourd sling|gourd shell|S/S|490/5|CP82|PC-N,WATER
D|wooden_bowl|a shallow birch drinking bowl|birch|S/S|550/10|CP77|DW-N,WATER
D|water_bottle|a stitched leather caravan water bottle|leather|S/S|900/15|CP81|DW-N,WATER
D|copper_cauldron|a small hammered copper cauldron|copper|L/S|7000/56|CP53|DW-N,WATER
D|serving_tray|a low birch caravan serving tray|birch|N/S|2000/18|CP76|DW-N,OPEN
D|horn_cup|a polished steppe horn cup|horn|T/S|180/6|CP78|DW-N,WATER
D|food_basket|a lidded felt food basket|felt|N/S|1550/14|CP67|DW-N,POROUS
```

### P16 — West African court and Atlantic trade household

Complete West African court/trade package. Generic merchant boxes, sample bags, bales, baskets, and strong chests remain shared where form matches. Retained rows add currency/weight compartmenting, court-scale installed storage, gourd-shell liquid behaviour, or distinctive cast-metal and carved service forms.

```text
T|brassweight_box|a fitted brassweight trader's box|wood|S/G|2400/63|CP66|TR-L,COURT
T|cowrie_currency_chest|a divided cowrie-currency chest|wood|N/G|18000/161|CP25|TR-L,COURT
T|ivory_trade_casket|a small carved ivory trade casket|ivory|S/E|3500/612|CP84|TR-L,COURT
T|textile_sample_chest|a broad Atlantic textile-sample chest|wood|L/G|28000/172|CP24|TR-L,COURT
T|coastal_document_chest|a wax-sealable coastal document chest|wood|N/G|14000/130|CP23|TR-L,COURT
F|regalia_chest|a carved court regalia chest|wood|VL/E|48000/465|CP02|FU-L,COURT
F|brassware_shelf|a broad brassware display shelf|wood|VL/G|58000/298|CP06|FU-L,COURT
F|storage_hamper|a great lidded woven storage hamper|wicker|VL/S|28800/101|CP02|FU-N,COURT,POROUS
F|serving_table|a carved ceremonial serving table|wood|VL/E|70000/540|CP16|FU-L,COURT
F|palace_store_cupboard|a deep palace store cupboard|wood|VL/G|78000/368|CP05|FU-L,COURT
P|cowrie_purse|a netted cowrie waist purse|leather|VS/S|220/15|CP70|PC-N,COURT
P|amulet_pouch|a small tooled leather amulet pouch|leather|VS/G|190/18|CP65|PC-L,COURT
P|bead_merchants_bag|a broad bead merchant's shoulder bag|cotton|N/S|540/15|CP75|PC-N,COURT
P|calabash_sling|a netted calabash bottle sling|gourd shell|S/S|490/5|CP82|PC-N,COURT,WATER
D|brass_bowl|a cast brass court serving bowl|brass|S/G|800/38|CP49|DW-L,COURT,WATER
D|wooden_cup|a carved West African wooden cup|wood|T/G|180/10|CP78|DW-L,COURT,WATER
D|gourd_bowl|a polished court gourd bowl|gourd shell|S/S|190/4|CP77|DW-N,COURT,WATER
D|woven_basket|a close-lidded woven food basket|wicker|N/S|1700/12|CP67|DW-N,COURT,POROUS
D|copper_ewer|a hammered Atlantic copper ewer|copper|N/G|2300/98|CP51|DW-L,COURT,WATER
D|wooden_platter|a broad carved court platter|wood|N/G|2000/32|CP76|DW-L,COURT,OPEN
```

### P17 — Sahelian, Red Sea, and Swahili merchant household

Complete Sahel/Red Sea/Indian Ocean package. Generic caravan, spice, document, merchant, and customs containers remain shared. Retained rows add manuscript sealing, ivory-parcel fitting, fixed water storage, culture-gated document wear, or imported/local service forms.

```text
T|caravan_chest|a leather-covered Sahel caravan chest|cedar|N/G|18000/217|CP25|TR-L,GUILD
T|manuscript_trade_coffer|a wax-sealable manuscript trade coffer|cedar|N/G|14000/175|CP23|TR-L,GUILD
T|ivory_parcel_lockbox|a fitted ivory-parcel lockbox|wood|S/G|3200/60|CP84|TR-L,GUILD
T|spice_measure_box|a divided Indian Ocean spice box|wood|S/G|2400/63|CP66|TR-L,GUILD
T|customs_seal_chest|a Red Sea customs seal chest|cedar|N/G|14000/175|CP23|TR-L,GUILD,MARITIME
F|manuscript_cupboard|a tall Sahelian manuscript cupboard|cedar|VL/G|78000/496|CP05|FU-L,GUILD
F|covered_storage_chest|a leather-covered merchant storage chest|cedar|VL/G|48000/366|CP02|FU-L,GUILD
F|ceramic_display_shelf|an imported-ceramic display shelf|wood|VL/G|58000/298|CP06|FU-L,GUILD
F|merchant_counter|a low Swahili merchant counter|wood|H/S|105000/260|CP04|FU-N,GUILD
F|water_storage_vat|a fixed plaster-lined water vat|plaster|H/S|210000/480|CP21|FU-N,GUILD,WATER
P|manuscript_satchel|a flap-front Sahel manuscript satchel|leather|S/G|940/57|CP61|PC-L,GUILD
P|cowrie_coin_pouch|a small Indian Ocean cowrie pouch|leather|VS/S|190/10|CP65|PC-N,GUILD
P|rolled_document_case|a shoulder-slung Red Sea document case|leather|S/G|750/44|CP72|PC-L,GUILD
P|water_gourd_sling|a woven Swahili water-gourd sling|gourd shell|S/S|490/5|CP82|PC-N,GUILD,WATER
D|imported_bowl|a glazed imported coastal ceramic bowl|faience|S/G|600/22|CP32|DW-L,GUILD,WATER
D|brass_ewer|a narrow engraved Red Sea brass ewer|brass|N/G|2450/108|CP51|DW-L,GUILD,WATER
D|water_jar|a broad Swahili earthenware water jar|earthenware|VL/S|11500/31|CP38|DW-N,GUILD,WATER
D|serving_tray|a flat carved coastal serving tray|wood|N/S|2000/18|CP76|DW-N,GUILD,OPEN
D|horn_cup|a polished Sahelian carved horn cup|horn|T/G|180/10|CP78|DW-L,GUILD,WATER
D|palm_fibre_basket|a lidded palm-fibre food basket|raffia cloth|N/S|1100/12|CP67|DW-N,GUILD,POROUS
```

### P18 — Mesoamerican market and tribute household

Complete Mesoamerican package. Generic sacks, bales, baskets, packets, and colonial import containers remain shared only where admitted. Retained rows represent tribute/codex/cacao capacity, raised storage installations, or distinctive ceramic and gourd service forms; ritual-only meanings remain outside this domestic pass.

```text
T|cacao_market_basket|a close-lidded cacao market basket|wicker|L/S|4100/20|CP69|TR-N,POROUS
T|feather_merchants_basket|a tall feather merchant's basket|reed|L/G|3850/27|CP62|TR-L,OPEN
T|shell_ornament_box|a divided shell-ornament trade box|wood|S/G|2400/63|CP66|TR-L
T|codex_transport_chest|a fitted codex transport chest|wood|N/G|14000/130|CP23|TR-L
T|tribute_textile_chest|a broad tribute-textile chest|wood|L/G|28000/172|CP24|TR-L
F|market_platform|a raised market display platform|wood|VL/S|58000/170|CP06|FU-N
F|basket_rack|a tall woven-basket storage rack|wood|VL/S|55000/150|CP18|FU-N
F|codex_shelf_chest|a low bark-paper codex chest|wood|VL/G|78000/368|CP07|FU-L
F|jar_storage_platform|a raised pottery-jar platform|wood|VL/S|55000/150|CP18|FU-N
F|tribute_bin_wall|a many-bayed tribute-store bin wall|wood|VL/S|55000/150|CP18|FU-N
P|market_bag|a woven cotton market shoulder bag|cotton|N/S|540/15|CP75|PC-N
P|net_carrying_bag|a knotted net burden bag|cotton|N/S|1250/29|CP63|PC-N
P|codex_satchel|a stiff bark-paper codex satchel|paper|S/G|440/25|CP87|PC-L
P|cacao_purse|a small woven cacao-bean pouch|cotton|VS/S|110/7|CP65|PC-N
D|cacao_cup|a painted cylindrical cacao cup|earthenware|T/G|210/7|CP33|DW-L,WATER
D|tripod_bowl|a painted tripod serving bowl|earthenware|S/G|630/11|CP32|DW-L,WATER
D|gourd_cup|a polished Mesoamerican gourd cup|gourd shell|T/S|60/2|CP78|DW-N,WATER
D|storage_jar|a tall painted tribute storage jar|earthenware|VL/G|11500/55|CP38|DW-L,WATER
D|food_basket|a close-woven lidded market basket|wicker|N/S|1700/12|CP67|DW-N,POROUS
D|wooden_platter|a broad carved market platter|wood|N/G|2000/32|CP76|DW-L,OPEN
```

### P19 — Andean household and storehouse

Complete Andean package. Generic trade sacks, bundles, chests, and colonial packages remain shared where applicable. Retained rows add quipu-specific narrow storage, vessel cradling, raised storehouse installations, camelid-textile wearable containers, or distinctive pointed-jar and flared-cup forms.

```text
T|quipu_record_chest|a fitted quipu record chest|wood|N/G|14000/119|CP22|TR-L
T|textile_bale_chest|a broad camelid-textile bale chest|wood|L/G|28000/172|CP24|TR-L
T|maize_store_basket|a deep lidded maize-store basket|wicker|L/S|4100/20|CP69|TR-N,POROUS
T|metal_ornament_casket|a divided metal-ornament casket|wood|S/G|2400/63|CP66|TR-L
T|chicha_vessel_cradle|a padded chicha-vessel cradle|wicker|L/S|4200/18|CP62|TR-N,OPEN
F|quipu_cord_rack|a many-pegged quipu cord rack|wood|VL/G|55000/262|CP18|FU-L
F|raised_storage_shelf|a raised Andean household shelf|wood|VL/S|55000/150|CP18|FU-N
F|textile_chest|a lidded camelid-textile chest|wood|VL/G|48000/271|CP02|FU-L
F|jar_stand|a braced Andean ceramic-jar stand|wood|VL/S|55000/150|CP18|FU-N
F|measuring_bin|a fixed storehouse measuring bin|wood|L/S|38000/95|CP12|FU-N,OPEN
P|coca_pouch|a patterned camelid-wool coca pouch|camelid wool|VS/G|120/13|CP65|PC-L
P|quipu_satchel|a fitted quipu-keeper's satchel|camelid wool|S/G|620/43|CP61|PC-L
P|carrying_bag|a broad Andean woven carrying bag|camelid wool|N/S|1400/32|CP63|PC-N
P|gourd_bottle_sling|a woven Andean gourd-bottle sling|gourd shell|S/S|490/5|CP82|PC-N,WATER
D|pointed_storage_jar|a pointed-base ceramic storage jar|earthenware|VL/G|11500/55|CP38|DW-L,WATER
D|flared_drinking_cup|a flared carved wooden drinking cup|wood|T/G|180/10|CP78|DW-L,WATER
D|double_serving_bowl|a joined double ceramic serving bowl|earthenware|S/G|630/11|CP32|DW-L,WATER
D|woven_basket|a close-lidded Andean household basket|wicker|N/S|1700/12|CP67|DW-N,POROUS
D|cooking_pot|a round-bellied Andean clay pot|clay|L/S|5200/28|CP36|DW-N,WATER
D|wooden_platter|a shallow carved Andean platter|wood|N/G|2000/32|CP76|DW-L,OPEN
```

### P20 — Caribbean, North American, and colonial Atlantic contact post

Complete Atlantic contact package. Shared colonial commodity packaging, tools, beads, sacks, crates, and mission-adjacent writing goods require explicit admission and are not cloned. Retained rows add fitted exchange/assay capacity, installed contact-post furniture, or Indigenous and hybrid container forms; coercive colonial contexts must remain intentional in builder notes and placement.

```text
T|mission_supply_chest|a compartmented mission supply chest|oak|L/G|28000/172|CP24|TR-L,RELIGIOUS
T|iron_tool_exchange_chest|a fitted iron-tool exchange chest|oak|N/G|18000/161|CP25|TR-L
T|shell_bead_trade_coffer|a divided shell-bead trade coffer|wood|S/G|2400/63|CP66|TR-L
T|glass_bead_casket|a padded contact glass-bead casket|cedar|S/G|2400/85|CP66|TR-L
T|assay_strongbox|a heavy colonial assay strongbox|wrought iron|L/G|78000/543|CP55|TR-L
F|mission_cupboard|a deep mission-house cupboard|oak|VL/S|78000/210|CP05|FU-N,RELIGIOUS
F|trade_counter|a broad contact-post trade counter|oak|H/S|105000/260|CP04|FU-N
F|burden_basket_rack|a tall burden-basket rack|wood|VL/S|55000/150|CP18|FU-N
F|refectory_table|a long colonial refectory table|oak|H/S|118000/300|CP11|FU-N
F|archive_chest|a fixed port-and-mission archive chest|oak|VL/G|48000/271|CP02|FU-L,RELIGIOUS,MARITIME
P|cotton_shoulder_bag|a woven Caribbean cotton shoulder bag|cotton|N/S|540/15|CP75|PC-N
P|hide_trade_pouch|a soft contact-zone hide pouch|leather|VS/S|190/10|CP65|PC-N
P|shell_bead_pouch|a narrow shell-bead belt pouch|leather|VS/G|190/18|CP65|PC-L
P|birchbark_document_box|a folded birch-bark document box|birch bark|S/G|500/34|CP85|PC-L
D|caribbean_gourd_cup|a polished Caribbean gourd cup|gourd shell|T/S|60/2|CP78|DW-N,WATER
D|carved_wooden_bowl|a deep Caribbean carved bowl|wood|S/G|550/18|CP77|DW-L,WATER
D|cassava_tray|a broad woven cassava-serving tray|wicker|N/S|1200/12|CP76|DW-N,OPEN
D|colonial_pitcher|a green-glazed colonial pitcher|earthenware|N/S|2750/16|CP35|DW-N,WATER
D|mission_plate|a plain pewter mission plate|pewter|S/S|980/18|CP43|DW-N,RELIGIOUS,OPEN
D|storage_jar|a broad glazed contact-zone jar|earthenware|VL/S|11500/31|CP38|DW-N,WATER
```

## Implementation contract and validation

- Fixed furniture omits `Holdable`; portable trade, personal, and domestic goods include it. Personal rows always include a belt or wearable carrying component.
- Liquid vessels are finite and non-self-refilling; never substitute `WaterSource` mechanics.
- Do not stack dry- and liquid-container providers, or another provider onto the requested till, basket, box, cup, bowl, basin, ewer, pitcher, pot, jar, or vat profiles.
- Contact, mission, tribute, and colonial rows require deliberate builder notes and placement; catalogue presence does not make those systems neutral or universal.
- Validation: 400 rows; 400 unique stable references; 400 unique sdescs; 147 shared aliases identified; no component/material/interface/portability errors.

