---
title: Paint a world, then paste it into FutureMUD
summary: Terrain Planner 2.0 turns large-area building into a visual, hosted workflow for terrain and feature tags.
date: 2026-08-26
tags: terrain planner, builders, worldbuilding, release
---
Building a large wilderness, district, or regional map should begin with the shape of the place—not with a long sequence of room-editing commands. FutureMUD Terrain Planner & Engine API 2.0.0 gives builders a dedicated visual workspace where they can sketch that shape, refine it, and carry the finished plan back into the game.

The planner presents the map as a fast, zoomable canvas. Builders can paint terrain with live colours and glyphs from their game, draw rectangles, fill connected areas, erase mistakes, inspect coordinates, and undo or redo a stroke. A searchable palette keeps a game with a substantial terrain catalogue manageable, while browser autosave and versioned project files make it practical to work on a map over several sessions.

## Tags become part of the plan

Terrain is only one layer of a useful regional plan. Version 2.0 also introduces tag painting for the feature rectangles used by the autobuilder.

A cell can carry several tags at once. The planner gives used tags compact, deterministic colours on the grid and collects their full hierarchical names in an editable legend, keeping the map readable without losing context. Builders can therefore mark forests, roads, districts, riverbanks, encounter regions, or their own game-specific concepts while they lay out the terrain beneath them.

When the plan is ready, separate copy buttons produce the bottom-left, row-major terrain and feature masks expected by FutureMUD's `cell new` workflow. The planner does not create rooms or alter the game database: it remains a deliberate planning tool, with the builder reviewing and pasting the generated masks in-game.

## A service for the whole building team

This release replaces the old local desktop and unfinished experimental planner projects with one hosted product. A game operator installs the planner beside the Web MUD Client, gives it its own HTTPS hostname, and upgrades the visual client and read-only Engine API together.

Builders sign in with their existing FutureMUD game account. Access is limited to registered, unsuspended accounts with Admin authority or higher; the planner neither creates accounts nor stores passwords in the browser. Terrain and tag catalogues are read live from the game database, while maps remain local to the builder's browser unless they explicitly export a project file.

Self-contained packages are available for Windows x64, Linux x64, and Linux ARM64. They include service installers, Caddy integration, health-checked upgrades, automatic rollback, checksum verification, and signed update manifests. Operators can find the installation walkthrough in the package, and builders can find the mask workflow in the updated room-building and autobuilder guides.

Terrain Planner 2.0 is the beginning of a proper hosted world-design surface for FutureMUD: quick enough for a 200-by-200-cell plan, careful about live game data, and focused on helping builders think spatially before they commit thousands of rooms to the world.
