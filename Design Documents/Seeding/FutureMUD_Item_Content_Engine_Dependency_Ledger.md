# FutureMUD Item Content Engine Dependency Ledger

This is the consolidated backlog for seeded item requests that the current engine cannot represent honestly. The corresponding data-only pass seeds only supported content; none of the 100 prototype names below appears in the maintained seeded item-component catalogue.

## Status

- Deferred item-component prototypes: **100**.
- Early Modern military partition: **56 supported** and **100 deferred**, from the original 156 requests.
- Additional Renaissance household deferrals: **0**.
- Antiquity item references awaiting engine work: **21**.
- New runtime component families proposed: **6**, with **55** directly dependent military prototypes.
- This ledger is a backlog only. It does not authorize placeholder components, runtime APIs, database migrations, or item rows that advertise unsupported behaviour.

## Existing component-family enhancements

### Completed initial closure tranche

The first tranche is implemented, seeded idempotently, exported, documented, and tested:

- `CashRegister_PreIndustrial_TillChest` uses the single-container `LockingCashRegister` family, retaining shop/currency till behavior and adding normal lock, key, picking, forcing, copy, revision, and XML state paths.
- `MusketPaperCartridge_*` for all seven stock bores persist an explicit powder mass and wad flag; definitions without those fields retain legacy weapon-charge and included-wad behavior.
- `Container_CartridgeBandolier` uses standard-container allowed/blocked tag admission and accepts the functional paper-cartridge and wooden-charge families.
- `Bayonet_Plug`, `Bayonet_Socket`, and `Bayonet_Sword` use the firearm attachment slots and their attached items' existing melee profiles; only plug bayonets block firing.
- `Crossbow_Cranequin`, `Crossbow_GoatsFoot`, `Crossbow_Lever`, `Crossbow_SpanningHook`, and `Crossbow_Windlass` require their matching tagged stock spanning tools and persist ready state.
- The eight Antiquity tack references use existing `RidingGear` and `HitchGear` capabilities. The prior ledger entry was a seeded-content and maintained-catalogue integration gap, not a missing engine family.

### Crossbow magazine and emplacement behaviour — 3 prototypes

The four earlier supported crossbow profiles and five tool-spanned profiles use behavior the ranged system can now express. These three still require repeating magazines or emplacement constraints.

| Prototype | Catalogue uses |
| --- | ---: |
| `Crossbow_Repeating` | 3 |
| `Crossbow_Repeating_Light` | 3 |
| `Crossbow_Wall` | 3 |

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

### Multi-projectile musket ammunition — 4 prototypes

Measured single-projectile paper cartridges are supported. These profiles still require multiple projectile payloads and spread.

| Prototype | Catalogue uses |
| --- | ---: |
| `MusketBuckAndBall_0.65 Bore` | 1 |
| `MusketBuckAndBall_0.75 Bore` | 1 |
| `MusketBuckshot_0.55 Bore` | 1 |
| `MusketBuckshot_0.75 Bore` | 1 |
### Multi-slot carrier behaviour — 1 prototype

| Prototype | Catalogue uses | Required engine work |
| --- | ---: | --- |
| `Holster_PairedSaddle` | 3 | A sheath/holster capable of holding and independently drawing two compatible pistols. |

### Existing melee-family enhancements

The data pass seeds `Melee_Lance` and `Melee_HookedPolearm` using supported attacks. Their names and current move sets do not claim the following deferred mechanics:

- `Melee_Lance`: mounted/couched charge checks, movement-linked impact, bracing, and dismount consequences.
- `Melee_HookedPolearm`: explicit hook, pull, trip, anti-rider, and forced-dismount attacks.

## New component families and dependent military prototypes — 55

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

## Antiquity engine gaps — 21 dependent item references

These are item references, not additional names in the 100-prototype total.

### `Instrument` — 9 items

`antiquity_wooden_lyre`, `antiquity_kithara`, `antiquity_reed_flute`, `antiquity_double_aulos`, `antiquity_frame_drum`, `antiquity_sistrum`, `antiquity_bronze_war_horn`, `antiquity_ship_signal_trumpet`, `antiquity_temple_ritual_rattle`

The common foundation must support playable audible instruments, performance quality, loudness/range, hand and posture rules, sustained playing state, emotes, and prog hooks. Military signals should extend this capability rather than form a second unrelated audio-performance system.

### `GameSet` — 7 items

`antiquity_senet_game_board`, `antiquity_mehen_game_board`, `antiquity_latrunculi_board`, `antiquity_royal_game_board`, `antiquity_mancala_board`, `antiquity_temple_divination_board`, `antiquity_tavern_game_set`

Required behaviour includes persistent game state, players and spectators, legal move or move-log handling, reset/concede flow, and optional gambling or skill hooks.

### Dimension-aware `MeasuringInstrument` — 1 item

`antiquity_wooden_measuring_rod`

The current measuring family supports weight and fluid volume. Length/cubit/survey measurement requires item dimensions and a corresponding measurement mode.

### Extended `OfferingReceiver` — 4 items

`antiquity_temple_libation_table`, `antiquity_oil_lamp_shrine`, `antiquity_oracular_tripod`, `antiquity_blood_offering_bowl`

These require direct poured-liquid libations and/or specialized lamp, oracle, blood-offering, ritual ownership/history, cooldown, law, clan, or religion integration beyond the existing item-offering V1.

## Completion boundary

The data-only dependency completion pass may seed components only when the current runtime can express their advertised behaviour. A prototype moves out of this ledger only when its engine support, persistence, builder workflow, tests, and maintained catalogue export are complete.
