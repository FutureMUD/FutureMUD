# RouteRoom Spatial System

## Purpose

On-foot and vehicle travel toward an exit anchor uses `IRoomExit.IsExitKeyword`: cardinal aliases select the exact direction, so `n` cannot select north-west and `nw` is recognised. Named exits retain their authored keyword matching. Anchor lookup retains its route-specific visibility and distance rules, allowing travel toward a visible exit before it is within interaction reach. This does not change route geometry or persistence.

A `RouteRoom` represents a long, essentially linear part of the world without requiring a builder to create thousands of ordinary rooms. A route room is still an `IRoom` for ownership, terrain, weather, overlays, and compatibility, but physical entities in it also have an exact longitudinal coordinate:

```text
(room, layer, metres from the negative endpoint)
```

Examples include roads between towns, rail corridors, rivers, tunnels, and the traversable surface of a very large mobile platform. Vehicle operating itineraries are deliberately separate and are called `VehicleRoute`; a RouteRoom is spatial topology, while a VehicleRoute is a revisioned sequence of travel and portal steps.

## V1.0 Boundary

V1.0 includes:

- exact longitudinal coordinates from `0` through the authored room length;
- characters, top-level items, tracks, vehicles, parties, mounts, drags, and hitch cohorts;
- named landmarks and coordinate-banded exits;
- metric proximity, perception, reach, interaction, combat, and local output;
- hybrid pathfinding across ordinary rooms and RouteRooms;
- continuous longitudinal movement with durable checkpoints;
- persistence and restart recovery;
- builder commands, validation, diagnostics, FutureProg access, and vehicle integration.

V1.0 deliberately does not simulate lateral or vertical coordinates, collision, overtaking, signalling, dispatch, physical consist length, or lane occupancy. All members of a party, mount/rider, drag, or hitch cohort use one reference coordinate.

## Room Model

RouteRoom is an optional persisted capability of the concrete `Room` implementation. It is not a subclass, a room overlay, or a self-loop exit. An ordinary room has `RoomSpatialType.Ordinary` and no route definition. A RouteRoom has `RoomSpatialType.LinearRoute` and one `IRouteRoomDefinition`.

The definition stores:

- length in metres;
- default arrival coordinate;
- positive and negative direction labels;
- metres per room-equivalent;
- topology version;
- ordered landmarks;
- one anchor for each exit side hosted by the room.

Route terrain is uniform in v1. Builders use landmarks for local description and split a route into multiple RouteRooms when terrain or environmental behavior changes materially.

### Coordinates

Coordinates are inclusive between `0` and `LengthMetres`. Runtime calculations use normal engine length values and `double`; durable values use `decimal(18,3)`. NaN, infinity, negative values, and values beyond the room length are invalid. Builder input is parsed by the game's unit manager and player-facing values are localised.

Every RouteRoom has a default coordinate used when a source cannot supply a more specific coordinate. Placement otherwise follows these rules:

1. An explicit coordinate or landmark wins.
2. A newly placed world entity inherits the actor or source coordinate.
3. A carried, worn, contained, installed, hitched, or vehicle-interior entity inherits its owner's effective coordinate.
4. A normal-to-route exit uses the destination side's deterministic arrival coordinate.
5. Legacy null-coordinate rows in ordinary rooms remain unchanged. A null coordinate loaded inside a RouteRoom is reported and repaired only through an explicit recovery path.

Top-level characters, items, vehicles, and tracks persist their coordinate. Nested objects do not retain a divergent world coordinate.

Character and NPC construction is coordinate-first. Source-aware surfaces such as `instance spawn ... here`, `npc load`, source-relative FutureProgs, projections, magical copies and clones, possessed bodies, animated corpses, and craft products pass the source's effective `SpatialLocation` into the factory before the actor is added to the game world. A deliberately room-only authored spawner instead resolves the RouteRoom's default coordinate. The complete location is validated before any `Characters` or `CharacterInstances` row is committed, and persistent instance reload validates the stored coordinate before materialisation. No invalid null or out-of-bounds RouteRoom row may be left behind after a failed spawn.

### SpatialLocation

`SpatialLocation` is the public location value:

```csharp
public readonly record struct SpatialLocation(
	IRoom Room,
	RoomLayer Layer,
	double? RoutePositionMetres);
```

`ILocateable.Location`, `RoomLayer`, and `InRoomLocation` remain compatibility projections. `SharesRoomLayerWith` means raw room/layer equality. `ColocatedWith` retains ordinary-room behavior but, inside a RouteRoom, means Immediate-or-nearer after explicit relationship overrides are applied.

## Distance and Proximity

`IRouteSpatialService` is the sole authority for effective coordinates, exact separation, proximity, reach, nearby queries, and portal accessibility. Consumers must not reproduce threshold arithmetic.

The default metric mapping is:

| Separation | Proximity |
| --- | --- |
| explicit touch, containment, grapple, inventory, or authored relationship | Intimate |
| `0m` through `3m` | Immediate |
| greater than `3m` through `10m` | Proximate |
| greater than `10m` through `100m` | Distant |
| greater than `100m` through `500m` | Very Distant |
| greater than `500m` | Unapproximable |

Same-coordinate entities are Immediate unless an explicit relationship makes them Intimate. Two entities on different layers of one RouteRoom are never closer than Very Distant without an explicit relationship.

Each RouteRoom has a metres-per-room-equivalent setting, inheriting the global `RouteCellDefaultRoomEquivalentMetres` value of `100m`. Existing room-count ranges use exact metres divided by that value. APIs that still require an integer round only at the final adapter.

## Locality Index

Each RouteRoom maintains an ordered index of stationary top-level entities per layer. Range queries use the ordered coordinate key and do not enumerate all occupants. Active lazy movers are held in a separate bounded set and merged into results using their effective coordinates. Inserting, removing, changing layer, materialising movement, or changing coordinate updates the index atomically.

Whole-room enumeration remains available for topology, administration, environmental systems explicitly scoped to the entire RouteRoom, and persistence. Normal speech, emotes, movement echoes, light, heat, scent, witness events, targeting, stacking, and item manipulation use a source-position-aware local query.

Direct physical operations use the configured Immediate band. Inventory-plan scouting and execution, connection and installation reach checks, restraint application and removal, dragging and drag assistance, medical application, injection and feeding, prosthetic attachment and removal, manipulation of another character's containers or external implants, combat item retrieval, party movement membership, pursuit, and local-project participation therefore revalidate the participants' effective coordinates before starting and when a delayed action commits. Visibility at Distant or Very Distant range does not imply manipulation reach. Ordinary rooms retain their historic same-room and same-layer behavior.

Player commands that operate on an item in the current location use the same Immediate rule. This includes clan paymaster objects, market stalls and listable shop fixtures, discussion boards, visible timepieces, item valuation and ownership inspection. Character paymasters must be colocated. Local out-of-character speech uses the configured Very Distant band, matching the RouteRoom speech horizon instead of broadcasting across the entire linear room. Staff purge commands and storyteller whole-room maintenance remain explicitly administrative scopes.

Active local projects persist their authored layer and nullable `decimal(18,3)` RouteRoom coordinate. A project created in a RouteRoom inherits the creator's effective coordinate; a RouteRoom project with a null, non-finite, or out-of-bounds coordinate fails load with an actionable diagnostic. Project lists, labour queues, worker ticks, completion output, duplicate-resource checks, and produced items use the project's Immediate neighbourhood. A worker who leaves that neighbourhood stops contributing, while two unrelated projects or resources kilometres apart can coexist in one RouteRoom. Ordinary-room projects remain room-wide for compatibility.

### Surface liquids

Room surface-liquid state is spatial inside a RouteRoom. A point spill, blood loss, magic-created liquid, or migrated legacy puddle is stored against its layer and source coordinate, rounded to the persisted millimetre precision. Its description and interaction surface are visible only from Immediate range; a compatible liquid state kilometres away in the same RouteRoom neither merges with it nor appears locally.

A null coordinate has a different, explicit meaning for this subsystem: it is a uniform environmental surface state for the entire layer, used for effects such as route-wide precipitation. It is not a valid top-level entity coordinate. Existing ordinary-room and pre-RouteRoom surface-liquid XML without a coordinate continues to load as the legacy per-layer state. Coordinate-bearing state in an ordinary room, or a non-finite or out-of-bounds RouteRoom coordinate, fails load with an actionable diagnostic rather than being clamped.

Ambient scents emitted by positioned items persist the source coordinate and their metric reach. Scent visibility and inhaled drug pulses query only that longitudinal band; an occupant kilometres away in the same RouteRoom neither smells the source nor receives its dose. Legacy scent effects without coordinate metadata retain their existing whole-room meaning so saved environmental scents remain loadable. Ordinary-room scent propagation is unchanged.

### Audio and magic locality

Audio emitted inside a RouteRoom starts at the source's effective coordinate. Its `AudioVolume` is interpreted as a maximum room-equivalent path cost: longitudinal distance consumes the source RouteRoom's metres-per-room-equivalent scale, each traversed exit contributes its normal exit cost, and volume attenuates once for each additional room-equivalent beyond the first. Same-room listeners are selected through the ordered locality index. Propagation may cross a RouteRoom boundary only through an authored anchor whose coordinate band is reachable within the remaining cost; it never uses legacy room-vicinity scans, unanchored portals, or a whole-RouteRoom broadcast. The same weighted propagation is selected for an ordinary-room source whenever its bounded audio graph reaches RouteRoom topology, allowing sound to cross a valid anchor in either direction without turning the RouteRoom into a shortcut. Audio whose bounded graph is entirely ordinary retains the legacy behavior.

Structured receiver-side noise uses `emitnoise(source, volume, propagation, mode, type, echo)`. Its finite `propagation` budget is independent of `AudioVolume`; `topological` prices ordinary-room exits at one each, while `coordinate` uses their coordinate-aware cost without creating adjacency. RouteRoom traversal always remains anchored and longitudinally priced. Each character reached by the cheapest valid route receives one `CharacterNoiseReceived` event, with the first direction from that listener back toward the source. A receiver may call `canhearnoise` to apply the listener's hearing profile and listen check; event reception itself does not mean that the character noticed the sound.

Local text or output in a RouteRoom likewise requires a source with a valid effective coordinate. A source-less or invalidly positioned local output fails closed: it reaches no occupants, remote observers, or legacy room-echo subscribers. Item-originated local output carries the item's effective or inherited coordinate. Callers that deliberately require a whole-RouteRoom environmental or administrative broadcast must request the explicit `Room` output range.

Magic powers authored for `SameLocationOnly` treat Immediate-or-nearer RouteRoom targets as local. Powers authored for `AdjacentLocationsOnly` use a hybrid spatial path costing at most one room-equivalent, so they can reach the appropriate longitudinal neighbourhood or a target immediately across an accessible anchored portal without treating the rest of either RouteRoom as adjacent. Candidate acquisition uses the locality index before validating candidates against the weighted pathfinder. Ordinary-to-ordinary magic range behavior remains unchanged.

### Combat spatial effects

Ranged scatter inside a RouteRoom always carries an exact impact coordinate. A directly struck replacement target supplies that coordinate; a same-room miss without a replacement target deterministically retains the original target's effective coordinate. Scatter candidate selection uses the locality index within the Immediate threshold, breath-weapon aftermath uses the Proximate threshold, and explosive aftermath maps its authored maximum proximity to the corresponding metric threshold. Sharing a RouteRoom is never sufficient: an occupant kilometres from the impact is not a victim. Ordinary-room scatter and aftermath retain their existing layer and whole-room behavior.

Successful pushback inside a RouteRoom physically moves the target away from the attacker by `RouteCellPushbackMetresPerSuccessDegree` multiplied by at least one success degree, then clamps the result to the inclusive route endpoints. If attacker and target have exactly the same coordinate, the target moves toward the farther endpoint; the positive endpoint wins an exact midpoint tie. Existing clinch/grapple cleanup, melee separation, combat delay, and vehicle-stability handling still apply. Ordinary-room pushback remains non-coordinate movement.

## Landmarks

A landmark has a stable id, name, keywords, exact coordinate, description, and display order. Landmarks are navigation and presentation aids; they do not divide the room or create ordinary-room boundaries.

Landmarks can be used by:

- `travel to` and `drive to`;
- `goto ... at` and `transfer ... at`;
- stop authoring and route compilation;
- relative look/scan descriptions;
- FutureProg nearest-landmark queries.

Moving, adding, deleting, or renaming a landmark increments the RouteRoom topology version. An approved vehicle route pins ids and compiled coordinates and therefore fails closed when its topology versions no longer match. Compiled exit steps pin both endpoint topology versions independently, including ordinary-to-route, route-to-ordinary and route-to-route anchor crossings; exit-only legs cannot evade invalidation.

## Anchored Exits

Every exit side in a RouteRoom has a `RouteExitAnchor` keyed by `(ExitId, RoomId)` with:

- inclusive minimum coordinate;
- inclusive maximum coordinate;
- deterministic arrival coordinate.

An actor may traverse the exit only while their effective coordinate lies inside the authored band. Passing through the band during longitudinal travel never automatically traverses the exit. `travel to <exit>` moves to the nearest point in the band and stops. A pinned `RoomExitStep` in a vehicle journey may subsequently traverse it.

Normal-to-route traversal lands at the route side's arrival coordinate. Route-to-route exits require a valid anchor on each route side. Ordinary-to-ordinary exits remain unchanged.

Topology code and `ExitsFor(null)` can inspect every exit. Actor-facing resolution returns only currently accessible exits. `look` and `scan` may describe a visible portal ahead or behind even when it is not yet traversable.

Changing an anchor increments topology version. Removing or shortening a RouteRoom is blocked when doing so would strand an anchor or its arrival coordinate.

## Hybrid Pathfinding

Spatial paths contain typed steps:

- `LinearRouteStep`: exact movement within one RouteRoom;
- `RoomExitStep`: ordinary exit traversal, including a portal between RouteRooms or between normal and route rooms.

The graph contains route endpoints, landmarks, stops, and exit-interval boundaries. Adjacent longitudinal nodes have a weight of exact metres divided by the room's room-equivalent. Ordinary exit traversal costs one room-equivalent adjusted by the existing exit cost. Arbitrary source and target coordinates are injected as temporary nodes.

A compiled path reports exact route metres, floating-point room-equivalent cost, crossed exits, referenced topology versions, and ordered typed steps. Compiled topology is cached by version.

Hybrid expansion enumerates exit topology independently of an actor, then applies the exit's authored source-layer visibility unless the caller explicitly requests a layer-agnostic search. The destination node always uses `IRoomExit.MovementTransition(...).TargetLayer`; compiled vehicle steps therefore preserve real layer changes at ordinary/RouteRoom portals instead of copying the source layer. Linear movement within a RouteRoom retains that resolved layer.

Legacy exit-only path APIs remain valid for ordinary-room paths. They must reject paths that require longitudinal RouteRoom travel and must never treat entry into a ten-kilometre RouteRoom as a one-room shortcut.

The shared `FollowingPath` effect can execute an `ISpatialPath`: `RoomExitStep` delegates to the existing actor movement/door strategy and `LinearRouteStep` starts `LinearRouteMovement`. Route topology versions are pinned when following begins. Every step revalidates the actor's exact origin, topology version, live exit/layer/transition, actor suitability, and route anchors before it executes; a mismatch removes the path effect and stops safely. Common legal-patrol transitions and the route-aware `PathToLocationAI` fallback use this seam. Execution and corpse-recovery patrols target the condemned character or corpse's exact effective coordinate; configured equipment and execution rooms resolve to their authored RouteRoom default coordinate. Ordinary exit paths still use the legacy queue, while ordinary endpoints whose legacy search fails may take a bounded typed path through an intermediate RouteRoom without flattening its longitudinal distance.

## Longitudinal Movement

`LinearRouteMovement` runs alongside ordinary exit movement. It records:

- root mover and atomic participant cohort;
- start coordinate and target coordinate or interval;
- direction and speed;
- a monotonic elapsed-time source for live interpolation (UTC wall-clock values are diagnostic persistence metadata only);
- durable operation id and checkpoint sequence;
- resource state required for an idempotent commit.

The current position is interpolated lazily. Longitudinal movement does not call `Room.Leave` or `Room.Enter`. Position is materialised on stop, interaction, combat commitment, logout, graceful shutdown, speed change, target arrival, portal crossing, resource checkpoint, and at least every `RouteCellMovementCheckpointSeconds` (default `30`).

Character begin, durable-progress/checkpoint, arrival, and cancellation echoes are emitted with the mover's effective coordinate as their source and `Local` output scope. Consequently only observers within the configured RouteRoom local-distance ceiling on the same layer receive them; sharing a long RouteRoom is not sufficient. These lifecycle echoes never use the whole-room room scope. Whole-route movement announcements require an explicitly authored environmental or administrative broadcast.

Movement exposes the following append-only FutureProg `EventType` hooks. They fire on every character in the atomic movement cohort and on every moving vehicle exterior, but not separately on dragged cargo or hitch gear. All payload types use the legacy hook type range; `operationid` is the durable movement GUID rendered as text and `direction` is `Positive` or `Negative`.

| Event type | Ordered parameters |
| --- | --- |
| `RouteMovementBegin` | `mover`, `routecell`, `originmetres`, `destinationmetres`, `direction`, `speedmetrespersecond`, `operationid` |
| `RoutePositionChanged` | `mover`, `routecell`, `previousmetres`, `currentmetres`, `operationid` |
| `RouteMovementProgress` | `mover`, `routecell`, `previousmetres`, `currentmetres`, `destinationmetres`, `direction`, `speedmetrespersecond`, `operationid` |
| `RouteMovementComplete` | `mover`, `routecell`, `originmetres`, `destinationmetres`, `direction`, `operationid` |
| `RouteMovementCancelled` | `mover`, `routecell`, `originmetres`, `currentmetres`, `destinationmetres`, `direction`, `reason`, `operationid` |

`RoutePositionChanged` and `RouteMovementProgress` fire, in that order, only after an advancing coordinate has been durably checkpointed. Completion follows the final progress pair. Cancellation follows any final progress pair after the cohort has stopped, including readiness, resource, topology, and explicit stop failures. Longitudinal movement does not impersonate ordinary `CharacterEnterRoom` or `CharacterLeaveRoom` events. Tracks include origin coordinate, destination coordinate, and longitudinal direction.

Stamina, fuel, power, and tow stress are charged in proportion to distance at durable, idempotent checkpoints. Each checkpoint coordinate, newly-ledgered charge, and the resulting resource mutation is saved in one database transaction; only the affected bodies and resource components are saved. A successful checkpoint restores their pre-checkpoint changed/queued state, while a failed checkpoint retains any queue entry needed to persist the restored rollback value; neither path flushes or removes unrelated queued work. A crash therefore cannot leave a committed ledger entry whose resource mutation was lost. Repeating an operation id and sequence with the same payload cannot charge resources twice. A replay whose coordinate, remaining duration, or aggregate resource payload differs from the durable checkpoint is an idempotency conflict: it fails closed and is reported through the normal movement fault/system diagnostic path. If applying, saving, or committing a checkpoint throws, runtime coordinates and reversible resource state return to the previous durable checkpoint before the operation stops; the failed sequence is not reported as progress and cannot leave an unledgered deduction. If the database reports an ambiguous commit error, the persistence seam re-reads the operation and accepts it only when the full checkpoint payload is durably present. Completion cleanup is idempotent: a cleanup failure does not replay a committed charge, and any retained active-motion row remains usable by restart recovery. A readiness failure materialises and commits the current coordinate and reports an actionable reason without charging uncommitted resources.

Graceful shutdown commits the exact effective coordinate. Crash recovery restores the last durable checkpoint and applies no downtime travel. Generic actors stop there. Active scheduled vehicle journeys may resume there and add the offline duration to displayed delay.

## Player Commands

```text
travel forward|backward
travel forward|backward <distance>
travel to <distance|landmark|visible exit|target>
stop
look forward|backward
scan forward|backward
```

A direction without a distance targets the matching endpoint. `stop` materialises and cancels active longitudinal movement. Commands preserve their established ordinary-room meanings outside RouteRooms.

## Builder Commands and Safety

RouteRoom authoring extends `room set`:

```text
room set route create <length>
room set route clear
room set route length <length>
room set route default <distance|landmark>
room set route direction positive <name>
room set route direction negative <name>
room set route roomequivalent <length|default>
room set route landmark add <distance> <name>
room set route landmark rename|keywords|distance|description|delete ...
room set route exit <exit> band <minimum> <maximum> arrival <distance>
room set route show
room set route map
room set route validate
```

Conversion is available through the room builder once the current room is clear of other live or persisted character instances, top-level items, vehicles, projects, tracks, coordinate-bound surface liquid, and all existing exits. The builder performing the conversion remains in the room and is placed at `0m`; both the primary compatibility row and physical-instance row are updated with the geometry in one serializable transaction. Exits are linked and anchored only after conversion. Clear and shortening operations apply the same persisted-state audit and fail while an entity, track, point spill, landmark, anchor, stop, approved route, service, or active journey would be stranded. Geometry and the builder's resulting coordinate commit atomically; the commands never clamp or silently discard persisted state.

Every topology mutation increments `TopologyVersion`, invalidates dependent draft compilation, and prevents approved services from departing until revalidated. An active journey keeps its pinned steps and never silently reroutes.

## Persistence and Recovery

The spatial foundation persists:

- RouteRoom definitions and landmarks;
- route-side exit anchors;
- top-level entity coordinates;
- active motion checkpoints;
- idempotent resource-commit ledger entries.

Boot validates finite coordinates, bounds, valid anchor bands and arrivals, landmark positions, and topology dependencies. Invalid state is logged with entity, room, coordinate, and a recovery command. It is not wrapped, normalised, or discarded.

## Vehicle Integration

RoomScale interiors are stable ordinary rooms hosted by a vehicle. Occupants stay in those rooms while the exterior projection moves. Their effective external spatial coordinate is inherited from the vehicle exterior for environment, perception through access points, boarding, and route movement.

Route vehicle travel uses the same `LinearRouteMovement`, locality, path, checkpoint, readiness, hitch-graph, and resource-ledger services as characters. Powered vehicles use their route speed. Externally pulled vehicles use the motive root's speed and stamina. All cohort members share a reference coordinate in v1.

For a compiled portal step, an externally pulled vehicle's incoming character or mount hitch is the permitted motive edge into the recursive vehicle train. Readiness validates that edge and the complete train before movement. The motive character, riders/party/drag participants, top-level physical hitch gear, and every downstream vehicle cross together, while RoomScale interior occupants stay in their persistent hosted rooms. A RouteRoom destination assigns the pinned arrival coordinate to all top-level members. A failure returns the underlying readiness or graph reason and leaves the cohort on the source side; it is never reduced to a generic “could not traverse route step” result.

`VehicleRoute`, `VehicleService`, and `VehicleJourney` are operational concepts described in the vehicle design. They pin typed spatial steps and RouteRoom topology versions; they do not change the RouteRoom spatial model.

Vehicle players use `drive forward|backward [<distance>]`, `drive to <distance|landmark|visible exit|route stop>`, `drive route <approved vehicle route>`, and `drive stop`. Builders author operational routes and recurring services with `vehicleroute` and `vehicleservice`; see [Vehicle System](../Vehicle_System.md) for the exact workflow. RouteRoom exit bands never imply automatic traversal during free driving. Only an explicit normal exit command or a compiled `RoomExitStep` crosses the portal.

After `drive stop` materialises a mid-leg coordinate, `drive route` resumes the approved pinned itinerary only when exactly one remaining compiled continuation contains that coordinate. It starts a new durable operation at the materialised coordinate. Off-leg, ambiguous, terminal, or topology-invalid coordinates fail closed; manual route execution does not silently rewind, pick a branch, recompile, or reroute.

## Static Configuration Defaults

| Setting | Default |
| --- | ---: |
| `RouteCellImmediateDistanceMetres` | `3` |
| `RouteCellProximateDistanceMetres` | `10` |
| `RouteCellDistantDistanceMetres` | `100` |
| `RouteCellVeryDistantDistanceMetres` | `500` |
| `RouteCellDefaultRoomEquivalentMetres` | `100` |
| `RouteCellMovementCheckpointSeconds` | `30` |
| `RouteCellPushbackMetresPerSuccessDegree` | `1` |
| `VehicleServiceRetrySeconds` | `30` |
| `VehicleServiceDefaultMaximumHoldMinutes` | `15` |
| `VehicleRouteDefaultDockingToleranceMetres` | `2` |

Thresholds must be strictly increasing and positive at boot. Invalid configuration is a boot diagnostic, not a license to produce inconsistent locality.

## Verification Contract

Required regression coverage includes:

- ordinary rooms and legacy null coordinates;
- a `10,000m` RouteRoom with a `7,100-7,200m` portal band;
- exact threshold boundaries and explicit Intimate relationships;
- cross-layer behavior and per-room room-equivalents;
- indexed queries with thousands of occupants;
- fake-clock interpolation, stopping, durable checkpoints, and crash recovery;
- parties, mounts, drags, and recursive hitch cohorts;
- local perception, output, stacking, manipulation, combat, crime, and effects;
- hybrid paths across normal rooms, anchored portals, and chained RouteRooms;
- RoomScale vehicles and scheduled journeys using the same spatial contracts.

The automated suites exercise these contracts at service and command level. Live telnet acceptance also covers the scheduled train, occupied mobile platform, and mount-drawn recursive wagon train, including two durable wagon checkpoints, reboot reload, pinned mid-route portal traversal in both directions, and the final longitudinal-plus-portal arrival into the far-town platform. The maintained blank-database snapshot includes all three RouteRoom/vehicle migrations. See the [V1.0 acceptance evidence](../Verification/RouteCell_Vehicle_V1_Acceptance_Evidence.md). Repeated reboot testing on an upgraded production-scale world and a long-duration soak remain release-candidate gates.
