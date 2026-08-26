---
title: FutureMUD Terrain Planner & Engine API 2.0.4
summary: Repairs Windows PowerShell 5.1 upgrades and makes junction rollback reliable.
date: 2026-08-27
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.4 corrects a Windows PowerShell 5.1 compatibility issue in the 2.0.3 installer. Upgrades no longer use the newer process argument API that is unavailable in the PowerShell version included with supported Windows Server installations.

The installer now retains the stable service executable path during upgrades—the service always runs from the `current` junction—so an in-place release only switches that junction, restarts the existing service, and checks readiness. Its rollback logic also converts Windows PowerShell's junction-target array to the single target path before rebuilding the prior link.

For one last time, installations from any earlier 2.0.x build should use the archive-install procedure in the deployment guide. Once 2.0.4 is healthy, the signed updater handles later releases normally.
