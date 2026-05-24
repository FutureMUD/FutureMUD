# Antiquity Item Component Gap Report

This document records proposed stock-content additions for the antiquity item seeder after comparing the current antiquity item suite against the generic component prototypes seeded by `UsefulSeeder`, plus health and animal-butchery stock outputs.

The goal is not to replace the existing antiquity catalogue. It is a forward-looking implementation guide for logical item groups that fit the technology level, social setting, and current MUD functionality.

## Existing Coverage Baseline

The current antiquity item suite already has broad coverage for:

- clothing, jewellery, armour, weapons, shields, belts, sheaths, insulation, and worn variables.
- furniture, containers, liquid vessels, amphorae, tableware, doors, gates, latches, keys, keyrings, lighting, braziers, and repair kits.
- food and beverage prototypes, prepared-food components, fermenting and aging vessels, food tools, and culture-gated foodway stock.
- writing surfaces and documents using `PaperSheet`, `Book`, `ScribingImplement`, and `InscribableSurface` components.
- medical supplies using treatment, pill, topical cream, smokeable, immobilising, crutch, and prosthetic component families.
- butchery outputs for raw meat, hides, offal, bones, fat, venom organs, feathers, scales, chitin, shell, teeth, claws, horns, antlers, tusks, and mythical animal products.

## Data-Only Item Additions Using Existing Component Prototypes

These items can be added to the antiquity seeder without new runtime component types. Some use stock generic components exactly as seeded; others would be better if the component prototype names were later made more setting-specific, but they are still implementable today.

### Timekeeping Items

Current gap: `TimePiece` exists in the stock component catalogue, but antiquity does not currently have visible sundials, water clocks, watch clocks, or temple timekeepers.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_stone_courtyard_sundial` | a carved stone courtyard sundial | Heavy fixed object. Use `TimePiece_WallClock` or `TimePiece_Standard`, `Destroyable_Misc`, and a stone material. Good for temples, villas, fora, agoras, and public gardens. |
| `antiquity_bronze_portable_gnomon` | a bronze portable gnomon | Small carried item. Use `TimePiece_Standard`, `Holdable`, and `Destroyable_Misc`. Useful for travellers, surveyors, priests, or military officers. |
| `antiquity_marked_watch_candle` | a marked wax watch candle | Consumable-looking time marker. Use `TimePiece_Standard`, `Holdable`, and `Destroyable_Misc`; consider a morph or craft chain later if candle burning should matter mechanically. |
| `antiquity_temple_water_clock` | a clay temple water clock | Stationary civic or ritual item. Use `TimePiece_WallClock`, `Container_Small_Table` or a similar support surface only if it should hold contents, and `Destroyable_Misc`. |
| `antiquity_watchman_hour_board` | a painted watchman's hour board | Simple public schedule object. Use `TimePiece_WallClock`, `Destroyable_WoodenHeavy`, and optionally `InscribableSurface` if it should double as an editable schedule board. |

### Public Water and Bathing Infrastructure

Current gap: `WaterSource` exists in the generic stock set and the legacy item seeder has wells and cisterns, but the antiquity rework does not yet make public water infrastructure part of its own setting package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_stone_public_well` | a stone public well | Fixed source. Use `Infinite_WaterSource` or `Infinite_SpringWaterSource`, `Destroyable_Furniture`, and stone material. Good village, fort, and shrine fixture. |
| `antiquity_lined_cistern` | a lined stone cistern | Fixed source/storage object. Use `Infinite_WaterSource` or a new cistern water-source prototype if added later; add `Destroyable_Furniture`. |
| `antiquity_bronze_fountain_spout` | a bronze fountain spout | Public source tied to urban infrastructure. Use `Infinite_WaterSource` and `Destroyable_Misc`; bronze or stone depending on item body. |
| `antiquity_bathhouse_plunge_pool` | a tiled bathhouse plunge pool | Large fixed water source. Use `Infinite_WaterSource` or `WaterSource_ProgControlled`, `Destroyable_Furniture`, and a stone or tile material. |
| `antiquity_temple_purification_basin` | a temple purification basin | Ritual source. Use `Infinite_WaterSource`, `Container_Large_Table` only if it should receive offerings or tools, and `Destroyable_Misc`. |
| `antiquity_irrigation_channel_outlet` | an irrigation channel outlet | Agricultural source. Use `Infinite_RiverWaterSource` or `WaterSource_ProgControlled`; best paired with room/zone placement rather than loaded as portable stock. |

### Drag Aids, Litters, and Heavy-Carry Tools

Current gap: `DragAid` exists in the general stock component package, while antiquity medical currently covers crutches and prosthetics but not group movement aids.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_canvas_field_stretcher` | a canvas field stretcher | Use `DragAid_Stretcher`, `Holdable`, and `Destroyable_Misc`. Medical, military, and disaster-response item. |
| `antiquity_wooden_corpse_bier` | a wooden corpse bier | Use `DragAid_Stretcher` or `DragAid_Sled`, `Destroyable_WoodenHeavy`, and a wooden material. Good for funerary and temple contexts. |
| `antiquity_rope_carrying_sling` | a rope carrying sling | Use `DragAid_Sling`, `Wear_*` or `Holdable` depending on how it is carried, and `Destroyable_Misc`. |
| `antiquity_pack_travois` | a rawhide pack travois | Use `DragAid_Travois`, `Destroyable_WoodenHeavy`, and leather/wood materials. Good nomadic, pastoral, and frontier content. |
| `antiquity_cargo_sled` | a low wooden cargo sled | Use `DragAid_Sled`, `Destroyable_WoodenHeavy`, and a large size. Supports quarry, warehouse, dock, and military supply scenes. |

### Games, Gambling, and Leisure

Current gap: `Dice` components are seeded, including fair and custom-face dice support, but antiquity does not currently seed gambling or leisure objects as a visible social package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_bone_knucklebones` | a set of bone knucklebones | Use `Dice_d4` or a custom `Dice` prototype with named faces if adding a new component prototype. Add `Holdable` and `Destroyable_Misc`. |
| `antiquity_ivory_six_sided_die` | an ivory six-sided die | Use `Dice_d6`, `Holdable`, and `Destroyable_Misc`. Good tavern, camp, and elite-gaming item. |
| `antiquity_casting_sticks` | a bundle of marked casting sticks | Use `Dice_Fudge` or a custom-face dice prototype. Add `Holdable`; tag for divination or gambling depending on intended use. |
| `antiquity_senet_game_board` | a painted senet game board | Data-only version can be a `Holdable`/`Container_Tray` prop with counters; a richer version belongs under a future `GameSet` component. |
| `antiquity_latrunculi_board` | a gridded latrunculi board | As above: board and counters as props now, later backed by a rules-aware game component if implemented. |
| `antiquity_tavern_gambling_cup` | a leather dice cup | Use `Container_Cup`-like support if available or `Holdable` only, with `Destroyable_Misc`. Pairs with dice items rather than requiring its own component. |

### Markets, Stalls, Weights, and Measures

Current gap: the runtime has `ShopStall` and `MarketGoodWeight`, but the antiquity package mostly models merchant furniture and containers rather than functional market instruments.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_market_canvas_stall` | a canvas-covered market stall | Use `ShopStall` if a stock component prototype is added, otherwise `Container_Counter`, `Table_*`, and `Destroyable_Furniture` as a prop. |
| `antiquity_market_counter_chest` | a lockable merchant counter chest | Use `ShopStall` if available, or `LockingContainer_Lockbox` plus `Container_Counter` style composition where exclusivity allows. |
| `antiquity_bronze_standard_weight_set` | a bronze standard weight set | Use `MarketGoodWeight` if a component prototype is added, plus `Container_Pouch` or `Container_Small_Cabinet` as appropriate. |
| `antiquity_stone_grain_measure` | a stone grain measure | Prop version uses `Holdable`, `Destroyable_Misc`, and market/standard-tool tags. A functional version wants a measurement component. |
| `antiquity_wooden_measuring_rod` | a marked wooden measuring rod | Use `Holdable`, `Destroyable_Misc`, and professional-tool tags; later could be a component-backed measuring instrument. |
| `antiquity_balance_scale` | a bronze balance scale | Prop version uses `Holdable` or furniture support plus `Destroyable_Misc`. Functional weighing needs a new component type. |

### Locksmithing and Security Tools

Current gap: antiquity has locks, keys, latches, and keyrings, while generic stock already has `Locksmithing Tool` components. The setting can support a small security-tool package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_bronze_lockpick_set` | a bronze lockpick set | Use `Locksmithing_Poor` or `Locksmithing_Standard`, `Holdable`, and `Destroyable_Misc`. Good for thieves, guards, and locksmiths. |
| `antiquity_locksmith_probe_roll` | a roll of lock probes | Use `Locksmithing_Standard` or `Locksmithing_Fine`; small portable item with leather material and a tool tag. |
| `antiquity_lock_installation_kit` | a locksmith's installation kit | Use `Locksmithing_Installation`, `Container_Pouch` or `Container_Trunk`, and `Destroyable_Misc`. |
| `antiquity_key_filing_kit` | a key filing kit | Use `Locksmithing_Fabrication`, `Holdable`, and metalworking/locksmithing tags. |
| `antiquity_guard_key_cord` | a cord of guard keys | Use `Keyring_Large`, `Wear_*` or `Holdable`, and `Destroyable_Misc`. Complements existing keyring coverage with a civic/security flavour. |

### Civic Notice and Administrative Boards

Current gap: `Board` exists as a message-board component, but there is no antiquity-specific public posting surface.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_forum_notice_board` | a whitewashed forum notice board | Use `Board`, `Destroyable_WoodenHeavy`, and an appropriate board record. Configure view/post progs for public, clan, court, or market use. |
| `antiquity_temple_decree_board` | a temple decree board | Use `Board`, `Destroyable_WoodenHeavy`, and restricted post permissions for priests or officials. |
| `antiquity_barracks_roster_board` | a barracks roster board | Use `Board` or `InscribableSurface`; `Board` is best if postings need persistence and access rules. |

## Missing Seeded Component Prototypes Within Existing Component Types

These gaps do not require brand-new engine functionality. They need new component prototypes, seeded by `UsefulSeeder` or by the antiquity package before items reference them. The item additions listed here become cleaner and less misleading once these prototypes exist.

### Antiquity TimePiece Variants

Missing: setting-specific `TimePiece` component prototypes with antiquity display strings and settable/non-settable behaviour.

Needed prototypes:

- `TimePiece_Antiquity_Sundial`: non-settable, crude daylight-style display string.
- `TimePiece_Antiquity_WaterClock`: non-settable or priest/setter-only, suited to temple/court clocks.
- `TimePiece_Antiquity_MarkedCandle`: portable, coarse display string, optionally paired with future burn/morph behaviour.
- `TimePiece_Antiquity_WatchBoard`: public watch-shift display tied to the default clock and timezone.

Items enabled or improved:

- `antiquity_stone_courtyard_sundial`
- `antiquity_bronze_portable_gnomon`
- `antiquity_marked_watch_candle`
- `antiquity_temple_water_clock`
- `antiquity_watchman_hour_board`

### Antiquity WaterSource Variants

Missing: fixed ancient water-source component prototypes with names, capacities, switches, and liquid defaults that do not read like modern sinks, bottles, or bathtubs.

Needed prototypes:

- `WaterSource_Antiquity_PublicWell`: infinite or large-capacity potable water source.
- `WaterSource_Antiquity_Cistern`: potable or brackish stored source, optionally switchable/lockable for city control.
- `WaterSource_Antiquity_Fountain`: public flowing source.
- `WaterSource_Antiquity_BathPool`: large bathhouse water source.
- `WaterSource_Antiquity_RitualBasin`: temple-purification source, probably finite or prog-controlled.
- `WaterSource_Antiquity_IrrigationOutlet`: agricultural or room-fixture source.

Items enabled or improved:

- `antiquity_stone_public_well`
- `antiquity_lined_cistern`
- `antiquity_bronze_fountain_spout`
- `antiquity_bathhouse_plunge_pool`
- `antiquity_temple_purification_basin`
- `antiquity_irrigation_channel_outlet`

### Antiquity ShopStall and MarketGoodWeight Variants

Missing: seeded stock component prototypes for market stalls and market-good weights that game owners can attach without hand-authoring each example.

Needed prototypes:

- `ShopStall_Antiquity_OpenCounter`: open stall surface with generous weight capacity and no transparent-container weirdness.
- `ShopStall_Antiquity_LockableCounter`: lockable market counter for unattended goods.
- `ShopStall_Antiquity_PortableBooth`: lighter stall for fairs, camps, and travelling merchants.
- `MarketGoodWeight_Antiquity_StapleFood`: market weight multiplier for grain, bread, oil, wine, beer, or other staple categories once local market categories exist.
- `MarketGoodWeight_Antiquity_LuxuryCraft`: multiplier for jewellery, glass, dyed cloth, incense, or fine pottery categories.
- `MarketGoodWeight_Antiquity_MilitarySupply`: multiplier for weapons, armour, arrows, shields, pack gear, and repair stock.

Items enabled or improved:

- `antiquity_market_canvas_stall`
- `antiquity_market_counter_chest`
- `antiquity_bronze_standard_weight_set`
- `antiquity_stone_grain_measure`
- `antiquity_merchant_tax_tablet`
- `antiquity_armoury_supply_counter`

### Custom Dice and Casting Components

Missing: antiquity-specific dice prototypes with non-modern face labels and loaded/fair variants.

Needed prototypes:

- `Dice_Antiquity_Knucklebones`: four named faces or weighted values suitable for astragaloi.
- `Dice_Antiquity_CastingSticks`: marked/unmarked stick faces.
- `Dice_Antiquity_LoadedD6`: cheating example for taverns and gambling dens.
- `Dice_Antiquity_DivinationLots`: symbolic faces for temple, oracle, or folk-divination use.

Items enabled or improved:

- `antiquity_bone_knucklebones`
- `antiquity_casting_sticks`
- `antiquity_ivory_loaded_die`
- `antiquity_divination_lots`
- `antiquity_tavern_gambling_set`

### DragAid Tuning Variants

Missing: setting-specific drag-aid components with tuned user limits and effort multipliers for medical, funerary, pastoral, and cargo contexts.

Needed prototypes:

- `DragAid_Antiquity_FieldStretcher`: two to four users, high wounded-person movement multiplier.
- `DragAid_Antiquity_CorpseBier`: two to six users, funerary/corpse-focused naming.
- `DragAid_Antiquity_CargoSled`: one to four users, cargo-heavy multiplier.
- `DragAid_Antiquity_PackTravois`: one to two users, pastoral or frontier cargo support.
- `DragAid_Antiquity_CarryingSling`: one user, moderate multiplier for small loads.

Items enabled or improved:

- `antiquity_canvas_field_stretcher`
- `antiquity_wooden_corpse_bier`
- `antiquity_cargo_sled`
- `antiquity_pack_travois`
- `antiquity_rope_carrying_sling`

### Antiquity Locksmithing Variants

Missing: tuned antiquity locksmithing tool components, especially low-tech, breakable, or installation/fabrication-specialised versions.

Needed prototypes:

- `Locksmithing_Antiquity_BronzePoor`: breakable, penalty-bearing improvised pick set.
- `Locksmithing_Antiquity_BronzeStandard`: standard lockpick and probe roll.
- `Locksmithing_Antiquity_FineSteel`: rare fine lockpicks for late-antique or elite toolkits.
- `Locksmithing_Antiquity_Installation`: installation/configuration kit for builders and locksmith NPCs.
- `Locksmithing_Antiquity_Fabrication`: key filing and fabrication kit.

Items enabled or improved:

- `antiquity_bronze_lockpick_set`
- `antiquity_locksmith_probe_roll`
- `antiquity_fine_steel_lockpick_case`
- `antiquity_lock_installation_kit`
- `antiquity_key_filing_kit`

## Missing Engine Functionality That Could Become New Component Types

These gaps are not merely missing stock data. They describe gameplay that does not appear to be represented by a suitable current item component type, or where existing components would only produce a static prop.

### Instrument Component

Missing functionality:

- playable musical instruments with room and adjacent-room echoes.
- skill-checked performance quality.
- instrument loudness, range, hand requirements, and posture restrictions.
- optional repeated performance or sustained playing state.
- optional prog hooks for ritual, morale, stealth disruption, crowd reaction, military signalling, or cultural recognition.

Rough component guidance:

- Component type name: `Instrument`.
- Prototype settings: instrument family, performance trait/check, loudness/range, required hands, allowed postures, default play emote, failure emote, style/tune labels, optional `CanPlayProg`, optional `OnPlayProg`.
- Runtime commands: `play <instrument> [style]`, possibly `stop playing`.
- Interfaces should expose whether the item is currently being played and the audible echo profile.

Items enabled:

- `antiquity_wooden_lyre`
- `antiquity_kithara`
- `antiquity_reed_flute`
- `antiquity_double_aulos`
- `antiquity_frame_drum`
- `antiquity_sistrum`
- `antiquity_bronze_war_horn`
- `antiquity_ship_signal_trumpet`
- `antiquity_temple_ritual_rattle`

### SealStamp and SealedDocument Components

Missing functionality:

- authenticating documents, containers, orders, and tablets with a seal.
- preserving identity of the seal owner, clan, office, or issuer.
- visible seal impressions that can be inspected, forged, broken, or compared.
- tamper evidence for scrolls, tablets, storage jars, chests, doors, and official packages.

Rough component guidance:

- Component type names: `SealStamp` and `Sealable`.
- `SealStamp` settings: seal design text, owner/clan/office metadata, material, forgery difficulty, optional authority prog.
- `Sealable` settings: allowed seal media, current seal state, whether opening breaks the seal, inspection difficulty, whether a broken seal leaves residue.
- Runtime commands: `seal <target> with <stamp> [using <wax/clay>]`, `break seal <target>`, `inspect seal <target>`, possibly `compare seal`.
- FutureProg hooks should be able to read issuer, broken/unbroken state, and design metadata.

Items enabled:

- `antiquity_bronze_signet_ring`
- `antiquity_cylinder_seal`
- `antiquity_clay_bulla`
- `antiquity_wax_seal_cake`
- `antiquity_sealed_papyrus_scroll`
- `antiquity_sealed_clay_tablet`
- `antiquity_tax_office_seal_box`
- `antiquity_merchant_contract_bundle`
- `antiquity_grain_amphora_seal`

### Scale and Measuring Instrument Component

Missing functionality:

- weighing loose commodities and ordinary items.
- measuring volume or length with a physical in-world tool.
- producing trusted or falsifiable measures for trade, taxation, and legal disputes.
- modelling honest and dishonest weights or calibration errors.

Rough component guidance:

- Component type name: `MeasuringInstrument`, with optional subtypes or modes for weight, volume, and length.
- Prototype settings: measurement mode, precision, capacity, units displayed, calibration error, required counterweight/container, check difficulty to detect cheating.
- Runtime commands: `weigh <item> on <scale>`, `measure <liquid/commodity/item> with <instrument>`, `calibrate <instrument>`, `inspect calibration`.
- Market and economy code should be able to use measured quantity where appropriate, but the first pass can simply report measures to players.

Items enabled:

- `antiquity_bronze_balance_scale`
- `antiquity_standard_weight_set`
- `antiquity_false_weight_set`
- `antiquity_stone_grain_measure`
- `antiquity_oil_measure_cup`
- `antiquity_wine_measure_cup`
- `antiquity_surveying_rod`
- `antiquity_cubit_rod`
- `antiquity_tax_assessor_measure_kit`

### GameSet Component

Missing functionality:

- board games or counter games with stateful pieces, turn order, legal moves, and spectators.
- gambling hooks that are more structured than rolling dice.
- persistence of a game-in-progress on an item.

Rough component guidance:

- Component type name: `GameSet`.
- Prototype settings: game family, board layout, piece definitions, max players, whether spectators can inspect, whether random setup is used, optional skill/check hooks.
- Runtime commands: `game start`, `game join`, `game move`, `game concede`, `game reset`, and `look <board>` state rendering.
- Keep first implementation rules-light: enough for move logging, piece placement, and social play before modelling every historical rule set.

Items enabled:

- `antiquity_senet_game_board`
- `antiquity_mehen_game_board`
- `antiquity_latrunculi_board`
- `antiquity_royal_game_board`
- `antiquity_mancala_board`
- `antiquity_temple_divination_board`
- `antiquity_tavern_game_set`

### Offering and Ritual Focus Component

Missing functionality:

- offerings that are consumed, counted, accepted, rejected, spoiled, or recorded.
- altars, censers, lamps, and libation tables that do more than hold items.
- ritual state that FutureProgs, NPCs, clans, laws, or magic systems can inspect.

Rough component guidance:

- Component type names: `OfferingReceiver` and optionally `RitualFocus`.
- Prototype settings: accepted item tags/materials/liquids, consumption behaviour, required actor checks, accepted-offering echo, rejected-offering echo, optional cooldown, optional owner clan/religion/cult metadata.
- Runtime commands: `offer <item> at <focus>`, `pour <liquid> as offering`, `light incense at <focus>`, or reuse existing put/pour commands with component interception.
- FutureProg hooks should read recent offerings, actor, accepted item, quantity, and focus identity.

Items enabled:

- `antiquity_stone_household_altar`
- `antiquity_temple_libation_table`
- `antiquity_bronze_incense_censer`
- `antiquity_votive_figurine_basin`
- `antiquity_oil_lamp_shrine`
- `antiquity_funeral_offering_tray`
- `antiquity_oracular_tripod`
- `antiquity_blood_offering_bowl`

### Animal Tack and Harness Component

Missing functionality:

- tack that meaningfully connects a character, animal, cart, chariot, or pack load.
- pack saddles and panniers that increase carried cargo in a mount-aware way.
- harnesses and yokes that let animals pull vehicles or drag aids without being treated like ordinary worn clothing.
- reins, bridles, and control gear that can affect riding, leading, or animal handling checks.

Rough component guidance:

- Component type names: `AnimalTack`, `Harness`, or a small family split by role.
- Prototype settings: supported body plans, supported animal sizes, tack role, cargo multiplier, control modifier, required worn locations, whether the tack can link to a vehicle or drag aid.
- Runtime hooks should integrate with mount, drag, vehicle, and animal-handling systems rather than being purely item-local.
- This is a larger cross-subsystem pass and should not be slipped into a small item-seeder-only change.

Items enabled:

- `antiquity_leather_bridle`
- `antiquity_pack_saddle`
- `antiquity_mule_pannier_set`
- `antiquity_ox_yoke`
- `antiquity_chariot_harness`
- `antiquity_camel_cargo_saddle`
- `antiquity_warhorse_barding_harness`
- `antiquity_rope_lead_halter`

## Suggested Implementation Order

1. Add data-only items that use existing components cleanly: dice, lockpicks, drag aids, notice boards, and a first water-source set.
2. Add antiquity-specific component prototypes for `TimePiece`, `WaterSource`, `DragAid`, `Dice`, `Locksmithing Tool`, `ShopStall`, and `MarketGoodWeight`.
3. Add matching item prototypes that use those new component prototypes and update this document with the shipped names.
4. Choose one new runtime component family for a first deeper gameplay pass. `Instrument` is the best first candidate because it is socially important, highly visible in roleplay, and relatively self-contained.
5. Treat `SealStamp`/`Sealable`, `MeasuringInstrument`, and `OfferingReceiver` as the next most setting-defining component families because they support administration, trade, religion, law, and trust.
