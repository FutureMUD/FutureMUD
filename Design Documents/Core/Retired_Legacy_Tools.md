# Retired legacy tools

Retirement recorded: 15 November 2026.

## Removed service projects

`MudSharp Watcher` and `FutureMUD Upgrade Service` are obsolete and are removed from the repository and `MudSharp.sln`. Neither is a maintained release product.

- The Watcher desktop application must no longer be recommended as an engine launcher, process supervisor or updater. Use the host's service/process management and the maintained product release process instead.
- The Upgrade Service prototype must no longer be used for publishing or downloading engine packages. The maintained website publishing implementation is under `FutureMUD.Web/Publishing`, with its own authentication boundary.

Their implementation remains available in Git history. The [engine maturity audit](Engine_Maturity_Audit_20261114.md) records the historical concerns and their disposition by retirement. This repository cleanup does not uninstall software from external hosts.

## Retired terrain-tool versions

The maintained terrain product is [Terrain Planner and Engine API](../Building/Terrain_Planner_and_Engine_API.md), under `TerrainPlanner/`. Its server, browser client, contracts, deployment tools and tests remain supported.

The former `Terrain API`, `Terrain Planner Core`, `Terrain Planner Blazor WASM` and `TerrainEditorBlazor` projects are retired. Their source had already been removed before this cleanup; remaining local generated `bin`/`obj` output is removed as part of it. These names must not be restored as current solution projects.

The archived `MigrationBackup/8ecac18a/Terrain Planner Tool` project, its NuGet migration log/package list, and the obsolete local `.user`/publish-profile settings are also removed with explicit owner approval. The source-controlled archive remains recoverable from Git history. Local historical packages under `Publish/Terrain Planner` are outside this source cleanup and are not an active project.

The historical backfill workflow intentionally names old terrain project paths at fixed historical source revisions. It does not build those projects from current `master`, and its mappings remain for historical release compatibility. Previously published downloads and the planner's deprecated authenticated `/Terrain` compatibility endpoint are unchanged.

DatabaseSeeder's current terrain definitions, stock-terrain catalogue and autobuilder terrain seeding are engine functionality and remain in place.
