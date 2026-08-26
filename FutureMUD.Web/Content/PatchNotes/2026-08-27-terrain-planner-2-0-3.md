---
title: FutureMUD Terrain Planner & Engine API 2.0.3
summary: Makes Windows in-place upgrades safe and diagnoses service-command failures clearly.
date: 2026-08-27
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.3 corrects two Windows upgrade issues found during first hosted deployment. The installer now replaces only its `current` release junction, without prompting to recursively remove the release it points to.

It also configures the existing Windows Service through argument-safe process invocation, preserving the quoted executable command line. If Windows rejects a service configuration, the installer now reports the `sc.exe` exit code and diagnostic output instead of returning a generic failure.

The deployment guide now explains the one-off bootstrap installation needed by early 2.0 releases. After 2.0.3 is installed, the signed updater can perform subsequent upgrades normally.
