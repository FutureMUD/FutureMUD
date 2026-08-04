# FutureMUD Household Seeder Coverage Audit

## Current source result

This audit supersedes the 3 August coverage snapshot for Renaissance household rows. It is generated from the live Early Modern canonical manifest and the current 1,000-row Renaissance generated manifest, rather than relying on the earlier Renaissance count.

| Era | Direct household rows | Shared pre-industrial trade containers | Comparable available rows |
|---|---:|---:|---:|
| Medieval report reference | 1,128 (611 containers + 517 furniture) | 147 | 1,275 |
| Renaissance current source | 1,000 | 147 | 1,147 |
| Early Modern current source | 1,000 (520 furniture + 480 container/service) | 147 | 1,147 |

Early Modern and Renaissance are each 128 rows (10.04%) below the Medieval comparable baseline. The exact 1,000-direct-row and 147-shared-row target therefore lands effectively at the requested 10% parity boundary; whole rows cannot make the arithmetic exactly 90.00% of 1,275.

## Early Modern source checks

- Exact direct split: **520 furniture/fixed fixtures** and **480 container/service rows**.
- Culture coverage: **36** exact `Culture / Early Modern / Shared / ...` tags, every one on at least 15 new rows.
- Quality spread: Poor 15, Substandard 43, Standard 383, Good 414, VeryGood 73, Great 29, Excellent 43.
- Size coverage: Tiny, VerySmall, Small, Normal, Large, VeryLarge, Huge, and Enormous where the form warrants it.
- Shared generic crates, chests, sacks, bales, and commodity packaging remain in the pre-industrial layer rather than being cloned under Early Modern stable references.

## Presentation and deferred scope

Both era catalogues have one-to-one source-backed description data. Fixed furniture receives a room-facing long description; portable goods do not. Direct-output crafting routes are intentionally out of scope and remain a separate Renaissance/Early Modern production-chain pass.
