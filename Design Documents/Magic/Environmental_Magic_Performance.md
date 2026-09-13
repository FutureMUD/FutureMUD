# Environmental magic performance measurements

The complete 108-scenario run succeeded on 13 September 2026. It exercised the delivered coordinator, actual cells and resource operations, factory-loaded environmental generators, and the real second-heartbeat callback. The largest fully active workloads made sustained progress, but did **not** maintain a strict 60-second update interval: their conservative active-update age bounds reached 74–88 seconds. The 5 ms pump limit is a soft admission budget, not a maximum callback duration.

The [raw measurements](Environmental_Magic_Measurements.json) contain every scenario and unrounded value. See the [runner instructions](../../MudSharp%20Benchmarks/EnvironmentalMagicBenchmark.md) and [runtime contract](Environmental_Magic_Runtime.md).

## Configuration and method

| Item | Measured configuration |
| --- | --- |
| Start time | 2026-09-13 11:41:27 UTC |
| Runtime/build | .NET 10.0.12, Release, X64 |
| Operating system | Microsoft Windows 10.0.26200 |
| Processor | AMD64 Family 26 Model 68 Stepping 0, AuthenticAMD; 32 logical processors |
| Available memory reported by runtime | 66,155,474,944 bytes, approximately 61.6 GiB |
| Population matrix | 1,000 / 10,000 / 30,000 physical cells; one / three outputs |
| Activity matrix | 0% / 1% / 10% / 100% initially below maximum |
| Policies | Constant baseline; native forage yield; compiled, non-static location FutureProg |
| Steady cases | 72; 120 simulated seconds of warm-up, then 3,720 measured seconds each |
| Other cases | 18 all-empty bootstraps and 18 bulk source changes; 120 measured seconds each |
| Deployment settings | 60-second active cadence; 3,600-second audit; 1,024 visits; 8,192 output work units; 5 ms soft pump budget |

The deterministic clock advances one simulated second per callback without sleeping. The constant policy uses capacity 100 and rate 1 per minute. The forage policy reads a real profile's `herbs` yield; the compiled prog branches on the physical cell ID and returns `80 + 20`. These are test policies, not seeded content. Accepted active populations matched the requested populations at both ends of every steady sample.

The fixture uses actual `Cell`, overlays, terrain, resource definitions, expressions, forage access, FutureProg and coordinator code. The world facade uses Moq; registry storage, deferred saves and the explicit-operation store are in-memory substitutes. Measurements exclude database latency, networking, other gameplay and the source-change setup time. Pump timings come from the coordinator's own stopwatch. Throughput and allocation measurements include heartbeat/fixture instrumentation; allocations are cumulative bytes allocated, **not resident memory**. Accelerated simulation changes garbage-collection timing, so these results are workload evidence rather than a production-host capacity guarantee.

Reproduce from the repository root after restoring declared packages:

```powershell
dotnet build 'MudSharp Benchmarks/MudSharp Benchmarks.csproj' -c Release --no-restore -m:1
dotnet run --project 'MudSharp Benchmarks/MudSharp Benchmarks.csproj' -c Release --no-build --no-restore -- --environmental-magic --output 'Design Documents/Magic/Environmental_Magic_Measurements.json'
```

## Completeness and structural observations

All 108 expected scenario identities are present: 72 steady, 18 bootstrap and 18 burst. Across 272,160 measured callbacks, every case retained one second-heartbeat subscriber, one existing global scheduler entry, and zero environmental per-holder delegates. No pump exceeded the 1,024-cell visit ceiling. Every case ended with zero faulted cells. There were zero explicit-operation reads, commits or synchronous save flushes during these background workloads.

All constant/full and other zero-activity steady cases made zero business writes and queued zero saves. At 30,000 cells, each zero-activity case evaluated 30,985 snapshots over 3,720 seconds; with 300 active cells, that rose to 49,574–49,579 snapshots. This is useful production plus rolling audit work, rather than 30,000 callbacks every minute.

The complete measured run recorded 18,227,088 cell/input snapshots, 145,410,520 formula evaluations, 6,064,236 input-prog executions, 34,623,730 business writes, and 683,060 newly queued saves. Its measured loops consumed 180.1 wall seconds, excluding world construction, warm-up and source-change setup. Resource changes coalesce into the existing save queue; the fixture does not model a database draining that queue.

## Steady results at 30,000 cells

Times are milliseconds per pump. `Active age bound` is the configured 60-second cadence plus the largest observed ready-work lateness while active work exists. It conservatively includes possible audit/dirty-queue lateness; it is not an exact per-cell latency percentile. A dash means no active work. `Audit age` is time since the oldest registration's last accepted check, including active checks, so it is much smaller for a fully active population.

| Outputs | Active | Policy | Mean ms | p95 ms | Max ms | Active age bound s | Audit age s |
| ---: | ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 0% | Constant | 0.044 | 0.048 | 12.343 | — | 3599 |
| 1 | 0% | Forage | 0.112 | 0.072 | 15.549 | — | 3599 |
| 1 | 0% | Compiled prog | 0.050 | 0.054 | 11.086 | — | 3600 |
| 1 | 1% | Constant | 0.070 | 0.105 | 16.192 | 60.94 | 3599 |
| 1 | 1% | Forage | 0.088 | 0.137 | 12.827 | 60.87 | 3599 |
| 1 | 1% | Compiled prog | 0.075 | 0.112 | 9.659 | 60.71 | 3599 |
| 1 | 10% | Constant | 0.300 | 0.440 | 11.272 | 60.89 | 3599 |
| 1 | 10% | Forage | 0.371 | 0.603 | 9.755 | 60.67 | 3599 |
| 1 | 10% | Compiled prog | 0.341 | 0.538 | 10.011 | 60.60 | 3599 |
| 1 | 100% | Constant | 3.617 | 12.748 | 24.648 | 74 | 74 |
| 1 | 100% | Forage | 4.240 | 12.800 | 20.275 | 76 | 76 |
| 1 | 100% | Compiled prog | 4.048 | 14.261 | 25.208 | 75 | 75 |
| 3 | 0% | Constant | 0.081 | 0.089 | 8.667 | — | 3599 |
| 3 | 0% | Forage | 0.090 | 0.098 | 10.802 | — | 3599 |
| 3 | 0% | Compiled prog | 0.079 | 0.084 | 8.810 | — | 3599 |
| 3 | 1% | Constant | 0.121 | 0.185 | 9.511 | 60.87 | 3600 |
| 3 | 1% | Forage | 0.135 | 0.238 | 8.547 | 60.86 | 3599 |
| 3 | 1% | Compiled prog | 0.126 | 0.199 | 8.195 | 60.74 | 3600 |
| 3 | 10% | Constant | 0.577 | 1.357 | 11.366 | 60.89 | 3600 |
| 3 | 10% | Forage | 0.657 | 1.456 | 10.677 | 60.84 | 3600 |
| 3 | 10% | Compiled prog | 0.632 | 1.160 | 10.767 | 60.96 | 3600 |
| 3 | 100% | Constant | 6.018 | 12.961 | 21.114 | 80 | 80 |
| 3 | 100% | Forage | 6.088 | 12.193 | 21.551 | 88 | 88 |
| 3 | 100% | Compiled prog | 6.594 | 14.987 | 22.584 | 81 | 81 |

At 1,000 cells, the worst steady p95 was 0.546 ms and worst ready-work lateness was 0.656 seconds. At 10,000 cells, the corresponding values were 6.575 ms and 7 seconds. The 10,000-cell, three-output, fully active cases reached conservative active-age bounds of 64–67 seconds.

Work and allocations for the six fully active 30,000-cell samples are shown below. Every sample queued 30,000 new saves. Formula counts include maximum/rate evaluations before and after settlement; compiled input progs run once per cell snapshot, shared across all outputs.

| Outputs | Policy | Cell snapshots | Formula evaluations | Input progs | Allocated GiB | Cell visits / wall second |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 1 | Constant | 1,881,703 | 7,526,812 | 0 | 18.07 | 139,817 |
| 1 | Forage | 1,860,727 | 7,442,908 | 0 | 19.81 | 117,934 |
| 1 | Compiled prog | 1,871,009 | 7,484,036 | 1,871,009 | 19.57 | 124,191 |
| 3 | Constant | 1,822,562 | 21,870,744 | 0 | 43.26 | 81,391 |
| 3 | Forage | 1,706,663 | 20,479,956 | 0 | 42.31 | 75,346 |
| 3 | Compiled prog | 1,754,512 | 21,054,144 | 1,754,512 | 43.12 | 71,485 |

## Bootstrap and source bursts

All 18 bootstrap cases had every cell accepted as active by the end of their 120-second samples. All 18 bulk changes finished with zero dirty/discovery work, zero ready-work lateness, and every cell dormant at the lower capacity. These are bounded observations over the measured interval, not a claim that no queue remained temporarily overdue.

| 30,000-cell workload | Outputs | Policy | Mean ms | p95 ms | Max ms | Maximum ready lateness s | Final ready lateness s |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: |
| Empty bootstrap | 1 | Constant | 4.603 | 20.359 | 41.266 | 0.97 | 0 |
| Empty bootstrap | 1 | Forage | 5.942 | 29.089 | 44.886 | 0.92 | 0 |
| Empty bootstrap | 1 | Compiled prog | 5.210 | 20.746 | 45.458 | 0.81 | 0 |
| Empty bootstrap | 3 | Constant | 7.716 | 22.167 | 32.162 | 1.24 | 1 |
| Empty bootstrap | 3 | Forage | 7.742 | 21.442 | 27.841 | 4.69 | 4 |
| Empty bootstrap | 3 | Compiled prog | 7.010 | 18.393 | 28.264 | 19 | 19 |
| Source burst | 1 | Constant | 2.016 | 6.748 | 14.257 | 0 | 0 |
| Source burst | 1 | Forage | 2.690 | 7.686 | 75.575 | 42 | 0 |
| Source burst | 1 | Compiled prog | 2.078 | 5.845 | 11.744 | 0 | 0 |
| Source burst | 3 | Constant | 3.594 | 11.057 | 13.857 | 0 | 0 |
| Source burst | 3 | Forage | 3.868 | 10.421 | 42.928 | 69 | 0 |
| Source burst | 3 | Compiled prog | 3.869 | 10.721 | 19.452 | 0 | 0 |

The native burst deliberately performs 30,000 individual yield consumptions before pumping: setup took 3.284–3.332 wall seconds. Those operations already dirtied the cells, so later capacity clamps required zero *additional* save-queue requests, despite making 30,000 or 90,000 resource writes. Equivalent generator/prog definition changes took 0.203–0.535 ms of setup and used incremental discovery. They are different source workloads; the native batch time is not the latency of one profile edit.

## Supported envelope and tuning

This run supports near-minute sampled service for the tested 30,000-cell workloads with up to 10% active: conservative active-age bounds stayed below 61 seconds, p95 stayed at or below 1.456 ms, and the oldest dormant audit stayed within 3,600 seconds. Individual outliers still exceeded the soft budget.

At 30,000 fully active cells, one-output work was budget-limited in 22.0–49.5% of pumps. Three-output work was budget-limited in 83.7–100%, with some work still 1–4 seconds overdue at the sample end and a worst observed ready delay of 28 seconds. Treat strict 60-second service for these fully active heavy populations as unsupported by this measurement. Delayed elapsed production is retained by the runtime accounting contract; queue lateness is visible rather than hidden by discarding time.

Keep the 5 ms default as a conservative starting allocation to this subsystem. This run does not compare alternative budgets or include the rest of a live main loop, so it does not justify increasing that default globally. A deployment needing tighter latency should first measure its own policy cost, population and competing gameplay, then tune its explicit cell/output/time budgets and accepted cadence. Synchronous progs and garbage-collection pauses can exceed the soft budget; admission stops only after the current cell finishes. Structural regression tests enforce scheduling shape and accounting correctness; these wall-clock measurements guide configuration.
