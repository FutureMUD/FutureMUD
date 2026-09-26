# Bounded land rejuvenation verification

Work began 22 September 2026; final verification resumed 26 September 2026.
Task: `MAGIC-LAND-REJUVENATION-04`.

Authoritative assignment: `04_Bounded_Land_Rejuvenation_Implementation_Brief.md`,
**Task 4 — Bounded land-rejuvenation spell effect**, Revision 2, read from the supplied
`FutureMUD_Task4_Rejuvenation_Handoff_Revision_2` attachment directory.
Starting checkout: `037ac65c3f22359b093f685b838457a5b17871ee`, containing prerequisite PR #763.
Before publication, master `cdf6f79997cc7b5081e51ceee2e1c45cf6adc39a` was integrated to
retain its independently delivered item-description schema. The unpublished Task 4
migration was regenerated against that combined model with the same table-only Up/Down.
The delivery is one implementation PR; it does not include merging, deployment or stock content.

## Delivered contract

The ordinary `rejuvenateland` room/rooms template uses captured trait expressions,
the invocation's resolved positive duration, normal casting costs and the existing
spell parent/child lifecycle. Legacy casts, direct Vancian casts and independent
spell-backed powers share admission. Scroll and substance payloads are explicitly
unsupported. There is no new gathering, concentration, vegetation-healing or refill route.

See [spell authoring](Magic_System_Spells.md#bounded-land-rejuvenation) for the exact
`budget`, `rate`, `eligibility`, `continuation`, `local`, `desc` and `colour` grammar.
Policies require the exact non-static boolean `(character, location)` signature,
including refusal of wildcard parameter lists and broader compatible types.
Expressions use the casting trait and `SpellDuration` bonus context; `power` and
`outcome` retain their invocation enum integer meanings. Extra Vancian variables
come only from the existing numerical invocation wrapper.

The profile's `magicalrepaircap <non-negative-number|none>` is independent of natural
`repair`. Older profiles default to `none`. The documented slow example uses cap 1.
One physical cell has at most one active or unresolved treatment. Target admission
precedes all target child construction and exclusive cleanup, preserving the old
parent and unrelated siblings on rejection. Costs remain per invocation.

`LandRejuvenationTreatments` is the authoritative progress store. Its versioned typed
checkpoint contains captured identity, rate, budget, lifetime, earned work, status,
sequence, cancellation intent and the exact pending ecological request. Child XML
holds the treatment reference, not an independently restorable budget. The prepared
request is persisted first; the existing ecological transaction then commits its
receipt, scars, resource balances and confirmed progress together. Optimistic revision
checks prevent stale checkpoints from overwriting confirmed work. Reconciliation
adopts durable evidence without repair, policy execution, foreign acknowledgement or
reviving a cancelled child. Only the timed lane may retry an exactly identified,
proven rolled-back request.

The coordinator's fifth indexed lane shares its original heartbeat and count/time
budgets. Online monotonic time starts after attachment/load. Profile edits close the
old-rate segment; unknown policy intervals are discarded conservatively. Normal
expiry clips and accounts its final interval once. Explicit removal and failed
maintenance discard uncommitted time. Zero scars terminate before later damage.
When natural settlement reaches zero inside a new damage operation, durable
termination is confirmed first. A failed termination save refuses that operation
without consuming the natural sample; retry cannot reuse the ended treatment.

`ConservativeScarRepair` rounds the remaining scar upward by one representable step
when subtraction would exceed the allowance. Reported and charged repair is exactly
the representable explicit decrement after natural settlement. Sub-ULP work is
bounded by remaining budget and lifetime, carried to later visits, and creates no
ineffective receipt. Checkpoint conservation validation allows only floating-point
rounding tolerance (`max(double.Epsilon, initialBudget * 1e-12)`); this is not a minimum
spell rate or a grant of extra repair.

## Execution receipts

All runs below are Windows-native using the checked-out .NET 10 projects. The native
database was an owned loopback MySQL 8.0.45 instance, never a game database.

| Check | Actual result and local evidence |
| --- | --- |
| Focused rejuvenation run 9 | PASS: 60/60, no skips, native and shell exit 0. `.artifacts/rejuvenation-focused-9/rejuvenation-focused-9.trx` and its adjacent log. |
| First full fast gate | 5,806/5,808 passed, no skips; all 3,657 core cases passed (including 61 rejuvenation rows). Two existing snapshot-helper tests hit shared `%TEMP%/FutureMUD-SnapshotRefresh` ACL denial. Stable-source run `20260926T093611Z-e3c333bf89c9`; `.artifacts/rejuvenation-fast-final.log`. Final run uses a fresh owned temp root. |
| Final focused rejuvenation run 10 | PASS: 65/65, no skips, native/shell exit 0. Includes natural-zero-before-damage and failed termination-save/retry rows. `.artifacts/rejuvenation-focused-10/rejuvenation-focused-10.trx`. |
| Final integrated full fast gate | FAIL: 5,843/5,844 passed, no skips/inconclusive rows; source stable. All 3,691 core, 1,380 seeder and 62 persistence cases pass. Only `TitleCase_CapitalisesWords` fails: expected `Earth's Sky`, actual `Earth'S Sky`. Run `20260926T100816Z-fc15261c204d`, native/shell exit 1; `.artifacts/rejuvenation-fast-integrated.log`. |
| Isolated master failure reproduction | Same one-test failure reproduced on clean master `cdf6f79997cc7b5081e51ceee2e1c45cf6adc39a`, without any rejuvenation changes. Test/implementation match master. Native/shell exit 1; `.artifacts/rejuvenation-titlecase-master/titlecase-master.trx` and `.artifacts/rejuvenation-titlecase-master.log`. No unrelated helper repair or assertion change. |
| Debug/Release engine | PASS, both zero warnings/errors and exit 0. Debug rebuilt through the final native harness build (`.artifacts/rejuvenation-native-14-build.log`); targeted Release `.artifacts/rejuvenation-release-final.log`. |
| EF model parity | PASS: no pending model changes with generated migration `20260926094429_LandRejuvenationTreatments`; `.artifacts/rejuvenation-integrated-parity.log`. |
| Focused blank snapshot tests | PASS: 8/8, no skips, native/shell exit 0. `.artifacts/rejuvenation-snapshot-final/rejuvenation-snapshot-final.trx`. |
| Native run 10 | PASS: R-P01–R-P06, direct Vancian/power probe, refreshed-snapshot import; `nativeHarnessExit=0`. `.artifacts/rejuvenation-native-10.log`. |
| Snapshot/native run 11 | PASS: all native probes, native and shell exit 0, owned cleanup complete. `.artifacts/rejuvenation-native-11.log`. Full snapshot refresh/import preserves `utf8mb4_0900_ai_ci`; manifest UTC `2026-09-26T09:34:28.4231551Z`. |
| Integrated upgrade/native run 12 | PASS: imported master migration `20260922124142_AddGameItemDescriptionOverrides`, upgraded to `20260926094429_LandRejuvenationTreatments`; all native probes, exit 0 and cleanup. `.artifacts/rejuvenation-native-12-upgrade.log`. |
| Integrated fresh/native run 13 | PASS: full migration-chain refresh, import already at `20260926094429_LandRejuvenationTreatments`, all native probes, exit 0 and cleanup. `.artifacts/rejuvenation-native-13-fresh.log`; manifest UTC `2026-09-26T09:51:16.6090764Z`. |
| Final runtime/native run 14 | PASS: R-P01–R-P06 plus direct Vancian/power probes on the final runtime including the natural-zero boundary fix. Native/shell exit 0; disposable database and owned MySQL instance removed. `.artifacts/rejuvenation-native-14-final.log`. |

Earlier failures are retained locally, not counted as acceptance. They include
fixture compile/setup errors, a wrapper bootstrap `CS2012` access denial, and a run
with 49 passing TRX assertions but contradictory shell exit 1. Run 9 closed that exit
discrepancy with explicit native/shell zero. Native setup failures exposed an overly
long disposable database name and a shared temporary-directory ACL; the harness
now uses a short unique database name and owned `TEMP`/`TMP` for snapshot export.
The final fast gate also used a fresh owned `TEMP`/`TMP`, clearing both prior snapshot
ACL failures. Its compiler-held temporary analyzer files could not all be removed;
that unit-test temp directory remains locally. Native database/server cleanup succeeded.
The full gate remains red solely for the independently reproduced master title-case
failure; it is not represented as a passing gate. These runs do not establish hosted CI status.

## Automated requirement map

The named methods are in
[`LandRejuvenationTests.cs`](../../MudSharpCore%20Unit%20Tests/LandRejuvenationTests.cs).
Rows refer to assertions, not merely test categories. The malformed textual-version
row was added after run 9 and is included in the final focused run and full gate.

| ID | Executable evidence and asserted boundary |
| --- | --- |
| R-T01 | `Template_FactoryBuilderCloneAndCompatibility_ArePureAndExplicit`, `Admission_InvalidValuesDoNotAttachOrRepair`, policy tests: builder/load factories, exact template discriminator, clone/XML/show/help, room compatibility, malformed version/formula/policy, zero/negative/non-finite values and overflow. |
| R-T02 | Template/admission tests, parent reload and scheduling inspection: no repair, resource refill or treatment registration from construction/help/inspection; load waits for attachment. |
| R-T03 | `Treatment_ExampleA_RepairsTwoThenDispelPreservesCompletedWork` and both callback orders of `Treatment_ExampleB_FinalHalfMinuteIsAccountedOnce`: 2 units in 120 seconds at cap 1; exactly 2.5 units over 150 seconds. |
| R-T04 | Examples, precision and `NaturalRepair_IsNotSpentFromBudget_AndZeroTransitionEndsBeforeNewDamage`: actual explicit work respects all four bounds, while natural recovery spends no budget. |
| R-T05 | `Admission_DifferentSpellAndCasterCannotResetExistingBudget` plus native R-P05: cross-spell/caster conflict cannot stack or reset progress. |
| R-T06 | `CastSpell_ExclusiveAndCompositeConflict_PreservesOldParentAndSiblings`: same parent, treatment and sibling survive; no new composite sibling is invoked. |
| R-T07 | Example A, continuation and unresolved-dispel tests: no cancellation burst or reversal of completed scars; registrations end. |
| R-T08 | `ParentChild_XmlReload_UsesAuthoritativeBudgetAndFreshOnlineEpoch`, `SaveBetweenVisits_CheckpointsEarnedWorkWithoutRepair_ThenRestoresFinalInterval`, native R-P03: actual parent/child XML, retained budget/lifetime, fresh epoch, no replay. |
| R-T09 | Natural/explicit zero tests and native R-P05: old treatment is terminal before fresh real Land damage. |
| R-T10 | `Repair_InvalidUnrelatedOutputAndNoField_StillRepairsWithoutRefill`, `Repair_ClosesOldProductionSample_AndLaterRegenerationUsesRestoredCapacity`: cap 80→85 with mana 30 unchanged, then later ordinary regeneration reaches 35. |
| R-T11 | Native R-P02/R-P06: actual Land, persistent timed repair, unchanged consumed crop/prepaid stock/history, and later native forage recovery. |
| R-T12 | `CastSpell_RepeatedRoomAndIndependentTargets_PaysOnceAndInstallsPerCell` and native direct command probes: distinct cells, one invocation payment, real non-admin legacy/Vancian cost and slot semantics. |
| R-T13 | Template compatibility assertions, existing scroll/substance suites and native direct Vancian probe: payload rejection does not disable direct casting. |
| R-I01 | Three rows of `Treatment_FullZeroCapacityAndDormantCells_AdvanceWithoutResourceProduction`: full, zero-capacity and zero-rate cells repair exactly 1 while resource balances stay unchanged. |
| R-I02 | Invalid-output test, `Treatment_InvalidOrganicDefinition_DoesNotBlockRepair`, `Admission_InvalidRepairPolicyOrScalarState_PreservesEvidenceWithoutInstallation`: unrelated output/organic errors and absent field permit repair; malformed repair cap/NaN scalar refuse without erasing evidence or installing work. |
| R-I03 | Three `ProfileCeiling_RoundTripsIndependentlyFromNaturalRepair` rows plus cap-edit test: none/zero/positive round-trip, malformed cap is repair-specific, zero ends work without erasing scars. |
| R-I04 | `ProfileCap_EditIsProspective_AndDisableIsTerminal`, both `Expressions_CaptureCastingTraitAndSustainedBonusContext` rows: cap increase/decrease split old/new segments; later template/trait edits cannot change captured values; direct Vancian wrapping included. |
| R-I05 | Both `Binding_DisableOrReplaceThenRestore_DoesNotReviveTreatment` rows: disabled or different effective profile ends work, even after restoring the original binding. |
| R-I06 | Example B and saved-between-visits test: delayed aggregate, final half-minute, duplicate pump/expiry and either callback order apply each segment once. |
| R-I07 | Example A and B: explicit dispel discards an unprocessed half-minute; normal expiry accounts it. |
| R-I08 | `Caster_IndependentLogoutContinues_LocalQuitCancelsWithoutLoading`, native reader: independent logout/creator absence continues without `TryGetCharacter`. |
| R-I09 | Six local-loss rows, logout test and `Continuation_FalsePolicyEndsRemoteTreatment`: cell/layer/plane, death, stasis, acting-instance replacement, logout and policy failure terminate. |
| R-I10 | `Policy_BuilderLoadAndEvaluationRejectInexactOrInvalidSignature`, continuation tests: exact signatures, wildcard/type/cache/compile rejection, false result and read-only reentrant mutation fail safely. |
| R-I11 | Parent reload/save tests: constructor does no work, parent attachment precedes activation, duplicate callbacks do not register twice. |
| R-I12 | `ReloadMissingSource_TerminatesPersistedSlot_WithoutRepair`, `DuplicateLoadedParentIdentity_IsIdempotent`, malformed progress rows: orphan, unsupported/textual version, contradictory budget and pending state cannot create fresh work; unresolved records remain inspectable. |
| R-I13 | `Precision_SubUlpWorkAccumulatesWithoutReceipts_AndTinyOrdinaryRepairIsValid`: at scar `1e12`, `1e-6` work creates no receipt/decrement until representable accumulation; ordinary tiny repair remains valid. |
| R-I14 | Precision matrix over near-zero, ordinary and large scars with epsilon/near-budget allowances: exact observed decrement is non-negative and never exceeds allowance. |
| R-I15 | Both atomic-step unit rows, natural-zero retry regression and native R-P04: rollback leaves no partial durable progress; a retry whose scars naturally vanish creates no magic receipt. |
| R-I16 | Atomic-step and `FinalCommittedRepair_LostAcknowledgementRemainsCompletedAfterConfirmation`, native R-P04: same ID, one budget debit, coherent terminal cache/store on final repair. |
| R-I17 | `ForeignPendingOperation_IsNeverAcknowledged_AndBlockedTimeEarnsNothing`: no foreign acknowledgement, no earned work during quarantine and no paused-time burst. |
| R-I18 | `Dispel_UnresolvedRollback_CancelsWithoutRetryOrResurrection`, native R-P04: cancellation survives uncertainty and confirmation never reinstalls the child. |
| R-I19 | Natural-zero transition/retry tests, both rows of `NaturalZeroThenDamage_BeforePump_TerminatesOldTreatment` and native zero-then-Land probe: ended treatment cannot repair later scars, including before a normal pump; failed termination persistence refuses new damage and preserves its sample for retry. |
| R-I20 | `RepairCommit_RejectsNestedSameCellLandDebit_AndAllowsOtherCell`, `LandDebit_RejectsNestedRepair_WithoutBlockingAnotherCell`: both mutation directions refuse same-cell reentry without partial debit; another cell remains independent. |
| R-I21 | Exclusive/composite and repeated-room tests: preserve old siblings, one child per distinct cell and one ordinary payment per invocation. |
| R-I22 | `Scheduling_ThirtyThousandCells_VisitsOnlyIndexedTreatmentsAndInspectionIsPure`: forbidden cell/field enumeration, zero recurring treatment reads/visits with no treatments, one existing heartbeat. |
| R-I23 | Scheduling and `Dispose_RemovesActiveWorkAndHeartbeatWithoutCrossWorldReferences`: one indexed active visit among 30,000 cells; visit budget respected; unregister/dispose clear work and heartbeat; a fresh world advances independently. |
| R-I24 | Description/inspection, malformed progress, atomic confirmation and foreign-operation tests: stable scars, counts and IDs; inspection does not execute continuation or load the creator. |

## Native persistence and command evidence

Maintained executable harness:
[`Program.Rejuvenation.cs`](../../Temporary%20Scratch%20App/GatheringNativePersistenceHarness/Program.Rejuvenation.cs),
[`Program.RejuvenationVancian.cs`](../../Temporary%20Scratch%20App/GatheringNativePersistenceHarness/Program.RejuvenationVancian.cs).

| Probe | Observed assertions in passing native runs 10–14 |
| --- | --- |
| R-P01 | Real builder editor configures profile cap 1 and spell budget 12/rate 2/duration 600/cost 0.25; DB source reloads; non-admin `MagicGeneric` room cast pays and attaches without repair. |
| R-P02 | Real Land creates 20 scars. A timed step yields scar 19, budget 11, total repaired 1 and acknowledged sequence 1 in a fresh MySQL context before global save. Crop stock 9/prepaid 0.75, casting/gathering payments, pressure and destructive timestamp remain correct. |
| R-P03 | Separate reader processes reconstruct stale parent XML against current authoritative rows without a saving shutdown. Active record loads at scar 19/budget 11 then advances once to 18/10. Dispelled record loads and remains 19/11. Creator is absent and never auto-loaded. |
| R-P04 | Owned MySQL `BEFORE UPDATE` trigger raises a provider error: fresh contexts see rollback of scars/progress/receipt. Staff confirm performs no repair; timed exact-ID retry changes 19→18/budget 12→11. Separately, a wrapper throws **after the real transaction commits**: fresh contexts see 17/budget 10; confirmation adopts it once, including terminal cancellation. |
| R-P05 | Actual same/different source spell recasts preserve the original parent. Ordinary `DispelMagicEffect` stops future work and writes cancellation. Explicit zero followed by a real new Land action leaves old treatment terminal and 20 new scars untouched. |
| R-P06 | Capacity 80→81 while mana stays 30; native crop stock/prepaid fraction unchanged. Forage remains 99.5 at repair and later native `YieldTick` reaches 100 with the improved factor. Independent creator absence works in reader; local mode ends without loading the absent caster. |

The Vancian extension executes actual non-admin command routes with a compiled
capability policy, prepared repertoire and two slots. Slot 1 is spent, slot 2 stays
prepared; invocation bindings capture budget 13/rate 1 and pay 0.25. A separately
granted spell-backed power pays 0.25 without spending slot 2.

Substitutions are deliberate and bounded: the harness supplies world registries,
deterministic checks/weather and monotonic time, a minimal body-prototype plane, and
catalogue fixtures. It uses actual Character, Body, Cell, agriculture/forage owners,
spell/command/Vancian/effect paths, save manager, EF and MySQL transaction/receipt
stores. It is not a full Telnet/server boot or a production load benchmark. Ten
bounded pumps at the same clock instant allow the default soft time budget to visit
the cold treatment lane; they grant no additional elapsed time. Unit fake-store
faults are separate from the real provider trigger and after-commit response-loss probe.

## Deterministic slow-restoration demonstration

Example A is executable: scar 20, budget 12, captured rate 2/minute, profile cap
1/minute, duration 600 seconds. Installation changes nothing. After 120 online
seconds scar is 18 and remaining budget 10. Dispel 30 seconds later preserves scar
18, discards the unprocessed half-minute and removes active work. Advancing another
600 seconds repairs nothing. Example B verifies the final half-minute separately:
budget 2.5/rate 1/duration 150 repairs exactly 2.5, in either expiry/pump order.

## Scheduling and schema evidence

The structural 30,000-cell test records zero treatment visits and zero additional
treatment reads for the no-treatment heartbeat. After one installation it records
exactly one treatment visit and one unit repaired; global cell/field enumeration is
configured to throw. Total visits stay within `MaximumCellVisits`. Disposal removes
the one existing second-heartbeat subscription. These are work-count assertions,
not a production throughput guarantee.

Migration `20260926094429_LandRejuvenationTreatments` was generated with `dotnet-ef`
9.0.11 matching checked-out EF packages. Its Up creates only the checkpoint table
and cell/status index; Down drops that table. Model, designer and EF snapshot are
aligned. Earlier native runs independently upgraded the prerequisite snapshot
`20260920025846_LandGatheringSourceAccounting` to the original unpublished migration.
The final integrated schema is checked again against the newer master snapshot. The full refresh
replays the real migration chain and folds the older appended deltas into the dump;
the maintained MySQL lower-case table naming and existing default collation are retained.

Reproduction commands from repository root (serial builds, no shared mutable run):

```powershell
dotnet test 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -c Debug -m:1 --no-restore --filter 'FullyQualifiedName~LandRejuvenationTests'
dotnet build 'Temporary Scratch App/GatheringNativePersistenceHarness/GatheringNativePersistenceHarness.csproj' -c Debug -m:1 --no-restore
& 'Temporary Scratch App/GatheringNativePersistenceHarness/Run-IsolatedAcceptance.ps1' -RejuvenationOnly -RefreshSnapshot
scripts/test-unit.ps1 -OutputMode Compact -TimeoutSeconds 1800
dotnet build MudSharpCore/MudSharpCore.csproj -c Release -m:1 --no-restore
dotnet ef migrations has-pending-model-changes --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build
git diff --check
```

The isolated script verifies ownership before cleanup and removes its disposable
database and MySQL data directory. Credentials and raw transient logs remain local.
