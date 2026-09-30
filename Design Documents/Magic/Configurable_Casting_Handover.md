# ARM-02 implementation handover

Implements [ARM02_Configurable_Casting_Implementation_Brief.md](ARM02_Configurable_Casting_Implementation_Brief.md)
on the ARM-01 merge `cacbd2d26057cfdaaabf9532c6d4e210c50f9f10`. The user's implementation
request authorises this brief despite its retained proposal header. See
[Configurable_Casting.md](Configurable_Casting.md) for player, builder, FutureProg and
staff recovery instructions.

## Requirement and acceptance map

| Requirement | Delivered behavior and executable evidence |
| --- | --- |
| Optional capability policy and stable identity | `SkillLevelBasedMagicCapability.Casting*` loads/saves versioned immutable policies and validates explicit admissions, referenced IDs, bounds, duplicates and cycles. `Policy_InvalidIdentifiersDuplicatesAndCycles_ReportsErrorsWithoutMutation` and `OldAndVancianDefinitions_*` cover malformed and old definitions. |
| Clone ownership | Capability clones remap policy/admission/edge GUIDs while retaining shared spell IDs. Native Earth/Sorcerer builders assert distinct local identities. Spell clones rebind triggers/effects to the new spell; the native shared-trait fixture uses the clone. |
| Vancian and independent routes | Configured Vancian enablement refuses. `DirectTrigger_ConfiguredKnownProgIsNotLegacy_IndependentLegacyGrantRestoresRoute` exercises refusal followed by ordinary paid legacy casting without configured acquisition. Existing Vancian/spell-power suites remain acceptance dependants. |
| World-local service and immutable advisory quote | Commands and typed Progs call `MagicCastingService`. Cast re-resolves body, permission, target, numerical configuration, actual plan inputs, combined costs and balances under the canonical owner guard. Quotes cannot be redeemed as payment tokens. |
| Pure knowledge and route queries | `Queries_EmptyReserveAndInactiveCapability_RetainKnowledgeWithoutMutation` proves no writes, checks or grants at zero reserve or without the capability. Knowledge remains visible separately from availability. |
| Explicit, durable acquisition | Staff enrol/grant and authored grant Prog record provenance. Acquisition is written before opening the native skill. `Enrolment_*` and `Grant_PersistenceFailureCannotOpenOrInferSkill_ExplicitRetrySucceeds` cover authority, idempotence and interrupted grants. |
| Shared grade meaning | Spell-level profile version, seven grade mappings, raw proficiency thresholds, difficulty steps, next-grade-only overreach, 1.5 cost multiplier and 25% mastery fixture. Controlled 2/raw 42 permits grade 3 at 22.5 energy; grades 4/7 refuse. |
| Detached route numerics | `NumericalCopies_ParallelRoutesAndGrades_DoNotContaminateCatalogueOrEachOther` covers overlapping routes, different traits, duration, cost, grade scalars, native school and definition reload. Strict evaluation rejects invalid/non-finite results instead of substituting zero. |
| Typed scalar adapter | Only indexed `boost.Bonus` bindings are accepted. Native Stone Skin combines real spell armour and a body-owned agility trait: grades 1/3 yield -1/-3. Invalid index/token/field/context variables disable readiness before payment. |
| One paid native invocation | Native inventory plans retain feasibility, execution, finalisation and input quarantine. Duplicate resource destinations aggregate. `Cast_FailedNativeCheck_*`, duplicate-cost and missing-material tests cover paid failure, retained lockouts and free refusals. |
| Native improvement and canonical deadlines | `NativeImprovementScope_SuppressesOnlyActingCastCheck_AndRestoresOtherChecks` exercises the real standard-check improvement boundary. The service performs one canonical native skill-use attempt at its eligible 60-second opportunity; mastery uses its shared 600-second opportunity. |
| Applied operation reporting | Damage, armour, boost, personal tag ward and blindness removal apply through one path. Delivered wound deltas qualify instantaneous damage. Actual removal qualifies; empty removal, absorbed damage, reportless effects and rejected targets do not prove mastery. |
| Native group behavior | Empty, all-ward-blocked and multiple-applied-target regressions retain native invocation outcomes, paid costs, once-only caster effects and one improvement/mastery opportunity. Existing resistance/group tests remain dependants. |
| Shared canonical skill and second-body acquisition | Character-owned additions/lookups/removals route to the identity; body traits stay physical. Linked language additions/removals reach loaded instances. Native second-body acquisition opens one persisted native skill at 10 and both bodies share it. |
| Canonical configured reserves | Cast, gathering destination quote/credit and passive callbacks resolve the same owner. Native second-body gathering persists canonical credit and acting-body wounds. Failed gathering persistence marks the canonical resource holder dirty for retry. |
| Generator lifecycle | Native detach/restore and focus changes retain one callback and balance. Sorcerer has no passive income. Ownership tests cover authored sleep settings, mixed resource filtering, independent legacy generators, unrelated recipients and an NPC secondary. |
| Bounded prerequisite evaluation | Reverse indexes use changed spell/trait IDs. Explicit enrolment and a current permanent route are required for automatic edges. Shared-trait Earth acquisition does not grant the independent Void branch. Native Wardcraft exercises an Earth prerequisite and Ember Lance a separate per-spell trait. |
| Real command and Prog integration | `MagicCastingIntegrationTests` calls `MagicModule.MagicGeneric` and compiles all five registered typed Progs. Physical restrictions and payment are shared; ambiguity requires `via`. Native command actor permissions remain non-admin. |
| Later-slice exclusion | Practice, formula, quiet and area syntax refuses explicitly. No lessons, scroll learning, manufacturing or wand/staff adapters are added. |
| Durable operation boundaries | Receipts retain owner, actor/body, route, spell/trait/reserve, material IDs, combined costs, stage, deadlines, results and bounded diagnostics. Fault tests cover payment, effect execution, improvement and mastery/result writes. |
| Conservative recovery | No effect replay, automatic refund or new mastery roll. An acknowledged durable sample may advance once; an unwritten sample cannot. Staff can acknowledge malformed receipts without inferring progress. |
| Cross-route/body/input quarantine | Uncertain spell, trait, reserve and material inputs remain blocked. Native restart probes cover an alternate tradition and a second spell sharing trait/reserve while an untouched route remains quotable. Transferred uncertain items remain blocked across identities. |
| Native provider errors | A disposable MySQL trigger rejects `MasterySampleRecorded` with SQLSTATE 45000. The receipt retains its actual inner provider message; recovery retains the proven pre-advance grade. |
| Persistence artifacts | Generated migration `20260927050300_ConfigurableCasting`, designer and model snapshot create four empty tables without converting old knowledge. Native migration/model parity and the seeder blank snapshot complete the persistence release unit. |

The regression fixtures are in `MudSharpCore Unit Tests/MagicCasting*`. Native executable
assertions are in `Temporary Scratch App/GatheringNativePersistenceHarness/Program.Casting*.cs`.

## Verification record

Final verification completed on 27 September 2026. The working tree remained unchanged
during each reported automated run. The final fast gate includes all 47 new ARM-02 tests.
Only this handover and its extracted transcript changed after the final gates.

| Check | Result and retained evidence |
| --- | --- |
| ARM-02 focused regressions | **47 passed**, no failures/skips; run `20260927T062500Z-14c171494560`. All 47 also passed in the final fast gate. |
| Blank snapshot regressions | **9 passed**, no failures/skips; run `20260927T062650Z-b8f8d9f565c2`. Also included in the final fast gate. |
| Repaired legacy compatibility fixtures | **3 passed**, no failures/skips; run `20260927T065042Z-4bc655996ac6`. Exercises Vancian knowledge, independent spell-backed power payment and concrete resource credit/stasis. |
| Final ten-project fast gate | **6,125 passed**, zero failures/skips, complete native TRX and stable source; run `20260927T065116Z-4951a270fc1d`. Core 3,937; seeder 1,394; shared library 524; database library 63; website 54; converter 43; expression engine 36; terrain planner 34; bot 22; reporting 18. |
| EF generation and parity | Migration/designer/model snapshot generated with the repository-matched `dotnet-ef` 9.0.11. `has-pending-model-changes` passed; `.artifacts/configurable-casting/ef-parity.log`. Native schema acceptance independently passed `HasPendingModelChanges()`. |
| Blank snapshot refresh | Full refresh on a separate disposable `fm_snap_*` schema, recorded in `.artifacts/configurable-casting/native-06-snapshot.log`. Manifest: product `3.6.1.0`, latest migration `20260927050300_ConfigurableCasting`, generated UTC `2026-09-27T06:22:44.0636744Z`. That development run's later native fixture failure is not counted as acceptance. |
| Blank snapshot import | Final native run imported the maintained SQL into a new MySQL 8.0.45 database, verified all four casting tables were empty and confirmed no pending migrations/model changes. |
| Native harness build | Debug build passed with **zero warnings and errors**; `.artifacts/configurable-casting/native-build-final.log`. |
| Native command/persistence acceptance | **PASS**, exit 0; `.artifacts/configurable-casting/native-final.log`, completed UTC `2026-09-27T06:53:33Z`. Both non-admin routes, canonical secondary-body casting/acquisition/gathering, retained effects, shared deadlines, five fault stages, provider failure, quarantine/recovery and real Ember Lance/Wardcraft operations passed. Owned database/server cleanup passed. |
| Documentation and diff | Seven local guide/handover/harness links and the transcript's source-log SHA256 passed validation. Tracked diff and new source/document whitespace checks passed. |

Native command/persistence output and operation IDs are preserved in the checked-in
[acceptance transcript](Configurable_Casting_Native_Acceptance.txt). It is separate from
the automated-test TRX records. Raw test receipts, per-project TRX, fingerprints and logs
remain in `.artifacts/test-runs/<run-id>/`. The final full-gate fingerprint was
`008f8f0dff8b72ae2ad914c865b3372dd2a289e6b3ab86420d34920d3a33b7ad` at both start and end.
Fingerprints include the selected project scope; focused and full-gate hashes therefore
need not match each other.

The initial full gate, `20260927T062706Z-f2bbfbc7c320`, recorded 6,120 passes and five
failures. Two were snapshot scratch-directory ACL failures; three were incomplete older
test fixtures (a missing non-generic collection enumerator, and an uninitialised capability
catalogue/generator dictionary). The fixture setup was repaired without weakening any
assertion. The successful rerun used an owned `TEMP`/`TMP` directory under
`.artifacts/configurable-casting`.

Run `20260927T063854Z-93a35190020b` is **INCONCLUSIVE**, with no tests executed: the reporter
blocked while fingerprinting a large Git diff because it reads stdout and stderr serially
and Git emitted line-ending warnings. Only the verified Git descendants were stopped.
The final runs used a process-scoped `GIT_CONFIG_COUNT` entry for `core.safecrlf=false`;
repository/global Git configuration was not changed. This avoids the warning stream;
the reporting helper itself was outside this implementation's scope.

Earlier native development failures are retained in `.artifacts/configurable-casting/`,
including incomplete fixture anatomy/catalogues and omitted native wound-queue flushing.
An initial missing inner provider diagnostic was also corrected. The final native run
uses real living organs and the production save queue and passes the persisted-wound
assertion. Failed, superseded and incomplete runs are not counted as passing acceptance.

Run the native acceptance with:

```powershell
dotnet build 'Temporary Scratch App/GatheringNativePersistenceHarness/GatheringNativePersistenceHarness.csproj' --no-restore -m:1
& '.\Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1' -CastingOnly
```

`-RefreshSnapshot` additionally refreshes the blank snapshot on a separate disposable
`fm_snap_*` database. Build `DatabaseSeeder` first. The script launches a private loopback
MySQL instance, verifies its data directory before shutdown, and deletes only its owned
temporary data. Never point this acceptance at a game database.

## Acquisition cascade correction - 30 September 2026

Implements [ARM02 C1](ARM02_C1_Acquisition_Cascade_Fix_Brief.md). The historical
ARM-02 results above did not cover acquisition ordering. `EvaluateEdges` previously
marked an admission evaluated before its prerequisites were satisfied, discarding a
later attempt after an upstream grant. Removing that call-wide suppression lets newly
eligible admissions complete in the same affected cascade, independent of admission
or queue order. Only successful new acquisitions enqueue downstream entries; existing
acquisitions and rejected candidates produce no further work. Enrolment, applicable
permanent routes, canonical ownership, route-bound thresholds, quarantine, policy
validation and acquisition-before-skill persistence ordering are retained.

`MagicCastingAcquisitionTests` adds 31 service-level cases: reverse-ordered reconciliation
and long enrolment chains; all 24 diamond admission permutations with both prerequisite
orders (48 fresh fixtures); unmet joins; spell/trait/combined notification seeds; exact
grade and raw-skill thresholds; native skill opening and persistence ordering; restricted
and overlapping routes; prerequisite/target/reserve quarantine with an independent
branch; repeated notifications and service restart; unmet prerequisites; cycles and
duplicate/missing prerequisites. Acquisition neither casts nor spends reserves.

| C1 check | Actual result and local receipt |
| --- | --- |
| Pre-fix ordering regressions | **2 failed**, 0 passed/skipped; run `20260930T073056Z-678446aa16ef`. Both `Reconcile_ReverseOrderedChain_AcquiresEntireCascadeInOneCall` and `Enrol_ReverseOrderedLongChain_AcquiresEntireCascadeBeforeReturning` failed their exact acquired-set assertions against unchanged production code. |
| Fixed acquisition suite | **31 passed**, 0 failures/skips; run `20260930T073157Z-61547f4aca7e`, filter `FullyQualifiedName~MagicCastingAcquisitionTests`. Includes both formerly failing ordering regressions. |
| All casting regressions | **78 passed**, 0 failures/skips; run `20260930T073448Z-14364818683d`, filter `FullyQualifiedName~MagicCasting`. Includes the existing enrolled Earth/shared-trait versus unenrolled Void test and ownership regressions. |
| Full core gate | **4,087 passed**, 0 failures/skips; run `20260930T073506Z-2fdf2ab1f327`, unfiltered core suite. |

All four runs have complete counts and stable source. They used the core reporter,
Debug/net10.0, SDK 10.0.401, `-FailOnSkipped` and single-node builds. Base HEAD was
`a79578c8d84121fe50231cb53a6ca8369c7fdb1b` plus the scoped worktree changes. The pre-fix
fingerprint was `cf194627e4fe0257aa43ffe758b82a97c2766fdff28d198e6b6e7ee844c8356e`;
the three fixed runs share `de7bc04459817145b0e06009ba87904ceadb215ca91cfd325b3365e1b5bfab80`
at start and end. Only documentation and line-ending normalization changed after those passing runs; executable behavior remained unchanged.

Receipts, failures, source fingerprints and native logs remain local under
`.artifacts/test-runs/<run-id>/`. Each run's TRX is
`invocations/MudSharpCore_Unit_Tests-8fa3ea0d/net10-41544e89/1/results.trx` beneath that
root. An initial test-mock compilation failure (`20260930T072848Z-584bec461115`)
executed no tests and is not ordering evidence. A separate reporter bootstrap attempt
(`bootstrap-237e88e0bc6a4993b279505e90a6ad5e`) failed because the escalated process could
not access the sandbox-account TEMP directory; no tests ran. The corrected broader
runs used the host user's temp directory. Restore and reporter builds used the existing
local `NuGetAudit=false` workaround; reporter builds also used their established
`NoWarn=NU1902;NU1510` switches. No SDK/package versions or committed warning policy
changed.

This correction changes only the acquisition queue, regressions and documentation.
No new MySQL harness, installed-world Telnet or climate run was performed; persistence
semantics and schema are unchanged, as scoped by the C1 brief. Historical native
acceptance remains historical evidence and does not prove this ordering correction.
## Scope of native evidence

The native harness uses generated MySQL schema, real character resource persistence,
native `Character` skill/resource methods, real `Body`, `Skill`, effects, inventory plans,
health strategies, wounds and the production casting/gathering stores. Separate processes
reload state and retained effects. Check outcomes and mastery randomness are controlled;
the minimal world registry, heartbeat and unrelated catalogues use test doubles. Native
skills use a non-improving model so exact persisted values are deterministic; improvement
attempt suppression is separately tested against `StandardCheck`.

This is command/service and native persistence acceptance. It does not claim a full
installed-world Telnet session, full stock/preset balance, the complete proposed repertoire,
or 04C closure. No production database is migrated. Publication and merge are separate
workflow steps and do not expand this acceptance scope or authorise deployment.
