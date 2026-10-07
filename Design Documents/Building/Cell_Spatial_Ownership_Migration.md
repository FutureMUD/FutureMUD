# Cell spatial ownership migration

## Final Room terminology checkpoint

The live concept is now `Room` / `IRoom`, mechanically the former Cell with its numeric identity preserved. The **legacy containing Room** described by the historical migration stages below is eliminated. The new `RoomTerminology` stage follows contraction; it renames physical tables, columns, indexes and named constraints without recreating tables or changing identities. Four provenance tables become `RoomSpatial*Ledger`; their historical parent ID becomes `LegacyRoomId` before the retained child ID becomes `RoomId`.

Populated naming cutovers require all writers stopped. The naming migration does not require a maintenance-session flag. Source/target collisions, unknown dependencies and affected triggers refuse. MySQL DDL cannot be treated as an atomic rollback: preserve a complete verified backup and matching binary/external files. Automatic Down is unsupported; restoring that backup is the recovery method, and loses later writes if they have resumed.

The approved empty-parent policy retains every original AreaId/parent-ID pair in independent Area provenance, with a nullable child ID for a discarded membership. Empty-parent warnings remain in the matching parent provenance. Areas themselves and their other memberships remain. Both initial and final ledger snapshots preserve these dispositions; cardinality, orphan and genuine external-reference refusals remain intact. The unpublished expansion/contraction metadata was regenerated through EF design services to match this nullable representation. The final naming stage also permits null child IDs for previously contracted databases with the earlier required ledger columns.

The naming stage also preserves the exact update and delete actions of 18 explicitly identified legacy foreign keys. Older installations can retain `ON UPDATE CASCADE` even though the EF baseline uses the default update action; the 2019 ActiveProjects SQL is one documented example. Their column order, local schema, target and expected delete semantics must still match. After all preflight checks pass, a session temporary table captures their actions before any foreign key is dropped. Recreated constraints use fixed reviewed renamed identifiers and those validated actions; postflight compares both actions to the capture. Unsupported action variants and mismatched relationships still refuse. A stale capture table refuses rather than being reused, and any failure after DDL still requires the full backup recovery described below. This source correction does not establish native migration acceptance.

New public type names are Room; new persisted type/ID pairs use `Room:v2`. Explicit legacy `Cell` references retain their child IDs. Bare historical `Room` references still refuse, even if their ID matches an unrelated surviving Room; unknown qualified versions refuse. Position writers, emote targets, tether anchors, fixed perceivers, Crime loading and check-result comparisons use this boundary. Default-hook categories use public Room with Cell-category compatibility, not the storage qualifier.

This is an unaccepted source checkpoint until current managed verification, independent review, blank-snapshot refresh/import and complete native upgrade/boot/restore qualification have succeeded. Earlier receipts and the bounded LabMUD default repair do not qualify this naming stage. The sections below record the preceding structural design using its historical Cell/Room labels.

## Contraction checkpoint

`20261006161646_CellSpatialContraction` follows the additive checkpoint below. The current runtime, shared interfaces and EF model have no Room entity. Cells directly own Zone/XYZ and Areas; Cell → Zone → Shard is the ownership chain. Existing numeric Cell IDs, unique names, exits, overlays, hosted interiors, routes, physical instances, custody and other Cell references remain unchanged. Historical migrations, RPI source room vocabulary, builder commands, RoomLayer and Cell-valued script API names remain compatible. Legacy spatial package DTOs are frozen wire records, not runtime owners.

This remains a review/qualification checkpoint. Do not publish, deploy, enable the held release, or run it against a game world without operator cutover preparation.

### Final writer freeze and source reconciliation

Expansion is not dual-write. Before contraction, stop every engine, installer, converter and extension writer. Capture complete database backup plus matching binaries and external files, record schema/history/server/casing/collations, classify extensions and prove restoration on a separate owned instance. These remain operator preparation requirements; the migrations do not enforce them through a maintenance-session flag.

The contraction preflight runs before game-table DDL and does not require a maintenance-session flag. It repeats the cardinality, parent, overlay, Area, typed-reference and unknown RoomId/FK guards below, and additionally refuses orphan derivative Cell/Area data, invalid owning-zone default Cells, unknown dependencies on Areas_Rooms or Cells.RoomId, and SQL views/routines/triggers/events with unreadable definitions or Room/Areas_Rooms/RoomId references. Such references must be explicitly reviewed, never silently erased or retargeted. Triggers attached to Rooms or Areas_Rooms are refused unconditionally, even when their bodies contain no legacy tokens, because dropping the owning table would silently drop the trigger. The diagnostic identifies a trigger for explicit disposition; no automatic conversion is attempted.

If the copied Cell Zone/XYZ or either direction of the Area mapping differs from the current Room source, contraction refuses by default. A reviewed final frozen session may also set `@FutureMUD_CellSpatialReconcile=1`. This copies the then-current sole-child mapping, updates only changed Cell metadata, removes only derivative Area links absent from the authoritative Room source, and adds missing links. It refuses reconciliation when unclassified Cell/Area-Cell triggers or Area-Cell dependents could give those writes external effects. It does not repair orphan source data or choose among multiple children.

### Provenance and destructive boundary

The original `CellRoomMigrationLedger` and `CellRoomAreaMigrationLedger` remain unchanged. Two independent final ledgers (`CellRoomContractionLedger`, `CellRoomAreaContractionLedger`) snapshot every current Room and Area/Room relationship under the final freeze. Thus old-writer edits between checkpoints do not erase initial migration evidence. Every unreferenced empty Room keeps final zone/XYZ and a warning with null Cell ID. Ledgers have no live FKs, so later world edits cannot erase their historical identities.

After the final snapshot and reconciliation transaction commits, a second guard verifies complete Room/Cell metadata, exact bidirectional Area mapping, counts and final provenance before any old FK/table/column is dropped. Then contraction removes Cells→Rooms, Areas_Rooms, Rooms and Cells.RoomId, and makes direct Zone/XYZ required. It does not deduplicate coordinates or regenerate IDs. Familiar Area `.rooms` values are Cells, as before.

### Failure and downgrade

MySQL DDL commits independently. A failure may leave final ledgers/copy committed, old tables partially dropped, or the complete new schema present without applied migration history. Keep all writers stopped; record actual schema/history/guard/ledger state. Do not blindly retry or invoke expansion Down. Restore the complete pre-contraction database, matching pre-contraction binary and external files, and verify keys/values/table definitions/SQL object definitions/history before a reviewed retry. A refusal can leave the reserved guard routine; full restore or narrowly verified cleanup of that exact newly-created routine is required. Never drop a preexisting object that conflicts with the reserved name.

Contraction Down deliberately throws before DDL. There is no automatic lossless Room recreation after direct-cell writers resume: empty parents, later cells, renamed keys and changed relationships cannot be inferred from current data. Restoring an older backup after reopening writers loses subsequent changes and requires an explicit recovery decision. Keep immutable initial/final provenance and protected cutover backups.

### Runtime and qualification boundaries

Creation, cloning, loading, saving, builders, Areas, maps, sound, autobuilders, vehicles, dwellings, FutureProg, fresh seeding and RPI target writes now use direct Cells. Hosted exterior projection remains distinct from stored ownership; ordinary and raw character/item membership use the stored Zone/Shard once. Rezone reconciles aggregate membership without movement callbacks and saves affected zone default selections. Simulation cells borrow stored metadata and Areas independently, register with no owning collection, and receive no global key.

Version 4 spatial exports contain direct Cell zone/XYZ and Area Cell keys. Versions 1–3 verify their original checksums before normalization; ambiguous/missing/Area-referenced empty parents refuse before import. Imported copies allocate fresh IDs and no global key. True serialized Room referents fail closed at runtime; they are not treated as numeric Cell references. Cell-valued AI storyteller fields such as ScopeRoomId, hotel rooms and historical audit fields retain their existing semantics.

Qualification must cover old→expanded→contracted unequal IDs, duplicate coordinates, overlapping Areas, empty warning provenance, refusals before game mutation, stale-copy refusal and explicit reconciliation, unchanged initial ledgers, final ledger correspondence, retained Cell/dependent data, partial-DDL/history-boundary full restore, unrelated SQL objects, first/cold hydration and current native lifecycle regressions. The maintained snapshot must be regenerated with the supported helper on an owned disposable database and match the official EF model/designer/migration. Record actual evidence separately; passing a build is not migration or gameplay qualification. Arbitrary extension formats, external files and case-sensitive Linux require installation-specific audit/rehearsal.

## Historical additive checkpoint

## Expansion checkpoint

`20261006143539_CellSpatialExpansion` is the additive first checkpoint of Room elimination. It does **not** remove `Room`, `IRoom`, `Rooms`, `Areas_Rooms` or `Cells.RoomId`. The runtime at the expansion revision still reads and writes the old Room structures. New nullable `Cells.ZoneId/X/Y/Z`, `Areas_Cells` and two provenance ledgers contain the frozen source's afterimage. They are not maintained by old runtime writes.

This checkpoint is for review and disposable qualification, not standalone deployment. Keep a real cutover in maintenance until the direct-cell runtime and contraction checkpoint are ready. Apart from stale afterimages, old runtime rezoning could leave the new Zone foreign key pointing at the former zone and change subsequent deletion constraints.

The approved final design has one direct cell owner: Cell → Zone → Shard. Familiar builder vocabulary, `RoomLayer`, FutureProg location names and the location-valued Area `.rooms` property remain compatible. Cell IDs and existing cell-based references must remain unchanged. Historical migrations remain append-only.

## Preconditions and refusal

Before applying this checkpoint to a populated world, stop all writers, retain the matching old binary and external files, create a complete consistent database backup, and prove that backup can be restored on an isolated instance. Protect that backup from retention pruning. Record the actual migration history, server version, table casing, collation, Room/Cell cardinalities and all old table keys and values. A spatial export is not a database backup.

Engine/installer startup and direct EF migration APIs can execute expansion for populated or empty databases without a maintenance-session flag. Complete the writer freeze, backup and restore preparation above before an operator attempts a populated-world cutover. Empty databases follow the same structural preflight checks when generating the maintained blank snapshot.

The migration executes its SQL preflight before its first table change, including when called through startup or direct EF migration APIs. It refuses:

- Multiple cells in any Room, even where numeric IDs happen to match. No first child, winner or grouping adapter is chosen.
- Orphan cell parents, Room zones, Zone shards, Area memberships, missing current overlays or overlays belonging to another cell.
- An Area membership referencing an empty Room. At this checkpoint the later instruction to reject referenced empty Rooms applies to Area links too; an explicit reviewed disposition is required.
- Foreign keys to Rooms other than the known `Cells.RoomId` and `Areas_Rooms.RoomId` relationships, or an unclassified `RoomId` column. The historical expansion guard exempted AIStorytellerSituations from this column scan. Its stock Cell-valued column is actually `ScopeRoomId`; contraction removes that table exemption and rejects an added `RoomId` column.
- String type columns holding `Room`, or text/JSON containing a serialized `*Type="Room"`, `*Type:'Room'` or `<*Type>Room</*Type>` reference. The diagnostic identifies the table and column. These are refused even for a sole-child Room: changing a historical/entity referent into a cell is not assumed to preserve its meaning.

Serialized scanning is conservative and is not a proof that arbitrary extension formats contain no Room references. Before contraction, classify local extension tables, formats and true referents explicitly. A serialized spelling not covered by these patterns must be audited; do not silently retarget it. Stage 1's runtime identifier validation remains the authoritative invariant/case comparison; this migration does not rewrite keys or substitute database collation equality for it.

The guard uses a reserved migration-specific stored procedure and parameterized regular-expression scans. The migration connection needs normal schema privileges plus `CREATE ROUTINE`, `ALTER ROUTINE` and `EXECUTE`. Missing privileges fail before table changes. A refused call can leave this reserved preflight routine behind; a subsequent guard attempt refuses its preexisting name. Inspect the routine and recorded failure, then explicitly remove only the confirmed migration-owned guard or restore the full backup before retrying. A preexisting unknown routine is never overwritten. No game rows are repaired.

The attribute/JSON alternative begins at `Type`, without a greedy identifier prefix. This preserves its previous unanchored matches: any prefixed match also contains the same matching `Type` suffix, and the old prefix permitted zero characters. The XML element alternative retains its prefix because `<` anchors the element name. This avoids repeated identifier scanning in large stock definitions without changing the candidate columns, case/whitespace rules, conservative malformed-input matches or MySQL regex resource limits. A regex timeout still aborts preflight; it is never treated as zero references.

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
- The expansion qualification runtime hydrated the expanded database in a fresh context and a separate cold process. This checkpoint tests native Cell construction, **not** a complete server boot or the future Room-free runtime.
- Restore after partial DDL recovers original schema, data and migration history on the owned private instance. The production restore procedure and external-file consistency still require operator rehearsal.

The qualification fixture populates independent Room/Cell IDs, overlapping Areas, duplicate XYZ, an empty Room, overlays/exits/default location, character/body/wounds/resources, a primary physical instance and its generated keys, item prototypes and ground/container/body custody, a route, a track, vehicle prototype/interior/occupancy (including its generated instance key), and cell environmental/resource state. Every original table column is compared, including empty tables; this inventory does not claim every gameplay subsystem is populated. Recovery also compares exact table definitions and all original migration-history rows. Existing functions/events/triggers, arbitrary external files and a case-sensitive Linux server are not represented by this fixture; audit/rehearse those before a real cutover.

The expansion checkpoint originally required the following separate contraction work, now represented by the checkpoint above: remove runtime/interface/model Room ownership; transfer raw custody/currency/displacement membership to Cell/Zone/Shard while preserving callback boundaries; retain vehicle/corpse/combat authority checks; update creation/load/save/area/weather/map/seeder/converter paths; freeze v1–v3 spatial package DTOs before introducing v4 direct-cell metadata; regenerate the required-field/drop-structure migration; assert the final copy before dropping the old foreign key and tables; refresh the maintained snapshot and qualify native gameplay/recovery and full managed gates. Do not enable the held release or publish/deploy this checkpoint.
