# Damage Balance First Pass Report

## Scope
This report covers the April 2026 first-pass stock-human damage rebalance. The work stays primarily in seeder and static data:

- `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- `DatabaseSeeder/Seeders/HumanSeeder.cs`
- `DatabaseSeeder/Seeders/CombatSeeder.cs`
- `scripts/damage-balance-first-pass.ps1`

No weapon damage formulas, bone HP, organ HP, organ cover, bone cover, or core routing architecture were changed.

## What Changed

### Sever thresholds
- Major severables were moved to `27` so one-hit severing lines up with `Grievous` outer wounds.
- Minor severables were moved to `18`.
- Tiny appendages were moved to `12`.
- Non-severables were left unchanged.

### Fracture severity interpretation
- `Human Full Model` fracture percentage bands were tightened:
  - `Moderate` now starts at `45%`
  - `Severe` now starts at `60%`
  - `VerySevere` now starts at `75%`
- Absolute ordinary-wound bands were intentionally left alone.

### Natural armour
- The old shared human flesh armour was split into:
  - `Human Racial Tissue Armour`
  - `Human Natural Flesh Armour`
  - `Human Cranial Flesh Armour`
- Racial tissue now behaves like a light baseline layer instead of a second strong flesh gate.
- Cranial flesh now passes more of the meaningful hit to the skull.
- Bone armour was left unchanged.

### Flesh-layer damage transformation
- Ordinary flesh now downgrades slash/chop-like hits to crushing only at `<= Small`.
- Ordinary flesh now downgrades pierce/ballistic/shrapnel only at `<= Superficial`.
- `ArmourPiercing` was explicitly neutralised on the new flesh layers so it no longer collapses to crushing there.

### Worn armour families
- Medieval and general armour families were retuned to flatten the top end.
- Chainmail and plate still protect very well, but they leak more damage than before and no longer erase as many hits outright.
- Heavy clothing now matters more as padding and backing against blunt trauma.

## Validation
Run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\damage-balance-first-pass.ps1
```

Validation assumptions:

- `Strength 15`
- item `quality 5`
- human natural-armour `quality 2`
- stock `static` damage mode
- chainmail worn over heavy clothing
- helmeted head case uses a `Platemail` family helmet
- stochastic internal rolls are reported as seeded chances, not sampled

### Representative before/after outcomes

| Scenario | Before | After | Read |
| --- | --- | --- | --- |
| `Longsword -> upper arm` | outer wound `23.63 VerySevere`, no sever | outer wound `29.91 Grievous`, sever | Major-limb severing now matches wound labels |
| `Axe -> shin` | outer wound `30.51 Grievous`, tibia fracture `Superficial`, no sever | outer wound `38.89 Grievous`, tibia fracture `Minor`, sever | Chopping limbs now feel more dangerous externally |
| `Spear -> breast` | inward `15.60 Piercing` | inward `20.52 Piercing` | Naked torso no longer feels over-padded |
| `Spear -> chainmail + heavy clothing` | inward `11.28 Crushing` | inward `11.26 Piercing` | Armour still strips damage, but is less binary and less eager to blunt the stab |
| `Warhammer -> shin` | tibia fracture `Minor` at `42.44%` | tibia fracture `Moderate` at `52.91%` | Fractures now read materially harsher |
| `Warhammer -> shin + mail` | tibia fracture `Superficial` | tibia fracture `Small` | Padding remains valuable but no longer erases fracture tone as easily |
| `Warhammer -> unhelmeted forehead` | skull fracture `Minor`, brain packet `48.39 Crushing` | skull fracture `Severe`, brain packet `69.36 Crushing` | Skull now does more of the meaningful work in head survivability |
| `Warhammer -> platemail helmet` | pre-skull inward `40.75`, skull fracture `Superficial` | pre-skull inward `66.63`, skull fracture `Moderate` | Plate stays strong but leaks meaningfully instead of becoming a total wall |

### Behavioural shift summary
- Major-limb sever checks in a `Longsword -> upper arm` degree sweep moved from `0/6` before to `2/6` after.
- Sword outer wounds at `Severe+` stayed high, but the top-end sever threshold now lines up with those wound labels.
- Deterministic `Warhammer -> shin` fractures at `Moderate+` moved from `0/6` before to `2/6` after.
- Unhelmeted forehead strikes now pass `91.27` inward before the skull instead of `63.67`, and the frontal-cranial fracture read moves from `Minor` to `Severe`.
- `Spear -> chainmail + heavy clothing` no longer collapses to a small blunt packet at the flesh stage; it stays a meaningful piercing threat if it gets through.

## Why These Changes Were Chosen
- Sever thresholds were the clearest mismatch between wound labels and the actual numbers a stock combat hit produces.
- Fracture severity was reading too softly because the percentage bands were forgiving, not because bone HP was necessarily wrong.
- The old human natural-armour stack made naked humans feel like they had two heavy flesh layers before bones and organs mattered.
- Head survivability was leaning too much on flesh attenuation and not enough on the skull as a distinct hard-protection layer.
- Worn armour families were stacking strong dissipate and absorb behavior in a way that made top-end protection too binary.

## Intentionally Left Unchanged
- weapon and attack damage formulas
- routing architecture
- bone HP values
- organ HP values
- organ cover and bone cover data
- ordinary wound absolute severity bands
- ballistic and stab-vest family tuning

## Second-Pass Considerations
- Review whether `BallisticArmourPiercing` should also be explicitly neutralised in flesh layers instead of inheriting the engine default transform.
- Revisit some chopping and crushing limb results once real seeded gear sets are playtested together, not just in isolation.
- Decide whether helmets should get a head-specific armour-family pass rather than continuing to rely on the generic family tables.
- Re-evaluate modern armour families after the medieval/general families have had some playtest time.
