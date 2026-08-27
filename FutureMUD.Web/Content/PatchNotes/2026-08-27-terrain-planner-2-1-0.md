---
title: FutureMUD Terrain Planner & Engine API 2.1.0
summary: Makes terrain painting dependable after resize, uses the shared FutureMUD identity, and exports unambiguous tag-ID masks.
date: 2026-08-27
tags: terrainplanner, release, builders, autobuilder
---

**Compatibility note:** Terrain Planner 2.1.0 exports numeric tag IDs in feature masks. Install Engine 2.8.0 before pasting a new tag mask into `Feature Rectangle`; the terrain mask itself is unchanged.

## Reliable Grid Editing

The planner now discards queued input from a grid that has just been resized. This prevents a delayed stroke from appearing in a distant coordinate after changing dimensions. Clearing a terrain or the full map also removes blank cells from the canvas cache, so the visual grid now agrees with the copied mask.

## Stable Tag Masks

Tag masks now contain positive framework tag IDs, with multiple IDs in one cell separated by `|`. The palette continues to show the full hierarchical tag name, but the exported value is stable if a tag is renamed and unambiguous when different tag branches share a short name.

Use the planner's **Copy tag-ID mask** control with Engine 2.8.0's `Feature Rectangle` area template. Existing hand-written name masks need to be converted to tag IDs; `tag list` shows the IDs available in the game.

## Familiar FutureMUD Branding

The hosted planner now uses the same FutureMUD logo asset as the public website on both its sign-in and builder screens. Existing 2.0.5-or-later installations can use the normal signed in-place updater.
