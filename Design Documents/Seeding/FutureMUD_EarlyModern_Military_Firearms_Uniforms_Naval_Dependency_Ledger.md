# FutureMUD Early Modern Military, Firearms, Armour, Ammunition, Accessories, and Naval Dependency Ledger

> Source request ledger for `FutureMUD_EarlyModern_Military_Firearms_Uniforms_Naval_Design_Reference.md`. The implementation status below supersedes the original all-or-nothing assumption; engine-dependent entries are consolidated in `FutureMUD_Item_Content_Engine_Dependency_Ledger.md`.

## Status

- Original component requests: **156**.
- Supported and now seeded: **66**.
- Deferred for engine work: **90**.
- New component types still required: **4**.
- Original requests using existing component types: **98**, partitioned into 53 supported additions and 45 existing-family behaviour deferrals.
- Prototypes depending on the four still-missing component types: **45**.
- New solid materials required: **0**.
- New functional tags seeded: **11** for paper cartridges, wooden charges, crossbow spanning tools, musical instruments, military signals, and military standards.
- Supported additions: 8 armour, 11 melee, 4 bow, 9 crossbow, 2 blowgun, 2 thrown, 6 wearable, 3 hand-tool, 7 paper-cartridge, 1 constrained-container, 3 bayonet-attachment, 4 signal-instrument, and 6 military-standard profiles.
- Existing runtime families still requiring extension: repeating and emplaced crossbows, `Musket` ignition state, multi-projectile musket ammunition, paired holsters, couched charges, and hook/pull/trip attacks remain deferred.
- Decision-ready engine backlog: [FutureMUD Item Content Engine Dependency Ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md).

## Assumption contract

The main catalogue treats all names in this ledger as available. Implementation may rename a dependency only if every affected catalogue row and this ledger are updated together. Do not silently substitute a superficially similar existing component when that would erase ignition, attachment, coverage, ammunition, carriage, signal, or carry behaviour.

## New component types

### 1. `MuzzleloadingArtillery`

- **Purpose:** crew-served muzzleloading guns and mortars.
- **Why new:** the existing `Musket` family is handheld/wieldable and does not model carriage or emplacement, multiple crew positions, elevation, traverse, recoil, shot families, mortar arcs, or artillery-scale loading.
- **Minimum state:** compatible ammunition families; charge range; loaded projectile; wad/ram state; vent/priming state; ignition state; elevation; traverse; mount/carriage relationship; required crew actions; recoil; fouling; misfire and catastrophic-failure hooks.
- **Interaction boundary:** the component should not expose ordinary handheld wielding. Portable field pieces remain movable items by weight/drag/transport systems, while firing requires a valid emplacement or mount state.

### 2. `ArtilleryAmmunition`

- **Purpose:** ammunition accepted by `MuzzleloadingArtillery`.
- **Why new:** ordinary musket ball/cartridge components cannot express nominal gun weight, stone versus iron shot, bar shot, multi-projectile spread, case opening, shell fuse state, or mortar compatibility.
- **Minimum state:** projectile family; nominal weight or class; compatible artillery profiles; projectile count/spread where relevant; fuse requirement and fuse state for shells; impact and area-effect family; recoverability.

### 3. `ArtilleryMount`

- **Purpose:** mount, carriage, or socket relationship for an artillery piece.
- **Why new:** a swivel gun needs a host socket and constrained traverse/elevation; later field and naval carriage work will need the same attachment boundary.
- **Minimum state:** compatible artillery profiles; installed piece; host or room position; traverse/elevation limits; remove/install actions; fixed versus transportable state.

### 4. `WeaponCarrierAttachment`

- **Purpose:** attach slings, lanyards, and weapon loops to compatible weapons.
- **Why new:** these accessories are not ordinary clothing and should not require pretending that a sling is a standalone shoulder garment. They must retain or carry a specific weapon.
- **Minimum state:** compatible weapon size/type; attachment point; carried location; draw/release time; retention behaviour; exclusivity; whether the carrier remains attached while the weapon is wielded.

### 5. `MilitaryStandard` — implemented

- **Purpose:** colours, standards, guidons, ensigns, pennants, and signal flags with persistent identity/design data.
- **Implemented:** identity and design defaults with per-copy overrides; optional unit or ship association; recognition; planting and take-up; named flag signals; durable ownership-aware friendly/captured/unclaimed custody; distinct capture counting; recovery; hooks; administrator controls; and FutureProg queries. No morale bonus is assumed.

### 7. `SignalInstrument` — implemented

- **Implemented:** a specialization of the general `Instrument` family with named signal patterns, route-aware audible output, performance checking, deliberately unrecognisable failure audio, cooldown and stamina costs, emotes, builder settings, and FutureProg gates/hooks.

## Existing runtime extensions

### `Musket` and muzzleloading firing state

The current generic muzzleloader sequence already covers loose powder, ball, wad, cartridge, ramrod, cleaning, jams, misfires, wet powder, and catastrophic failure. Extend it rather than creating a parallel firearm component type.

Required additions:

- ignition family: matchlock, wheellock, snaphaunce, doglock, miquelet, or flintlock;
- match state, serpentine state, and exposed-match weather interaction;
- wheel winding, key/spanner use, pyrite/flint state, and spring/lock readiness for wheellocks;
- pan/frizzen state and flint wear for snaphaunce, doglock, miquelet, and flintlock families;
- rifled-barrel flag and associated loading/accuracy effects for specialist rifles;
- shot payload family and multi-projectile spread for blunderbuss and buckshot loads;
- bayonet mount key and mounted-bayonet firing rules;
- wall-gun or rest-required flag where a firearm is not meant to be fired normally unsupported.

Do not duplicate component bodies solely for bore. Use generated/configured profiles and matching ranged-weapon definitions; keep database identifiers data-driven.

### `MusketCartridge` and projectile payloads

Paper cartridges must represent a measured powder charge, compatible projectile payload, wad state, and bore. Buckshot and buck-and-ball require multiple projectile handling rather than masquerading as one oversized round ball. Existing loose round-ball components remain valid and are reused.

### Existing bow, crossbow, blowgun, thrown, melee, armour, wearable, container, sheath, and hand-tool types

These types can receive new seeded prototypes and associated weapon/armour/wear data without a new component type only where the current runtime expresses the advertised behaviour. Nine crossbows now cover East Asian, heavy, light, pellet, and five tool-spanned variants; repeating-magazine and wall-emplacement variants remain deferred. The six wearable profiles are seeded with checked bodypart coverage and layering. Armour profiles are balanced from existing pre-modern donor definitions rather than modern ballistic armour.

## Original requests using existing component types — 98

Implementation partition: armour, melee, bow, nine supportable crossbows, blowguns, thrown weapons, wearables, hand tools, seven single-projectile paper cartridges, the constrained bandolier, three bayonets, four signal instruments, and six military standards are seeded (66 total). Repeating/emplaced crossbows, ignition-specific muskets, multi-projectile ammunition, the paired saddle holster, weapon-carrier attachments, and artillery remain among the 90 deferred requests.

### 1. `Armour` prototypes — 8

Adds period-distinct protection profiles without creating a new armour runtime family.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Armour_Brigandine` | 15 | Brigandine and jack-of-plates resistance between heavy cloth and rigid plate. |
| `Armour_BuffLeather` | 15 | Thick buff-hide protection distinct from ordinary boiled leather. |
| `Armour_ChainAndPlate` | 42 | Combined mail-and-plate, four-mirror, splinted, and plated-mail protection. |
| `Armour_Padded` | 45 | Purpose-built quilted armour distinct from ordinary heavy clothing. |
| `Armour_PlateLight` | 45 | Light munition, open-helmet, and mobility-oriented plate. |
| `Armour_PlateMedium` | 138 | Standard field plate for cuirasses, helmets, and limb harness. |
| `Armour_ProofedPlate` | 33 | Heavier proofed plate for cuirassiers, siege work, and specialist protection. |
| `Armour_Rattan` | 9 | Layered rattan or cane armour with distinct mass and damage behaviour. |

### 2. `MeleeWeapon` prototypes — 11

Adds missing Early Modern melee and training profiles using the existing melee-weapon component type.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Melee_Bayonet` | 9 | Bayonet melee profile; attachment behaviour is supplied separately. |
| `Melee_HookedPolearm` | 21 | Bills, guisarmes, forks, Lochaber axes, and other hook-capable polearms. |
| `Melee_Lance` | 12 | Mounted lance profile distinct from an ordinary long spear. |
| `Melee_Poleblade` | 30 | Partisans, spontoon, ranseur, corseque, glaive, voulge, bardiche, naginata, and yari-like poleblades. |
| `Melee_Sabre` | 58 | Curved single-edged sword and cutlass profile. |
| `Melee_Smallsword` | 10 | Light thrusting smallsword profile distinct from a rapier. |
| `Melee_Training_Bayonet` | 2 | Nonlethal bayonet-drill profile. |
| `Melee_Training_Lance` | 2 | Wooden or blunted lance-drill profile. |
| `Melee_Training_Poleblade` | 2 | Wooden or blunted poleblade-drill profile. |
| `Melee_Training_Sabre` | 4 | Wooden or blunted sabre/cutlass drill profile. |
| `Melee_Training_Smallsword` | 2 | Wooden or blunted smallsword drill profile. |

### 3. `Bow` prototypes — 4

Adds composite-bow and asymmetrical-yumi profiles using the existing bow runtime.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `CompositeBow_Heavy` | 9 | Heavy composite bow with high draw and long siyahs. |
| `CompositeBow_Light` | 9 | Compact mounted or hunting composite bow. |
| `CompositeBow_War` | 15 | General military composite bow. |
| `Yumi` | 3 | Asymmetrical long-bow profile with distinct handling. |

### 4. `Crossbow` prototypes — 12

Adds spanning-method, power, pellet, repeating, and emplacement profiles using the existing crossbow runtime.

Implementation status: the five tool-spanned rows and four earlier rows are seeded. The two repeating rows and `Crossbow_Wall` remain deferred.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Crossbow_Cranequin` | 3 | Steel-prod crossbow with cranequin spanning, high power, and long reload. |
| `Crossbow_EastAsian` | 3 | Straight-stock military crossbow profile suitable for East Asian trigger and stock geometry. |
| `Crossbow_GoatsFoot` | 3 | Medium crossbow using a goat's-foot lever. |
| `Crossbow_Heavy` | 9 | Heavy steel-prod or naval crossbow. |
| `Crossbow_Lever` | 3 | Crossbow spanned by an integrated or removable lever. |
| `Crossbow_Light` | 6 | Light hunting or bamboo-prod crossbow. |
| `Crossbow_Pellet` | 6 | Pellet crossbow or stonebow using spherical shot. |
| `Crossbow_Repeating` | 3 | Full-size repeating crossbow with magazine-fed rapid cycling. |
| `Crossbow_Repeating_Light` | 3 | Lighter repeating crossbow with reduced power. |
| `Crossbow_SpanningHook` | 3 | Belt-hook or spanning-hook crossbow. |
| `Crossbow_Wall` | 3 | Very heavy wall or emplacement crossbow. |
| `Crossbow_Windlass` | 3 | Very heavy crossbow using a windlass. |

### 5. `Blowgun` prototypes — 2

Separates short and long blowgun handling within the existing blowgun runtime.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Blowgun_Long` | 9 | Long hunting or war blowgun profile. |
| `Blowgun_Short` | 6 | Compact blowgun profile. |

### 6. `Thrown` prototypes — 2

Adds disc/ring/star and thrown-club profiles within the existing thrown-weapon runtime.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Throwing_Club` | 3 | Thrown club profile. |
| `Throwing_Disc` | 6 | Thrown disc, ring, or star profile. |

### 7. `Musket` prototypes — 37

Adds ignition- and form-specific muzzleloader components. These require the runtime extension described below but remain under the existing Musket component type.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Musket_Doglock_Blunderbuss75` | 3 | Ignition- and form-specific muzzleloader profile for Doglock Blunderbuss75; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Doglock_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Doglock Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Doglock_Musket70` | 3 | Ignition- and form-specific muzzleloader profile for Doglock Musket70; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Flintlock_Blunderbuss75` | 3 | Ignition- and form-specific muzzleloader profile for Flintlock Blunderbuss75; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Flintlock_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Flintlock Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Flintlock_FowlingPiece60` | 3 | Ignition- and form-specific muzzleloader profile for Flintlock FowlingPiece60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Flintlock_WallGun80` | 3 | Ignition- and form-specific muzzleloader profile for Flintlock WallGun80; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Arquebus55` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Arquebus55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Blunderbuss75` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Blunderbuss75; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Caliver60` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Caliver60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_FowlingPiece60` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock FowlingPiece60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_HeavyMusket80` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock HeavyMusket80; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Jezail65` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Jezail65; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Musket75` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Musket75; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Tanegashima60` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Tanegashima60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_Toradar65` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock Toradar65; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Matchlock_WallGun80` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock WallGun80; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Miquelet_Blunderbuss75` | 3 | Ignition- and form-specific muzzleloader profile for Miquelet Blunderbuss75; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Miquelet_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Miquelet Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Miquelet_Escopeta65` | 3 | Ignition- and form-specific muzzleloader profile for Miquelet Escopeta65; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Miquelet_Musket70` | 3 | Ignition- and form-specific muzzleloader profile for Miquelet Musket70; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Snaphaunce_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Snaphaunce Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Snaphaunce_FowlingPiece60` | 3 | Ignition- and form-specific muzzleloader profile for Snaphaunce FowlingPiece60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Snaphaunce_LongGun65` | 3 | Ignition- and form-specific muzzleloader profile for Snaphaunce LongGun65; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Snaphaunce_Musket70` | 3 | Ignition- and form-specific muzzleloader profile for Snaphaunce Musket70; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Wheellock_Carbine60` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock Carbine60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Wheellock_FowlingPiece60` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock FowlingPiece60; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Wheellock_LongGun65` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock LongGun65; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Musket_Wheellock_Rifle55` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock Rifle55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Doglock55` | 3 | Ignition- and form-specific muzzleloader profile for Doglock55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Flintlock_Duelling45` | 3 | Ignition- and form-specific muzzleloader profile for Flintlock Duelling45; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Matchlock55` | 3 | Ignition- and form-specific muzzleloader profile for Matchlock55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Miquelet55` | 3 | Ignition- and form-specific muzzleloader profile for Miquelet55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Snaphaunce55` | 3 | Ignition- and form-specific muzzleloader profile for Snaphaunce55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Wheellock45` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock45; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |
| `Pistol_Wheellock55` | 3 | Ignition- and form-specific muzzleloader profile for Wheellock55; must carry its own bore, handling, reload, ignition, and spread/rifling settings. |

### 8. `MusketCartridge` prototypes — 11

Adds paper-cartridge, buckshot, and buck-and-ball payload profiles using the existing musket-ammunition family after the payload extension described below.

Implementation status: all seven single-projectile paper-cartridge rows are seeded with explicit charge and wad data. The four multi-projectile rows remain deferred.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `MusketBuckAndBall_0.65 Bore` | 1 | Paper cartridge carrying one ball plus buckshot for the named bore and mixed-projectile pattern. |
| `MusketBuckAndBall_0.75 Bore` | 1 | Paper cartridge carrying one ball plus buckshot for the named bore and mixed-projectile pattern. |
| `MusketBuckshot_0.55 Bore` | 1 | Buckshot charge for the named bore and spread pattern. |
| `MusketBuckshot_0.75 Bore` | 1 | Buckshot charge for the named bore and spread pattern. |
| `MusketPaperCartridge_0.45 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.55 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.6 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.65 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.7 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.75 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |
| `MusketPaperCartridge_0.8 Bore` | 1 | Paper cartridge combining a measured powder charge, wad, and compatible round ball for the named bore. |

### 9. `Wearable` prototypes — 6

Adds combined armour-coverage profiles that cannot be represented by one existing body-part wearable without losing layering or coverage.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Wear_ArmHarness` | 3 | Combined upper-arm, elbow, and forearm plate coverage without torso coverage. |
| `Wear_FullPlateHarness` | 6 | Full field/parade harness coverage including torso, limbs, hands, and feet. |
| `Wear_HalfArmourHarness` | 3 | Torso, shoulder, upper-arm, and limited hip coverage for half armour. |
| `Wear_LegHarness` | 3 | Combined thigh, knee, shin, and foot armour coverage without torso coverage. |
| `Wear_ShoulderArmHarness` | 3 | Shoulder plus complete arm coverage for reinforced arm sets. |
| `Wear_ThreeQuarterHarness` | 6 | Torso, arms, hands, hips, and thighs to knee/upper shin for three-quarter armour. |

### 10. `Container` prototypes — 1

Adds a cartridge-constrained bandolier container rather than treating every charge as an unrestricted pouch.

Implementation status: seeded using standard-container allowed and blocked functional tag lists.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Container_CartridgeBandolier` | 3 | Bandolier container sized and constrained for paper cartridges or wooden charges. |

### 11. `Sheath` prototypes — 1

Adds a paired saddle-holster profile able to accept two compatible pistols.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Holster_PairedSaddle` | 3 | Paired saddle-holster sheath profile accepting two compatible pistols. |

### 12. `HandTool` prototypes — 3

Adds craft-speed profiles for firearm, cartridge, and artillery work rather than leaving every specialist tool mechanically inert.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Tool_Artillery_General` | 5 | General artillery-operation hand tool. |
| `Tool_CartridgeMaking_General` | 2 | General paper-cartridge and measured-charge production hand tool. |
| `Tool_Gunsmithing_General` | 9 | General firearm lock, barrel, and maintenance hand tool. |

## Deferred prototypes using new component types — 58

### 1. `MuzzleloadingArtillery` prototypes — 20

New crew-served artillery runtime; guns and mortars cannot be safely or accurately represented as oversized handheld muskets.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Artillery_CoehornMortar` | 3 | Crew-served muzzleloading artillery profile for CoehornMortar; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Culverin` | 3 | Crew-served muzzleloading artillery profile for Culverin; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_DemiCannon` | 3 | Crew-served muzzleloading artillery profile for DemiCannon; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_DemiCulverin` | 3 | Crew-served muzzleloading artillery profile for DemiCulverin; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Falcon` | 3 | Crew-served muzzleloading artillery profile for Falcon; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Falconet` | 3 | Crew-served muzzleloading artillery profile for Falconet; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FieldGun12lb` | 3 | Crew-served muzzleloading artillery profile for FieldGun12lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FieldGun3lb` | 3 | Crew-served muzzleloading artillery profile for FieldGun3lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FieldGun6lb` | 3 | Crew-served muzzleloading artillery profile for FieldGun6lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FieldGun9lb` | 3 | Crew-served muzzleloading artillery profile for FieldGun9lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FieldMortar` | 3 | Crew-served muzzleloading artillery profile for FieldMortar; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_FullCannon` | 3 | Crew-served muzzleloading artillery profile for FullCannon; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Minion` | 3 | Crew-served muzzleloading artillery profile for Minion; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_NavalGun18lb` | 3 | Crew-served muzzleloading artillery profile for NavalGun18lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_NavalGun24lb` | 3 | Crew-served muzzleloading artillery profile for NavalGun24lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_NavalGun32lb` | 3 | Crew-served muzzleloading artillery profile for NavalGun32lb; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Peterero` | 3 | Crew-served muzzleloading artillery profile for Peterero; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_Saker` | 3 | Crew-served muzzleloading artillery profile for Saker; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_SiegeMortar` | 3 | Crew-served muzzleloading artillery profile for SiegeMortar; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |
| `Artillery_SwivelGun` | 3 | Crew-served muzzleloading artillery profile for SwivelGun; includes compatible ammunition family, charge range, loading cycle, elevation, recoil, and mount rules. |

### 2. `ArtilleryAmmunition` prototypes — 20

New artillery projectile family for solid shot, stone shot, bar shot, grapeshot, case shot, and fused shells.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `ArtilleryCaseShot_12lb` | 1 | Artillery case-shot payload for the named gun class. |
| `ArtilleryCaseShot_3lb` | 1 | Artillery case-shot payload for the named gun class. |
| `ArtilleryCaseShot_6lb` | 1 | Artillery case-shot payload for the named gun class. |
| `ArtilleryGrapeshot_Heavy` | 1 | Artillery grapeshot payload with the named weight class and multi-projectile spread. |
| `ArtilleryGrapeshot_Light` | 1 | Artillery grapeshot payload with the named weight class and multi-projectile spread. |
| `ArtilleryGrapeshot_Medium` | 1 | Artillery grapeshot payload with the named weight class and multi-projectile spread. |
| `ArtilleryShell_Carcass` | 1 | Fused artillery shell payload with the named shell class and effect family. |
| `ArtilleryShell_Heavy` | 1 | Fused artillery shell payload with the named shell class and effect family. |
| `ArtilleryShell_Light` | 1 | Fused artillery shell payload with the named shell class and effect family. |
| `ArtilleryShot_12lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_18lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_1lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_24lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_32lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_3lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_6lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_9lb` | 1 | Solid round-shot artillery projectile for the named nominal weight. |
| `ArtilleryShot_Bar` | 1 | Linked bar-shot projectile intended for naval rigging and structural targets. |
| `ArtilleryStoneShot_Heavy` | 1 | Dressed stone artillery projectile with the named weight class. |
| `ArtilleryStoneShot_Light` | 1 | Dressed stone artillery projectile with the named weight class. |

### 3. `ArtilleryMount` prototypes — 1

New mount relationship for swivel guns and later artillery-mount expansion.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `ArtilleryMount_Swivel` | 1 | Ship, wall, or carriage socket mount for a swivel gun. |

### 4. `BayonetAttachment` prototypes — 3

New attachment family that mounts a bayonet to a compatible firearm and applies firing/melee rules.

Implementation status: the component family and all three profiles are seeded; stock bayonet items retain their ordinary melee and beltable components.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `Bayonet_Plug` | 3 | Plug-bayonet attachment; mounted state blocks firing. |
| `Bayonet_Socket` | 3 | Socket-bayonet attachment; mounted state permits firing where the firearm mount allows it. |
| `Bayonet_Sword` | 3 | Sword-bayonet attachment; mounted state permits firing where the firearm mount allows it. |

### 5. `WeaponCarrierAttachment` prototypes — 4

New attachment family for weapon slings, lanyards, and long-gun loops.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `WeaponLanyard_Pistol` | 3 | Retention lanyard attachment for a compatible pistol. |
| `WeaponLoop_LongGun` | 3 | Belt or harness loop for carrying a compatible long gun. |
| `WeaponSling_Carbine` | 3 | Short-gun sling attachment profile. |
| `WeaponSling_LongGun` | 6 | Musket, rifle, or long-gun sling attachment profile. |

### 6. `MilitaryStandard` prototypes — 6 supported

New identity-bearing military standard family for colours, guidons, ensigns, pennants, and signal flags.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `MilitaryStandard_CavalryStandard` | 3 | Identity-bearing cavalry standard. |
| `MilitaryStandard_Guidon` | 3 | Forked or swallow-tailed guidon. |
| `MilitaryStandard_InfantryColour` | 3 | Identity-bearing infantry colour. |
| `MilitaryStandard_NavalEnsign` | 3 | Ship or naval ensign. |
| `MilitaryStandard_Pennant` | 3 | Small lance or command pennant. |
| `MilitaryStandard_SignalFlag` | 3 | Hand signal flag with configurable design and signal role. |

### 7. `SignalInstrument` prototypes — 4 supported

New audible military-signal family for drums, fifes, kettle drums, and speaking trumpets.

| Component prototype | Catalogue row uses | Purpose |
|---|---:|---|
| `SignalInstrument_FieldDrum` | 3 | Audible field-drum signals. |
| `SignalInstrument_Fife` | 3 | Audible fife signals. |
| `SignalInstrument_KettleDrum` | 3 | Audible kettle-drum signals. |
| `SignalInstrument_SpeakingTrumpet` | 3 | Projected voice or command-call signalling. |

## Ranged-weapon and attack-definition dependencies

Every new `MeleeWeapon`, `Bow`, `Crossbow`, `Blowgun`, `Thrown`, `Musket`, and `MuzzleloadingArtillery` component prototype needs a compatible weapon-type and attack-definition record. These are data dependencies, not extra item-component types.

- Sabre, smallsword, bayonet, lance, poleblade, and hooked-polearm profiles need appropriate reach, handedness, damage mix, stamina, recovery, and attack selections.
- Composite bows and yumi need draw, range, readiness, unready-event, and ammunition compatibility settings distinct from ordinary self bows.
- Crossbows need spanning-method and reload timing, with repeating and pellet profiles using appropriate magazines or projectile compatibility.
- Firearms need one generated weapon profile per item-component profile, using ignition/form/bore data rather than copied hard-coded database IDs.
- Artillery needs crew-scale attack definitions, ballistic arc or direct-fire family, area/multi-projectile handling, emplacement checks, and compatible projectile classes.

## Material audit

No new solid material is required. All primary materials used by the catalogue resolve to the maintained material export. Important existing materials include `mild steel`, `carbon steel`, `crucible steel`, `cast iron`, `bronze`, `bell bronze`, `brass`, `wrought iron`, `lead`, `tin`, `canvas`, `broadcloth`, `linen`, `wool`, `cotton`, `silk`, `satin`, `velvet`, `felt`, `cashmere`, `cow leather`, `deer leather`, `goat leather`, `rawhide`, `rattan`, `bamboo`, `horn`, `bone`, `horsehair`, `paper`, `hemp`, and the exact seeded woods used in the item rows.

Do not create a new `buff leather` material for this catalogue. Buff coats use exact existing leather materials and the new `Armour_BuffLeather` protection profile.

## Tag audit

No new tag is required. The main reference defines exact reusable tag profiles built only from maintained hierarchy paths. Shared and direct-admission rows retain their live source tags.

## Existing-component reconciliation

All component names not listed in this ledger already exist in the maintained seeded component export. The catalogue uses existing `Holdable`, destroyable, wearable, armour, shield, ammunition, bow, crossbow, sling, blowgun, musket-ball, musket-cartridge, sheath, holster, belt, beltable, container, liquid-container, stackable, insulation, cover, and rack/stand prototypes where exact matches exist.

## Acceptance criteria

- Exactly 66 supported component prototype names are seeded and exactly 90 military requests remain deferred.
- None of the 90 deferred military names appears in the maintained component catalogue.
- The six still-missing component types must be implemented before their 55 dependent prototypes are seeded.
- No new solid material or tag is created by this branch.
- Existing prototype names are reused exactly; no near-duplicate underscore/space variant is introduced.
- Firearm components are ignition-specific and data-generated; bore-only copies do not contain hard-coded database IDs.
- Plug, socket, and sword bayonets enforce their different firing rules.
- Paper cartridges, buckshot, and buck-and-ball produce their declared payloads.
- Artillery cannot be fired as an ordinary handheld weapon and accepts only compatible ammunition.
- Combined armour wear profiles cover the intended bodyparts without colliding with separate underlayers or duplicate wear interfaces.
- Weapon slings, lanyards, and loops attach to compatible weapons rather than acting as cosmetic standalone clothing.
- Standards and signal instruments persist their design/signal data and do not grant undeclared morale or magical effects.
- Unit tests validate every catalogue component, material, tag profile, and stable reference before the item seeder is enabled.
