# Migration batching release qualification

This receipt qualifies the non-breaking migration compilation change for Engine 2.15.1 and Database Seeder 3.8.1. The earlier [Room release qualification](Engine_2.15_Seeder_3.8_Release_Qualification_20261022.md) remains historical evidence for the unpublished 2.15.0 / 3.8.0 candidates. Those immutable tags are not reused.

## Compatibility and compilation

The original migration sources, operations, snapshot and context are unchanged. The 278 designer files are assigned to 20 bounded helper projects. Migration discovery remains in the original persistence assembly. Only five redundant token-count/arithmetic assertions were removed; substantive historical upgrade coverage remains.

An isolated comparison against the original unpartitioned persistence assembly passed for all 282 migration IDs and target models, the current context design model and snapshot. Generated MySQL upgrade SQL was identical:

| Starting migration | Characters | SHA-256 |
| --- | ---: | --- |
| Empty history | 1,093,739 | `0DBDFA24681324DD8988C1A6D53F90753C8C060F0F0949D414810B5098CDD756` |
| `20261006143539_CellSpatialExpansion` | 362,985 | `3FBAF74E4CCC238E365B2CD55B452C5CDAB91F955B9F603A5831A831ECFF6E3E` |

The baseline assembly SHA-256 was `4B9E8CB6EDD960DD95CDF9E0888023F39E773A71EE3F9EE5CC5760D1DB594784`. The comparison used MySQL 8.0.45 provider settings without connecting to a database.

A serial Release/linux-x64 rebuild with embedded symbols, optimization and the existing analyzers passed in 164.29 seconds. Peak individual process memory was 1,744,191,488 bytes (1.62 GiB), and the sampled process tree peaked at 1,936,756,736 bytes (1.80 GiB). The comparable unpartitioned build peaked at approximately 19.64 GiB and took 52.4 seconds. Batching trades longer clean compilation for bounded peak memory; generated-source caching avoids unnecessary incremental rewrites. It does not remove historical metadata. [Issue #815](https://github.com/FutureMUD/FutureMUD/issues/815) tracks future compaction and an explicit supported upgrade checkpoint.

## Automated checks

The full Release gate `20261022T112911Z-b6e921e5b6ba` passed all ten suites: **11,646 passed, zero failed, three skipped**. Source was stable throughout (fingerprint `cba6a883afcd043b6c3078749c8ab8cd63dc7ed261915c968f6a01b0095a95cb`). The three skips are the existing Seeder Unix filesystem cases on Windows. All ten builds passed. The persistence suite includes ten new projection, partition validation and cache-integrity cases.

The earlier sandbox run `20261022T111602Z-0b2fa928883c` remains inconclusive: its builds passed, but testhosts could not connect and no tests executed. The successful rerun used the normal host environment; it does not rewrite the earlier receipt.

## Native database checks

All writes were confined to three owned `futuremud_release_*_20261022` databases on the local MySQL instance. The original `labmud_dbo` was not migrated or seeded. Private backups, identity comparisons and transcripts remain in ignored release artifacts.

The corrected pre-cutover fixture was restored and all 601 table counts, checksums and definition hashes matched its saved fingerprint. The newly compiled persistence assembly executed an injected failure immediately after the durable legacy `DROP TABLE Rooms`: the table was absent and the contraction history entry had not been written. A full restore again matched all 601 original table fingerprints exactly. An uninterrupted upgrade then advanced history from 269 to 284 entries.

Before gameplay, the upgraded fixture preserved all 34,797 checked character/body/instance/item identities, all 1,365 playable location IDs and their Zone/XYZ values, and existing physical Area membership. Legacy parent 11370 mapped to playable Room 11; item 1 retained the historical `Cell` wire label with corrected target 11. Empty legacy parent 11 retained null-child provenance.

The bundled blank SQL was imported into the third owned database and verified against the repository snapshot after only database-name substitution. Migration history remained at 282 entries with no pending migration. The product manifest now identifies Seeder 3.8.1; snapshot SQL and generation timestamp remain unchanged because the schema was already current.

## Representative packages

Both win-x64 products published successfully with the manifest's framework-dependent, single-file, native-bundling, untrimmed settings and embedded symbols. The Engine executable is 192,617,303 bytes and the Seeder executable is 234,255,188 bytes. Each recorded bundle contains all 20 migration helper implementation assemblies exactly once and excludes `MigrationModelCompiler` and its Roslyn 4.13 dependencies. Existing runtime Roslyn 4.8 dependencies are retained.

The clean dependency manifests contain both project and synthetic `.Reference` records for helper assemblies; both records identify the same implementation DLL. This does not duplicate the physical bundle payload. Runtime acceptance is established by the packaged export and native checks below.

The packaged Engine documentation export passed on the normal host and reports version 2.15.1 with 633 commands, 738 prog functions, 398 prog types, 13 collection extensions and 253 item components. Its source revision is the recorded base `c50fa1271316eea1f7692cf56261258aa1604baa` plus this candidate's uncommitted changes; the publication workflow exports the final tagged revision. A sandboxed apphost attempt exited before export; the successful host run is recorded separately.

The same packaged executable (SHA-256 `6D573BB456AD8805E86B4EF635A7D58744EBDD03BDE9D1104AC563B16BBC2D36`) passed four native scenarios without retries: first and cold starts of the current-world copy, followed by first and cold starts of the upgraded historical copy. Each reached readiness, accepted the fixture login, executed `look`, `time`, `room show` and `save`, completed the save, then exited normally after `shutdown stop`. The current copy resolved Room 1312, and the historical copy resolved Room 1314. The current copy's first startup applied its three pending migrations through the production upgrade path.

All four server processes shut down cleanly. The three owned databases were removed after checking that none had remaining client connections; the original database, shared MySQL service and retained backups were preserved. These checks cover ordinary startup, room resolution, saves and recovery, not exhaustive gameplay or arbitrary installation-specific SQL/serializers. Hosted cross-platform checks and production publication are separate release gates.
