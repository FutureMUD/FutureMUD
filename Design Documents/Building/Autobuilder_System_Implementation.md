# Autobuilder System Implementation

## Purpose

The autobuilder subsystem exists to let builders generate cells in bulk without hand-linking and hand-describing every location. It is intentionally split into two cooperating models:

- Area templates decide topology: how many cells to create, which cells exist, how exits are linked, and what arguments the builder must supply.
- Room templates decide presentation: terrain, cell name, description text, light, outdoors behaviour, forage profile, and any description randomisation.

This split is the key design choice to preserve when extending the subsystem.

## Core Contracts

### `IAutobuilderArea`

Area templates are the orchestration layer.

- `Parameters` exposes the ordered argument contract for builders.
- `TryArguments(...)` validates raw builder input and returns typed arguments.
- `ExecuteTemplate(...)` creates cells and exits from those arguments.
- `ShowCommandByLine` is the short summary shown in builder listings.
- `Clone(...)` creates a persistent copy with its own database row.

`AutobuilderAreaBase` supplies the shared persistence, rename/summary editing, and parameter parsing pipeline.

### `IAutobuilderRoom`

Room templates are the cell materialisation layer.

- `CreateRoom(...)` creates a new cell for a supplied terrain.
- `RedescribeRoom(...)` updates an already-created cell, typically after area tags/features are known.
- `ShowCommandByline` is the short summary shown in room-template listings.
- `Clone(...)` creates a persistent copy with its own database row.

`AutobuilderRoomBase` owns shared save logic and the `ApplyAutobuilderTagsAsFrameworkTags` toggle. When enabled, any autobuilder tag that matches a framework tag name is applied directly to the created cell.

### `IAutobuilderParameter`

Parameters are deliberately simple and ordered.

- They describe one slot in the builder argument list.
- Validation can inspect already-parsed earlier arguments.
- This is how terrain masks, feature masks, room-template references, and numeric dimensions are all unified behind the same pipeline.

## Persistence Model

Autobuilders are persisted as data, not code configuration.

- `AutobuilderAreaTemplate` stores `Id`, `Name`, `TemplateType`, and XML `Definition`.
- `AutobuilderRoomTemplate` stores `Id`, `Name`, `TemplateType`, and XML `Definition`.

The runtime contract is:

1. `TemplateType` selects the concrete loader.
2. `Definition` is parsed by that loader.
3. Edits round-trip back into XML via `SaveToXml()`.

There is no schema-version layer in front of these XML definitions, so backward compatibility depends on loaders remaining tolerant of missing elements and sensible defaults.

## Loader Architecture

`AutobuilderFactory` is the registry and dispatch point.

- Concrete template classes expose a static `RegisterAutobuilderLoader()` method.
- `AutobuilderFactory.InitialiseAutobuilders()` scans the assembly with reflection and invokes those registration methods.
- Loaders are registered twice:
  - persisted load by `TemplateType`
  - builder creation by the short type keyword used in `autoarea edit new ...` or `autoroom edit new ...`

This means any new autobuilder type needs four things:

1. a concrete class
2. XML load/save support
3. static registration
4. a stable `TemplateType` string

## Built-In Template Families

### Room templates

- `simple`: one fixed name/description pair
- `room by terrain`: deterministic per-terrain overrides
- `room random description`: per-terrain defaults plus weighted description elements

### Area templates

- `rectangle`
- `rectangle diagonals`
- `terrain rectangle`
- `terrain feature rectangle`
- `room by terrain random features`
- `cylinder`

The important distinction is that only the area side knows about masks and exit linking. Only the room side knows about description composition.

## Runtime Flow

The normal execution path is:

1. Builder opens a cell overlay package.
2. Builder runs `cell new <autoarea> ...`.
3. The chosen `IAutobuilderArea` parses its ordered parameters.
4. The area template creates cells, linking exits as it goes.
5. The area template calls the selected `IAutobuilderRoom` for each created cell.
6. Some area templates gather feature tags first and call `RedescribeRoom(...)` afterwards so descriptions can react to those tags.
7. Optional `prog=<prog>` arguments can run post-processing against the generated result set.

This is why area templates should stay focused on structure and room templates on presentation.

## Random Description Pipeline

`AutobuilderRoomRandomDescription` is the descriptive workhorse.

It combines:

- a default `AutobuilderRoomInfo`
- optional per-terrain `AutobuilderRoomInfo` overrides
- an expression for how many random elements to pick
- a weighted list of `IAutobuilderRandomDescriptionElement`
- an optional fixed sentence appended to all results

Element types currently supported:

- simple description elements
- road-aware description elements
- grouped weighted elements

Elements can be:

- terrain-limited
- tag-limited
- mandatory if valid
- fixed to a sentence position
- weighted relative to siblings

`CreateRoom(...)` and `RedescribeRoom(...)` both use the same mandatory/fixed-position selection rules, so area-generated feature tags can reliably enforce ordered prose layers.

Road elements are special because they interpret one tag as `tag=directions` and expand `$directions`, `$thedirections`, and `$dashdirections` tokens. In practice that means seeded road prose must key off topology tags such as `Trail Straight`, `Trail Bend`, or `Dirt Road Tee`, not a plain base tag like `Trail`.

## Feature Propagation

There are two different feature/tag paths:

- `terrain feature rectangle` takes an explicit feature mask from the builder.
- `room by terrain random features` generates features internally through feature groups.

Feature groups currently include:

- simple density-based feature groups
- uniform per-room feature groups
- road feature groups

Those features become string tags passed into the room template. If `ApplyAutobuilderTagsAsFrameworkTags` is on, matching framework tags are also written to the cells themselves.

The stock wilderness package also uses internal marker tags such as `Physical Primary` and `Physical Secondary` so its mandatory first and second prose layers remain stable even when the same descriptive feature name is reused in different terrain domains.

## Seeder Split: Core vs Useful

The seeder intentionally divides responsibilities.

### `CoreDataSeeder`

Core seeding installs the structural baseline:

- `Blank` room template
- stock area-shape templates such as `Rectangle`, `Terrain Rectangle`, and `Feature Rectangle`

These are about geometry and workflow, not rich stock prose.

### `UsefulSeeder`

Useful seeding now owns the wilderness descriptive bootstrap package:

- room template: `Seeded Terrain Wilderness Grouped Description`
- area template: `Seeded Terrain Wilderness Grouped Features`
- the supporting wilderness terrain feature tag hierarchy

These templates are built from the seeded terrain catalogue and are meant to be usable immediately with either the core area templates or the stock wilderness area template, plus Terrain Planner.

That split matches the intended product model:

- Core package: shape first, manual description later
- Useful package: ready-to-use terrain-matched wilderness prose and feature generation

## Terrain Planner Role

`Terrain Planner Core` is a WPF tool that produces the terrain mask expected by terrain-aware area templates.

- It targets `net10.0-windows7.0`.
- It paints a grid using terrain colour/text metadata.
- It exports a row-major comma-separated mask.
- `0` means no cell at that position.

Terrain data can be imported from:

- clipboard JSON
- a simple HTTP endpoint, typically the `Terrain API` project's `/Terrain` route

The planner only needs these terrain fields:

- `Id`
- `Name`
- `TerrainEditorColour`
- `TerrainEditorText`

The mask format must match the order expected by `AutobuilderAreaTerrainRectangle`: top-left to right, then row by row downward.

## Extension Guidance

When adding a new autobuilder type:

1. Decide whether it is structural or descriptive. Start from area vs room responsibility, not from convenience.
2. Reuse `AutobuilderAreaBase` or `AutobuilderRoomBase`.
3. Keep XML tolerant of missing nodes.
4. Register a stable loader keyword and template type.
5. Add or update seeder support only if the type should be part of stock content.
6. Add repeatability coverage if seeding is involved.
7. Update the autobuilder design and builder docs in the same change.

## Current Constraints

- XML definitions are versionless and must stay backwards-compatible.
- Terrain Planner creates terrain masks only; feature masks are still textual/manual unless a custom tool is added.
- Tag-driven description variation only matters when the chosen area template actually supplies tags.
- The stock wilderness package assumes the seeded terrain catalogue and its `Terrain` tag root already exist before the Useful autobuilder package is installed.
