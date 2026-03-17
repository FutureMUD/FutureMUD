---
name: futuremud-climate-seeder
description: Use when adding or tuning FutureMUD WeatherSeeder climate templates, canonical weather-event transitions, regional temperature fluctuation settings, opposite-hemisphere controller behavior, or analyzer-backed weather tests.
---

# FutureMUD Climate Seeder

Use this skill when you need to add a new seeded climate or retune an existing one in FutureMUD. The target is believable output grounded in real climate references and locked in with analyzer-based tests.

## Assumed Tooling

Assume these Python packages are installed and use them by default:

- `PyYAML` for skill-tooling validation and metadata scripts
- `pandas` for reading exported weather CSVs and calculating quick aggregates
- `matplotlib` for fast visual checks of seasonal temperatures, rainfall frequency, and wind distributions

Prefer these tools over ad hoc manual parsing when they are available.

## Workflow

1. Read the runtime model before touching seed data.
2. Research a real-world reference climate and extract benchmark ranges.
3. Add or tune the climate template, derived-model adjustments, seasonal temperatures, and regional fluctuation metadata.
4. Verify with analyzer-backed unit tests and, when useful, CSV output.
5. Summarize the final climate against the benchmarks, including any remaining gaps.

## Seeder Template Format

Follow the current multi-template pattern instead of bolting bespoke logic straight into `WeatherSeeder.cs`.

- Keep orchestration in `WeatherSeeder.cs`: `SeedData` should iterate `GetClimateProfiles()` and seed one shared northern-hemisphere baseline regional climate from each profile.
- Treat `WeatherSeederClimateProfile` as the template contract. A template should declare the climate model name, regional climate prefix, seasonal temperature ranges, transition-chance delegates, stability settings, Koppen/reference metadata, and description text inputs.
- Define climate templates in `DatabaseSeeder/Seeders/WeatherSeeder.ClimateTemplates.cs`. Current examples are `CreateTemperateOceanicProfile()`, `CreateHumidSubtropicalProfile()`, and `CreateMediterraneanProfile()`.
- Prefer `CreateDerivedClimateModel()` when the new climate is mostly a retuned version of the seeded oceanic model. Add a climate-specific transition adjuster and fallback adjuster instead of cloning the whole base transition graph.
- Keep the oceanic template as the reference baseline unless the new climate truly needs a fundamentally different event graph.
- Seed a single canonical weather-event catalog. Do not add hot/cold weather-event variants; temperature day-to-day variation now belongs in `RegionalClimate`.
- When removing temperature-only transitions, reallocate that probability back onto the equivalent canonical event so `ChangeChance` and transition weights still reflect the simplified graph.
- Seed `RegionalClimate.TemperatureFluctuationStandardDeviation` and `RegionalClimate.TemperatureFluctuationPeriod` from the template so regional climates explain their slow multi-day drift separately from weather-event temperature effects.
- Southern-hemisphere duplication is gone. Use the weather controller's `OppositeHemisphere` flag to phase-shift seasons by half a celestial year instead of seeding duplicate south-season rows.

Current seeded pattern:

- `Temperate` / oceanic is the base seeded model.
- `Humid Subtropical` and `Mediterranean` are derived templates built by scaling and retuning the seeded oceanic transitions.
- `CreateRegionalClimate()` seeds one northern-hemisphere baseline regional climate per template, and controllers opt into the opposite hemisphere at runtime.

## Read First

Understand these files before making seeder changes:

- `C:/Users/Luke/source/repos/FutureMUD/DatabaseSeeder/Seeders/WeatherSeeder.cs`
- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore/Climate/ClimateModels/TerrestrialClimateModel.cs`
- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore/Climate/RegionalClimate.cs`
- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore/Climate/WeatherController.cs`
- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore/Climate/Analysis/WeatherStatisticsAnalyzer.cs`
- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore/Climate/WeatherEvents/`
- `C:/Users/Luke/source/repos/FutureMUD/FutureMUDLibrary/Climate/`

Keep these runtime rules in mind:

- Seasonal and hourly base temperatures come from the regional climate definition.
- Sheltered temperature is the base temperature plus the regional temperature fluctuation plus the current weather event temperature effect.
- Outdoor temperature can be pushed further by precipitation and wind effects.
- `WeatherController.OppositeHemisphere` changes seasonal lookup by half a celestial year; it does not require duplicated seeded seasons.
- `TerrestrialClimateModel` amplifies small probability mistakes into unrealistic long-run weather distributions.

## Research

Always browse for sources. Use a classification page only to define the climate family, then calibrate against official normals for a representative location.

Good source order:

1. A concise climate definition, for example [Wikipedia oceanic climate](https://en.wikipedia.org/wiki/Oceanic_climate).
2. Official normals from national meteorological agencies.
3. Reputable secondary sources only if official normals are unavailable.

Example: for temperate oceanic calibration, London-area Met Office normals are a better tuning target than a generic summary article.

Prefer sources such as:

- Met Office
- NOAA
- BOM
- MeteoFrance
- DWD
- other national weather agencies

Extract concrete benchmarks:

- seasonal mean temperature range
- wet-day or rainy-period frequency
- snow frequency, if any
- typical windiness
- rarity of severe wind events
- strength of seasonal contrast

Do not calibrate from travel sites, blogs, or a single informal source.

## Tuning Method

Work in this order:

1. Tune the weather-state shape.
2. Tune persistence and change rate.
3. Tune temperatures last.

Lessons from the temperate oceanic pass:

- Inspect "getting worse" and "getting better" transitions separately. They are often not symmetric.
- Make sure overcast can genuinely clear. A bad clear path can trap the model in wet states.
- Wind escalation weights are highly sensitive. Overdo them and the whole climate becomes unrealistically windy.
- Light precipitation should be able to persist for a while, but heavy precipitation should stay uncommon unless the target climate truly demands it.
- Cloud build and cloud clearing usually need different weights.
- Stability should allow believable runs of similar weather without making the model static.

Lessons from the derived-template passes:

- If a climate is wrong in one season but broadly right elsewhere, question the seasonal target state before endlessly shaving multipliers.
- Mediterranean winter should rest on `Humid`, not `LightRain`; making actual rain the target state caused unrealistic wet persistence.
- Use fallback adjusters for residual shape changes, but fix the main recognized transition family first when possible.
- Tune autumn onset and winter persistence separately for dry-summer climates.

Lessons from the runtime simplification pass:

- Removing temperature-variant events changes long-run wet/dry occupancy because the graph now has fewer distinct states. Expect to recalibrate analyzer bounds after the simplification.
- Keep temperature fluctuation logic in `RegionalClimate` and `WeatherController`, not in extra weather-event rows.
- If a climate only differs by hemisphere, prefer the controller flag over separate regional-climate records.

Temperature guidance:

- Start from real-world seasonal expectations for a benchmark city or region.
- Set the seasonal/hourly baseline first, then use regional fluctuation settings for slow day-to-day drift and weather-event temperature effects for weather-driven deviations.
- Keep snow rare unless the target climate genuinely supports regular snowfall.
- Check both sheltered temperatures and outdoor-feel impacts from wind and rain.

Aim for results within roughly 10% of the benchmark where the simulation exposes a comparable measure, then apply a suspension-of-disbelief check.

## Testing

Do not stop at code inspection. Add or update analyzer-backed tests in:

- `C:/Users/Luke/source/repos/FutureMUD/MudSharpCore Unit Tests/`

Use `WeatherSeederOceanicClimateTests.cs` as the pattern for a climate-specific regression test if a similar example already exists in the repo.

Preferred pattern:

1. Seed `WeatherSeeder` into an EF Core in-memory context.
2. Build the relevant `WeatherTransitionSnapshot` data from the seeded records.
3. Run `WeatherStatisticsAnalyzer.AnalyzeSimulation`.
4. Assert broad climate bands rather than exact values.

Current test structure to preserve:

- Keep the shared regression class in `WeatherSeederOceanicClimateTests.cs`, now named `WeatherSeederClimateTests`.
- Reuse `AnalyzeSeededNorthernHemisphereClimate(...)` when the climate is a northern-hemisphere template with the standard seeded seasons.
- Keep explicit regression coverage for `WeatherClimateUtilities` and the analyzer's opposite-hemisphere / temperature-fluctuation path when those runtime rules change.
- Add one climate-specific regression per template with broad, benchmark-driven assertions rather than creating a new bespoke harness each time.

Check at least:

- annual wet occupancy
- seasonal wet occupancy where relevant
- snow occupancy
- wind distribution, especially severe-event rarity
- seasonal mean temperatures

Use wide but meaningful assertions so the test protects behavior without becoming brittle.

If you touch the skill itself or other Codex skill metadata, run the standard skill validation scripts with `PyYAML` available rather than doing a manual frontmatter check.

Iterate with:

```powershell
dotnet test "C:\Users\Luke\source\repos\FutureMUD\MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj" --filter "WeatherStatisticsAnalyzerTests|ImplementorModuleWeatherStatsCsvTests|WeatherSeeder"
```

If you add a climate-specific regression test, include its class name in the filter while tuning.

## CSV Checks

If you need a human-readable sanity check, use the Weather Analysis implementor command to export CSVs and inspect:

- precipitation frequency and persistence
- wind distribution
- seasonal temperatures
- event mix over the year

Load the CSVs with `pandas` first. Use quick groupings, monthly summaries, and descriptive statistics before inspecting raw rows.

If the aggregate numbers look suspicious or hard to compare, use `matplotlib` to produce simple line charts or histograms for:

- monthly or seasonal temperature means
- precipitation-event frequency
- wind-speed or wind-category distribution

Use CSVs to spot shape problems, then tighten the analyzer test so the fix stays covered.

## Finish Criteria

When finishing the task:

- cite the sources used
- state the benchmark you inferred from them
- report the analyzer or CSV outcome after tuning
- call out any remaining mismatch or uncertainty
- mention the exact tests you ran

If a relevant design document exists under `Design Documents/`, update it in the same task.
