# FutureMUD Damage System Design

## Purpose
This document describes the current stock damage pipeline after the April 2026 combat-balance profile work. It is a current-state reference for seeded formulas, profile-specific tuning, and the runtime order that consumes them.

Primary source files:

- `DatabaseSeeder/Seeders/CombatSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- `MudSharpCore/Body/Implementations/BodyBiology.cs`
- `MudSharpCore/Combat/ArmourType.cs`
- `MudSharpCore/Health/Strategies/BaseHealthStrategy.cs`
- `MudSharpCore/Health/Strategies/BrainHitpointsStrategy.cs`
- `MudSharpCore/Health/Strategies/ComplexLivingHealthStrategy.cs`

## Combat Balance Profiles
Stock combat content now supports two seeder-selected balance profiles:

- `stock`
- `combat-rebalance`

`HumanSeeder` is the only seeder that asks the question. The answer is recorded through the shared `SeederChoice` path and then reused by `CombatSeeder`, `AnimalSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder` without prompting again.

Current seeder behavior:

- reruns update stock-owned named records in place rather than creating duplicate races, attack suites, or armour types
- combat message style remains a separate shared concern and is still reused regardless of balance profile
- the stock profile still exposes the legacy damage-randomness question
- the combat-rebalance profile suppresses that randomness question and instead seeds RimWorld-style bodypart HP, hit chance, armour formulas, and shared damage expressions directly

Unless a section explicitly calls out both paths, the numerical tables below describe the `combat-rebalance` profile because that is where the seeded values now differ most materially from the legacy stock formulas.

## Core Runtime Order
For ordinary external stock-human hits, the runtime order is:

`attack damage -> worn armour -> racial natural armour -> bodypart natural armour -> bone routing -> bone natural armour -> organ routing -> wound creation -> severity interpretation -> sever formula / threshold check`

Important runtime details:

- Worn layers are processed outermost first.
- Racial natural armour applies only to ordinary external hits.
- Bodypart natural armour creates two outputs:
  - the struck bodypart wound packet
  - the inward packet for bones and organs
- Bone wounds are created from the inward packet before bone-armour reduction to organs.
- Organ routing uses the remaining inward packet after bone protection.
- Current armour pass-through is evaluated from the incoming packet for that layer, not from the dissipated packet.

## Health Strategy Severity
Stock full humans use `Human Full Model` (`ComplexLiving`).

### Ordinary wound absolute bands
These are unchanged in this pass:

| Severity | Lower | Upper |
| --- | ---: | ---: |
| `None` | `-1` | `0` |
| `Superficial` | `0` | `2` |
| `Minor` | `2` | `4` |
| `Small` | `4` | `7` |
| `Moderate` | `7` | `12` |
| `Severe` | `12` | `18` |
| `VerySevere` | `18` | `27` |
| `Grievous` | `27` | `40` |
| `Horrifying` | `40` | `100+` |

### Fracture percentage bands
These were tightened so meaningful fractures read serious earlier:

| Severity | Lower % | Upper % |
| --- | ---: | ---: |
| `Superficial` | `0.00` | `0.15` |
| `Minor` | `0.15` | `0.30` |
| `Small` | `0.30` | `0.45` |
| `Moderate` | `0.45` | `0.60` |
| `Severe` | `0.60` | `0.75` |
| `VerySevere` | `0.75` | `0.87` |
| `Grievous` | `0.87` | `0.95` |
| `Horrifying` | `0.95` | `1.00+` |

Only `BoneFracture` uses percentage severity interpretation in the stock organic pipeline.

## Severing
Severing now has two runtime paths, both evaluated after the struck-bodypart wound packet has been created:

- Formula path: if `damage.Bodypart.SeverFormula` is present, `BodyBiology` evaluates it through `ExpressionEngine` with `damage` and numeric `damagetype` parameters. A result of `>= 1` severs the part; `< 1` does not.
- Legacy path: if no sever formula is present, severing still requires a severable bodypart, the final struck-bodypart packet meeting the modified sever threshold, and a damage type that `CanSever()`.

Legacy runtime sever threshold:

- `bodypart.SeveredThreshold * Race.BodypartHealthMultiplier`

### Current stock-human sever policy

- `stock` keeps the legacy numeric-threshold-only path.
- `combat-rebalance` keeps threshold-driven severing for larger limbs in severity-aligned ranges, but uses probabilistic sever formulas for smaller externals such as fingers, toes, ears, eyes, hands, and feet.
- stock human `BodypartHealthMultiplier` remains `1`, so human thresholds and sever formulas are not additionally scaled at runtime.

## Human Natural Armour Stack
Stock humans now use three separate flesh-armour identities plus unchanged bone armour.

Both balance profiles seed the same armour identities. The `stock` profile keeps the older deterministic attenuation formulas, while `combat-rebalance` rewrites those named armour types in place with stochastic stop / partial-pass / full-pass style formulas.

### Race-level settings

| Setting | Value |
| --- | --- |
| `BodypartHealthMultiplier` | `1` |
| `NaturalArmourQuality` | `2` |
| `NaturalArmourType` | `Human Racial Tissue Armour` |
| `NaturalArmourMaterial` | unset |

### 1. Human Racial Tissue Armour
Role:

- light baseline tissue attenuation
- not the main outer wound-forming layer

Current behaviour:

- physical dissipate: `damage - quality * 0.5`
- cut/pierce-like pass-through: `damage * 0.95`
- blunt/shock/fall/sonic pass-through: `damage * 0.90`
- non-physical handling stays close to the old human defaults

Effect:

- naked humans no longer get a heavy second flesh gate before the bodypart layer matters

### 2. Human Natural Flesh Armour
Role:

- main ordinary outer-flesh layer for non-cranial bodyparts

Current behaviour:

- keeps the older flesh dissipate formulas tied to material shear and impact yield
- keeps the older default pass-through behaviour so the struck part still takes the main outer wound

Reference yields used by seeded human flesh materials:

| Material | Shear Yield | Impact Yield |
| --- | ---: | ---: |
| `flesh` | `25000` | `10000` |
| `bony flesh` | `50000` | `60000` |

### 3. Human Cranial Flesh Armour
Applied to:

- scalp
- back of head
- forehead
- face
- cheeks
- nose
- eye sockets
- brows
- temples

Role:

- still forms scalp and face wounds
- passes more meaningful surviving damage to the skull so skull protection does more of the real work

Current behaviour:

- keeps the same flesh dissipate logic as ordinary flesh
- physical pass-through is higher:
  - slash/chop/crush/bite/claw/shearing/wrenching: `damage * 0.92`
  - pierce/ballistic/shrapnel/AP/BAP: `damage * 0.95`

### 4. Human Natural Bone Armour
Bone armour is unchanged in this pass.

Reference compact-bone yields:

| Material | Shear Yield | Impact Yield |
| --- | ---: | ---: |
| `compact bone` | `115000` | `200000` |

Bone armour remains the main hard-protection layer between flesh and organs.

## Damage-Type Transformation Policy

### Generic worn armour and bone armour
These remain on the long-standing stock policy:

| From | To | Threshold |
| --- | --- | --- |
| `Slashing` | `Crushing` | `<= Severe` |
| `Chopping` | `Crushing` | `<= Severe` |
| `Piercing` | `Crushing` | `<= Moderate` |
| `Ballistic` | `Crushing` | `<= Moderate` |
| `Bite` | `Crushing` | `<= Severe` |
| `Claw` | `Crushing` | `<= Severe` |
| `Shearing` | `Crushing` | `<= Severe` |
| `Wrenching` | `Crushing` | `<= Severe` |
| `BallisticArmourPiercing` | `Crushing` | `<= Small` |
| `ArmourPiercing` | `Crushing` | engine default |

### Human flesh layers
Human racial, ordinary flesh, and cranial flesh now use a more conservative flesh-only downgrade policy:

| From | To | Threshold |
| --- | --- | --- |
| `Slashing` | `Crushing` | `<= Small` |
| `Chopping` | `Crushing` | `<= Small` |
| `Bite` | `Crushing` | `<= Small` |
| `Claw` | `Crushing` | `<= Small` |
| `Shearing` | `Crushing` | `<= Small` |
| `Wrenching` | `Crushing` | `<= Small` |
| `Piercing` | `Crushing` | `<= Superficial` |
| `Ballistic` | `Crushing` | `<= Superficial` |
| `Shrapnel` | `Crushing` | `<= Superficial` |
| `ArmourPiercing` | stays `ArmourPiercing` | explicitly neutralised |

This keeps edge/point blunting common on worn armour and bone, but much less aggressive on naked flesh.

## Stock Armour Family Tuning
This pass retuned the medieval and general armour families without changing the runtime armour algorithm.

| Armour family | Power damage | Current intent |
| --- | ---: | --- |
| `Light Clothing` | `0.45` | always leaky, weak edge/point baseline |
| `Heavy Clothing` | `0.90` | meaningful padding and backing, especially vs blunt |
| `Ultra Heavy Clothing` | `1.10` | stronger padding, still not hard armour |
| `Boiled Leather` | `1.40` | decent against edges, weak to piercing and heavy blunt |
| `Studded Leather` | `1.80` | better slash case, still weak to piercing/blunt |
| `Leather Scale` | `2.20` | strong vs slash/chop, default vs ordinary piercing, weak to heavy blunt/AP |
| `Metal Scale` | `3.60` | strong vs slash/chop, good but not absolute vs piercing |
| `Laminar` | `3.70` | plate-adjacent segmented armour, weaker than full plate |
| `Lamellar` | `3.50` | similar role, slightly weaker than laminar |
| `Chainmail` | `3.50` | excellent vs slash/chop, middling vs ordinary piercing, weak to heavy blunt |
| `Platemail` | `4.50` | excellent vs slash/chop, strong vs ordinary piercing, no longer as binary at the top end |

Families intentionally left unchanged in this pass:

- ballistic vest families
- stab-vest families

The `combat-rebalance` profile does additionally overwrite the shared stock weapon and unarmed damage expressions, but it keeps the existing named attack catalogue and combat-message catalogue intact.

## Non-Human Stock Notes

### Animal offensive and durability scaling
Seeded ordinary animals now use row-backed racial attribute bonuses rather than a creation-time bonus prog.

Current stock animal scaling is now authored in three layers:

- a size-based physical baseline
- an attack-loadout role adjustment
- a bodypart-health-derived toughness adjustment

That means the stock animal catalogue now uses race attributes to reinforce what the body and attack templates were already implying:

- tiny nuisance animals lose a large amount of effective `Strength` and `Constitution`
- ordinary predators gain moderate `Strength` with some speed retained
- large apex predators and megafauna gain major `Strength` and `Constitution`
- the existing `BodypartHealthMultiplier` values now line up with the same offensive/durability story instead of floating beside a flat attribute model

Representative seeded outcomes from the April 2026 animal and mythic attribute pass:

- `Mouse`: average stock bite packet dropped from about `16.5` to `4.3`
- `Wolf`: average stock bite packet rose from about `64.9` to `77.2`
- `Bear`: average claw packet rose from about `79.6` to `109.0`
- `Hippopotamus`: average bite packet rose from about `87.0` to `118.8`
- `Elephant`: average gore packet rose from about `53.9` to `89.2`
- `Orca`: average bite packet rose from about `101.7` to `138.4`

### Animal full-model fracture bands
Stock non-human `ComplexLiving` bodies now use the same tightened percentage bands as stock full humans:

| Severity | Lower % | Upper % |
| --- | ---: | ---: |
| `Superficial` | `0.00` | `0.15` |
| `Minor` | `0.15` | `0.30` |
| `Small` | `0.30` | `0.45` |
| `Moderate` | `0.45` | `0.60` |
| `Severe` | `0.60` | `0.75` |
| `VerySevere` | `0.75` | `0.87` |
| `Grievous` | `0.87` | `0.95` |
| `Horrifying` | `0.95` | `1.00+` |

### Animal natural armour
Ordinary seeded animals still do not get a separate race-level natural-armour layer. Their stock external routing remains:

`attack damage -> bodypart natural armour -> bone routing -> bone natural armour -> organ routing`

Under `combat-rebalance`, `Non-Human Natural Armour` is rewritten in place so that:

- cut and pierce hits pass more meaningful damage inward
- blunt hits still attenuate more than edges and points, but less than before
- ordinary flesh layers are less eager to convert edge and point damage into `Crushing`

Current non-human flesh-only downgrade policy:

| From | To | Threshold |
| --- | --- | --- |
| `Slashing` | `Crushing` | `<= Small` |
| `Chopping` | `Crushing` | `<= Small` |
| `Bite` | `Crushing` | `<= Small` |
| `Claw` | `Crushing` | `<= Small` |
| `Shearing` | `Crushing` | `<= Small` |
| `Wrenching` | `Crushing` | `<= Small` |
| `Piercing` | `Crushing` | `<= Superficial` |
| `Ballistic` | `Crushing` | `<= Superficial` |
| `Shrapnel` | `Crushing` | `<= Superficial` |
| `ArmourPiercing` | stays `ArmourPiercing` | explicitly neutralised |

Current non-human natural-armour pass-through:

- cut / pierce / slash-like damage: `damage * 0.90`
- impact-like damage: `damage * 0.85`
- other damage: `damage * 0.80`

### Animal sever policy
Animal severing is now profile-aware:

- `stock` continues to use legacy numeric thresholds
- `combat-rebalance` leaves many major parts on thresholds but adds probabilistic sever formulas to minor appendages, tails, and wings where the seeded body family calls for it
- size and bodypart role now also feed the seeded bodypart HP and hit-chance values for stock animal bodies

## Mythical Race Armour Inheritance
Seeded mythical races now follow the stock model they are actually built from:

- humanoid-default mythical races use `Human Racial Tissue Armour`
- animal-default mythical races use no extra race-level natural armour

That removes the old accidental double-layering on animal-default mythics, where they were receiving both:

- a race-level `Non-Human Natural Armour`
- cloned animal bodyparts that already had `Non-Human Natural Armour`

Humanoid-default mythics still inherit the human racial/bodypart/cranial split through their cloned humanoid body parts plus human-style racial tissue.

Under `combat-rebalance`, reruns also refresh mythic bodyparts in place from their reference humanoid or animal bodies so the profile's HP, hit-chance, armour, and sever-formula tuning propagates without seeding duplicate races.

## Mythical Offensive and Durability Scaling
Seeded mythical races now use row-backed racial attribute bonuses rather than the old shared all-zero prog.

Current stock mythical tuning is explicit per race:

- every mythic race now carries a seeded `NonHumanAttributeProfile`
- every mythic race now carries an intentional `BodypartHealthMultiplier`
- `SeedRace` turns that profile into per-attribute `Races_Attributes.AttributeBonus` rows

This means mythical catalogue entries now have distinct physical ceilings instead of sharing the same baseline:

- small mythics such as `Cockatrice` stay dangerous mainly through attack type and temperament rather than giant raw stats
- combat hybrids such as `Griffin`, `Wyvern`, and `Centaur` now sit clearly above ordinary animals of comparable scale
- colossal mythics such as `Dragon` and `Eastern Dragon` now combine top-end attack quality with appropriately huge physical stats and bodypart HP scaling

Representative seeded outcomes from the same deterministic validation:

- `Cockatrice`: average peck damage stays around `23.7`, but bodypart scale drops from `111.5` to `78.0`
- `Griffin`: average claw packet rises from about `72.3` to `89.4`, bodypart scale from `111.5` to `188.0`
- `Wyvern`: average bite packet rises from about `72.3` to `91.9`, bodypart scale from `111.5` to `199.8`
- `Dragon`: average bite packet rises from about `109.0` to `138.4`, bodypart scale from `111.5` to `294.0`
- `Centaur`: average hoof packet rises from about `46.1` to `57.8`, bodypart scale from `111.5` to `174.8`

## Robot Protection Stack
Seeded robots now use a three-part protection model rather than reusing old human-flesh armour definitions for every layer.

### Race-level robot armour

| Setting | Value |
| --- | --- |
| `NaturalArmourType` | `Robot Frame Armour` |
| `NaturalArmourMaterial` | `Robot Chassis Alloy` |
| `NaturalArmourQuality` | `4` |

`Robot Frame Armour` is a light structural baseline, not the main protective wall.

### Robot bodypart armour

| Layer | Role |
| --- | --- |
| `Robot Natural Armour` | main heavy plating for articulated robot externals |
| `Robot Light Armour` | lighter external plating for utility and small chassis bodies |
| `Robot Internal Armour` | internal component damping for robot organs / circuitry |

Robot plating now:

- leaks more meaningful damage than the old double-cloned natural-armour stack
- still tends to blunt defeated cuts and chops into `Crushing`
- stays stronger against edges than against heavy crushing or determined penetration

### Robot sever policy
Robot severing is also profile-aware:

- `stock` continues to use legacy numeric thresholds
- `combat-rebalance` adds sever formulas for sensors, antennae, wheels, tracks, wings, and mandibles while leaving other parts on the legacy path
- robot bodypart HP, hit chance, and race `BodypartHealthMultiplier` now scale from chassis size in the rebalance profile

## Anatomy Highlights Used In Validation
These seeded stock-human internals matter most for the profile-validation set:

| External target | Deterministic internal relation |
| --- | --- |
| `right upper arm` | `right humerus` at `50%` chance, `140` max life |
| `right shin` | `right tibia` at `100%` chance, `150` max life |
| `right breast` | right ribs each `5%`; `right lung` bodypart coverage `100%` |
| `forehead` | `frontal cranial bone` at `100%`, `140` max life, covers `brain` at `100%` |

## Current Balance Read
The seeded combat-rebalance outcomes now line up with the intended human-facing read:

- major-limb severing can happen at approximately `Grievous` outer wound levels
- smaller severables are tuned down from the old `100`-point regime
- fractures become materially harsher earlier without changing bone HP
- racial natural armour is now a light baseline instead of a second full flesh gate
- unhelmeted head protection is driven more by skull fracture and skull pass-through than by scalp attenuation alone
- heavy armour still protects strongly, but chain and plate leak more meaningful damage packets than before
- stock and combat-rebalance continue to share the same named attack suites and combat-message catalogues, so the main behavioural change is in formulas and body durability rather than in content identity

## Known Boundaries
This pass intentionally leaves these areas alone:

- attack catalogue identities and combat-message formatting surfaces
- layered routing architecture
- dissipate/absorb algorithm
- bone HP, organ HP, organ cover, and bone cover
- ordinary wound absolute severity bands
- consciousness, movement, breathing, and the other downstream health-state rules outside the damage-routing and anatomy-tuning surfaces

Second-pass candidates:

- whether `BallisticArmourPiercing` should also get explicit flesh-layer neutralisation rather than following the engine default transform
- whether head-specific worn armour families need a dedicated pass beyond generic family tuning
- whether some modern armour families should be flattened in the same way as the medieval families
