---
name: futuremud-climate-seeder
description: "Add, tune, or diagnose FutureMUD WeatherSeeder climate profiles and climate-specific runtime/analyzer behaviour, including event transitions, temperature fluctuation and opposite-hemisphere seasons. Skip unrelated seeder work, general weather questions and cosmetic edits; use for calibration only when simulation behaviour or benchmarks are in scope."
---

# FutureMUD Climate Seeder

Deliver a supported climate-profile change or a diagnosed climate/runtime regression, with verification appropriate to that change. Do not turn a narrow bug fix into a full climate-research and retuning exercise.

Paths below are repository-relative. Apply the instructions for the files you change, including `DatabaseSeeder/AGENTS.md` and the owning runtime/test instructions when relevant.

## Choose the task path

- **Add/recalibrate a climate:** establish benchmark evidence, inspect the relevant profile/runtime seams, tune, and add/update analyzer-backed regression coverage.
- **Fix a transition, hemisphere, fluctuation or analyzer defect:** reproduce the affected behaviour, inspect that seam and its callers, and use existing benchmarks unless the fix changes the calibration target.
- **Read-only diagnosis or an explicitly requested skill/document edit:** inspect the relevant definitions and report findings; do not mutate seed data or run long simulations merely because this skill was invoked.

## Read only the relevant model surface

| Change or uncertainty | Source |
| --- | --- |
| Seeding orchestration or profile contract | `DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.cs` and the declaration of `WeatherSeederClimateProfile`. |
| Template parameters or derived climate adjustments | `DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.ClimateTemplates.cs` and the affected adjuster. |
| Transition weights, stability or persistence | `MudSharpCore/Climate/ClimateModels/TerrestrialClimateModel.cs` and the affected weather events. |
| Base temperature or slow multi-day drift | `MudSharpCore/Climate/RegionalClimate.cs` and the relevant `WeatherController.cs` path. |
| Hemisphere/season lookup | `MudSharpCore/Climate/WeatherController.cs` and the relevant `WeatherClimateUtilities` implementation/tests. |
| Analyzer results or statistical assertions | `MudSharpCore/Climate/Analysis/WeatherStatisticsAnalyzer.cs` and the affected analyzer/regression tests. |
| Shared climate contract | The affected declaration under `FutureMUDLibrary/Climate/`. |

For calibration choices and known graph failure modes, read the needed section of [climate tuning notes](references/climate-tuning.md). Expand the source search only when the local evidence shows another dependency.

## Preserve these contracts

- Keep orchestration in `WeatherSeeder.cs` and profiles in the current multi-template pattern. Prefer deriving a model from the oceanic baseline when the event graph is fundamentally shared.
- Use one canonical weather-event catalogue, not hot/cold variants. Keep slow temperature fluctuation in regional climate/controller state.
- When collapsing temperature-only transitions, reallocate probability to the equivalent canonical event; preserve meaningful change chances and transition weights.
- Seed one northern-hemisphere regional baseline per profile. Use `WeatherController.OppositeHemisphere` for the half-celestial-year phase shift, not duplicate southern season rows.
- Preserve the distinction between base/sheltered temperature and outdoor wind/precipitation effects.

## Calibration evidence

When adding or changing real-world calibration, browse for official meteorological normals for a representative location and record the station/location, reference period, metric and units. A classification source describes the family; it does not replace empirical targets. Prefer national weather agencies; label secondary evidence or unresolved gaps.

An existing, adequately documented benchmark can be reused for a runtime bug fix. Do not browse solely to restate it. If sources are unavailable, make provisional assumptions explicit rather than presenting an inferred target as an observed normal.

Compare like with like: wet event occupancy is not automatically wet-day frequency or rainfall amount. Choose metric-specific tolerances; a blanket percentage error is unsuitable for temperatures near zero or rare events. See the tuning notes when translating observations into simulation assertions.

## Verification and output

Use `MudSharpCore Climate Tests/WeatherSeederOceanicClimateTests.cs` (class `WeatherSeederClimateTests`) as the existing slow seeded-climate pattern. Reuse the current seeding/snapshot/analyzer helpers instead of creating another harness. Slow climate regressions belong in `MudSharpCore Climate Tests`, not `MudSharpCore Unit Tests`.

For template changes, assert meaningful broad bands for annual/seasonal wet occupancy, snow, wind/severe-event rarity, and seasonal temperatures as relevant. For runtime/analyzer changes, also run focused regressions in the owning runtime/shared test suite; do not substitute the slow suite for focused contract tests.

After the relevant restore, a focused seeded-climate check is:

```text
dotnet test "MudSharpCore Climate Tests/MudSharpCore Climate Tests.csproj" -c Debug --no-restore -m:1 --filter WeatherSeederClimateTests
```

For the complete climate suite, use `scripts/test-unit-climate.ps1` on Windows or `scripts/test-unit-climate.sh` on POSIX. Run seeder/core tests as required by the other changed contracts. Follow the repository's verification reference for restore/sandbox issues.

CSV analysis and charts are optional diagnostics. Use available tools (for example `pandas`/`matplotlib`, or the standard CSV library); do not assume packages or machine-local validator scripts are installed. If editing this skill, validate frontmatter, metadata and links without treating that as a reason to simulate weather.

For calibration, report sources, target metrics, comparable measured outcomes and remaining mismatches. For a bug fix, report the defect, changed seam and regression result. In either case, distinguish tests actually run from unverified checks, and update design documentation whose described behaviour changed.
