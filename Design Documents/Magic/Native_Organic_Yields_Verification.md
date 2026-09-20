# Native Organic Yields Verification

## Task 3A remaining corrections and native acceptance (20 September 2026)

Assignment: `03A_PR754_Remaining_Corrections_and_Verification.md`, **Task 3A — Finish the PR #754 corrections and native acceptance**, task ID `MAGIC-LAND-YIELDS-03A-CORRECTIONS-FOLLOWUP`. The checked-out source was reviewed PR #754 head `100f3d9f0502bcbe67e6b0d36620ec2f82bae87a` on `codex/pr754-remaining-corrections`. The final commands below ran against that revision plus the uncommitted corrective changeset. This section supersedes the historical C-P03 gap recorded below; the earlier run remains as dated evidence.

Crop DailyTick now retains the pre-feature nutrient clamp followed by the pollination clamp. Tests cover neutral boundary sequences `0/-1/+2 -> 2`, `100/+1/-2 -> 98`, and `100/-1/+2 -> 100`, explicit factor 1, near-boundary cases, non-neutral positive suppression with unchanged costs, health ordering, orchard final-expression headroom and fractional progress. The native owner constructs the current positive crop/woodland context used both for production and conversion validation; the cell shares its hourly forage context. Pasture has no present positive recovery context between relevant operations. Pollination lookup uses the coordinator's active apiary candidate index. The two specified crop-health formulas were exercised with a real field, profile and coordinator at current baselines 4 and 1, including a valid plan refused after the baseline changed, with no stock, prepaid credit or revision change.

| Final local check | Observed result |
| --- | --- |
| `dotnet build MudSharpCore/MudSharpCore.csproj -c Debug --no-restore -m:1` | Passed |
| `dotnet build MudSharpCore/MudSharpCore.csproj -c Release --no-restore -m:1` | Passed; three existing analyzer RS2008 warnings |
| `dotnet test 'FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj' -c Debug --no-restore -m:1` | Passed 505/505 |
| `dotnet test 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 --filter 'FullyQualifiedName~AgricultureNativeOrganicAccountingTests\|FullyQualifiedName~AgricultureOperationTests\|FullyQualifiedName~EnvironmentalMagicCoordinatorTests\|FullyQualifiedName~EnvironmentalMagicGeneratorTests\|FullyQualifiedName~EnvironmentalMagicSurfaceTests\|FullyQualifiedName~ForagingRuntimeTests\|FullyQualifiedName~SaveManagerFailureRecoveryTests\|FullyQualifiedName~Gathering'` | Passed 261/261 |
| Same focused filter with `-c Release` | Passed 261/261 |
| `scripts/test-unit.ps1` | Passed all nine listed projects: library 505, expression 36, seeder 1379, core 3534, persistence library 58, bot 22, converter 43, web 54, terrain planner 34; total 5665 |
| `dotnet build 'Temporary Scratch App/GatheringNativePersistenceHarness/GatheringNativePersistenceHarness.csproj' -c Debug -m:1 --no-restore -v:q` | Passed, zero warnings/errors |
| `& 'Temporary Scratch App/GatheringNativePersistenceHarness/Run-IsolatedAcceptance.ps1'` | Passed, `nativeHarnessExit=0`, MySQL 8.0.45, both owned databases and temporary server deleted |

The final full harness created `futuremud_gather_gc_20260920001258_d097be8cd6` and `futuremud_land_20260920001343_2afeb94b3b`. GC-P01/P02/P03 passed independent wound, mana and receipt reconstruction. Y-T02 persisted environmental profile 1 and clone 2, leaving two original sources and one cloned source. C-P01 used staged fields 4 and 5, then production-constructed fields 8 (cell 7) and 9 (cell 8): first insert independently showed 50 pending; factor 0.5 persisted 25 assessed, factor 0 persisted 0 assessed, with reload and unchanged re-entry. C-P02 field 6 persisted orchard stock 81, prepaid 0.75 and yield remainder 0.5 after the harvest; a reconstructed tick persisted 82.

C-P03 used field 7, cell 9 and profile 1. The real cell loaded a persisted overlay and forage profile. Forage ordinary consumption saved herbs 90; the coordinator's scar damage made `1 - scardamage / 20` invalid and refused both a new plan and an earlier plan at apply. An independent read still found 90. Repair restored validity and one quarter debit persisted 89.75. The crop field's active apiary supplied baseline 4 for `2.0 / baselineincrease`; a quarter debit persisted stock 9 and prepaid 0.75. Turning off pollination changed baseline to 1 and made the factor invalid: planning and application refused, and independent reads still found stock 9, prepaid 0.75 and revision 1. Restoring pollination permitted one quarter debit, independently observed as stock 9, prepaid 0.50 and revision 2. Y-P01/P02/P03 and the Y-T24 provider rollback/retry probe also passed. The land harness took 34,483 ms.

The fixture substitutes unrelated world catalogues and deterministic weather. C-P03 uses a controllable apiary candidate; other land cases use a configured ecological scalar. The environmental profile, coordinator, cell, field, owner saves, and independent EF/MySQL reads are production paths. The disposable runner verified ownership before deleting both databases and gracefully shutting down and deleting only its temporary MySQL instance. No schema change or migration was needed. No live Telnet session or hosted CI result is claimed by this local run; the hosted result mentioned in the assignment is historical.

## Task 3A correction run (19 September 2026)

Against PR #754 head `f22028b3455a76d6399934bdfb38c0f59a73192d`, the wider agriculture, environmental, forage, SaveManager and gathering filter passed 288/288 in both Debug and Release. The engine built in both configurations. `scripts/test-unit.ps1` passed every listed project: library 505, expression 36, seeder 1379, core 3532, persistence library 58, bot 22, converter 43, web 54 and terrain planner 34. These counts are from the local correction worktree before publication.

The isolated MySQL 8.0.45 harness created and cleaned up owned `futuremud_gather_gc_20260919125851_0e016c5b44` and `futuremud_land_20260919125932_afcbcd1010`. In the land database, C-P01 used saved pending pasture fixtures: field 4 assessed staged 50 at factor 0.5 as 25; field 5 assessed staged 50 at factor 0 as 0. The normal field owner checkpoint saved stock and the versioned assessment marker together, and independent reconstruction observed the assessed result. C-P02 used retained orchard field 6: a prepaid 0.25 debit left opening stock 99 and prepaid fraction 0.75; harvest bonus +5 at factor 0.5 persisted stock 81, prepaid 0.75 and yield progress 0.5; a reconstructed later native tick persisted stock 82 and cleared that progress. The existing Y-P01–Y-P03 and native provider failure/retry probes also passed. Harness fixtures substitute world catalogues, weather and ecological scalar factors; C-P01 stages the new field's pending XML through the fixture seeder rather than invoking the new-field constructor. The real coordinator dynamic-validity path is covered by `EnvironmentalMagicCoordinatorTests.OrganicConversion_DynamicInvalidPenaltyRejectsPlanAndApplyWithoutDebitingStock`; a combined database-backed coordinator C-P03 run remains unexecuted.

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
