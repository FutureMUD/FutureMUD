---
title: FutureMUD Terrain Planner & Engine API 2.0.6
summary: Handles duplicate tag short names without preventing the builder catalogue from loading.
date: 2026-08-27
tags: terrainplanner, release, builder, bugfix
---

Terrain Planner & Engine API 2.0.6 corrects an issue that could leave the builder catalogue loading indefinitely after sign-in when a game's tag catalogue included the same short tag name in more than one hierarchy branch.

The planner now continues to load and shows every tag using its full hierarchy. Tags whose short names are not globally unique are clearly marked as unavailable for feature-mask painting, importing, and automatic selection. This prevents a feature mask from silently choosing a different tag from the one the builder intended.

Installations already running 2.0.5 can use the signed in-place updater.
