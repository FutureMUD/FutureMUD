# Engine 2.15.0 / Database Seeder 3.8.0 release qualification

Recorded 22 October 2026 on Windows x64, .NET SDK 10.0.401 and MySQL 8.0.45. The user authorised local LabMUD verification followed by publication of both products with the approved patch notes. This receipt closes the representative LabMUD Room-upgrade and restore hold; it is not a certificate for every historical/custom database or every platform.

## Source and execution boundary

The candidate starts at master `1d8534bb3663d54d4481d23f4b84c286b050ca19`, with the Engine 2.15.0 / Seeder 3.8.0 version changes, approved patch notes, snapshot product metadata and the eight-effect serializer-inventory correction. All native servers used copied or verified published binaries, separate ports and explicitly owned databases. No seeder refresh or migration was applied to the original `labmud_dbo`.

The original already records `CellSpatialExpansion`, `CellSpatialContraction`, `RoomTerminology` and `TargetedLegacyRoomReferences`. Its 1,369 retained contraction mappings all resolve to live playable locations. Its complete 606-base-table copy matched every row count and checksum. At the end of verification, all original table counts/checksums were still unchanged. This corroborates the earlier report that LabMUD had already crossed the Room boundary.

Private SQL backups, transcripts and full row identities remain in ignored `.artifacts/release-readiness-20261022/`; they are not published with this receipt. Test-only login credentials and disabled outbound integration bindings were confined to copies. Native startup used the production entrypoint and the existing local Telnet helper; no loader or command dispatcher was replaced.

## Current LabMUD copy

1. Restored the consistent current backup into a new owned database; all 606 base tables matched the original.
2. Applied its three pending release migrations through the real EF migration service. History advanced from 281 to 284 entries, including retained historical entries, with no pending migration afterward.
3. Booted the final Release engine, logged in, inspected Room 1312 through `room show`, saved, and executed `shutdown stop` successfully.
4. Repeated the normal production boot, login, room inspection, save and graceful shutdown as a cold-reader check.

The initial smoke reached readiness but refused the helper's assumed account password. It did not qualify login/shutdown. The owned process was stopped, the copied account was explicitly configured as a disposable test fixture, and the successful first/cold runs were recorded separately. No original account credential was changed.

## Historical release and structural recovery

The retained LabMUD backup dated 3 April 2026 contains the old containing Rooms and playable Cells. It was restored into a second owned database and advanced through managed EF migration execution to `20261003170627_SpellOwnedRemainsRemovalIntent`, the schema shipped with Engine 2.14.0. The published Windows 2.14.0 archive was downloaded and verified against its published SHA-256:

`F20E75CB77341154A59148CDDCF3E5122A33B557B5761F26609DA20E01739656`

The actual released executable passed native boot, account login, room inspection, save and graceful shutdown against that populated pre-cutover schema.

A controlled test fixture then reparented playable Cell 11 to legacy Room 11370 without changing its playable ID, added a matching legacy position reference on item 1, and retained the now-empty old parent 11. This exercised unequal namespaces and empty-parent provenance with the original database untouched.

The first migration attempt correctly refused a pre-existing invalid default: Zone 8 selected Cell 496, whose owner is Zone 9. A full restore reproduced all 601 pre-cutover tables exactly, including row counts, checksums, table-definition hashes and migration history. The current world's corrected default, 1377, does not exist in this dated fixture; Zone 8 has no eligible own location there. Only that disposable fixture's invalid default was cleared to null, retaining the migration's ownership guard. No production code or guard was relaxed.

The corrected fixture was backed up again. A test-only command interceptor then threw immediately **after** the real contraction `DROP TABLE Rooms` completed. Inspection proved the legacy table was gone while the contraction migration-history entry was absent. Restoration of the full pre-cutover backup again reproduced all 601 table counts, checksums and DDL hashes exactly. The intentional interruption remains an expected injected failure, not an ordinary successful migration.

After restoration, an uninterrupted migrate-to-latest completed. Before any subsequent gameplay, comparison proved:

| Invariant | Result |
| --- | --- |
| Character, body, physical-instance and item identity sets | All 34,797 identities preserved |
| Playable location IDs | All 1,365 former Cell IDs preserved as Rooms |
| Zone and XYZ for each playable location | Exact preservation for all 1,365 |
| Existing physical Area membership | Exact preservation |
| Unequal parent-to-child mapping | Legacy parent 11370 maps to playable Room 11 |
| Supported legacy position reference | Item 1 changed from `Room:11370` to `Cell:11` |
| Empty legacy parent | Parent 11 retained as null-child warning provenance |
| Cross-zone defaults | No invalid owning-zone default remained |

The upgraded historical world then passed first and cold production Release boots, native account login, `look`, `room show`, `save` and `shutdown stop`. The observed location remained Room 1312. These samples establish ordinary startup and persistence; they do not claim every builder operation, extension format, hosted-interior lifecycle or failure permutation was exercised.

## Snapshot, tests and packaging

The bundled blank snapshot imported into a third owned database. Its 282 migration entries already reached `20261021023928_ConfigurableRaceAgeDescriptions`; a managed migrate-to-latest left that count unchanged. Snapshot product metadata now names Seeder 3.8.0; its generation timestamp and SQL were retained because the schema was already current.

All four targeted Release builds succeeded. Run `20261022T040044Z-f7d1c563e06d` produced complete passing native TRX invocations:

| Suite | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Engine | 8,206 | 0 | 0 |
| Seeder | 2,085 | 0 | 3 |
| Persistence library | 301 | 0 | 0 |
| Website | 54 | 0 | 0 |

The three skips are the Unix filesystem-permission cases on Windows. The wrapper's overall result remains **INCONCLUSIVE / SOURCE_CHANGED**: importing the native Telnet helper regenerated its tracked Python bytecode cache during the concurrent run. That generated file was restored, and future native imports disabled bytecode writes. Recomputing the reporter's own fingerprint matched the start value exactly (`5541e881ade7d3bb0eb093ed9078789f0d7f16e04163b7a39a8a7820ce125d9b`); the four tested assembly hashes were also unchanged. This identifies the unrelated change but does not rewrite the original receipt into a passing run. Fresh publication/hosted checks retain their independent authority.

The eight new built-in effect serializers were audited and classified without changing their payloads: limb/trait/power/clan IDs remain in their own namespaces, and `PsionicDome.RoomId` is already a post-cutover playable Room ID. The inventory assertion and all eight new namespace-preservation cases passed. Independent focused review found no actionable defect.

The database-free Release catalogue export reports Engine 2.15.0 with all five families populated: 13 collection extensions, 633 commands, 253 item components, 738 prog functions and 398 prog types. Its local source revision identifies the base plus the recorded candidate edits; publication exports the final tagged revision again.

Representative Seeder `win-x64` publication succeeded with framework-dependent, single-file, native-bundling, untrimmed settings. Product-tag workflows must still validate their exact version commit, all three runtime packages, publishing smoke test and production promotion before shipment is claimed.

## Retained limits

Original and corrected-fixture refusals, the injected interruption, the failed assumed-login attempt and the wrapper's inconclusive receipt are retained. Case-sensitive Linux migration execution, arbitrary custom serializers/SQL objects and exhaustive gameplay remain installation-specific checks. The separately accepted exceptional Borrow retirement failure and ammunition investigations are not closed by this work.

All owned MUD processes were shut down. The three uniquely named disposable databases were removed after verifying no remaining client connection; the shared MySQL server was left running. The original database and retained backups are preserved. Publication outcomes are recorded separately by the release operator.
