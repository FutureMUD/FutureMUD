# FutureMUD Damage System Design

## Purpose
This document describes the current humanoid-facing damage pipeline in FutureMUD as implemented in the repository on 2026-04-11. It is a runtime reference and seed-data reference, not a redesign.

The focus is the stock human setup because that is the default path that fully exercises armour, bodyparts, bones, organs, wounds, and severity:

- Race and anatomy seeded by `DatabaseSeeder/Seeders/HumanSeeder.cs` and `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- Combat formulas seeded by `DatabaseSeeder/Seeders/CombatSeeder.cs`
- Damage routing implemented in `MudSharpCore/Body/Implementations/BodyBiology.cs`
- Armour math implemented in `MudSharpCore/Combat/ArmourType.cs`
- Wound creation and severity implemented in `MudSharpCore/Health/Strategies/BaseHealthStrategy.cs`, `BrainHitpointsStrategy.cs`, and `ComplexLivingHealthStrategy.cs`

## Scope
This is a `Humanoid Core` document:

- It fully covers stock human and player-facing damage.
- It groups attacks by shared seeded formula family rather than repeating every individual attack row.
- It includes the exact seeded human health formulas, human natural-armour formulas, and the seeded human bodypart, organ, and bone health scales that feed damage interpretation.
- It mentions non-human or non-humanoid behavior only where that changes the humanoid reading of the pipeline.

## Core Runtime Path
For the stock `Human Full Model` the damage path is:

`attack source -> seeded damage expression -> resolved IDamage -> worn armour layers -> magic armour -> racial natural armour -> bodypart natural armour -> bone pass-down -> organ pass-down -> health strategy wound creation -> severity interpretation`

In code terms:

1. A combat move or projectile resolves a seeded `TraitExpression` into `DamageAmount`, `PainAmount`, and `StunAmount`.
2. `BodyBiology.PassiveSufferDamage` applies worn armour in reverse wear order, so the last worn item is hit first.
3. Prosthetics can intercept the hit after worn armour.
4. Magic armour effects apply.
5. Racial natural armour applies as another reduction layer.
6. Bodypart natural armour splits the hit into:
   - damage suffered by the struck outer bodypart
   - damage passed deeper into bones and organs
7. Bone routing may create fractures or bone wounds, and bone natural armour can reduce what continues inward.
8. Organ routing may create one or more organ wounds from the remaining inward damage.
9. The active health strategy turns the final `IDamage` packets into concrete `IWound` instances.
10. Severity is interpreted from either:
   - absolute wound damage for ordinary wounds
   - percentage-of-part-health for fracture wounds

## Which Stock Human Strategy Matters Most
The stock human seeder installs three main health strategies:

| Stock Name | Type | What it means for damage |
| --- | --- | --- |
| `Human HP` | `BrainHitpoints` | Simplified hit-point model. Body routing still matters, but wound creation is simpler and there are no separate maximum pain and stun pools. |
| `Human HP Plus` | `BrainHitpoints` | Same simplified wound model, but now heart checks and hypoxia damage are enabled. |
| `Human Full Model` | `ComplexLiving` | Full stock human model. This is the canonical reference for bodypart, bone, organ, pain, stun, and fracture behavior. |

`Human Full Model` is the main reference for this document because `ComplexLivingHealthStrategy.SufferDamage` is the stock strategy that can create both ordinary organic wounds and `BoneFracture` wounds from the same end-to-end hit sequence.

## Sources Of Damage
### 1. Melee weapon attacks
Most melee weapon attacks seeded in `CombatSeeder` point at one of a small set of shared damage expressions:

- `Training`
- `Terrible`
- `Bad`
- `Poor`
- `Normal`
- `Good`
- `Very Good`
- `Great`
- `Coup de Grace`

The attack row itself contributes:

- weapon type
- attack verb and move type
- damage type such as `Slashing`, `Piercing`, or `Crushing`
- stamina, delay, reach, and combat-difficulty values

The expression contributes the actual damage magnitude.

### 2. Unarmed and natural humanoid attacks
The unarmed section in `CombatSeeder` seeds a second shared formula family for fists, feet, knees, elbows, bites, and similar natural attacks. These formulas still scale from attacker stats and check degree, but they use:

- `str + 2 * quality`

instead of:

- `str * quality`

### 3. Ranged and ammunition damage
Stock ranged damage is mostly seeded on ammunition types rather than on the launcher:

- bows and crossbows use arrow and bolt formulas
- slings use sling-bullet formulas
- modern pistols use cartridge formulas
- muskets use musket-ball formulas

The seeded projectile expressions use:

- `quality`
- `degree`
- `pointblank`

### 4. Non-attack damage that still enters body routing
Not all damage originates from a weapon attack. `BodyBiology` still receives and routes:

- `Burning`
- `Freezing`
- `Chemical`
- `Electrical`
- `Shockwave`
- `Hypoxia`
- `Cellular`
- `Sonic`
- `Necrotic`
- `Falling`
- `Eldritch`
- `Arcane`

Two damage types deserve special mention:

- `Hypoxia` and `Cellular` have custom handling in `BodyBiology` and can bypass ordinary worn-armour flow.
- `Shockwave` is treated as especially good at reaching all bones and all organs.

## Scaling Inputs
These are the variables that actually move the numbers.

### Damage source inputs
| Input | Where it comes from | Used by |
| --- | --- | --- |
| `strength` | attacker trait in seeded weapon formulas | weapon and unarmed melee formulas |
| `quality` | weapon or projectile item quality, 0-11, stock standard is 5 | nearly all seeded attack and armour formulas |
| `degree` | check success degree, usually 0-5 | weapon, unarmed, and ammunition formulas |
| `pointblank` | projectile context, `1` at own bodypart or coup-de-grace range, otherwise `0` | ammunition formulas |
| `DamageType` | attack or projectile row | armour family selection, bone routing, organ routing, damage transformation, sever checks |
| `PenetrationOutcome` | combat opposed outcome | worn-armour penetration gate |

### Armour and material inputs
| Input | Where it comes from | Used by |
| --- | --- | --- |
| armour quality | worn item quality or race natural armour quality | dissipate and absorb expressions |
| armour material | item material or body material | density, conductivity, organic-ness, and yield values |
| `strength` material parameter | shear yield for cut/pierce-like hits, impact yield for crush/shock/fall | armour dissipate and absorb formulas |
| angle | angle of incidence in degrees | available to armour expressions, though stock human formulas do not depend on it |

### Anatomy and health inputs
| Input | Where it comes from | Used by |
| --- | --- | --- |
| `Race.BodypartHealthMultiplier` | race seed | scales hitpoints and sever thresholds for all bodyparts |
| bodypart `MaxLife` | bodypart prototype seed | hitpoint basis for limbs and externals |
| organ `MaxLife` | organ prototype seed | wound cap and organ durability |
| bone `MaxLife` | bone prototype seed | fracture and bone durability |
| bodypart damage/pain/stun modifiers | bodypart prototype seed | wound creation on the struck part |
| `BoneEffectiveHealthModifier` | bone/bodypart prototype or static setting | fracture severity scaling |
| severity thresholds | health strategy definition | organ eligibility, damage-type transforms, final wound severity |

## Armour Layering And Pass-Through
### Worn layer order
`BodyBiology` builds the armour stack like this:

- collect worn items covering the struck bodypart
- drop profiles marked `NoArmour`
- reverse the list

So:

- first worn = deepest
- last worn = outermost and hit first

### Penetration gate
Each worn armour layer first runs a penetration check in `ArmourType.AbsorbDamage(IDamage damage, IArmour armour, ...)`.

If the attacker's penetration outcome beats the defender's penetration defense by at least the armour type's `MinimumPenetrationDegree`, the armour is bypassed and the original damage passes through unchanged.

### Dissipate vs absorb
Every armour layer, including natural armour, has two separate phases:

1. `Dissipate`
   - applied first
   - reduces the damage the layer itself suffers
   - if this drops damage, pain, and stun all to zero or below, nothing continues
2. `Absorb`
   - applied after the layer has already suffered its own wound
   - reduces what passes to the next layer

For non-item layers, `ArmourType.AbsorbDamage(IDamage damage, ItemQuality quality, IMaterial material, ...)` returns:

- `SufferedDamage`
- `PassThroughDamage`

For worn item layers, `ArmourType.AbsorbDamage(IDamage damage, IArmour armour, ...)`:

- creates item wounds on the armour item itself
- returns the reduced damage to the next layer

### Damage-type transformation
Armour types can transform the passed-through damage type when the passed-through severity is low enough.

The stock generic armour families, human racial armour, and human bone armour all use these transformations:

- `Slashing -> Crushing` when pass-through severity is `<= Severe`
- `Chopping -> Crushing` when `<= Severe`
- `Piercing -> Crushing` when `<= Moderate`
- `Ballistic -> Crushing` when `<= Moderate`
- `Bite -> Crushing` when `<= Severe`
- `Claw -> Crushing` when `<= Severe`
- `Shearing -> Crushing` when `<= Severe`
- `ArmourPiercing -> Crushing` when `<= Small`
- `Wrenching -> Crushing` when `<= Severe`

This matters because a hit can begin as a cut or stab and arrive at bones and organs as blunt trauma if earlier layers have already taken most of the edge off it.

### Layer order in the stock human path
For a normal external bodypart hit, the runtime order is:

1. worn armour items
2. prosthetic interception
3. magic armour effects
4. racial natural armour
5. bodypart natural armour
6. bone natural armour on any bone that gets hit

The stock human seed sets:

- `NaturalArmourQuality = 2`
- `NaturalArmourType = Human Natural Armour`

## Bodypart, Bone, And Organ Routing
### External target selection
Combat has already selected a struck bodypart by the time `BodyBiology` receives the hit. From there:

- ordinary external bodyparts are processed as outer tissue first
- bones and organs are only entered by pass-down from the struck outer part
- direct bone or organ hits can still exist if an already-routed internal hit is being recursively resolved

### Bone eligibility
`CanBreakBones` in `BodyBiology` decides whether a damage type can reach bones:

| Damage Type Group | Bone Result |
| --- | --- |
| `Hypoxia`, `Electrical`, `Cellular` | never damages bones |
| `Shockwave` | damages all bones |
| `Crushing`, `Falling`, `Chopping` | damages bones and uses grouped bone behavior |
| `Ballistic`, `Piercing`, `Slashing`, `Claw`, `Shrapnel`, `BallisticArmourPiercing`, `ArmourPiercing`, `Bite`, `Shearing`, `Wrenching`, `Eldritch`, `Arcane` | can damage one bone |

`BoneHitChance` further changes the raw seeded hit chance:

- `Piercing`, `ArmourPiercing`, `BallisticArmourPiercing`: `original^1.4`
- `Ballistic`, `Bite`: `original^1.2`
- everything else: unchanged

This makes narrow penetrating hits less likely to clip a bone than their raw coverage percentage suggests.

### Bone handling
When a bone is hit:

1. bone natural armour may reduce the damage passed farther inward
2. the bone itself is damaged with a separate `Damage` packet
3. `ComplexLivingHealthStrategy` may create a `BoneFracture` and possibly an ordinary organic wound depending on the bone/bodypart prototype type
4. if the bone covers organs, those organs can then be checked with the post-bone pass-through damage

If an existing bone wound on that bone is already `Grievous` or worse, the bone stops functioning as meaningful protection for the organs behind it.

### Organ eligibility
`CanDamageOrgans` decides whether the inward damage is eligible to create organ wounds:

| Damage Type Group | Organ Result |
| --- | --- |
| `Hypoxia`, `Electrical`, `Shockwave`, `Cellular` | always damages organs, damages all organs, high chance |
| `Crushing`, `Falling` | damages organs, not all organs, high chance |
| `Ballistic`, `BallisticArmourPiercing`, `ArmourPiercing`, `Piercing`, `Eldritch`, `Arcane` | damages organs only if severity is `>= Moderate` |
| `Burning` | damages organs if `>= Moderate`, damages all organs if `>= VerySevere`, high chance if `>= Severe` |
| default fallback | damages organs only if severity is `>= Severe` |

Organ hit chance is:

- `organInfo.HitChance / 100.0`
- doubled if `highChance`

If `damageAllOrgans` is false, the body stops after the first organ hit.

### Bone-to-organ cover
Bones can protect organs with seeded cover percentages. Examples from the stock human seed:

- skull bones cover the brain at very high percentages
- vertebrae cover spinal-cord sections
- sternum covers the heart and parts of each lung
- ribs cover lungs and parts of the heart

### Severing
After armour and internal routing, the remaining external damage can sever if all of these are true:

- the bodypart `CanSever`
- remaining external `DamageAmount >= Race.ModifiedSeverthreshold(bodypart)`
- the final damage type `CanSever()`

Race sever thresholds are:

- `bodypart.SeveredThreshold * Race.BodypartHealthMultiplier`

### Prosthetics and lodging
Two more edge cases happen in the same pipeline:

- a prosthetic targeted at the struck part or one of its downstream parts can intercept the hit after worn armour
- a `LodgableItem` can be stripped from deeper routing if an earlier layer has already lodged it in a wound

## Health Strategy Outcomes And Severity
### Stock severity interpretation
`BaseHealthStrategy.GetSeverity(double damage)` uses absolute damage bands:

| Severity | Lower | Upper |
| --- | ---: | ---: |
| `None` | -1 | 0 |
| `Superficial` | 0 | 2 |
| `Minor` | 2 | 4 |
| `Small` | 4 | 7 |
| `Moderate` | 7 | 12 |
| `Severe` | 12 | 18 |
| `VerySevere` | 18 | 27 |
| `Grievous` | 27 | 40 |
| `Horrifying` | 40 | 100 |

`BaseHealthStrategy.GetSeverityFor(IWound wound, IHaveWounds owner)` then does one of two things:

- ordinary wounds use those same absolute damage bands
- fractures use percentage bands if `UseDamagePercentageSeverities` is true

### Percentage severity bands used by `Human Full Model`
These are the seeded percentage ranges used by the stock full human model:

| Severity | Lower % | Upper % |
| --- | ---: | ---: |
| `Superficial` | 0.00 | 0.40 |
| `Minor` | 0.40 | 0.55 |
| `Small` | 0.55 | 0.65 |
| `Moderate` | 0.65 | 0.75 |
| `Severe` | 0.75 | 0.85 |
| `VerySevere` | 0.85 | 0.90 |
| `Grievous` | 0.90 | 0.95 |
| `Horrifying` | 0.95 | 1.00+ |

### Why fractures feel different
`BoneFracture.UseDamagePercentageSeverities` is true, so fracture severity is based on:

- `wound.CurrentDamage / life`

where `life` is:

- `Body.HitpointsForBodypart(bone)` for characters
- multiplied by `bone.BoneEffectiveHealthModifier` for fracture wounds

For plain internal human bones based on `BaseBoneProto`, `BoneEffectiveHealthModifier = 1.0`.

For bony bodypart prototypes based on `BonyDrapeableBodypartProto`, `BoneEffectiveHealthModifier` comes from the static setting:

- `BonyPartEffectiveHitpointForBonebreakModifier = 2.0`

## Worked Examples
The examples below use deterministic assumptions:

- stock seeder random mode: `static`
- attacker `Strength = 15`
- item quality `5` (`Standard`)
- explicitly named stock attacks from `CombatSeeder`
- stated success degree
- stock human race with `NaturalArmourQuality = 2`

For readability, the examples assume:

- flesh shear-yield baseline behaves like the stock formula comments' `25000` reference point
- flesh impact-yield baseline behaves like the stock formula comments' `10000` reference point
- bone impact-yield baseline behaves like the stock formula comments' `200000` reference point for the human bone-armour crushing examples

Those assumptions match the seeded expressions' own reference scales and keep the arithmetic legible.

### Example 1. Longsword slash into chainmail, light clothing, and abdomen
Assumptions:

- attack: `Longsword 1-Handed Swing`
- formula family: `Normal`
- damage type: `Slashing`
- success degree: `5`
- target: `abdomen`
- worn outer layer: `Chainmail`
- worn inner layer: `Light Clothing`

#### Raw attack formula
Stock static `Normal` weapon formula:

```text
max(1, 0.3 * 0.6 * (str * quality) * sqrt(degree+1))
```

Substitute values:

```text
0.18 * (15 * 5) * sqrt(6)
= 0.18 * 75 * 2.44949
= 33.07
```

So the initial `IDamage` is approximately:

- damage `33.07 slashing`
- pain `33.07`
- stun `33.07`

#### Layer 1: Chainmail
For `Chainmail`, slashing is a strong type and `powerDamage = 4.0`.

```text
dissipate = damage - quality * (powerDamage * 0.65)
          = 33.07 - 5 * 2.6
          = 20.07

absorb multiplier = 1 - powerDamage*0.03 - quality*powerDamage*0.03
                  = 1 - 0.12 - 0.60
                  = 0.28

pass-through = 20.07 * 0.28 = 5.62
```

Result after chainmail:

- chainmail item suffers a wound from `20.07`
- `5.62 slashing` continues inward

#### Layer 2: Light Clothing
For `Light Clothing`, slashing is a weak type and `powerDamage = 0.5`.

```text
dissipate = 5.62 - 5 * (0.5 * 0.1)
          = 5.62 - 0.25
          = 5.37

absorb multiplier = 1 - powerDamage*0.01 - quality*powerDamage*0.01
                  = 1 - 0.005 - 0.025
                  = 0.97

pass-through = 5.37 * 0.97 = 5.21
```

Result after light clothing:

- clothing item suffers a wound from `5.37`
- `5.21 slashing` continues inward

#### Layer 3: Human racial natural armour
Human racial natural armour, slashing:

```text
dissipate = 5.21 - 2 * 25000/25000 * 0.75
          = 5.21 - 1.5
          = 3.71

pass-through = 3.71 * 0.8 = 2.97
```

#### Layer 4: Abdomen natural armour
Abdomen natural armour uses the same human-natural-armour family:

```text
suffered by abdomen = 2.97 - 1.5 = 1.47
passed inward = 1.47 * 0.8 = 1.18
```

Because the passed-through severity is only `Superficial`, the stock damage transformation turns:

- `Slashing -> Crushing`

for the inward packet.

So after the abdomen itself is struck:

- abdomen outer wound packet = `1.47 slashing`
- inward packet = `1.18 crushing`

#### Bone and organ pass-down
The abdomen has:

- sternum internal hit chance `50%`
- liver `33%`
- spleen `20%`
- stomach `20%`

At `1.18 crushing`, the inward packet is still eligible to try bone and organ routing, but this is already a very small blunt-trauma packet. In a deterministic worked example, assume the random sternum and organ rolls miss.

#### Final wound list
Using stock `Human Full Model` severity bands:

- chainmail item wound: created from `20.07` damage on the item
- light-clothing item wound: created from `5.37` damage on the item
- abdomen wound: `1.47` damage, `Superficial`
- bone wound: none
- organ wound: none

This is the typical armored-cut outcome: most of the hit is spent on worn layers, the body only takes a superficial outer wound, and internal routing is technically possible but unlikely to matter.

### Example 2. Spear thrust through clothing into a lung
Assumptions:

- attack: `Spear 1-Handed Stab`
- formula family: `Normal`
- damage type: `Piercing`
- success degree: `3`
- target: `right breast`
- worn layer: `Heavy Clothing`

#### Raw attack formula
Same static `Normal` weapon formula:

```text
0.18 * (15 * 5) * sqrt(4)
= 0.18 * 75 * 2
= 27.00
```

Initial packet:

- damage `27.00 piercing`
- pain `27.00`
- stun `27.00`

#### Layer 1: Heavy Clothing
For `Heavy Clothing`, piercing is a weak type and `powerDamage = 0.8`.

```text
dissipate = 27.00 - 5 * (0.8 * 0.1)
          = 27.00 - 0.4
          = 26.60

absorb multiplier = 1 - 0.8*0.01 - 5*0.8*0.01
                  = 1 - 0.008 - 0.04
                  = 0.952

pass-through = 26.60 * 0.952 = 25.32
```

#### Layer 2: Human racial natural armour
Human racial natural armour, piercing:

```text
dissipate = 25.32 - 1.5 = 23.82
pass-through = 23.82 * 0.8 = 19.06
```

#### Layer 3: Right-breast natural armour
Again using the human-natural-armour family:

```text
suffered by right breast = 19.06 - 1.5 = 17.56
passed inward = 17.56 * 0.8 = 14.05
```

Here the inward packet is still `Piercing`, because `14.05` is `Severe`, not `Moderate` or below.

#### Bone and organ pass-down
Right-breast anatomy is favorable to an organ strike:

- ribs are present, but each individual rib chance is low
- the right lung is a `100%` organ target on the right breast

For a deterministic example, assume the rib checks miss and the lung is the organ that resolves.

That yields:

- outer breast wound packet = `17.56 piercing`
- right-lung organ wound packet = `14.05 piercing`

#### Final wound list
Using stock `Human Full Model` severity bands:

- heavy-clothing item wound: created from `26.60` damage on the item
- right-breast wound: `17.56` damage, `Severe`
- bone wound: none
- right-lung wound: `14.05` damage, `Severe`

This is the classic penetrating-hit path: the bodypart itself is badly wounded, enough piercing energy survives the natural-armour layers to stay a stab rather than downgrade into blunt trauma, and the organ routing then lands a second severe wound internally.

### Example 3. Warhammer crushing blow to a shin, with outer wound plus fracture
Assumptions:

- attack: `Warhammer Crush Right Leg`
- formula family: `Coup de Grace`
- damage type: `Crushing`
- success degree: `5`
- target: `right shin`
- no worn armour

This is intentionally a strong crushing example so the fracture remains visible after the full pipeline.

#### Raw attack formula
Static `Coup de Grace` formula:

```text
0.6 * str * quality * sqrt(degree+1)
= 0.6 * 15 * 5 * sqrt(6)
= 45 * 2.44949
= 110.23
```

Initial packet:

- damage `110.23 crushing`
- pain `110.23`
- stun `110.23`

#### Layer 1: Human racial natural armour
Human racial natural armour, crushing:

```text
dissipate = 110.23 - 1.5 = 108.73
pass-through = 108.73 * 0.8 = 86.98
```

#### Layer 2: Right-shin natural armour
Right shin uses the same human-natural-armour family for the outer tissue:

```text
suffered by shin = 86.98 - 1.5 = 85.48
passed inward = 85.48 * 0.8 = 68.38
```

So before internal routing:

- outer shin wound packet = `85.48 crushing`
- inward packet = `68.38 crushing`

#### Bone pass-down
The stock human seed gives the right shin:

- `rtibia` internal hit chance `100%`

The tibia therefore takes the internal hit in this worked example.

Bone natural armour does not reduce the bone's own damage packet here, because `CheckBoneDamage` applies the original inward packet directly to the bone and uses bone natural armour only for what would continue deeper.

So the tibia receives:

- fracture/bone packet = `68.38 crushing`

For a plain internal bone such as the tibia:

- `BoneEffectiveHealthModifier = 1.0`
- seeded `MaxLife = 150`
- fracture severities use the percentage model

Fracture severity ratio:

```text
68.38 / 150 = 0.4559
```

That lands in the stock `Minor` fracture band (`0.40 - 0.55`).

#### Final wound list
Using stock `Human Full Model` severity bands:

- right-shin wound: `85.48` damage, `Horrifying`
- right-tibia fracture: `68.38` damage against `150` fracture life, `Minor`
- organ wound: none for this limb path

This shows the two-layer result that matters for limb trauma:

- the flesh and surface tissue can take an ordinary wound
- the internal bone can simultaneously take a fracture wound from the same hit

## Seeded Formula Catalogue
### A. Stock human health-strategy formulas
#### `Human HP`
Type: `BrainHitpoints`

| Property | Seeded value |
| --- | --- |
| Maximum hit points | `100+con` |
| Healing tick damage | `100+con` |
| Lodge damage | `max(0, damage-15)` |
| `% health per penalty` | `0.2` |
| Check heart | `false` |
| Use hypoxia damage | `false` |
| Knockout on critical | `true` |
| Knockout duration | `240` seconds |
| Severity ranges | absolute ranges from `Superficial 0-2` to `Horrifying 40-100` |

#### `Human HP Plus`
Type: `BrainHitpoints`

Same as `Human HP`, except:

| Property | Seeded value |
| --- | --- |
| Check heart | `true` |
| Use hypoxia damage | `true` |

#### `Human Full Model`
Type: `ComplexLiving`

| Property | Seeded value |
| --- | --- |
| Maximum hit points | `100+con` |
| Maximum stun | `100+(con+wil)/2` |
| Maximum pain | `100+wil` |
| Healing tick damage | `100+con` |
| Healing tick stun | `100+con` |
| Healing tick pain | `100+con` |
| Lodge damage | `max(0, damage - 30)` |
| `% health per penalty` | `0.2` |
| `% stun per penalty` | `0.2` |
| `% pain per penalty` | `0.2` |
| Severity ranges | absolute plus percentage bands |

### B. Stock melee weapon damage formulas
The seeder switches a multiplier based on the selected repo-wide randomness mode:

| Randomness mode | `startingmultiplier` | extra random term |
| --- | ---: | --- |
| `static` | `0.6` | none |
| `partial` | `0.705882` | `* rand(0.7,1.0)` |
| `random` | `1.0` | `* rand(0.2,1.0)` |

#### Weapon formulas
| Family | Seeded formula |
| --- | --- |
| `Training` | `max(1,10-quality){random}` |
| `Terrible` | `max(1,0.1*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Bad` | `max(1,0.2*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Poor` | `max(1,0.25*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Normal` | `max(1,0.3*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Good` | `max(1,0.4*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Very Good` | `max(1,0.45*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Great` | `max(1,0.5*startingmultiplier * (str * quality) * sqrt(degree+1){random})` |
| `Coup de Grace` | `max(1,1.0*startingmultiplier * str * quality * sqrt(degree+1){random})` |

### C. Stock humanoid attack-family mapping
Grouped by shared seeded expression family:

| Weapon family | Shared formula families used by stock attacks |
| --- | --- |
| Knife | `Bad`, `Terrible`, plus stronger coup-de-grace throat attacks |
| Dagger | `Bad`, `Terrible` |
| Club | `Bad`, `Normal`, `Good` |
| Mace | `Poor` |
| Improvised | `Terrible`, `Bad`, `Poor`, `Normal`, `Good` |
| Shortsword | `Bad`, `Poor` |
| Longsword | `Bad`, `Normal`, `Good`, `Coup de Grace` |
| Zweihander | `Bad`, `Poor`, `Good`, `Great`, `Coup de Grace` |
| Axe | `Good`, `Very Good` |
| Halberd | `Terrible`, `Bad`, `Poor`, `Normal`, `Good`, `Very Good` |
| Spear / Longspear | `Bad`, `Poor`, `Normal`, `Good` |
| Rapier | `Terrible`, `Poor`, `Normal` |
| Mattock | `Bad`, `Very Good`, `Great`, `Coup de Grace` |
| Warhammer | `Bad`, `Very Good`, `Great`, `Coup de Grace` |
| Training variants | `Training` |

Representative seeded attacks:

- `Longsword 1-Handed Swing` -> `Normal`
- `Longsword 1-Handed Stab` -> `Normal`
- `Club 1-Handed Swing` -> `Normal`
- `Warhammer 1-Handed Swing` -> `Very Good`
- `Warhammer 2-Handed Heavy Swing` -> `Great`
- `Warhammer Crush Right Leg` -> `Coup de Grace`

### D. Stock unarmed and natural-humanoid formulas
These are seeded separately from armed melee.

| Family | Seeded formula |
| --- | --- |
| `Terrible` | `0.33333 * (str + (2 * quality)) * sqrt(degree+1){random}` |
| `Bad` | `0.66666 * (str + (2 * quality)) * sqrt(degree+1){random}` |
| `Normal` | `1.0 * (str + (2 * quality)) * sqrt(degree+1){random}` |
| `Good` | `1.25 * (str + (2 * quality)) * sqrt(degree+1){random}` |
| `Great` | `1.5 * (str + (2 * quality)) * sqrt(degree+1){random}` |

These are reused by seeded punches, kicks, knees, elbows, bites, and similar natural attacks.

### E. Stock ranged and ammunition formulas
#### Bow, crossbow, and sling ammunition seeded in `CombatSeeder`
| Ammunition | Damage type | Seeded formula |
| --- | --- | --- |
| Field Point Arrow | `BallisticArmourPiercing` | `15 + quality * 0.75 * degree` |
| Broadhead Arrow | `Piercing` | `30 + quality * 0.75 * degree` |
| Concussive Arrow | `Crushing` | `20 + quality * 0.75 * degree` |
| Target Arrow | `Piercing` | `quality * 0.75 * degree` |
| Padded Arrow | `Crushing` | `10-quality` |
| Field Point Bolt | `BallisticArmourPiercing` | `15 + quality * 0.75 * degree` |
| Broadhead Bolt | `Piercing` | `30 + quality * 0.75 * degree` |
| Concussive Bolt | `Crushing` | `20 + quality * 0.75 * degree` |
| Target Bolt | `Piercing` | `quality * 0.75 * degree` |
| Padded Bolt | `Crushing` | `10-quality` |
| Sling Bullet | `Crushing` | `quality * 0.75 * degree` |

#### Modern-cartridge ammunition seeded in `CombatSeeder`
| Ammunition | Seeded formula |
| --- | --- |
| `9mm Parabellum` | `(6+(pointblank*6))*quality*sqrt(degree+1)` |
| `25ACP` | `(4.5+(pointblank*6))*quality*sqrt(degree+1)` |
| `32ACP` | `(6+(pointblank*6))*quality*sqrt(degree+1)` |
| `38ACP` | `(6.5+(pointblank*6))*quality*sqrt(degree+1)` |
| `38Super` | `(7.4+(pointblank*6))*quality*sqrt(degree+1)` |
| `45ACP` | `(9+(pointblank*6))*quality*sqrt(degree+1)` |

#### Musket ammunition seeded in `CombatSeeder`
| Ammunition | Seeded formula |
| --- | --- |
| `0.8 Bore Musket Shot` | `rand(10,30) + quality * sqrt(degree+1) + (pointblank * 30)` |
| `0.75 Bore Musket Shot` | `rand(8,27) + quality * sqrt(degree+1) + (pointblank * 30)` |
| `0.7 Bore Musket Shot` | `rand(7,24) + quality * sqrt(degree+1) * 0.9 + (pointblank * 30)` |
| `0.65 Bore Musket Shot` | `rand(6,21) + quality * sqrt(degree+1) * 0.8 + (pointblank * 30)` |
| `0.60 Bore Musket Shot` | `rand(6,18) + quality * sqrt(degree+1) * 0.7 + (pointblank * 30)` |
| `0.55 Bore Musket Shot` | `rand(6,18) + quality * sqrt(degree+1) * 0.6 + (pointblank * 30)` |
| `0.45 Bore Musket Shot` | `rand(5,15) + quality * sqrt(degree+1) * 0.5 + (pointblank * 30)` |

### F. Generic seeded armour formula families from `CombatSeeder`
The helper used for nearly all stock armour families is:

#### Dissipate modifiers
| Affinity bucket | Formula suffix |
| --- | --- |
| super | `-(quality * power * 2.0)` |
| strong | `-(quality * power * 0.65)` |
| default | `-(quality * power * 0.35)` |
| weak | `-(quality * power * 0.1)` |
| zero | effectively no protection for that bucket |

`Hypoxia` and `Cellular` are left untouched by the generic helper.

#### Absorb modifiers
| Affinity bucket | Formula suffix |
| --- | --- |
| super | `*(1.0 - power*0.05 - (quality*power*0.05))` |
| strong | `*(1.0 - power*0.03 - (quality*power*0.03))` |
| default | `*(1.0 - power*0.02 - (quality*power*0.02))` |
| weak | `*(1.0 - power*0.01 - (quality*power*0.01))` |
| zero | effectively no protection for that bucket |

#### Seeded stock armour families
| Armour type | Penetration | Base diff | Stacked diff | Power damage | Power pain | Power stun |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Light Clothing | 1 | 0 | 0 | 0.5 | 0.75 | 0.6 |
| Heavy Clothing | 1 | 0 | 1 | 0.8 | 1.2 | 0.9 |
| Ultra Heavy Clothing | 1 | 1 | 2 | 1.2 | 2.5 | 1.5 |
| Level I Ballistic Armour | 1 | 1 | 2 | 3.0 | 3.0 | 3.5 |
| Level IIa Ballistic Armour | 1 | 1 | 2 | 3.75 | 4.0 | 5.0 |
| Level II Ballistic Armour | 1 | 1 | 2 | 4.5 | 5.5 | 6.6 |
| Level IIIa Ballistic Armour | 1 | 1 | 2 | 5.0 | 7.0 | 7.5 |
| Level III Ballistic Armour | 1 | 2 | 3 | 5.5 | 7.5 | 8.0 |
| Level IV Ballistic Armour | 1 | 2 | 4 | 6.25 | 8.5 | 8.9 |
| Level 1 Stab Vest | 1 | 0 | 1 | 4.0 | 7.0 | 7.5 |
| Level 2 Stab Vest | 1 | 1 | 1 | 5.0 | 7.0 | 7.5 |
| Level 3 Stab Vest | 1 | 1 | 2 | 6.0 | 7.0 | 7.5 |
| Boxing Gloves | 1 | 1 | 2 | 1.2 | 2.5 | 1.5 |
| Boiled Leather | 1 | 1 | 2 | 1.6 | 2.9 | 1.9 |
| Studded Leather | 1 | 1 | 2 | 2.1 | 3.5 | 2.5 |
| Leather Scale | 1 | 1 | 2 | 2.5 | 3.9 | 2.9 |
| Metal Scale | 1 | 1 | 2 | 4.0 | 5.0 | 4.5 |
| Laminar | 1 | 1 | 2 | 4.0 | 5.0 | 4.5 |
| Lamellar | 1 | 1 | 2 | 4.0 | 5.0 | 4.5 |
| Chainmail | 1 | 1 | 2 | 4.0 | 5.0 | 4.5 |
| Platemail | 1 | 2 | 3 | 6.0 | 6.0 | 8.0 |

Special case:

- `Hearing Protection` only meaningfully protects against `Sonic` damage, using custom direct formulas instead of the generic family helper.

### G. Stock human natural-armour formula families
#### Human racial and outer-bodypart natural armour
Name: `Human Natural Armour`

Core seeded patterns:

| Damage group | Dissipate formula |
| --- | --- |
| cut/pierce/ballistic/bite/claw/shearing/armour-piercing/shrapnel | `damage - (quality * strength/25000 * 0.75)` |
| crushing/shockwave/sonic/wrenching/falling | `damage - (quality * strength/10000 * 0.75)` |
| burning/freezing/chemical/electrical/hypoxia/cellular/necrotic/eldritch/arcane | `damage - (quality * 0.75)` |

Absorb pattern:

- most ordinary pass-through damage uses `damage * 0.8`
- `Hypoxia` and `Cellular` have no meaningful lower-layer absorb path here

#### Human organ natural armour
Name: `Human Natural Organ Armour`

Core seeded patterns:

| Damage group | Dissipate formula | Pass-through formula |
| --- | --- | --- |
| most physical types | `damage-(1.0*quality)` | `damage*(0.8-(quality*0.02))` |
| ballistic/shockwave/shrapnel | `damage*1.15-(1.0*quality)` | ballistic uses `damage*(0.9-(quality*0.02))` |
| burning/freezing/chemical/electrical | `damage-(1.0*quality)` | `damage*(0.5-(quality*0.02))` |
| armour-piercing | `damage-(1.0*quality)` | `damage*(1.0-(quality*0.02))` |
| hypoxia/cellular | `damage-(quality*0.75)` | `0` |

Organ damage is final in the stock human seed, so there is no deeper pass-down beyond the organ itself.

#### Human bone natural armour
Name: `Human Natural Bone Armour`

Core seeded patterns:

| Damage group | Dissipate formula | Pass-through formula |
| --- | --- | --- |
| cut/pierce/ballistic/bite/claw/shearing/armour-piercing/shrapnel | `max(damage*0.1, damage-(quality * 2 * strength/115000))` | usually `damage*(0.8-(quality*0.02))` |
| crushing/shockwave/sonic/wrenching/falling | `max(damage*0.1, damage-(quality * 2 * strength/200000))` | usually `damage*(0.8-(quality*0.02))` |
| burning/freezing/chemical/electrical | `damage - (quality * 2)` | `damage*(0.5-(quality*0.02))` |
| armour-piercing pass-through | same dissipate bucket | `damage*(1.0-(quality*0.02))` |
| hypoxia | `0` | `0` |

The same low-severity damage-type transformations to crushing are also seeded on human bone armour.

### H. Race-level health and sever scaling
For stock humans:

| Setting | Seeded value |
| --- | --- |
| `BodypartHealthMultiplier` | `1` |
| `NaturalArmourQuality` | `2` |
| `NaturalArmourType` | `Human Natural Armour` |

Runtime formulas:

| Concept | Runtime formula |
| --- | --- |
| modified hitpoints | `part.MaxLife * Race.BodypartHealthMultiplier` |
| modified sever threshold | `part.SeveredThreshold * Race.BodypartHealthMultiplier` |
| live body hitpoints used in severity | `Race.ModifiedHitpoints(part) * applicable merits` |

### I. Stock human bodypart health scales
Grouped by shared seeded values:

| Bodyparts | `MaxLife` | Seeded sever threshold |
| --- | ---: | ---: |
| abdomen, breasts, upper back, lower back, shoulders, shoulder blades, neck, back of neck, upper arms, elbows, forearms, wrists, hips, thighs, thigh backs | 80 | usually `-1` or `100` depending on part |
| buttocks | 40 | `-1` |
| throat, face, chin, cheeks, mouth, nose, forehead, eye sockets, groin, knees, knee backs, shins, calves, ankles, heels, feet | 40 | throat `100`, most lower-limb severables `100`, many face/groin parts `-1` |
| tongue, toes | 20 | tongue `-1`, toes `100` |
| eyes, ears, temples, testicles, penis | 10 | eyes/ears/temples `30`, testicles/penis `50` |
| nipples, thumbs, fingers | 5 | nipples `-1`, digits `100` |

Important explicit sever thresholds from the human seed:

- neck, back of neck, throat: `100`
- upper arms, elbows, forearms, wrists: `100`
- thighs, knees, knee backs, shins, calves, ankles, heels, feet: `100`
- eyes, ears, temples: `30`
- penis, testicles: `50`

### J. Stock human organ health scales
| Organ | `MaxLife` | Notes |
| --- | ---: | --- |
| brain | 50 | vital, strong stun implications |
| heart | 50 | vital |
| liver | 50 | vital support organ |
| spleen | 50 | support organ |
| stomach | 50 | support organ |
| large intestines | 50 | support organ |
| small intestines | 50 | support organ |
| kidneys | 50 each | paired organs |
| lungs | 50 each | paired organs |
| trachea | 50 | airway organ |
| esophagus | 50 | digestive conduit |
| upper, middle, lower spinal cord | 15 each | low life, high consequence |
| inner ears | 15 each | low life organ |

Representative seeded organ hit chances:

- brain from scalp: `100%`
- trachea from throat: `50%`
- right lung from right breast: `100%`
- left lung from left breast: `100%`
- heart from left breast: `33%`
- liver from abdomen: `33%`
- small intestines from abdomen: `50%`
- kidneys from lower back: `20%`

### K. Stock human bone health scales
Grouped by repeated seeded values:

| Bone group | `MaxLife` |
| --- | ---: |
| sternum, femurs | 200 |
| pelvis major bones, clavicles, scapulae, tibias, fibulas | 150 |
| major skull and facial bones, humeri, radii | 140 |
| ulnae | 120 |
| ribs 1-12 left and right | 100 each |
| patellae | 90 |
| vertebrae and spinal segments | 80 |
| calcanei, tali | 80 |
| carpals, tarsals, naviculars, cuboids, cuneiforms, many metacarpals and metatarsals | 40 |
| finger and toe phalanges | 20 |

Representative seeded internal bone hit chances:

- humerus in upper arm: `50%`
- humerus in elbow: `100%`
- radius and ulna in forearm: `33%` each
- radius and ulna in wrist: `50%` each
- patella in knee: `100%`
- tibia in shin: `100%`
- fibula in calf: `33%`

Representative bone-to-organ cover:

- skull bones -> brain: very high
- sternum -> heart: `80%`
- sternum -> each lung: `17.5%`
- ribs -> lungs and parts of heart: low-to-moderate cover by rib
- vertebrae -> matching spinal cord section: `100%`

### L. Bone-break tuning from static settings
The stock fracture split used by bony bodyparts is controlled by static settings:

| Setting | Value |
| --- | ---: |
| `BonyPartEffectiveHitpointForBonebreakModifier` | `2.0` |
| `BonyPartBoneBreakDamageSlashing` | `0.4` |
| `BonyPartBoneBreakDamageChopping` | `0.4` |
| `BonyPartBoneBreakDamageCrushing` | `0.4` |
| `BonyPartBoneBreakDamagePiercing` | `0.4` |
| `BonyPartBoneBreakDamageBallistic` | `0.4` |
| `BonyPartBoneBreakDamageShockwave` | `0.4` |
| `BonyPartBoneBreakDamageBite` | `0.4` |
| `BonyPartBoneBreakDamageClaw` | `0.4` |
| `BonyPartBoneBreakDamageShearing` | `0.4` |
| `BonyPartBoneBreakDamageArmourPiercing` | `0.4` |
| `BonyPartBoneBreakDamageWrenching` | `0.4` |
| `BonyPartBoneBreakDamageShrapnel` | `0.4` |
| `BonyPartBoneBreakDamageFalling` | `0.4` |
| `BonyPartBoneBreakDamageEldritch` | `0.4` |
| `BonyPartBoneBreakDamageArcane` | `0.4` |
| non-breaking buckets such as burning, freezing, chemical, electrical, hypoxia, cellular, sonic, necrotic | `0` |

Stock leeway values:

| Damage type | Leeway |
| --- | ---: |
| Slashing | 30 |
| Chopping | 30 |
| Crushing | 10 |
| Piercing | 40 |
| Ballistic | 30 |
| Shockwave | 20 |
| Bite | 20 |
| Claw | 40 |
| Shearing | 20 |
| ArmourPiercing | 30 |
| Wrenching | 20 |
| Shrapnel | 30 |
| Falling | 20 |
| Eldritch | 20 |
| Arcane | 20 |

## Notes On Randomness Modes
The repo-wide `CombatSeeder` randomness choice only changes the front-end attack formula multiplier:

- `static` removes the explicit random term
- `partial` adds `rand(0.7,1.0)`
- `random` adds `rand(0.2,1.0)`

The downstream body pipeline does not change. Armour layering, natural-armour routing, bone checks, organ checks, wound creation, and severity interpretation all behave the same once the `IDamage` numbers have been resolved.

## Validation Checklist
This document was written against the following source-of-truth areas:

- `DatabaseSeeder/Seeders/CombatSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeeder.cs`
- `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- `MudSharpCore/Body/Implementations/BodyBiology.cs`
- `MudSharpCore/Combat/ArmourType.cs`
- `MudSharpCore/Health/Strategies/BaseHealthStrategy.cs`
- `MudSharpCore/Health/Strategies/BrainHitpointsStrategy.cs`
- `MudSharpCore/Health/Strategies/ComplexLivingHealthStrategy.cs`
- `FutureMUDLibrary/Framework/DefaultStaticSettings.cs`

It is intentionally current-state documentation. Where the stock content reuses a shared formula family, this document preserves that grouping rather than pretending every attack row has unique math.
