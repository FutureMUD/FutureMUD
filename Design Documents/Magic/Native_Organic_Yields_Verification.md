# Native Organic Yields Verification

## Scope

This is the durable evidence record for `MAGIC-LAND-YIELDS-03A`, implemented from checkout `13b495103da639114670d3759f9647519f446bdb` on `codex/native-yields-ecological-penalties` and rebased onto current `master`. The final disposable harness ran against clean post-rebase code revision `16327f882932185b2737de5976f693d2fc19d21c`; the later amendment only incorporated this evidence into documentation. It separates commands actually executed from code inspection, builder demonstrations and remaining limitations.

## Tests executed

Executed on Windows, 19 September 2026, against the final feature source. The full core suite and harness build were repeated after the rebase.

| Check | Result |
| --- | --- |
| `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1` | Passed, 0 errors |
| `dotnet build MudSharpCore\MudSharpCore.csproj -c Release --no-restore -m:1` | Passed, 0 errors |
| `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1` | Passed, 3,525/3,525 |
| `dotnet test 'FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1` | Passed, 505/505 |
| `dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Release --no-restore -m:1 --filter 'FullyQualifiedName~AgricultureNativeOrganicAccountingTests\|FullyQualifiedName~AgricultureOperationTests\|FullyQualifiedName~EnvironmentalMagicCoordinatorTests\|FullyQualifiedName~EnvironmentalMagicGeneratorTests\|FullyQualifiedName~EnvironmentalMagicSurfaceTests\|FullyQualifiedName~ForagingRuntimeTests\|FullyQualifiedName~SaveManagerFailureRecoveryTests'` | Passed, 209/209 |
| `dotnet build 'Temporary Scratch App\GatheringNativePersistenceHarness\GatheringNativePersistenceHarness.csproj' -c Debug --no-restore -m:1` | Passed, 0 errors |
| `& 'Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1'` | Passed, `nativeHarnessExit=0`; both owned databases and temporary server deleted |
| Y-P04 and Y-T25 focused Debug run with normal console logger | Passed, 2/2; Y-P04 298 ms, Y-T25 2 s, combined 3.2676 s |
| `git diff --check` | Passed; Git printed only LF-to-CRLF notices |

The build output also contained `NU1900` because the NuGet vulnerability feed was unavailable, plus existing nullable warnings in unrelated core/test files. No warning suppression was added. No full live MUD login, hosted CI result, or production-server throughput claim is implied by these checks.

## Code inspected

- Environmental definition validation, channel-specific fail-closed behavior, formula dependency evaluation, coordinator calculation/invalidation and indexed agriculture lookup.
- Native cell forage projection, decimal-domain exact consumption, ordinary-consumer and hourly-recovery synchronization, plus all co-staged cell dirty flags and environmental revision on failed save.
- Agriculture crop/orchard, woodland and pasture lifecycle, shared prepaid credit, owner compare/apply synchronization, daily tick, operation, craft reservation, grazing and save/load paths.
- Existing gathering service and receipt boundary: Task 3A adds no Land method, personal magic payout, operation receipt, per-source scheduler or schema migration.

## Authoring and gameplay demonstration

The production profile builder path was invoked by the disposable harness with these editor inputs: `environmental <resource-id> Land Harness Organic Profile`, `organic source add forage herbs`, `organic source add crop`, and `organic penalty forage 1 - scardamage / 20`. `SaveManager` persisted profile `1`; reload observed two declarations and one penalty. A production clone became profile `2`; removing crop and saving the clone left the independently reloaded original at two declarations and the clone at one (`Y-T02=passed`). This is an executable builder-path demonstration, not a claim of an interactive telnet session.

The real coordinator/cell test `NativeForageRecovery_BuilderOptInScarsAndCoordinatorRepairAffectOnlyFutureRecovery` exercised the same `organic source add forage herbs` and `organic penalty forage 1 - scardamage / 20` editor surface. From 100 herbs, ordinary consumption left 40. The next hourly tick added 10 at neutral scar damage, then an applied scar of 10 reduced the following increment to 5. Staff repair of 10 left stock and environmental mana unchanged immediately; the next hourly tick added 10. An unconfigured cell remained at baseline `+10` despite the same scar. The crop declaration, with no indexed field, inspected as `Absent`; the test forbade field enumeration during repair and observed none. The command-dispatcher test separately verified staff `magic environment yields here` inspection and `yields repair here crop` routing without mutation on inspection.

The Y-T25 real coordinator test used 30,000 cells and the final indexed cell. Inspection reported 90 herbs, an exact 0.25 debit left 89.75, and one native hourly tick produced 99.75. It asserted unchanged cell/field enumeration counts, second/hour subscriptions, profile delegates and scheduler entries; its observed test duration was 2 s. This is a bounded test-path measurement, not a sustained-load benchmark.

## Persistence fixtures

The final isolated run launched a fresh loopback-only MySQL 8.0.45 instance, imported the supported blank snapshot, and created only generated, marker-owned databases. The Task 3A database was `futuremud_land_20260919091014_ea913afcc9`; the pre-existing Self-gathering regression database was `futuremud_gather_gc_20260919090927_7a16a7dee9`. The latter passed its GC-P01/P02/P03 independent-reader receipt/wound checks; its operation IDs were `091b72d7-f99f-40cd-b4c8-e4c14d25d4a4` and `95015a7d-b2c1-4561-ad10-6ab365331f74`.

| Proof | Fixture identity and observed quantities | Elapsed |
| --- | --- | --- |
| Y-T02 | Created environmental profile `1`; clone `2`; reloaded original sources `2`, clone sources `1`, clone penalties `1` | Included in land run |
| Y-P01 | Crop field `1`, physical cell `1`, generation `1`: opening `Q=10,C=0`; quarter debit independently read as `Q=9,C=0.75`; a separate process reconstructed the owner, debited `0.75` with whole debit `0`, and persisted `Q=9,C=0` | 7,019 ms |
| Y-P02 | Crop field `1`: replacement generation `1 -> 2`; recovery factor `0.25`; independently reloaded second tick retained stock `50`, crop-health remainder `0.50`, crop-yield remainder `0.50`, growth days `2` | 62 ms |
| Y-P03 | Crop field `2`: real craft reservation consumed five yield points and invalidated its earlier plan; annual harvest removed the crop and predecessor fraction. Pasture field `3`: opening `10`, quarter debit reduced stock to `9`, real herd grazing debited `2`, closing native stock `7` and prepaid `0.25` | 62 ms |
| Y-T24 provider failure | Field `3`: an owned trigger rejected the first update; independent read remained stock `7`, prepaid `0.25`; after trigger removal `SaveManager` retry persisted stock `7`, prepaid `0` with a newer revision | 42 ms |

The Task 3A harness took 34,403 ms. Its only substitutes were unrelated world catalogues, deterministic weather and a configured ecological scalar; field accounting, operation/tick, craft reservation, grazing, `SaveManager`, EF/MySQL and independent-reader reconstruction were production paths. The runner reported `nativeHarnessExit=0`, twice `cleanup=deleted-owned-database`, and `temporaryMySqlCleanup=gracefully-shut-down-and-deleted-owned-instance`. The temporary databases and server were not retained; this table is the durable fixture record.

## Completion boundary and limitations

**Source accounting and penalties implemented; Land command not yet implemented by this PR.** No player-facing selection, personal-mana payout, rejuvenation spell, inventory drain, NPC policy or legal response is part of this delivery. The debit plan is short-lived and non-idempotent; the later Land command must attach the native mutation to its parent durable gathering receipt and treat ambiguous provider outcomes as uncertain. No live MUD/telnet session or deployed-world acceptance was run. The 30,000-cell assertion shows indexed, subscription-neutral behavior for this path, not a production throughput guarantee.
