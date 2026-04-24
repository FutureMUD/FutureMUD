# Non-Human Damage Balance First Pass Report

## Scope
This pass extended the April 2026 seeder-only damage rebalance from stock humans to:

- ordinary animals
- mythical animal races
- robots

The work stayed in seeder and static-data authoring. It did not change the runtime routing architecture, wound-creation flow, organ or bone coverage, or weapon damage formulas.

## What Changed

### Animals

- Tightened the stock non-human `ComplexLiving` fracture percentage bands to the harsher first-pass ranges.
- Reworked `Non-Human Natural Armour` so ordinary flesh layers:
  - pass more cut / pierce damage inward
  - retain slightly more blunt attenuation than edge attenuation
  - stop converting cut / pierce damage to `Crushing` as early as before
- Added a shared sever-threshold cap in the central animal `AddBodypart` helper:
  - tiny severables cap to `12`
  - very small / explicit minor appendages cap to `18`
  - larger positive severables cap to `27`

### Mythical animals

- Humanoid-default mythical races now use `Human Racial Tissue Armour`.
- Animal-default mythical races no longer receive a separate race-level `Non-Human Natural Armour`.
- This removes the old double-layer behaviour where many animal-default mythics had both:
  - race-level non-human natural armour
  - cloned animal bodyparts already using non-human natural armour
- Existing seeded mythical races get their natural-armour default corrected when the seeder is rerun.

### Robots

- Replaced the stale robot clone assumptions with explicit robot armour templates:
  - `Robot Frame Armour`
  - `Robot Natural Armour`
  - `Robot Light Armour`
  - `Robot Internal Armour`
- Robot races now use `Robot Frame Armour` at race level instead of reusing full plating as both the racial and bodypart layer.
- Existing robot races are updated on rerun to point at the new frame layer.
- Added a shared sever-threshold cap for custom robot bodyparts:
  - tiny parts cap to `12`
  - eyes / sensors / antennae / similar minor externals cap to `18`
  - larger positive severables cap to `27`

### 2026 second-pass race attribute tuning

- Robot racial attribute bonuses are no longer size-only. Chassis role now adjusts the size baseline:
  - `Pneumatic Hammer Robot`: `Strength +6`, `Constitution +5`, `Agility -1`, `Dexterity -2`
  - `Sword-Hand Robot`: `Strength +3`, `Constitution +2`, `Agility +2`, `Dexterity +2`
  - `Winged Robot`: `Strength +1`, `Constitution +1`, `Agility +3`, `Dexterity +1`
  - `Tracked Robot`: `Strength +4`, `Constitution +5`, `Agility -2`, `Dexterity -2`
  - `Roomba Robot`: `Strength -4`, `Constitution +1`, `Agility +2`, `Dexterity +0`
  - `Robot Cockroach`: `Strength -4`, `Constitution +3`, `Agility +4`, `Dexterity +1`
- Culture-specific fantasy races now have explicit row-backed defaults rather than inline one-off calls:
  - `Elf`: `Strength -1`, `Constitution +0`, `Agility +2`, `Dexterity +3`
  - `Hobbit`: `Strength -3`, `Constitution +2`, `Agility +1`, `Dexterity +2`
  - `Dwarf`: `Strength +2`, `Constitution +4`, `Agility -1`, `Dexterity +0`
  - `Orc`: `Strength +3`, `Constitution +2`, `Agility +0`, `Dexterity -1`
  - `Troll`: `Strength +9`, `Constitution +8`, `Agility -3`, `Dexterity -4`

## Validation Snapshot
Validation used [nonhuman-damage-balance-first-pass.ps1](/C:/Users/luker/OneDrive/source/repos/FutureMUD/scripts/nonhuman-damage-balance-first-pass.ps1), which applies deterministic seeded formulas in static mode.

Representative before/after outcomes:

| Scenario | Before | After |
| --- | --- | --- |
| `Longsword -> quadruped foreleg` | `Grievous`, no sever, `Superficial` fracture read | `Grievous`, severs, `Minor` fracture read |
| `Warhammer -> quadruped head` | skull fracture reads `Minor` | skull fracture reads `Moderate` |
| `Longsword -> griffin wing` | extra mythic race armour reduces hit to `VerySevere`, no sever | no extra race layer, hit reads `Grievous`, severs |
| `Longsword -> robot upper arm` | old double-plating stack leaves only `17.14 Crushing` inward | split frame + plating leaves `23.88 Crushing` inward |
| `Axe -> robot sensor pod` | `Severe`, no sever, threshold `90` | `Grievous`, severs, threshold `18` |

Key behavioural shifts from the harness:

- major-limb animal severing is now reachable around `Grievous`
- animal fractures read materially harsher at the same inward packet
- animal-default mythical races no longer feel invisibly over-padded
- robot protection is still strong, but less binary than the old race-plus-bodypart plating stack
- custom robot severables are no longer tuned far above ordinary combat wound labels

## Intentionally Left Unchanged

- runtime routing order
- wound absolute severity bands
- animal and robot bodypart HP values
- bone HP values
- organ HP values
- organ cover and bone cover
- worn-armour family tuning from the earlier human pass
- weapon damage families and attack formulas

## Second-Pass Candidates

- Split ordinary animal natural armour into more explicit categories if future validation shows mammals, scaled reptiles, birds, and arthropods still feel too homogenised.
- Build a direct engine-backed validation harness instead of a seeded-formula mirror once a lightweight deterministic combat fixture exists.
- Revisit robot internal damage semantics separately from organic fracture language if stock robot injuries still read too “organic” in play.
- Review mythical humanoid hybrids individually if any cloned body layouts create odd edge cases around wings, horns, or merged torso regions.
