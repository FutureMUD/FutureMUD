# Pathfinding System

## Runtime Model

FutureMUD has two pathfinding modes:

- Exact searches use the existing shortest-path helper behaviour and remain the default for all existing callers.
- Hierarchical searches are opt-in through `PathSearchOptions` and are intended for long-distance AI, tooling, and administrative queries where a live-valid route is more important than a globally shortest route.

Those modes select a search algorithm. They are orthogonal to the path representation:

- legacy exit paths contain only ordinary cell-exit traversals;
- spatial paths contain typed `LinearRouteStep` and `CellExitStep` records and can cross both ordinary cells and [RouteCells](./Route_Cell_System.md).

`IExitManager` owns the `IPathfindingService`. The service keeps an immutable topology snapshot containing stable map facts only: cell ids, cluster ids, zone-aware coordinate buckets, and boundary connections between clusters. It does not cache actor suitability, door state, lock state, full paths, or live exit objects.

The spatial service maintains a second compiled view for each RouteCell topology version. It contains ordered nodes for endpoints, landmark coordinates, stop coordinates, and exit-band boundaries. It does not contain entity positions or active-mover state.

## Incremental Indexing

Topology invalidation is deliberately cheap. Exit overlay rebuilds, transient exit registration/removal, and cell deletion mark the index dirty and cancel any in-progress builder, but they do not rebuild the index synchronously. Cell creation is also detected from the live cell count; if a cell appears during an idle rebuild, the active builder finishes the snapshot it started with and immediately queues the next rebuild.

The main engine loop calls `PathfindingService.DoIdleWork(...)` during the existing idle window, before lazy-load flushing receives the remaining time budget. The default loop cap is small, currently 3ms per tick, and the builder also has a work-unit cap so tests and future tuning can force multi-slice rebuilds. Each builder captures the cell list at the start of its build so live cell additions between idle slices cannot mutate the enumerator it is using. A new snapshot is published only after all queued cells have been processed.

Door and lock changes do not invalidate the topology snapshot. Hierarchical queries validate every returned segment against live exits and the caller's suitability predicate, so stale topology can suggest a candidate route but cannot return an invalid path through a locked, closed, actor-blocked, or otherwise unsuitable exit.

Changing a RouteCell length, landmark, or exit anchor increments that cell's topology version. Its compiled longitudinal graph is discarded lazily. Dependent vehicle-route drafts become invalid, and approved services fail closed until revalidated; active journeys keep their pinned steps and do not silently reroute.

## Spatial Graph And Weights

A spatial query accepts exact `SpatialLocation` source and target values. Source and target coordinates are injected as temporary graph nodes and connected to their immediate ordered neighbours. Longitudinal edges have a floating-point weight of:

```text
absolute metres between coordinates / RouteCell metres per room-equivalent
```

Each ordinary exit traversal costs one room-equivalent before existing exit multipliers or caller-specific costs. An anchored exit connects its interval boundary nodes to the opposite cell. Route-to-route exits require valid anchors on both sides; normal-to-route traversal uses the route side's authored arrival coordinate.

Exit expansion is layer-aware even though topology discovery uses a null perceiver: normal searches require the exit to appear on the source node's room layer, and the destination node takes its layer from `ICellExit.MovementTransition(...).TargetLayer`. An explicit `ignoreLayers` search may select one of the exit's authored appearance layers to resolve that transition, but it does not invent a target layer or copy the source layer blindly.

The returned `ISpatialPath` reports:

- exact total longitudinal metres;
- floating-point room-equivalent cost;
- every crossed exit;
- ordered typed steps with exact endpoints;
- referenced RouteCell topology versions when compiled for durable use.

Distance and cost are not rounded during search. An adapter rounds only when an old API contract requires an integer room count.

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

`IPathfindingService` also exposes spatial-path overloads for exact-coordinate source and target values. Route-aware consumers use this form whenever either endpoint or an intermediate cell is a RouteCell. The shared AI base exposes a typed-path hook, `PathToLocationAI` supplies the representative route-aware fallback, and common legal-patrol deployment, preparation, route, and return movement use the same bounded helper.

Legacy `PathBetween`/`ExitsBetween` adapters remain supported for paths made wholly from ordinary cells. They reject a result that would require longitudinal travel and never flatten entry into a long RouteCell into a single room-sized hop.

## Typed Path Execution

`FollowingPath` retains its existing exit queue for ordinary paths and additionally accepts an `ISpatialPath`. Its typed queue executes `CellExitStep` through the existing actor movement strategy and `LinearRouteStep` through `LinearRouteMovement`. A longitudinal step remains queued until the actor reaches its exact destination coordinate; the normal AI/patrol tick then advances to the next step.

Typed followers pin every referenced RouteCell topology version. Before each action they revalidate the pins and the actor's exact expected origin. A `CellExitStep` additionally revalidates that the live exit side still exists, appears on the expected layer, passes the actor-specific suitability predicate, produces the compiled transition layer, and still has the required source/destination anchors and arrival coordinate. A `LinearRouteStep` revalidates its route identity, bounds, direction, and distance before movement starts. Any mismatch removes the following effect without attempting a substitute movement or silently rerouting.

AI and patrol integration preserves the legacy result whenever an ordinary exit-only path exists. A bounded spatial fallback is attempted when that search fails, including ordinary endpoints whose only valid connection crosses an intermediate RouteCell. Its metric room-equivalent cost uses the same caller budget, so a long route cannot reappear as a one-room shortcut.

## Operational Notes

Pathfinding diagnostics expose the current snapshot version, dirty state, queued build state, snapshot counts, and last slice/build timings. These counters are intended for admin debugging and future performance tuning.

The v1 implementation intentionally avoids background threads. Live game objects are not generally thread-safe, so the index is built in main-loop idle slices and published as a complete snapshot when ready.

Spatial topology follows the same rule. Compilation and invalidation occur on the main loop, while arbitrary entity positions and active motion interpolation are resolved at query time through `IRouteSpatialService`.
