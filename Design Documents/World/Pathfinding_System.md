# Pathfinding System

## Runtime Model

FutureMUD has two pathfinding modes:

- Exact searches use the existing shortest-path helper behaviour and remain the default for all existing callers.
- Hierarchical searches are opt-in through `PathSearchOptions` and are intended for long-distance AI, tooling, and administrative queries where a live-valid route is more important than a globally shortest route.

`IExitManager` owns the `IPathfindingService`. The service keeps an immutable topology snapshot containing stable map facts only: cell ids, cluster ids, zone-aware coordinate buckets, and boundary connections between clusters. It does not cache actor suitability, door state, lock state, full paths, or live exit objects.

## Incremental Indexing

Topology invalidation is deliberately cheap. Exit overlay rebuilds, transient exit registration/removal, and cell deletion mark the index dirty and cancel any in-progress builder, but they do not rebuild the index synchronously. Cell creation is also detected from the live cell count; if a cell appears during an idle rebuild, the active builder finishes the snapshot it started with and immediately queues the next rebuild.

The main engine loop calls `PathfindingService.DoIdleWork(...)` during the existing idle window, before lazy-load flushing receives the remaining time budget. The default loop cap is small, currently 3ms per tick, and the builder also has a work-unit cap so tests and future tuning can force multi-slice rebuilds. Each builder captures the cell list at the start of its build so live cell additions between idle slices cannot mutate the enumerator it is using. A new snapshot is published only after all queued cells have been processed.

Door and lock changes do not invalidate the topology snapshot. Hierarchical queries validate every returned segment against live exits and the caller's suitability predicate, so stale topology can suggest a candidate route but cannot return an invalid path through a locked, closed, actor-blocked, or otherwise unsuitable exit.

## Query Behaviour

`PathSearchAlgorithm.Exact` always uses the existing exact search.

`PathSearchAlgorithm.Automatic` uses exact search below the configured threshold and hierarchical search at or above it.

`PathSearchAlgorithm.Hierarchical` uses the last complete topology snapshot. If no snapshot exists, or if the query cannot assemble a live-valid route within its bounded retry count, it returns no path. It does not force a full-world exact fallback.

Hierarchical routing works in two phases:

1. Search the abstract cluster graph for candidate boundary exits.
2. Resolve each segment between source, boundary cells, and target cells with the exact helper, using the caller's live suitability predicate.

If a live segment or boundary exit fails validation, that abstract edge is blocked for the current query and another high-level route is attempted until the retry limit is reached.

## Extension Points

Opt-in overloads are available on the known-destination helper family:

- `PathBetween(..., PathSearchOptions options)`
- multi-target `PathBetween(..., PathSearchOptions options)`
- `ExitsBetween(..., PathSearchOptions options)`
- `DistanceBetween(..., PathSearchOptions options)`

Vicinity scans and target-acquisition helpers remain exact-only in this version because they are usually local perception workflows and are sensitive to nearest-target semantics.

## Operational Notes

Pathfinding diagnostics expose the current snapshot version, dirty state, queued build state, snapshot counts, and last slice/build timings. These counters are intended for admin debugging and future performance tuning.

The v1 implementation intentionally avoids background threads. Live game objects are not generally thread-safe, so the index is built in main-loop idle slices and published as a complete snapshot when ready.
