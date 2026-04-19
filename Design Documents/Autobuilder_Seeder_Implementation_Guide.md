# Autobuilder Seeder Implementation Guide

## Intent

The autobuilder seeding strategy now has a clear split:

- `CoreDataSeeder` owns structural starter templates.
- `UsefulSeeder` owns terrain-aware starter room templates that produce immediately usable descriptions.

This guide covers the Useful side only.

## File Layout

The Useful autobuilder architecture lives in:

- `DatabaseSeeder/Seeders/UsefulSeeder.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.Autobuilder.cs`

`UsefulSeeder.cs` handles package exposure and installer flow.

`UsefulSeeder.Autobuilder.cs` handles:

- stock template names
- package classification
- terrain-tag lookup
- stock template generation
- repeatable ensure/update logic
- reusable XML helper builders for future autobuilder seeding

## Seeding Flow

The installed flow is:

1. `SeederQuestions` exposes the `autobuilder` question only when terrains exist and the package is not fully installed.
2. `ShouldSeedData(...)` includes autobuilder package state only when the terrain catalogue exists.
3. `SeedTerrainAutobuilder(...)` delegates to `SeedTerrainAutobuilderCore(...)`.
4. `SeedTerrainAutobuilderCore(...)`:
   - excludes the placeholder `Void` terrain
   - builds terrain tag lookup data
   - generates stock room template definitions
   - upserts them by name

This keeps the package rerunnable and safe for partial installs.

## Stock Content Policy

Useful autobuilder seeding is deliberately scoped to room templates.

Current stock templates:

- `Seeded Terrain Baseline`
- `Seeded Terrain Random Description`

Do not use UsefulSeeder to duplicate the core geometry templates unless the product direction changes intentionally. The current design assumes builders will pair Useful room templates with the shape templates already provided by `CoreDataSeeder`.

## Helper Layers

### Catalogue-level helpers

- `BuildTerrainTagLookup(...)`
- `GetTerrainsByTag(...)`
- `IsOutdoorsTerrain(...)`
- `BuildSeededTerrainBaseDescription(...)`

These convert the terrain catalogue into authoring-friendly categories without hard-coding terrain IDs.

### Template assembly helpers

- `BuildStockAutobuilderRoomTemplates(...)`
- `BuildStockRandomDescriptionElements(...)`

These functions decide what the stock package actually contains.

### XML utility helpers

These are reusable building blocks for future seeding:

- room info XML
- `room by terrain` template XML
- `room random description` template XML
- simple, road, and group description elements
- area-template XML
- feature group XML
- feature XML

Even though UsefulSeeder currently seeds only room templates, the extra helpers are there so future stock content can be added in a consistent style instead of hand-writing XML strings inline.

## Repeatability Rules

Useful autobuilder seeding must stay idempotent.

Required behaviours:

- rerunning must not duplicate templates
- rerunning must repair missing templates
- rerunning must refresh malformed or stale definitions for stock templates

That is why the code uses `SeederRepeatabilityHelper.EnsureNamedEntity(...)` and rewrites the stock definition each run.

## Adding A New Stock Room Template

Use this checklist:

1. Add the template name to `StockAutobuilderRoomTemplateNames`.
2. Add a new definition in `BuildStockAutobuilderRoomTemplates(...)`.
3. Prefer building the XML through the helper functions rather than embedding raw XML text.
4. Derive terrain applicability from seeded terrain tags or terrain behaviour, not from fragile hard-coded IDs.
5. Add a repeatability test covering install and rerun behaviour.

## Working With Random Description Templates

The main things to preserve are:

- one default room-info block
- per-terrain overrides
- a valid random element expression for the default block
- a valid random element expression for each terrain override
- at least one description element

One subtle rule matters here: terrain overrides in the random-description template still need a `NumberOfRandomElements` node. If you omit it, runtime fallback becomes `1`, which can silently change the intended behaviour.

## Area Helpers And Future Expansion

`UsefulSeeder.Autobuilder.cs` already includes helpers for:

- terrain rectangle definitions
- random feature rectangle definitions
- simple/uniform/road feature groups
- standard and adjacent features

These exist so future stock autobuilder packages can be added without inventing new XML conventions. For now, they are intentionally not used to seed default Useful area templates.

## Tests To Keep

The unit-test coverage for autobuilder seeding should always include:

- none / partial / full package classification
- installer question visibility
- successful seed through the public seeder entry point
- rerun repair without duplication

When adding new stock templates, update the stock-name assertions and verify the generated XML structure that matters for runtime.

## Practical Guidance For Future Contributors

- Treat `Void` as infrastructure, not player-facing terrain content.
- Prefer tag-driven descriptive buckets such as `Urban`, `Aquatic`, `Arid`, or `Lunar`.
- Keep UsefulSeeder focused on starter content that works immediately after install.
- Push game-specific or highly opinionated prose into cloned templates or downstream game seeders instead of broadening the stock Useful package too far.
