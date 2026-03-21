# Combat Settings Defaults Design

## Overview

Combat settings now resolve through a shared fallback pipeline instead of relying on whichever valid global template happens to appear first in the collection. This makes NPC defaults predictable, allows builders to define race and template-specific defaults, and supports smarter global defaults through optional priority progs.

## Fallback Order

When a character needs a combat setting and does not have a valid persisted personal setting, the engine resolves defaults in this order:

1. NPC template default combat setting
2. Race default combat setting
3. Highest-priority valid global combat setting

Explicit NPC template and race defaults do not bypass normal availability checks. If a chosen global setting is not valid for the character, resolution falls through to the next source.

Persisted character combat settings remain authoritative. This feature only changes fallback selection when no valid saved setting is already in place.

For newly created characters and NPCs, fallback assignment is provisional during construction so late-initialising items do not trigger an early save before they have a database ID. Once the insert has completed and the character has its real ID, the engine revalidates the fallback selection using the normal rules above. If that final validation picks a different setting, the character is queued for a follow-up save with the corrected combat setting.

## Priority Prog

Global combat settings can now optionally define a `PriorityProg`.

- Return type: `Number`
- Parameters: a single `Toon`
- Null or missing prog result: treated as `0`
- Selection rule: highest numeric priority wins
- Tie-break: lower combat-setting ID wins

Availability progs are unchanged:

- Return type: `Boolean`
- Parameters: a single `Character`

This separation keeps priority focused on preference ordering while availability continues to determine whether a strategy is legal for a given user.

## Builder Surface

### Combat Setting Builder

Combat settings now expose:

- `combat config availprog <prog|clear>`
- `combat config priorityprog <prog|clear>`

Prog-targeting builder lookups use `ProgLookupFromBuilderInput` for consistent validation and error messaging.

### Race Builder

Races now expose:

- `combatsetting <setting>`
- `combatsetting none`

Only global combat settings may be assigned as race defaults.

### NPC Template Builder

NPC templates now expose:

- `combatsetting <setting>`
- `combatsetting none`

Only global combat settings may be assigned as NPC template defaults.

## Persistence

Two new persisted links support this feature:

- `CharacterCombatSettings.PriorityProgId`
- `Races.DefaultCombatSettingId`

NPC template defaults are stored in the template XML definition for both simple and variable templates.

## Seeder Behaviour

The seeder now treats combat strategies as name-addressable canonical content rather than assuming the combat-settings table is empty.

- `CombatSeeder` always seeds its stock combat strategies and then ensures the non-humanoid canonical catalogue exists by name.
- Animal, mythical, and robot seeders ensure the combat strategies they need before assigning race defaults.
- This keeps reruns on older worlds compatible even when new combat settings were introduced after the original install.

## Canonical Non-Humanoid Catalogue

The seeded non-humanoid defaults are:

- `Beast Brawler`
- `Beast Clincher`
- `Beast Behemoth`
- `Beast Skirmisher`
- `Beast Swooper`
- `Beast Artillery`
- `Beast Coward`
- `Construct Brawler`
- `Construct Skirmisher`
- `Construct Artillery`

Humanoid seeded races that are intended to behave like weapon-using NPCs use `Melee (Auto)`.
