# FutureMUD Damage System Design

## Purpose
This document describes the current stock-human damage pipeline after the April 2026 seeder first-pass balance pass. It is a current-state reference for seeded formulas, static tuning, and the runtime order that consumes them.

Primary source files:

- `DatabaseSeeder/Seeders/CombatSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- `MudSharpCore/Body/Implementations/BodyBiology.cs`
- `MudSharpCore/Combat/ArmourType.cs`
- `MudSharpCore/Health/Strategies/BaseHealthStrategy.cs`
- `MudSharpCore/Health/Strategies/BrainHitpointsStrategy.cs`
- `MudSharpCore/Health/Strategies/ComplexLivingHealthStrategy.cs`

## Core Runtime Order
For ordinary external stock-human hits, the runtime order is:

`attack damage -> worn armour -> racial natural armour -> bodypart natural armour -> bone routing -> bone natural armour -> organ routing -> wound creation -> severity interpretation -> sever check`

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
Severing still requires all of the following:

- the bodypart is severable
- the final struck-bodypart wound packet meets the modified sever threshold
- the final damage type can sever

Runtime sever threshold:

- `bodypart.SeveredThreshold * Race.BodypartHealthMultiplier`

Stock human `BodypartHealthMultiplier` remains `1`, so seeded thresholds are the live thresholds.

### Current stock-human sever policy

| Category | Target threshold | Examples |
| --- | ---: | --- |
| Major severables | `27` | neck, throat, upper arms, forearms, wrists, thighs, knees, shins, calves, ankles, heels, feet |
| Minor severables | `18` | hands, eyes, ears, temples, penis, testicles |
| Tiny severables | `12` | thumbs, fingers, toes |
| Non-severables | `-1` | torso surfaces, most face flesh, nipples, tongue, many organ-protection surfaces |

This aligns stock human severing with one-hit wound labels rather than the old `100`-point major-limb thresholds.

## Human Natural Armour Stack
Stock humans now use three separate flesh-armour identities plus unchanged bone armour.

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
- weapon damage formulas

## Non-Human Stock Notes

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

`Non-Human Natural Armour` was adjusted so that:

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
Animal bodypart sever thresholds are still authored per body, but the shared seeder now caps positive thresholds down into severity-aligned ranges instead of leaving many stock parts at `50-100`:

- tiny severables cap to `12`
- very small and explicit minor appendages cap to `18`
- other positive severables cap to `27`

This cap only lowers existing positive sever thresholds; it does not make non-severable parts severable.

## Mythical Race Armour Inheritance
Seeded mythical races now follow the stock model they are actually built from:

- humanoid-default mythical races use `Human Racial Tissue Armour`
- animal-default mythical races use no extra race-level natural armour

That removes the old accidental double-layering on animal-default mythics, where they were receiving both:

- a race-level `Non-Human Natural Armour`
- cloned animal bodyparts that already had `Non-Human Natural Armour`

Humanoid-default mythics still inherit the human racial/bodypart/cranial split through their cloned humanoid body parts plus human-style racial tissue.

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
Custom robot bodyparts now use the same first-pass cap logic as the animal sweep:

- tiny parts cap to `12`
- eyes, sensors, antennae, and similar minor externals cap to `18`
- larger positive severables cap to `27`

This mostly matters for custom robot chassis parts such as sensor pods, wheels, tracks, and attachments, because humanoid-derived robot bodies already inherit the updated humanoid sever thresholds.

## Anatomy Highlights Used In Validation
These seeded stock-human internals matter most for the first-pass validation set:

| External target | Deterministic internal relation |
| --- | --- |
| `right upper arm` | `right humerus` at `50%` chance, `140` max life |
| `right shin` | `right tibia` at `100%` chance, `150` max life |
| `right breast` | right ribs each `5%`; `right lung` bodypart coverage `100%` |
| `forehead` | `frontal cranial bone` at `100%`, `140` max life, covers `brain` at `100%` |

## Current Balance Read
The seeded first-pass outcomes now line up with the intended human-facing read:

- major-limb severing can happen at approximately `Grievous` outer wound levels
- smaller severables are tuned down from the old `100`-point regime
- fractures become materially harsher earlier without changing bone HP
- racial natural armour is now a light baseline instead of a second full flesh gate
- unhelmeted head protection is driven more by skull fracture and skull pass-through than by scalp attenuation alone
- heavy armour still protects strongly, but chain and plate leak more meaningful damage packets than before

## Known Boundaries
This pass intentionally leaves these areas alone:

- attack and weapon damage formulas
- layered routing architecture
- dissipate/absorb algorithm
- bone HP, organ HP, organ cover, and bone cover
- ordinary wound absolute severity bands

Second-pass candidates:

- whether `BallisticArmourPiercing` should also get explicit flesh-layer neutralisation rather than following the engine default transform
- whether head-specific worn armour families need a dedicated pass beyond generic family tuning
- whether some modern armour families should be flattened in the same way as the medieval families
