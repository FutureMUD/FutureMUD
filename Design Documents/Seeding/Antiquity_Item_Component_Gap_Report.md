# Antiquity Item Component Gap Report

This document records proposed stock-content additions for the antiquity item seeder after comparing the current antiquity item suite against the generic component prototypes seeded by `UsefulSeeder`, plus health and animal-butchery stock outputs.

The goal is not to replace the existing antiquity catalogue. It is a forward-looking implementation guide for logical item groups that fit the technology level, social setting, and current MUD functionality.

Cross-era foundation note: genuinely shared workshop foundations use `historic_*` stable references in `ItemSeeder.HistoricFoundation.cs` and are seeded for either antiquity or medieval installs. Antiquity culture-specific clothing, weapons, jewellery, foodways, and document forms remain under their existing `antiquity_*` references.

Current review status: the implemented data-only, seal/sealable, weight/fluid measurement, incense, V1 item-offering, and animal-tack work below was verified or completed rather than reimplemented. The remaining 21 item references are consolidated under `Instrument` (9), `GameSet` (7), dimension-aware `MeasuringInstrument` (1), and extended `OfferingReceiver` (4) in the [item content engine dependency ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md).

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

Seeder status: the item prototypes listed in this section are now seeded by `ItemSeeder.AntiquityComponentGaps.cs`. Items whose richer behaviour depends on still-deferred systems, such as physical length measurement or rules-aware board games, are seeded as ordinary props.

### Timekeeping Items

Original gap: `TimePiece` exists in the stock component catalogue, but antiquity did not have visible sundials, water clocks, watch clocks, or temple timekeepers.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_stone_courtyard_sundial` | a carved stone courtyard sundial | Heavy fixed object. Use `TimePiece_WallClock` or `TimePiece_Standard`, `Destroyable_Misc`, and a stone material. Good for temples, villas, fora, agoras, and public gardens. |
| `antiquity_bronze_portable_gnomon` | a bronze portable gnomon | Small carried item. Use `TimePiece_Standard`, `Holdable`, and `Destroyable_Misc`. Useful for travellers, surveyors, priests, or military officers. |
| `antiquity_marked_watch_candle` | a marked wax watch candle | Consumable-looking time marker. Use `TimePiece_Standard`, `Holdable`, and `Destroyable_Misc`; consider a morph or craft chain later if candle burning should matter mechanically. |
| `antiquity_temple_water_clock` | a clay temple water clock | Stationary civic or ritual item. Use `TimePiece_WallClock`, `Container_Small_Table` or a similar support surface only if it should hold contents, and `Destroyable_Misc`. |
| `antiquity_watchman_hour_board` | a painted watchman's hour board | Simple public schedule object. Use `TimePiece_WallClock`, `Destroyable_WoodenHeavy`, and optionally `InscribableSurface` if it should double as an editable schedule board. |

### Public Water and Bathing Infrastructure

Original gap: `WaterSource` exists in the generic stock set and the legacy item seeder has wells and cisterns, but the antiquity rework did not make public water infrastructure part of its own setting package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_stone_public_well` | a stone public well | Fixed source. Use `Infinite_WaterSource` or `Infinite_SpringWaterSource`, `Destroyable_Furniture`, and stone material. Good village, fort, and shrine fixture. |
| `antiquity_lined_cistern` | a lined stone cistern | Fixed source/storage object. Use `Infinite_WaterSource` or a new cistern water-source prototype if added later; add `Destroyable_Furniture`. |
| `antiquity_bronze_fountain_spout` | a bronze fountain spout | Public source tied to urban infrastructure. Use `Infinite_WaterSource` and `Destroyable_Misc`; bronze or stone depending on item body. |
| `antiquity_bathhouse_plunge_pool` | a tiled bathhouse plunge pool | Large fixed water source. Use `Infinite_WaterSource` or `WaterSource_ProgControlled`, `Destroyable_Furniture`, and a stone or tile material. |
| `antiquity_temple_purification_basin` | a temple purification basin | Ritual source. Use `Infinite_WaterSource`, `Container_Large_Table` only if it should receive offerings or tools, and `Destroyable_Misc`. |
| `antiquity_irrigation_channel_outlet` | an irrigation channel outlet | Agricultural source. Use `Infinite_RiverWaterSource` or `WaterSource_ProgControlled`; best paired with room/zone placement rather than loaded as portable stock. |

### Drag Aids, Litters, and Heavy-Carry Tools

Original gap: `DragAid` exists in the general stock component package, while antiquity medical covered crutches and prosthetics but not group movement aids.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_canvas_field_stretcher` | a canvas field stretcher | Use `DragAid_Stretcher`, `Holdable`, and `Destroyable_Misc`. Medical, military, and disaster-response item. |
| `antiquity_wooden_corpse_bier` | a wooden corpse bier | Use `DragAid_Stretcher` or `DragAid_Sled`, `Destroyable_WoodenHeavy`, and a wooden material. Good for funerary and temple contexts. |
| `antiquity_rope_carrying_sling` | a rope carrying sling | Use `DragAid_Sling`, `Wear_*` or `Holdable` depending on how it is carried, and `Destroyable_Misc`. |
| `antiquity_pack_travois` | a rawhide pack travois | Use `DragAid_Travois`, `Destroyable_WoodenHeavy`, and leather/wood materials. Good nomadic, pastoral, and frontier content. |
| `antiquity_cargo_sled` | a low wooden cargo sled | Use `DragAid_Sled`, `Destroyable_WoodenHeavy`, and a large size. Supports quarry, warehouse, dock, and military supply scenes. |

### Games, Gambling, and Leisure

Original gap: `Dice` components are seeded, including fair and custom-face dice support, but antiquity did not seed gambling or leisure objects as a visible social package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_bone_knucklebones` | a set of bone knucklebones | Use `Dice_d4` or a custom `Dice` prototype with named faces if adding a new component prototype. Add `Holdable` and `Destroyable_Misc`. |
| `antiquity_ivory_six_sided_die` | an ivory six-sided die | Use `Dice_d6`, `Holdable`, and `Destroyable_Misc`. Good tavern, camp, and elite-gaming item. |
| `antiquity_casting_sticks` | a bundle of marked casting sticks | Use `Dice_Fudge` or a custom-face dice prototype. Add `Holdable`; tag for divination or gambling depending on intended use. |
| `antiquity_senet_game_board` | a painted senet game board | Data-only version can be a `Holdable`/`Container_Tray` prop with counters; a richer version belongs under a future `GameSet` component. |
| `antiquity_latrunculi_board` | a gridded latrunculi board | As above: board and counters as props now, later backed by a rules-aware game component if implemented. |
| `antiquity_tavern_gambling_cup` | a leather dice cup | Use `Container_Cup`-like support if available or `Holdable` only, with `Destroyable_Misc`. Pairs with dice items rather than requiring its own component. |

### Markets, Stalls, Weights, and Measures

Original gap: the runtime has `ShopStall` and `MarketGoodWeight`, but the antiquity package mostly modelled merchant furniture and containers rather than functional market instruments.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_market_canvas_stall` | a canvas-covered market stall | Use `ShopStall` if a stock component prototype is added, otherwise `Container_Counter`, `Table_*`, and `Destroyable_Furniture` as a prop. |
| `antiquity_market_counter_chest` | a lockable merchant counter chest | Use `ShopStall` if available, or `LockingContainer_Lockbox` plus `Container_Counter` style composition where exclusivity allows. |
| `antiquity_bronze_standard_weight_set` | a bronze standard weight set | Use `MarketGoodWeight` if a component prototype is added, plus `Container_Pouch` or `Container_Small_Cabinet` as appropriate. |
| `antiquity_stone_grain_measure` | a stone grain measure | Prop version uses `Holdable`, `Destroyable_Misc`, and market/standard-tool tags. A functional version wants a measurement component. |
| `antiquity_wooden_measuring_rod` | a marked wooden measuring rod | Use `Holdable`, `Destroyable_Misc`, and professional-tool tags; later could be a component-backed measuring instrument. |
| `antiquity_balance_scale` | a bronze balance scale | Prop version uses `Holdable` or furniture support plus `Destroyable_Misc`. Functional weighing needs a new component type. |

### Locksmithing and Security Tools

Original gap: antiquity has locks, keys, latches, and keyrings, while generic stock already has `Locksmithing Tool` components. The setting can support a small security-tool package.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_bronze_lockpick_set` | a bronze lockpick set | Use `Locksmithing_Poor` or `Locksmithing_Standard`, `Holdable`, and `Destroyable_Misc`. Good for thieves, guards, and locksmiths. |
| `antiquity_locksmith_probe_roll` | a roll of lock probes | Use `Locksmithing_Standard` or `Locksmithing_Fine`; small portable item with leather material and a tool tag. |
| `antiquity_lock_installation_kit` | a locksmith's installation kit | Use `Locksmithing_Installation`, `Container_Pouch` or `Container_Trunk`, and `Destroyable_Misc`. |
| `antiquity_key_filing_kit` | a key filing kit | Use `Locksmithing_Fabrication`, `Holdable`, and metalworking/locksmithing tags. |
| `antiquity_guard_key_cord` | a cord of guard keys | Use `Keyring_Large`, `Wear_*` or `Holdable`, and `Destroyable_Misc`. Complements existing keyring coverage with a civic/security flavour. |

### Civic Notice and Administrative Boards

Original gap: `Board` exists as a message-board component, but there was no antiquity-specific public posting surface. The seeded board items are currently writable/inscribable props rather than live message-board bindings because board-backed component prototypes need world-specific board records and permissions.

| Unique Name | Short Description | Build Pointers |
| --- | --- | --- |
| `antiquity_forum_notice_board` | a whitewashed forum notice board | Use `Board`, `Destroyable_WoodenHeavy`, and an appropriate board record. Configure view/post progs for public, clan, court, or market use. |
| `antiquity_temple_decree_board` | a temple decree board | Use `Board`, `Destroyable_WoodenHeavy`, and restricted post permissions for priests or officials. |
| `antiquity_barracks_roster_board` | a barracks roster board | Use `Board` or `InscribableSurface`; `Board` is best if postings need persistence and access rules. |

## Seeded Component Prototypes Within Existing Component Types

These gaps did not require brand-new engine functionality. They are now seeded by `UsefulSeeder.ItemComponents.cs` as stock component prototypes so antiquity items can reference setting-appropriate variants instead of borrowing misleading generic or modern examples.

ItemSeeder status: the item prototypes named in the "Items enabled or improved" lists below are now seeded by `ItemSeeder.AntiquityComponentGaps.cs`.

### Antiquity TimePiece Variants

Seeder status: setting-specific `TimePiece` component prototypes with antiquity display strings and settable/non-settable behaviour are now present.

Seeded prototypes:

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

Seeder status: fixed ancient water-source component prototypes with names, capacities, switches, and liquid defaults that do not read like modern sinks, bottles, or bathtubs are now present.

Seeded prototypes:

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

Seeder status: seeded stock component prototypes for market stalls and market-good weights are now present for game owners to attach without hand-authoring each example.

Seeded prototypes:

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

Seeder status: antiquity-specific dice prototypes with non-modern face labels and loaded/fair variants are now present.

Seeded prototypes:

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

Seeder status: setting-specific drag-aid components with tuned user limits and effort multipliers for medical, funerary, pastoral, and cargo contexts are now present.

Seeded prototypes:

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

Seeder status: tuned antiquity locksmithing tool components, especially low-tech, breakable, or installation/fabrication-specialised versions, are now present.

Seeded prototypes:

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

## Implemented New Component Families

These gaps originally needed new runtime component families. They are now first-class component types with seeded examples in `UsefulSeeder.ItemComponents.cs`.

ItemSeeder status: the `SealStamp`, `Sealable`, and `MeasuringInstrument` item examples listed below are now seeded by `ItemSeeder.AntiquityComponentGaps.cs`. The wooden measuring rod is intentionally complete as a static prop. Decision: **will not implement** a dedicated mechanic for it; it is not waiting on a dimension system.

### SealStamp and Sealable Components

Runtime status: `SealStamp` and `Sealable` are implemented as separate component families. A stamp carries the authoritative design metadata, while `Sealable` is an attachable tamper-evidence component that can coexist with containers, writeable surfaces, openables, scrolls, envelopes, jars, or bundles.

Implemented behaviour:

- `seal <target> with <stamp> [using <medium>]` records the seal design, issuer, owner/clan/office metadata, stamp material, medium descriptor, sealing actor, and sealing time.
- `break seal <target>`, opening, reading, writing, drawing, or retitling a sealed item break the seal and leave configured residue evidence while allowing the access action to continue.
- `inspect seal <target>` and `compare seal <sealed item> with <stamp|sealed item>` use the existing appraisal-style inspection path rather than a new check type.
- Item FutureProg dot references expose whether an item is sealable, sealed, broken, or residue-marked, plus seal design, issuer, owner, clan, office, material, medium, and sealing actor metadata.

Seeded prototypes:

- `SealStamp_Antiquity_BronzeSignet`: bronze signet or ring seal for household, office, or personal authority.
- `SealStamp_Antiquity_CylinderSeal`: cylinder seal for clay impressions and administrative records.
- `Sealable_Document_Wax`: wax-sealable document surface.
- `Sealable_Document_Clay`: clay-sealable document or tablet surface.
- `Sealable_Envelope`: envelope seal support.
- `Sealable_Scroll`: scroll seal support.
- `Sealable_Container_Wax`: wax-sealed container, jar, chest, or package support.
- `Sealable_Antiquity_Clay_Tablet_Edge`, `Sealable_Antiquity_Clay_Bulla`, `Sealable_Antiquity_Papyrus_Letter`, `Sealable_Antiquity_Papyrus_Scroll`, `Sealable_Antiquity_Papyrus_Packet`, `Sealable_Antiquity_Wax_Tablet_Diptych`, `Sealable_Antiquity_Linen_Document_Bundle`, and `Sealable_Antiquity_Archive_Jar_Cap`: antiquity-specific tablet, scroll, packet, bundle, bulla, and archive-vessel seal support.
- `Sealable_Medieval_Parchment_Charter`, `Sealable_Medieval_Parchment_Roll`, `Sealable_Medieval_Rag_Paper_Letter`, `Sealable_Medieval_Official_Writ`, `Sealable_Medieval_East_Asian_Scroll`, `Sealable_Medieval_Palm_Leaf_Bundle`, `Sealable_Medieval_Document_Pouch`, and `Sealable_Medieval_Archive_Box`: medieval parchment, paper, scroll, pouch, and archive seal support keyed to the seeded medieval writing surfaces.
- `Sealable_Modern_Business_Envelope`, `Sealable_Modern_Padded_Envelope`, `Sealable_Modern_File_Folder`, `Sealable_Modern_Security_Envelope`, `Sealable_Modern_Evidence_Bag`, `Sealable_Modern_Registered_Mail_Pouch`, `Sealable_Modern_Courier_Tube`, `Sealable_Modern_Diplomatic_Pouch`, and `Sealable_Modern_Archive_Box`: modern envelope, evidence, courier, custody, and archive seal support using adhesive, security tape, numbered seal, plastic seal, and related media.

Items enabled or improved:

- `antiquity_bronze_signet_ring`
- `antiquity_cylinder_seal`
- `antiquity_clay_bulla`
- `antiquity_wax_seal_cake`
- `antiquity_sealed_papyrus_scroll`
- `antiquity_sealed_clay_tablet`
- `antiquity_tax_office_seal_box`
- `antiquity_merchant_contract_bundle`
- `antiquity_grain_amphora_seal`
- `Envelope` as a holdable, closable container, writeable surface, and sealable item.
- `Scroll` as a holdable, writeable, and sealable item.

### MeasuringInstrument Component

Runtime status: `MeasuringInstrument` is implemented for `Weight` and `FluidVolume` modes. The stock wooden measuring rod deliberately remains a roleplay and set-dressing prop, and no dimension-aware mode is planned for it.

Implemented behaviour:

- `weigh <item> on <instrument>` measures item weight with instrument precision, capacity, stable drift, and calibration bias.
- `measure <target> with <instrument>` measures supported fluid-volume targets with the same drift and calibration model.
- `calibrate <instrument>` clears deliberate bias and resets drift state.
- `calibrate <instrument> wrong <+/-amount|+/-percent>` deliberately records a bad calibration for false measures.
- `inspect calibration <instrument>` uses existing appraisal-style inspection and reports mode, precision, drift, use count, and deliberate bias according to the viewer's appraisal outcome.
- Item quality scales calibration drift per use, with better-quality instruments drifting less.

Seeded prototypes:

- `MeasuringInstrument_Antiquity_BalanceScale`: weight-mode balance scale.
- `MeasuringInstrument_Antiquity_StandardWeights`: weight-mode official or merchant weight set.
- `MeasuringInstrument_Antiquity_FalseWeights`: weight-mode biased weight set for dishonest trade.
- `MeasuringInstrument_Antiquity_GrainMeasure`: weight-mode grain measure.
- `MeasuringInstrument_Antiquity_OilCup`: fluid-volume oil measure.
- `MeasuringInstrument_Antiquity_WineCup`: fluid-volume wine measure.
- `MeasuringInstrument_Antiquity_TaxAssessorKit`: weight-mode official inspection kit.

Items enabled or improved:

- `antiquity_bronze_balance_scale`
- `antiquity_standard_weight_set`
- `antiquity_false_weight_set`
- `antiquity_stone_grain_measure`
- `antiquity_oil_measure_cup`
- `antiquity_wine_measure_cup`
- `antiquity_tax_assessor_measure_kit`

## Missing Engine Functionality That Could Become New Component Types

These gaps are not merely missing stock data. They describe gameplay that does not appear to be represented by a suitable current item component type, or where existing components would only produce a static prop.

The consolidated ledger is authoritative for implementation ordering and exact dependent names. In particular, general `Instrument` is the shared audible-performance foundation for the military `SignalInstrument` specialization.

### Instrument Component

Implemented functionality:

- `Instrument` supports skill-checked sustained performances with immediate and ten-second repeated output.
- Loudness uses the existing cell audio-routing and attenuation service; no alert event is emitted merely for playing.
- Prototype settings cover family, trait, difficulty, volume, hands, handheld/worn/room use, positions, stamina, interval, styles, emotes, and play/stop FutureProgs.
- `play <instrument> [style] [(emote)]` starts a non-saving performance effect; `stop playing` ends it.
- Movement, combat, item loss, invalid posture, incapacity, exhaustion, deletion, and logout interrupt the performance.
- `SignalInstrument` extends the same foundation for named military calls rather than duplicating audio-performance behavior.

Seeded component profiles:

- `Instrument_Antiquity_WoodenLyre`
- `Instrument_Antiquity_Kithara`
- `Instrument_Antiquity_ReedFlute`
- `Instrument_Antiquity_DoubleAulos`
- `Instrument_Antiquity_FrameDrum`
- `Instrument_Antiquity_Sistrum`
- `Instrument_Antiquity_BronzeWarHorn`
- `Instrument_Antiquity_ShipSignalTrumpet`
- `Instrument_Antiquity_TempleRitualRattle`

Items now enabled:

- `antiquity_wooden_lyre`
- `antiquity_kithara`
- `antiquity_reed_flute`
- `antiquity_double_aulos`
- `antiquity_frame_drum`
- `antiquity_sistrum`
- `antiquity_bronze_war_horn`
- `antiquity_ship_signal_trumpet`
- `antiquity_temple_ritual_rattle`

### GameSet Component

Implementation status: reserved for a separate detailed system-design slice. The game-set family is the only remaining Antiquity engine dependency in this report.

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

### Offering Receiver and Incense Burner Components

Implemented functionality:

- `IncenseBurner` is a lightable transparent container for tagged incense fuel. It burns contained fuel by weight, emits room LOOK scent text, spreads ambient scent to nearby cells, and exposes scent metadata to the `tracks` command without creating movement tracks.
- `OfferingReceiver` is a transparent/open ritual focus that accepts broad item offerings, optionally gates them by tags, supports `offer <item> at <focus>` and `burn <item> at <focus>`, consumes burned offerings by default, and can create configured residue.
- `OfferingReceiver` runs `CanOfferProg`, `OnOfferProg`, and `OnBurnProg` with `(Character actor, Item focus, Item offering)`.
- The event stream now exposes `OfferingReceived`, `OfferingReceivedWitness`, `OfferingBurned`, and `OfferingBurnedWitness`, with payloads `(focus, actor, offering)` and `(focus, actor, offering, witness)`.
- Liquid-enabled receivers support `libate <amount> from <container> at <focus> [(emote)]`. The poured liquid is consumed from the open source container, admitted through allowed/blocked liquid tags and minimum/maximum volumes, and recorded in a compact per-item summary.
- Liquid gates and hooks receive `(Character actor, Item focus, Item source, LiquidMixture liquid, Number amount)`. The component supports `CanOfferLiquidProg`, `WhyCannotOfferLiquidProg`, `OnOfferLiquidProg`, optional text-returning `OracleResponseProg`, and focus/witness liquid-offering events.

Explicit extension boundary:

- Detailed provenance logs, ritual ownership policy, spoilage counters, cooldowns, and law/clan/religion rules remain custom systems layered through the supplied progs/events.
- The oil-lamp shrine composes the existing `Lantern` component for lighting and fuel state. A libation is a separate ritual action and does not silently refill the lamp.

Seeded support:

- `IncenseBurner_Antiquity_BronzeCenser`
- `OfferingReceiver_Antiquity_HouseholdAltar`
- `OfferingReceiver_Antiquity_VotiveBasin`
- `OfferingReceiver_Antiquity_FuneralTray`
- `OfferingReceiver_Antiquity_TempleLibationTable`
- `OfferingReceiver_Antiquity_OilLampShrine`
- `OfferingReceiver_Antiquity_OracularTripod`
- `OfferingReceiver_Antiquity_BloodOfferingBowl`

Items enabled:

- `antiquity_bronze_incense_censer`
- `antiquity_resin_incense_pellets`
- `antiquity_household_altar`
- `antiquity_votive_offering_basin`
- `antiquity_funeral_offering_tray`
- `antiquity_temple_libation_table`
- `antiquity_oil_lamp_shrine`
- `antiquity_oracular_tripod`
- `antiquity_blood_offering_bowl`

### Animal Tack and Harness Component

Implementation status: complete using the existing `RidingGear` and `HitchGear` families. The earlier gap assessment predated those runtime capabilities; the remaining work was seeded item composition and maintained-catalogue export.

- bridles, pack saddles, bitless control gear, and riding harnesses now contribute their existing mounted-control and stability roles.
- yokes, draft harnesses, and lead ropes use existing hitch roles and effort/user constraints.
- mule panniers combine `RidingGear_PackSaddle` with one ordinary container component, preserving a single container inventory.
- warhorse barding combines the riding harness with existing wearable and armour behavior.

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
2. Completed in `UsefulSeeder`: antiquity-specific component prototypes for `TimePiece`, `WaterSource`, `DragAid`, `Dice`, `Locksmithing Tool`, `ShopStall`, `MarketGoodWeight`, `SealStamp`, `Sealable`, and `MeasuringInstrument`, plus the existing `RidingGear` and `HitchGear` profile exports.
3. Add matching item prototypes that use those new component prototypes and update this document with the shipped names.
4. `IncenseBurner` and `OfferingReceiver` now cover incense, item-offering burn hooks, and consumptive tagged liquid libations; richer ritual policy remains an external hook-driven integration.
5. The eight tack and harness references are seeded using existing runtime roles, and the measuring rod is intentionally a static prop. The seven game-set references are the remaining dependency and await their own detailed design.
