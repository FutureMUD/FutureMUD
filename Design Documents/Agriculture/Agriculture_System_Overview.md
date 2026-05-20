# FutureMUD Agriculture System Overview

## Purpose
Agriculture is a first-class subsystem for fields, crops, pastures, herds, and managed woodlands. It deliberately sits beside crafting, projects, weather, property, terrain, and NPC systems rather than replacing any of them.

The core abstraction is `IAgricultureField`: zero or one field can exist in a cell, and multiple fields are represented by multiple cells. A field holds visible 0-100 scores, an active use, and optional crop, herd, or woodland state.

## Design Goals
- Give farmers meaningful choices without requiring soil-science simulation.
- Use projects for labour, tools, materials, and group work.
- Use weather and terrain as coarse inputs, not as a second climate engine.
- Let builders seed useful defaults, then tune the local agricultural fiction.
- Support arable fields, gardens, orchards, pasture, and managed woodland/coppice in the same field model.

## Main Concepts
| Concept | Runtime role |
| --- | --- |
| Field profile | Default score package and allowed uses for new fields. |
| Field | Persisted cell-bound state, one per cell at most. |
| Crop definition | Builder-editable crop parameters for growth, harvest window, moisture, and temperature tolerance. |
| Herd definition | Abstract livestock definition with animal-unit pressure and optional NPC template for drawdown. |
| Woodland definition | Managed tree stand or coppice definition with establishment and harvest cycle timings. |
| Operation | Project-backed field operation such as sowing, drainage, weeding, harvesting, grazing, coppicing, felling, or clearing land. |

## Supported Field Uses
`AgricultureFieldUse` currently supports:

- `Fallow`: improvement, recovery, and preparation.
- `Crop`: annual crops, gardens, and orchard-like seasonal production.
- `Pasture`: abstract herds and grazing pressure.
- `Woodland`: coppice, pollard, timber stands, managed groves, and land clearing.

A field has one primary use at a time. Temporary transitions, such as grazing fallow land or stubble, are modelled through operations that change the field's use.

## Player Surface
The player-facing command is `field`.

Key workflows:

- `field` or `field look` inspects the field in the current cell.
- `field start <operation> [target]` starts the project backing an agriculture operation.
- `field harvest` finds an applicable harvest operation for the current crop.
- `field herd draw <herd> [count]` materialises live NPC livestock from an abstract herd when the herd definition has a template.
- `field herd absorb <npc> <herd>` removes an eligible NPC from the live world and adds it to the abstract herd.

## Builder Surface
Administrators can use `field` to manage cell fields and agriculture definitions:

- `field create [profile]`, `field delete`, `field set <score> <0-100>`, `field reset`, and `field tick`.
- `field profile list|show|create|set|delete`.
- `field crop list|show|create|set|delete`.
- `field herds list|show|create|set|delete`.
- `field woodland list|show|create|set|delete`.
- `field operation list|show|create|set|delete`.
- `terrain set agriculture <profile|none>` sets the default agriculture profile for future field creation on that terrain.

Terrain defaults do not create fields automatically. They are a builder convenience for `field create` and FutureProg field creation.

## Integrations
- Projects supply the labour/material structure and long-running work cadence.
- Weather supplies coarse daily moisture, stress, and growth pressure.
- Terrain supplies a default field profile.
- Property gates field work when the cell belongs to a property.
- FutureProg can inspect, create, delete, mutate, and start work on fields.
- NPC templates are used only when abstract herd stock is deliberately drawn down into live animals.

## Seed Content
`AgricultureSeeder` installs stock profiles, crop definitions, herd definitions, woodland definitions, operation definitions, and project templates. The stock package is generic and builder-editable rather than setting-specific.
