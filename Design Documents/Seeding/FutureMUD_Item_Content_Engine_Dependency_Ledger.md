# FutureMUD Item Content Engine Dependency Ledger

This is the consolidated backlog for seeded item requests that the current engine cannot represent honestly. The Early Modern military closure described below is implemented and seeded. The Renaissance military catalogue now consumes the same completed #575 component groundwork; the remaining engine backlog is limited to the listed Antiquity references.

## Status

- Deferred item-component prototypes: **0** for the Early Modern military ledger.
- Early Modern military partition: **156 supported** and **0 deferred**, from the original 156 requests.
- Renaissance military catalogue: **96 source-backed rows**, all validated against existing #575 component profiles before item creation.
- Additional Renaissance household deferrals: **0**.
- Antiquity item references awaiting engine work: **7**.
- New runtime component families delivered: **5**, with **45** directly dependent military prototypes.
- The military entries below document delivered runtime seams, persisted XML, builder surfaces, and seeder ownership; they are not placeholder component rows.

## Existing component-family enhancements

### Completed initial closure tranche

The first tranche is implemented, seeded idempotently, exported, documented, and tested:

- `CashRegister_PreIndustrial_TillChest` uses the single-container `LockingCashRegister` family, retaining shop/currency till behavior and adding normal lock, key, picking, forcing, copy, revision, and XML state paths.
- `MusketPaperCartridge_*` for all seven stock bores persist an explicit powder mass and wad flag; definitions without those fields retain legacy weapon-charge and included-wad behavior.
- `Container_CartridgeBandolier` uses standard-container allowed/blocked tag admission and accepts the functional paper-cartridge and wooden-charge families.
- `Bayonet_Plug`, `Bayonet_Socket`, and `Bayonet_Sword` use the firearm attachment slots and their attached items' existing melee profiles; only plug bayonets block firing.
- `Crossbow_Cranequin`, `Crossbow_GoatsFoot`, `Crossbow_Lever`, `Crossbow_SpanningHook`, and `Crossbow_Windlass` require their matching tagged stock spanning tools and persist ready state.
- The eight Antiquity tack references use existing `RidingGear` and `HitchGear` capabilities. The prior ledger entry was a seeded-content and maintained-catalogue integration gap, not a missing engine family.
- General `Instrument` provides skill-checked sustained playing, posture/hand/use-mode admission, stamina ticks, route-aware audio, interruption, emotes, and FutureProg hooks. Nine Antiquity instruments are seeded against it.
- `SignalInstrument` specializes `Instrument` with named calls, per-item cooldowns, stamina costs, deliberately unrecognisable failed audio, and a success hook. Four profiles and twelve source items are seeded.
- `MilitaryStandard` supplies identity, recognition, optional unit/ship association, planting, named flag signals, durable ownership-aware custody, distinct capture counting, recovery, transition hooks, administrator state controls, and FutureProg queries. Six profiles and eighteen source items are seeded; no implicit morale or scoring effect is added.
- `OfferingReceiver` now accepts consumptive liquid libations through `libate`, with allowed/blocked liquid-tag admission, volume bounds, FutureProg gates/hooks, optional oracle text, events, and a compact per-item summary. Four Antiquity ritual profiles and items are seeded.
- `antiquity_wooden_measuring_rod` is intentionally complete as a static holdable prop. Decision: **will not implement** a dedicated measuring-rod mechanic or a length/dimension engine solely for this item, so it is not an engine dependency.

### Crossbow magazine and emplacement behaviour — 3 prototypes

Repeating profiles persist magazine and cycle state, while wall crossbows require emplacement before loading or firing.

| Prototype | Catalogue uses |
| --- | ---: |
| `Crossbow_Repeating` | 3 |
| `Crossbow_Repeating_Light` | 3 |
| `Crossbow_Wall` | 3 |

### Ignition-specific muzzleloaders — 37 prototypes

These profiles use persisted ignition-family state, match and wheel readiness, ignition-stone wear, weather interaction, rifling/rest flags, and existing bayonet restrictions.

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

Measured paper cartridges use the shared projectile-count and scatter path for buckshot and buck-and-ball loads.

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

`Melee_Lance` has a charge-only couched attack that uses capped mount speed/reach and the established mounted knockdown path. `Melee_HookedPolearm` adds a pull attack beside the donor trip moves; a successful opposed hook against a rider uses that same displacement path.

## New component families and dependent military prototypes — 45

### `ArtilleryPiece` — complete

Implemented as the shared crew-served ranged platform with persisted loading/emplacement state, removable-chamber support, mounts, crew roles, movement vetoes, and direct room-level operation.

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

This family persists nominal calibre class and compatible artillery profiles. Solid, stone, and bar projectiles use single ballistic rounds; grapeshot and case shot use the shared projectile-count and spread pipeline. Shell and carcass rows retain their selected fuse in the artillery firing solution before the normal projectile lifecycle resolves.

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

The family relates a gun to a host socket or carriage with installation, removal, elevation, traverse, and fixed/transportable state.

### `WeaponCarrierAttachment` — 4 dependents

| Dependent prototype | Catalogue uses |
| --- | ---: |
| `WeaponLanyard_Pistol` | 3 |
| `WeaponLoop_LongGun` | 3 |
| `WeaponSling_Carbine` | 3 |
| `WeaponSling_LongGun` | 6 |

The family retains or carries a specific compatible weapon and models attachment point, retention, and the transition between carried, wielded, and hanging state.

### Completed `MilitaryStandard` family — 6 supported profiles

| Dependent prototype | Catalogue uses |
| --- | ---: |
- `MilitaryStandard_CavalryStandard` (3 catalogue uses)
- `MilitaryStandard_Guidon` (3 catalogue uses)
- `MilitaryStandard_InfantryColour` (3 catalogue uses)
- `MilitaryStandard_NavalEnsign` (3 catalogue uses)
- `MilitaryStandard_Pennant` (3 catalogue uses)
- `MilitaryStandard_SignalFlag` (3 catalogue uses)

Implemented with prototype defaults plus per-copy identity and association overrides, planted state, durable ownership-aware custody, distinct capture counting, recovery, recognition, named signal hooks, administrator controls, and FutureProg queries. It deliberately grants no undeclared morale effect.

### Completed `SignalInstrument` specialization — 4 supported profiles

| Dependent prototype | Catalogue uses |
| --- | ---: |
- `SignalInstrument_FieldDrum` (3 catalogue uses)
- `SignalInstrument_Fife` (3 catalogue uses)
- `SignalInstrument_KettleDrum` (3 catalogue uses)
- `SignalInstrument_SpeakingTrumpet` (3 catalogue uses)

`SignalInstrument` is a military specialization of the general `Instrument` capability, sharing audible performance, skill, range, posture, hand, stamina, and echo infrastructure while adding named command patterns and cooldown-limited signal hooks.

## Renaissance military seeder closure - existing #575 groundwork consumed

The Renaissance catalogue is data-only and does not add a runtime family, persistence field, or migration. It uses the completed `Musket_Matchlock_*`, `Musket_Wheellock_*`, `Pistol_Matchlock*`, `Pistol_Wheellock*`, `MusketBall_*`, `MusketPaperCartridge_*`, `Artillery*`, `ArtilleryMount_Swivel`, weapon-carrier, armour, shield, melee, and ranged profiles already maintained by the Combat and Useful seeders.

| Renaissance family | Rows | Dependency treatment |
| --- | ---: | --- |
| Melee weapons | 14 | Existing melee profiles; distinct Renaissance forms only. |
| Ranged weapons | 11 | Existing bow, crossbow, sling, and ammunition profiles. |
| Firearms and ammunition | 15 | Matchlock/wheellock and compatible ball or paper-cartridge profiles only. |
| Artillery | 17 | Early field pieces, compatible shot, and the existing swivel mount; no Coehorn or default later naval patterns. |
| Armour and barding | 18 | Existing expanded wear and armour profiles. |
| Shields | 8 | Existing buckler, rotella, pavise, dhal, adarga, targe, hide, and wicker profiles. |
| Military support and field gear | 13 | Existing bandolier, artillery tool, carrier, standard, and signal profiles. |

The Item seeder checks all three dependency classes before it writes an era item: materials, full tag paths, and component profiles. If an early-gun package was declined, it stops with the missing profile names and directs the builder to rerun Combat's weapons, ranged-weapons, and early-gun packages before rerunning Items. It does not install a declined Combat package. The maintained exports already contain this closure's dependencies, so no material, tag, or component export extension was required.

The canonical table lives in `FutureMUD_Renaissance_Military_Firearms_Armour_Design_Reference.md`; its generator copies authored descriptions verbatim, rejects generic seeding language, validates all maintained dependencies, and produces `ItemSeeder.Renaissance.MilitaryManifestData.Generated.cs`.

## Antiquity engine gaps — 7 dependent item references

These are item references, not additional names in the 90-prototype total.

### Completed `Instrument` family — 9 supported items

The nine requested Antiquity instrument items are now seeded against exact `Instrument_Antiquity_*` profiles and are no longer part of this backlog.

The common foundation now supports playable audible instruments, performance quality, loudness/range, hand and posture rules, sustained playing state, emotes, and prog hooks.

### `GameSet` — 7 items

`antiquity_senet_game_board`, `antiquity_mehen_game_board`, `antiquity_latrunculi_board`, `antiquity_royal_game_board`, `antiquity_mancala_board`, `antiquity_temple_divination_board`, `antiquity_tavern_game_set`

Required behaviour includes persistent game state, players and spectators, legal move or move-log handling, reset/concede flow, and optional gambling or skill hooks.

This family is deliberately reserved for a separate detailed system-design slice rather than being folded into the ritual-content work.

### Completed liquid `OfferingReceiver` extension — 4 supported items

`antiquity_temple_libation_table`, `antiquity_oil_lamp_shrine`, `antiquity_oracular_tripod`, and `antiquity_blood_offering_bowl` are seeded against exact liquid-enabled `OfferingReceiver_Antiquity_*` profiles. The oil-lamp shrine composes the existing `Lantern` capability rather than duplicating lighting state. Ritual ownership, detailed history, cooldown, law, clan, and religion policy remain external FutureProg/event integrations and are not required to represent these stock items honestly.

## Industrialised prerequisite audit - no new engine family required

The September 2026 Stage 1 audit compares all 244 canonical runtime item-component registrations against 4,402 exported reusable profiles and a focused material, liquid, gas and tag prerequisite ledger. It recovered 22 already seeded profiles from export drift and added 23 rerunnable owner-controlled profiles using existing dedicated mechanics.

The remaining Industrialised-relevant differences are recorded in `Industrialised_Component_Prerequisite_Audit.tsv`. Nine are dependency-bound to anatomy, finished-item output/source references or VehicleSeeder graphs, and the rest are explicit system, alternate-stock or futuristic dispositions. No reusable-stock gap or new engine component-family gap remains.

The power-tool unit mismatch is closed. `IProducePower` exposes a duration-aware spike overload, `PowerToolGameItemComponent` passes watts and duration separately, and finite battery stores debit the correct watt-hours while continuous providers validate the instantaneous load. Focused regressions protect the overload choice and finite-store arithmetic.

If Stage 2 discovers further behavior that registered types cannot represent honestly, that concrete behavior and dependent item references must be added here before catalogue admission.

## Completion boundary

The data-only dependency completion pass may seed components only when the current runtime can express their advertised behaviour. A prototype moves out of this ledger only when its engine support, persistence, builder workflow, tests, and maintained catalogue export are complete.
