# Animal And Mythic Attribute Scaling Report

## What changed
This pass focused specifically on seeded animal and mythical race physical scaling.

### Animals
- Ordinary animals no longer seed an all-zero racial attribute bonus prog.
- `AnimalSeeder` now builds racial physical bonuses from:
- `SizeCategory`
- attack-loadout role
- existing `BodypartHealthMultiplier`
- Selected species also get small explicit overrides where the generic rule would undersell or oversell them.

### Mythics
- Mythical races no longer use `_alwaysZero` for `AttributeBonusProg`.
- `MythicalRaceTemplate` now carries:
- an explicit `NonHumanAttributeProfile`
- an explicit `BodypartHealthMultiplier`
- `SeedRace` now applies both directly to the seeded race.

## Why it changed
The old seeding model flattened almost all non-human physical attributes.

That created two bad outcomes:

- offensive scaling depended too heavily on attack quality alone
- bodypart durability often did not match the creature fantasy

The stock attack expressions already scale heavily from `Strength`, and the stock HP model already scales from the seeded health trait. Moving animal and mythic races onto intentional physical bonuses gives the catalogue a much more believable spread without touching runtime combat architecture.

## Validation
Validation script:

- `scripts/nonhuman-animal-mythic-attribute-balance.ps1`

Assumptions:

- average base attribute from `3d6+1` = `11.5`
- deterministic degree `5`
- stock seeded attack expressions
- bodypart durability proxy = `(100 + Constitution) * BodypartHealthMultiplier`

Headline before vs after results:

| Race | Attack | Damage Before | Damage After | Bodypart Scale Before | Bodypart Scale After |
| --- | --- | ---: | ---: | ---: | ---: |
| `Mouse` | `Small Bite` | `16.5` | `4.3` | `11.2` | `10.2` |
| `Wolf` | `Carnivore Bite` | `64.9` | `77.2` | `111.5` | `115.5` |
| `Bear` | `Claw Swipe` | `79.6` | `109.0` | `156.1` | `174.3` |
| `Hippopotamus` | `Carnivore Bite` | `87.0` | `118.8` | `167.2` | `188.2` |
| `Elephant` | `Tusk Gore` | `53.9` | `89.2` | `223.0` | `259.0` |
| `Orca` | `Shark Bite` | `101.7` | `138.4` | `178.4` | `200.8` |
| `Cockatrice` | `Beak Peck` | `23.7` | `23.7` | `111.5` | `78.0` |
| `Griffin` | `Claw Swipe` | `72.3` | `89.4` | `111.5` | `188.0` |
| `Wyvern` | `Carnivore Bite` | `72.3` | `91.9` | `111.5` | `199.8` |
| `Dragon` | `Carnivore Bite` | `109.0` | `138.4` | `111.5` | `294.0` |
| `Centaur` | `Hoof Stomp` | `46.1` | `57.8` | `111.5` | `174.8` |

Important behavioural shifts:

- mice and similar tiny creatures now do nuisance damage rather than serious mauling damage
- wolves, bears, sharks, and orcas now separate more clearly from generic midline animals
- hippos, elephants, and dragons now combine size, raw damage, and bodypart durability in a way that reads much closer to expectation
- mythical races no longer all share the same effective durability frame

The strongest comparison from the deterministic run:

- `Hippopotamus` now deals about `27.6x` the average damage of a `Mouse`
- `Dragon` now deals about `32.2x` the average damage of a `Mouse`

## What was intentionally left unchanged
- runtime damage architecture
- weapon attack formulas
- stock natural-armour routing logic from the earlier non-human pass
- existing ordinary animal `BodypartHealthMultiplier` values, except that they now contribute to attribute-derived durability more meaningfully
- ordinary wound severity bands

## Second-pass considerations
- Review whether some herbivores should gain more explicit agility or charge-specialisation rather than mostly scaling through strength and constitution.
- Check whether a few large but non-aggressive animals should have lower default combat aggression despite now having more believable damage if forced to fight.
- Consider whether some mythical attacks such as `Dragonfire Breath` should get an explicit second-pass review independent of physical strength scaling.
- If future testing shows outliers, move more animals from heuristic scaling into explicit per-species profiles rather than inflating the general tables.
