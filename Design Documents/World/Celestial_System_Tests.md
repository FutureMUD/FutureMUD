# FutureMUD Celestial System Tests

## Purpose
Celestial changes are risky in two different ways:

- runtime math can drift or break observer-frame transforms
- seeded stock packages can become inconsistent with the runtime

The automated suite therefore keeps runtime regressions and seeder regressions separate.

## Coverage Split

| Test project | Scope |
| --- | --- |
| `MudSharpCore Unit Tests` | Physical and authored numeric regressions, nonstandard clocks, event/public API contracts, zone/lifecycle/perception integration, resource guards and performance counters |
| `DatabaseSeeder Unit Tests` | Stock package creation, linkage, dependency rules, seeded constants, authored rerun preservation and Debug replay contracts |

## Runtime Coverage
Authored coverage is in `AuthoredCelestialNumericalTests`, `AuthoredCelestialIntegrationTests`, `AuthoredCelestialValidationTests`, `AuthoredCelestialWorldTests`, `AuthoredCelestialFutureProgTests` and `AuthoredCelestialPerformanceTests`. The companion fixture JSON is copied to the test output; all fourteen authored fixtures use their supplied independent expectations. Additional tests cover randomized geometry and sampled-event oracles, source failures and limits, real zone authority/light/lifecycle, timezone projection, echoes, atomic edits and calendar boundaries, every registered event overload, zero-allocation numerical paths and 1/100/1,000-subscriber fanout.

`AuthoredCelestialSeederTests` checks six example records, compiling their source and preserving edits/IDs while restoring a missing member. The executable native harness is `scripts/AuthoredCelestialSmokeWorld`; use its runbook for a disposable MySQL replay, nonblank refusal, in-game commands and persistence evidence. No existing physical numerical golden values are changed.

The modern runtime coverage lives in four focused suites:

- `CelestialTests.cs`
- `PlanetaryMoonTests.cs`
- `PlanetFromMoonTests.cs`
- `SunFromPlanetaryMoonTests.cs`

The intent is no longer "basic smoke coverage". Each supported modern model has deterministic numeric regression rows with at least three fixed data points. Those rows assert concrete outputs such as:

- current day number
- arbitrary-instant ephemeris day numbers
- right ascension and declination when available or practically accessible
- elevation
- azimuth
- illumination
- phase classification
- eclipse outcome where relevant
- time-of-day classification
- movement direction

The linked models also assert relationships back to their source objects, such as:

- `PlanetFromMoon` remaining opposite the linked moon in equatorial coordinates
- `PlanetFromMoon` illumination remaining complementary to the linked moon
- `SunFromPlanetaryMoon` remaining close to, but not identical to, the root solar direction near conjunction

`CelestialTests.cs` also covers the astronomical event solver. The current solver coverage checks nth-next sunrise advancement plus supported solar longitude, new moon, full moon, and deterministic visible-crescent event searches.

## Direction Sampling Coverage
All four runtime suites retain explicit non-24x60 clock coverage for movement direction sampling.

That matters because the direction logic compares the current sky position with the sky position one in-game minute earlier. The implementation must use the actual clock dimensions, not a hidden Earth-like `1 / 1440` assumption.

## Seeder Coverage
`DatabaseSeeder Unit Tests/CelestialSeederTests.cs` verifies:

- Earth sun package creation
- Earth moon-view package creation and linkage
- gas giant moon-view package creation and linkage
- dependency failure when the Earth moon package has no matching root sun
- rerun detection through `ShouldSeedData`
- seeded orbital and observer-frame constants for the stock packages

The seeded-constant assertions now cover the corrected modern data, including:

- solar eccentricity, semi-major axis, and apparent angular radius
- moon semi-major axis
- moon sidereal epoch and sidereal rate
- `PlanetFromMoon` stored `SunAngularRadius`

## Removed Legacy Coverage
There is no longer a supported `OldSun` runtime path, so there is no legacy compatibility suite for that type.

If an existing world still contains persisted `OldSun` data, load now fails explicitly and the content must be migrated to the modern `Sun` model.

## When to Add More Coverage
Add or expand numeric regression coverage whenever a change touches:

- orbital equations
- true-anomaly approximations
- angle normalization
- local sidereal time handling
- observer-frame transforms
- eclipse thresholds
- illumination formulas
- day-number or fractional-day handling
- arbitrary-instant ephemeris APIs
- astronomical event search or nth-next behavior

Seeder tests should be updated whenever a change touches:

- stock constants
- XML authoring shape
- linked IDs
- package detection
- package dependency rules
