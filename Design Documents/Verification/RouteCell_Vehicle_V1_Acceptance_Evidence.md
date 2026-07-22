# RouteCell and RoomScale Vehicle V1.0 Acceptance Evidence

Evidence date: 22 July 2026
Scope: first-class linear `RouteCell` geometry, RoomScale vehicle interiors and docking, and revisioned `VehicleRoute` services and journeys.

## Outcome

The RouteCell and RoomScale Vehicle V1.0 slice passed its implementation gate. The three required worked examples were built and driven in a migrated disposable development world, the maintained blank-database snapshot includes the final migration, and the complete default repository unit suite passed.

This is a V1.0 subsystem acceptance record. It is not an upgraded production-world soak or a claim that post-V1 transport simulation features are implemented.

## Live Worked Examples

| Example | Result | Evidence |
| --- | --- | --- |
| Scheduled train | Passed. A multi-compartment RoomScale train boarded an occupant through a transient platform docking, travelled a pinned RouteCell service in both directions, arrived with the occupant still in the stable interior, rebuilt docking, and reloaded after reboot. | [Occupied train ride](./RouteCell_Vehicle_V1_Train_Occupied_Ride_Passed.txt), [post-ride reboot](./RouteCell_Vehicle_V1_Train_Reboot_After_Ride.txt) |
| Massive mobile platform | Passed. An occupied RoomScale platform was controlled from its hosted interior, moved through an ordinary cell exit, kept its interior cell ids and controller stable, rebuilt docking at arrival, and reloaded after reboot. | [Platform drive](./RouteCell_Vehicle_V1_Platform_Drive_Passed.txt), [arrival and reboot](./RouteCell_Vehicle_V1_Platform_Arrival_Reboot_Passed.txt) |
| Horse-drawn wagon train | Passed. A mount was the motive root for two recursively hitched wagons with physical hitch gear and a driver aboard the lead wagon. The cohort entered and travelled the authored 10km intertown route, remained active through more than two durable 30-second checkpoints, stopped and reloaded at one exact coordinate, traversed the quarry portal atomically in both directions, and completed the terminal longitudinal and portal steps into the far-town platform. | [Compiled West Town-to-East Town route](./RouteCell_Vehicle_V1_Wagon_Route_Transcript.txt), [two-checkpoint continuation](./RouteCell_Vehicle_V1_Wagon_Two_Checkpoint_Continuation_Passed.txt), [checkpoint reboot](./RouteCell_Vehicle_V1_Wagon_Reboot_Passed.txt), [RouteCell-to-ordinary portal](./RouteCell_Vehicle_V1_Wagon_Portal_Traversal_Passed.txt), [ordinary-to-RouteCell portal and 10m continuation](./RouteCell_Vehicle_V1_Wagon_Portal_Return_Passed.txt), [far-terminal arrival](./RouteCell_Vehicle_V1_Wagon_East_Town_Arrival_Passed.txt) |

The wagon checkpoint run committed both wagons at exactly `234.225m`, retained the driver/control assignment, kept both exterior projections synchronized, and returned no hitch-audit findings after reboot. The submitted `QA Wagon Road` compiled from West Town to East Town as three typed steps covering exactly `9,998m` and `101.98` room-equivalents. For the mid-route portal proof, the stopped cohort was moved while the MUD was offline from `234.225m` to `7,140m` by a one-use guarded fixture operation. After the portal round trip, the stopped cohort was similarly moved from `7,140m` to `9,990m` for the far-terminal proof. On both occasions the guard required exactly two matching vehicles, four matching top-level projection/hitch items, two characters, two character instances, and zero active motions. The routes and movements were then authored, compiled, approved, and executed through the live command/runtime paths; this samples the full long-duration route without misrepresenting approximately 39 minutes of real-time waiting as additional functional coverage.

The return route preview proves the hybrid typed path explicitly:

1. `CellExitStep`: ordinary quarry cell #5 to RouteCell #2 at the portal's deterministic `7,150m` arrival coordinate.
2. `LinearRouteStep`: RouteCell #2 from `7,150m` to the authored `7,140m` stop, exact distance `10m`, cost `0.100` room-equivalents.

The outbound route proves that merely occupying an exit band does not traverse it: traversal occurred only when the approved route executed its pinned `CellExitStep`.

The terminal transcript begins with both wagons durably synchronized at `9,990m`, executes the approved route's final `LinearRouteStep` to the `9,998m` terminal band and its pinned `CellExitStep`, and ends with the driver, mount, hitch gear, and both wagon projections in East Station Platform. Both projections report in sync and the post-arrival hitch audit reports no findings.

## Automated and Build Verification

The following completed sequentially on Windows from the final source state:

- `dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false`: passed.
- `FutureMUDLibrary`, `MudsharpDatabaseLibrary`, `MudSharpCore`, and `DatabaseSeeder` targeted builds: passed with 0 warnings and 0 errors.
- `FutureMUDLibrary Unit Tests`: 412 passed.
- `MudsharpDatabaseLibrary Unit Tests`: 31 passed.
- `MudSharpCore Unit Tests`: 1,978 passed.
- `scripts\test-unit.ps1`: 3,101 passed, 0 failed, 0 skipped:
  - 412 shared-library;
  - 21 expression-engine;
  - 545 DatabaseSeeder;
  - 1,978 core runtime;
  - 31 database-library;
  - 22 Discord bot;
  - 43 worldfile-converter;
  - 49 web.
- `dotnet ef migrations has-pending-model-changes`: no model changes after the final migration.
- `git diff --check`: passed; only line-ending conversion notices were reported.

Focused spatial/vehicle coverage includes exact bounds and proximity thresholds, indexed locality, continuous interpolation and checkpoint idempotency, source-aware placement, combat/AI/path integration, anchored portals, RoomScale hosted cells and docking, automatic and manual typed exit traversal, route revisions, recurrence and journey recovery, topology invalidation, and recursive mixed hitch cohorts.

## Persistence and Packaging

The EF migration chain ends with:

1. `20260722063400_RouteCellSpatialFoundation`
2. `20260722071041_RoomScaleVehicleInteriors`
3. `20260722100951_VehicleRoutesAndServices`

`DatabaseSeeder/BlankDatabaseSnapshot.manifest.json` names `20260722100951_VehicleRoutesAndServices` as its latest migration, and the unfiltered 545-test DatabaseSeeder suite passed against the refreshed SQL and manifest.

## V1.0 Boundary

Accepted V1.0 scope includes exact linear coordinates, anchored exits, weighted hybrid paths, local perception/interaction/combat, durable longitudinal movement, mounts and recursive hitches, persistent RoomScale interiors, ordinary-exit and RouteCell vehicle movement, docking/transit UX, and revisioned services/journeys.

The following remain explicitly post-V1: 2D/3D coordinates, collision and lane occupancy, dispatch/signalling, physical consist length, overtaking, automatic reverse-route generation, dynamic rerouting, fares/reservations, rich ownership/lease policy, and full electrical/liquid network simulation. Repeated reboot testing on an upgraded production-scale world and a long-duration release-candidate soak remain release-process gates rather than missing V1.0 behavior.
