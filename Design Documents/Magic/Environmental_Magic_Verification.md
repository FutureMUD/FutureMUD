# Environmental magic verification — 13 September 2026

The revised Task 1 completion checks were executed in a Windows worktree using SDK 10.0.401 and the repository's declared .NET 10 / EF Core 9.0.11 packages. [Acceptance mapping](Environmental_Magic_Acceptance.md) identifies the actual assertions for all 13 functional and 24 scheduling requirements. [Performance results](Environmental_Magic_Performance.md) report the separate 108-scenario measured workload.

## Builds and automated checks

Targeted `MudSharpCore` Debug and Release builds passed with `-m:1`; the final Release build reported only three pre-existing nullable warnings in unrelated combat/perceiver files. The benchmark Release build passed. `scripts/test-unit.ps1` built the test projects serially, then executed its isolated test hosts:

| Suite | Passed |
| --- | ---: |
| FutureMUDLibrary | 505 |
| ExpressionEngine | 36 |
| DatabaseSeeder | 1,378 |
| MudSharpCore | 3,421 |
| MudsharpDatabaseLibrary | 54 |
| DiscordBotCore | 22 |
| RPI Engine Worldfile Converter | 43 |
| FutureMUD.Web | 54 |
| TerrainPlanner | 34 |
| **Total** | **5,547** |

No failures or skipped tests were reported. This includes real command registration/permission checks for administrators and ordinary players, actual native forage/agriculture mutations, 30,000-cell heartbeat cases, controlled clock and replay failures, and the new schema/snapshot assertions. The opt-in climate suite is outside this change. Builds used the repository's permitted local `NU1902`/`NU1510` suppression; the broad script disables NuGet vulnerability queries after earlier network lookup warnings. These are compilation/test results, not a dependency-security audit. Final `git diff --check` and document-link/method-mapping checks passed.

The current master update `f30870b6` (independent spell-backed power routing) was incorporated without conflicts before publication. All 3,421 core tests passed again on that integrated tree, and the targeted Release engine build was repeated.

## MySQL migration and installer

Migration `20260913105616_EnvironmentalMagic` was generated with EF tooling, including its designer and model snapshot. The generated model reports no pending changes. The bundled installer SQL and manifest were refreshed through the production snapshot API, rather than manually reconstructing schema SQL.

| Isolated target | Executed result |
| --- | --- |
| `futuremud_environment_df74_upg_0913110421` | Imported prior installer state through `20260912061254_VancianMagic`, then passed the guarded upgrade to the new migration |
| `futuremud_environment_df74_snap_0913110421` | Full snapshot refresh passed; manifest records product 3.5.1.0 and the new latest migration |
| `futuremud_environment_df74_fresh_0913110421` | Fresh import of the new installer snapshot passed |

No stock environmental profiles or world policy were seeded. The migration adds nullable bindings/default inheritance, one optional ecological row per physical cell, and historical operation receipts without replacing existing balances.

## Live authoring, persistence and restart

The installed MUD tester launched this worktree against the uniquely named clone `futuremud_environment_df74_live_0913112741`. The source development world was read only to create the clone. Runtime damage/balances were authored through public in-game commands; no SQL inserts stood in for gameplay/service operations. The helper stopped each process it launched.

1. Created the location-capable simple resource `EnvSmokeEssence` and environmental profile `EnvSmokeProfile` through `magic resource/regenerator edit new`. Set base capacity 1,000, rate 60 per real minute and maximum `max(0, basecapacity - scardamage)`.
2. Attached the profile with `magic environment cell here EnvSmokeProfile`. Initial inspection showed zero balance and a valid 1,000 maximum. The coordinator later advanced the existing balance to `60.73426060000001` through actual online elapsed time.
3. Applied damage 8 and pressure 4 with operation GUID `1b0fa318-4f58-454d-a752-c06ff0cdbf87`, repeated the exact request, and received the recorded result. Applied repair 3 under GUID `f95d38cf-7ae7-4524-a88b-c118de2cdbed`. Remaining scars were 5; the original destructive timestamp remained `2026-09-13 11:37:39 UTC`.
4. Used the actual clone command to create `EnvSmokeClone`. Both original and clone reloaded with valid independent versioned definitions. Flushed the save queue, stopped the server, and read the persisted binding, balance, ecological row, two receipts and generator XML from MySQL.
5. Restarted the server. The first inspection retained balance `60.734261` at displayed precision, scars 5 and the same timestamp. Pressure had decayed to approximately 3.428758; no shutdown-time mana appeared. Repeating the damage request again returned its recorded receipt and left scars at 5.
6. Disabled the cell profile and flushed again. Diagnostics showed zero configured/producing/dirty/faulted entries and empty production/audit/discovery queues. A final read-only MySQL check found binding mode 2, null profile ID, scars 5, revision 3 and exactly the same two receipts. Balance `69.44342780000002` includes the final online interval settled at detach and remains stored.

## Failure and measurement boundaries

Deterministic tests exercise failed writes, rolled-back claims, failed confirmation and a lost acknowledgement after a simulated durable commit. The real MySQL run proves successful atomic persistence and replay after process restart; it does not inject a real network break during `COMMIT`, an OS crash, or simultaneous independent servers writing the same cell. Unknown outcomes remain frozen until the exact operation identity is confirmed; no automatic write replay is attempted.

The measured harness uses real coordinator/cell/generator/heartbeat code with substituted world registries and save sinks. It excludes database, networking and competing gameplay costs. At 30,000 cells with up to 10% active, measured conservative active-update age stayed below 61 seconds. Fully active heavy cases reached 74–88 seconds and exceeded the 5 ms soft admission budget. Strict minute service for that latter workload is unsupported by this run; the runtime retains delayed elapsed production and reports queue age. The raw matrix includes allocations and transient bursts, including the larger native-source burst outliers.
