# RPI Engine Worldfile Converter Instructions

## Purpose

This project is a side-channel importer for resurrecting archived RPI Engine setting content inside FutureMUD.

It exists to:

- read archived RPI Engine source code and region files
- reconstruct world data that originally lived in `objs.*`, `mobs.*`, `rooms.*`, `crafts.txt`, and related world files
- map that data onto existing FutureMUD runtime systems and seeded content
- import the converted results into an already-seeded FutureMUD database

This is not intended to be a replacement for the normal FutureMUD `DatabaseSeeder`. The current approach is that the normal seeder establishes a valid FutureMUD baseline first, and then this converter layers Middle-earth-specific legacy content on top.

## Current Scope

Implemented or actively in progress:

- item parsing and conversion
- clan parsing and conversion
- JSON export for converted data
- dry-run and execute import paths for converter-driven database import

Planned future conversion targets:

- rooms
- NPCs
- crafts
- shops
- skills
- cultures or culture overlays where needed
- clan refinements beyond the current pass
- other hardcoded RPI data that can be reasonably inferred from source

Potential future seeder work:

- a Middle-earth-oriented skills package or seeder option set
- additional Middle-earth-specific baseline packages where the converter currently has to rely on generic seeded data

## Seeder Baseline Assumptions

Assume the target FutureMUD world has already been seeded before this converter is run.

The expected baseline is that the user has run all of the following seeders:

- `Core Seeder`
- `Time Seeder`
  - with Middle-earth options selected
- `Celestial Seeder`
  - with sun and moon selected
- `Skill Package Seeder`
  - note: a dedicated Middle-earth skills package may be desirable in future
- `Human Seeder`
- `Combat Seeder`
- `Animal Seeder`
- `Health Seeder`
- `Mythical Animal Seeder`
- `Currency Seeder`
- `Clan Seeder`
- `Useful Seeder`
- `Culture Seeder`
  - with Middle-earth options selected

The intended workflow is:

1. Run the normal FutureMUD seeders to establish the baseline world.
2. Ensure the target database contains the expected seeded materials, liquids, calendar, timezone, clan infrastructure, and other stock content.
3. Run this converter afterwards to add Middle-earth legacy content.

Do not assume this converter is being run against an empty database.

## Source of Truth

For this project, the authoritative sources are:

- the archived RPI Engine C/C++ code under `Old SOI Code/`
- the archived region files under `soiregions-main/`

Do not assume a matching legacy MySQL database exists.

If converter behavior depends on how a legacy worldfile value should be interpreted, the answer should come from the archived engine source first.

## Guardrails And Decisions So Far

- Keep this as a side-channel importer first.
  - Reusable mapping/export code is encouraged, but do not move the project wholesale into `DatabaseSeeder` unless explicitly asked.
- Prefer mapping onto existing FutureMUD runtime systems and seeded components.
  - Do not invent converter-only runtime behavior if the engine does not already support it.
- Preserve provenance.
  - Imported records should keep enough source information to trace back to the originating alias, vnum, file, or raw source values.
- Prefer dry-run-safe workflows.
  - `apply-*` commands should default to validation or dry-run behavior unless `--execute` is explicitly supplied.
- Fail loudly on missing baseline dependencies.
  - Missing seeded materials, liquids, calendars, timezones, clan infrastructure, or similar prerequisites should be surfaced as validation issues, not silently ignored.
- Prefer structured parsing over heuristics when possible.
  - If the old source code defines semantics, use that before leaning on descriptive text heuristics.
- When heuristics are necessary, keep them explicit and documented.
- Do not import legacy systems that are intentionally out of scope for the current pass.
  - Example: clan forums, paygrades, appointments, and external controls are intentionally excluded right now.

## Import Philosophy

The goal is not perfect one-to-one recreation. The goal is the best functional reconstruction that fits FutureMUD cleanly and minimizes manual rebuilding effort.

That means:

- use functional imports where the mapping is confident
- use tagged fallback imports where the source behavior is too ambiguous
- keep enough metadata that future passes can improve earlier imports without losing source intent

## Documentation Expectations

When changing converter behavior, keep the relevant mapping notes up to date:

- `RpiItemConversionMapping.md`
- `RpiClanConversionMapping.md`

If future room, NPC, craft, or shop converters are added, add matching mapping documents for them as part of the same work.

If the baseline seeder assumptions change, update this file in the same task.

## Practical Guidance For Future Agents

- Read the old source before making assumptions about legacy flags or value slots.
- Treat the FutureMUD seeded baseline as a dependency contract.
- Prefer additive, auditable changes over clever opaque heuristics.
- Keep CLI output useful for audit work.
- When scanning region files, avoid matching free-text prose as if it were structured data.
- If a future change requires brand-new runtime support in FutureMUD, pause and make that design decision explicit instead of quietly encoding it into the converter.
