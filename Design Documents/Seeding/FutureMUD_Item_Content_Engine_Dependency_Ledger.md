# FutureMUD Item Content Engine Dependency Ledger

This is the consolidated backlog for seeded item requests that the current engine cannot represent honestly. The corresponding data-only pass seeds only supported content; none of the 117 prototype names below appears in the maintained seeded item-component catalogue.

## Status

- Deferred item-component prototypes: **117**.
- Early Modern military partition: **40 supported** and **116 deferred**, from the original 156 requests.
- Additional Renaissance household deferral: **1**, bringing the consolidated total to 117.
- Antiquity item references awaiting engine work: **29**.
- New runtime component families proposed: **7**, with **58** directly dependent military prototypes.
- This ledger is a backlog only. It does not authorize placeholder components, runtime APIs, database migrations, or item rows that advertise unsupported behaviour.

## Existing component-family enhancements

### Lockable cash register — 1 prototype

The current `CashRegister` component is a container but is not lockable. `LockingContainer` would lose the shop/currency interaction, so the requested hybrid remains deferred.

| Prototype | Catalogue uses | Required engine work |
| --- | ---: | --- |
| `CashRegister_PreIndustrial_TillChest` | 10 | Add lock/open state and lock interaction to `CashRegister`, or provide a supported composition boundary between cash-register and lockable-container behaviour. |

### Crossbow spanning, magazine, and emplacement behaviour — 8 prototypes

The four supported crossbow profiles (`Crossbow_EastAsian`, `Crossbow_Heavy`, `Crossbow_Light`, and `Crossbow_Pellet`) use behaviour the current ranged system can express. These eight require explicit spanning tools, repeating magazines, or emplacement constraints.

| Prototype | Catalogue uses |
| --- | ---: |
| `Crossbow_Cranequin` | 3 |
| `Crossbow_GoatsFoot` | 3 |
| `Crossbow_Lever` | 3 |
| `Crossbow_Repeating` | 3 |
| `Crossbow_Repeating_Light` | 3 |
| `Crossbow_SpanningHook` | 3 |
| `Crossbow_Wall` | 3 |
| `Crossbow_Windlass` | 3 |

### Ignition-specific muzzleloaders — 37 prototypes

These profiles remain under the existing `Musket` family, but require ignition-family state, lock readiness, weather interaction, rifling/spread flags, and rest or bayonet-mount constraints before they can be truthful.

| Prototype | Catalogue uses |
| --- | ---: |
| `Musket_Doglock_Blunderbuss75` | 3 |
| `Musket_Doglock_Carbine60` | 3 |
| `Musket_Doglock_Musket70` | 3 |
| `Musket_Flintlock_Blunderbuss75` | 3 |
| `Musket_Flintlock_Carbine60` | 3 |
| `Musket_Flintlock_FowlingPiece60` | 3 |
| `Musket_Flintlock_WallGun80` | 3 |
| `Musket_Matchlock_Arquebus55` | 3 |
| `Musket_Matchlock_Blunderbuss75` | 3 |
| `Musket_Matchlock_Caliver60` | 3 |
| `Musket_Matchlock_Carbine60` | 3 |
| `Musket_Matchlock_FowlingPiece60` | 3 |
| `Musket_Matchlock_HeavyMusket80` | 3 |
| `Musket_Matchlock_Jezail65` | 3 |
| `Musket_Matchlock_Musket75` | 3 |
| `Musket_Matchlock_Tanegashima60` | 3 |
| `Musket_Matchlock_Toradar65` | 3 |
| `Musket_Matchlock_WallGun80` | 3 |
| `Musket_Miquelet_Blunderbuss75` | 3 |
| `Musket_Miquelet_Carbine60` | 3 |
| `Musket_Miquelet_Escopeta65` | 3 |
| `Musket_Miquelet_Musket70` | 3 |
| `Musket_Snaphaunce_Carbine60` | 3 |
| `Musket_Snaphaunce_FowlingPiece60` | 3 |
| `Musket_Snaphaunce_LongGun65` | 3 |
| `Musket_Snaphaunce_Musket70` | 3 |
| `Musket_Wheellock_Carbine60` | 3 |
| `Musket_Wheellock_FowlingPiece60` | 3 |
| `Musket_Wheellock_LongGun65` | 3 |
| `Musket_Wheellock_Rifle55` | 3 |
| `Pistol_Doglock55` | 3 |
| `Pistol_Flintlock_Duelling45` | 3 |
| `Pistol_Matchlock55` | 3 |
| `Pistol_Miquelet55` | 3 |
| `Pistol_Snaphaunce55` | 3 |
| `Pistol_Wheellock45` | 3 |
| `Pistol_Wheellock55` | 3 |

### Measured and multi-projectile musket ammunition — 11 prototypes

These profiles require the existing `MusketCartridge` family to carry a measured powder charge, bore-compatible projectile payload, wad state, and multi-projectile spread.

| Prototype | Catalogue uses |
| --- | ---: |
| `MusketBuckAndBall_0.65 Bore` | 1 |
| `MusketBuckAndBall_0.75 Bore` | 1 |
| `MusketBuckshot_0.55 Bore` | 1 |
| `MusketBuckshot_0.75 Bore` | 1 |
| `MusketPaperCartridge_0.45 Bore` | 1 |
| `MusketPaperCartridge_0.55 Bore` | 1 |
| `MusketPaperCartridge_0.6 Bore` | 1 |
| `MusketPaperCartridge_0.65 Bore` | 1 |
| `MusketPaperCartridge_0.7 Bore` | 1 |
| `MusketPaperCartridge_0.75 Bore` | 1 |
| `MusketPaperCartridge_0.8 Bore` | 1 |

### Content-constrained carrier behaviour — 2 prototypes

| Prototype | Catalogue uses | Required engine work |
| --- | ---: | --- |
| `Container_CartridgeBandolier` | 3 | Container admission constrained to compatible paper cartridges or wooden charges. |
| `Holster_PairedSaddle` | 3 | A sheath/holster capable of holding and independently drawing two compatible pistols. |

### Existing melee-family enhancements

The data pass seeds `Melee_Lance` and `Melee_HookedPolearm` using supported attacks. Their names and current move sets do not claim the following deferred mechanics:

- `Melee_Lance`: mounted/couched charge checks, movement-linked impact, bracing, and dismount consequences.
- `Melee_HookedPolearm`: explicit hook, pull, trip, anti-rider, and forced-dismount attacks.

## New component families and dependent military prototypes — 58

### `MuzzleloadingArtillery` — 20 dependents

This family must model crew loading, charge and projectile state, vent/priming, elevation, traverse, recoil, fouling, misfire, and mount/emplacement rules.

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `Artillery_CoehornMortar` | 3 |
| `Artillery_Culverin` | 3 |
| `Artillery_DemiCannon` | 3 |
| `Artillery_DemiCulverin` | 3 |
| `Artillery_Falcon` | 3 |
| `Artillery_Falconet` | 3 |
| `Artillery_FieldGun12lb` | 3 |
| `Artillery_FieldGun3lb` | 3 |
| `Artillery_FieldGun6lb` | 3 |
| `Artillery_FieldGun9lb` | 3 |
| `Artillery_FieldMortar` | 3 |
| `Artillery_FullCannon` | 3 |
| `Artillery_Minion` | 3 |
| `Artillery_NavalGun18lb` | 3 |
| `Artillery_NavalGun24lb` | 3 |
| `Artillery_NavalGun32lb` | 3 |
| `Artillery_Peterero` | 3 |
| `Artillery_Saker` | 3 |
| `Artillery_SiegeMortar` | 3 |
| `Artillery_SwivelGun` | 3 |

### `ArtilleryAmmunition` — 20 dependents

This family must represent nominal shot class, compatible guns, solid/stone/bar/multi-projectile payloads, shell fuses, spread, impact effects, and recoverability.

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `ArtilleryCaseShot_12lb` | 1 |
| `ArtilleryCaseShot_3lb` | 1 |
| `ArtilleryCaseShot_6lb` | 1 |
| `ArtilleryGrapeshot_Heavy` | 1 |
| `ArtilleryGrapeshot_Light` | 1 |
| `ArtilleryGrapeshot_Medium` | 1 |
| `ArtilleryShell_Carcass` | 1 |
| `ArtilleryShell_Heavy` | 1 |
| `ArtilleryShell_Light` | 1 |
| `ArtilleryShot_12lb` | 1 |
| `ArtilleryShot_18lb` | 1 |
| `ArtilleryShot_1lb` | 1 |
| `ArtilleryShot_24lb` | 1 |
| `ArtilleryShot_32lb` | 1 |
| `ArtilleryShot_3lb` | 1 |
| `ArtilleryShot_6lb` | 1 |
| `ArtilleryShot_9lb` | 1 |
| `ArtilleryShot_Bar` | 1 |
| `ArtilleryStoneShot_Heavy` | 1 |
| `ArtilleryStoneShot_Light` | 1 |

### `ArtilleryMount` — 1 dependent

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `ArtilleryMount_Swivel` | 1 |

The family must relate a gun to a host socket or carriage with installation, removal, elevation, traverse, and fixed/transportable state.

### `BayonetAttachment` — 3 dependents

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `Bayonet_Plug` | 3 |
| `Bayonet_Socket` | 3 |
| `Bayonet_Sword` | 3 |

The family must attach a separate bayonet item to compatible firearms, change the mounted melee profile, enforce exclusivity, and distinguish plug-bayonet firing blockage from socket/sword-bayonet rules.

### `WeaponCarrierAttachment` — 4 dependents

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `WeaponLanyard_Pistol` | 3 |
| `WeaponLoop_LongGun` | 3 |
| `WeaponSling_Carbine` | 3 |
| `WeaponSling_LongGun` | 6 |

The family must retain or carry a specific compatible weapon and model attachment point, draw/release timing, retention, and the transition between carried and wielded state.

### `MilitaryStandard` — 6 dependents

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `MilitaryStandard_CavalryStandard` | 3 |
| `MilitaryStandard_Guidon` | 3 |
| `MilitaryStandard_InfantryColour` | 3 |
| `MilitaryStandard_NavalEnsign` | 3 |
| `MilitaryStandard_Pennant` | 3 |
| `MilitaryStandard_SignalFlag` | 3 |

The family must persist standard family, design/identity, optional unit or ship association, carried/planted state, and recognition/signal hooks. It should not grant an undeclared morale effect.

### `SignalInstrument` — 4 dependents

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `SignalInstrument_FieldDrum` | 3 |
| `SignalInstrument_Fife` | 3 |
| `SignalInstrument_KettleDrum` | 3 |
| `SignalInstrument_SpeakingTrumpet` | 3 |

`SignalInstrument` should be a military specialization of the general `Instrument` capability below, sharing audible performance, skill, range, posture, hand, stamina, and echo infrastructure while adding recognized command patterns.

## Antiquity engine gaps — 29 dependent item references

These are item references, not additional names in the 117-prototype total.

### `Instrument` — 9 items

`antiquity_wooden_lyre`, `antiquity_kithara`, `antiquity_reed_flute`, `antiquity_double_aulos`, `antiquity_frame_drum`, `antiquity_sistrum`, `antiquity_bronze_war_horn`, `antiquity_ship_signal_trumpet`, `antiquity_temple_ritual_rattle`

The common foundation must support playable audible instruments, performance quality, loudness/range, hand and posture rules, sustained playing state, emotes, and prog hooks. Military signals should extend this capability rather than form a second unrelated audio-performance system.

### `GameSet` — 7 items

`antiquity_senet_game_board`, `antiquity_mehen_game_board`, `antiquity_latrunculi_board`, `antiquity_royal_game_board`, `antiquity_mancala_board`, `antiquity_temple_divination_board`, `antiquity_tavern_game_set`

Required behaviour includes persistent game state, players and spectators, legal move or move-log handling, reset/concede flow, and optional gambling or skill hooks.

### `AnimalTack` / `Harness` — 8 items

`antiquity_leather_bridle`, `antiquity_pack_saddle`, `antiquity_mule_pannier_set`, `antiquity_ox_yoke`, `antiquity_chariot_harness`, `antiquity_camel_cargo_saddle`, `antiquity_warhorse_barding_harness`, `antiquity_rope_lead_halter`

This cross-subsystem family must link animals, riders, pack loads, vehicles, and drag aids rather than treating functional tack as ordinary worn clothing.

### Dimension-aware `MeasuringInstrument` — 1 item

`antiquity_wooden_measuring_rod`

The current measuring family supports weight and fluid volume. Length/cubit/survey measurement requires item dimensions and a corresponding measurement mode.

### Extended `OfferingReceiver` — 4 items

`antiquity_temple_libation_table`, `antiquity_oil_lamp_shrine`, `antiquity_oracular_tripod`, `antiquity_blood_offering_bowl`

These require direct poured-liquid libations and/or specialized lamp, oracle, blood-offering, ritual ownership/history, cooldown, law, clan, or religion integration beyond the existing item-offering V1.

## Completion boundary

The data-only dependency completion pass may seed components only when the current runtime can express their advertised behaviour. A prototype moves out of this ledger only when its engine support, persistence, builder workflow, tests, and maintained catalogue export are complete.
