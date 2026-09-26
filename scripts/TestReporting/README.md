# Test-reporting execution guarantees

Use the paired repository test scripts as described in the [test execution reference](../../.codex/references/test-execution.md). The reporter retains native logs and structured TRX evidence; missing or contradictory evidence never counts as a pass.

Each worktree has one cooperative owner. The reporter holds `.artifacts/test-runs/.worktree.lock` with `FileShare.None` for the entire run and keeps the current run identifier in the readable `.worktree.lock.owner` sidecar. A contender reports `BLOCKED` without building or testing. The marker is diagnostic only: a stale marker does not block a run after the operating system releases the lock. Other tools and editors do not participate in this lock.

Windows redirected process streams use synchronous pipe handles in [.NET 10](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Diagnostics.Process/src/System/Diagnostics/Process.Windows.cs). Each stdout/stderr copy therefore gets a dedicated reader thread so parallel test hosts do not consume the thread-pool workers needed for exit and deadline continuations. Other platforms retain asynchronous copies. Log capture remains bounded after process exit; an incomplete drain is still reported as incomplete evidence.

The reporter suite exercises eight simultaneous hosts, both complete 200 KB output streams, interprocess ownership and release, contradictory exits, missing/stale/malformed reports, source changes and deliberate timeouts. Healthy fake-runner scenarios have a 30-second budget for loaded CI machines; the intentional timeout scenario retains its three-second limit.
