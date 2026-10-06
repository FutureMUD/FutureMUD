# Cell spatial ownership migration

## Expansion checkpoint

`20261006143539_CellSpatialExpansion` is the additive first checkpoint of Room elimination. It does **not** remove `Room`, `IRoom`, `Rooms`, `Areas_Rooms` or `Cells.RoomId`. Current runtime code still reads and writes the old Room structures. New nullable `Cells.ZoneId/X/Y/Z`, `Areas_Cells` and two provenance ledgers contain the frozen source's afterimage. They are not maintained by old runtime writes.

This checkpoint is for review and disposable qualification, not standalone deployment. Keep a real cutover in maintenance until the direct-cell runtime and contraction checkpoint are ready. Apart from stale afterimages, old runtime rezoning could leave the new Zone foreign key pointing at the former zone and change subsequent deletion constraints.

The approved final design has one direct cell owner: Cell → Zone → Shard. Familiar builder vocabulary, `RoomLayer`, FutureProg location names and the location-valued Area `.rooms` property remain compatible. Cell IDs and existing cell-based references must remain unchanged. Historical migrations remain append-only.

## Preconditions and refusal

Before applying this checkpoint to a populated world, stop all writers, retain the matching old binary and external files, create a complete consistent database backup, and prove that backup can be restored on an isolated instance. Protect that backup from retention pruning. Record the actual migration history, server version, table casing, collation, Room/Cell cardinalities and all old table keys and values. A spatial export is not a database backup.

Normal engine/installer migration connections cannot upgrade a populated Room table. An operator must explicitly set `@FutureMUD_CellSpatialMaintenance=1` on the **same database session** that executes the reviewed migration, after satisfying the conditions above. The session flag is an acknowledgement, not proof of a backup or writer freeze. Do not add it to routine startup. Empty databases may migrate without the flag so the maintained blank snapshot can be generated.

The migration executes its SQL preflight before its first table change, including when called through startup or direct EF migration APIs. It refuses:

- Multiple cells in any Room, even where numeric IDs happen to match. No first child, winner or grouping adapter is chosen.
- Orphan cell parents, Room zones, Zone shards, Area memberships, missing current overlays or overlays belonging to another cell.
- An Area membership referencing an empty Room. At this checkpoint the later instruction to reject referenced empty Rooms applies to Area links too; an explicit reviewed disposition is required.
- Foreign keys to Rooms other than the known `Cells.RoomId` and `Areas_Rooms.RoomId` relationships, or an unclassified `RoomId` column. The known AI storyteller `RoomId` column denotes a **Cell ID** and is left alone.
- String type columns holding `Room`, or text/JSON containing a serialized `*Type="Room"`, `*Type:'Room'` or `<*Type>Room</*Type>` reference. The diagnostic identifies the table and column. These are refused even for a sole-child Room: changing a historical/entity referent into a cell is not assumed to preserve its meaning.

Serialized scanning is conservative and is not a proof that arbitrary extension formats contain no Room references. Before contraction, classify local extension tables, formats and true referents explicitly. A serialized spelling not covered by these patterns must be audited; do not silently retarget it. Stage 1's runtime identifier validation remains the authoritative invariant/case comparison; this migration does not rewrite keys or substitute database collation equality for it.

The guard uses a reserved migration-specific stored procedure and parameterized regular-expression scans. The migration connection needs normal schema privileges plus `CREATE ROUTINE`, `ALTER ROUTINE` and `EXECUTE`. Missing privileges fail before table changes. A refused call can leave this reserved preflight routine behind; a subsequent guard attempt refuses its preexisting name. Inspect the routine and recorded failure, then explicitly remove only the confirmed migration-owned guard or restore the full backup before retrying. A preexisting unknown routine is never overwritten. No game rows are repaired.

## Mapping and provenance

For Room 9000 → Cell 8101 in Zone 3 at `(17,-2,4)`, copy Zone 3 and those coordinates onto **Cell 8101**. Do not use Cell 9000. Area `(7,9000)` becomes `(7,8101)`. All original Cells, cell overlays, exits, characters, instances, items, logs, scripts, routes, vehicles and other dependents retain their keys and values.

`CellRoomMigrationLedger` records every original Room's ID, sole Cell ID (or null), zone and coordinates. It retains metadata and a removal warning for every unreferenced empty Room. `CellRoomAreaMigrationLedger` records each original Area/Room link and its exact mapped Cell ID. Neither ledger has foreign keys to live entities: future entity deletion must not erase migration provenance. `Areas_Cells` retains its composite Area/Cell key and live foreign keys. The new Cell→Zone foreign key restricts zone deletion. Coordinates remain nonunique; overlapping coordinates and overlapping Areas are preserved.

No source Room is deleted in expansion. An empty Room's ledger warning permits later contraction removal; it does not claim removal has already happened. Area objects and their weather/controller configuration remain intact, including unrelated or empty Areas.

## Failure, recovery and downgrade

MySQL DDL implicitly commits. EF or application transaction rollback is **not** an expansion recovery mechanism. Expansion adds its schema first; its copy runs in a separate explicit DML transaction. A failure after schema creation can leave new columns/tables present while migration history still says the checkpoint is unapplied.

After any failure, keep writers stopped. Record actual schema, migration history, reserved routine and the ledgers' state. Do not blindly retry EF against partial DDL. Restore the full pre-expansion backup together with the matching old binary/files, verify the original table/column structure and every original key/value, and only then retry a reviewed migration. Qualification injects a failure after expansion DDL and before copy, demonstrates that the DDL persists, then restores and compares every original table definition (including foreign keys, indexes, defaults, collations and counters), value and migration-history row. It also injects failure after the committed copy but before migration history, and verifies the same full restore boundary.

The generated expansion `Down` drops only derivative columns/tables and their constraints; all original Room structures remain authoritative at this checkpoint. It loses ledger/afterimage evidence and must not be treated as a rollback after a future direct-cell runtime begins writing new state. After contraction, recovery requires the pre-cutover database/binary/files backup. Reopening writers creates a data-loss boundary for restoring that older backup; no lossless automatic downgrade is promised.

## Postflight and next checkpoint

Before contraction, rerun preflight under the final writer freeze and reconcile the afterimage from the then-current authoritative Room data. Expansion is not a dual-write system. New Rooms, cells, area changes or coordinate/rezone changes made by the old runtime after expansion make this copy stale.

Require exact comparison of every original table's rows and columns, then verify:

- Every Cell ID maps to its actual Room's zone/XYZ; every accepted Room has exactly one ledger entry.
- Every old Area/Room link has exactly one ledger entry and one new Area/Cell link; unrelated Area objects/configuration remain unchanged.
- Empty Room metadata/warnings are fully accounted for; duplicates are not deduplicated; unknown references were refused.
- Original numeric identifiers, cell keys and all existing dependent keys/values are unchanged.
- The current runtime hydrates the expanded database in a fresh context and a separate cold process. This checkpoint tests native Cell construction, **not** a complete server boot or the future Room-free runtime.
- Restore after partial DDL recovers original schema, data and migration history on the owned private instance. The production restore procedure and external-file consistency still require operator rehearsal.

The qualification fixture populates independent Room/Cell IDs, overlapping Areas, duplicate XYZ, an empty Room, overlays/exits/default location, character/body/wounds/resources, a primary physical instance and its generated keys, item prototypes and ground/container/body custody, a route, a track, vehicle prototype/interior/occupancy (including its generated instance key), and cell environmental/resource state. Every original table column is compared, including empty tables; this inventory does not claim every gameplay subsystem is populated. Recovery also compares exact table definitions and all original migration-history rows. Existing functions/events/triggers, arbitrary external files and a case-sensitive Linux server are not represented by this fixture; audit/rehearse those before a real cutover.

Contraction remains a separate implementation/verification checkpoint: remove runtime/interface/model Room ownership; transfer raw custody/currency/displacement membership to Cell/Zone/Shard while preserving callback boundaries; retain vehicle/corpse/combat authority checks; update creation/load/save/area/weather/map/seeder/converter paths; freeze v1–v3 spatial package DTOs before introducing v4 direct-cell metadata; regenerate the required-field/drop-structure migration; assert the final copy before dropping the old foreign key and tables; refresh the maintained snapshot and qualify native gameplay/recovery and full managed gates. Do not enable the held release or publish/deploy this checkpoint.
