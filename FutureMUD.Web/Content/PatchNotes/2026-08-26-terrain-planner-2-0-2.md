---
title: FutureMUD Terrain Planner & Engine API 2.0.2
summary: Corrects the Windows updater's signed archive download route.
date: 2026-08-26
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.2 corrects the Windows updater's archive URL. The updater now retrieves the signed manifest from the stable `latest` endpoint and downloads its named archive from the matching immutable version endpoint before verifying its signature and SHA-256 checksum.

This makes in-place upgrades from 2.0.0 and 2.0.1 work with the public FutureMUD download service while retaining the same health-checked activation and rollback safeguards.
