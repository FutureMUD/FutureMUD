---
title: FutureMUD Terrain Planner & Engine API 2.0.5
summary: Preserves existing direct Caddy site definitions during Windows installation and upgrades.
date: 2026-08-27
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.5 corrects the Windows Caddy site detection used by the installer. When the Web MUD Client's main Caddyfile already contains a direct site block for the planner hostname, the installer now recognises it and preserves that configuration.

This prevents a duplicate Terrain Planner site fragment from being added, avoiding Caddy's ambiguous site definition validation error. Existing successful service activation and the active Caddy configuration remain untouched.

Installations on any earlier 2.0.x release should use the archive-install procedure once for 2.0.5. After that one-time update, use the signed in-place updater for future releases.
