# ARM02 C1 acquisition cascade ordering fix

## Implementation request

Fix automatic spell acquisition so a valid prerequisite cascade completes in one evaluation, regardless of admission or work-queue order. Add deterministic runtime regression tests, retain the existing acquisition restrictions, and update the relevant casting documentation. Keep the change focused on the acquisition work queue. Do not add a new progression system, schema migration, scheduler, or whole-world scan.

This brief is ready to use as a standalone Codex implementation task. Re-read the current checkout and its applicable instructions before editing. If the reported defect has already been fixed, verify the regression coverage and report that instead of reintroducing a parallel solution. Commit or publish only when the invoking task authorizes it; do not merge or deploy.

## Verified baseline

- Repository: `FutureMUD/FutureMUD`
- Branch inspected: `master`
- Commit: [`385bffa41be7bb65e06b19463c59ac415e28afc4`](https://github.com/FutureMUD/FutureMUD/commit/385bffa41be7bb65e06b19463c59ac415e28afc4)
- Source review date: 30 September 2026 UTC
- Finding: still present in `MudSharpCore/Magic/Casting/MagicCastingService.Acquisition.cs`, in `EvaluateEdges`

This is a source-verified defect and implementation plan. No implementation or new regression run was performed while preparing the brief. The historical passing runs in the ARM-02 handover do not cover this ordering defect.

## Read first

Follow these repository instructions and references:

- [Repository instructions](../../AGENTS.md)
- [Core runtime instructions](../../MudSharpCore/AGENTS.md)
- [Core unit-test instructions](../../MudSharpCore%20Unit%20Tests/AGENTS.md)
- [Verification and documentation map](../../.codex/references/verification-and-docs.md)
- [Test execution and receipts](../../.codex/references/test-execution.md)
- [Configurable casting guide](Configurable_Casting.md), especially prerequisite acquisition and staff acquisition
- [ARM-02 handover](Configurable_Casting_Handover.md), especially bounded prerequisite evaluation and the limits of historical acceptance

The relevant implementation and tests are:

- `MudSharpCore/Magic/Casting/MagicCastingService.Acquisition.cs`: indexing, explicit enrolment/grants, progress notifications, cascade evaluation
- `MudSharpCore/Magic/Casting/MagicCastingService.Resources.cs`: `Reconcile` and permanent-route restoration
- `MudSharpCore/Magic/Casting/MagicCastingService.cs`: canonical owner, guard, acquisition lookup and quarantine
- `MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.Casting.cs`: immutable policy loading and prerequisite validation
- `MudSharpCore Unit Tests/MagicCastingFixture.cs`: `MagicCastingFixture`, `CastingMemoryStore`, controllable skill values and write counts
- `MudSharpCore Unit Tests/MagicCastingIntegrationTests.cs`: `SharedTraitAndPrerequisites_AcquireOnlyTheEnrolledPermanentRouteEdges`
- `MudSharpCore Unit Tests/MagicCastingTests.cs`: enrolment idempotence, persistence-failure safety and invalid-policy tests

At the inspected revision, core and core-test projects target `net10.0`; core tests use MSTest and Moq. Use the versions declared by the actual checkout. No new package is needed.

## Current behavior and root cause

`EnsureIndex` builds reverse indexes from prerequisite spell IDs and their route-bound trait IDs to `(Capability, Spell)` admissions. `NotifyProgress` seeds a queue from the changed spell and/or trait. Enrolment and permanent-route reconciliation can also seed admission lists. After a successful new acquisition, `EvaluateEdges` queues the admissions dependent on that spell.

The evaluation has a call-local `evaluated` set. It executes `evaluated.Add(entry)` immediately after dequeuing, before checking whether the admission's prerequisites are satisfied. An admission that fails because an upstream spell is not yet acquired remains marked for the entire call. If a later grant queues it again, the set discards that second attempt.

For example, let A already be acquired and let B require A and C require B. Start evaluation with `[C, B]`:

1. C enters `evaluated`, then fails because B is absent
2. B is granted, which queues C
3. C is skipped because it is already in `evaluated`

C remains unacquired until another eligible evaluation happens. A diamond has the same problem when its join is evaluated before the final branch is acquired. The defect is observable through public service operations; it does not require concurrent mutation.

There is an important regression-test trap: `Enrol` invokes `Reconcile` and then directly invokes `EvaluateEdges`. Two separate passes can accidentally hide a short-chain defect. A single-call `Reconcile` test or a sufficiently long reverse-ordered enrolment chain is required.

## Required semantics

1. A failed prerequisite check is provisional for the current cascade. A later successful grant that affects that admission must allow it to be reconsidered.
2. Valid chains and all-prerequisite joins reach the same acquired-spell set for every admission/seed order, within the affected reverse-index work. Assert the acquired set, not incidental grant order or which of two eligible routes supplies provenance.
3. Only a successful new grant schedules its downstream spell edges. Do not continually requeue blocked admissions, poll until something changes, or retry persistence failures in a tight loop.
4. Each canonical identity/spell is acquired once. Repeated routes, notifications and queue entries retain the original acquisition, grade, provenance, native proficiency and opportunity deadlines.
5. Keep current permanent-route selection, canonical ownership and guard/re-entrancy behavior. The fix must not change which body can supply the qualifying permanent capability.
6. Preserve explicit enrolment, enabled valid policy, current capability, applicable permanent capability merit, explicit admission and all prerequisite checks. Native skill presence by itself does not grant knowledge. Temporary effects alone do not enable automatic acquisition.
7. Preserve prerequisite minimum controlled grade, raw proficiency through that prerequisite's binding in the selected route, and quarantine checks. Preserve `GrantCore` validation and its acquisition-before-native-skill persistence ordering.
8. Preserve rejection of invalid policies, including cycles and duplicate/missing prerequisites. Do not make cyclic definitions usable as part of this fix.
9. Acquisition must not cast, charge reserves, roll checks/mastery, create cast receipts, or reset existing proficiency/deadlines.

These requirements concern the current affected-admission cascade. Do not expand the task into unrelated trait-notification delivery, configuration invalidation, profile-version migration or persistence recovery changes.

## Minimal implementation approach

Remove the call-wide rule that an admission may be attempted only once. The smallest valid fix removes the permanent `evaluated` suppression and retains the existing queue and `result.Changed` downstream scheduling. Already-acquired spells are already skipped before `GrantCore`.

If de-duplication is useful, track only entries currently pending in the queue:

- De-duplicate initial seeds while preserving their supplied order
- Remove an entry from the pending set when it is dequeued
- Add downstream entries only if they are not already pending
- Allow an entry that was processed unsuccessfully to be enqueued after a later grant

Use one of those approaches, not both layers of bookkeeping. An explanatory comment should distinguish queue de-duplication from final eligibility. Do not solve the bug by sorting spell IDs, relying on `HashSet` iteration order, imposing a global topological sort, clearing all state on each pass, or introducing an arbitrary retry limit.

Termination follows from the existing mutation rules: the seed list is finite; each newly acquired canonical spell causes downstream enqueueing once; already-acquired or rejected candidates produce no further work. Even without pending de-duplication, the number of enqueues is bounded by initial seeds plus the outgoing indexed entries of successfully acquired spells. Cycles remain configuration errors. A failed candidate should stop naturally when no successful grant can change its prerequisites.

Keep changes primarily in `MagicCastingService.Acquisition.cs`, with a focused test file and small documentation updates. No interface, database model, migration, seeder, or paid-casting change is expected.

## Regression test specification

Add `MudSharpCore Unit Tests/MagicCastingAcquisitionTests.cs`, using the existing fixture and `MudSharp_Unit_Tests` namespace. Prefer service-level behavior tests over source-text assertions, reflective invocation of `EvaluateEdges`, or a new production test API.

### Fixture setup

Create the additional spells through `MagicCastingFixture.NewSpell` and configure valid grade profiles with `grades fixture`. Construct ordered policy XML with `NewCapability(..., casting)` or use the existing builder surface. XML is particularly useful when exact admission order is the input under test.

- Use spell 1 as A; use distinct spell IDs for the remaining nodes
- Ensure every spell uses the designated cost resource, each admission/key is valid and each prerequisite references an admission in the same capability
- Make only A a starting spell; `MagicCastingFixture.Admission` defaults `starting` to true, so explicitly set every non-root to false
- Give ordinary cascade edges minimum grade 1 and attainable raw proficiency, such as 0; use specific higher thresholds in boundary tests
- Add the test capability to `ActiveCapabilities`; supply an `IMagicCapabilityMerit` with `Applies(actor) == true` and that capability in its `Capabilities`
- Isolate unrelated default capabilities where useful. Assert `CastingConfigurationErrors()` is empty before testing a valid graph
- For a single reconciliation test, seed the legitimate existing `CastingEnrolment` in `CastingMemoryStore`, seed A with `Acquire(1)`, then make the first `Reconcile(actor)` call. Do not enrol or reconcile earlier and accidentally complete/cache the route
- For public notification tests, establish enrolment while the root is still below a required grade or skill threshold, then update the fixture's durable root grade or raw skill and call `NotifyProgress` once
- Do not use sleeps, random graph generation, wall-clock timing, or unspecified collection iteration as the only way to reproduce the defect

### Required tests

1. `Reconcile_ReverseOrderedChain_AcquiresEntireCascadeInOneCall`

   Use A -> B -> C -> D, with admissions ordered D, C, B, A, a seeded enrolment and only A acquired. Invoke `Reconcile` once. Assert A/B/C/D are acquired, each new acquisition is grade 1 with the correct profile version, and no spell is written twice. This is a deterministic pre-fix failure: the ordered initial seed list places each dependent before its prerequisite.

2. `Enrol_ReverseOrderedLongChain_AcquiresEntireCascadeBeforeReturning`

   Use A -> B -> C -> D -> E, admissions E, D, C, B, A, only A marked starting, and no prior acquisition/enrolment. Invoke `Enrol` once. Assert all five acquisitions and the completed starting-grant version. Five nodes deliberately prevent the existing pair of evaluations from masking the defect. Repeat enrolment and assert no new acquisition or skill mutation.

3. `Reconcile_DiamondAdmissionPermutations_AcquiresJoinOnce`

   A unlocks B and C; D requires both B and C. Use all 24 admission permutations, or an equally explicit exhaustive four-node permutation helper. Seed A and enrolment, then reconcile once for each fresh fixture. Every case must acquire B/C/D exactly once. Include D-first cases and vary the order of D's two prerequisite entries. Add a negative case where one branch's grade/proficiency remains below threshold: D must stay absent even if the other branch succeeds.

4. `NotifyProgress_SharedTraitAndSpellSeeds_CompleteAffectedCascade`

   Exercise `spellId`, `traitId`, and both arguments with overlapping candidates. Use a chain and a join whose candidates may already be in the initial affected set. Vary authored admission order. Assert the same complete reachable acquisition set after one notification, with no duplicate acquisitions. The deterministic reconciliation tests above remain the direct red test; this coverage must not assume a documented `HashSet` iteration order.

5. `NotifyProgress_PrerequisiteThresholdsAndBindings_GrantOnlyWhenAllSatisfied`

   First verify absence below minimum controlled grade, then below minimum raw proficiency, and finally acquisition exactly at both thresholds. Configure a prerequisite's trait override distinct from the default/shared trait. A high default or another route's trait must not satisfy that edge. Include a chain where granting an upstream spell opens its missing native skill at the profile's opening value and the dependent requires that value. Check the grant is persisted before the new skill is added, using existing fixture instrumentation where necessary.

6. `NotifyProgress_RestrictedRoute_DoesNotAcquireBranches`

   Parameterize or separate cases for no enrolment, disabled policy, capability unavailable to the selected body, missing permanent merit, non-applicable merit, and temporary-capability-only availability. Start with acquired prerequisites and sufficient skills, notify once, and assert no branch acquisition or unrelated mutation. Retain the existing integration test proving the enrolled Earth/shared-trait branch does not grant the unenrolled independent Void branch. Include overlapping routes admitting the same spell and verify one canonical acquisition.

7. `NotifyProgress_QuarantinedPrerequisiteOrTarget_DoesNotAcquire`

   Use valid unresolved `CastingOperation` receipts, following the existing quarantine tests. Independently cover a prerequisite spell/trait and the destination spell/trait or reserve blocked by `GrantCore`. Assert the cascade stops at the blocked point while proven unrelated branches can proceed. Keep quarantine behavior unchanged; do not add automatic recovery.

8. `NotifyProgress_RepeatedAndRestartedCascade_PreservesExistingState`

   After successful acquisition, snapshot acquisition records/versions, existing skill values, store writes, flushes, reserves and opportunity records. Send duplicate notifications and restart the service with `f.Restart()`, then notify again. Assert no additional grants or resets, no cast checks/mastery samples/payment, and the original acquisition/provenance/deadlines remain intact. Verify a permanently unmet prerequisite leaves the queue without additional writes or an endless retry.

9. `NotifyProgress_InvalidCycle_DoesNotAcquireOrLoop`

   Build a multi-node cycle and a self-cycle as invalid authored policies, seed enrolment and relevant prerequisite state directly for the test, then notify. Assert validation reports a cycle and no new acquisition occurs. Preserve `Policy_InvalidIdentifiersDuplicatesAndCycles_ReportsErrorsWithoutMutation`; also keep duplicate/missing prerequisite rejection intact. Do not bypass production configuration validation to make a cyclic graph grant spells.

Use `[DataTestMethod]`/`[DataRow]` or small deterministic helpers to avoid duplicating setup. Add assertion messages that identify the graph/order/restriction. Prefer checking exact acquired IDs and record versions over depending only on `Store.Writes`, since enrolment and other setup also write.

## Validation plan

Run from the repository root and keep tested inputs unchanged during each run. Read the checked-out scripts before using their flags. Restore only if required:

```text
dotnet restore "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" -m:1 -p:RestoreBuildInParallel=false
```

On POSIX, use the repository reporter:

```bash
bash ./scripts/test-unit-core.sh --output-mode compact --filter 'FullyQualifiedName~MagicCastingAcquisitionTests' --fail-on-skipped
bash ./scripts/test-unit-core.sh --output-mode compact --filter 'FullyQualifiedName~MagicCasting' --fail-on-skipped
bash ./scripts/test-unit-core.sh --output-mode compact --fail-on-skipped
git diff --check
```

Windows equivalents:

```powershell
.\scripts\test-unit-core.ps1 -OutputMode Compact -Filter 'FullyQualifiedName~MagicCastingAcquisitionTests' -FailOnSkipped
.\scripts\test-unit-core.ps1 -OutputMode Compact -Filter 'FullyQualifiedName~MagicCasting' -FailOnSkipped
.\scripts\test-unit-core.ps1 -OutputMode Compact -FailOnSkipped
git diff --check
```

The core wrapper performs a targeted no-restore build before its no-build/no-restore test stage. The focused casting filter includes fixture-dependent casting, integration and ownership tests. The final core pass covers runtime dependants without defaulting to the full solution or the opt-in climate suite. Use the existing compact receipt workflow for substantial runs; retain the run ID, counts, source fingerprints and TRX locations. A direct targeted build, if needed for diagnosis, is `dotnet build MudSharpCore/MudSharpCore.csproj -c Debug --no-restore -m:1`.

Before the production change, run the new deterministic reverse-order regression to establish the expected failure. Then apply the minimal queue fix and rerun the focused tests and final core gate. Restore/build problems, zero selected tests, skipped tests and incomplete receipts must be reported accurately. Do not count them as passes or change SDK/package versions simply to get a green run. The verification reference permits local audit-access workarounds when relevant; disclose any used.

A new MySQL harness run, migration or whole installed-world session is not required for this local queue change with unchanged persistence semantics. If implementation expands into persistence or native ownership behavior, stop and justify the additional scope and checks rather than claiming the old native acceptance proves it.

## Documentation and completion

Update the prerequisite paragraph in `Configurable_Casting.md` to state that newly acquired prerequisite spells are followed through the affected cascade independent of authored ordering, while retaining all enrolment/permanent-route/threshold restrictions. Add a short dated correction and the actual new verification receipt to `Configurable_Casting_Handover.md`. Preserve its historical results and evidence limits.

Completion requires:

- A focused queue change that permits newly eligible entries to be revisited
- A demonstrated pre-fix regression and post-fix pass for the deterministic reverse-order case
- Passing chain, join, notification, restriction, idempotence and invalid-cycle coverage
- Actual focused and core verification receipts, or an explicit account of blockers and unexecuted checks
- No migration, gameplay-policy change, reserve/payment mutation, or unrelated refactor
- A concise result explaining the cause, changed files, tests and remaining limitations

## Pinned source references

These links preserve the exact reviewed source even if `master` moves:

- [Acquisition service and defective work queue](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/MudSharpCore/Magic/Casting/MagicCastingService.Acquisition.cs#L115-L161)
- [Reconciliation entry point](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/MudSharpCore/Magic/Casting/MagicCastingService.Resources.cs)
- [Policy validation and cycle checks](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.Casting.cs)
- [Existing fixture and in-memory state store](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/MudSharpCore%20Unit%20Tests/MagicCastingFixture.cs)
- [Existing integration and shared-trait prerequisite test](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/MudSharpCore%20Unit%20Tests/MagicCastingIntegrationTests.cs)
- [Historical ARM-02 implementation handover](https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/Design%20Documents/Magic/Configurable_Casting_Handover.md)
