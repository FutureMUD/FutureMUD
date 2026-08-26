---
title: FutureMUD Terrain Planner & Engine API 2.0.0
summary: Unifies the hosted terrain and tag planner with its authenticated read-only Engine API.
date: 2026-08-26
tags: terrainplanner, release, builders, deployment
---

Terrain Planner is now a hosted builder service bundled with its read-only Engine API. Game operators install and upgrade one self-contained package beside the Web MUD Client, while builders open a dedicated HTTPS hostname and sign in with an existing registered, unsuspended Admin-or-higher game account.

The new canvas workspace supports smooth large-grid painting, flood fill, rectangles, eyedropper, erasing, pan/zoom, coordinate inspection, undo/redo, searchable live terrain and tag palettes, and local autosave. Builders can paint multiple tags onto each room, customise the tag legend colours, and export both the terrain mask and feature/tag mask required by `cell new "Terrain Rectangle"` and `cell new "Feature Rectangle"`.

Projects can be imported or exported as versioned JSON. Catalogue refreshes report renamed or deleted tags without silently changing an open map's export names, and cached catalogue data keeps local editing available during a temporary database outage.

The combined service ships for Windows x64, Linux x64, and Linux ARM64 with signed update manifests, checksum verification, hardened services, health-checked activation, rollback, and isolated Caddy configuration. The separate Terrain API product is retired; its final historical download remains available. The authenticated `/Terrain` compatibility route remains for planner 2.x and will be removed in the next major planner release.
