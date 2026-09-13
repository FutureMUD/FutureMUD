# Environmental magic measurement runner

This opt-in runner measures the delivered coordinator through the real `HeartbeatManager.ManuallyFireHeartbeatSecond` callback. It creates actual constructor-loaded cells, overlays, terrains, simple resources and factory-loaded environmental regenerators. Native-yield scenarios use actual forage profiles and pure cell yield access. Prog scenarios use a compiled, non-static FutureProg with a location argument and a branch over the physical cell ID.

The runner shares its fixture with the core structural tests. The world facade, registry storage and save sink are substitutes; no live game database is used. The world facade uses Moq, whose call interception adds measurable time and allocations. These numbers are a reproducible workload measurement, not a production capacity claim.

From the repository root, build serially, then run:

```powershell
dotnet build 'MudSharp Benchmarks/MudSharp Benchmarks.csproj' -c Release --no-restore -m:1
dotnet run --project 'MudSharp Benchmarks/MudSharp Benchmarks.csproj' -c Release --no-build --no-restore -- --environmental-magic --output artifacts/environmental-magic-measurements.json
```

Restore the benchmark project first if its declared packages are not already restored. Other benchmark commands continue to use the existing BenchmarkDotNet dispatcher.

The default matrix has 72 steady scenarios: 1,000 / 10,000 / 30,000 physical cells; one / three outputs; 0% / 1% / 10% / 100% initial active populations; and constant / native-yield / compiled-prog policies. It also runs 18 all-empty bootstrap scenarios and 18 bulk source-change scenarios. Bootstrap begins with missing ambient balances so the delivered registration path must initialise them at zero. Each source burst starts with full cells and lowers capacity through native forage consumption, a generator edit, or a changed compiled policy followed by explicit invalidation.

The clock advances one simulated second per pump without sleeping. Steady scenarios warm up for 120 simulated seconds and measure 3,720 seconds, covering the hourly reconciliation cycle. Bootstrap and burst measurements use at least 120 simulated seconds. Defaults are the product's 60-second active cadence, 3,600-second reconciliation, 1,024 cell visits, 8,192 output work units and a 5 ms soft pump budget. A synchronous prog cannot be preempted by that soft budget.

For a short harness smoke check:

```powershell
dotnet run --project 'MudSharp Benchmarks/MudSharp Benchmarks.csproj' -c Release --no-build --no-restore -- --environmental-magic --sizes 1000 --warmup-seconds 120 --sample-seconds 180 --output artifacts/environmental-magic-smoke.json
```

`--budget-ms` and `--cell-visits` override those two deployment limits for comparison. `--sizes`, `--warmup-seconds` and `--sample-seconds` narrow the runner, so report their actual values and do not describe a narrowed run as the full matrix.

The JSON output records runtime, OS, architecture, processor information, available memory, configured budgets and each scenario's actual active/dormant counts. It includes central callback and global scheduler counts, per-holder environmental delegates, cell/input/formula/prog evaluations, business writes, dirty-save requests, allocations, coordinator pump mean/p95/maximum, throughput, pending-work and audit ages, overload counters and unfinished burst work. `MaximumActiveUpdateAgeBoundSeconds` adds the active cadence to the oldest ready-work lateness while accepted active work exists; it is a conservative bound on active-update age because the late queue may be an audit or dirty queue. Zero means there was no accepted active work. Callback timing uses the coordinator's own stopwatch; an additional mean includes the outer heartbeat and fixture overhead. Allocation measurements include that overhead. Explicit-operation store and synchronous flush counts must remain zero in these background workloads.

The JSON file is refreshed after each completed scenario. A partial file is useful evidence after interruption, but completeness requires 108 scenario records at the default three sizes. Interpret pending work and ready age directly: queue lateness is retained and reported, never hidden by dropping elapsed production. Deterministic core tests enforce structural budgets; wall-clock values guide deployment tuning.
