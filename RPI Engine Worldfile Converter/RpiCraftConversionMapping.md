# RPI Craft Conversion Mapping

## Purpose

This document records how archived RPI Engine `crafts.txt` data is interpreted by the side-channel converter and mapped onto FutureMUD crafting concepts.

The craft converter is intended to run after the normal Middle-earth seeder baseline has already been applied. It imports live FutureMUD crafts, not seeder templates.

## Source Of Truth

Craft behavior is reconstructed from:

- `soiregions-main/crafts.txt`
- archived RPI Engine craft semantics in the preserved source tree

When there is ambiguity, prefer explicit legacy craft structure over descriptive heuristics.

## CLI Workflow

The craft pass exposes three commands:

- `analyze-crafts`
- `export-crafts`
- `apply-crafts`

`apply-crafts` defaults to dry-run behavior unless `--execute` is supplied.

## Parser Coverage

The parser captures:

- craft header fields: craft name, subcraft name, command
- top-level directives: `fail`, `failobjs`, `failmobs`, `flags`, `sectors`, `seasons`, `weather`, `opening`, `race`, `clans`, `followers`, `ic_delay`, `fail_delay`, `start_key`, `end_key`
- phase directives: `1st`, `2nd`, `3rd`, `self`, `group`, `1stfail`, `2ndfail`, `3rdfail`, `groupfail`, `t`, `cost`, `skill`, `attr`, `tool`, `mob`, `spell`, `open`, `req`
- phase item lists and fail item lists
- raw source lines and parse warnings

## Input And Tool Mapping

Legacy item list roles map as follows:

- `used` -> FutureMUD `CraftInput`
- `held` -> FutureMUD `CraftTool` with desired state `Held`
- `wielded` -> FutureMUD `CraftTool` with desired state `Wielded`
- `worn` -> FutureMUD `CraftTool` with desired state `Worn`
- `in-room` -> FutureMUD `CraftTool` with desired state `InRoom`

Normalization order for multi-vnum requirements is:

1. use a shared reusable tag already present on all referenced converted items
2. if that fails, create a reusable converter-owned family tag when a strong shared family can be inferred, e.g. knives
3. if that still fails, create an explicit-set converter-owned tag for that craft slot

Single-vnum requirements remain direct prototype references.

## Trait Resolution

Legacy skill and attribute names are resolved through curated alias maps.

- common weapon and utility skill names resolve to seeded FutureMUD traits
- common attributes resolve to seeded FutureMUD attribute traits
- unresolved checks are dropped from the runtime mapping and surfaced as `unresolved-craft-trait` warnings

When multiple checks exist:

- repeated or mixed checks are reduced to one primary FutureMUD craft check
- skill checks are preferred over attribute checks when both appear in the same phase ordering slot
- non-primary checks are preserved as warnings and raw metadata

## Constraint Mapping

The converter generates FutureProg plans for:

- hidden craft listing behavior
- terrain requirements derived from legacy sectors
- seasonal requirements
- weather requirements
- opening-skill requirements
- race requirements
- clan and clan-rank requirements
- follower-count requirements

Clan-rank handling has one deliberate compatibility fallback:

- if a craft requires clan rank `member` or `membership` but the imported FutureMUD clan only exposes guild-path or military-path ranks, the converter treats that as an any-membership requirement rather than failing the craft

This is primarily to support legacy guild-style aliases where the archived data mixes `member` checks with clans that effectively use `Apprentice` / `Journeyman` / `Master` rank ladders.

Legacy race constraints are normalized onto seeded FutureMUD race names rather than preserved as ethnicity-like subraces. Examples:

- `Beorian Human`, `Marachian Human`, `Haladin Human`, `Harad Human`, `Easterling Human` -> `Human`
- `Noldo Elf`, `Sinda Elf`, `Avar Elf` -> `Elf`
- `Cave Troll`, `Hill Troll` -> `Troll`
- `Giant Spider` -> `Spider`
- `Warhorse` -> `Horse`

Current sector mapping is intentionally conservative:

- `inside` -> `Hall`
- `city` -> `Urban Street`
- `road` -> `Compacted Dirt Road`
- `trail` -> `Trail`
- `field` -> `Field`
- `woods` / `forest` -> `Broadleaf Forest`
- `hills` -> `Hills`
- `mountain` -> `Mountainside`
- `swamp` -> `Temperate Freshwater Swamp`
- `dock` -> `Riverbank`
- `crowsnest` -> `Rooftop`
- `pasture` -> `Pasture`
- `heath` -> `Heath`
- `pit` -> `Dungeon`
- `lean to` -> `Cave Entrance`
- `lake` -> `Lake`
- `river` -> `River`
- `ocean` -> `Ocean`
- `reef` -> `Reef`
- `underwater` -> `Deep Lake`

If a craft is `hidden`, the generated `AppearInCraftsListProg` should suppress it from listings.

## Cost Mapping

- `cost moves` maps to phase `StaminaUsage` and a derived exertion level
- `cost hits` is preserved as provenance only in pass one and raised as `legacy-hit-cost`
- `ic_delay` and `fail_delay` are preserved as metadata and warnings only

## Product Mapping

Legacy craft outputs map as follows:

- ordinary `produced` outputs -> `SimpleProduct`
- `give` outputs -> `SimpleProduct` with `legacy-give-output` audit warning
- recoverable fail outputs that mirror one consumed input -> `UnusedInput`
- keyed `start_key` / `end_key` outputs -> converter-generated `Prog` product selection

Top-level `failobjs` and per-phase `Fail n` lists become fail products where possible.

## Deferred Behavior

The following are intentionally deferred in the current pass:

- phase `mob` outputs
- top-level `failmobs`
- legacy spell, open, and req phase directives beyond raw preservation

Crafts that rely on deferred mobile output behavior are marked `Deferred` and skipped by `apply-crafts`.

Crafts with no importable products after mapping are also marked `Deferred`.

## Baseline Validation

`apply-crafts` validates against the seeded FutureMUD database and reports missing dependencies for:

- traits
- terrains
- races
- weather events
- clans and clan ranks
- item prototypes imported by the item converter
- non-generated tags

Generated converter-owned tags and FutureProgs are created on demand during execute mode.

## Known Current Tradeoffs

- keyed craft variants currently prefer import-safe `Prog` product selection rather than deeper item-variable reconstruction
- follower constraints are enforced through generated FutureProg checks, not through a dedicated craft runtime extension
- direct-to-inventory legacy semantics for `give` are not recreated; products appear as normal craft outputs
- OOC cooldown behavior is intentionally not recreated
