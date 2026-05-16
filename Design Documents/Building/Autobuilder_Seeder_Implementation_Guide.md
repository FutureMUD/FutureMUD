# Autobuilder Seeder Implementation Guide

## Intent

The autobuilder seeding strategy now has a clear split:

- `CoreDataSeeder` owns structural starter templates.
- `UsefulSeeder` owns the wilderness grouped starter package: a terrain-aware random-description room template, a random-features area template, and supporting terrain feature tags.

This guide covers the Useful side only.

## File Layout

The Useful autobuilder architecture lives in:

- `DatabaseSeeder/Seeders/UsefulSeeder.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.Autobuilder.cs`
- `DatabaseSeeder/Seeders/UsefulSeeder.Autobuilder.WildernessGroupedTerrain.cs`

`UsefulSeeder.cs` handles package exposure and installer flow.

`UsefulSeeder.Autobuilder.cs` handles:

- stock package names
- package classification
- wilderness tag upsert
- repeatable ensure/update logic
- reusable XML helper builders for future autobuilder seeding

`UsefulSeeder.Autobuilder.WildernessGroupedTerrain.cs` handles:

- the wilderness grouped room-template definition
- the wilderness random-features area-template definition
- the supporting terrain feature taxonomy
- the domain/feature lookup tables that drive the seeded XML

## Seeding Flow

The installed flow is:

1. `SeederQuestions` exposes the `autobuilder` question only when terrains exist and the package is not fully installed.
2. `ShouldSeedData(...)` includes autobuilder package state only when the terrain catalogue exists.
3. `SeedTerrainAutobuilder(...)` delegates to `SeedTerrainAutobuilderCore(...)`.
4. `SeedTerrainAutobuilderCore(...)`:
   - excludes the placeholder `Void` terrain
   - ensures the wilderness terrain feature tags exist under the `Terrain` root
   - generates the stock wilderness room template definition
   - generates the stock wilderness area template definition
   - upserts all stock-owned templates by name

This keeps the package rerunnable and safe for partial installs.

## Stock Content Policy

Useful autobuilder seeding now owns a cohesive wilderness starter package rather than only room templates.

Current stock templates:

- Room: `Seeded Terrain Wilderness Grouped Description`
- Area: `Seeded Terrain Wilderness Grouped Features`

Current stock supporting tags:

- terrain feature categories under `Terrain -> Feature -> Descriptive Element`
- wilderness sensory/resource/helper tags used by the grouped package

Do not use UsefulSeeder to duplicate the core geometry templates unless the product direction changes intentionally. The current design assumes builders will either pair the stock wilderness room template with core geometry templates or use the seeded wilderness area template directly.

## Helper Layers

### Catalogue-level helpers

- `BuildSeededTerrainBaseDescription(...)`
- wilderness terrain-domain lookup builders in `UsefulSeeder.Autobuilder.WildernessGroupedTerrain.cs`

These convert the terrain catalogue into authoring-friendly categories without hard-coding terrain IDs.

### Template assembly helpers

- `BuildWildernessGroupedTerrainAutobuilderRoomTemplates(...)`
- `BuildWildernessGroupedTerrainAutobuilderAreaTemplates(...)`
- `BuildWildernessGroupedTerrainDescriptionElements(...)`

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

The same helpers are now used to seed both the stock wilderness room template and the stock wilderness area template, so future stock content can be added without hand-writing XML strings inline.

## Repeatability Rules

Useful autobuilder seeding must stay idempotent.

Required behaviours:

- rerunning must not duplicate templates
- rerunning must repair missing templates
- rerunning must refresh malformed or stale definitions for stock room and area templates
- rerunning must add any missing wilderness feature tags back into the hierarchy
- rerunning must leave legacy `Seeded Terrain Baseline` / `Seeded Terrain Random Description` rows alone if they already exist in a live database

That is why the code uses `SeederRepeatabilityHelper.EnsureNamedEntity(...)` and rewrites the stock definition each run.

## Adding Or Replacing Stock Autobuilder Content

Use this checklist:

1. Add the template name to `StockAutobuilderRoomTemplateNames`.
2. Add any stock area template name to `StockAutobuilderAreaTemplateNames`.
3. Add or update the wilderness tag definitions if new framework tags are required.
4. Prefer building the XML through the helper functions rather than embedding raw XML text.
5. Derive terrain applicability from seeded terrain/domain mappings, not from fragile hard-coded IDs.
6. Add a repeatability test covering install and rerun behaviour.

## Working With Random Description Templates

The main things to preserve are:

- one default room-info block
- per-terrain overrides
- a valid random element expression for the default block
- a valid random element expression for each terrain override
- at least one description element
- any mandatory sentence layers required by the package's design intent

One subtle rule matters here: terrain overrides in the random-description template still need a `NumberOfRandomElements` node. If you omit it, runtime fallback becomes `1`, which can silently change the intended behaviour.

Two more subtle rules matter for road-heavy stock content:

- road-aware prose that expands `$directions`, `$thedirections`, or `$dashdirections` must key off topology tags such as `Animal Trail Straight`, `Trail Bend`, or `Dirt Road Tee`, not the plain base road tag
- if the stock package needs guaranteed prose layers, the area template must emit stable marker tags and the room template must mark the corresponding description groups as `mandatory` with fixed positions

## Area Helpers And Stock Usage

`UsefulSeeder.Autobuilder.cs` includes helpers for:

- terrain rectangle definitions
- random feature rectangle definitions
- simple/uniform/road feature groups
- standard and adjacent features

These are now used by the stock wilderness package to seed `Seeded Terrain Wilderness Grouped Features`, and they remain the preferred building blocks for future stock area templates.

## Tests To Keep

The unit-test coverage for autobuilder seeding should always include:

- none / partial / full package classification
- installer question visibility
- successful seed through the public seeder entry point
- rerun repair without duplication

When adding new stock templates, update the stock-name assertions and verify the generated XML structure that matters for runtime.

For the wilderness grouped package, that XML-shape coverage should include:

- mandatory position `1` groups for primary physical prose
- mandatory position `2` groups for secondary physical prose
- no road description element triggered by a plain base road tag
- area feature groups for primary/secondary physical layers, optional sensory/resource layers, and supported road types

## Practical Guidance For Future Contributors

- Treat `Void` as infrastructure, not player-facing terrain content.
- Prefer tag-driven descriptive buckets such as `Urban`, `Aquatic`, `Arid`, or `Lunar`.
- Keep UsefulSeeder focused on starter content that works immediately after install.
- For grouped prose, prefer explicit layer-marker tags over inferring category from a feature name that may be reused in multiple roles.
- Push game-specific or highly opinionated prose into cloned templates or downstream game seeders instead of broadening the stock Useful package too far.
