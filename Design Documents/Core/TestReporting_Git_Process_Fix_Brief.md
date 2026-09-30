# Test reporting Git process reliability implementation brief

## Objective

Fix the Git subprocess deadlock in `scripts/TestReporting/Program.cs` and make Git evidence collection terminate predictably. A large warning stream must not stall a run, and a failed, cancelled or incomplete fingerprint must never become passing verification evidence.

Implement a focused reporter change with deterministic regression tests. Preserve the existing fingerprint algorithm, report schema, CLI and Windows/POSIX behavior. Do not change repository or global Git configuration, suppress line-ending warnings as the fix, introduce result caching, or redesign the test runner.

## Verified baseline

Reviewed `FutureMUD/FutureMUD` master at [385bffa41be7bb65e06b19463c59ac415e28afc4][revision] on 30 September 2026. Recheck the implementation checkout before editing; references below are pinned to this revision.

- [Git and Fingerprint][git-code], lines 497–539: Git redirects both streams but awaits stdout to EOF before reading stderr. It has no cancellation or timeout. Fingerprint hashes HEAD, the binary diff against HEAD, untracked file identities/content and selected project source/configuration files.
- [Run lifecycle][run-code], lines 141–203: the run deadline exists, but HEAD and both fingerprint calls do not receive its token. End fingerprinting runs even after the main operation fails or times out.
- [Existing process execution][execute-code], lines 431–485: build/test processes already drain both streams, attempt owned process-tree termination and bound subsequent waits. Windows readers use dedicated threads to avoid thread-pool starvation. Reuse these established principles without copying their behavior blindly.
- [ARM-02 handover][incident], lines 75–80: run `20260927T063854Z-93a35190020b` was inconclusive with no tests executed because Git line-ending warnings filled the serially drained pipe during a large diff. The successful later runs used process-scoped `GIT_CONFIG_COUNT` with `core.safecrlf=false`; the helper defect remained open.
- [Reporter tests][runner-tests] already use temporary Git repositories and a compiled [FakeDotnet helper][fake-dotnet]. Their high-output test exercises dotnet hosts, not Git. The [reporter project][reporter-project] exposes internals to the [MSTest project][test-project]; both target `net10.0`.
- The reviewed reporter has no result-cache lookup or reuse mechanism. The safety requirement is complete, valid source evidence. Do not add an empty, partial, HEAD-only or previously successful fallback fingerprint that another consumer could mistake for a valid identity.

Microsoft documents the same [two redirected stream deadlock][redirect-docs]. Awaiting an asynchronous read immediately still leaves the other pipe unread.

## Repository rules and change boundaries

Read the checkout's [AGENTS.md][instructions], applicable nested instructions, [verification guidance][verification] and [test execution contract][test-execution]. At the reviewed revision there are no nested `AGENTS.md` files under `scripts/`. Follow the declared framework/package versions; do not upgrade tooling.

Expected changes:

- `scripts/TestReporting/Program.cs`, with a small internal Git/process helper file only if this makes lifecycle testing clearer
- `scripts/TestReporting.Tests/RunnerTests.cs` and/or focused Git-process tests
- A small compiled fake-process helper, or a clearly separated mode in existing `FakeDotnet`; update project references/exclusions only as needed
- `scripts/TestReporting/README.md` and the affected timeout/error paragraphs in `.codex/references/test-execution.md`

Do not touch gameplay, databases, migrations, unrelated wrappers or broad process infrastructure. The historical ARM-02 record must remain an accurate account of that run; no rewrite is required.

## Required implementation behavior

### Drain both streams throughout execution

Start both output readers immediately after a successful process start, before awaiting either reader or process exit. Completion requires exit zero and complete stdout/stderr drains.

Keep `UseShellExecute=false`, repository working directory and `ArgumentList`; never build a shell command. Preserve stdout exactly as the existing text-based fingerprint input, including embedded NUL separators, newlines and final characters. Do not use line callbacks that normalize or discard these characters. Retain the current decoding/hash behavior rather than silently changing hashes.

Preserve the existing dedicated-reader approach on Windows, or prove an equivalent implementation under `DOTNET_PROCESSOR_COUNT=2`. Merely moving blocked reads onto ordinary thread-pool workers can undermine deadlines on constrained machines.

The stdout needed for hashing must be complete. Stderr may retain a bounded diagnostic excerpt, but the reader must continue consuming and discarding beyond that limit until EOF. Never stop reading because an excerpt buffer is full. Sanitize and bound surfaced diagnostics with the existing `Clean` conventions. Successful Git commands may emit warnings; nonempty stderr alone is not failure.

### Bound operations and cleanup

Thread the existing run cancellation token through the initial HEAD read, both fingerprint passes and every Git operation. Check it before launching a child and between file enumeration/hash operations; pass it to supported asynchronous file hashing.

Use a named, internal, test-injectable 60-second ceiling for each Git operation, linked to the run token. This also bounds Git when human mode has no overall timeout. A supplied run deadline takes precedence; a fresh Git ceiling must not reset or extend it. Document this new ceiling.

On cancellation, operation timeout, drain failure or another exception after launch:

1. Stop treating captured output as usable evidence.
2. Attempt `Kill(entireProcessTree: true)` for the process this invocation owns if it is still running. Never kill processes by name or enumerate unrelated Git processes for termination.
3. Reap the parent and finish/close its readers with bounded cleanup. Use an overall cleanup allowance of at most ten seconds, consistent with the existing five-second exit and drain waits; do not follow a timed wait with an unbounded wait.
4. Observe reader task faults, dispose streams/processes and report any failed or uncertain cleanup. Keep the original timeout/failure reason when cleanup also fails.

A cancelled [WaitForExitAsync][wait-docs] cancels the wait, so process cleanup remains explicit. [Kill documentation][kill-docs] also warns that parent exit does not prove every descendant has exited. If inherited pipes remain open after parent exit, bound the drain and report incomplete evidence; do not claim successful tree cleanup merely from `HasExited`.

Do not launch a fresh end-fingerprint pass once the run deadline has expired. Do not repeat fingerprinting when initial source capture never succeeded. Final receipt writing and lock release still need to run. This task does not require new Ctrl+C handling or a public cancellation status.

### Preserve evidence and reporting contracts

Use existing schema-version-1 fields and issue categories. Keep Compact/Json output within the existing 8 KiB budget and never stream raw Git output into the receipt.

- **Exit zero and complete drains:** return the full stdout, even if warnings were emitted.
- **Nonzero Git exit:** reject all stdout from that command; report a bounded, sanitized diagnostic identifying the operation and exit code. Treat it as `REPORTING_ERROR`/INCONCLUSIVE, not a valid empty diff.
- **Git cannot start:** classify it as `PREREQUISITE_UNAVAILABLE`. Before build/test execution this produces BLOCKED, exit 2 in Compact/Json. This is an intentional improvement over today's generic exception path. Avoid a redundant end-fingerprint failure turning that result into INCONCLUSIVE.
- **Run cancellation or Git ceiling:** retain `TIMEOUT`, INCONCLUSIVE/exit 3 in Compact/Json, unless existing confirmed-failure precedence applies. An internally cancelled helper should propagate cancellation rather than return partial output.
- **End fingerprint unavailable or incomplete:** retain unknown source stability and an appropriate existing issue; preserve `SOURCE_CHANGED` for missing/changed end evidence and include `TIMEOUT` when applicable. Never assign a partial end hash or set `source_stable=true`.
- **Two successful fingerprints:** compare them as today. Set stability true only from two completed valid captures. A differing hash retains `SOURCE_CHANGED`.

Preserve confirmed test/build FAIL precedence and Human mode's existing nonzero-for-non-PASS mapping. Keep the worktree lock held through owned subprocess cleanup, release it on every terminal path, and retain partial artifacts as evidence without presenting them as completed verification.

Do not alter Git arguments, scope ordering, results-root exclusions, project selection, binary diff flags or hashing inputs to hide the problem. No `git config` writes, injected `core.safecrlf=false`, warning redirection to null or replacement of dirty-tree evidence with HEAD alone.

## Deterministic regression plan

Use the existing MSTest project and compiled helper pattern. A narrow internal seam for executable path and timeout values is sufficient; avoid a general process abstraction. For child-reporter integration, a test-only Git executable override analogous to `FUTUREMUD_TEST_DOTNET` is acceptable if needed. Set it only on each spawned process's environment. Do not mutate the developer's PATH or Git configuration.

A fake Git mode should accept normal arguments and produce controlled HEAD, diff and NUL-separated file-list responses. Store control files, invocation counters and PID markers outside fingerprinted source, under an owned temporary directory or excluded artifacts root. Reject unexpected commands so a test cannot silently exercise the wrong branch. Use explicit readiness markers before cancellation; avoid sleeps as proof that a subprocess reached the desired state.

Add the following focused coverage, combining cases with data-driven tests where useful:

1. **Pipe saturation regression:** write at least 2 MiB to stderr while keeping stdout open, then write a known stdout payload and exit zero. Repeat with stdout-first and alternating blocks on both streams. Include a no-final-newline tail and NULs in stdout. Assert exact complete stdout and successful completion. The stderr-first case must reproduce the old serial-reader failure under an outer watchdog.
2. **Failure and missing executable:** emit partial stdout plus a large stderr stream containing control characters and a secret-labelled value, then exit 7. Assert failure, exit-code context and sanitized bounded diagnostics; no usable fingerprint. Launch a nonexistent absolute executable and assert the start-failure classification. Reporter integration must show BLOCKED/2 before any fake-dotnet build/test and unknown source stability.
3. **Operation timeout and caller cancellation:** helper publishes its PID/readiness and waits indefinitely, with both a quiet variant and an output-producing variant. Use a short injected operation timeout; separately cancel a caller token after readiness, plus test an already-cancelled token starts no child. Assert bounded return, cancellation/failure outcome, parent exit, observed reader completion and lock availability. When testing a live owned descendant, record its PID and verify its termination separately.
4. **Incomplete drain:** helper exits while an owned descendant keeps a redirected pipe open. Assert the drain deadline produces incomplete evidence rather than hanging or returning success. The test harness must own and clean up that descendant in `finally`; report any cleanup limitation instead of hiding it.
5. **Run deadline and final fingerprint:** fake Git hangs specifically during the first fingerprint, then in a separate case only during the end fingerprint after FakeDotnet has produced passing TRX. Assert TIMEOUT/INCONCLUSIVE, never PASS, no invented stable hash, bounded receipt and release of the worktree lock. Verify the expired/failed-start path does not start another fingerprint pass.
6. **Fingerprint compatibility:** retain real-Git tests for unchanged source, source mutation and custom results-root exclusion. Add a small fixture with a tracked edit and untracked files, including a filename with spaces, to compare expected pre-fix hash-input semantics with the new helper. Warnings on successful fake Git operations must not alter a stable hash.

Drain stdout and stderr concurrently in the test harness itself. Healthy helper scenarios can retain the suite's 30-second budget and 60-second outer watchdog. Inject short limits for intentional timeout tests, with sufficient cleanup/watchdog slack for loaded Windows CI. Every watchdog failure must kill/reap only its owned children and preserve useful diagnostics.

Run on both Windows and Linux, matching the [existing CI matrix][ci]. No line-ending settings or developer machine warning behavior should be required to reproduce the deadlock.

## Verification commands

From the repository root, with the declared .NET SDK available, restore only if needed:

```text
dotnet restore scripts/TestReporting.Tests/TestReporting.Tests.csproj -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
dotnet test scripts/TestReporting.Tests/TestReporting.Tests.csproj -c Debug --no-restore -m:1
```

Then verify the public entry point on each available platform:

```bash
bash ./scripts/test-unit.sh --output-mode compact --project scripts/TestReporting.Tests/TestReporting.Tests.csproj
```

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'scripts/TestReporting.Tests/TestReporting.Tests.csproj'
```

These flags and project paths are present at the reviewed revision. Run the direct test command first so a broken reporter cannot be the only mechanism used to validate its own fix. The targeted wrapper run then exercises bootstrap, receipt production and fingerprint integration. Avoid concurrent runs/builds that mutate the same inputs.

Finish with `git diff --check` and review the changed-file list. A full solution build, climate run or gameplay smoke test is not required for this infrastructure-only fix. If only one OS is available locally, record that limitation and require the other CI job before declaring cross-platform verification complete.

## Acceptance and handover

The implementation is complete when the old serial-reader regression demonstrably fails under a bounded watchdog, the fixed helper passes it on Windows and Linux, timeout/cancellation cannot leave an unbounded wait, failed source evidence cannot yield PASS, and all existing reporter tests remain green.

Return the changed files, exact test commands/outcomes, tested OSes, source revision, relevant receipt/artifact paths and any cleanup uncertainty or unexecuted checks. State whether the 60-second Git ceiling affected any legitimate fixture. Report only executed checks as passing. Publication or merge is a separate action.

[revision]: https://github.com/FutureMUD/FutureMUD/commit/385bffa41be7bb65e06b19463c59ac415e28afc4
[git-code]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting/Program.cs#L497-L539
[run-code]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting/Program.cs#L141-L203
[execute-code]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting/Program.cs#L431-L485
[incident]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/Design%20Documents/Magic/Configurable_Casting_Handover.md#L75-L80
[runner-tests]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting.Tests/RunnerTests.cs
[fake-dotnet]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting.Tests/FakeDotnet/Program.cs
[reporter-project]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting/TestReporting.csproj
[test-project]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/scripts/TestReporting.Tests/TestReporting.Tests.csproj
[instructions]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/AGENTS.md
[verification]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/.codex/references/verification-and-docs.md
[test-execution]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/.codex/references/test-execution.md
[ci]: https://github.com/FutureMUD/FutureMUD/blob/385bffa41be7bb65e06b19463c59ac415e28afc4/.github/workflows/unit-tests.yml
[redirect-docs]: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandarderror?view=net-10.0
[wait-docs]: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.waitforexitasync?view=net-10.0
[kill-docs]: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.kill?view=net-10.0

