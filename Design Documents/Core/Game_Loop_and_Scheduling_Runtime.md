# Game Loop and Scheduling Runtime

## Runtime cadence

`Futuremud.StartGameLoop()` is the authoritative 250 ms game-loop cadence. Each iteration admits accepted TCP connections and applies transport events, then processes player commands, idler warnings, clocks, normal schedules, effect schedules, logging, closed connections, optional Discord work, and the periodic save flush. Remaining time is available to bounded pathfinding and lazy-load work before the thread sleeps.

The TCP server is event-driven. A cancellable accept task publishes new sockets to a pending queue; the game-loop network phase constructs their menus and control contexts before starting one asynchronous read pump and one single-writer output pump per connection. There is no dedicated polling thread or idle 100 ms wake-up. Socket tasks parse or transmit bytes only: connection admission, account and MXP mutations, commands, link-dead effects, and connection disposal remain game-loop work.

Complete commands enter a bounded sixteen-command channel. A full channel pauses the socket reader and relies on TCP backpressure, so commands are not dropped, reordered, or coalesced. The game loop retains its reusable randomized work list and executes at most one command per ready connection per tick.

All text, prompts, and Telnet negotiation frames share one ordered output writer. Staged and queued output is bounded to 2 MiB and 256 frames per connection. A non-reading client that exceeds either limit is disconnected rather than being allowed to grow the managed heap indefinitely. Ordinary logout, timeout, and administrative closure drains queued output for up to two seconds; socket failure and limit violations abort immediately. Engine shutdown stops acceptance, allows connection drains for up to five seconds, and then aborts any remainder.

## Schedules and heartbeats

The normal scheduler and effect scheduler use stable min-heaps ordered by trigger UTC and insertion order. Due schedules are fired until none remain due. This deliberately preserves repeating schedule catch-up: a delayed server executes every missed repetition rather than coalescing or dropping it.

`HeartbeatManager` is a one-second repeating schedule. Hard heartbeats fire at their normal cadence; fuzzy heartbeats retain their five-generation distribution. Heartbeat subscribers execute synchronously on the game-loop thread, so expensive callbacks should subscribe only while they have active work.

## Demand-gated work

Static `GameItemHealthStrategy` items evaluate destruction when wounded and once during item login. They do not retain a recurring ten-second heartbeat because their status cannot change with time. Body-backed and item-component override health behaviour continues to own its existing periodic processing.

Track decay remains per-cell and fuzzy-minute. A cell with no expired tracks performs no database delete, avoiding an empty EF command for every tracked cell.

## Diagnostics

Junior administrators can use `debug performance`, `debug performance on`, `debug performance off`, and `debug performance reset`. Monitoring is disabled by default and is in-memory only. When enabled it records loop and scheduler timing, allocations, memory/GC state, heartbeat callback timing, subscriber counts, network bytes and operations, queue high-water marks, connection counts, slow-client disconnects, and transport errors. Network aggregates are atomic and never retain connection instances. Diagnostics do not create persistence records or alter runtime scheduling behaviour.

## Boundaries

This runtime model intentionally keeps the 250 ms game loop, save/log cadence, heartbeat callback semantics, Telnet protocol, and one-command-per-ready-connection behavior. `IServer` and `IPlayerConnection` remain source-compatible; event-driven implementations advertise optional async lifecycle interfaces, while synchronous implementations retain their existing entry points.

The listener binds to the IP address and port on the first two lines of `Connection.config`. Its optional third line is a comma-separated allowlist of trusted PROXY protocol senders. Two-line legacy files trust loopback (`127.0.0.1` and `::1`) so the recommended same-host `MudClientProxy` deployment preserves the browser's real address. A blank third line disables the trust boundary. PROXY headers from any other peer are treated as ordinary Telnet input, so operators must list only the exact private proxy addresses they control.

The trusted proxy address is resolved before admission and flood accounting. The resulting client address is then used by `PlayerConnection`, the database-backed site-ban check, duplicate-registration checks, and the TCP flood window. Consequently the `Bans` table remains the single authoritative ban list for direct Telnet and WebSocket clients; the proxy does not maintain a second list that can drift.

TLS termination, Discord transport, callback coalescing, and schedule execution budgets are outside this runtime boundary. No networking state or diagnostic session is persisted.
