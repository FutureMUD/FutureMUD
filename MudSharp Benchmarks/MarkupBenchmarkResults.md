# Markup benchmark results — 6 November 2026

The audited markup changes substantially reduce repeated optional-target emote rendering and characteristic-preview work. No clear timing regression appeared in the sampled cases. Several live characteristic cases and construction plus first render were effectively unchanged in this short run.

## Comparison

- Baseline: clean source exported from commit `68c8b0921c31afb1ba5dad62496537aa655b91ca`.
- Changed: the same checkout's uncommitted markup audit, with owning source hashes recorded in the local metadata artifact.
- Harness: identical `MarkupBenchmarks.cs` in both builds; SHA-256 `5032bdb0421ff1b1a9ea86a0cec2be1f47100eb70baccbd3b09fba5a5a202e4f`.
- BenchmarkDotNet 0.15.8, Release, .NET SDK 10.0.401, .NET 10.0.12, X64 RyuJIT x86-64-v4, Windows 11 build 26300.9711.
- One launch, three warmup iterations, five requested measurement iterations, 150 ms per iteration, with the default outlier policy and memory diagnoser. Each benchmark runs in its own process. Baseline and changed runs were sequential; compilation completed before measurement.
- Both measured runs completed all twelve cases. Initial preparation attempts produced no measurements because of a build timeout and duplicate project discovery. The completed runs used a 600-second build allowance; the archived benchmark project filename was hidden after the baseline run to avoid duplicate discovery.

## Results

Times are microseconds per operation. The two repeated emote cases render to twenty distinct observers per invocation; BenchmarkDotNet normalises their times and allocations to **one recipient render**. Other cases report one parse or output render. Allocation columns use bytes, not rounded kilobytes.

A dagger means the baseline and changed 99.9% timing confidence intervals overlap. Treat those timing differences as inconclusive in this run; allocation reductions remain directly observable. Interval overlap is a conservative reporting rule, not a formal equivalence test.

| Case | Before µs ± error | After µs ± error | Time reduction | Before B | After B | Allocation reduction |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Live characteristics: articles | 12.048 ± 0.144 | 10.739 ± 0.151 | +10.9% | 29,656 | 26,176 | 11.7% |
| Preview: articles | 15.150 ± 0.305 | 12.198 ± 0.220 | +19.5% | 34,552 | 27,672 | 19.9% |
| Live characteristics: basic/fancy † | 0.969 ± 0.016 | 0.973 ± 0.017 | -0.5% | 3,384 | 3,384 | -0.0% |
| Preview: basic/fancy | 2.899 ± 0.079 | 1.502 ± 0.024 | +48.2% | 9,032 | 5,744 | 36.4% |
| Live characteristics: full description † | 2.875 ± 0.046 | 2.837 ± 0.111 | +1.3% | 9,968 | 9,968 | -0.0% |
| Preview: full description | 4.949 ± 0.024 | 3.692 ± 0.108 | +25.4% | 15,136 | 12,744 | 15.8% |
| Live characteristics: simple † | 0.717 ± 0.015 | 0.719 ± 0.010 | -0.3% | 2,288 | 2,288 | -0.0% |
| Preview: simple † | 1.202 ± 0.018 | 1.164 ± 0.023 | +3.2% | 2,760 | 2,760 | -0.0% |
| Ordinary emote: repeated render | 0.235 ± 0.005 | 0.181 ± 0.002 | +23.2% | 712 | 544 | 23.6% |
| Optional-target emote: repeated render | 1.753 ± 0.006 | 0.176 ± 0.002 | +90.0% | 2,792 | 640 | 77.1% |
| Optional-target emote: construct + first render † | 3.791 ± 0.047 | 3.702 ± 0.149 | +2.4% | 7,304 | 7,128 | 2.4% |
| Recipient-assigned output † | 1.679 ± 0.033 | 1.624 ± 0.027 | +3.3% | 2,504 | 2,336 | 6.7% |

## Interpretation and limits

- Reusing a parsed optional branch delivers the largest gain: repeated rendering is about 10 times as fast and allocates 77.1% fewer bytes. Construction plus the first render shows no clear timing change; the main benefit comes when an emote is reused across recipients.
- Ordinary emote rendering takes 23.2% less time and allocates 23.6% fewer bytes, consistent with removing the per-render LINQ/closure overhead.
- Preview basic/fancy lookup takes 48.2% less time and allocates 36.4% fewer bytes. Full-description preview takes 25.4% less time and allocates 15.8% fewer bytes.
- Plural-aware article fixtures improve both live and preview parsing: 10.9% and 19.5% less time respectively, with 11.7% and 19.9% fewer allocated bytes.
- Simple live characteristics, live basic/fancy forms, and live full descriptions show no clear timing change and identical allocation counts. Assigned outputs allocate 6.7% fewer bytes, but their small timing difference is inconclusive.

These are warmed parser microbenchmarks with lightweight engine stand-ins and real in-memory characteristic definitions/values. Timed paths avoid Moq invocation recording, databases and server startup. Characteristic setup checks live/preview output agreement in each build. The emote scenarios use ordinary descriptions and numeric targets, without speech or player target lookup. Live characteristics have no worn obscurers. These cases do not measure tattoos/scars, bodypart counts, persistence, target-dependency enumeration, networking or whole-server throughput.

Processor identification and power-plan control were unavailable in the sandbox. The single short run is useful for large relative differences and allocation counts; it does not establish small timing improvements or production latency guarantees.

## Reproduction

Restore the benchmark project if required, then build it with `dotnet build "MudSharp Benchmarks/MudSharp Benchmarks.csproj" -c Release --no-restore -m:1`. Run the following command in each version's benchmark-project directory, supplying separate artifact directories:

```powershell
$env:NuGetAudit = 'false'
dotnet 'bin/Release/net10.0/MudSharp Benchmarks.dll' `
    --filter '*MarkupBenchmarks*' --job Short `
    --iterationTime 150 --warmupCount 3 --iterationCount 5 --launchCount 1 `
    --buildTimeout 600 --exporters JSON --artifacts '<absolute-artifact-directory>'
```

The local audit-lookup workaround does not change package versions or committed project settings. For a clean baseline, export the commit above to a separate directory and copy the exact harness into its benchmark project. Keep that directory outside the changed checkout, or hide its benchmark project file after the baseline run, because BenchmarkDotNet recursively discovers project files.

Local evidence (git-ignored):

- `../.artifacts/markup-benchmarks/before-measured/results/`: baseline JSON, CSV, HTML and Markdown.
- `../.artifacts/markup-benchmarks/after-final/results/`: changed JSON, CSV, HTML and Markdown.
- `../.artifacts/markup-benchmarks/comparison.json`: full-precision paired results.
- `../.artifacts/markup-benchmarks/metadata.json`: baseline commit and source/harness hashes.
