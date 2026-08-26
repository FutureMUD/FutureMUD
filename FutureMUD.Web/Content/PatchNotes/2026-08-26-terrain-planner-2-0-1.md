---
title: FutureMUD Terrain Planner & Engine API 2.0.1
summary: Corrects hosted planner account sign-in and makes first installation beside the Web MUD Client reliable.
date: 2026-08-26
tags: terrainplanner, release, deployment, bugfix
---

Terrain Planner & Engine API 2.0.1 fixes a database-query translation error which could cause a server error when a builder signed in with a valid game account. Existing registered, unsuspended Admin-or-higher accounts now authenticate normally against the FutureMUD database.

The Windows installer now creates its durable configuration before it creates a release, so the documented first-run stop can be rerun safely. It runs the planner as low-privilege `LocalService` with a service SID limited to the planner's shared configuration and key directory.

When installed beside the Web MUD Client, the installer now discovers Caddy and its active Caddyfile from the Web MUD Client's scheduled task, validates and reloads it with the correct adapter, and accepts explicit Caddy paths for non-standard installations. The deployment guide now shows the archive's inner runtime directory, the Caddy override, and the TLS 1.2 verification step needed by Windows PowerShell 5.1.
