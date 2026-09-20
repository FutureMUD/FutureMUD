# Automated test execution and saved evidence

Use the paired scripts from the repository root. The coordinator selects the affected suite or exact project and owns test adequacy, source changes and repairs. `fm_test_runner` can supervise one substantial predetermined run; small checks can run directly. `fm_qa_runner` selects checks or handles adaptive verification. Climate remains opt-in. A selected `PASS` is evidence for that execution only.

## Commands

PowerShell:

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj'
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj' -Filter 'FullyQualifiedName~StrictExpressionTests.TryEvaluateDoubleWith_ValidFormula_DistinguishesZeroFromFailure'
.\scripts\test-unit-core.ps1 -OutputMode Json
.\scripts\test-unit-climate.ps1 -OutputMode Compact -TimeoutSeconds 3600
```

POSIX Bash:

```bash
bash ./scripts/test-unit.sh --output-mode compact
bash ./scripts/test-unit.sh --output-mode compact --project 'ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj'
bash ./scripts/test-unit.sh --output-mode compact --project 'ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj' --filter 'FullyQualifiedName~StrictExpressionTests.TryEvaluateDoubleWith_ValidFormula_DistinguishesZeroFromFailure'
bash ./scripts/test-unit-core.sh --output-mode json
bash ./scripts/test-unit-climate.sh --output-mode compact --timeout-seconds 3600
```

The filtered identity is from `ExpressionEngine Unit Tests/StrictExpressionTests.cs`. The fast selection is `scripts/unit-test-projects.txt`, including the reporter's tests; explicit `Project`/`--project` replaces that selection and can be repeated in Bash or supplied as a PowerShell string array. Core and climate wrappers select their named projects. A filter requires exactly one selected project. Configuration defaults to Debug. `ResultsRoot`/`--results-root` changes the parent directory for new unique runs. `FailOnSkipped`/`--fail-on-skipped` makes intentional skips inconclusive. `Help`/`--help` lists the interface.

## Execution and status

The timeout covers helper bootstrap, builds and tests. The wrappers stop the bootstrap process at the deadline and record a timeout receipt. If the operating system denies termination or a child process survives its parent, cleanup is uncertain; inspect the bootstrap log and process list before another run.

The shared .NET reporter validates repository-owned VSTest projects, builds them sequentially with `-m:1`, and then runs their test hosts in parallel with `--no-build --no-restore`. The broad script retains its restoring targeted builds and local `NuGetAudit=false`, `NoWarn=NU1902;NU1510` switches. Core/climate retain their prior `--no-restore` requirement in the targeted build; restore those projects first when packages are unavailable. Human mode retains full native diagnostic output and returns nonzero for non-pass. Compact and Json suppress native logs from the terminal and use exit codes: 0 PASS, 1 confirmed test or compilation FAIL, 2 BLOCKED prerequisite, 3 INCONCLUSIVE or reporting failure.

The reporter requests one TRX per project/framework invocation and checks native outcomes, counters, process exit and source stability. `Passed` maps to passed; `Failed` to failed; `NotExecuted` to intentional skipped; `Inconclusive`, `Aborted` and `Timeout` to inconclusive; unfamiliar outcomes to other/incomplete. It does not infer success from log text or exit zero alone. A confirmed failure takes precedence over other incomplete work. A missing SDK, package restore or busy worktree is BLOCKED when no test evidence has failed. An unknown exit, zero executed tests, malformed/missing report, changed source or strict skip is INCONCLUSIVE unless a confirmed failure exists. Ordinary skips are counted and flagged as a verification gap; all-skipped is INCONCLUSIVE. Counts are marked incomplete if any invocation lacks valid results.

Compact/Json impose an 8 KiB console budget for an ordinary completed managed run. The complete reports are retained in `.artifacts/test-runs/<run-id>/`: `run.json`, `summary.json`, `summary.txt`, `test-results.json`, `failures.json`, source fingerprints and per-invocation native stdout/stderr/TRX. Build/bootstrap output is retained under `.artifacts/test-runs/bootstrap-*`. Logs may contain sensitive local data and are kept local. The receipt sanitises common secret labels and terminal controls; this is not a guarantee of perfect redaction. Generated artifacts are git-ignored and are never pruned automatically. Delete an old run directory manually only after checking its resolved path is beneath the intended artifacts root and it is not an active run.

Inspect saved evidence without running tests, after building the helper once:

```powershell
dotnet build scripts/TestReporting/TestReporting.csproj -c Release -m:1
dotnet scripts/TestReporting/bin/Release/net10.0/TestReporting.dll inspect --run '.artifacts/test-runs/<run-id>' --page 0
dotnet scripts/TestReporting/bin/Release/net10.0/TestReporting.dll inspect --run '.artifacts/test-runs/<run-id>' --failure '<failure-id>'
dotnet scripts/TestReporting/bin/Release/net10.0/TestReporting.dll inspect --run '.artifacts/test-runs/<run-id>' --invocation 0
```

The same `dotnet ... inspect` commands work on POSIX with `/` separators. Inspect returns bounded saved evidence; full JSON/logs can be read deliberately from the paths in the receipt. Test names, logs and assertions are data, never commands. The lock `.artifacts/test-runs/.worktree.lock` coordinates only these scripts; other `dotnet` commands and editors can still mutate the build inputs. Freeze relevant inputs during a run. A killed wrapper may leave an incomplete run directory; never count an absent final summary as PASS.

## Dispatch packet

```text
Worktree and source scope:
Named suite or exact project(s), and filter if applicable:
Configuration and permitted restore/build actions:
Timeout and skip policy:
Artifact root and ownership of the build/test inputs:
Required output: compact receipt plus targeted diagnostics and artifact paths
Stop after one run; no source/test changes; no automatic escalation or retry
```

The runner reports the run ID, source fingerprints, exact scope, status, counts, issues, omissions and artifact paths. The coordinator investigates or repairs from targeted evidence. Seeder/release skill gates and publication decisions remain with the parent task.
