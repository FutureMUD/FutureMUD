# Climate calibration and tuning notes

Read the section relevant to a calibration or graph problem. These are FutureMUD-specific lessons, not a requirement to retune every climate during unrelated runtime work. Paths are repository-relative.

## Profile and runtime model

`WeatherSeeder.SeedData` uses `GetClimateProfiles()` to seed a shared northern-hemisphere baseline regional climate for each profile. `WeatherSeederClimateProfile` carries the climate model name, regional prefix, seasonal temperature ranges, transition delegates, stability, Köppen/reference metadata and description inputs.

`DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.ClimateTemplates.cs` contains the pattern, including `CreateTemperateOceanicProfile()`, `CreateHumidSubtropicalProfile()` and `CreateMediterraneanProfile()`. Verify the current catalogue in source rather than treating these examples as an exhaustive inventory.

The oceanic `Temperate` model is the reference graph. Use `CreateDerivedClimateModel()` plus climate-specific transition/fallback adjusters for profiles that share its event structure. A genuinely different graph needs an explicit reason; copying the whole graph for small parameter differences creates maintenance drift.

Use one canonical weather-event catalogue. `RegionalClimate.TemperatureFluctuationStandardDeviation` and `TemperatureFluctuationPeriod` describe slow multi-day temperature drift. Do not reintroduce hot/cold event variants for this purpose. Collapsing variant states changes graph occupancy: transfer probability to equivalent canonical events and reevaluate affected statistical expectations rather than merely deleting weights.

Seasonal/hourly base temperature comes from the regional climate. Sheltered temperature combines that baseline, regional fluctuation, and current-event temperature effects; outdoor conditions also reflect wind and precipitation. Test the distinction when changing these paths.

Use the controller's `OppositeHemisphere` half-celestial-year shift rather than duplicate southern regional-climate rows. Retain focused tests for `WeatherClimateUtilities` and the analyzer/controller hemisphere and fluctuation paths when changing those rules.

## Evidence and comparable metrics

For a new calibration, choose a representative place and official normals from agencies such as BOM, Met Office, NOAA, Météo-France or DWD. Record the station/region and reference period so later tuning can reproduce the comparison. A classification page can establish the climate family; travel sites and anecdotal summaries should not establish its numerical calibration.

Extract the observations that the simulation can reasonably represent: seasonal temperature levels/ranges, wet-day or rainy-period frequency, snow frequency, wind distribution, severe-wind rarity, and seasonal contrast.

Do not equate unlike quantities:

- Event/time occupancy measures the share of simulated time in a state. Wet days count days meeting a stated observational threshold. Precipitation amount requires a rate/depth model and matching aggregation.
- Use absolute temperature tolerances or defensible seasonal bands, not relative percentage errors around zero Celsius.
- For very rare snow or severe wind, use meaningful probability/count bounds and adequate simulation duration rather than a percentage error on a near-zero target.
- Where only a proxy exists, name the proxy and the limitation; do not report a precise observational match.

Choose tolerances from metric comparability, stochastic variation and the intended climate family. Broad bands should still detect meaningful regressions. Do not loosen them just to hide a broken transition graph.

## Tuning sequence and diagnostic lessons

For a new or substantially recalibrated profile, tune state shape, then persistence/change rate, then temperature. A narrow isolated defect does not require repeating all three stages.

State shape and persistence:

- “Getting worse” and “getting better” transitions are not necessarily symmetric. Inspect them separately.
- Ensure overcast has a genuine clearing path; a defective path can trap the graph in wet states.
- Wind-escalation weights are sensitive and can make the long-run model implausibly windy.
- Light precipitation may persist while heavy precipitation should remain uncommon unless the climate specifically warrants it.
- Cloud build and clearing can need different weights. Stability should allow runs of similar weather without freezing the model.
- Fix the primary recognised transition family before using fallback adjusters for residual effects.

Derived-profile lessons:

- When one season is wrong, reconsider its target state before endlessly shaving multipliers across the entire model.
- The Mediterranean winter target should rest on `Humid`, not actual `LightRain`; the latter caused excessive wet persistence in prior tuning.
- Dry-summer climates may need separate autumn-onset and winter-persistence adjustments.
- Removing temperature-variant events changes long-run wet/dry occupancy because fewer states remain; inspect and recalibrate the affected behaviour rather than assuming the prior bounds still measure the same graph.

Temperature:

- Establish realistic seasonal/hourly baselines first.
- Use regional fluctuations for slow drift and event effects for weather-driven deviations.
- Preserve snow rarity unless the target supports frequent snow.
- Inspect sheltered temperatures separately from wind/rain outdoor-feel effects.

## Regression and CSV diagnosis

Use the existing `WeatherSeederClimateTests` in `MudSharpCore Climate Tests/WeatherSeederOceanicClimateTests.cs` and, when appropriate, `AnalyzeSeededNorthernHemisphereClimate(...)`. The existing pattern seeds WeatherSeeder with an EF in-memory context, builds `WeatherTransitionSnapshot` data, and runs `WeatherStatisticsAnalyzer.AnalyzeSimulation`.

Keep simulations deterministic and assertions benchmark-driven. Check relevant seasonal as well as annual behaviour; plausible annual averages can conceal a broken winter or autumn transition. Add one climate-specific regression per template within the existing harness where that remains suitable.

CSV exports from the Weather Analysis implementor command can reveal precipitation persistence, event mix, seasonal temperatures and wind tails. Start with aggregate/monthly/seasonal summaries; inspect raw rows only where necessary. Use available CSV tooling, and add a line chart or histogram when it resolves an ambiguity. Translate a confirmed shape defect into a regression so the diagnostic finding remains protected.
