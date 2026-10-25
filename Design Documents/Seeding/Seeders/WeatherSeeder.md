# WeatherSeeder

## Start here to add content

Edit [WeatherSeeder.ClimateProfiles.cs](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.ClimateProfiles.cs): **GetClimateProfiles for a new stock climate; CreateWeatherEvent in WeatherSeeder.cs owns event derivation**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up Weather and Seasons.
Baseline availability: **Enabled**. The entrypoint is [WeatherSeeder.cs](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `operation`, `weathercelestial`, `rain`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.
- The Celestial seeder must have installed at least one celestial object.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

All catalogue definitions are authored C#. [WeatherSeeder.cs](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.cs) owns event derivation, seasons and regional persistence; [Definitions](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.Definitions.cs) owns descriptor/enumeration/profile types; [ClimateProfiles](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.ClimateProfiles.cs) owns named climate probability/temperature data; [ClimateModels](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.ClimateModels.cs) builds transitions; [Hazards](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.Hazards.cs) owns the separate update operation. There is no external catalogue or generated input asset.

**Representative existing entry.** `temperate_mid_winter` is an actual season: `SeedData` resolves a celestial → `CreateSeasons` → local `AddSeason("temperate_mid_winter", "Winter", "Mid Winter", 0)` → name-based `Season` creation/reuse → `context.Seasons.Add` / scalar assignments → `SaveChanges`. Profiles subsequently consume the seasons in `CreateClimateModel` and `CreateRegionalClimate`; those helpers create/update climate model XML and regional climate/season relationship rows. A new season must be usable by the climate-profile season-group maps, not merely appear in `Seasons`.

**Worked addition.** To add an additional stock season boundary, add a distinct call such as `AddSeason("temperate_example_thaw", "Spring", "Example Thaw", 75)` alongside the existing season calls, after choosing an appropriate orbital-day boundary for the selected celestial model. Its key is stable; display names/group/day are intentional content. Update stock-season presence inventory, profile seasonal temperature ranges and transition mappings so that `CreateClimateModel` and `CreateRegionalClimate` can resolve it. Review all profile consumers of the group label and any northern/southern projection behavior. The example day is illustrative, not a researched climate calibration. No new menu registration is needed for a stock addition used by all profiles.

**Materially different path: generated weather events.** `CreateWeatherEvents` loops every `PrecipitationLevel` and `WindLevel`, calls `CreateWeatherEvent`, then `AddEventWithTempVariations` / local `AddEvent`. Rain with still wind is derived from `PrecipitationLevel.Rain` and `WindLevel.Still`; the event name comes from `DescribeEnum()` and a temperature suffix, rather than an independently authored row named in Definitions.cs. Local AddEvent ensures `WeatherEvents`, assigns narrative, temperatures, precipitation/wind, light, sky/time flags, rain/simple runtime type, counts-as and descriptor indices. Events are saved before references/echo definitions and climate transitions consume generated IDs. To add an event variant, extend the actual combination/variation generation and descriptor coverage in WeatherSeeder.cs plus the corresponding enums/tier policy and transition builder. A standalone descriptor object or one AddEvent call without model transitions is incomplete. Runtime precipitation/wind enum changes cross shared/runtime contracts and require their owning implementation work.

Operator `operation` selects install or the separate hazard update; `rain` selects full/soak/none. Full enables rain events and puddles; soak enables rain events and disables puddles; none uses simple events and does not change the existing puddle setting. The hazard updater targets recognized stock models and does not retarget controllers.

**Fresh installation timing.** A single eligible daylight celestial is selected automatically. With multiple eligible bodies, `weathercelestial` selects an ID and suggests the lowest eligible ID. Physical `Sun` and authored `RailSun`/`ScriptedSun`/`RailMoon`/`ScriptedMoon` definitions explicitly governing time of day supply their native celestial year and feed clock; unsupported indirect views, missing clocks, nonpositive dimensions and unrepresentable season layouts are rejected before writes. Authored sources must refer to a calendar using that clock. No terrestrial fallback is used for real installations.

The shared internal [SeederWeatherTiming](../../../DatabaseSeeder/Seeders/Utilities/SeederWeatherTiming.cs) retains the 24-point daily curve as a periodic day-fraction function. An H-hour clock gets exactly H values sampled by cyclic linear interpolation at `24 * hour / H`; a 24-hour clock keeps identical values. Each conventional day onset is scaled by `round(day * celestialYearDays / 365, AwayFromZero)`. All twelve integer onsets must remain distinct and inside the year, including fractional years. Multi-day fluctuation periods use native minutes per day; processing and echo intervals explicitly authored in minutes keep those units. Conventional temperature bands, transition weights and terrestrial benchmark targets remain unchanged.

**Failure/rerun boundary.** Weather installation is fresh-only and refuses an existing weather catalogue. Timing validation precedes all writes; subsequent installation still uses staged saves. Existing installations receive no timing conversion, and there is no timing-repair operation. The separate hazards update uses an explicitly canonical 24-hour/60-minute/365-day disposable reference to verify graphs; it does not change season onsets, regional hourly tables or fluctuation periods. Custom graphs retain the existing whole-graph protection.

**Setting weather.** The optional [Armageddon weather package](ArmageddonSettingSeeder.md#regional-weather) reuses this canonical catalogue, owned dust variants and transition row writer. It supplies its own nine-season heating and regional wind/dust weights, independently of conventional climate families. `rain=none` disables physical soaking/puddles, not precipitation occurrence.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

The explicit hazards update adds lightning and dust to verified stock climates and preserves builder customisations.

Legacy definitions are adopted only after matching canonical stock data; managed climates with builder edits are reported and preserved. Controllers are never retargeted.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

Timing, fresh-only behavior, preserving hazard updates and question inventories have automated regression coverage. All 50 climate tests passed, including unchanged terrestrial benchmarks and 12 Armageddon profile/seed runs. Native MySQL qualification covered installation, fresh-context reloads, preserving reruns and a concrete nine-hour controller across hour/year boundaries. Full/focused seeder evidence, build results and limitations are recorded in the [weather verification report](../../World/Weather_Verification.md#non-terrestrial-timing-and-armageddon-weather--25-october-2026).

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
