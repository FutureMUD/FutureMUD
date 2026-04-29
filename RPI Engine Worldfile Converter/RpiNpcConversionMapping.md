# RPI NPC Conversion Mapping

## Purpose

This document records the current-pass rules for importing `mobs.*` from the archived RPI Engine corpus into FutureMUD NPC templates.

The NPC pass is intentionally conservative:

- import live FutureMUD NPC templates through the side-channel converter
- use `Simple` templates for ordinary mobs
- use `Variable` templates for `FLAG_VARIABLE` mobs
- defer fundamentally non-template cases such as `ACT_VEHICLE`
- preserve unsupported legacy behavior as provenance and audit warnings

## Source of Truth

NPC parsing semantics come from:

- `Old SOI Code/src/hash.cpp`
- `Old SOI Code/src/create_mobile.cpp`
- the archived `mobs.*` files in `soiregions-main`

The parser intentionally tolerates the shorter mob numeric blocks present in the preserved archive, even where newer `hash.cpp` paths contain extra fields.

## Template Mapping

One RPI mob becomes one FutureMUD NPC template.

- `FLAG_VARIABLE` => `Variable`
- all other non-deferred mobs => `Simple`
- `ACT_VEHICLE` => deferred, exported, and reported but not applied as an NPC template

Template names are deterministic:

- `RPI NPC <vnum> - <label>`

The template row name is technical metadata. The player-facing short and full descriptions remain the converted RPI descriptions.

## Race And Ethnicity Rules

Race and ethnicity mapping is intentionally high-confidence only.

Current hardcoded examples include:

- `Haradrim` => `Human + Haradrim`
- `Gondorian` cues => `Human + Gondorian`
- `Dúnedain` cues => `Human + Gondorian Dunedain` or `Human + Arnorian Dunedain`
- `Noldo` => `Elf + Noldor`
- `Sinda` / `Sindar` => `Elf + Sindar`
- `Silvan` / `Wood Elf` => `Elf + Silvan`
- `Uruk` / `Uruk-Hai` => `Orc + Uruk` / `Orc + Uruk-Hai`
- generic `Orc` cues => `Orc + Uruk`
- generic `Dwarf` cues => `Dwarf + Longbeard`
- `Cave Troll`, `Hill Troll`, `Olog-Hai` => `Troll + matching ethnicity`
- lexical animals such as wolves, wargs, horses, birds, rats, and spiders => seeded animal or mythical-animal races where available

If the converter cannot confidently resolve a race, the NPC stays exportable but is blocked from `apply-npcs`.

## Culture Rules

Culture resolution reuses the room-pass zone grouping evidence.

Order of preference:

1. race and ethnicity evidence
2. resolved room-zone group name
3. explicit lexical world cues inside the mob descriptions

Current examples:

- `Gondorian` ethnicity => `Gondorian` culture
- `Haradrim`, `Black Numenorean`, or `Umbaric` => `Haradrim` or `Corsair` depending on zone cues
- Elves use zone-backed cultures like `Rivendell`, `Lothlórien`, `Mithlond`, `Forlindon`, `Hardlindon`, and `Woodland Realm`
- Orcs use zone-backed cultures like `Mordorian Orc`, `Misty Mountains Orc`, `Grey Mountains Orc`, `Isengard Orc`, and `Mirkwood Orc`
- incorporeal spirit races such as `Wraith` use the supernatural seeder's `Spirit Court` culture
- animals, spiders, wargs, and current troll fallback use `Animal`

If the converter cannot confidently resolve culture, the NPC remains exportable but is blocked from `apply-npcs`.

## Names

FutureMUD templates require valid technical names even when the archived mob was generic.

Simple templates:

- if the first keyword looks like a real personal name, use it
- otherwise synthesize a technical name from ethnicity, culture, or race plus the source vnum

Variable templates:

- use the seeded name cultures and random-name profiles attached to the resolved ethnicity or culture
- do not attempt to reproduce legacy `create_description()` output exactly in this pass

## Stats, Skills, And Languages

Attribute aliases map directly:

- `STR` => `Strength`
- `INT` => `Intelligence`
- `WIL` => `Willpower`
- `AUR` => `Aura`
- `DEX` => `Dexterity`
- `CON` => `Constitution`
- `AGI` => `Agility`

Skill mapping uses a curated alias cleanup pass:

- `Small-Blade` => `Small Blade`
- `Shield-Use` => `Shield Use`
- `Danger-Sense` => `Danger Sense`
- and similar direct name normalisations

Unknown or unmapped legacy skills are reported explicitly and block `apply-npcs` for affected records.

Language traits require matching seeded accents. `apply-npcs` validates that a default or fallback accent exists for every imported language skill.

## Natural Armour

RPI natural armour is treated as a race-resolution concern because FutureMUD natural armour is race-level.

This pass:

- prefers seeded races and ethnicities whose natural armour is already a good fit
- emits `NaturalArmourGap`-style warnings when the archived mob armour materially exceeds the seeded race quality
- does not invent per-template natural armour overrides

## AI Mapping

Only clean FutureMUD fits are mapped in v1.

- `ACT_AGGRESSIVE` => `AggressiveToAllOtherSpecies`
- `ACT_AGGRESSIVE + ACT_MEMORY` => `TrackingAggressiveToAllOtherSpecies` with an approximation warning
- `ACT_SENTINEL` is treated as stationary intent and preserved in provenance rather than requiring a bespoke AI
- `ACT_ENFORCER` is preserved and warned unless a law baseline is explicitly available in a future pass
- `ACT_WILDLIFE` is preserved and warned unless a cleaner wildlife AI path exists

## Deferred Data

The following are parsed and exported but not converted in this pass:

- shopkeeper payloads
- venom payloads
- morph payloads
- clan memberships
- carcass and skinning metadata
- fallback and home-room semantics
- vehicle mobs

These appear in the export JSON, audit sidecars, and builder comments so later passes can extend them without reparsing the archive.

## Apply Idempotency

Imported NPC templates are stamped with a provenance marker in `EditableItem.BuilderComment`:

- `RPINPCIMPORT|<file>|<vnum>|<template-kind>|<legacy-race>|<zone-group>`

`apply-npcs` uses that marker to skip rerunning imports over the same archived source NPCs.

`apply-npcs` is intentionally partial-import capable. It validates the whole converted corpus, then creates or audits records that are ready, skips deferred records, and skips invalid records with per-NPC validation errors. This lets the import pass be run while unresolved legacy mobs remain in the audit backlog instead of blocking every ready NPC template.

## Known Backlog

- law-light enforcer AI or a formal `LawSeeder` baseline requirement
- shop conversion and live shopkeeper AI import
- memory-aware aggressor behavior closer to RPI `ACT_MEMORY`
- better wildlife, herd, predator-prey, and mount AI coverage
- spawn-time clan assignment for imported NPC templates
- richer reconstruction of legacy variable description generation
