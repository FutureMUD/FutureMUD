---
title: FutureMUD Terrain Planner & Engine API 2.0.7
summary: Restores reliable public publishing for the duplicate-tag catalogue correction and clarifies hosted-installation steps.
date: 2026-08-27
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.7 makes the duplicate-tag catalogue correction from the unpublished 2.0.6 build available through the normal signed update channel. Builders can now sign in and load catalogues containing the same short tag name in different hierarchy branches without the planner stalling.

The website publisher now transfers releases in Cloudflare-compatible chunks, preventing long upload requests from timing out on the hosted website. The hosted-installation guide also makes the Windows archive's nested package directory and the non-standard Caddy fallback explicit.

Installations already running Terrain Planner 2.0.5 or later can use the signed in-place updater.
