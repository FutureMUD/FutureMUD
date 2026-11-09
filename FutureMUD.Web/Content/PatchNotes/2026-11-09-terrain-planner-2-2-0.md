---
title: FutureMUD Terrain Planner & Engine API 2.2.0
summary: Adds stacked terrain plans, configurable vertical connections and compressed complete-stack exports for the Engine autobuilder.
date: 2026-11-09
tags: terrainplanner, release, builders, autobuilder
---

**Compatibility:** Complete-stack imports require Engine 2.17.0 or later. Terrain Planner remains one service containing its hosted UI and read-only Engine API, with self-contained packages for Windows x64, Linux x64 and Linux ARM64.

## Plan across multiple levels

Add, duplicate, delete and renumber levels, including negative elevations for underground areas. Each level has its own terrain and tags, with shared map dimensions. Undo/redo covers painting, level changes, resizing and vertical connections.

Matching occupied positions on adjacent levels connect up/down by default. Builders can suppress a connection, require climbing with a selected difficulty, or allow falling toward the lower room. A fall link without climbing requires flight to travel upward. Empty positions and gaps between levels are never bridged.

## Export complete stacks

Export the whole plan for the Engine's new `terrain stack` autobuilder. Use a compressed token for small plans or wrapped editor text for larger plans. The Engine validates the payload and its terrain/tag references before creating rooms.

Missing catalogue references and unresolved historical features remain preserved in project files and must be resolved before exporting to the game. Clipboard denial leaves selectable export text available.

## Existing projects

Existing single-level projects load as level zero and save in schema 2, preserving terrain, tags, colours and unresolved features. Keep a copy of older project files before saving if you need to reopen them with an older planner.

Traditional terrain and tag masks remain available for the selected level. Planner levels represent separate connected game rooms rather than elevations within one room.

Use [/downloads](/downloads) for packages and checksums. Follow the included deployment guide for installation and signed updates.
