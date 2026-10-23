# Game Loop and Scheduling Runtime

## Runtime cadence

`Futuremud.StartGameLoop()` is the authoritative 250 ms game-loop cadence. Each iteration admits accepted TCP connections and applies transport events, then processes player commands, idler warnings, clocks, normal schedules, effect schedules, logging, closed connections, optional Discord work, and the periodic save flush. Remaining time is available to bounded pathfinding and lazy-load work before the thread sleeps.

The TCP server is event-driven. A cancellable accept task publishes new sockets to a pending queue; the game-loop network phase constructs their menus and control contexts before starting one asynchronous read pump and one single-writer output pump per connection. There is no dedicated polling thread or idle 100 ms wake-up. Socket tasks parse or transmit bytes only: connection admission, account and MXP mutations, commands, link-dead effects, and connection disposal remain game-loop work.

Complete commands enter a bounded sixteen-command channel. A full channel pauses the socket reader and relies on TCP backpressure, so commands are not dropped, reordered, or coalesced. The game loop retains its reusable randomized work list and executes at most one command per ready connection per tick.

An exception during player input aborts only that connection and prevents its remaining queued commands from executing. The normal game-loop disposal path detaches the controller. The failure retains the existing crash report, but diagnostic callbacks or log-write failures are contained too. Debug and Release builds use the same boundary; a player-command exception does not terminate the server process.

All text, prompts, and Telnet negotiation frames share one ordered output writer. Staged and queued output is bounded to 2 MiB and 256 frames per connection. A non-reading client that exceeds either limit is disconnected rather than being allowed to grow the managed heap indefinitely. Ordinary logout, timeout, and administrative closure drains queued output for up to two seconds; socket failure and limit violations abort immediately. Engine shutdown stops acceptance, allows connection drains for up to five seconds, and then aborts any remainder.

## Schedules and heartbeats

The normal scheduler and effect scheduler use stable min-heaps ordered by trigger UTC and insertion order. The normal scheduler fires at most 1,000 schedules per dispatch pass, retaining all remaining entries for the next game-loop iteration. This bounds self-rescheduling work while preserving repeating schedule catch-up: a delayed server executes every missed repetition incrementally rather than coalescing or dropping it. Callbacks remain synchronous; the dispatch count limit does not interrupt an individual callback.

`HeartbeatManager` is a one-second repeating schedule. Hard heartbeats fire at their normal cadence; fuzzy heartbeats retain their five-generation distribution. Heartbeat subscribers execute synchronously on the game-loop thread, so expensive callbacks should subscribe only while they have active work.

## Demand-gated work

Static `GameItemHealthStrategy` items evaluate destruction when wounded and once during item login. They do not retain a recurring ten-second heartbeat because their status cannot change with time. Body-backed and item-component override health behaviour continues to own its existing periodic processing.

Track decay remains per-cell and fuzzy-minute. A cell with no expired tracks performs no database delete, avoiding an empty EF command for every tracked cell.

## Diagnostics

### Exception reports

Release startup and game-loop failures, isolated player-input failures (both configurations), and unexpected email-worker exits use the shared `Server.ExceptionReporter`. Reports remain UTF-8 text files in the working directory with the `Fatal Exception` prefix. UTC timestamps and a unique suffix, combined with exclusive creation, prevent simultaneous reports from overwriting each other. The header identifies whether the process is terminating; the historical filename prefix also applies to isolated failures.

Every report includes a report ID, UTC time, engine assembly version, informational version (including source revision when embedded in the build), build configuration, actual .NET runtime, OS/process architecture, process ID, uptime, thread identity, current operation and a best-effort memory snapshot. Startup stages and game-loop phases are recorded before execution even when performance monitoring is disabled. Operation context flows through asynchronous continuations and is isolated between workers.

Ordinary reports retain `Exception.ToString()` and enrich nested exceptions, aggregate children and reflection loader exceptions. The reflection handler tolerates null type slots. Additional fields include HRESULT, MySQL codes/SQL state, tracked EF entity metadata/table/state/primary keys/modified property names, XML/JSON locations, socket/HTTP classifications and file/type load identifiers. EF reporting never queries the database or dumps non-key property values. Extra diagnostics are bounded to 64 distinct exception nodes, 32 EF entries per exception, 64 modified property names per entry and 4,096 characters per field; truncation is labelled. Original exception text is retained without that field limit.

Optional context callbacks are independently guarded. Player reports include input, account, character and room IDs, character description and editing target identifiers; a broken description cannot suppress the original exception. Effect loading and scheduled callbacks attach operation/owner context and rethrow the original failure (unknown effect types retain their original lookup error as the inner exception). Schedule removal and retry behaviour are unchanged. Other callers can annotate `Exception.Data` with `Operation`, `EntityId`, `EntityType`, `DefinitionId` or `RequestId`; only these keys are exported, using bounded scalar values without arbitrary object serialization. This is contextual annotation, not automatic instrumentation of every definition loader.

File failures fall back to stderr. The existing Discord crash notification receives the report through the existing connection when available; notification failure cannot invalidate the local file, and notification is still attempted if the file cannot be written. Background worker notifications are queued for delivery by the game loop, so workers never access the Discord connection concurrently. Pending notifications are also drained after the email worker stops and during Release final shutdown; fatal/player reports retain direct delivery. No external reporting service is introduced. Reports can contain player input, exception messages, paths and primary keys: treat them as sensitive local diagnostics and review before sharing. Email worker reports additionally omit exception messages/data to preserve the email privacy contract described in [Outbound Email Delivery](Outbound_Email_Delivery.md).

Reporting is best-effort: stderr can also be unavailable, and catastrophic process failures such as stack exhaustion or forced termination cannot be guaranteed a managed text report. No database access, world traversal, forced collection or diagnostic monitoring activation is performed by the shared formatter.

Senior administrators can use `debug performance`, `debug performance on`, `debug performance off`, and `debug performance reset`. Monitoring is disabled by default and is in-memory only. When enabled it records loop and scheduler timing, allocations, memory/GC state, heartbeat callback timing, subscriber counts, network bytes and operations, queue high-water marks, connection counts, slow-client disconnects, and transport errors. Network aggregates are atomic and never retain connection instances. Diagnostics do not create persistence records or alter runtime scheduling behaviour.

`debug census [loaded character id]` reports current Actors, CachedActors, NPCs, Bodies,
and distinct current body objects attached to actors/cache/NPCs. `Bodies` is the separate
detached-body registry; ordinary actor construction does not populate it. It also reports
main/effect schedule heap counts and every hard/fuzzy heartbeat cadence, including all
five stored generations. An optional exact loaded character ID adds the actual quit,
deleted, combat-join, death, movement-start and movement-stop delegate counts and loaded followers. It never resolves
an unloaded character or fires callbacks. These snapshots support before/after lifecycle
qualification; retained historical database rows are measured separately. Run this staff
diagnostic on the game-loop thread, as with the existing scheduler diagnostics.

`debug scheduler` also identifies each queued effect's concrete type and its actual
owner's framework type and 64-bit ID, alongside the existing remaining duration and
description. It reads the queued objects directly without loading entities or firing
callbacks. Use it to distinguish a lingering summon schedule from an ordinary timed
effect on a retained actor or body.

## Boundaries

This runtime model intentionally keeps the 250 ms game loop, save/log cadence, heartbeat callback semantics, Telnet protocol, and one-command-per-ready-connection behavior. `IServer` and `IPlayerConnection` remain source-compatible; event-driven implementations advertise optional async lifecycle interfaces, while synchronous implementations retain their existing entry points.

The listener binds to the IP address and port on the first two lines of `Connection.config`. Its optional third line is a comma-separated allowlist of trusted PROXY protocol senders. Two-line legacy files trust loopback (`127.0.0.1` and `::1`) so the recommended same-host `MudClientProxy` deployment preserves the browser's real address. A blank third line disables the trust boundary. PROXY headers from any other peer are treated as ordinary Telnet input, so operators must list only the exact private proxy addresses they control.

The trusted proxy address is resolved before admission and flood accounting. The resulting client address is then used by `PlayerConnection`, the database-backed site-ban check, duplicate-registration checks, and the TCP flood window. Consequently the `Bans` table remains the single authoritative ban list for direct Telnet and WebSocket clients; the proxy does not maintain a second list that can drift.

TLS termination, Discord transport, callback coalescing, and individual callback execution time limits are outside this runtime boundary. No networking state or diagnostic session is persisted.
