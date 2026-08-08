# Game Loop and Scheduling Runtime

## Runtime cadence

`Futuremud.StartGameLoop()` is the authoritative 250 ms game-loop cadence. Each iteration processes player commands, idler warnings, clocks, normal schedules, effect schedules, logging, closed connections, optional Discord work, and the periodic save flush. Remaining time is available to bounded pathfinding and lazy-load work before the thread sleeps.

The TCP server remains a separate synchronous polling loop with a 100 ms cadence. It reads and sends socket traffic only; command execution remains on the game-loop thread.

## Schedules and heartbeats

The normal scheduler and effect scheduler use stable min-heaps ordered by trigger UTC and insertion order. Due schedules are fired until none remain due. This deliberately preserves repeating schedule catch-up: a delayed server executes every missed repetition rather than coalescing or dropping it.

`HeartbeatManager` is a one-second repeating schedule. Hard heartbeats fire at their normal cadence; fuzzy heartbeats retain their five-generation distribution. Heartbeat subscribers execute synchronously on the game-loop thread, so expensive callbacks should subscribe only while they have active work.

## Demand-gated work

Static `GameItemHealthStrategy` items evaluate destruction when wounded and once during item login. They do not retain a recurring ten-second heartbeat because their status cannot change with time. Body-backed and item-component override health behaviour continues to own its existing periodic processing.

Track decay remains per-cell and fuzzy-minute. A cell with no expired tracks performs no database delete, avoiding an empty EF command for every tracked cell.

## Diagnostics

Junior administrators can use `debug performance`, `debug performance on`, `debug performance off`, and `debug performance reset`. Monitoring is disabled by default and is in-memory only. When enabled it records loop and scheduler timing, allocations, memory/GC state, heartbeat callback timing, and subscriber counts. It does not create persistence records or alter runtime scheduling behaviour.

## Boundaries

This runtime model intentionally keeps the 250 ms game loop, 100 ms TCP polling, save/log cadence, and heartbeat callback semantics. Event-driven TCP I/O, callback coalescing, and schedule execution budgets are separate future work because each changes overload or connection behaviour.
